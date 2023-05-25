using Decen.Common.Logs.Structs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.BasicImpl
{
    /// <summary>
    /// 集合类，它提供了在events数组上迭代
    /// </summary>
    public class EventCollection : IEnumerable<Event>, IDisposable
    {
        private Event[] events = null;

        #region nested types
        private class EventCollectionEnumerator : IEnumerator<Event>
        {
            #region private fields
            private int index = -1;
            private EventCollection eventCollection = null;
            private bool disposed = false;
            #endregion

            #region properties
            public Event Current
            {
                get
                {
                    VerifyNotDisposed();
                    try
                    {
                        Event element = eventCollection.events[index];
                        return element;
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        throw new InvalidOperationException(e.Message);
                    }
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    VerifyNotDisposed();
                    return Current;
                }
            }
            #endregion

            #region ctor
            public EventCollectionEnumerator(EventCollection eventCollection)
            {
                disposed = false;
                this.eventCollection = eventCollection;
            }
            #endregion

            public void Dispose()
            {
                VerifyNotDisposed();
                disposed = true;
            }

            public bool MoveNext()
            {
                VerifyNotDisposed();
                index++;
                return (index < eventCollection.events.Length);
            }

            public void Reset()
            {
                VerifyNotDisposed();
                index = -1;
            }
            private void VerifyNotDisposed()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("EventCollectionEnumerator");
                }
                else if (eventCollection.Disposed)
                {
                    throw new ObjectDisposedException("EventCollection");
                }
            }
        }
        #endregion

        #region properties

        /// <summary>
        ///返回一个布尔值，指示此实例是否已被释放。
        /// </summary>
        public bool Disposed
        {
            get
            {
                VerifyNotDisposed();
                return events == null;
            }
        }

        /// <summary>
        /// 获取集合中元素的数量
        /// </summary>
        public int Length
        {
            get
            {
                VerifyNotDisposed();

                if (events == null)
                {
                    throw new ObjectDisposedException("EventCollection");
                }

                if (events != null)
                    return events.Length;
                else
                    return 0;
            }
        }
        #endregion

        #region ctor 
        /// <summary>
        /// 将由此类封装的事件数组
        /// </summary>
        /// <param name="events"></param>
        public EventCollection(Event[] events)
        {
            this.events = events;
        }
        #endregion

        /// <summary>
        /// 处理实例
        /// </summary>
        public void Dispose()
        {
            VerifyNotDisposed();
            events = null;
        }

        /// <summary>
        /// 集合枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Event> GetEnumerator()
        {
            VerifyNotDisposed();
            return new EventCollectionEnumerator(this);
        }


        ///可用于枚举此集合的枚举器
        IEnumerator IEnumerable.GetEnumerator()
        {
            VerifyNotDisposed();
            return new EventCollectionEnumerator(this);
        }
        private void VerifyNotDisposed()
        {
            if (events == null)
            {
                throw new ObjectDisposedException("EventCollection");
            }
        }
    }
}
