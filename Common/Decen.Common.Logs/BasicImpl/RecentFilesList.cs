using Decen.Common.Logs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Decen.Common.Logs.BasicImpl
{
    public class RecentFilesList
    {
        IEnumerable<string> names;
        private string name = null;
        public RecentFilesList(List<string> filenameoptions)
        {
            names = filenameoptions;
        }
        static string[] readLinesSafe(string name)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    return File.ReadAllLines(name);
                }
                catch (Exception) when (i < 4)
                {
                    Thread.Sleep(100);
                }
            }

            return null;
        }

        static void setHidden(string name)
        {
            if (OperatingSystem.Current == OperatingSystem.Windows)
                File.SetAttributes(name, FileAttributes.Hidden | FileAttributes.Archive);
        }

        string[] ensureFileExistsAndReadLines()
        {
            var _names = names;
            if (name != null)
                _names = new[] { name }; // a name was already decided.
            foreach (var name in _names)
            {
                try
                {
                    if (!File.Exists(name))
                    {
                        File.Create(name).Close();

                        try
                        {
                            setHidden(name);
                        }
                        catch
                        {
                            // The file could not be made hidden. This is probably ok.
                        }
                        this.name = name;
                        return Array.Empty<string>();
                    }
                    if (readLinesSafe(name) is string[] str)
                    {
                        this.name = name;
                        return str;
                    }
                }
                catch
                {
                    // ignore exceptions throws. Try a different file.   
                }
            }
            return Array.Empty<string>();
        }


        static Mutex recentLock = new Mutex(false, "opentap_recent_logs_mutex");

        public string[] GetRecent()
        {
            recentLock.WaitOne();
            try
            {
                return ensureFileExistsAndReadLines();
            }
            finally
            {
                recentLock.ReleaseMutex();
            }
        }

        public void AddRecent(string newname)
        {
            recentLock.WaitOne();
            try
            {
                var currentFiles = ensureFileExistsAndReadLines().Append(newname).DistinctLast();
                currentFiles.RemoveIf(x => File.Exists(x) == false);
                using (var f = File.Open(name, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    f.SetLength(0);
                    using (var sw = new StreamWriter(f))
                    {
                        currentFiles.ForEach(sw.WriteLine);
                    }
                }
            }
            finally
            {
                recentLock.ReleaseMutex();
            }
        }
    }
}
