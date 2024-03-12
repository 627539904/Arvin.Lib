# Arvin.Lib
Net类库项目，用于Nuget扩展与知识体系整合，计划包含语法糖、算法、复杂运算、常用工具等，以避免工作学习中不断重复开发。

# Arvin.AITest

## 情感模型
- SentimentModel.mbconfig 情感模型-入门，yelp_labelled.txt为数据集
- 方案：数据分类

## 预测价格
- TaxiFarePrediction 预测出租车费
- 数据集：taxi-fare-train.csv
    - 数据集特分析
    - vendor_id： 出租车供应商的 ID 是一项特征。
    - rate_code： 出租车行程的费率类型是一项特征。
    - passenger_count： 行程中的乘客人数是一项特征。
    - trip_time_in_secs： 这次行程所花的时间。 希望在行程完成前预测行程费用。 当时并不知道行程时间有多长。 因此，行程时间不是一项特征，需要从模型删除此列。
    - trip_distance： 行程距离是一项特征。
    - payment_type： 付款方式（现金或信用卡）是一项特征。
    - fare_amount： 支付的总出租车费用是一个标签。【标签是指要预测的列】
- 方案：值预测
- 教程来源：https://learn.microsoft.com/zh-cn/dotnet/machine-learning/tutorials/predict-prices-with-model-builder
  
## 分析Web网站评论情绪
- 教程来源：https://learn.microsoft.com/zh-cn/dotnet/machine-learning/tutorials/sentiment-analysis-model-builder