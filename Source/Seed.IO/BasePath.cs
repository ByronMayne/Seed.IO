#nullable enable
using System;

namespace Seed.IO
{
    public abstract class BasePath : IEquatable<BasePath>
    {
        protected readonly string? m_path;

        protected BasePath(string path)
        {
            m_path = PathUtility.Normalize(path);
        }

        /// <summary>
        /// Returns back the hash code for our path.
        /// </summary>
        public override int GetHashCode()
        {
            return m_path?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Returns back the path we are representing. 
        /// </summary>
        public override string? ToString()
        {
            return m_path;
        }

        /// <summary>
        /// Compairs if two paths are pointing to the same value. 
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is BasePath basePath)
            {
                return string.Equals(basePath.m_path, m_path);
            }
            return false;
        }

        /// <summary>
        /// Get sif two paths are equal to each other
        /// </summary>
        public bool Equals(BasePath other)
        {
            if (other == null) return false;
            return string.Equals(other.m_path, m_path, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns back the raw string of this path. 
        /// </summary>
        public static implicit operator string?(BasePath path)
        {
            return path?.ToString();
        }
    }
}
