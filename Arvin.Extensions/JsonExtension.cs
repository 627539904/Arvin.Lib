﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
        // 序列化对象到JSON字符串的扩展方法  
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static string SerializeObject(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
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
            return JsonConvert.SerializeObject(model);
        }
        #endregion
    }
}
