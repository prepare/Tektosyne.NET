using System;
using System.Globalization;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a rectangular region in two-dimensional space, using <see cref="Double"/>
    /// coordinates.</summary>
    /// <remarks><para>
    /// <b>RectD</b> is an immutable structure whose four <see cref="Double"/> coordinates and 
    /// extensions define a rectangular region in two-dimensional space.
    /// </para><para>
    /// The <see cref="RectD.Left"/>, <see cref="RectD.Top"/>, <see cref="RectD.Right"/>, and <see
    /// cref="RectD.Bottom"/> properties assume drawing orientation rather than mathematical
    /// orientation. That is, x-coordinates increase towards the right but y-coordinates increase
    /// downward. This is the same orientation used by all BCL rectangle structures.
    /// </para><para>
    /// <b>RectD</b> uses a <em>geometric inclusion model</em> to determine which coordinates are
    /// contained within the rectangle, like <b>System.Windows.Rect</b>. That is, <see
    /// cref="RectD.Width"/> and <see cref="RectD.Height"/> act like the dimensions of a closed
    /// polygon, indicating the greatest coordinates within the <see cref="RectD"/>. Therefore, the
    /// coordinates <see cref="RectD.Right"/> (= <see cref="RectD.Left"/> + <see
    /// cref="RectD.Width"/>) and <see cref="RectD.Bottom"/> (= <see cref="RectD.Top"/> + <see
    /// cref="RectD.Height"/>) are still considered part of the <see cref="RectD"/>.
    /// </para><para>
    /// Use the <see cref="RectI"/> structure to represent rectangles with <see cref="Int32"/>
    /// components, and the <see cref="RectF"/> structure to represent rectangles with <see
    /// cref="Single"/> components. You can convert <see cref="RectD"/> instances to and from <see
    /// cref="RectI"/> and <see cref="RectF"/> instances, rounding off the <see cref="Double"/>
    /// components as necessary.</para></remarks>

    [Serializable]
    public struct RectD: IEquatable<RectD> {
        #region RectD(Double, Double, Double, Double)

        /// <overloads>
        /// Initializes a new instance of the <see cref="RectD"/> structure.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="RectD"/> structure with the specified
        /// <see cref="Double"/> coordinates and dimensions.</summary>
        /// <param name="x">
        /// The smallest <see cref="X"/> coordinate within the <see cref="RectD"/>.</param>
        /// <param name="y">
        /// The smallest <see cref="Y"/> coordinate within the <see cref="RectD"/>.</param>
        /// <param name="width">
        /// The <see cref="Width"/> of the <see cref="RectD"/>.</param>
        /// <param name="height">
        /// The <see cref="Height"/> of the <see cref="RectD"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is less than zero.</exception>

        public RectD(double x, double y, double width, double height) {
            if (width < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "width", width, Strings.ArgumentNegative);

            if (height < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "height", height, Strings.ArgumentNegative);

            X = x; Y = y;
            Width = width;
            Height = height;
        }

        #endregion
        #region RectD(PointD, PointD)

        /// <summary>
        /// Initializes a new instance of the <see cref="RectD"/> structure that contains the
        /// specified <see cref="PointD"/> coordinates.</summary>
        /// <param name="point1">
        /// The first <see cref="PointD"/> that the <see cref="RectD"/> must contain.</param>
        /// <param name="point2">
        /// The second <see cref="PointD"/> that the <see cref="RectD"/> must contain.</param>
        /// <remarks>
        /// <see cref="Location"/> is set to the smaller coordinate of <paramref name="point1"/> and
        /// <paramref name="point2"/> in each dimension, and <see cref="Size"/> is set to the
        /// difference between the larger and the smaller coordinate in each dimension.</remarks>

        public RectD(PointD point1, PointD point2) {
            double left, top, right, bottom;

            if (point1.X < point2.X) {
                left = point1.X; right = point2.X;
            } else {
                left = point2.X; right = point1.X;
            }

            if (point1.Y < point2.Y) {
                top = point1.Y; bottom = point2.Y;
            } else {
                top = point2.Y; bottom = point1.Y;
            }

            X = left; Y = top;
            Width = right - left;
            Height = bottom - top;
        }

        #endregion
        #region RectD(PointD, SizeD)

        /// <summary>
        /// Initializes a new instance of the <see cref="RectD"/> structure with the specified
        /// <see cref="PointD"/> coordinates and <see cref="SizeD"/> dimensions.</summary>
        /// <param name="location">
        /// The <see cref="Location"/> of the <see cref="RectD"/>.</param>
        /// <param name="size">
        /// The <see cref="Size"/> of the <see cref="RectD"/>.</param>

        public RectD(PointD location, SizeD size) {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="RectD"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="RectD"/> instance that was created with the default
        /// constructor.</remarks>

        public static readonly RectD Empty = new RectD();

        #endregion
        #region Public Properties
        #region X

        /// <summary>
        /// The smallest x-coordinate within the <see cref="RectD"/>.</summary>
        /// <remarks>
        /// <b>X</b> is the x-coordinate of the left edge of the <see cref="RectD"/>, assuming that
        /// x-coordinates increase to the right.</remarks>

        public readonly double X;

        #endregion
        #region Y

        /// <summary>
        /// The smallest y-coordinate within the <see cref="RectD"/>.</summary>
        /// <remarks>
        /// <b>Y</b> is the y-coordinate of the top edge of the <see cref="RectD"/>, assuming that
        /// y-coordinates increase downward.</remarks>

        public readonly double Y;

        #endregion
        #region Width

        /// <summary>
        /// The horizontal extension of the <see cref="RectD"/>.</summary>
        /// <remarks>
        /// <b>Width</b> is never less than zero.</remarks>

        public readonly double Width;

        #endregion
        #region Height

        /// <summary>
        /// The vertical extension of the <see cref="RectD"/>.</summary>
        /// <remarks>
        /// <b>Height</b> is never less than zero.</remarks>

        public readonly double Height;

        #endregion
        #region Location

        /// <summary>
        /// Gets the coordinates of the upper-left corner of the <see cref="RectD"/>.</summary>
        /// <value>
        /// A <see cref="PointD"/> containing the <see cref="X"/> and <see cref="Y"/> coordinates.
        /// </value>
        /// <remarks>
        /// <b>Location</b> holds the smallest x- and y-coordinates that are contained within the
        /// <see cref="RectD"/>.</remarks>

        public PointD Location {
            get { return new PointD(X, Y); }
        }

        #endregion
        #region Size

        /// <summary>
        /// Gets the extension of the <see cref="RectD"/>.</summary>
        /// <value>
        /// A <see cref="SizeD"/> containing the <see cref="Width"/> and <see cref="Height"/>
        /// dimensions.</value>
        /// <remarks>
        /// The <see cref="RectD"/> covers the area beginning at <see cref="Location"/> and
        /// extending over <see cref="Size"/> with increasing x- and y-coordinates.</remarks>

        public SizeD Size {
            get { return new SizeD(Width, Height); }
        }

        #endregion
        #region Left

        /// <summary>
        /// Gets the x-coordinate of the left edge of the <see cref="RectD"/>.</summary>
        /// <value>
        /// The <see cref="X"/> coordinate of the <see cref="RectD"/>.</value>
        /// <remarks>
        /// <b>Left</b> is the smallest y-coordinate that is contained within the <see
        /// cref="RectD"/>.</remarks>

        public double Left {
            get { return X; }
        }

        #endregion
        #region Top

        /// <summary>
        /// Gets the y-coordinate of the top edge of the <see cref="RectD"/>.</summary>
        /// <value>
        /// The <see cref="Y"/> coordinate of the <see cref="RectD"/>.</value>
        /// <remarks>
        /// <b>Top</b> is the smallest y-coordinate that is contained within the <see
        /// cref="RectD"/>.</remarks>

        public double Top {
            get { return Y; }
        }

        #endregion
        #region Right

        /// <summary>
        /// Gets the x-coordinate of the right edge of the <see cref="RectD"/>.</summary>
        /// <value>
        /// The sum of the <see cref="X"/> coordinate and the <see cref="Width"/> dimension.</value>
        /// <remarks>
        /// <b>Right</b> is the greatest x-coordinate that is contained within the <see
        /// cref="RectD"/>.</remarks>

        public double Right {
            get { return X + Width; }
        }

        #endregion
        #region Bottom

        /// <summary>
        /// Gets the y-coordinate of the bottom edge of the <see cref="RectD"/>.</summary>
        /// <value>
        /// The sum of the <see cref="Y"/> coordinate and the <see cref="Height"/> dimension.
        /// </value>
        /// <remarks>
        /// <b>Bottom</b> is the greatest y-coordinate that is contained within the <see
        /// cref="RectD"/>.</remarks>

        public double Bottom {
            get { return Y + Height; }
        }

        #endregion
        #region TopLeft

        /// <summary>
        /// Gets the upper-left corner of the <see cref="RectD"/>.</summary>
        /// <value>
        /// A <see cref="PointD"/> whose <see cref="PointD.X"/> coordinate equals <see
        /// cref="Left"/> and whose <see cref="PointD.Y"/> coordinate equals <see cref="Top"/>.
        /// </value>
        /// <remarks>
        /// <b>TopLeft</b> returns the same value as <see cref="Location"/>.</remarks>

        public PointD TopLeft {
            get { return new PointD(X, Y); }
        }

        #endregion
        #region TopRight

        /// <summary>
        /// Gets the upper-right corner of the <see cref="RectD"/>.</summary>
        /// <value>
        /// A <see cref="PointD"/> whose <see cref="PointD.X"/> coordinate equals <see
        /// cref="Right"/> and whose <see cref="PointD.Y"/> coordinate equals <see cref="Top"/>.
        /// </value>

        public PointD TopRight {
            get { return new PointD(X + Width, Y); }
        }

        #endregion
        #region BottomLeft

        /// <summary>
        /// Gets the lower-left corner of the <see cref="RectD"/>.</summary>
        /// <value>
        /// A <see cref="PointD"/> whose <see cref="PointD.X"/> coordinate equals <see
        /// cref="Left"/> and whose <see cref="PointD.Y"/> coordinate equals <see cref="Bottom"/>.
        /// </value>

        public PointD BottomLeft {
            get { return new PointD(X, Y + Height); }
        }

        #endregion
        #region BottomRight

        /// <summary>
        /// Gets the lower-right corner of the <see cref="RectD"/>.</summary>
        /// <value>
        /// A <see cref="PointD"/> whose <see cref="PointD.X"/> coordinate equals <see
        /// cref="Right"/> and whose <see cref="PointD.Y"/> coordinate equals <see cref="Bottom"/>.
        /// </value>

        public PointD BottomRight {
            get { return new PointD(X + Width, Y + Height); }
        }

        #endregion
        #endregion
        #region Public Methods
        #region Circumscribe()

        /// <overloads>
        /// Circumscribes a rectangle around the specified coordinates.</overloads>
        /// <summary>
        /// Circumscribes a <see cref="RectI"/> around the <see cref="RectD"/>.</summary>
        /// <returns>
        /// A <see cref="RectI"/> that entirely covers the <see cref="RectD"/>.</returns>
        /// <remarks>
        /// <b>Circumscribe</b> returns a <see cref="RectI"/> that contains the <see
        /// cref="Fortran.Floor"/> of the <see cref="X"/> and <see cref="Y"/> coordinates, and the
        /// <see cref="Fortran.Ceiling"/> of the <see cref="Width"/> and <see cref="Height"/>
        /// dimensions. This ensures that the <see cref="RectD"/> is entirely covered.</remarks>

        public RectI Circumscribe() {
            return new RectI(
                Fortran.Floor(X), Fortran.Floor(Y),
                Fortran.Ceiling(Width), Fortran.Ceiling(Height));
        }

        #endregion
        #region Circumscribe(PointD[])

        /// <summary>
        /// Circumscribes a <see cref="RectD"/> around the specified <see cref="PointD"/>
        /// coordinates.</summary>
        /// <param name="points">
        /// An <see cref="Array"/> containing the <see cref="PointD"/> coordinates whose bounds to
        /// determine.</param>
        /// <returns>
        /// The smallest <see cref="RectD"/> that contains all specified <paramref name="points"/>.
        /// </returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="points"/> is a null reference or an empty array.</exception>

        public static RectD Circumscribe(params PointD[] points) {
            if (points == null || points.Length == 0)
                ThrowHelper.ThrowArgumentNullOrEmptyException("points");

            double x0 = Double.MaxValue, y0 = Double.MaxValue;
            double x1 = Double.MinValue, y1 = Double.MinValue;

            foreach (PointD point in points) {
                if (x0 > point.X) x0 = point.X;
                if (y0 > point.Y) y0 = point.Y;
                if (x1 < point.X) x1 = point.X;
                if (y1 < point.Y) y1 = point.Y;
            }

            return new RectD(x0, y0, x1 - x0, y1 - y0);
        }

        #endregion
        #region Contains(Double, Double)

        /// <overloads>
        /// Indicates whether the <see cref="RectD"/> contains the specified coordinates.
        /// </overloads>
        /// <summary>
        /// Indicates whether the <see cref="RectD"/> contains the specified <see cref="Double"/>
        /// coordinates.</summary>
        /// <param name="x">
        /// The x-coordinate to examine.</param>
        /// <param name="y">
        /// The y-coordinate to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectD"/> contains the specified <paramref name="x"/> and
        /// <paramref name="y"/> coordinates; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> assumes that the <see cref="RectD"/> contains the <see cref="Right"/>
        /// and <see cref="Bottom"/> coordinates.</remarks>

        public bool Contains(double x, double y) {
            return (x >= X && y >= Y && x <= X + Width && y <= Y + Height);
        }

        #endregion
        #region Contains(PointD)

        /// <summary>
        /// Indicates whether the <see cref="RectD"/> contains the specified <see cref="PointD"/>
        /// coordinates.</summary>
        /// <param name="point">
        /// The <see cref="PointD"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectD"/> contains the specified <paramref name="point"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> assumes that the <see cref="RectD"/> contains the <see cref="Right"/>
        /// and <see cref="Bottom"/> coordinates.</remarks>

        public bool Contains(PointD point) {
            return Contains(point.X, point.Y);
        }

        #endregion
        #region Contains(RectD)

        /// <summary>
        /// Indicates whether the <see cref="RectD"/> contains the specified rectangle.</summary>
        /// <param name="rect">
        /// The <see cref="RectD"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectD"/> entirely contains the specified <paramref
        /// name="rect"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> returns <c>true</c> even if both <see cref="RectD"/> instances have a
        /// <see cref="Width"/> or <see cref="Height"/> of zero, provided they have the same <see
        /// cref="Location"/> in the corresponding dimension.</remarks>

        public bool Contains(RectD rect) {

            return (rect.X >= X && rect.Y >= Y
                && rect.X + rect.Width <= X + Width
                && rect.Y + rect.Height <= Y + Height);
        }

        #endregion
        #region ContainsOpen(Double, Double)

        /// <overloads>
        /// Indicates whether the <see cref="RectD"/> contains the specified coordinates, excluding
        /// <see cref="Right"/> and <see cref="Bottom"/>.</overloads>
        /// <summary>
        /// Indicates whether the <see cref="RectD"/> contains the specified <see cref="Double"/>
        /// coordinates, excluding <see cref="Right"/> and <see cref="Bottom"/>.</summary>
        /// <param name="x">
        /// The x-coordinate to examine.</param>
        /// <param name="y">
        /// The y-coordinate to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectD"/> contains the specified <paramref name="x"/> and
        /// <paramref name="y"/> coordinates, excluding <see cref="Right"/> and <see
        /// cref="Bottom"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>ContainsOpen</b> assumes that the <see cref="RectD"/> does not contain the <see
        /// cref="Right"/> and <see cref="Bottom"/> coordinates, emulating <see cref="RectI"/>
        /// behavior.</remarks>

        public bool ContainsOpen(double x, double y) {
            return (x >= X && y >= Y && x < X + Width && y < Y + Height);
        }

        #endregion
        #region ContainsOpen(PointD)

        /// <summary>
        /// Indicates whether the <see cref="RectD"/> contains the specified <see cref="PointD"/>
        /// coordinates, excluding <see cref="Right"/> and <see cref="Bottom"/>.</summary>
        /// <param name="point">
        /// The <see cref="PointD"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectD"/> contains the specified <paramref name="point"/>,
        /// excluding <see cref="Right"/> and <see cref="Bottom"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <b>ContainsOpen</b> assumes that the <see cref="RectD"/> does not contain the <see
        /// cref="Right"/> and <see cref="Bottom"/> coordinates, emulating <see cref="RectI"/>
        /// behavior.</remarks>

        public bool ContainsOpen(PointD point) {
            return ContainsOpen(point.X, point.Y);
        }

        #endregion
        #region GetDistanceVector

        /// <summary>
        /// Finds the distance vector from the specified <see cref="PointD"/> coordinates to the
        /// nearest edges of the <see cref="RectD"/>.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="PointD"/> vector indicating the distance of each <paramref name="q"/> 
        /// coordinate from the nearest corresponding edge of the <see cref="RectD"/>.</returns>
        /// <remarks><para>
        /// <b>GetDistanceVector</b> defines the components of the returned <see cref="PointD"/>
        /// vector as follows, assuming that <em>qx</em> and <em>qy</em> are the coordinates of
        /// <paramref name="q"/>:
        /// </para><list type="table"><listheader>
        /// <term><b>X</b></term><term><b>Y</b></term><description>Condition</description>
        /// </listheader><item>
        /// <term>0</term><term></term><description>
        /// <see cref="Left"/> &lt;= <em>qx</em> &lt;= <see cref="Right"/></description>
        /// </item><item>
        /// <term><em>qx</em> – <see cref="Left"/></term><term></term>
        /// <description><em>qx</em> &lt; <see cref="Left"/></description>
        /// </item><item>
        /// <term><em>qx</em> – <see cref="Right"/></term><term></term>
        /// <description><em>qx</em> &gt; <see cref="Right"/></description>
        /// </item><item>
        /// <term/><term>0</term><description>
        /// <see cref="Top"/> &lt;= <em>qy</em> &lt;= <see cref="Bottom"/></description>
        /// </item><item>
        /// <term/><term><em>qy</em> – <see cref="Top"/></term>
        /// <description><em>qy</em> &lt; <see cref="Top"/></description>
        /// </item><item>
        /// <term/><term><em>qy</em> – <see cref="Bottom"/></term>
        /// <description><em>qy</em> &gt; <see cref="Bottom"/></description>
        /// </item></list><para>
        /// Each vector component is zero exactly if the corresponding <paramref name="q"/>
        /// coordinate lies within the corresponding <see cref="RectD"/> extension. Otherwise, the
        /// component’s absolute value indicates the coordinate’s distance from the nearest <see
        /// cref="RectD"/> edge, and its sign indicates that edge itself.</para></remarks>

        public PointD GetDistanceVector(PointD q) {
            double qx = q.X - X, qy = q.Y - Y;

            double x = (qx < 0 ? qx : qx > Width ? qx - Width : 0);
            double y = (qy < 0 ? qy : qy > Height ? qy - Height : 0);

            return new PointD(x, y);
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="RectD"/> instance.</summary>
        /// <returns>
        /// An <see cref="Double"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the values of the <see cref="X"/>, <see cref="Y"/>, <see
        /// cref="Width"/>, and <see cref="Height"/> properties.</remarks>

        public override unsafe int GetHashCode() {
            unchecked {
                double x = X, y = Y, w = Width, h = Height;
                long xi = *((long*) &x), yi = *((long*) &y);
                long wi = *((long*) &w), hi = *((long*) &h);

                return (int) xi ^ (int) (xi >> 32) ^ (int) yi ^ (int) (yi >> 32)
                    ^ (int) wi ^ (int) (wi >> 32) ^ (int) hi ^ (int) (hi >> 32);
            }
        }

        #endregion
        #region Intersect(LineD)

        /// <overloads>
        /// Intersects the <see cref="RectD"/> with the specified object.</overloads>
        /// <summary>
        /// Intersects the <see cref="RectD"/> with the specified <see cref="LineD"/>.</summary>
        /// <param name="line">
        /// The <see cref="LineD"/> to intersect with the <see cref="RectD"/>.</param>
        /// <param name="intersection">
        /// On success, the intersection of the <see cref="RectD"/> and the specified <paramref
        /// name="line"/>; otherwise, <see cref="LineD.Empty"/>.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectD"/> intersects with the specified <paramref
        /// name="line"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Intersect</b> performs the Liang-Barsky line clipping algorithm. This C#
        /// implementation was adapted from the C implementation by Daniel White, published at <a
        /// href="http://www.skytopia.com/project/articles/compsci/clipping.html">Skytopia</a>.
        /// </remarks>

        public bool Intersect(LineD line, out LineD intersection) {

            double x0 = line.Start.X, y0 = line.Start.Y;
            double dx = line.End.X - x0, dy = line.End.Y - y0;
            double t0 = 0, t1 = 1, p = 0, q = 0;

            // traverse all four rectangle borders
            for (int border = 0; border < 4; border++) {
                switch (border) {
                    case 0: p = -dx; q = x0 - X; break;
                    case 1: p = +dx; q = X + Width - x0; break;
                    case 2: p = -dy; q = y0 - Y; break;
                    case 3: p = +dy; q = Height + Y - y0; break;
                }

                if (p == 0) {
                    // parallel line outside of rectangle
                    if (q < 0) goto failure;
                } else {
                    double r = q / p;
                    if (p < 0) {
                        if (r > t1) goto failure;
                        if (r > t0) t0 = r;
                    } else {
                        if (r < t0) goto failure;
                        if (r < t1) t1 = r;
                    }
                }
            }

            intersection = new LineD(
                x0 + t0 * dx, y0 + t0 * dy,
                x0 + t1 * dx, y0 + t1 * dy);
            return true;

        failure:
            intersection = LineD.Empty;
            return false;
        }

        #endregion
        #region Intersect(PointD[])

        /// <summary>
        /// Intersects the <see cref="RectD"/> with the specified arbitrary polygon.</summary>
        /// <param name="polygon">
        /// An <see cref="Array"/> containing <see cref="PointD"/> coordinates that are the vertices
        /// of the polygon to intersect with the <see cref="RectD"/>.</param>
        /// <param name="intersection">
        /// On success, the intersection of the <see cref="RectD"/> and the specified <paramref
        /// name="polygon"/>; otherwise, an empty <see cref="Array"/>.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectD"/> intersects with the specified <paramref
        /// name="polygon"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="polygon"/> is a null reference or an empty array.</exception>
        /// <remarks><para>
        /// <b>Intersect</b> performs the Sutherland–Hodgman polygon clipping algorithm, optimized
        /// for an axis-aligned <see cref="RectD"/> as the clipping polygon. At intersection points,
        /// the border coordinates of the <see cref="RectD"/> are copied rather than computed,
        /// allowing exact floating-point comparisons.
        /// </para><para>
        /// The specified <paramref name="polygon"/> and the returned <paramref
        /// name="intersection"/> are implicitly assumed to be closed, with an edge connecting the
        /// first and last vertex. Therefore, all vertices should be different.
        /// </para><para>
        /// Unless the specified <paramref name="polygon"/> is convex, the returned <paramref
        /// name="intersection"/> may represent multiple polygons, connected across the borders of
        /// the <see cref="RectD"/>.</para></remarks>

        public bool Intersect(PointD[] polygon, out PointD[] intersection) {
            if (polygon == null || polygon.Length == 0)
                ThrowHelper.ThrowArgumentNullOrEmptyException("polygon");

            // input/output storage for intermediate polygons
            int outputLength = polygon.Length;
            PointD[] inputVertices = new PointD[3 * outputLength];
            PointD[] outputVertices = new PointD[3 * outputLength];
            Array.Copy(polygon, outputVertices, outputLength);

            double q = 0;
            bool startInside = false, endInside = false;

            // traverse all four rectangle borders
            for (int border = 0; border < 4; border++) {
                switch (border) {
                    case 0: q = X; break;
                    case 1: q = X + Width; break;
                    case 2: q = Y; break;
                    case 3: q = Y + Height; break;
                }

                // last output is new input for current border
                PointD[] swap = inputVertices;
                inputVertices = outputVertices;
                outputVertices = swap;
                int inputLength = outputLength;
                outputLength = 0;

                // check all polygon edges against infinite border
                PointD start = inputVertices[inputLength - 1];
                for (int i = 0; i < inputLength; i++) {
                    PointD end = inputVertices[i];

                    switch (border) {
                        case 0: startInside = (start.X >= q); endInside = (end.X >= q); break;
                        case 1: startInside = (start.X <= q); endInside = (end.X <= q); break;
                        case 2: startInside = (start.Y >= q); endInside = (end.Y >= q); break;
                        case 3: startInside = (start.Y <= q); endInside = (end.Y <= q); break;
                    }

                    // store intersection point if border crossed
                    if (startInside != endInside) {
                        double x, y, dx = end.X - start.X, dy = end.Y - start.Y;
                        if (border < 2) {
                            x = q;
                            y = (x == end.X ? end.Y : start.Y + (x - start.X) * dy / dx);
                        } else {
                            y = q;
                            x = (y == end.Y ? end.X : start.X + (y - start.Y) * dx / dy);
                        }
                        outputVertices[outputLength++] = new PointD(x, y);
                    }

                    // also store end point if inside rectangle
                    if (endInside) outputVertices[outputLength++] = end;
                    start = end;
                }

                if (outputLength == 0) break;
            }

            intersection = new PointD[outputLength];
            Array.Copy(outputVertices, intersection, outputLength);

            return (outputLength > 0);
        }

        #endregion
        #region Intersect(RectD)

        /// <summary>
        /// Intersects the <see cref="RectD"/> with the specified rectangle.</summary>
        /// <param name="rect">
        /// The <see cref="RectD"/> to intersect with this instance.</param>
        /// <param name="intersection">
        /// On success, the intersection of the <see cref="RectD"/> and the specified <paramref
        /// name="rect"/>; otherwise, <see cref="Empty"/>.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectD"/> intersects with the specified <paramref
        /// name="rect"/>; otherwise, <c>false</c>.</returns>

        public bool Intersect(RectD rect, out RectD intersection) {

            double x = Math.Max(X, rect.X);
            double y = Math.Max(Y, rect.Y);
            double width = Math.Min(X + Width, rect.X + rect.Width) - x;
            double height = Math.Min(Y + Height, rect.Y + rect.Height) - y;

            if (height < 0 || width < 0) {
                intersection = Empty;
                return false;
            } else {
                intersection = new RectD(x, y, width, height);
                return true;
            }
        }

        #endregion
        #region IntersectsWith(LineD)

        /// <overloads>
        /// Determines whether the <see cref="RectD"/> intersects with the specified object.
        /// </overloads>
        /// <summary>
        /// Determines whether the <see cref="RectD"/> intersects with the specified <see
        /// cref="LineD"/>.</summary>
        /// <param name="line">
        /// The <see cref="LineD"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectD"/> intersects with the specified <paramref
        /// name="line"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>IntersectsWith</b> performs the same Liang-Barsky line clipping algorithm as <see
        /// cref="Intersect(LineD, out LineD)"/>, but without computing the intersecting line
        /// segment.</remarks>

        public bool IntersectsWith(LineD line) {

            double x0 = line.Start.X, y0 = line.Start.Y;
            double dx = line.End.X - x0, dy = line.End.Y - y0;
            double t0 = 0, t1 = 1, p = 0, q = 0;

            // traverse all four rectangle borders
            for (int border = 0; border < 4; border++) {
                switch (border) {
                    case 0: p = -dx; q = x0 - X; break;
                    case 1: p = +dx; q = X + Width - x0; break;
                    case 2: p = -dy; q = y0 - Y; break;
                    case 3: p = +dy; q = Height + Y - y0; break;
                }

                if (p == 0) {
                    // parallel line outside of rectangle
                    if (q < 0) return false;
                } else {
                    double r = q / p;
                    if (p < 0) {
                        if (r > t1) return false;
                        else if (r > t0) t0 = r;
                    } else {
                        if (r < t0) return false;
                        else if (r < t1) t1 = r;
                    }
                }
            }

            return true;
        }

        #endregion
        #region IntersectsWith(RectD)

        /// <summary>
        /// Determines whether the <see cref="RectD"/> intersects with the specified rectangle.
        /// </summary>
        /// <param name="rect">
        /// The <see cref="RectD"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectD"/> intersects with the specified <paramref
        /// name="rect"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>IntersectsWith</b> returns <c>true</c> even if both <see cref="RectD"/> instances
        /// have a <see cref="Width"/> or <see cref="Height"/> of zero, provided they have the same
        /// <see cref="Location"/> in the corresponding dimension.</remarks>

        public bool IntersectsWith(RectD rect) {

            return (rect.X + rect.Width >= X && rect.X <= X + Width
                && rect.Y + rect.Height >= Y && rect.Y <= Y + Height);
        }

        #endregion
        #region Locate(PointD)

        /// <overloads>
        /// Determines the location of the specified <see cref="PointD"/> coordinates relative to
        /// the <see cref="RectD"/>.</overloads>
        /// <summary>
        /// Determines the location of the specified <see cref="PointD"/> coordinates relative to
        /// the <see cref="RectD"/>, using exact coordinate comparisons.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="RectLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="RectD"/>.</returns>
        /// <remarks>
        /// <b>Locate</b> never returns <see cref="RectLocation.None"/>, and always returns a
        /// bitwise combination of an <b>…X</b> and a <b>…Y</b> value.</remarks>

        public RectLocation Locate(PointD q) {
            double qx = q.X - X, qy = q.Y - Y;

            RectLocation x = (qx < 0 ? RectLocation.BeforeX :
                (qx == 0 ? RectLocation.StartX :
                (qx < Width ? RectLocation.InsideX :
                (qx == Width ? RectLocation.EndX : RectLocation.AfterX))));

            RectLocation y = (qy < 0 ? RectLocation.BeforeY :
                (qy == 0 ? RectLocation.StartY :
                (qy < Height ? RectLocation.InsideY :
                (qy == Height ? RectLocation.EndY : RectLocation.AfterY))));

            return x | y;
        }

        #endregion
        #region Locate(PointD, Double)

        /// <summary>
        /// Determines the location of the specified <see cref="PointD"/> coordinates relative to
        /// the <see cref="RectD"/>, given the specified epsilon for coordinate comparisons.
        /// </summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <param name="epsilon">
        /// The maximum absolute distance at which coordinates should be considered equal.</param>
        /// <returns>
        /// A <see cref="RectLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="RectD"/>.</returns>
        /// <remarks><para>
        /// <b>Locate</b> is identical with the basic <see cref="Locate(PointD)"/> overload but uses
        /// the specified <paramref name="epsilon"/> to compare individual coordinates.
        /// </para><para>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but <b>Locate</b>
        /// does not check this condition.</para></remarks>

        public RectLocation Locate(PointD q, double epsilon) {
            double qx = q.X - X, qy = q.Y - Y;

            RectLocation x = (Math.Abs(qx) <= epsilon ? RectLocation.StartX :
                (Math.Abs(qx - Width) <= epsilon ? RectLocation.EndX :
                (qx < 0 ? x = RectLocation.BeforeX :
                (qx < Width ? x = RectLocation.InsideX : RectLocation.AfterX))));

            RectLocation y = (Math.Abs(qy) <= epsilon ? RectLocation.StartY :
                (Math.Abs(qy - Height) <= epsilon ? RectLocation.EndY :
                (qy < 0 ? y = RectLocation.BeforeY :
                (qy < Height ? y = RectLocation.InsideY : RectLocation.AfterY))));

            return x | y;
        }

        #endregion
        #region Offset(Double, Double)

        /// <overloads>
        /// Moves the <see cref="RectD"/> by the specified offset.</overloads>
        /// <summary>
        /// Moves the <see cref="RectD"/> by the specified <see cref="Double"/> values.</summary>
        /// <param name="x">
        /// The horizontal offset applied to the <see cref="RectD"/>.</param>
        /// <param name="y">
        /// The vertical offset applied to the <see cref="RectD"/>.</param>
        /// <returns>
        /// A new <see cref="RectD"/> with the same <see cref="Size"/> as this instance, and whose
        /// <see cref="X"/> and <see cref="Y"/> coordinates are offset by the specified <paramref
        /// name="x"/> and <paramref name="y"/> values.</returns>

        public RectD Offset(double x, double y) {
            return new RectD(X + x, Y + y, Width, Height);
        }

        #endregion
        #region Offset(PointD)

        /// <summary>
        /// Moves the <see cref="RectD"/> by the specified <see cref="PointD"/> vector.</summary>
        /// <param name="vector">
        /// A <see cref="PointD"/> value whose components define the horizontal and vertical offset
        /// applied to the <see cref="RectD"/>.</param>
        /// <returns>
        /// A new <see cref="RectD"/> with the same <see cref="Size"/> as this instance, and whose
        /// <see cref="Location"/> is offset by the specified <paramref name="vector"/>.</returns>

        public RectD Offset(PointD vector) {
            return new RectD(Location + vector, Size);
        }

        #endregion
        #region Round

        /// <summary>
        /// Converts the <see cref="RectD"/> to a <see cref="RectI"/> by rounding coordinates and
        /// dimensions to the nearest <see cref="Int32"/> values.</summary>
        /// <returns>
        /// A <see cref="RectI"/> instance whose <see cref="RectI.Location"/> and <see
        /// cref="RectI.Size"/> properties equal the corresponding properties of the <see
        /// cref="RectD"/>, rounded to the nearest <see cref="Int32"/> values.</returns>
        /// <remarks>
        /// The <see cref="Double"/> components of the <see cref="RectD"/> are converted to <see
        /// cref="Int32"/> components using <see cref="Fortran.NInt"/> rounding.</remarks>

        public RectI Round() {
            return new RectI(Fortran.NInt(X), Fortran.NInt(Y),
                Fortran.NInt(Width), Fortran.NInt(Height));
        }

        #endregion
        #region ToRectF

        /// <summary>
        /// Converts the <see cref="RectD"/> to a <see cref="RectF"/> by casting coordinates and
        /// dimensions to the equivalent <see cref="Single"/> values.</summary>
        /// <returns>
        /// A <see cref="RectF"/> instance whose <see cref="RectF.Location"/> and <see
        /// cref="RectF.Size"/> properties equal the corresponding properties of the <see
        /// cref="RectD"/>, cast to the equivalent <see cref="Single"/> values.</returns>

        public RectF ToRectF() {
            return new RectF((float) X, (float) Y, (float) Width, (float) Height);
        }

        #endregion
        #region ToRectI

        /// <summary>
        /// Converts the <see cref="RectD"/> to a <see cref="RectI"/> by truncating coordinates and
        /// dimensions to the nearest <see cref="Int32"/> values.</summary>
        /// <returns>
        /// A <see cref="RectI"/> instance whose <see cref="RectI.Location"/> and <see
        /// cref="RectI.Size"/> properties equal the corresponding properties of the <see
        /// cref="RectD"/>, truncated to the nearest <see cref="Int32"/> values.</returns>

        public RectI ToRectI() {
            return new RectI((int) X, (int) Y, (int) Width, (int) Height);
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="RectD"/>.</summary>
        /// <returns>
        /// A <see cref="String"/> containing the values of the <see cref="X"/>, <see cref="Y"/>,
        /// <see cref="Width"/>, and <see cref="Height"/> properties.</returns>

        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture,
                "{{X={0}, Y={1}, Width={2}, Height={3}}}", X, Y, Width, Height);
        }

        #endregion
        #region Union

        /// <summary>
        /// Determines the union of the <see cref="RectD"/> and the specified rectangle.</summary>
        /// <param name="rect">
        /// The <see cref="RectD"/> to combine with this instance.</param>
        /// <returns>
        /// A <see cref="RectD"/> that contains the union of the specified <paramref name="rect"/>
        /// and this instance.</returns>

        public RectD Union(RectD rect) {

            double x = Math.Min(X, rect.X);
            double y = Math.Min(Y, rect.Y);
            double width = Math.Max(X + Width, rect.X + rect.Width) - x;
            double height = Math.Max(Y + Height, rect.Y + rect.Height) - y;

            return new RectD(x, y, width, height);
        }

        #endregion
        #endregion
        #region Public Operators
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="RectD"/> instances have the same value.</summary>
        /// <param name="a">
        /// The first <see cref="RectD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="RectD"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(RectD)"/> method to test the two <see
        /// cref="RectD"/> instances for value equality.</remarks>

        public static bool operator ==(RectD a, RectD b) {
            return a.Equals(b);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="RectD"/> instances have different values.</summary>
        /// <param name="a">
        /// The first <see cref="RectD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="RectD"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different from the value of
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(RectD)"/> method to test the two <see
        /// cref="RectD"/> instances for value inequality.</remarks>

        public static bool operator !=(RectD a, RectD b) {
            return !a.Equals(b);
        }

        #endregion
        #region RectD(RectF)

        /// <summary>
        /// Converts a <see cref="RectF"/> to a <see cref="RectD"/> with identical coordinates and
        /// dimensions.</summary>
        /// <param name="rect">
        /// The <see cref="RectF"/> instance to convert into a <see cref="RectD"/> instance.</param>
        /// <returns>
        /// A <see cref="RectD"/> instance whose <see cref="Location"/> and <see cref="Size"/>
        /// properties equal the corresponding properties of the specified <paramref name="rect"/>.
        /// </returns>

        public static implicit operator RectD(RectF rect) {
            return new RectD(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion
        #region RectD(RectI)

        /// <summary>
        /// Converts a <see cref="RectI"/> to a <see cref="RectD"/> with identical coordinates and
        /// dimensions.</summary>
        /// <param name="rect">
        /// The <see cref="RectI"/> instance to convert into a <see cref="RectD"/> instance.</param>
        /// <returns>
        /// A <see cref="RectD"/> instance whose <see cref="Location"/> and <see cref="Size"/>
        /// properties equal the corresponding properties of the specified <paramref name="rect"/>.
        /// </returns>

        public static implicit operator RectD(RectI rect) {
            return new RectD(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion
        #region RectF(RectD)

        /// <summary>
        /// Converts a <see cref="RectD"/> to a <see cref="RectF"/> by casting coordinates and
        /// dimensions to the equivalent <see cref="Single"/> values.</summary>
        /// <param name="rect">
        /// The <see cref="RectD"/> instance to convert into a <see cref="RectF"/> instance.</param>
        /// <returns>
        /// A <see cref="RectF"/> instance whose <see cref="RectF.Location"/> and <see
        /// cref="RectF.Size"/> properties equal the corresponding properties of the specified
        /// <paramref name="rect"/>, cast to the equivalent <see cref="Single"/> values.</returns>

        public static explicit operator RectF(RectD rect) {
            return rect.ToRectF();
        }

        #endregion
        #region RectI(RectD)

        /// <summary>
        /// Converts a <see cref="RectD"/> to a <see cref="RectI"/> by truncating coordinates and
        /// dimensions to the nearest <see cref="Int32"/> values.</summary>
        /// <param name="rect">
        /// The <see cref="RectD"/> instance to convert into a <see cref="RectI"/> instance.</param>
        /// <returns>
        /// A <see cref="RectI"/> instance whose <see cref="RectI.Location"/> and <see
        /// cref="RectI.Size"/> properties equal the corresponding properties of the specified
        /// <paramref name="rect"/>, truncated to the nearest <see cref="Int32"/> values.</returns>

        public static explicit operator RectI(RectD rect) {
            return rect.ToRectI();
        }

        #endregion
        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="RectD"/> instances have the same value.</overloads>
        /// <summary>
        /// Determines whether this <see cref="RectD"/> instance and a specified object, which must
        /// be a <see cref="RectD"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="RectD"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="RectD"/> instance and its
        /// value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="RectD"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(RectD)"/> overload to test
        /// the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is RectD))
                return false;

            return Equals((RectD) obj);
        }

        #endregion
        #region Equals(RectD)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="RectD"/> have the same
        /// value.</summary>
        /// <param name="rect">
        /// A <see cref="RectD"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="rect"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="X"/>, <see cref="Y"/>, <see
        /// cref="Width"/>, and <see cref="Height"/> properties of the two <see cref="RectD"/>
        /// instances to test for value equality.</remarks>

        public bool Equals(RectD rect) {
            return (X == rect.X && Y == rect.Y
                && Width == rect.Width && Height == rect.Height);
        }

        #endregion
        #region Equals(RectD, RectD)

        /// <summary>
        /// Determines whether two specified <see cref="RectD"/> instances have the same value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="RectD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="RectD"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(RectD)"/> overload to test the
        /// two <see cref="RectD"/> instances for value equality.</remarks>

        public static bool Equals(RectD a, RectD b) {
            return a.Equals(b);
        }

        #endregion
        #region Equals(RectD, RectD, Double)

        /// <summary>
        /// Determines whether two specified <see cref="RectD"/> instances have the same value,
        /// given the specified epsilon.</summary>
        /// <param name="a">
        /// The first <see cref="RectD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="RectD"/> to compare.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which the coordinates and dimensions of <paramref
        /// name="a"/> and <paramref name="b"/> should be considered equal.</param>
        /// <returns>
        /// <c>true</c> if the absolute difference between the coordinates and dimensions of
        /// <paramref name="a"/> and <paramref name="b"/> is less than or equal to <paramref
        /// name="epsilon"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but <b>Equals</b>
        /// does not check this condition.</remarks>

        public static bool Equals(RectD a, RectD b, double epsilon) {

            return (Math.Abs(a.X - b.X) <= epsilon
                && Math.Abs(a.Y - b.Y) <= epsilon
                && Math.Abs(a.Width - b.Width) <= epsilon
                && Math.Abs(a.Height - b.Height) <= epsilon);
        }

        #endregion
        #endregion
    }
}
