using System;
using System.ComponentModel;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a regular polygon.</summary>
    /// <remarks><para>
    /// <b>RegularPolygon</b> represents a regular polygon with three or more sides of a given
    /// length, and with one of the orientations defined by <see cref="PolygonOrientation"/>. The
    /// vertex coordinates of all polygons are symmetrical across the vertical axis, and those of
    /// polygons with an even number of sides are also symmetrical across the horizontal axis.
    /// </para><para>
    /// Upon construction, <b>RegularPolygon</b> calculates the radii of the inscribed and
    /// circumscribed circles, the coordinates of all vertices, and the minimum bounding rectangle.
    /// All property values are immutable once defined. Methods that seem to change the side length
    /// of a given <b>RegularPolygon</b> return a new instance instead, similar to the <see
    /// cref="String"/> class.</para></remarks>

    [Serializable]
    public class RegularPolygon {
        #region RegularPolygon(Double, Int32, PolygonOrientation)

        /// <overloads>
        /// Initializes a new instance of the <see cref="RegularPolygon"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="RegularPolygon"/> class with the specified
        /// side length, number of sides, and orientation.</summary>
        /// <param name="length">
        /// The length of each side of the <see cref="RegularPolygon"/>.</param>
        /// <param name="sides">
        /// The number of sides of the <see cref="RegularPolygon"/>.</param>
        /// <param name="orientation">
        /// The orientation of the <see cref="RegularPolygon"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="length"/> is equal to or less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="sides"/> is less than three.</para></exception>
        /// <exception cref="InvalidEnumArgumentException">
        /// <paramref name="orientation"/> is not a valid <see cref="PolygonOrientation"/> value.
        /// </exception>
        /// <remarks>
        /// The <see cref="VertexNeighbors"/> property is initialized to <c>false</c>.</remarks>

        public RegularPolygon(double length, int sides, PolygonOrientation orientation):
            this(length, sides, orientation, false) { }

        #endregion
        #region RegularPolygon(Double, Int32, PolygonOrientation, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="RegularPolygon"/> class with the specified
        /// side length, number of sides, orientation, and vertex neighbors flag.</summary>
        /// <param name="length">
        /// The length of each side of the <see cref="RegularPolygon"/>.</param>
        /// <param name="sides">
        /// The number of sides of the <see cref="RegularPolygon"/>.</param>
        /// <param name="orientation">
        /// The orientation of the <see cref="RegularPolygon"/>.</param>
        /// <param name="vertexNeighbors">
        ///  <c>true</c> if <see cref="RegularPolygon"/> shapes that share only a common vertex are
        /// considered neighbors; otherwise, <c>false</c>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="length"/> is equal to or less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="sides"/> is less than three.
        /// </para><para>-or-</para><para>
        /// <paramref name="vertexNeighbors"/> is <c>true</c>, and <paramref name="sides"/> is
        /// greater than four.</para></exception>
        /// <exception cref="InvalidEnumArgumentException">
        /// <paramref name="orientation"/> is not a valid <see cref="PolygonOrientation"/> value.
        /// </exception>

        public RegularPolygon(double length, int sides,
            PolygonOrientation orientation, bool vertexNeighbors) {

            if (length <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "length", length, Strings.ArgumentNotPositive);

            if (sides < 3)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "sides", sides, Strings.ArgumentLessValue, 3);

            if (vertexNeighbors && sides > 4)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "vertexNeighbors", vertexNeighbors, Strings.ArgumentTrue);

            Length = length;
            Sides = sides;
            Orientation = orientation;
            VertexNeighbors = vertexNeighbors;

            // compute maximum neighbors on edges and vertices
            Connectivity = (vertexNeighbors ? 2 * sides : sides);

            // determine whether a top connection exists
            HasTopIndex = (vertexNeighbors || (sides % 2 == 0 ?
                orientation == PolygonOrientation.OnEdge :
                orientation == PolygonOrientation.OnVertex));

            // compute angle of one segment between vertices
            double angle, segment = (2.0 * Math.PI) / sides;

            // compute radii of circumscribed and inscribed circles
            OuterRadius = length / (2.0 * Math.Sin(segment / 2.0));
            InnerRadius = OuterRadius * Math.Cos(segment / 2.0);

            // compute angle of first vertex and check orientation
            switch (orientation) {

                case PolygonOrientation.OnEdge:
                    angle = (sides % 2 == 0 ? segment : 0.0);
                    break;

                case PolygonOrientation.OnVertex:
                    angle = (sides % 2 == 0 ? 0.0 : segment);
                    break;

                default:
                    ThrowHelper.ThrowInvalidEnumArgumentException(
                        "orientation", (int) orientation, typeof(PolygonOrientation));
                    angle = 0.0;
                    break;
            }

            // halve angle and rotate 90° counter-clockwise
            angle = (angle - Math.PI) / 2.0;

            // compute and store vertex coordinates around center
            Vertices = new PointD[sides];
            for (int i = 0; i < sides; i++, angle += segment)
                Vertices[i] = new PointD(
                    OuterRadius * Math.Cos(angle), OuterRadius * Math.Sin(angle));

            // compute and store circumscribed rectangle
            Bounds = RectD.Circumscribe(Vertices);
        }

        #endregion
        #region Public Properties
        #region Bounds

        /// <summary>
        /// The bounding rectangle that is circumscribed around the <see cref="RegularPolygon"/>.
        /// </summary>
        /// <remarks><para>
        /// <b>Bounds</b> always has a positive <see cref="RectD.Size"/> in both dimensions. The
        /// area covered by <b>Bounds</b> is horizontally centered on the <see
        /// cref="RegularPolygon"/>, and also vertically for an even number of <see cref="Sides"/>.
        /// </para><para>
        /// All coordinates are relative to the center of the polygon and use drawing orientation
        /// rather than mathematical orientation. That is, y-coordinates increase downward and both
        /// <see cref="RectD.Location"/> coordinates are negative.</para></remarks>

        public readonly RectD Bounds;

        #endregion
        #region Connectivity

        /// <summary>
        /// The maximum number of neighbors for the <see cref="RegularPolygon"/>.</summary>
        /// <remarks><para>
        /// <b>Connectivity</b> equals the number of <see cref="Sides"/> if <see
        /// cref="VertexNeighbors"/> is <c>false</c>; otherwise, twice that number.
        /// </para><para>
        /// <b>Connectivity</b> applies to regular grids of adjacent identical <see
        /// cref="RegularPolygon"/> shapes, such as the ones represented by <see
        /// cref="PolygonGrid"/>.
        /// </para><para>
        /// <b>Connectivity</b> determines the index range used by the various index conversion
        /// methods, and also the <see cref="PolygonGrid.Connectivity"/> of a <see
        /// cref="PolygonGrid"/> based on this <see cref="RegularPolygon"/>.</para></remarks>

        public readonly int Connectivity;

        #endregion
        #region HasTopIndex

        /// <summary>
        /// <c>true</c> if index zero within the <see cref="Connectivity"/> range corresponds to the
        /// topmost edge or vertex of the <see cref="RegularPolygon"/>; <c>false</c> if this index
        /// corresponds to the edge to the right of the topmost vertex.</summary>
        /// <remarks><para>
        /// <b>HasTopIndex</b> is <c>true</c> if one of the following conditions holds:
        /// </para><list type="bullet"><item>
        /// <see cref="VertexNeighbors"/> is <c>true</c>.
        /// </item><item>
        /// <see cref="Orientation"/> equals <see cref="PolygonOrientation.OnEdge"/>, and <see
        /// cref="Sides"/> is divisible by two.
        /// </item><item>
        /// <see cref="Orientation"/> equals <see cref="PolygonOrientation.OnVertex"/>, and <see
        /// cref="Sides"/> is not divisible by two.</item></list></remarks>

        public readonly bool HasTopIndex;

        #endregion
        #region InnerRadius

        /// <summary>
        /// The radius of the circle that is inscribed within the <see cref="RegularPolygon"/>.
        /// </summary>
        /// <remarks>
        /// <b>InnerRadius</b> is always greater than zero.</remarks>

        public readonly double InnerRadius;

        #endregion
        #region Length

        /// <summary>
        /// The length of each side of the <see cref="RegularPolygon"/>.</summary>
        /// <remarks>
        /// <b>Length</b> is always greater than zero.</remarks>

        public readonly double Length;

        #endregion
        #region Orientation

        /// <summary>
        /// The orientation of the <see cref="RegularPolygon"/>.</summary>>

        public readonly PolygonOrientation Orientation;

        #endregion
        #region OuterRadius

        /// <summary>
        /// The radius of the circle that is circumscribed around the <see cref="RegularPolygon"/>.
        /// </summary>
        /// <remarks>
        /// <b>OuterRadius</b> is always greater than zero.</remarks>

        public readonly double OuterRadius;

        #endregion
        #region Sides

        /// <summary>
        /// The number of sides of the <see cref="RegularPolygon"/>.</summary>
        /// <remarks>
        /// <b>Sides</b> is always greater than or equal to three.</remarks>

        public readonly int Sides;

        #endregion
        #region VertexNeighbors

        /// <summary>
        /// <c>true</c> if <see cref="RegularPolygon"/> shapes that share only a common vertex are
        /// considered neighbors; otherwise, <c>false</c>.</summary>
        /// <remarks><para>
        /// <b>VertexNeighbors</b> applies to regular grids of adjacent identical <see
        /// cref="RegularPolygon"/> shapes, such as the ones represented by <see
        /// cref="PolygonGrid"/>.
        /// </para><para>
        /// <b>VertexNeighbors</b> always returns <c>false</c> if <see cref="Sides"/> is greater
        /// than four, as inner angles of more than 90 degrees prevent the elements of a <see
        /// cref="PolygonGrid"/> from sharing a vertex without also sharing an edge.
        /// </para><para>
        /// <see cref="RegularPolygon"/> shapes that share a common edge are always considered
        /// neighbors, regardless of the value of <b>VertexNeighbors</b>. The maximum number of
        /// shared edges, and possibly vertices, equals the number of <see cref="Sides"/>.
        /// </para><para>
        /// <b>VertexNeighbors</b> and <see cref="Sides"/> determine the <see cref="Connectivity"/>
        /// of the <see cref="RegularPolygon"/> which in turn determines the index range used by
        /// <see cref="AngleToIndex"/> and <see cref="IndexToAngle"/>.</para></remarks>

        public readonly bool VertexNeighbors;

        #endregion
        #region Vertices

        /// <summary>
        /// The coordinates of all vertices the <see cref="RegularPolygon"/>.</summary>
        /// <remarks><para>
        /// <b>Vertices</b> holds an <see cref="Array"/> of <see cref="PointD"/> values indicating
        /// the coordinates of all vertices of the <see cref="RegularPolygon"/>, starting with the
        /// topmost vertex or with the right-hand one of two topmost vertices, and continuing
        /// clockwise. <b>Vertices</b> always contains <see cref="Sides"/> elements.
        /// </para><para>
        /// All coordinates are relative to the center of the polygon and use drawing orientation
        /// rather than mathematical orientation. That is, y-coordinates increase downward and the
        /// first <b>Vertices</b> element has a negative y-coordinate.</para></remarks>

        public readonly PointD[] Vertices;

        #endregion
        #endregion
        #region Public Methods
        #region AngleToIndex

        /// <summary>
        /// Converts the specified central angle to the index of the corresponding edge or vertex.
        /// </summary>
        /// <param name="angle">
        /// The central angle to convert, in degrees. This value is taken <see
        /// cref="Fortran.Modulo"/> 360 degrees, and may therefore be outside the interval [0, 360).
        /// </param>
        /// <returns>
        /// The zero-based index of the edge or vertex at the specified <paramref name="angle"/>.
        /// </returns>
        /// <remarks><para>
        /// The specified <paramref name="angle"/> is measured from the center of the <see
        /// cref="RegularPolygon"/>, and increases clockwise from the right-hand side of the x-axis.
        /// </para><para>
        /// If <see cref="VertexNeighbors"/> is <c>false</c>, the returned index enumerates all
        /// edges in clockwise direction. Counting starts at the topmost edge if <see
        /// cref="HasTopIndex"/> is <c>true</c>, and with the edge to the right of the topmost
        /// vertex otherwise.
        /// </para><para>
        /// If <see cref="VertexNeighbors"/> is <c>true</c>, the returned index enumerates all edges
        /// and vertices in an alternating sequence. Counting starts with the topmost edge for <see
        /// cref="PolygonOrientation.OnEdge"/> orientation and with the topmost vertex otherwise,
        /// continuing clockwise.
        /// </para><para>
        /// Valid indices range from zero to <see cref="Connectivity"/> less one. The 360 degrees of
        /// a full rotation around the central point are evenly divided among this range so that
        /// each index corresponds to an equal arc. If <see cref="VertexNeighbors"/> is <c>true</c>,
        /// the arcs that are mapped to edge indices cover only the central half of each edge. The
        /// arcs covering the outer parts are mapped to vertex indices instead.</para></remarks>

        public int AngleToIndex(double angle) {
            double segment = 360.0 / Connectivity;
            if (HasTopIndex) angle += segment / 2.0;
            angle = Fortran.Modulo(angle + 90.0, 360.0);
            return (int) (angle / segment);
        }

        #endregion
        #region Circumscribe(Double)

        /// <overloads>
        /// Creates a copy of the <see cref="RegularPolygon"/> that is circumscribed around the
        /// specified circle or rectangle.</overloads>
        /// <summary>
        /// Creates a copy of the <see cref="RegularPolygon"/> that is circumscribed around the
        /// specified circle.</summary>
        /// <param name="radius">
        /// The radius of the circle around which to circumscribe the <see cref="RegularPolygon"/>.
        /// </param>
        /// <returns>
        /// A copy of this <see cref="RegularPolygon"/> instance whose <see cref="InnerRadius"/>
        /// equals the specified <paramref name="radius"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="radius"/> is equal to or less than zero.</exception>
        /// <remarks>
        /// <b>Circumscribe</b> returns the current <see cref="RegularPolygon"/> if the specified
        /// <paramref name="radius"/> equals the current <see cref="InnerRadius"/>.</remarks>

        public RegularPolygon Circumscribe(double radius) {
            if (radius == InnerRadius)
                return this;

            if (radius <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "radius", radius, Strings.ArgumentNotPositive);

            double newLength = 2.0 * radius * Math.Tan(Math.PI / Sides);
            return new RegularPolygon(newLength, Sides, Orientation, VertexNeighbors);
        }

        #endregion
        #region Circumscribe(Double, Double)

        /// <summary>
        /// Creates a copy of the <see cref="RegularPolygon"/> that is circumscribed around the
        /// specified rectangle.</summary>
        /// <param name="width">
        /// The width of the rectangle around which to circumscribe the <see
        /// cref="RegularPolygon"/>.</param>
        /// <param name="height">
        /// The height of the rectangle around which to circumscribe the <see
        /// cref="RegularPolygon"/>.</param>
        /// <returns>
        /// A copy of this <see cref="RegularPolygon"/> instance whose area completely covers a
        /// rectangle with the specified <paramref name="width"/> and <paramref name="height"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is equal to or less than zero.
        /// </exception>
        /// <remarks>
        /// <b>Circumscribe</b> returns exact results for triangles and squares only. For other
        /// polygons, the returned <see cref="RegularPolygon"/> is an approximation that includes
        /// some excess space around an inscribed rectangle with the specified <paramref
        /// name="width"/> and <paramref name="height"/>.</remarks>

        public RegularPolygon Circumscribe(double width, double height) {
            if (width <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "width", width, Strings.ArgumentNotPositive);

            if (height <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "height", height, Strings.ArgumentNotPositive);

            double newLength;

            if (Sides == 3) {
                /*
                 * Triangle: Width is always equal to or smaller than one edge.
                 * The height of the triangle is at least the specified height
                 * plus an extra bit, depending on the width of the rectangle.
                 */

                const double angle = Math.PI / 3.0; // 60° angle
                double triangleHeight = height + width * Math.Tan(angle) / 2.0;
                double heightLength = triangleHeight / Math.Sin(angle);

                newLength = Math.Max(width, heightLength);
            }
            else if (Sides == 4) {
                /*
                 * Square: Lying squares trivially cover an inscribed rectangle.
                 * Standing squares have a diagonal that equals the sum of the
                 * specified width and height, hence the side length.
                 */

                if (Orientation == PolygonOrientation.OnEdge)
                    newLength = Math.Max(width, height);
                else
                    newLength = (width + height) / Math.Sqrt(2.0);
            }
            else {
                /*
                 * For any other polygons, we approximate the diameter of the
                 * inscribed circle by the diagonal of the specified rectangle.
                 * Then we circumscribe the polygon around this circle.
                 */

                double diameter = Math.Sqrt(width * width + height * height);
                newLength = diameter * Math.Tan(Math.PI / Sides);
            }

            return new RegularPolygon(newLength, Sides, Orientation, VertexNeighbors);
        }

        #endregion
        #region CompassToIndex

        /// <summary>
        /// Converts the specified <see cref="Compass"/> direction to the index of the corresponding
        /// edge or vertex.</summary>
        /// <param name="compass">
        /// The <see cref="Compass"/> direction to convert.</param>
        /// <returns>
        /// The zero-based index of the edge or vertex closest to the specified <paramref
        /// name="compass"/> direction.</returns>
        /// <remarks>
        /// <b>CompassToIndex</b> invokes <see cref="AngleToIndex"/> on the <see cref="Int32"/>
        /// value of the specified <paramref name="compass"/> direction, less 90 degress. Please
        /// refer to <see cref="AngleToIndex"/> for an explanation of index values.</remarks>

        public int CompassToIndex(Compass compass) {
            return AngleToIndex((int) compass - 90);
        }

        #endregion
        #region IndexToAngle

        /// <summary>
        /// Converts the specified index of an edge or vertex to the corresponding central angle.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of an edge or vertex. This value is taken <see
        /// cref="Fortran.Modulo"/> the current <see cref="Connectivity"/>, and may therefore be
        /// negative or greater than the maximum index.</param>
        /// <returns>
        /// The central angle, in degrees, of the edge or vertex with the specified <paramref
        /// name="index"/>. This value is always in the interval [0, 360).</returns>
        /// <remarks><para>
        /// If <see cref="VertexNeighbors"/> is <c>false</c>, the specified <paramref name="index"/>
        /// enumerates all edges in clockwise direction. Counting starts at the topmost edge if <see
        /// cref="HasTopIndex"/> is <c>true</c>, and with the edge to the right of the topmost
        /// vertex otherwise.
        /// </para><para>
        /// If <see cref="VertexNeighbors"/> is <c>true</c>, the specified <paramref name="index"/>
        /// enumerates all edges and vertices in an alternating sequence. Counting starts with the
        /// topmost edge for <see cref="PolygonOrientation.OnEdge"/> orientation and with the
        /// topmost vertex otherwise, continuing clockwise.
        /// </para><para>
        /// The returned angle is measured from the center of the <see cref="RegularPolygon"/>, and
        /// increases clockwise from the right-hand side of the x-axis.
        /// </para><para>
        /// Valid indices range from zero to <see cref="Connectivity"/> less one. The angle
        /// associated with each index is the angle from the central point to a vertex or to the
        /// middle of an edge, respectively.</para></remarks>

        public double IndexToAngle(int index) {
            double segment = 360.0 / Connectivity;
            double angle = Fortran.Modulo(index, Connectivity) * segment;
            if (!HasTopIndex) angle += segment / 2.0;
            return Fortran.Modulo(angle - 90.0, 360.0);
        }

        #endregion
        #region IndexToCompass

        /// <summary>
        /// Converts the specified index of an edge or vertex to the corresponding <see
        /// cref="Compass"/> direction.</summary>
        /// <param name="index">
        /// The zero-based index of an edge or vertex. This value is taken <see
        /// cref="Fortran.Modulo"/> the current <see cref="Connectivity"/>, and may therefore be
        /// negative or greater than the maximum index.</param>
        /// <returns>
        /// The <see cref="Compass"/> direction closest to the edge or vertex with the specified
        /// <paramref name="index"/>.</returns>
        /// <remarks>
        /// <b>IndexToCompass</b> first adds 90° to the result of <see cref="IndexToAngle"/> for the
        /// specified <paramref name="index"/>, and then returns the result of <see
        /// cref="Angle.DegreesToCompass"/> for that angle. Please refer to <see
        /// cref="IndexToAngle"/> for an explanation of index values.</remarks>

        public Compass IndexToCompass(int index) {
            double degrees = IndexToAngle(index) + 90;
            return Angle.DegreesToCompass(degrees);
        }

        #endregion
        #region Inflate

        /// <summary>
        /// Creates an inflated copy of the <see cref="RegularPolygon"/>.</summary>
        /// <param name="delta">
        /// The amount by which to inflate the <see cref="OuterRadius"/> of the <see
        /// cref="RegularPolygon"/>.</param>
        /// <returns>
        /// A copy of this <see cref="RegularPolygon"/> instance whose <see cref="OuterRadius"/> has
        /// been inflated by the specified <paramref name="delta"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="delta"/> is equal to or less than the negative value of <see
        /// cref="OuterRadius"/>.</exception>
        /// <remarks><para>
        /// <b>Inflate</b> returns the current <see cref="RegularPolygon"/> if the specified
        /// <paramref name="delta"/> is zero.
        /// </para><para>
        /// Otherwise, <b>Inflate</b> creates a new <see cref="RegularPolygon"/> whose <see
        /// cref="OuterRadius"/> equals the current value plus <paramref name="delta"/>. The ratio
        /// of the new <see cref="Length"/> to the current value is the same as the ratio of the new
        /// <see cref="OuterRadius"/> to the current value.
        /// </para><para>
        /// The specified <paramref name="delta"/> may be negative to decrease the size of the new
        /// <see cref="RegularPolygon"/> rather than increase it. The <see cref="Sides"/> and <see
        /// cref="Orientation"/> values of the new <see cref="RegularPolygon"/> are always identical
        /// to the current values.</para></remarks>

        public RegularPolygon Inflate(double delta) {
            if (delta == 0.0) return this;

            if (delta <= -OuterRadius)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "delta", delta, Strings.ArgumentNotGreaterValue, -OuterRadius);

            double newLength = Length * (OuterRadius + delta) / OuterRadius;
            return new RegularPolygon(newLength, Sides, Orientation, VertexNeighbors);
        }

        #endregion
        #region Inscribe(Double)

        /// <overloads>
        /// Creates a copy of the <see cref="RegularPolygon"/> that is inscribed in the specified
        /// circle or rectangle.</overloads>
        /// <summary>
        /// Creates a copy of the <see cref="RegularPolygon"/> that is inscribed in the specified
        /// circle.</summary>
        /// <param name="radius">
        /// The radius of the circle in which to inscribe the <see cref="RegularPolygon"/>.</param>
        /// <returns>
        /// A copy of this <see cref="RegularPolygon"/> instance whose <see cref="OuterRadius"/>
        /// equals the specified <paramref name="radius"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="radius"/> is equal to or less than zero.</exception>
        /// <remarks>
        /// <b>Inscribe</b> returns the current <see cref="RegularPolygon"/> if the specified
        /// <paramref name="radius"/> equals the current <see cref="OuterRadius"/>.</remarks>

        public RegularPolygon Inscribe(double radius) {
            if (radius == OuterRadius)
                return this;

            if (radius <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "radius", radius, Strings.ArgumentNotPositive);

            double newLength = 2.0 * radius * Math.Sin(Math.PI / Sides);
            return new RegularPolygon(newLength, Sides, Orientation, VertexNeighbors);
        }

        #endregion
        #region Inscribe(Double, Double)

        /// <summary>
        /// Creates a copy of the <see cref="RegularPolygon"/> that is inscribed in the specified
        /// rectangle.</summary>
        /// <param name="width">
        /// The width of the rectangle in which to inscribe the <see cref="RegularPolygon"/>.
        /// </param>
        /// <param name="height">
        /// The height of the rectangle in which to inscribe the <see cref="RegularPolygon"/>.
        /// </param>
        /// <returns>
        /// A copy of this <see cref="RegularPolygon"/> instance whose <see cref="Bounds"/> match
        /// exactly either the specified <paramref name="width"/> or <paramref name="height"/>, and
        /// do not exceed the corresponding value for the other dimension.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is equal to or less than zero.
        /// </exception>
        /// <remarks>
        /// <b>Inscribe</b> returns the current <see cref="RegularPolygon"/> if the specified
        /// <paramref name="width"/> and <paramref name="height"/> equal the <see
        /// cref="RectD.Size"/> of the current <see cref="Bounds"/>.</remarks>

        public RegularPolygon Inscribe(double width, double height) {
            if (width == Bounds.Width && height == Bounds.Height)
                return this;

            if (width <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "width", width, Strings.ArgumentNotPositive);

            if (height <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "height", height, Strings.ArgumentNotPositive);

            // compute angle of one segment between vertices
            double newLength, halfSegment = Math.PI / Sides;

            if (Sides % 4 == 0) {
                /*
                 * All edges of the circumscribed rectangle face either edges or vertices
                 * of the inscribed polygon. If edges, we use the diameter of the inscribed
                 * circle to determine the side length; otherwise, that of the circumcircle.
                 */

                double diameter = Math.Min(width, height);

                if (Orientation == PolygonOrientation.OnEdge)
                    newLength = diameter * Math.Tan(halfSegment);
                else
                    newLength = diameter * Math.Sin(halfSegment);
            }
            else if (Sides % 2 == 0) {
                /*
                 * One pair of edges of the circumscribed rectangle face edges of the
                 * inscribed polygon, and the other pair face vertices. We compute the
                 * side length resulting from the inscribed circle for one pair and from
                 * the circumcircle for the other, and then choose the smaller length.
                 */

                double innerDiameter, outerDiameter;

                if (Orientation == PolygonOrientation.OnEdge) {
                    innerDiameter = height; outerDiameter = width;
                } else {
                    innerDiameter = width; outerDiameter = height;
                }

                double outerLength = outerDiameter * Math.Sin(halfSegment);
                double innerLength = innerDiameter * Math.Tan(halfSegment);

                newLength = Math.Min(innerLength, outerLength);
            }
            else {
                /*
                 * RegularPolygon is symmetrical horizontally but not vertically. We base
                 * all calculations on the circumcircle and derive the distance between
                 * top and bottom vertex from the height of the rectangle, and twice the
                 * distance to the rightmost vertex from the width of the rectangle.
                 */

                double topAngle, segment = 2.0 * halfSegment;
                int rightIndex = (Sides / 4);

                if (Orientation == PolygonOrientation.OnEdge) {
                    topAngle = 0.0;
                    if ((Sides - 1) % 4 != 0) ++rightIndex;
                } else
                    topAngle = segment;

                topAngle = (topAngle - Math.PI) / 2.0;
                double radiusFactor = Math.Sin(halfSegment);

                double rightAngle = topAngle + rightIndex * segment;
                double widthLength = width * radiusFactor / Math.Cos(rightAngle);

                double bottomAngle = topAngle + (Sides / 2) * segment;
                double heightLength = 2.0 * height * radiusFactor /
                    (Math.Sin(bottomAngle) - Math.Sin(topAngle));

                newLength = Math.Min(widthLength, heightLength);
            }

            return new RegularPolygon(newLength, Sides, Orientation, VertexNeighbors);
        }

        #endregion
        #region OpposingIndex

        /// <summary>
        /// Determines the index of the edge or vertex opposite to the edge or vertex with the
        /// specified index.</summary>
        /// <param name="index">
        /// The zero-based index of an edge or vertex. This value is taken <see
        /// cref="Fortran.Modulo"/> the current <see cref="Connectivity"/>, and may therefore be
        /// negative or greater than the maximum index.</param>
        /// <returns>
        /// The zero-based index of the edge or vertex opposite to the specified <paramref
        /// name="index"/>. This value is always less than <see cref="Connectivity"/>.</returns>
        /// <exception cref="PropertyValueException">
        /// <see cref="Connectivity"/> is not divisible by two. Opposing indices only exist if the
        /// total number of indices is even.</exception>
        /// <remarks>
        /// <b>OpposingIndex</b> should use the same index sequence as the <see
        /// cref="AngleToIndex"/> and <see cref="IndexToAngle"/> methods.</remarks>

        public int OpposingIndex(int index) {

            if (Connectivity % 2 != 0)
                ThrowHelper.ThrowPropertyValueExceptionWithFormat(
                    "Connectivity", Connectivity, Strings.PropertyNotDivisible, 2);

            return Fortran.Modulo(index + Connectivity / 2, Connectivity);
        }

        #endregion
        #region Resize

        /// <summary>
        /// Creates a copy of the <see cref="RegularPolygon"/> with a different side length.
        /// </summary>
        /// <param name="length">
        /// The new value for the <see cref="Length"/> property.</param>
        /// <returns>
        /// A copy of this <see cref="RegularPolygon"/> instance whose <see cref="Length"/> equals
        /// the specified <paramref name="length"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is equal to or less than zero.</exception>
        /// <remarks><para>
        /// <b>Resize</b> returns the current <see cref="RegularPolygon"/> if the specified
        /// <paramref name="length"/> equals the current <see cref="Length"/>.
        /// </para><para>
        /// Otherwise, <b>Resize</b> creates a new <see cref="RegularPolygon"/> whose <see
        /// cref="Length"/> equals the specified <paramref name="length"/>, and whose other
        /// properties retain their current values.</para></remarks>

        public RegularPolygon Resize(double length) {
            if (length == Length) return this;

            if (length <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "length", length, Strings.ArgumentNotPositive);

            return new RegularPolygon(length, Sides, Orientation, VertexNeighbors);
        }

        #endregion
        #endregion
    }
}
