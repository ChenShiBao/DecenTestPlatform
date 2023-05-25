using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Decen.Common.Helpers
{
    public static class UserSystemConfig
    {
        private static Configuration configObject;

        //指定配置文件路径
        public static bool Init(string configPath)
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configPath;
            configObject = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            if (configObject.HasFile)
                return true;
            else
                return false;
        }
        //设置键值
        public static bool SetConfig(string key, string value)
        {
            try
            {
                if (!configObject.AppSettings.Settings.AllKeys.Contains(key))
                    configObject.AppSettings.Settings.Add(key, value);
                else
                    configObject.AppSettings.Settings[key].Value = value;
                configObject.Save(ConfigurationSaveMode.Modified);
                return true;
            }
            catch { return false; }
        }

        //获取键值
        public static string GetConfig(string key)
        {
            string val = string.Empty;
            if (configObject.AppSettings.Settings.AllKeys.Contains(key))
                val = configObject.AppSettings.Settings[key].Value;
            return val;
        }
    }
}
