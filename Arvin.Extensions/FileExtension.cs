using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Arvin.Extensions
{
    /// <summary>
    /// 文件操作扩展类、语法糖
    /// </summary>
    public static class FileExtension
    {
        public static void WriteAllText(this string content, string path, Encoding encoding=null)
        {
            if (!string.IsNullOrEmpty(path))
                return;
            if (encoding == null)
                File.WriteAllText(path, content);
            else
                File.WriteAllText(path, content, encoding);
        }

        public static void WriteAllLines(this string[] content, string path, Encoding encoding = null)
        {
            if (!string.IsNullOrEmpty(path))
                return;
            if (encoding == null)
                File.WriteAllLines(path, content);
            else
                File.WriteAllLines(path, content, encoding);
        }
    }
}
