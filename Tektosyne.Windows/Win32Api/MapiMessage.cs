using System;
using System.Runtime.InteropServices;

namespace Tektosyne.Win32Api {

    /// <summary>
    /// Contains information about a message.</summary>
    /// <remarks>
    /// This type mirrors the <b>MapiMessage</b> (Simple MAPI) structure defined in the Platform
    /// SDK. Please refer to the Microsoft Platform SDK documentation for details.</remarks>

    [CLSCompliant(false)]
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MapiMessage: IDisposable {
        #region ulReserved

        /// <summary>
        /// Reserved; must be zero.</summary>

        public uint ulReserved;

        #endregion
        #region lpszSubject

        /// <summary>
        /// A <see cref="String"/> describing the message subject.</summary>
        /// <remarks>
        /// The message subject is typically limited to 256 characters or less. If this member is a
        /// null reference or an empty string, the user has not entered subject text.</remarks>

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpszSubject;

        #endregion
        #region lpszNoteText

        /// <summary>
        /// A <see cref="String"/> containing the message text.</summary>
        /// <remarks>
        /// If this member is a null reference or an empty string, there is no message text.
        /// </remarks>

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpszNoteText;

        #endregion
        #region lpszMessageType

        /// <summary>
        /// A <see cref="String"/> indicating a non-IPM type of message.</summary>
        /// <remarks>
        /// Client applications can select message types for their non-IPM messages. Clients that
        /// only support IPM messages can ignore the <b>lpszMessageType</b> member when reading
        /// messages and set it to a null reference or an empty string when sending messages.
        /// </remarks>

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpszMessageType;

        #endregion
        #region lpszDateReceived

        /// <summary>
        /// A <see cref="String"/> indicating the date when the message was received.</summary>
        /// <remarks>
        /// The format is <c>YYYY/MM/DD HH:MM</c>, using a 24-hour clock.</remarks>

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpszDateReceived;

        #endregion
        #region lpszConversationID

        /// <summary>
        /// A <see cref="String"/> identifying the conversation thread to which the message belongs.
        /// </summary>
        /// <remarks>
        /// Some messaging systems can ignore and not return this member.</remarks>

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpszConversationID;

        #endregion
        #region flFlags

        /// <summary>
        /// A bitmask of message status flags.</summary>
        /// <remarks><para>
        /// The following flags can be set:
        /// </para><list type="bullet"><item>
        /// <term><see cref="MapiMessageFlags.MAPI_RECEIPT_REQUESTED"/></term>
        /// <description>A receipt notification is requested. Client applications set this flag when
        /// sending a message.</description>
        /// </item><item>
        /// <term><see cref="MapiMessageFlags.MAPI_SENT"/></term>
        /// <description>The message has been sent.</description>
        /// </item><item>
        /// <term><see cref="MapiMessageFlags.MAPI_UNREAD"/></term>
        /// <description>The message has not been read.</description>
        /// </item></list></remarks>

        [MarshalAs(UnmanagedType.U4)]
        public MapiMessageFlags flFlags;

        #endregion
        #region lpOriginator

        /// <summary>
        /// A pointer to a <see cref="MapiRecipDesc"/> structure containing information about the
        /// sender of the message.</summary>
        /// <remarks><para>
        /// <b>lpOriginator</b> is initialized to an empty <see cref="SafeGlobalHandle"/> object.
        /// Use <see cref="SafeGlobalHandle.AllocateHandle"/> to allocate an unmanaged memory block.
        /// </para><para>
        /// Any unmanaged memory allocated for <b>lpOriginator</b> is released by <see
        /// cref="Dispose"/> or else by the <see cref="SafeGlobalHandle"/> finalizer.
        /// </para></remarks>

        public SafeGlobalHandle lpOriginator = new SafeGlobalHandle();

        #endregion
        #region nRecipCount

        /// <summary>
        /// The number of message recipient structures in the array pointed to by the <see
        /// cref="lpRecips"/> member.</summary>
        /// <remarks>
        /// A value of zero indicates that no recipients are included.</remarks>

        public uint nRecipCount;

        #endregion
        #region lpRecips

        /// <summary>
        /// A pointer to an array of <see cref="MapiRecipDesc"/> structures, each containing
        /// information about a message recipient.</summary>
        /// <remarks><para>
        /// <b>lpRecips</b> is initialized to an empty <see cref="SafeGlobalHandle"/> object. Use
        /// <see cref="SafeGlobalHandle.AllocateHandle"/> to allocate an unmanaged memory block.
        /// </para><para>
        /// Any unmanaged memory allocated for <b>lpRecips</b> is released by <see cref="Dispose"/>
        /// or else by the <see cref="SafeGlobalHandle"/> finalizer.</para></remarks>

        public SafeGlobalHandle lpRecips = new SafeGlobalHandle();

        #endregion
        #region nFileCount

        /// <summary>
        /// The number of structures describing file attachments in the array pointed to by the <see
        /// cref="lpFiles"/> member.</summary>
        /// <remarks>
        /// A value of zero indicates that no file attachments are included.</remarks>

        public uint nFileCount;

        #endregion
        #region lpFiles

        /// <summary>
        /// A pointer to an array of <see cref="MapiFileDesc"/> structures, each containing
        /// information about a file attachment.</summary>
        /// <remarks><para>
        /// <b>lpFiles</b> is initialized to an empty <see cref="SafeGlobalHandle"/> object. Use
        /// <see cref="SafeGlobalHandle.AllocateHandle"/> to allocate an unmanaged memory block.
        /// </para><para>
        /// Any unmanaged memory allocated for <b>lpFiles</b> is released by <see cref="Dispose"/>
        /// or else by the <see cref="SafeGlobalHandle"/> finalizer.</para></remarks>

        public SafeGlobalHandle lpFiles = new SafeGlobalHandle();

        #endregion
        #region IDisposable Members

        /// <summary>
        /// Releases all resources used by the <see cref="MapiMessage"/> object.</summary>
        /// <remarks>
        /// <b>Dispose</b> releases any unmanaged memory allocated for <see cref="lpOriginator"/>,
        /// <see cref="lpRecips"/>, and <see cref="lpFiles"/>.</remarks>

        public void Dispose() {

            if (lpOriginator != null) lpOriginator.Dispose();
            if (lpRecips != null) lpRecips.Dispose();
            if (lpFiles != null) lpFiles.Dispose();
        }

        #endregion
    }
}
