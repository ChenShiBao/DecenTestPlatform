using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Decen.Common.Logs.BasicImpl
{
    public class OperatingSystem
    {
        public static readonly OperatingSystem Windows = new OperatingSystem(nameof(Windows));
        public static readonly OperatingSystem Linux = new OperatingSystem(nameof(Linux));
        public static readonly OperatingSystem MacOS = new OperatingSystem(nameof(MacOS));
        public static readonly OperatingSystem Unsupported = new OperatingSystem(nameof(Unsupported));
        public override string ToString() => Name;
        public string Name { get; }
        OperatingSystem(string name)
        {
            Name = name;
        }

        static OperatingSystem getCurrent()
        {

            if (Path.DirectorySeparatorChar == '\\')
            {
                return OperatingSystem.Windows;
            }
            else
            {
                if (isMacOs())
                {
                    return OperatingSystem.MacOS;
                }
                else if (Directory.Exists("/proc/"))
                {
                    return OperatingSystem.Linux;
                }
            }
            return OperatingSystem.Unsupported;
        }

        static bool isMacOs()
        {
            try
            {
                var startInfo = new ProcessStartInfo("uname");
                startInfo.RedirectStandardOutput = true;
                var process = Process.Start(startInfo);
                process.WaitForExit(1000);
                var uname = process.StandardOutput.ReadToEnd();
                return uname.ToLowerInvariant().Contains("darwin");
            }
            catch
            {
                // ignored
            }

            return false;
        }

        static OperatingSystem current;
        public static OperatingSystem Current
        {
            get
            {
                if (current == null)
                {
                    current = getCurrent();
                }
                return current;
            }
        }
    }
}
