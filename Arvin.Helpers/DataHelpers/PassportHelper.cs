using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Arvin.Helpers
{
    /// <summary>
    /// 护照验证
    /// </summary>
    public class PassportHelper
    {
        public readonly static string PassportPattern = "";

        public static bool Check(string passport)
        {
            return Regex.IsMatch(passport, PassportPattern);
        }
    }
}
