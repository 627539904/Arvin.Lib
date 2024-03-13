using Arvin.AITest.Guide.ML_NET.API.GitHubIssueClassification;
using Arvin.AITest.Guide.ML_NET.API.SentimentAnalysis;
using Arvin.AITest.Guide.ML_NET.API.TaxiFarePrediction;
using Arvin.Helpers;
using Arvin_AITest;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace Arvin.AITest
{
    public class Program
    {
        static void Main(string[] args)
        {
            LogHelper.Init();

            //FirstExample();
            //Program_SentimentAnalysis.Test();
            //Program_GitHubIssueClassification.Test();
            Program_TaxiFarePrediction.Test();

            Console.ReadKey();
        }

        #region Model Builder
        //入门教程
        static void FirstExample()
        {
            // Add input data
            var sampleData = new SentimentModel.ModelInput()
            {
                Col0 = "This restaurant was wonderful."
            };

            // Load model and predict output of sample data
            var result = SentimentModel.Predict(sampleData);

            // If Prediction is 1, sentiment is "Positive"; otherwise, sentiment is "Negative"
            var sentiment = result.PredictedLabel == 1 ? "好" : "坏";
            LogHelper.Info($"Text: {sampleData.Col0}\nSentiment: {sentiment}");
        }
        #endregion

    }
}
