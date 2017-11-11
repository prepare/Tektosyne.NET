using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using Tektosyne.Collections;
using Tektosyne.Geometry;
using Tektosyne.Windows;
using Tektosyne.Xml;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> for testing the <see cref="Subdivision"/> class.</summary>
    /// <remarks>
    /// <b>SubdivisionTest</b> shows a planar subdivision created from a random Voronoi diagram. All
    /// edges and faces are labeled with their keys.</remarks>

    public partial class SubdivisionTest: Window {
        #region SubdivisionTest()

        public SubdivisionTest() {
            InitializeComponent();

            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(50),
                DispatcherPriority.ApplicationIdle, OnTimerTick, Dispatcher);

            DrawSubdivision(null);
        }

        #endregion
        #region Private Fields

        // timer to check mouse position
        private readonly DispatcherTimer _timer;

        // mapping of half-edge keys to paths
        private Int32Dictionary<Path> _edges = Int32Dictionary<Path>.Empty;

        // current subdivision & selections
        private Subdivision _division;
        private int _nearestEdge = -1, _selectedEdge = -1, _selectedVertex = -1;

        #endregion
        #region DrawSubdivision(Canvas, Double, Subdivision)

        /// <summary>
        /// Draws the specified <see cref="Subdivision"/> to the specified <see cref="Canvas"/>.
        /// </summary>
        /// <param name="canvas">
        /// The <see cref="Canvas"/> to draw on.</param>
        /// <param name="fontSize">
        /// The <see cref="Control.FontSize"/> for all text labels.</param>
        /// <param name="division">
        /// The <see cref="Subdivision"/> to draw on <paramref name="canvas"/>.</param>
        /// <returns>
        /// An <see cref="Int32Dictionary{T}"/> that maps the keys of all half-edges in <paramref
        /// name="division"/> to the corresponding WPF <see cref="Path"/>.</returns>

        internal static Int32Dictionary<Path> DrawSubdivision(
            Canvas canvas, double fontSize, Subdivision division) {

            canvas.Children.Clear();
            var edges = new Int32Dictionary<Path>(division.Edges.Count);

            // draw vertices
            const double radius = 2.0;
            foreach (PointD vertex in division.Vertices.Keys) {

                var circle = new Ellipse() {
                    Width = 2 * radius, Height = 2 * radius,
                    Stroke = Brushes.Black,
                    Fill = Brushes.Black
                };

                Canvas.SetLeft(circle, vertex.X - radius);
                Canvas.SetTop(circle, vertex.Y - radius);

                canvas.Children.Add(circle);
            }

            // draw half-edges with keys
            foreach (var pair in division.Edges) {
                LineD line = pair.Value.ToLine();

                double deltaX = line.End.X - line.Start.X;
                double deltaY = line.End.Y - line.Start.Y;
                double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                // slightly offset half-edge toward incident face
                double offsetX = radius * deltaX / length;
                double offsetY = radius * deltaY / length;

                double x0 = line.Start.X + offsetX - offsetY;
                double y0 = line.Start.Y + offsetX + offsetY;
                double x1 = line.End.X - offsetX - offsetY;
                double y1 = line.End.Y + offsetX - offsetY;

                // draw hook in direction of half-edge
                double angle = line.Angle + 0.75 * Math.PI;
                Point hookStart = new Point(x1 - deltaX / 4, y1 - deltaY / 4);
                Point hookEnd = new Point(
                    hookStart.X + Math.Cos(angle) * 8,
                    hookStart.Y + Math.Sin(angle) * 8);

                PathFigure[] figures = new PathFigure[] {
                    new PathFigure(new Point(x0, y0), new PathSegment[] {
                        new LineSegment(new Point(x1, y1), true) }, false),
                    new PathFigure(hookStart, new PathSegment[] {
                        new LineSegment(hookEnd, true) }, false)
                };

                PathGeometry geometry = new PathGeometry(figures);
                geometry.Freeze();
                Path path = new Path() { Stroke = Brushes.Red, Data = geometry };
                canvas.Children.Add(path);
                edges.Add(pair.Key, path);

                // draw key of current half-edge
                double centerX = (line.Start.X + line.End.X) / 2;
                double fontSizeX = (pair.Key < 10 && deltaY > 0) ? fontSize / 2 : fontSize;
                double textX = centerX - fontSizeX * deltaY / length - fontSize / 1.8;

                double centerY = (line.Start.Y + line.End.Y) / 2;
                double textY = centerY + fontSize * deltaX / length - fontSize / 1.5;

                TextBlock text = new TextBlock(new Run(pair.Key.ToString()));
                Canvas.SetLeft(text, textX);
                Canvas.SetTop(text, textY);
                canvas.Children.Add(text);
            }

            // draw keys of bounded faces
            foreach (var pair in division.Faces) {
                if (pair.Key == 0) continue;

                PointD centroid = pair.Value.OuterEdge.CycleCentroid;
                TextBlock text = new TextBlock(new Run(pair.Key.ToString()));
                text.FontWeight = FontWeights.Bold;

                Canvas.SetLeft(text, centroid.X - fontSize / 2);
                Canvas.SetTop(text, centroid.Y - fontSize / 2);
                canvas.Children.Add(text);
            }

            return edges;
        }

        #endregion
        #region DrawSubdivision(Subdivision)

        private void DrawSubdivision(Subdivision division) {

            // generate new random subdivision if desired
            if (division == null) {
                PointD offset = new PointD(OutputBox.Width * 0.2, OutputBox.Height * 0.2);
                SizeD scale = new SizeD(OutputBox.Width * 0.6, OutputBox.Height * 0.6);

                int count = MersenneTwister.Default.Next(4, 12);
                PointD[] points = new PointD[count];

                for (int i = 0; i < points.Length; i++)
                    points[i] = new PointD(
                        offset.X + MersenneTwister.Default.NextDouble() * scale.Width,
                        offset.Y + MersenneTwister.Default.NextDouble() * scale.Height);

                // outer bounds for Voronoi pseudo-vertices
                double margin = 3 * FontSize;
                RectD bounds = new RectD(margin, margin,
                    OutputBox.Width - 2 * margin, OutputBox.Height - 2 * margin);

                var result = Voronoi.FindAll(points, bounds);
                division = result.ToVoronoiSubdivision().Source;
            }

            _division = division;
            _division.Validate();
            _nearestEdge = -1;

            _edges = DrawSubdivision(OutputBox, FontSize, division);
            OutputBox.Children.Add(VertexCircle);
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
        #region OnMouseDown

        protected override void OnMouseDown(MouseButtonEventArgs args) {
            base.OnMouseDown(args);
            if (args.Handled || _division == null)
                return;

            Point cursor = Mouse.GetPosition(OutputBox);
            if (cursor.X < 0 || cursor.X >= OutputBox.Width ||
                cursor.Y < 0 || cursor.Y >= OutputBox.Height)
                return;

            if (AddEdgeToggle.IsChecked == true) {
                if (_selectedVertex >= 0) {
                    PointD oldVertex = _division.Vertices.GetKey(_selectedVertex);
                    _division.AddEdge(oldVertex, cursor.ToPointD());
                }
            } else if (RemoveEdgeToggle.IsChecked == true) {
                if (_selectedEdge >= 0)
                    _division.RemoveEdge(_selectedEdge);

            } else if (SplitEdgeToggle.IsChecked == true) {
                if (_selectedEdge >= 0)
                    _division.SplitEdge(_selectedEdge);

            } else if (ConnectVertexToggle.IsChecked == true) {
                if (_selectedVertex >= 0) {
                    PointD oldVertex = _division.Vertices.GetKey(_selectedVertex);
                    CollectionsUtility.AnyRandom(_division.Vertices.Keys,
                        p => (_division.AddEdge(oldVertex, p) != null));
                }
            } else if (MoveVertexToggle.IsChecked == true) {
                if (_selectedVertex >= 0) {
                    PointD newVertex = cursor.ToPointD();
                    _division.MoveVertex(_selectedVertex, newVertex);
                }
            } else if (RemoveVertexToggle.IsChecked == true) {
                if (_selectedVertex >= 0)
                    _division.RemoveVertex(_selectedVertex);

            } else return;

            DrawSubdivision(_division);
            SetNearestEdge(null, 0);
            SetSelectedEdge(-1);
            SetSelectedVertex(-1);
        }

        #endregion
        #region OnRenumberEdges

        private void OnRenumberEdges(object sender, RoutedEventArgs args) {
            args.Handled = true;
            if (_division.RenumberEdges())
                DrawSubdivision(_division);
        }

        #endregion
        #region OnRenumberFaces

        private void OnRenumberFaces(object sender, RoutedEventArgs args) {
            args.Handled = true;
            if (_division.RenumberFaces())
                DrawSubdivision(_division);
        }

        #endregion
        #region OnTimerTick

        private void OnTimerTick(object sender, EventArgs args) {
            if (_division == null) return;

            // clear current selections, if any
            if (_nearestEdge >= 0) SetNearestEdge(null, 0);
            if (_selectedEdge >= 0) SetSelectedEdge(-1);
            if (_selectedVertex >= 0) SetSelectedVertex(-1);

            // check if mouse cursor is over subdivision
            Point cursor = Mouse.GetPosition(OutputBox);
            if (cursor.X < 0 || cursor.X >= OutputBox.Width ||
                cursor.Y < 0 || cursor.Y >= OutputBox.Height)
                return;

            // show nearest half-edge, if any
            PointD q = cursor.ToPointD();
            double distance;
            SubdivisionEdge edge = _division.FindNearestEdge(q, out distance);
            if (edge != null) {
                SetNearestEdge(edge, distance);
                if (distance <= 30) SetSelectedEdge(edge.Key);
            }

            // show nearest vertex, if any
            var vertices = _division.Vertices;
            if (vertices.Count > 0) {
                var vertexComparer = (PointDComparerY) vertices.Comparer;
                int index = vertexComparer.FindNearest(vertices.Keys, q);
                SetSelectedVertex(index);
            }
        }

        #endregion
        #region SetNearestEdge

        private void SetNearestEdge(SubdivisionEdge edge, double distance) {
            NearestDistance.Content = distance.ToString("##0.00");

            if (edge == null) {
                _nearestEdge = -1;
                NearestEdge.Content = -1;
                NearestFace.Content = -1;
            } else {
                _nearestEdge = edge.Key;
                NearestEdge.Content = edge.Key;
                NearestFace.Content = edge.Face.Key;
            }
        }

        #endregion
        #region SetSelectedEdge

        private void SetSelectedEdge(int edge) {
            Path path;
            if (_selectedEdge >= 0 && _edges.TryGetValue(_selectedEdge, out path)) {
                path.Stroke = Brushes.Red;
                path.StrokeThickness = 1;
            }

            _selectedEdge = edge;
            if (_selectedEdge >= 0 && _edges.TryGetValue(_selectedEdge, out path)) {
                path.Stroke = Brushes.Black;
                path.StrokeThickness = 2;
            }
        }

        #endregion
        #region SetSelectedVertex

        private void SetSelectedVertex(int vertex) {
            _selectedVertex = (_division == null ? -1 : vertex);

            if (_selectedVertex >= 0) {
                PointD p = _division.Vertices.GetKey(_selectedVertex);
                Canvas.SetLeft(VertexCircle, p.X - VertexCircle.Width / 2);
                Canvas.SetTop(VertexCircle, p.Y - VertexCircle.Height / 2);
                VertexCircle.Visibility = Visibility.Visible;
            } else
                VertexCircle.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}
