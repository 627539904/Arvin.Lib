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
            // ���ţ�Arrange��: ������������Ķ����״̬  
            var input = hasValsList;
            var expectedResult = false;
            var input2 = nullList;
            var expectedResult2 = true;

            // ִ�У�Act��: ����Ҫ���Եķ���  
            var actualResult = input.IsNullOrEmpty();
            var actualResult2 = input2.IsNullOrEmpty();

            // ���ԣ�Assert��: ��֤�������õĽ���Ƿ����Ԥ��  
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

            //���п�ֵ�����л�ת��
            //var testModel = input.FromJson<FromJsonTest>();
            //LogHelper.Info($"���鳤�ȣ�{testModel.TupleDouble.Item2.Length}");//TupleDouble
            var testModel = input2.FromJson<FromJsonTest>();
            LogHelper.Info($"���鳤�ȣ�{testModel.ArrDouble.Length}");//ArrDouble
        }
        #endregion
    }
}