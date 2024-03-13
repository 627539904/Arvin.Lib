using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arvin.AITest.Guide.ML_NET.API.TextClassificationTF
{
    public class Config_TCTF
    {
        /// <summary>
        /// 已训练模型的文件路径
        /// </summary>
        public static string _modelPath = Path.Combine(Environment.CurrentDirectory, "sentiment_model");
    }
}
