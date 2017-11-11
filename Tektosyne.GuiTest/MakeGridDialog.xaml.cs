using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

using Tektosyne.Geometry;
using Tektosyne.Windows;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Allows the user to print or save <see cref="PolygonGrid"/> instances.</summary>
    /// <remarks>
    /// <b>MakeGridDialog</b> allows the user to print or save a <see cref="PolygonGrid"/> of an
    /// arbitrary size, based on a user-defined <see cref="RegularPolygon"/>.</remarks>

    public partial class MakeGridDialog: Window {
        #region MakeGridDialog()

        public MakeGridDialog() {
            _ignoreControls = true;
            InitializeComponent();
            _ignoreControls = false;

            // create initial element & grid
            OnElementChanged(this, EventArgs.Empty);
        }

        #endregion
        #region Grid & Element

        private bool _ignoreControls;

        internal PolygonGrid Grid { get; private set; }
        internal RegularPolygon Element { get; private set; }

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
        #region EnableControls

        private void EnableControls(int sides, PolygonOrientation orientation) {
            _ignoreControls = true;

            bool onEdge = (orientation == PolygonOrientation.OnEdge);
            RowLeftToggle.IsEnabled = !onEdge;
            RowRightToggle.IsEnabled = !onEdge;
            if (!onEdge) RowRightToggle.IsChecked = true;

            if (sides == 4) {
                ShiftNoneToggle.IsEnabled = onEdge;
                ColumnUpToggle.IsEnabled = !onEdge;
                ColumnDownToggle.IsEnabled = !onEdge;
                if (onEdge) ShiftNoneToggle.IsChecked = true;
            } else {
                ShiftNoneToggle.IsEnabled = false;
                ColumnUpToggle.IsEnabled = onEdge;
                ColumnDownToggle.IsEnabled = onEdge;
                if (onEdge) ColumnDownToggle.IsChecked = true;
            }

            _ignoreControls = false;
        }

        #endregion
        #region OnElementChanged

        private void OnElementChanged(object sender, EventArgs args) {

            RoutedEventArgs routedArgs = args as RoutedEventArgs;
            if (routedArgs != null) routedArgs.Handled = true;
            if (_ignoreControls) return;

            // determine side count and orientation
            int sides = (SquareToggle.IsChecked == true ? 4 : 6);
            PolygonOrientation orientation = (OnEdgeToggle.IsChecked == true ?
                PolygonOrientation.OnEdge : PolygonOrientation.OnVertex);

            // determine side length
            double length = (double) ElementUpDown.Value;
            Element = new RegularPolygon(length, sides, orientation);

            // determine valid & default controls
            EnableControls(sides, orientation);

            // recalculate grid accordingly
            OnGridChanged(sender, args);
        }

        #endregion
        #region OnGridChanged

        private void OnGridChanged(object sender, EventArgs args) {

            RoutedEventArgs routedArgs = args as RoutedEventArgs;
            if (routedArgs != null) routedArgs.Handled = true;
            if (_ignoreControls) return;

            Grid = new PolygonGrid(Element, GridShift);

            // determine number of columns and rows
            int width = (int) ColumnsUpDown.Value;
            int height = (int) RowsUpDown.Value;
            Grid.Size = new SizeI(width, height);

            // show resulting output size
            RectD bounds = Grid.DisplayBounds;
            WidthBox.Text = bounds.Width.ToString("N0");
            HeightBox.Text = bounds.Height.ToString("N0");
        }

        #endregion
        #region OnSave

        private void OnSave(object sender, RoutedEventArgs args) {
            args.Handled = true;

            // show preview and wait for input
            SaveGridDialog dialog = new SaveGridDialog() { Owner = this };
            dialog.VisualHost.VisualChild = CreateVisual(new PointD());
            if (dialog.ShowDialog() != true) return;

            // ask user for PNG file name
            string file = GetSaveFile();
            if (String.IsNullOrEmpty(file)) return;

            // grow by (1,1) to ensure all lines are fully visible
            SizeD size = Grid.DisplayBounds.Size;
            int width = Fortran.Ceiling(size.Width + 1),
                height = Fortran.Ceiling(size.Height + 1);

            // render grid to standard ARGB bitmap
            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(CreateVisual(new PointD()));
            bitmap.Freeze();

            try {
                // encode bitmap as PNG file
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Interlace = PngInterlaceOption.Off;
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                using (FileStream stream = new FileStream(file, FileMode.Create))
                    encoder.Save(stream);
            }
            catch (Exception e) {
                string message = String.Format(
                    "An error occurred while attempting\n" +
                    "to save the generated bitmap.\n\n{0}",
                    e.Message);

                MessageBox.Show(this, message, "Save Grid Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
        #region OnPrint

        private void OnPrint(object sender, RoutedEventArgs args) {
            args.Handled = true;

            // use 0.5" margins in both dimensions
            PointD margin = new PointD(48, 48);
            var host = new ContainerVisualHost(CreateVisual(margin));

            // create a page for the grid drawing...
            FixedPage page = new FixedPage();
            page.Children.Add(host);

            // ..and a page content holder for the page...
            PageContent content = new PageContent();
            ((IAddChild) content).AddChild(page);

            // ...and a document for the page content holder...
            FixedDocument document = new FixedDocument();
            document.Pages.Add(content);

            // ...and a document viewer for the document!
            DocumentViewer viewer = new DocumentViewer();
            viewer.Document = document;

            try {
                Window dialog = new Window() { Owner = this };
                dialog.Title = "Print Grid";
                dialog.Content = viewer;
                dialog.ShowDialog();
            }
            catch (Exception e) {
                string message = String.Format(
                    "An error occurred while attempting\n" +
                    "to print the generated bitmap.\n\n{0}",
                    e.Message);

                MessageBox.Show(this, message, "Print Grid Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
        #region CreateVisual

        private DrawingVisual CreateVisual(PointD margin) {

            // shift by (0.5, 0.5) to simulate "pixel snapping"
            margin += new PointD(0.5, 0.5);
            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext context = geometry.Open())
                Grid.Draw(context, margin, false);

            // ensure that all line joins are fully drawn
            Pen pen = new Pen(Brushes.Black, 1);
            pen.StartLineCap = pen.EndLineCap = PenLineCap.Square;
            pen.LineJoin = PenLineJoin.Miter;
            pen.MiterLimit = 100;

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
                context.DrawGeometry(null, pen, geometry);

            return visual;
        }

        #endregion
        #region GetSaveFile

        private string GetSaveFile() {
            SaveFileDialog dialog = new SaveFileDialog();

            // only accept PNG graphics format
            dialog.DefaultExt = ".png";
            dialog.Filter = "Portable Network Graphics (*.png)|*.png|All files (*.*)|*.*";
            dialog.FilterIndex = 0;
            dialog.Title = "Save Grid As";

            if (dialog.ShowDialog(this) == true)
                return dialog.FileName;

            return null;
        }

        #endregion
    }
}
