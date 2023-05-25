using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Decen.Common.Helpers
{
    /// <summary>
    /// 获取web.config configuration settings.
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string SQLiteConnectionString
        {
            get
            {
                return GetConnectionString("SQLiteConnectionString");
            }
        }

        public static string GetConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        public static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static bool GetAppSettingByBool(string key)
        {
            if (GetAppSetting(key).Equals("1"))
                return true;
            else if (GetAppSetting(key).Equals("0"))
                return false;
            else
                return Convert.ToBoolean(GetAppSetting(key));
        }
    }
}
