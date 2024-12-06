using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Arvin.Extensions
{
    /// <summary>
    /// 文件操作扩展类、语法糖
    /// </summary>
    public static class FileExtension
    {
        public static string ReadFile(this string path)
        {
            if (!File.Exists(path)) return null;
            return File.ReadAllText(path);
        }

        public static void WriteFile(this string path, string content)
        {
            content.SaveToFile(path);
        }


        static void WriteAllText(this string content, string path, Encoding encoding=null)
        {
            if (encoding == null)
                File.WriteAllText(path, content);
            else
                File.WriteAllText(path, content, encoding);
        }

        /// <summary>
        /// 根据文件路径获取目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDirectoryFromPath(this string path)
        {
            return Path.GetDirectoryName(path);
        }

        // 确保路径的文件夹存在
        public static void InitDirectory(this string path)
        {
            string directoryPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public static void SaveToFile(this string content, string path, Encoding encoding = null)
        {
            path.InitDirectory();
            content.WriteAllText(path, encoding);
        }
        public static void SaveToFile(this string[] content, string path)
        {
            path.InitDirectory();
            File.WriteAllLines(path, content, Encoding.UTF8);//无论文件是否存在，都重新写入
        }
        /// <summary>
        /// Dictonary保存到文件
        /// Key=Value文本文件列表
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="path"></param>
        public static void SaveToFile(this Dictionary<string,string> dic,string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            File.WriteAllLines(path, dic.Select(x => $"{x.Key}={x.Value}"), Encoding.UTF8);//无论文件是否存在，都重新写入
        }

        public static string[] ReadAllLines(this string path)
        {
            if (!File.Exists(path)) return null;
            return File.ReadAllLines(path);
        }

        public static void LoadFromFile(this Dictionary<string,string> dic, string path,bool isFirstLoad=true)
        {
            path.WriteFile(string.Empty);//如果文件不存在，则创建一个空文件
            var content= path.ReadAllLines();
            if (content == null || content.Length == 0) //文件为空
            {
                dic.SaveToFile(path);//数据同步
                return;
            }
            if(dic.Count==0) //字典为空，直接赋值
            {
                content.ToList().ForEach(x => dic.AddOrUpdate(x.Split('=')[0], x.Split('=')[1], path));
                return;
            }
            //数据合并：字典文件都不为空，数据可能存在冲突，数据合并以文件数据为准
            var dicRead= content.ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1]);
            if(isFirstLoad)
                dic.Clear();
            dicRead.ToList().ForEach(x => dic.AddOrUpdate(x.Key, x.Value));
            dic.SaveToFile(path);//数据同步
            return;
        }

        public static void AddOrUpdate(this Dictionary<string, string> dic, string key, string value, string path)
        {
            dic.AddOrUpdate(key, value);
            dic.SaveToFile(path);
        }

        public static void Delete(this Dictionary<string, string> dic, string key, string path)
        {
            dic.Remove(key);
            dic.SaveToFile(path);
        }
        public static void Remove(this Dictionary<string, string> dic, string key, string path)
        {
            Delete(dic, key, path);
            dic.SaveToFile(path);
        }
    }
}
