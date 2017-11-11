using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides an enhanced <see cref="MessageBox"/> dialog containing a separate display area for
    /// an optional second message text.</summary>
    /// <remarks><para>
    /// <b>MessageDialog</b> supports most of the features of the built-in <see cref="MessageBox"/>
    /// and optionally accepts a second message text which will be shown in a scrollable <see
    /// cref="TextBox"/>. This facility might be used to report technical details, so as to keep the
    /// primary message text short and obvious.
    /// </para><para>
    /// The <b>MessageDialog</b> is resizable if a second message text is specified, with the <see
    /// cref="TextBox"/> filling all additional space as the dialog is grown.</para></remarks>

    public partial class MessageDialog: Window {
        #region MessageDialog(...)

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDialog"/> class.</summary>
        /// <param name="summary">
        /// The text to display in the primary message area.</param>
        /// <param name="caption">
        /// The text to display in the title bar of the dialog.</param>
        /// <param name="details">
        /// The text to display in the secondary message area.</param>
        /// <param name="detailsCaption">
        /// The text to display above the secondary message area.</param>
        /// <param name="buttons">
        /// A <see cref="MessageBoxButton"/> value specifying the buttons to display at the bottom
        /// of the dialog.</param>
        /// <param name="image">
        /// The source for the <see cref="Image"/> to display to the left of the primary message
        /// area.</param>
        /// <remarks><para>
        /// The <see cref="MessageDialog"/> window will be centered on its <see
        /// cref="Window.Owner"/> by default, if one exists.
        /// </para><para>
        /// All <see cref="String"/> parameters may be null references or empty strings. If
        /// <paramref name="details"/> is not a valid string, the secondary message area is hidden
        /// and the <paramref name="detailsCaption"/> parameter is ignored. In this case, the
        /// <see cref="Window.ResizeMode"/> is also set to <see cref="ResizeMode.CanMinimize"/>.
        /// </para><para>
        /// When any of the specified <paramref name="buttons"/> is clicked, the <see
        /// cref="MessageDialogResult"/> property is set to the corresponding <see
        /// cref="MessageBoxResult"/> value and the <b>MessageDialog</b> is closed. "OK" and "Yes"
        /// cause the <see cref="Window.ShowDialog"/> method to return <c>true</c>, while "Cancel"
        /// and "No" cause a return value of <c>false</c>.
        /// </para><para>
        /// The specified <paramref name="image"/> may be a null reference to indicate that no image
        /// should appear. Use the <see cref="WindowsUtility.GetSystemBitmap"/> method to retrieve
        /// <see cref="BitmapSource"/> objects for the predefined <see cref="MessageBoxImage"/>
        /// values.</para></remarks>

        public MessageDialog(string summary, string caption,
            string details, string detailsCaption,
            MessageBoxButton buttons, ImageSource image) {

            InitializeComponent();

            // show caption if specified
            if (!String.IsNullOrEmpty(caption))
                Title = caption;

            // show summary if specified
            if (!String.IsNullOrEmpty(summary))
                DialogSummary.Text = summary;

            // show image if specified
            if (image != null)
                DialogImage.Source = image;
            else
                DialogImage.Visibility = Visibility.Collapsed;

            // remember details and show details caption if specified
            if (!String.IsNullOrEmpty(details)) {
                DialogDetails.Tag = details;

                if (!String.IsNullOrEmpty(detailsCaption))
                    DetailsCaption.Text = detailsCaption;
                else
                    DetailsCaption.Visibility = Visibility.Collapsed;
            } else {
                // otherwise, hide details area and disable resizing
                ResizeMode = ResizeMode.CanMinimize;
                DialogDetails.Visibility = Visibility.Collapsed;
                DetailsCaption.Visibility = Visibility.Collapsed;
            }

            // create bottom row of buttons
            switch (buttons) {

                case MessageBoxButton.OKCancel:
                    OKButton.IsDefault = true;
                    CancelButton.IsCancel = true;
                    OKButton.Visibility = CancelButton.Visibility = Visibility.Visible;
                    break;

                case MessageBoxButton.YesNo:
                    YesButton.IsDefault = true;
                    NoButton.IsCancel = true;
                    YesButton.Visibility = NoButton.Visibility = Visibility.Visible;
                    break;

                case MessageBoxButton.YesNoCancel:
                    YesButton.IsDefault = true;
                    CancelButton.IsCancel = true;
                    YesButton.Visibility = NoButton.Visibility =
                        CancelButton.Visibility = Visibility.Visible;
                    break;

                default:
                    OKButton.IsDefault = true;
                    OKButton.IsCancel = true;
                    OKButton.Visibility = Visibility.Visible;
                    break;
            }

            // no button has been clicked yet
            MessageDialogResult = MessageBoxResult.None;
        }

        #endregion
        #region MessageDialogResult

        /// <summary>
        /// Gets the result of the <see cref="MessageDialog"/>.</summary>
        /// <value>
        /// A <see cref="MessageBoxResult"/> value indicating which of the buttons of the <see
        /// cref="MessageDialog"/> was clicked. The default is <see cref="MessageBoxResult.None"/>.
        /// </value>
        /// <remarks>
        /// <b>MessageDialogResult</b> always corresponds to the return value of the <see
        /// cref="Window.ShowDialog"/> method, but encodes the clicked <see cref="Button"/> as a 
        /// <see cref="MessageBoxResult"/> value rather than a <see cref="Nullable{Boolean}"/>.
        /// </remarks>

        public MessageBoxResult MessageDialogResult { get; private set; }

        #endregion
        #region OnContentRendered

        /// <summary>
        /// Raises and handles the <see cref="Window.ContentRendered"/> event.</summary>
        /// <param name="args">
        /// An <see cref="EventArgs"/> object containing event data.</param>
        /// <remarks><para>
        /// <b>OnContentRendered</b> first raises the <see cref="Window.ContentRendered"/> event by
        /// calling the base class implementation of <see cref="Window.OnContentRendered"/>.
        /// </para><para>
        /// <b>OnContentRendered</b> then handles the <b>ContentRendered</b> event by performing the
        /// following actions:
        /// </para><list type="number"><item>
        /// If the <see cref="FrameworkElement.Tag"/> of the secondary message area contains text,
        /// disable automatic window resizing, set the current <see cref="FrameworkElement.Height"/>
        /// to a reasonable value, and then display the desired secondary message.
        /// </item><item>
        /// Set <see cref="FrameworkElement.MinWidth"/> and <see cref="FrameworkElement.MinHeight"/>
        /// to the current <see cref="FrameworkElement.ActualWidth"/> and <see
        /// cref="FrameworkElement.ActualHeight"/>, respectively.</item></list></remarks>

        protected override void OnContentRendered(EventArgs args) {
            base.OnContentRendered(args);

            if (DialogDetails.Tag != null) {

                // lock dialog size before showing details
                SizeToContent = SizeToContent.Manual;
                Height = Math.Max(ActualHeight + 48, 0.75 * ActualWidth);

                DialogDetails.Text = (string) DialogDetails.Tag;
            }

            MinWidth = ActualWidth;
            MinHeight = ActualHeight;
        }

        #endregion
        #region Show(..., Exception, ...)

        /// <overloads>
        /// Displays a modal <see cref="MessageDialog"/>.</overloads>
        /// <summary>
        /// Displays a modal <see cref="MessageDialog"/> with the specified owner, message,
        /// exception data, buttons, and image.</summary>
        /// <param name="owner">
        /// The <see cref="Window"/> that owns the dialog.</param>
        /// <param name="summary">
        /// The text to display in the primary message area.</param>
        /// <param name="caption">
        /// The text to display in the title bar of the dialog.</param>
        /// <param name="exception">
        /// An <see cref="Exception"/> object providing additional primary and/or secondary message
        /// text, or a null reference for no additional text.</param>
        /// <param name="buttons">
        /// A <see cref="MessageBoxButton"/> value specifying the buttons to display at the bottom
        /// of the dialog.</param>
        /// <param name="image">
        /// The source for the <see cref="Image"/> to display to the left of the primary message
        /// area.</param>
        /// <returns>
        /// A <see cref="Nullable{Boolean}"/> value indicating how the <see cref="MessageDialog"/>
        /// was dismissed.</returns>
        /// <remarks><para>
        /// <b>Show</b> displays a modal <see cref="MessageDialog"/> with the specified parameters.
        /// Please refer to the <see cref="MessageDialog"/> constructor for details.
        /// </para><para>
        /// The primary message text is the specified <paramref name="summary"/>, if any; otherwise,
        /// the <see cref="Exception.Message"/> of the specified <paramref name="exception"/>, if
        /// any; otherwise, a localized message indicating an unknown error.
        /// </para><para>
        /// The secondary message area shows data on the specified <paramref name="exception"/> with
        /// a localized caption reading "Technical Details". The data includes the exception <see
        /// cref="Exception.Message"/>; the exception <see cref="DetailException.Detail"/> for <see
        /// cref="DetailException"/> objects; and the full string representation.</para></remarks>

        public static bool? Show(Window owner, string summary, string caption,
            Exception exception, MessageBoxButton buttons, ImageSource image) {

            string message = Strings.UnknownError, details = null;

            // show exception data if specified
            if (exception != null) {
                StringBuilder builder = new StringBuilder();

                // show exception message or "Unknown Error"
                message = StringUtility.Validate(exception.Message, message);
                builder.AppendLine(message);
                builder.AppendLine();

                // show technical details if specified
                DetailException de = exception as DetailException;
                if (de != null && !String.IsNullOrEmpty(de.Detail)) {
                    builder.AppendLine(de.Detail);
                    builder.AppendLine();
                }

                // show full exception text
                builder.Append(exception.ToString());
                details = builder.ToString();
            }

            // default to exception message or "Unknown Error"
            summary = StringUtility.Validate(summary, message);

            // let another Show overload handle the rest
            return Show(owner, summary, caption, details, null, buttons, image);
        }

        #endregion
        #region Show(..., String, String, ...)

        /// <summary>
        /// Displays a modal <see cref="MessageDialog"/> with the specified owner, messages,
        /// captions, buttons, and image.</summary>
        /// <param name="owner">
        /// The <see cref="Window"/> that owns the dialog.</param>
        /// <param name="summary">
        /// The text to display in the primary message area.</param>
        /// <param name="caption">
        /// The text to display in the title bar of the dialog.</param>
        /// <param name="details">
        /// The text to display in the secondary message area.</param>
        /// <param name="detailsCaption">
        /// The text to display above the secondary message area.</param>
        /// <param name="buttons">
        /// A <see cref="MessageBoxButton"/> value specifying the buttons to display at the bottom
        /// of the dialog.</param>
        /// <param name="image">
        /// The source for the <see cref="Image"/> to display to the left of the primary message
        /// area.</param>
        /// <returns>
        /// A <see cref="Nullable{Boolean}"/> value indicating how the <see cref="MessageDialog"/>
        /// was dismissed.</returns>
        /// <remarks><para>
        /// <b>Show</b> displays a modal <see cref="MessageDialog"/> with the specified parameters.
        /// Please refer to the <see cref="MessageDialog"/> constructor for details.
        /// </para><para>
        /// The <see cref="MessageDialog"/> is centered on the screen if the specified <paramref
        /// name="owner"/> is a null reference. If <paramref name="details"/> is valid but <paramref
        /// name="detailsCaption"/> is not, a localized caption reading "Technical Details" is
        /// shown.</para></remarks>

        public static bool? Show(Window owner, string summary,
            string caption, string details, string detailsCaption,
            MessageBoxButton buttons, ImageSource image) {

            // use default details caption if required and not specified
            if (!String.IsNullOrEmpty(details) &&
                String.IsNullOrEmpty(detailsCaption))
                detailsCaption = Strings.TechnicalDetails;

            MessageDialog dialog = new MessageDialog(summary,
                caption, details, detailsCaption, buttons, image);

            // center on screen if no owner specified
            if (owner == null)
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            else
                dialog.Owner = owner;

            return dialog.ShowDialog();
        }

        #endregion
        #region Private Event Handlers
        #region YesButtonClick

        /// <summary>
        /// Handles the <see cref="ButtonBase.Click"/> event for the "Yes" <see cref="Button"/>.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="Object"/> where the event handler is attached.</param>
        /// <param name="args">
        /// A <see cref="RoutedEventArgs"/> object containing event data.</param>
        /// <remarks>
        /// <b>YesButtonClick</b> sets the <see cref="MessageDialogResult"/> property to <see
        /// cref="MessageBoxResult.Yes"/> and the <see cref="Window.DialogResult"/> property to
        /// <c>true</c>.</remarks>

        private void YesButtonClick(object sender, RoutedEventArgs args) {
            args.Handled = true;
            MessageDialogResult = MessageBoxResult.Yes;
            DialogResult = true;
        }

        #endregion
        #region NoButtonClick

        /// <summary>
        /// Handles the <see cref="ButtonBase.Click"/> event for the "No" <see cref="Button"/>.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="Object"/> where the event handler is attached.</param>
        /// <param name="args">
        /// A <see cref="RoutedEventArgs"/> object containing event data.</param>
        /// <remarks>
        /// <b>NoButtonClick</b> sets the <see cref="MessageDialogResult"/> property to <see
        /// cref="MessageBoxResult.No"/> and the <see cref="Window.DialogResult"/> property to
        /// <c>false</c>.</remarks>

        private void NoButtonClick(object sender, RoutedEventArgs args) {
            args.Handled = true;
            MessageDialogResult = MessageBoxResult.No;
            DialogResult = false;
        }

        #endregion
        #region OKButtonClick

        /// <summary>
        /// Handles the <see cref="ButtonBase.Click"/> event for the "OK" <see cref="Button"/>.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="Object"/> where the event handler is attached.</param>
        /// <param name="args">
        /// A <see cref="RoutedEventArgs"/> object containing event data.</param>
        /// <remarks>
        /// <b>OKButtonClick</b> sets the <see cref="MessageDialogResult"/> property to <see
        /// cref="MessageBoxResult.OK"/> and the <see cref="Window.DialogResult"/> property to
        /// <c>true</c>.</remarks>

        private void OKButtonClick(object sender, RoutedEventArgs args) {
            args.Handled = true;
            MessageDialogResult = MessageBoxResult.OK;
            DialogResult = true;
        }

        #endregion
        #region CancelButtonClick

        /// <summary>
        /// Handles the <see cref="ButtonBase.Click"/> event for the "Cancel" <see cref="Button"/>.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="Object"/> where the event handler is attached.</param>
        /// <param name="args">
        /// A <see cref="RoutedEventArgs"/> object containing event data.</param>
        /// <remarks>
        /// <b>CancelButtonClick</b> sets the <see cref="MessageDialogResult"/> property to <see
        /// cref="MessageBoxResult.Cancel"/> and the <see cref="Window.DialogResult"/> property to
        /// <c>false</c>.</remarks>

        private void CancelButtonClick(object sender, RoutedEventArgs args) {
            args.Handled = true;
            MessageDialogResult = MessageBoxResult.Cancel;
            DialogResult = false;
        }

        #endregion
        #endregion
    }
}
