using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace Tektosyne.Win32Api {

    /// <summary>
    /// Interfaces the Windows system library "mapi32.dll".</summary>
    /// <remarks>
    /// Contrary to what the MSDN Library (4/2002) states, the MAPI library does not support
    /// Unicode, only plain 8-bit strings. Therefore, the <b>Mapi</b> class marshals all <see
    /// cref="String"/> fields and parameters as <b>UnmanagedType.LPStr</b>.</remarks>

    [CLSCompliant(false)]
    public static class Mapi {
        #region MAPIAddress

        /// <summary>
        /// Creates or modifies a set of address list entries.</summary>
        /// <param name="lhSession">
        /// Handle to a Simple MAPI session, or <see cref="UIntPtr.Zero"/> to indicate that MAPI
        /// should log on the user and create a temporary session.</param>
        /// <param name="ulUIParam">
        /// Parent window handle, or <see cref="UIntPtr.Zero"/> to indicate that if a dialog is
        /// displayed, it is application modal.</param>
        /// <param name="lpszCaption">
        /// The caption for the address list dialog box, or a null reference or an empty string for
        /// the default caption "Address Book".</param>
        /// <param name="nEditFields">
        /// A value from 0 through 4, indicating the number of edit controls that should be present
        /// in the address list. A value of 0 indicates that the address list may be browsed but not
        /// edited. Please refer to the Platform SDK documentation for further details.</param>
        /// <param name="lpszLabels">
        /// The label for the edit control in the address-list dialog box, or a null reference or an
        /// empty string for the default control label "To". This parameter is ignored if <paramref
        /// name="nEditFields"/> has a value other than one.</param>
        /// <param name="nRecips">
        /// The number of entries in the <paramref name="lpRecips"/> array. If the value of this
        /// parameter is zero, <paramref name="lpRecips"/> is ignored.</param>
        /// <param name="lpRecips">
        /// Pointer to an array of <see cref="MapiRecipDesc"/> structures defining the initial
        /// recipient entries to be used to populate the address list dialog box. Please refer to
        /// the Platform SDK documentation for further details.</param>
        /// <param name="flFlags">
        /// Bitmask of option flags.</param>
        /// <param name="ulReserved">
        /// Reserved; must be zero.</param>
        /// <param name="lpnNewRecips">
        /// Returns the number of entries in the <paramref name="lppNewRecips"/> recipient output
        /// array.</param>
        /// <param name="lppNewRecips">
        /// Pointer to an array of <see cref="MapiRecipDesc"/> structures containing the final list
        /// of recipients. This array is allocated by <b>MAPIAddress</b> and must be freed using
        /// <see cref="MAPIFreeBuffer"/>, even if there are no new recipients. Please refer to the
        /// Platform SDK documentation for further details.</param>
        /// <returns>
        /// One of the values defined in <see cref="MapiError"/>.</returns>
        /// <remarks><para>
        /// The following flags can be set in the <paramref name="flFlags"/> parameter:
        /// </para><list type="bullet"><item>
        /// <term>MapiFlags.MAPI_LOGON_UI</term>
        /// <description>A dialog box should be displayed to prompt the user to log on if required.
        /// When the <see cref="MapiFlags.MAPI_LOGON_UI"/> flag is not set, the client application
        /// does not display a logon dialog box and returns an error value if the user is not logged
        /// on.</description>
        /// </item><item>
        /// <term>MapiFlags.MAPI_NEW_SESSION</term>
        /// <description>An attempt should be made to create a new session rather than acquire the
        /// environment’s shared session. If the <see cref="MapiFlags.MAPI_NEW_SESSION"/> flag is
        /// not set, <b>MAPIAddress</b> uses an existing shared session.</description>
        /// </item></list><para>
        /// This method invokes the <b>MAPIAddress</b> (Simple MAPI) function defined in the Windows
        /// system library "mapi32.dll". Please refer to the Microsoft Platform SDK documentation
        /// for details.
        /// </para><para>
        /// <see cref="Net.MapiMail.Address"/> provides an easy-to-use managed wrapper for this
        /// method.</para></remarks>

        [DllImport("mapi32.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern MapiError MAPIAddress(
            UIntPtr lhSession,
            UIntPtr ulUIParam,
            [MarshalAs(UnmanagedType.LPStr)]
            string lpszCaption,
            uint nEditFields,
            [MarshalAs(UnmanagedType.LPStr)]
            string lpszLabels, // single string despite plural form!
            uint nRecips,
            IntPtr lpRecips,
            [MarshalAs(UnmanagedType.U4)]
            MapiFlags flFlags,
            uint ulReserved,
            out uint lpnNewRecips,
            out SafeMapiHandle lppNewRecips);

        #endregion
        #region MAPIFreeBuffer

        /// <summary>
        /// Frees memory allocated by the messaging system.</summary>
        /// <param name="pv">
        /// Pointer to memory allocated by the messaging system. This pointer is returned by the
        /// <see cref="MAPIAddress"/>, <b>MAPIReadMail</b>, and <see cref="MAPIResolveName"/>
        /// functions.</param>
        /// <returns><para>
        /// One of the following <see cref="MapiError"/> values:
        /// </para><list type="bullet"><item>
        /// <term>MapiError.SUCCESS_SUCCESS</term>
        /// <description>The call succeeded and the memory was freed.</description>
        /// </item><item>
        /// <term>MapiError.MAPI_E_FAILURE</term>
        /// <description>One or more unspecified errors occurred. The memory could not be freed.
        /// </description></item></list></returns>

        [DllImport("mapi32.dll", CharSet = CharSet.Ansi), SuppressUnmanagedCodeSecurity]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern MapiError MAPIFreeBuffer(IntPtr pv);

        #endregion
        #region MAPIResolveName

        /// <summary>
        /// Transforms a message recipient’s name as entered by a user to an unambiguous address
        /// list entry.</summary>
        /// <param name="lhSession">
        /// Handle to a Simple MAPI session, or <see cref="UIntPtr.Zero"/> to indicate that MAPI
        /// should log on the user and create a temporary session.</param>
        /// <param name="ulUIParam">
        /// Parent window handle, or <see cref="UIntPtr.Zero"/> to indicate that if a dialog is
        /// displayed, it is application modal.</param>
        /// <param name="lpszName">
        /// Recipient name to be resolved.</param>
        /// <param name="flFlags">
        /// Bitmask of option flags.</param>
        /// <param name="ulReserved">
        /// Reserved; must be zero.</param>
        /// <param name="lppRecip">
        /// Pointer to a <see cref="MapiRecipDesc"/> structure if the resolution results in a single
        /// match. The recipient structure contains the resolved name and related information.
        /// Memory for this structure must be freed using <see cref="MAPIFreeBuffer"/>.</param>
        /// <returns>
        /// One of the values defined in <see cref="MapiError"/>.</returns>
        /// <remarks><para>
        /// The following flags can be set in the <paramref name="flFlags"/> parameter:
        /// </para><list type="bullet"><item>
        /// <term>MapiFlags.MAPI_AB_NOMODIFY</term>
        /// <description>The caller is requesting that the dialog box be read-only, prohibiting
        /// changes. <b>MAPIResolveName</b> ignores this flag if <see cref="MapiFlags.MAPI_DIALOG"/>
        /// is not set.</description>
        /// </item><item>
        /// <term>MapiFlags.MAPI_DIALOG</term>
        /// <description>A dialog box should be displayed for name resolution. If this flag is not
        /// set and the name cannot be resolved, <b>MAPIResolveName</b> returns the <see
        /// cref="MapiError.MAPI_E_AMBIGUOUS_RECIPIENT"/> value.</description>
        /// </item><item>
        /// <term>MapiFlags.MAPI_LOGON_UI</term>
        /// <description>A dialog box should be displayed to prompt the user to log on if required.
        /// When the <see cref="MapiFlags.MAPI_LOGON_UI"/> flag is not set, the client application
        /// does not display a logon dialog box and returns an error value if the user is not logged
        /// on.</description>
        /// </item><item>
        /// <term>MapiFlags.MAPI_NEW_SESSION</term>
        /// <description>An attempt should be made to create a new session rather than acquire the
        /// environment’s shared session. If the <see cref="MapiFlags.MAPI_NEW_SESSION"/> flag is
        /// not set, <b>MAPIResolveName</b> uses an existing shared session.</description>
        /// </item></list><para>
        /// This method invokes the <b>MAPIResolveName</b> (Simple MAPI) function defined in the
        /// Windows system library "mapi32.dll". Please refer to the Microsoft Platform SDK
        /// documentation for details.
        /// </para><para>
        /// <see cref="Net.MapiMail.ResolveName"/> provides an easy-to-use managed wrapper for this
        /// method.</para></remarks>

        [DllImport("mapi32.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern MapiError MAPIResolveName(
            UIntPtr lhSession,
            UIntPtr ulUIParam,
            [MarshalAs(UnmanagedType.LPStr)]
            string lpszName,
            [MarshalAs(UnmanagedType.U4)]
            MapiFlags flFlags,
            uint ulReserved,
            out SafeMapiHandle lppRecip);

        #endregion
        #region MAPISendMail

        /// <summary>
        /// Sends a standard message, with or without user interaction.</summary>
        /// <param name="lhSession">
        /// Handle to a Simple MAPI session, or <see cref="UIntPtr.Zero"/> to indicate that MAPI
        /// should log on the user and create a temporary session.</param>
        /// <param name="ulUIParam">
        /// Parent window handle, or <see cref="UIntPtr.Zero"/> to indicate that if a dialog is
        /// displayed, it is application modal.</param>
        /// <param name="lpMessage">
        /// Pointer to a <see cref="MapiMessage"/> structure containing the message to be sent.
        /// </param>
        /// <param name="flFlags">
        /// Bitmask of option flags.</param>
        /// <param name="ulReserved">
        /// Reserved; must be zero.</param>
        /// <returns>
        /// One of the values defined in <see cref="MapiError"/>.</returns>
        /// <remarks><para>
        /// The following flags can be set in the <paramref name="flFlags"/> parameter:
        /// </para><list type="bullet"><item>
        /// <term>MapiFlags.MAPI_DIALOG</term>
        /// <description>A dialog should be displayed to prompt the user for recipients and other
        /// sending options. When <see cref="MapiFlags.MAPI_DIALOG"/> is not set, at least one
        /// recipient must be specified.</description>
        /// </item><item>
        /// <term>MapiFlags.MAPI_LOGON_UI</term>
        /// <description>A dialog should be displayed to prompt the user to log on if required. When
        /// the <see cref="MapiFlags.MAPI_LOGON_UI"/> flag is not set, the client application does
        /// not display a logon dialog and returns an error value if the user is not logged on.
        /// <b>MAPISendMail</b> ignores this flag if the <em>lpszMessageID</em> parameter is empty.
        /// </description>
        /// </item><item>
        /// <term>MapiFlags.MAPI_NEW_SESSION</term>
        /// <description>An attempt should be made to create a new session rather than acquire the
        /// environment’s shared session. If the <see cref="MapiFlags.MAPI_NEW_SESSION"/> flag is
        /// not set, <b>MAPISendMail</b> uses an existing shared session.</description>
        /// </item></list><para>
        /// This method invokes the <b>MAPISendMail</b> (Simple MAPI) function defined in the
        /// Windows system library "mapi32.dll". Please refer to the Microsoft Platform SDK
        /// documentation for details.
        /// </para><para>
        /// <see cref="Net.MapiMail.SendMail"/> provides an easy-to-use managed wrapper for this
        /// method.</para></remarks>

        [DllImport("mapi32.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern MapiError MAPISendMail(
            UIntPtr lhSession,
            UIntPtr ulUIParam,
            [In, MarshalAs(UnmanagedType.LPStruct)]
            MapiMessage lpMessage,
            [MarshalAs(UnmanagedType.U4)]
            MapiFlags flFlags,
            uint ulReserved);

        #endregion
    }
}
