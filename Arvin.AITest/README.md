# Arvin.Lib
Net类库项目，用于Nuget扩展与知识体系整合，计划包含语法糖、算法、复杂运算、常用工具等，以避免工作学习中不断重复开发。

# Arvin.AITest

## Model Buidler教程（低代码）,适合无编程基础

### 情感模型
- SentimentModel.mbconfig 情感模型-入门，yelp_labelled.txt为数据集
- 方案：数据分类

### 预测价格
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
  
## API教程，适合有编程基础
- 来源：https://learn.microsoft.com/zh-cn/dotnet/machine-learning/tutorials/
- 关键词提取（不包含顺序）：
  - 上下文：MLContext mlContext = new MLContext(seed: 0);
  - 数据集：训练集和测试集
  - IDataView
    - content.Data.LoadFromTextFile(训练集)
  - 标签(模型的输出)：
    - Label
    - Score:Label
  - 算法选择：###.算法
  - 应用训练：pipeline.Fit(dataView)
  - 评估
    - 评估指标

### 分析情绪（二元分类）
- ML_NET.API.SentimentAnalysis
- 数据集：yelp_labelled.txt

### 对支持问题进行分类（多类分类）
- ML_NET.API.GitHubIssueClassification
- 数据集：
  - issues_train.tsv 定型机器虚席模型
  - issues_test.tsv 评估模型的准确度

### 预测价格（回归）
- ML_NET.API.TaxiFarePrediction
- 数据集：
  - taxi-fare-train.csv 
  - taxi-fare-test.csv 
- 算法：Regression

### 对鸢尾花进行分类（K平均值聚类分析）

### 推荐影片（矩阵因子分解）

### 图像分类（迁移学习）

### 对图像进行分类（模型组合）

### 预测自行车租赁需求（时序）

### 调用量峰值（异常情况检测）

### 产品销售分析（异常情况检测）

### 检测图像中的对象（对象检测）

### 使用TensorFlow对情绪分类（文本分类）：分析Web网站评论情绪
- 数据集：sentiment_model
  - saved_model.pb：TensorFlow 模型本身。 该模型采用表示 IMDB 评论字符串中文本的特征的固定长度（大小为 600）整数数组，并输出两个概率（总和为 1）：输入评论具有正面情绪的概率，以及输入评论具有负面情绪的概率。
  - imdb_word_index.csv：从单个单词到整数值的映射。 映射用于为 TensorFlow 模型生成输入特征
- 教程来源：https://learn.microsoft.com/zh-cn/dotnet/machine-learning/tutorials/sentiment-analysis-model-builder