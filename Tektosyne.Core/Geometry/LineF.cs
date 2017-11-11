using System;
using System.Globalization;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a directed line segment in two-dimensional space, using <see cref="Single"/>
    /// coordinates.</summary>
    /// <remarks><para>
    /// <b>LineF</b> is an immutable structure whose four <see cref="Single"/> coordinates indicate
    /// the <see cref="LineF.Start"/> and <see cref="LineF.End"/> points of a directed line segment.
    /// Its <see cref="LineF.Angle"/> and <see cref="LineF.Length"/> are calculated on demand.
    /// </para><para>
    /// Use the <see cref="LineI"/> structure to represent line segments with <see cref="Int32"/>
    /// coordinates, and the <see cref="LineD"/> structure to represent line segments with <see
    /// cref="Double"/> coordinates. You can convert <see cref="LineF"/> instances to and from <see
    /// cref="LineI"/> instances, rounding off the <see cref="Single"/> coordinates as necessary.
    /// </para></remarks>

    [Serializable]
    public struct LineF: IEquatable<LineF> {
        #region LineF(Single, Single, Single, Single)

        /// <overloads>
        /// Initializes a new instance of the <see cref="LineF"/> structure.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="LineF"/> structure with the specified start
        /// and end coordinates.</summary>
        /// <param name="startX">
        /// The <see cref="PointF.X"/> coordinate of the <see cref="Start"/> point.</param>
        /// <param name="startY">
        /// The <see cref="PointF.Y"/> coordinate of the <see cref="Start"/> point.</param>
        /// <param name="endX">
        /// The <see cref="PointF.X"/> coordinate of the <see cref="End"/> point.</param>
        /// <param name="endY">
        /// The the <see cref="PointF.Y"/> coordinate of the <see cref="End"/> point.</param>

        public LineF(float startX, float startY, float endX, float endY) {
            Start = new PointF(startX, startY);
            End = new PointF(endX, endY);
        }

        #endregion
        #region LineF(PointF, PointF)

        /// <summary>
        /// Initializes a new instance of the <see cref="LineF"/> structure with the specified start
        /// and end coordinates.</summary>
        /// <param name="start">
        /// The <see cref="Start"/> point of the <see cref="LineF"/>.</param>
        /// <param name="end">
        /// The <see cref="End"/> point of the <see cref="LineF"/>.</param>

        public LineF(PointF start, PointF end) {
            Start = start;
            End = end;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="LineF"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="LineF"/> instance that was created with the default
        /// constructor.</remarks>

        public static readonly LineF Empty = new LineF();

        #endregion
        #region Public Properties
        #region Start

        /// <summary>
        /// The start coordinates of the <see cref="LineF"/>.</summary>

        public readonly PointF Start;

        #endregion
        #region End

        /// <summary>
        /// The end coordinates of the <see cref="LineF"/>.</summary>

        public readonly PointF End;

        #endregion
        #region Angle

        /// <summary>
        /// Gets the angle of the <see cref="LineF"/>, in radians.</summary>
        /// <value><para>
        /// The angle, in radians, of the direction on the Cartesian plane into which the line
        /// segment represented by the <see cref="LineF"/> is pointing.
        /// </para><para>-or-</para><para>
        /// Zero if <see cref="Length"/> equals zero.</para></value>
        /// <remarks><para>
        /// <b>Angle</b> returns the result of <see cref="Math.Atan2"/> for the vertical and
        /// horizontal distances between the <see cref="End"/> and <see cref="Start"/> points.
        /// </para><para>
        /// <b>Angle</b> equals zero if the <see cref="LineF"/> extends horizontally to the right,
        /// and increases as the line turns clockwise (y-axis pointing downward) or
        /// counter-clockwise (y-axis pointing upward). The possible range of values is (-<see
        /// cref="Math.PI"/>, +<see cref="Math.PI"/>].</para></remarks>

        public double Angle {
            get {
                double x = End.X - Start.X;
                double y = End.Y - Start.Y;
                return Math.Atan2(y, x);
            }
        }

        #endregion
        #region InverseSlope

        /// <summary>
        /// Gets the inverse slope of the <see cref="LineF"/>.</summary>
        /// <value><para>
        /// The quotient of the horizontal and the vertical extent of the <see cref="LineF"/>.
        /// </para><para>-or-</para><para>
        /// <see cref="Single.MaxValue"/> if the <see cref="LineF"/> is horizontal.</para></value>
        /// <remarks>
        /// <b>InverseSlope</b> may return a negative value, depending on the <see cref="Angle"/> of
        /// the <see cref="LineF"/>. The value of this property equals 1/<see cref="Slope"/>.
        /// </remarks>

        public float InverseSlope {
            get {
                float d = End.Y - Start.Y;
                return (d == 0 ? Single.MaxValue : (End.X - Start.X) / d);
            }
        }

        #endregion
        #region Length

        /// <summary>
        /// Gets the absolute length of the <see cref="LineF"/>.</summary>
        /// <value>
        /// A non-negative <see cref="Double"/> value indicating the absolute length of the line
        /// segment represented by the <see cref="LineF"/>.</value>
        /// <remarks>
        /// <b>Length</b> returns the square root of the sum of the squares of the horizontal and
        /// vertical distances between the <see cref="End"/> and <see cref="Start"/> points.
        /// </remarks>

        public double Length {
            get {
                double x = End.X - Start.X;
                double y = End.Y - Start.Y;
                return Math.Sqrt(x * x + y * y);
            }
        }

        #endregion
        #region LengthSquared

        /// <summary>
        /// Gets the squared absolute length of the <see cref="LineF"/>.</summary>
        /// <value>
        /// A non-negative <see cref="Single"/> value that equals the square of the <see
        /// cref="Length"/> property.</value>
        /// <remarks>
        /// <b>LengthSquared</b> performs the same operations as <see cref="Length"/> but without
        /// the final <see cref="Math.Sqrt"/> call, and is therefore faster if you only need the
        /// squared <see cref="Length"/>.</remarks>

        public float LengthSquared {
            get {
                float x = End.X - Start.X;
                float y = End.Y - Start.Y;
                return (x * x + y * y);
            }
        }

        #endregion
        #region Slope

        /// <summary>
        /// Gets the slope of the <see cref="LineF"/>.</summary>
        /// <value><para>
        /// The quotient of the vertical and the horizontal extent of the <see cref="LineF"/>.
        /// </para><para>-or-</para><para>
        /// <see cref="Single.MaxValue"/> if the <see cref="LineF"/> is vertical.</para></value>
        /// <remarks>
        /// <b>Slope</b> may return a negative value, depending on the <see cref="Angle"/> of the
        /// <see cref="LineF"/>. The value of this property equals 1/<see cref="InverseSlope"/>.
        /// </remarks>

        public float Slope {
            get {
                float d = End.X - Start.X;
                return (d == 0 ? Single.MaxValue : (End.Y - Start.Y) / d);
            }
        }

        #endregion
        #region Vector

        /// <summary>
        /// Gets the vector defined by the <see cref="LineF"/>.</summary>
        /// <value>
        /// A <see cref="PointF"/> value defined as (<see cref="End"/> - <see cref="Start"/>).
        /// </value>
        /// <remarks>
        /// <b>Vector</b> returns the <see cref="End"/> point of the <see cref="LineF"/> with its
        /// <see cref="Start"/> point shifted to the origins of the coordinate system.</remarks>

        public PointF Vector {
            get { return new PointF(End.X - Start.X, End.Y - Start.Y); }
        }

        #endregion
        #endregion
        #region Public Methods
        #region DistanceSquared

        /// <summary>
        /// Determines the squared distance between the <see cref="LineF"/> and the specified <see
        /// cref="PointF"/> coordinates.</summary>
        /// <param name="q">
        /// The <see cref="PointF"/> coordinates to examine.</param>
        /// <returns>
        /// The squared distance between <paramref name="q"/> and the <see cref="LineF"/>.</returns>
        /// <remarks>
        /// <b>DistanceSquared</b> returns either the squared length of the perpendicular dropped
        /// from <paramref name="q"/> on the <see cref="LineF"/>, or the squared distance between
        /// <paramref name="q"/> and <see cref="Start"/> or <see cref="End"/> if the perpendicular
        /// does not intersect the <see cref="LineF"/>.</remarks>

        public float DistanceSquared(PointF q) {
            if (q == Start || q == End) return 0;

            float x = Start.X, y = Start.Y;
            float ax = End.X - x, ay = End.Y - y;

            // set (x,y) to nearest LineF point from q
            if (ax != 0 || ay != 0) {
                float u = ((q.X - x) * ax + (q.Y - y) * ay) / (ax * ax + ay * ay);
                if (u > 1) {
                    x = End.X; y = End.Y;
                } else if (u > 0) {
                    x += u * ax; y += u * ay;
                }
            }

            x = q.X - x; y = q.Y - y;
            return (x * x + y * y);
        }

        #endregion
        #region FindX

        /// <summary>
        /// Finds the x-coordinate for the specified y-coordinate on the <see cref="LineF"/> or its
        /// infinite extension.</summary>
        /// <param name="y">
        /// The y-coordinate on the <see cref="LineF"/> to examine.</param>
        /// <returns><para>
        /// The x-coordinate for the specified <paramref name="y"/> coordinate on the <see
        /// cref="LineF"/> or its infinite extension.
        /// </para><para>-or-</para><para>
        /// <see cref="Single.MaxValue"/> if the <see cref="LineF"/> is horizontal.</para></returns>
        /// <remarks>
        /// <b>FindX</b> returns the <see cref="PointF.X"/> component of <see cref="Start"/> or <see
        /// cref="End"/> if the specified <paramref name="y"/> coordinate exactly equals the
        /// corresponding <see cref="PointF.Y"/> component; otherwise, a computed x-coordinate.
        /// </remarks>

        public float FindX(float y) {

            float d = End.Y - Start.Y;
            if (d == 0) return Single.MaxValue;

            if (y == Start.Y) return Start.X;
            if (y == End.Y) return End.X;

            return Start.X + (y - Start.Y) * (End.X - Start.X) / d;
        }

        #endregion
        #region FindY

        /// <summary>
        /// Finds the y-coordinate for the specified x-coordinate on the <see cref="LineF"/> or its
        /// infinite extension.</summary>
        /// <param name="x">
        /// The x-coordinate on the <see cref="LineF"/> to examine.</param>
        /// <returns><para>
        /// The y-coordinate for the specified <paramref name="x"/> coordinate on the <see
        /// cref="LineF"/> or its infinite extension.
        /// </para><para>-or-</para><para>
        /// <see cref="Single.MaxValue"/> if the <see cref="LineF"/> is vertical.</para></returns>
        /// <remarks>
        /// <b>FindY</b> returns the <see cref="PointF.Y"/> component of <see cref="Start"/> or <see
        /// cref="End"/> if the specified <paramref name="x"/> coordinate exactly equals the
        /// corresponding <see cref="PointF.X"/> component; otherwise, a computed y-coordinate.
        /// </remarks>

        public float FindY(float x) {

            float d = End.X - Start.X;
            if (d == 0) return Single.MaxValue;

            if (x == Start.X) return Start.Y;
            if (x == End.X) return End.Y;

            return Start.Y + (x - Start.X) * (End.Y - Start.Y) / d;
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="LineF"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the <see cref="PointF.X"/> and <see cref="PointF.Y"/>
        /// components of the <see cref="Start"/> and <see cref="End"/> properties.</remarks>

        public override unsafe int GetHashCode() {
            unchecked {
                float x0 = Start.X, y0 = Start.Y, x1 = End.X, y1 = End.Y;
                int x0i = *((int*) &x0), y0i = *((int*) &y0);
                int x1i = *((int*) &x1), y1i = *((int*) &y1);
                return x0i ^ y0i ^ x1i ^ y1i;
            }
        }

        #endregion
        #region Intersect(LineF)

        /// <overloads>
        /// Finds the intersection between the <see cref="LineF"/> and a specified line segment.
        /// </overloads>
        /// <summary>
        /// Finds the intersection between this instance and a specified <see cref="LineF"/>, using
        /// exact coordinate comparisons.</summary>
        /// <param name="line">
        /// A <see cref="LineF"/> to intersect with this instance.</param>
        /// <returns>
        /// A <see cref="LineIntersection"/> instance that describes if and how the specified
        /// <paramref name="line"/> and this instance intersect.</returns>
        /// <remarks>
        /// <b>Intersect</b> returns the result of <see cref="LineIntersection.Find(PointD, PointD,
        /// PointD, PointD)"/> for the <see cref="Start"/> and <see cref="End"/> points of this
        /// instance and of the specified <paramref name="line"/>, in that order.</remarks>

        public LineIntersection Intersect(LineF line) {
            return LineIntersection.Find(Start, End, line.Start, line.End);
        }

        #endregion
        #region Intersect(LineF, Single)

        /// <summary>
        /// Finds the intersection between this instance and a specified <see cref="LineF"/>, given
        /// the specified epsilon for coordinate comparisons.</summary>
        /// <param name="line">
        /// A <see cref="LineF"/> to intersect with this instance.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which coordinates and intermediate results should be
        /// considered equal. This value is always raised to a minium of 1e-10.</param>
        /// <returns>
        /// A <see cref="LineIntersection"/> instance that describes if and how the specified
        /// <paramref name="line"/> and this instance intersect.</returns>
        /// <remarks>
        /// <b>Intersect</b> returns the result of <see cref="LineIntersection.Find(PointD, PointD,
        /// PointD, PointD, Double)"/> for the <see cref="Start"/> and <see cref="End"/> points of
        /// this instance and of the specified <paramref name="line"/>, in that order, and for the
        /// specified <paramref name="epsilon"/>.</remarks>

        public LineIntersection Intersect(LineF line, float epsilon) {
            return LineIntersection.Find(Start, End, line.Start, line.End, epsilon);
        }

        #endregion
        #region Intersect(PointF)

        /// <summary>
        /// Finds the intersection between the <see cref="LineF"/> and the perpendicular dropped
        /// from the specified <see cref="PointF"/> coordinates.</summary>
        /// <param name="q">
        /// The <see cref="PointF"/> coordinates to examine.</param>
        /// <returns>
        /// The <see cref="PointF"/> coordinates where the perpendicular dropped from <paramref
        /// name="q"/> intersects the <see cref="LineF"/> or its infinite extension.</returns>
        /// <remarks>
        /// <b>Intersect</b> returns <see cref="Start"/> if <see cref="Length"/> equals zero.
        /// </remarks>

        public PointF Intersect(PointF q) {

            if (q == Start) return Start;
            if (q == End) return End;

            float x = Start.X, y = Start.Y;
            float ax = End.X - x, ay = End.Y - y;
            if (ax == 0 && ay == 0) return Start;

            float u = ((q.X - x) * ax + (q.Y - y) * ay) / (ax * ax + ay * ay);
            return new PointF(x + u * ax, y + u * ay);
        }

        #endregion
        #region Locate(PointF)

        /// <overloads>
        /// Determines the location of the specified <see cref="PointF"/> coordinates relative to
        /// the <see cref="LineF"/>.</overloads>
        /// <summary>
        /// Determines the location of the specified <see cref="PointF"/> coordinates relative to
        /// the <see cref="LineF"/>, using exact coordinate comparisons.</summary>
        /// <param name="q">
        /// The <see cref="PointF"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="LineLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="LineF"/>.</returns>
        /// <remarks><para>
        /// <b>Locate</b> never returns <see cref="LineLocation.None"/>. The values <see
        /// cref="LineLocation.Left"/> and <see cref="LineLocation.Right"/> assume that
        /// y-coordinates increase upward.
        /// </para><para>
        /// <b>Locate</b> is based on the <c>classify</c> algorithm by Michael J. Laszlo,
        /// <em>Computational Geometry and Computer Graphics in C++</em>, Prentice Hall 1996, p.76.
        /// </para></remarks>

        public LineLocation Locate(PointF q) {

            float qx0 = q.X - Start.X, qy0 = q.Y - Start.Y;
            if (qx0 == 0 && qy0 == 0)
                return LineLocation.Start;

            float qx1 = q.X - End.X, qy1 = q.Y - End.Y;
            if (qx1 == 0 && qy1 == 0)
                return LineLocation.End;

            float ax = End.X - Start.X, ay = End.Y - Start.Y;
            float area = ax * qy0 - qx0 * ay;
            if (area > 0) return LineLocation.Left;
            if (area < 0) return LineLocation.Right;

            if (qx0 * qx1 <= 0 && qy0 * qy1 <= 0)
                return LineLocation.Between;

            if (ax * qx0 < 0 || ay * qy0 < 0)
                return LineLocation.Before;
            else
                return LineLocation.After;
        }

        #endregion
        #region Locate(PointF, Single)

        /// <summary>
        /// Determines the location of the specified <see cref="PointF"/> coordinates relative to
        /// the <see cref="LineF"/>, given the specified epsilon for coordinate comparisons.
        /// </summary>
        /// <param name="q">
        /// The <see cref="PointF"/> coordinates to examine.</param>
        /// <param name="epsilon">
        /// The maximum absolute value at which intermediate results should be considered zero.
        /// </param>
        /// <returns>
        /// A <see cref="LineLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="LineF"/>.</returns>
        /// <remarks><para>
        /// <b>Locate</b> is identical with the basic <see cref="Locate(PointF)"/> overload but uses
        /// the specified <paramref name="epsilon"/> to compare intermediate results to zero.
        /// </para><para>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but <b>Locate</b>
        /// does not check this condition.</para></remarks>

        public LineLocation Locate(PointF q, float epsilon) {

            float qx0 = q.X - Start.X, qy0 = q.Y - Start.Y;
            if (Math.Abs(qx0) <= epsilon && Math.Abs(qy0) <= epsilon)
                return LineLocation.Start;

            float qx1 = q.X - End.X, qy1 = q.Y - End.Y;
            if (Math.Abs(qx1) <= epsilon && Math.Abs(qy1) <= epsilon)
                return LineLocation.End;

            float ax = End.X - Start.X, ay = End.Y - Start.Y;
            float area = ax * qy0 - qx0 * ay;
            float epsilon2 = epsilon * (Math.Abs(ax) + Math.Abs(ay));
            if (area > epsilon2) return LineLocation.Left;
            if (area < -epsilon2) return LineLocation.Right;

            if ((qx0 * qx1 <= 0 || Math.Abs(qx0) <= epsilon || Math.Abs(qx1) <= epsilon) &&
                (qy0 * qy1 <= 0 || Math.Abs(qy0) <= epsilon || Math.Abs(qy1) <= epsilon))
                return LineLocation.Between;

            if (ax * qx0 < 0 || ay * qy0 < 0)
                return LineLocation.Before;
            else
                return LineLocation.After;
        }

        #endregion
        #region LocateCollinear(PointF)

        /// <overloads>
        /// Determines the location of the specified <see cref="PointF"/> coordinates relative to
        /// the <see cref="LineF"/>, assuming they are collinear.</overloads>
        /// <summary>
        /// Determines the location of the specified <see cref="PointF"/> coordinates relative to
        /// the <see cref="LineF"/>, assuming they are collinear and using exact coordinate
        /// comparisons.</summary>
        /// <param name="q">
        /// The <see cref="PointF"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="LineLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="LineF"/>.</returns>
        /// <remarks>
        /// <b>LocateCollinear</b> is identical with <see cref="Locate"/> but assumes that <paramref
        /// name="q"/> is collinear with the <see cref="LineF"/>, and therefore never returns the
        /// values <see cref="LineLocation.Left"/> or <see cref="LineLocation.Right"/>.</remarks>

        public LineLocation LocateCollinear(PointF q) {

            float qx0 = q.X - Start.X, qy0 = q.Y - Start.Y;
            if (qx0 == 0 && qy0 == 0)
                return LineLocation.Start;

            float qx1 = q.X - End.X, qy1 = q.Y - End.Y;
            if (qx1 == 0 && qy1 == 0)
                return LineLocation.End;

            if (qx0 * qx1 <= 0 && qy0 * qy1 <= 0)
                return LineLocation.Between;

            if ((End.X - Start.X) * qx0 < 0 || (End.Y - Start.Y) * qy0 < 0)
                return LineLocation.Before;
            else
                return LineLocation.After;
        }

        #endregion
        #region LocateCollinear(PointF, Single)

        /// <summary>
        /// Determines the location of the specified <see cref="PointF"/> coordinates relative to
        /// the <see cref="LineF"/>, assuming they are collinear and given the specified epsilon for
        /// coordinate comparisons.</summary>
        /// <param name="q">
        /// The <see cref="PointF"/> coordinates to examine.</param>
        /// <param name="epsilon">
        /// The maximum absolute value at which intermediate results should be considered zero.
        /// </param>
        /// <returns>
        /// A <see cref="LineLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="LineF"/>.</returns>
        /// <remarks><para>
        /// <b>LocateCollinear</b> is identical with the basic <see cref="LocateCollinear(PointF)"/>
        /// overload but uses the specified <paramref name="epsilon"/> to compare <paramref
        /// name="q"/> to the <see cref="Start"/> and <see cref="End"/> points.
        /// </para><para>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but
        /// <b>LocateCollinear</b> does not check this condition.</para></remarks>

        public LineLocation LocateCollinear(PointF q, float epsilon) {

            float qx0 = q.X - Start.X, qy0 = q.Y - Start.Y;
            if (Math.Abs(qx0) <= epsilon && Math.Abs(qy0) <= epsilon)
                return LineLocation.Start;

            float qx1 = q.X - End.X, qy1 = q.Y - End.Y;
            if (Math.Abs(qx1) <= epsilon && Math.Abs(qy1) <= epsilon)
                return LineLocation.End;

            if ((qx0 * qx1 <= 0 || Math.Abs(qx0) <= epsilon || Math.Abs(qx1) <= epsilon) &&
                (qy0 * qy1 <= 0 || Math.Abs(qy0) <= epsilon || Math.Abs(qy1) <= epsilon))
                return LineLocation.Between;

            if ((End.X - Start.X) * qx0 < 0 || (End.Y - Start.Y) * qy0 < 0)
                return LineLocation.Before;
            else
                return LineLocation.After;
        }

        #endregion
        #region Reverse

        /// <summary>
        /// Reverses the direction of the <see cref="LineF"/>.</summary>
        /// <returns>
        /// A new <see cref="LineF"/> instance whose <see cref="Start"/> property equals the <see
        /// cref="End"/> property of this instance, and vice versa.</returns>

        public LineF Reverse() {
            return new LineF(End, Start);
        }

        #endregion
        #region Round

        /// <summary>
        /// Converts the <see cref="LineF"/> to a <see cref="LineI"/> by rounding coordinates to the
        /// nearest <see cref="Int32"/> values.</summary>
        /// <returns>
        /// A <see cref="LineI"/> instance whose <see cref="LineI.Start"/> and <see
        /// cref="LineI.End"/> properties equal the corresponding properties of the <see
        /// cref="LineF"/>, rounded to the nearest <see cref="Int32"/> values.</returns>
        /// <remarks>
        /// The <see cref="Single"/> coordinates of the <see cref="LineF"/> are converted to <see
        /// cref="Int32"/> coordinates using <see cref="Fortran.NInt"/> rounding.</remarks>

        public LineI Round() {
            return new LineI(
                Fortran.NInt(Start.X), Fortran.NInt(Start.Y),
                Fortran.NInt(End.X), Fortran.NInt(End.Y));
        }

        #endregion
        #region ToLineI

        /// <summary>
        /// Converts the <see cref="LineF"/> to a <see cref="LineI"/> by truncating coordinates to
        /// the nearest <see cref="Int32"/> values.</summary>
        /// <returns>
        /// A <see cref="LineI"/> instance whose <see cref="LineI.Start"/> and <see
        /// cref="LineI.End"/> properties equal the corresponding properties of the <see
        /// cref="LineF"/>, truncated to the nearest <see cref="Int32"/> values.</returns>

        public LineI ToLineI() {
            return new LineI((int) Start.X, (int) Start.Y, (int) End.X, (int) End.Y);
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="LineF"/>.</summary>
        /// <returns>
        /// A <see cref="String"/> containing the coordinates of the <see cref="Start"/> and <see
        /// cref="End"/> properties.</returns>

        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture,
                "{{Start.X={0}, Start.Y={1}, End.X={2}, End.Y={3}}}",
                Start.X, Start.Y, End.X, End.Y);
        }

        #endregion
        #endregion
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="LineF"/> instances have the same value.</summary>
        /// <param name="a">
        /// The first <see cref="LineF"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="LineF"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(LineF)"/> method to test the two <see
        /// cref="LineF"/> instances for value equality.</remarks>

        public static bool operator ==(LineF a, LineF b) {
            return a.Equals(b);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="LineF"/> instances have different values.</summary>
        /// <param name="a">
        /// The first <see cref="LineF"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="LineF"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different from the value of
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(LineF)"/> method to test the two <see
        /// cref="LineF"/> instances for value inequality.</remarks>

        public static bool operator !=(LineF a, LineF b) {
            return !a.Equals(b);
        }

        #endregion
        #region LineF(LineI)

        /// <summary>
        /// Converts a <see cref="LineI"/> to a <see cref="LineF"/> with identical coordinates.
        /// </summary>
        /// <param name="line">
        /// The <see cref="LineI"/> instance to convert into a <see cref="LineF"/> instance.</param>
        /// <returns>
        /// A <see cref="LineF"/> instance whose <see cref="Start"/> and <see cref="End"/>
        /// properties equal the corresponding properties of the specified <paramref name="line"/>.
        /// </returns>

        public static implicit operator LineF(LineI line) {
            return new LineF(line.Start.X, line.Start.Y, line.End.X, line.End.Y);
        }

        #endregion
        #region LineI(LineF)

        /// <summary>
        /// Converts a <see cref="LineF"/> to a <see cref="LineI"/> by truncating coordinates to the
        /// nearest <see cref="Int32"/> values.</summary>
        /// <param name="line">
        /// The <see cref="LineF"/> instance to convert into a <see cref="LineI"/> instance.</param>
        /// <returns>
        /// A <see cref="LineI"/> instance whose <see cref="LineI.Start"/> and <see
        /// cref="LineI.End"/> properties equal the corresponding properties of the specified
        /// <paramref name="line"/>, truncated to the nearest <see cref="Int32"/> values.</returns>

        public static explicit operator LineI(LineF line) {
            return line.ToLineI();
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="LineF"/> instances have the same value.</overloads>
        /// <summary>
        /// Determines whether this <see cref="LineF"/> instance and a specified object, which must
        /// be a <see cref="LineF"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="LineF"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="LineF"/> instance and its
        /// value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="LineF"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(LineF)"/> overload to test
        /// the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is LineF))
                return false;

            return Equals((LineF) obj);
        }

        #endregion
        #region Equals(LineF)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="LineF"/> have the same
        /// value.</summary>
        /// <param name="line">
        /// A <see cref="LineF"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="line"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="Start"/> and <see cref="End"/>
        /// properties of the two <see cref="LineF"/> instances to test for value equality.
        /// </remarks>

        public bool Equals(LineF line) {
            return (Start == line.Start && End == line.End);
        }

        #endregion
        #region Equals(LineF, LineF)

        /// <summary>
        /// Determines whether two specified <see cref="LineF"/> instances have the same value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="LineF"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="LineF"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(LineF)"/> overload to test the
        /// two <see cref="LineF"/> instances for value equality.</remarks>

        public static bool Equals(LineF a, LineF b) {
            return a.Equals(b);
        }

        #endregion
        #region Equals(LineF, LineF, Single)

        /// <summary>
        /// Determines whether two specified <see cref="LineF"/> instances have the same value,
        /// given the specified epsilon.</summary>
        /// <param name="a">
        /// The first <see cref="LineF"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="LineF"/> to compare.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which the coordinates of <paramref name="a"/> and
        /// <paramref name="b"/> should be considered equal.</param>
        /// <returns>
        /// <c>true</c> if the absolute difference between the coordinates of <paramref name="a"/>
        /// and <paramref name="b"/> is less than or equal to <paramref name="epsilon"/>; otherwise,
        /// <c>false</c>.</returns>
        /// <remarks>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but <b>Equals</b>
        /// does not check this condition.</remarks>

        public static bool Equals(LineF a, LineF b, float epsilon) {

            return (PointF.Equals(a.Start, b.Start, epsilon)
                && PointF.Equals(a.End, b.End, epsilon));
        }

        #endregion
        #endregion
    }
}
