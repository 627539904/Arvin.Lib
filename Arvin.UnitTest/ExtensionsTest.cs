using Arvin.Extensions;
using Arvin.Helpers;

namespace Arvin.UnitTest
{
    public class ExtensionsTest
    {
        #region CheckExtension
        List<string> nullList = null;
        List<string> hasValsList = new List<string> { "123", "456" };
        [Fact]
        public void Test_IsNullOrEmpty()
        {
            // 安排（Arrange）: 创建测试所需的对象和状态  
            var input = hasValsList;
            var expectedResult = false;
            var input2 = nullList;
            var expectedResult2 = true;

            // 执行（Act）: 调用要测试的方法  
            var actualResult = input.IsNullOrEmpty();
            var actualResult2 = input2.IsNullOrEmpty();

            // 断言（Assert）: 验证方法调用的结果是否符合预期  
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedResult2, actualResult2);
        }
        #endregion

        #region ConvertExtension
        class FromJsonTest
        {
            //public (double[] Item1, double?[] Item2) TupleDouble { get; set; }
            public double?[] ArrDouble { set; get; }
            //public List<(double[], double[])> SourceTargetMetrixPairs { get; set; }
        }
        [Fact]
        public void Test_FromJson()
        {
            var input = "{\"Item1\":[5.537384295534317e-15,-0.800000011920929,0,0,0.800000011920929,5.537384295534317e-15,0,0,0,0,1,0,-292.3512878417969,-811.814453125,0,1],\"Item2\":[null,null,null,0,null,null,null,0,null,null,null,0,-292.3512878417969,-811.814453125,0,1]}";
            var input2 = "{\"ArrDouble\":[null,null,null,0,null,null,null,0,null,null,null,0,-292.3512878417969,-811.814453125,0,1]}";

            //带有空值的序列化转换
            //var testModel = input.FromJson<FromJsonTest>();
            //LogHelper.Info($"数组长度：{testModel.TupleDouble.Item2.Length}");//TupleDouble
            var testModel = input2.FromJson<FromJsonTest>();
            LogHelper.Info($"数组长度：{testModel.ArrDouble.Length}");//ArrDouble
        }
        #endregion
    }
}