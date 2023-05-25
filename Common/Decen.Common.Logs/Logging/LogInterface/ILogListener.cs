using Decen.Common.Logs.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.Logging.LogInterface
{
    public interface ILogListener
    {
        /// <summary>
        /// 记录多个事件时调用的消息。
        /// </summary>
        void EventsLogged(IEnumerable<Event> Events);

        /// <summary>
        /// 当日志上下文请求此侦听器必须刷新其所有输出资源时调用。
        /// </summary>
        void Flush();
    }
}
