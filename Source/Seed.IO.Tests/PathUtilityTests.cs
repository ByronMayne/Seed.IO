
using Xunit;

namespace Seed.IO
{
    public class PathUtilityTests
    {
        [Fact]
        public void GetComponents_EmptyValue_ReturnsEmptyArray()
            => Assert.Empty(PathUtility.GetComponents(""));

        [Fact]
        public void GetComponents_Null_ReturnsEmptyArray()
           => Assert.Empty(PathUtility.GetComponents(null));

        [Theory]
        [InlineData("C:\\", 1)]
        [InlineData("C:\\files", 2)]
        [InlineData("c:\\files\\images", 3)]
        public void GetComponents_ValidPath_ReturnsExpected(string input, int expectedCount)
            => Assert.Equal(PathUtility.GetComponents(input).Length, expectedCount);


        [Theory]
        [InlineData("/", true)]
        [InlineData("/Disks/data", true)]
        [InlineData("\\Testing", false)]
        [InlineData("C:\\Testing", false)]
        [InlineData(null, false)]
        public void Input_HasUnixRoot_MatchesExpected(string input, bool hasRoot)
            => Assert.Equal(PathUtility.HasUnixRoot(input), hasRoot);

        [Theory]
        [InlineData("C:\\Disks/data", true)]
        [InlineData("F:\\", true)]
        [InlineData("\\Testing", false)]
        [InlineData("C:\\Testing", true)]
        [InlineData("/Testing/Data",false)]
        [InlineData(null, false)]
        public void Input_HasWindowsRoot_MatchesExpected(string input, bool hasRoot)
            => Assert.Equal(hasRoot, PathUtility.HasWinRoot(input));


        //[Test]
        //public void ValidateSeprator()
        //{
        //    Assert.Throws<ArgumentException>(() => PathUtility.ValidateChosenSeparator("C:\\", '/'));
        //    Assert.Throws<ArgumentException>(() => PathUtility.ValidateChosenSeparator("/data", '\\'));
        //    Assert.DoesNotThrow(() => PathUtility.ValidateChosenSeparator("C://Items", '\\'));
        //    Assert.DoesNotThrow(() => PathUtility.ValidateChosenSeparator("C:\\", '\\'));
        //    Assert.DoesNotThrow(() => PathUtility.ValidateChosenSeparator("/Data/Items", '/'));
        //    Assert.DoesNotThrow(() => PathUtility.ValidateChosenSeparator(null, '/'));
        //    Assert.DoesNotThrow(() => PathUtility.ValidateChosenSeparator(null, null));
        //}

        //[Test]
        //public void GetSeperator()
        //{
        //    Assert.AreEqual(PathUtility.GetSeparator("/data/items"), '/');
        //    Assert.AreEqual(PathUtility.GetSeparator("C:\\data\\items"), '\\');
        //    Assert.AreEqual(PathUtility.GetSeparator("C:/data/items"), '\\');
        //    Assert.DoesNotThrow(() => PathUtility.GetSeparator(null));
        //}

        //[Test]
        //public void HasRoot()
        //{
        //    Assert.True(PathUtility.HasRoot("/data/items"));
        //    Assert.True(PathUtility.HasRoot("C:\\data\\items"));
        //    Assert.False(PathUtility.HasRoot("\\data"));
        //    Assert.False(PathUtility.HasRoot(""));
        //    Assert.False(PathUtility.HasRoot(null));
        //}

        //[Test]
        //public void Normalize()
        //{
        //    //Assert.AreEqual("/data/Items", PathUtility.Normalize("/data\\Items"));
        //    //Assert.AreEqual("C:\\data\\other\\stuff", PathUtility.Normalize("C:/data/other\\stuff"));
        //    //Assert.AreEqual("C:\\data", PathUtility.Normalize("C:/data/other\\.."));
        //    //Assert.AreEqual("data", PathUtility.Normalize(".\\data"));
        //    //Assert.AreEqual(null, PathUtility.Normalize(null));
        //    //Assert.Throws<ArgumentException>(() => PathUtility.Normalize("C:\\..\\.."));
        //    //Assert.Throws<ArgumentException>(() => PathUtility.Normalize("C:\\data\\..\\.."));
        //}
    }
}