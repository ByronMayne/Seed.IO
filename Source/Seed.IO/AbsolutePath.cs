using Seed.IO.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace Seed.IO
{
    /// <summary>
    /// An absolute value refers to the complete details needed to locate a file or 
    /// folder, starting from the root element and ending with the other subdirectories.
    /// </summary>
    [DebuggerDisplay("{Value, nq}")]
    [TypeConverter(typeof(AbsolutePathTypeConverter))]
    public readonly struct AbsolutePath : IComparable<AbsolutePath>, IEquatable<AbsolutePath>, ISerializable
    {
        public readonly string Value;
        public readonly bool CaseSensitive;

        /// <summary>
        /// Gets the default value for an absolute path 
        /// </summary>
        public static AbsolutePath Default { get; }

        /// <summary>
        ///  Gets the fully qualified path of the current working directory. 
        /// </summary>
        public static AbsolutePath CurrentDirectory
            => new AbsolutePath(Environment.CurrentDirectory);

        /// <summary>
        /// Gets the directory on disk where the entry entryAssembly is located
        /// </summary>
        public static AbsolutePath EntryAssemblyDirectory { get; }

        /// <summary>
        /// Get if the <see cref="AbsolutePath"/> is currently the default value
        /// </summary>
        public bool IsDefault => !string.IsNullOrWhiteSpace(Value);

        static AbsolutePath()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            EntryAssemblyDirectory = new AbsolutePath(Path.GetDirectoryName(entryAssembly.Location));
            Default = new AbsolutePath("", false);
        }

        public AbsolutePath(string value) : this(PathUtility.Normalize(value)!, true)
        { }

        private AbsolutePath(string value, bool validate)
        {
            if (validate)
            {
                if (string.IsNullOrWhiteSpace(nameof(value)))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!PathUtility.IsRooted(value))
                {
                    throw new ArgumentException($"Absolute path must be rooted, the one provided was '{value}'.");
                }
            }
            Value = value;
            CaseSensitive = !PathUtility.HasWindowsRoot(value);
        }

        /// <summary>
        /// Used to allow absolute paths to be read and written to xaml.
        /// </summary>
        private AbsolutePath(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            Value = serializationInfo.GetString(nameof(Value));
            CaseSensitive = serializationInfo.GetBoolean(nameof(CaseSensitive));
        }

        /// <summary>
        /// Allows for combining paths using the division operator 
        /// </summary>
        public static AbsolutePath operator /(AbsolutePath left, string right)
            => new AbsolutePath(PathUtility.Combine(left, right));

        /// <summary>
        /// Allows two absolute paths to check if they are equal
        /// </summary>
        public static bool operator ==(AbsolutePath left, AbsolutePath right)
            => left.Equals(right);

        /// <summary>
        /// Allows two absolute paths to check if they are not equal
        /// </summary>
        public static bool operator !=(AbsolutePath left, AbsolutePath right)
         => !left.Equals(right);

        /// <summary>
        /// Attempts to parse a string into an <see cref="AbsolutePath"/>
        /// </summary>
        /// <param name="path">The path to parse</param>
        /// <param name="absolutePath">The result if it could be parsed</param>
        /// <returns>True if the result could parse otherwise false</returns>
        public static bool TryParse(string path, out AbsolutePath absolutePath)
            => TryParse(path, false, out absolutePath);

        /// <summary>
        /// Gets the directory where where the assembly of the provided type is located
        /// </summary>
        /// <typeparam name="T">The type to get the assembly directory from</typeparam>
        public static AbsolutePath AssemblyDirectoryOfType<T>()
            => new AbsolutePath(typeof(T).Assembly.Location);

        /// <summary>
        /// Attempts to parse a string into an <see cref="AbsolutePath"/>
        /// </summary>
        /// <param name="path">The path to parse</param>
        /// <param name="expandVariables">If true the environment variables will be expanded using <see cref="Environment.ExpandEnvironmentVariables(string)"/></param>
        /// <param name="absolutePath">The result if it could be parsed</param>
        /// <returns>True if the result could parse otherwise false</returns>
        public static bool TryParse(string path, bool expandVariables, out AbsolutePath absolutePath)
        {
            absolutePath = Default;

            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            if (expandVariables)
            {
                path = Environment.ExpandEnvironmentVariables(path);
            }

            if (!Path.IsPathRooted(path))
            {
                return false;
            }

            absolutePath = new AbsolutePath(path, true);
            return true;
        }

        /// <summary>
        /// Allows us to convert from strings to an absolute value. 
        /// </summary>
        public static explicit operator AbsolutePath(string path)
        {
            if (!PathUtility.IsRooted(path))
            {
                throw new ArgumentException($"The value 'value' must be rooted");
            }
            return new AbsolutePath(path);
        }

        /// <summary>
        /// Converts an <see cref="AbsolutePath"/> to a <see cref="string"/>
        /// </summary>
        /// <param name="absolutePath">The absolute path to convert</param>
        public static implicit operator string(AbsolutePath absolutePath)
            => absolutePath.Value;

        /// <summary>
        /// Returns the parent of this value if it exists. 
        /// </summary>
        public AbsolutePath GetParent()
        {
            return this / "..";
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        public bool Equals(AbsolutePath other)
        {
            StringComparison stringComparison = CaseSensitive || other.CaseSensitive
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            return string.Equals(other.Value, Value, stringComparison);
        }

        /// <inheritdoc cref="IComparable{T}"/>
        public int CompareTo(AbsolutePath other)
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
            return obj is AbsolutePath absolutePath
                ? Equals(absolutePath)
                : false;
        }
    }
}
