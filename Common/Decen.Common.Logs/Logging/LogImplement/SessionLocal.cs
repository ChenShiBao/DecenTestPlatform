using Decen.Common.Logs.Logging.LogInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.Logging.LogImplement
{
    /// <summary>
    /// 用于保存特定于会话的值。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SessionLocal<T> : ISessionLocal
    {
        /// <summary>
        ///当会话中的所有线程都已完成时，自动处理该值。
        ///只有当T是IDisposable时才有任何效果
        /// </summary>
        public bool AutoDispose { get; }

        public T Value
        {
            get
            {
                for (var session = Session.Current; session != null; session = session.Parent)
                {
                    if (session.sessionLocals.TryGetValue(this, out object val))
                        return (T)val;
                    if (session == session.Parent) throw new InvalidOperationException("This should not be possible");
                }
                return default;
            }
            set => Session.Current.sessionLocals[this] = value;
        }


        public SessionLocal(T rootValue, bool autoDispose = true) : this(autoDispose)
        {
            if (Equals(rootValue, default(T)) == false)
                Session.RootSession.sessionLocals[this] = rootValue;
        }

        public SessionLocal(bool autoDispose = true)
        {
            AutoDispose = autoDispose;
        }
    }
}
