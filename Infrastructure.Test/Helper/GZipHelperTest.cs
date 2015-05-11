using Infrastructure.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.Test.Helper
{
    [TestClass]
    public class GZipHelperTest
    {
        [TestMethod]
        public void Test()
        {
            var temp = "abc123!@#";
            var comressed = GZipHelper.GZipCompressString(temp);
            var decomressed = GZipHelper.GZipDecompressString(comressed);
            Assert.AreEqual(temp, decomressed);
        }
    }
}
