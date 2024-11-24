using System;
using System.Collections.Generic;
using System.Text;

namespace Arvin.Helpers
{
    public partial class AConfig
    {
        public static string BaseDir= AppDomain.CurrentDomain.BaseDirectory;
        static Dictionary<string,string> dicConfig = new Dictionary<string, string>();//用于多自定义路径的配置，配置文件内容建议是configFile
        public static string GetConfigPath(string configFile)
        {
            return System.IO.Path.Combine(BaseDir, configFile);
        }
        public static string GetConfig(string key)
        {
            if (dicConfig.ContainsKey(key))
                return dicConfig[key];
            return null;
        }
        public static void SetConfig(string key, string value)
        {
            if (dicConfig.ContainsKey(key))
                dicConfig[key] = value;
            else
                dicConfig.Add(key, value);
        }
    }
}
