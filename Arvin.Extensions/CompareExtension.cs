using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arvin.Extensions
{
    /// <summary>
    /// 比较器扩展
    /// </summary>
    public static class CompareExtension
    {
        #region 比较器> < >= <= ==
        public static bool IsGreater<T>(this T a, T b)
            where T : IComparable<T>
        {
            return a.CompareTo(b) > 0;
        }
        public static bool IsLess<T>(this T a, T b)
            where T : IComparable<T>
        {
            return a.CompareTo(b) < 0;
        }
        public static bool IsEqual<T>(this T a, T b)
            where T : IComparable<T>
        {
            return a.CompareTo(b) == 0;
        }
        public static bool IsGreaterEqual<T>(this T a, T b)
            where T : IComparable<T>
        {
            return a.IsGreater(b) || a.IsEqual(b);
        }
        public static bool IsLessEqual<T>(this T a, T b)
            where T : IComparable<T>
        {
            return a.IsLess(b) || a.IsEqual(b);
        }
        #endregion

        #region Distinct

        /// <summary>
        /// 复合比较器，自定义Equals，比较方式自己指定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="funcEquals"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> list, Func<T, T, bool> funcEquals)
        {
            return list.Distinct(new CommonComparer<T>(funcEquals)); //int用不上，随便指定的
        }
        /// <summary>
        /// 分组去重，Group+Select模式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="groupPredicate"></param>
        /// <param name="groupSelectedPredicate"></param>
        /// <returns></returns>
        public static List<T> DistinctSelect<T>(this List<T> source, Func<T, T, bool> groupPredicate, Func<T, IEnumerable<T>, List<T>> groupSelectPredicates)
        {
            var dic = source.ToDictionary(p => p, p => groupSelectPredicates(p, source.Where(x => groupPredicate(x, p))));
            return dic.SelectMany(p => p.Value).Distinct().ToList();
        }

        /// <summary>
        /// 择优去重算法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">源数据</param>
        /// <param name="groupPredicate">去重分组</param>
        /// <param name="fitBestFunc">择优判定</param>
        /// <param name="isReverseSelect">结果反转，默认不反转</param>
        /// <returns></returns>
        public static List<T> DistinctFitBest<T>(this List<T> source, Func<T, T, bool> groupPredicate, Func<List<T>, List<T>> fitBestFunc, bool isReverseSelect = false)
        {
            var groups = source.GroupWhere(groupPredicate).ToList();
            var res = groups.ConvertAllMany(fitBestFunc);
            if (isReverseSelect)
                res = source.Except(res).ToList();
            return res;
        }
        #endregion

        #region IsEqual 等值比较
        public static bool IsEqual(this decimal x, decimal y, double tolerance = 0.001, bool isContainBoundary = true)
        {
            return IsEqual(x.ToDouble(), y.ToDouble(), tolerance, isContainBoundary);
        }
        /// <summary>
        /// 误差值内相等，默认误差值0.001，默认包含0.001,如果不需要包含，设置为false
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tolerance"></param>
        /// <param name="isContainBoundary">是否包含误差边界值</param>
        /// <returns></returns>
        public static bool IsEqual(this double x, double y, double tolerance = 0.001, bool isContainBoundary = true)
        {
            if (tolerance == 0) return x == y;
            if (isContainBoundary)
                return Math.Abs(x - y) <= tolerance;
            else
                return Math.Abs(x - y) < tolerance;
        }
        public static bool IsEqualCore<T>(this T x, T y, Func<bool> isEqualFunc)
        {
            if (x == null || y == null) return false;
            if (x.Equals(y)) return true;
            return isEqualFunc();
        }
        #endregion
    }

    /// <summary>
    /// 通用比较器，根据需要指定需要比较的属性或比较方式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommonComparer<T> : IEqualityComparer<T>
    {
        Func<T, T, bool> _customEqual = null;

        /// <summary>
        /// 构造函数（复合），自定义比较器Equals实现，灵活多变,优先使用
        /// </summary>
        /// <param name="func"></param>
        public CommonComparer(Func<T, T, bool> func)
        {
            _customEqual = func;
        }

        public bool Equals(T x, T y)
        {
            if (_customEqual != null)
                return _customEqual(x, y);
            return x.Equals(y);
        }

        public int GetHashCode(T obj)
        {
            return 1;
        }
    }
}
