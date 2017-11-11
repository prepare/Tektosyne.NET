using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Tektosyne.IO {

    /// <summary>
    /// Provides auxiliary methods for <b>System.IO</b>.</summary>

    public static class IOUtility {
        #region CopyAndReplace

        /// <summary>
        /// Copies the specified text file while recursively replacíng file inclusion tags with the
        /// referenced files.</summary>
        /// <param name="source">
        /// The text file to copy to <paramref name="target"/> while expanding any embedded file
        /// inclusion tags.</param>
        /// <param name="target">
        /// The text file to copy <paramref name="source"/> to, along with any included files.
        /// </param>
        /// <param name="fileGroup">
        /// The name of the match <see cref="Group"/> within the <paramref name="includeSearch"/>
        /// pattern that identifies file names.</param>
        /// <param name="includeSearch">
        /// The <see cref="Regex"/> search pattern describing the file inclusion tags embedded in
        /// <paramref name="source"/>.</param>
        /// <param name="includePrefix">
        /// The <see cref="Regex.Replace"/> pattern to write to <paramref name="target"/> before
        /// file inclusion begins.</param>
        /// <param name="includeSuffix">
        /// The <see cref="Regex.Replace"/> pattern to write to <paramref name="target"/> after file
        /// inclusion ends.</param>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="source"/>, <paramref name="target"/>, or <paramref name="fileGroup"/> is
        /// a null reference or an empty string.</exception>
        /// <remarks><para>
        /// <b>CopyAndReplace</b> reads the specified <paramref name="source"/> text file, line by
        /// line, and matches each line against the specified <paramref name="includeSearch"/>
        /// pattern. If no match is found, the line is simply copied to the specified <paramref
        /// name="target"/> file; otherwise, the specified <paramref name="fileGroup"/> within the
        /// <see cref="Regex"/> match is assumed to indicate another text file whose contents will
        /// be read and copied to <paramref name="target"/> in the same fashion. The <paramref
        /// name="source"/> line that matched <paramref name="includeSearch"/> is discarded. Any
        /// existing file of the same name as <paramref name="target"/> is overwritten.
        /// </para><para>
        /// The <paramref name="includePrefix"/> and <paramref name="includeSuffix"/> parameters
        /// allow the creation of "marker lines" bracketing any included file content. In formalized
        /// input text such as XML or source code, these arguments might evaluate to some kind of
        /// comments in order to aid later reconstruction of the file inclusion process.
        /// </para><para>
        /// Either or both of the <paramref name="includePrefix"/> and <paramref
        /// name="includeSuffix"/> arguments may be null references or empty strings to suppress the
        /// creation of marker lines. Valid <paramref name="includePrefix"/> and <paramref
        /// name="includeSuffix"/> parameters are passed to the <see cref="Regex.Replace"/> method
        /// of the specified <paramref name="includeSearch"/> pattern. They are applied to any line
        /// producing a non-empty <see cref="Regex.Match"/> result for <paramref
        /// name="includeSearch"/>, i.e. any line containing a file inclusion tag. This allows
        /// reusing the specified <paramref name="fileGroup"/> and any other named groups in
        /// <paramref name="includeSearch"/> to create marker lines containing the name of the
        /// included file and other variable data.</para></remarks>
        /// <example><para>
        /// Consider an XML file containing file inclusion tags of the following form:
        /// </para><code>
        /// &lt;include href="section.xml"/&gt;
        /// </code><para>
        /// We want to replace all such lines with the following sequence of lines:
        /// </para><code>
        /// &lt;!-- include_open "section.xml --&gt;
        /// </code><code>
        /// <em>...contents of "section.xml" follow here...</em>
        /// </code><code>
        /// &lt;!-- include_close "section.xml --&gt;
        /// </code><para>
        /// Assuming "input.xml" as the name of the input file and "output.xml" as the name of the
        /// output file, the following invocation of <b>CopyAndReplace</b> would achieve the desired
        /// result:
        /// </para><code>
        /// CopyAndReplace("input.xml", "output.xml", "file",
        ///     new Regex(@"&lt;include href=""(?&lt;file&gt;[^""]*)""/&gt;"),
        ///     @"&lt;!-- include_open ""${file}"" --&gt;",
        ///     @"&lt;!-- include_close ""${file}"" --&gt;");
        /// </code><para>
        /// Note how the group identifier "file" is used both in the search expression and in the
        /// parameters for the marker lines.</para></example>

        public static void CopyAndReplace(string source, string target, string fileGroup,
            Regex includeSearch, string includePrefix, string includeSuffix) {

            if (String.IsNullOrEmpty(source))
                ThrowHelper.ThrowArgumentNullOrEmptyException("source");

            if (String.IsNullOrEmpty(target))
                ThrowHelper.ThrowArgumentNullOrEmptyException("target");

            if (String.IsNullOrEmpty(fileGroup))
                ThrowHelper.ThrowArgumentNullOrEmptyException("fileGroup");

            // create stream writer for output file
            using (StreamWriter writer = new StreamWriter(target, false, Encoding.UTF8)) {

                // recursively process all include tags
                List<String> openFiles = new List<String>();

                RecursiveCopy(source, writer, fileGroup, includeSearch,
                    includePrefix, includeSuffix, openFiles);

                Debug.Assert(openFiles.Count == 0, "Open file list not empty");
            }
        }

        #endregion
        #region SearchDirectory

        /// <summary>
        /// Searches a single directory for a file matching the specified pattern.</summary>
        /// <param name="directory">
        /// The directory to search for <paramref name="pattern"/>.</param>
        /// <param name="pattern">
        /// The file name pattern to search for. The pattern may contain wildcards but no directory
        /// prefixes.</param>
        /// <returns><para>
        /// The first file name in <paramref name="directory"/> that matches the specified <paramref
        /// name="pattern"/>.
        /// </para><para>-or-</para><para>
        /// A null reference if no match was found or <paramref name="directory"/> does not exist.
        /// </para></returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="directory"/> or <paramref name="pattern"/> is a null reference or an
        /// empty string.</exception>
        /// <remarks><para>
        /// <b>SearchDirectory</b> prefixes the returned file name, if any, with the specified
        /// <paramref name="directory"/>.
        /// </para><para>
        /// The search <paramref name="pattern"/> may contain any wildcards accepted by the file
        /// system, as described in <see cref="Directory.GetFiles(String, String)"/>.
        /// </para></remarks>

        public static string SearchDirectory(string directory, string pattern) {

            if (String.IsNullOrEmpty(directory))
                ThrowHelper.ThrowArgumentNullOrEmptyException("directory");

            if (String.IsNullOrEmpty(pattern))
                ThrowHelper.ThrowArgumentNullOrEmptyException("pattern");

            // don’t search nonexistent directory
            if (!Directory.Exists(directory))
                return null;

            /*
             * Undocumented behavior: Directory.GetFiles prefixes
             * all returned file names with the specified path.
             */

            string[] files = Directory.GetFiles(directory, pattern);
            return (files.Length == 0 ? null : files[0]);
        }

        #endregion
        #region SearchDirectoryTree

        /// <summary>
        /// Searches a directory tree for a file matching the specified pattern.</summary>
        /// <param name="rootDirectory">
        /// The root directory of the directory tree to search for <paramref name="pattern"/>.
        /// </param>
        /// <param name="pattern">
        /// The file name pattern to search for. The pattern may contain wildcards but no directory
        /// prefixes.</param>
        /// <returns><para>
        /// The first file name in the directory tree starting with <paramref name="rootDirectory"/>
        /// that matches the specified <paramref name="pattern"/>.
        /// </para><para>-or-</para><para>
        /// A null reference if no match was found or <paramref name="rootDirectory"/> does not
        /// exist.</para></returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="rootDirectory"/> or <paramref name="pattern"/> is a null reference or an
        /// empty string.</exception>
        /// <remarks><para>
        /// <b>SearchDirectoryTree</b> performs a depth-first search starting with the specified
        /// <paramref name="rootDirectory"/>, but always checks all files in the present directory
        /// before descending another level.
        /// </para><para>
        /// <b>SearchDirectoryTree</b> prefixes the returned file name, if any, with the specified
        /// <paramref name="rootDirectory"/> and any intermediate subdirectories.
        /// </para><para>
        /// The search <paramref name="pattern"/> may contain any wildcards accepted by the file
        /// system, as described in <see cref="Directory.GetFiles(String, String)"/>.
        /// </para></remarks>

        public static string SearchDirectoryTree(string rootDirectory, string pattern) {

            // search root directory for specified file
            string file = SearchDirectory(rootDirectory, pattern);
            if (file != null) return file;

            // don’t search nonexistent directory
            if (!Directory.Exists(rootDirectory))
                return null;

            /*
             * Undocumented behavior: Directory.GetDirectories prefixes
             * all returned directory names with the specified path.
             */

            // search subdirectory tree for specified file
            string[] dirs = Directory.GetDirectories(rootDirectory);
            foreach (string dir in dirs) {
                file = SearchDirectoryTree(dir, pattern);
                if (file != null) return file;
            }

            // file not found
            return null;
        }

        #endregion
        #region Private Methods
        #region RecursiveCopy

        /// <summary>
        /// Copies the specified text file while recursively replacing file inclusion tags with the
        /// referenced files.</summary>
        /// <param name="source">
        /// The text file to copy to <paramref name="writer"/>, while expanding any embedded file
        /// inclusion tags.</param>
        /// <param name="writer">
        /// The <see cref="StreamWriter"/> to copy <paramref name="source"/> to, along with any
        /// included files.</param>
        /// <param name="fileGroup">
        /// The name of the match <see cref="Group"/> within the <paramref name="includeSearch"/>
        /// pattern that identifies file names.</param>
        /// <param name="includeSearch">
        /// The <see cref="Regex"/> search pattern describing the file inclusion tags embedded in
        /// <paramref name="source"/>.</param>
        /// <param name="includePrefix">
        /// The <see cref="Regex.Replace"/> pattern to write to <paramref name="writer"/> before
        /// file inclusion begins.</param>
        /// <param name="includeSuffix">
        /// The <see cref="Regex.Replace"/> pattern to write to <paramref name="writer"/> after file
        /// inclusion ends.</param>
        /// <param name="openFiles">
        /// The names of all files which are still open and should be skipped when encountered again
        /// in a <paramref name="fileGroup"/> match.</param>
        /// <remarks><para>
        /// <b>RecursiveCopy</b> uses the <paramref name="openFiles"/> list to keep track of all
        /// files opened by previous recursion levels. Any inclusion tag specifying one of these
        /// files is silently ignored to prevent endless loops. The initial caller should supply an
        /// empty <see cref="List{String}"/> for <paramref name="openFiles"/>, and will receive an
        /// empty <see cref="List{String}"/> upon return.
        /// </para><para>
        /// Please refer to <see cref="CopyAndReplace"/> for details on other parameters.
        /// </para></remarks>

        private static void RecursiveCopy(string source, StreamWriter writer,
            string fileGroup, Regex includeSearch, string includePrefix,
            string includeSuffix, List<String> openFiles) {

            // create stream reader for current input file
            using (StreamReader reader = new StreamReader(source)) {
                string line;
                while ((line = reader.ReadLine()) != null) {

                    // append line to output stream if no match
                    if (!includeSearch.IsMatch(line)) {
                        writer.WriteLine(line);
                        continue;
                    }

                    // get named group with file path
                    Match includeMatch = includeSearch.Match(line);
                    string includeFile = includeMatch.Groups[fileGroup].Value;

                    // check if file is already open
                    if (openFiles.Contains(includeFile))
                        continue;

                    // append to list of open files
                    openFiles.Add(includeFile);

                    // insert prefix marker if specified
                    if (!String.IsNullOrEmpty(includePrefix))
                        writer.WriteLine(includeSearch.Replace(line, includePrefix));

                    // read embedded file recursively
                    RecursiveCopy(includeFile, writer, fileGroup, includeSearch,
                        includePrefix, includeSuffix, openFiles);

                    // insert suffix marker if specified
                    if (!String.IsNullOrEmpty(includeSuffix))
                        writer.WriteLine(includeSearch.Replace(line, includeSuffix));

                    // remove from list of open files
                    openFiles.Remove(includeFile);
                }
            }
        }

        #endregion
        #endregion
    }
}
