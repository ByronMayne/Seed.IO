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
        public void WindowsSlash()
        {
            AbsolutePath lhs = (AbsolutePath)"C:\\";
            lhs /= "data";
            Assert.AreEqual("C:\\data", lhs.ToString());
            lhs /= "tools";
            Assert.AreEqual("C:\\data\\tools", lhs.ToString());
            lhs /= "..";
            Assert.AreEqual("C:\\data", lhs.ToString());
        }

        [Test]
        public void LinuxSlash()
        {
            AbsolutePath lhs = (AbsolutePath)"/";
            Assert.AreEqual("/", lhs.ToString());

            lhs /= "data";
            Assert.AreEqual("/data", lhs.ToString());

            lhs /= "images";
            Assert.AreEqual("/data/images", lhs.ToString());

            lhs /= "art";
            Assert.AreEqual("/data/images/art", lhs.ToString());

            lhs /= "..";
            Assert.AreEqual("/data/images", lhs.ToString());
        }
    }
}
