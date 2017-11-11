using System;
using System.Globalization;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a rectangular region in two-dimensional space, using <see cref="Single"/>
    /// coordinates.</summary>
    /// <remarks><para>
    /// <b>RectF</b> is an immutable structure whose four <see cref="Single"/> coordinates and 
    /// extensions define a rectangular region in two-dimensional space.
    /// </para><para>
    /// The <see cref="RectF.Left"/>, <see cref="RectF.Top"/>, <see cref="RectF.Right"/>, and <see
    /// cref="RectF.Bottom"/> properties assume drawing orientation rather than mathematical
    /// orientation. That is, x-coordinates increase towards the right but y-coordinates increase
    /// downward. This is the same orientation used by all BCL rectangle structures.
    /// </para><para>
    /// <b>RectF</b> uses a <em>geometric inclusion model</em> to determine which coordinates are
    /// contained within the rectangle, like <b>System.Windows.Rect</b>. That is, <see
    /// cref="RectF.Width"/> and <see cref="RectF.Height"/> act like the dimensions of a closed
    /// polygon, indicating the greatest coordinates within the <see cref="RectF"/>. Therefore, the
    /// coordinates <see cref="RectF.Right"/> (= <see cref="RectF.Left"/> + <see
    /// cref="RectF.Width"/>) and <see cref="RectF.Bottom"/> (= <see cref="RectF.Top"/> + <see
    /// cref="RectF.Height"/>) are still considered part of the <see cref="RectF"/>.
    /// </para><note type="caution">
    /// The equivalent BCL type, <b>System.Drawing.RectangleF</b>, uses an index inclusion model
    /// like <see cref="RectI"/>, not a geometric inclusion model like <b>RectF</b>. This makes no
    /// sense for floating-point coordinates and is evidently an artifact of GDI+ compatibility.
    /// </note><para>
    /// Use the <see cref="RectI"/> structure to represent rectangles with <see cref="Int32"/>
    /// components, and the <see cref="RectD"/> structure to represent rectangles with <see
    /// cref="Double"/> components. You can convert <see cref="RectF"/> instances to and from <see
    /// cref="RectI"/> instances, rounding off the <see cref="Single"/> components as necessary.
    /// </para></remarks>

    [Serializable]
    public struct RectF: IEquatable<RectF> {
        #region RectF(Single, Single, Single, Single)

        /// <overloads>
        /// Initializes a new instance of the <see cref="RectF"/> structure.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="RectF"/> structure with the specified
        /// <see cref="Single"/> coordinates and dimensions.</summary>
        /// <param name="x">
        /// The smallest <see cref="X"/> coordinate within the <see cref="RectF"/>.</param>
        /// <param name="y">
        /// The smallest <see cref="Y"/> coordinate within the <see cref="RectF"/>.</param>
        /// <param name="width">
        /// The <see cref="Width"/> of the <see cref="RectF"/>.</param>
        /// <param name="height">
        /// The <see cref="Height"/> of the <see cref="RectF"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is less than zero.</exception>

        public RectF(float x, float y, float width, float height) {
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
        #region RectF(PointF, PointF)

        /// <summary>
        /// Initializes a new instance of the <see cref="RectF"/> structure that contains the
        /// specified <see cref="PointF"/> coordinates.</summary>
        /// <param name="point1">
        /// The first <see cref="PointF"/> that the <see cref="RectF"/> must contain.</param>
        /// <param name="point2">
        /// The second <see cref="PointF"/> that the <see cref="RectF"/> must contain.</param>
        /// <remarks>
        /// <see cref="Location"/> is set to the smaller coordinate of <paramref name="point1"/> and
        /// <paramref name="point2"/> in each dimension, and <see cref="Size"/> is set to the
        /// difference between the larger and the smaller coordinate in each dimension.</remarks>

        public RectF(PointF point1, PointF point2) {
            float left, top, right, bottom;

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
        #region RectF(PointF, SizeF)

        /// <summary>
        /// Initializes a new instance of the <see cref="RectF"/> structure with the specified
        /// <see cref="PointF"/> coordinates and <see cref="SizeF"/> dimensions.</summary>
        /// <param name="location">
        /// The <see cref="Location"/> of the <see cref="RectF"/>.</param>
        /// <param name="size">
        /// The <see cref="Size"/> of the <see cref="RectF"/>.</param>

        public RectF(PointF location, SizeF size) {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="RectF"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="RectF"/> instance that was created with the default
        /// constructor.</remarks>

        public static readonly RectF Empty = new RectF();

        #endregion
        #region Public Properties
        #region X

        /// <summary>
        /// The smallest x-coordinate within the <see cref="RectF"/>.</summary>
        /// <remarks>
        /// <b>X</b> is the x-coordinate of the left edge of the <see cref="RectF"/>, assuming that
        /// x-coordinates increase to the right.</remarks>

        public readonly float X;

        #endregion
        #region Y

        /// <summary>
        /// The smallest y-coordinate within the <see cref="RectF"/>.</summary>
        /// <remarks>
        /// <b>Y</b> is the y-coordinate of the top edge of the <see cref="RectF"/>, assuming that
        /// y-coordinates increase downward.</remarks>

        public readonly float Y;

        #endregion
        #region Width

        /// <summary>
        /// The horizontal extension of the <see cref="RectF"/>.</summary>
        /// <remarks>
        /// <b>Width</b> is never less than zero.</remarks>

        public readonly float Width;

        #endregion
        #region Height

        /// <summary>
        /// The vertical extension of the <see cref="RectF"/>.</summary>
        /// <remarks>
        /// <b>Height</b> is never less than zero.</remarks>

        public readonly float Height;

        #endregion
        #region Location

        /// <summary>
        /// Gets the coordinates of the upper-left corner of the <see cref="RectF"/>.</summary>
        /// <value>
        /// A <see cref="PointF"/> containing the <see cref="X"/> and <see cref="Y"/> coordinates.
        /// </value>
        /// <remarks>
        /// <b>Location</b> holds the smallest x- and y-coordinates that are contained within the
        /// <see cref="RectF"/>.</remarks>

        public PointF Location {
            get { return new PointF(X, Y); }
        }

        #endregion
        #region Size

        /// <summary>
        /// Gets the extension of the <see cref="RectF"/>.</summary>
        /// <value>
        /// A <see cref="SizeF"/> containing the <see cref="Width"/> and <see cref="Height"/>
        /// dimensions.</value>
        /// <remarks>
        /// The <see cref="RectF"/> covers the area beginning at <see cref="Location"/> and
        /// extending over <see cref="Size"/> with increasing x- and y-coordinates.</remarks>

        public SizeF Size {
            get { return new SizeF(Width, Height); }
        }

        #endregion
        #region Left

        /// <summary>
        /// Gets the x-coordinate of the left edge of the <see cref="RectF"/>.</summary>
        /// <value>
        /// The <see cref="X"/> coordinate of the <see cref="RectF"/>.</value>
        /// <remarks>
        /// <b>Left</b> is the smallest y-coordinate that is contained within the <see
        /// cref="RectF"/>.</remarks>

        public float Left {
            get { return X; }
        }

        #endregion
        #region Top

        /// <summary>
        /// Gets the y-coordinate of the top edge of the <see cref="RectF"/>.</summary>
        /// <value>
        /// The <see cref="Y"/> coordinate of the <see cref="RectF"/>.</value>
        /// <remarks>
        /// <b>Top</b> is the smallest y-coordinate that is contained within the <see
        /// cref="RectF"/>.</remarks>

        public float Top {
            get { return Y; }
        }

        #endregion
        #region Right

        /// <summary>
        /// Gets the x-coordinate of the right edge of the <see cref="RectF"/>.</summary>
        /// <value>
        /// The sum of the <see cref="X"/> coordinate and the <see cref="Width"/> dimension.</value>
        /// <remarks>
        /// <b>Right</b> is the greatest x-coordinate that is contained within the <see
        /// cref="RectF"/>.</remarks>

        public float Right {
            get { return X + Width; }
        }

        #endregion
        #region Bottom

        /// <summary>
        /// Gets the y-coordinate of the bottom edge of the <see cref="RectF"/>.</summary>
        /// <value>
        /// The sum of the <see cref="Y"/> coordinate and the <see cref="Height"/> dimension.
        /// </value>
        /// <remarks>
        /// <b>Bottom</b> is the greatest y-coordinate that is contained within the <see
        /// cref="RectF"/>.</remarks>

        public float Bottom {
            get { return Y + Height; }
        }

        #endregion
        #region TopLeft

        /// <summary>
        /// Gets the upper-left corner of the <see cref="RectF"/>.</summary>
        /// <value>
        /// A <see cref="PointF"/> whose <see cref="PointF.X"/> coordinate equals <see
        /// cref="Left"/> and whose <see cref="PointF.Y"/> coordinate equals <see cref="Top"/>.
        /// </value>
        /// <remarks>
        /// <b>TopLeft</b> returns the same value as <see cref="Location"/>.</remarks>

        public PointF TopLeft {
            get { return new PointF(X, Y); }
        }

        #endregion
        #region TopRight

        /// <summary>
        /// Gets the upper-right corner of the <see cref="RectF"/>.</summary>
        /// <value>
        /// A <see cref="PointF"/> whose <see cref="PointF.X"/> coordinate equals <see
        /// cref="Right"/> and whose <see cref="PointF.Y"/> coordinate equals <see cref="Top"/>.
        /// </value>

        public PointF TopRight {
            get { return new PointF(X + Width, Y); }
        }

        #endregion
        #region BottomLeft

        /// <summary>
        /// Gets the lower-left corner of the <see cref="RectF"/>.</summary>
        /// <value>
        /// A <see cref="PointF"/> whose <see cref="PointF.X"/> coordinate equals <see
        /// cref="Left"/> and whose <see cref="PointF.Y"/> coordinate equals <see cref="Bottom"/>.
        /// </value>

        public PointF BottomLeft {
            get { return new PointF(X, Y + Height); }
        }

        #endregion
        #region BottomRight

        /// <summary>
        /// Gets the lower-right corner of the <see cref="RectF"/>.</summary>
        /// <value>
        /// A <see cref="PointF"/> whose <see cref="PointF.X"/> coordinate equals <see
        /// cref="Right"/> and whose <see cref="PointF.Y"/> coordinate equals <see cref="Bottom"/>.
        /// </value>

        public PointF BottomRight {
            get { return new PointF(X + Width, Y + Height); }
        }

        #endregion
        #endregion
        #region Public Methods
        #region Circumscribe()

        /// <overloads>
        /// Circumscribes a rectangle around the specified coordinates.</overloads>
        /// <summary>
        /// Circumscribes a <see cref="RectI"/> around the <see cref="RectF"/>.</summary>
        /// <returns>
        /// A <see cref="RectI"/> that entirely covers the <see cref="RectF"/>.</returns>
        /// <remarks>
        /// <b>Circumscribe</b> returns a <see cref="RectI"/> that contains the <see
        /// cref="Fortran.Floor"/> of the <see cref="X"/> and <see cref="Y"/> coordinates, and the
        /// <see cref="Fortran.Ceiling"/> of the <see cref="Width"/> and <see cref="Height"/>
        /// dimensions. This ensures that the <see cref="RectF"/> is entirely covered.</remarks>

        public RectI Circumscribe() {
            return new RectI(
                Fortran.Floor(X), Fortran.Floor(Y),
                Fortran.Ceiling(Width), Fortran.Ceiling(Height));
        }

        #endregion
        #region Circumscribe(PointF[])

        /// <summary>
        /// Circumscribes a <see cref="RectF"/> around the specified <see cref="PointF"/>
        /// coordinates.</summary>
        /// <param name="points">
        /// An <see cref="Array"/> containing the <see cref="PointF"/> coordinates whose bounds to
        /// determine.</param>
        /// <returns>
        /// The smallest <see cref="RectF"/> that contains all specified <paramref name="points"/>.
        /// </returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="points"/> is a null reference or an empty array.</exception>

        public static RectF Circumscribe(params PointF[] points) {
            if (points == null || points.Length == 0)
                ThrowHelper.ThrowArgumentNullOrEmptyException("points");

            float x0 = Single.MaxValue, y0 = Single.MaxValue;
            float x1 = Single.MinValue, y1 = Single.MinValue;

            foreach (PointF point in points) {
                if (x0 > point.X) x0 = point.X;
                if (y0 > point.Y) y0 = point.Y;
                if (x1 < point.X) x1 = point.X;
                if (y1 < point.Y) y1 = point.Y;
            }

            return new RectF(x0, y0, x1 - x0, y1 - y0);
        }

        #endregion
        #region Contains(Single, Single)

        /// <overloads>
        /// Indicates whether the <see cref="RectF"/> contains the specified coordinates.
        /// </overloads>
        /// <summary>
        /// Indicates whether the <see cref="RectF"/> contains the specified <see cref="Single"/>
        /// coordinates.</summary>
        /// <param name="x">
        /// The x-coordinate to examine.</param>
        /// <param name="y">
        /// The y-coordinate to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectF"/> contains the specified <paramref name="x"/> and
        /// <paramref name="y"/> coordinates; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> assumes that the <see cref="RectF"/> contains the <see cref="Right"/>
        /// and <see cref="Bottom"/> coordinates.</remarks>

        public bool Contains(float x, float y) {
            return (x >= X && y >= Y && x <= X + Width && y <= Y + Height);
        }

        #endregion
        #region Contains(PointF)

        /// <summary>
        /// Indicates whether the <see cref="RectF"/> contains the specified <see cref="PointF"/>
        /// coordinates.</summary>
        /// <param name="point">
        /// The <see cref="PointF"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectF"/> contains the specified <paramref name="point"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> assumes that the <see cref="RectF"/> contains the <see cref="Right"/>
        /// and <see cref="Bottom"/> coordinates.</remarks>

        public bool Contains(PointF point) {
            return Contains(point.X, point.Y);
        }

        #endregion
        #region Contains(RectF)

        /// <summary>
        /// Indicates whether the <see cref="RectF"/> contains the specified rectangle.</summary>
        /// <param name="rect">
        /// The <see cref="RectF"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectF"/> entirely contains the specified <paramref
        /// name="rect"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> returns <c>true</c> even if both <see cref="RectF"/> instances have a
        /// <see cref="Width"/> or <see cref="Height"/> of zero, provided they have the same <see
        /// cref="Location"/> in the corresponding dimension.</remarks>

        public bool Contains(RectF rect) {

            return (rect.X >= X && rect.Y >= Y
                && rect.X + rect.Width <= X + Width
                && rect.Y + rect.Height <= Y + Height);
        }

        #endregion
        #region ContainsOpen(Single, Single)

        /// <overloads>
        /// Indicates whether the <see cref="RectF"/> contains the specified coordinates, excluding
        /// <see cref="Right"/> and <see cref="Bottom"/>.</overloads>
        /// <summary>
        /// Indicates whether the <see cref="RectF"/> contains the specified <see cref="Single"/>
        /// coordinates, excluding <see cref="Right"/> and <see cref="Bottom"/>.</summary>
        /// <param name="x">
        /// The x-coordinate to examine.</param>
        /// <param name="y">
        /// The y-coordinate to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectF"/> contains the specified <paramref name="x"/> and
        /// <paramref name="y"/> coordinates, excluding <see cref="Right"/> and <see
        /// cref="Bottom"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>ContainsOpen</b> assumes that the <see cref="RectF"/> does not contain the <see
        /// cref="Right"/> and <see cref="Bottom"/> coordinates, emulating <see cref="RectI"/>
        /// behavior.</remarks>

        public bool ContainsOpen(float x, float y) {
            return (x >= X && y >= Y && x < X + Width && y < Y + Height);
        }

        #endregion
        #region ContainsOpen(PointF)

        /// <summary>
        /// Indicates whether the <see cref="RectF"/> contains the specified <see cref="PointF"/>
        /// coordinates, excluding <see cref="Right"/> and <see cref="Bottom"/>.</summary>
        /// <param name="point">
        /// The <see cref="PointF"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectF"/> contains the specified <paramref name="point"/>,
        /// excluding <see cref="Right"/> and <see cref="Bottom"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <b>ContainsOpen</b> assumes that the <see cref="RectF"/> does not contain the <see
        /// cref="Right"/> and <see cref="Bottom"/> coordinates, emulating <see cref="RectI"/>
        /// behavior.</remarks>

        public bool ContainsOpen(PointF point) {
            return ContainsOpen(point.X, point.Y);
        }

        #endregion
        #region GetDistanceVector

        /// <summary>
        /// Finds the distance vector from the specified <see cref="PointF"/> coordinates to the
        /// nearest edges of the <see cref="RectF"/>.</summary>
        /// <param name="q">
        /// The <see cref="PointF"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="PointF"/> vector indicating the distance of each <paramref name="q"/> 
        /// coordinate from the nearest corresponding edge of the <see cref="RectF"/>.</returns>
        /// <remarks><para>
        /// <b>GetDistanceVector</b> defines the components of the returned <see cref="PointF"/>
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
        /// coordinate lies within the corresponding <see cref="RectF"/> extension. Otherwise, the
        /// component’s absolute value indicates the coordinate’s distance from the nearest <see
        /// cref="RectF"/> edge, and its sign indicates that edge itself.</para></remarks>

        public PointF GetDistanceVector(PointF q) {
            float qx = q.X - X, qy = q.Y - Y;

            float x = (qx < 0 ? qx : qx > Width ? qx - Width : 0);
            float y = (qy < 0 ? qy : qy > Height ? qy - Height : 0);

            return new PointF(x, y);
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="RectF"/> instance.</summary>
        /// <returns>
        /// An <see cref="Single"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the values of the <see cref="X"/>, <see cref="Y"/>, <see
        /// cref="Width"/>, and <see cref="Height"/> properties.</remarks>

        public override unsafe int GetHashCode() {
            unchecked {
                float x = X, y = Y, w = Width, h = Height;
                int xi = *((int*) &x), yi = *((int*) &y);
                int wi = *((int*) &w), hi = *((int*) &h);
                return xi ^ yi ^ wi ^ hi;
            }
        }

        #endregion
        #region Intersect(LineF)

        /// <overloads>
        /// Intersects the <see cref="RectF"/> with the specified object.</overloads>
        /// <summary>
        /// Intersects the <see cref="RectF"/> with the specified <see cref="LineF"/>.</summary>
        /// <param name="line">
        /// The <see cref="LineF"/> to intersect with the <see cref="RectF"/>.</param>
        /// <param name="intersection">
        /// On success, the intersection of the <see cref="RectF"/> and the specified <paramref
        /// name="line"/>; otherwise, <see cref="LineF.Empty"/>.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectF"/> intersects with the specified <paramref
        /// name="line"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Intersect</b> performs the Liang-Barsky line clipping algorithm. This C#
        /// implementation was adapted from the C implementation by Daniel White, published at <a
        /// href="http://www.skytopia.com/project/articles/compsci/clipping.html">Skytopia</a>.
        /// </remarks>

        public bool Intersect(LineF line, out LineF intersection) {

            float x0 = line.Start.X, y0 = line.Start.Y;
            float dx = line.End.X - x0, dy = line.End.Y - y0;
            float t0 = 0, t1 = 1, p = 0, q = 0;

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
                    float r = q / p;
                    if (p < 0) {
                        if (r > t1) goto failure;
                        if (r > t0) t0 = r;
                    } else {
                        if (r < t0) goto failure;
                        if (r < t1) t1 = r;
                    }
                }
            }

            intersection = new LineF(
                x0 + t0 * dx, y0 + t0 * dy,
                x0 + t1 * dx, y0 + t1 * dy);
            return true;

        failure:
            intersection = LineF.Empty;
            return false;
        }

        #endregion
        #region Intersect(PointF[])

        /// <summary>
        /// Intersects the <see cref="RectF"/> with the specified arbitrary polygon.</summary>
        /// <param name="polygon">
        /// An <see cref="Array"/> containing <see cref="PointF"/> coordinates that are the vertices
        /// of the polygon to intersect with the <see cref="RectF"/>.</param>
        /// <param name="intersection">
        /// On success, the intersection of the <see cref="RectF"/> and the specified <paramref
        /// name="polygon"/>; otherwise, an empty <see cref="Array"/>.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectF"/> intersects with the specified <paramref
        /// name="polygon"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="polygon"/> is a null reference or an empty array.</exception>
        /// <remarks><para>
        /// <b>Intersect</b> performs the Sutherland–Hodgman polygon clipping algorithm, optimized
        /// for an axis-aligned <see cref="RectF"/> as the clipping polygon. At intersection points,
        /// the border coordinates of the <see cref="RectF"/> are copied rather than computed,
        /// allowing exact floating-point comparisons.
        /// </para><para>
        /// The specified <paramref name="polygon"/> and the returned <paramref
        /// name="intersection"/> are implicitly assumed to be closed, with an edge connecting the
        /// first and last vertex. Therefore, all vertices should be different.
        /// </para><para>
        /// Unless the specified <paramref name="polygon"/> is convex, the returned <paramref
        /// name="intersection"/> may represent multiple polygons, connected across the borders of
        /// the <see cref="RectF"/>.</para></remarks>

        public bool Intersect(PointF[] polygon, out PointF[] intersection) {
            if (polygon == null || polygon.Length == 0)
                ThrowHelper.ThrowArgumentNullOrEmptyException("polygon");

            // input/output storage for intermediate polygons
            int outputLength = polygon.Length;
            PointF[] inputVertices = new PointF[3 * outputLength];
            PointF[] outputVertices = new PointF[3 * outputLength];
            Array.Copy(polygon, outputVertices, outputLength);

            float q = 0;
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
                PointF[] swap = inputVertices;
                inputVertices = outputVertices;
                outputVertices = swap;
                int inputLength = outputLength;
                outputLength = 0;

                // check all polygon edges against infinite border
                PointF start = inputVertices[inputLength - 1];
                for (int i = 0; i < inputLength; i++) {
                    PointF end = inputVertices[i];

                    switch (border) {
                        case 0: startInside = (start.X >= q); endInside = (end.X >= q); break;
                        case 1: startInside = (start.X <= q); endInside = (end.X <= q); break;
                        case 2: startInside = (start.Y >= q); endInside = (end.Y >= q); break;
                        case 3: startInside = (start.Y <= q); endInside = (end.Y <= q); break;
                    }

                    // store intersection point if border crossed
                    if (startInside != endInside) {
                        float x, y, dx = end.X - start.X, dy = end.Y - start.Y;
                        if (border < 2) {
                            x = q;
                            y = (x == end.X ? end.Y : start.Y + (x - start.X) * dy / dx);
                        } else {
                            y = q;
                            x = (y == end.Y ? end.X : start.X + (y - start.Y) * dx / dy);
                        }
                        outputVertices[outputLength++] = new PointF(x, y);
                    }

                    // also store end point if inside rectangle
                    if (endInside) outputVertices[outputLength++] = end;
                    start = end;
                }

                if (outputLength == 0) break;
            }

            intersection = new PointF[outputLength];
            Array.Copy(outputVertices, intersection, outputLength);

            return (outputLength > 0);
        }

        #endregion
        #region Intersect(RectF)

        /// <summary>
        /// Intersects the <see cref="RectF"/> with the specified rectangle.</summary>
        /// <param name="rect">
        /// The <see cref="RectF"/> to intersect with this instance.</param>
        /// <param name="intersection">
        /// On success, the intersection of the <see cref="RectF"/> and the specified <paramref
        /// name="rect"/>; otherwise, <see cref="Empty"/>.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectF"/> intersects with the specified <paramref
        /// name="rect"/>; otherwise, <c>false</c>.</returns>

        public bool Intersect(RectF rect, out RectF intersection) {

            float x = Math.Max(X, rect.X);
            float y = Math.Max(Y, rect.Y);
            float width = Math.Min(X + Width, rect.X + rect.Width) - x;
            float height = Math.Min(Y + Height, rect.Y + rect.Height) - y;

            if (height < 0 || width < 0) {
                intersection = Empty;
                return false;
            } else {
                intersection = new RectF(x, y, width, height);
                return true;
            }
        }

        #endregion
        #region IntersectsWith(LineF)

        /// <overloads>
        /// Determines whether the <see cref="RectF"/> intersects with the specified object.
        /// </overloads>
        /// <summary>
        /// Determines whether the <see cref="RectF"/> intersects with the specified <see
        /// cref="LineF"/>.</summary>
        /// <param name="line">
        /// The <see cref="LineF"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectF"/> intersects with the specified <paramref
        /// name="line"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>IntersectsWith</b> performs the same Liang-Barsky line clipping algorithm as <see
        /// cref="Intersect(LineF, out LineF)"/>, but without computing the intersecting line
        /// segment.</remarks>

        public bool IntersectsWith(LineF line) {

            float x0 = line.Start.X, y0 = line.Start.Y;
            float dx = line.End.X - x0, dy = line.End.Y - y0;
            float t0 = 0, t1 = 1, p = 0, q = 0;

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
                    float r = q / p;
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
        #region IntersectsWith(RectF)

        /// <summary>
        /// Determines whether the <see cref="RectF"/> intersects with the specified rectangle.
        /// </summary>
        /// <param name="rect">
        /// The <see cref="RectF"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectF"/> intersects with the specified <paramref
        /// name="rect"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>IntersectsWith</b> returns <c>true</c> even if both <see cref="RectF"/> instances
        /// have a <see cref="Width"/> or <see cref="Height"/> of zero, provided they have the same
        /// <see cref="Location"/> in the corresponding dimension.</remarks>

        public bool IntersectsWith(RectF rect) {

            return (rect.X + rect.Width >= X && rect.X <= X + Width
                && rect.Y + rect.Height >= Y && rect.Y <= Y + Height);
        }

        #endregion
        #region Locate(PointF)

        /// <overloads>
        /// Determines the location of the specified <see cref="PointF"/> coordinates relative to
        /// the <see cref="RectF"/>.</overloads>
        /// <summary>
        /// Determines the location of the specified <see cref="PointF"/> coordinates relative to
        /// the <see cref="RectF"/>, using exact coordinate comparisons.</summary>
        /// <param name="q">
        /// The <see cref="PointF"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="RectLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="RectF"/>.</returns>
        /// <remarks>
        /// <b>Locate</b> never returns <see cref="RectLocation.None"/>, and always returns a
        /// bitwise combination of an <b>…X</b> and a <b>…Y</b> value.</remarks>

        public RectLocation Locate(PointF q) {
            float qx = q.X - X, qy = q.Y - Y;

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
        #region Locate(PointF, Single)

        /// <summary>
        /// Determines the location of the specified <see cref="PointF"/> coordinates relative to
        /// the <see cref="RectF"/>, given the specified epsilon for coordinate comparisons.
        /// </summary>
        /// <param name="q">
        /// The <see cref="PointF"/> coordinates to examine.</param>
        /// <param name="epsilon">
        /// The maximum absolute distance at which coordinates should be considered equal.</param>
        /// <returns>
        /// A <see cref="RectLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="RectF"/>.</returns>
        /// <remarks><para>
        /// <b>Locate</b> is identical with the basic <see cref="Locate(PointF)"/> overload but uses
        /// the specified <paramref name="epsilon"/> to compare individual coordinates.
        /// </para><para>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but <b>Locate</b>
        /// does not check this condition.</para></remarks>

        public RectLocation Locate(PointF q, float epsilon) {
            float qx = q.X - X, qy = q.Y - Y;

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
        #region Offset(Single, Single)

        /// <overloads>
        /// Moves the <see cref="RectF"/> by the specified offset.</overloads>
        /// <summary>
        /// Moves the <see cref="RectF"/> by the specified <see cref="Single"/> values.</summary>
        /// <param name="x">
        /// The horizontal offset applied to the <see cref="RectF"/>.</param>
        /// <param name="y">
        /// The vertical offset applied to the <see cref="RectF"/>.</param>
        /// <returns>
        /// A new <see cref="RectF"/> with the same <see cref="Size"/> as this instance, and whose
        /// <see cref="X"/> and <see cref="Y"/> coordinates are offset by the specified <paramref
        /// name="x"/> and <paramref name="y"/> values.</returns>

        public RectF Offset(float x, float y) {
            return new RectF(X + x, Y + y, Width, Height);
        }

        #endregion
        #region Offset(PointF)

        /// <summary>
        /// Moves the <see cref="RectF"/> by the specified <see cref="PointF"/> vector.</summary>
        /// <param name="vector">
        /// A <see cref="PointF"/> value whose components define the horizontal and vertical offset
        /// applied to the <see cref="RectF"/>.</param>
        /// <returns>
        /// A new <see cref="RectF"/> with the same <see cref="Size"/> as this instance, and whose
        /// <see cref="Location"/> is offset by the specified <paramref name="vector"/>.</returns>

        public RectF Offset(PointF vector) {
            return new RectF(Location + vector, Size);
        }

        #endregion
        #region Round

        /// <summary>
        /// Converts the <see cref="RectF"/> to a <see cref="RectI"/> by rounding coordinates and
        /// dimensions to the nearest <see cref="Int32"/> values.</summary>
        /// <returns>
        /// A <see cref="RectI"/> instance whose <see cref="RectI.Location"/> and <see
        /// cref="RectI.Size"/> properties equal the corresponding properties of the <see
        /// cref="RectF"/>, rounded to the nearest <see cref="Int32"/> values.</returns>
        /// <remarks>
        /// The <see cref="Single"/> components of the <see cref="RectF"/> are converted to <see
        /// cref="Int32"/> components using <see cref="Fortran.NInt"/> rounding.</remarks>

        public RectI Round() {
            return new RectI(Fortran.NInt(X), Fortran.NInt(Y),
                Fortran.NInt(Width), Fortran.NInt(Height));
        }

        #endregion
        #region ToRectI

        /// <summary>
        /// Converts the <see cref="RectF"/> to a <see cref="RectI"/> by truncating coordinates and
        /// dimensions to the nearest <see cref="Int32"/> values.</summary>
        /// <returns>
        /// A <see cref="RectI"/> instance whose <see cref="RectI.Location"/> and <see
        /// cref="RectI.Size"/> properties equal the corresponding properties of the <see
        /// cref="RectF"/>, truncated to the nearest <see cref="Int32"/> values.</returns>

        public RectI ToRectI() {
            return new RectI((int) X, (int) Y, (int) Width, (int) Height);
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="RectF"/>.</summary>
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
        /// Determines the union of the <see cref="RectF"/> and the specified rectangle.</summary>
        /// <param name="rect">
        /// The <see cref="RectF"/> to combine with this instance.</param>
        /// <returns>
        /// A <see cref="RectF"/> that contains the union of the specified <paramref name="rect"/>
        /// and this instance.</returns>

        public RectF Union(RectF rect) {

            float x = Math.Min(X, rect.X);
            float y = Math.Min(Y, rect.Y);
            float width = Math.Max(X + Width, rect.X + rect.Width) - x;
            float height = Math.Max(Y + Height, rect.Y + rect.Height) - y;

            return new RectF(x, y, width, height);
        }

        #endregion
        #endregion
        #region Public Operators
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="RectF"/> instances have the same value.</summary>
        /// <param name="a">
        /// The first <see cref="RectF"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="RectF"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(RectF)"/> method to test the two <see
        /// cref="RectF"/> instances for value equality.</remarks>

        public static bool operator ==(RectF a, RectF b) {
            return a.Equals(b);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="RectF"/> instances have different values.</summary>
        /// <param name="a">
        /// The first <see cref="RectF"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="RectF"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different from the value of
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(RectF)"/> method to test the two <see
        /// cref="RectF"/> instances for value inequality.</remarks>

        public static bool operator !=(RectF a, RectF b) {
            return !a.Equals(b);
        }

        #endregion
        #region RectF(RectI)

        /// <summary>
        /// Converts a <see cref="RectI"/> to a <see cref="RectF"/> with identical coordinates and
        /// dimensions.</summary>
        /// <param name="rect">
        /// The <see cref="RectI"/> instance to convert into a <see cref="RectF"/> instance.</param>
        /// <returns>
        /// A <see cref="RectF"/> instance whose <see cref="Location"/> and <see cref="Size"/>
        /// properties equal the corresponding properties of the specified <paramref name="rect"/>.
        /// </returns>

        public static implicit operator RectF(RectI rect) {
            return new RectF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #endregion
        #region RectI(RectF)

        /// <summary>
        /// Converts a <see cref="RectF"/> to a <see cref="RectI"/> by truncating coordinates and
        /// dimensions to the nearest <see cref="Int32"/> values.</summary>
        /// <param name="rect">
        /// The <see cref="RectF"/> instance to convert into a <see cref="RectI"/> instance.</param>
        /// <returns>
        /// A <see cref="RectI"/> instance whose <see cref="RectI.Location"/> and <see
        /// cref="RectI.Size"/> properties equal the corresponding properties of the specified
        /// <paramref name="rect"/>, truncated to the nearest <see cref="Int32"/> values.</returns>

        public static explicit operator RectI(RectF rect) {
            return rect.ToRectI();
        }

        #endregion
        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="RectF"/> instances have the same value.</overloads>
        /// <summary>
        /// Determines whether this <see cref="RectF"/> instance and a specified object, which must
        /// be a <see cref="RectF"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="RectF"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="RectF"/> instance and its
        /// value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="RectF"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(RectF)"/> overload to test
        /// the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is RectF))
                return false;

            return Equals((RectF) obj);
        }

        #endregion
        #region Equals(RectF)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="RectF"/> have the same
        /// value.</summary>
        /// <param name="rect">
        /// A <see cref="RectF"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="rect"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="X"/>, <see cref="Y"/>, <see
        /// cref="Width"/>, and <see cref="Height"/> properties of the two <see cref="RectF"/>
        /// instances to test for value equality.</remarks>

        public bool Equals(RectF rect) {
            return (X == rect.X && Y == rect.Y
                && Width == rect.Width && Height == rect.Height);
        }

        #endregion
        #region Equals(RectF, RectF)

        /// <summary>
        /// Determines whether two specified <see cref="RectF"/> instances have the same value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="RectF"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="RectF"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(RectF)"/> overload to test the
        /// two <see cref="RectF"/> instances for value equality.</remarks>

        public static bool Equals(RectF a, RectF b) {
            return a.Equals(b);
        }

        #endregion
        #region Equals(RectF, RectF, Single)

        /// <summary>
        /// Determines whether two specified <see cref="RectF"/> instances have the same value,
        /// given the specified epsilon.</summary>
        /// <param name="a">
        /// The first <see cref="RectF"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="RectF"/> to compare.</param>
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

        public static bool Equals(RectF a, RectF b, float epsilon) {

            return (Math.Abs(a.X - b.X) <= epsilon
                && Math.Abs(a.Y - b.Y) <= epsilon
                && Math.Abs(a.Width - b.Width) <= epsilon
                && Math.Abs(a.Height - b.Height) <= epsilon);
        }

        #endregion
        #endregion
    }
}
