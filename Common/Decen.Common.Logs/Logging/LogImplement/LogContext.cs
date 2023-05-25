using Decen.Common.Logs.BasicImpl;
using Decen.Common.Logs.Logging.LogInterface;
using Decen.Common.Logs.Structs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Decen.Common.Logs.Logging.LogImplement
{
    public class LogContext : ILogContext, ILogContext2, IDisposable
    {
        readonly LogQueue LogQueue = new LogQueue();

        public bool HasListeners => listeners.Count > 0;

        public readonly List<ILogListener> listeners = new List<ILogListener>();

        public long processedMessages;
        public long OutstandingMessages => LogQueue.PostedMessages - processedMessages;

        public ILogTimestampProvider timeStamper = new AccurateStamper();

        //AutoResetEvent 可以用来在不同线程之间发信号，通常用于解决线程等待
        public readonly AutoResetEvent flushBarrier = new AutoResetEvent(false);
        public readonly AutoResetEvent newEventOccured = new AutoResetEvent(false);

        public readonly Thread processor;
        public LogContext(bool startProcessor = true)
        {
            if (startProcessor)
            {
                processor = new Thread(ProcessLog) { IsBackground = true, Name = "Log processing" };
                processor.Start();
            }
        }

        static LogContext EmptyLogContext()
        {
            return new LogContext(false);
        }

        public void ProcessLog()
        {
            var copy = new List<ILogListener>();
            Event[] bunch = new Event[0];
            while (isDisposed == false)
            {
                newEventOccured.WaitOne();
                flushBarrier.WaitOne(100);
                int count = LogQueue.DequeueBunch(ref bunch);
                if (count > 0)
                {
                    lock (listeners)
                    {
                        copy.Clear();
                        foreach (var thing in listeners)
                            copy.Add(thing);
                    }
                    if (copy.Count > 0)
                    {
                        if (timeStamper != null)
                            for (int i = 0; i < bunch.Length; i++)
                                bunch[i].Timestamp = timeStamper.ConvertToTicks(bunch[i].Timestamp);
                        foreach (var listener in copy)
                        {
                            try
                            {
                                using (var events = new EventCollection(bunch))
                                {
                                    listener.EventsLogged(events);
                                }
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }
                    processedMessages += bunch.LongLength;
                }
            }
        }

        public ILog CreateLog(string source)
        {
            return new Log(this, source);
        }

        public void RemoveLog(ILog logSource)
        {
            if (logSource is Log log)
                log.Context = EmptyLogContext();
        }

        public void AttachListener(ILogListener listener)
        {
            lock (listeners)
                listeners.Add(listener);
        }

        public void DetachListener(ILogListener listener)
        {
            lock (listeners)
                listeners.Remove(listener);
        }

        public ReadOnlyCollection<ILogListener> GetListeners()
        {
            return new ReadOnlyCollection<ILogListener>(listeners);
        }

        public bool Flush(int timeoutMs = 0)
        {
            if (isDisposed) return true;
            long posted = LogQueue.PostedMessages;

            flushBarrier.Set();
            newEventOccured.Set();

            if (timeoutMs == 0)
            {
                while (((processedMessages - posted)) < 0)
                {
                    Thread.Yield();
                    newEventOccured.Set();
                    flushBarrier.Set();
                }
                return true;
            }

            {
                var sw = Stopwatch.StartNew();
                while ((processedMessages - posted) < 0 && sw.ElapsedMilliseconds < timeoutMs)
                {
                    Thread.Yield();
                    newEventOccured.Set();
                    flushBarrier.Set();
                }
                return (processedMessages - posted) < 0;
            }
        }

        public bool Flush(TimeSpan timeout)
        {
            return Flush((int)timeout.TotalMilliseconds);
        }

        bool isDisposed;
        public void Dispose()
        {
            Flush();
            isDisposed = true;
            newEventOccured.Set();
            flushBarrier.Set();
        }

        public bool Async { get; set; }

        public int MessageBufferSize { get; set; }

        public ILogTimestampProvider Timestamper { get { return timeStamper; } set { timeStamper = value; } }


        void injectEvent(Event @event)
        {
            lock (listeners)
            {
                using (EventCollection eventCollection = new EventCollection(new[] { @event }))
                {
                    listeners.ForEach(l => l.EventsLogged(eventCollection));
                }
            }
        }

        public void AddEvent(Event evt)
        {
            if (Async)
            {
                int msgCnt = MessageBufferSize;
                if (msgCnt > 0)
                {
                    while (OutstandingMessages > msgCnt)
                        Thread.Sleep(1);
                }
                LogQueue.Enqueue(evt);
            }
            else
            {
                injectEvent(evt);
            }
            this.newEventOccured.Set();
        }

        internal class Log : LogInjector, ILog
        {
            private readonly string _source;

            public Log(LogContext context, string source) : base(context)
            {
                _source = source;
            }

            public void LogEvent(int eventType, string message)
            {
                LogEvent(_source, eventType, message);
            }

            public void LogEvent(int eventType, string message, params object[] args)
            {
                LogEvent(_source, eventType, message, args);
            }

            public void LogEvent(int eventType, long durationNs, string message)
            {
                LogEvent(_source, eventType, durationNs, message);
            }

            public void LogEvent(int eventType, long durationNs, string message, params object[] args)
            {
                LogEvent(_source, eventType, durationNs, message, args);
            }

            string ILog.Source => _source;
        }

        public long GetProcessedMessages() => processedMessages;
    }
}
