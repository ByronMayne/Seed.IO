#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace Seed.IO
{
    /// <summary>
    /// A relative path is a way to specify the location of a directory relative to another directory
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public readonly struct RelativePath : IComparable<RelativePath>, IEquatable<RelativePath>, ISerializable
    {
        public readonly string Value;
        public readonly bool CaseSensitive;

        /// <summary>
        /// Gets the default value for an absolute path 
        /// </summary>
        public static RelativePath Default { get; }

        /// <summary>
        /// Get if the <see cref="AbsolutePath"/> is currently the default value
        /// </summary>
        public bool IsDefault => this == Default;

        static RelativePath()
        {
            Default = new RelativePath(".", false);
        }

        public RelativePath(string value) : this(PathUtility.Normalize(value)!, true)
        {
            if(value == null)
            {
                throw new ArgumentNullException(nameof(value), "You must provide a non-null value for a relative path");
            }
        }

        private RelativePath(string value, bool validate)
        {
            if (validate)
            {
                if (string.IsNullOrWhiteSpace(nameof(value)))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (PathUtility.IsRooted(value))
                {
                    throw new ArgumentException($"Relative paths must not be rooted, the one provided was '{value}'.");
                }
            }
            Value = value;
            CaseSensitive = !PathUtility.HasWindowsRoot(value);
        }

        /// <summary>
        /// Used to allow absolute paths to be read and written to xaml.
        /// </summary>
        private RelativePath(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            Value = serializationInfo.GetString(nameof(Value));
            CaseSensitive = serializationInfo.GetBoolean(nameof(CaseSensitive));
        }

        /// <summary>
        /// Allows for combining paths using the devision operator 
        /// </summary>
        public static RelativePath operator /(RelativePath left, string right)
            => new RelativePath(PathUtility.Combine(left, right));

        /// <summary>
        /// Allows two absolute paths to check if they are equal
        /// </summary>
        public static bool operator ==(RelativePath left, RelativePath right)
            => left.Equals(right);

        /// <summary>
        /// Allows two absolute paths to check if they are not equal
        /// </summary>
        public static bool operator !=(RelativePath left, RelativePath right)
         => !left.Equals(right);

        /// <summary>
        /// Attempts to parse a string into an <see cref="RelativePath"/>
        /// </summary>
        /// <param name="path">The path to parse</param>
        /// <param name="relativePath">The result if it could be parsed</param>
        /// <returns>True if the result could parse otherwise false</returns>
        public static bool TryParse(string path, out RelativePath relativePath)
            => TryParse(path, false, out relativePath);

        /// <summary>
        /// Attempts to parse a string into an <see cref="RelativePath"/>
        /// </summary>
        /// <param name="path">The path to parse</param>
        /// <param name="expandVaraibles">If true the enviroment variables will be expanded using <see cref="Environment.ExpandEnvironmentVariables(string)"/></param>
        /// <param name="relativePath">The result if it could be parsed</param>
        /// <returns>True if the result could parse otherwise false</returns>
        public static bool TryParse(string path, bool expandVaraibles, out RelativePath relativePath)
        {
            relativePath = Default;

            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            if (expandVaraibles)
            {
                path = Environment.ExpandEnvironmentVariables(path);
            }

            if (Path.IsPathRooted(path))
            {
                return false;
            }

            relativePath = new RelativePath(path, true);
            return true;
        }

        /// <summary>
        /// Allows us to convert from strings to an absolute value. 
        /// </summary>
        public static explicit operator RelativePath(string path)
        {
            if (!PathUtility.IsRooted(path))
            {
                throw new ArgumentException($"The value 'value' must be rooted");
            }
            return new RelativePath(path);
        }

        /// <summary>
        /// Converts an <see cref="RelativePath"/> to a <see cref="string"/>
        /// </summary>
        /// <param name="relativePath">The absolute path to convert</param>
        public static implicit operator string(RelativePath relativePath)
            => relativePath.Value;

        /// <summary>
        /// Returns the parent of this value if it exists. 
        /// </summary>
        public RelativePath GetParent()
        {
            return this / "..";
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(RelativePath other)
        {
            StringComparison stringComparison = CaseSensitive || other.CaseSensitive
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            return string.Equals(other.Value, Value, stringComparison);
        }

        /// <inheritdoc cref="IComparable{T}"/>
        public int CompareTo(RelativePath other)
            => Value.CompareTo(other.Value);

        /// <inheritdoc cref="object"/>
        public override string ToString()
            => Value;

        /// <inheritdoc cref="object"/>
        public override int GetHashCode()
        {
            int hashCode = 1005732101;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + CaseSensitive.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc cref="ISerializable"/>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Value), Value);
            info.AddValue(nameof(CaseSensitive), CaseSensitive);
        }

        private string GetDebuggerDisplay()
            => Value;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            return obj is RelativePath relativePath
                ? Equals(relativePath)
                : false;
        }
    }
}
