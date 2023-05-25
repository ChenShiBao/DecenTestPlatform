using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Decen.Common.Logs.Logging.LogInterface
{
    [ComVisible(false)]
    public interface ILog
    {

        /// <summary>
        /// 记录一个事件。
        /// </summary>
        void LogEvent(int EventType, string Message);

        void LogEvent(int EventType, string Message, params object[] Args);

        void LogEvent(int EventType, long DurationNS, string Message);

        void LogEvent(int EventType, long DurationNS, string Message, params object[] Args);

        string Source { get; }
    }
}
