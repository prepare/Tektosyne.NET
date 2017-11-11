using System;
using System.Windows.Forms;

using WpfLabel = System.Windows.Controls.Label;
using WindowsFormsHost = System.Windows.Forms.Integration.WindowsFormsHost;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides a <see cref="ComponentControl"/> that hosts a single <see cref="NumericUpDownEx"/>
    /// control.</summary>
    /// <remarks><para>
    /// <b>NumericUpDownHost</b> combines a <see cref="NumericUpDownEx"/> control with dedicated
    /// <see cref="ComponentControl.ErrorProvider"/> and <see cref="ComponentControl.ToolTip"/>
    /// components for easy hosting in Windows Presentation Foundation.
    /// </para><para>
    /// To use a <see cref="NumericUpDownEx"/> from XAML, simply add a <c>WindowsFormsHost</c>
    /// element containing a single <b>NumericUpDownHost</b> element. The height of the hosted
    /// control is automatically set to the height of the <b>NumericUpDownEx</b> control and should
    /// not be specified.
    /// </para><para>
    /// The width of the hosted control should be specified on the <c>WindowsFormsHost</c> element
    /// for consistency with WPF measurements and scaling. The width of the hosted
    /// <b>NumericUpDownEx</b> control equals the specified width less the width of the
    /// <b>ErrorProvider</b> icon.
    /// </para><para>
    /// <b>NumericUpDownHost</b> also provides proxies for the most important <see
    /// cref="NumericUpDown"/> properties and events so that they can be set directly from XAML.
    /// Lastly, when the <b>NumericUpDownHost</b> receives input focus, it is automatically shifted
    /// to the hosted <see cref="NumericUpDownEx"/> control.
    /// </para><note type="caution">
    /// The <see cref="WpfLabel.Target"/> binding of a WPF <see cref="WpfLabel"/> does not reliably
    /// transfer input focus to a <see cref="WindowsFormsHost"/> or to its hosted Windows Forms <see
    /// cref="Control"/>. Use <see cref="WindowsFormsHostEx"/> for this purpose.</note></remarks>

    public class NumericUpDownHost: ComponentControl {
        #region NumericUpDownHost()

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericUpDownHost"/> class.</summary>
        /// <remarks>
        /// This constructor creates a new <see cref="HostedControl"/> with the associated <see
        /// cref="ComponentControl.ErrorProvider"/> and <see cref="ComponentControl.ToolTip"/>
        /// components.</remarks>

        public NumericUpDownHost() {
            HostedControl = new NumericUpDownEx();
            HostedControl.ErrorProvider = ErrorProvider;
            HostedControl.ToolTip = ToolTip;
            Controls.Add(HostedControl);
        }

        #endregion
        #region HostedControl

        /// <summary>
        /// The <see cref="NumericUpDownEx"/> control that is hosted by the <see
        /// cref="NumericUpDownHost"/> control.</summary>

        public readonly NumericUpDownEx HostedControl;

        #endregion
        #region Public Properties
        #region Accelerations

        /// <summary>
        /// Gets the collection of sorted acceleration objects for the <see cref="HostedControl"/>.
        /// </summary>
        /// <value>
        /// A <see cref="NumericUpDownAccelerationCollection"/> containing the <see
        /// cref="NumericUpDownAcceleration"/> objects for the <see cref="HostedControl"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="NumericUpDown.Accelerations"/> for details.</remarks>

        public NumericUpDownAccelerationCollection Accelerations {
            get { return HostedControl.Accelerations; }
        }

        #endregion
        #region BorderStyle

        /// <summary>
        /// Gets or sets the border style for the <see cref="HostedControl"/>.</summary>
        /// <value>
        /// A <see cref="System.Windows.Forms.BorderStyle"/> value indicating the border style of
        /// the <see cref="HostedControl"/>. The default is <see
        /// cref="System.Windows.Forms.BorderStyle.Fixed3D"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="UpDownBase.BorderStyle"/> for details.</remarks>

        public BorderStyle BorderStyle {
            get { return HostedControl.BorderStyle; }
            set { HostedControl.BorderStyle = value; }
        }

        #endregion
        #region DecimalPlaces

        /// <summary>
        /// Gets or sets the number of decimal places to display in the <see cref="HostedControl"/>.
        /// </summary>
        /// <value>
        /// The number of decimal places to display in the <see cref="HostedControl"/>. The default
        /// is zero.</value>
        /// <remarks>
        /// Please refer to <see cref="NumericUpDown.DecimalPlaces"/> for details.</remarks>

        public int DecimalPlaces {
            get { return HostedControl.DecimalPlaces; }
            set { HostedControl.DecimalPlaces = value; }
        }

        #endregion
        #region Hexadecimal

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="HostedControl"/> should display
        /// its value in hexadecimal format.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="HostedControl"/> should display its value in hexadecimal
        /// format; otherwise, <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks>
        /// Please refer to <see cref="NumericUpDown.Hexadecimal"/> for details.</remarks>

        public bool Hexadecimal {
            get { return HostedControl.Hexadecimal; }
            set { HostedControl.Hexadecimal = value; }
        }

        #endregion
        #region Increment

        /// <summary>
        /// Gets or sets the value by which the arrow buttons increment or decrement the value of
        /// the <see cref="HostedControl"/>.</summary>
        /// <value>
        /// The value by which the arrow buttons increment or decrement the value of the <see
        /// cref="HostedControl"/>. The default is one.</value>
        /// <remarks>
        /// Please refer to <see cref="NumericUpDown.Increment"/> for details.</remarks>

        public decimal Increment {
            get { return HostedControl.Increment; }
            set { HostedControl.Increment = value; }
        }

        #endregion
        #region Maximum

        /// <summary>
        /// Gets or sets the maximum value for the <see cref="HostedControl"/>.</summary>
        /// <value>
        /// The maximum value for the <see cref="HostedControl"/>. The default is 100.</value>
        /// <remarks><para>
        /// Please refer to <see cref="NumericUpDown.Maximum"/> for details.
        /// </para><para>
        /// Setting this property also calls <see cref="NumericUpDownEx.UpdateToolTip"/> on the <see
        /// cref="HostedControl"/>.</para></remarks>

        public decimal Maximum {
            get { return HostedControl.Maximum; }
            set {
                HostedControl.Maximum = value;
                HostedControl.UpdateToolTip();
            }
        }

        #endregion
        #region Minimum

        /// <summary>
        /// Gets or sets the minimum value for the <see cref="HostedControl"/>.</summary>
        /// <value>
        /// The minimum value for the <see cref="HostedControl"/>. The default is zero.</value>
        /// <remarks><para>
        /// Please refer to <see cref="NumericUpDown.Minimum"/> for details.
        /// </para><para>
        /// Setting this property also calls <see cref="NumericUpDownEx.UpdateToolTip"/> on the <see
        /// cref="HostedControl"/>.</para></remarks>

        public decimal Minimum {
            get { return HostedControl.Minimum; }
            set {
                HostedControl.Minimum = value;
                HostedControl.UpdateToolTip();
            }
        }

        #endregion
        #region ReadOnly

        /// <summary>
        /// Gets or sets a value indicating whether the value of the <see cref="HostedControl"/> can
        /// be changed only with the arrow buttons.</summary>
        /// <value>
        /// <c>true</c> if the value of the <see cref="HostedControl"/> can be changed only with the
        /// arrow buttons; otherwise, <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks>
        /// Please refer to <see cref="UpDownBase.ReadOnly"/> for details.</remarks>

        public bool ReadOnly {
            get { return HostedControl.ReadOnly; }
            set { HostedControl.ReadOnly = value; }
        }

        #endregion
        #region TextAlign

        /// <summary>
        /// Gets or sets the alignment of text within the <see cref="HostedControl"/>.</summary>
        /// <value>
        /// A <see cref="HorizontalAlignment"/> value indicating the alignment of text within the
        /// <see cref="HostedControl"/>. The default is <see cref="HorizontalAlignment.Left"/>.
        /// </value>
        /// <remarks>
        /// Please refer to <see cref="UpDownBase.TextAlign"/> for details.</remarks>

        public HorizontalAlignment TextAlign {
            get { return HostedControl.TextAlign; }
            set { HostedControl.TextAlign = value; }
        }

        #endregion
        #region ThousandsSeparator

        /// <summary>
        /// Gets or sets a value indicating whether a thousands separator is displayed in the <see
        /// cref="HostedControl"/>.</summary>
        /// <value>
        /// <c>true</c> if a thousands separator is displayed in the <see cref="HostedControl"/>;
        /// otherwise, <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks>
        /// Please refer to <see cref="NumericUpDown.ThousandsSeparator"/> for details.</remarks>

        public bool ThousandsSeparator {
            get { return HostedControl.ThousandsSeparator; }
            set { HostedControl.ThousandsSeparator = value; }
        }

        #endregion
        #region UpDownAlign

        /// <summary>
        /// Gets or sets the alignment of the arrow buttons for the <see cref="HostedControl"/>.
        /// </summary>
        /// <value>
        /// A <see cref="LeftRightAlignment"/> value indicating the alignment of arrow buttons for
        /// the <see cref="HostedControl"/>. The default is <see cref="HorizontalAlignment.Right"/>.
        /// </value>
        /// <remarks>
        /// Please refer to <see cref="UpDownBase.UpDownAlign"/> for details.</remarks>

        public LeftRightAlignment UpDownAlign {
            get { return HostedControl.UpDownAlign; }
            set { HostedControl.UpDownAlign = value; }
        }

        #endregion
        #region Value

        /// <summary>
        /// Gets or sets the numeric value shown in the <see cref="HostedControl"/>.</summary>
        /// <value>
        /// The numeric value shown in the <see cref="HostedControl"/>. The default is zero.</value>
        /// <remarks>
        /// Please refer to <see cref="NumericUpDown.Value"/> for details.</remarks>

        public decimal Value {
            get { return HostedControl.Value; }
            set { HostedControl.Value = value; }
        }

        #endregion
        #endregion
        #region OnGotFocus

        /// <summary>
        /// Raises and handles the <see cref="Control.GotFocus"/> event.</summary>
        /// <param name="args">
        /// An <see cref="EventArgs"/> object containing event data.</param>
        /// <remarks><para>
        /// <b>OnGotFocus</b> first raises the <see cref="Control.GotFocus"/> event by calling the
        /// base class implementation of <see cref="Control.OnGotFocus"/>.
        /// </para><para>
        /// <b>OnGotFocus</b> then handles the <b>GotFocus</b> event by invoking <see
        /// cref="Control.Focus"/> on the <see cref="HostedControl"/>.</para></remarks>

        protected override void OnGotFocus(EventArgs args) {
            base.OnGotFocus(args);
            HostedControl.Focus();
        }

        #endregion
        #region OnLayout

        /// <summary>
        /// Raises and handles the <see cref="Control.Layout"/> event.</summary>
        /// <param name="args">
        /// A <see cref="LayoutEventArgs"/> object containing event data.</param>
        /// <remarks><para>
        /// <b>OnLayout</b> first raises the <see cref="Control.Layout"/> event by calling the base
        /// class implementation of <see cref="Control.OnLayout"/>.
        /// </para><para>
        /// <b>OnLayout</b> then handles the <b>Layout</b> event by performing the following
        /// actions:
        /// </para><list type="number"><item>
        /// If the <see cref="Control.Width"/> of the <see cref="NumericUpDownHost"/> is still zero,
        /// set it to the <see cref="Control.Width"/> of the <see cref="HostedControl"/>.
        /// </item><item>
        /// Set the <see cref="Control.Width"/> of the <see cref="HostedControl"/> to that of the
        /// <see cref="NumericUpDownHost"/> minus the icon width of the associated <see
        /// cref="ComponentControl.ErrorProvider"/>.
        /// </item><item>
        /// Set the <see cref="Control.Height"/> of the <see cref="NumericUpDownHost"/> to that of
        /// the <see cref="HostedControl"/>.
        /// </item></list><para>
        /// We always use the hosted control’s height because a <see cref="NumericUpDown"/> control
        /// has a fixed height based on the current <see cref="Control.Font"/>.</para></remarks>

        protected override void OnLayout(LayoutEventArgs args) {
            base.OnLayout(args);

            if (Width == 0) Width = HostedControl.Width;
            HostedControl.Width = Width - ErrorProvider.Icon.Width;
            Height = HostedControl.Height;
        }

        #endregion
        #region ValueChanged

        /// <summary>
        /// Occurs when the value of the <see cref="HostedControl"/> has been changed.</summary>
        /// <remarks>
        /// Please refer to <see cref="NumericUpDown.ValueChanged"/> for details.</remarks>

        public event EventHandler ValueChanged {
            add { HostedControl.ValueChanged += value; }
            remove { HostedControl.ValueChanged -= value; }
        }

        #endregion
    }
}
