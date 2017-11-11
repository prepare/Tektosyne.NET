using System;
using System.Globalization;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a rectangular region in two-dimensional space, using <see cref="Int32"/>
    /// coordinates.</summary>
    /// <remarks><para>
    /// <b>RectI</b> is an immutable structure whose four <see cref="Int32"/> coordinates and 
    /// extensions define a rectangular region in two-dimensional space.
    /// </para><para>
    /// The <see cref="RectI.Left"/>, <see cref="RectI.Top"/>, <see cref="RectI.Right"/>, and <see
    /// cref="RectI.Bottom"/> properties assume drawing orientation rather than mathematical
    /// orientation. That is, x-coordinates increase towards the right but y-coordinates increase
    /// downward. This is the same orientation used by all BCL rectangle structures.
    /// </para><para>
    /// <b>RectI</b> uses an <em>index inclusion model</em> to determine which coordinates are
    /// contained within the rectangle, like <b>System.Drawing.Rectangle</b>. That is, <see
    /// cref="RectI.Width"/> and <see cref="RectI.Height"/> act like the <b>Count</b> property of an
    /// indexed collection, indicating coordinates one unit beyond the <see cref="RectI"/>.
    /// Therefore, the coordinates <see cref="RectI.Right"/> (= <see cref="RectI.Left"/> + <see
    /// cref="RectI.Width"/>) and <see cref="RectI.Bottom"/> (= <see cref="RectI.Top"/> + <see
    /// cref="RectI.Height"/>) are <em>not</em> considered part of the <see cref="RectI"/>.
    /// </para><para>
    /// Use the <see cref="RectF"/> structure to represent rectangles with <see cref="Single"/>
    /// components, and the <see cref="RectD"/> structure to represent rectangles with <see
    /// cref="Double"/> components.</para></remarks>

    [Serializable]
    public struct RectI: IEquatable<RectI> {
        #region RectI(Int32, Int32, Int32, Int32)

        /// <overloads>
        /// Initializes a new instance of the <see cref="RectI"/> structure.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="RectI"/> structure with the specified
        /// <see cref="Int32"/> coordinates and dimensions.</summary>
        /// <param name="x">
        /// The smallest <see cref="X"/> coordinate within the <see cref="RectI"/>.</param>
        /// <param name="y">
        /// The smallest <see cref="Y"/> coordinate within the <see cref="RectI"/>.</param>
        /// <param name="width">
        /// The <see cref="Width"/> of the <see cref="RectI"/>.</param>
        /// <param name="height">
        /// The <see cref="Height"/> of the <see cref="RectI"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is less than zero.</exception>

        public RectI(int x, int y, int width, int height) {
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
        #region RectI(PointI, SizeI)

        /// <summary>
        /// Initializes a new instance of the <see cref="RectI"/> structure with the specified
        /// <see cref="PointI"/> coordinates and <see cref="SizeI"/> dimensions.</summary>
        /// <param name="location">
        /// The <see cref="Location"/> of the <see cref="RectI"/>.</param>
        /// <param name="size">
        /// The <see cref="Size"/> of the <see cref="RectI"/>.</param>

        public RectI(PointI location, SizeI size) {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="RectI"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="RectI"/> instance that was created with the default
        /// constructor.</remarks>

        public static readonly RectI Empty = new RectI();

        #endregion
        #region Public Properties
        #region X

        /// <summary>
        /// The smallest x-coordinate within the <see cref="RectI"/>.</summary>
        /// <remarks>
        /// <b>X</b> is the x-coordinate of the left edge of the <see cref="RectI"/>, assuming that
        /// x-coordinates increase to the right.</remarks>

        public readonly int X;

        #endregion
        #region Y

        /// <summary>
        /// The smallest y-coordinate within the <see cref="RectI"/>.</summary>
        /// <remarks>
        /// <b>Y</b> is the y-coordinate of the top edge of the <see cref="RectI"/>, assuming that
        /// y-coordinates increase downward.</remarks>

        public readonly int Y;

        #endregion
        #region Width

        /// <summary>
        /// The horizontal extension of the <see cref="RectI"/>.</summary>
        /// <remarks>
        /// <b>Width</b> is never less than zero.</remarks>

        public readonly int Width;

        #endregion
        #region Height

        /// <summary>
        /// The vertical extension of the <see cref="RectI"/>.</summary>
        /// <remarks>
        /// <b>Height</b> is never less than zero.</remarks>

        public readonly int Height;

        #endregion
        #region Location

        /// <summary>
        /// Gets the coordinates of the upper-left corner of the <see cref="RectI"/>.</summary>
        /// <value>
        /// A <see cref="PointI"/> containing the <see cref="X"/> and <see cref="Y"/> coordinates.
        /// </value>
        /// <remarks>
        /// <b>Location</b> holds the smallest x- and y-coordinates that are contained within the
        /// <see cref="RectI"/>.</remarks>

        public PointI Location {
            get { return new PointI(X, Y); }
        }

        #endregion
        #region Size

        /// <summary>
        /// Gets the extension of the <see cref="RectI"/>.</summary>
        /// <value>
        /// A <see cref="SizeI"/> containing the <see cref="Width"/> and <see cref="Height"/>
        /// dimensions.</value>
        /// <remarks>
        /// The <see cref="RectI"/> covers the area beginning at <see cref="Location"/> and
        /// extending over <see cref="Size"/> with increasing x- and y-coordinates, but excluding
        /// the greatest coordinates in each dimension.</remarks>

        public SizeI Size {
            get { return new SizeI(Width, Height); }
        }

        #endregion
        #region Left

        /// <summary>
        /// Gets the x-coordinate of the left edge of the <see cref="RectI"/>.</summary>
        /// <value>
        /// The <see cref="X"/> coordinate of the <see cref="RectI"/>.</value>
        /// <remarks>
        /// <b>Left</b> is the smallest y-coordinate that is contained within the <see
        /// cref="RectI"/>.</remarks>

        public int Left {
            get { return X; }
        }

        #endregion
        #region Top

        /// <summary>
        /// Gets the y-coordinate of the top edge of the <see cref="RectI"/>.</summary>
        /// <value>
        /// The <see cref="Y"/> coordinate of the <see cref="RectI"/>.</value>
        /// <remarks>
        /// <b>Top</b> is the smallest y-coordinate that is contained within the <see
        /// cref="RectI"/>.</remarks>

        public int Top {
            get { return Y; }
        }

        #endregion
        #region Right

        /// <summary>
        /// Gets the x-coordinate just beyond the right edge of the <see cref="RectI"/>.</summary>
        /// <value>
        /// The sum of the <see cref="X"/> coordinate and the <see cref="Width"/> dimension.</value>
        /// <remarks>
        /// <b>Right</b> is the smallest x-coordinate that is not smaller than <see cref="Left"/>
        /// and not contained in the <see cref="RectI"/>.</remarks>

        public int Right {
            get { return X + Width; }
        }

        #endregion
        #region Bottom

        /// <summary>
        /// Gets the y-coordinate just beyond the bottom edge of the <see cref="RectI"/>.</summary>
        /// <value>
        /// The sum of the <see cref="Y"/> coordinate and the <see cref="Height"/> dimension.
        /// </value>
        /// <remarks>
        /// <b>Bottom</b> is the smallest y-coordinate that is not smaller than <see cref="Top"/>
        /// and not contained in the <see cref="RectI"/>.</remarks>

        public int Bottom {
            get { return Y + Height; }
        }

        #endregion
        #region TopLeft

        /// <summary>
        /// Gets the upper-left corner of the <see cref="RectI"/>.</summary>
        /// <value>
        /// A <see cref="PointI"/> whose <see cref="PointI.X"/> coordinate equals <see
        /// cref="Left"/> and whose <see cref="PointI.Y"/> coordinate equals <see cref="Top"/>.
        /// </value>
        /// <remarks>
        /// <b>TopLeft</b> returns the same value as <see cref="Location"/>.</remarks>
        
        public PointI TopLeft {
            get { return new PointI(X, Y); }
        }

        #endregion
        #region TopRight

        /// <summary>
        /// Gets the upper-right corner of the <see cref="RectI"/>.</summary>
        /// <value>
        /// A <see cref="PointI"/> whose <see cref="PointI.X"/> coordinate equals <see
        /// cref="Right"/> and whose <see cref="PointI.Y"/> coordinate equals <see cref="Top"/>.
        /// </value>

        public PointI TopRight {
            get { return new PointI(X + Width, Y); }
        }

        #endregion
        #region BottomLeft

        /// <summary>
        /// Gets the lower-left corner of the <see cref="RectI"/>.</summary>
        /// <value>
        /// A <see cref="PointI"/> whose <see cref="PointI.X"/> coordinate equals <see
        /// cref="Left"/> and whose <see cref="PointI.Y"/> coordinate equals <see cref="Bottom"/>.
        /// </value>

        public PointI BottomLeft {
            get { return new PointI(X, Y + Height); }
        }

        #endregion
        #region BottomRight

        /// <summary>
        /// Gets the lower-right corner of the <see cref="RectI"/>.</summary>
        /// <value>
        /// A <see cref="PointI"/> whose <see cref="PointI.X"/> coordinate equals <see
        /// cref="Right"/> and whose <see cref="PointI.Y"/> coordinate equals <see cref="Bottom"/>.
        /// </value>

        public PointI BottomRight {
            get { return new PointI(X + Width, Y + Height); }
        }

        #endregion
        #endregion
        #region Public Methods
        #region Circumscribe

        /// <summary>
        /// Circumscribes a <see cref="RectI"/> around the specified <see cref="PointI"/>
        /// coordinates.</summary>
        /// <param name="points">
        /// An <see cref="Array"/> containing the <see cref="PointI"/> coordinates whose bounds to
        /// determine.</param>
        /// <returns>
        /// The smallest <see cref="RectI"/> that contains all specified <paramref name="points"/>.
        /// </returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="points"/> is a null reference or an empty array.</exception>
        /// <remarks>
        /// <b>Circumscribe</b> returns a <see cref="RectI"/> whose <see cref="Right"/> and <see
        /// cref="Bottom"/> coordinates are one greater than the greatest x- and y-coordinates found
        /// among the specified <paramref name="points"/>. This ensures that <see cref="Contains"/>
        /// succeeds for all <paramref name="points"/>.</remarks>

        public static RectI Circumscribe(params PointI[] points) {
            if (points == null || points.Length == 0)
                ThrowHelper.ThrowArgumentNullOrEmptyException("points");

            int x0 = Int32.MaxValue, y0 = Int32.MaxValue;
            int x1 = Int32.MinValue, y1 = Int32.MinValue;

            foreach (PointI point in points) {
                if (x0 > point.X) x0 = point.X;
                if (y0 > point.Y) y0 = point.Y;
                if (x1 < point.X) x1 = point.X;
                if (y1 < point.Y) y1 = point.Y;
            }

            return new RectI(x0, y0, x1 - x0 + 1, y1 - y0 + 1);
        }

        #endregion
        #region Contains(Int32, Int32)

        /// <overloads>
        /// Indicates whether the <see cref="RectI"/> contains the specified coordinates.
        /// </overloads>
        /// <summary>
        /// Indicates whether the <see cref="RectI"/> contains the specified <see cref="Int32"/>
        /// coordinates.</summary>
        /// <param name="x">
        /// The x-coordinate to examine.</param>
        /// <param name="y">
        /// The y-coordinate to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectI"/> contains the specified <paramref name="x"/> and
        /// <paramref name="y"/> coordinates; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> assumes that the <see cref="RectI"/> does not contain the <see
        /// cref="Right"/> and <see cref="Bottom"/> coordinates.</remarks>

        public bool Contains(int x, int y) {
            return (x >= X && y >= Y && x < X + Width && y < Y + Height);
        }

        #endregion
        #region Contains(PointI)

        /// <summary>
        /// Indicates whether the <see cref="RectI"/> contains the specified <see cref="PointI"/>
        /// coordinates.</summary>
        /// <param name="point">
        /// The <see cref="PointI"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectI"/> contains the specified <paramref name="point"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> assumes that the <see cref="RectI"/> does not contain the <see
        /// cref="Right"/> and <see cref="Bottom"/> coordinates.</remarks>

        public bool Contains(PointI point) {
            return Contains(point.X, point.Y);
        }

        #endregion
        #region Contains(RectI)

        /// <summary>
        /// Indicates whether the <see cref="RectI"/> contains the specified rectangle.</summary>
        /// <param name="rect">
        /// The <see cref="RectI"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectI"/> entirely contains the specified <paramref
        /// name="rect"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> returns <c>true</c> even if both <see cref="RectI"/> instances have a
        /// <see cref="Width"/> or <see cref="Height"/> of zero, provided they have the same <see
        /// cref="Location"/> in the corresponding dimension.</remarks>

        public bool Contains(RectI rect) {

            return (rect.X >= X && rect.Y >= Y
                && rect.X + rect.Width <= X + Width
                && rect.Y + rect.Height <= Y + Height);
        }

        #endregion
        #region ContainsClosed(Int32, Int32)

        /// <overloads>
        /// Indicates whether the <see cref="RectI"/> contains the specified coordinates, including
        /// <see cref="Right"/> and <see cref="Bottom"/>. </overloads>
        /// <summary>
        /// Indicates whether the <see cref="RectI"/> contains the specified <see cref="Int32"/>
        /// coordinates, including <see cref="Right"/> and <see cref="Bottom"/>.</summary>
        /// <param name="x">
        /// The x-coordinate to examine.</param>
        /// <param name="y">
        /// The y-coordinate to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectI"/> contains the specified <paramref name="x"/> and
        /// <paramref name="y"/> coordinates, including <see cref="Right"/> and <see
        /// cref="Bottom"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>ContainsClosed</b> assumes that the <see cref="RectI"/> contains the <see
        /// cref="Right"/> and <see cref="Bottom"/> coordinates, emulating <see cref="RectD"/> and
        /// <see cref="RectF"/> behavior.</remarks>

        public bool ContainsClosed(int x, int y) {
            return (x >= X && y >= Y && x <= X + Width && y <= Y + Height);
        }

        #endregion
        #region ContainsClosed(PointI)

        /// <summary>
        /// Indicates whether the <see cref="RectI"/> contains the specified <see cref="PointI"/>
        /// coordinates, including <see cref="Right"/> and <see cref="Bottom"/>.</summary>
        /// <param name="point">
        /// The <see cref="PointI"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectI"/> contains the specified <paramref name="point"/>,
        /// including <see cref="Right"/> and <see cref="Bottom"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <b>ContainsClosed</b> assumes that the <see cref="RectI"/> contains the <see
        /// cref="Right"/> and <see cref="Bottom"/> coordinates, emulating <see cref="RectD"/> and
        /// <see cref="RectF"/> behavior.</remarks>

        public bool ContainsClosed(PointI point) {
            return ContainsClosed(point.X, point.Y);
        }

        #endregion
        #region GetDistanceVector

        /// <summary>
        /// Finds the distance vector from the specified <see cref="PointI"/> coordinates to the
        /// nearest edges of the <see cref="RectI"/>.</summary>
        /// <param name="q">
        /// The <see cref="PointI"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="PointI"/> vector indicating the distance of each <paramref name="q"/> 
        /// coordinate from the nearest corresponding edge of the <see cref="RectI"/>.</returns>
        /// <remarks><para>
        /// <b>GetDistanceVector</b> defines the components of the returned <see cref="PointI"/>
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
        /// coordinate lies within the corresponding <see cref="RectI"/> extension. Otherwise, the
        /// component’s absolute value indicates the coordinate’s distance from the nearest <see
        /// cref="RectI"/> edge, and its sign indicates that edge itself.</para></remarks>

        public PointI GetDistanceVector(PointI q) {
            int qx = q.X - X, qy = q.Y - Y;

            int x = (qx < 0 ? qx : qx > Width ? qx - Width : 0);
            int y = (qy < 0 ? qy : qy > Height ? qy - Height : 0);

            return new PointI(x, y);
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="RectI"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the values of the <see cref="X"/>, <see cref="Y"/>, <see
        /// cref="Width"/>, and <see cref="Height"/> properties.</remarks>

        public override int GetHashCode() {
            unchecked { return X ^ Y ^ Width ^ Height; }
        }

        #endregion
        #region Intersect

        /// <summary>
        /// Intersects the <see cref="RectI"/> with the specified rectangle.</summary>
        /// <param name="rect">
        /// The <see cref="RectI"/> to intersect with this instance.</param>
        /// <param name="intersection">
        /// On success, the intersection of the <see cref="RectI"/> and the specified <paramref
        /// name="rect"/>; otherwise, <see cref="Empty"/>.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectI"/> intersects with the specified <paramref
        /// name="rect"/>; otherwise, <c>false</c>.</returns>

        public bool Intersect(RectI rect, out RectI intersection) {

            int x = Math.Max(X, rect.X);
            int y = Math.Max(Y, rect.Y);
            int width = Math.Min(X + Width, rect.X + rect.Width) - x;
            int height = Math.Min(Y + Height, rect.Y + rect.Height) - y;

            if (height < 0 || width < 0) {
                intersection = Empty;
                return false;
            } else {
                intersection = new RectI(x, y, width, height);
                return true;
            }
        }

        #endregion
        #region IntersectsWith

        /// <summary>
        /// Determines whether the <see cref="RectI"/> intersects with the specified rectangle.
        /// </summary>
        /// <param name="rect">
        /// The <see cref="RectI"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="RectI"/> intersects with the specified <paramref
        /// name="rect"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>IntersectsWith</b> always returns <c>false</c> if both <see cref="RectI"/> instances
        /// have a <see cref="Width"/> or <see cref="Height"/> of zero, even if they have the same
        /// <see cref="Location"/> in the corresponding dimension.</remarks>

        public bool IntersectsWith(RectI rect) {

            return (rect.X + rect.Width > X && rect.X < X + Width
                && rect.Y + rect.Height > Y && rect.Y < Y + Height);
        }

        #endregion
        #region Locate

        /// <summary>
        /// Determines the location of the specified <see cref="PointI"/> coordinates relative to
        /// the <see cref="RectI"/>.</summary>
        /// <param name="q">
        /// The <see cref="PointI"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="RectLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="RectI"/>.</returns>
        /// <remarks><para>
        /// <b>Locate</b> never returns <see cref="RectLocation.None"/>, and always returns a
        /// bitwise combination of an <b>…X</b> and a <b>…Y</b> value.
        /// </para><para>
        /// <b>Locate</b> assumes that the <see cref="RectI"/> does not contain the <see
        /// cref="Right"/> and <see cref="Bottom"/> coordinates.</para></remarks>

        public RectLocation Locate(PointI q) {
            int qx = q.X - X, qy = q.Y - Y;

            RectLocation x = (qx < 0 ? RectLocation.BeforeX :
                (qx == 0 ? RectLocation.StartX :
                (qx < Width - 1 ? RectLocation.InsideX :
                (qx == Width - 1 ? RectLocation.EndX : RectLocation.AfterX))));

            RectLocation y = (qy < 0 ? RectLocation.BeforeY :
                (qy == 0 ? RectLocation.StartY :
                (qy < Height - 1 ? RectLocation.InsideY :
                (qy == Height - 1 ? RectLocation.EndY : RectLocation.AfterY))));

            return x | y;
        }

        #endregion
        #region LocateClosed

        /// <summary>
        /// Determines the location of the specified <see cref="PointI"/> coordinates relative to
        /// the <see cref="RectI"/>, including <see cref="Right"/> and <see cref="Bottom"/>.
        /// </summary>
        /// <param name="q">
        /// The <see cref="PointI"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="RectLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="RectI"/>.</returns>
        /// <remarks>
        /// <b>LocateClosed</b> is identical with <see cref="Locate"/> but assumes that the <see
        /// cref="RectI"/> contains the <see cref="Right"/> and <see cref="Bottom"/> coordinates,
        /// emulating <see cref="RectD"/> and <see cref="RectF"/> behavior.</remarks>

        public RectLocation LocateClosed(PointI q) {
            int qx = q.X - X, qy = q.Y - Y;

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
        #region Offset(Int32, Int32)

        /// <overloads>
        /// Moves the <see cref="RectI"/> by the specified offset.</overloads>
        /// <summary>
        /// Moves the <see cref="RectI"/> by the specified <see cref="Int32"/> values.</summary>
        /// <param name="x">
        /// The horizontal offset applied to the <see cref="RectI"/>.</param>
        /// <param name="y">
        /// The vertical offset applied to the <see cref="RectI"/>.</param>
        /// <returns>
        /// A new <see cref="RectI"/> with the same <see cref="Size"/> as this instance, and whose
        /// <see cref="X"/> and <see cref="Y"/> coordinates are offset by the specified <paramref
        /// name="x"/> and <paramref name="y"/> values.</returns>

        public RectI Offset(int x, int y) {
            return new RectI(X + x, Y + y, Width, Height);
        }

        #endregion
        #region Offset(PointI)

        /// <summary>
        /// Moves the <see cref="RectI"/> by the specified <see cref="PointI"/> vector.</summary>
        /// <param name="vector">
        /// A <see cref="PointI"/> value whose components define the horizontal and vertical offset
        /// applied to the <see cref="RectI"/>.</param>
        /// <returns>
        /// A new <see cref="RectI"/> with the same <see cref="Size"/> as this instance, and whose
        /// <see cref="Location"/> is offset by the specified <paramref name="vector"/>.</returns>

        public RectI Offset(PointI vector) {
            return new RectI(Location + vector, Size);
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="RectI"/>.</summary>
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
        /// Determines the union of the <see cref="RectI"/> and the specified rectangle.</summary>
        /// <param name="rect">
        /// The <see cref="RectI"/> to combine with this instance.</param>
        /// <returns>
        /// A <see cref="RectI"/> that contains the union of the specified <paramref name="rect"/>
        /// and this instance.</returns>

        public RectI Union(RectI rect) {

            int x = Math.Min(X, rect.X);
            int y = Math.Min(Y, rect.Y);
            int width = Math.Max(X + Width, rect.X + rect.Width) - x;
            int height = Math.Max(Y + Height, rect.Y + rect.Height) - y;

            return new RectI(x, y, width, height);
        }

        #endregion
        #endregion
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="RectI"/> instances have the same value.</summary>
        /// <param name="a">
        /// The first <see cref="RectI"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="RectI"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(RectI)"/> method to test the two <see
        /// cref="RectI"/> instances for value equality.</remarks>

        public static bool operator ==(RectI a, RectI b) {
            return a.Equals(b);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="RectI"/> instances have different values.</summary>
        /// <param name="a">
        /// The first <see cref="RectI"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="RectI"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different from the value of
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(RectI)"/> method to test the two <see
        /// cref="RectI"/> instances for value inequality.</remarks>

        public static bool operator !=(RectI a, RectI b) {
            return !a.Equals(b);
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="RectI"/> instances have the same value.</overloads>
        /// <summary>
        /// Determines whether this <see cref="RectI"/> instance and a specified object, which must
        /// be a <see cref="RectI"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="RectI"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="RectI"/> instance and its
        /// value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="RectI"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(RectI)"/> overload to test
        /// the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is RectI))
                return false;

            return Equals((RectI) obj);
        }

        #endregion
        #region Equals(RectI)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="RectI"/> have the same
        /// value.</summary>
        /// <param name="rect">
        /// A <see cref="RectI"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="rect"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="X"/>, <see cref="Y"/>, <see
        /// cref="Width"/>, and <see cref="Height"/> properties of the two <see cref="RectI"/>
        /// instances to test for value equality.</remarks>

        public bool Equals(RectI rect) {
            return (X == rect.X && Y == rect.Y
                && Width == rect.Width && Height == rect.Height);
        }

        #endregion
        #region Equals(RectI, RectI)

        /// <summary>
        /// Determines whether two specified <see cref="RectI"/> instances have the same value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="RectI"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="RectI"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(RectI)"/> overload to test the
        /// two <see cref="RectI"/> instances for value equality.</remarks>

        public static bool Equals(RectI a, RectI b) {
            return a.Equals(b);
        }

        #endregion
        #endregion
    }
}
