using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arvin.Helpers;
using Microsoft.ML;

namespace Arvin.AITest.Guide.ML_NET.API.TaxiFarePrediction
{
    public class Program_TaxiFarePrediction
    {
        static string _trainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-train.csv");
        static string _testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "taxi-fare-test.csv");
        static string _modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "Model.zip");
        public static void Test()
        {
            MLContext mlContext = new MLContext(seed: 0);
            var model = Train(mlContext, _trainDataPath);
            Evaluate(mlContext, model);
            TestSinglePrediction(mlContext, model);
        }

        static ITransformer Train(MLContext mlContext, string dataPath)
        {
            //加载数据
            IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(dataPath, hasHeader: true, separatorChar: ',');
            //提取并转换数据
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "FareAmount") //将输入类型的FareAmount的值复制到输出类型的Label
                //字段转换为数字，OneHotEncodingTransformer 转换类（它将不同的数字键值分配到每列的不同值）
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "VendorIdEncoded", inputColumnName: "VendorId"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "RateCodeEncoded", inputColumnName: "RateCode"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PaymentTypeEncoded", inputColumnName: "PaymentType"))
                //Concatenate 转换类将所有功能列合并到“Features”列
                .Append(mlContext.Transforms.Concatenate("Features", "VendorIdEncoded", "RateCodeEncoded", "PassengerCount", "TripDistance", "PaymentTypeEncoded"))
                //选择算法
                .Append(mlContext.Regression.Trainers.FastTree());
            //定型模型
            var model=pipeline.Fit(dataView);
            //返回模型
            return model;
        }

        static void Evaluate(MLContext mlContext, ITransformer model)
        {
            //加载测试数据集
            IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(_testDataPath, hasHeader: true, separatorChar: ',');
            //创建回归计算器
            var predictions = model.Transform(dataView);
            //评估模型并创建指标
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");
            //显示指标
            ALog.Info();
            ALog.Info($"*************************************************");
            ALog.Info($"*       Model quality metrics evaluation         ");
            ALog.Info($"*------------------------------------------------");
            ALog.Info($"*       决定系数:      {metrics.RSquared:0.##}");
            ALog.Info($"*       均方误差根:      {metrics.RootMeanSquaredError:0.##}");
        }

        //使用预测模型
        static void TestSinglePrediction(MLContext mlContext, ITransformer model)
        {
            //创建测试数据的单个注释
            var predictionFunction = mlContext.Model.CreatePredictionEngine<TaxiTrip, TaxiTripFarePrediction>(model);
            //根据测试数据预测费用
            var taxiTripSample = new TaxiTrip()
            {
                VendorId = "VTS",
                RateCode = "1",
                PassengerCount = 1,
                TripTime = 1140,
                TripDistance = 3.75f,
                PaymentType = "CRD",
                FareAmount = 0 // To predict. Actual/Observed = 15.5
            };
            var prediction = predictionFunction.Predict(taxiTripSample);
            //结合测试数据和预测进行报告
            //显示预测结果
            ALog.Info($"**********************************************************************");
            ALog.Info($"预测费用: {prediction.FareAmount:0.####}, 实际费用: 15.5");
            ALog.Info($"**********************************************************************");
        }
    }
}
