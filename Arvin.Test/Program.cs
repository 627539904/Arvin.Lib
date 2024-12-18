// See https://aka.ms/new-console-template for more information
using Arvin.Helpers;
using System;
using System.Threading.Tasks;


// 目标网站的URL  
string url = "https://www.google.com.hk/";

HttpHelper.GetAsync(url);

// 等待用户按键，防止控制台窗口立即关闭  
Console.ReadLine();

