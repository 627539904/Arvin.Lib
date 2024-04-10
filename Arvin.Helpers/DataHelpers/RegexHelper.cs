using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Arvin.Helpers
{
    /// <summary>
    /// 正则表达式帮助类
    /// </summary>
    public partial class RegexHelper
    {
        public const string LineStart = "^";//匹配行首
        public const string LineEnd = "$";//匹配行尾
        public static string CommonPattern = "(.*?)";//万能匹配
        public static string Host = $"https://{CommonPattern}/";
        public static string ChineseChar = "[\u4e00-\u9fa5]+";//仅匹配中文字符
        public static string SpaceChar = "(\\s*?)";
        public static string MobilePhone = @"(13[0-9]|14[0-9]|15[0-9]|16[0|1|3|4|6|8|9]|17[2-9]|18[0-9]|19[0-9])\d{8}";//虚商号段（2021-05）：162 165 167 170 171
        public static string MobilePhoneAll = @"(13[0-9]|14[0-9]|15[0-9]|16[0-9]|17[0-9]|18[0-9]|19[0-9])\d{8}";//包括虚拟运营商
        public static string IDCard = @"(\d{18})|(\d{17}(\d|X|x)|(\d{15}))";//身份证号
        public static string Date_20yyMMdd = "20[0-9]{2}[0|1][0-9]{1}[0-3]{1}[0-9]{1}";//匹配字符串中形如20210716的部分

        public static Match MatchHost(string source)
        {
            return Regex.Match(source, Host, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 用于匹配带换行符的.（多用于万能匹配中匹配内容有换行内容）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static Match MatchDotAll(string source, string pattern)
        {
            return Regex.Match(source, pattern, RegexOptions.Singleline);
        }

        /// <summary>
        /// 用于匹配带换行符的.
        /// </summary>
        public static MatchCollection MatchesDotAll(string source, string pattern)
        {
            return Regex.Matches(source, pattern, RegexOptions.Singleline);
        }

        public static bool IsMatch(string source, string pattern)
        {
            return Regex.IsMatch(source, pattern, RegexOptions.IgnoreCase);
        }
    }
}
