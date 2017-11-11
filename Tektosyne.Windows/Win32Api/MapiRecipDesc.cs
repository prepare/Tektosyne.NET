using System;
using System.Runtime.InteropServices;

namespace Tektosyne.Win32Api {

    /// <summary>
    /// Contains information about a message sender or recipient.</summary>
    /// <remarks>
    /// This type mirrors the <b>MapiRecipDesc</b> (Simple MAPI) structure defined in the Platform
    /// SDK. Please refer to the Microsoft Platform SDK documentation for details.</remarks>

    [CLSCompliant(false)]
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MapiRecipDesc {
        #region ulReserved

        /// <summary>
        /// Reserved; must be zero.</summary>

        public uint ulReserved;

        #endregion
        #region ulRecipClass

        /// <summary>
        /// Contains a numeric value that indicates the type of recipient.</summary>
        /// <remarks><para>
        /// Possible values are:
        /// </para><list type="table"><listheader>
        /// <term>Value</term><description>Constant</description><description>Meaning</description>
        /// </listheader><item>
        /// <term>0</term><description><see cref="MapiRecipClass.MAPI_ORIG"/></description>
        /// <description>Indicates the original sender of the message.</description>
        /// </item><item>
        /// <term>1</term><description><see cref="MapiRecipClass.MAPI_TO"/></description>
        /// <description>Indicates a primary message recipient.</description>
        /// </item><item>
        /// <term>2</term><description><see cref="MapiRecipClass.MAPI_CC"/></description>
        /// <description>Indicates a recipient of a message copy.</description>
        /// </item><item>
        /// <term>3</term><description><see cref="MapiRecipClass.MAPI_BCC"/></description>
        /// <description>Indicates a recipient of a blind copy.</description>
        /// </item></list></remarks>

        [MarshalAs(UnmanagedType.U4)]
        public MapiRecipClass ulRecipClass;

        #endregion
        #region lpszName

        /// <summary>
        /// The display name of the message recipient or sender.</summary>

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpszName;

        #endregion
        #region lpszAddress

        /// <summary>
        /// The provider-specific address of the recipient or sender.</summary>
        /// <remarks><para>
        /// This address is provider-specific message delivery data. Generally, the messaging system
        /// provides such addresses for inbound messages. For outbound messages, the
        /// <b>lpszAddress</b> member can contain an address entered by the user for a recipient not
        /// in an address book (that is, a custom recipient).
        /// </para><para>
        /// The format of an address pointed to by the <b>lpszAddress</b> member is [<em>address
        /// type</em>][<em>e-mail address</em>]. Examples of valid addresses are
        /// <c>FAX:206-555-1212</c> and <c>SMTP:M@X.COM</c>.</para></remarks>

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpszAddress;

        #endregion
        #region ulEIDSize

        /// <summary>
        /// The size, in bytes, of the entry identifier pointed to by the <see cref="lpEntryID"/>
        /// member.</summary>

        public uint ulEIDSize;

        #endregion
        #region lpEntryID

        /// <summary>
        /// A pointer to an opaque entry identifier used by a messaging system service provider to
        /// identify the message recipient.</summary>
        /// <remarks>
        /// Entry identifiers have meaning only for the service provider; client applications will
        /// not be able to decipher them. The messaging system uses this member to return valid
        /// entry identifiers for all recipients or senders listed in the address book.</remarks>

        public IntPtr lpEntryID;

        #endregion
    }
}
