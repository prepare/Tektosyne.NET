using System;
using System.IO;

namespace Tektosyne.IO {

    /// <summary>
    /// Provides auxiliary methods for <b>System.IO.Path</b>.</summary>

    public static class PathEx {
        #region AddDirectorySeparator

        /// <summary>
        /// Adds a trailing directory separator character to the specified file path.</summary>
        /// <param name="path">
        /// The file path to which to add a trailing directory separator character.</param>
        /// <returns>
        /// The specified <paramref name="path"/>, with a single trailing <see
        /// cref="Path.DirectorySeparatorChar"/> if neither this character nor <see
        /// cref="Path.AltDirectorySeparatorChar"/> were already present.</returns>
        /// <remarks>
        /// <b>RemoveDirectorySeparator</b> returns a string containing a single <see
        /// cref="Path.DirectorySeparatorChar"/> if the specified <paramref name="path"/> is a null
        /// reference or an empty string.</remarks>

        public static string AddDirectorySeparator(string path) {

            if (String.IsNullOrEmpty(path))
                return new String(Path.DirectorySeparatorChar, 1);

            // add directory separator character if missing
            int last = path.Length - 1;
            if (path[last] != Path.DirectorySeparatorChar &&
                path[last] != Path.AltDirectorySeparatorChar)
                path = path + Path.DirectorySeparatorChar;

            return path;
        }

        #endregion
        #region Equals

        /// <summary>
        /// Determines whether the two specified file paths are equivalent.</summary>
        /// <param name="firstPath">
        /// The first file path to compare.</param>
        /// <param name="secondPath">
        /// The second file path to compare.</param>
        /// <returns>
        /// <c>true</c> if the two file paths are equivalent, ignoring character case and directory
        /// separator variants.</returns>
        /// <remarks><para>
        /// <b>Equals</b> uses the following rules to test if the specified <paramref
        /// name="firstPath"/> equals the specified <paramref name="secondPath"/>:
        /// </para><list type="bullet"><item>
        /// Any trailing <see cref="Path.DirectorySeparatorChar"/> or <see
        /// cref="Path.AltDirectorySeparatorChar"/> characters are removed before the comparison.
        /// </item><item>
        /// <paramref name="firstPath"/> and <paramref name="secondPath"/> must have the same
        /// length.
        /// </item><item>
        /// All characters that match either <see cref="Path.DirectorySeparatorChar"/> or <see
        /// cref="Path.AltDirectorySeparatorChar"/> are considered equal.
        /// </item><item>
        /// All other characters are compared using the <see
        /// cref="StringComparison.OrdinalIgnoreCase"/> option.
        /// </item></list><para>
        /// If either argument is a null reference or an empty string, <b>Equals</b> returns
        /// <c>true</c> exactly if the other argument is also a null reference or an empty string;
        /// otherwise, <c>false</c>.</para></remarks>

        public static bool Equals(string firstPath, string secondPath) {

            // check for empty strings or null references
            if (String.IsNullOrEmpty(firstPath))
                return (String.IsNullOrEmpty(secondPath));
            else if (String.IsNullOrEmpty(secondPath))
                return false;

            // remove any trailing directory separator characters
            firstPath = RemoveDirectorySeparator(firstPath);
            secondPath = RemoveDirectorySeparator(secondPath);

            // check for different lengths
            if (firstPath.Length != secondPath.Length)
                return false;

            // normalize remaining directory separator characters
            firstPath = NormalizeDirectorySeparator(firstPath);
            secondPath = NormalizeDirectorySeparator(secondPath);

            // compare normalized paths, ignoring case and culture
            return firstPath.Equals(secondPath, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
        #region GetTempFileName

        /// <summary>
        /// Returns a unique name for a temporary file with the specified extension.</summary>
        /// <param name="extension">
        /// The new extension (with a leading period). Specify a null reference for no extension.
        /// </param>
        /// <returns>
        /// The full path to a uniquely named file with the specified <paramref name="extension"/>
        /// in the Windows directory for temporary files.</returns>
        /// <remarks><para>
        /// <b>GetTempFileName</b> performs the following steps to determine its return value:
        /// </para><list type="number"><item>
        /// Call <see cref="Path.GetRandomFileName"/> to obtain a random file name without a
        /// directory prefix or extension suffix.
        /// </item><item>
        /// Prepend the directory for temporary files returned by <see cref="Path.GetTempPath"/>.
        /// </item><item>
        /// Append the specified <paramref name="extension"/>.
        /// </item><item>
        /// Repeat from step #1 if a file already exists at that location; otherwise, return the
        /// resulting file name.
        /// </item></list></remarks>

        public static string GetTempFileName(string extension) {
            string path = null, tempDir = Path.GetTempPath();

            do {
                path = Path.GetRandomFileName();
                path = Path.Combine(tempDir, path);
                path = Path.ChangeExtension(path, extension);
            } while (File.Exists(path));

            return path;
        }

        #endregion
        #region NormalizeDirectorySeparator

        /// <summary>
        /// Normalizes all directory separator characters in the specified file path.</summary>
        /// <param name="path">
        /// The file path whose directory separator characters to normalize.</param>
        /// <returns>
        /// The specified <paramref name="path"/>, in which all occurrences of <see
        /// cref="Path.AltDirectorySeparatorChar"/> have been replaced with <see
        /// cref="Path.DirectorySeparatorChar"/>.</returns>
        /// <remarks>
        /// <b>NormalizeDirectorySeparator</b> returns an empty string if the specified <paramref
        /// name="path"/> is a null reference or an empty string.</remarks>

        public static string NormalizeDirectorySeparator(string path) {

            if (String.IsNullOrEmpty(path)) return "";

            return (path.IndexOf(Path.AltDirectorySeparatorChar) < 0 ? path :
                path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
        }

        #endregion
        #region RemoveDirectorySeparator

        /// <summary>
        /// Removes any trailing directory separator characters from the specified file path.
        /// </summary>
        /// <param name="path">
        /// The file path whose trailing directory separator characters to remove.</param>
        /// <returns>
        /// The specified <paramref name="path"/>, without any trailing <see
        /// cref="Path.DirectorySeparatorChar"/> or <see cref="Path.AltDirectorySeparatorChar"/>
        /// characters.</returns>
        /// <remarks>
        /// <b>RemoveDirectorySeparator</b> returns an empty string if the specified <paramref
        /// name="path"/> is a null reference or an empty string.</remarks>

        public static string RemoveDirectorySeparator(string path) {

            return (String.IsNullOrEmpty(path) ? "" :
                path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        #endregion
        #region Shorten

        /// <summary>
        /// Shortens the specified file path by the specified directory.</summary>
        /// <param name="path">
        /// The file path that may start with the specified <paramref name="directory"/> prefix.
        /// </param>
        /// <param name="directory">
        /// The directory prefix to remove from the specified <paramref name="path"/>.</param>
        /// <returns>
        /// The specified <paramref name="path"/> without the specified <paramref name="directory"/>
        /// prefix.</returns>
        /// <remarks><para>
        /// <b>Shorten</b> calls <see cref="Equals"/> to determine whether the beginning of the
        /// specified <paramref name="path"/> up to the length of <paramref name="directory"/>
        /// matches the specified <paramref name="directory"/>.
        /// </para><para>
        /// On success, <b>Shorten</b> returns an empty string if both arguments have the same
        /// length. Otherwise, if either the last character of <paramref name="directory"/> or the
        /// first character of the remaining part of <paramref name="path"/> matches
        /// <b>DirectorySeparatorChar</b> or <b>AltDirectorySeparatorChar</b>, <b>Shorten</b>
        /// returns the part of <paramref name="path"/> that follows this separator character.
        /// </para><para>
        /// <b>Shorten</b> returns the unmodified value of the specified <paramref name="path"/>
        /// under the following conditions:
        /// </para><list type="bullet"><item>
        /// <paramref name="path"/> is a null reference or an empty string.
        /// </item><item>
        /// <paramref name="directory"/> is a null reference or an empty string.
        /// </item><item>
        /// The length of <paramref name="directory"/> is greater than the length of <paramref
        /// name="path"/>.
        /// </item><item>
        /// <paramref name="path"/> does not start with <paramref name="directory"/>.
        /// </item><item>
        /// <paramref name="path"/> starts with <paramref name="directory"/> but the two parts are
        /// not separated by <see cref="Path.DirectorySeparatorChar"/> or <see
        /// cref="Path.AltDirectorySeparatorChar"/>.</item><para>
        /// <b>Shorten</b> never returns a null reference. If the method were to return <paramref
        /// name="path"/> and this argument is a null reference, <b>Shorten</b> will return an empty
        /// string instead.</para></list></remarks>

        public static string Shorten(string path, string directory) {

            // reflect path if either string is null or empty
            if (String.IsNullOrEmpty(path) ||
                String.IsNullOrEmpty(directory))
                return path ?? "";

            // reflect path if shorter than directory
            if (path.Length < directory.Length) return path;

            // test for full equality if same length
            if (path.Length == directory.Length) {
                if (Equals(path, directory)) return "";
                return path;
            }

            // reflect path if directory prefix different
            string prefix = path.Substring(0, directory.Length);
            if (!Equals(prefix, directory)) return path;

            char separator = Path.DirectorySeparatorChar;
            char altSeparator = Path.AltDirectorySeparatorChar;

            // add one if directory doesn’t end with separator
            int last = directory.Length - 1;
            if (path[last] != separator && path[last] != altSeparator) {
                ++last;

                // reflect path if no directory separator found
                if (path[last] != separator && path[last] != altSeparator) {
                    return path;
                }
            }

            // return path minus directory and separator
            return path.Substring(last + 1);
        }

        #endregion
    }
}
