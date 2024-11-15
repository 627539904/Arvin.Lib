using Arvin.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Arvin.Helpers.ToolHelpers
{
    public static class WechartHelper
    {
        public readonly static HttpClient client = new HttpClient();
        public static WxEntity GetAccessToken(string appid, string appSecret)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appid}&secret={appSecret}";
            var result = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result.ToModel<WxEntity>();
            if (!result.errcode.IsNullOrWhiteSpace())//请求access_token出错时计入日志
                ALog.Info($"【GetAccessToken】errcode:{result.errcode} errmsg:{result.errmsg}");
            return result;
        }

        public static string GetMenu(string token)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/get_current_selfmenu_info?access_token={token}";
            var result = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            return result;
        }

        public static ReturnEntity AddMenu(string token, string menuJson)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/menu/create?access_token={token}";
            var content = new StringContent(menuJson, UTF8Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result.ToModel<ReturnEntity>();
            if (result.errcode != "0")
                ALog.Info($"【AddMenu】errcode:{result.errcode} errmsg:{result.errmsg}");
            return result;
        }

        public static ReturnEntity AddMenu_File(string token, string filePath = "./wechartMenu.json")
        {
            var menuJson = "";
            using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
            {
                menuJson = sr.ReadToEnd();
            }
            return AddMenu(token, menuJson);
        }

        public static ReturnEntity DelMenu(string token)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={token}";
            var result = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result.ToModel<ReturnEntity>();
            if (result.errcode != "0")
                ALog.Info($"【DelMenu】errcode:{result.errcode} errmsg:{result.errmsg}");
            return result;
        }

#if NETSTANDARD2_0_OR_GREATER||NET6_0_OR_GREATER
        //执行该认证Uri后，如果成功，redirect_uri后会追加code参数，故code可从web端获取
        public static string GetCodeAuthUri(string appId, string redirect_uri, string state = "123")
        {
            string url = $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={appId}&redirect_uri={HttpUtility.UrlEncode(redirect_uri)}&response_type=code&scope=snsapi_base&state={state}#wechat_redirect";
            return url;
        }
#endif

        public static string GetAccessToken_Web(string appId, string appSecret, string code)
        {
            throw new NotImplementedException();
        }
    }

    public class ReturnEntity
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
    }

    public class WxEntity : ReturnEntity
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}
