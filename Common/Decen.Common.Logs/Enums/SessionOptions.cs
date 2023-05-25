using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.Enums
{
    [Flags]
    public enum SessionOptions
    {
        /// <summary>
        /// 不应用任何特殊行为。像这样启动一个会话，和刚刚启动一个TapThread是一样的。
        /// </summary>
        None = 0,
        /// <summary>
        /// Component settings are cloned for the sake of this session. Instrument, DUT etc instances are cloned.
        /// When this is used, test plans should be reloaded in the new context. This causes resources to be serialized.
        /// </summary>
        OverlayComponentSettings = 1,
        /// <summary>
        /// 重定向日志记录的会话中写入的日志消息只会转到该会话中添加的LogListener。
        /// Log messages written in Sessions that redirect logging only go to LogListeners that are added in that session.
        /// </summary>
        RedirectLogging = 2,
        ///// <summary>
        ///// When this option is specified, the thread context will not be a child of the calling context. 
        ///// Instead the session will be the root of a new separate context.
        ///// This will affect the behavior of e.g. <see cref="TapThread.Abort()"/> and <see cref="ThreadHierarchyLocal{T}"/>.
        ///// </summary>
        //ThreadHierarchyRoot = 4,
    }
}
