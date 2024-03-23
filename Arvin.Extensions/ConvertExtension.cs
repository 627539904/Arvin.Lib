using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Arvin.Extensions
{
    /// <summary>
    /// 转换扩展类
    /// </summary>
    public static class ConvertExtension
    {
        #region 数值转换 DefualtValue
        public static T ToStruct<T>(this string value, T defaultValue = default(T)) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            try
            {
                // 尝试使用 Convert.ChangeType 进行转换  
                // 这要求目标类型有一个合适的转换方法  
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                // 如果 Convert.ChangeType 失败，尝试使用 TypeConverter  
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter.CanConvertFrom(typeof(string)))
                {
                    return (T)converter.ConvertFromInvariantString(value);
                }
            }

            // 如果以上方法都失败，返回默认值  
            return defaultValue;
        }
        // sbyte 类型  
        public static sbyte ToSByte(this string value, sbyte def = 0)
        {
            if (sbyte.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out sbyte result))
            {
                return result;
            }
            return def;
        }

        // byte 类型  
        public static byte ToByte(this string value, byte def = 0)
        {
            if (byte.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out byte result))
            {
                return result;
            }
            return def;
        }

        // short (Int16) 类型  
        public static short ToInt16(this string value, short def = 0)
        {
            if (short.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out short result))
            {
                return result;
            }
            return def;
        }

        // ushort 类型  
        public static ushort ToUInt16(this string value, ushort def = 0)
        {
            if (ushort.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out ushort result))
            {
                return result;
            }
            return def;
        }

        // int (Int32) 类型  
        public static int ToInt32(this string value, int def = 0)
        {
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                return result;
            }
            return def;
        }

        // uint (UInt32) 类型  
        public static uint ToUInt32(this string value, uint def = 0)
        {
            if (uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint result))
            {
                return result;
            }
            return def;
        }

        // long (Int64) 类型  
        public static long ToInt64(this string value, long def = 0)
        {
            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long result))
            {
                return result;
            }
            return def;
        }

        // ulong (UInt64) 类型  
        public static ulong ToUInt64(this string value, ulong def = 0)
        {
            if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong result))
            {
                return result;
            }
            return def;
        }

        // float 类型  
        public static float ToSingle(this string value, float def = 0)
        {
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            return def;
        }
        public static float ToFloat(this string value, float def = 0)
        {
            return value.ToSingle(def);
        }

        // double 类型  
        public static double ToDouble(this string value, double def = 0)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            return def;
        }

        // decimal 类型  
        public static decimal ToDecimal(this string value, decimal def = 0)
        {
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }
            return def;
        }
        #endregion

        #region 枚举转换
        public static T ToEnum<T>(this int value) where T : struct, Enum
        {
            try
            {
                return (T)Enum.ToObject(typeof(T), value);
            }
            catch
            {
                return default;
            }
        }
        public static T ToEnum<T>(this string value, T def = default(T)) where T : struct, Enum
        {
            if (int.TryParse(value, out int intValue))
            {
                return intValue.ToEnum<T>();
            }

            if (Enum.TryParse<T>(value, true, out T result))
            {
                return result;
            }
            return def;
        }
        public static string GetDescription(this Enum value)
        {
            if (value == null)
                return null;

            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name == null)
                return null;

            var field = type.GetField(name);
            if (field == null)
                return null;

            var attr = field.GetCustomAttributes(false)
                             .OfType<DescriptionAttribute>()
                             .FirstOrDefault();
            return attr?.Description ?? value.ToString();
        }
        #endregion

        #region 转换 Convert/ToList/SelectTo
        public static List<T> ToListFromMany<T>(this List<List<T>> lists)
        {
            return lists.ConvertAllMany(p => p);
        }
        public static List<TOutput> ConvertAllMany<T, TOutput>(this IEnumerable<T> source, Func<T, List<TOutput>> selector, bool isIgnoreEx = true)
        {
            if (source.IsNullOrEmpty()) return new List<TOutput>();
            Func<T, List<TOutput>> selectorIgnoreEx = null;
            if (isIgnoreEx && selector != null)
                selectorIgnoreEx = p =>
                {
                    try
                    {
                        if (p == null)
                            return new List<TOutput>();
                        return selector(p) ?? new List<TOutput>();
                    }
                    catch
                    {
                        return default;
                    }
                };
            var resSelect = source.SelectMany(selectorIgnoreEx ?? selector);
            var res = source.SelectMany(selectorIgnoreEx ?? selector).ToList();
            return res;
        }
        public static List<TOutput> ConvertAllIgnoreNull<T, TOutput>(this List<T> source, Converter<T, TOutput> convert)
        {
            if (source == null) return new List<TOutput>();
            source = source.FilterNull();
            return source.ConvertAll(convert).FilterNull();
        }
        public static List<ResultT> ConvertToList<T, ResultT>(this List<T> source)
            where T : ResultT
        {
            if (source.IsNullOrEmpty()) return new List<ResultT>();
            return source.ConvertAll(p => (ResultT)p);
        }
        public static List<T> ItemToList<T>(this T model)
        {
            return new List<T> { model };
        }
        #endregion


    }
}