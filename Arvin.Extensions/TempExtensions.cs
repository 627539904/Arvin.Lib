using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Linq;
using CsvHelper;

namespace Arvin.Extensions
{
    public static class TempExtensions
    {

        #region 语法糖
        public static T DefaultValue<T>(this T value, T defualtValue)
        {
            if (value == null)
                return defualtValue;
            if (value.GetType() == typeof(string))
                if (string.IsNullOrWhiteSpace(value as string))
                    return defualtValue;
            return value;
        }
        /// <summary>
        /// 将相对路径转为绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToAbsolutePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (Path.IsPathRooted(path)) //检查路径中是否包含盘符
                return path;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        public const string Pattern_FilePath_Local_Absolute = @"^(?:[a-zA-Z]:\\(?:[^\\]+\\)*[^\\]+\.[^\\]+)$";
        public static bool IsFilePath(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;
            string pattern = Pattern_FilePath_Local_Absolute;
            return Regex.IsMatch(text, pattern);
        }

        public static List<string> ToList(this StringBuilder sb)
        {
            return sb.ToString().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        #endregion
    }
}
