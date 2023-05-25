using Decen.Common.Logs.Enums;
using Decen.Common.Logs.Logging.LogImplement;
using Decen.Common.Logs.Logging.LogInterface;
using Decen.Common.Logs.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.Listeners
{
    public class TraceListener : ILogListener
    {
        void ILogListener.EventsLogged(IEnumerable<Event> events)
        {
            TraceEvents(events);
        }

        /// <summary>
        /// Receives all log messages. The virtual method simply calls <see cref="TraceEvent(string, LogEventType, int, string)"/> directly.  
        /// </summary>
        public virtual void TraceEvents(IEnumerable<Event> events)
        {
            foreach (var evt in events)
                TraceEvent(evt.Source, (LogEventType)evt.EventType, 0, evt.Message);
        }

        /// <summary>
        /// Empty TraceEvent method.
        /// </summary>
        public virtual void TraceEvent(string source, LogEventType eventType, int id, string format)
        {
        }

        /// <summary>
        /// Empty TraceEvent method.
        /// </summary>
        public virtual void TraceEvent(string source, LogEventType eventType, int id, string format, params object[] args)
        {
            TraceEvent(source, eventType, id, string.Format(format, args));
        }

        /// <summary>
        /// Virtual method to match System.Diagnostics.TraceListener. Might be removed.
        /// </summary>
        public virtual void Write(string str)
        {
        }

        /// <summary>
        /// Virtual method to match System.Diagnostics.TraceListener. Might be removed.
        /// </summary>
        public virtual void WriteLine(string str)
        {
        }

        /// <summary>
        /// Waits until all sent log messages have been processed by this and all other TraceListeners.
        /// </summary>
        public virtual void Flush()
        {
            Log.Flush();
        }
    }
}
