using System;
using Xunit;

namespace Seed.IO.Tests
{
    public class PathUtilityTests
    {
        public string WindowsPath { get; }
        public string UnixPath { get; }


        public PathUtilityTests()
        {
            WindowsPath = "C:\\files\\cats";
            UnixPath = "/files/cats";
        }

        [Fact]
        public void WindowsPath_IsWindowsRoot_ReturnsTrue()
        {
            Assert.True(PathUtility.HasWindowsRoot(WindowsPath));
        }

        [Fact]
        public void WindowsPath_IsUnixRoot_ReturnsFalse()
        {
            Assert.False(PathUtility.HasUnixRoot(WindowsPath));
        }

        [Fact]
        public void WindowsPath_IsRooted_ReturnsTrue()
        {
            Assert.True(PathUtility.IsRooted(WindowsPath));
        }

        [Fact]
        public void UnixPath_IsWindowsRoot_ReturnsTrue()
        {
            Assert.False(PathUtility.HasWindowsRoot(UnixPath));
        }

        [Fact]
        public void UnixPath_IsUnixRoot_ReturnsFalse()
        {
            Assert.True(PathUtility.HasUnixRoot(UnixPath));
        }

        [Fact]
        public void UnixPath_IsRooted_ReturnsTrue()
        {
            Assert.True(PathUtility.IsRooted(UnixPath));
        }

        [Fact]
        public void NormalizeAbsolutePath_WithDuplicateSeperators_ReturnsExpected()
        {
            Assert.Equal("C:\\cats\\dogs", PathUtility.Normalize("C:\\\\cats\\\\dogs"));
        }

        [Fact]
        public void PathWIthMixedSeperator_Normalize_ReturnsExpected()
        {
            Assert.Equal("C:\\cats\\dogs", PathUtility.Normalize("C:\\cats/dogs"));
        }

        [Fact]
        public void NormalizeAbsolutePath_WithTrailingSeperator_RemovesTrailingSeperator()
        {
            Assert.Equal("C:\\cats\\dogs", PathUtility.Normalize("C:\\cats\\dogs\\"));
        }

        [Fact]
        public void NormalizeAbsolutePath_WithToManyRecursions_ThrowsException()
        {
            _ = Assert.Throws<ArgumentException>(() => PathUtility.Normalize("C:\\files\\..\\.."));
        }

        [Fact]
        public void NormalizeRelativePath_OutOfBounds_MatchesExpected()
        {
            Assert.Equal("..\\..", PathUtility.Normalize(".\\..\\.."));
        }
    }
}