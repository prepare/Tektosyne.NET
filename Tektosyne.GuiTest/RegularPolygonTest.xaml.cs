using System;
using System.Windows;
using System.Windows.Media;

using Tektosyne.Geometry;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> for testing the <see cref="RegularPolygon"/> class.
    /// </summary>
    /// <remarks><para>
    /// <b>RegularPolygonTest</b> draws a resizable polygon with user-defined side count and
    /// orientation. The drawing includes the center of the polygon, the inscribed and circumscribed
    /// circles in red, and the bounding rectangle in green.
    /// </para><para>
    /// Additionally, the user may adjust a delta value to draw another polygon in blue that is
    /// identical to the first polygon except for an inflated or deflated circumcircle radius.
    /// </para></remarks>

    public partial class RegularPolygonTest: Window {
        #region RegularPolygonTest()

        public RegularPolygonTest() {
            Instance = this;
            InitializeComponent();

            // create default polygons
            UpdatePolygons(new Size(200, 200));
        }

        #endregion
        #region Internal Properties

        internal static RegularPolygonTest Instance { get; private set; }
        internal RegularPolygon StandardPolygon { get; private set; }
        internal RegularPolygon InflatedPolygon { get; private set; }

        #endregion
        #region UpdatePolygons

        internal void UpdatePolygons(Size layoutSize) {

            // determine side count and orientation
            int sides = (int) SidesUpDown.Value;
            PolygonOrientation orientation = (OnEdgeToggle.IsChecked == true ?
                PolygonOrientation.OnEdge : PolygonOrientation.OnVertex);

            // compute side length based on layout size and side count
            double layout = Math.Min(layoutSize.Width, layoutSize.Height);
            double length = 2.5 * layout / sides;
            StandardPolygon = new RegularPolygon(length, sides, orientation);

            // determine inflation relative to standard polygon
            int inflation = (int) InflationUpDown.Value;
            InflatedPolygon = StandardPolygon.Inflate(inflation);
        }

        #endregion
        #region OnPolygonChanged

        private void OnPolygonChanged(object sender, EventArgs args) {
            RoutedEventArgs routedArgs = args as RoutedEventArgs;
            if (routedArgs != null) routedArgs.Handled = true;

            if (PolygonBox != null)
                PolygonBox.InvalidateVisual();
        }

        #endregion
    }

    /// <summary>
    /// Renders the <see cref="RegularPolygon"/> defined by the <see cref="RegularPolygonTest"/>
    /// dialog.</summary>

    public class PolygonRenderer: FrameworkElement {
        #region Private Fields

        private bool _hasRendered;
        private Size _layoutSize;

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
             * WPF supplies an extremely large availableSize during initial layout,
             * so we artificially restrict it until after the first rendering pass.
             */
            if (!_hasRendered)
                measureSize = new Size(
                    Math.Min(200, availableSize.Width),
                    Math.Min(200, availableSize.Height));

            return measureSize;
        }

        #endregion
        #region OnRender

        protected override void OnRender(DrawingContext dc) {
            base.OnRender(dc);
            if (RegularPolygonTest.Instance == null) return;

            // calculate polygon measures for layout size
            RegularPolygonTest.Instance.UpdatePolygons(_layoutSize);
            RegularPolygon standard = RegularPolygonTest.Instance.StandardPolygon;
            RegularPolygon inflated = RegularPolygonTest.Instance.InflatedPolygon;

            // create requisite pens
            Pen blackPen = new Pen(Brushes.Black, 1),
                bluePen = new Pen(Brushes.Blue, 1),
                greenPen = new Pen(Brushes.Green, 1),
                redPen = new Pen(Brushes.Red, 1);

            // translate origin to center of layout area
            dc.PushTransform(new TranslateTransform(
                _layoutSize.Width / 2, _layoutSize.Height / 2));

            // draw standard polygon and its center point
            var geometry = new PathGeometry();
            geometry.Figures.Add(standard.ToFigure());
            dc.DrawGeometry(null, blackPen, geometry);
            dc.DrawEllipse(null, blackPen, new Point(), 2, 2);

            // draw inflated polygon if available
            if (inflated != standard) {
                geometry = new PathGeometry();
                geometry.Figures.Add(inflated.ToFigure());
                dc.DrawGeometry(null, bluePen, geometry);
            }

            // draw inscribed circle
            dc.DrawEllipse(null, redPen, new Point(0, 0),
                standard.InnerRadius, standard.InnerRadius);

            // draw circumscribed circle
            dc.DrawEllipse(null, redPen, new Point(0, 0),
                standard.OuterRadius, standard.OuterRadius);

            // draw circumscribed rectangle
            dc.DrawRectangle(null, greenPen, standard.Bounds.ToWpfRect());

            dc.Pop();
            _hasRendered = true;
        }

        #endregion
    }
}
