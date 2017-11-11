using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Tektosyne.Geometry;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> for testing the <see cref="PolygonGrid"/> class.</summary>
    /// <remarks><para>
    /// <b>PolygonGridTest</b> draws a resizable <see cref="PolygonGrid"/> based on a user-defined
    /// <see cref="RegularPolygon"/>. The window tracks the current element and shows its
    /// coordinates.
    /// </para><para>
    /// Additionally, the user may change the <see cref="PolygonGridShift"/>, highlight all
    /// neighbors within a given distance, and show all distances from the current element.
    /// </para></remarks>

    public partial class PolygonGridTest: Window {
        #region PolygonGridTest()

        public PolygonGridTest() {
            Instance = this;
            _ignoreControls = true;
            InitializeComponent();
            _ignoreControls = false;

            // create default polygon grid
            UpdatePolygons();
            UpdateGrid(new Size(200, 200));
        }

        #endregion
        #region Private Fields

        private static readonly SizeI _gridSize = new SizeI(10, 10);
        private bool _ignoreControls;

        #endregion
        #region Internal Properties

        internal static PolygonGridTest Instance { get; private set; }
        internal PolygonGrid Grid { get; private set; }
        internal RegularPolygon StandardPolygon { get; private set; }
        internal RegularPolygon InsetPolygon { get; private set; }
        internal StreamGeometry InsetGeometry { get; private set; }

        #endregion
        #region GridShift

        private PolygonGridShift GridShift {
            get {
                if (ColumnUpToggle.IsChecked == true) return PolygonGridShift.ColumnUp;
                if (ColumnDownToggle.IsChecked == true) return PolygonGridShift.ColumnDown;
                if (RowLeftToggle.IsChecked == true) return PolygonGridShift.RowLeft;
                if (RowRightToggle.IsChecked == true) return PolygonGridShift.RowRight;

                return PolygonGridShift.None;
            }
        }

        #endregion
        #region UpdateGrid

        internal void UpdateGrid(Size layoutSize) {
            Grid = new PolygonGrid(StandardPolygon, GridShift);
            Grid.Size = _gridSize;

            // check for available space when using standard size
            double ratioX = layoutSize.Width / Grid.DisplayBounds.Width;
            double ratioY = layoutSize.Height / Grid.DisplayBounds.Height;

            // increase grid size accordingly if possible
            Grid.Size = new SizeI(
                (int) (_gridSize.Width * ratioX) - 1,
                (int) (_gridSize.Height * ratioY) - 1);

            // show resulting actual grid size
            ColumnsBox.Text = Grid.Size.Width.ToString();
            RowsBox.Text = Grid.Size.Height.ToString();
        }

        #endregion
        #region UpdatePolygons

        private void UpdatePolygons() {

            // determine side count and integral parameters
            int sides = (SquareToggle.IsChecked == true ? 4 : 6);
            PolygonOrientation orientation = (OnEdgeToggle.IsChecked == true ?
                PolygonOrientation.OnEdge : PolygonOrientation.OnVertex);
            bool vertexNeighbors = (sides <= 4 ? (VertexNeighborsToggle.IsChecked == true) : false);

            // adjust side length based on side count
            double length = (double) (160.0 / sides);
            StandardPolygon = new RegularPolygon(length, sides, orientation, vertexNeighbors);
            InsetPolygon = StandardPolygon.Inflate(-3.0);

            // store inset vertices as stream geometry
            InsetGeometry = new StreamGeometry();
            using (StreamGeometryContext context = InsetGeometry.Open())
                InsetPolygon.Draw(context, PointD.Empty, true);
            InsetGeometry.Freeze();

            // determine valid & default controls
            EnableControls(sides, orientation);
        }

        #endregion
        #region OnContentRendered

        protected override void OnContentRendered(EventArgs args) {
            base.OnContentRendered(args);
            if (MinHeight == 0.0) MinHeight = ActualHeight;
        }

        #endregion
        #region OnMouseDown

        protected override void OnMouseDown(MouseButtonEventArgs args) {
            base.OnMouseDown(args);
            args.Handled = true;
            PolygonGridBox.HandleMouseDown(args);
        }

        #endregion
        #region OnMouseMove

        protected override void OnMouseMove(MouseEventArgs args) {
            base.OnMouseMove(args);
            args.Handled = true;
            PolygonGridBox.HandleMouseMove(args);
        }

        #endregion
        #region EnableControls

        private void EnableControls(int sides, PolygonOrientation orientation) {
            _ignoreControls = true;

            bool onEdge = (orientation == PolygonOrientation.OnEdge);
            RowLeftToggle.IsEnabled = !onEdge;
            RowRightToggle.IsEnabled = !onEdge;
            if (!onEdge) RowRightToggle.IsChecked = true;

            if (sides == 4) {
                VertexNeighborsToggle.IsEnabled = true;

                ShiftNoneToggle.IsEnabled = onEdge;
                ColumnUpToggle.IsEnabled = !onEdge;
                ColumnDownToggle.IsEnabled = !onEdge;
                if (onEdge) ShiftNoneToggle.IsChecked = true;
            } else {
                VertexNeighborsToggle.IsEnabled = false;

                ShiftNoneToggle.IsEnabled = false;
                ColumnUpToggle.IsEnabled = onEdge;
                ColumnDownToggle.IsEnabled = onEdge;
                if (onEdge) ColumnDownToggle.IsChecked = true;
            }

            _ignoreControls = false;
        }

        #endregion
        #region OnElementChanged

        private void OnElementChanged(object sender, RoutedEventArgs args) {
            args.Handled = true;
            if (_ignoreControls) return;

            UpdatePolygons();
            Grid = null;
            PolygonGridBox.InvalidateVisual();
        }

        #endregion
        #region OnGridChanged

        private void OnGridChanged(object sender, RoutedEventArgs args) {
            args.Handled = true;
            if (_ignoreControls) return;

            Grid = null;
            PolygonGridBox.InvalidateVisual();
        }

        #endregion
    }

    /// <summary>
    /// Renders the <see cref="PolygonGrid"/> defined by the <see cref="PolygonGridTest"/> dialog.
    /// </summary>

    public class PolygonGridRenderer: FrameworkElement {
        #region Private Fields

        private static readonly Size _border = new Size(12, 12);
        private static readonly Brush[] _neighborBrushes = {
            Brushes.Green,
            Brushes.YellowGreen,
            Brushes.LightGreen,
        };

        private bool _hasRendered;
        private Size _layoutSize, _oldLayoutSize;
        private StreamGeometry _gridGeometry;

        private PointI _showElement = PolygonGrid.InvalidLocation;
        private bool _showDistances;
        private int _showNeighbors;

        #endregion
        #region ArrangeOverride

        protected override Size ArrangeOverride(Size finalSize) {
            _layoutSize = finalSize;
            return finalSize;
        }

        #endregion
        #region MeasureOverride

        protected override Size MeasureOverride(Size availableSize) {
            Size measureSize = availableSize;
            /*
             * WPF always makes extremely large sizes available during initial layout,
             * so we artificially restrict them until after the first rendering pass.
             */

            if (!_hasRendered)
                measureSize = new Size(
                    Math.Min(200, availableSize.Width),
                    Math.Min(200, availableSize.Height));

            return measureSize;
        }

        #endregion
        #region HandleMouseDown

        internal void HandleMouseDown(MouseButtonEventArgs args) {

            // update current element if changed
            HandleMouseMove(args);
            if (_showElement == PolygonGrid.InvalidLocation)
                return;

            switch (args.ChangedButton) {

                case MouseButton.Left:
                    // highlight immediate neighbors of current element
                    _showDistances = false;
                    _showNeighbors = 1;
                    break;

                case MouseButton.Middle:
                    // highlight further neighbors of current element
                    _showDistances = false;
                    _showNeighbors = 3;
                    break;

                case MouseButton.Right:
                    // show distances from current element
                    _showDistances = true;
                    _showNeighbors = 0;
                    break;
            }

            InvalidateVisual();
        }

        #endregion
        #region HandleMouseMove

        internal void HandleMouseMove(MouseEventArgs args) {

            // determine element at cursor
            Point cursor = args.GetPosition(this);
            cursor = new Point(cursor.X - _border.Width, cursor.Y - _border.Height);
            PointI element = PolygonGridTest.Instance.Grid.DisplayToGrid(cursor.X, cursor.Y);

            // quit if current element unchanged
            if (_showElement == element)
                return;

            // show current element coordinates
            PolygonGridTest.Instance.CursorBox.Text = String.Format(
                CultureInfo.CurrentCulture, "{0},{1}", element.X, element.Y);

            // clear old element, neighbors & distances
            if (_showElement != PolygonGrid.InvalidLocation) {
                _showDistances = false;
                _showNeighbors = 0;
            }

            // highlight current element
            _showElement = element;
            InvalidateVisual();
        }

        #endregion
        #region OnRender

        protected override void OnRender(DrawingContext dc) {
            base.OnRender(dc);
            if (PolygonGridTest.Instance == null) return;

            // update grid if parameters or layout size changed
            if (PolygonGridTest.Instance.Grid == null ||
                _layoutSize != _oldLayoutSize) {

                _oldLayoutSize = _layoutSize;
                PolygonGridTest.Instance.UpdateGrid(_layoutSize);

                // draw updated grid to stream geometry
                _gridGeometry = new StreamGeometry();
                using (StreamGeometryContext context = _gridGeometry.Open())
                    PolygonGridTest.Instance.Grid.Draw(context, PointD.Empty, false);
                _gridGeometry.Freeze();
            }

            // superimpose decorations on prerendered grid
            dc.PushTransform(new TranslateTransform(_border.Width, _border.Height));
            dc.DrawGeometry(null, new Pen(Brushes.Black, 1), _gridGeometry);
            DrawDecoration(dc);
            dc.Pop();

            _hasRendered = true;
        }

        #endregion
        #region DrawDecoration

        private void DrawDecoration(DrawingContext dc) {
            if (_showElement == PolygonGrid.InvalidLocation)
                return;

            // always draw current element
            DrawElement(dc, _showElement, Brushes.Red, -1);
            PolygonGrid grid = PolygonGridTest.Instance.Grid;

            // show grid distances if desired
            if (_showDistances) {
                for (int x = 0; x < grid.Size.Width; x++)
                    for (int y = 0; y < grid.Size.Height; y++) {
                        PointI target = new PointI(x, y);
                        if (target != _showElement) {
                            int distance = grid.GetStepDistance(_showElement, target);

                            // use different brushes to highlight distances
                            int index = (distance - 1) % _neighborBrushes.Length;
                            DrawElement(dc, target, _neighborBrushes[index], distance);
                        }
                    }
            }

            // highlight neighbors if desired
            if (_showNeighbors > 0) {
                IList<PointI> neighbors = grid.GetNeighbors(_showElement, _showNeighbors);
                foreach (PointI neighbor in neighbors)
                    DrawElement(dc, neighbor, Brushes.Yellow, -1);
            }
        }

        #endregion
        #region DrawElement

        private void DrawElement(DrawingContext dc,
            PointI element, Brush brush, int distance) {

            // translate origin to center of current polygon
            PolygonGridTest dialog = PolygonGridTest.Instance;
            PointD center = dialog.Grid.GridToDisplay(element);
            dc.PushTransform(new TranslateTransform(center.X, center.Y));

            // draw inset polygon with specified brush
            if (brush != null)
                dc.DrawGeometry(brush, new Pen(Brushes.Black, 1), dialog.InsetGeometry);

            // draw distance if desired
            if (distance > 0) {
                Typeface typeface = dialog.FontFamily.GetTypefaces().ElementAt(0);

                FormattedText text = new FormattedText(distance.ToString(),
                    CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                    typeface, dialog.FontSize, Brushes.Black);

                dc.DrawText(text, new Point(-text.Width / 2.0, -text.Height / 2.0));
            }

            dc.Pop();
        }

        #endregion
    }
}
