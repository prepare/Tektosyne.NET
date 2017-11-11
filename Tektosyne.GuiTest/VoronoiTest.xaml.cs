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
    /// Provides a <see cref="Window"/> for testing the <see cref="Voronoi"/> algorithm.</summary>
    /// <remarks>
    /// <b>VoronoiTest</b> draws a random set of points and then superimposes its Voronoi diagram
    /// and Delaunay triangulation.</remarks>

    public partial class VoronoiTest: Window {
        #region VoronoiTest()

        public VoronoiTest() {
            InitializeComponent();
            DrawDiagrams(null);
        }

        #endregion
        #region Private Fields

        // current point set
        private PointD[] _points;

        #endregion
        #region DrawDiagrams

        private void DrawDiagrams(PointD[] points) {
            RectD bounds = new RectD(0, 0, OutputBox.Width, OutputBox.Height);

            // generate new random point set if desired
            if (points == null) {
                PointD offset = new PointD(bounds.Width * 0.1, bounds.Height * 0.1);
                SizeD scale = new SizeD(bounds.Width * 0.8, bounds.Height * 0.8);

                int count = MersenneTwister.Default.Next(4, 20);
                points = new PointD[count];
                for (int i = 0; i < points.Length; i++)
                    points[i] = new PointD(
                        offset.X + MersenneTwister.Default.NextDouble() * scale.Width,
                        offset.Y + MersenneTwister.Default.NextDouble() * scale.Height);
            }

            _points = points;
            var result = Voronoi.FindAll(points, bounds);
            OutputBox.Children.Clear();

            // draw source points
            const double radius = 2.0;
            for (int i = 0; i < points.Length; i++) {
                Ellipse circle = new Ellipse() {
                    Width = 2 * radius, Height = 2 * radius,
                    Stroke = Brushes.Black,
                    Fill = Brushes.Black
                };

                Canvas.SetLeft(circle, points[i].X - radius);
                Canvas.SetTop(circle, points[i].Y - radius);
                OutputBox.Children.Add(circle);
            }

            // draw interior of Voronoi regions
            foreach (PointD[] region in result.VoronoiRegions) {
                Polygon polygon = new Polygon() {
                    Points = region.ToWpfPoints(),
                    Stroke = Brushes.White,
                    StrokeThickness = 6.0,
                    Fill = Brushes.PaleGoldenrod
                };
                OutputBox.Children.Add(polygon);
            }

            // draw edges of Voronoi diagram
            foreach (VoronoiEdge edge in result.VoronoiEdges) {
                PointD start = result.VoronoiVertices[edge.Vertex1];
                PointD end = result.VoronoiVertices[edge.Vertex2];

                Line line = new Line() {
                    X1 = start.X, Y1 = start.Y,
                    X2 = end.X, Y2 = end.Y,
                    Stroke = Brushes.Red
                };
                OutputBox.Children.Add(line);
            }

            // draw edges of Delaunay triangulation
            foreach (LineD edge in result.DelaunayEdges) {
                Line line = new Line() {
                    X1 = edge.Start.X, Y1 = edge.Start.Y,
                    X2 = edge.End.X, Y2 = edge.End.Y,
                    Stroke = Brushes.Blue,
                    StrokeDashArray = { 3.0, 2.0 }
                };
                OutputBox.Children.Add(line);
            }
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
            DrawDiagrams(null);
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
                DrawDiagrams(points);
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
