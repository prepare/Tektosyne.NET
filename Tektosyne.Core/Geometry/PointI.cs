using System;
using System.Globalization;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a location in two-dimensional space, using <see cref="Int32"/> coordinates.
    /// </summary>
    /// <remarks><para>
    /// <b>PointI</b> is an immutable structure whose two <see cref="Int32"/> coordinates define a
    /// mathematical point in two-dimensional space, or alternatively a two-dimensional vector.
    /// </para><para>
    /// Use the <see cref="PointF"/> structure to represent points with <see cref="Single"/>
    /// coordinates, and the <see cref="PointD"/> structure to represent points with <see
    /// cref="Double"/> coordinates.</para></remarks>

    [Serializable]
    public struct PointI: IEquatable<PointI> {
        #region PointI(Int32, Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="PointI"/> structure with the specified
        /// coordinates.</summary>
        /// <param name="x">
        /// The <see cref="X"/> coordinate of the <see cref="PointI"/>.</param>
        /// <param name="y">
        /// The <see cref="Y"/> coordinate of the <see cref="PointI"/>.</param>

        public PointI(int x, int y) {
            X = x;
            Y = y;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="PointI"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="PointI"/> instance that was created with the default
        /// constructor.</remarks>

        public static readonly PointI Empty = new PointI();

        #endregion
        #region X

        /// <summary>
        /// The x-coordinate of the <see cref="PointI"/>.</summary>

        public readonly int X;

        #endregion
        #region Y

        /// <summary>
        /// The y-coordinate of the <see cref="PointI"/>.</summary>

        public readonly int Y;

        #endregion
        #region Angle

        /// <summary>
        /// Gets the polar angle of the vector represented by the <see cref="PointI"/>, in radians.
        /// </summary>
        /// <value><para>
        /// The polar angle, in radians, of the vector represented by the <see cref="PointI"/>.
        /// </para><para>-or-</para><para>
        /// Zero if <see cref="X"/> and <see cref="Y"/> both equal zero.</para></value>
        /// <remarks>
        /// <b>Angle</b> returns the result of <see cref="Math.Atan2"/> for the <see cref="Y"/> and
        /// <see cref="X"/> coordinates. The possible range of values is (-<see cref="Math.PI"/>,
        /// +<see cref="Math.PI"/>].</remarks>

        public double Angle {
            get { return Math.Atan2(Y, X); }
        }

        #endregion
        #region Length

        /// <summary>
        /// Gets the absolute length of the vector represented by the <see cref="PointI"/>.
        /// </summary>
        /// <value>
        /// A non-negative <see cref="Double"/> value indicating the absolute length of the vector
        /// represented by the <see cref="PointI"/>.</value>
        /// <remarks>
        /// <b>Length</b> returns the square root of the sum of the squares of the <see cref="X"/>
        /// and <see cref="Y"/> coordinates.</remarks>

        public double Length {
            get { return Math.Sqrt(X * X + Y * Y); }
        }

        #endregion
        #region LengthSquared

        /// <summary>
        /// Gets the squared absolute length of the vector represented by the <see cref="PointF"/>.
        /// </summary>
        /// <value>
        /// A non-negative <see cref="Int32"/> value that equals the square of the <see
        /// cref="Length"/> property.</value>
        /// <remarks>
        /// <b>LengthSquared</b> performs the same operations as <see cref="Length"/> but without
        /// the final <see cref="Math.Sqrt"/> call, and is therefore faster if you only need the
        /// squared <see cref="Length"/>.</remarks>

        public int LengthSquared {
            get { return (X * X + Y * Y); }
        }

        #endregion
        #region Public Methods
        #region Add

        /// <summary>
        /// Adds the coordinates of the specified <see cref="PointI"/> to this instance.</summary>
        /// <param name="point">
        /// The <see cref="PointI"/> whose coordinates to add to this instance.</param>
        /// <returns>
        /// A <see cref="PointI"/> whose <see cref="X"/> and <see cref="Y"/> properties equal the
        /// corresponding properties of the specified <paramref name="point"/> added to those of
        /// this instance.</returns>

        public PointI Add(PointI point) {
            return new PointI(X + point.X, Y + point.Y);
        }

        #endregion
        #region AngleBetween(PointI)

        /// <overloads>
        /// Computes the angle between two <see cref="PointI"/> vectors.</overloads>
        /// <summary>
        /// Computes the angle between the vector represented by this instance and the specified
        /// <see cref="PointI"/> vector.</summary>
        /// <param name="vector">
        /// The <see cref="PointI"/> vector to compare with this instance.</param>
        /// <returns>
        /// The angle, in radians, between this instance and the specified <paramref
        /// name="vector"/>, in that order.</returns>
        /// <remarks>
        /// <b>AngleBetween</b> returns the result of <see cref="Math.Atan2"/> for the cross product
        /// length and the scalar dot product of the two vectors. The possible range of values is
        /// (-<see cref="Math.PI"/>, +<see cref="Math.PI"/>].</remarks>

        public double AngleBetween(PointI vector) {

            double y = X * vector.Y - Y * vector.X;
            double x = X * vector.X + Y * vector.Y;

            return Math.Atan2(y, x);
        }

        #endregion
        #region AngleBetween(PointI, PointI)

        /// <summary>
        /// Computes the angle between the vectors from this instance to the specified <see
        /// cref="PointI"/> coordinates.</summary>
        /// <param name="a">
        /// The <see cref="PointI"/> coordinates where the first vector ends.</param>
        /// <param name="b">
        /// The <see cref="PointI"/> coordinates where the second vector ends.</param>
        /// <returns>
        /// The angle, in radians, between the vectors from this instance to <paramref name="a"/>
        /// and from this instance to <paramref name="b"/>, in that order.</returns>
        /// <remarks>
        /// <b>AngleBetween</b> returns the result of <see cref="Math.Atan2"/> for the cross product
        /// length and the scalar dot product of the two vectors. The possible range of values is
        /// (-<see cref="Math.PI"/>, +<see cref="Math.PI"/>].</remarks>

        public double AngleBetween(PointI a, PointI b) {

            double ax = a.X - X, ay = a.Y - Y;
            double bx = b.X - X, by = b.Y - Y;

            double y = ax * by - ay * bx;
            double x = ax * bx + ay * by;

            return Math.Atan2(y, x);
        }

        #endregion
        #region CrossProductLength(PointI)

        /// <overloads>
        /// Computes the length of the cross-product of two <see cref="PointI"/> vectors.
        /// </overloads>
        /// <summary>
        /// Computes the length of the cross-product of the vector represented by this instance and
        /// the specified <see cref="PointI"/> vector.</summary>
        /// <param name="vector">
        /// The <see cref="PointI"/> vector to multiply with this instance.</param>
        /// <returns>
        /// An <see cref="Int32"/> value indicating the length of the cross-product of this instance
        /// and the specified <paramref name="vector"/>, in that order.</returns>
        /// <remarks>
        /// The absolute value of <b>CrossProductLength</b> equals the area of the parallelogram
        /// spanned by this instance and the specified <paramref name="vector"/>. The sign indicates
        /// their spatial relationship, which is described in the other <see
        /// cref="CrossProductLength(PointI, PointI)"/> overload.</remarks>

        public int CrossProductLength(PointI vector) {
            return X * vector.Y - vector.X * Y;
        }

        #endregion
        #region CrossProductLength(PointI, PointI)

        /// <summary>
        /// Computes the length of the cross-product of the vectors from this instance to the
        /// specified <see cref="PointI"/> coordinates.</summary>
        /// <param name="a">
        /// The <see cref="PointI"/> coordinates where the first vector ends.</param>
        /// <param name="b">
        /// The <see cref="PointI"/> coordinates where the second vector ends.</param>
        /// <returns>
        /// An <see cref="Int32"/> value indicating the length of the cross-product of the vectors
        /// from this instance to <paramref name="a"/> and from this instance to <paramref
        /// name="b"/>, in that order.</returns>
        /// <remarks><para>
        /// The absolute value of <b>CrossProductLength</b> equals the area of the parallelogram
        /// spanned by the two vectors from the current coordinates to <paramref name="a"/> and
        /// <paramref name="b"/>. The sign indicates their spatial relationship, as follows:
        /// </para><list type="table"><listheader>
        /// <term>Return Value</term><description>Relationship</description>
        /// </listheader><item>
        /// <term>Less than zero</term><description>
        /// The sequence from this instance to <paramref name="a"/> and then <paramref name="b"/>
        /// constitutes a right-hand turn, assuming y-coordinates increase upward.</description>
        /// </item><item>
        /// <term>Zero</term><description>
        /// This instance, <paramref name="a"/>, and <paramref name="b"/> are collinear.
        /// </description></item><item>
        /// <term>Greater than zero</term><description>
        /// The sequence from this instance to <paramref name="a"/> and then <paramref name="b"/>
        /// constitutes a left-hand turn, assuming y-coordinates increase upward.</description>
        /// </item></list></remarks>

        public int CrossProductLength(PointI a, PointI b) {
            return (a.X - X) * (b.Y - Y) - (b.X - X) * (a.Y - Y);
        }

        #endregion
        #region FromPolar

        /// <summary>
        /// Creates a <see cref="PointI"/> from the specified polar coordinates.</summary>
        /// <param name="length">
        /// The distance from the origin to the <see cref="PointI"/>.</param>
        /// <param name="angle">
        /// The polar angle, in radians, of the <see cref="PointI"/>.</param>
        /// <returns>
        /// A <see cref="PointI"/> whose <see cref="Length"/> and <see cref="Angle"/> equal the
        /// specified <paramref name="length"/> and <paramref name="angle"/>.</returns>
        /// <remarks><para>
        /// <b>FromPolar</b> returns <see cref="Empty"/> if the specified <paramref name="length"/>
        /// equals zero, and inverts the signs of the <see cref="X"/> and <see cref="Y"/>
        /// coordinates if <paramref name="length"/> is less than zero.
        /// </para><para>
        /// The calculated <see cref="X"/> and <see cref="Y"/> coordinates are converted to the
        /// nearest <see cref="Int32"/> values using <see cref="Fortran.NInt"/> rounding. The
        /// resulting <see cref="Length"/> and <see cref="Angle"/> may differ accordingly from the
        /// specified <paramref name="length"/> and <paramref name="angle"/>.</para></remarks>

        public static PointI FromPolar(double length, double angle) {
            return new PointI(
                Fortran.NInt(length * Math.Cos(angle)),
                Fortran.NInt(length * Math.Sin(angle)));
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="PointI"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the values of the <see cref="X"/> and <see cref="Y"/>
        /// properties.</remarks>

        public override int GetHashCode() {
            unchecked { return X ^ Y; }
        }

        #endregion
        #region IsCollinear

        /// <summary>
        /// Determines whether this instance is collinear with the specified <see cref="PointI"/>
        /// instances.</summary>
        /// <param name="a">
        /// The first <see cref="PointI"/> instance to examine.</param>
        /// <param name="b">
        /// The second <see cref="PointI"/> instance to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="PointI"/> is collinear with <paramref name="a"/> and
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>IsCollinear</b> returns <c>true</c> exactly if <see cref="CrossProductLength(PointI,
        /// PointI)"/> returns zero for <paramref name="a"/> and <paramref name="b"/>.</remarks>

        public bool IsCollinear(PointI a, PointI b) {
            return (CrossProductLength(a, b) == 0);
        }

        #endregion
        #region Multiply

        /// <summary>
        /// Multiplies the vectors represented by the specified <see cref="PointI"/> and by this
        /// instance.</summary>
        /// <param name="vector">
        /// The <see cref="PointI"/> to multiply with this instance.</param>
        /// <returns>
        /// An <see cref="Int32"/> value that represents the scalar dot product of the specified
        /// <paramref name="vector"/> and this instance.</returns>
        /// <remarks>
        /// <b>Multiply</b> returns the sum of the pairwise products of the <see cref="X"/> and <see
        /// cref="Y"/> coordinates of both instances. That sum equals <see cref="LengthSquared"/> if
        /// the specified <paramref name="vector"/> equals this instance.</remarks>

        public int Multiply(PointI vector) {
            return X * vector.X + Y * vector.Y;
        }

        #endregion
        #region Restrict

        /// <summary>
        /// Restricts the <see cref="PointI"/> to the specified coordinate range.</summary>
        /// <param name="minX">
        /// The smallest permissible <see cref="X"/> coordinate.</param>
        /// <param name="minY">
        /// The smallest permissible <see cref="Y"/> coordinate.</param>
        /// <param name="maxX">
        /// The greatest permissible <see cref="X"/> coordinate.</param>
        /// <param name="maxY">
        /// The greatest permissible <see cref="Y"/> coordinate.</param>
        /// <returns>
        /// A <see cref="PointI"/> whose <see cref="X"/> and <see cref="Y"/> coordinates equal those
        /// of this instance, restricted to the indicated coordinate range.</returns>

        public PointI Restrict(int minX, int minY, int maxX, int maxY) {
            int x = X, y = Y;

            if (x < minX) x = minX; else if (x > maxX) x = maxX;
            if (y < minY) y = minY; else if (y > maxY) y = maxY;

            return new PointI(x, y);
        }

        #endregion
        #region Subtract

        /// <summary>
        /// Subtracts the coordinates of the specified <see cref="PointI"/> from this instance.
        /// </summary>
        /// <param name="point">
        /// The <see cref="PointI"/> whose coordinates to subtract from this instance.</param>
        /// <returns>
        /// A <see cref="PointI"/> whose <see cref="X"/> and <see cref="Y"/> properties equal the
        /// corresponding properties of the specified <paramref name="point"/> subtracted from those
        /// of this instance.</returns>

        public PointI Subtract(PointI point) {
            return new PointI(X - point.X, Y - point.Y);
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="PointI"/>.</summary>
        /// <returns>
        /// A <see cref="String"/> containing the values of the <see cref="X"/> and <see cref="Y"/>
        /// properties.</returns>

        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture, "{{X={0}, Y={1}}}", X, Y);
        }

        #endregion
        #endregion
        #region Public Operators
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="PointI"/> instances have the same value.</summary>
        /// <param name="a">
        /// The first <see cref="PointI"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="PointI"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(PointI)"/> method to test the two <see
        /// cref="PointI"/> instances for value equality.</remarks>

        public static bool operator ==(PointI a, PointI b) {
            return a.Equals(b);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="PointI"/> instances have different values.</summary>
        /// <param name="a">
        /// The first <see cref="PointI"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="PointI"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different from the value of
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(PointI)"/> method to test the two <see
        /// cref="PointI"/> instances for value inequality.</remarks>

        public static bool operator !=(PointI a, PointI b) {
            return !a.Equals(b);
        }

        #endregion
        #region operator+

        /// <summary>
        /// Adds the coordinates of two <see cref="PointI"/> instances.</summary>
        /// <param name="a">
        /// The first <see cref="PointI"/> to add.</param>
        /// <param name="b">
        /// The second <see cref="PointI"/> to add.</param>
        /// <returns>
        /// A <see cref="PointI"/> whose <see cref="X"/> and <see cref="Y"/> properties equal the
        /// corresponding properties of <paramref name="a"/> added to those of <paramref name="b"/>.
        /// </returns>
        /// <remarks>
        /// This operator invokes <see cref="Add"/> to add the coordinates of the two <see
        /// cref="PointI"/> instances.</remarks>

        public static PointI operator +(PointI a, PointI b) {
            return a.Add(b);
        }

        #endregion
        #region operator*

        /// <summary>
        /// Multiplies the vectors represented by two <see cref="PointI"/> instances.</summary>
        /// <param name="a">
        /// The first <see cref="PointI"/> to multiply.</param>
        /// <param name="b">
        /// The second <see cref="PointI"/> to multiply.</param>
        /// <returns>
        /// An <see cref="Int32"/> value that represents the scalar dot product of <paramref
        /// name="a"/> and <paramref name="b"/>.</returns>
        /// <remarks>
        /// This operator invokes <see cref="Multiply"/> to multiply the vectors represented by the
        /// two <see cref="PointI"/> instances.</remarks>

        public static int operator *(PointI a, PointI b) {
            return a.Multiply(b);
        }

        #endregion
        #region operator-

        /// <summary>
        /// Subtracts the coordinates of two <see cref="PointI"/> instances.</summary>
        /// <param name="a">
        /// The <see cref="PointI"/> to subtract from.</param>
        /// <param name="b">
        /// The <see cref="PointI"/> to subtract.</param>
        /// <returns>
        /// A <see cref="PointI"/> whose <see cref="X"/> and <see cref="Y"/> properties equal the
        /// corresponding properties of <paramref name="a"/> subtracted from those of <paramref
        /// name="b"/>.</returns>
        /// <remarks>
        /// This operator invokes <see cref="Subtract"/> to subtract the coordinates of the two <see
        /// cref="PointI"/> instances.</remarks>

        public static PointI operator -(PointI a, PointI b) {
            return a.Subtract(b);
        }

        #endregion
        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="PointI"/> instances have the same value.</overloads>
        /// <summary>
        /// Determines whether this <see cref="PointI"/> instance and a specified object, which must
        /// be a <see cref="PointI"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="PointI"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="PointI"/> instance and its
        /// value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="PointI"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(PointI)"/> overload to test
        /// the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is PointI))
                return false;

            return Equals((PointI) obj);
        }

        #endregion
        #region Equals(PointI)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="PointI"/> have the same
        /// value.</summary>
        /// <param name="point">
        /// A <see cref="PointI"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="point"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="X"/> and <see cref="Y"/> properties
        /// of the two <see cref="PointI"/> instances to test for value equality.</remarks>

        public bool Equals(PointI point) {
            return (X == point.X && Y == point.Y);
        }

        #endregion
        #region Equals(PointI, PointI)

        /// <summary>
        /// Determines whether two specified <see cref="PointI"/> instances have the same value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="PointI"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="PointI"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(PointI)"/> overload to test the
        /// two <see cref="PointI"/> instances for value equality.</remarks>

        public static bool Equals(PointI a, PointI b) {
            return a.Equals(b);
        }

        #endregion
        #endregion
    }
}
