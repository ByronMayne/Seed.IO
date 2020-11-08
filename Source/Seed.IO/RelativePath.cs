#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Seed.IO
{
	/// <summary>
	/// A relative path is a way to specify the location of a directory relative to another directory
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{m_path}")]
	public struct RelativePath :
		IEquatable<RelativePath>,
		IComparable<RelativePath>,
		ISerializable
	{
		private readonly string? m_path;
		private readonly StringComparison m_stringComparison;
		private readonly char m_seperator;

		/// <summary>
		/// Gets an absolute path that has no value
		/// </summary>
		public static RelativePath Empty { get; }

		/// <summary>
		/// Gets a relative path that repersents the current directory
		/// </summary>
		public static RelativePath Current { get; }

		/// <summary>
		/// Gets if this absolute path has a value or not
		/// </summary>
		public bool IsEmpty => this == Empty;

		static RelativePath()
		{
			Empty = new RelativePath("", null, false);
			Current = new RelativePath(".", null, false);
		}

		/// <summary>
		/// Creatse a new instance of a relative path with the seperator being parsed out
		/// </summary>
		/// <param name="path">THe path you want to be relative</param>
		public RelativePath(string path) : this(path, null, true)
		{ }

		/// <summary>
		/// Creates a new instance of a relative path and the option to choose the seperator 
		/// that is used.
		/// </summary>
		/// <param name="path">The path you want to create the instance from</param>
		/// <param name="seperator">The seperator to use</param>
		public RelativePath(string path, char seperator) : this(path, seperator, true)
		{ }

		private RelativePath(string path, char? seperator, bool checkValue)
		{
			if (checkValue)
			{
				if (path == null) throw new ArgumentNullException(nameof(path));
				if (path.Length == 0) throw new ArgumentException(nameof(path), "The parameter has to have a value.");

				if (PathUtility.HasRoot(path))
				{
					throw new ArgumentException(nameof(path), $"An relative path must not be rooted, the path sent in was '{path}'.");
				}
			}
			m_seperator = seperator ?? PathUtility.GetSeparator(path);
			m_path = PathUtility.Normalize(path, m_seperator) ?? "";
			m_stringComparison = PathUtility.HasWinRoot(m_path)
				? StringComparison.OrdinalIgnoreCase
				: StringComparison.Ordinal;
		}

		internal RelativePath(SerializationInfo info, StreamingContext context)
		{
			m_path = info.GetString(nameof(m_path));
			m_seperator = info.GetChar(nameof(m_seperator));
			m_stringComparison = (StringComparison)info.GetInt32(nameof(m_stringComparison));
		}

		/// <summary>
		/// Allows paths to be combined using the forward slash operator 
		/// </summary>
		public static RelativePath operator /(RelativePath left, string right)
		{
			if (PathUtility.HasRoot(right))
			{
				throw new InvalidOperationException($"An relative path can't be combined with absolute path on the right hand side. " +
					$"Base is '{left.m_path}' and is trying to be combined with '{right}'/.");
			}
			string path = PathUtility.Combine(left, right);
			return new RelativePath(path, left.m_seperator);
		}
			

		/// <summary>
		/// Stops users from tryin to combine an absolute path on the right hand side of a 
		/// relative path.
		/// </summary>
		[Obsolete("A AbsolutePath can't be combined with a relative path on the right hand side. Switch parameters around.", true)]
		public static RelativePath operator /(RelativePath left, AbsolutePath right)
			=> throw new InvalidOperationException("A absolute path can't be used on the right hand side.");

		/// <summary>
		/// Allows us to convert from strings to an absolute path. 
		/// </summary>
		public static explicit operator RelativePath(string path)
			=> new RelativePath(path, null, true);

		/// <summary>
		/// Takes in a a string path and returns back a parsed
		/// out <see cref="RelativePath"/>. If the path can not be parsed
		/// an exception will be thrown.
		/// </summary>
		/// <param name="path">The path you want the relative path for</param>
		/// <returns>The parsed out path</returns>
		public static RelativePath Parse(string path)
			=> new RelativePath(path, null, true);

		/// <summary>
		/// Takes in a string path and a seperator and returns a parsed
		/// out relative path. If the path is not able to be parsed an exception will be
		/// thrown.
		/// </summary>
		/// <param name="path">The path you want to parse</param>
		/// <param name="seperator">The seperator that should be used</param>
		/// <returns>The parsed out path</returns>
		public static RelativePath Parse(string path, char seperator)
			=> new RelativePath(path, seperator, true);


		/// <summary>
		/// Attempts to parse a string into an <see cref="AbsolutePath"/> if it's successful 
		/// true is returned and the value is set, otherwise value is <see cref="Empty"/> and
		/// false if returned.
		/// </summary>
		/// <param name="path">The path you want to try parsing</param>
		/// <param name="value">The result if the parse worked.</param>
		/// <returns>True if it could be parsed and false if it could not</returns>
		public static bool TryParse(string path, out RelativePath value)
		{
			value = Empty;

			// It's better to not have to throw exceptions we we do the simple checks first 

			if (string.IsNullOrWhiteSpace(path))
				return false;

			if (PathUtility.HasRoot(path))
				return false;

			try
			{
				value = new RelativePath(path, null, true);
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
		public RelativePath GetParent()
		{
			return this / "..";
		}

		/// <summary>
		/// Get sif two paths are equal to each other
		/// </summary>
		public bool Equals(RelativePath other)
			=> string.Equals(other.m_path, m_path, m_stringComparison);

		/// <inheritdoc cref="System.Object"/>
		public override string ToString()
			=> m_path ?? "";

		/// <inheritdoc cref="System.Object"/>
		public override int GetHashCode()
			=> ToString().GetHashCode();

		/// <inheritdoc cref="IComparable{T}"/>
		public int CompareTo(RelativePath other)
			=> string.Compare(m_path, other.m_path, m_stringComparison);

		/// <inheritdoc cref="ISerializable"/>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(m_stringComparison), (int)m_stringComparison);
			info.AddValue(nameof(m_path), m_path);
			info.AddValue(nameof(m_seperator), m_seperator);
		}

		/// <summary>
		/// Returns back the raw string of this path. 
		/// </summary>
		public static implicit operator string?(RelativePath path)
			=> path.ToString();

		/// <summary>
		/// Allows absolute paths to be converted to a boolean if they are not empty
		/// </summary>
		/// <param name="path">The path to check</param>
		/// <returns>True if it has a value and false if it does not</returns>
		public static bool operator true(RelativePath path)
			=> !path.IsEmpty;

		/// <summary>
		/// Allows an <see cref="AbsolutePath"/> to be converted to a boolean value based off
		/// if it's empty.
		/// </summary>
		/// <param name="path">The path</param>
		/// <returns>true if the path is empty and false if it's not</returns>
		public static bool operator false(RelativePath path)
			=> path.IsEmpty;
	}
}
