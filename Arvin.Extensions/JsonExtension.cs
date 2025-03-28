﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;

namespace Arvin.Extensions
{
    /// <summary>
    /// Json相关操作
    /// </summary>
    public static class JsonExtension
    {
        #region JOject相关操作
        public static JObject ToJObject(this object json)
        {
            return JObject.Parse(json.ToString());
        }

        public static JObject ToJObject(this string json)
        {
            return JObject.Parse(json);
        }

        public static JArray ToJArray(this string json)
        {
            return JArray.Parse(json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="nodePath">英文右斜线分割，如 nodeName1/nodeName2/...</param>
        /// <returns></returns>
        public static JToken ToJToken(this JObject obj, string nodePath)
        {
            string[] strs = nodePath.Split('/');
            JToken token = obj;
            foreach (var nodeName in strs)
            {
                token = token[nodeName];
            }
            return token;
        }

        public static JToken ToJToken(this string json, string nodePath)
        {
            return json.ToJObject().ToJToken(nodePath);
        }

        public static List<T> ToList<T>(this JToken token)
        {
            return token.ToString().ToList<T>();
        }

        public static T ToModel<T>(this JToken token)
        {
            return token.ToString().ToModel<T>();
        }
        public static JObject AddJsonNode(this JObject obj, JProperty jProperty)
        {
            obj.Add(jProperty);
            return obj;
        }

        public static JObject AddJsonNode(this JToken token, JProperty jProperty)
        {
            return token.ToJObject().AddJsonNode(jProperty);
        }
        #endregion

        #region Json序列化
        public static JsonSerializerSettings IgnoreNull(this JsonSerializerSettings settings)
        {
            if (settings == null)
                settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;// 忽略值为null的属性
            return settings;
        }
        /// <summary>
        /// 格式化json字符串
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static JsonSerializerSettings Format(this JsonSerializerSettings settings)
        {
            if (settings == null)
                settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            return settings;
        }
        // 序列化对象到JSON字符串的扩展方法  
        public static string ToJson(this object obj, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }
        /// <summary>
        /// 输出模式，忽略null并格式化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonForPrint(this object obj)
        {
            return ToJson(obj, new JsonSerializerSettings().IgnoreNull().Format());
        }
        public static string ToJsonIgnoreNull(this object obj)
        {
            return SerializeObject(obj, true);
        }
        public static string SerializeObject(this object obj, bool isIgnoreNull = false)
        {
            JsonSerializerSettings settings = null;
            if (isIgnoreNull)
                settings = settings.IgnoreNull();
            return JsonConvert.SerializeObject(obj, settings);
            //if (isIgnoreNull)
            //    return JsonConvert.SerializeObject(obj, settings);
            //else
            //    return JsonConvert.SerializeObject(obj);
        }

        // 从JSON字符串反序列化到指定类型的对象的扩展方法  
        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static T DeserializeObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static List<T> ToList<T>(this string json)
        {
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        public static T ToModel<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToJsonString(this object model)
        {
            return SerializeObject(model);
        }
        #endregion

        #region Parser
        public static dynamic TryParseJson(this string json, Func<JProperty, object> customRule = null)
        {
            try
            {
                JToken root = JToken.Parse(json);
                return ParseToken(root, customRule);
            }
            catch
            {
                return null; // 解析失败返回 null ,说明字符串不是json格式
            }
        }

        private static dynamic ParseToken(this JToken token, Func<JProperty, object> rule)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    var expando = new ExpandoObject();
                    var dict = expando as IDictionary<string, object>;
                    foreach (JProperty prop in token)
                    {
                        // 应用自定义规则（如字段名转换）
                        object value = rule != null ? rule(prop) : ParseToken(prop.Value, rule);
                        dict[prop.Name] = value ?? ParseToken(prop.Value, rule); // 规则未处理则递归解析
                    }
                    return expando;
                case JTokenType.Array:
                    var list = new List<dynamic>();
                    foreach (JToken item in token)
                        list.Add(ParseToken(item, rule));
                    return list;
                case JTokenType.String:
                    return token.Value<string>();
                case JTokenType.Integer:
                    return token.Value<long>();
                case JTokenType.Float:
                    return token.Value<double>();
                case JTokenType.Boolean:
                    return token.Value<bool>();
                default:
                    return token.ToString();
            }
        }
        #endregion
    }
}
