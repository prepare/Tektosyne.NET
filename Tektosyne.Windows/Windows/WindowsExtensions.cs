using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

using GdiColor = System.Drawing.Color;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides extension methods for <b>System.Windows</b> types.</summary>

    public static class WindowsExtensions {
        #region DoEvents

        /// <summary>
        /// Processes all messages currently in the message queue of the specified <see
        /// cref="Dispatcher"/>.</summary>
        /// <param name="dispatcher">
        /// The <see cref="Dispatcher"/> whose messages to process.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>DoEvents</b> synchronously invokes an empty delegate with <see
        /// cref="DispatcherPriority.Background"/> priority on the specified <paramref
        /// name="dispatcher"/>. This forces the <paramref name="dispatcher"/> to process all queued
        /// messages with a higher priority, such as redrawing the display.
        /// </para><note type="caution">
        /// <b>DoEvents</b> emulates the behavior of the eponymous Windows Forms method and shares
        /// its risk of reentrancy. The processed messages may cause recursion into user code or
        /// unexpected changes to the application state before <b>DoEvents</b> returns.
        /// </note></remarks>

        public static void DoEvents(this Dispatcher dispatcher) {
            dispatcher.Invoke(new Action(delegate { }), DispatcherPriority.Background);
        }

        #endregion
        #region GetTypeface

        /// <summary>
        /// Gets the current <see cref="Typeface"/> of the specified <see cref="Control"/>.
        /// </summary>
        /// <param name="control">
        /// The <see cref="Control"/> whose <see cref="Typeface"/> to return.</param>
        /// <returns>
        /// The current <see cref="Typeface"/> of the specified <paramref name="control"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="control"/> is a null reference.</exception>
        /// <remarks>
        /// <b>GetTypeface</b> creates the current <see cref="Typeface"/> of the specified <paramref
        /// name="control"/> from its current <see cref="Control.FontFamily"/>, <see
        /// cref="Control.FontStyle"/>, <see cref="Control.FontWeight"/>, and <see
        /// cref="Control.FontStretch"/> values.</remarks>

        public static Typeface GetTypeface(this Control control) {

            return new Typeface(control.FontFamily, control.FontStyle,
                control.FontWeight, control.FontStretch);
        }

        #endregion
        #region ScrollStep(ScrollViewer, ScrollDirection, Double, Double)

        /// <summary>
        /// Scrolls a <see cref="ScrollViewer"/> by the specified distance in the specified <see
        /// cref="ScrollDirection"/>.</summary>
        /// <param name="viewer">
        /// The <see cref="ScrollViewer"/> to scroll.</param>
        /// <param name="direction">
        /// A <see cref="ScrollDirection"/> value indicating the direction in which to scroll.
        /// </param>
        /// <param name="width">
        /// The horizontal distance by which to scroll, in device-independent pixels.</param>
        /// <param name="height">
        /// The vertical distance by which to scroll, in device-independent pixels.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="viewer"/> is a null reference.</exception>
        /// <exception cref="InvalidEnumArgumentException">
        /// <paramref name="direction"/> is not a valid <see cref="ScrollDirection"/> value.
        /// </exception>
        /// <remarks><para>
        /// <b>ScrollStep</b> uses either <paramref name="width"/> or <paramref name="height"/> to
        /// change the <see cref="ScrollViewer.HorizontalOffset"/> or <see
        /// cref="ScrollViewer.VerticalOffset"/> of the specified <paramref name="viewer"/>,
        /// depending on the specified <paramref name="direction"/>:
        /// </para><list type="table"><listheader>
        /// <term>Specified <paramref name="direction"/></term>
        /// <description>Use of <paramref name="width"/> or <paramref name="height"/></description>
        /// </listheader><item>
        /// <term><see cref="ScrollDirection.Left"/></term>
        /// <description>Scroll left by <paramref name="width"/></description>
        /// </item><item>
        /// <term><see cref="ScrollDirection.Right"/></term>
        /// <description>Scroll right by <paramref name="width"/></description>
        /// </item><item>
        /// <term><see cref="ScrollDirection.Up"/></term>
        /// <description>Scroll up by <paramref name="height"/></description>
        /// </item><item>
        /// <term><see cref="ScrollDirection.Down"/></term>
        /// <description>Scroll down by <paramref name="height"/></description>
        /// </item></list></remarks>

        public static void ScrollStep(this ScrollViewer viewer,
            ScrollDirection direction, double width, double height) {

            switch (direction) {
                case ScrollDirection.Left:
                    viewer.ScrollToHorizontalOffset(viewer.HorizontalOffset - width);
                    break;

                case ScrollDirection.Right:
                    viewer.ScrollToHorizontalOffset(viewer.HorizontalOffset + width);
                    break;

                case ScrollDirection.Up:
                    viewer.ScrollToVerticalOffset(viewer.VerticalOffset - height);
                    break;

                case ScrollDirection.Down:
                    viewer.ScrollToVerticalOffset(viewer.VerticalOffset + height);
                    break;

                default:
                    ThrowHelper.ThrowInvalidEnumArgumentException(
                        "direction", (int) direction, typeof(ScrollDirection));
                    break;
            }
        }

        #endregion
        #region ScrollStep(ScrollViewer, ScrollDirection, Size)

        /// <summary>
        /// Scrolls a <see cref="ScrollViewer"/> by the specified distance in the specified <see
        /// cref="ScrollDirection"/>.</summary>
        /// <param name="viewer">
        /// The <see cref="ScrollViewer"/> to scroll.</param>
        /// <param name="direction">
        /// A <see cref="ScrollDirection"/> value indicating the direction in which to scroll.
        /// </param>
        /// <param name="stepSize">
        /// A <see cref="Size"/> value indicating the horizontal or vertical distance by which to
        /// scroll, in device-independent pixels.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="viewer"/> is a null reference.</exception>
        /// <exception cref="InvalidEnumArgumentException">
        /// <paramref name="direction"/> is not a valid <see cref="ScrollDirection"/> value.
        /// </exception>
        /// <remarks>
        /// <b>ScrollStep</b> calls the other <see cref="ScrollStep(ScrollViewer, ScrollDirection,
        /// Double, Double)"/> overload with the <see cref="Size.Width"/> and <see
        /// cref="Size.Height"/> components of the specified <paramref name="stepSize"/>. Please see
        /// there for details.</remarks>

        public static void ScrollStep(this ScrollViewer viewer,
            ScrollDirection direction, Size stepSize) {

            ScrollStep(viewer, direction, stepSize.Width, stepSize.Height);
        }

        #endregion
        #region SelectAndShow(ListBox, Int32)

        /// <overloads>
        /// Selects the specified <see cref="ListBox"/> item and ensures that it is visible.
        /// </overloads>
        /// <summary>
        /// Selects the <see cref="ListBox"/> item with the specified <see cref="Int32"/> index, and
        /// ensures that it is visible.</summary>
        /// <param name="listBox">
        /// The <see cref="ListBox"/> containing the item.</param>
        /// <param name="index">
        /// The zero-based index of the item to select. This argument may be less than zero or
        /// otherwise invalid.</param>
        /// <returns>
        /// The specified <paramref name="index"/> if valid for the specified <paramref
        /// name="listBox"/>; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="listBox"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>SelectAndShow</b> selects the item with the specified <paramref name="index"/> in the
        /// <see cref="ItemsControl.Items"/> collection of the specified <paramref name="listBox"/>,
        /// and also calls <see cref="ListBox.ScrollIntoView"/> to ensure that it is visible.
        /// </para><para>
        /// <b>SelectAndShow</b> does nothing if the specified <paramref name="index"/> is less than
        /// zero, or greater than or equal to the number of <see cref="ItemsControl.Items"/> in the 
        /// <paramref name="listBox"/>.
        /// </para><para>
        /// If the <see cref="ListBox.SelectionMode"/> of the specified <paramref name="listBox"/>
        /// equals <see cref="SelectionMode.Single"/>, the indicated item becomes the new <see
        /// cref="Selector.SelectedItem"/>; otherwise, it is added to the <see
        /// cref="ListBox.SelectedItems"/> collection.
        /// </para><note type="caution">
        /// <see cref="ListView"/> derives from <see cref="ListBox"/> but its default <see
        /// cref="ListBox.SelectionMode"/> is <see cref="SelectionMode.Extended"/> rather than <see
        /// cref="SelectionMode.Single"/>.</note></remarks>

        public static int SelectAndShow(this ListBox listBox, int index) {

            // check for valid index
            if (index < 0 || index >= listBox.Items.Count)
                return -1;

            // select and show item, if any
            object item = listBox.Items[index];

            if (listBox.SelectionMode == SelectionMode.Single)
                listBox.SelectedIndex = index;
            else
                listBox.SelectedItems.Add(item);

            listBox.ScrollIntoView(item);
            return index;
        }

        #endregion
        #region SelectAndShow(ListBox, Object)

        /// <summary>
        /// Selects the <see cref="ListBox"/> item that equals or contains the specified <see
        /// cref="Object"/>, and ensures that it is visible.</summary>
        /// <param name="listBox">
        /// The <see cref="ListBox"/> containing the item.</param>
        /// <param name="item">
        /// The item to select. This argument may be a null reference.</param>
        /// <returns>
        /// The index position of <paramref name="item"/> if found within the specified <paramref
        /// name="listBox"/>; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="listBox"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>SelectAndShow</b> selects the specified <paramref name="item"/> in the <see
        /// cref="ItemsControl.Items"/> collection of the specified <paramref name="listBox"/>, and
        /// also calls <see cref="ListBox.ScrollIntoView"/> to ensure that it is visible.
        /// </para><para>
        /// <b>SelectAndShow</b> does nothing if the specified <paramref name="item"/> is not found
        /// among the <see cref="ItemsControl.Items"/> in the <paramref name="listBox"/>.
        /// </para><para>
        /// Please refer to the other <see cref="SelectAndShow(ListBox, Int32)"/> overload for
        /// further details.</para></remarks>

        public static int SelectAndShow(this ListBox listBox, object item) {

            // determine item index, if any
            int index = listBox.Items.IndexOf(item);
            if (index < 0) return -1;

            // select and show item if found
            if (listBox.SelectionMode == SelectionMode.Single)
                listBox.SelectedIndex = index;
            else
                listBox.SelectedItems.Add(item);

            listBox.ScrollIntoView(item);
            return index;
        }

        #endregion
        #region ToGdiColor

        /// <summary>
        /// Converts the specified WPF <see cref="Color"/> to its GDI+ representation.</summary>
        /// <param name="color">
        /// The WPF <see cref="Color"/> value to convert.</param>
        /// <returns>
        /// A GDI+ <see cref="GdiColor"/> value whose alpha and color channels are identical to
        /// those of the specified <paramref name="color"/>.</returns>
        /// <remarks>
        /// <b>ToGdiColor</b> only converts the <see cref="Byte"/> channel values of the specified
        /// <paramref name="color"/> since the GDI+ <see cref="GdiColor"/> structure cannot
        /// represent the full scRGB color space.</remarks>

        public static GdiColor ToGdiColor(this Color color) {
            return GdiColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        #endregion
        #region ToWpfColor

        /// <summary>
        /// Converts the specified GDI+ <see cref="GdiColor"/> to its WPF representation.</summary>
        /// <param name="color">
        /// The GDI+ <see cref="GdiColor"/> value to convert.</param>
        /// <returns>
        /// A WPF <see cref="Color"/> value whose alpha and color channels are identical to those of
        /// the specified <paramref name="color"/>.</returns>

        public static Color ToWpfColor(this GdiColor color) {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        #endregion
    }
}
