using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace Arvin.AITest.Guide.ML_NET.API.GitHubIssueClassification
{
    /// <summary>
    /// 输入数据集类
    /// </summary>
    public class GitHubIssue
    {
        /// <summary>
        /// GitHub 问题 ID
        /// </summary>
        [LoadColumn(0)]
        public string? ID { get; set; }
        /// <summary>
        /// 定型预测
        /// </summary>
        [LoadColumn(1)]
        public string? Area { get; set; }
        /// <summary>
        /// （GitHub 问题标题）是用于预测 Area 的第一个 feature
        /// </summary>
        [LoadColumn(2)]
        public required string Title { get; set; }
        /// <summary>
        /// 用于预测 Area 的第二个 feature
        /// </summary>
        [LoadColumn(3)]
        public required string Description { get; set; }
    }
    /// <summary>
    /// 定型模型后用于预测的类
    /// </summary>
    public class IssuePrediction
    {
        /// <summary>
        /// PredictedLabel 在预测和评估过程中使用
        /// </summary>
        [ColumnName("PredictedLabel")]
        public string? Area;
    }
}
