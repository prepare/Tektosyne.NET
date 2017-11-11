using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Tektosyne.Geometry;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides a secondary buffer associated with a <see cref="WriteableBitmap"/>.</summary>
    /// <remarks><para>
    /// Prior to .NET 3.5 SP1, the <see cref="WriteableBitmap"/> class required a secondary buffer
    /// when accessing its pixel data. <b>BitmapBuffer</b> provides such a buffer in the form of a
    /// <see cref="UInt32"/> array holding color values in the standard <see
    /// cref="PixelFormats.Pbgra32"/> format.
    /// </para><para>
    /// You can permanently associate a <b>BitmapBuffer</b> with a <see cref="WriteableBitmap"/>, or
    /// exchange pixel data with specified bitmaps. You can directly manipulate the pixel data
    /// stored in the <b>BitmapBuffer</b>, or use several auxiliary methods for this purpose.
    /// </para><note type="implementnotes">
    /// As of .NET 3.5 SP1, the <see cref="WriteableBitmap"/> class allows direct access to its own
    /// <see cref="WriteableBitmap.BackBuffer"/>. Various helper methods in the <see
    /// cref="BitmapUtility"/> class exploit this feature to duplicate the functionality of
    /// <b>BitmapBuffer</b> without the need for a secondary buffer. You might still wish to use
    /// <b>BitmapBuffer</b> as a separate compositing buffer, however.</note></remarks>

    public class BitmapBuffer {
        #region BitmapBuffer(Int32, Int32)

        /// <overloads>
        /// Initializes a new instance of the <see cref="BitmapBuffer"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapBuffer"/> class with the specified
        /// width and height.</summary>
        /// <param name="width">
        /// The initial <see cref="SizeI.Width"/> component of the <see cref="Size"/> property.
        /// </param>
        /// <param name="height">
        /// The initial <see cref="SizeI.Height"/> component of the <see cref="Size"/> property.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is equal to or less than zero.
        /// </exception>
        /// <remarks>
        /// The <see cref="Pixels"/> property is initialized to a <see cref="UInt32"/> array whose
        /// size is the product of the specified <paramref name="width"/> and <paramref
        /// name="height"/>. The <see cref="Bitmap"/> property is initialized to a null reference.
        /// </remarks>

        public BitmapBuffer(int width, int height) {
            Size = new SizeI(width, height);
        }

        #endregion
        #region BitmapBuffer(WriteableBitmap)

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapBuffer"/> class with the specified
        /// <see cref="WriteableBitmap"/>.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> associated with the <see cref="BitmapBuffer"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The <see cref="BitmapSource.Format"/> of <paramref name="bitmap"/> does not equal <see
        /// cref="PixelFormats.Pbgra32"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <remarks><para>
        /// The <see cref="SizeI.Width"/> and <see cref="SizeI.Height"/> components of the <see
        /// cref="Size"/> property are initialized with the <see cref="BitmapSource.PixelWidth"/>
        /// and <see cref="BitmapSource.PixelHeight"/> of the specified <paramref name="bitmap"/>,
        /// respectively.
        /// </para><para>
        /// The <see cref="Pixels"/> property is initialized to a <see cref="UInt32"/> array big
        /// enough to hold all pixels in the specified <paramref name="bitmap"/>. The actual pixel
        /// data is not copied, however. Call <see cref="Read"/> if you wish to initialize <see
        /// cref="Pixels"/> with the pixel data of the <paramref name="bitmap"/>.</para></remarks>

        public BitmapBuffer(WriteableBitmap bitmap) {
            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            Bitmap = bitmap;
        }

        #endregion
        #region Private Fields

        // property backers
        private WriteableBitmap _bitmap;
        private uint[] _pixels;
        private SizeI _size;

        #endregion
        #region Bitmap

        /// <summary>
        /// Gets or sets the <see cref="WriteableBitmap"/> associated with the <see
        /// cref="BitmapBuffer"/>.</summary>
        /// <value>
        /// The <see cref="WriteableBitmap"/> associated with the <see cref="BitmapBuffer"/>. The
        /// default is a null reference.</value>
        /// <exception cref="ArgumentException">
        /// The property is set to a bitmap whose <see cref="BitmapSource.Format"/> does not equal
        /// <see cref="PixelFormats.Pbgra32"/>.</exception>
        /// <remarks><para>
        /// A valid <b>Bitmap</b> always has a <see cref="BitmapSource.Format"/> of  <see
        /// cref="PixelFormats.Pbgra32"/>, and its <see cref="BitmapSource.PixelWidth"/> and <see
        /// cref="BitmapSource.PixelHeight"/> properties equal the <see cref="SizeI.Width"/> and
        /// <see cref="SizeI.Height"/> components of the <see cref="Size"/> property, respectively.
        /// </para><para>
        /// Setting <b>Bitmap</b> to a new value that is neither a null reference nor identical to
        /// the current value also updates the <see cref="Size"/> property and reallocates the <see
        /// cref="Pixels"/> array if necessary. The current contents of the <see cref="Pixels"/>
        /// array are lost in this case.</para></remarks>

        public WriteableBitmap Bitmap {
            [DebuggerStepThrough]
            get { return _bitmap; }
            set {
                if (value != null && value != _bitmap) {

                    if (value.Format != PixelFormats.Pbgra32)
                        ThrowHelper.ThrowArgumentExceptionWithFormat(
                            "value", Tektosyne.Strings.ArgumentPropertyInvalid, "Format");

                    // update size and buffer array if necessary
                    Size = new SizeI(value.PixelWidth, value.PixelHeight);
                }

                _bitmap = value;
            }
        }

        #endregion
        #region Bounds

        /// <summary>
        /// Gets the bounding rectangle of the <see cref="BitmapBuffer"/>.</summary>
        /// <value>
        /// The bounding rectangle of the <see cref="BitmapBuffer"/>, starting at (0,0) and
        /// extending to the <see cref="SizeI.Width"/> and <see cref="SizeI.Height"/> of the current
        /// <see cref="Size"/>.</value>
        /// <remarks>
        /// <b>Bounds</b> is provided as a convenience for methods or calculations that require a
        /// complete bounding rectangle rather than individual dimensions.</remarks>

        public RectI Bounds {
            get { return new RectI(0, 0, _size.Width, _size.Height); }
        }

        #endregion
        #region Pixels

        /// <summary>
        /// Gets the pixel data managed by the <see cref="BitmapBuffer"/>.</summary>
        /// <value>
        /// A <see cref="UInt32"/> array containing the pixel data managed by the <see
        /// cref="BitmapBuffer"/>.</value>
        /// <remarks><para>
        /// The size of the <b>Pixels</b> array equals the value of the <see cref="Size"/> property.
        /// Each array element equates one pixel in <see cref="PixelFormats.Pbgra32"/> format. The
        /// array is automatically reallocated whenever the <see cref="Size"/> property changes. Its
        /// current contents are lost in this case.
        /// </para><para>
        /// Use the <see cref="Read"/> and <see cref="Write"/> methods to synchronize the contents
        /// of the <b>Pixels</b> array with the associated <see cref="Bitmap"/> or with another <see
        /// cref="BitmapSource"/>.</para></remarks>

        [CLSCompliant(false)]
        public uint[] Pixels {
            [DebuggerStepThrough]
            get { return _pixels; }
        }

        #endregion
        #region Size

        /// <summary>
        /// Gets or sets the size of the <see cref="BitmapBuffer"/>.</summary>
        /// <value>
        /// The size of the <see cref="BitmapBuffer"/>, in pixels.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The property is set to a value whose <see cref="SizeI.Width"/> or <see
        /// cref="SizeI.Height"/> is equal to or less than zero.</exception>
        /// <remarks><para>
        /// <b>Size</b> determines both the number of elements in the <see cref="Pixels"/> array and
        /// the <see cref="BitmapSource.PixelWidth"/> and <see cref="BitmapSource.PixelHeight"/>
        /// dimensions of the associated <see cref="Bitmap"/>, if any.
        /// </para><para>
        /// Changing <b>Size</b> reallocates the <see cref="Pixels"/> array as well as any
        /// associated <see cref="Bitmap"/>. The <see cref="BitmapSource.DpiX"/> and <see
        /// cref="BitmapSource.DpiY"/> values of the current <see cref="Bitmap"/> are kept but its
        /// contents are lost, as are those of the current <see cref="Pixels"/> array.
        /// </para></remarks>

        public SizeI Size {
            [DebuggerStepThrough]
            get { return _size; }
            set {
                if (value == _size) return;

                if (value.Width <= 0 || value.Height <= 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(
                        "value", value, Strings.ArgumentContainsNotPositive);

                // reallocate buffer array
                _size = value;
                _pixels = new uint[value.Width * value.Height];

                // reallocate associated bitmap, if any
                if (_bitmap != null)
                    Bitmap = new WriteableBitmap(value.Width, value.Height,
                        _bitmap.DpiX, _bitmap.DpiY, PixelFormats.Pbgra32, null);
            }
        }

        #endregion
        #region Stride

        /// <summary>
        /// Gets the stride of the <see cref="BitmapBuffer"/>.</summary>
        /// <value>
        /// The <see cref="SizeI.Width"/> component of the current <see cref="Size"/>, multiplied by
        /// four.</value>
        /// <remarks>
        /// Copying pixel data from a <see cref="BitmapSource"/> or to a <see
        /// cref="WriteableBitmap"/> methods require the "stride" of the source or target array,
        /// i.e. the number of bytes per pixel row. Since the <see cref="Pixels"/> array contains
        /// pixel data in the <see cref="PixelFormats.Pbgra32"/> format, each pixel occupies four
        /// bytes. <b>Stride</b> therefore returns the current pixel width times four.</remarks>

        public int Stride {
            get { return _size.Width * 4; }
        }

        #endregion
        #region Public Methods
        #region BlendPixel

        /// <summary>
        /// Blends the specified pixel in the <see cref="BitmapBuffer"/> with the specified <see
        /// cref="Color"/>.</summary>
        /// <param name="x">
        /// The x-coordinate of the pixel to blend with <paramref name="color"/>.</param>
        /// <param name="y">
        /// The y-coordinate of the pixel to blend with <paramref name="color"/>.</param>
        /// <param name="color">
        /// The <see cref="Color"/> to blend with the pixel at the specified location in the <see
        /// cref="BitmapBuffer"/>.</param>
        /// <remarks><para>
        /// <b>BlendPixel</b> blends the <see cref="Pixels"/> element at the specified <paramref
        /// name="x"/> and <paramref name="y"/> coordinates with the <see
        /// cref="PixelFormats.Pbgra32"/> representation of the specified <paramref name="color"/>.
        /// </para><para>
        /// <b>BlendPixel</b> calls <see cref="BitmapUtility.BlendPbgra32"/> to perform alpha
        /// blending between the specified <paramref name="color"/> (acting as the overlay) and this
        /// <see cref="BitmapBuffer"/> (acting as the blending target). No coordinate checking is
        /// performed.</para></remarks>

        public void BlendPixel(int x, int y, Color color) {
            uint value = BitmapUtility.ColorToPbgra32(color);
            int i = x + _size.Width * y;
            _pixels[i] = BitmapUtility.BlendPbgra32(value, _pixels[i]);
        }

        #endregion
        #region Clear()

        /// <overloads>
        /// Clears the <see cref="BitmapBuffer"/>.</overloads>
        /// <summary>
        /// Clears the <see cref="BitmapBuffer"/>.</summary>
        /// <remarks>
        /// <b>Clear</b> sets all elements of the <see cref="Pixels"/> array to zero, which is the
        /// <see cref="PixelFormats.Pbgra32"/> equivalent of transparent black color.</remarks>

        public void Clear() {
            Array.Clear(_pixels, 0, _pixels.Length);
        }

        #endregion
        #region Clear(Color)

        /// <summary>
        /// Clears the <see cref="BitmapBuffer"/> with the specified <see cref="Color"/>.</summary>
        /// <param name="color">
        /// The new <see cref="Color"/> for all pixels in the <see cref="BitmapBuffer"/>.</param>
        /// <remarks>
        /// <b>Clear</b> sets all elements of the <see cref="Pixels"/> array to the <see
        /// cref="PixelFormats.Pbgra32"/> representation of the specified <paramref name="color"/>.
        /// </remarks>

        public void Clear(Color color) {
            uint value = BitmapUtility.ColorToPbgra32(color);
            for (int i = 0; i < _pixels.Length; i++)
                _pixels[i] = value;
        }

        #endregion
        #region Clear(RectI, Color)

        /// <summary>
        /// Clears the specified rectangle in the <see cref="BitmapBuffer"/> with the specified <see
        /// cref="Color"/>.</summary>
        /// <param name="bounds">
        /// The pixel rectangle in the <see cref="BitmapBuffer"/> to fill with the specified
        /// <paramref name="color"/>.</param>
        /// <param name="color">
        /// The new <see cref="Color"/> for all pixels within the specified <paramref
        /// name="bounds"/> of the <see cref="BitmapBuffer"/>.</param>
        /// <remarks>
        /// <b>Clear</b> sets all <see cref="Pixels"/> elements within the specified <paramref
        /// name="bounds"/> to the <see cref="PixelFormats.Pbgra32"/> representation of the
        /// specified <paramref name="color"/>. No coordinate checking is performed.</remarks>

        public void Clear(RectI bounds, Color color) {

            uint value = BitmapUtility.ColorToPbgra32(color);
            int offset = bounds.X + _size.Width * bounds.Y;

            for (int y = 0; y < bounds.Height; y++) {
                for (int x = 0; x < bounds.Width; x++)
                    _pixels[offset + x] = value;

                offset += _size.Width;
            }
        }

        #endregion
        #region GetPixel

        /// <summary>
        /// Gets the <see cref="Color"/> of the specified pixel in the <see cref="BitmapBuffer"/>.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate of the pixel whose <see cref="Color"/> to return.</param>
        /// <param name="y">
        /// The y-coordinate of the pixel whose <see cref="Color"/> to return.</param>
        /// <returns>
        /// The <see cref="Color"/> of the pixel at the specified location in the <see
        /// cref="BitmapBuffer"/>.</returns>
        /// <remarks>
        /// <b>GetPixel</b> returns the <see cref="Color"/> equivalent of the <see cref="Pixels"/>
        /// element at the specified <paramref name="x"/> and <paramref name="y"/> coordinates. No
        /// coordinate checking is performed.</remarks>

        public Color GetPixel(int x, int y) {
            uint value = _pixels[x + _size.Width * y];
            return BitmapUtility.ColorFromPbgra32(value);
        }

        #endregion
        #region Grow

        /// <summary>
        /// Grows the <see cref="BitmapBuffer"/> to the specified size.</summary>
        /// <param name="width">
        /// The new minimum <see cref="SizeI.Width"/> component of the <see cref="Size"/> property.
        /// </param>
        /// <param name="height">
        /// The new minimum <see cref="SizeI.Height"/> component of the <see cref="Size"/> property.
        /// </param>
        /// <remarks><para>
        /// <b>Grow</b> sets the <see cref="Size"/> property to the specified <paramref
        /// name="width"/> and <paramref name="height"/> if one or both exceed the corresponding
        /// <see cref="Size"/> component; otherwise, <b>Grow</b> does nothing.
        /// </para><para>
        /// As usual, changing <see cref="Size"/> reallocates both the <see cref="Pixels"/> array
        /// and the associated <see cref="Bitmap"/>, if any. Their existing contents are lost.
        /// </para></remarks>

        public void Grow(int width, int height) {
            if (_size.Width < width || _size.Height < height)
                Size = new SizeI(width, height);
        }

        #endregion
        #region MakeOpaque()

        /// <overloads>
        /// Makes the <see cref="BitmapBuffer"/> fully opaque.</overloads>
        /// <summary>
        /// Makes the <see cref="BitmapBuffer"/> fully opaque.</summary>
        /// <remarks>
        /// <b>MakeOpaque</b> replaces all <see cref="Pixels"/> elements with their <see
        /// cref="BitmapUtility.OpaquePbgra32"/> equivalents. Fully transparent pixels remain
        /// unchanged.</remarks>

        public void MakeOpaque() {
            for (int i = 0; i < _pixels.Length; i++)
                _pixels[i] = BitmapUtility.OpaquePbgra32(_pixels[i]);
        }

        #endregion
        #region MakeOpaque(RectI)

        /// <summary>
        /// Makes the specified rectangle in the <see cref="BitmapBuffer"/> fully opaque.</summary>
        /// <param name="bounds">
        /// The pixel rectangle in the <see cref="BitmapBuffer"/> to make opaque.</param>
        /// <remarks>
        /// <b>MakeOpaque</b> replaces all <see cref="Pixels"/> elements within the specified
        /// <paramref name="bounds"/> with their <see cref="BitmapUtility.OpaquePbgra32"/>
        /// equivalents. Fully transparent pixels remain unchanged. No coordinate checking is
        /// performed.</remarks>

        public void MakeOpaque(RectI bounds) {
            int offset = bounds.X + _size.Width * bounds.Y;

            for (int y = 0; y < bounds.Height; y++) {
                for (int x = 0; x < bounds.Width; x++) {
                    int i = offset + x;
                    _pixels[i] = BitmapUtility.OpaquePbgra32(_pixels[i]);
                }

                offset += _size.Width;
            }
        }

        #endregion
        #region MakeTransparent

        /// <summary>
        /// Makes the specified <see cref="Color"/> transparent in the <see cref="BitmapBuffer"/>.
        /// </summary>
        /// <param name="color">
        /// The <see cref="Color"/> to replace with transparent black.</param>
        /// <remarks><para>
        /// <b>MakeTransparent</b> sets all fully opaque <see cref="Pixels"/> of the specified
        /// <paramref name="color"/> to zero, which is the <see cref="PixelFormats.Pbgra32"/>
        /// equivalent of transparent black color. Partially transparent <see cref="Pixels"/> and
        /// <see cref="Pixels"/> of any other color remain unchanged.
        /// </para><para>
        /// <b>MakeTransparent</b> ignores the alpha channel of the specified <paramref
        /// name="color"/> and only replaces fully opaque <see cref="Pixels"/> because the
        /// imprecision caused by the premultiplication of color channels in the <see
        /// cref="PixelFormats.Pbgra32"/> format would otherwise produce false matches.
        /// </para><note type="implementnotes">
        /// The predefined <see cref="Colors.Transparent"/> color is transparent <em>white</em> (all
        /// color channels maximized), not transparent <em>black</em> (all color channels zero).
        /// That color is not representable in <see cref="PixelFormats.Pbgra32"/> format since all
        /// color channel information of fully transparent color is lost.</note></remarks>

        public void MakeTransparent(Color color) {
            uint value = BitmapUtility.ColorToOpaquePbgra32(color);

            for (int i = 0; i < _pixels.Length; i++)
                if (_pixels[i] == value) _pixels[i] = 0u;
        }

        #endregion
        #region Overlay(...)

        /// <overloads>
        /// Overlays pixel data from another <see cref="BitmapBuffer"/>.</overloads>
        /// <summary>
        /// Overlays pixel data from another <see cref="BitmapBuffer"/>, with alpha blending.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate where to begin writing in this <see cref="BitmapBuffer"/>.</param>
        /// <param name="y">
        /// The y-coordinate where to begin writing in this <see cref="BitmapBuffer"/>.</param>
        /// <param name="source">
        /// Another <see cref="BitmapBuffer"/> containing the rectangle to overlay.</param>
        /// <param name="bounds">
        /// The pixel rectangle within the specified <paramref name="source"/> to overlay.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>Overlay</b> blends all <see cref="Pixels"/> elements within the specified <paramref
        /// name="bounds"/> of the specified <paramref name="source"/> buffer with the corresponding
        /// <see cref="Pixels"/> elements in this <see cref="BitmapBuffer"/>, starting with the
        /// specified <paramref name="x"/> and <paramref name="y"/> coordinates.
        /// </para><para>
        /// <b>Overlay</b> calls <see cref="BitmapUtility.BlendPbgra32"/> to perform alpha blending
        /// between the specified <paramref name="source"/> buffer (acting as the overlay) and this
        /// <see cref="BitmapBuffer"/> (acting as the blending target). No coordinate checking is
        /// performed.</para></remarks>

        public void Overlay(int x, int y, BitmapBuffer source, RectI bounds) {
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");

            int sourceOffset = bounds.X + source._size.Width * bounds.Y;
            int targetOffset = x + _size.Width * y;

            for (int dy = 0; dy < bounds.Height; dy++) {
                for (int dx = 0; dx < bounds.Width; dx++) {
                    uint p0 = source._pixels[sourceOffset + dx];
                    uint p1 = _pixels[targetOffset + dx];
                    _pixels[targetOffset + dx] = BitmapUtility.BlendPbgra32(p0, p1);
                }

                sourceOffset += source._size.Width;
                targetOffset += _size.Width;
            }
        }

        #endregion
        #region Overlay(..., Byte)

        /// <summary>
        /// Overlays pixel data from another <see cref="BitmapBuffer"/>, with the specified alpha
        /// channel threshold.</summary>
        /// <param name="x">
        /// The x-coordinate where to begin writing in this <see cref="BitmapBuffer"/>.</param>
        /// <param name="y">
        /// The y-coordinate where to begin writing in this <see cref="BitmapBuffer"/>.</param>
        /// <param name="source">
        /// Another <see cref="BitmapBuffer"/> containing the rectangle to overlay.</param>
        /// <param name="bounds">
        /// The pixel rectangle within the specified <paramref name="source"/> to overlay.</param>
        /// <param name="alpha">
        /// The alpha channel threshold below which <paramref name="source"/> pixels will be
        /// ignored.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>Overlay</b> copies only those <see cref="Pixels"/> elements within the specified
        /// <paramref name="bounds"/> of the specified <paramref name="source"/> buffer whose alpha
        /// channel meets or exceeds the specified <paramref name="alpha"/> threshold.
        /// </para><para>
        /// The copies are written to the <see cref="Pixels"/> array of this <see
        /// cref="BitmapBuffer"/>, starting with the specified <paramref name="x"/> and <paramref
        /// name="y"/> coordinates. No alpha blending or coordinate checking is performed.
        /// </para></remarks>

        public void Overlay(int x, int y, BitmapBuffer source, RectI bounds, byte alpha) {
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");

            int sourceOffset = bounds.X + source._size.Width * bounds.Y;
            int targetOffset = x + _size.Width * y;

            for (int dy = 0; dy < bounds.Height; dy++) {
                for (int dx = 0; dx < bounds.Width; dx++) {
                    uint value = source._pixels[sourceOffset + dx];
                    if ((value >> 24) >= alpha)
                        _pixels[targetOffset + dx] = value;
                }

                sourceOffset += source._size.Width;
                targetOffset += _size.Width;
            }
        }

        #endregion
        #region Overlay(...)

        /// <summary>
        /// Overlays pixel data from another <see cref="BitmapBuffer"/>, with alpha blending.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate where to begin writing in this <see cref="BitmapBuffer"/>.</param>
        /// <param name="y">
        /// The y-coordinate where to begin writing in this <see cref="BitmapBuffer"/>.</param>
        /// <param name="source">
        /// Another <see cref="BitmapBuffer"/> containing the rectangle to overlay.</param>
        /// <param name="bounds">
        /// The pixel rectangle within the specified <paramref name="source"/> to overlay.</param>
        /// <param name="color">
        /// The <see cref="Color"/> to substitute for all <paramref name="source"/> pixels.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>Overlay</b> blends all <see cref="Pixels"/> elements within the specified <paramref
        /// name="bounds"/> of the specified <paramref name="source"/> buffer with the corresponding
        /// <see cref="Pixels"/> elements in this <see cref="BitmapBuffer"/>, starting with the
        /// specified <paramref name="x"/> and <paramref name="y"/> coordinates.
        /// </para><para>
        /// <b>Overlay</b> calls <see cref="BitmapUtility.BlendPbgra32"/> to perform alpha blending
        /// between the specified <paramref name="source"/> buffer (acting as the overlay) and this
        /// <see cref="BitmapBuffer"/> (acting as the blending target). No coordinate checking is
        /// performed.
        /// </para><para>
        /// <b>Overlay</b> substitutes the specified <paramref name="color"/> for the actual color
        /// channels of each <paramref name="source"/> pixel, while using its alpha channel to
        /// govern alpha blending with the corresponding <see cref="Pixels"/> element. The alpha
        /// channel of the specified <paramref name="color"/> is ignored.</para></remarks>

        public void Overlay(int x, int y, BitmapBuffer source, RectI bounds, Color color) {
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");

            int sourceOffset = bounds.X + source._size.Width * bounds.Y;
            int targetOffset = x + _size.Width * y;

            for (int dy = 0; dy < bounds.Height; dy++) {
                for (int dx = 0; dx < bounds.Width; dx++) {
                    uint p0 = source._pixels[sourceOffset + dx];
                    uint p1 = _pixels[targetOffset + dx];

                    // multiply color channels with source alpha
                    color.A = (byte) (p0 >> 24);
                    p0 = BitmapUtility.ColorToPbgra32(color);

                    _pixels[targetOffset + dx] = BitmapUtility.BlendPbgra32(p0, p1);
                }

                sourceOffset += source._size.Width;
                targetOffset += _size.Width;
            }
        }

        #endregion
        #region Read()

        /// <overloads>
        /// Reads pixel data from a bitmap into the <see cref="BitmapBuffer"/>.</overloads>
        /// <summary>
        /// Reads pixel data from the associated <see cref="Bitmap"/> into the <see
        /// cref="BitmapBuffer"/>.</summary>
        /// <remarks>
        /// <b>Read</b> replaces the entire contents of the <see cref="Pixels"/> array with the
        /// pixel data of the associated <see cref="Bitmap"/>.</remarks>

        public void Read() {
            _bitmap.CopyPixels(_pixels, Stride, 0);
        }

        #endregion
        #region Read(RectI)

        /// <summary>
        /// Reads pixel data from the specified rectangle in the associated <see cref="Bitmap"/>
        /// into the <see cref="BitmapBuffer"/>.</summary>
        /// <param name="bounds">
        /// The pixel rectangle in the <see cref="BitmapBuffer"/> to read.</param>
        /// <remarks>
        /// <b>Read</b> replaces the specified <paramref name="bounds"/> within the <see
        /// cref="Pixels"/> array with the pixel data of the associated <see cref="Bitmap"/>. No
        /// coordinate checking is performed.</remarks>

        public void Read(RectI bounds) {
            int offset = bounds.X + _size.Width * bounds.Y;
            _bitmap.CopyPixels(bounds.ToInt32Rect(), _pixels, Stride, offset);
        }

        #endregion
        #region Read(BitmapSource)

        /// <summary>
        /// Reads pixel data from the specified <see cref="BitmapSource"/> into the <see
        /// cref="BitmapBuffer"/>.</summary>
        /// <param name="bitmap">
        /// The <see cref="BitmapSource"/> to read from.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <remarks>
        /// <b>Read</b> replaces the contents of the <see cref="Pixels"/> array with the pixel data
        /// of the specified <paramref name="bitmap"/>. The copied area starts at (0,0) and extends
        /// either to the <see cref="Size"/> of the <see cref="BitmapBuffer"/> or to that of the
        /// specified <paramref name="bitmap"/> in each dimension, whichever is smaller.</remarks>

        public void Read(BitmapSource bitmap) {
            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            Int32Rect source = new Int32Rect(0, 0,
                Math.Min(bitmap.PixelWidth, _size.Width),
                Math.Min(bitmap.PixelHeight, _size.Height));

            bitmap.CopyPixels(source, _pixels, Stride, 0);
        }

        #endregion
        #region SetPixel

        /// <summary>
        /// Sets the specified pixel in the <see cref="BitmapBuffer"/> to the specified <see
        /// cref="Color"/>.</summary>
        /// <param name="x">
        /// The x-coordinate of the pixel to change to <paramref name="color"/>.</param>
        /// <param name="y">
        /// The y-coordinate of the pixel to change to <paramref name="color"/>.</param>
        /// <param name="color">
        /// The new <see cref="Color"/> for the pixel at the specified location in the <see
        /// cref="BitmapBuffer"/>.</param>
        /// <remarks>
        /// <b>SetPixel</b> sets the <see cref="Pixels"/> element at the specified <paramref
        /// name="x"/> and <paramref name="y"/> coordinates to the <see
        /// cref="PixelFormats.Pbgra32"/> representation of the specified <paramref name="color"/>.
        /// No alpha blending or coordinate checking is performed.</remarks>

        public void SetPixel(int x, int y, Color color) {
            uint value = BitmapUtility.ColorToPbgra32(color);
            _pixels[x + _size.Width * y] = value;
        }

        #endregion
        #region Write()

        /// <overloads>
        /// Writes the pixel data of the <see cref="BitmapBuffer"/> to a bitmap.</overloads>
        /// <summary>
        /// Writes the pixel data of the <see cref="BitmapBuffer"/> to the associated <see
        /// cref="Bitmap"/>.</summary>
        /// <remarks>
        /// <b>Write</b> replaces the entire pixel data of the associated <see cref="Bitmap"/> with
        /// the contents of the <see cref="Pixels"/> array.</remarks>

        public void Write() {
            Int32Rect bounds = new Int32Rect(0, 0, _size.Width, _size.Height);
            _bitmap.WritePixels(bounds, _pixels, Stride, 0);
        }

        #endregion
        #region Write(RectI)

        /// <summary>
        /// Writes the pixel data of the specified rectangle in the <see cref="BitmapBuffer"/> to
        /// the associated <see cref="Bitmap"/>.</summary>
        /// <param name="bounds">
        /// The pixel rectangle in the <see cref="BitmapBuffer"/> to write.</param>
        /// <remarks>
        /// <b>Write</b> replaces the pixel data within the specified <paramref name="bounds"/> of
        /// the associated <see cref="Bitmap"/> with the contents of the <see cref="Pixels"/> array.
        /// No coordinate checking is performed.</remarks>

        public void Write(RectI bounds) {
            int offset = bounds.X + _size.Width * bounds.Y;
            _bitmap.WritePixels(bounds.ToInt32Rect(), _pixels, Stride, offset);
        }

        #endregion
        #region Write(WriteableBitmap)

        /// <summary>
        /// Writes the pixel data of the <see cref="BitmapBuffer"/> to the specified <see
        /// cref="WriteableBitmap"/>.</summary>
        /// <param name="bitmap">
        /// The <see cref="WriteableBitmap"/> to write to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bitmap"/> is a null reference.</exception>
        /// <remarks>
        /// <b>Write</b> replaces the pixel data of the specified <paramref name="bitmap"/> with the
        /// contents of the <see cref="Pixels"/> array. The copied area starts at (0,0) and extends
        /// either to the <see cref="Size"/> of the <see cref="BitmapBuffer"/> or to that of the
        /// specified <paramref name="bitmap"/> in each dimension, whichever is smaller.</remarks>

        public void Write(WriteableBitmap bitmap) {
            if (bitmap == null)
                ThrowHelper.ThrowArgumentNullException("bitmap");

            Int32Rect source = new Int32Rect(0, 0,
                Math.Min(bitmap.PixelWidth, _size.Width),
                Math.Min(bitmap.PixelHeight, _size.Height));

            bitmap.WritePixels(source, _pixels, Stride, 0);
        }

        #endregion
        #endregion
    }
}
