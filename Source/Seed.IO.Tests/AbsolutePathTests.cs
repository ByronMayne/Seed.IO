using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Seed.IO.Tests
{
	public class AbsolutePathTests
	{
		[Test]
		public void Constructor_WithNullPath_ThrowsException()
			=> Assert.Throws<ArgumentNullException>(() => new AbsolutePath(null));

		[Test]
		public void Constructor_WithEmptyPath_ThrowsException()
			=> Assert.Throws<ArgumentException>(() => new AbsolutePath(""));

		[Test]
		public void Constructor_WithRelativePath_ThrowsException()
			=> Assert.Throws<ArgumentException>(() => new AbsolutePath("./Files"));

		[Test]
		public void Combine_WithRawAbsolutePath_ThrowsException()
		{
			AbsolutePath basePath = AbsolutePath.Parse("C:/data");

			Assert.Throws<InvalidOperationException>(() =>
			{
				AbsolutePath result = basePath / "C:/files";
			});
		}

		[Test]
		public void Combine_WithRelativePath_MatchesExpected()
		{
			AbsolutePath basePath = AbsolutePath.Parse("C://data");
			RelativePath relativePath = RelativePath.Parse("./files");
			AbsolutePath result = basePath / relativePath;
			Assert.AreEqual("C:\\data\\files", result.ToString());
		}

		[Test]
		public void Combine_WithRawRelative_MatchesExpected()
		{
			AbsolutePath basePath = AbsolutePath.Parse("C://data");
			AbsolutePath result = basePath / "./files";
			Assert.AreEqual("C:\\data\\files", result.ToString());
		}

		[Test]
		public void TryParse_WithInvalidValue_DoesNotThrow()
			=> AbsolutePath.TryParse("fdsafdasfda", out var _);

		[Test]
		public void TryParse_WithInvalidValue_ReturnsFalse()
			=> Assert.IsFalse(AbsolutePath.TryParse("fdasfdafa", out var _));

		[Test]
		public void TryParse_WithValidValue_ReturnsTrue()
			=> Assert.IsTrue(AbsolutePath.TryParse("C:/files", out var _));

		[Test]
		public void TrayParse_WithValidValue_OutputMatches()
		{
			Assume.That(AbsolutePath.TryParse("C:/files", out AbsolutePath result));
			Assert.AreEqual("C:\\files", result.ToString());
		}

		[Test]
		public void SerializedInstance_MemberCount_IsEqual()
		{
			ISerializable serializable = AbsolutePath.Parse("C:\\files\\images");
			SerializationInfo serializationInfo = new SerializationInfo(typeof(AbsolutePath), new FormatterConverter());
			serializable.GetObjectData(serializationInfo, default(StreamingContext));
			Assert.AreEqual(2, serializationInfo.MemberCount, "We should have serialized two members");
		}

		[Test]
		public void SerializedInstance_WhenDeserialized_IsEqualToOrginal()
		{
			AbsolutePath startingValue = AbsolutePath.Parse("C:\\files\\images");
			ISerializable serializable = startingValue;
			SerializationInfo serializationInfo = new SerializationInfo(typeof(AbsolutePath), new FormatterConverter());
			serializable.GetObjectData(serializationInfo, default(StreamingContext));

			AbsolutePath endingValue = new AbsolutePath(serializationInfo, new StreamingContext());
			Assert.AreEqual(startingValue, endingValue);
		}

		[Test]
		public void RelativePath_ChildDirectory_MatchesExcepted()
		{
			AbsolutePath lhs = AbsolutePath.Parse("C:\\files");
			AbsolutePath rhs = AbsolutePath.Parse("c:\\files\\images\\");
			RelativePath relativePath = lhs.GetRelative(rhs);
			Assert.AreEqual(".\\images", relativePath.ToString());
		}

		[Test]
		public void RelativePath_FromFileToParentDirectory_MatchesExpected()
		{
			AbsolutePath lhs = AbsolutePath.Parse("c:\\files\\images\\cat.png");  
			AbsolutePath rhs = AbsolutePath.Parse("C:\\files");
			RelativePath relativePath = lhs.GetRelative(rhs);
			Assert.AreEqual(".\\..\\..", relativePath.ToString());
		}


		[Test]
		public void RelativePath_ParentDirectoryWithChild_MatchesExpected()
		{
			AbsolutePath lhs = AbsolutePath.Parse("c:\\files\\images\\cat.png");
			AbsolutePath rhs = AbsolutePath.Parse("C:\\files\\dog.png");
			RelativePath relativePath = lhs.GetRelative(rhs);
			Assert.AreEqual(".\\..\\..\\dog.png", relativePath.ToString());
		}

		[Test]
		public void RelativePath_BranchedPath_MatchesExpected()
		{
			AbsolutePath lhs = AbsolutePath.Parse("c:\\files\\images\\cat.png");
			AbsolutePath rhs = AbsolutePath.Parse("C:\\files\\videos\\dog.png");
			RelativePath relativePath = lhs.GetRelative(rhs);
			Assert.AreEqual(".\\..\\..\\videos\\dog.png", relativePath.ToString());
		}

		[Test]
		public void RelativePath_NoCommonRoot_ThrowsException()
		{
			AbsolutePath lhs = AbsolutePath.Parse("C:\\files\\imges");
			AbsolutePath rhs = AbsolutePath.Parse("D:\\files\\images");
			Assert.Throws<InvalidOperationException>(() => lhs.GetRelative(rhs));
		}

		[Test]
		public void GetParent_WithValidValidParent_MatchesExpected()
		{
			AbsolutePath absolutePath = AbsolutePath.Parse("C:\\files\\images");
			AbsolutePath result = absolutePath.GetParent();
			Assert.AreEqual("C:\\files", result.ToString());
		}

		[Test]
		public void GetParent_WithoutValidParent_ThrowsException()
		{
			AbsolutePath absolutePath = AbsolutePath.Parse("C:\\");
			Assert.Throws<InvalidOperationException>(() => absolutePath.GetParent());
		}

		[Test]
		public void IsEmpty_CheckValue_MatchesExpected()
		{
			Assert.IsTrue(new AbsolutePath().IsEmpty);
			Assert.IsFalse(new AbsolutePath("C://").IsEmpty);
		}
	}
}
