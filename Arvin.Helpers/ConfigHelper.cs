using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Arvin.Helpers
{
    /// <summary>
    /// 配置文件操作帮助类
    /// </summary>
    public class ConfigHelper
    {
        public static readonly string ConStr = System.Configuration.ConfigurationManager.ConnectionStrings["default"].ToString();

        public static IConfiguration GetDefaultConfiguration()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    //设置配置文件基本路径
                    .SetBasePath(Environment.CurrentDirectory)
                    //添加json配置。参数：optional配置文件是否为可选的，reloadOnChange是否热加载
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
            return configuration;
        }

        static IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());

        static ConfigHelper()
        {
            LoadJsonConfig();
        }

        static void LoadJsonConfig(string filePath = "appsettings.json")
        {
            builder = builder
                    //添加json配置。参数：optional配置文件是否为可选的，reloadOnChange是否热加载
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        }

        public static string GetSectionValue(string key)
        {
            return builder.Build().GetSection(key).Value;
        }

        public static string GetConnectionString(string name)
        {
            return builder.Build().GetConnectionString(name);
        }
    }
}
