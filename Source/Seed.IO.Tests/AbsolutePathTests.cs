using System;
using System.Runtime.Serialization;
using Xunit;

namespace Seed.IO.Tests
{
	public class AbsolutePathTests
	{
		[Fact]
		public void Constructor_WithNullPath_ThrowsException()
			=> Assert.Throws<ArgumentNullException>(() => new AbsolutePath(null));

		[Fact]
		public void Constructor_WithEmptyPath_ThrowsException()
			=> Assert.Throws<ArgumentException>(() => new AbsolutePath(""));

		[Fact]
		public void Constructor_WithRelativePath_ThrowsException()
			=> Assert.Throws<ArgumentException>(() => new AbsolutePath("./Files"));

		[Fact]
		public void Combine_WithRawAbsolutePath_ThrowsException()
		{
			AbsolutePath basePath = AbsolutePath.Parse("C:/data");

			Assert.Throws<InvalidOperationException>(() =>
			{
				AbsolutePath result = basePath / "C:/files";
			});
		}

		[Fact]
		public void Combine_WithRelativePath_MatchesExpected()
		{
			AbsolutePath basePath = AbsolutePath.Parse("C://data");
			RelativePath relativePath = RelativePath.Parse("./files");
			AbsolutePath result = basePath / relativePath;
			Assert.Equal("C:\\data\\files", result.ToString());
		}

		[Fact]
		public void Combine_WithRawRelative_MatchesExpected()
		{
			AbsolutePath basePath = AbsolutePath.Parse("C://data");
			AbsolutePath result = basePath / "./files";
			Assert.Equal("C:\\data\\files", result.ToString());
		}

		[Fact]
		public void TryParse_WithInvalidValue_DoesNotThrow()
			=> AbsolutePath.TryParse("fdsafdasfda", out var _);

		[Fact]
		public void TryParse_WithInvalidValue_ReturnsFalse()
			=> Assert.False(AbsolutePath.TryParse("fdasfdafa", out var _));

		[Fact]
		public void TryParse_WithValidValue_ReturnsTrue()
			=> Assert.True(AbsolutePath.TryParse("C:/files", out var _));

		[Fact]
		public void TrayParse_WithValidValue_OutputMatches()
		{
			Assume.That(AbsolutePath.TryParse("C:/files", out AbsolutePath result));
			
			Assert.Equal("C:\\files", result.ToString());
		}

		[Fact]
		public void SerializedInstance_MemberCount_IsEqual()
		{
			ISerializable serializable = AbsolutePath.Parse("C:\\files\\images");
			SerializationInfo serializationInfo = new SerializationInfo(typeof(AbsolutePath), new FormatterConverter());
			serializable.GetObjectData(serializationInfo, default(StreamingContext));
			Assert.Equal(2, serializationInfo.MemberCount);
		}

		[Fact]
		public void SerializedInstance_WhenDeserialized_IsEqualToOrginal()
		{
			AbsolutePath startingValue = AbsolutePath.Parse("C:\\files\\images");
			ISerializable serializable = startingValue;
			SerializationInfo serializationInfo = new SerializationInfo(typeof(AbsolutePath), new FormatterConverter());
			serializable.GetObjectData(serializationInfo, default(StreamingContext));

			AbsolutePath endingValue = new AbsolutePath(serializationInfo, new StreamingContext());
			Assert.Equal(startingValue, endingValue);
		}

		[Fact]
		public void RelativePath_ChildDirectory_MatchesExcepted()
		{
			AbsolutePath lhs = AbsolutePath.Parse("C:\\files");
			AbsolutePath rhs = AbsolutePath.Parse("c:\\files\\images\\");
			RelativePath relativePath = lhs.GetRelative(rhs);
			Assert.Equal(".\\images", relativePath.ToString());
		}

		[Fact]
		public void RelativePath_FromFileToParentDirectory_MatchesExpected()
		{
			AbsolutePath lhs = AbsolutePath.Parse("c:\\files\\images\\cat.png");
			AbsolutePath rhs = AbsolutePath.Parse("C:\\files");
			RelativePath relativePath = lhs.GetRelative(rhs);
			Assert.Equal(".\\..\\..", relativePath.ToString());
		}


		[Fact]
		public void RelativePath_ParentDirectoryWithChild_MatchesExpected()
		{
			AbsolutePath lhs = AbsolutePath.Parse("c:\\files\\images\\cat.png");
			AbsolutePath rhs = AbsolutePath.Parse("C:\\files\\dog.png");
			RelativePath relativePath = lhs.GetRelative(rhs);
			Assert.Equal(".\\..\\..\\dog.png", relativePath.ToString());
		}

		[Fact]
		public void RelativePath_BranchedPath_MatchesExpected()
		{
			AbsolutePath lhs = AbsolutePath.Parse("c:\\files\\images\\cat.png");
			AbsolutePath rhs = AbsolutePath.Parse("C:\\files\\videos\\dog.png");
			RelativePath relativePath = lhs.GetRelative(rhs);
			Assert.Equal(".\\..\\..\\videos\\dog.png", relativePath.ToString());
		}

		[Fact]
		public void RelativePath_NoCommonRoot_ThrowsException()
		{
			AbsolutePath lhs = AbsolutePath.Parse("C:\\files\\imges");
			AbsolutePath rhs = AbsolutePath.Parse("D:\\files\\images");
			Assert.Throws<InvalidOperationException>(() => lhs.GetRelative(rhs));
		}

		[Fact]
		public void GetParent_WithValidValidParent_MatchesExpected()
		{
			AbsolutePath absolutePath = AbsolutePath.Parse("C:\\files\\images");
			AbsolutePath result = absolutePath.GetParent();
			Assert.Equal("C:\\files", result.ToString());
		}

		[Fact]
		public void GetParent_WithoutValidParent_ThrowsException()
		{
			AbsolutePath absolutePath = AbsolutePath.Parse("C:\\");
			Assert.Throws<InvalidOperationException>(() => absolutePath.GetParent());
		}

		[Fact]
		public void IsEmpty_CheckValue_MatchesExpected()
		{
			Assert.True(new AbsolutePath().IsEmpty);
			Assert.False(new AbsolutePath("C://").IsEmpty);
		}
	}
}
