using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Decen.Common.Extensions
{
    public static class DataConvertToViewModelExtension
    { /// <summary>
      /// 数据映射转换
      /// </summary>
      /// <typeparam name="TResult"></typeparam>
      /// <param name="source"></param>
      /// <returns></returns>
        public static TResult Adapt<TResult>(this object source) where TResult : class, new()
        {
            Type sourceType = source.GetType();
            TResult result = Activator.CreateInstance<TResult>();
            foreach (PropertyInfo pi in sourceType.GetProperties())
            {
                var p = result.GetType().GetProperties();
                PropertyInfo info = result.GetType().GetProperties().Where(d => d.Name == pi.Name).FirstOrDefault();
                if (info != null)
                {
                    info.SetValue(result, pi.GetValue(source, null));
                }
            }
            return result;
        }
    }
}
