using System;
using System.IO;

namespace Seed.IO
{
    /// <summary>
    /// Contains utility methods for working with file paths
    /// </summary>
    public static class PathUtility
    {
        /// The separator used for windows paths
        /// </summary>
        public const char WINDOWS_SEPARATOR = '\\';
        /// <summary>
        /// The separator used in linux paths
        /// </summary>
        public const char LINUX_SEPARATOR = '/';
        /// <summary>
        /// The list of path separators used on all platforms
        /// </summary>
        public static readonly char[] PathSeparators;

        static PathUtility()
        {
            PathSeparators = new[] { WINDOWS_SEPARATOR, LINUX_SEPARATOR };
        }

        /// <summary>
        /// Creates a new directory in the temp folder for you to use. 
        /// </summary>
        /// <returns>An absolute path to a temp directory</returns>
        public static AbsolutePath GetTempDirectory()
        {
            string tempFile = Path.GetTempFileName();
            string tempDirectory = Path.ChangeExtension(tempFile, null);
            AbsolutePath result = new AbsolutePath(tempDirectory);
            _ = Directory.CreateDirectory(result);

            return result;
        }

        /// <summary>
        /// Gets the relative path between two paths
        /// </summary>
        public static RelativePath GetRelative(string source, string target)
        {
            string normalizedPath = Normalize(source);
            if (source.Length < target.Length)
            {
                return RelativePath.Default;
            }

            string relativePath = normalizedPath.Substring(target.Length);
            return new RelativePath(relativePath);
        }

        /// <summary>
        /// Takes in a string path and resolve all directory navigators ('.', '..') and returns
        /// back a constant format with all slashes facing the same direction. 
        /// </summary>
        /// <param name="path">The path you want to normalize</param>
        /// <param name="separator">The optional separator you want to use</param>
        /// <returns></returns>
        public static string Normalize(string path, char? separator = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }

            path = Environment.ExpandEnvironmentVariables(path);

            separator ??= GetSeparator(path);
            string? root = GetPathRoot(path);
            bool isRooted = IsRooted(path);
            string tail = root == null ? path : path.Substring(root.Length);
            string[] components = tail.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
            string[] normalized = new string[components.Length];
            int index = 0;

            foreach (string component in components)
            {
                switch (component)
                {
                    case ".":
                        continue;
                    case "..":
                        if (index == 0)
                        {
                            if (isRooted)
                            {
                                throw new ArgumentException($"Cannot normalize '{path}' beyond path root.");
                            }
                        }
                        else if (normalized[index - 1] != "..")
                        {
                            index--;
                            continue;
                        }
                        break;
                }

                normalized[index] = component;
                index++;
            }

            return Combine(root, string.Join(separator.ToString(), normalized, 0, index), separator);
        }


        /// <summary>
        /// Combines two paths into one.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The result of the combination</returns>
        public static string Combine(string? left, string? right, char? separator = null)
        {
            left = Trim(left);
            right = Trim(right);

            if (IsRooted(right))
            {
                throw new ArgumentException("The second path must not be rooted to be combined", nameof(right));
            }

            if (string.IsNullOrWhiteSpace(left))
            {
                return right;
            }

            if (string.IsNullOrWhiteSpace(right))
            {
                return HasWindowsRoot(left) ? $@"{left}{WINDOWS_SEPARATOR}" : left;
            }

            ValidateChosenSeparator(left, separator);
            if (separator is null)
            {
                separator = GetSeparator(left);
            }
            else
            {
                return Join(left, right, separator.Value);
            }

            if (HasWindowsRoot(left))
            {
                return Join(left, right, WINDOWS_SEPARATOR);
            }

            return HasUnixRoot(left) ? Join(left, right, LINUX_SEPARATOR) : Join(left, right, separator.Value);
        }


        /// <summary>
        /// Joins two paths together and adds the separator if it's not already added.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        private static string Join(string left, string right, char separator)
        {
            int length = left.Length;
            char[] result = new char[left.Length + right.Length + 1];


            if (left[length - 1] == separator)
            {
                length--;
            }

            left.CopyTo(0, result, 0, length);

            result[length] = separator;
            length++;

            int startIndex = 0;
            if (right[0] == separator)
            {
                startIndex++;
            }
            right.CopyTo(startIndex, result, length, right.Length - startIndex);
            length += right.Length - startIndex;

            return new string(result, 0, length);
        }

        /// <summary>
        /// Validates that the separator sent in matches the path we path if it's rooted. If no
        /// separator is specified we just escape.
        /// </summary>
        internal static void ValidateChosenSeparator(string path, char? separator = null)
        {
            if (separator == null || string.IsNullOrEmpty(path))
            {
                return;
            }

            if (HasWindowsRoot(path) && separator != WINDOWS_SEPARATOR)
            {
                throw new ArgumentException($"For Windows-rooted paths the separator must be '{WINDOWS_SEPARATOR}'.");
            }

            if (HasUnixRoot(path) && separator != LINUX_SEPARATOR)
            {
                throw new ArgumentException($"For Linux-rooted paths the separator must be '{LINUX_SEPARATOR}'.");
            }
        }

        /// <summary>
        /// Gets the separator char based off the rooted directory, if the path is not rooted we just return the default for the current platofrm.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">path - We are unable to get the separator from a null path</exception>
        public static char GetSeparator(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path), "We are unable to get the separator from a null path");
            }

            if (HasWindowsRoot(path))
            {
                return WINDOWS_SEPARATOR;
            }

            return HasUnixRoot(path) ? LINUX_SEPARATOR : Path.DirectorySeparatorChar;
        }


        /// <summary>
        /// Given a path we return back it's root 
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string? GetPathRoot(string? path)
        {
            if (path == null)
            {
                return null;
            }

            if (HasWindowsRoot(path))
            {
                return path.Substring(0, 2);
            }

            return HasUnixRoot(path) ? LINUX_SEPARATOR.ToString() : null;
        }

        /// <summary>
        /// Trims the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public static string Trim(string? path)
        {
            return path == null
                ? string.Empty
                : HasUnixRoot(path)
                ? path
                : path.TrimEnd(PathSeparators);
        }

        /// <summary>
        /// Determines whether the path is rooted to windows
        /// </summary>
        /// <param name="path">The path.</param>
        public static bool HasWindowsRoot(string path)
        {
            return path?.Length > 1 && char.IsLetter(path[0]) && path[1] == ':';
        }

        /// <summary>
        /// Determines whether a given path is rooted to linux.
        /// </summary>
        /// <param name="path">The path.</param>
        public static bool HasUnixRoot(string path)
        {
            return path?.Length > 0 && path[0] == LINUX_SEPARATOR;
        }

        /// <summary>
        /// Determines whether the specified path is rooted.
        /// </summary>
        /// <param name="path">The path you want to check.</param>
        public static bool IsRooted(string path)
        {
            return HasWindowsRoot(path) || HasUnixRoot(path);
        }
    }
}
