using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides a <see cref="Control"/> with a <see cref="Container"/> for components.</summary>
    /// <remarks><para>
    /// <b>ComponentControl</b> automates the disposal of non-<see cref="Control"/> components by
    /// exposing a <see cref="ComponentControl.Components"/> property to which they can be attached.
    /// </para><para>
    /// Additional properties handle the automatic creation and disposal of three frequently used
    /// components: <see cref="ErrorProvider"/>, <see cref="HelpProvider"/>, and <see
    /// cref="ToolTip"/>.</para></remarks>

    public class ComponentControl: Control {
        #region Private Fields

        // property backers
        private ErrorProvider _errorProvider;
        private HelpProvider _helpProvider;
        private ToolTip _toolTip;

        #endregion
        #region Components

        /// <summary>
        /// The <see cref="Container"/> object managing components for the <see
        /// cref="ComponentControl"/>.</summary>
        /// <remarks><para>
        /// Call <see cref="Container.Add"/> to attach any desired <see cref="Component"/> to the
        /// <b>Components</b> container. All attached components are automatically disposed of when
        /// <see cref="Control.Dispose"/> is invoked on the <see cref="ComponentControl"/>. This
        /// mimics the automatic disposal of <see cref="Control"/> objects attached to <see
        /// cref="Control.Controls"/>.
        /// </para><para>
        /// The implementation of <b>Components</b> is identical to the one automatically generated
        /// by Visual Studio, except that the designer-generated code does not provide public access
        /// to the <see cref="Container"/> object.</para></remarks>

        public readonly Container Components = new Container();

        #endregion
        #region ErrorProvider

        /// <summary>
        /// Gets the <see cref="System.Windows.Forms.ErrorProvider"/> associated with the <see
        /// cref="ComponentControl"/>.</summary>
        /// <value>
        /// The <see cref="System.Windows.Forms.ErrorProvider"/> associated with the <see
        /// cref="ComponentControl"/>.</value>
        /// <remarks>
        /// When this property is first accessed, a new <see
        /// cref="System.Windows.Forms.ErrorProvider"/> component is created and added to the <see
        /// cref="Components"/> container for automatic disposal. The created <b>ErrorProvider</b>
        /// is cached and returned on subsequent accesses.</remarks>

        public ErrorProvider ErrorProvider {
            [DebuggerStepThrough]
            get {
                if (_errorProvider == null)
                    _errorProvider = new ErrorProvider(Components);

                return _errorProvider;
            }
        }

        #endregion
        #region HelpProvider

        /// <summary>
        /// Gets the <see cref="System.Windows.Forms.HelpProvider"/> associated with the <see
        /// cref="ComponentControl"/>.</summary>
        /// <value>
        /// The <see cref="System.Windows.Forms.HelpProvider"/> associated with the <see
        /// cref="ComponentControl"/>.</value>
        /// <remarks>
        /// When this property is first accessed, a new <see
        /// cref="System.Windows.Forms.HelpProvider"/> component is created and added to the <see
        /// cref="Components"/> container for automatic disposal. The created <b>HelpProvider</b> is
        /// cached and returned on subsequent accesses.</remarks>

        public HelpProvider HelpProvider {
            [DebuggerStepThrough]
            get {
                if (_helpProvider == null) {
                    _helpProvider = new HelpProvider();
                    Components.Add(_helpProvider);
                }

                return _helpProvider;
            }
        }

        #endregion
        #region ToolTip

        /// <summary>
        /// Gets the <see cref="System.Windows.Forms.ToolTip"/> associated with the <see
        /// cref="ComponentControl"/>.</summary>
        /// <value>
        /// The <see cref="System.Windows.Forms.ToolTip"/> associated with the <see
        /// cref="ComponentControl"/>.</value>
        /// <remarks>
        /// When this property is first accessed, a new <see cref="System.Windows.Forms.ToolTip"/>
        /// component is created and added to the <see cref="Components"/> container for automatic
        /// disposal. The created <b>ToolTip</b> is cached and returned on subsequent accesses.
        /// </remarks>

        public ToolTip ToolTip {
            [DebuggerStepThrough]
            get {
                if (_toolTip == null)
                    _toolTip = new ToolTip(Components);

                return _toolTip;
            }
        }

        #endregion
        #region IDisposable Members

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ComponentControl"/> and
        /// optionally releases the managed resources.</summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
        /// only unmanaged resources.</param>
        /// <remarks>
        /// <b>Dispose</b> invokes <see cref="Container.Dispose"/> on the associated <see
        /// cref="Components"/> if <paramref name="disposing"/> is <c>true</c>. Please refer to <see
        /// cref="Control.Dispose(Boolean)"/> for more details.</remarks>

        protected override void Dispose(bool disposing) {

            if (disposing && Components != null)
                Components.Dispose();

            base.Dispose(disposing);
        }

        #endregion
    }
}
