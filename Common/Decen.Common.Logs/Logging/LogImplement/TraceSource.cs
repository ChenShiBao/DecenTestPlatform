using Decen.Common.Logs.Enums;
using Decen.Common.Logs.Logging.LogInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.Logging.LogImplement
{
    public class TraceSource
    {
        internal ILog log;

        internal object Owner;
        public TraceSource(ILog logSource)
        {
            log = logSource;
        }

        LogInjector redirectedLog => Log.RedirectedLog;

        public void Flush()
        {
            Log.Flush();
        }


        public void TraceEvent(LogEventType te, int id, string message)
        {

            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (redirectedLog != null)
                redirectedLog.LogEvent(log.Source, (int)te, message);
            else
                log.LogEvent((int)te, message);
        }

        public void TraceEvent(long durationNs, LogEventType te, int id, string message, params object[] args)
        {
            if (redirectedLog != null)
                redirectedLog.LogEvent(log.Source, (int)te, durationNs, message, args);
            else
                log.LogEvent((int)te, durationNs, message, args);
        }

        public void TraceEvent(long durationNs, LogEventType te, int id, string message)
        {
            if (redirectedLog != null)
                redirectedLog.LogEvent(log.Source, (int)te, durationNs, message);
            else
                log.LogEvent((int)te, durationNs, message);
        }

        public void TraceEvent(LogEventType te, int id, string message, params object[] args)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (redirectedLog != null)
                redirectedLog.LogEvent(log.Source, (int)te, message, args);
            else
                log.LogEvent((int)te, message, args);
        }
    }
}
