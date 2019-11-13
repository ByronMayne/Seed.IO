using NUnit.Framework;

namespace Seed.IO
{
    public class PathTests
    {
        [Test]
        public void Building()
        {
            Assert.AreEqual("C:\\docs\\items", (string)(AbsolutePath)"C://docs/items");
            Assert.AreEqual("C:\\", (string)(AbsolutePath)"C:\\data\\..");
        }

        [Test]
        public void SlashOverride()
        {
            AbsolutePath lhs = (AbsolutePath)"C:\\";
            lhs /= "data";
            Assert.AreEqual("C:\\data", lhs.ToString());
            lhs /= "tools";
            Assert.AreEqual("C:\\data\\tools", lhs.ToString());
            lhs /= "..";
            Assert.AreEqual("C:\\data", lhs.ToString());
        }
    }
}
