using System;
using System.Runtime.Serialization;

namespace Seed.IO
{
	/// <summary>
	/// An absolute path refers to the complete details needed to locate a file or 
	/// folder, starting from the root element and ending with the other subdirectories.
	/// </summary>

	[Serializable]
	public struct AbsolutePath :
		IEquatable<AbsolutePath>,
		IComparable<AbsolutePath>,
		ISerializable
	{
		private readonly string? m_path;
		private readonly StringComparison m_stringComparison;

		/// <summary>
		/// Gets an absolute path that has no value
		/// </summary>
		public static AbsolutePath Empty { get; }

		/// <summary>
		/// Gets if this absolute path has a value or not
		/// </summary>
		public bool IsEmpty => this == Empty;

		static AbsolutePath()
		{
			Empty = new AbsolutePath("", false);
		}

		/// <summary>
		/// Creates a new instance of an <see cref="AbsolutePath"/> that must have a value. If you want
		/// to use an empty value use <see cref="Empty"/> instaed.
		/// </summary>
		/// <param name="path">The path you want to use</param>
		/// <exception cref="ArgumentNullException">The path is null or empty</exception>
		public AbsolutePath(string path) : this(path, true)
		{ }

		private AbsolutePath(string path, bool checkValue)
		{
			if (checkValue)
			{
				if (string.IsNullOrEmpty(path))
				{
					throw new ArgumentNullException(nameof(path), "The parameter has to have a value.");
				}
				if (PathUtility.HasRoot(path))
				{
					throw new ArgumentException(nameof(path), $"An absolute path must be rooted, the path sent in was '{path}'.");
				}
			}
			m_path = path;
			m_stringComparison = PathUtility.HasWinRoot(m_path)
				? StringComparison.OrdinalIgnoreCase
				: StringComparison.Ordinal;
		}

		private AbsolutePath(SerializationInfo info, StreamingContext context)
		{
			m_path = info.GetString(nameof(m_path));
			m_stringComparison = (StringComparison)info.GetInt32(nameof(m_stringComparison));
		}

		/// <summary>
		/// Allows paths to be combined using the forward slash operator 
		/// </summary>
		public static AbsolutePath operator /(AbsolutePath left, string right)
			=> new AbsolutePath(PathUtility.Combine(left, right));

		/// <summary>
		/// Allows us to convert from strings to an absolute path. 
		/// </summary>
		public static explicit operator AbsolutePath(string path)
			=> new AbsolutePath(path, true);

		/// <summary>
		/// Creates a compile time error instead of a runtime error when users try to 
		/// combined two absolute paths.
		/// </summary>
		[Obsolete("Two absolute paths can not be merged as they both have roots defined", true)]
		public static AbsolutePath operator /(AbsolutePath left, AbsolutePath right)
			=> throw new NotImplementedException();

		/// <summary>
		/// Attempts to parse a string into an <see cref="AbsolutePath"/> if it's successful 
		/// true is returned and the value is set, otherwise value is <see cref="Empty"/> and
		/// false if returned.
		/// </summary>
		/// <param name="path">The path you want to try parsing</param>
		/// <param name="value">The result if the parse worked.</param>
		/// <returns>True if it could be parsed and false if it could not</returns>
		public static bool TryParse(string path, out AbsolutePath value)
		{
			value = Empty;

			// It's better to not have to throw exceptions we we do the simple checks first 

			if (string.IsNullOrWhiteSpace(path))
				return false;

			if (!PathUtility.HasRoot(path))
				return false;

			try
			{
				value = new AbsolutePath(path, true);
			}
			catch
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Returns the parent of this path if it exists. 
		/// </summary>
		public AbsolutePath GetParent()
		{
			return this / "..";
		}

		/// <summary>
		/// Get sif two paths are equal to each other
		/// </summary>
		public bool Equals(AbsolutePath other)
			=> string.Equals(other.m_path, m_path, m_stringComparison);

		/// <inheritdoc cref="System.Object"/>
		public override string ToString()
			=> m_path ?? "";

		/// <inheritdoc cref="System.Object"/>
		public override int GetHashCode()
			=> ToString().GetHashCode();

		/// <inheritdoc cref="IComparable{T}"/>
		public int CompareTo(AbsolutePath other)
			=> string.Compare(m_path, other.m_path, m_stringComparison);

		/// <inheritdoc cref="ISerializable"/>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(m_stringComparison), (int)m_stringComparison);
			info.AddValue(nameof(m_path), m_path);
		}

		/// <summary>
		/// Returns back the raw string of this path. 
		/// </summary>
		public static implicit operator string?(AbsolutePath path)
			=> path.ToString();

		/// <summary>
		/// Allows absolute paths to be converted to a boolean if they are not empty
		/// </summary>
		/// <param name="path">The path to check</param>
		/// <returns>True if it has a value and false if it does not</returns>
		public static bool operator true(AbsolutePath path)
			=> !path.IsEmpty;

		/// <summary>
		/// Allows an <see cref="AbsolutePath"/> to be converted to a boolean value based off
		/// if it's empty.
		/// </summary>
		/// <param name="path">The path</param>
		/// <returns>true if the path is empty and false if it's not</returns>
		public static bool operator false(AbsolutePath path)
			=> path.IsEmpty;
	}
}
