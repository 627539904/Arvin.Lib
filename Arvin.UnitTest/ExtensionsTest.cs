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
    }
}