using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Helpers
{
    public static class TimestampHelper
    {
        /// <summary>  
        /// 将c# DateTime时间格式转换时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static string DateTimeToTimestamp(DateTime time)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name="timeStamp"></param>        
        /// <returns></returns>        
        public static DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// 时间戳转为C#格式时间10位
        /// </summary>
        /// <param name="curSeconds">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        private static DateTime GetDateTimeFrom1970Ticks(long curSeconds)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return dtStart.AddSeconds(curSeconds);
        }

        /// <summary>
        /// 验证时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <param name="interval">差值（秒）</param>
        /// <returns></returns>
        public static bool IsTime(long time, double interval)
        {
            DateTime dt = GetDateTimeFrom1970Ticks(time);
            //取现在时间
            DateTime dt1 = DateTime.Now.AddSeconds(interval);
            DateTime dt2 = DateTime.Now.AddSeconds(interval * -1);
            if (dt > dt2 && dt < dt1)
            {
                return true;
            }

            return false;
        }
    }
}
