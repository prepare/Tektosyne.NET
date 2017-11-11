using System;

namespace Tektosyne.Win32Api {

    /// <summary>
    /// Defines flags supplied to Simple MAPI calls.</summary>
    /// <remarks>
    /// <b>MapiFlags</b> defines all flag values that can be supplied to Simple MAPI function calls.
    /// Bitwise combinations are possible.</remarks>

    [Flags]
    public enum MapiFlags {
        #region MAPI_LOGON_UI

        /// <summary>
        /// A dialog should be displayed to prompt the user to log on if required.</summary>

        MAPI_LOGON_UI = 0x00000001,

        #endregion
        #region MAPI_NEW_SESSION

        /// <summary>
        /// An attempt should be made to create a new session rather than acquire the environment’s
        /// shared session.</summary>

        MAPI_NEW_SESSION = 0x00000002,

        #endregion
        #region MAPI_DIALOG

        /// <summary>
        /// A dialog should be displayed to prompt the user for recipients and other sending
        /// options.</summary>

        MAPI_DIALOG = 0x00000008,

        #endregion
        #region MAPI_EXTENDED

        /// <summary>
        /// Log on with extended capabilities.</summary>

        MAPI_EXTENDED = 0x00000020,

        #endregion
        #region MAPI_UNREAD_ONLY

        /// <summary>
        /// Only unread messages of the specified type should be enumerated.</summary>

        MAPI_UNREAD_ONLY = 0x00000020,

        #endregion
        #region MAPI_USE_DEFAULT

        /// <summary>
        /// The messaging subsystem should substitute the profile name of the default profile.
        /// </summary>

        MAPI_USE_DEFAULT = 0x00000040,

        #endregion
        #region MAPI_ENVELOPE_ONLY

        /// <summary>
        /// Only message headers should be retrieved.</summary>

        MAPI_ENVELOPE_ONLY = 0x00000040,

        #endregion
        #region MAPI_PEEK

        /// <summary>
        /// The retrieved messages should not be marked as read.</summary>

        MAPI_PEEK = 0x00000080,

        #endregion
        #region MAPI_GUARANTEE_FIFO

        /// <summary>
        /// The message identifiers returned should be in the order of time received.</summary>

        MAPI_GUARANTEE_FIFO = 0x00000100,

        #endregion
        #region MAPI_BODY_AS_FILE

        /// <summary>
        /// The message text should be sent as a file attachment.</summary>

        MAPI_BODY_AS_FILE = 0x00000200,

        #endregion
        #region MAPI_AB_NOMODIFY

        /// <summary>
        /// The address book dialog should be read-only, prohibiting changes.</summary>

        MAPI_AB_NOMODIFY = 0x00000400,

        #endregion
        #region MAPI_SUPPRESS_ATTACH

        /// <summary>
        /// The file attachments of incoming messages should not be copied.</summary>

        MAPI_SUPPRESS_ATTACH  = 0x00000800,

        #endregion
        #region MAPI_FORCE_DOWNLOAD

        /// <summary>
        /// An attempt should be made to download all of the user’s messages before returning.
        /// </summary>

        MAPI_FORCE_DOWNLOAD = 0x00001000,

        #endregion
        #region MAPI_LONG_MSGID

        /// <summary>
        /// The returned message identifier can be as long as 512 characters.</summary>

        MAPI_LONG_MSGID = 0x00004000,

        #endregion
        #region MAPI_PASSWORD_UI

        /// <summary>
        /// A dialog should be displayed to prompt the user for the profile password.</summary>

        MAPI_PASSWORD_UI = 0x00020000

        #endregion
    }
}
