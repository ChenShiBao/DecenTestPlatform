using Decen.Common.Logs.Structs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Decen.Common.Logs.Logging
{
    /// <summary>
    /// 通用日志消息队列
    /// </summary>
    public class LogQueue
    {
        public LogBuffer _first;
        public LogBuffer _last;
        public static long _postedMessages;

        public static long PostedMessages
        {
            get
            {
                return _postedMessages;
            }

            set
            {
                _postedMessages = value;
            }
        }

        /// <summary>
        /// 允许无原子锁插入的固定长度日志缓冲区。
        /// </summary>
        public class LogBuffer
        {
            ///要在缓冲区中为多少日志消息腾出空间。
            const int Capacity = 1024 * 16;

            public LogBuffer Next = null;

            readonly Event[] logEvents = new Event[Capacity];
            readonly bool[] written = new bool[Capacity];

            int first;
            int last;

            int lastRead
            {
                get
                {
                    for (int i = 0; i < Capacity; i++)
                    {
                        if (written[i] == false)
                            return i;
                    }
                    return Capacity;
                }
            }

            public bool Empty
            {
                get { return (last <= first) || (first >= Capacity); }
            }

            public bool Done
            {
                get { return last >= (Capacity - 1); }
            }

            public bool PushMessage(string source, string message, long time, long duration, int eventType)
            {
                var index = Interlocked.Increment(ref last) - 1;

                if (index > (Capacity - 1))
                    return false;

                logEvents[index] = new Event
                {
                    Source = source,
                    Message = message,
                    Timestamp = time,
                    DurationNS = duration,
                    EventType = eventType
                };

                written[index] = true;

                return true;
            }

            public bool PushEvent(Event evt)
            {
                var index = Interlocked.Increment(ref last) - 1;

                if (index > (Capacity - 1))
                    return false;

                logEvents[index] = evt;

                written[index] = true;

                return true;
            }

            public ArraySegment<Event> PopCurrent()
            {
                int oldFirst = first;
                first = lastRead;
                int newFirst = first;

                if (newFirst > Capacity)
                    newFirst = Capacity;

                if (newFirst > oldFirst)
                    return new ArraySegment<Event>(logEvents, oldFirst, newFirst - oldFirst);

                return new ArraySegment<Event>(logEvents, 0, 0);
            }
        }


        readonly object lck = new object();

        ///防止过多数据写入缓冲区
        void maybeWaitForProcessing()
        {
            ////检查每个缓冲区的大小。
            ////如果容量超过4倍，我们将等待处理。
            //while (_first?.Next?.Next?.Next?.Next != null)
            //    TapThread.Sleep(10);
        }

        public void Enqueue(string source, string message, long time, long duration, int eventType)
        {
            maybeWaitForProcessing();
            Interlocked.Increment(ref _postedMessages);
            while (true)
            {
                LogBuffer buf = _last;
                if (!buf.PushMessage(source, message, time, duration, eventType))
                {
                    lock (lck)
                    {
                        if (!buf.Done)
                            continue;
                        LogBuffer nb = new LogBuffer();
                        if (Interlocked.CompareExchange(ref _last, nb, buf) == buf)
                            buf.Next = nb;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        public void Enqueue(Event evt)
        {
            maybeWaitForProcessing();
            Interlocked.Increment(ref _postedMessages);
            while (true)
            {
                LogBuffer buf = _last;
                if (!buf.PushEvent(evt))
                {
                    lock (lck)
                    {
                        if (!buf.Done)
                            continue;
                        LogBuffer nb = new LogBuffer();
                        if (Interlocked.CompareExchange(ref _last, nb, buf) == buf)
                            buf.Next = nb;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public Event[] DequeueBunch()
        {
            LogBuffer buf = _first;
            if (buf.Empty)
            {
                if (buf.Done)
                {
                    var oldFirst = _first;
                    var next = oldFirst.Next;

                    if (next != null)
                        Interlocked.CompareExchange(ref _first, next, oldFirst);
                }

                return null;
            }

            var res = buf.PopCurrent();

            if (buf.Done && buf.Empty)
            {
                var oldFirst1 = _first;
                var next1 = oldFirst1.Next;

                if (next1 != null)
                    Interlocked.CompareExchange(ref _first, next1, oldFirst1);
            }

            if ((res.Offset == 0) && (res.Count == res.Array.Length))
            {
                return res.Array;
            }

            Event[] res2 = new Event[res.Count];
            Array.Copy(res.Array, res.Offset, res2, 0, res.Count);
            return res2;
        }

        public int DequeueBunch(ref Event[] into)
        {
            LogBuffer buf = _first;
            if (buf.Empty)
            {
                if (buf.Done)
                {
                    var oldFirst = _first;
                    var next = oldFirst.Next;

                    if (next != null)
                        Interlocked.CompareExchange(ref _first, next, oldFirst);
                }
                return 0;
            }

            var res = buf.PopCurrent();

            if (buf.Done && buf.Empty)
            {
                var oldFirst1 = _first;
                var next1 = oldFirst1.Next;

                if (next1 != null)
                    Interlocked.CompareExchange(ref _first, next1, oldFirst1);
            }

            if (res.Count != into.Length)
            {
                Array.Resize(ref into, res.Count);
            }

            Array.Copy(res.Array, res.Offset, into, 0, res.Count);
            return res.Count;
        }

        public LogQueue()
        {
            _first = new LogBuffer();
            _last = _first;
        }

        public bool IsEmpty
        {
            get
            {
                LogBuffer buf = _first;

                return buf.Empty && (buf.Next == null);
            }
        }
    }
}
