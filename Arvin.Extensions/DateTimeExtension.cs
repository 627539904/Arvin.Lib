using System;
using System.Collections.Generic;
using System.Text;

namespace Arvin.Extensions
{
    public static class DateTimeExtension
    {
        //北京时间为UTC+8:00
        //Unix时间纪元为1970-01-01
        //时间戳=(当前UTC时间-Unix时间纪元)的秒数或毫秒数
        #region 时间戳
        public static long ToTimeStamp(this DateTime dt, TimeStampMode level = TimeStampMode.s)
        {
            long res = 0;
            var ts = dt.ToUniversalTime() - new DateTime(1970, 1, 1);
            switch (level)
            {
                case TimeStampMode.s:
                    res = (long)ts.TotalSeconds;
                    break;
                case TimeStampMode.ms:
                    res = (long)ts.TotalMilliseconds;
                    break;
            }
            return res;
        }

        public static DateTime ToLocalDateTime(this long timeStamp, TimeStampMode level = TimeStampMode.s)
        {
            DateTime res = timeStamp.ToUTCDateTime(level).AddHours(8);
            return res;
        }

        public static DateTime ToUTCDateTime(this long timeStamp, TimeStampMode level = TimeStampMode.s)
        {
            DateTime res = new DateTime(1970, 1, 1);
            switch (level)
            {
                case TimeStampMode.s:
                    res = res.AddSeconds(timeStamp);
                    break;
                case TimeStampMode.ms:
                    res = res.AddMilliseconds(timeStamp);
                    break;
            }
            return res;
        }
        #endregion 时间戳

        #region for string
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToDateString(this DateTime dt)
        {
            return ToDateTimeString(dt, DateTimeFormat.StandardDate);
        }

        /// <summary>
        /// HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToTimeString(this DateTime dt)
        {
            return ToDateTimeString(dt, DateTimeFormat.StandardTime);
        }
        /// <summary>
        /// 默认yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime dt, DateTimeFormat format = DateTimeFormat.StandardDateTime)
        {
            string result;
            switch (format)
            {
                case DateTimeFormat.StandardDate:
                    result = dt.ToString("yyyy-MM-dd");
                    break;
                case DateTimeFormat.StandardTime:
                    result = dt.ToString("HH:mm:ss");
                    break;
                case DateTimeFormat.StandardDateTime2:
                    result = dt.ToString("yyyy-MM-dd HH:mm:ss fff");
                    break;
                case DateTimeFormat.StandardDateTime:
                default:
                    result = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    break;
            }
            return result;
        }

        /// <summary>
        /// 将yyyyMMddHHmmss转换为yyyy-MM-dd HH:mm:ss,一般用于订单号等的逆处理
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startSubIndex"></param>
        /// <returns></returns>
        public static string SubDataTimeFromString(this string s, int startSubIndex = 0)
        {
            return $"{s.Substring(startSubIndex, 4)}-{s.Substring(startSubIndex + 4, 2)}-{s.Substring(startSubIndex + 6, 2)} {s.Substring(startSubIndex + 8, 2)}:{s.Substring(startSubIndex + 10, 2)}:{s.Substring(startSubIndex + 12, 2)}";
        }
        /// <summary>
        /// 将yyyyMMdd转换为yyyy-MM-dd,一般用于订单号等的逆处理
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startSubIndex"></param>
        /// <returns></returns>
        public static string SubDataFromString(this string s, int startSubIndex = 0)
        {
            return $"{s.Substring(startSubIndex, 4)}-{s.Substring(startSubIndex + 4, 2)}-{s.Substring(startSubIndex + 6, 2)}";
        }
        /// <summary>
        /// 将HHmmss转换为HH:mm:ss,一般用于订单号等的逆处理
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startSubIndex"></param>
        /// <returns></returns>
        public static string SubTimeFromString(this string s, int startSubIndex = 0)
        {
            return $"{s.Substring(startSubIndex, 2)}:{s.Substring(startSubIndex + 2, 2)}:{s.Substring(startSubIndex + 4, 2)}";
        }
        #endregion
    }

    /// <summary>
    /// 时间戳模式
    /// </summary>
    public enum TimeStampMode
    {
        s,
        ms
    }

    public enum DateTimeFormat
    {
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        StandardDate,
        /// <summary>
        /// HH:mm:ss
        /// </summary>
        StandardTime,
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        StandardDateTime,
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss fff
        /// </summary>
        StandardDateTime2
    }
}
