using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Arvin.Extensions
{
    /// <summary>
    /// 集合操作扩展
    /// </summary>
    public static class CollectionExtension
    {
        #region Group
        //聚类：根据自定义同组断言聚类分组
        public static List<List<T>> GroupWhere<T>(this List<T> source, Func<T, T, bool> isGroupFunc, Func<T, T, bool> isEqualFunc = null)
        {
            if (source.IsNullOrEmpty()) return new List<List<T>>();
            if (source.Count() == 1) return source.ItemToList();
            List<List<T>> res = new List<List<T>>();
            List<T> hasUsed = new List<T>();//优化迭代器
            foreach (var item in source)
            {
                if (hasUsed.Contains(item))
                    continue;
                var groupItems = source.Where(p => isGroupFunc(p, item)).ToList();//同组元素
                var sameGroup = res.FirstOrDefault(x => x.MergeList(groupItems).Count() > x.MergeList(groupItems).Distinct(isEqualFunc).Count());
                if (sameGroup == null)
                {
                    res.Add(groupItems); //新组增加
                }
                else
                {
                    sameGroup.AddRange(groupItems.Except(sameGroup)); //同组合并
                }
                hasUsed.AddRange(groupItems);
            }
            return res;
        }


        //public static List<TOutput> SelectCombination<T, TOutput>(this IEnumerable<T> source, Func<T, T, TOutput> groupFunc)
        //{
        //    var pairs = (from x in source
        //               from y in source
        //               where !x.Equals(y)
        //               select (x, y)).ToList();
        //    var res = pairs.ConvertAll(p => groupFunc(p.x, p.y));
        //    res= res.FilterNull();
        //    return res;
        //}

        /// <summary>
        /// 排列组合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="source"></param>
        /// <param name="groupFunc"></param>
        /// <returns></returns>
        public static List<(T x, T y)> SelectCombination<T>(this IEnumerable<T> source, Func<T, T, bool> matchFunc)
        {
            var pairs = (from x in source
                         from y in source
                         where matchFunc(x, y)
                         select (x, y)).ToList();
            return pairs;
        }
        /// <summary>
        /// 排列组合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="matchFunc"></param>
        /// <returns></returns>
        public static List<(T x, TSecond y)> SelectCombination<T, TSecond>(this IEnumerable<T> source, IEnumerable<TSecond> second, Func<T, TSecond, bool> matchFunc)
        {
            var pairs = (from x in source
                         from y in second
                         where matchFunc(x, y)
                         select (x, y)).ToList();
            return pairs;
        }

        /// <summary>
        /// 有序集合获取有序连续元素对
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns>List<i=(source[i], source[i+1])>,其中i为有序集合中元素的顺序</returns>
        public static List<(T x, T y)> SelectContinuousPair<T>(this List<T> source)
        {
            List<(T x, T y)> res = new List<(T x, T y)>();
            for (int i = 0; i < source.Count - 1; i++)
                res.Add((source[i], source[i + 1]));
            return res;
        }
        /// <summary>
        /// 连续元素对是否在有序集合中相邻
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pairX"></param>
        /// <param name="pairY"></param>
        /// <returns></returns>
        public static bool IsNearForContinuousPair<T>(this (T x, T y) pairX, (T x, T y) pairY)
        {
            return pairX.PairToList().MergeList(pairY.PairToList()).Distinct().Count() == 3;
        }
        public static List<T> PairToList<T>(this (T x, T y) pair)
        {
            return new List<T> { pair.x, pair.y };
        }
        #endregion

        #region 过滤 Filter
        public static List<T> FilterNull<T>(this List<T> source)
        {
            if (source.IsNullOrEmpty()) return source;
            return source.Where(p => p != null).ToList();
        }
        /// <summary>
        /// 过滤null值(自定义什么是null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate">null判定</param>
        /// <returns></returns>
        public static IEnumerable<T> FilterNull<T>(this IEnumerable<T> source,Func<T,bool> predicate)
        {
            if (source.IsNullOrEmpty()) return source;
            return source.Where(p => !predicate(p)).ToList();
        }
        public static Dictionary<TKey, TValue> FilterNull<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            if (source.IsNullOrEmpty()) return source;
            return source.Where(p => p.Value != null).ToDictionary(p => p.Key, p => p.Value);
        }
        public static Dictionary<TKey, List<TValue>> FilterNull<TKey, TValue>(this Dictionary<TKey, List<TValue>> source)
        {
            if (source.IsNullOrEmpty()) return source;
            return source.Where(p => !p.Value.IsNullOrEmpty()).ToDictionary(p => p.Key, p => p.Value);
        }
        #endregion

        #region 元素操作：Add/Remove/Replace

        /// <summary>
        /// 交换索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public static List<T> Swap<T>(this List<T> list, int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
            return list;
        }

        /// <summary>
        /// 添加：重复时不添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <param name="isEqual"></param>
        public static void Add<T>(this List<T> source, T item, Func<T, T, bool> isEqual)
        {
            if (source == null) return;
            var match = source.FirstOrDefault(p => isEqual(p, item));
            if (match != null) return;
            source.Add(item);
        }
        public static void AddOrUpdate<T>(this IList<T> source, T item, Func<T, T, bool> isEqual)
        {
            if (source == null) return;
            var match = source.FirstOrDefault(p => isEqual(p, item));
            if(match != null)
                source.Remove(match);
            source.Add(item);
        }
        /// <summary>
        /// 新增或取消
        /// 默认取消断言（重复取消）：source.Contains(item)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="item"></param>
        /// <param name="cancelPredicate"></param>
        public static void AddOrCancel<T>(this List<T> source, T item, Func<List<T>, T, bool> cancelPredicate = null)
        {
            cancelPredicate = cancelPredicate ?? ((s, p) => s.Contains(p));
            if (!cancelPredicate(source, item))
                source.Add(item);
        }
        public static void AddOrCancelIfNull<T>(this List<T> source, T item)
        {
            if (item != null)
                source.Add(item);
        }
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
                dic[key] = value;
            else
                dic.Add(key, value);
        }

        public static void AddIfNewKeyOrNullValue<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (!dic.ContainsKey(key))
                dic.Add(key, value);
            else
            {
                if (dic[key] == null)
                    dic[key] = value;
            }
        }

        public static void AddOrAppend<TKey, TValueItem>(this Dictionary<TKey, List<TValueItem>> dic, TKey key, List<TValueItem> value)
        {
            if (dic.ContainsKey(key))
                dic[key].AddRange(value);
            else
                dic.Add(key, value);
        }


        public static void AddRange<T>(this List<T> source, List<T> items, Func<T, T, bool> isEqual)
        {
            if (items.IsNullOrEmpty()) return;
            if (source == null) return;
            foreach (var item in items)
            {
                source.Add(item, isEqual);
            }
        }
        //targeList Null时不加入集合
        public static void AddRangeOrCancel<T>(this List<T> source, IEnumerable<T> targeList, bool isFilterNull = true)
        {
            if (source == null)
                source = new List<T>();
            if (!targeList.IsNullOrEmpty())
            {
                if (isFilterNull)
                    targeList = targeList.Where(p => p != null);
                source.AddRange(targeList);
            }
        }
        public static List<T> RemoveLast<T>(this List<T> source)
        {
            source.Remove(source.LastOrDefault());
            return source;
        }
        public static List<T> RemoveRange<T>(this List<T> source, IEnumerable<T> removeList)
        {
            int n = source.RemoveAll(p => removeList.Contains(p));
            return source;
        }
        public static void Replace<T>(this List<T> source, T oldItem, T newItem, Func<T, T, bool> equalFunc = null)
        {
            if (source.IsNullOrEmpty() || newItem == null || oldItem == null) return;

            if (equalFunc == null)
                source.Remove(oldItem);
            else
                source.RemoveAll(p => equalFunc(oldItem, p));
            source.Add(newItem);
        }
        #endregion

        #region 分解
        /// <summary>
        /// 大组拆分（递归：应拆尽拆）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="largePredicate">大组断言</param>
        /// <param name="splitFunc">拆分方案</param>
        /// <param name="maxTimes">最大可拆次数</param>
        /// <returns></returns>
        public static List<List<T>> SplitLarge<T>(this List<T> source, Func<List<T>, bool> largePredicate, Func<List<T>, List<List<T>>> splitFunc, int maxTimes = 10)
        {
            if (source.IsNullOrEmpty() || largePredicate == null || splitFunc == null) return new List<List<T>>();
            if (source.Count == 1) //元素过少，无需拆分
                return source.ItemToList();
            if (!largePredicate(source)) //非大组，不拆
                return source.ItemToList();
            if (maxTimes < 0) //无可消耗次数，不拆
                return source.ItemToList();
            //大组，拆
            var res = splitFunc(source);
            if (res.Count <= 1) //拆分失败,放弃
                return source.ItemToList();
            maxTimes--;
            return res.ConvertAllMany(p => p.SplitLarge(largePredicate, splitFunc, maxTimes));
        }
        #endregion

        #region 合并
        public static List<T> MergeList<T>(this T obj, params T[] mergeItem)
        {
            List<T> res = new List<T>();
            if (obj == null)
                return res;
            if (mergeItem.IsNullOrEmpty()) return obj.ItemToList();
            res.Add(obj);
            foreach (var item in mergeItem)
            {
                res.Add(item);
            }
            res = res.FilterNull();
            return res;
        }
        /// <summary>
        /// 多个list进行元素合并
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="mergeLists"></param>
        /// <returns></returns>
        public static List<T> MergeList<T>(this List<T> list, params List<T>[] mergeLists)
        {
            list = list ?? new List<T>();
            if (mergeLists.IsNullOrEmpty()) return list;
            list = list.ToList();
            foreach (var item in mergeLists)
            {
                if (item == null)
                    continue;
                list.AddRange(item.ToList());
            }
            return list;
        }
        public static Dictionary<TKey,TValue> MeregDic<TKey,TValue>(this Dictionary<TKey,TValue> dic, params Dictionary<TKey,TValue>[] mergeDics)
        {
            foreach (var dicItem in mergeDics)
            {
                if (dicItem == null)
                    continue;
                //dic = dic.Concat(dicItem).ToDictionary(x => x.Key, x => x.Value);
                foreach (var pairItem in dicItem)
                {
                    dic.AddIfNewKeyOrNullValue(pairItem.Key, pairItem.Value);
                }
            }
            return dic;
        }
        #endregion

        #region 查询 Find/Get
        public static T GetItemByIndex<T>(this IEnumerable<T> list, int index)
        {
            if (list.IsNullOrEmpty() || index < 0)
                return default;
            return list.ElementAtOrDefault(index);
        }

        /// <summary>
        /// 获取记录所在行
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static List<T> GetListByItem<T>(this List<List<T>> lists, T item)
        {
            foreach (var arr in lists)
            {
                if (arr.Contains(item))
                    return arr;
            }
            return null;
        }
        /// <summary>
        /// 两个集合排列组合对比后距离/关系最近的键值对
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="comparePropFunc"></param>
        /// <returns></returns>
        public static (T x, TSecond y) FindMinPair<T, TSecond>(this IEnumerable<T> first, IEnumerable<TSecond> second, Func<T, TSecond, double> comparePropFunc)
        {
            var anyStd = comparePropFunc(first.First(), second.First());
            return first.SelectCombination(second, (x, y) => comparePropFunc(x, y) <= anyStd).FindMin(p => comparePropFunc(p.x, p.y));
        }

        /// <summary>
        /// 用于择优算法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="bestFunc">择优判定</param>
        /// <returns></returns>
        public static List<T> FindBest<T>(this IEnumerable<T> source, Func<IEnumerable<T>, T> bestFunc)
        {
            if (source.IsNullOrEmpty()) return new List<T>();
            var res = bestFunc(source);
            if (res == null)
                return new List<T>();
            return res.ItemToList();
        }

        /// <summary>
        /// 获取最大值所在的记录
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TSource FindMax<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source.IsNullOrEmpty()) return default;
            return source.OrderBy(selector).Last();
        }

        /// <summary>
        /// 获取非最大值所有的记录
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> FindNonMaxElements<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            var max = source.FindMax(selector);
            return source.Where(r => selector(r) != selector(max));
        }

        /// <summary>
        /// 获取最小值所在的记录
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="expectData">source中需要排除掉的元素</param>
        /// <returns></returns>
        public static TSource FindMin<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector, params TSource[] expectData)
        {
            if (source.IsNullOrEmpty()) return default;
            if (!expectData.IsNullOrEmpty())
                source = source.Except(expectData);
            return source.OrderBy(selector).FirstOrDefault();
        }

        /// <summary>
        /// 获取非最小值所有的记录
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> FindNonMinElements<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            var min = source.FindMin(selector);
            return source.Where(r => selector(r) != selector(min));
        }

        /// <summary>
        /// 获取最大值集合，用于存在并列最大时
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<TSource> FindMaxs<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector, double tolerance = 0)
        {
            if (source.IsNullOrEmpty() || source.Count() == 0) return default;
            var max = source.FindMax(selector);
            return source.Where(p => selector(p).IsEqual(selector(max), tolerance)).ToList();
        }

        /// <summary>
        /// 获取最大值集合的索引，用于存在并列最大时
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<int> FindMaxIndexs<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source.IsNullOrEmpty()) return default;
            var maxList = source.FindMaxs(selector);
            var list = source.ToList();

            var intIndexs = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < maxList.Count; i++)
                {
                    if (list[i].Equals(maxList[j]))
                    {
                        intIndexs.Add(i);
                        break;
                    }
                }
            }
            return intIndexs;
        }

        /// <summary>
        /// 获取最小值集合，用于存在并列最小时
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<TSource> FindMins<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector, double tolerance = 0)
        {
            if (source.IsNullOrEmpty()) return default;
            var min = source.FindMin(selector);
            return source.Where(p => selector(p).IsEqual(selector(min), tolerance)).ToList();
        }

        /// <summary>
        /// 获取最小值集合的索引，用于存在并列最小时
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<int> FindMinIndexs<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source.IsNullOrEmpty()) return default;
            var maxList = source.FindMins(selector);
            var list = source.ToList();

            var intIndexs = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < maxList.Count; i++)
                {
                    if (list[i].Equals(maxList[j]))
                    {
                        intIndexs.Add(i);
                        break;
                    }
                }
            }
            return intIndexs;
        }

        /// <summary>
        /// 查找大OverValue的最小值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="func">计算比较的函数</param>
        /// <param name="overValue"></param>
        /// <returns></returns>
        public static TSource FindOverMin<TSource>(this IEnumerable<TSource> source, Func<TSource, double> func, double overValue)
        {
            TSource min = default(TSource);
            double mv = double.MinValue;
            foreach (var item in source)
            {
                var v = func(item);
                if (v > overValue)
                {
                    if (mv == double.MinValue)
                    {
                        mv = v;
                        min = item;
                    }
                    else if (mv > v)
                    {
                        mv = v;
                        min = item;
                    }
                }
            }
            return min;
        }
        #endregion

        #region 字典操作扩展 Dictionary
        public static bool ContainKeyAny(this Dictionary<string, string> dic,string partkey)
        {
            return dic.Keys.Any(k => k.Contains(partkey));
        }
        public static bool ContainKeyAnyIgnoreCase(this Dictionary<string, string> dic, string partkey)
        {
            return dic.Keys.Any(k => k.ToLower().Contains(partkey.ToLower()));
        }
        public static IList<T> ToIList<T,Tkey,TValue>(this Dictionary<Tkey,TValue> dic,Func<KeyValuePair<Tkey,TValue>,T> mapper)
        {
            return dic.Select(mapper).ToList();
        }
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
        public static TKey GetKeyByValue<TKey, TValue>(this Dictionary<TKey, TValue> dic, TValue value, Func<TValue, TValue, bool> equalFunc = null)
        {
            foreach (var item in dic)
            {
                if (equalFunc == null)
                {
                    if (item.Value.Equals(value))
                        return item.Key;
                }
                else
                {
                    if (equalFunc(item.Value, value))
                        return item.Key;
                }
            }
            return default;
        }
        public static List<TValueItem> GetValuesByValue<TKey, TValueItem>(this Dictionary<TKey, List<TValueItem>> dic, TValueItem value, Func<TValueItem, TValueItem, bool> equalFunc = null)
        {
            return dic[dic.GetKeyByValue(value, equalFunc)];
        }
        public static TKey GetKeyByValue<TKey, TValueItem>(this Dictionary<TKey, List<TValueItem>> dic, TValueItem value, Func<TValueItem, TValueItem, bool> equalFunc = null)
        {
            foreach (var item in dic)
            {
                if (item.Value.Contains(value, equalFunc))
                    return item.Key;
            }
            return default;
        }
        public static List<TKey> GetKeysByValue<TKey, TValueItem>(this Dictionary<TKey, List<TValueItem>> dic, TValueItem value, Func<TValueItem, TValueItem, bool> equalFunc = null)
        {
            List<TKey> list = new List<TKey>();
            foreach (var item in dic)
            {
                if (item.Value.Contains(value, equalFunc))
                    list.Add(item.Key);
            }
            return list;
        }
        /// <summary>
        /// 映射反转，反转需要满足TValue无重复，如果重复，会导致数据丢失
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static Dictionary<TValue, TKey> ReverseMapping<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            Dictionary<TValue, TKey> resDic = new Dictionary<TValue, TKey>();
            foreach (var item in dic)
            {
                resDic.AddOrUpdate(item.Value, item.Key);
            }
            return resDic;
        }
        /// <summary>
        /// 映射反转，反转需要满足TValueItem无重复，如果重复，会导致数据丢失
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static Dictionary<TValueItem, TKey> ReverseMapping<TKey, TValueItem>(this Dictionary<TKey, List<TValueItem>> dic)
        {
            Dictionary<TValueItem, TKey> resDic = new Dictionary<TValueItem, TKey>();
            foreach (var item in dic)
            {
                item.Value.ToList().ForEach(p => resDic.AddOrUpdate(p, item.Key));
            }
            return resDic;
        }
        /// <summary>
        /// 映射反转，反转时如果TValue重复，则将其对应的TKey合并为List
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static Dictionary<TValue, List<TKey>> ReverseMappingCanMerge<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            Dictionary<TValue, List<TKey>> resDic = new Dictionary<TValue, List<TKey>>();
            foreach (var item in dic)
            {
                resDic.AddOrAppend(item.Value, item.Key.ItemToList());
            }
            return resDic;
        }
        #endregion

        #region 集合运算
        /// <summary>
        /// 交集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> IntersectList<T>(IEnumerable<T> first, IEnumerable<T> second)
        where T : IEquatable<T>
        {
            HashSet<T> set = new HashSet<T>(first);
            return second.Where(set.Contains);
        }
        #endregion

        #region 复合运算 break-list分解
        /// <summary>
        /// list分解未list组列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="breakIndex">分界点</param>
        /// <returns></returns>
        public static List<List<T>> BreakList<T>(this List<T> list, List<int> breakIndexs)
        {
            List<List<T>> res = new List<List<T>>();
            foreach (var index in breakIndexs)
            {
                var pickList = list.Take(index + 1).ToList();
                res.Add(pickList);
                list.RemoveRange(pickList);
            }
            return res;
        }
        /// <summary>
        /// 获取中断点索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicateBreak">中断条件</param>
        /// <returns></returns>
        public static List<int> GetBreakIndexs<T>(this List<T> list, Func<T, T, bool> predicateBreak)
        {
            List<int> res = new List<int>();
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (predicateBreak(list[i], list[i + 1]))
                    res.Add(i);
            }
            return res;
        }
        #endregion

        #region IList扩展
        public static BindingList<T> ToBindingList<T>(this IList<T> list)
        {
            return new BindingList<T>(list);
        }
        public static BindingList<T> ToBindingList<T>(this IEnumerable<T> source)
        {
            return new BindingList<T>(source.ToList());
        }
        public static void RemoveBy<T>(this IList<T> source, Func<T, bool> predicate)
        {
            source.Remove(source.FirstOrDefault(predicate));
        }
        public static void RemoveAll<T>(this IList<T> source, Func<T, bool> predicate)
        {
            var removeList = source.Where(predicate).ToList();
            foreach (var item in removeList)
            {
                source.Remove(item);
            }
        }
        #endregion

        #region 排序
        public static IEnumerable<T> OrderByAsc<T>(this IEnumerable<T> source, string propertyName)
        {
            return source.AsQueryable()
                        .OrderBy(propertyName + " ascending");
        }
        public static IEnumerable<T> OrderByDesc<T>(this IEnumerable<T> source,string propertyName)
        {
            return source.AsQueryable()
                        .OrderBy(propertyName + " descending");
        }
        #endregion
    }


}