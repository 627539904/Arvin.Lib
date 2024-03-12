﻿
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arvin.Helpers
{
    public class LogHelper
    {
        private LogHelper()
        {
            
        }
        public static void Init()
        {
            //全局日志管理
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .WriteTo.Console()
                            //.WriteTo.File("log.txt", rollingInterval: RollingInterval.Day) //需要Install-Package Serilog.Sinks.File
                            .CreateLogger();
        }
        public static void Debug()
        {
            throw new NotImplementedException();
        }

        public static void Error()
        {
            throw new NotImplementedException();
        }

        public static void Fatal()
        {
            throw new NotImplementedException();
        }

        public static void Info(string content)
        {
            Log.Logger.Information(content);
        }

        public static void Warn()
        {
            throw new NotImplementedException();
        }
    }
}
