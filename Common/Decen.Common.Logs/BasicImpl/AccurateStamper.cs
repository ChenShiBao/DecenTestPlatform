using Decen.Common.Logs.Logging.LogInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decen.Common.Logs.BasicImpl
{
    public class AccurateStamper : ILogTimestampProvider
    {
        public override string ToString() => "Local and Accurate";
        long ILogTimestampProvider.Timestamp()
        {
            return DateTime.UtcNow.Ticks;
        }

        Stopwatch sw = Stopwatch.StartNew();
        long UtcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).Ticks;
        const long ticksPerSecond = 10000000; // TimeSpan.FromSeconds(1).Ticks;
        long ILogTimestampProvider.ConvertToTicks(long timestamp)
        {
            if (sw.ElapsedTicks > ticksPerSecond)
            {
                UtcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).Ticks;
                sw.Restart();
            }

            return (timestamp + UtcOffset);
        }
    }
}
