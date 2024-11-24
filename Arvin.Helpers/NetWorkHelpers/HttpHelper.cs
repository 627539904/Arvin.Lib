using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
                ALog.Info(content);
                return content;
            }
            catch (HttpRequestException e)
            {
                // 处理请求异常  
                ALog.Info("\nException Caught!");
                ALog.Info($"Message :{e.Message} ");
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
                ALog.Info("\nException Caught!");
                ALog.Info($"Message :{e.Message} ");
                return null;
            }
        }


        #region Old
        public static Dictionary<ContentType, string> dicContentType = new Dictionary<ContentType, string>
        {
            {ContentType.Json,"application/json;charset=UTF-8"},
            {ContentType.Xml,"application/xml;charset=UTF-8" }
        };

        #region GetHttpWebRequest
        public static HttpWebRequest GetHttpWebRequest(string url)
        {
            return (HttpWebRequest)WebRequest.Create(url);
        }

        public static HttpWebRequest GetHttpWebRequest(string url, ContentType contentType, int timeout = 30000)
        {
            return GetHttpWebRequest(url, dicContentType[contentType], timeout);
        }

        public static string LoadSource(string url)
        {
            WebClient client = new WebClient();
            client.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = client.DownloadData(url); //从指定网站下载数据
            string pageHtml = Encoding.UTF8.GetString(pageData);
            return pageHtml;
        }

        public static HttpWebRequest GetHttpWebRequest(string url, string contentType, int timeout = 30000)
        {
            HttpWebRequest httpWebRequest = GetHttpWebRequest(url);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Timeout = timeout;
            return httpWebRequest;
        }
        #endregion GetHttpWebRequest

        #region HttpPost
        public static string HttpPostToXml(string url, string body)
        {
            HttpWebRequest httpWebRequest = GetHttpWebRequest(url, ContentType.Xml);
            return HttpPost(httpWebRequest, body);
        }

        public static string HttpPostToJson(string url, string body)
        {
            HttpWebRequest httpWebRequest = GetHttpWebRequest(url, ContentType.Json);
            return HttpPost(httpWebRequest, body);
        }
        public static string HttpPost(string url, string body)
        {
            HttpWebRequest httpWebRequest = GetHttpWebRequest(url);
            return HttpMethod(httpWebRequest, "POST", body);
        }

        public static string HttpPost(HttpWebRequest httpWebRequest, string body)
        {
            return HttpMethod(httpWebRequest, "POST", body);
        }
        public static string HttpPost(string url, string body, WebHeaderCollection collection, ContentType type,
            string accept = "application/json, text/plain, */*", int timeOut = 60000)
        {
            HttpWebRequest httpWebRequest = GetHttpWebRequest(url, type, timeOut);
            // 请求头的设置
            httpWebRequest.Headers = collection;
            httpWebRequest.Accept = accept;
            return HttpPost(httpWebRequest, body);
        }
        #endregion HttpPost

        #region HttpGet

        public static string HttpGet(string url)
        {
            HttpWebRequest httpWebRequest = GetHttpWebRequest(url);
            return HttpGet(httpWebRequest);
        }

        public static string HttpGet(HttpWebRequest httpWebRequest)
        {
            return HttpMethod(httpWebRequest, "GET", "");
        }
        #endregion HttpGet

        public static string HttpMethod(HttpWebRequest httpWebRequest, string methodName, string body)
        {
            try
            {
                httpWebRequest.Method = methodName;

                httpWebRequest.Method = methodName;
                if (methodName.Equals("POST", StringComparison.OrdinalIgnoreCase)
                    || methodName.Equals("PUT", StringComparison.OrdinalIgnoreCase)
                    || methodName.Equals("PATCH", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(body);
                    httpWebRequest.ContentLength = bytes.Length;
                    httpWebRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
                }

                string responseContent = "";
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        responseContent = streamReader.ReadToEnd();
                    }
                    httpWebRequest.Abort();
                }
                return responseContent;
            }
            catch (Exception ex)
            {
                return ex.Message + ex.InnerException + ex.StackTrace;
            }
        }
        #endregion
    }

    /// <summary>
    /// 内容类型/媒体类型
    /// </summary>
    public enum ContentType
    {
        Xml,
        Json,
        Text,
        Image,
        Video,
        Audio,
        Html
    }
}
