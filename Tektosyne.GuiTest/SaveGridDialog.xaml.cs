using System;
using System.Windows;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides the "Save Grid" dialog for the <see cref="MakeGridDialog"/>.</summary>
    /// <remarks>
    /// <b>SaveGridDialog</b> shows a preview of the current polygon grid, and returns <c>true</c>
    /// if the user clicks the "Save Grid" button.</remarks>

    public partial class SaveGridDialog: Window {

        public SaveGridDialog() {
            InitializeComponent();
        }

        private void OnSave(object sender, RoutedEventArgs args) {
            args.Handled = true;
            DialogResult = true;
        }
    }
}
