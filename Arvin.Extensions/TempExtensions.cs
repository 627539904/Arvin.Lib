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
        #region 集合运算
        public static IEnumerable<T> IntersectList<T>(IEnumerable<T> first, IEnumerable<T> second)
        where T : IEquatable<T>
        {
            HashSet<T> set = new HashSet<T>(first);
            return second.Where(set.Contains);
        }
        #endregion

        #region 字典操作
        //字典转csv
        public static void ToCsv<TRecord>(this Dictionary<string, int> dic, string csvPath, Func<KeyValuePair<string, int>, TRecord> mapper)
        {
            using (var writer = new StreamWriter(csvPath, false, Encoding.UTF8))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(dic.Select(mapper));
                }
            }
        }
        #endregion

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
