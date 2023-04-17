using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Seed.IO.Tests
{
    public class RelativePathTests
    {
        [Theory]
        [InlineData(".")]
        [InlineData(".//files/cats")]
        [InlineData(".\\files")]
        [InlineData("..\\..")]
        public void TryParse_WithValidPath_ReturnsTrue(string path)
            => Assert.True(RelativePath.TryParse(path, false, out _));

        [Theory]
        [InlineData("C://files")]
        [InlineData(null)]
        [InlineData("/files")]
        public void TryParse_WithInvalidPath_ReturnFalse(string path)
            => Assert.False(RelativePath.TryParse(path, false, out _));

        [Fact]
        public void IsDefault_WithNonDefaultPath_ReturnsFalse()
            => Assert.False(new RelativePath(".//files").IsDefault);

        [Fact]
        public void IsDefault_WithDefaultPath_ReturnsTrue()
            => Assert.True(RelativePath.Default.IsDefault);

        [Fact]
        public void Constructor_WithNullValue_ThrowsException()
            => Assert.Throws<ArgumentNullException>(() => new RelativePath(null!));
    }
}
