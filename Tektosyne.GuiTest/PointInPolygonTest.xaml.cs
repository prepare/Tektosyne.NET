using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Tektosyne.Geometry;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> for testing the <see cref="GeoAlgorithms.PointInPolygon"/>
    /// algorithm.</summary>
    /// <remarks>
    /// <b>PointInPolygonTest</b> draws a random arbitrary polygon and displays the <see
    /// cref="PolygonLocation"/> of the mouse cursor relative to that polygon.</remarks>

    public partial class PointInPolygonTest: Window {
        #region PointInPolygonTest()

        public PointInPolygonTest() {
            InitializeComponent();
            _polygon = GeoAlgorithms.RandomPolygon(0.0, 0.0, OutputBox.Width, OutputBox.Height);

            OutputBox.Children.Add(new Polygon() {
                Points = _polygon.ToWpfPoints(),
                Stroke = Brushes.Black
            });
        }

        #endregion
        #region Private Fields

        PointD[] _polygon;
        decimal _tolerance;

        #endregion
        #region OnMouseMove

        protected override void OnMouseMove(MouseEventArgs args) {
            base.OnMouseMove(args);
            args.Handled = true;

            PointD cursor = args.GetPosition(OutputBox).ToPointD();

            // determine relative location of cursor
            PolygonLocation location = (_tolerance == 0m ?
                GeoAlgorithms.PointInPolygon(cursor, _polygon) :
                GeoAlgorithms.PointInPolygon(cursor, _polygon, (double) _tolerance));

            LocationLabel.Content = location;
        }

        #endregion
        #region OnToleranceChanged

        private void OnToleranceChanged(object sender, EventArgs args) {
            RoutedEventArgs routedArgs = args as RoutedEventArgs;
            if (routedArgs != null) routedArgs.Handled = true;

            _tolerance = ToleranceUpDown.Value;
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
