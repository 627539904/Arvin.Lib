using Arvin.Extensions;

namespace Arvin.UnitTest
{
    public class ExtensionsTest
    {
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
    }
}