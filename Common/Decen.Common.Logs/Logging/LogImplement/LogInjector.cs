using Decen.Common.Logs.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.Logging.LogImplement
{
    public class LogInjector
    {
        internal LogContext Context;
        public LogInjector(LogContext context) => Context = context;

        public void logEvent(Event evt) => Context.AddEvent(evt);

        public void LogEvent(string source, int eventType, string message)
        {
            if (Context.HasListeners)
            {
                long timestamp = getTimestamp();
                var evt = new Event(0, eventType, message, source, timestamp);
                logEvent(evt);
            }
        }

        public void LogEvent(string source, int eventType, string message, params object[] args)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (Context.HasListeners)
            {
                var messageFmt = string.Format(message, args);
                LogEvent(source, eventType, messageFmt);
            }
        }

        long getTimestamp()
        {
            long timestamp = 0;
            var timestamper = Context.Timestamper;
            if (timestamper != null)
                timestamp = timestamper.Timestamp();
            return timestamp;
        }

        public void LogEvent(string source, int eventType, long durationNs, string message)
        {
            if (Context.HasListeners)
            {
                long timestamp = getTimestamp();
                var evt = new Event(durationNs, eventType, message, source, timestamp);
                logEvent(evt);
            }
        }

        public void LogEvent(string source, int eventType, long durationNs, string message, params object[] args)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (Context.HasListeners)
            {
                var messageFmt = string.Format(message, args);
                LogEvent(source, eventType, durationNs, messageFmt);
            }
        }
    }
}
