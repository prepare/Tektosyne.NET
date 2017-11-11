namespace Tektosyne.Windows {

    /// <summary>
    /// Specifies the default themes predefined by Windows Presentation Foundation.</summary>
    /// <remarks>
    /// <b>DefaultTheme</b> defines all possible combinations of basic Windows theme and
    /// (non-standard) color variant that ship with Windows Presentation Foundation.</remarks>

    public enum DefaultTheme {

        /// <summary>
        /// Specifies that WPF should select one of the other <see cref="DefaultTheme"/> values,
        /// based on the current Windows version and user settings.</summary>
        System,

        /// <summary>
        /// Specifies the Classic theme used by Windows 2000 and earlier.</summary>
        Classic,

        /// <summary>
        /// Specifies the Luna theme used by Windows XP.</summary>
        Luna,

        /// <summary>
        /// Specifies the Luna theme used by Windows XP (green variant).</summary>
        LunaHomestead,

        /// <summary>
        /// Specifies the Luna theme used by Windows XP (silver variant).</summary>
        LunaMetallic,

        /// <summary>
        /// Specifies the Royale theme used by Windows XP Media Center Edition.</summary>
        Royale,

        /// <summary>
        /// Specifies the Aero theme used by Windows Vista and Windows 7.</summary>
        Aero,

        /// <summary>
        /// Specifies the Aero 2 theme used by Windows 8 and later.</summary>
        Aero2,

        /// <summary>
        /// Specifies the Aero Lite theme used by Windows 8 and later.</summary>
        AeroLite
    }
}
