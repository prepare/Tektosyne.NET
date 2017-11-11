using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Tektosyne.Windows;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides the top-level window for the Tektosyne GUI Test application.</summary>
    /// <remarks>
    /// The <b>Tektosyne.GuiTest</b> project serves as a scratchpad to test the <b>Tektosyne</b>
    /// library. The <b>MainWindow</b> class defines a main menu with several commands that test
    /// various <b>Tektosyne</b> classes. Menu items and event handlers can be modified as needed.
    /// </remarks>

    public partial class MainWindow: Window {
        #region MainWindow()

        public MainWindow() {
            InitializeComponent();
        }

        #endregion
        #region GetCommandText

        private static string GetCommandText(ICommand command) {
            RoutedUICommand uiCommand = command as RoutedUICommand;
            return (uiCommand == null ? "Unknown Command" : uiCommand.Text);
        }

        #endregion
        #region Command Event Handlers

        private void CommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            MessageBox.Show(this, "This command is not yet implemented.",
                GetCommandText(args.Command));
        }

        private void MenuGotFocus(object sender, RoutedEventArgs args) {
            args.Handled = true;
            MenuItem item = args.Source as MenuItem;
            if (item != null) StatusMessage.Content = item.ToolTip;
        }

        private void SubmenuClosed(object sender, RoutedEventArgs args) {
            args.Handled = true;
            StatusMessage.ClearValue(Label.ContentProperty);
        }

        #endregion
        #region File Menu

        private void ExitCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            System.Windows.Application.Current.Shutdown();
        }

        private void AssemblyCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new AssemblyDialog() { Owner = this };
            dialog.ShowDialog();
        }

        private void BenchmarkCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new BenchmarkDialog() { Owner = this };
            dialog.ShowDialog();
        }

        #endregion
        #region Geometry Menu

        private void ConvexHullCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new ConvexHullTest() { Owner = this };
            dialog.ShowDialog();
        }

        private void IntersectionCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new IntersectionTest() { Owner = this };
            dialog.ShowDialog();
        }

        private void PointInPolygonCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new PointInPolygonTest() { Owner = this };
            dialog.ShowDialog();
        }

        private void SubdivisionCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new SubdivisionTest() { Owner = this };
            dialog.ShowDialog();
        }

        private void SubdivisionIntersectionCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new SubdivisionIntersection() { Owner = this };
            dialog.ShowDialog();
        }

        private void VoronoiCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new VoronoiTest() { Owner = this };
            dialog.ShowDialog();
        }

        #endregion
        #region Grid & Graph Menu

        private void RegularPolygonCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new RegularPolygonTest() { Owner = this };
            dialog.ShowDialog();
        }

        private void PolygonGridCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new PolygonGridTest() { Owner = this };
            dialog.ShowDialog();
        }

        private void MakeGridCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new MakeGridDialog() { Owner = this };
            dialog.ShowDialog();
        }

        private void GraphCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new GraphDialog() { Owner = this };
            dialog.ShowDialog();
        }

        #endregion
        #region Windows Menu

        private void ConcurrentDrawingCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new ConcurrentDrawingTest() { Owner = this };
            dialog.ShowDialog();
        }

        private void MessageDialogCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            string summary = "This is a brief summary of the message.";

            string details = "This is a detailed description of the message, " +
                "including perhaps a lengthy exception stack trace.\n\n" +
                "MessageDialog combines the standard MessageBox icons and buttons " + 
                "with a scrollable text box for detailed information. " +
                "You can resize the dialog to increase the size of this scrollable text box.";

            ImageSource icon = WindowsUtility.GetSystemBitmap(MessageBoxImage.Information);
            MessageDialog.Show(this, summary, "Message Dialog",
                details, null, MessageBoxButton.OKCancel, icon);
        }

        private void SimpleMapiCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new SimpleMapiTest() { Owner = this };
            dialog.ShowDialog();
        }

        private void BitmapPaintCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new BitmapPaintTest() { Owner = this };
            dialog.ShowDialog();
        }

        private void BitmapOverlayCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            Window dialog = new BitmapOverlayTest() { Owner = this };
            dialog.ShowDialog();
        }

        #endregion
    }
}
