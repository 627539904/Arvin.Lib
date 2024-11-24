using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Drawing.Imaging;



#if NET6_0_OR_GREATER
using System.ComponentModel.DataAnnotations;
#endif

namespace Arvin.Extensions
{
    /// <summary>
    /// string 扩展方法
    /// </summary>
    public static class StringExtension
    {
        //加密
        //解密
        //格式化

        #region Check
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
        #endregion

        #region 字符串处理
        /// <summary>
        /// 将字符串转换为转义后的字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToTransferString(this string s)
        {
            s = s.Replace("\\", "\\\\");
            s = s.Replace("'", "\'");
            s = s.Replace("\"", "\\\"");
            return s;
        }
        /// <summary>
        /// 首次匹配与替换
        /// </summary>
        /// <param name="s"></param>
        /// <param name="oldStr"></param>
        /// <param name="newStr"></param>
        /// <returns></returns>
        public static string ReplaceFirstMatch(this string s, string oldStr, string newStr)
        {
            int fisrtIndex = s.IndexOf(oldStr);
            s = s.Remove(fisrtIndex, oldStr.Length);   //移除旧字符串
            s = s.Insert(fisrtIndex, newStr);  //插入被替换的字符串
            return s;
        }
        /// <summary>
        /// 用指定分隔符连接字符串
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sep"></param>
        /// <returns></returns>
        public static string Connect(this IEnumerable<string> list, string separator = "")
        {
            return string.Join(separator, list);
        }
        #endregion

        #region 正则
        /// <summary>
        /// 将正则表达式的所有匹配项以字符串的形式输出
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToStringFromList(this MatchCollection list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Match item in list)
            {
                sb.AppendLine(item.Value);
            }
            return sb.ToString();
        }
        public static Match Match(this string source, string pattern)
        {
            return Regex.Match(source, pattern);
        }

        public static Match Match(this string source, string pattern, RegexOptions options)
        {
            return Regex.Match(source, pattern, options);
        }

        public static MatchCollection Matches(this string source, string pattern)
        {
            return Regex.Matches(source, pattern);
        }
        #endregion

        #region 字符串反转
        /// <summary>
        /// 字符串反转，可用于常规反转，对于存在字符叠加的字符串，如越南文等存在字符和重音字叠加的，需要特殊处理
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Reverse(this string source)
        {
            char[] a = source.ToCharArray();
            System.Array.Reverse(a);
            return new string(a);
        }

        /// <summary>
        /// 字符串反转，适用于全球文字
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ReverseGlobal(this string source)
        {
            List<string> list = ListGlobal(source);
            list.Reverse();//反转list顺序
            return string.Join("", list.ToArray());//通过指定分隔符连接字符串数组中的每一个元素
        }

        /// <summary>
        /// 将含有全球化字符的字符串，转换为全球化字符列表
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        static List<string> ListGlobal(string source)
        {
            List<string> list = new List<string>();
            TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(source);
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current.ToString());
            }
            return list;
        }
        #endregion 字符串反转

        #region Unicode中文互相转换
        public static string StringToUnicode(this string source)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(source);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }

        public static string UnicodeToString(this string source)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }

        #endregion Unicode中文互相转换

        #region ToString
        /// <summary>
        /// 数组转字符串
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToString(this string[] arr, char separator)
        {
            if (arr.Length < 1)
                return "";
            if (arr.Length == 1)
                return arr[0];
            StringBuilder result = new StringBuilder();
            int i = 0;
            while (i < arr.Length)
            {
                if (i == arr.Length - 1)
                    result.Append(arr[i]);
                else
                    result.Append(arr[i] + separator);
                i++;
            }
            return result.ToString();
        }
        #endregion

        #region 数字转换(业务)
#if NET6_0_OR_GREATER
        /// <summary>
        /// 转换为中文大写
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static char ToChineseLowerCaseWithRange([Range(0, 9)] this int num)
        {
            return num.ToChineseLowerCase();
        }
#endif
        /// <summary>
        /// 转换为中文小写
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static char ToChineseLowerCase(this int num)
        {
            if (!num.IsInRange(0, 9))
                throw new ArgumentOutOfRangeException(nameof(num), "该值必须介于0和9之间.");
            char result = default;
            switch (num)
            {
                case 1:
                    result = '一';
                    break;
                case 2:
                    result = '二';
                    break;
                case 3:
                    result = '三';
                    break;
                case 4:
                    result = '四';
                    break;
                case 5:
                    result = '五';
                    break;
                case 6:
                    result = '六';
                    break;
                case 7:
                    result = '七';
                    break;
                case 8:
                    result = '八';
                    break;
                case 9:
                    result = '九';
                    break;
                case 0:
                    result = '〇';
                    break;
            }
            return result;
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// 转换为中文大写
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static char ToChineseUpperCaseWithRange([Range(0, 9)] this int num)
        {
            return num.ToChineseUpperCase();
        }
#endif
        /// <summary>
        /// 转换中文大写
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>

        public static char ToChineseUpperCase(this int num)
        {
            if (!num.IsInRange(0, 9))
                throw new ArgumentOutOfRangeException(nameof(num), "该值必须介于0和9之间.");
            char result = default;
            switch (num)
            {
                case 1:
                    result = '壹';
                    break;
                case 2:
                    result = '贰';
                    break;
                case 3:
                    result = '叁';
                    break;
                case 4:
                    result = '肆';
                    break;
                case 5:
                    result = '伍';
                    break;
                case 6:
                    result = '陆';
                    break;
                case 7:
                    result = '柒';
                    break;
                case 8:
                    result = '捌';
                    break;
                case 9:
                    result = '玖';
                    break;
                case 0:
                    result = '零';
                    break;
            }
            return result;
        }
        #endregion 数字转换

        #region Contain
        public static bool IsContains(this string str,IEnumerable<string> list)
        {
            return list.Any(p=>str.ToLower().Contains(p.ToLower()));
        }
        #endregion

        public static string TextToImage(this string text, string fileName="test.png",Size? size=null)
        {
            if (size == null)
                size = new Size(150, 150);
            int w = size.Value.Width;
            int h = size.Value.Height;
            // 创建画布
            Bitmap bitmap = new Bitmap(w, h);//需要System.Drawing.Common包
            // 创建画笔及字体
            Graphics graphics = Graphics.FromImage(bitmap);//定义背景颜色/画布

            // 初始字体大小和样式
            Font initialFont = new Font("Arial", 10); // 起始字体大小可以随意设置，但不要太小
            SizeF textSize = graphics.MeasureString(text, initialFont);

            float fontSize = w / textSize.Width * initialFont.Size; // 计算字体大小
            Font font = new Font(initialFont.FontFamily, fontSize, initialFont.Style);//定义字体和大小
            Brush brush = new SolidBrush(Color.Red);//定义字体颜色

            // 计算文字绘制位置以水平居中
            PointF textPosition = new PointF(0, (h - fontSize) / 2);

            // 绘制文字
            graphics.DrawString(text, font, brush, textPosition);
            // 保存为图片
            bitmap.Save(fileName, ImageFormat.Png);
            return fileName;
        }
    }
}
