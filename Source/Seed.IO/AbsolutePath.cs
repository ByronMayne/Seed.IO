using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Seed.IO
{
	/// <summary>
	/// An absolute path refers to the complete details needed to locate a file or 
	/// folder, starting from the root element and ending with the other subdirectories.
	/// </summary>

	[Serializable]
	[DebuggerDisplay("{m_path}")]
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
				if (path == null) throw new ArgumentNullException(nameof(path));
				if (path.Length == 0) throw new ArgumentException(nameof(path), "The parameter has to have a value.");

				if (!PathUtility.HasRoot(path))
				{
					throw new ArgumentException(nameof(path), $"An absolute path must be rooted, the path sent in was '{path}'.");
				}
			}
			m_path = PathUtility.Normalize(path);
			m_stringComparison = PathUtility.HasWinRoot(m_path)
				? StringComparison.OrdinalIgnoreCase
				: StringComparison.Ordinal;
		}

		internal AbsolutePath(SerializationInfo info, StreamingContext context)
		{
			m_path = info.GetString(nameof(m_path));
			m_stringComparison = (StringComparison)info.GetInt32(nameof(m_stringComparison));
		}

		/// <summary>
		/// Allows paths to be combined using the forward slash operator 
		/// </summary>
		public static AbsolutePath operator /(AbsolutePath left, string right)
		{
			if(PathUtility.HasRoot(right))
			{
				throw new InvalidOperationException($"An absolute path can't be combined with another absolute path. " +
					$"Base is '{left.m_path}' and is trying to be combined with '{right}'/.");
			}
			string path = PathUtility.Combine(left, right);
			return new AbsolutePath(path, false);
		}

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
		/// Parses a path into an absolute path, if the path is invalid 
		/// and exception will be thrown
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static AbsolutePath Parse(string path)
			=> new AbsolutePath(path);

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

		public RelativePath GetRelative(AbsolutePath relativeTo)
		{
			if(TryGetRelative(relativeTo, out RelativePath result))
			{
				return result;
			}
			
			throw new InvalidOperationException($"The path {this} does not share a common root with '{relativeTo}'.");
		}

		/// <summary>
		/// Attempts to get the relative path difference between two absolute paths. This only works if
		/// both paths share a common root.
		/// </summary>
		/// <param name="relativeTo">TThe path to get a relative path for</param>
		/// <param name="result">the resulting relative path</param>
		/// <returns>True if a relative path could be created and false if it could not</returns>
		public bool TryGetRelative(AbsolutePath relativeTo, out RelativePath result)
		{
			result = RelativePath.Empty;

			string[] lhsComponents = PathUtility.GetComponents(this);
			string[] rhsComponents = PathUtility.GetComponents(relativeTo);

			int commonRoot = -1;

			int length = Math.Min(lhsComponents.Length, rhsComponents.Length);

			for(int i = 0; i < length; i++)
			{
				if(string.Equals(lhsComponents[i], rhsComponents[i], m_stringComparison))
				{
					commonRoot++;
				}
				else
				{
					break;
				}
			}

			// No matching root 
			if(commonRoot <= 0)
			{
				return false;
			}

			// Add one so that we take into account copying the first index
			commonRoot += 1;

			List<string> paths = new List<string>();

			paths.Add(".");

			// Add back tracks ../
			for(int i = 0; i < lhsComponents.Length - commonRoot; i++)
			{
				paths.Add("..");
			}
			// Add difference 
			for(int i = commonRoot; i < rhsComponents.Length; i++)
			{
				paths.Add(rhsComponents[i]);
			}

			char seperator = PathUtility.GetSeparator(this);

			string builtPath = string.Join(seperator.ToString(), paths);

			result = RelativePath.Parse(builtPath);

			return true;
		}

		/// <summary>
		/// Returns back the raw string of this path. 
		/// </summary>
		public static implicit operator string(AbsolutePath path)
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
