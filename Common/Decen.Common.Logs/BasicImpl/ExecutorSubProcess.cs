using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Decen.Common.Logs.BasicImpl
{
    public class ExecutorSubProcess : IDisposable
    {
        public class EnvVarNames
        {
            public static string TpmInteropPipeName = "TPM_PIPE";
            public static string ParentProcessExeDir = "TPM_PARENTPROCESSDIR";
            public static string OpenTapInitDirectory = "OPENTAP_INIT_DIRECTORY";
        }

        NamedPipeServerStream Pipe { get; set; }
        Process Process { get; set; }

        ProcessStartInfo start;

        public int WaitForExit()
        {
            if (Process != null)
            {
                Process.WaitForExit();
                return Process.ExitCode;
            }
            else
                throw new InvalidOperationException("Process has not been started");
        }

        public ExecutorSubProcess(ProcessStartInfo start)
        {
            this.start = start;
        }

        void pipeConnected()
        {

            byte[] buffer = new byte[1024];

            void readMessage()
            {
                Pipe.ReadAsync(buffer, 0, buffer.Length, tokenSource.Token)
                    .ContinueWith(tsk =>
                    {
                        if ((tsk.IsCanceled || tsk.IsFaulted) == false)
                            gotMessage(tsk.Result);
                    });
            }

            void gotMessage(int cnt)
            {
                if (tokenSource.IsCancellationRequested)
                    return;
                var str = Encoding.UTF8.GetString(buffer, 0, cnt);
                pushMessage(str);
                Array.Clear(buffer, 0, cnt);
                readMessage();
            }
            readMessage();
        }

        public event EventHandler<string> MessageReceived;

        void pushMessage(string msg)
        {
            if (MessageReceived != null)
                MessageReceived(this, msg);
        }

        CancellationTokenSource tokenSource = new CancellationTokenSource();

        public static ExecutorSubProcess Create(string name, string args, bool isolated = false)
        {
            var start = new ProcessStartInfo(name, args)
            {
                WorkingDirectory = Path.GetFullPath(Directory.GetCurrentDirectory()),
                UseShellExecute = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            if (isolated)
            {
                start.Environment[EnvVarNames.ParentProcessExeDir] = ExecutorClient.ExeDir;
            }
            return new ExecutorSubProcess(start);
        }

        public static NamedPipeServerStream getStream(out string pipeName)
        {
        Start:
            string pipename = pipeName = Guid.NewGuid().ToString().Replace("-", "");
            try
            {
                return new NamedPipeServerStream(pipename, PipeDirection.In, 10);
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(100);
                goto Start;
            }
        }

        public void Start()
        {
            string pipeName;
            Pipe = getStream(out pipeName);
            start.Environment[EnvVarNames.TpmInteropPipeName] = pipeName;
            Pipe.WaitForConnectionAsync().ContinueWith(_ => pipeConnected());
            Process = Process.Start(start);
            Process.EnableRaisingEvents = true;

            Task t1 = RedirectOutput(Process.StandardOutput, Console.Write);
            Task t2 = RedirectOutput(Process.StandardError, Console.Error.Write);
        }

        async Task RedirectOutput(StreamReader reader, Action<string> callback)
        {
            char[] buffer = new char[256];
            int count;

            while ((count = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                callback(new string(buffer, 0, count));
            }
        }

        public void Dispose()
        {
            tokenSource.Cancel();
            if (Pipe != null)
                Pipe.Dispose();
            if (Process != null)
                Process.Dispose();
        }

        public void Env(string Name, string Value)
        {
            start.Environment[Name] = Value;
        }
    }
}
