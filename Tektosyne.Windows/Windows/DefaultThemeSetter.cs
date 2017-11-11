using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace Tektosyne.Windows {

    /// <summary>
    /// Selects one of the default themes defined by Windows Presentation Foundation.</summary>
    /// <remarks><para>
    /// <b>DefaultThemeSetter</b> is a utility class with a single public method, <see
    /// cref="DefaultThemeSetter.Select"/>, that directly manipulates two private fields in the
    /// currently loaded <b>PresentationFramework</b> assembly to change the default theme of the
    /// current WPF application.
    /// </para><para>
    /// This method obviously requires “full trust” permissions and may break with any future WPF
    /// revision. However, there is one big advantage over the usual method of importing the desired
    /// theme dictionary into the current application resources: the specified theme sets the 
    /// <em>default</em> styles for all visual elements. This means you can adjust implicit styles
    /// without having to explicitly specify base styles or worrying about infinite recursion.
    /// </para></remarks>

    public static class DefaultThemeSetter {
        #region Private Fields

        // mapping of DefaultTheme values to name & color strings
        private readonly static Dictionary<DefaultTheme, ValueTuple<String, String>>
            _defaultThemes = new Dictionary<DefaultTheme, ValueTuple<String, String>>() {
                { DefaultTheme.System, ValueTuple<String, String>.Empty },
                { DefaultTheme.Classic, ValueTuple.Create("Classic", "") },
                { DefaultTheme.Luna, ValueTuple.Create("Luna", "NormalColor") },
                { DefaultTheme.LunaHomestead, ValueTuple.Create("Luna", "Homestead") },
                { DefaultTheme.LunaMetallic, ValueTuple.Create("Luna", "Metallic") },
                { DefaultTheme.Royale, ValueTuple.Create("Royale", "NormalColor") },
                { DefaultTheme.Aero, ValueTuple.Create("Aero", "NormalColor") },
                { DefaultTheme.Aero2, ValueTuple.Create("Aero2", "NormalColor") },
                { DefaultTheme.AeroLite, ValueTuple.Create("AeroLite", "NormalColor") }
        };

        #endregion
        #region Select

        /// <summary>
        /// Selects the specified <see cref="DefaultTheme"/> for the current WPF application.
        /// </summary>
        /// <param name="theme">
        /// A <see cref="DefaultTheme"/> value indicating the desired combination of Windows theme
        /// and color variant.</param>
        /// <remarks><para>
        /// <b>Select</b> does nothing if the specified <paramref name="theme"/> equals <see
        /// cref="DefaultTheme.System"/> or an invalid <see cref="DefaultTheme"/> value.
        /// </para><para>
        /// Otherwise, <b>Select</b> sets two private static fields indicating the desired Windows
        /// theme and color variant within the currently loaded <b>PresentationFramework</b>
        /// assembly. The new values are derived from the specified <paramref name="theme"/> value.
        /// </para><para>
        /// This method is only effective if <b>Select</b> is called before WPF attempts to load the
        /// default theme. Therefore, you should call <b>Select</b> in the constructor of your WPF
        /// application.</para></remarks>

        public static void Select(DefaultTheme theme) {

            ValueTuple<String, String> themeValue;
            _defaultThemes.TryGetValue(theme, out themeValue);
            if (themeValue.Item1 == null || themeValue.Item2 == null)
                return;

            Assembly presentationFramework = Assembly.GetAssembly(typeof(Window));
            if (presentationFramework != null) {
                Type themeWrapper = presentationFramework.GetType("MS.Win32.UxThemeWrapper");
                if (themeWrapper != null) {
                    SetField(themeWrapper, "_themeName", themeValue.Item1);
                    SetField(themeWrapper, "_themeColor", themeValue.Item2);
                }
            }
        }

        #endregion
        #region Private Methods
        #region SetField

        /// <summary>
        /// Sets the specified non-public static field of the specified <see cref="Type"/> to the
        /// specified <see cref="String"/> value.</summary>
        /// <param name="type">
        /// The <see cref="Type"/> whose field to set.</param>
        /// <param name="name">
        /// The name of the field to set.</param>
        /// <param name="value">
        /// The new <see cref="String"/> value for the indicated field.</param>
        /// <remarks>
        /// <b>SetField</b> does nothing if the specified <paramref name="type"/> does not contain a
        /// non-public static field with the specified <paramref name="name"/>.</remarks>

        private static void SetField(Type type, string name, string value) {
            FieldInfo field = type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic);
            if (field != null) field.SetValue(null, value);
        }

        #endregion
        #endregion
    }
}
