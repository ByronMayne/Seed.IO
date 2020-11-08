#nullable enable
using System;
using System.IO;

namespace Seed.IO
{
	public static class PathUtility
	{
		internal const char WINDOWS_SEPARATOR = '\\';
		internal const char UNIX_SEPARATOR = '/';
		internal static readonly char[] PATH_SEPARATORS = { WINDOWS_SEPARATOR, UNIX_SEPARATOR };

		/// <summary>
		/// Splits a path into it's individual components.
		/// </summary>
		/// <param name="path">The path you want to split up</param>
		/// <returns></returns>
		public static string[] GetComponents(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return Array.Empty<string>();
			}
			return path.Split(PATH_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
		}



		/// <summary>
		/// Takes a string and returns back it in a consistent format with braces matching
		/// the platform and resolving parent '..' and current directory '.' markers.  
		/// </summary>
		/// <returns></returns>
		public static string Normalize(string path, char? separator = null)
		{
			if (string.IsNullOrEmpty(path))
			{
				return "";
			}

			ValidateChosenSeparator(path, separator);

			path ??= string.Empty;
			separator ??= GetSeparator(path);
			string? root = GetPathRoot(path);
			bool isAbsolute = !string.IsNullOrEmpty(root);
			string tail = root == null ? path : path.Substring(root.Length);
			string[] components = tail.Split(PATH_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
			string[] normalized = new string[components.Length];
			int index = 0;
			for (int i = 0; i < components.Length; i++)
			{
				string component = components[i];

				// If not the first component and it's just a '.' we can ignore it
				if (index != 0 && component.Length == 1 && component[0] == '.')
					continue;


				// It is an upwards directory
				if (component.Length == 2 && component[0] == '.' && component[1] == '.')
				{
					// This case we are at the root and we are asking for the parent directory. Like 'C:/..' this is an impossible path
					if (index == 0)
					{
						if (isAbsolute)
						{
							throw new InvalidOperationException($"Can not normalize an absolute path beyond it's root. The path was '{path}'.");
						}
					}
					else if (index > 0)
					{
						string previous = normalized[index - 1];

						if (previous != ".." && previous != ".")
						{
							index--;
							continue;
						}
					}
				}
				normalized[index] = component;
				index++;
			}

			return Combine(root, string.Join(separator.ToString(), normalized, 0, index), separator);
		}

		/// <summary>
		/// Takes too paths and combines them into one. 
		/// </summary>
		internal static string Combine(string? left, string? right, char? separator = null)
		{
			left = Trim(left);
			right = Trim(right);

			if (HasRoot(right))
			{
				throw new ArgumentException("The second path must not be rooted to be combined", nameof(right));
			}

			if (string.IsNullOrWhiteSpace(left))
				return right;

			if (string.IsNullOrWhiteSpace(right))
				return !HasWinRoot(left) ? left : $@"{left}\";

			ValidateChosenSeparator(left, separator);
			separator = separator ?? GetSeparator(left);

			if (HasWinRoot(left))
				return $@"{left}\{right}";
			if (HasUnixRoot(left))
			{
				if (left.Length == 1)
				{
					return $"{left}{right}";
				}
				return $"{left}/{right}";
			}

			return $"{left}{separator}{right}";
		}

		/// <summary>
		/// Returns back if a path is rooted.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool HasRoot(string path)
		{
			return GetPathRoot(path) != null;
		}

		/// <summary>
		/// Takes in a path and returns the seperator for it. If one could
		/// not be found the default for the C# environment is returned. 
		/// </summary>
		public static char GetSeparator(string path)
		{
			if (HasWinRoot(path))
				return WINDOWS_SEPARATOR;
			if (HasUnixRoot(path))
				return UNIX_SEPARATOR;
			return Path.DirectorySeparatorChar;
		}

		/// <summary>
		/// Returns back the root of our path if we have one
		/// </summary>
		public static string? GetPathRoot(string path)
		{
			if (path == null)
			{
				return null;
			}

			if (HasUnixRoot(path))
			{
				return UNIX_SEPARATOR.ToString();
			}

			if (HasWinRoot(path))
			{
				return path.Substring(0, 2);
			}

			return null;
		}

		/// <summary>
		/// Validates that the separator sent in matches the path we path if it's rooted. 
		/// </summary>
		public static void ValidateChosenSeparator(string path, char? separator = null)
		{
			if (separator == null || string.IsNullOrEmpty(path))
			{
				return;
			}

			if (HasWinRoot(path) && separator != WINDOWS_SEPARATOR)
			{
				throw new ArgumentException($"For Windows-rooted paths the separator must be '{WINDOWS_SEPARATOR}'.");
			}

			if (HasUnixRoot(path) && separator != UNIX_SEPARATOR)
			{
				throw new ArgumentException($"For Unix-rooted paths the separator must be '{UNIX_SEPARATOR}'.");
			}
		}

		/// <summary>
		/// Returns back if our path has a Unix root. 
		/// </summary>
		public static bool HasUnixRoot(string path)
		{
			return path?.Length > 0 && path[0] == UNIX_SEPARATOR;
		}

		/// <summary>
		/// Returns back if our path has a Windows root. 
		/// </summary>
		public static bool HasWinRoot(string path)
		{
			return path?.Length > 1 && char.IsLetter(path[0]) && path[1] == ':';
		}

		/// <summary>
		/// Removes a path of all extra characters 
		/// </summary>
		private static string Trim(string? path)
		{
			if (path == null)
				return "";

			return HasUnixRoot(path)
			  ? path
			  : path.TrimEnd(PATH_SEPARATORS);
		}
	}
}
