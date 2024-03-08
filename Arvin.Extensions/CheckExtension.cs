using System;
using System.Collections.Generic;
using System.Linq;

namespace Arvin.Extensions
{
    /// <summary>
    /// 检查相关扩展
    /// </summary>
    public static class CheckExtension
    {
        #region 检查、判定
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || list.Count() == 0;
        }
        public static bool Contains<T>(this IEnumerable<T> source, T value, Func<T, T, bool> equalFunc = null)
        {
            IEqualityComparer<T> comparer = null;
            if (equalFunc == null)
                return source.Contains(value, comparer);

            return source.Any(p => equalFunc(p, value));
        }
        #endregion
    }
}