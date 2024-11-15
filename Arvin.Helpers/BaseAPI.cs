using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arvin.Helpers
{
    public class BaseAPI
    {
        protected readonly HttpClient _httpClient;

        protected string MediaType { get; set; } = "application/json";
        /// <summary>
        /// 初始化API
        /// </summary>
        /// <param name="baseUrl">类似http://localhost:11434</param>
        /// <param name="mediaType"></param>
        public BaseAPI(string baseUrl, string mediaType = "application/json")
        {
            this.MediaType = mediaType;
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
        }
        public BaseAPI() { }

        public async Task<bool> HttpGetAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // 发送GET请求  
                    HttpResponseMessage response = await client.GetAsync(url);

                    // 检查响应状态码  
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        // 如果API调用失败，则打印错误信息  
                        Console.WriteLine($"API调用失败，状态码: {response.StatusCode}");
                    }

                    // 读取响应内容（如果需要的话）  
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                    return false;
                }
            }
            catch (HttpRequestException e)
            {
                // 处理请求异常  
                Console.WriteLine($"\n异常捕获: {e.Message}");
                return false;
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _httpClient.GetAsync(url);
        }

        public async Task<string> HttpGetStringAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // 发送GET请求  
                    HttpResponseMessage response = await client.GetAsync(url);

                    // 检查响应状态码  
                    if (response.IsSuccessStatusCode)
                    {
                        // 读取响应内容（如果需要的话）
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseBody);
                        return responseBody;
                    }
                    else
                    {
                        // 如果API调用失败，则打印错误信息  
                        Console.WriteLine($"API调用失败，状态码: {response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                // 处理请求异常  
                Console.WriteLine($"\n异常捕获: {e.Message}");
                return null;
            }
        }

        HttpContent GetDefaultHttpContent(object requestBody)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore // 忽略值为null的属性
            };
            string requestStr = JsonConvert.SerializeObject(requestBody, settings);
            HttpContent content = new StringContent(requestStr, Encoding.UTF8, this.MediaType);
            return content;
        }

        public async Task<HttpResponseMessage> PostAsyncByEndPoint(string endpoint, object requestBody,bool isStream=false)
        {
            //var settings = new JsonSerializerSettings
            //{
            //    NullValueHandling = NullValueHandling.Ignore // 忽略值为null的属性
            //};
            //string requestStr = JsonConvert.SerializeObject(requestBody, settings);
            //HttpContent content = new StringContent(requestStr, Encoding.UTF8, this.MediaType);
            HttpContent content = GetDefaultHttpContent(requestBody);
            HttpResponseMessage response = null;
            if (isStream)
            {
                //流式传输，用于大文件传输，不用等待传输完成
                var message = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };
                response = await _httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
            }
            else
                response= await _httpClient.PostAsync(endpoint, content);
            return response;
        }

        public async Task<HttpResponseMessage> HeadAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
            return response;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url, object requestBody=null)
        {
            HttpResponseMessage response;
            if (requestBody == null)
            {
                response = await _httpClient.DeleteAsync(url);
                return response;
            }
            else
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, url)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
                };

                response = await _httpClient.SendAsync(request);
            }
            return response;
        }
    }
}
