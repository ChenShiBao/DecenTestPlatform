using Decen.Common.Logs.Enums;
using Decen.Common.Logs.Logging.LogInterface;
using Decen.Common.Logs.Structs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Decen.Common.Logs.Logging.LogImplement
{
    public static class Log
    {
        static readonly LogContext rootLogContext = (LogContext)LogFactory.CreateContext();
        static readonly SessionLocal<LogInjector> logField = new SessionLocal<LogInjector>(null);
        static readonly SessionLocal<LogContext> sessionLogContext = new SessionLocal<LogContext>(rootLogContext);
        internal static LogInjector RedirectedLog => logField.Value;
        public static ILogContext Context => sessionLogContext.Value;
        internal static ILogTimestampProvider Timestamper
        {
            get => rootLogContext.Timestamper;
            set => rootLogContext.Timestamper = value;
        }

        internal static void WithNewContext()
        {
            var ctx = new LogContext();

            logField.Value = new LogInjector(ctx);
            sessionLogContext.Value = ctx;
        }

        public static void AddListener(ILogListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));
            var ctx = Context;
            ctx.Flush();
            ctx.AttachListener(listener);
        }

        public static void RemoveListener(ILogListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));
            var ctx = Context;
            listener.Flush();
            ctx.DetachListener(listener);
            listener.Flush();
        }

        public static ReadOnlyCollection<ILogListener> GetListeners()
        {
            return sessionLogContext.Value?.GetListeners();
        }

        public static TraceSource CreateSource(string name)
        {
            return new TraceSource(rootLogContext.CreateLog(name));
        }

        static readonly System.Runtime.CompilerServices.ConditionalWeakTable<object, TraceSource> ownedTraceSources = new System.Runtime.CompilerServices.ConditionalWeakTable<object, TraceSource>();
        static readonly object addlock = new object();

        public static TraceSource CreateSource(string name, object owner)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));
            var source = CreateSource(name);
            source.Owner = owner;
            lock (addlock)
            {
                ownedTraceSources.Remove(owner);
                ownedTraceSources.Add(owner, source); // in this version of .NET there is no Update method...
            }
            return source;
        }


        public static TraceSource GetOwnedSource(object owner)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));
            lock (addlock)
            {
                ownedTraceSources.TryGetValue(owner, out TraceSource source);
                return source;
            }
        }


        public static void RemoveSource(TraceSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            rootLogContext.RemoveLog(source.log);
        }

        static Log()
        {
            Trace.UseGlobalLock = false;
            rootLogContext.Async = true;
            rootLogContext.MessageBufferSize = 8 * 1024 * 1024;
        }

        [ThreadStatic]
        static StringBuilder sb;

        static void traceEvent(this TraceSource trace, TimeSpan elapsed, LogEventType eventType, string message, params object[] args)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            var timespan = ShortTimeSpan.FromSeconds(elapsed.TotalSeconds);

            if (sb == null)
                sb = new StringBuilder();
            sb.Clear();
            if (args.Length == 0)
            {
                sb.Append(message);
            }
            else
            {
                sb.AppendFormat(message, args);
            }
            sb.Append(" [");
            timespan.ToString(sb);
            sb.Append("]");
            long durationNs = elapsed.Ticks * NanoSecondsPerTick;
            trace.TraceEvent(durationNs, eventType, 0, sb.ToString());
        }

        const long NanoSecondsPerTick = 1_000_000_000 / TimeSpan.TicksPerSecond;

        static void traceEvent(this TraceSource trace, LogEventType eventType, string message, params object[] args)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (args.Length == 0)
            {
                trace.TraceEvent(eventType, 0, message);
            }
            else
            {
                trace.TraceEvent(eventType, 0, message, args);
            }
        }

        static void exceptionEvent(this TraceSource trace, Exception exception, LogEventType eventType)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            WriteException(trace, exception, eventType);
        }

        public static void TraceInformation(this TraceSource trace, string message)
        {
            Info(trace, message);
        }

        public static void Debug(this TraceSource trace, string message, params object[] args)
        {
            traceEvent(trace, LogEventType.Debug, message, args);
        }

        public static void Info(this TraceSource trace, Stopwatch timer, string message, params object[] args)
        {
            if (timer == null)
                throw new ArgumentNullException("timer");
            traceEvent(trace, timer.Elapsed, LogEventType.Information, message, args);
        }

        public static void Debug(this TraceSource trace, Stopwatch timer, string message, params object[] args)
        {
            if (timer == null)
                throw new ArgumentNullException("timer");
            traceEvent(trace, timer.Elapsed, LogEventType.Debug, message, args);
        }

        public static void Warning(this TraceSource trace, Stopwatch timer, string message, params object[] args)
        {
            if (timer == null)
                throw new ArgumentNullException("timer");
            traceEvent(trace, timer.Elapsed, LogEventType.Warning, message, args);
        }

        public static void Error(this TraceSource trace, Stopwatch timer, string message, params object[] args)
        {
            if (timer == null)
                throw new ArgumentNullException("timer");
            traceEvent(trace, timer.Elapsed, LogEventType.Error, message, args);
        }

        public static void Info(this TraceSource trace, TimeSpan elapsed, string message, params object[] args)
        {
            traceEvent(trace, elapsed, LogEventType.Information, message, args);
        }

        public static void Debug(this TraceSource trace, TimeSpan elapsed, string message, params object[] args)
        {
            traceEvent(trace, elapsed, LogEventType.Debug, message, args);
        }


        public static void Warning(this TraceSource trace, TimeSpan elapsed, string message, params object[] args)
        {
            traceEvent(trace, elapsed, LogEventType.Warning, message, args);
        }

        public static void Error(this TraceSource trace, TimeSpan elapsed, string message, params object[] args)
        {
            traceEvent(trace, elapsed, LogEventType.Error, message, args);
        }

        public static void Info(this TraceSource trace, string message, params object[] args)
        {
            traceEvent(trace, LogEventType.Information, message, args);
        }


        public static void Warning(this TraceSource trace, string message, params object[] args)
        {
            traceEvent(trace, LogEventType.Warning, message, args);
        }


        public static void Error(this TraceSource trace, string message, params object[] args)
        {
            traceEvent(trace, LogEventType.Error, message, args);
        }


        public static void Debug(this TraceSource trace, Exception exception)
        {
            exceptionEvent(trace, exception, LogEventType.Debug);
        }


        public static void Error(this TraceSource trace, Exception exception)
        {
            exceptionEvent(trace, exception, LogEventType.Error);
        }

        const int callerStackTraceLimit = 4;
        static void WriteStackTrace(TraceSource trace, StackTrace stack, LogEventType level)
        {
            var frames = stack.GetFrames() ?? Array.Empty<StackFrame>();
            var lines = frames
                .Skip(3)
                .Where(line => line.HasMethod());
            int lineCount = lines.Count();

            lines = lines.Take(callerStackTraceLimit);

            foreach (StackFrame line in lines)
            {
                var fixedLine = $"at {line.GetMethod()}";
                if (line.HasSource())
                {
                    fixedLine += $" in {line.GetFileName()}:line {line.GetFileLineNumber()}";
                }
                trace.TraceEvent(level, 2, "    " + fixedLine, false);
            }

            if (lineCount > callerStackTraceLimit)
            {
                trace.TraceEvent(level, 2, "    ...");
            }
        }

        static void WriteException(TraceSource trace, Exception exception, LogEventType level, bool appendStack = true, bool isInner = false)
        {
            var exceptionName = exception.GetType().Name;
            try
            {
                var exceptionMessage = exception.Message
                    .Replace("{", "{{")
                    .Replace("}", "}}");
                if (isInner)
                    trace.TraceEvent(level, 2, "  Inner " + exceptionName + ": " + exceptionMessage, false);
                else
                    trace.TraceEvent(level, 2, exceptionName + ": " + exceptionMessage);
                if (exception.StackTrace != null)
                {
                    string[] lines = exception.StackTrace.Split(new char[] { '\n' });
                    foreach (string line in lines)
                    {
                        var fixedLine = line.Replace("{", "{{").Replace("}", "}}").Trim();
                        trace.TraceEvent(LogEventType.Debug, 2, "    " + fixedLine, false);
                    }
                }

                if (exception is ReflectionTypeLoadException)
                {
                    ReflectionTypeLoadException reflectionTypeLoadException = (ReflectionTypeLoadException)exception;
                    foreach (Exception ex in reflectionTypeLoadException.LoaderExceptions)
                    {
                        WriteException(trace, ex, level, false);
                    }
                }
                else if (exception is AggregateException ag)
                {
                    foreach (var inner in ag.InnerExceptions)
                    {
                        WriteException(trace, inner, level, false, true);
                    }
                }
                else if (exception.InnerException is Exception inner)
                {
                    WriteException(trace, inner, level, false, true);
                }
            }
            catch (Exception)
            {
                trace.TraceEvent(level, 2, "Error caught while logging an exception.");
            }

            if (appendStack)
            {
                try
                {
                    var stackTrace = new StackTrace(true);
                    trace.TraceEvent(level, 2, "Exception caught at:");
                    WriteStackTrace(trace, stackTrace, level);
                }
                catch
                {
                    trace.TraceEvent(level, 2, "Error retrieving current stacktrace.");
                }
            }
        }


        public static void Flush()
        {
            rootLogContext.Flush();
        }


        public static void StartSync()
        {
            Flush();
            rootLogContext.Async = false;
        }

        public static void StopSync()
        {
            rootLogContext.Async = true;
            Flush();
        }
    }
}
