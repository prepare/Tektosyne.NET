using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Tektosyne;
using Tektosyne.Windows;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> for testing the <see cref="BitmapBuffer"/> and <see
    /// cref="BitmapUtility"/> classes.</summary>
    /// <remarks><para>
    /// <b>BitmapOverlayTest</b> shows a <see cref="WriteableBitmap"/> that represents a background
    /// image, and another <see cref="WriteableBitmap"/> that represents an immutable foreground
    /// image. Both images are associated with a <see cref="BitmapBuffer"/>. The user may overlay
    /// the two images in various ways, make the combined images opaque, restore the original
    /// background image, or create a new random background image.
    /// </para><para>
    /// This dialog tests manipulating pixel rectangles. The user may choose whether to use <see
    /// cref="BitmapUtility"/> methods to access the <see cref="WriteableBitmap"/> directly, or to
    /// use indirect access through a <see cref="BitmapBuffer"/> object.</para></remarks>

    public partial class BitmapOverlayTest: Window {
        #region BitmapOverlayTest()

        public BitmapOverlayTest() {
            _ignoreControls = true;
            InitializeComponent();

            DrawBackground();
            DrawForeground();
            _ignoreControls = false;
        }

        #endregion
        #region Fields & Properties

        private bool _ignoreControls;
        internal bool UseBitmapBuffer { get; private set; }

        internal RenderTargetBitmap BackgroundBitmap { get; private set; }
        internal BitmapBuffer BackgroundBuffer { get; private set; }

        internal RenderTargetBitmap ForegroundBitmap { get; private set; }
        internal BitmapBuffer ForegroundBuffer { get; private set; }

        #endregion
        #region Overlay Commands

        private void OnAlphaBlending(object sender, RoutedEventArgs args) {
            args.Handled = true;
            Overlay(0, Colors.Transparent);
        }

        private void OnAlphaCutoff(object sender, RoutedEventArgs args) {
            args.Handled = true;
            Overlay(127, Colors.Transparent);
        }

        private void OnColorSubstitution(object sender, RoutedEventArgs args) {
            args.Handled = true;
            Overlay(0, Colors.Aqua);
        }

        #endregion
        #region OnCreateBackground

        private void OnCreateBackground(object sender, RoutedEventArgs args) {
            args.Handled = true;

            // create new off-screen background bitmap
            BackgroundBitmap.Clear();
            BackgroundBitmap.Render(CreateBackground(
                BackgroundBitmap.PixelWidth, BackgroundBitmap.PixelHeight));

            // copy new background to associated bitmap
            OnRestoreBackground(sender, args);
        }

        #endregion
        #region OnMakeOpaque

        private void OnMakeOpaque(object sender, RoutedEventArgs args) {
            args.Handled = true;

            if (UseBitmapBuffer) {
                BackgroundBuffer.MakeOpaque();
                BackgroundBuffer.Write();
            } else {
                WriteableBitmap bitmap = BackgroundBuffer.Bitmap;
                bitmap.Lock();
                bitmap.MakeOpaque();
                bitmap.Unlock();
            }
        }

        #endregion
        #region OnMethodChanged

        private void OnMethodChanged(object sender, RoutedEventArgs args) {
            args.Handled = true;
            if (_ignoreControls) return;

            UseBitmapBuffer = (BitmapBufferToggle.IsChecked == true);
            OnRestoreBackground(sender, args);
        }

        #endregion
        #region OnRestoreBackground

        private void OnRestoreBackground(object sender, RoutedEventArgs args) {
            args.Handled = true;

            // copy original background to associated bitmap
            BackgroundBuffer.Read(BackgroundBitmap);
            BackgroundBuffer.Write();
        }

        #endregion
        #region CreateBackground

        private DrawingVisual CreateBackground(int width, int height) {

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen()) {

                Pen pen = new Pen(Brushes.Black, 1.0);
                for (int i = 0; i < 10; i++) {

                    // pick a random transparent fill color
                    Color color = Color.FromArgb(
                        (byte) (MersenneTwister.Default.Next(223) + 32),
                        (byte) MersenneTwister.Default.Next(255),
                        (byte) MersenneTwister.Default.Next(255),
                        (byte) MersenneTwister.Default.Next(255));

                    // pick a random upper-left corner
                    double x = MersenneTwister.Default.NextDouble() * (width - 10),
                        y = MersenneTwister.Default.NextDouble() * (height - 10);

                    // pick a random width and height
                    Rect bounds = new Rect(x, y,
                        10 + MersenneTwister.Default.NextDouble() * (width - x - 10),
                        10 + MersenneTwister.Default.NextDouble() * (height - y - 10));

                    // draw random rectangle with rounded corners
                    context.DrawRoundedRectangle(new SolidColorBrush(color), pen, bounds, 5, 5);
                }
            }

            return visual;
        }

        #endregion
        #region DrawBackground

        private void DrawBackground() {

            // create off-screen background bitmap
            BackgroundBitmap = new RenderTargetBitmap(200, 200, 96, 96, PixelFormats.Pbgra32);
            BackgroundBitmap.Render(CreateBackground(
                BackgroundBitmap.PixelWidth, BackgroundBitmap.PixelHeight));

            // create & show writeable background bitmap
            var writeableBitmap = new WriteableBitmap(BackgroundBitmap);
            BackgroundImage.Source = writeableBitmap;

            // associate background bitmap with buffer
            BackgroundBuffer = new BitmapBuffer(writeableBitmap);
            BackgroundBuffer.Read();
        }

        #endregion
        #region DrawForeground

        private void DrawForeground() {

            // draw foreground bitmap contents
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen()) {
                Pen pen = new Pen(Brushes.Black, 2.0);

                // draw translucent body of smiley
                Brush brush = new RadialGradientBrush(
                    Colors.Yellow, Color.FromArgb(32, 255, 255, 0));
                context.DrawEllipse(brush, pen, new Point(100, 100), 80, 80);

                // draw eyes of smiley
                context.DrawEllipse(Brushes.Black, pen, new Point(70, 80), 10, 10);
                context.DrawEllipse(Brushes.Black, pen, new Point(130, 80), 10, 10);

                // draw mouth of smiley
                PathFigure figure = new PathFigure() {
                    StartPoint = new Point(50, 120),
                    Segments = { new ArcSegment(new Point(150, 120), new Size(60, 70),
                        0, false, SweepDirection.Counterclockwise, true) }
                };

                context.DrawGeometry(null, pen, new PathGeometry() { Figures = { figure } });
            }

            // create & show foreground bitmap
            ForegroundBitmap = new RenderTargetBitmap(200, 200, 96, 96, PixelFormats.Pbgra32);
            ForegroundBitmap.Render(visual);

            // create & show writeable foreground bitmap
            var writeableBitmap = new WriteableBitmap(ForegroundBitmap);
            ForegroundImage.Source = writeableBitmap;

            // associate foreground bitmap with buffer
            ForegroundBuffer = new BitmapBuffer(writeableBitmap);
            ForegroundBuffer.Read();
        }

        #endregion
        #region Overlay

        private void Overlay(byte alpha, Color color) {
            if (UseBitmapBuffer) {
                // restore original background first
                BackgroundBuffer.Read(BackgroundBitmap);

                // use alpha cutoff, color substitution, or regular alpha blending
                if (alpha != 0)
                    BackgroundBuffer.Overlay(0, 0, ForegroundBuffer, ForegroundBuffer.Bounds, alpha);
                else if (color != Colors.Transparent)
                    BackgroundBuffer.Overlay(0, 0, ForegroundBuffer, ForegroundBuffer.Bounds, color);
                else
                    BackgroundBuffer.Overlay(0, 0, ForegroundBuffer, ForegroundBuffer.Bounds);

                BackgroundBuffer.Write();
            } else {
                // restore original background first
                WriteableBitmap bitmap = BackgroundBuffer.Bitmap;
                bitmap.Lock();
                bitmap.Read(0, 0, BackgroundBitmap);

                // use alpha cutoff, color substitution, or regular alpha blending
                if (alpha != 0)
                    bitmap.Overlay(0, 0, ForegroundBuffer.Bitmap, ForegroundBuffer.Bounds, alpha);
                else if (color != Colors.Transparent)
                    bitmap.Overlay(0, 0, ForegroundBuffer.Bitmap, ForegroundBuffer.Bounds, color);
                else
                    bitmap.Overlay(0, 0, ForegroundBuffer.Bitmap, ForegroundBuffer.Bounds);

                bitmap.Unlock();
            }
        }

        #endregion
    }
}
