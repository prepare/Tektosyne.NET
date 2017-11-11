using System;
using System.Runtime.InteropServices;

namespace Tektosyne.Win32Api {

    /// <summary>
    /// Contains information about a file containing a message attachment stored as a temporary
    /// file.</summary>
    /// <remarks><para>
    /// The temporary file can contain a static OLE object, an embedded OLE object, an embedded
    /// message, and other types of files.
    /// </para><para>
    /// This type mirrors the <b>MapiFileDesc</b> (Simple MAPI) structure defined in the Platform
    /// SDK. Please refer to the Microsoft Platform SDK documentation for details.</para></remarks>

    [CLSCompliant(false)]
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MapiFileDesc {
        #region ulReserved

        /// <summary>
        /// Reserved; must be zero.</summary>

        public uint ulReserved;

        #endregion
        #region flFlags

        /// <summary>
        /// A bitmask of attachment flags.</summary>
        /// <remarks><para>
        /// The following flags can be set:
        /// </para><list type="bullet"><item>
        /// <term><see cref="MapiFileFlags.MAPI_OLE"/></term>
        /// <description>The attachment is an OLE object. If <b>MAPI_OLE_STATIC</b> is also set, the
        /// attachment is a static OLE object. If <b>MAPI_OLE_STATIC</b> is not set, the attachment
        /// is an embedded OLE object.</description>
        /// </item><item>
        /// <term><see cref="MapiFileFlags.MAPI_OLE_STATIC"/></term>
        /// <description>The attachment is a static OLE object.</description>
        /// </item></list><para>
        /// If neither flag is set, the attachment is treated as a data file.</para></remarks>

        [MarshalAs(UnmanagedType.U4)]
        public MapiFileFlags flFlags;

        #endregion
        #region nPosition

        /// <summary>
        /// An integer used to indicate where in the message text to render the attachment.
        /// </summary>
        /// <remarks>
        /// Attachments replace the character found at a certain position in the message text. That
        /// is, attachments replace the character in the <see cref="MapiMessage"/> structure field
        /// <c>NoteText[nPosition]</c>. A value of – 1 (0xFFFFFFFF) means the attachment position is
        /// not indicated; the client application will have to provide a way for the user to access
        /// the attachment.</remarks>

        public uint nPosition;

        #endregion
        #region lpszPathName

        /// <summary>
        /// The fully qualified path of the attached file.</summary>
        /// <remarks>
        /// This path should include the disk drive letter and directory name.</remarks>

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpszPathName;

        #endregion
        #region lpszFileName

        /// <summary>
        /// The attachment filename seen by the recipient.</summary>
        /// <remarks>
        /// <b>lpszFileName</b> may differ from the filename in the <see cref="lpszPathName"/>
        /// member if temporary files are being used. If the <b>lpszFileName</b> member is a null
        /// reference or an empty string, the filename from <see cref="lpszPathName"/> is used.
        /// </remarks>

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpszFileName;

        #endregion
        #region lpFileType

        /// <summary>
        /// A pointer to the attachment file type.</summary>
        /// <remarks>
        /// The attachment file type can be represented with a <see cref="MapiFileTagExt"/>
        /// structure. A value of <see cref="IntPtr.Zero"/> indicates an unknown file type or a file
        /// type determined by the operating system.</remarks>

        public IntPtr lpFileType;

        #endregion
    }
}
