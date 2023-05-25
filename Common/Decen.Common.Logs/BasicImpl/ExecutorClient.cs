using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Decen.Common.Logs.BasicImpl
{
    public class ExecutorClient : IDisposable
    {
        /// <summary>
        /// Is this process an isolated sub process of tap.exe
        /// </summary>
        public static bool IsRunningIsolated => Environment.GetEnvironmentVariable(ExecutorSubProcess.EnvVarNames.ParentProcessExeDir) != null;
        /// <summary>
        /// Is this process a sub process of tap.exe
        /// </summary>
        public static bool IsExecutorMode => Environment.GetEnvironmentVariable(ExecutorSubProcess.EnvVarNames.TpmInteropPipeName) != null;

        public static string ExeDir
        {
            get
            {
                if (IsRunningIsolated)
                    return Environment.GetEnvironmentVariable(ExecutorSubProcess.EnvVarNames.ParentProcessExeDir);
                else
                {
                    var exePath = Environment.GetEnvironmentVariable(ExecutorSubProcess.EnvVarNames.OpenTapInitDirectory);
                    if (exePath != null)
                        return exePath;
                    return GetOpenTapDllLocation();
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        //static string GetOpenTapDllLocation() => Path.GetDirectoryName(typeof(PluginSearcher).Assembly.Location);
        static string GetOpenTapDllLocation() => Path.GetDirectoryName(typeof(Nullable).Assembly.Location);

        Task pipeConnect;
        PipeStream pipe;

        public ExecutorClient()
        {
            var pipename = Environment.GetEnvironmentVariable(ExecutorSubProcess.EnvVarNames.TpmInteropPipeName);
            if (pipename != null)
            {
                var pipe2 = new NamedPipeClientStream(".", pipename, PipeDirection.Out, PipeOptions.WriteThrough);
                pipeConnect = pipe2.ConnectAsync();
                pipe = pipe2;
            }
        }

        public void Dispose()
        {
            pipe.Dispose();
        }

        internal void MessageServer(string newname)
        {
            if (!pipeConnect.Wait(TimeSpan.FromSeconds(10)))
            {
                throw new TimeoutException("Isolated process failed to connect to host process within reasonable time.");
            }
            var toWrite = Encoding.UTF8.GetBytes(newname);
            pipe.Write(toWrite, 0, toWrite.Length);
        }
    }
}
