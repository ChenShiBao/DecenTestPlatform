using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.Structs
{
    /// <summary>
    /// 包含有关事件的所有信息的结构。
    /// </summary>
    public struct Event
    {
        /// <summary>
        /// 构建新的结构。
        /// </summary>
        ///<param name=“duration”>事件的持续时间（以纳秒为单位）</参数>
        ///<param name=“eventType”>记录此事件的事件类型</参数>
        ///<param name=“message”>事件的消息</参数>
        ///<param name=“source”>记录此事件的日志源标识符</参数>
        ///<param name=“timestamp”>系统中事件的时间戳</参数>
        public Event(long duration, int eventType, string message, string source, long timestamp)
        {
            DurationNS = duration;
            EventType = eventType;
            Message = message;
            Source = source;
            Timestamp = timestamp;
        }
        public int EventType;

        public string Source;

        public long Timestamp;
        ///事件的持续时间（以纳秒为单位）。
        public long DurationNS;

        public string Message;
        ///创建此事件结构的字符串表示形式
        public override string ToString() => $"{Timestamp} : {Source} : {Message}";
    }
}
