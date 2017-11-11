using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Icon = System.Drawing.Icon;
using SystemIcons = System.Drawing.SystemIcons;
using GdiGraphics = System.Drawing.Graphics;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides auxiliary methods for <b>System.Windows</b>.</summary>

    public static class WindowsUtility {
        #region ScreenDpi

        /// <summary>
        /// Gets the dots-per-inch resolution of the primary screen.</summary>
        /// <value>
        /// A <see cref="Point"/> containing the dots-per-inch resolution of the primary screen,
        /// with the horizontal dpi in the <see cref="Point.X"/> component and the vertical dpi in
        /// the <see cref="Point.Y"/> component.</value>
        /// <remarks>
        /// <b>ScreenDpi</b> returns the dots-per-inch resolution of a GDI+ <see
        /// cref="GdiGraphics"/> object created from a null HWND, i.e. the primary Windows desktop.
        /// </remarks>

        public static Point ScreenDpi {
            get {
                using (var graphics = GdiGraphics.FromHwnd(IntPtr.Zero))
                    return new Point(graphics.DpiX, graphics.DpiY);
            }
        }

        #endregion
        #region GetMemoryStatus

        /// <summary>
        /// Gets a <see cref="String"/> describing the current memory status.</summary>
        /// <returns>
        /// A multi-line <see cref="String"/> indicating the available and total physical, virtual,
        /// and process memory.</returns>
        /// <remarks>
        /// <b>GetMemoryStatus</b> invokes <see cref="Win32Api.Kernel.GlobalMemoryStatusEx"/> to
        /// obtain information on the current memory status. The returned <see cref="String"/>
        /// consists of three lines, all of which end with a space to compensate for transmission
        /// mechanisms that drop carriage returns and line feeds (i.e. MAPI).</remarks>

        public static string GetMemoryStatus() {

            var memory = new Win32Api.MemoryStatusEx();
            memory.dwLength = (uint) Marshal.SizeOf(typeof(Win32Api.MemoryStatusEx));
            Win32Api.Kernel.GlobalMemoryStatusEx(memory);

            return String.Format(CultureInfo.CurrentCulture, Strings.InformationMemory,
                memory.ulAvailPhys >> 20, memory.ulTotalPhys >> 20,
                memory.ulAvailPageFile >> 20, memory.ulTotalPageFile >> 20,
                memory.ulAvailVirtual >> 20, memory.ulTotalVirtual >> 20);
        }

        #endregion
        #region GetSystemBitmap

        /// <summary>
        /// Returns a <see cref="BitmapSource"/> created from the <see cref="Icon"/> for the
        /// specified <see cref="MessageBoxImage"/> value.</summary>
        /// <param name="image">
        /// The <see cref="MessageBoxImage"/> value whose corresponding <see cref="Icon"/> to find.
        /// </param>
        /// <returns>
        /// A <see cref="BitmapSource"/> created from the <see cref="SystemIcons"/> element that
        /// corresponds to the specified <paramref name="image"/>.</returns>
        /// <remarks>
        /// <b>GetSystemBitmap</b> returns a new <see cref="BitmapSource"/> object created from the
        /// <see cref="Icon"/> returned by <see cref="GetSystemIcon(MessageBoxImage)"/> for the
        /// specified <paramref name="image"/>, or a null reference if <b>GetSystemImage</b> returns
        /// a null reference.</remarks>

        public static BitmapSource GetSystemBitmap(MessageBoxImage image) {
            Icon icon = GetSystemIcon(image);
            return (icon == null ? null :
                Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, null));
        }

        #endregion
        #region GetSystemIcon

        /// <summary>
        /// Returns the <see cref="Icon"/> for the specified <see cref="MessageBoxImage"/> value.
        /// </summary>
        /// <param name="image">
        /// The <see cref="MessageBoxImage"/> value whose corresponding <see cref="Icon"/> to
        /// return.</param>
        /// <returns>
        /// The <see cref="SystemIcons"/> element that corresponds to the specified <paramref
        /// name="image"/>.</returns>
        /// <remarks><para>
        /// <b>GetSystemIcon</b> always returns the <see cref="SystemIcons"/> element with the same
        /// name as the specified <paramref name="image"/>, with two exceptions:
        /// </para><list type="bullet"><item>
        /// <see cref="MessageBoxImage.None"/> maps to a null reference.
        /// </item><item>
        /// <see cref="MessageBoxImage.Stop"/> maps to the <see cref="SystemIcons.Error"/> icon.
        /// </item></list></remarks>

        public static Icon GetSystemIcon(MessageBoxImage image) {
            /*
             * We cannot use switch/case here because the MessageBoxImage enumeration
             * contains too many duplicate values which switch/case does not allow.
             */

            if (image == MessageBoxImage.Asterisk)
                return SystemIcons.Asterisk;

            if (image == MessageBoxImage.Error || image == MessageBoxImage.Stop)
                return SystemIcons.Error;

            if (image == MessageBoxImage.Exclamation)
                return SystemIcons.Exclamation;

            if (image == MessageBoxImage.Hand)
                return SystemIcons.Hand;

            if (image == MessageBoxImage.Information)
                return SystemIcons.Information;

            if (image == MessageBoxImage.Question)
                return SystemIcons.Question;

            if (image == MessageBoxImage.Warning)
                return SystemIcons.Warning;

            return null;
        }

        #endregion
        #region GetVisualDpi

        /// <summary>
        /// Returns the dots-per-inch resolution of the specified <see cref="Visual"/>.</summary>
        /// <param name="visual">
        /// The <see cref="Visual"/> to examine.</param>
        /// <returns>
        /// A <see cref="Point"/> containing the dots-per-inch resolution of the specified <paramref
        /// name="visual"/>, with the horizontal dpi in the <see cref="Point.X"/> component and the
        /// vertical dpi in the <see cref="Point.Y"/> component.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="visual"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>GetVisualDpi</b> returns a default resolution of 96 dpi in both dimensions if the
        /// <see cref="CompositionTarget.TransformFromDevice"/> matrix for the specified <paramref
        /// name="visual"/> is invalid or cannot be determined.
        /// </para><para>
        /// Specifying a <paramref name="visual"/> that is currently visible on the screen returns
        /// the dots-per-inch resolution of that screen. <see cref="ScreenDpi"/> provides an easier
        /// way to obtain this value for the primary screen, however.</para></remarks>

        public static Point GetVisualDpi(Visual visual) {

            // default to device-independent resolution
            double dpiX = 96, dpiY = 96;

            PresentationSource source = PresentationSource.FromVisual(visual);
            if (source != null) {
                Matrix matrix = source.CompositionTarget.TransformFromDevice;
                if (matrix.M11 > 0) dpiX /= matrix.M11;
                if (matrix.M22 > 0) dpiY /= matrix.M22;
            }

            return new Point(dpiX, dpiY);
        }

        #endregion
    }
}
