using System;
using System.Linq;
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
    /// Provides a <see cref="Window"/> for testing the <see cref="GeoAlgorithms.ConvexHull"/>
    /// algorithm.</summary>
    /// <remarks>
    /// <b>ConvexHullTest</b> draws a random set of points and then superimposes the polygon that
    /// constitutes its convex hull.</remarks>

    public partial class ConvexHullTest: Window {
        #region ConvexHullTest()

        public ConvexHullTest() {
            InitializeComponent();
            DrawConvexHull(null);
        }

        #endregion
        #region Private Fields

        // current point set
        private PointD[] _points;

        #endregion
        #region DrawConvexHull

        private void DrawConvexHull(PointD[] points) {
            const double radius = 4.0;

            // generate new random point set if desired
            if (points == null) {
                Size scale = new Size(OutputBox.Width - 4 * radius, OutputBox.Height - 4 * radius);
                int count = MersenneTwister.Default.Next(4, 40);
                RectD bounds = new RectD(2 * radius, 2 * radius, scale.Width, scale.Height);
                points = GeoAlgorithms.RandomPoints(
                    count, bounds, new PointDComparerY(), 2 * radius);
            }

            _points = points;
            PointD[] polygon = GeoAlgorithms.ConvexHull(points);
            OutputBox.Children.Clear();

            // draw hull vertices filled, other points hollow
            for (int i = 0; i < points.Length; i++) {
                bool isVertex = polygon.Contains(points[i]);

                var circle = new Ellipse() {
                    Width = 2 * radius, Height = 2 * radius,
                    Stroke = Brushes.Black,
                    Fill = isVertex ? Brushes.Black : null
                };

                Canvas.SetLeft(circle, points[i].X - radius);
                Canvas.SetTop(circle, points[i].Y - radius);

                OutputBox.Children.Add(circle);
            }

            // draw edges of convex hull
            OutputBox.Children.Add(new Polygon() {
                Points = polygon.ToWpfPoints(),
                Stroke = Brushes.Red
            });
        }

        #endregion
        #region CopyCommandExecuted

        private void CopyCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;

            try {
                string text = XmlSerialization.Serialize<PointD[]>(_points);
                Clipboard.SetText(text);
            }
            catch (Exception e) {
                MessageDialog.Show(this,
                    "An error occurred while attempting to copy\n" +
                    "the current point set to the clipboard.",
                    Strings.ClipboardCopyError, e, MessageBoxButton.OK,
                    WindowsUtility.GetSystemBitmap(MessageBoxImage.Error));
            }
        }

        #endregion
        #region NewCommandExecuted

        private void NewCommandExecuted(object sender, ExecutedRoutedEventArgs args) {
            args.Handled = true;
            DrawConvexHull(null);
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
                PointD[] points = XmlSerialization.Deserialize<PointD[]>(text);
                DrawConvexHull(points);
            }
            catch (Exception e) {
                MessageDialog.Show(this,
                    "An error occurred while attempting to\n" + 
                    "paste a new point set from the clipboard.",
                    Strings.ClipboardPasteError, e, MessageBoxButton.OK,
                    WindowsUtility.GetSystemBitmap(MessageBoxImage.Error));
            }
        }

        #endregion
    }
}
