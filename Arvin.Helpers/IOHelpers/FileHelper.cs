using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Arvin.Helpers
{
    /// <summary>
    /// 文件操作帮助类
    /// </summary>
    public partial class FileHelper
    {
        //文件读写
        //压缩解压

        public FileInfo Info { get; set; }
        public FileHelper(string filePath)
        {
            this.Info = new FileInfo(filePath);
            if (!Info.Exists)
            {
                Directory.CreateDirectory(Info.Directory.FullName);
                Info.Create().Close();
            }
        }


        public void WriteToFile(string content)
        {
            using (StreamWriter sw = new StreamWriter(Info.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
            {
                sw.Write(content);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
        }

        public static void WriteToText(string path, string contents)
        {
            FileHelper file = new FileHelper(path);
            file.WriteToFile(contents);
        }
    }
}
