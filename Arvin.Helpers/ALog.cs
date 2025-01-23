using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.Helpers
{
    public class ALog
    {
        public static string AppType = "Cmd";//console or other
        public static bool IsShowDeubg=true;//全局控制是否显示Debug信息
        static Action<string> LogAction = text => DefaultWrite(text);

        public static void SetLogAction(Action<string> action)
        {
            LogAction = action;
        }
        public static void Write(string msg)
        {
            LogAction(msg);
        }

        static void DefaultWrite(string msg)
        {
            if (AppType=="Cmd")
                Console.Write(msg);
            else
                System.Diagnostics.Debug.Write(msg);
        }

        public static void WriteLine(string msg = "")
        {
            Write(msg + Environment.NewLine);
        }

        public static void Info(string msg="")
        {
            WriteLine("Info:" + msg);
        }

        public static void Error(string msg)
        {
            WriteLine("Error:" + msg);
        }

        public static void Deubg(string msg, bool? isShow = true)
        {
            bool isPrint = isShow ?? IsShowDeubg;
            if (isPrint)
                WriteLine("Debug:" + msg);
        }

        public static void LogToLine(string content, string log_dir = null)
        {
            Console.WriteLine(content);
            bool isPrintToFile = !string.IsNullOrEmpty(log_dir);
            if (isPrintToFile)
            {
                log_dir = $@"{log_dir}\{DateTime.Now.ToString("yyyy-MM-dd")}";
                if (!Directory.Exists(log_dir))
                    Directory.CreateDirectory(log_dir);
                string log_path = $@"{log_dir}\{DateTime.Now.Hour}.txt";
                File.AppendAllText(log_path, content + System.Environment.NewLine);
            }
        }
    }
}
