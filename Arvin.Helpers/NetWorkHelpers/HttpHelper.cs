using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.Helpers
{
    public class HttpHelper
    {
        // HttpClient是线程安全的，所以通常建议作为静态字段复用  
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// 发送GET请求到指定URL，并返回响应的字符串内容
        /// </summary>
        /// <param name="url"></param>
        /// <param name="readAsString">true-使用url指定字符集读取,false-忽略字符集，使用UTF8字节流读取</param>
        public async static Task<string> GetAsync(string url, bool readAsString = false)
        {
            // 创建一个HttpClient实例  
            try
            {
                // 发送GET请求  
                HttpResponseMessage response = await httpClient.GetAsync(url);

                // 确保请求成功  
                response.EnsureSuccessStatusCode();

                // 读取响应内容  
                string content = "";
                if (readAsString)
                    content = await response.Content.ReadAsStringAsync();
                else
                {
                    // 使用UTF-8读取内容，忽略Content-Type头部中的字符集  
                    var byteArray = await response.Content.ReadAsByteArrayAsync();
                    content = Encoding.UTF8.GetString(byteArray);
                }
                // 输出网页内容到控制台  
                LogHelper.Info(content);
                return content;
            }
            catch (HttpRequestException e)
            {
                // 处理请求异常  
                LogHelper.Info("\nException Caught!");
                LogHelper.Info($"Message :{e.Message} ");
                return null;
            }
        }

        /// <summary>  
        /// 发送POST请求到指定URL，并发送JSON格式的请求体，返回响应的字符串内容  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="jsonContent">要发送的JSON格式的请求体</param>  
        /// <returns>响应的字符串内容</returns>  
        public static async Task<string> PostAsync(string url, string jsonContent)
        {
            try
            {
                // 将JSON内容转换为字节数组  
                byte[] contentBytes = Encoding.UTF8.GetBytes(jsonContent);

                // 创建一个HttpContent对象，设置其内容类型和内容  
                using (var content = new ByteArrayContent(contentBytes))
                {
                    // 设置请求体的内容类型为JSON  
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    // 发送POST请求到指定的URL，并附带请求体  
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    // 确保请求成功  
                    response.EnsureSuccessStatusCode();

                    // 读取响应的内容为字符串  
                    string contentString = await response.Content.ReadAsStringAsync();

                    // 返回响应的字符串内容  
                    return contentString;
                }
            }
            catch (HttpRequestException e)
            {
                // 处理请求异常  
                LogHelper.Info("\nException Caught!");
                LogHelper.Info($"Message :{e.Message} ");
                return null;
            }
        }
    }
}
