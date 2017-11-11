namespace Tektosyne.Win32Api {

    /// <summary>
    /// Defines recipient classes for MAPI messages.</summary>
    /// <remarks>
    /// <b>MapiRecipClass</b> defines all possible values for the <see
    /// cref="MapiRecipDesc.ulRecipClass"/> field of the <see cref="MapiRecipDesc"/> structure.
    /// </remarks>

    public enum MapiRecipClass {

        /// <summary>
        /// Indicates the original sender of the message.</summary>

        MAPI_ORIG = 0,

        /// <summary>
        /// Indicates a primary message recipient.</summary>

        MAPI_TO = 1,

        /// <summary>
        /// Indicates a recipient of a message copy.</summary>

        MAPI_CC = 2,

        /// <summary>
        /// Indicates a recipient of a blind copy.</summary>

        MAPI_BCC = 3
    }
}
