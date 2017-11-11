using System;
using System.Windows;
using System.Windows.Forms;

using WindowInteropHelper = System.Windows.Interop.WindowInteropHelper;
using WpfIWin32Window = System.Windows.Interop.IWin32Window;

namespace Tektosyne.Windows {

    /// <summary>
    /// Wraps a Win32 HWND in the Windows Forms <see cref="IWin32Window"/> interface.</summary>
    /// <remarks><para>
    /// WPF and Windows Forms both define top-level windows that are associated with Win32 window
    /// handles (HWNDs) and provide simple interfaces to extract these handles. Incredibly, WPF does
    /// not reuse the <see cref="IWin32Window"/> interface defined by Windows Forms for this purpose
    /// but defines its own incompatible <b>IWin32Window</b> interface.
    /// </para><para>
    /// Although the two interfaces are functionally identical they still constitute different
    /// types, with no conversion between them. This means you cannot pass a WPF window to a Windows
    /// Forms dialog that expects an <see cref="IWin32Window"/> instance, even though you already
    /// have the HWND required for that interface.
    /// </para><para>
    /// <b>HwndWrapper</b> resolves this sad defect by providing a trivial Windows Forms <see
    /// cref="IWin32Window"/> implementation whose <see cref="IWin32Window.Handle"/> property simply
    /// returns a value specified during construction as either a naked <see cref="IntPtr"/>, a WPF
    /// <see cref="Window"/>, or the WPF variant of <see cref="WpfIWin32Window"/>.</para></remarks>

    public class HwndWrapper: IWin32Window {
        #region HwndWrapper(IntPtr)

        /// <overloads>
        /// Initializes a new instance of the <see cref="HwndWrapper"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="HwndWrapper"/> class with the specified
        /// <see cref="IntPtr"/>.</summary>
        /// <param name="handle">
        /// The Win32 window handle wrapped by the <see cref="HwndWrapper"/>.</param>

        public HwndWrapper(IntPtr handle) {
            _handle = handle;
        }

        #endregion
        #region HwndWrapper(Window)

        /// <summary>
        /// Initializes a new instance of the <see cref="HwndWrapper"/> class with the specified WPF
        /// <see cref="Window"/>.</summary>
        /// <param name="window">
        /// The WPF <see cref="Window"/> whose Win32 window handle is wrapped by the <see
        /// cref="HwndWrapper"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="window"/> is a null reference.</exception>
        /// <remarks>
        /// <b>Handle</b> is set to the <see cref="WindowInteropHelper.Handle"/> property of a new
        /// <see cref="WindowInteropHelper"/> instance created for the specified <paramref
        /// name="window"/>.</remarks>

        public HwndWrapper(Window window) {
            var helper = new WindowInteropHelper(window);
            _handle = helper.Handle;
        }

        #endregion
        #region HwndWrapper(WpfIWin32Window)

        /// <summary>
        /// Initializes a new instance of the <see cref="HwndWrapper"/> class with the specified WPF
        /// <see cref="WpfIWin32Window"/>.</summary>
        /// <param name="window">
        /// The WPF <see cref="WpfIWin32Window"/> whose Win32 window handle is wrapped by the <see
        /// cref="HwndWrapper"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="window"/> is a null reference.</exception>
        /// <remarks>
        /// <b>Handle</b> is set to the <see cref="WpfIWin32Window.Handle"/> property of the
        /// specified <paramref name="window"/>.</remarks>

        public HwndWrapper(WpfIWin32Window window) {
            if (window == null)
                ThrowHelper.ThrowArgumentNullException("window");

            _handle = window.Handle;
        }

        #endregion
        #region Private Fields

        // property backers
        private readonly IntPtr _handle;

        #endregion
        #region Handle

        /// <summary>
        /// Gets the Win32 window handle wrapped by the <see cref="HwndWrapper"/>.</summary>
        /// <value>
        /// The Win32 window handle wrapped by the <see cref="HwndWrapper"/>. The default is <see
        /// cref="IntPtr.Zero"/>.</value>
        /// <remarks>
        /// <b>Handle</b> never changes once the <see cref="HwndWrapper"/> has been constructed.
        /// </remarks>

        public IntPtr Handle {
            get { return _handle; }
        }

        #endregion
    }
}
