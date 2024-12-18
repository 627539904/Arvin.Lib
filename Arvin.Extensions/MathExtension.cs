using System;
using System.Collections.Generic;
using System.Linq;

namespace Arvin.Extensions
{
    /// <summary>
    /// 数学扩展
    /// </summary>
    public static class MathExtension
    {
        #region 常用
        public static double ToRound(this double d, int digits = 2)
        {
            return Math.Round(d, digits);
        }
        public static double ToAbs(this double d)
        {
            return Math.Abs(d);
        }
        /// <summary>
        /// 平方
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Pow2(this double d)
        {
            return d * d;
        }
        /// <summary>
        /// n次方
        /// </summary>
        /// <param name="d"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double Pow(this double d, int n)
        {
            return Math.Pow(d, n);
        }
        #endregion

        #region 集合运算 Except/Intersect/Union
        //并集：同类型合并
        public static IEnumerable<T> UnionAll<T>(this IEnumerable<T> source,IEnumerable<T> second, Func<T, T, bool> equalPredicate = null)
        {
            if (source.IsNullOrEmpty() || second.IsNullOrEmpty()) return source;
            if (equalPredicate == null)
                return source.Union(second);
            return source.Union(second).Distinct(equalPredicate);
        }
        //同类型排除
        public static IEnumerable<T> ExceptAll<T>(this IEnumerable<T> source, IEnumerable<T> expectList, Func<T, T, bool> expectPredicate = null)
        {
            if (source.IsNullOrEmpty() || expectList.IsNullOrEmpty()) return source;
            if (expectPredicate == null)
                return source.Except(expectList);
            var expect = source.Where(p => expectList.Any(x => expectPredicate(x, p)));
            return source.Except(expect);
        }
        //不同类型排除，需提供排除断言
        public static IEnumerable<T> ExceptWhere<T, TTarget>(this IEnumerable<T> source, IEnumerable<TTarget> expectList, Func<T, TTarget, bool> expectPredicate)
        {
            if (source.IsNullOrEmpty() || expectList.IsNullOrEmpty()) return source;
            if (expectPredicate == null && typeof(T) != typeof(TTarget)) //未提供排除断言
                return source;
            var expect = source.Where(p => expectList.Any(x => expectPredicate(p, x)));
            return source.Except(expect);
        }
        public static IEnumerable<T> Intersect<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> equalFunc = null)
        {
            IEqualityComparer<T> comparer = null;
            if (equalFunc == null)
                return first.Intersect(second, comparer);//如果没有自定义比较，则使用Linq原始比较
            var merge = first.ToList().MergeList(second.ToList());//全集
            var distnctMerge = merge.Distinct(equalFunc);//去重后的全集
            var interect = merge.Except(distnctMerge).Distinct(equalFunc);//交集：重集去重
            return interect;
        }

        //交集取反
        public static IEnumerable<T> IntersectReserve<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> equalFunc = null)
        {
            equalFunc = equalFunc ?? ((x, y) => x.Equals(y));
            var interect = first.Intersect(second, equalFunc);
            var union = first.ToList().MergeList(second.ToList());
            var res = union.ExceptWhere(interect, equalFunc);
            return res;
        }
        #endregion
    }
}