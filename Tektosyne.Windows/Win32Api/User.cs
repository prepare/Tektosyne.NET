using System;
using System.Runtime.InteropServices;

using Tektosyne.Geometry;

namespace Tektosyne.Win32Api {

    /// <summary>
    /// Interfaces the Windows system library "user32.dll".</summary>

    public static class User {
        #region GetCursorPos

        /// <summary>
        /// Retrieves the cursor’s position, in screen coordinates.</summary>
        /// <param name="point">
        /// A <see cref="PointI"/> structure that receives the screen coordinates of the cursor.
        /// </param>
        /// <returns>
        /// <c>true</c> on success; otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// The cursor position is always specified in screen coordinates and is not affected by the
        /// mapping mode of the window that contains the cursor. The input desktop must be the
        /// current desktop when you call GetCursorPos.
        /// </para><para>
        /// Please refer to the Microsoft Platform SDK documentation for further details.
        /// </para></remarks>

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out PointI point);

        #endregion
        #region WindowFromPoint

        /// <summary>
        /// Retrieves a handle to the window that contains the specified point.</summary>
        /// <param name="point">
        /// The <see cref="PointI"/> structure that defines the point to be checked.</param>
        /// <returns>
        /// A handle to the window that contains the specified <paramref name="point"/>.</returns>
        /// <remarks><para>
        /// If no window exists at the given point, the return value is zero. If the point is over a
        /// static text control, the return value is a handle to the window under the static text
        /// control. Hidden or disabled windows are ignored.
        /// </para><para>
        /// This method invokes the <b>WindowFromPoint</b> function defined in the Windows system
        /// library "user32.dll". Please refer to the Microsoft Platform SDK documentation for
        /// details.</para></remarks>

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(PointI point);

        #endregion
    }
}
