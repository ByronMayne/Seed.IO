#nullable enable
using System;

namespace Seed.IO
{
    /// <summary>
    /// A relative path is a way to specify the location of a directory relative to another directory
    /// </summary>
    public sealed class RelativePath : BasePath
    {
        public RelativePath(string path) : base(path)
        {
            if (PathUtility.GetPathRoot(path) != null)
                throw new ArgumentException("A relative path must not be rooted.");
        }

        public static RelativePath operator /(RelativePath left, string right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));

            return new RelativePath(PathUtility.Combine(left, right));
        }

        /// <summary>
        /// Returns the parent of this path if it exists. 
        /// </summary>
        public RelativePath GetParent()
        {
            return this / "..";
        }
    }
}
