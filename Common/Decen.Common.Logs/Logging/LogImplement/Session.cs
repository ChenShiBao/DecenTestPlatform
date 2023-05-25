using Decen.Common.Logs.BasicImpl;
using Decen.Common.Logs.Enums;
using Decen.Common.Logs.Logging.LogInterface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decen.Common.Logs.Logging.LogImplement
{
    public class Session : IDisposable
    {
        static readonly ThreadField<Session> sessionTField = new ThreadField<Session>(ThreadFieldMode.Cached);

        public static readonly Session RootSession;

        //TapThread threadContext;
        internal readonly ConcurrentDictionary<ISessionLocal, object> sessionLocals = new ConcurrentDictionary<ISessionLocal, object>();

        public readonly Session Parent;

        static Session()
        {
            RootSession = new Session(Guid.NewGuid(), SessionOptions.None, true);
            //RootSession.threadContext = TapThread.Current;
        }

        internal void DisposeSessionLocals()
        {
            foreach (var item in sessionLocals)
            {
                if (item.Key.AutoDispose && item.Value is IDisposable disp)
                {
                    disp.Dispose();
                }
            }
            sessionLocals.Clear();
        }

        /// <summary>
        /// Gets the currently active session.
        /// </summary>
        public static Session Current => sessionTField.Value ?? RootSession;

        /// <summary>
        /// Gets the session ID for this session.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the flags used to create/start this session.
        /// </summary>
        public SessionOptions Options { get; }

        Session(Guid id, SessionOptions options, bool rootSession = false)
        {
            Id = id;
            Options = options;
            if (!rootSession)
            {
                //threadContext = TapThread.Current;
                //Parent = Current;
            }
        }

        static TraceSource _log;
        // lazily loaded to prevent a circular dependency between Session and LogContext.
        // 延迟加载以防止会话和LogContext之间的循环依赖。
        static TraceSource log => _log ?? (_log = Log.CreateSource(nameof(Session)));

        readonly Stack<IDisposable> disposables = new Stack<IDisposable>();

        /// <summary> Disposes the session. </summary>
        public void Dispose()
        {
            var exceptions = new List<Exception>();
            while (disposables.Count > 0)
            {
                try
                {
                    var item = disposables.Pop();
                    item.Dispose();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            foreach (var ex in exceptions)
            {
                log.Error("Caught error while disposing session: {0}", ex.Message);
                log.Debug(ex);
            }
        }

        public static Session Create(SessionOptions options = SessionOptions.OverlayComponentSettings | SessionOptions.RedirectLogging, Guid? id = null)
        {
            //var session = new Session(id.HasValue ? id.Value : Guid.NewGuid(), options);
            //session.disposables.Push(TapThread.UsingThreadContext(session.DisposeSessionLocals));
            //session.Activate();
            //return session;

            return null;
        }

        public static Session Create(SessionOptions options) => Create(options, null);


        /// <summary>
        /// 创建一个新会话，并在该会话的上下文中运行指定的操作。当操作完成时，会话将自动被处置。
        /// </summary>
        public static void Start(Action action, SessionOptions options = SessionOptions.OverlayComponentSettings | SessionOptions.RedirectLogging)
        {
            //var session = new Session(Guid.NewGuid(), options);
            //TapThread.Start(() =>
            //{
            //    try
            //    {
            //        session.Activate();
            //        sessionTField.Value = session;
            //        action();
            //    }
            //    finally
            //    {
            //        session.Dispose();
            //    }
            //}, session.DisposeSessionLocals, $"SessionRootThread-{session.Id}");
        }

        /// <summary>
        /// 在给定会话的上下文中同步运行指定的操作
        /// </summary>
        public void RunInSession(Action action)
        {
            //TapThread.WithNewContext(action, threadContext);
        }

        void Activate()
        {
            try
            {
                //sessionTField.Value = this;
                //if (Options.HasFlag(SessionOptions.OverlayComponentSettings))
                //    ComponentSettings.BeginSession();
                //if (Options.HasFlag(SessionOptions.RedirectLogging))
                //    Log.WithNewContext();
            }
            catch
            {
                Dispose();
                throw;
            }
        }


    }
}
