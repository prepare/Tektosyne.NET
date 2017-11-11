using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides a <see cref="TextBlock"/> that shows formatted data.</summary>
    /// <remarks>
    /// <b>FormatTextBlock</b> enhances <see cref="TextBlock"/> with persistent format
    /// specifications and methods that use these specifications to format their arguments. This
    /// simplifies showing formatted data, such as numbers, in a WPF application.</remarks>

    public class FormatTextBlock: TextBlock {
        #region Private Fields

        // property backers
        private string _format = "{0}";

        #endregion
        #region AllowFormatException

        /// <summary>
        /// Gets or sets a value indicating whether format exceptions are propagated to the client.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Show"/> propagates exceptions of type <see
        /// cref="FormatException"/> to the caller; otherwise, <c>false</c>. The default is
        /// <c>false</c>.</value>
        /// <remarks>
        /// Exceptions of a type other than <see cref="FormatException"/> are propagated to the
        /// client regardless of the value of <b>AllowFormatException</b>.</remarks>

        public bool AllowFormatException { get; set; }

        #endregion
        #region Format

        /// <summary>
        /// Gets or sets the format specifications for the <see cref="FormatTextBlock"/>.</summary>
        /// <value>
        /// A <see cref="String"/> containing zero or more format specifications that <see
        /// cref="Show"/> will supply to <see cref="String.Format"/>. The default is the literal
        /// string "{0}".</value>
        /// <remarks><para>
        /// <b>Format</b> returns an empty string when set to a null reference. Changing this
        /// property has no effect until the next call to <see cref="Show"/>.
        /// </para><para>
        /// Invalid format specifications may cause a <see cref="FormatException"/> whose
        /// propagation is controlled by the <see cref="AllowFormatException"/> property.
        /// </para></remarks>

        public string Format {
            [DebuggerStepThrough]
            get { return _format ?? ""; }
            [DebuggerStepThrough]
            set { _format = value; }
        }

        #endregion
        #region Show(Object[])

        /// <overloads>
        /// Shows a formatted message containing the values of the specified objects.</overloads>
        /// <summary>
        /// Shows a formatted message containing the values of the specified objects.</summary>
        /// <param name="args">
        /// An <see cref="Array"/> of zero or more <see cref="Object"/> instances to be formatted.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="args"/> is a null reference.</exception>
        /// <exception cref="FormatException"><para>
        /// <see cref="Format"/> contains an invalid format specification.
        /// </para><para>-or-</para><para>
        /// A number indicating an argument to be formatted is less than zero, or greater than or
        /// equal to the length of the <paramref name="args"/> array.</para></exception>
        /// <remarks><para>
        /// <b>Show</b> invokes <see cref="String.Format"/> with the current <see cref="Format"/>
        /// specifications and the specified <paramref name="args"/> array, and assigns the
        /// resulting string to the <see cref="TextBlock.Text"/> property.
        /// </para><para>
        /// The <b>Text</b> property remains unchanged if an error occurred during formatting. The
        /// <see cref="AllowFormatException"/> property determines whether exceptions of type <see
        /// cref="FormatException"/> are propagated to the caller. Exceptions of any other type are
        /// always propagated to the caller.</para></remarks>

        public void Show(params object[] args) {
            try {
                Text = String.Format(CultureInfo.CurrentCulture, Format, args);
            }
            catch (FormatException) {
                if (AllowFormatException) throw;
            }
        }

        #endregion
        #region Show(IFormatProvider, Object[])

        /// <summary>
        /// Shows a formatted message containing the values of the specified objects, using the
        /// specified culture-specific formatting information.</summary>
        /// <param name="provider">
        /// An <see cref="IFormatProvider"/> object that supplies culture-specific formatting
        /// information.</param>
        /// <param name="args">
        /// An <see cref="Array"/> of zero or more <see cref="Object"/> instances to be formatted.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="args"/> is a null reference.</exception>
        /// <exception cref="FormatException"><para>
        /// <see cref="Format"/> contains an invalid format specification.
        /// </para><para>-or-</para><para>
        /// A number indicating an argument to be formatted is less than zero, or greater than or
        /// equal to the length of the <paramref name="args"/> array.</para></exception>
        /// <remarks><para>
        /// <b>Show</b> invokes <see cref="String.Format"/> with the current <see cref="Format"/>
        /// specifications and the specified <paramref name="provider"/> and <paramref name="args"/>
        /// array, and assigns the resulting string to the <see cref="TextBlock.Text"/> property.
        /// </para><para>
        /// The <b>Text</b> property remains unchanged if an error occurred during formatting. The
        /// <see cref="AllowFormatException"/> property determines whether exceptions of type <see
        /// cref="FormatException"/> are propagated to the caller. Exceptions of any other type are
        /// always propagated to the caller.</para></remarks>

        public void Show(IFormatProvider provider, params object[] args) {
            try {
                string message = String.Format(provider, Format, args);
                Text = message;
            }
            catch (FormatException) {
                if (AllowFormatException) throw;
            }
        }

        #endregion
    }
}
