using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides a <see cref="NumericUpDown"/> control that correctly validates user input.
    /// </summary>
    /// <remarks><para>
    /// The standard Windows Forms <see cref="NumericUpDown"/> control provides the <see
    /// cref="NumericUpDown.Minimum"/> and <see cref="NumericUpDown.Maximum"/> properties to
    /// restrict the range of values that a user may enter, but only actually validates the current
    /// <see cref="NumericUpDown.Value"/> against this range if it was entered using the arrow
    /// controls. Any value that was manually typed into the text box is never validated.
    /// </para><para>
    /// <b>NumericUpDownEx</b> adds the missing validation for manually entered values. The user
    /// cannot enter a value that is less than <b>Minimum</b> or greater than <b>Maximum</b>.
    /// Additionally, the current <b>Value</b> is restricted to this range whenever the control
    /// loses input focus.
    /// </para><para>
    /// However, the user is allowed to completely clear the text entry field and start typing a new
    /// value without being interrupted, as long as the input focus remains on the
    /// <b>NumericUpDownEx</b> control. If the control loses focus while the field value is invalid,
    /// it is reset to <b>Minimum</b> or <b>Maximum</b>.
    /// </para><para>
    /// <b>NumericUpDownEx</b> also provides new <see cref="NumericUpDownEx.ErrorProvider"/> and
    /// <see cref="NumericUpDownEx.ToolTip"/> properties that allow the display of error indicators
    /// and informational messages, respectively, which further reduce the potential for erroneous
    /// data entry.</para><note type="caution">
    /// As the <see cref="NumericUpDown"/> control does not expose an event to monitor changes to
    /// the <b>Minimum</b> and <b>Maximum</b> properties, you must manually call <see
    /// cref="NumericUpDownEx.UpdateToolTip"/> whenever you change these properties after a
    /// <b>ToolTip</b> component has been attached.</note></remarks>

    public class NumericUpDownEx: NumericUpDown {
        #region Private Fields

        // range used for error and info message
        private decimal _minimumValue, _maximumValue;
        private string _minimumText, _maximumText;

        // property backers
        private ErrorProvider _errorProvider;
        private string _errorMessage, _userErrorMessage;

        private ToolTip _toolTip;
        private string _infoMessage, _userInfoMessage;

        #endregion
        #region Public Properties
        #region ErrorMessage

        /// <summary>
        /// Gets or sets the error message shown by the associated <see cref="ErrorProvider"/>.
        /// </summary>
        /// <value>
        /// The message that is shown when the user hovers the mouse pointer over the error icon
        /// displayed by the <see cref="ErrorProvider"/> object associated with the <see
        /// cref="NumericUpDownEx"/> control.</value>
        /// <remarks><para>
        /// The default <b>ErrorMessage</b> shows the current <see cref="NumericUpDown.Minimum"/>
        /// and <see cref="NumericUpDown.Maximum"/> values, and informs the user that invalid values
        /// will be reset to <b>Minimum</b> or <b>Maximum</b> unless corrected.
        /// </para><para>
        /// Setting <b>ErrorMessage</b> to a null reference or an empty string restores the default
        /// message.</para></remarks>

        public string ErrorMessage {
            get {
                if (!String.IsNullOrEmpty(_userErrorMessage))
                    return _userErrorMessage;

                CreateStrings();
                return _errorMessage;
            }
            [DebuggerStepThrough]
            set { _userErrorMessage = value; }
        }

        #endregion
        #region ErrorProvider

        /// <summary>
        /// Gets or sets the <see cref="System.Windows.Forms.ErrorProvider"/> for the <see
        /// cref="NumericUpDownEx"/> control.</summary>
        /// <value>
        /// The <see cref="System.Windows.Forms.ErrorProvider"/> component that is used to indicate
        /// invalid values while the user is editing the text entry field. The default is a null
        /// reference.</value>
        /// <remarks><para>
        /// Setting <b>ErrorProvider</b> to a valid reference causes <see cref="OnTextChanged"/> to
        /// show an error indicator as long as the text entry field contains an invalid value.
        /// Hovering the mouse pointer over the error icon displays the current <see
        /// cref="ErrorMessage"/>.
        /// </para><para>
        /// Setting <b>ErrorProvider</b> to a null reference disables the error indicator.
        /// </para></remarks>

        public ErrorProvider ErrorProvider {
            [DebuggerStepThrough]
            get { return _errorProvider; }
            set {
                if (_errorProvider != value) {
                    SetErrorProviderText("");
                    _errorProvider = value;
                    SetErrorProviderText(IsTextValid ? "" : ErrorMessage);
                }
            }
        }

        #endregion
        #region InfoMessage

        /// <summary>
        /// Gets or sets the informational message shown by the associated <see cref="ToolTip"/>.
        /// </summary>
        /// <value>
        /// The message that is shown when the user hovers the mouse pointer over the <see
        /// cref="NumericUpDownEx"/> control.</value>
        /// <remarks><para>
        /// The default <b>InfoMessage</b> shows the current <see cref="NumericUpDown.Minimum"/> and
        /// <see cref="NumericUpDown.Maximum"/> values.
        /// </para><para>
        /// Setting <b>InfoMessage</b> to a null reference or an empty string restores the default
        /// message.</para></remarks>

        public string InfoMessage {
            get {
                if (!String.IsNullOrEmpty(_userInfoMessage))
                    return _userInfoMessage;

                CreateStrings();
                return _infoMessage;
            }
            [DebuggerStepThrough]
            set { _userInfoMessage = value; }
        }

        #endregion
        #region IsTextValid

        /// <summary>
        /// Determines whether <see cref="Control.Text"/> contains a valid numeric value.</summary>
        /// <value>
        /// <c>true</c> if <see cref="Control.Text"/> contains a valid numeric value between <see
        /// cref="NumericUpDown.Minimum"/> and <see cref="NumericUpDown.Maximum"/>; otherwise,
        /// <c>false</c>.</value>
        /// <remarks>
        /// <b>IsTextValid</b> allows clients to validate the <see cref="Control.Text"/> property
        /// without changing its contents. The <see cref="NumericUpDown.Value"/> property cannot be
        /// used for this purpose because accessing <b>Value</b> overwrites <b>Text</b> if it does
        /// not contain a valid numeric value.</remarks>

        public bool IsTextValid {
            get {
                // empty field is always invalid
                if (Text.Length == 0) return false;

                /*
                 * We use Int64.TryParse for the Hexadecimal case because
                 * Decimal.TryParse does not support NumberStyles.HexNumber,
                 * and hexadecimal values cannot have a fractional part.
                 */

                if (Hexadecimal) {
                    long value = 0;
                    return (Int64.TryParse(Text, NumberStyles.HexNumber,
                        NumberFormatInfo.CurrentInfo, out value) &&
                        value >= Minimum && value <= Maximum);
                } else {
                    decimal value = 0m;
                    return (Decimal.TryParse(Text, NumberStyles.Number,
                        NumberFormatInfo.CurrentInfo, out value) &&
                        value >= Minimum && value <= Maximum);
                }
            }
        }

        #endregion
        #region MaxTextLength

        /// <summary>
        /// Gets the maximum length of the string representation of any valid <see
        /// cref="NumericUpDown.Value"/>.</summary>
        /// <value>
        /// The <see cref="String.Length"/> of the result of <see cref="FormatValue"/> for <see
        /// cref="NumericUpDown.Minimum"/> or <see cref="NumericUpDown.Maximum"/>, whichever is
        /// greater.</value>
        /// <remarks>
        /// <see cref="OnTextChanged"/> calls <see cref="NumericUpDown.ValidateEditText"/> as soon
        /// as the user has entered <b>MaxTextLength</b> or more characters.</remarks>

        public int MaxTextLength {
            get {
                CreateStrings();
                return Math.Max(_minimumText.Length, _maximumText.Length);
            }
        }

        #endregion
        #region ToolTip

        /// <summary>
        /// Gets or sets the <see cref="System.Windows.Forms.ToolTip"/> for the <see
        /// cref="NumericUpDownEx"/> control.</summary>
        /// <value>
        /// The <see cref="System.Windows.Forms.ToolTip"/> component that is used to display an
        /// informational message while the mouse pointer hovers over the <see
        /// cref="NumericUpDownEx"/> control. The default is a null reference.</value>
        /// <remarks><para>
        /// Setting <b>ToolTip</b> to a valid reference displays the current <see
        /// cref="InfoMessage"/> while the user hovers the mouse pointer over the <see
        /// cref="NumericUpDownEx"/> control.
        /// </para><para>
        /// Setting <b>ToolTip</b> to a null reference disables the informational message.
        /// </para></remarks>

        public ToolTip ToolTip {
            [DebuggerStepThrough]
            get { return _toolTip; }
            set {
                if (_toolTip != value) {
                    SetToolTipText("");
                    _toolTip = value;
                    SetToolTipText(InfoMessage);
                }
            }
        }

        #endregion
        #endregion
        #region Private Methods
        #region CreateStrings

        /// <summary>
        /// Creates default backer and helper strings for the <see cref="ErrorMessage"/>, <see
        /// cref="InfoMessage"/>, and <see cref="MaxTextLength"/> properties.</summary>
        /// <remarks>
        /// All created strings are based on the values of the <see cref="NumericUpDown.Minimum"/>
        /// and <see cref="NumericUpDown.Maximum"/> properties. <b>CreateStrings</b> therefore only
        /// creates new strings if these properties were changed since the last invocation.
        /// </remarks>

        private void CreateStrings() {
            bool changed = false;

            // string representation of Minimum
            if (_minimumText == null || _minimumValue != Minimum) {
                _minimumValue = Minimum;
                _minimumText = FormatValue(Minimum);
                changed = true;
            }

            // string representation of Maximum
            if (_maximumText == null || _maximumValue != Maximum) {
                _maximumValue = Maximum;
                _maximumText = FormatValue(Maximum);
                changed = true;
            }

            if (changed) {
                IFormatProvider culture = CultureInfo.CurrentCulture;

                // default message for ErrorProvider
                _errorMessage = String.Format(culture,
                    Strings.ControlRangeError, _minimumText, _maximumText);

                // default message for ToolTip
                _infoMessage = String.Format(culture,
                    Strings.ControlRangeInfo, _minimumText, _maximumText);
            }
        }

        #endregion
        #region SetErrorProviderText

        /// <summary>
        /// Updates the associated <see cref="ErrorProvider"/> with the specified message.</summary>
        /// <param name="message">
        /// The message to show using the <see cref="ErrorProvider"/> component.</param>
        /// <remarks>
        /// The specified <paramref name="message"/> may be an empty string to disable the
        /// associated <see cref="ErrorProvider"/> component.</remarks>

        private void SetErrorProviderText(string message) {

            if (_errorProvider != null)
                _errorProvider.SetError(this, message);
        }

        #endregion
        #region SetToolTipText

        /// <summary>
        /// Updates the associated <see cref="ToolTip"/> with the specified message.</summary>
        /// <param name="message">
        /// The message to show using the <see cref="ToolTip"/> component.</param>
        /// <remarks>
        /// The specified <paramref name="message"/> may be an empty string to disable the
        /// associated <see cref="ToolTip"/> component.</remarks>

        private void SetToolTipText(string message) {

            if (_toolTip != null)
                for (int i = 0; i < Controls.Count; i++)
                    _toolTip.SetToolTip(Controls[i], message);
        }

        #endregion
        #endregion
        #region Protected Methods
        #region OnLostFocus

        /// <summary>
        /// Raises and handles the <see cref="Control.LostFocus"/> event.</summary>
        /// <param name="args">
        /// An <see cref="EventArgs"/> object containing event data.</param>
        /// <remarks><para>
        /// <b>OnLostFocus</b> first raises the <see cref="Control.LostFocus"/> event by calling the
        /// base class implementation of <see cref="NumericUpDown.OnLostFocus"/>.
        /// </para><para>
        /// <b>OnLostFocus</b> then handles the <b>LostFocus</b> event by attempting to parse <see
        /// cref="Control.Text"/> as a <see cref="Decimal"/> number. If the attempt fails, <see
        /// cref="NumericUpDown.Value"/> is set to <see cref="NumericUpDown.Minimum"/>, and <see
        /// cref="NumericUpDown.Text"/> is set to the result of <see cref="FormatValue"/> for that
        /// value.
        /// </para><para>
        /// This manual update is necessary because the <see cref="NumericUpDown.UpdateEditText"/>
        /// method that is called during normal validation does not change the <see
        /// cref="Control.Text"/> property at all if it does not contain a valid <see
        /// cref="Decimal"/> number.</para></remarks>

        protected override void OnLostFocus(EventArgs args) {
            base.OnLostFocus(args);

            decimal value;
            if (!Decimal.TryParse(Text, out value)) {
                Value = Minimum;
                Text = FormatValue(Value);
            }
        }

        #endregion
        #region OnTextChanged

        /// <summary>
        /// Raises and handles the <see cref="Control.TextChanged"/> event.</summary>
        /// <param name="args">
        /// An <see cref="EventArgs"/> object containing event data.</param>
        /// <remarks><para>
        /// <b>OnTextChanged</b> first raises the <see cref="Control.TextChanged"/> event by calling
        /// the base class implementation of <see cref="Control.OnTextChanged"/>.
        /// </para><para>
        /// <b>OnTextChanged</b> then handles the <b>TextChanged</b> event by calling <see
        /// cref="NumericUpDown.ValidateEditText"/> if <see cref="UpDownBase.UserEdit"/> is
        /// <c>true</c> and the <see cref="Control.Text"/> property contains at least <see
        /// cref="MaxTextLength"/> characters.
        /// </para><para>
        /// Moreover, if <see cref="ErrorProvider"/> is a valid reference, its error indicator is
        /// shown or hidden, depending on the value of the <see cref="IsTextValid"/> property.
        /// </para></remarks>

        protected override void OnTextChanged(EventArgs args) {
            base.OnTextChanged(args);

            // check whether Text is valid
            bool valid = true;

            if (UserEdit) {
                if (Text.Length >= MaxTextLength)
                    ValidateEditText();
                else
                    valid = IsTextValid;
            }

            SetErrorProviderText(valid ? "" : ErrorMessage);
        }

        #endregion
        #region OnValidating

        /// <summary>
        /// Raises and handles the <see cref="Control.Validating"/> event.</summary>
        /// <param name="args">
        /// A <see cref="CancelEventArgs"/> object containing event data.</param>
        /// <remarks><para>
        /// <b>OnValidating</b> first raises the <see cref="Control.Validating"/> event by calling
        /// the base class implementation of <see cref="Control.OnValidating"/>.
        /// </para><para>
        /// <b>OnValidating</b> then handles the <b>Validating</b> event by calling <see
        /// cref="NumericUpDown.ValidateEditText"/>.
        /// </para><para>
        /// This method triggers <see cref="OnTextChanged"/> if the value of the <see
        /// cref="Control.Text"/> property was invalid, thereby also updating the <see
        /// cref="ErrorProvider"/>, if present.</para></remarks>

        protected override void OnValidating(CancelEventArgs args) {
            base.OnValidating(args);
            ValidateEditText();
        }

        #endregion
        #endregion
        #region FormatValue

        /// <summary>
        /// Formats the specified value according to the current format settings of the <see
        /// cref="NumericUpDownEx"/> control.</summary>
        /// <param name="value">
        /// The <see cref="Decimal"/> value to format.</param>
        /// <returns>
        /// A <see cref="String"/> representation of the specified <paramref name="value"/> that
        /// matches the current format settings of this <see cref="NumericUpDownEx"/> control.
        /// </returns>
        /// <remarks>
        /// <b>FormatValue</b> exposes the string formatting used internally by the <see
        /// cref="NumericUpDown.UpdateEditText"/> method. The exact format depends on the current
        /// values of the <see cref="NumericUpDown.DecimalPlaces"/>, <see
        /// cref="NumericUpDown.Hexadecimal"/>, and <see cref="NumericUpDown.ThousandsSeparator"/>
        /// properties.</remarks>

        public string FormatValue(decimal value) {
            IFormatProvider culture = CultureInfo.CurrentCulture;

            /*
             * We cast to Int64 for the Hexadecimal case because
             * Decimal.ToString does not support hexadecimal formatting,
             * and hexadecimal values cannot have a fractional part.
             */

            if (Hexadecimal)
                return ((long) value).ToString("X", culture);

            string format = String.Format(culture,
                (ThousandsSeparator ? "N{0}" : "F{0}"), DecimalPlaces);

            return value.ToString(format, culture);
        }

        #endregion
        #region UpdateToolTip

        /// <summary>
        /// Updates the associated <see cref="ToolTip"/> with the current <see cref="InfoMessage"/>.
        /// </summary>
        /// <remarks>
        /// <b>UpdateToolTip</b> refreshes the associated <see cref="ToolTip"/> component, if any,
        /// with the current <see cref="InfoMessage"/>. Call this method whenever the <see
        /// cref="NumericUpDown.Minimum"/> or <see cref="NumericUpDown.Maximum"/> values change
        /// after the <see cref="ToolTip"/> component has been set.</remarks>

        public void UpdateToolTip() {
            SetToolTipText(InfoMessage);
        }

        #endregion
    }
}
