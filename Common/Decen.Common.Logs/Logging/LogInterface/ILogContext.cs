using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Decen.Common.Logs.Logging.LogInterface
{
    [ComVisible(false)]
    public interface ILogContext
    {
        /// <summary>
        /// 使用给定的源标识符创建日志源。
        /// </summary>
        ILog CreateLog(string Source);

        /// <summary>
        /// 从上下文删除日志
        /// </summary>
        void RemoveLog(ILog LogSource);

        /// <summary>
        /// 日志监听器
        /// </summary>
        void AttachListener(ILogListener Listener);

        /// <summary>
        /// 分离日志，自动刷新上下文
        /// </summary>
        /// <param name="Listener"></param>
        void DetachListener(ILogListener Listener);

        ///刷新调用此方法时接收到的所有事件，但只等待几毫秒。
        ///<param name=“TimeoutMS”>等待消息的最长时间。如果为0，它将无限等待</参数>
        ///＜return＞如果等待成功，则为True；如果发生超时，则为false</返回>
        bool Flush(int TimeoutMS = 0);


        ///刷新在调用此方法的时刻接收到的所有事件，但只等待给定的持续时间。
        ///<param name=“Timeout”>等待消息的最长时间，或零等待无限</参数>
        ///＜return＞如果等待成功，则为True；如果发生超时，则为false</返回>
        bool Flush(TimeSpan Timeout);

        /// 用于所有后续记录的事件的时间戳方法。
        ILogTimestampProvider Timestamper { get; set; }


        ///如果为true，则将日志上下文设置为异步模式（避免在事件处理之前从调用返回日志源的潜在同步模式问题）。
        ///当为false时，日志源总是等待，直到所有日志侦听器都处理完事件。
        bool Async { get; set; }

        ///未完成事件的最大数量。仅与模式相关。
        int MessageBufferSize { get; set; }
    }
}
