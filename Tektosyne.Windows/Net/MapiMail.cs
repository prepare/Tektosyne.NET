using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Tektosyne.Win32Api;

namespace Tektosyne.Net {

    /// <summary>
    /// Provides support for the Simple MAPI e-mail protocol.</summary>
    /// <remarks><para>
    /// The e-mail support provided by the standard classes of the .NET Framework requires certain
    /// Windows system files that are not available on consumer versions of the Windows operating
    /// system. <b>MapiMail</b> relies on the Simple MAPI protocol which is available on all Windows
    /// versions supported by the .NET Framework and which is fully supported by all Microsoft
    /// e-mail clients, including the free Outlook Express that comes with MS Internet Explorer.
    /// </para><para>
    /// However, users of non-Microsoft e-mail clients might have to perform additional installation
    /// steps to enable Simple MAPI support. Moreover, such clients usually do not support any
    /// Simple MAPI features other than sending e-mail. Setup instructions for an application using
    /// the <b>MapiMail</b> class should notify the user of these possibilities.
    /// </para><note type="implementnotes">
    /// Some MAPI functions may change the application’s current working directory (CWD). All
    /// <b>MapiMail</b> methods that invoke MAPI functions therefore record the CWD on entry and
    /// restore it on exit.</note></remarks>

    public static class MapiMail {
        #region Address

        /// <summary>
        /// Allows the user to select recipients from the Simple MAPI address book.</summary>
        /// <returns>
        /// An <see cref="Array"/> of <see cref="MapiAddress"/> instances holding the selected
        /// address book names and the corresponding e-mail addresses.</returns>
        /// <exception cref="MapiException">
        /// <see cref="Mapi.MAPIAddress"/> indicated an error.</exception>
        /// <remarks><para>
        /// <b>Address</b> shows a dialog allowing the user to select e-mail recipients from an
        /// address book, using the Win32 API call <see cref="Mapi.MAPIAddress"/> which is part of
        /// the Simple MAPI protocol. Only the "To" recipient type is available.
        /// </para><para>
        /// <b>Address</b> returns an empty array if the user cancelled the dialog or did not select
        /// any recipients. No <see cref="MapiException"/> is generated in this case.
        /// </para><note type="caution">
        /// <para>Non-Microsoft e-mail clients rarely support <b>MAPIAddress</b>. Some might report
        /// an error while others might show a nonfunctional dialog or return invalid data. There is
        /// no workaround other than using a Microsoft program as the default e-mail client.
        /// </para><para>
        /// Also note that Simple MAPI does not specify the order in which <b>MAPIAddress</b>
        /// returns the selected recipients. <b>Address</b> maintains the selection order for
        /// Outlook Express, but the order may be reversed or completely arbitrary for other e-mail
        /// clients.</para></note></remarks>

        public static MapiAddress[] Address() {

            // remember current working directory
            string currentDir = Directory.GetCurrentDirectory();

            // prepare recipient list data
            uint nNewRecips = 0;
            SafeMapiHandle lpNewRecips = null;

            try {
                // invoke MAPIAddress to create a recipient list
                MapiFlags flags = MapiFlags.MAPI_LOGON_UI;
                MapiError code = Mapi.MAPIAddress(UIntPtr.Zero, UIntPtr.Zero, null, 1,
                    null, 0, IntPtr.Zero, flags, 0, out nNewRecips, out lpNewRecips);

                // check for user cancellation
                if (code == MapiError.MAPI_E_USER_ABORT)
                    return new MapiAddress[0];

                // check for null recipient list (buggy MAPI server)
                if (code == MapiError.SUCCESS_SUCCESS && nNewRecips > 0
                    && (lpNewRecips == null || lpNewRecips.IsInvalid))
                    code = MapiError.MAPI_E_FAILURE;

                // throw exception if MAPI reports failure
                if (code != MapiError.SUCCESS_SUCCESS)
                    ThrowMapiException(code);

                // create (possibly empty) output recipient array
                MapiAddress[] entries = new MapiAddress[nNewRecips];
                if (nNewRecips == 0) return entries;

                // prepare managed recipient object
                int size = Marshal.SizeOf(typeof(MapiRecipDesc));
                MapiRecipDesc recip = new MapiRecipDesc();

                // retrieve unmanaged memory blocks
                for (int i = 0; i < nNewRecips; i++) {

                    // copy current array element to managed memory
                    lpNewRecips.GetMemory(i * size, recip);

                    // HACK: Outlook Express inverts the order of selected entries.
                    // We re-invert the order since OE is the most likely client.
                    int index = (int) (nNewRecips - i - 1);

                    // store entry name and address
                    entries[index] = new MapiAddress(recip.lpszName, recip.lpszAddress);
                }

                return entries;
            }
            finally {
                // release unmanaged memory block
                if (lpNewRecips != null) lpNewRecips.Dispose();

                // restore original working directory
                Directory.SetCurrentDirectory(currentDir);
            }
        }

        #endregion
        #region GetErrorString

        /// <summary>
        /// Returns a localized description of the specified MAPI error code.</summary>
        /// <param name="code">
        /// A <see cref="MapiError"/> value indicating the MAPI error code whose description to
        /// retrieve.</param>
        /// <returns>
        /// A localized message that describes the condition indicated by <paramref name="code"/>.
        /// </returns>
        /// <remarks>
        /// <b>GetErrorString</b> returns a generic message indicating an unknown MAPI error if
        /// there is no specific message for the specified <paramref name="code"/>.</remarks>

        public static string GetErrorString(MapiError code) {
            switch (code) {

                case MapiError.SUCCESS_SUCCESS:                 return Strings.SuccessSuccess;
                case MapiError.MAPI_E_USER_ABORT:               return Strings.MapiUserAbort;
                case MapiError.MAPI_E_FAILURE:                  return Strings.MapiFailure;
                case MapiError.MAPI_E_LOGIN_FAILURE:            return Strings.MapiLoginFailure;
                case MapiError.MAPI_E_DISK_FULL:                return Strings.MapiDiskFull;
                case MapiError.MAPI_E_INSUFFICIENT_MEMORY:      return Strings.MapiInsufficientMemory;
                case MapiError.MAPI_E_ACCESS_DENIED:            return Strings.MapiAccessDenied;
                case MapiError.MAPI_E_TOO_MANY_SESSIONS:        return Strings.MapiTooManySessions;
                case MapiError.MAPI_E_TOO_MANY_FILES:           return Strings.MapiTooManyFiles;
                case MapiError.MAPI_E_TOO_MANY_RECIPIENTS:      return Strings.MapiTooManyRecipients;
                case MapiError.MAPI_E_ATTACHMENT_NOT_FOUND:     return Strings.MapiAttachmentNotFound;
                case MapiError.MAPI_E_ATTACHMENT_OPEN_FAILURE:  return Strings.MapiAttachmentOpenFailure;
                case MapiError.MAPI_E_ATTACHMENT_WRITE_FAILURE: return Strings.MapiAttachmentWriteFailure;
                case MapiError.MAPI_E_UNKNOWN_RECIPIENT:        return Strings.MapiUnknownRecipient;
                case MapiError.MAPI_E_BAD_RECIPTYPE:            return Strings.MapiBadRecipType;
                case MapiError.MAPI_E_NO_MESSAGES:              return Strings.MapiNoMessages;
                case MapiError.MAPI_E_INVALID_MESSAGE:          return Strings.MapiInvalidMessage;
                case MapiError.MAPI_E_TEXT_TOO_LARGE:           return Strings.MapiTextTooLarge;
                case MapiError.MAPI_E_INVALID_SESSION:          return Strings.MapiInvalidSession;
                case MapiError.MAPI_E_TYPE_NOT_SUPPORTED:       return Strings.MapiTypeNotSupported;
                case MapiError.MAPI_E_AMBIGUOUS_RECIPIENT:      return Strings.MapiAmbiguousRecipient;
                case MapiError.MAPI_E_MESSAGE_IN_USE:           return Strings.MapiMessageInUse;
                case MapiError.MAPI_E_NETWORK_FAILURE:          return Strings.MapiNetworkFailure;
                case MapiError.MAPI_E_INVALID_EDITFIELDS:       return Strings.MapiInvalidEditFields;
                case MapiError.MAPI_E_INVALID_RECIPS:           return Strings.MapiInvalidRecips;
                case MapiError.MAPI_E_NOT_SUPPORTED:            return Strings.MapiNotSupported;

                default: return Strings.MapiUnknown;
            }
        }

        #endregion
        #region GetErrorStringAndCode

        /// <summary>
        /// Returns a localized description of the specified MAPI error code, followed by the
        /// numerical code itself.</summary>
        /// <param name="code">
        /// A <see cref="MapiError"/> value indicating the MAPI error code whose description to
        /// retrieve.</param>
        /// <returns>
        /// A localized message that describes the condition indicated by <paramref name="code"/>,
        /// followed by the numerical <paramref name="code"/> itself.</returns>
        /// <remarks>
        /// <b>GetErrorString</b> returns a generic message indicating an unknown MAPI error if
        /// there is no specific message for the specified <paramref name="code"/>.</remarks>

        public static string GetErrorStringAndCode(MapiError code) {

            return GetErrorString(code) + String.Format(
                CultureInfo.CurrentCulture, Strings.FormatCode, (int) code);
        }

        #endregion
        #region ResolveName

        /// <summary>
        /// Resolves a recipient name using the Simple MAPI address book.</summary>
        /// <param name="name">
        /// Recipient name to be resolved. This name may be incomplete, misspelled, or otherwise
        /// ambiguous.</param>
        /// <returns>
        /// A <see cref="MapiAddress"/> holding the unambiguous address book name that is the
        /// closest match for the specified <paramref name="name"/>, and the corresponding default
        /// e-mail address.</returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="name"/> is a null reference or an empty string.</exception>
        /// <exception cref="MapiException">
        /// <see cref="Mapi.MAPIResolveName"/> indicated an error.</exception>
        /// <remarks><para>
        /// <b>ResolveName</b> determines the unambiguous address book entry of the specified
        /// recipient, using the Win32 API call <see cref="Mapi.MAPIResolveName"/> which is part of
        /// the Simple MAPI protocol.
        /// </para><para>
        /// If no match is found or the specified <paramref name="name"/> resolves to more than one
        /// address book entry, the user is presented with an address book dialog and asked to
        /// select an entry. User cancellation generates a <see cref="MapiException"/> whose <see
        /// cref="MapiException.Code"/> may be <see cref="MapiException.Abort"/> but also any other
        /// <see cref="MapiError"/> code, depending on the MAPI server.
        /// </para><note type="caution">
        /// Non-Microsoft e-mail clients rarely support <b>MAPIResolveName</b>. Some might report an
        /// error while others might return empty strings or invalid data. There is no workaround
        /// other than using a Microsoft program as the default e-mail client.</note></remarks>

        public static MapiAddress ResolveName(string name) {
            if (String.IsNullOrEmpty(name))
                ThrowHelper.ThrowArgumentNullOrEmptyException("name");

            // remember current working directory
            string currentDir = Directory.GetCurrentDirectory();

            // prepare recipient structure
            SafeMapiHandle lpRecip = null;

            try {
                // invoke MAPIResolveName to resolve this name
                MapiFlags flags = MapiFlags.MAPI_DIALOG | MapiFlags.MAPI_LOGON_UI;
                MapiError code = Mapi.MAPIResolveName(UIntPtr.Zero,
                    UIntPtr.Zero, name, flags, 0, out lpRecip);

                // check for null recipient (buggy MAPI server)
                if (code == MapiError.SUCCESS_SUCCESS && (lpRecip == null || lpRecip.IsInvalid))
                    code = MapiError.MAPI_E_FAILURE;

                // throw exception if MAPI reports failure
                if (code != MapiError.SUCCESS_SUCCESS)
                    ThrowMapiException(code);

                // retrieve unmanaged memory block on success
                MapiRecipDesc recip = (MapiRecipDesc) lpRecip.GetMemory(0, typeof(MapiRecipDesc));

                // return recipient name and address
                return new MapiAddress(recip.lpszName, recip.lpszAddress);
            }
            finally {
                // release unmanaged memory block
                if (lpRecip != null) lpRecip.Dispose();

                // restore original working directory
                Directory.SetCurrentDirectory(currentDir);
            }
        }

        #endregion
        #region SendMail

        /// <summary>
        /// Creates and sends an e-mail message using the Simple MAPI protocol.</summary>
        /// <param name="subject">
        /// The subject line of the e-mail message.</param>
        /// <param name="noteText">
        /// The text of the e-mail message.</param>
        /// <param name="recipients">
        /// An <see cref="Array"/> of <see cref="MapiAddress"/> instances holding the display names
        /// and SMTP addresses of all message recipients. The protocol identifier "smtp:" is
        /// automatically prepended to any non-empty addresses without this prefix.</param>
        /// <param name="attachments">
        /// An <see cref="Array"/> of <see cref="MapiAddress"/> instances holding the display names
        /// and fully qualified local file paths of any attachment files sent to the <paramref
        /// name="recipients"/>.</param>
        /// <exception cref="MapiException">
        /// <see cref="Mapi.MAPISendMail"/> indicated an error.</exception>
        /// <remarks><para>
        /// <b>SendMail</b> creates and sends an e-mail message with optional file attachments,
        /// using the Win32 API call <see cref="Mapi.MAPISendMail"/> which is part of the Simple
        /// MAPI protocol. The originator is left undefined which will cause Simple MAPI to assert
        /// the user’s default e-mail account as the originator.
        /// </para><para>
        /// The <paramref name="subject"/> and <paramref name="noteText"/> parameters may be a null
        /// reference or an empty string to leave the corresponding field blank. The <paramref
        /// name="recipients"/> and <paramref name="attachments"/> parameters may be null references
        /// or empty arrays to create a message without recipients or file attachments,
        /// respectively.
        /// </para><para>
        /// The e-mail message is presented to the user who can choose to edit (filling in any blank
        /// fields or adding text as desired), send, or cancel the message. User cancellation
        /// generates a <see cref="MapiException"/> whose <see cref="MapiException.Code"/> is <see
        /// cref="MapiException.Abort"/>.</para></remarks>

        public static void SendMail(string subject, string noteText,
            MapiAddress[] recipients, MapiAddress[] attachments) {

            // remember current working directory
            string currentDir = Directory.GetCurrentDirectory();

            // construct MAPI message descriptor
            MapiMessage message = new MapiMessage();
            message.lpszSubject = subject;
            message.lpszNoteText = noteText;

            try {
                // create any specified recipients
                if (recipients != null && recipients.Length > 0) {

                    // store count of recipient descriptors
                    int count = recipients.Length;
                    message.nRecipCount = (uint) count;

                    // allocate memory for recipient descriptors
                    int size = Marshal.SizeOf(typeof(MapiRecipDesc));
                    message.lpRecips.AllocateHandle(count * size);

                    // construct recipient descriptors
                    MapiRecipDesc recip = new MapiRecipDesc();
                    for (int i = 0; i < count; i++) {

                        // prepend "smtp:" to address if not present
                        string address = recipients[i].Address;
                        if (!String.IsNullOrEmpty(address) &&
                            !address.StartsWith("smtp:", StringComparison.OrdinalIgnoreCase))
                            address = "smtp:" + address;

                        // create MAPI recipient descriptor
                        recip.ulRecipClass = MapiRecipClass.MAPI_TO;
                        recip.lpszName = recipients[i].Name;
                        recip.lpszAddress = address;

                        // copy recipient descriptor to unmanaged memory
                        message.lpRecips.SetMemory(recip, i * size, false);
                    }
                }

                // create any specified attachments
                if (attachments != null && attachments.Length > 0) {

                    // store count of attachment descriptors
                    int count = attachments.Length;
                    message.nFileCount = (uint) count;

                    // allocate memory for attachment descriptors
                    int size = Marshal.SizeOf(typeof(MapiFileDesc));
                    message.lpFiles.AllocateHandle(count * size);

                    // construct attachment descriptors
                    MapiFileDesc fileDesc = new MapiFileDesc();
                    for (int i = 0; i < count; i++) {

                        // create MAPI file attachment descriptor
                        fileDesc.lpszFileName = attachments[i].Name;
                        fileDesc.lpszPathName = attachments[i].Address;
                        fileDesc.nPosition = 0xffffffff; // don’t embed files

                        // copy attachment descriptor to unmanaged memory
                        message.lpFiles.SetMemory(fileDesc, i * size, false);
                    }
                }

                // invoke MAPISendMail to deliver this message
                MapiFlags flags = MapiFlags.MAPI_DIALOG | MapiFlags.MAPI_LOGON_UI;
                MapiError code = Mapi.MAPISendMail(UIntPtr.Zero, UIntPtr.Zero, message, flags, 0);

                // throw exception if MAPI reports failure
                if (code != MapiError.SUCCESS_SUCCESS)
                    ThrowMapiException(code);
            }
            finally {
                // release unmanaged memory blocks
                message.Dispose();

                // restore original working directory
                Directory.SetCurrentDirectory(currentDir);
            }
        }

        #endregion
        #region ThrowMapiException

        /// <summary>
        /// Throws a <see cref="MapiException"/>.</summary>
        /// <param name="code">
        /// The code of the MAPI or Simple MAPI error that caused the exception.</param>
        /// <remarks>
        /// Please refer to the <see cref="ThrowHelper"/> class for the rationale behind this
        /// method.</remarks>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowMapiException(MapiError code) {
            throw new MapiException(code);
        }

        #endregion
    }
}
