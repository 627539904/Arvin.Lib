using StackExchange.Redis;
using System;
using System.Collections.Generic;
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
        // 连接到本地的Redis服务器（默认端口是6379）
        static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("your_redis_host:6379,password=your_password"); //远程
        public RedisHelper()
        {

        }
        public int DbIndex { get; set; } = -1;
        public RedisHelper(int dbIndex)
        {
            DbIndex = dbIndex;
        }
        public IDatabase GetDB()
        {
            // 获取数据库实例（默认是db0）
            if (DbIndex > 0)
            {
                return redis.GetDatabase(DbIndex);
            }
            return redis.GetDatabase();
        }
        // Select
        public async Task<string> GetStringAsync(string key)
        {
            var db = GetDB();
            var value = await db.StringGetAsync(key).ConfigureAwait(false);
            return value;
        }

        //Add Or Update
        public async Task SetAsync(string key, string value)
        {
            var db = GetDB();
            await db.StringSetAsync(key, value).ConfigureAwait(false);
        }

        public async Task<bool> DeleteKeyAsync(string key)
        {
            var db = GetDB();
            return await db.KeyDeleteAsync(key).ConfigureAwait(false);
        }


        public void Close()
        {
            redis.Close();
        }
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
