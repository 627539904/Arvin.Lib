using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace Arvin.AITest.Guide.ML_NET.API.SentimentAnalysis
{
    /// <summary>
    /// 数据集类
    /// </summary>
    public class SentimentData
    {
        /// <summary>
        /// 分析文本：用户评论
        /// </summary>
        [LoadColumn(0)]
        public string? SentimentText;

        /// <summary>
        /// 标记预测值：情绪值， 1（正面）或 0（负面）
        /// </summary>
        [LoadColumn(1), ColumnName("Label")]
        public bool Sentiment;
    }

    /// <summary>
    /// 输出类：训练后使用的预测类
    /// </summary>
    public class SentimentPrediction : SentimentData
    {
        /// <summary>
        /// 模型预测值
        /// </summary>
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
        /// <summary>
        /// 校准到具有积极情绪的文本几率的分数
        /// </summary>
        public float Probability { get; set; }
        /// <summary>
        /// 模型计算得出的原始分数
        /// </summary>
        public float Score { get; set; }
    }
}
