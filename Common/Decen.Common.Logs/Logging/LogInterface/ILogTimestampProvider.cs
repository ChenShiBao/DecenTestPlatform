using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Logs.Logging.LogInterface
{
    /// <summary>
    /// 时间戳使用机制
    /// </summary>
    public interface ILogTimestampProvider //: ITapPlugin
    {
        /// <summary>
        /// 当前生成的时间戳
        /// </summary>
        /// <returns></returns>
        long Timestamp();

        /// <summary>
        /// 将timestamp方法生成的时间戳转换为Ticks。
        /// </summary>
        long ConvertToTicks(long timestamp);
    }
}
