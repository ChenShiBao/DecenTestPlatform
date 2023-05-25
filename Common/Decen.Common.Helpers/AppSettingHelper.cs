using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;

namespace Decen.Common.Helpers
{
    public static class AppSettingHelper
    {
        //private static readonly System.Web.Caching.Cache _httpRuntimeCache = HttpRuntime.Cache;

        /// <summary>
        /// 读取客户设置
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static string GetSettingString(string settingName)
        {
            try
            {
                string settingString = ConfigurationManager.AppSettings[settingName].ToString();
                return settingString;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 更新设置
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="valueName"></param>
        public static void UpdateSettingString(string settingName, string valueName)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ConfigurationManager.AppSettings[settingName] != null)
            {
                config.AppSettings.Settings.Remove(settingName);
            }
            config.AppSettings.Settings.Add(settingName, valueName);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetCache<T>(string key)
        {
            //T result;
            //try
            //{
            //    result = (T)_httpRuntimeCache.Get(key);
            //}
            //catch (Exception)
            //{
            //    result = default(T);
            //}
            //return result;


            T result = default(T);
            return result; 
        }

        /// <summary>
        /// 获取缓存(List<T>)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> GetCacheList<T>(string key)
        {
            //List<T> result;
            //try
            //{
            //    result = (List<T>)_httpRuntimeCache.Get(key);
            //}
            //catch (Exception ex)
            //{
            //    result = default(List<T>);
            //}
            //return result;

            List<T> result = default(List<T>);
            return result;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static string SetCache<T>(string key, T data)
        {
            //string result;
            //try
            //{
            //    if (data == null)
            //    {
            //        result = "缓存对象不能为空";
            //    }
            //    else
            //    {
            //        _httpRuntimeCache.Insert(key, data);
            //        result = "success";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    result = ex.Message;
            //}
            //return result;


            return "";
        }

        /// <summary>
        /// 设置缓存(list<T>)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static string SetCache<T>(string key, List<T> data)
        {
            //string result;
            //try
            //{
            //    if (data == null)
            //    {
            //        result = "缓存对象不能为空";
            //    }
            //    else
            //    {
            //        _httpRuntimeCache.Insert(key, data);
            //        result = "success";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    result = ex.Message;
            //}
            //return result;


            return "";
        }


        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Remove(string key)
        {
            //string result;
            //try
            //{
            //    _httpRuntimeCache.Remove(key);
            //    result = "success";
            //}
            //catch (Exception ex)
            //{
            //    result = ex.Message;
            //}
            //return result;

            return "";
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string RemoveList(int num)
        {
            string result;
            try
            {
                //var lt = GetCacheList<FileInfosPageListViewModel>("01");
                //for (int i=0;i< lt.Count;i++)
                //{
                //    if (lt[i].FileInfoID==num)
                //    {
                //        _httpRuntimeCache.Remove();
                //    }
                //}

                result = "success";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }


        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <returns></returns>
        public static string Clear()
        {
            //string result;
            //try
            //{
            //    var enumerator = _httpRuntimeCache.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        _httpRuntimeCache.Remove(enumerator.Key.ToString());
            //    }
            //    result = "success";
            //}
            //catch (Exception ex)
            //{
            //    result = ex.Message;
            //}
            //return result;

            return "";
        }


    }
}
