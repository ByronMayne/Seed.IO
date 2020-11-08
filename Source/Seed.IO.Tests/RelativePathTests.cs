using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Seed.IO.Tests
{
	class RelativePathTests
	{
		[Test]
		public void Constructor_WithNullPath_ThrowsException()
			=> Assert.Throws<ArgumentNullException>(() => new RelativePath(null));

		[Test]
		public void Constructor_WithEmptyPath_ThrowsException()
			=> Assert.Throws<ArgumentException>(() => new RelativePath(""));

		[Test]
		public void Constructor_WithAbsolutePath_ThrowsException()
			=> Assert.Throws<ArgumentException>(() => new RelativePath("C:/Files"));

		[Test]
		public void Combine_WithRawAbsolutePath_ThrowsException()
		{
			RelativePath basePath = RelativePath.Parse("./data");

			Assert.Throws<InvalidOperationException>(() =>
			{
				RelativePath result = basePath / "C:/files";
			});
		}

		[Test]
		public void Combine_WithRelativePath_MatchesExpected()
		{
			RelativePath basePath = RelativePath.Parse(".\\data");
			RelativePath relativePath = RelativePath.Parse("\\files");
			RelativePath result = basePath / relativePath;
			Assert.AreEqual(".\\data\\files", result.ToString());
		}

		[Test]
		public void Combine_WithRawRelative_MatchesExpected()
		{
			RelativePath basePath = RelativePath.Parse(".\\data");
			RelativePath result = basePath / "./files";
			Assert.AreEqual(".\\data\\files", result.ToString());
		}

		[Test]
		public void TryParse_WithInvalidValue_DoesNotThrow()
			=> RelativePath.TryParse("fdsafdasfda", out var _);

		[Test]
		public void TryParse_WithAbsolute_ReturnsFalse()
			=> Assert.IsFalse(RelativePath.TryParse("C:\\fdasfdafa", out var _));

		[Test]
		public void TryParse_WithValidValue_ReturnsTrue()
			=> Assert.IsTrue(RelativePath.TryParse(".\\files", out var _));

		[Test]
		public void TrayParse_WithValidValue_OutputMatches()
		{
			Assume.That(RelativePath.TryParse(".\\files", out RelativePath result));
			Assert.AreEqual(".\\files", result.ToString());
		}

		[Test]
		public void SerializedInstance_MemberCount_IsEqual()
		{
			ISerializable serializable = RelativePath.Parse(".\\files\\images");
			SerializationInfo serializationInfo = new SerializationInfo(typeof(AbsolutePath), new FormatterConverter());
			serializable.GetObjectData(serializationInfo, default(StreamingContext));
			Assert.AreEqual(3, serializationInfo.MemberCount, "We should have serialized three members");
		}

		[Test]
		public void SerializedInstance_WhenDeserialized_IsEqualToOrginal()
		{
			RelativePath startingValue = RelativePath.Parse(".\\files\\images");
			ISerializable serializable = startingValue;
			SerializationInfo serializationInfo = new SerializationInfo(typeof(RelativePath), new FormatterConverter());
			serializable.GetObjectData(serializationInfo, default(StreamingContext));

			RelativePath endingValue = new RelativePath(serializationInfo, new StreamingContext());
			Assert.AreEqual(startingValue, endingValue);
		}

		[Test]
		public void GetParent_CanBeResolved_MatchesExpected()
		{
			RelativePath path = RelativePath.Parse(".\\files\\images");
			RelativePath result = path.GetParent();
			Assert.AreEqual(".\\files", result.ToString());
		}

		[Test]
		public void GetParent_CanNotBeResolved_ThrowsException()
		{
			RelativePath relativePath = RelativePath.Parse(".\\files");
			RelativePath parent = relativePath.GetParent();
			Assert.AreEqual(".", parent.ToString());
		}
	}
}
