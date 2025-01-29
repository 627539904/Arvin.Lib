using Arvin.Extensions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.Helpers.DBHelpers
{
    /// <summary>
    /// Redis操作类
    /// 支持的数据结构：
    /// String
    /// Hash：一个键值对的集合，类似于 Python 中的字典或 Java 中的 HashMap，但每个哈希表都是独立存储的。
    /// List：一个有序的字符串列表，可以从两端进行元素的添加和移除操作。
    /// Set：一个无序的字符串集合，不允许重复元素，支持集合间的操作如并集、交集和差集。
    /// SortedSet：类似于集合，但每个元素都会关联一个分数（score），元素按分数排序。
    /// 位图（Bitmaps）：通过位数组的形式来存储和操作二进制数据。
    /// HyperLogLog：一种用于估计集合中唯一元素数量的概率数据结构。
    /// 地理空间索引（Geospatial）：用于存储地理位置信息，支持地理位置的查询和计算。
    /// </summary>
    public class RedisHelper
    {
        #region 基本
        // 连接到本地的Redis服务器（默认端口是6379）
        private ConnectionMultiplexer redis;
        //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("your_redis_host:6379,password=your_password"); //远程

        public int DbIndex { get; set; } = -1;
        public RedisHelper(int dbIndex=0, string connectionString = "localhost")
        {
            if (redis == null)
                redis = ConnectionMultiplexer.Connect(connectionString);
            DbIndex = dbIndex;
        }
        static Dictionary<int, IDatabase> dicDb = new Dictionary<int, IDatabase>();
        public IDatabase GetDB()
        {
            if (dicDb.ContainsKey(DbIndex))
            {
                return dicDb[DbIndex];
            }
            // 获取数据库实例（默认是db0）
            IDatabase db;
            if (DbIndex > 0)
            {
                db= redis.GetDatabase(DbIndex);
            }
            else
            {
                db = redis.GetDatabase();
            }
            dicDb.Add(DbIndex, db);
            return db;
        }
        public async Task KeyExpireAsync(string redisKey, TimeSpan expiry)
        {
            await GetDB().KeyExpireAsync(redisKey, expiry);
        }
        public async Task<bool> KeyDeleteAsync(string key)
        {
            var db = GetDB();
            return await db.KeyDeleteAsync(key);
        }
        public bool KeyDelete(string key)
        {
            var db = GetDB();
            return db.KeyDelete(key);
        }


        public void Close()
        {
            redis.Close();
        }

        public bool Exists(string redisKey)
        {
            return GetDB().KeyExists(redisKey);
        }
        ~RedisHelper()
        {
            Close();
        }
        #endregion


        #region String
        // Select
        public async Task<string> GetStringAsync(string key)
        {
            var db = GetDB();
            var value = await db.StringGetAsync(key);
            return value;
        }

        //Add Or Update
        public async Task SetAsync(string key, string value)
        {
            var db = GetDB();
            await db.StringSetAsync(key, value);
        }
        #endregion


        #region List

        public async Task<List<T>> GetList<T>(string redisKey)
        {
            RedisValue[] arr = await GetDB().ListRangeAsync(redisKey);
            List<T> list = new List<T>();

            foreach (var redisValue in arr)
            {
                if (redisValue.IsNullOrEmpty)
                    continue;
                string json = redisValue.ToString();
                list.Add(json.ToModel<T>());
            }

            return list;
        }
        public async Task SetList<T>(string redisKey, List<T> list)
        {
            // 清空现有的列表（可选，根据需求决定是否要清空）
            //await GetDB().ListTrimAsync(redisKey, 0, -1, true); // 实际上这是移除列表中的所有元素，相当于清空
            // 注意：如果不希望清空列表，应该使用其他逻辑来追加元素

            // 序列化列表中的每个对象并添加到 Redis 列表中
            foreach (var item in list)
            {
                string json = item.ToJsonIgnoreNull();
                await GetDB().ListRightPushAsync(redisKey, json);
            }
        }
        public async Task SetList<T>(string redisKey, List<T> list, TimeSpan expiry)
        {
            await SetList(redisKey, list);
            // 设置过期时间
            await GetDB().KeyExpireAsync(redisKey, expiry);
        }
        #endregion

        #region Hash
        public async Task SetHash(string redisKey, Dictionary<string, decimal> sectorIndexes)
        {
            var dic = sectorIndexes.ToDictionary(k => k.Key, v => v.Value.ToDouble());
            await SetHash(redisKey, dic);
        }
        public async Task SetHash(string redisKey, Dictionary<string, double> sectorIndexes)
        {
            var db = GetDB();
            var hash = new HashEntry[sectorIndexes.Count];
            int i = 0;
            foreach (var entry in sectorIndexes)
            {
                hash[i] = new HashEntry(entry.Key, entry.Value);//Key,Value的值需要能够被转换为字符串
                i++;
            }
            await db.HashSetAsync(redisKey, hash);
        }
        
        public async Task SetHash(string redisKey, Dictionary<string, string> sectorIndexes)
        {
            var db = GetDB();
            var hash = new HashEntry[sectorIndexes.Count];
            int i = 0;
            foreach (var entry in sectorIndexes)
            {
                hash[i] = new HashEntry(entry.Key, entry.Value);//Key,Value的值需要能够被转换为字符串
                i++;
            }
            await db.HashSetAsync(redisKey, hash);
        }
        public async Task SetHash(string redisKey, Dictionary<string, string> sectorIndexes, TimeSpan expiry)
        {
            await SetHash(redisKey, sectorIndexes);
            // 设置过期时间
            await GetDB().KeyExpireAsync(redisKey, expiry);
        }
        public async Task SetHash<TValue>(string redisKey, Dictionary<string, TValue> sectorIndexes)
        {
            var dic = sectorIndexes.ToDictionary(k => k.Key, v => v.Value.ToString());
            await SetHash(redisKey, dic);
        }
        public async Task SetHash<TValue>(string redisKey, Dictionary<string, TValue> sectorIndexes, TimeSpan expiry)
        {
            await SetHash(redisKey, sectorIndexes);
            // 设置过期时间
            await GetDB().KeyExpireAsync(redisKey, expiry);
        }
        public async Task SetHash<T>(string redisKey, Dictionary<string, List<T>> sectorIndexes)
        {
            var dic = sectorIndexes.ToDictionary(k => k.Key, v => v.Value.ToJson());
            await SetHash(redisKey, dic);
        }
        public async Task SetHash<T>(string redisKey, Dictionary<string, List<T>> sectorIndexes, TimeSpan expiry)
        {
            await SetHash(redisKey, sectorIndexes);
            // 设置过期时间
            await GetDB().KeyExpireAsync(redisKey, expiry);
        }
        public Dictionary<string, double> GetHash(string redisKey)
        {
            var db = GetDB();
            var hash = db.HashGetAll(redisKey);
            var result = new Dictionary<string, double>();

            foreach (var entry in hash)
            {
                if (double.TryParse(entry.Value.ToString(), out double value))
                {
                    result[entry.Name] = value;
                }
                else
                {
                    // 处理无法解析为 double 的情况，这里简单跳过
                    Console.WriteLine($"Warning: Value '{entry.Value}' for key '{entry.Name}' could not be parsed as double.");
                }
            }

            return result;
        }
        public Dictionary<string, TValue> GetHash<TValue>(string redisKey)
        {
            var db = GetDB();
            var hash = db.HashGetAll(redisKey);
            var result = new Dictionary<string, TValue>();

            foreach (var entry in hash)
            {
                try
                {
                    result[entry.Name] = (TValue)Convert.ChangeType(entry.Value, typeof(TValue));
                }
                catch
                {
                    // 处理无法解析为 TValue 的情况，这里简单跳过
                    ALog.Error($"Value '{entry.Value}' for key '{entry.Name}' could not be parsed as {typeof(TValue)}.");
                }
            }

            return result;
        }
        #endregion
    }

    public enum NoSqlValueType
    {
        String,
        List,
        /// <summary>
        /// 【自动去重】一个无序的字符串集合，不允许重复元素，支持集合间的操作如并集、交集和差集。
        /// </summary>
        Set,
        /// <summary>
        /// 【自动排序+去重】类似于集合，但每个元素都会关联一个分数（score），元素按分数排序。
        /// </summary>
        SortedSet,
        /// <summary>
        /// 哈希表，用于存储对象，每个对象由多个字段组成，每个字段存储一个键值对。
        /// </summary>
        Hash,
        /// <summary>
        /// 位图
        /// </summary>
        Bitmaps,
        /// <summary>
        /// 用于基数估计的算法，适用于统计独立元素的数量，比如网站的独立访客数。
        /// </summary>
        HyperLogLog,
        /// <summary>
        /// 地理空间索引:用于存储地理空间信息，比如地图上的位置信息。
        /// </summary>
        Geospatial
    }
}
