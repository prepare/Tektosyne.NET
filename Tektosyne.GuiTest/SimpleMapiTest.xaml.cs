using System;
using System.Windows;
using Tektosyne.Net;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> for testing the <see cref="MapiMail"/> class.</summary>
    /// <remarks>
    /// <b>SimpleMapiTest</b> allows the user to exercise the <see cref="MapiMail.Address"/>, <see
    /// cref="MapiMail.ResolveName"/>, and <see cref="MapiMail.SendMail"/> methods of the <see
    /// cref="MapiMail"/> class. The exact results depend on the user’s e-mail client.</remarks>

    public partial class SimpleMapiTest: Window {

        public SimpleMapiTest() {
            InitializeComponent();
        }

        private void OnAddressBook(object sender, RoutedEventArgs args) {
            args.Handled = true;
            try {
                MapiAddress[] recips = MapiMail.Address();
                if (recips.Length > 0)
                    AddressBox.Text = StringUtility.Validate(recips[0].Address, recips[0].Name);
            }
            catch (MapiException e) {
                MessageBox.Show(this, e.Message, "Simple MAPI Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnResolveName(object sender, RoutedEventArgs args) {
            args.Handled = true;
            try {
                MapiAddress recip = MapiMail.ResolveName(AddressBox.Text);
                AddressBox.Text = StringUtility.Validate(recip.Address, recip.Name);
            }
            catch (MapiException e) {
                MessageBox.Show(this, e.Message, "Simple MAPI Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnSendMail(object sender, RoutedEventArgs args) {
            args.Handled = true;
            try {
                MapiMail.SendMail("Test Message", "This is the text of the test message.",
                    new[] { new MapiAddress("", AddressBox.Text) }, null);
            }
            catch (MapiException e) {
                MessageBox.Show(this, e.Message, "Simple MAPI Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
