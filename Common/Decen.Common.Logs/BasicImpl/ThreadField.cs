using Decen.Common.Logs.AbstractClass;
using Decen.Common.Logs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.BasicImpl
{
    /// <summary>
    /// 线程字段是管理线程字段值的静态对象。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThreadField<T> : ThreadField
    {
        readonly int mode;

        bool isCached => (mode & (int)ThreadFieldMode.Cached) > 0;

        public ThreadField(ThreadFieldMode threadFieldMode = ThreadFieldMode.None) => mode = (int)threadFieldMode;

        public T Value
        {
            get => Get();
            set => Set(value);
        }

        public T GetCached()
        {
            //var thread = TapThread.Current;
            //if (thread.Fields != null && thread.Fields.Length > Index && thread.Fields[Index] is T x)
            //    return x;
            return default;
        }

        T Get()
        {
            //var thread = TapThread.Current;
            //bool isParent = false;

            //// iterate through parent threads.
            //while (thread != null)
            //{
            //    object found;
            //    if (thread.Fields != null && thread.Fields.Length > Index && (found = thread.Fields[Index]) != null)
            //    {
            //        if (isCached)
            //        {
            //            if (isParent)
            //                set(found); // set the value on the current thread (not on parent).
            //            if (ReferenceEquals(found, DefaultCacheMarker))
            //                return default;
            //        }
            //        return (T)found;
            //    }

            //    thread = thread.Parent;
            //    isParent = true;
            //}

            //if (isCached)
            //    set(DefaultCacheMarker);

            return default;
        }

        void set(object value)
        {
            //var currentThread = TapThread.Current;
            //if (currentThread.Fields == null)
            //    currentThread.Fields = new object[Index + 1];
            //else if (currentThread.Fields.Length <= Index)
            //{
            //    var newArray = new object[Index + 1];
            //    currentThread.Fields.CopyTo(newArray, 0);
            //    currentThread.Fields = newArray;
            //}
            //currentThread.Fields[Index] = value;
        }

        void Set(T value) => set(value);
    }
}
