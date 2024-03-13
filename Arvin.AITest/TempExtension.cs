using Arvin.AITest.Guide.ML_NET.API.SentimentAnalysis;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ML.DataOperationsCatalog;

namespace Arvin.AITest
{
    public static class TempExtension
    {
        #region ML.NET 语法糖
        /// <summary>
        /// 从文件中加载数据(包含训练集和测试集)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mlContext"></param>
        /// <param name="dataPath"></param>
        /// <param name="hasHeader">文本文件是否包含表头，默认不包含</param>
        /// <param name="testFraction">指定数据的测试集百分比，默认值为10%</param>
        /// <returns></returns>
        public static TrainTestData LoadAllDataFromTextFile<T>(MLContext mlContext, string dataPath, bool hasHeader = false, double testFraction = 0.1)
        {
            //加载数据
            IDataView dataView = mlContext.Data.LoadFromTextFile<T>(dataPath, hasHeader: hasHeader);//定义数据架构并读取文件

            //将加载的数据集拆分为训练数据集和测试数据集
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction);//拆分为训练数据集和测试数据集.testFraction:指定数据的测试集百分比,默认值为 10%.

            //返回拆分的训练数据集和测试数据集
            return splitDataView;
        }
        #endregion
    }
}
