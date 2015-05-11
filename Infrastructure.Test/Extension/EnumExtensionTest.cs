using Infrastructure.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.Test.Extension
{
    [TestClass]
    public class EnumExtensionTest
    {
        public enum Color
        {
            [System.ComponentModel.Description("红色")]
            Red,
            [System.ComponentModel.Description("绿色")]
            Green,
            Blue
        }

        [TestMethod]
        public void Test()
        {
            Assert.AreEqual("红色", Color.Red.GetDescription());
            Assert.AreEqual("绿色", Color.Green.GetDescription());
            Assert.AreNotEqual("蓝色", Color.Blue.GetDescription());
        }
    }
}
