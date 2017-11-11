namespace Tektosyne.Win32Api {

    /// <summary>
    /// Defines error codes returned by Simple MAPI calls.</summary>
    /// <remarks>
    /// <b>MapiError</b> defines all error codes that can be returned by Simple MAPI functions
    /// calls.</remarks>

    public enum MapiError {
        #region SUCCESS_SUCCESS

        /// <summary>
        /// The operation completed successfully.</summary>

        SUCCESS_SUCCESS = 0,

        #endregion
        #region MAPI_E_USER_ABORT

        /// <summary>
        /// The user cancelled one of the dialog boxes.</summary>

        MAPI_E_USER_ABORT = 1,

        #endregion
        #region MAPI_E_FAILURE

        /// <summary>
        /// One or more unspecified errors occurred.</summary>

        MAPI_E_FAILURE = 2,

        #endregion
        #region MAPI_E_LOGIN_FAILURE

        /// <summary>
        /// There was no default logon, and the user failed to log on successfully when the logon
        /// dialog was displayed.</summary>

        MAPI_E_LOGIN_FAILURE = 3,

        #endregion
        #region MAPI_E_DISK_FULL

        /// <summary>
        /// An attachment could not be written to a temporary file because there was not enough
        /// space on the disk.</summary>

        MAPI_E_DISK_FULL = 4,

        #endregion
        #region MAPI_E_INSUFFICIENT_MEMORY

        /// <summary>
        /// There was insufficient memory to proceed.</summary>

        MAPI_E_INSUFFICIENT_MEMORY = 5,

        #endregion
        #region MAPI_E_ACCESS_DENIED

        /// <summary>
        /// An attempt to copy a profile or message service failed.</summary>

        MAPI_E_ACCESS_DENIED = 6,

        #endregion
        #region MAPI_E_TOO_MANY_SESSIONS

        /// <summary>
        /// The user had too many sessions open simultaneously.</summary>

        MAPI_E_TOO_MANY_SESSIONS = 8,

        #endregion
        #region MAPI_E_TOO_MANY_FILES

        /// <summary>
        /// There were too many file attachments.</summary>

        MAPI_E_TOO_MANY_FILES = 9,

        #endregion
        #region MAPI_E_TOO_MANY_RECIPIENTS

        /// <summary>
        /// There were too many recipients.</summary>

        MAPI_E_TOO_MANY_RECIPIENTS = 10,

        #endregion
        #region MAPI_E_ATTACHMENT_NOT_FOUND

        /// <summary>
        /// The specified attachment was not found.</summary>

        MAPI_E_ATTACHMENT_NOT_FOUND = 11,

        #endregion
        #region MAPI_E_ATTACHMENT_OPEN_FAILURE

        /// <summary>
        /// The specified attachment could not be opened.</summary>

        MAPI_E_ATTACHMENT_OPEN_FAILURE = 12,

        #endregion
        #region MAPI_E_ATTACHMENT_WRITE_FAILURE

        /// <summary>
        /// An attachment could not be written to a temporary file. Check directory permissions.
        /// </summary>

        MAPI_E_ATTACHMENT_WRITE_FAILURE = 13,

        #endregion
        #region MAPI_E_UNKNOWN_RECIPIENT

        /// <summary>
        /// A recipient did not appear in the address list.</summary>

        MAPI_E_UNKNOWN_RECIPIENT = 14,

        #endregion
        #region MAPI_E_BAD_RECIPTYPE

        /// <summary>
        /// The type of a recipient was not <see cref="MapiRecipClass.MAPI_TO"/>, <see
        /// cref="MapiRecipClass.MAPI_CC"/>, or <see cref="MapiRecipClass.MAPI_BCC"/>.</summary>

        MAPI_E_BAD_RECIPTYPE = 15,

        #endregion
        #region MAPI_E_NO_MESSAGES

        /// <summary>
        /// A matching message could not be found.</summary>

        MAPI_E_NO_MESSAGES = 16,

        #endregion
        #region MAPI_E_INVALID_MESSAGE

        /// <summary>
        /// An invalid message identifier was passed in the <b>lpszMessageID</b> parameter.
        /// </summary>

        MAPI_E_INVALID_MESSAGE = 17,

        #endregion
        #region MAPI_E_TEXT_TOO_LARGE

        /// <summary>
        /// The text in the message was too large.</summary>

        MAPI_E_TEXT_TOO_LARGE = 18,

        #endregion
        #region MAPI_E_INVALID_SESSION

        /// <summary>
        /// An invalid session handle was passed in the <b>lhSession</b> parameter.</summary>

        MAPI_E_INVALID_SESSION = 19,

        #endregion
        #region MAPI_E_TYPE_NOT_SUPPORTED

        /// <summary>
        /// Type not supported (undocumented error code).</summary>

        MAPI_E_TYPE_NOT_SUPPORTED = 20,

        #endregion
        #region MAPI_E_AMBIGUOUS_RECIPIENT

        /// <summary>
        /// A recipient matched more than one of the recipient descriptor structures and <see
        /// cref="MapiFlags.MAPI_DIALOG"/> was not set.</summary>

        MAPI_E_AMBIGUOUS_RECIPIENT = 21,

        #endregion
        #region MAPI_E_MESSAGE_IN_USE

        /// <summary>
        /// Message in use (undocumented error code).</summary>

        MAPI_E_MESSAGE_IN_USE = 22,

        #endregion
        #region MAPI_E_NETWORK_FAILURE

        /// <summary>
        /// A network error prevented successful completion of the operation.</summary>

        MAPI_E_NETWORK_FAILURE = 23,

        #endregion
        #region MAPI_E_INVALID_EDITFIELDS

        /// <summary>
        /// The value of the <b>nEditFields</b> parameter was outside the range of 0 through 4.
        /// </summary>

        MAPI_E_INVALID_EDITFIELDS = 24,

        #endregion
        #region MAPI_E_INVALID_RECIPS

        /// <summary>
        /// One or more recipients were invalid or did not resolve to any address.</summary>

        MAPI_E_INVALID_RECIPS = 25,

        #endregion
        #region MAPI_E_NOT_SUPPORTED

        /// <summary>
        /// The operation was not supported by the underlying messaging system.</summary>

        MAPI_E_NOT_SUPPORTED = 26

        #endregion
    }
}
