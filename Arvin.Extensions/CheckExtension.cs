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

        #region Range
        /// <summary>
        /// 范围值检查
        /// </summary>
        /// <param name="num"></param>
        /// <param name="range"></param>
        /// <param name="hasBoundaryVal">true-[range.min,range.max],false-[range.min,range,max]</param>
        /// <returns></returns>
        public static bool IsInRange(this int num, (int min, int max) range, bool hasBoundaryVal = true)
        {
            if (range.min > range.max)
                range = (range.max, range.min);
            if (hasBoundaryVal)
                return num >= range.min && num <= range.max;
            else
                return num > range.min && num < range.max;
        }
        public static bool IsInRange(this int num, int min, int max, bool hasBoundaryVal = true)
        {
            if (min > max)
                StructExt.Swap(ref min, ref min);
            return num.IsInRange((min, max), hasBoundaryVal);
        }
        /// <summary>
        /// 范围值检查
        /// </summary>
        /// <param name="num"></param>
        /// <param name="range"></param>
        /// <param name="hasBoundaryVal">true-[range.min,range.max],false-[range.min,range,max]</param>
        /// <returns></returns>
        public static bool IsInRange(this double num, (double min, double max) range, bool hasBoundaryVal = true)
        {
            if (range.min > range.max)
                range = (range.max, range.min);
            if (hasBoundaryVal)
                return num >= range.min && num <= range.max;
            else
                return num > range.min && num < range.max;
        }
        public static bool IsInRange(this double num, double min, double max, bool hasBoundaryVal = true)
        {
            if (min > max)
                StructExt.Swap(ref min, ref min);
            return num.IsInRange((min, max), hasBoundaryVal);
        }
        public static bool IsInRange<T>(this T num, T min, T max) where T : IComparable<T>
        {
            if (num.CompareTo(min) < 0) return false;
            if (num.CompareTo(max) > 0) return false;
            return true;
        }
        public static bool IsInRange<T>(this T num, T min, T max, bool hasBoundaryVal = true)
            where T : struct, IComparable<T>
        {
            if (min.IsGreater(max))
                StructExt.Swap(ref min, ref min);
            return num.IsInRange((min, max), hasBoundaryVal);
        }
        /// <summary>
        /// 范围值检查
        /// </summary>
        /// <param name="num"></param>
        /// <param name="range"></param>
        /// <param name="hasBoundaryVal">true-[range.min,range.max],false-[range.min,range,max]</param>
        /// <returns></returns>
        public static bool IsInRange<T>(this T num, (T min, T max) range, bool hasBoundaryVal = true)
            where T : IComparable<T>
        {
            if (range.min.IsGreater(range.max))
                range = (range.max, range.min);
            if (hasBoundaryVal)
                return num.IsGreaterEqual(range.min) && num.IsLessEqual(range.max);
            else
                return num.IsGreater(range.min) && num.IsLess(range.max);
        }
        #endregion
    }

    /// <summary>
    /// 结构/数值类操作扩展，用于无法通过this进行操作的场景
    /// </summary>
    public class StructExt
    {
        public static void Swap<T>(ref T a, ref T b)
            where T : struct
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }
}