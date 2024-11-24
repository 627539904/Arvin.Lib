using Arvin.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arvin.Helpers
{
    /// <summary>
    /// 缓存帮助类
    /// 目前过期策略为定时过期，后期可根据需要扩展为滑动过期
    /// </summary>
    public class CacheHelper
    {
        readonly static Dictionary<string, CacheModel<object>> CacheDic = new Dictionary<string, CacheModel<object>>();

        public static long ExpiresSeconds { get; set; } = 60 * 5;//默认5分钟过期

        public static void Add(string key, object value)
        {
            Add(key, value, DateTime.Now.AddSeconds(ExpiresSeconds));
        }

        public static void Add(string key, object value, DateTime expiresTime)
        {
            CacheDic.Add(key, new CacheModel<object>
            {
                Value = value,
                ExpiresTime = expiresTime
            });
        }

        public static void Set(string key, object value)
        {
            CacheDic[key].Value = value;
        }

        public static T Get<T>(string key)
        {
            var res = CacheDic[key];
            return (T)res.Value;
        }

        public static bool Remove(string key)
        {
            return CacheDic.Remove(key);
        }

        public static bool ContainsKey(string key)
        {
            var res = CacheDic.ContainsKey(key);
            if (res)
            {
                var cache = CacheDic[key];
                if (cache.ExpiresTime <= DateTime.Now)
                {
                    Remove(key);
                    res = false;
                }
            }
            return res;
        }

        internal static void SetCache(string key, object value, DateTime expiresTime, bool isSlide = false)
        {
            if (ContainsKey(key))
            {
                if (isSlide)
                    SetSlide(key, value, expiresTime);
                else
                    Set(key, value);
            }
            else
                Add(key, value, expiresTime);
        }

        internal static T GetCache<T>(string key)
        {
            if (ContainsKey(key))
            {
                return Get<T>(key);
            }
            return default;
        }

        /// <summary>
        /// 滑动过期
        /// </summary>
        public static void SetSlide(string key, object value, DateTime expiresTime)
        {
            CacheDic[key].Value = value;
            CacheDic[key].ExpiresTime = expiresTime;
        }

        /// <summary>
        /// 获取缓存文件路径
        /// 支持缓存文件[yyyyMMdd]这种时间占位符
        /// </summary>
        /// <param name="cachePath"></param>
        /// <param name="cacheFile"></param>
        /// <returns></returns>
        public static string GetCachePath(string cachePath)
        {
            if (string.IsNullOrEmpty(cachePath))
                return "";
            //提取cacheFile中的日期占位符,如[yyyyMMdd]
            string pattern = @"\[([^\]]+)\]";
            var placeholder = cachePath.Match(pattern).Value;
            var dataFormat=placeholder.Replace("[", "").Replace("]", "");
            string path = $"{cachePath.Replace(placeholder, DateTime.Now.ToString(dataFormat))}";
            return path;
        }
    }

    public class CacheModel<T>
    {
        public T Value { get; set; }
        public DateTime ExpiresTime { get; set; }
    }
}
