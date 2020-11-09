using System;
using System.Runtime.Serialization;
using Xunit;

namespace Seed.IO.Facts
{
	public class RelativePathFacts
	{
		[Fact]
		public void Constructor_WithNullPath_ThrowsException()
			=> Assert.Throws<ArgumentNullException>(() => new RelativePath(null));

		[Fact]
		public void Constructor_WithEmptyPath_ThrowsException()
			=> Assert.Throws<ArgumentException>(() => new RelativePath(""));

		[Fact]
		public void Constructor_WithAbsolutePath_ThrowsException()
			=> Assert.Throws<ArgumentException>(() => new RelativePath("C:/Files"));

		[Fact]
		public void Combine_WithRawAbsolutePath_ThrowsException()
		{
			RelativePath basePath = RelativePath.Parse("./data");

			Assert.Throws<InvalidOperationException>(() =>
			{
				RelativePath result = basePath / "C:/files";
			});
		}

		[Fact]
		public void Combine_WithRelativePath_MatchesExpected()
		{
			RelativePath basePath = RelativePath.Parse(".\\data");
			RelativePath relativePath = RelativePath.Parse("\\files");
			RelativePath result = basePath / relativePath;
			Assert.Equal(".\\data\\files", result.ToString());
		}

		[Fact]
		public void Combine_WithRawRelative_MatchesExpected()
		{
			RelativePath basePath = RelativePath.Parse(".\\data");
			RelativePath result = basePath / "./files";
			Assert.Equal(".\\data\\files", result.ToString());
		}

		[Fact]
		public void TryParse_WithInvalidValue_DoesNotThrow()
			=> RelativePath.TryParse("fdsafdasfda", out var _);

		[Fact]
		public void TryParse_WithAbsolute_ReturnsFalse()
			=> Assert.False(RelativePath.TryParse("C:\\fdasfdafa", out var _));

		[Fact]
		public void TryParse_WithValidValue_ReturnsTrue()
			=> Assert.True(RelativePath.TryParse(".\\files", out var _));

		[Fact]
		public void TrayParse_WithValidValue_OutputMatches()
		{
			Assume.That(RelativePath.TryParse(".\\files", out RelativePath result));
			Assert.Equal(".\\files", result.ToString());
		}

		[Fact]
		public void SerializedInstance_MemberCount_IsEqual()
		{
			ISerializable serializable = RelativePath.Parse(".\\files\\images");
			SerializationInfo serializationInfo = new SerializationInfo(typeof(AbsolutePath), new FormatterConverter());
			serializable.GetObjectData(serializationInfo, default(StreamingContext));
			Assert.Equal(3, serializationInfo.MemberCount);
		}

		[Fact]
		public void SerializedInstance_WhenDeserialized_IsEqualToOrginal()
		{
			RelativePath startingValue = RelativePath.Parse(".\\files\\images");
			ISerializable serializable = startingValue;
			SerializationInfo serializationInfo = new SerializationInfo(typeof(RelativePath), new FormatterConverter());
			serializable.GetObjectData(serializationInfo, default(StreamingContext));

			RelativePath endingValue = new RelativePath(serializationInfo, new StreamingContext());
			Assert.Equal(startingValue, endingValue);
		}

		[Fact]
		public void GetParent_CanBeResolved_MatchesExpected()
		{
			RelativePath path = RelativePath.Parse(".\\files\\images");
			RelativePath result = path.GetParent();
			Assert.Equal(".\\files", result.ToString());
		}

		[Fact]
		public void GetParent_CanNotBeResolved_ThrowsException()
		{
			RelativePath relativePath = RelativePath.Parse(".\\files");
			RelativePath parent = relativePath.GetParent();
			Assert.Equal(".", parent.ToString());
		}
	}
}
