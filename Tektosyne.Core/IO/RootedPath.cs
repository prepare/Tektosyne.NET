using System;
using System.IO;

namespace Tektosyne.IO {

    /// <summary>
    /// Provides a file path that may be relative to a root folder.</summary>
    /// <remarks><para>
    /// <b>RootedPath</b> wraps a <see cref="RootedPath.RootFolder"/> and a file path that may be
    /// located below that root folder. The <see cref="RootedPath.AbsolutePath"/> and <see
    /// cref="RootedPath.RelativePath"/> properties return different transformations of the wrapped
    /// file path, suitable either for I/O operations or for text display.
    /// </para><para>
    /// <b>AbsolutePath</b> automatically prepends <b>RootFolder</b> if the file path is specified
    /// as a relative path, and <b>RelativePath</b> automatically strips <b>RootFolder</b> if the
    /// current <b>AbsolutePath</b> is located below that folder. <b>RootedPath</b> does not check
    /// whether any of the specified folders and paths actually exist in the file system.
    /// </para><para>
    /// <b>RootedPath</b> is an immutable reference type, just like <see cref="String"/>. The
    /// methods <see cref="RootedPath.Change"/> and <see cref="RootedPath.Clear"/> return a new
    /// <b>RootedPath</b> instance, leaving the current instance unchanged.</para></remarks>

    [Serializable]
    public class RootedPath: ICloneable, IEquatable<RootedPath> {
        #region RootedPath(RootedPath)

        /// <overloads>
        /// Initializes a new instance of the <see cref="RootedPath"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="RootedPath"/> class that is a shallow copy
        /// of the specified instance.</summary>
        /// <param name="path">
        /// The <see cref="RootedPath"/> object whose property values should be copied to the new
        /// instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is a null reference.</exception>
        /// <remarks>
        /// This "copy constructor" does not need to perform a deep copy as all properties are
        /// backed by immutable <see cref="String"/> objects.</remarks>

        public RootedPath(RootedPath path) {
            if (path == null)
                ThrowHelper.ThrowArgumentNullException("path");

            AbsolutePath = path.AbsolutePath;
            RootFolder = path.RootFolder;
        }

        #endregion
        #region RootedPath(String)

        /// <summary>
        /// Initializes a new instance of the <see cref="RootedPath"/> class with the specified root
        /// folder.</summary>
        /// <param name="rootFolder">
        /// The root folder for the <see cref="RootedPath"/>.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="rootFolder"/> is not an absolute file path.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="rootFolder"/> is a null reference or an empty string.</exception>
        /// <remarks><para>
        /// If the specified <paramref name="rootFolder"/> does not end with a directory separator
        /// character, a single <see cref="Path.DirectorySeparatorChar"/> is appended before the 
        /// value is assigned to the <see cref="RootFolder"/> property.
        /// </para><para>
        /// <see cref="AbsolutePath"/> is set to the same value as <paramref name="rootFolder"/>.
        /// This means that <see cref="RelativePath"/> will return an empty string.</para></remarks>

        public RootedPath(string rootFolder): this(rootFolder, null) { }

        #endregion
        #region RootedPath(String, String)

        /// <summary>
        /// Initializes a new instance of the <see cref="RootedPath"/> class with the specified root
        /// folder and current file path.</summary>
        /// <param name="rootFolder">
        /// The root folder for the <see cref="RootedPath"/>.</param>
        /// <param name="filePath">
        /// An absolute or relative file path used to initialize the remaining properties. This
        /// argument may be a null reference or an empty string.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="rootFolder"/> is not an absolute file path.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="rootFolder"/> is a null reference or an empty string.</exception>
        /// <remarks><para>
        /// If the specified <paramref name="rootFolder"/> does not end with a directory separator
        /// character, a single <see cref="Path.DirectorySeparatorChar"/> is appended before the 
        /// value is assigned to the <see cref="RootFolder"/> property.
        /// </para><para>
        /// If the specified <paramref name="filePath"/> is a null reference, an empty string, or an
        /// absolute file path that is equivalent to <paramref name="rootFolder"/>, <see
        /// cref="AbsolutePath"/> is initialized to <paramref name="rootFolder"/>. This means that
        /// <see cref="RelativePath"/> will return an empty string.
        /// </para><para>
        /// If <paramref name="filePath"/> is an absolute file path that is <em>not</em> equivalent
        /// to <paramref name="rootFolder"/>, <see cref="AbsolutePath"/> is initialized to <paramref
        /// name="filePath"/>. Otherwise, if <paramref name="filePath"/> is a relative file path,
        /// <see cref="AbsolutePath"/>  is initialized to the combination of <paramref
        /// name="rootFolder"/> and <paramref name="filePath"/>. In either case, <see
        /// cref="RelativePath"/> will return <paramref name="filePath"/>.</para></remarks>

        public RootedPath(string rootFolder, string filePath) {

            if (String.IsNullOrEmpty(rootFolder))
                ThrowHelper.ThrowArgumentNullOrEmptyException("rootFolder");
            if (!Path.IsPathRooted(rootFolder))
                ThrowHelper.ThrowArgumentException("rootFolder", Strings.ArgumentNotRooted);

            rootFolder = PathEx.AddDirectorySeparator(rootFolder);
            RootFolder = String.Intern(rootFolder);

            if (String.IsNullOrEmpty(filePath))
                AbsolutePath = rootFolder;
            else {
                AbsolutePath = Path.Combine(rootFolder, filePath);
                if (PathEx.Equals(AbsolutePath, rootFolder))
                    AbsolutePath = rootFolder;
            }
        }

        #endregion
        #region Public Properties
        #region AbsolutePath

        /// <summary>
        /// The absolute file path wrapped by the <see cref="RootedPath"/>.</summary>
        /// <remarks>
        /// <b>AbsolutePath</b> always returns a non-empty absolute file path.</remarks>

        public readonly string AbsolutePath;

        #endregion
        #region DirectoryName

        /// <summary>
        /// Gets the directory information for <see cref="AbsolutePath"/>.</summary>
        /// <value>
        /// The result of <see cref="Path.GetDirectoryName"/> for <see cref="AbsolutePath"/>.
        /// </value>
        /// <remarks>
        /// <b>DirectoryName</b> never returns a null reference or an empty string since <see
        /// cref="AbsolutePath"/> always contains a directory prefix.</remarks>

        public string DirectoryName {
            get { return Path.GetDirectoryName(AbsolutePath); }
        }

        #endregion
        #region FileName

        /// <summary>
        /// Gets the file name and extension of <see cref="AbsolutePath"/>.</summary>
        /// <value>
        /// The result of <see cref="Path.GetFileName"/> for <see cref="AbsolutePath"/>.</value>
        /// <remarks>
        /// <b>FileName</b> never returns a null reference, but it returns an empty string if <see
        /// cref="AbsolutePath"/> ends with a directory or volume separator character.</remarks>

        public string FileName {
            get { return Path.GetFileName(AbsolutePath); }
        }

        #endregion
        #region IsEmpty

        /// <summary>
        /// Gets a value indicating whether the <see cref="RootedPath"/> wraps an empty relative
        /// path.</summary>
        /// <value>
        /// <c>true</c> if <see cref="RelativePath"/> is an empty string; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// <b>IsEmpty</b> is <c>true</c> exactly if <see cref="AbsolutePath"/> and <see
        /// cref="RootFolder"/> contain equivalent file paths. That is the case if both properties
        /// contain either identical string values or different string values that evaluate to the
        /// same file path.</remarks>

        public bool IsEmpty {
            get { return (RelativePath.Length == 0); }
        }

        #endregion
        #region RelativePath

        /// <summary>
        /// Gets the relative path wrapped by the <see cref="RootedPath"/>.</summary>
        /// <value>
        /// The value of the <see cref="AbsolutePath"/> property, shortened by the value of the
        /// <see cref="RootFolder"/> property if the absolute path is located below that folder.
        /// </value>
        /// <remarks><para>
        /// <b>RelativePath</b> never returns a null reference, but it may return an empty string if
        /// <see cref="AbsolutePath"/> and <see cref="RootFolder"/> contain equivalent file paths.
        /// </para><para>
        /// <b>RelativePath</b> calls <see cref="PathEx.Shorten"/> to determine its value; please
        /// see there for further details on the path shortening algorithm.</para></remarks>

        public string RelativePath {
            get {
                return (AbsolutePath == RootFolder ? "" :
                    PathEx.Shorten(AbsolutePath, RootFolder));
            }
        }

        #endregion
        #region RootFolder

        /// <summary>
        /// The root folder for the <see cref="RootedPath"/>.</summary>
        /// <remarks>
        /// <b>RootFolder</b> always returns a non-empty absolute file path that ends with a
        /// directory separator character. <b>RootFolder</b> is stripped from the <see
        /// cref="RelativePath"/> if the <see cref="AbsolutePath"/> is located below that folder.
        /// </remarks>

        public readonly string RootFolder;

        #endregion
        #endregion
        #region Change

        /// <summary>
        /// Changes the file path wrapped by the <see cref="RootedPath"/>.</summary>
        /// <param name="filePath">
        /// The absolute or relative file path to wrap.</param>
        /// <returns>
        /// A new <see cref="RootedPath"/> instance with the same <see cref="RootFolder"/> as this
        /// instance, and that wraps the specified <paramref name="filePath"/>.</returns>
        /// <remarks>
        /// <b>Change</b> has the same effect as <see cref="Clear"/> if the specified <paramref
        /// name="filePath"/> is a null reference, an empty string, or an absolute file path that is
        /// equivalent to <see cref="RootFolder"/>.</remarks>

        public RootedPath Change(string filePath) {
            return new RootedPath(RootFolder, filePath);
        }

        #endregion
        #region Clear

        /// <summary>
        /// Clears the file path wrapped by the <see cref="RootedPath"/>.</summary>
        /// <returns>
        /// A new <see cref="RootedPath"/> instance with the same <see cref="RootFolder"/> as this
        /// instance, and that wraps an empty file path.</returns>

        public RootedPath Clear() {
            return new RootedPath(RootFolder);
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="RootedPath"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the results of <see cref="String.GetHashCode"/> for the <see
        /// cref="RootFolder"/> and <see cref="AbsolutePath"/> properties.</remarks>

        public override int GetHashCode() {
            unchecked { return RootFolder.GetHashCode() ^ AbsolutePath.GetHashCode(); }
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="RootedPath"/>.</summary>
        /// <returns>
        /// The value of the <see cref="AbsolutePath"/> property.</returns>

        public override string ToString() {
            return AbsolutePath;
        }

        #endregion
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="RootedPath"/> instances have the same value.</summary>
        /// <param name="x">
        /// The first <see cref="RootedPath"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="RootedPath"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(RootedPath, RootedPath)"/> method to test
        /// the two <see cref="RootedPath"/> instances for value equality.</remarks>

        public static bool operator ==(RootedPath x, RootedPath y) {
            return Equals(x, y);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="RootedPath"/> instances have different values.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="RootedPath"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="RootedPath"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is different from the value of
        /// <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(RootedPath, RootedPath)"/> method to test
        /// the two <see cref="RootedPath"/> instances for value inequality.</remarks>

        public static bool operator !=(RootedPath x, RootedPath y) {
            return !Equals(x, y);
        }

        #endregion
        #region ICloneable Members

        /// <summary>
        /// Creates a shallow copy of the <see cref="RootedPath"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="RootedPath"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> invokes the "copy constructor", <see cref="RootedPath(RootedPath)"/>, to
        /// create a shallow copy of the current instance.</remarks>

        public object Clone() {
            return new RootedPath(this);
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="RootedPath"/> instances have the same value.
        /// </overloads>
        /// <summary>
        /// Determines whether this <see cref="RootedPath"/> instance and a specified object, which
        /// must be a <see cref="RootedPath"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="RootedPath"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="RootedPath"/> instance and
        /// its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="RootedPath"/> instance,
        /// or an instance of a derived class, <b>Equals</b> invokes the strongly-typed <see
        /// cref="Equals(RootedPath)"/> overload to test the two instances for value equality.
        /// </remarks>

        public override bool Equals(object obj) {
            return Equals(obj as RootedPath);
        }

        #endregion
        #region Equals(RootedPath)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="RootedPath"/> have the same
        /// value.</summary>
        /// <param name="path">
        /// A <see cref="RootedPath"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="path"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="RootFolder"/> and <see
        /// cref="AbsolutePath"/> properties of the two <see cref="RootedPath"/> instances to test
        /// for value equality.</remarks>

        public bool Equals(RootedPath path) {

            if (Object.ReferenceEquals(path, this)) return true;
            if (Object.ReferenceEquals(path, null)) return false;

            return (RootFolder == path.RootFolder
                && AbsolutePath == path.AbsolutePath);
        }

        #endregion
        #region Equals(RootedPath, RootedPath)

        /// <summary>
        /// Determines whether two specified <see cref="RootedPath"/> instances have the same value.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="RootedPath"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="RootedPath"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(RootedPath)"/> overload to test
        /// the two <see cref="RootedPath"/> instances for value equality.</remarks>

        public static bool Equals(RootedPath x, RootedPath y) {

            if (Object.ReferenceEquals(x, null))
                return Object.ReferenceEquals(y, null);

            return x.Equals(y);
        }

        #endregion
        #endregion
    }
}
