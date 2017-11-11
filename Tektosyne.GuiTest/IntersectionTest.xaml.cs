using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Tektosyne.Geometry;
using Tektosyne.Windows;
using Tektosyne.Xml;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> for testing the brute force variant of the <see
    /// cref="MultiLineIntersection"/> algorithm.</summary>
    /// <remarks>
    /// <b>IntersectionTest</b> draws a random set of lines and marks any points of intersection
    /// that were found.</remarks>

    public partial class IntersectionTest: Window {
        #region IntersectionTest()

        public IntersectionTest() {
            InitializeComponent();
            DrawIntersections(null);
        }

        #endregion
        #region Private Fields

        // current lines & intersections
        private LineD[] _lines;
        MultiLinePoint[] _crossings;

        #endregion
        #region DrawIntersections

        private void DrawIntersections(LineD[] lines) {
            const double radius = 4.0;

            // generate new random line set if desired
            if (lines == null) {
                Size scale = new Size(OutputBox.Width - 4 * radius, OutputBox.Height - 4 * radius);

                int count = MersenneTwister.Default.Next(3, 20);
                lines = new LineD[count];

                for (int i = 0; i < lines.Length; i++)
                    lines[i] = GeoAlgorithms.RandomLine(
                        2 * radius, 2 * radius, scale.Width, scale.Height);
            }

            _lines = lines;
            double epsilon = (double) ToleranceUpDown.Value;
            _crossings = (epsilon > 0.0 ?
                MultiLineIntersection.FindSimple(lines, epsilon) :
                MultiLineIntersection.FindSimple(lines));

            LinesLabel.Content = String.Format("{0}/{1}", lines.Length, _crossings.Length);
            OutputBox.Children.Clear();

            // draw line set
            foreach (LineD line in lines) {
                var shape = new Line() {
                    X1 = line.Start.X, Y1 = line.Start.Y,
                    X2 = line.End.X, Y2 = line.End.Y,
                    Stroke = Brushes.Black
                };

                OutputBox.Children.Add(shape);
            }

            // draw intersections as hollow circles
            foreach (var crossing in _crossings) {
                var circle = new Ellipse() {
                    Width = 2 * radius, Height = 2 * radius,
                    Stroke = Brushes.Red
                };

                Canvas.SetLeft(circle, crossing.Shared.X - radius);
                Canvas.SetTop(circle, crossing.Shared.Y - radius);

                OutputBox.Children.Add(circle);
            }
        }

        #endregion
        #region CopyCommandExecuted

        private void CopyCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;

            try {
                string text = XmlSerialization.Serialize<LineD[]>(_lines);
                Clipboard.SetText(text);
            }
            catch (Exception e) {
                MessageDialog.Show(this,
                    "An error occurred while attempting to copy\n" +
                    "the current line set to the clipboard.",
                    Strings.ClipboardCopyError, e, MessageBoxButton.OK,
                    WindowsUtility.GetSystemBitmap(MessageBoxImage.Error));
            }
        }

        #endregion
        #region NewCommandExecuted

        private void NewCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            DrawIntersections(null);
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
                DrawIntersections(lines);
            }
            catch (Exception e) {
                MessageDialog.Show(this,
                    "An error occurred while attempting to\n" +
                    "paste a new line set from the clipboard.",
                    Strings.ClipboardPasteError, e, MessageBoxButton.OK,
                    WindowsUtility.GetSystemBitmap(MessageBoxImage.Error));
            }
        }

        #endregion
        #region OnSplit

        private void OnSplit(object sender, RoutedEventArgs args) {
            args.Handled = true;

            var lines = MultiLineIntersection.Split(_lines, _crossings);
            DrawIntersections(lines);
        }

        #endregion
        #region OnToleranceChanged

        private void OnToleranceChanged(object sender, EventArgs args) {
            RoutedEventArgs routedArgs = args as RoutedEventArgs;
            if (routedArgs != null) routedArgs.Handled = true;

            DrawIntersections(_lines);
        }

        #endregion
        #region OnToleranceMaximum

        private void OnToleranceMaximum(object sender, RoutedEventArgs args) {
            args.Handled = true;
            ToleranceUpDown.Value = ToleranceUpDown.Maximum;
        }

        #endregion
        #region OnToleranceMinimum

        private void OnToleranceMinimum(object sender, RoutedEventArgs args) {
            args.Handled = true;
            ToleranceUpDown.Value = ToleranceUpDown.Minimum;
        }

        #endregion
    }
}
