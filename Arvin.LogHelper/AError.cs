using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arvin.Extensions;

namespace Arvin.LogHelper
{
    /// <summary>
    /// 错误类,用于记录错误信息，并进行错误处理
    /// </summary>
    public class AError
    {
        public static string CmdHandle(string errorMsg)
        {
            if (errorMsg.Contains("ModuleNotFoundError"))
            {
                string pattern = @"ModuleNotFoundError: No module named '([^']+)'";
                string moduleName=errorMsg.Match(pattern).Groups[1].Value;
                if (moduleName == "cv2")
                {
                    moduleName = "opencv-python";//用于图像处理和计算机视觉任务
                }
                string res= $"pip install {moduleName}";
                ALog.WriteLine($"错误处理:{res}");
                return res;
            }
            return "";
        }
    }
}
