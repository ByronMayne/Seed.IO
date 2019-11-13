using NUnit.Framework;
using Seed.IO;
using System;

namespace Seed.IO
{
    public class PathUtilityTests
    {
        [Test]
        public void IsUnixPath()
        {
            Assert.True(PathUtility.HasUnixRoot("/Disks/data"));
            Assert.True(PathUtility.HasUnixRoot("/"));
            Assert.False(PathUtility.HasUnixRoot("\\Testing"));
            Assert.False(PathUtility.HasUnixRoot("C:\\Testing"));
            Assert.False(PathUtility.HasUnixRoot(null));
        }

        [Test]
        public void IsWindowsPath()
        {
            Assert.True(PathUtility.HasWinRoot("C:\\Disks/data"));
            Assert.True(PathUtility.HasWinRoot("F:\\"));
            Assert.False(PathUtility.HasWinRoot("\\Testing"));
            Assert.False(PathUtility.HasWinRoot("/Testing/Data"));
            Assert.False(PathUtility.HasWinRoot(null));
        }

        [Test]
        public void ValidateSeprator()
        {
            Assert.Throws<ArgumentException>(() => PathUtility.ValidateChosenSeparator("C:\\", '/'));
            Assert.Throws<ArgumentException>(() => PathUtility.ValidateChosenSeparator("/data", '\\'));
            Assert.DoesNotThrow(() => PathUtility.ValidateChosenSeparator("C://Items", '\\'));
            Assert.DoesNotThrow(() => PathUtility.ValidateChosenSeparator("C:\\", '\\'));
            Assert.DoesNotThrow(() => PathUtility.ValidateChosenSeparator("/Data/Items", '/'));
            Assert.DoesNotThrow(() => PathUtility.ValidateChosenSeparator(null, '/'));
            Assert.DoesNotThrow(() => PathUtility.ValidateChosenSeparator(null, null));
        }

        [Test]
        public void GetSeperator()
        {
            Assert.AreEqual(PathUtility.GetSeparator("/data/items"), '/');
            Assert.AreEqual(PathUtility.GetSeparator("C:\\data\\items"), '\\');
            Assert.AreEqual(PathUtility.GetSeparator("C:/data/items"), '\\');
            Assert.DoesNotThrow(() => PathUtility.GetSeparator(null));
        }

        [Test]
        public void HasRoot()
        {
            Assert.True(PathUtility.HasRoot("/data/items"));
            Assert.True(PathUtility.HasRoot("C:\\data\\items"));
            Assert.False(PathUtility.HasRoot("\\data"));
            Assert.False(PathUtility.HasRoot(""));
            Assert.False(PathUtility.HasRoot(null));
        }

        [Test]
        public void Normalize()
        {
            Assert.AreEqual("/data/Items", PathUtility.Normalize("/data\\Items"));
            Assert.AreEqual("C:\\data\\other\\stuff", PathUtility.Normalize("C:/data/other\\stuff"));
            Assert.AreEqual("C:\\data", PathUtility.Normalize("C:/data/other\\.."));
            Assert.AreEqual("data", PathUtility.Normalize(".\\data"));
            Assert.AreEqual(null, PathUtility.Normalize(null));
            Assert.Throws<ArgumentException>(() => PathUtility.Normalize("C:\\..\\.."));
            Assert.Throws<ArgumentException>(() => PathUtility.Normalize("C:\\data\\..\\.."));
        }
    }
}