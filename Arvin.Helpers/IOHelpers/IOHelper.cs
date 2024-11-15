using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.Helpers
{
    public class IOHelper
    {

    }

    public partial class DirectoryHelper
    {
        //获取文件夹内所有文件
        public static List<string> GetFiles(string path)
        {
            return Directory.GetFiles(path).ToList();
        }

        //获取文件夹内所有文件夹
        public static List<string> GetDirectories(string path)
        {
            return Directory.GetDirectories(path).ToList();
        }

        //获取文件夹内包含指定前缀的文件全路径，前缀为文件名前缀
        public static List<string> GetFilesWithPrefix(string path, string prefix)
        {
            return Directory.GetFiles(path).Where(x => FileHelper.IsFileNameWithPrefix(x,prefix)).ToList();
        }

        //获取文件夹中指定文件名的文件全路径（文件名无扩展名）
        public static List<string> GetFilesWithName(string path, string name)
        {
            return Directory.GetFiles(path).Where(x =>FileHelper.GetFileNameWithoutExtension(x) == name).ToList();
        }

        //打开指定文件夹
        public static void OpenDirectory(string path,Action<string> WriteLine=null)
        {
            string folderPath = path;
            if (Directory.Exists(folderPath))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = folderPath,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // 对于Linux和macOS，可以使用其他方法打开文件夹，例如xdg-open或open命令  
                    // 但这通常需要具体环境的配置，这里简单处理为不打开  
                    WriteLine?.Invoke("在Linux或macOS上无法直接打开文件夹。");
                }
            }
            else
            {
                WriteLine?.Invoke("文件夹不存在。");
            }
        }

        //清理文件夹
        public static void ClearDirectory(string path, Action<string> WriteLine = null)
        {
            string folderPath = path;
            if (Directory.Exists(folderPath))
            {
                // 删除所有子文件夹和文件  
                foreach (string file in Directory.GetFiles(folderPath))
                {
                    File.Delete(file);
                }

                foreach (string dir in Directory.GetDirectories(folderPath))
                {
                    Directory.Delete(dir, true); // true表示递归删除  
                }

                WriteLine?.Invoke("文件夹内容已清理。");
            }
            else
            {
                WriteLine?.Invoke("文件夹不存在，无需清理。");
            }
        }
    }


    public partial class FileHelper
    {
        //获取文件名
        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        //获取文件扩展名
        public static string GetFileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        //获取文件名（无扩展名）
        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        //判断文件是否存在
        public static bool IsFileExists(string path)
        {
            return File.Exists(path);
        }

        //判断文件名是否包含指定前缀
        public static bool IsFileNameWithPrefix(string path, string prefix)
        {
            return Path.GetFileName(path).Contains(prefix);
        }

        //csv文件是否存在或为空
        public static bool IsCsvNullOrEmpty(string path)
        {
            return !File.Exists(path) || File.ReadAllLines(path).Length == 0;
        }

        //生成csv文件
        public static void CreateCsvFile(string path, List<string> lines)
        {
            File.WriteAllLines(path, lines);
        }
    }
}
