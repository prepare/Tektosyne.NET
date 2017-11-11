using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Tektosyne;
using Tektosyne.Geometry;
using Tektosyne.Windows;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> for testing the <see cref="BitmapBuffer"/> and <see
    /// cref="BitmapUtility"/> classes.</summary>
    /// <remarks><para>
    /// <b>BitmapPaintTest</b> shows a <see cref="WriteableBitmap"/> that is associated with a <see
    /// cref="BitmapBuffer"/>. The left side of the bitmap shows opaque and translucent color blocks
    /// which let the user select a drawing color. The right side is empty and lets the user draw
    /// single pixels (left clicks) or solid squares (right clicks) using the current color.
    /// </para><para>
    /// This dialog tests reading and writing individual pixels. The user may choose whether to use
    /// <see cref="BitmapUtility"/> methods to access the <see cref="WriteableBitmap"/> directly, or
    /// to use indirect access through a <see cref="BitmapBuffer"/> object.</para></remarks>

    public partial class BitmapPaintTest: Window {
        #region BitmapPaintTest()

        public BitmapPaintTest() {
            _ignoreControls = true;
            InitializeComponent();

            // create bitmap with associated paint buffer
            PaintBitmap = new WriteableBitmap(450, 250, 96, 96, PixelFormats.Pbgra32, null);
            PaintBuffer = new BitmapBuffer(PaintBitmap);
            PaintColor = Colors.Black;
            DrawColorBoxes();

            BitmapImage.Source = PaintBitmap;
            SelectDrawPixel();
            _ignoreControls = false;
        }

        #endregion
        #region Fields & Properties

        private const int _opaqueWidth = 50, _translucentWidth = 100;
        private bool _ignoreControls;

        private readonly Color[] _colors = new Color[] {
            Colors.Black, Colors.Blue, Colors.Magenta, Colors.Red,
            Colors.Yellow, Colors.Cyan, Colors.Lime, Colors.White};

        // method to use when drawing pixels
        internal delegate void DrawPixelDelegate(int x, int y, Color color);

        internal DrawPixelDelegate DrawPixel { get; private set; }
        internal WriteableBitmap PaintBitmap { get; private set; }
        internal BitmapBuffer PaintBuffer { get; private set; }
        internal Color PaintColor { get; private set; }
        internal bool UseBitmapBuffer { get; private set; }

        #endregion
        #region OnBitmapMouseDown

        private void OnBitmapMouseDown(object sender, MouseButtonEventArgs args) {
            args.Handled = true;

            PointI p = GetBitmapPosition(args);
            if (p.X >= _translucentWidth)
                DrawOnBitmap(args, p);
            else if (p.X >= 0) {
                // obtain new painting color from color box
                PaintColor = (UseBitmapBuffer ?
                    PaintBuffer.GetPixel(p.X, p.Y) :
                    PaintBitmap.GetPixel(p.X, p.Y));

                SelectDrawPixel();
            }
        }

        #endregion
        #region OnBitmapMouseMove

        private void OnBitmapMouseMove(object sender, MouseEventArgs args) {
            args.Handled = true;

            PointI p = GetBitmapPosition(args);
            if (p.X >= _translucentWidth)
                DrawOnBitmap(args, p);
        }

        #endregion
        #region OnMethodChanged

        private void OnMethodChanged(object sender, RoutedEventArgs args) {
            args.Handled = true;
            if (_ignoreControls) return;

            UseBitmapBuffer = (BitmapBufferToggle.IsChecked == true);
            DrawColorBoxes();
            SelectDrawPixel();
        }

        #endregion
        #region DrawColorBoxes

        private void DrawColorBoxes() {

            if (UseBitmapBuffer)
                PaintBuffer.Clear();
            else {
                PaintBitmap.Lock();
                PaintBitmap.Clear();
            }

            int step = PaintBuffer.Size.Height / _colors.Length;

            // draw color boxes on left side of bitmap
            for (int i = 0; i < _colors.Length; i++) {
                Color opaque = _colors[i];
                Color translucent = Color.FromArgb(63, opaque.R, opaque.G, opaque.B);

                // draw translucent variant to the right of opaque color
                RectI opaqueBounds = new RectI(0, i * step, _opaqueWidth, step);
                RectI translucentBounds = new RectI(
                    _opaqueWidth, i * step, _translucentWidth - _opaqueWidth, step);

                if (UseBitmapBuffer) {
                    PaintBuffer.Clear(opaqueBounds, opaque);
                    PaintBuffer.Clear(translucentBounds, translucent);
                } else {
                    PaintBitmap.Clear(opaqueBounds, opaque);
                    PaintBitmap.Clear(translucentBounds, translucent);
                }
            }

            if (UseBitmapBuffer)
                PaintBuffer.Write();
            else
                PaintBitmap.Unlock();
        }

        #endregion
        #region DrawOnBitmap

        private void DrawOnBitmap(MouseEventArgs args, PointI p) {
            RectI bounds = RectI.Empty;
            if (!UseBitmapBuffer) PaintBitmap.Lock();

            if (args.LeftButton == MouseButtonState.Pressed) {
                bounds = new RectI(p.X, p.Y, 1, 1);
                DrawPixel(p.X, p.Y, PaintColor);
            }
            else if (args.RightButton == MouseButtonState.Pressed) {
                const int r = 4;

                // define rectangle around cursor position
                int x0 = Math.Max(_translucentWidth, p.X - r), y0 = Math.Max(0, p.Y - r),
                    x1 = Math.Min(PaintBuffer.Size.Width - 1, p.X + r),
                    y1 = Math.Min(PaintBuffer.Size.Height - 1, p.Y + r);

                bounds = new RectI(x0, y0, x1 - x0 + 1, y1 - y0 + 1);
                for (int x = x0; x <= x1; x++)
                    for (int y = y0; y <= y1; y++)
                        DrawPixel(x, y, PaintColor);
            }

            if (UseBitmapBuffer)
                PaintBuffer.Write(bounds);
            else
                PaintBitmap.Unlock();
        }

        #endregion
        #region GetBitmapPosition

        private PointI GetBitmapPosition(MouseEventArgs args) {

            // get rounded position relative to bitmap
            Point cursor = args.GetPosition(BitmapImage);
            int x = Fortran.NInt(cursor.X), y = Fortran.NInt(cursor.Y);

            // check for valid position within bitmap
            if (x < 0 || y < 0 || x >= PaintBuffer.Size.Width || y >= PaintBuffer.Size.Height)
                return new PointI(-1, -1);

            return new PointI(x, y);
        }

        #endregion
        #region SelectDrawPixel

        private void SelectDrawPixel() {
            DrawPixel = (UseBitmapBuffer ?
                (PaintColor.A == 255 ?
                    new DrawPixelDelegate(PaintBuffer.SetPixel) :
                    new DrawPixelDelegate(PaintBuffer.BlendPixel)) :
                (PaintColor.A == 255 ?
                    new DrawPixelDelegate(PaintBitmap.SetPixel) :
                    new DrawPixelDelegate(PaintBitmap.BlendPixel)));
        }

        #endregion
    }
}
