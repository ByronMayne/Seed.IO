using System;

namespace Seed.IO
{
    /// <summary>
    /// An absolute path refers to the complete details needed to locate a file or 
    /// folder, starting from the root element and ending with the other subdirectories.
    /// </summary>
    public sealed class AbsolutePath : BasePath
    {
        public AbsolutePath(string path) : base(path)
        {
        }


        public static AbsolutePath operator /(AbsolutePath left, string right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return new AbsolutePath(PathUtility.Combine(left, right));
        }

        public static AbsolutePath operator -(AbsolutePath left, AbsolutePath right)
        {
            if(left.m_path.EndsWith(right.m_path))
            {
                return new AbsolutePath(left.m_path.Substring(0, right.m_path.Length));
            }
            return left;
        }


        /// <summary>
        /// Allows us to convert from strings to an absolute path. 
        /// </summary>
        public static explicit operator AbsolutePath(string path)
        {
            if (path is null)
                return null;

            if(!PathUtility.HasRoot(path))
            {
                throw new ArgumentException($"The path 'path' must be rooted");
            }
            return new AbsolutePath(path);

        }
        /// <summary>
        /// Returns the parent of this path if it exists. 
        /// </summary>
        public AbsolutePath GetParent()
        {
            return this / "..";
        }
    }
}
