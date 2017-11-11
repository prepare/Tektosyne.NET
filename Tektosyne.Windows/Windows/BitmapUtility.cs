using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Tektosyne.Geometry;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides auxiliary methods for the <see cref="WriteableBitmap"/> class.</summary>
    /// <remarks>
    /// <b>BitmapUtility</b> provides conversions between <see cref="Color"/> values and their <see
    /// cref="PixelFormats.Pbgra32"/> representations, and also several methods to directly
    /// manipulate the <see cref="WriteableBitmap.BackBuffer"/> of a <see cref="WriteableBitmap"/>.
    /// The bitmap must always use the <see cref="PixelFormats.Pbgra32"/> format.</remarks>

    public static class BitmapUtility {
        #region BlendPixel

        /// <summary>
        /// Blends the specified pixel in the specified <see cref="WriteableBitmap"/> with the
        /// specified <see cref="Color"/>.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> containing the pixel whose color to blend.</param>
        /// <param name="x">
        /// The x-coordinate of the pixel to blend with <paramref name="color"/>.</param>
        /// <param name="y">
        /// The y-coordinate of the pixel to blend with <paramref name="color"/>.</param>
        /// <param name="color">
        /// The <see cref="Color"/> to blend with the pixel at the specified location in the
        /// specified <paramref name="bitmap"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="bitmap"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>BlendPixel</b> blends the pixel at the specified <paramref name="x"/> and <paramref
        /// name="y"/> coordinates within the <see cref="WriteableBitmap.BackBuffer"/> of the
        /// specified <paramref name="bitmap"/> with the <see cref="PixelFormats.Pbgra32"/>
        /// representation of the specified <paramref name="color"/>. No coordinate checking is
        /// performed.
        /// </para><note type="caution">
        /// For improved performance, <b>BlendPixel</b> does not attempt to lock the specified
        /// <paramref name="bitmap"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/>
        /// and <see cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="bitmap"/>, respectively.</note></remarks>

        public static unsafe void BlendPixel(
            this WriteableBitmap bitmap, int x, int y, Color color) {

            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            // use byte* as stride is in bytes, not in pixels
            byte* line = (byte*) bitmap.BackBuffer + bitmap.BackBufferStride * y;
            uint* addr = (uint*) line + x;
            *addr = BlendPbgra32(ColorToPbgra32(color), *addr);
            bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
        }

        #endregion
        #region BlendPbgra32

        /// <summary>
        /// Performs alpha blending of two specified <see cref="PixelFormats.Pbgra32"/> color
        /// values.</summary>
        /// <param name="source">
        /// The <see cref="PixelFormats.Pbgra32"/> color value to overlay on the <paramref
        /// name="target"/> color.</param>
        /// <param name="target">
        /// The <see cref="PixelFormats.Pbgra32"/> color value on which the <paramref
        /// name="source"/> color is overlaid.</param>
        /// <returns><para>
        /// The specified <paramref name="source"/> color value if its alpha channel is 255.
        /// </para><para>-or-</para><para>
        /// The specified <paramref name="target"/> color value if the alpha channel of <paramref
        /// name="source"/> is zero.
        /// </para><para>-or-</para><para>
        /// A new <see cref="PixelFormats.Pbgra32"/> color value that is the result of overlaying
        /// <paramref name="source"/> on <paramref name="target"/> with alpha blending.
        /// </para></returns>

        [CLSCompliant(false)]
        public static uint BlendPbgra32(uint source, uint target) {

            // handle transparent or opaque color
            uint a0 = (source >> 24);
            if (a0 == 0) return target;
            if (a0 == 255) return source;

            // get source color channels
            uint r0 = (source & 0x00FF0000) >> 16;
            uint g0 = (source & 0x0000FF00) >> 8;
            uint b0 = (source & 0x000000FF);

            // get target alpha and color channels
            uint a1 = (target >> 24);
            uint r1 = (target & 0x00FF0000) >> 16;
            uint g1 = (target & 0x0000FF00) >> 8;
            uint b1 = (target & 0x000000FF);

            // compute blended alpha and color channels
            uint a = (255 * a0 + (255 - a0) * a1 + 127) / 255;
            uint r = (255 * r0 + (255 - a0) * r1 + 127) / 255;
            uint g = (255 * g0 + (255 - a0) * g1 + 127) / 255;
            uint b = (255 * b0 + (255 - a0) * b1 + 127) / 255;

            uint value = b | (g << 8) | (r << 16) | (a << 24);
            return value;
        }

        #endregion
        #region Clear(...)

        /// <overloads>
        /// Clears the specified <see cref="WriteableBitmap"/>.</overloads>
        /// <summary>
        /// Clears the specified <see cref="WriteableBitmap"/>.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> whose pixels to clear.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="bitmap"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>Clear</b> sets all pixels within the specified <paramref name="bitmap"/> to zero,
        /// which is the <see cref="PixelFormats.Pbgra32"/> equivalent of transparent black color.
        /// </para><note type="caution">
        /// For improved performance, <b>Clear</b> does not attempt to lock the specified <paramref
        /// name="bitmap"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/> and <see
        /// cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="bitmap"/>, respectively.</note></remarks>

        public static unsafe void Clear(this WriteableBitmap bitmap) {
            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            Int32Rect bounds = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            int count = bounds.Width * bounds.Height;

            uint* addr = (uint*) bitmap.BackBuffer;
            for (int i = 0; i < count; i++)
                *addr++ = 0u;

            bitmap.AddDirtyRect(bounds);
        }

        #endregion
        #region Clear(..., Color)

        /// <summary>
        /// Clears the specified <see cref="WriteableBitmap"/> with the specified <see
        /// cref="Color"/>.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> whose pixels to clear.</param>
        /// <param name="color">
        /// The new <see cref="Color"/> for all pixels in the specified <paramref name="bitmap"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="bitmap"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>Clear</b> sets all pixels within the specified <paramref name="bitmap"/> to the <see
        /// cref="PixelFormats.Pbgra32"/> representation of the specified <paramref name="color"/>.
        /// </para><note type="caution">
        /// For improved performance, <b>Clear</b> does not attempt to lock the specified <paramref
        /// name="bitmap"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/> and <see
        /// cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="bitmap"/>, respectively.</note></remarks>

        public static unsafe void Clear(this WriteableBitmap bitmap, Color color) {
            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            uint value = BitmapUtility.ColorToPbgra32(color);
            Int32Rect bounds = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            int count = bounds.Width * bounds.Height;

            uint* addr = (uint*) bitmap.BackBuffer;
            for (int i = 0; i < count; i++)
                *addr++ = value;

            bitmap.AddDirtyRect(bounds);
        }

        #endregion
        #region Clear(..., RectI, Color)

        /// <summary>
        /// Clears the specified rectangle within the specified <see cref="WriteableBitmap"/> with
        /// the specified <see cref="Color"/>.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> whose pixels to clear.</param>
        /// <param name="bounds">
        /// The pixel rectangle within the specified <paramref name="bitmap"/> to fill with the
        /// specified <paramref name="color"/>.</param>
        /// <param name="color">
        /// The new <see cref="Color"/> for all pixels within the specified <paramref
        /// name="bounds"/> of the specified <paramref name="bitmap"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="bitmap"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>Clear</b> sets all pixels within the specified <paramref name="bounds"/> of the
        /// specified <paramref name="bitmap"/> to the <see cref="PixelFormats.Pbgra32"/>
        /// representation of the specified <paramref name="color"/>. No coordinate checking is
        /// performed.
        /// </para><note type="caution">
        /// For improved performance, <b>Clear</b> does not attempt to lock the specified <paramref
        /// name="bitmap"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/> and <see
        /// cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="bitmap"/>, respectively.</note></remarks>

        public static unsafe void Clear(this WriteableBitmap bitmap, RectI bounds, Color color) {
            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            uint value = BitmapUtility.ColorToPbgra32(color);
            int stride = bitmap.BackBufferStride;

            // use byte* as stride is in bytes, not in pixels
            byte* line = (byte*) bitmap.BackBuffer + stride * bounds.Y;
            for (int y = 0; y < bounds.Height; y++) {

                uint* addr = (uint*) line + bounds.X;
                for (int x = 0; x < bounds.Width; x++)
                    *addr++ = value;

                line += stride;
            }

            bitmap.AddDirtyRect(bounds.ToInt32Rect());
        }

        #endregion
        #region ColorFromPbgra32

        /// <summary>
        /// Converts the specified <see cref="UInt32"/> value in <see cref="PixelFormats.Pbgra32"/>
        /// format to the corresponding <see cref="Color"/>.</summary>
        /// <param name="value">
        /// The <see cref="UInt32"/> value to convert.</param>
        /// <returns>
        /// The <see cref="Color"/> that is equivalent to the specified <paramref name="value"/>
        /// when interpreted in the <see cref="PixelFormats.Pbgra32"/> format.</returns>
        /// <remarks><para>
        /// <b>ColorFromPbgra32</b> assumes that the specified <paramref name="value"/> contains
        /// premultiplied color channels according to the <see cref="PixelFormats.Pbgra32"/> format.
        /// <b>ColorFromPbgra32</b> undos the premultiplication of the red, green, and blue channels
        /// with the alpha channel, and then creates a <see cref="Color"/> value from the resulting
        /// <see cref="Byte"/> values.
        /// </para><note type="caution">
        /// The premultiplication of <see cref="Byte"/> values greatly reduces color accuracy at
        /// high transparency levels since many color channels will be mapped to zero.
        /// <b>ColorFromPbgra32</b> cannot recreate the original color in such cases.
        /// </note></remarks>

        [CLSCompliant(false)]
        public static Color ColorFromPbgra32(uint value) {

            // extract alpha channel
            byte a = (byte) (value >> 24);
            if (a == 0) return new Color();

            // undo premultiplication of color channels
            byte r = (byte) ((255 * (value & 0x00FF0000) >> 16) / a);
            byte g = (byte) ((255 * (value & 0x0000FF00) >> 8) / a);
            byte b = (byte) ((255 * (value & 0x000000FF)) / a);

            Color color = Color.FromArgb(a, r, g, b);
            return color;
        }

        #endregion
        #region ColorToOpaquePbgra32

        /// <summary>
        /// Converts the specified <see cref="Color"/> to its <see cref="UInt32"/> representation in
        /// <see cref="PixelFormats.Pbgra32"/> format, ignoring its alpha channel.</summary>
        /// <param name="color">
        /// The <see cref="Color"/> value to convert.</param>
        /// <returns>
        /// An <see cref="UInt32"/> value containing the specified <paramref name="color"/>
        /// converted to <see cref="PixelFormats.Pbgra32"/> format with full opacity.</returns>
        /// <remarks><para>
        /// <b>ColorToOpaquePbgra32</b> creates an <see cref="UInt32"/> value from the red, green,
        /// and blue channels of the specified <paramref name="color"/>, ordered according to the
        /// <see cref="PixelFormats.Pbgra32"/> format.
        /// </para><para>
        /// <b>ColorToOpaquePbgra32</b> ignores the alpha channel of the specified <paramref
        /// name="color"/>. The corresponding <see cref="Byte"/> in the returned <see
        /// cref="UInt32"/> value is always set to 255.</para></remarks>

        [CLSCompliant(false)]
        public static uint ColorToOpaquePbgra32(Color color) {

            // extract color channels without alpha channel
            uint r = (uint) color.R;
            uint g = (uint) color.G;
            uint b = (uint) color.B;

            // compose color channels with full opacity
            uint value = (b | (g << 8) | (r << 16) | 0xFF000000);
            return value;
        }

        #endregion
        #region ColorToPbgra32

        /// <summary>
        /// Converts the specified <see cref="Color"/> to its <see cref="UInt32"/> representation in
        /// <see cref="PixelFormats.Pbgra32"/> format.</summary>
        /// <param name="color">
        /// The <see cref="Color"/> value to convert.</param>
        /// <returns>
        /// An <see cref="UInt32"/> value containing the specified <paramref name="color"/>
        /// converted to <see cref="PixelFormats.Pbgra32"/> format.</returns>
        /// <remarks>
        /// <b>ColorToPbgra32</b> premultiplies the red, green, and blue channels of the specified
        /// <paramref name="color"/> with the alpha channel, and then creates an <see
        /// cref="UInt32"/> value from the resulting <see cref="Byte"/> values ordered according to
        /// the <see cref="PixelFormats.Pbgra32"/> format.</remarks>

        [CLSCompliant(false)]
        public static uint ColorToPbgra32(Color color) {

            // premultiply color channels by alpha channel
            uint r = (uint) ((color.R * color.A) / 255);
            uint g = (uint) ((color.G * color.A) / 255);
            uint b = (uint) ((color.B * color.A) / 255);

            // compose all four channels in BGRA32 order
            uint value = (b | (g << 8) | (r << 16) | ((uint) color.A << 24));
            return value;
        }

        #endregion
        #region GetPixel

        /// <summary>
        /// Gets the <see cref="Color"/> of the specified pixel in the specified <see
        /// cref="WriteableBitmap"/>.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> containing the pixel whose color to get.</param>
        /// <param name="x">
        /// The x-coordinate of the pixel whose <see cref="Color"/> to return.</param>
        /// <param name="y">
        /// The y-coordinate of the pixel whose <see cref="Color"/> to return.</param>
        /// <returns>
        /// The <see cref="Color"/> of the pixel at the specified location in the specified
        /// <paramref name="bitmap"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <remarks>
        /// <b>GetPixel</b> returns the <see cref="Color"/> equivalent of the pixel at the specified
        /// <paramref name="x"/> and <paramref name="y"/> coordinates within the <see
        /// cref="WriteableBitmap.BackBuffer"/> of the specified <paramref name="bitmap"/>. No
        /// coordinate checking is performed.</remarks>

        public static unsafe Color GetPixel(this WriteableBitmap bitmap, int x, int y) {
            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            // use byte* as stride is in bytes, not in pixels
            byte* line = (byte*) bitmap.BackBuffer + bitmap.BackBufferStride * y;
            return ColorFromPbgra32(*((uint*) line + x));
        }

        #endregion
        #region Grow(RenderTargetBitmap, ...)

        /// <overloads>
        /// Grows the specified bitmap to the specified size.</overloads>
        /// <summary>
        /// Grows the specified <see cref="RenderTargetBitmap"/> to the specified size.</summary>
        /// <param name="bitmap">
        /// The <see cref="RenderTargetBitmap"/> to grow. This argument may be a null reference.
        /// </param>
        /// <param name="width">
        /// The new minimum <see cref="BitmapSource.PixelWidth"/> for the specified <paramref
        /// name="bitmap"/>.</param>
        /// <param name="height">
        /// The new minimum <see cref="BitmapSource.PixelWidth"/> for the specified <paramref
        /// name="bitmap"/>.</param>
        /// <param name="dpiX">
        /// The new <see cref="BitmapSource.DpiX"/> value for the specified <paramref
        /// name="bitmap"/> if it is reallocated.</param>
        /// <param name="dpiY">
        /// The new <see cref="BitmapSource.DpiY"/> value for the specified <paramref
        /// name="bitmap"/> if it is reallocated.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is zero or negative.</exception>
        /// <remarks><para>
        /// <b>Grow</b> sets the specified <paramref name="bitmap"/> argument to a new <see
        /// cref="RenderTargetBitmap"/> with the specified <paramref name="width"/> and <paramref
        /// name="height"/> if <paramref name="bitmap"/> is a null reference or its <see
        /// cref="BitmapSource.PixelWidth"/> or <see cref="BitmapSource.PixelHeight"/> properties
        /// are smaller than the specified <paramref name="width"/> or <paramref name="height"/>.
        /// Otherwise, the specified <paramref name="bitmap"/> remains unchanged.
        /// </para><para>
        /// The resolution of any newly allocated <see cref="RenderTargetBitmap"/> is set to the
        /// specified <paramref name="dpiX"/> and <paramref name="dpiY"/> values, and its <see
        /// cref="BitmapSource.Format"/> is always <see cref="PixelFormats.Pbgra32"/>. Any existing
        /// <paramref name="bitmap"/> contents are lost.</para></remarks>

        public static void Grow(ref RenderTargetBitmap bitmap,
            int width, int height, double dpiX, double dpiY) {

            if (width <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "width", width, Strings.ArgumentNotPositive);

            if (height <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "height", height, Strings.ArgumentNotPositive);

            if (bitmap == null || bitmap.PixelWidth < width || bitmap.PixelHeight < height)
                bitmap = new RenderTargetBitmap(width, height, dpiX, dpiY, PixelFormats.Pbgra32);
        }

        #endregion
        #region Grow(WriteableBitmap, ...)

        /// <summary>
        /// Grows the specified <see cref="WriteableBitmap"/> to the specified size.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> to grow. This argument may be a null reference.
        /// </param>
        /// <param name="width">
        /// The new minimum <see cref="BitmapSource.PixelWidth"/> for the specified <paramref
        /// name="bitmap"/>.</param>
        /// <param name="height">
        /// The new minimum <see cref="BitmapSource.PixelWidth"/> for the specified <paramref
        /// name="bitmap"/>.</param>
        /// <param name="dpiX">
        /// The new <see cref="BitmapSource.DpiX"/> value for the specified <paramref
        /// name="bitmap"/> if it is reallocated.</param>
        /// <param name="dpiY">
        /// The new <see cref="BitmapSource.DpiY"/> value for the specified <paramref
        /// name="bitmap"/> if it is reallocated.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is zero or negative.</exception>
        /// <remarks><para>
        /// <b>Grow</b> sets the specified <paramref name="bitmap"/> argument to a new <see
        /// cref="WriteableBitmap"/> with the specified <paramref name="width"/> and <paramref
        /// name="height"/> if <paramref name="bitmap"/> is a null reference or its <see
        /// cref="BitmapSource.PixelWidth"/> or <see cref="BitmapSource.PixelHeight"/> properties
        /// are smaller than the specified <paramref name="width"/> or <paramref name="height"/>.
        /// Otherwise, the specified <paramref name="bitmap"/> remains unchanged.
        /// </para><para>
        /// The resolution of any newly allocated <see cref="WriteableBitmap"/> is set to the
        /// specified <paramref name="dpiX"/> and <paramref name="dpiY"/> values. Its <see
        /// cref="BitmapSource.Format"/> is always <see cref="PixelFormats.Pbgra32"/>, and its <see
        /// cref="BitmapSource.Palette"/> is always a null reference. Any existing <paramref
        /// name="bitmap"/> contents are lost.</para></remarks>

        public static void Grow(ref WriteableBitmap bitmap,
            int width, int height, double dpiX, double dpiY) {

            if (width <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "width", width, Strings.ArgumentNotPositive);

            if (height <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "height", height, Strings.ArgumentNotPositive);

            if (bitmap == null || bitmap.PixelWidth < width || bitmap.PixelHeight < height)
                bitmap = new WriteableBitmap(width, height, dpiX, dpiY, PixelFormats.Pbgra32, null);
        }

        #endregion
        #region MakeOpaque(...)

        /// <overloads>
        /// Makes the specified <see cref="WriteableBitmap"/> fully opaque.</overloads>
        /// <summary>
        /// Makes the specified <see cref="WriteableBitmap"/> fully opaque.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> whose pixels to make opaque.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="bitmap"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>MakeOpaque</b> replaces all pixels within the specified <paramref name="bitmap"/>
        /// with their <see cref="OpaquePbgra32"/> equivalents. Fully transparent pixels remain
        /// unchanged.
        /// </para><note type="caution">
        /// For improved performance, <b>MakeOpaque</b> does not attempt to lock the specified
        /// <paramref name="bitmap"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/>
        /// and <see cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="bitmap"/>, respectively.</note></remarks>

        public static unsafe void MakeOpaque(this WriteableBitmap bitmap) {
            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            Int32Rect bounds = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            int count = bounds.Width * bounds.Height;

            uint* addr = (uint*) bitmap.BackBuffer;
            for (int i = 0; i < count; i++) {
                uint value = *addr;
                *addr++ = OpaquePbgra32(value);
            }

            bitmap.AddDirtyRect(bounds);
        }

        #endregion
        #region MakeOpaque(..., RectI)

        /// <summary>
        /// Makes the specified rectangle within the specified <see cref="WriteableBitmap"/> fully
        /// opaque.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> whose pixels to make opaque.</param>
        /// <param name="bounds">
        /// The pixel rectangle within the specified <paramref name="bitmap"/> to make opaque.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="bitmap"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>MakeOpaque</b> replaces all pixels within the specified <paramref name="bounds"/> in
        /// the specified <paramref name="bitmap"/> with their <see cref="OpaquePbgra32"/>
        /// equivalents. Fully transparent pixels remain unchanged. No coordinate checking is
        /// performed.
        /// </para><note type="caution">
        /// For improved performance, <b>MakeOpaque</b> does not attempt to lock the specified
        /// <paramref name="bitmap"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/>
        /// and <see cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="bitmap"/>, respectively.</note></remarks>

        public static unsafe void MakeOpaque(this WriteableBitmap bitmap, RectI bounds) {
            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            int stride = bitmap.BackBufferStride;

            // use byte* as stride is in bytes, not in pixels
            byte* line = (byte*) bitmap.BackBuffer + stride * bounds.Y;
            for (int y = 0; y < bounds.Height; y++) {

                uint* addr = (uint*) line + bounds.X;
                for (int x = 0; x < bounds.Width; x++) {
                    uint value = *addr;
                    *addr++ = OpaquePbgra32(value);
                }

                line += stride;
            }

            bitmap.AddDirtyRect(bounds.ToInt32Rect());
        }

        #endregion
        #region MakeTransparent

        /// <summary>
        /// Makes the specified <see cref="Color"/> transparent in the specified <see
        /// cref="WriteableBitmap"/>.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> whose pixels to make transparent.</param>
        /// <param name="color">
        /// The <see cref="Color"/> to replace with transparent black.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="bitmap"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>MakeTransparent</b> sets all fully opaque pixels of the specified <paramref
        /// name="color"/> within the specified <paramref name="bitmap"/> to zero, which is the <see
        /// cref="PixelFormats.Pbgra32"/> equivalent of transparent black color. Partially
        /// transparent pixels and pixels of any other color remain unchanged.
        /// </para><para>
        /// <b>MakeTransparent</b> ignores the alpha channel of the specified <paramref
        /// name="color"/> and only replaces fully opaque pixels because the imprecision caused by
        /// the premultiplication of color channels in the <see cref="PixelFormats.Pbgra32"/> format
        /// would otherwise produce false matches.
        /// </para><note type="implementnotes">
        /// The predefined <see cref="Colors.Transparent"/> color is transparent <em>white</em> (all
        /// color channels maximized), not transparent <em>black</em> (all color channels zero).
        /// That color is not representable in <see cref="PixelFormats.Pbgra32"/> format since all
        /// color channel information of fully transparent color is lost.
        /// </note><note type="caution">
        /// For improved performance, <b>MakeTransparent</b> does not attempt to lock the specified
        /// <paramref name="bitmap"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/>
        /// and <see cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="bitmap"/>, respectively.</note></remarks>

        public static unsafe void MakeTransparent(this WriteableBitmap bitmap, Color color) {
            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            Int32Rect bounds = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            int count = bounds.Width * bounds.Height;
            uint value = ColorToOpaquePbgra32(color);

            uint* addr = (uint*) bitmap.BackBuffer;
            for (int i = 0; i < count; i++) {
                if (*addr == value) *addr = 0u;
                ++addr;
            }

            bitmap.AddDirtyRect(bounds);
        }

        #endregion
        #region OpaquePbgra32

        /// <summary>
        /// Makes the specified <see cref="PixelFormats.Pbgra32"/> color value fully opaque.
        /// </summary>
        /// <param name="value">
        /// The <see cref="PixelFormats.Pbgra32"/> color value to make opaque.</param>
        /// <returns><para>
        /// The specified <paramref name="value"/> if its alpha channel is zero or 255.
        /// </para><para>-or-</para><para>
        /// A new <see cref="PixelFormats.Pbgra32"/> color value with an alpha channel of 255 and
        /// the unpremultiplied color channels of the specified <paramref name="value"/>.
        /// </para></returns>
        /// <remarks><para>
        /// <b>OpaquePbgra32</b> returns a fully transparent <paramref name="value"/> unchanged 
        /// since there is no way to restore the original color channels in this case. Obviously, a
        /// fully opaque <paramref name="value"/> is also returned unchanged.
        /// </para><para>
        /// Otherwise, <b>OpaquePbgra32</b> reverts the color channel premultiplication that the
        /// <see cref="PixelFormats.Pbgra32"/> format requires for non-opaque colors, and sets the
        /// alpha channel of the returned color value to 255.</para></remarks>

        [CLSCompliant(false)]
        public static uint OpaquePbgra32(uint value) {

            // reflect transparent or opaque color
            uint a = (value >> 24);
            if (a == 0 || a == 255)
                return value;

            // undo premultiplication of color channels
            uint r = (255 * (value & 0x00FF0000) >> 16) / a;
            uint g = (255 * (value & 0x0000FF00) >> 8) / a;
            uint b = (255 * (value & 0x000000FF)) / a;

            // create opaque color with original channels
            return (b | (g << 8) | (r << 16) | 0xFF000000);
        }

        #endregion
        #region Overlay(...)

        /// <overloads>
        /// Overlays pixel data from one specified <see cref="WriteableBitmap"/> onto another.
        /// </overloads>
        /// <summary>
        /// Overlays pixel data from one specified <see cref="WriteableBitmap"/> onto another, with
        /// alpha blending.</summary>
        /// <param name="target">
        /// The <see cref="WriteableBitmap"/> that is the target of the overlay.</param>
        /// <param name="x">
        /// The x-coordinate where to begin writing in the specified <paramref name="target"/>.
        /// </param>
        /// <param name="y">
        /// The y-coordinate where to begin writing in the specified <paramref name="target"/>.
        /// </param>
        /// <param name="source">
        /// The <see cref="WriteableBitmap"/> containing the rectangle to overlay.</param>
        /// <param name="bounds">
        /// The pixel rectangle within the specified <paramref name="source"/> to overlay.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="target"/> or <paramref name="source"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="target"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>Overlay</b> blends all pixels within the specified <paramref name="bounds"/> of the
        /// specified <paramref name="source"/> bitmap with the corresponding pixels within the
        /// specified <paramref name="target"/> bitmap, starting with the specified <paramref
        /// name="x"/> and <paramref name="y"/> coordinates.
        /// </para><para>
        /// <b>Overlay</b> calls <see cref="BlendPbgra32"/> to perform alpha blending between the
        /// <paramref name="source"/> (acting as the overlay) and the <paramref name="target"/>
        /// (acting as the blending target). No coordinate checking is performed.
        /// </para><note type="caution">
        /// For improved performance, <b>Overlay</b> does not attempt to lock the specified
        /// <paramref name="target"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/>
        /// and <see cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="target"/> bitmap, respectively.</note></remarks>

        public static unsafe void Overlay(this WriteableBitmap target,
            int x, int y, WriteableBitmap source, RectI bounds) {

            if (target == null)
                ThrowHelper.ThrowArgumentNullException("target");
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");

            int sourceStride = source.BackBufferStride;
            int targetStride = target.BackBufferStride;

            // use byte* as stride is in bytes, not in pixels
            byte* sourceLine = (byte*) source.BackBuffer + sourceStride * bounds.Y;
            byte* targetLine = (byte*) target.BackBuffer + targetStride * y;

            for (int dy = 0; dy < bounds.Height; dy++) {
                uint* sourceAddr = (uint*) sourceLine + bounds.X;
                uint* targetAddr = (uint*) targetLine + x;

                for (int dx = 0; dx < bounds.Width; dx++) {
                    *targetAddr = BlendPbgra32(*sourceAddr, *targetAddr);
                    ++sourceAddr; ++targetAddr;
                }

                sourceLine += sourceStride;
                targetLine += targetStride;
            }

            target.AddDirtyRect(new Int32Rect(x, y, bounds.Width, bounds.Height));
        }

        #endregion
        #region Overlay(..., Byte)

        /// <summary>
        /// Overlays pixel data from one specified <see cref="WriteableBitmap"/> onto another, with
        /// the specified alpha channel threshold.</summary>
        /// <param name="target">
        /// The <see cref="WriteableBitmap"/> that is the target of the overlay.</param>
        /// <param name="x">
        /// The x-coordinate where to begin writing in the specified <paramref name="target"/>.
        /// </param>
        /// <param name="y">
        /// The y-coordinate where to begin writing in the specified <paramref name="target"/>.
        /// </param>
        /// <param name="source">
        /// The <see cref="WriteableBitmap"/> containing the rectangle to overlay.</param>
        /// <param name="bounds">
        /// The pixel rectangle within the specified <paramref name="source"/> to overlay.</param>
        /// <param name="alpha">
        /// The alpha channel threshold below which <paramref name="source"/> pixels will be
        /// ignored.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="target"/> or <paramref name="source"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="target"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>Overlay</b> copies only those pixels within the specified <paramref name="bounds"/>
        /// of the specified <paramref name="source"/> bitmap whose alpha channel meets or exceeds
        /// the specified <paramref name="alpha"/> threshold.
        /// </para><para>
        /// The copies are written to the pixels of the specified <paramref name="target"/> bitmap,
        /// starting with the specified <paramref name="x"/> and <paramref name="y"/> coordinates.
        /// No alpha blending or coordinate checking is performed.
        /// </para><note type="caution">
        /// For improved performance, <b>Overlay</b> does not attempt to lock the specified
        /// <paramref name="target"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/>
        /// and <see cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="target"/> bitmap, respectively.</note></remarks>

        public static unsafe void Overlay(this WriteableBitmap target,
            int x, int y, WriteableBitmap source, RectI bounds, byte alpha) {

            if (target == null)
                ThrowHelper.ThrowArgumentNullException("target");
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");

            int sourceStride = source.BackBufferStride;
            int targetStride = target.BackBufferStride;

            // use byte* as stride is in bytes, not in pixels
            byte* sourceLine = (byte*) source.BackBuffer + sourceStride * bounds.Y;
            byte* targetLine = (byte*) target.BackBuffer + targetStride * y;

            for (int dy = 0; dy < bounds.Height; dy++) {
                uint* sourceAddr = (uint*) sourceLine + bounds.X;
                uint* targetAddr = (uint*) targetLine + x;

                for (int dx = 0; dx < bounds.Width; dx++) {
                    uint value = *sourceAddr;
                    if ((value >> 24) >= alpha) *targetAddr = value;
                    ++sourceAddr; ++targetAddr;
                }

                sourceLine += sourceStride;
                targetLine += targetStride;
            }

            target.AddDirtyRect(new Int32Rect(x, y, bounds.Width, bounds.Height));
        }

        #endregion
        #region Overlay(..., Color)

        /// <summary>
        /// Overlays pixel data from one specified <see cref="WriteableBitmap"/> onto another, with
        /// alpha blending and color substitution.</summary>
        /// <param name="target">
        /// The <see cref="WriteableBitmap"/> that is the target of the overlay.</param>
        /// <param name="x">
        /// The x-coordinate where to begin writing in the specified <paramref name="target"/>.
        /// </param>
        /// <param name="y">
        /// The y-coordinate where to begin writing in the specified <paramref name="target"/>.
        /// </param>
        /// <param name="source">
        /// The <see cref="WriteableBitmap"/> containing the rectangle to overlay.</param>
        /// <param name="bounds">
        /// The pixel rectangle within the specified <paramref name="source"/> to overlay.</param>
        /// <param name="color">
        /// The <see cref="Color"/> to substitute for all <paramref name="source"/> pixels.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="target"/> or <paramref name="source"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="target"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>Overlay</b> blends all pixels within the specified <paramref name="bounds"/> of the
        /// specified <paramref name="source"/> bitmap with the corresponding pixels within the
        /// specified <paramref name="target"/> bitmap, starting with the specified <paramref
        /// name="x"/> and <paramref name="y"/> coordinates.
        /// </para><para>
        /// <b>Overlay</b> calls <see cref="BlendPbgra32"/> to perform alpha blending between the
        /// <paramref name="source"/> (acting as the overlay) and the <paramref name="target"/>
        /// (acting as the blending target). No coordinate checking is performed.
        /// </para><para>
        /// <b>Overlay</b> substitutes the specified <paramref name="color"/> for the actual color
        /// channels of each <paramref name="source"/> pixel, while using its alpha channel to
        /// govern alpha blending with the corresponding <paramref name="target"/> pixel. The alpha
        /// channel of the specified <paramref name="color"/> is ignored.
        /// </para><note type="caution">
        /// For improved performance, <b>Overlay</b> does not attempt to lock the specified
        /// <paramref name="target"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/>
        /// and <see cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="target"/> bitmap, respectively.</note></remarks>

        public static unsafe void Overlay(this WriteableBitmap target,
            int x, int y, WriteableBitmap source, RectI bounds, Color color) {

            if (target == null)
                ThrowHelper.ThrowArgumentNullException("target");
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");

            int sourceStride = source.BackBufferStride;
            int targetStride = target.BackBufferStride;

            // use byte* as stride is in bytes, not in pixels
            byte* sourceLine = (byte*) source.BackBuffer + sourceStride * bounds.Y;
            byte* targetLine = (byte*) target.BackBuffer + targetStride * y;

            for (int dy = 0; dy < bounds.Height; dy++) {
                uint* sourceAddr = (uint*) sourceLine + bounds.X;
                uint* targetAddr = (uint*) targetLine + x;

                for (int dx = 0; dx < bounds.Width; dx++) {

                    // multiply color channels with source alpha
                    color.A = (byte) (*sourceAddr >> 24);
                    uint value = ColorToPbgra32(color);

                    *targetAddr = BlendPbgra32(value, *targetAddr);
                    ++sourceAddr; ++targetAddr;
                }

                sourceLine += sourceStride;
                targetLine += targetStride;
            }

            target.AddDirtyRect(new Int32Rect(x, y, bounds.Width, bounds.Height));
        }

        #endregion
        #region Read(...)

        /// <overloads>
        /// Reads pixel data from the specified <see cref="BitmapSource"/> into the specified <see
        /// cref="WriteableBitmap"/>.</overloads>
        /// <summary>
        /// Reads all pixel data from the specified <see cref="BitmapSource"/> into the specified
        /// <see cref="WriteableBitmap"/>.</summary>
        /// <param name="target">
        /// The <see cref="WriteableBitmap"/> whose pixels to overwrite.</param>
        /// <param name="x">
        /// The x-coordinate where to begin writing in the specified <paramref name="target"/>.
        /// </param>
        /// <param name="y">
        /// The y-coordinate where to begin writing in the specified <paramref name="target"/>.
        /// </param>
        /// <param name="source">
        /// The <see cref="BitmapSource"/> containing the pixels to read, in <see
        /// cref="PixelFormats.Pbgra32"/> format.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="target"/> or <paramref name="source"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="target"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>Read</b> copies all pixels within the specified <paramref name="source"/> bitmap to
        /// the specified <paramref name="target"/> bitmap, starting with the specified <paramref
        /// name="x"/> and <paramref name="y"/> coordinates. 
        /// </para><para>
        /// The width and height of the copied pixel rectangle is restricted either to the total
        /// area of the <paramref name="source"/> bitmap, or to the area between the specified
        /// <paramref name="x"/> and <paramref name="y"/> coordinates and the lower-right corner of
        /// the <paramref name="target"/> bitmap, whichever is smaller.
        /// </para><note type="caution">
        /// For improved performance, <b>Read</b> does not attempt to lock the specified <paramref
        /// name="target"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/> and <see
        /// cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="target"/> bitmap, respectively.</note></remarks>

        public static unsafe void Read(this WriteableBitmap target,
            int x, int y, BitmapSource source) {

            if (target == null)
                ThrowHelper.ThrowArgumentNullException("target");
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");

            // maximize rectangular area to copy
            int width = target.PixelWidth, height = target.PixelHeight;
            Int32Rect bounds = new Int32Rect(0, 0,
                Math.Min(source.PixelWidth, width - x),
                Math.Min(source.PixelHeight, height - y));

            // offset BackBuffer address by specified coordinates
            int stride = target.BackBufferStride;
            int offset = x * 4 + y * stride;
            IntPtr buffer = (IntPtr) ((byte*) target.BackBuffer + offset);
            int size = width * height * 4 - offset;

            source.CopyPixels(bounds, buffer, size, stride);
            target.AddDirtyRect(new Int32Rect(x, y, bounds.Width, bounds.Height));
        }

        #endregion
        #region Read(..., RectI)

        /// <summary>
        /// Reads all pixel data within the specified rectangle from the specified <see
        /// cref="BitmapSource"/> into the specified <see cref="WriteableBitmap"/>.</summary>
        /// <param name="target">
        /// The <see cref="WriteableBitmap"/> whose pixels to overwrite.</param>
        /// <param name="x">
        /// The x-coordinate where to begin writing in the specified <paramref name="target"/>.
        /// </param>
        /// <param name="y">
        /// The y-coordinate where to begin writing in the specified <paramref name="target"/>.
        /// </param>
        /// <param name="source">
        /// The <see cref="BitmapSource"/> containing the pixels to read, in <see
        /// cref="PixelFormats.Pbgra32"/> format.</param>
        /// <param name="bounds">
        /// The pixel rectangle within the specified <paramref name="source"/> to read.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="target"/> or <paramref name="source"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="target"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>Read</b> copies all pixels within the specified <paramref name="bounds"/> of the
        /// specified <paramref name="source"/> bitmap to the specified <paramref name="target"/>
        /// bitmap, starting with the specified <paramref name="x"/> and <paramref name="y"/>
        /// coordinates. No coordinate checking is performed.
        /// </para><note type="caution">
        /// For improved performance, <b>Read</b> does not attempt to lock the specified <paramref
        /// name="target"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/> and <see
        /// cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="target"/> bitmap, respectively.</note></remarks>

        public static unsafe void Read(this WriteableBitmap target,
            int x, int y, BitmapSource source, RectI bounds) {

            if (target == null)
                ThrowHelper.ThrowArgumentNullException("target");
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");

            // offset BackBuffer address by specified coordinates
            int stride = target.BackBufferStride;
            int offset = x * 4 + y * stride;
            IntPtr buffer = (IntPtr) ((byte*) target.BackBuffer + offset);
            int size = target.PixelWidth * target.PixelHeight * 4 - offset;

            source.CopyPixels(bounds.ToInt32Rect(), buffer, size, stride);
            target.AddDirtyRect(new Int32Rect(x, y, bounds.Width, bounds.Height));
        }

        #endregion
        #region Read(..., RectI, Boolean, Boolean)

        /// <summary>
        /// Reads all pixel data within the specified rectangle from the specified <see
        /// cref="WriteableBitmap"/> into the specified <see cref="WriteableBitmap"/>, with optional
        /// axis mirroring.</summary>
        /// <param name="target">
        /// The <see cref="WriteableBitmap"/> whose pixels to overwrite.</param>
        /// <param name="x">
        /// The x-coordinate where to begin writing in the specified <paramref name="target"/>.
        /// </param>
        /// <param name="y">
        /// The y-coordinate where to begin writing in the specified <paramref name="target"/>.
        /// </param>
        /// <param name="source">
        /// The <see cref="WriteableBitmap"/> containing the pixels to read, in <see
        /// cref="PixelFormats.Pbgra32"/> format.</param>
        /// <param name="bounds">
        /// The pixel rectangle within the specified <paramref name="source"/> to read.</param>
        /// <param name="mirrorX">
        /// <c>true</c> to mirror the pixels within the specified <paramref name="bounds"/> along
        /// the horizontal axis; <c>false</c> to leave the horizontal axis unchanged.</param>
        /// <param name="mirrorY">
        /// <c>true</c> to mirror the pixels within the specified <paramref name="bounds"/> along
        /// the vertical axis; <c>false</c> to leave the vertical axis unchanged.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="target"/> or <paramref name="source"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="target"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>Read</b> copies all pixels within the specified <paramref name="bounds"/> of the
        /// specified <paramref name="source"/> bitmap to the specified <paramref name="target"/>
        /// bitmap, starting with the specified <paramref name="x"/> and <paramref name="y"/>
        /// coordinates. No coordinate checking is performed.
        /// </para><note type="caution">
        /// For improved performance, <b>Read</b> does not attempt to lock the specified <paramref
        /// name="target"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/> and <see
        /// cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="target"/> bitmap, respectively.</note></remarks>

        public static unsafe void Read(this WriteableBitmap target, int x, int y,
            WriteableBitmap source, RectI bounds, bool mirrorX, bool mirrorY) {

            if (target == null)
                ThrowHelper.ThrowArgumentNullException("target");
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");

            int sourceStride = source.BackBufferStride;
            int targetStride = target.BackBufferStride;

            // use byte* as stride is in bytes, not in pixels
            byte* sourceLine = (byte*) source.BackBuffer +
                sourceStride * (mirrorY ? bounds.Bottom - 1 : bounds.Y);
            byte* targetLine = (byte*) target.BackBuffer + targetStride * y;

            /*
             * We cannot save the source increment as a positive or negative integer variable
             * because C# pointer arithmetic always casts the added value to an unsigned type,
             * resulting in an invalid pointer when attempting to add a negative offset.
             * Therefore, we must use if/else blocks to add or subtract a positive increment.
             */

            for (int dy = 0; dy < bounds.Height; dy++) {
                uint* sourceAddr = (uint*) sourceLine + (mirrorX ? bounds.Right - 1 : bounds.X);
                uint* targetAddr = (uint*) targetLine + x;

                if (mirrorX) {
                    for (int dx = 0; dx < bounds.Width; dx++) {
                        uint value = *sourceAddr;
                        *targetAddr = value;
                        --sourceAddr; ++targetAddr;
                    }
                } else {
                    for (int dx = 0; dx < bounds.Width; dx++) {
                        uint value = *sourceAddr;
                        *targetAddr = value;
                        ++sourceAddr; ++targetAddr;
                    }
                }

                if (mirrorY) sourceLine -= sourceStride;
                else sourceLine += sourceStride;
                targetLine += targetStride;
            }

            target.AddDirtyRect(new Int32Rect(x, y, bounds.Width, bounds.Height));
        }

        #endregion
        #region SetPixel

        /// <summary>
        /// Sets the specified pixel in the specified <see cref="WriteableBitmap"/> to the specified
        /// <see cref="Color"/>.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> containing the pixel whose color to set.</param>
        /// <param name="x">
        /// The x-coordinate of the pixel to change to <paramref name="color"/>.</param>
        /// <param name="y">
        /// The y-coordinate of the pixel to change to <paramref name="color"/>.</param>
        /// <param name="color">
        /// The new <see cref="Color"/> for the pixel at the specified location in the specified
        /// <paramref name="bitmap"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="bitmap"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>SetPixel</b> sets the pixel at the specified <paramref name="x"/> and <paramref
        /// name="y"/> coordinates within the <see cref="WriteableBitmap.BackBuffer"/> of the
        /// specified <paramref name="bitmap"/> to the <see cref="PixelFormats.Pbgra32"/>
        /// representation of the specified <paramref name="color"/>. No alpha blending or
        /// coordinate checking is performed.
        /// </para><note type="caution">
        /// For improved performance, <b>SetPixel</b> does not attempt to lock the specified
        /// <paramref name="bitmap"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/>
        /// and <see cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="bitmap"/>, respectively.</note></remarks>

        public static unsafe void SetPixel(
            this WriteableBitmap bitmap, int x, int y, Color color) {

            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            // use byte* as stride is in bytes, not in pixels
            byte* line = (byte*) bitmap.BackBuffer + bitmap.BackBufferStride * y;
            *((uint*) line + x) = ColorToPbgra32(color);
            bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
        }

        #endregion
        #region Shift

        /// <summary>
        /// Shifts the color channels of the specified <see cref="WriteableBitmap"/> by the
        /// specified offsets.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> whose color channels to shift.</param>
        /// <param name="r">
        /// The maximum offset by which to shift the red channel.</param>
        /// <param name="g">
        /// The maximum offset by which to shift the green channel.</param>
        /// <param name="b">
        /// The maximum offset by which to shift the blue channel.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="bitmap"/> has not been locked for write access.</exception>
        /// <remarks><para>
        /// <b>Shift</b> adjusts the color channels of all pixels within the specified <paramref
        /// name="bitmap"/> by the specified <paramref name="r"/>, <paramref name="g"/>, and
        /// <paramref name="b"/> offsets, using the <see cref="ShiftPbgra32"/> method. Please refer
        /// to that method for further details.
        /// </para><note type="caution">
        /// For improved performance, <b>Shift</b> does not attempt to lock the specified <paramref
        /// name="bitmap"/>. You must explicitly call <see cref="WriteableBitmap.Lock"/> and <see
        /// cref="WriteableBitmap.Unlock"/> before and after manipulating the <paramref
        /// name="bitmap"/>, respectively.</note></remarks>

        public static unsafe void Shift(this WriteableBitmap bitmap, int r, int g, int b) {
            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            Int32Rect bounds = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            int count = bounds.Width * bounds.Height;

            uint* addr = (uint*) bitmap.BackBuffer;
            for (int i = 0; i < count; i++) {
                uint value = *addr;
                *addr++ = ShiftPbgra32(value, r, g, b);
            }

            bitmap.AddDirtyRect(bounds);
        }

        #endregion
        #region ShiftPbgra32

        /// <summary>
        /// Shifts the color channels of the specified <see cref="PixelFormats.Pbgra32"/> color
        /// value by the specified offsets.</summary>
        /// <param name="value">
        /// The <see cref="PixelFormats.Pbgra32"/> color value whose channels to shift.</param>
        /// <param name="r">
        /// The maximum offset by which to shift the red channel.</param>
        /// <param name="g">
        /// The maximum offset by which to shift the green channel.</param>
        /// <param name="b">
        /// The maximum offset by which to shift the blue channel.</param>
        /// <returns>
        /// A new <see cref="PixelFormats.Pbgra32"/> color value whose color channels equal those of
        /// the specified <paramref name="value"/> plus the specified <paramref name="r"/>,
        /// <paramref name="g"/>, and <paramref name="b"/> offsets, multiplied by the alpha channel
        /// of <paramref name="value"/> and divided by 255.</returns>
        /// <remarks>
        /// <b>ShiftPbgra32</b> requires, but does not check, that the specified <paramref
        /// name="r"/>, <paramref name="g"/>, and <paramref name="b"/> values are in the closed
        /// interval [-255, +255]. Otherwise, the corresponding color channel would be shifted
        /// further than permitted by the alpha channel of the specified <paramref name="value"/>.
        /// </remarks>

        [CLSCompliant(false)]
        public static uint ShiftPbgra32(uint value, int r, int g, int b) {

            // extract alpha channel
            byte a = (byte) (value >> 24);
            if (a == 0) return value;

            // extract premultiplied color channels
            int rv = (byte) ((value & 0x00FF0000) >> 16);
            int gv = (byte) ((value & 0x0000FF00) >> 8);
            int bv = (byte) (value & 0x000000FF);

            // shift red channel by premultiplied offset
            if (r != 0) {
                rv += (a * r) / 255;
                if (rv < 0) rv = 0; else if (rv > 255) rv = 255;
            }

            // shift green channel by premultiplied offset
            if (g != 0) {
                gv += (a * g) / 255;
                if (gv < 0) gv = 0; else if (gv > 255) gv = 255;
            }

            // shift blue channel by premultiplied offset
            if (b != 0) {
                bv += (a * b) / 255;
                if (bv < 0) bv = 0; else if (bv > 255) bv = 255;
            }

            // create color with shifted channels
            return ((uint) bv | ((uint) gv << 8) | ((uint) rv << 16) | ((uint) a << 24));
        }

        #endregion
    }
}
