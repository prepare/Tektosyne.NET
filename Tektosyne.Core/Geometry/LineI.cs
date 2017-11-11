using System;
using System.Globalization;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a directed line segment in two-dimensional space, using <see cref="Int32"/>
    /// coordinates.</summary>
    /// <remarks><para>
    /// <b>LineI</b> is an immutable structure whose four <see cref="Int32"/> coordinates indicate
    /// the <see cref="LineI.Start"/> and <see cref="LineI.End"/> points of a directed line segment.
    /// Its <see cref="LineI.Angle"/> and <see cref="LineI.Length"/> are calculated on demand.
    /// </para><para>
    /// Use the <see cref="LineF"/> structure to represent line segments with <see cref="Single"/>
    /// coordinates, and the <see cref="LineD"/> structure to represent line segments with <see
    /// cref="Double"/> coordinates.</para></remarks>

    [Serializable]
    public struct LineI: IEquatable<LineI> {
        #region LineI(Int32, Int32, Int32, Int32)

        /// <overloads>
        /// Initializes a new instance of the <see cref="LineI"/> structure.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="LineI"/> structure with the specified start
        /// and end coordinates.</summary>
        /// <param name="startX">
        /// The <see cref="PointI.X"/> coordinate of the <see cref="Start"/> point.</param>
        /// <param name="startY">
        /// The <see cref="PointI.Y"/> coordinate of the <see cref="Start"/> point.</param>
        /// <param name="endX">
        /// The <see cref="PointI.X"/> coordinate of the <see cref="End"/> point.</param>
        /// <param name="endY">
        /// The the <see cref="PointI.Y"/> coordinate of the <see cref="End"/> point.</param>

        public LineI(int startX, int startY, int endX, int endY) {
            Start = new PointI(startX, startY);
            End = new PointI(endX, endY);
        }

        #endregion
        #region LineI(PointI, PointI)

        /// <summary>
        /// Initializes a new instance of the <see cref="LineI"/> structure with the specified start
        /// and end coordinates.</summary>
        /// <param name="start">
        /// The <see cref="Start"/> point of the <see cref="LineI"/>.</param>
        /// <param name="end">
        /// The <see cref="End"/> point of the <see cref="LineI"/>.</param>

        public LineI(PointI start, PointI end) {
            Start = start;
            End = end;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="LineI"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="LineI"/> instance that was created with the default
        /// constructor.</remarks>

        public static readonly LineI Empty = new LineI();

        #endregion
        #region Public Properties
        #region Start

        /// <summary>
        /// The start coordinates of the <see cref="LineI"/>.</summary>

        public readonly PointI Start;

        #endregion
        #region End

        /// <summary>
        /// The end coordinates of the <see cref="LineI"/>.</summary>

        public readonly PointI End;

        #endregion
        #region Angle

        /// <summary>
        /// Gets the angle of the <see cref="LineI"/>, in radians.</summary>
        /// <value><para>
        /// The angle, in radians, of the direction on the Cartesian plane into which the line
        /// segment represented by the <see cref="LineI"/> is pointing.
        /// </para><para>-or-</para><para>
        /// Zero if <see cref="Length"/> equals zero.</para></value>
        /// <remarks><para>
        /// <b>Angle</b> returns the result of <see cref="Math.Atan2"/> for the vertical and
        /// horizontal distances between the <see cref="End"/> and <see cref="Start"/> points.
        /// </para><para>
        /// <b>Angle</b> equals zero if the <see cref="LineI"/> extends horizontally to the right,
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
        /// Gets the inverse slope of the <see cref="LineI"/>.</summary>
        /// <value><para>
        /// The quotient of the horizontal and the vertical extent of the <see cref="LineI"/>.
        /// </para><para>-or-</para><para>
        /// <see cref="Double.MaxValue"/> if the <see cref="LineI"/> is horizontal.</para></value>
        /// <remarks>
        /// <b>InverseSlope</b> may return a negative value, depending on the <see cref="Angle"/> of
        /// the <see cref="LineI"/>. The value of this property equals 1/<see cref="Slope"/>.
        /// </remarks>

        public double InverseSlope {
            get {
                double d = End.Y - Start.Y;
                return (d == 0 ? Double.MaxValue : (End.X - Start.X) / d);
            }
        }

        #endregion
        #region Length

        /// <summary>
        /// Gets the absolute length of the <see cref="LineI"/>.</summary>
        /// <value>
        /// A non-negative <see cref="Double"/> value indicating the absolute length of the line
        /// segment represented by the <see cref="LineI"/>.</value>
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
        /// Gets the squared absolute length of the <see cref="LineI"/>.</summary>
        /// <value>
        /// A non-negative <see cref="Int32"/> value that equals the square of the <see
        /// cref="Length"/> property.</value>
        /// <remarks>
        /// <b>LengthSquared</b> performs the same operations as <see cref="Length"/> but without
        /// the final <see cref="Math.Sqrt"/> call, and is therefore faster if you only need the
        /// squared <see cref="Length"/>.</remarks>

        public int LengthSquared {
            get {
                int x = End.X - Start.X;
                int y = End.Y - Start.Y;
                return (x * x + y * y);
            }
        }

        #endregion
        #region Slope

        /// <summary>
        /// Gets the slope of the <see cref="LineI"/>.</summary>
        /// <value><para>
        /// The quotient of the vertical and the horizontal extent of the <see cref="LineI"/>.
        /// </para><para>-or-</para><para>
        /// <see cref="Double.MaxValue"/> if the <see cref="LineI"/> is vertical.</para></value>
        /// <remarks>
        /// <b>Slope</b> may return a negative value, depending on the <see cref="Angle"/> of the
        /// <see cref="LineI"/>. The value of this property equals 1/<see cref="InverseSlope"/>.
        /// </remarks>

        public double Slope {
            get {
                double d = End.X - Start.X;
                return (d == 0 ? Double.MaxValue : (End.Y - Start.Y) / d);
            }
        }

        #endregion
        #region Vector

        /// <summary>
        /// Gets the vector defined by the <see cref="LineI"/>.</summary>
        /// <value>
        /// A <see cref="PointI"/> value defined as (<see cref="End"/> - <see cref="Start"/>).
        /// </value>
        /// <remarks>
        /// <b>Vector</b> returns the <see cref="End"/> point of the <see cref="LineI"/> with its
        /// <see cref="Start"/> point shifted to the origins of the coordinate system.</remarks>

        public PointI Vector {
            get { return new PointI(End.X - Start.X, End.Y - Start.Y); }
        }

        #endregion
        #endregion
        #region Public Methods
        #region DistanceSquared

        /// <summary>
        /// Determines the squared distance between the <see cref="LineI"/> and the specified <see
        /// cref="PointI"/> coordinates.</summary>
        /// <param name="q">
        /// The <see cref="PointI"/> coordinates to examine.</param>
        /// <returns>
        /// The squared distance between <paramref name="q"/> and the <see cref="LineI"/>.</returns>
        /// <remarks>
        /// <b>DistanceSquared</b> returns either the squared length of the perpendicular dropped
        /// from <paramref name="q"/> on the <see cref="LineI"/>, or the squared distance between
        /// <paramref name="q"/> and <see cref="Start"/> or <see cref="End"/> if the perpendicular
        /// does not intersect the <see cref="LineI"/>.</remarks>

        public double DistanceSquared(PointI q) {
            if (q == Start || q == End) return 0;

            double x = Start.X, y = Start.Y;
            int ax = End.X - Start.X, ay = End.Y - Start.Y;

            // set (x,y) to nearest LineI point from q
            if (ax != 0 || ay != 0) {
                double u = ((q.X - x) * ax + (q.Y - y) * ay) / (double) (ax * ax + ay * ay);
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
        /// Finds the x-coordinate for the specified y-coordinate on the <see cref="LineI"/> or its
        /// infinite extension.</summary>
        /// <param name="y">
        /// The y-coordinate on the <see cref="LineI"/> to examine.</param>
        /// <returns><para>
        /// The x-coordinate for the specified <paramref name="y"/> coordinate on the <see
        /// cref="LineI"/> or its infinite extension.
        /// </para><para>-or-</para><para>
        /// <see cref="Double.MaxValue"/> if the <see cref="LineI"/> is horizontal.</para></returns>
        /// <remarks>
        /// <b>FindX</b> returns the <see cref="PointI.X"/> component of <see cref="Start"/> or <see
        /// cref="End"/> if the specified <paramref name="y"/> coordinate exactly equals the
        /// corresponding <see cref="PointI.Y"/> component; otherwise, a computed x-coordinate.
        /// </remarks>

        public double FindX(double y) {

            int d = End.Y - Start.Y;
            if (d == 0) return Double.MaxValue;

            if (y == Start.Y) return Start.X;
            if (y == End.Y) return End.X;

            return Start.X + (y - Start.Y) * (End.X - Start.X) / d;
        }

        #endregion
        #region FindY

        /// <summary>
        /// Finds the y-coordinate for the specified x-coordinate on the <see cref="LineI"/> or its
        /// infinite extension.</summary>
        /// <param name="x">
        /// The x-coordinate on the <see cref="LineI"/> to examine.</param>
        /// <returns><para>
        /// The y-coordinate for the specified <paramref name="x"/> coordinate on the <see
        /// cref="LineI"/> or its infinite extension.
        /// </para><para>-or-</para><para>
        /// <see cref="Double.MaxValue"/> if the <see cref="LineI"/> is vertical.</para></returns>
        /// <remarks>
        /// <b>FindY</b> returns the <see cref="PointI.Y"/> component of <see cref="Start"/> or <see
        /// cref="End"/> if the specified <paramref name="x"/> coordinate exactly equals the
        /// corresponding <see cref="PointI.X"/> component; otherwise, a computed y-coordinate.
        /// </remarks>

        public double FindY(double x) {

            int d = End.X - Start.X;
            if (d == 0) return Double.MaxValue;

            if (x == Start.X) return Start.Y;
            if (x == End.X) return End.Y;

            return Start.Y + (x - Start.X) * (End.Y - Start.Y) / d;
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="LineI"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the <see cref="PointI.X"/> and <see cref="PointI.Y"/>
        /// components of the <see cref="Start"/> and <see cref="End"/> properties.</remarks>

        public override int GetHashCode() {
            unchecked { return Start.X ^ Start.Y ^ End.X ^ End.Y; }
        }

        #endregion
        #region Intersect(LineI)

        /// <overloads>
        /// Finds the intersection between the <see cref="LineI"/> and a specified line segment.
        /// </overloads>
        /// <summary>
        /// Finds the intersection between this instance and a specified <see cref="LineI"/>.
        /// </summary>
        /// <param name="line">
        /// A <see cref="LineI"/> to intersect with this instance.</param>
        /// <returns>
        /// A <see cref="LineIntersection"/> instance that describes if and how the specified
        /// <paramref name="line"/> and this instance intersect.</returns>
        /// <remarks>
        /// <b>Intersect</b> returns the result of <see cref="LineIntersection.Find(PointD, PointD,
        /// PointD, PointD)"/> for the <see cref="Start"/> and <see cref="End"/> points of this
        /// instance and of the specified <paramref name="line"/>, in that order.</remarks>

        public LineIntersection Intersect(LineI line) {
            return LineIntersection.Find(Start, End, line.Start, line.End);
        }

        #endregion
        #region Intersect(PointI)

        /// <summary>
        /// Finds the intersection between the <see cref="LineI"/> and the perpendicular dropped
        /// from the specified <see cref="PointI"/> coordinates.</summary>
        /// <param name="q">
        /// The <see cref="PointI"/> coordinates to examine.</param>
        /// <returns>
        /// The <see cref="PointD"/> coordinates where the perpendicular dropped from <paramref
        /// name="q"/> intersects the <see cref="LineI"/> or its infinite extension.</returns>
        /// <remarks>
        /// <b>Intersect</b> returns <see cref="Start"/> if <see cref="Length"/> equals zero.
        /// </remarks>

        public PointD Intersect(PointI q) {

            if (q == Start) return Start;
            if (q == End) return End;

            int x = Start.X, y = Start.Y;
            int ax = End.X - x, ay = End.Y - y;
            if (ax == 0 && ay == 0) return Start;

            double u = ((q.X - x) * ax + (q.Y - y) * ay) / (double) (ax * ax + ay * ay);
            return new PointD(x + u * ax, y + u * ay);
        }

        #endregion
        #region Locate

        /// <summary>
        /// Determines the location of the specified <see cref="PointI"/> coordinates relative to
        /// the <see cref="LineI"/>.</summary>
        /// <param name="q">
        /// The <see cref="PointI"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="LineLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="LineI"/>.</returns>
        /// <remarks><para>
        /// <b>Locate</b> never returns <see cref="LineLocation.None"/>. The values <see
        /// cref="LineLocation.Left"/> and <see cref="LineLocation.Right"/> assume that
        /// y-coordinates increase upward.
        /// </para><para>
        /// <b>Locate</b> is based on the <c>classify</c> algorithm by Michael J. Laszlo,
        /// <em>Computational Geometry and Computer Graphics in C++</em>, Prentice Hall 1996, p.76.
        /// </para></remarks>

        public LineLocation Locate(PointI q) {

            int qx0 = q.X - Start.X, qy0 = q.Y - Start.Y;
            if (qx0 == 0 && qy0 == 0)
                return LineLocation.Start;

            int qx1 = q.X - End.X, qy1 = q.Y - End.Y;
            if (qx1 == 0 && qy1 == 0)
                return LineLocation.End;

            int ax = End.X - Start.X, ay = End.Y - Start.Y;
            int area = ax * qy0 - qx0 * ay;
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
        #region LocateCollinear

        /// <summary>
        /// Determines the location of the specified <see cref="PointI"/> coordinates relative to
        /// the <see cref="LineI"/>, assuming they are collinear.</summary>
        /// <param name="q">
        /// The <see cref="PointI"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="LineLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="LineI"/>.</returns>
        /// <remarks>
        /// <b>LocateCollinear</b> is identical with <see cref="Locate"/> but assumes that <paramref
        /// name="q"/> is collinear with the <see cref="LineI"/>, and therefore never returns the
        /// values <see cref="LineLocation.Left"/> or <see cref="LineLocation.Right"/>.</remarks>

        public LineLocation LocateCollinear(PointI q) {

            int qx0 = q.X - Start.X, qy0 = q.Y - Start.Y;
            if (qx0 == 0 && qy0 == 0)
                return LineLocation.Start;

            int qx1 = q.X - End.X, qy1 = q.Y - End.Y;
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
        #region Reverse

        /// <summary>
        /// Reverses the direction of the <see cref="LineI"/>.</summary>
        /// <returns>
        /// A new <see cref="LineI"/> instance whose <see cref="Start"/> property equals the <see
        /// cref="End"/> property of this instance, and vice versa.</returns>

        public LineI Reverse() {
            return new LineI(End, Start);
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="LineI"/>.</summary>
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
        /// Determines whether two <see cref="LineI"/> instances have the same value.</summary>
        /// <param name="a">
        /// The first <see cref="LineI"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="LineI"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(LineI)"/> method to test the two <see
        /// cref="LineI"/> instances for value equality.</remarks>

        public static bool operator==(LineI a, LineI b) {
            return a.Equals(b);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="LineI"/> instances have different values.</summary>
        /// <param name="a">
        /// The first <see cref="LineI"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="LineI"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different from the value of
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(LineI)"/> method to test the two <see
        /// cref="LineI"/> instances for value inequality.</remarks>

        public static bool operator!=(LineI a, LineI b) {
            return !a.Equals(b);
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="LineI"/> instances have the same value.</overloads>
        /// <summary>
        /// Determines whether this <see cref="LineI"/> instance and a specified object, which must
        /// be a <see cref="LineI"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="LineI"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="LineI"/> instance and its
        /// value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="LineI"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(LineI)"/> overload to test
        /// the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is LineI))
                return false;

            return Equals((LineI) obj);
        }

        #endregion
        #region Equals(LineI)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="LineI"/> have the same
        /// value.</summary>
        /// <param name="line">
        /// A <see cref="LineI"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="line"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="Start"/> and <see cref="End"/>
        /// properties of the two <see cref="LineI"/> instances to test for value equality.
        /// </remarks>

        public bool Equals(LineI line) {
            return (Start == line.Start && End == line.End);
        }

        #endregion
        #region Equals(LineI, LineI)

        /// <summary>
        /// Determines whether two specified <see cref="LineI"/> instances have the same value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="LineI"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="LineI"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(LineI)"/> overload to test the
        /// two <see cref="LineI"/> instances for value equality.</remarks>

        public static bool Equals(LineI a, LineI b) {
            return a.Equals(b);
        }

        #endregion
        #endregion
    }
}
