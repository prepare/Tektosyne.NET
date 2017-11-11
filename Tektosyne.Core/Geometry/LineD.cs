using System;
using System.Globalization;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a directed line segment in two-dimensional space, using <see cref="Double"/>
    /// coordinates.</summary>
    /// <remarks><para>
    /// <b>LineD</b> is an immutable structure whose four <see cref="Double"/> coordinates indicate
    /// the <see cref="LineD.Start"/> and <see cref="LineD.End"/> points of a directed line segment.
    /// Its <see cref="LineD.Angle"/> and <see cref="LineD.Length"/> are calculated on demand.
    /// </para><para>
    /// Use the <see cref="LineI"/> structure to represent line segments with <see cref="Int32"/>
    /// coordinates, and the <see cref="LineF"/> structure to represent line segments with <see
    /// cref="Single"/> coordinates. You can convert <see cref="LineD"/> instances to and from <see
    /// cref="LineF"/> and <see cref="LineI"/> instances, rounding off the <see cref="Double"/> 
    /// coordinates as necessary.</para></remarks>

    [Serializable]
    public struct LineD: IEquatable<LineD> {
        #region LineD(Double, Double, Double, Double)

        /// <overloads>
        /// Initializes a new instance of the <see cref="LineD"/> structure.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="LineD"/> structure with the specified start
        /// and end coordinates.</summary>
        /// <param name="startX">
        /// The <see cref="PointD.X"/> coordinate of the <see cref="Start"/> point.</param>
        /// <param name="startY">
        /// The <see cref="PointD.Y"/> coordinate of the <see cref="Start"/> point.</param>
        /// <param name="endX">
        /// The <see cref="PointD.X"/> coordinate of the <see cref="End"/> point.</param>
        /// <param name="endY">
        /// The the <see cref="PointD.Y"/> coordinate of the <see cref="End"/> point.</param>

        public LineD(double startX, double startY, double endX, double endY) {
            Start = new PointD(startX, startY);
            End = new PointD(endX, endY);
        }

        #endregion
        #region LineD(PointD, PointD)

        /// <summary>
        /// Initializes a new instance of the <see cref="LineD"/> structure with the specified start
        /// and end coordinates.</summary>
        /// <param name="start">
        /// The <see cref="Start"/> point of the <see cref="LineD"/>.</param>
        /// <param name="end">
        /// The <see cref="End"/> point of the <see cref="LineD"/>.</param>

        public LineD(PointD start, PointD end) {
            Start = start;
            End = end;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="LineD"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="LineD"/> instance that was created with the default
        /// constructor.</remarks>

        public static readonly LineD Empty = new LineD();

        #endregion
        #region Public Properties
        #region Start

        /// <summary>
        /// The start coordinates of the <see cref="LineD"/>.</summary>

        public readonly PointD Start;

        #endregion
        #region End

        /// <summary>
        /// The end coordinates of the <see cref="LineD"/>.</summary>

        public readonly PointD End;

        #endregion
        #region Angle

        /// <summary>
        /// Gets the angle of the <see cref="LineD"/>, in radians.</summary>
        /// <value><para>
        /// The angle, in radians, of the direction on the Cartesian plane into which the line
        /// segment represented by the <see cref="LineD"/> is pointing.
        /// </para><para>-or-</para><para>
        /// Zero if <see cref="Length"/> equals zero.</para></value>
        /// <remarks><para>
        /// <b>Angle</b> returns the result of <see cref="Math.Atan2"/> for the vertical and
        /// horizontal distances between the <see cref="End"/> and <see cref="Start"/> points.
        /// </para><para>
        /// <b>Angle</b> equals zero if the <see cref="LineD"/> extends horizontally to the right,
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
        /// Gets the inverse slope of the <see cref="LineD"/>.</summary>
        /// <value><para>
        /// The quotient of the horizontal and the vertical extent of the <see cref="LineD"/>.
        /// </para><para>-or-</para><para>
        /// <see cref="Double.MaxValue"/> if the <see cref="LineD"/> is horizontal.</para></value>
        /// <remarks>
        /// <b>InverseSlope</b> may return a negative value, depending on the <see cref="Angle"/> of
        /// the <see cref="LineD"/>. The value of this property equals 1/<see cref="Slope"/>.
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
        /// Gets the absolute length of the <see cref="LineD"/>.</summary>
        /// <value>
        /// A non-negative <see cref="Double"/> value indicating the absolute length of the line
        /// segment represented by the <see cref="LineD"/>.</value>
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
        /// Gets the squared absolute length of the <see cref="LineD"/>.</summary>
        /// <value>
        /// A non-negative <see cref="Double"/> value that equals the square of the <see
        /// cref="Length"/> property.</value>
        /// <remarks>
        /// <b>LengthSquared</b> performs the same operations as <see cref="Length"/> but without
        /// the final <see cref="Math.Sqrt"/> call, and is therefore faster if you only need the
        /// squared <see cref="Length"/>.</remarks>

        public double LengthSquared {
            get {
                double x = End.X - Start.X;
                double y = End.Y - Start.Y;
                return (x * x + y * y);
            }
        }

        #endregion
        #region Slope

        /// <summary>
        /// Gets the slope of the <see cref="LineD"/>.</summary>
        /// <value><para>
        /// The quotient of the vertical and the horizontal extent of the <see cref="LineD"/>.
        /// </para><para>-or-</para><para>
        /// <see cref="Double.MaxValue"/> if the <see cref="LineD"/> is vertical.</para></value>
        /// <remarks>
        /// <b>Slope</b> may return a negative value, depending on the <see cref="Angle"/> of the
        /// <see cref="LineD"/>. The value of this property equals 1/<see cref="InverseSlope"/>.
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
        /// Gets the vector defined by the <see cref="LineD"/>.</summary>
        /// <value>
        /// A <see cref="PointD"/> value defined as (<see cref="End"/> - <see cref="Start"/>).
        /// </value>
        /// <remarks>
        /// <b>Vector</b> returns the <see cref="End"/> point of the <see cref="LineD"/> with its
        /// <see cref="Start"/> point shifted to the origins of the coordinate system.</remarks>

        public PointD Vector {
            get { return new PointD(End.X - Start.X, End.Y - Start.Y); }
        }

        #endregion
        #endregion
        #region Public Methods
        #region DistanceSquared

        /// <summary>
        /// Determines the squared distance between the <see cref="LineD"/> and the specified <see
        /// cref="PointD"/> coordinates.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <returns>
        /// The squared distance between <paramref name="q"/> and the <see cref="LineD"/>.</returns>
        /// <remarks>
        /// <b>DistanceSquared</b> returns either the squared length of the perpendicular dropped
        /// from <paramref name="q"/> on the <see cref="LineD"/>, or the squared distance between
        /// <paramref name="q"/> and <see cref="Start"/> or <see cref="End"/> if the perpendicular
        /// does not intersect the <see cref="LineD"/>.</remarks>

        public double DistanceSquared(PointD q) {
            if (q == Start || q == End) return 0;

            double x = Start.X, y = Start.Y;
            double ax = End.X - x, ay = End.Y - y;

            // set (x,y) to nearest LineD point from q
            if (ax != 0 || ay != 0) {
                double u = ((q.X - x) * ax + (q.Y - y) * ay) / (ax * ax + ay * ay);
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
        /// Finds the x-coordinate for the specified y-coordinate on the <see cref="LineD"/> or its
        /// infinite extension.</summary>
        /// <param name="y">
        /// The y-coordinate on the <see cref="LineD"/> to examine.</param>
        /// <returns><para>
        /// The x-coordinate for the specified <paramref name="y"/> coordinate on the <see
        /// cref="LineD"/> or its infinite extension.
        /// </para><para>-or-</para><para>
        /// <see cref="Double.MaxValue"/> if the <see cref="LineD"/> is horizontal.</para></returns>
        /// <remarks>
        /// <b>FindX</b> returns the <see cref="PointD.X"/> component of <see cref="Start"/> or <see
        /// cref="End"/> if the specified <paramref name="y"/> coordinate exactly equals the
        /// corresponding <see cref="PointD.Y"/> component; otherwise, a computed x-coordinate.
        /// </remarks>

        public double FindX(double y) {

            double d = End.Y - Start.Y;
            if (d == 0) return Double.MaxValue;

            if (y == Start.Y) return Start.X;
            if (y == End.Y) return End.X;

            return Start.X + (y - Start.Y) * (End.X - Start.X) / d;
        }

        #endregion
        #region FindY

        /// <summary>
        /// Finds the y-coordinate for the specified x-coordinate on the <see cref="LineD"/> or its
        /// infinite extension.</summary>
        /// <param name="x">
        /// The x-coordinate on the <see cref="LineD"/> to examine.</param>
        /// <returns><para>
        /// The y-coordinate for the specified <paramref name="x"/> coordinate on the <see
        /// cref="LineD"/> or its infinite extension.
        /// </para><para>-or-</para><para>
        /// <see cref="Double.MaxValue"/> if the <see cref="LineD"/> is vertical.</para></returns>
        /// <remarks>
        /// <b>FindY</b> returns the <see cref="PointD.Y"/> component of <see cref="Start"/> or <see
        /// cref="End"/> if the specified <paramref name="x"/> coordinate exactly equals the
        /// corresponding <see cref="PointD.X"/> component; otherwise, a computed y-coordinate.
        /// </remarks>

        public double FindY(double x) {

            double d = End.X - Start.X;
            if (d == 0) return Double.MaxValue;

            if (x == Start.X) return Start.Y;
            if (x == End.X) return End.Y;

            return Start.Y + (x - Start.X) * (End.Y - Start.Y) / d;
        }

        #endregion
        #region FromIndexPoints

        /// <summary>
        /// Converts the specified collections of <see cref="PointD"/> coordinates and zero-based
        /// indices into the equivalent <see cref="LineD"/> instances.</summary>
        /// <param name="points">
        /// An <see cref="Array"/> containing all <see cref="PointD"/> coordinates that represent
        /// <see cref="Start"/> or <see cref="End"/> points.</param>
        /// <param name="indices">
        /// An <see cref="Array"/> containing all pairs of zero-based indices within <paramref
        /// name="points"/> that represent <see cref="LineD"/> instances.</param>
        /// <returns>
        /// An <see cref="Array"/> containing all <see cref="LineD"/> instances represented by the
        /// combination of <paramref name="points"/> and <paramref name="indices"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="points"/> or <paramref name="indices"/> is a null reference.</exception>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="indices"/> contains an index that is less than zero, or equal to or
        /// greater than the number of elements in <paramref name="points"/>.</exception>

        public static LineD[] FromIndexPoints(PointD[] points, PointI[] indices) {
            if (points == null)
                ThrowHelper.ThrowArgumentNullException("points");
            if (indices == null)
                ThrowHelper.ThrowArgumentNullException("indices");

            LineD[] lines = new LineD[indices.Length];

            for (int i = 0; i < indices.Length; i++)
                lines[i] = new LineD(points[indices[i].X], points[indices[i].Y]);

            return lines;
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="LineD"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the <see cref="PointD.X"/> and <see cref="PointD.Y"/>
        /// components of the <see cref="Start"/> and <see cref="End"/> properties.</remarks>

        public override unsafe int GetHashCode() {
            unchecked {
                double x0 = Start.X, y0 = Start.Y, x1 = End.X, y1 = End.Y;
                long x0i = *((long*) &x0), y0i = *((long*) &y0);
                long x1i = *((long*) &x1), y1i = *((long*) &y1);

                return (int) x0i ^ (int) (x0i >> 32) ^ (int) y0i ^ (int) (y0i >> 32)
                    ^ (int) x1i ^ (int) (x1i >> 32) ^ (int) y1i ^ (int) (y1i >> 32);
            }
        }

        #endregion
        #region Intersect(LineD)

        /// <overloads>
        /// Finds the intersection between the <see cref="LineD"/> and a specified line segment.
        /// </overloads>
        /// <summary>
        /// Finds the intersection between this instance and a specified <see cref="LineD"/>, using
        /// exact coordinate comparisons.</summary>
        /// <param name="line">
        /// A <see cref="LineD"/> to intersect with this instance.</param>
        /// <returns>
        /// A <see cref="LineIntersection"/> instance that describes if and how the specified
        /// <paramref name="line"/> and this instance intersect.</returns>
        /// <remarks>
        /// <b>Intersect</b> returns the result of <see cref="LineIntersection.Find(PointD, PointD,
        /// PointD, PointD)"/> for the <see cref="Start"/> and <see cref="End"/> points of this
        /// instance and of the specified <paramref name="line"/>, in that order.</remarks>

        public LineIntersection Intersect(LineD line) {
            return LineIntersection.Find(Start, End, line.Start, line.End);
        }

        #endregion
        #region Intersect(LineD, Double)

        /// <summary>
        /// Finds the intersection between this instance and a specified <see cref="LineD"/>, given
        /// the specified epsilon for coordinate comparisons.</summary>
        /// <param name="line">
        /// A <see cref="LineD"/> to intersect with this instance.</param>
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

        public LineIntersection Intersect(LineD line, double epsilon) {
            return LineIntersection.Find(Start, End, line.Start, line.End, epsilon);
        }

        #endregion
        #region Intersect(PointD)

        /// <summary>
        /// Finds the intersection between the <see cref="LineD"/> and the perpendicular dropped
        /// from the specified <see cref="PointD"/> coordinates.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <returns>
        /// The <see cref="PointD"/> coordinates where the perpendicular dropped from <paramref
        /// name="q"/> intersects the <see cref="LineD"/> or its infinite extension.</returns>
        /// <remarks>
        /// <b>Intersect</b> returns <see cref="Start"/> if <see cref="Length"/> equals zero.
        /// </remarks>

        public PointD Intersect(PointD q) {

            if (q == Start) return Start;
            if (q == End) return End;

            double x = Start.X, y = Start.Y;
            double ax = End.X - x, ay = End.Y - y;
            if (ax == 0 && ay == 0) return Start;

            double u = ((q.X - x) * ax + (q.Y - y) * ay) / (ax * ax + ay * ay);
            return new PointD(x + u * ax, y + u * ay);
        }

        #endregion
        #region Locate(PointD)

        /// <overloads>
        /// Determines the location of the specified <see cref="PointD"/> coordinates relative to
        /// the <see cref="LineD"/>.</overloads>
        /// <summary>
        /// Determines the location of the specified <see cref="PointD"/> coordinates relative to
        /// the <see cref="LineD"/>, using exact coordinate comparisons.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="LineLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="LineD"/>.</returns>
        /// <remarks><para>
        /// <b>Locate</b> never returns <see cref="LineLocation.None"/>. The values <see
        /// cref="LineLocation.Left"/> and <see cref="LineLocation.Right"/> assume that
        /// y-coordinates increase upward.
        /// </para><para>
        /// <b>Locate</b> is based on the <c>classify</c> algorithm by Michael J. Laszlo,
        /// <em>Computational Geometry and Computer Graphics in C++</em>, Prentice Hall 1996, p.76.
        /// </para></remarks>

        public LineLocation Locate(PointD q) {

            double qx0 = q.X - Start.X, qy0 = q.Y - Start.Y;
            if (qx0 == 0 && qy0 == 0)
                return LineLocation.Start;

            double qx1 = q.X - End.X, qy1 = q.Y - End.Y;
            if (qx1 == 0 && qy1 == 0)
                return LineLocation.End;

            double ax = End.X - Start.X, ay = End.Y - Start.Y;
            double area = ax * qy0 - qx0 * ay;
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
        #region Locate(PointD, Double)

        /// <summary>
        /// Determines the location of the specified <see cref="PointD"/> coordinates relative to
        /// the <see cref="LineD"/>, given the specified epsilon for coordinate comparisons.
        /// </summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <param name="epsilon">
        /// The maximum absolute value at which intermediate results should be considered zero.
        /// </param>
        /// <returns>
        /// A <see cref="LineLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="LineD"/>.</returns>
        /// <remarks><para>
        /// <b>Locate</b> is identical with the basic <see cref="Locate(PointD)"/> overload but uses
        /// the specified <paramref name="epsilon"/> to compare intermediate results to zero.
        /// </para><para>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but <b>Locate</b>
        /// does not check this condition.</para></remarks>

        public LineLocation Locate(PointD q, double epsilon) {

            double qx0 = q.X - Start.X, qy0 = q.Y - Start.Y;
            if (Math.Abs(qx0) <= epsilon && Math.Abs(qy0) <= epsilon)
                return LineLocation.Start;

            double qx1 = q.X - End.X, qy1 = q.Y - End.Y;
            if (Math.Abs(qx1) <= epsilon && Math.Abs(qy1) <= epsilon)
                return LineLocation.End;

            double ax = End.X - Start.X, ay = End.Y - Start.Y;
            double area = ax * qy0 - qx0 * ay;
            double epsilon2 = epsilon * (Math.Abs(ax) + Math.Abs(ay));
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
        #region LocateCollinear(PointD)

        /// <overloads>
        /// Determines the location of the specified <see cref="PointD"/> coordinates relative to
        /// the <see cref="LineD"/>, assuming they are collinear.</overloads>
        /// <summary>
        /// Determines the location of the specified <see cref="PointD"/> coordinates relative to
        /// the <see cref="LineD"/>, assuming they are collinear and using exact coordinate
        /// comparisons.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="LineLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="LineD"/>.</returns>
        /// <remarks>
        /// <b>LocateCollinear</b> is identical with <see cref="Locate"/> but assumes that <paramref
        /// name="q"/> is collinear with the <see cref="LineD"/>, and therefore never returns the
        /// values <see cref="LineLocation.Left"/> or <see cref="LineLocation.Right"/>.</remarks>

        public LineLocation LocateCollinear(PointD q) {

            double qx0 = q.X - Start.X, qy0 = q.Y - Start.Y;
            if (qx0 == 0 && qy0 == 0)
                return LineLocation.Start;

            double qx1 = q.X - End.X, qy1 = q.Y - End.Y;
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
        #region LocateCollinear(PointD, Double)

        /// <summary>
        /// Determines the location of the specified <see cref="PointD"/> coordinates relative to
        /// the <see cref="LineD"/>, assuming they are collinear and given the specified epsilon for
        /// coordinate comparisons.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <param name="epsilon">
        /// The maximum absolute value at which intermediate results should be considered zero.
        /// </param>
        /// <returns>
        /// A <see cref="LineLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the <see cref="LineD"/>.</returns>
        /// <remarks><para>
        /// <b>LocateCollinear</b> is identical with the basic <see cref="LocateCollinear(PointD)"/>
        /// overload but uses the specified <paramref name="epsilon"/> to compare <paramref
        /// name="q"/> to the <see cref="Start"/> and <see cref="End"/> points.
        /// </para><para>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but
        /// <b>LocateCollinear</b> does not check this condition.</para></remarks>

        public LineLocation LocateCollinear(PointD q, double epsilon) {

            double qx0 = q.X - Start.X, qy0 = q.Y - Start.Y;
            if (Math.Abs(qx0) <= epsilon && Math.Abs(qy0) <= epsilon)
                return LineLocation.Start;

            double qx1 = q.X - End.X, qy1 = q.Y - End.Y;
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
        /// Reverses the direction of the <see cref="LineD"/>.</summary>
        /// <returns>
        /// A new <see cref="LineD"/> instance whose <see cref="Start"/> property equals the <see
        /// cref="End"/> property of this instance, and vice versa.</returns>

        public LineD Reverse() {
            return new LineD(End, Start);
        }

        #endregion
        #region Round

        /// <summary>
        /// Converts the <see cref="LineD"/> to a <see cref="LineI"/> by rounding coordinates to the
        /// nearest <see cref="Int32"/> values.</summary>
        /// <returns>
        /// A <see cref="LineI"/> instance whose <see cref="LineI.Start"/> and <see
        /// cref="LineI.End"/> properties equal the corresponding properties of the <see
        /// cref="LineD"/>, rounded to the nearest <see cref="Int32"/> values.</returns>
        /// <remarks>
        /// The <see cref="Double"/> coordinates of the <see cref="LineD"/> are converted to <see
        /// cref="Int32"/> coordinates using <see cref="Fortran.NInt"/> rounding.</remarks>

        public LineI Round() {
            return new LineI(
                Fortran.NInt(Start.X), Fortran.NInt(Start.Y),
                Fortran.NInt(End.X), Fortran.NInt(End.Y));
        }

        #endregion
        #region ToLineF

        /// <summary>
        /// Converts the <see cref="LineD"/> to a <see cref="LineF"/> by casting coordinates to the
        /// equivalent <see cref="Single"/> values.</summary>
        /// <returns>
        /// A <see cref="LineF"/> instance whose <see cref="LineF.Start"/> and <see
        /// cref="LineF.End"/> properties equal the corresponding properties of the <see
        /// cref="LineD"/>, cast to the equivalent <see cref="Single"/> values.</returns>

        public LineF ToLineF() {
            return new LineF((float) Start.X, (float) Start.Y, (float) End.X, (float) End.Y);
        }

        #endregion
        #region ToLineI

        /// <summary>
        /// Converts the <see cref="LineD"/> to a <see cref="LineI"/> by truncating coordinates to 
        /// the nearest <see cref="Int32"/> values.</summary>
        /// <returns>
        /// A <see cref="LineI"/> instance whose <see cref="LineI.Start"/> and <see
        /// cref="LineI.End"/> properties equal the corresponding properties of the <see
        /// cref="LineD"/>, truncated to the nearest <see cref="Int32"/> values.</returns>

        public LineI ToLineI() {
            return new LineI((int) Start.X, (int) Start.Y, (int) End.X, (int) End.Y);
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="LineD"/>.</summary>
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
        #region Public Operators
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="LineD"/> instances have the same value.</summary>
        /// <param name="a">
        /// The first <see cref="LineD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="LineD"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(LineD)"/> method to test the two <see
        /// cref="LineD"/> instances for value equality.</remarks>

        public static bool operator ==(LineD a, LineD b) {
            return a.Equals(b);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="LineD"/> instances have different values.</summary>
        /// <param name="a">
        /// The first <see cref="LineD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="LineD"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different from the value of
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(LineD)"/> method to test the two <see
        /// cref="LineD"/> instances for value inequality.</remarks>

        public static bool operator !=(LineD a, LineD b) {
            return !a.Equals(b);
        }

        #endregion
        #region LineD(LineF)

        /// <summary>
        /// Converts a <see cref="LineF"/> to a <see cref="LineD"/> with identical coordinates.
        /// </summary>
        /// <param name="line">
        /// The <see cref="LineF"/> instance to convert into a <see cref="LineD"/> instance.</param>
        /// <returns>
        /// A <see cref="LineD"/> instance whose <see cref="Start"/> and <see cref="End"/>
        /// properties equal the corresponding properties of the specified <paramref name="line"/>.
        /// </returns>

        public static implicit operator LineD(LineF line) {
            return new LineD(line.Start.X, line.Start.Y, line.End.X, line.End.Y);
        }

        #endregion
        #region LineF(LineD)

        /// <summary>
        /// Converts a <see cref="LineD"/> to a <see cref="LineF"/> by casting coordinates to the
        /// equivalent <see cref="Single"/> values.</summary>
        /// <param name="line">
        /// The <see cref="LineD"/> instance to convert into a <see cref="LineI"/> instance.</param>
        /// <returns>
        /// A <see cref="LineF"/> instance whose <see cref="Start"/> and <see cref="End"/>
        /// properties equal the corresponding properties of the specified <paramref name="line"/>,
        /// cast to the equivalent <see cref="Single"/> values.</returns>

        public static explicit operator LineF(LineD line) {
            return line.ToLineF();
        }

        #endregion
        #region LineD(LineI)

        /// <summary>
        /// Converts a <see cref="LineI"/> to a <see cref="LineD"/> with identical coordinates.
        /// </summary>
        /// <param name="line">
        /// The <see cref="LineI"/> instance to convert into a <see cref="LineD"/> instance.</param>
        /// <returns>
        /// A <see cref="LineD"/> instance whose <see cref="Start"/> and <see cref="End"/>
        /// properties equal the corresponding properties of the specified <paramref name="line"/>.
        /// </returns>

        public static implicit operator LineD(LineI line) {
            return new LineD(line.Start.X, line.Start.Y, line.End.X, line.End.Y);
        }

        #endregion
        #region LineI(LineD)

        /// <summary>
        /// Converts a <see cref="LineD"/> to a <see cref="LineI"/> by truncating coordinates to the
        /// nearest <see cref="Int32"/> values.</summary>
        /// <param name="line">
        /// The <see cref="LineD"/> instance to convert into a <see cref="LineI"/> instance.</param>
        /// <returns>
        /// A <see cref="LineI"/> instance whose <see cref="LineI.Start"/> and <see
        /// cref="LineI.End"/> properties equal the corresponding properties of the specified
        /// <paramref name="line"/>, truncated to the nearest <see cref="Int32"/> values.</returns>

        public static explicit operator LineI(LineD line) {
            return line.ToLineI();
        }

        #endregion
        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="LineD"/> instances have the same value.</overloads>
        /// <summary>
        /// Determines whether this <see cref="LineD"/> instance and a specified object, which must
        /// be a <see cref="LineD"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="LineD"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="LineD"/> instance and its
        /// value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="LineD"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(LineD)"/> overload to test
        /// the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is LineD))
                return false;

            return Equals((LineD) obj);
        }

        #endregion
        #region Equals(LineD)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="LineD"/> have the same
        /// value.</summary>
        /// <param name="line">
        /// A <see cref="LineD"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="line"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="Start"/> and <see cref="End"/>
        /// properties of the two <see cref="LineD"/> instances to test for value equality.
        /// </remarks>

        public bool Equals(LineD line) {
            return (Start == line.Start && End == line.End);
        }

        #endregion
        #region Equals(LineD, LineD)

        /// <summary>
        /// Determines whether two specified <see cref="LineD"/> instances have the same value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="LineD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="LineD"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(LineD)"/> overload to test the
        /// two <see cref="LineD"/> instances for value equality.</remarks>

        public static bool Equals(LineD a, LineD b) {
            return a.Equals(b);
        }

        #endregion
        #region Equals(LineD, LineD, Single)

        /// <summary>
        /// Determines whether two specified <see cref="LineD"/> instances have the same value,
        /// given the specified epsilon.</summary>
        /// <param name="a">
        /// The first <see cref="LineD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="LineD"/> to compare.</param>
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

        public static bool Equals(LineD a, LineD b, double epsilon) {

            return (PointD.Equals(a.Start, b.Start, epsilon)
                && PointD.Equals(a.End, b.End, epsilon));
        }

        #endregion
        #endregion
    }
}
