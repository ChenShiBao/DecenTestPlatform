using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.Enums
{
    [Flags]
    public enum ThreadFieldMode
    {
        None = 0,
        ///缓存模式ThreadFields的速度要快一点，因为它们不需要迭代来查找常用值。
        ///在父线程中找到的值将升级到本地缓存。父线程线程字段值的更改在第一次被缓存
        Cached = 1
    }
}
