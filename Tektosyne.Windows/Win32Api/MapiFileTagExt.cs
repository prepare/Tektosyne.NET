using System;
using System.Runtime.InteropServices;

namespace Tektosyne.Win32Api {

    /// <summary>
    /// Specifies a message attachment’s type at its creation and its current form of encoding so
    /// that it can be restored to its original type at its destination.</summary>
    /// <remarks>
    /// This type mirrors the <b>MapiFileTagExt</b> (Simple MAPI) structure defined in the Platform
    /// SDK. Please refer to the Microsoft Platform SDK documentation for details.</remarks>

    [CLSCompliant(false)]
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MapiFileTagExt {
        #region ulReserved

        /// <summary>
        /// Reserved; must be zero.</summary>

        public uint ulReserved;

        #endregion
        #region cbTag

        /// <summary>
        /// The size, in bytes, of the value defined by the <see cref="lpTag"/> member.</summary>

        public uint cbTag;

        #endregion
        #region lpTag

        /// <summary>
        /// A pointer to an X.400 object identifier indicating the type of the attachment in its
        /// original form, for example "Microsoft Excel worksheet".</summary>

        public IntPtr lpTag;

        #endregion
        #region cbEncoding

        /// <summary>
        /// The size, in bytes, of the value defined by the <see cref="lpEncoding"/> member.
        /// </summary>

        public uint cbEncoding;

        #endregion
        #region lpEncoding

        /// <summary>
        /// A pointer to an X.400 object identifier indicating the form in which the attachment is
        /// currently encoded, for example MacBinary, UUENCODE, or binary.</summary>

        public IntPtr lpEncoding;

        #endregion
    }
}
