using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Decen.Common.Logs.Listeners;
using Decen.Common.Logs.BasicImpl;
using OperatingSystem = Decen.Common.Logs.BasicImpl.OperatingSystem;

namespace Decen.Common.Logs.Logging.LogImplement
{
    public static class SessionLogs
    {
        public static bool NoExclusiveWriteLock = false; //无独占写入锁定
        public static string currentLogFile;
        public static Task SystemInfoTask; //系统任务服务
        private static FileTraceListener traceListener;
        private static readonly TraceSource log = Log.CreateSource("Session");
        private static RecentFilesList recentSystemlogs = new RecentFilesList(getLogRecentFilesName());
        ///一次保存的文件数
        private const int maxNumberOfTraceFiles = 20;

        ///跟踪文件的最大允许大小
        private const long maxTotalSizeOfTraceFiles = 2_000_000_000L;

        ///如果两个会话需要相同的日志文件名，则会在名称中添加一个整数。
        ///这是我们测试新名称的最大次数。
        const int maxNumberOfConcurrentSessions = maxNumberOfTraceFiles;

        private static int sessionLogCount = 0;
        private static object sessionLogRotateLock = new object();

        private const string RecentFilesName = ".recent_logs";

        [DllImport("libc", EntryPoint = "symlink")]
        public static unsafe extern bool CreateHardLinkLin(char* target, char* linkpath);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "CreateHardlink")]
        public static extern bool CreateHardLinkWin(
          string lpFileName,
          string lpExistingFileName,
          IntPtr lpSecurityAttributes
        );

        /// <summary>
        /// 初始化log日志
        /// </summary>
        public static void Initialize(string logFileName)
        {
            Initialize(logFileName, NoExclusiveWriteLock);
        }

        public static void Initialize(string logFileName, bool noExclusiveWriteLock)
        {
            NoExclusiveWriteLock = noExclusiveWriteLock;
            if (currentLogFile == null)
            {
                Rename(logFileName);
                ////SystemInfoTask = Task.Factory.StartNew(PluginManager.Load).ContinueWith(tsk => SystemInfo());

                AppDomain.CurrentDomain.ProcessExit += FlushOnExit;
                AppDomain.CurrentDomain.UnhandledException += FlushOnExit;
            }
            else
            {
                if (currentLogFile != logFileName)
                    Rename(logFileName);
            }
            currentLogFile = logFileName;
            log.Debug($"Running '{Environment.CommandLine}' in '{Directory.GetCurrentDirectory()}'.");
        }

        private static void FlushOnExit(object sender, EventArgs e)
        {
            try
            {
                SystemInfoTask.Wait();
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException == false)
                    log.Debug("Unexpected error while printing system information.");
            }
            Flush();
        }

        /// <summary>
        /// 重命名以前初始化的临时日志文件。
        /// </summary>
        public static void Rename(string path)
        {
            try
            {
                rename(path);
            }
            catch (UnauthorizedAccessException e)
            {
                log.Warning("Unable to rename log file to {0} as permissions was denied.", path);
                log.Debug(e);
            }
            catch (IOException e)
            {
                log.Warning("Unable to rename log file to {0} as the file could not be created.", path);
                log.Debug(e);
            }
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newFile"></param>
        public static void rename(string path, bool newFile = false)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            List<string> recentFiles = recentSystemlogs.GetRecent().Where(File.Exists).ToList();
            long getTotalSize()
            {
                return recentFiles.Select(x => new FileInfo(x).Length).Sum();
            }

            bool checkCondition()
            {
                bool tooManyFiles = (recentFiles.Count + 1) > maxNumberOfTraceFiles;
                if (tooManyFiles) return true;

                if (recentFiles.Count <= 2) return false; // Do not remove the last couple of log files, event though they might exceed limits.
                var totalSize = getTotalSize();
                bool filesTooBig = totalSize > maxTotalSizeOfTraceFiles;
                if (filesTooBig) return true;
                return false;
            }

            int ridx = 0;
            while (checkCondition() && ridx < recentFiles.Count)
            {
                try
                {
                    if (File.Exists(recentFiles[ridx]))
                        File.Delete(recentFiles[ridx]);
                    recentFiles.RemoveAt(ridx);
                }
                catch (Exception)
                {
                    ridx++;
                }
            }
            string name = Path.GetFileNameWithoutExtension(path);
            string dir = Path.GetDirectoryName(path);
            string ext = Path.GetExtension(path);
            bool fileNameChanged = false;
            for (int idx = 0; idx < maxNumberOfConcurrentSessions; idx++)
            {
                try
                {
                    path = Path.Combine(dir, name + (idx == 0 ? "" : idx.ToString()) + ext);
                    if (traceListener == null || newFile)
                    {
                        if (string.IsNullOrWhiteSpace(dir) == false)
                            Directory.CreateDirectory(Path.GetDirectoryName(path));
                        if (NoExclusiveWriteLock)
                        {
                            var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read | FileShare.Delete);
                            traceListener = new FileTraceListener(stream);
                        }
                        else
                        {
                            traceListener = new FileTraceListener(path);
                        }

                        traceListener.FileSizeLimit = 100000000; // max size for log files is 100MB.
                        traceListener.FileSizeLimitReached += TraceListener_FileSizeLimitReached;
                        Log.AddListener(traceListener);
                    }
                    else
                    {
                        traceListener.ChangeFileName(path, NoExclusiveWriteLock);
                    }
                    fileNameChanged = true;
                    break;
                }
                catch
                {
                    // File was probably locked by another process.
                }
            }
            if (!fileNameChanged)
            {
                log.Debug("Unable to rename log file. Continuing with log '{0}'.", currentLogFile);
            }
            else
            {
                currentLogFile = path;
                log.Debug(sw, "Session log loaded as '{0}'.", currentLogFile);
                recentSystemlogs.AddRecent(Path.GetFullPath(path));
            }

            try
            {
                string latestPath = Path.Combine(dir, "Latest.txt");
                if (File.Exists(latestPath))
                    File.Delete(latestPath);
                CreateHardLink(path, latestPath);
            }
            catch (Exception ex)
            {
                //throw;
            }
        }



        public static unsafe void CreateHardLink(string targetFile, string linkName)
        {
            if (OperatingSystem.Current == OperatingSystem.Windows)
            {
                CreateHardLinkWin(linkName, targetFile, IntPtr.Zero);
            }
            else if (OperatingSystem.Current == OperatingSystem.Linux)
            {
                IntPtr target = Marshal.StringToCoTaskMemAnsi(targetFile);
                IntPtr link = Marshal.StringToCoTaskMemAnsi(linkName);
                CreateHardLinkLin((char*)target, (char*)link);
                Marshal.FreeCoTaskMem(target);
                Marshal.FreeCoTaskMem(link);
            }
            else if (OperatingSystem.Current == OperatingSystem.MacOS)
            {
                Process.Start("ln", $"\"{targetFile}\" \"{linkName}\"");
            }
        }

        public static void Flush()
        {
            if (traceListener != null)
                traceListener.Flush();
        }


        static List<string> getLogRecentFilesName()
        {
            //如果安装文件夹被用户写保护，将尝试寻找其他有效的位置。
            List<string> options = new List<string>();
            void addOption(Func<string> f)
            {
                try
                {
                    string option = f();
                    if (option != null)
                        options.Add(option);
                }
                catch
                {

                }
            }
            if (ExecutorClient.IsRunningIsolated)
            {
                addOption(() => Path.Combine(ExecutorClient.ExeDir, RecentFilesName));
            }
            addOption(() => Path.Combine(Path.GetDirectoryName(typeof(SessionLogs).Assembly.Location), RecentFilesName));
            addOption(() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), RecentFilesName));
            addOption(() => RecentFilesName);
            return options;
        }

        public static void TraceListener_FileSizeLimitReached(object sender, EventArgs e)
        {
            traceListener.FileSizeLimitReached -= TraceListener_FileSizeLimitReached;

            Task.Factory.StartNew(() =>
            {
                lock (sessionLogRotateLock)
                {
                    string newname = currentLogFile.Replace("__" + sessionLogCount.ToString(), "");

                    sessionLogCount += 1;
                    var nextFile = addLogRotateNumber(newname, sessionLogCount);

                    log.Info("Switching log to the file {0}", nextFile);

                    Log.RemoveListener((FileTraceListener)sender);

                    rename(nextFile, newFile: true);
                }
            });
        }

        public static string addLogRotateNumber(string fullname, int cnt)
        {
            if (cnt == 0) return fullname;
            var dir = Path.GetDirectoryName(fullname);
            var filename = Path.GetFileNameWithoutExtension(fullname);
            if (Path.HasExtension(fullname))
            {
                var ext = Path.GetExtension(fullname);
                return Path.Combine(dir, filename + "__" + cnt.ToString() + ext);
            }
            else
            {
                return Path.Combine(dir, filename + "__" + cnt.ToString());
            }
        }

        private static void SystemInfo()
        {
            //if (!String.IsNullOrEmpty(RuntimeInformation.OSDescription))
            //    log.Debug("{0}{1}", RuntimeInformation.OSDescription, RuntimeInformation.OSArchitecture); 

            //if (!String.IsNullOrEmpty(RuntimeInformation.FrameworkDescription))
            //    log.Debug(RuntimeInformation.FrameworkDescription); 
            //var version = SemanticVersion.Parse(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
            //log.Debug("OpenTAP Engine {0} {1}", version, RuntimeInformation.ProcessArchitecture);
        }

    }
}
