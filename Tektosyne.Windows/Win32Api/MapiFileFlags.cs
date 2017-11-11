using System;

namespace Tektosyne.Win32Api {

    /// <summary>
    /// Defines attachment flags for MAPI messages.</summary>
    /// <remarks>
    /// <b>MapiFileFlags</b> defines all possible values for the <see cref="MapiFileDesc.flFlags"/>
    /// field of the <see cref="MapiFileDesc"/> structure. Bitwise combinations are possible.
    /// </remarks>

    [Flags]
    public enum MapiFileFlags {

        /// <summary>
        /// Indicates an OLE object attachment.</summary>

        MAPI_OLE = 0x00000001,

        /// <summary>
        /// Indicates a static OLE object attachment.</summary>

        MAPI_OLE_STATIC = 0x00000002
    }
}
