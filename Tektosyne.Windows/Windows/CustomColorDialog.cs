using System;
using System.Drawing;
using System.Windows.Forms;

using WpfColor = System.Windows.Media.Color;
using WpfColors = System.Windows.Media.Colors;
using WpfWindow = System.Windows.Window;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides a <see cref="ColorDialog"/> with a persistent set of custom colors.</summary>
    /// <remarks>
    /// <b>CustomColorDialog</b> shows the custom color panel of the Windows Forms <see
    /// cref="ColorDialog"/> by default, and automatically persists the dialog’s set of custom
    /// colors to a static <see cref="CustomColorDialog.CustomColorSet"/> property if the static
    /// <see cref="CustomColorDialog.Show"/> methods are used to display the dialog.</remarks>

    public class CustomColorDialog: ColorDialog {
        #region CustomColorDialog(Color)

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomColorDialog"/> class with the
        /// specified selected color.</summary>
        /// <param name="color">
        /// The initially selected <see cref="Color"/>. <see cref="Color.Transparent"/> translates
        /// to <see cref="Color.Black"/>.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="CustomColorDialog"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="ColorDialog.AllowFullOpen"/></term>
        /// <description><c>true</c></description>
        /// </item><item>
        /// <term><see cref="ColorDialog.AnyColor"/></term><description><c>true</c></description>
        /// </item><item>
        /// <term><see cref="ColorDialog.Color"/></term>
        /// <description><paramref name="color"/>, or <b>Color.Black</b> if <paramref name="color"/>
        /// is <b>Color.Transparent</b>.</description>
        /// </item><item>
        /// <term><see cref="ColorDialog.CustomColors"/></term>
        /// <description>The value of the static <see cref="CustomColorSet"/> property.
        /// </description></item><item>
        /// <term><see cref="ColorDialog.FullOpen"/></term><description><c>true</c></description>
        /// </item><item>
        /// <term><see cref="ColorDialog.ShowHelp"/></term><description><c>false</c></description>
        /// </item><item>
        /// <term><see cref="ColorDialog.SolidColorOnly"/></term>
        /// <description><c>false</c></description>
        /// </item></list><para>
        /// A specified <paramref name="color"/> of <b>Color.Transparent</b> translates to the
        /// default value of <b>Color.Black</b> because <see cref="ColorDialog"/> cannot display or
        /// manipulate alpha channels.
        /// </para><para>
        /// The static <see cref="CustomColorSet"/> property is set by the <see cref="Show"/>
        /// methods. Unfortunately, it is technically impossible to automatically set this property
        /// when a client explicitly creates a <see cref="CustomColorDialog"/> control.
        /// </para></remarks>

        public CustomColorDialog(Color color): base() {

            // allow non-basic and custom colors
            AnyColor = true;
            FullOpen = true;

            // set selected and custom colors
            if (color != Color.Transparent) Color = color;
            CustomColors = CustomColorDialog.CustomColorSet;
        }

        #endregion
        #region CustomColorSet

        /// <summary>
        /// Gets or sets the persistent set of custom colors for the <see cref="CustomColorDialog"/>
        /// class.</summary>
        /// <value>
        /// An <see cref="Array"/> of <see cref="Int32"/> values that are the ARGB representations
        /// of <see cref="Color"/> values. The default is a null reference.</value>
        /// <remarks><para>
        /// <b>CustomColorSet</b> supplies the initial value for the <see
        /// cref="ColorDialog.CustomColors"/> property whenever a new <see
        /// cref="CustomColorDialog"/> control is created.
        /// </para><para>
        /// The property is set automatically by the <see cref="Show"/> methods when the user
        /// dismissed the dialog by clicking <b>OK</b>. Clients may also set <b>CustomColorSet</b>
        /// to any arbitrary set of custom colors.
        /// </para><para>
        /// The name of this property is "CustomColorSet" rather than "CustomColors" because C# does
        /// not allow overloading a property based on its storage type.
        /// </para><note type="caution">
        /// Although static, <b>CustomColorSet</b> and the two <see cref="Show"/> methods are 
        /// <em>not</em> thread-safe. This should not be an issue since dialogs are usually shown
        /// only on a single GUI thread per process.</note></remarks>

        public static int[] CustomColorSet { get; set; }

        #endregion
        #region Show(IWin32Window, Color)

        /// <overloads>
        /// Displays a <see cref="CustomColorDialog"/> in front of the specified window and with the
        /// specified color initially selected.</overloads>
        /// <summary>
        /// Displays a <see cref="CustomColorDialog"/> in front of the specified Windows Forms <see
        /// cref="IWin32Window"/> and with the specified GDI+ color initially selected.</summary>
        /// <param name="owner">
        /// A Windows Forms <see cref="IWin32Window"/> instance indicating the parent window of the
        /// dialog.</param>
        /// <param name="color"><para>
        /// The initially selected <see cref="Color"/>. <see cref="Color.Transparent"/> translates
        /// to <see cref="Color.Black"/>.
        /// </para><para>
        /// On return, contains the current color selection if the dialog was dismissed by clicking
        /// <b>OK</b>; otherwise unchanged.</para></param>
        /// <returns>
        /// A <see cref="DialogResult"/> value indicating how the <see cref="CustomColorDialog"/>
        /// was dismissed.</returns>
        /// <remarks><para>
        /// If the <see cref="CustomColorDialog"/> was dismissed by clicking <b>OK</b>, <b>Show</b>
        /// stores the current color selection in the <paramref name="color"/> argument.
        /// </para><para>
        /// The current set of <see cref="ColorDialog.CustomColors"/> is stored in the static <see
        /// cref="CustomColorSet"/> property so that it can be restored when the next instance of
        /// the <b>CustomColorDialog</b> class is created.</para></remarks>

        public static DialogResult Show(IWin32Window owner, ref Color color) {

            using (CustomColorDialog dialog = new CustomColorDialog(color)) {
                DialogResult result = dialog.ShowDialog(owner);

                // retrieve selected and custom colors on OK
                if (result == DialogResult.OK) {
                    color = dialog.Color;
                    CustomColorDialog.CustomColorSet = dialog.CustomColors;
                }

                return result;
            }
        }

        #endregion
        #region Show(WpfWindow, WpfColor)

        /// <summary>
        /// Displays a <see cref="CustomColorDialog"/> in front of the specified WPF <see
        /// cref="WpfWindow"/> and with the specified WPF color initially selected.</summary>
        /// <param name="owner">
        /// A WPF <see cref="WpfWindow"/> indicating the parent window of the dialog.</param>
        /// <param name="color"><para>
        /// The initially selected <see cref="WpfColor"/>. <see cref="WpfColors.Transparent"/>
        /// translates to <see cref="WpfColors.Black"/>.
        /// </para><para>
        /// On return, contains the current color selection if the dialog was dismissed by clicking
        /// <b>OK</b>; otherwise unchanged.</para></param>
        /// <returns>
        /// <c>true</c> if the <see cref="CustomColorDialog"/> was dismissed with a <see
        /// cref="DialogResult"/> of <see cref="DialogResult.OK"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks><para>
        /// If the <see cref="CustomColorDialog"/> was dismissed by clicking <b>OK</b>, <b>Show</b>
        /// stores the current color selection in the <paramref name="color"/> argument.
        /// </para><para>
        /// The current set of <see cref="ColorDialog.CustomColors"/> is stored in the static <see
        /// cref="CustomColorSet"/> property so that it can be restored when the next instance of
        /// the <b>CustomColorDialog</b> class is created.</para></remarks>

        public static bool Show(WpfWindow owner, ref WpfColor color) {

            using (CustomColorDialog dialog = new CustomColorDialog(color.ToGdiColor())) {
                DialogResult result = dialog.ShowDialog(new HwndWrapper(owner));

                // retrieve selected and custom colors on OK
                if (result == DialogResult.OK) {
                    color = dialog.Color.ToWpfColor();
                    CustomColorDialog.CustomColorSet = dialog.CustomColors;
                }

                return (result == DialogResult.OK);
            }
        }

        #endregion
    }
}
