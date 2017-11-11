using System;

namespace Tektosyne.Win32Api {

    /// <summary>
    /// Defines status flags for MAPI messages.</summary>
    /// <remarks>
    /// <b>MapiMessageFlags</b> defines all possible values for the <see
    /// cref="MapiMessage.flFlags"/> field of the <see cref="MapiMessage"/> structure. Bitwise
    /// combinations are possible.</remarks>

    [Flags]
    public enum MapiMessageFlags {

        /// <summary>
        /// Indicates that the message has not been read.</summary>

        MAPI_UNREAD = 0x00000001,

        /// <summary>
        /// Indicates that a receipt notification is requested.</summary>

        MAPI_RECEIPT_REQUESTED = 0x00000002,

        /// <summary>
        /// Indicates that the message has been sent.</summary>

        MAPI_SENT = 0x00000004
    }
}
