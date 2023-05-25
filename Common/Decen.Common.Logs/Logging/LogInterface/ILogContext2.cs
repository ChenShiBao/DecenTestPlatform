using Decen.Common.Logs.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.Logging.LogInterface
{
    /// <summary>
    /// ILogContext接口拓展.
    /// </summary>
    public interface ILogContext2 : ILogContext
    {
        /// <summary>
        /// 注册新事件
        /// </summary>
        void AddEvent(Event @event);
        /// <summary>
        /// 获取上下文是否有监听器
        /// </summary>
        bool HasListeners { get; }
    }
}
