using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arvin.Helpers;
using Microsoft.ML;
using Microsoft.ML.Data;
using static Microsoft.ML.DataOperationsCatalog;

namespace Arvin.AITest.Guide.ML_NET.API.SentimentAnalysis
{
    public class Program_SentimentAnalysis
    {
        static string _dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "yelp_labelled.txt");
        public static void Test()
        {
            MLContext mlContext = new MLContext();
            TrainTestData splitDataView = LoadData(mlContext);
            ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
            Evaluate(mlContext, model, splitDataView.TestSet);
            UseModelWithSingleItem(mlContext, model);
            UseModelWithBatchItems(mlContext, model);
        }
        /// <summary>
        /// 拆分数据集以进行模型训练和测试
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        static TrainTestData LoadData(MLContext mlContext)
        {
            //加载数据
            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: false);//定义数据架构并读取文件

            //将加载的数据集拆分为训练数据集和测试数据集
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);//拆分为训练数据集和测试数据集.testFraction:指定数据的测试集百分比,默认值为 10%.

            //返回拆分的训练数据集和测试数据集
            return splitDataView;
        }
        /// <summary>
        /// 生成和定型模型
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="splitTrainSet"></param>
        /// <returns></returns>
        static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            //提取并转换数据
            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))//将文本列 (SentimentText) 特征化为（Features）
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features")); //SdcaLogisticRegressionBinaryTrainer:分类算法-二元分类
            //定型模型
            LogHelper.Info("=============== 创建并训练模型 ===============");
            var model = estimator.Fit(splitTrainSet);//转换数据集并进行模型训练
            LogHelper.Info("=============== 结束训练 ===============");
            LogHelper.Info();
            //根据测试数据预测情绪
            //返回模型
            return model;
        }

        /// <summary>
        /// 评估模型
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="model"></param>
        /// <param name="splitTestSet"></param>
        static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            //加载测试数据集
            LogHelper.Info("=============== 用测试数据评估模型的准确性 ===============");
            IDataView predictions = model.Transform(splitTestSet);
            //创建 BinaryClassification 计算器
            //评估模型并创建指标
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");//评估：将预测值与测试数据集的实际Label进行比较，返回模型执行情况
            //显示指标
            LogHelper.Info();
            LogHelper.Info("模型质量指标评估");
            LogHelper.Info("--------------------------------");
            LogHelper.Info($"准确性: {metrics.Accuracy:P2}");
            LogHelper.Info($"可信度: {metrics.AreaUnderRocCurve:P2}");//指示模型对正面类和负面类进行正确分类的置信度。 应该使 AreaUnderRocCurve 尽可能接近 1。
            LogHelper.Info($"F1Score: {metrics.F1Score:P2}");//获取模型的 F1 分数，该分数是查准率和查全率之间的平衡关系的度量值。 应该使 F1Score 尽可能接近 1
            LogHelper.Info("=============== 评估完成 ===============");
        }

        /// <summary>
        /// 预测测试数据结果
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="model"></param>
        static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
        {
            //创建测试数据的单个注释
            //PredictionEngine:一个简便 API，可使用它对单个数据实例执行预测.不是线程安全,生产环境应使用PredictionEnginePool服务
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            //根据测试数据预测情绪
            SentimentData sampleStatement = new SentimentData
            {
                SentimentText = "This was a very bad steak"
            };
            //结合测试数据和预测进行报告
            var resultPrediction = predictionFunction.Predict(sampleStatement);//对单行数据进行预测
            //显示预测结果
            LogHelper.Info();
            LogHelper.Info("=============== 用单行文本和测试数据集进行预测测试 ===============");

            LogHelper.Info();
            LogHelper.Info($"测试文本: {resultPrediction.SentimentText} | 预测: {(Convert.ToBoolean(resultPrediction.Prediction) ? "积极" : "消极")} | 积极性: {resultPrediction.Probability} ");

            LogHelper.Info("=============== 结束预测 ===============");
            LogHelper.Info();
        }

        /// <summary>
        /// 部署和预测批项目
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="model"></param>
        static void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
        {
            //创建批处理测试数据
            IEnumerable<SentimentData> sentiments = new[]
            {
                new SentimentData{SentimentText = "This was a horrible meal"},
                new SentimentData{SentimentText = "I love this spaghetti."}
            };
            //根据测试数据预测情绪
            IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);
            IDataView predictions = model.Transform(batchComments);
            // 使用模型预测评论数据是积极（1）还是消极（0）。
            IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);
            //结合测试数据和预测进行报告
            //显示预测结果
            LogHelper.Info();

            LogHelper.Info("=============== 加载多样本模型的预测测试 ===============");
            foreach (SentimentPrediction prediction in predictedResults)
            {
                LogHelper.Info($"测试文本: {prediction.SentimentText} | 预测: {(Convert.ToBoolean(prediction.Prediction) ? "积极" : "消极")} | 积极性: {prediction.Probability} ");
            }
            LogHelper.Info("=============== 结束预测 ===============");
        }
    }
}
