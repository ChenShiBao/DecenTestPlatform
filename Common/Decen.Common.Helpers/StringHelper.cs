using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Common.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// 截取等宽中英文字符串
        /// </summary>
        /// <param name="str">要截取的字符串</param>
        /// <param name="length">要截取的中文字符长度</param>
        /// <param name="appendStr">截取后后追加的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string CutString(this string str, int length, string appendStr)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            int len = length * 2;
            //aequilateLength为中英文等宽长度,cutLength为要截取的字符串长度
            int aequilateLength = 0, cutLength = 0;
            Encoding encoding = Encoding.GetEncoding("gb2312");

            string cutStr = str.ToString();
            int strLength = cutStr.Length;
            byte[] bytes;
            for (int i = 0; i < strLength; i++)
            {
                bytes = encoding.GetBytes(cutStr.Substring(i, 1));
                if (bytes.Length == 2)//不是英文
                    aequilateLength += 2;
                else
                    aequilateLength++;

                if (aequilateLength <= len) cutLength += 1;

                if (aequilateLength > len)
                    return cutStr.Substring(0, cutLength) + appendStr;
            }
            return cutStr;
        }

        /// <summary>
        /// 截取等宽中英文字符串
        /// </summary>
        /// <param name="str">要截取的字符串</param>
        /// <param name="length">要截取的中文字符长度</param>
        /// <param name="defaultval">true 加上(...) false 不加任何字符</param>
        /// <returns>截取后的字符串</returns>
        public static string CutString(this string str, int length, bool defaultval = true)
        {
            return str.CutString(length, defaultval ? "..." : "");
        }

        /// <summary>
        /// 将集合
        /// 转换成字符串
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        public static string ConvertListToString<T>(this List<T> lists, string str = ",")
        {
            if (lists == null || lists.Count == 0)
            {
                return String.Empty;
            }

            string all = string.Empty;

            foreach (var item in lists)
            {
                all += item.ToString() + str;
            }
            all = all.Substring(0, all.Length - 1);

            return all;
        }

        /// <summary>
        /// 将字符串
        /// 转换成集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static List<T> ConvertObjectToList<T>(this string str, char ch = ',')
        {
            if (string.IsNullOrEmpty(str))
            {
                return default(List<T>);
            }

            List<T> objList = new List<T>();
            foreach (var item in str.Split(new char[] { ch }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (typeof(T) == typeof(int))
                {
                    var n = int.Parse(item);
                    var obj = (object)n;
                    objList.Add((T)obj);
                }
                else if (typeof(T) == typeof(string))
                {
                    var obj = (object)item;
                    objList.Add((T)obj);
                }
                else if (typeof(T) == typeof(decimal))
                {
                    var d = decimal.Parse(item);
                    var obj = (object)d;
                    objList.Add((T)obj);
                }
            }
            return objList;
        }
    }
}
