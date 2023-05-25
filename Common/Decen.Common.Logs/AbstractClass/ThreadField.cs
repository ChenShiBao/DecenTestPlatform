using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Decen.Common.Logs.AbstractClass
{
    public abstract class ThreadField
    {
        protected static readonly object DefaultCacheMarker = new object();
        static int threadFieldIndexer = 0;
        protected readonly int Index = GetThreadFieldIndex();
        static int GetThreadFieldIndex() => Interlocked.Increment(ref threadFieldIndexer);
    }
}
