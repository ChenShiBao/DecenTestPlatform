using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Helpers
{
    public static class ConvertToDataHelper
    {
        /// <summary>
        /// 将object转换为int类型
        /// 转换失败返回默认值：0
        /// </summary>
        /// <param name="obj">转换对象</param>
        /// <param name="defaultval">默认值（0）</param>
        /// <returns></returns>
        public static int ToInt(this object obj, int defaultval = 0)
        {
            if (obj.IsObjectNull())
            {
                return defaultval;
            }
            int result = 0;
            if (int.TryParse(obj.ToString(), out result))
            {
                return result;
            }
            else
            {
                return defaultval;
            }
        }

        /// <summary>
        /// 将object转换为decimal类型
        /// 转换失败返回默认值
        /// </summary>
        /// <param name="obj">转换对象</param>
        /// <param name="defaultval">默认值（0）</param>
        /// <returns>decimal</returns>
        public static decimal ToDecimal(this object obj, int defaultval = 0)
        {
            if (obj.IsObjectNull())
            {
                return defaultval;
            }
            decimal result = 0;
            if (decimal.TryParse(obj.ToString(), out result))
            {
                return result;
            }
            else
            {
                return defaultval;
            }
        }

        /// <summary>
        /// 将object转换为double类型
        /// 转换失败返回默认值
        /// </summary>
        /// <param name="obj">转换对象</param>
        /// <param name="defaultval">默认值（0）</param>
        /// <returns>double</returns>
        public static double ToDouble(this object obj, int defaultval = 0)
        {
            if (obj.IsObjectNull())
            {
                return defaultval;
            }
            double result = 0;
            if (double.TryParse(obj.ToString(), out result))
            {
                return result;
            }
            else
            {
                return defaultval;
            }
        }

        /// <summary>
        /// 将object转换为bool类型
        /// 转换失败返回默认值
        /// </summary>
        /// <param name="obj">转换对象</param>
        /// <param name="defaultval">默认值（0）</param>
        /// <returns>double</returns>
        public static bool ToBool(this object obj, bool defaultval = true)
        {
            if (obj.IsObjectNull())
            {
                return defaultval;
            }
            bool result = true;
            if (bool.TryParse(obj.ToString(), out result))
            {
                return result;
            }
            else
            {
                return defaultval;
            }
        }

        /// <summary>
        /// 验证object对象 转化日期格式
        /// </summary>
        /// <param name="obj">object对象</param>
        /// <returns>默认：NULL，反之 1900-01-01</returns>
        public static DateTime? ToDate(this object obj, bool defaultval = true)
        {
            if (obj.IsObjectNull())
            {
                if (defaultval)
                {
                    return null;
                }
                else
                {
                    return Convert.ToDateTime("1900-01-01 00:00:00");
                }
            }
            else
            {
                DateTime dt;
                if (DateTime.TryParse(obj.ToString(), out dt))
                {
                    return dt;
                }
                else
                {
                    if (defaultval)
                    {
                        return null;
                    }
                    else
                    {
                        return Convert.ToDateTime("1900-01-01");
                    }
                }
            }
        }

        /// <summary>
        /// 验证object对象是否为空
        /// 为空返回：true 不为空返回：false
        /// </summary>
        /// <param name="obj">object对象</param>
        /// <returns>bool</returns>
        public static bool IsObjectNull(this object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string ToNumeralString(this object obj, string format)
        {
            if (IsObjectNull(obj))
            {
                return null;
            }
            else
            {
                return obj.ToString().ToDouble().ToString(format);
            }
        }
        /// <summary> 
        /// 获取时间戳 
        /// </summary> 
        /// <returns></returns> 
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }
}
