using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

using WfControl = System.Windows.Forms.Control;
using WfLabel = System.Windows.Forms.Label;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides a <see cref="WindowsFormsHost"/> that shifts input focus to its hosted Windows
    /// Forms <see cref="WfControl"/>.</summary>
    /// <remarks><para>
    /// WPF input controls that do not declare their own <see cref="AccessText"/> are usually
    /// associated with a <see cref="Label"/> control whose <see cref="Label.Target"/> binding
    /// refers to the input control. When the label’s keyboard shortcut is pressed, WPF transfers
    /// the input focus to the targeted input control instead.
    /// </para><para>
    /// Unfortunately, binding <b>Target</b> directly to a <see cref="WindowsFormsHost"/> element or
    /// to its hosted Windows Forms control works unreliably. Apparently, only the first Windows
    /// Forms control hosted in a WPF <see cref="Window"/> is ever activated by this binding.
    /// </para><para>
    /// <b>WindowsFormsHostEx</b> works around this issue by overriding <see
    /// cref="UIElement.OnGotFocus"/> to explicitly shift the input focus to the hosted <see
    /// cref="WindowsFormsHost.Child"/> control. You can associate a hosted Windows Forms control
    /// with the keyboard shortcut of a WPF label as follows:
    /// </para><list type="number"><item>
    /// Substitute <b>WindowsFormsHostEx</b> for <see cref="WindowsFormsHost"/>.
    /// </item><item>
    /// Assign an <c>x:Name</c> to the <b>WindowsFormsHostEx</b> element.
    /// </item><item>
    /// Bind that name to the <b>Target</b> of the desired <b>Label</b>.</item></list></remarks>

    public class WindowsFormsHostEx: WindowsFormsHost {
        #region OnGotFocus

        /// <summary>
        /// Raises and handles the <see cref="UIElement.GotFocus"/> event.</summary>
        /// <param name="args">
        /// A <see cref="RoutedEventArgs"/> object containing event data.</param>
        /// <remarks><para>
        /// <b>OnGotFocus</b> raises the <see cref="UIElement.GotFocus"/> event by calling the base
        /// class implementation of <see cref="UIElement.OnGotFocus"/>.
        /// </para><para>
        /// <b>OnGotFocus</b> then handles the <b>GotFocus</b> event by invoking <see
        /// cref="WfControl.Focus"/> on the hosted <see cref="WindowsFormsHost.Child"/> control, if
        /// any.</para></remarks>

        protected override void OnGotFocus(RoutedEventArgs args) {
            base.OnGotFocus(args);
            if (Child != null) Child.Focus();
        }

        #endregion
    }
}
