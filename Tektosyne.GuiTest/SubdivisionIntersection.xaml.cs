using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Tektosyne.Geometry;
using Tektosyne.Windows;
using Tektosyne.Xml;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> for testing the <see cref="Subdivision.Intersection"/>
    /// method of the <see cref="Subdivision"/> class.</summary>
    /// <remarks>
    /// <b>SubdivisionIntersection</b> intersects an existing planar subdivision with a user-defined
    /// rectangle. All edges and faces are labeled with their keys.</remarks>

    public partial class SubdivisionIntersection: Window {
        #region SubdivisionIntersection()

        public SubdivisionIntersection() {
            InitializeComponent();

            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(50),
                DispatcherPriority.ApplicationIdle, OnTimerTick, Dispatcher);

            DrawSubdivision(null);
        }

        #endregion
        #region Private Fields

        // timer to check mouse position
        private readonly DispatcherTimer _timer;

        // current subdivision & face mapping
        private Subdivision _division;
        private ValueTuple<Int32, Int32>[] _faceKeys;

        #endregion
        #region OutputBounds

        private RectD OutputBounds {
            get {
                double margin = 2 * FontSize;
                return new RectD(margin, margin,
                    OutputBox.Width - 2 * margin, OutputBox.Height - 2 * margin);
            }
        }

        #endregion
        #region DrawSubdivision

        private void DrawSubdivision(Subdivision division) {

            // default to empty subdivision
            if (division == null) division = new Subdivision();

            _division = division;
            _division.Validate();
            SubdivisionTest.DrawSubdivision(OutputBox, FontSize, division);
        }

        #endregion
        #region CopyCommandExecuted

        private void CopyCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;

            try {
                LineD[] lines = _division.ToLines();
                string text = XmlSerialization.Serialize<LineD[]>(lines);
                Clipboard.SetText(text);
            }
            catch (Exception e) {
                MessageDialog.Show(this,
                    "An error occurred while attempting to copy\n" +
                    "the current subdivision to the clipboard.",
                    Strings.ClipboardCopyError, e, MessageBoxButton.OK,
                    WindowsUtility.GetSystemBitmap(MessageBoxImage.Error));
            }
        }

        #endregion
        #region NewCommandExecuted

        private void NewCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            DrawSubdivision(null);
        }

        #endregion
        #region PasteCommandCanExecute

        private void PasteCommandCanExecute(object sender, CanExecuteRoutedEventArgs args) {
            args.Handled = true;
            args.CanExecute = Clipboard.ContainsText();
        }

        #endregion
        #region PasteCommandExecuted

        private void PasteCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;

            try {
                string text = Clipboard.GetText();
                LineD[] lines = XmlSerialization.Deserialize<LineD[]>(text);
                Subdivision division = Subdivision.FromLines(lines);
                DrawSubdivision(division);
            }
            catch (Exception e) {
                MessageDialog.Show(this,
                    "An error occurred while attempting to\n" +
                    "paste a new subdivision from the clipboard.",
                    Strings.ClipboardPasteError, e, MessageBoxButton.OK,
                    WindowsUtility.GetSystemBitmap(MessageBoxImage.Error));
            }
        }

        #endregion
        #region OnIntersect

        private void OnIntersect(object sender, RoutedEventArgs args) {
            args.Handled = true;

            RectD bounds = OutputBounds;
            double x = bounds.Left + (double) LeftUpDown.Value;
            double y = bounds.Top + (double) TopUpDown.Value;
            double dx = (double) WidthUpDown.Value;
            double dy = (double) HeightUpDown.Value;

            Subdivision rectangle;
            if (sender == RectangleButton)
                rectangle = Subdivision.FromLines(new LineD[] {
                    new LineD(x, y, x + dx, y),
                    new LineD(x + dx, y, x + dx, y + dy),
                    new LineD(x + dx, y + dy, x, y + dy),
                    new LineD(x, y + dy, x, y)
                });
            else if (sender == DiamondButton)
                rectangle = Subdivision.FromLines(new LineD[] {
                    new LineD(x + dx/2, y, x + dx, y + dy/2),
                    new LineD(x + dx, y + dy/2, x + dx/2, y + dy),
                    new LineD(x + dx/2, y + dy, x, y + dy/2),
                    new LineD(x, y + dy/2, x + dx/2, y)
                });
            else return;

            rectangle.Validate();
            _division = Subdivision.Intersection(_division, rectangle, out _faceKeys);
            _division.Validate();
            SubdivisionTest.DrawSubdivision(OutputBox, FontSize, _division);
        }

        #endregion
        #region OnTimerTick

        private void OnTimerTick(object sender, EventArgs args) {
            if (_division == null) return;
            SubdivisionFace face = null;

            // check if mouse cursor is over subdivision
            Point cursor = Mouse.GetPosition(OutputBox);
            if (cursor.X >= 0 && cursor.X < OutputBox.Width &&
                cursor.Y >= 0 && cursor.Y < OutputBox.Height)
                face = _division.FindFace(cursor.ToPointD());

            // show current and previous face keys
            if (face == null) {
                CurrentFace.Content = -1;
                PreviousFace.Content = -1;
                IntersectFace.Content = -1;
            } else {
                CurrentFace.Content = face.Key;
                if (_faceKeys != null) {
                    PreviousFace.Content = _faceKeys[face.Key].Item1;
                    IntersectFace.Content = _faceKeys[face.Key].Item2;
                }
            }
        }

        #endregion
    }
}
