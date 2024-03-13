using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arvin.Helpers;
using Microsoft.ML;

namespace Arvin.AITest.Guide.ML_NET.API.GitHubIssueClassification
{
    internal class Program_GitHubIssueClassification
    {
        static string _appPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) ?? ".";
        static string _trainDataPath = Path.Combine(_appPath, "..", "..", "..", "Data", "issues_train.tsv");//定型模型的数据集路径
        static string _testDataPath = Path.Combine(_appPath, "..", "..", "..", "Data", "issues_test.tsv");//评估模型的数据集路径
        static string _modelPath = Path.Combine(_appPath, "..", "..", "..", "Models", "model.zip");//保存定型模型的路径

        static MLContext _mlContext;
        static ITransformer _trainedModel;
        static PredictionEngine<GitHubIssue, IssuePrediction> _predEngine;//单个预测
        public static void Test()
        {
            IDataView _trainingDataView;//处理数据集

            //使用具有随机种子 (seed: 0) 的新实例 MLContext 初始化 _mlContext 全局变量,获得跨多个定型的可重复/确定性结果
            _mlContext = new MLContext(seed: 0);

            _trainingDataView = _mlContext.Data.LoadFromTextFile<GitHubIssue>(_trainDataPath, hasHeader: true);//LoadFromTextFile() 用于定义数据架构并读取文件
            var pipeline = ProcessData();
            var trainingPipeline = BuildAndTrainModel(_trainingDataView, pipeline);
            Evaluate(_trainingDataView.Schema);
            PredictIssue();
        }

        /// <summary>
        /// 数据处理：
        /// 提取功能和转换数据
        /// 处理预处理/特征化
        /// </summary>
        /// <returns></returns>
        static IEstimator<ITransformer> ProcessData()
        {
            //提取并转换数据
            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Area", outputColumnName: "Label") //将 Area 列转换为数字键类型 Label 列(分类算法所接受的格式）
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Title", outputColumnName: "TitleFeaturized")) //Title特征化
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Description", outputColumnName: "DescriptionFeaturized")) //Description特征化
                .Append(_mlContext.Transforms.Concatenate("Features", "TitleFeaturized", "DescriptionFeaturized")) //将所有特征列合并到“特征”列. 默认情况下，学习算法仅处理“特征”列的特征
                .AppendCacheCheckpoint(_mlContext);//缓存数据视图，以便在使用缓存多次循环访问数据时获得更好的性能【用于小中型数据，大型数据集不使用】
            //返回处理管道
            return pipeline;
        }

        /// <summary>
        /// 生成和定型模型
        /// </summary>
        /// <param name="trainingDataView"></param>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        static IEstimator<ITransformer> BuildAndTrainModel(IDataView trainingDataView, IEstimator<ITransformer> pipeline)
        {
            //创建定型算法类
            var trainingPipeline = pipeline.Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features")) //SdcaMaximumEntropy:多类分类训练算法
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));//追加到 pipeline 并接受特征化的 Title 和 Description (Features) 以及 Label 输入参数，以便从历史数据中学习。
            //定型模型
            _trainedModel = trainingPipeline.Fit(trainingDataView);//适应数据
            //根据定型数据预测区域
            _predEngine = _mlContext.Model.CreatePredictionEngine<GitHubIssue, IssuePrediction>(_trainedModel);
            GitHubIssue issue = new GitHubIssue()
            {
                Title = "WebSockets communication is slow in my machine",
                Description = "The WebSockets communication used under the covers by SignalR looks like is going slow in my development machine.."
            };
            var prediction = _predEngine.Predict(issue);//对单行数据进行预测
            LogHelper.Info($"=============== 单一预测训练结果: {prediction.Area} ===============");
            //返回模型
            return trainingPipeline;
        }

        /// <summary>
        /// 评估模型
        /// </summary>
        /// <param name="trainingDataViewSchema"></param>
        static void Evaluate(DataViewSchema trainingDataViewSchema)
        {
            //加载测试数据集
            var testDataView = _mlContext.Data.LoadFromTextFile<GitHubIssue>(_testDataPath, hasHeader: true);//返回MulticlassClassificationMetrics,包含由多类分类计算器计算出的总体指标
            //创建多类评估程序
            var testMetrics = _mlContext.MulticlassClassification.Evaluate(_trainedModel.Transform(testDataView));
            //评估模型并创建指标
            //显示指标
            LogHelper.Info($"*************************************************************************************************************");
            LogHelper.Info($"*       多类分类模型度量指标-测试数据     ");
            LogHelper.Info($"*------------------------------------------------------------------------------------------------------------");
            LogHelper.Info($"*       微观准确性:  {testMetrics.MicroAccuracy:0.###}");
            LogHelper.Info($"*       宏观准确性:  {testMetrics.MacroAccuracy:0.###}");
            LogHelper.Info($"*       对数损失:    {testMetrics.LogLoss:0.###}");
            LogHelper.Info($"*       对数损失减小:{testMetrics.LogLossReduction:0.###}");
            LogHelper.Info($"*************************************************************************************************************");
            //将模型保存到文件
            SaveModelAsFile(_mlContext, trainingDataViewSchema, _trainedModel);
        }

        /// <summary>
        /// 训练后的模型进行序列化并将其存储为 zip 文件
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="trainingDataViewSchema"></param>
        /// <param name="model"></param>
        static void SaveModelAsFile(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            mlContext.Model.Save(model, trainingDataViewSchema, _modelPath);
        }

        /// <summary>
        /// 使用模型进行部署和预测
        /// </summary>
        static void PredictIssue()
        {
            //加载已保存的模型
            ITransformer loadedModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);
            //创建测试数据的单个问题
            GitHubIssue singleIssue = new GitHubIssue() { Title = "Entity Framework crashes", Description = "When connecting to the database, EF is crashing" };
            //根据测试数据预测区域
            _predEngine = _mlContext.Model.CreatePredictionEngine<GitHubIssue, IssuePrediction>(loadedModel);
            var prediction = _predEngine.Predict(singleIssue);
            //结合测试数据和预测进行报告
            //显示预测结果
            LogHelper.Info($"=============== 单词预测结果: {prediction.Area} ===============");
        }
    }
}
