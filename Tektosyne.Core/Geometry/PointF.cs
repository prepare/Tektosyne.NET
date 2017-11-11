using System;
using System.Globalization;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a location in two-dimensional space, using <see cref="Single"/> coordinates.
    /// </summary>
    /// <remarks><para>
    /// <b>PointF</b> is an immutable structure whose two <see cref="Single"/> coordinates define a
    /// mathematical point in two-dimensional space, or alternatively a two-dimensional vector.
    /// </para><para>
    /// Use the <see cref="PointI"/> structure to represent points with <see cref="Int32"/>
    /// coordinates, and the <see cref="PointD"/> structure to represent points with <see
    /// cref="Double"/> coordinates. You can convert <see cref="PointF"/> instances to and from <see
    /// cref="PointI"/> instances, rounding off the <see cref="Single"/> coordinates as necessary.
    /// </para></remarks>

    [Serializable]
    public struct PointF: IEquatable<PointF> {
        #region PointF(Single, Single)

        /// <summary>
        /// Initializes a new instance of the <see cref="PointF"/> structure with the specified
        /// coordinates.</summary>
        /// <param name="x">
        /// The <see cref="X"/> coordinate of the <see cref="PointF"/>.</param>
        /// <param name="y">
        /// The <see cref="Y"/> coordinate of the <see cref="PointF"/>.</param>

        public PointF(float x, float y) {
            X = x;
            Y = y;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="PointF"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="PointF"/> instance that was created with the default
        /// constructor.</remarks>

        public static readonly PointF Empty = new PointF();

        #endregion
        #region X

        /// <summary>
        /// The x-coordinate of the <see cref="PointF"/>.</summary>

        public readonly float X;

        #endregion
        #region Y

        /// <summary>
        /// The y-coordinate of the <see cref="PointF"/>.</summary>

        public readonly float Y;

        #endregion
        #region Angle

        /// <summary>
        /// Gets the polar angle of the vector represented by the <see cref="PointF"/>, in radians.
        /// </summary>
        /// <value><para>
        /// The polar angle, in radians, of the vector represented by the <see cref="PointF"/>.
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
        /// Gets the absolute length of the vector represented by the <see cref="PointF"/>.
        /// </summary>
        /// <value>
        /// A non-negative <see cref="Double"/> value indicating the absolute length of the vector
        /// represented by the <see cref="PointF"/>.</value>
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
        /// A non-negative <see cref="Single"/> value that equals the square of the <see
        /// cref="Length"/> property.</value>
        /// <remarks>
        /// <b>LengthSquared</b> performs the same operations as <see cref="Length"/> but without
        /// the final <see cref="Math.Sqrt"/> call, and is therefore faster if you only need the
        /// squared <see cref="Length"/>.</remarks>

        public float LengthSquared {
            get { return (X * X + Y * Y); }
        }

        #endregion
        #region Public Methods
        #region Add

        /// <summary>
        /// Adds the coordinates of the specified <see cref="PointF"/> to this instance.</summary>
        /// <param name="point">
        /// The <see cref="PointF"/> whose coordinates to add to this instance.</param>
        /// <returns>
        /// A <see cref="PointF"/> whose <see cref="X"/> and <see cref="Y"/> properties equal the
        /// corresponding properties of the specified <paramref name="point"/> added to those of
        /// this instance.</returns>

        public PointF Add(PointF point) {
            return new PointF(X + point.X, Y + point.Y);
        }

        #endregion
        #region AngleBetween(PointF)

        /// <overloads>
        /// Computes the angle between two <see cref="PointF"/> vectors.</overloads>
        /// <summary>
        /// Computes the angle between the vector represented by this instance and the specified
        /// <see cref="PointF"/> vector.</summary>
        /// <param name="vector">
        /// The <see cref="PointF"/> vector to compare with this instance.</param>
        /// <returns>
        /// The angle, in radians, between this instance and the specified <paramref
        /// name="vector"/>, in that order.</returns>
        /// <remarks>
        /// <b>AngleBetween</b> returns the result of <see cref="Math.Atan2"/> for the cross product
        /// length and the scalar dot product of the two vectors. The possible range of values is
        /// (-<see cref="Math.PI"/>, +<see cref="Math.PI"/>].</remarks>

        public double AngleBetween(PointF vector) {

            double y = X * vector.Y - Y * vector.X;
            double x = X * vector.X + Y * vector.Y;

            return Math.Atan2(y, x);
        }

        #endregion
        #region AngleBetween(PointF, PointF)

        /// <summary>
        /// Computes the angle between the vectors from this instance to the specified <see
        /// cref="PointF"/> coordinates.</summary>
        /// <param name="a">
        /// The <see cref="PointF"/> coordinates where the first vector ends.</param>
        /// <param name="b">
        /// The <see cref="PointF"/> coordinates where the second vector ends.</param>
        /// <returns>
        /// The angle, in radians, between the vectors from this instance to <paramref name="a"/>
        /// and from this instance to <paramref name="b"/>, in that order.</returns>
        /// <remarks>
        /// <b>AngleBetween</b> returns the result of <see cref="Math.Atan2"/> for the cross product
        /// length and the scalar dot product of the two vectors. The possible range of values is
        /// (-<see cref="Math.PI"/>, +<see cref="Math.PI"/>].</remarks>

        public double AngleBetween(PointF a, PointF b) {

            double ax = a.X - X, ay = a.Y - Y;
            double bx = b.X - X, by = b.Y - Y;

            double y = ax * by - ay * bx;
            double x = ax * bx + ay * by;

            return Math.Atan2(y, x);
        }

        #endregion
        #region CrossProductLength(PointF)

        /// <overloads>
        /// Computes the length of the cross-product of two <see cref="PointF"/> vectors.
        /// </overloads>
        /// <summary>
        /// Computes the length of the cross-product of the vector represented by this instance and
        /// the specified <see cref="PointF"/> vector.</summary>
        /// <param name="vector">
        /// The <see cref="PointF"/> vector to multiply with this instance.</param>
        /// <returns>
        /// A <see cref="Single"/> value indicating the length of the cross-product of this instance
        /// and the specified <paramref name="vector"/>, in that order.</returns>
        /// <remarks>
        /// The absolute value of <b>CrossProductLength</b> equals the area of the parallelogram
        /// spanned by this instance and the specified <paramref name="vector"/>. The sign indicates
        /// their spatial relationship, which is described in the other <see
        /// cref="CrossProductLength(PointF, PointF)"/> overload.</remarks>

        public float CrossProductLength(PointF vector) {
            return X * vector.Y - vector.X * Y;
        }

        #endregion
        #region CrossProductLength(PointF, PointF)

        /// <summary>
        /// Computes the length of the cross-product of the vectors from this instance to the
        /// specified <see cref="PointF"/> coordinates.</summary>
        /// <param name="a">
        /// The <see cref="PointF"/> coordinates where the first vector ends.</param>
        /// <param name="b">
        /// The <see cref="PointF"/> coordinates where the second vector ends.</param>
        /// <returns>
        /// A <see cref="Single"/> value indicating the length of the cross-product of the vectors
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

        public float CrossProductLength(PointF a, PointF b) {
            return (a.X - X) * (b.Y - Y) - (b.X - X) * (a.Y - Y);
        }

        #endregion
        #region FromPolar

        /// <summary>
        /// Creates a <see cref="PointF"/> from the specified polar coordinates.</summary>
        /// <param name="length">
        /// The distance from the origin to the <see cref="PointF"/>.</param>
        /// <param name="angle">
        /// The polar angle, in radians, of the <see cref="PointF"/>.</param>
        /// <returns>
        /// A <see cref="PointF"/> whose <see cref="Length"/> and <see cref="Angle"/> equal the
        /// specified <paramref name="length"/> and <paramref name="angle"/>.</returns>
        /// <remarks>
        /// <b>FromPolar</b> returns <see cref="Empty"/> if the specified <paramref name="length"/>
        /// equals zero, and inverts the signs of the <see cref="X"/> and <see cref="Y"/>
        /// coordinates if <paramref name="length"/> is less than zero.</remarks>

        public static PointF FromPolar(double length, double angle) {
            return new PointF(
                (float) (length * Math.Cos(angle)),
                (float) (length * Math.Sin(angle)));
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="PointF"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the values of the <see cref="X"/> and <see cref="Y"/>
        /// properties.</remarks>

        public override unsafe int GetHashCode() {
            unchecked {
                float x = X, y = Y;
                int xi = *((int*) &x), yi = *((int*) &y);
                return xi ^ yi;
            }
        }

        #endregion
        #region IsCollinear(PointF, PointF)

        /// <overloads>
        /// Determines whether this instance is collinear with the specified <see cref="PointF"/>
        /// instances.</overloads>
        /// <summary>
        /// Determines whether this instance is collinear with the specified <see cref="PointF"/>
        /// instances, using exact coordinate comparisons.</summary>
        /// <param name="a">
        /// The first <see cref="PointF"/> instance to examine.</param>
        /// <param name="b">
        /// The second <see cref="PointF"/> instance to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="PointF"/> is collinear with <paramref name="a"/> and
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>IsCollinear</b> returns <c>true</c> exactly if <see cref="CrossProductLength(PointF,
        /// PointF)"/> returns zero for <paramref name="a"/> and <paramref name="b"/>.</remarks>

        public bool IsCollinear(PointF a, PointF b) {
            return (CrossProductLength(a, b) == 0);
        }

        #endregion
        #region IsCollinear(PointF, PointF, Single)

        /// <summary>
        /// Determines whether this instance is collinear with the specified <see cref="PointF"/> 
        /// instances, given the specified epsilon for coordinate comparisons.</summary>
        /// <param name="a">
        /// The first <see cref="PointF"/> instance to examine.</param>
        /// <param name="b">
        /// The second <see cref="PointF"/> instance to examine.</param>
        /// <param name="epsilon">
        /// The maximum absolute value at which the result of <see cref="CrossProductLength(PointF,
        /// PointF)"/> should be considered zero.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="PointF"/> is collinear with <paramref name="a"/> and
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>IsCollinear</b> is identical with the basic <see cref="IsCollinear(PointF, PointF)"/>
        /// overload but succeeds if the absolute value of <see cref="CrossProductLength(PointF,
        /// PointF)"/> for <paramref name="a"/> and <paramref name="b"/> is equal to or less than
        /// the specified <paramref name="epsilon"/>.
        /// </para><para>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but
        /// <b>IsCollinear</b> does not check this condition.</para></remarks>

        public bool IsCollinear(PointF a, PointF b, float epsilon) {
            return (Math.Abs(CrossProductLength(a, b)) <= epsilon);
        }

        #endregion
        #region Move

        /// <summary>
        /// Moves the <see cref="PointF"/> a specified distance in the specified direction.
        /// </summary>
        /// <param name="target">
        /// The <see cref="PointF"/> coordinates that indicate the direction of the move.</param>
        /// <param name="distance">
        /// The amount by which to move the <see cref="PointF"/>.</param>
        /// <returns>
        /// A <see cref="PointF"/> that equals this instance, moved towards or away from <paramref
        /// name="target"/> by the specified <paramref name="distance"/>.</returns>
        /// <remarks><para>
        /// <b>Move</b> moves the <see cref="PointF"/> towards the specified <paramref
        /// name="target"/> if the specified <paramref name="distance"/> is positive, and away from
        /// <paramref name="target"/> if <paramref name="distance"/> is negative.
        /// </para><para>
        /// <b>Move</b> returns the unchanged <see cref="PointF"/> if it equals <paramref
        /// name="target"/>, or if <paramref name="distance"/> is zero.</para></remarks>

        public PointF Move(PointF target, float distance) {
            if (distance == 0) return this;
            float dx = target.X - X, dy = target.Y - Y;

            if (dx == 0) {
                if (dy > 0)
                    return new PointF(X, Y + distance);
                else if (dy < 0)
                    return new PointF(X, Y - distance);
                else
                    return this;
            }

            if (dy == 0) {
                if (dx > 0)
                    return new PointF(X + distance, Y);
                else
                    return new PointF(X - distance, Y);
            }

            double length = Math.Sqrt(dx * dx + dy * dy);
            return new PointF(
                X + (float) (distance * dx / length),
                Y + (float) (distance * dy / length));
        }

        #endregion
        #region Multiply

        /// <summary>
        /// Multiplies the vectors represented by the specified <see cref="PointF"/> and by this
        /// instance.</summary>
        /// <param name="vector">
        /// The <see cref="PointF"/> to multiply with this instance.</param>
        /// <returns>
        /// A <see cref="Single"/> value that represents the scalar dot product of the specified
        /// <paramref name="vector"/> and this instance.</returns>
        /// <remarks>
        /// <b>Multiply</b> returns the sum of the pairwise products of the <see cref="X"/> and <see
        /// cref="Y"/> coordinates of both instances. That sum equals <see cref="LengthSquared"/> if
        /// the specified <paramref name="vector"/> equals this instance.</remarks>

        public float Multiply(PointF vector) {
            return X * vector.X + Y * vector.Y;
        }

        #endregion
        #region Normalize

        /// <summary>
        /// Normalizes the vector represented by the <see cref="PointF"/>.</summary>
        /// <returns>
        /// A <see cref="PointF"/> with the same <see cref="Angle"/> as this instance, and whose
        /// <see cref="Length"/> equals one.</returns>

        public PointF Normalize() {
            double angle = Math.Atan2(Y, X);
            return new PointF((float) Math.Cos(angle), (float) Math.Sin(angle));
        }

        #endregion
        #region Restrict

        /// <summary>
        /// Restricts the <see cref="PointF"/> to the specified coordinate range.</summary>
        /// <param name="minX">
        /// The smallest permissible <see cref="X"/> coordinate.</param>
        /// <param name="minY">
        /// The smallest permissible <see cref="Y"/> coordinate.</param>
        /// <param name="maxX">
        /// The greatest permissible <see cref="X"/> coordinate.</param>
        /// <param name="maxY">
        /// The greatest permissible <see cref="Y"/> coordinate.</param>
        /// <returns>
        /// A <see cref="PointF"/> whose <see cref="X"/> and <see cref="Y"/> coordinates equal those
        /// of this instance, restricted to the indicated coordinate range.</returns>

        public PointF Restrict(float minX, float minY, float maxX, float maxY) {
            float x = X, y = Y;

            if (x < minX) x = minX; else if (x > maxX) x = maxX;
            if (y < minY) y = minY; else if (y > maxY) y = maxY;

            return new PointF(x, y);
        }

        #endregion
        #region Round

        /// <summary>
        /// Converts the <see cref="PointF"/> to a <see cref="PointI"/> by rounding coordinates to
        /// the nearest <see cref="Int32"/> values.</summary>
        /// <returns>
        /// A <see cref="PointI"/> instance whose <see cref="PointI.X"/> and <see cref="PointI.Y"/>
        /// properties equal the corresponding properties of the <see cref="PointF"/>, rounded to
        /// the nearest <see cref="Int32"/> values.</returns>
        /// <remarks>
        /// The <see cref="Single"/> coordinates of the <see cref="PointF"/> are converted to <see
        /// cref="Int32"/> coordinates using <see cref="Fortran.NInt"/> rounding.</remarks>

        public PointI Round() {
            return new PointI(Fortran.NInt(X), Fortran.NInt(Y));
        }

        #endregion
        #region Subtract

        /// <summary>
        /// Subtracts the coordinates of the specified <see cref="PointF"/> from this instance.
        /// </summary>
        /// <param name="point">
        /// The <see cref="PointF"/> whose coordinates to subtract from this instance.</param>
        /// <returns>
        /// A <see cref="PointF"/> whose <see cref="X"/> and <see cref="Y"/> properties equal the
        /// corresponding properties of the specified <paramref name="point"/> subtracted from those
        /// of this instance.</returns>

        public PointF Subtract(PointF point) {
            return new PointF(X - point.X, Y - point.Y);
        }

        #endregion
        #region ToPointI

        /// <summary>
        /// Converts the <see cref="PointF"/> to a <see cref="PointI"/> by truncating coordinates to
        /// the nearest <see cref="Int32"/> values.</summary>
        /// <returns>
        /// A <see cref="PointI"/> instance whose <see cref="PointI.X"/> and <see cref="PointI.Y"/>
        /// properties equal the corresponding properties of the <see cref="PointF"/>, truncated to
        /// the nearest <see cref="Int32"/> values.</returns>

        public PointI ToPointI() {
            return new PointI((int) X, (int) Y);
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="PointF"/>.</summary>
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
        /// Determines whether two <see cref="PointF"/> instances have the same value.</summary>
        /// <param name="a">
        /// The first <see cref="PointF"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="PointF"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(PointF)"/> method to test the two <see
        /// cref="PointF"/> instances for value equality.</remarks>

        public static bool operator ==(PointF a, PointF b) {
            return a.Equals(b);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="PointF"/> instances have different values.</summary>
        /// <param name="a">
        /// The first <see cref="PointF"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="PointF"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different from the value of
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(PointF)"/> method to test the two <see
        /// cref="PointF"/> instances for value inequality.</remarks>

        public static bool operator !=(PointF a, PointF b) {
            return !a.Equals(b);
        }

        #endregion
        #region operator+

        /// <summary>
        /// Adds the coordinates of two <see cref="PointF"/> instances.</summary>
        /// <param name="a">
        /// The first <see cref="PointF"/> to add.</param>
        /// <param name="b">
        /// The second <see cref="PointF"/> to add.</param>
        /// <returns>
        /// A <see cref="PointF"/> whose <see cref="X"/> and <see cref="Y"/> properties equal the
        /// corresponding properties of <paramref name="a"/> added to those of <paramref name="b"/>.
        /// </returns>
        /// <remarks>
        /// This operator invokes <see cref="Add"/> to add the coordinates of the two <see
        /// cref="PointF"/> instances.</remarks>

        public static PointF operator +(PointF a, PointF b) {
            return a.Add(b);
        }

        #endregion
        #region operator*

        /// <summary>
        /// Multiplies the vectors represented by two <see cref="PointF"/> instances.</summary>
        /// <param name="a">
        /// The first <see cref="PointF"/> to multiply.</param>
        /// <param name="b">
        /// The second <see cref="PointF"/> to multiply.</param>
        /// <returns>
        /// A <see cref="Single"/> value that represents the scalar dot product of <paramref
        /// name="a"/> and <paramref name="b"/>.</returns>
        /// <remarks>
        /// This operator invokes <see cref="Multiply"/> to multiply the vectors represented by the
        /// two <see cref="PointF"/> instances.</remarks>

        public static float operator *(PointF a, PointF b) {
            return a.Multiply(b);
        }

        #endregion
        #region operator-

        /// <summary>
        /// Subtracts the coordinates of two <see cref="PointF"/> instances.</summary>
        /// <param name="a">
        /// The <see cref="PointF"/> to subtract from.</param>
        /// <param name="b">
        /// The <see cref="PointF"/> to subtract.</param>
        /// <returns>
        /// A <see cref="PointF"/> whose <see cref="X"/> and <see cref="Y"/> properties equal the
        /// corresponding properties of <paramref name="a"/> subtracted from those of <paramref
        /// name="b"/>.</returns>
        /// <remarks>
        /// This operator invokes <see cref="Subtract"/> to subtract the coordinates of the two <see
        /// cref="PointF"/> instances.</remarks>

        public static PointF operator -(PointF a, PointF b) {
            return a.Subtract(b);
        }

        #endregion
        #region PointF(PointI)

        /// <summary>
        /// Converts a <see cref="PointI"/> to a <see cref="PointF"/> with identical coordinates.
        /// </summary>
        /// <param name="point">
        /// The <see cref="PointI"/> instance to convert into a <see cref="PointF"/> instance.
        /// </param>
        /// <returns>
        /// A <see cref="PointF"/> instance whose <see cref="X"/> and <see cref="Y"/> properties
        /// equal the corresponding properties of the specified <paramref name="point"/>.</returns>

        public static implicit operator PointF(PointI point) {
            return new PointF(point.X, point.Y);
        }

        #endregion
        #region PointI(PointF)

        /// <summary>
        /// Converts a <see cref="PointF"/> to a <see cref="PointI"/> by truncating coordinates to
        /// the nearest <see cref="Int32"/> values.</summary>
        /// <param name="point">
        /// The <see cref="PointF"/> instance to convert into a <see cref="PointI"/> instance.
        /// </param>
        /// <returns>
        /// A <see cref="PointI"/> instance whose <see cref="PointI.X"/> and <see cref="PointI.Y"/>
        /// properties equal the corresponding properties of the specified <paramref name="point"/>,
        /// truncated to the nearest <see cref="Int32"/> values.</returns>

        public static explicit operator PointI(PointF point) {
            return point.ToPointI();
        }

        #endregion
        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="PointF"/> instances have the same value.</overloads>
        /// <summary>
        /// Determines whether this <see cref="PointF"/> instance and a specified object, which must
        /// be a <see cref="PointF"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="PointF"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="PointF"/> instance and its
        /// value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="PointF"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(PointF)"/> overload to test
        /// the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is PointF))
                return false;

            return Equals((PointF) obj);
        }

        #endregion
        #region Equals(PointF)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="PointF"/> have the same
        /// value.</summary>
        /// <param name="point">
        /// A <see cref="PointF"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="point"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="X"/> and <see cref="Y"/> properties
        /// of the two <see cref="PointF"/> instances to test for value equality.</remarks>

        public bool Equals(PointF point) {
            return (X == point.X && Y == point.Y);
        }

        #endregion
        #region Equals(PointF, PointF)

        /// <summary>
        /// Determines whether two specified <see cref="PointF"/> instances have the same value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="PointF"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="PointF"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(PointF)"/> overload to test the
        /// two <see cref="PointF"/> instances for value equality.</remarks>

        public static bool Equals(PointF a, PointF b) {
            return a.Equals(b);
        }

        #endregion
        #region Equals(PointF, PointF, Single)

        /// <summary>
        /// Determines whether two specified <see cref="PointF"/> instances have the same value,
        /// given the specified epsilon.</summary>
        /// <param name="a">
        /// The first <see cref="PointF"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="PointF"/> to compare.</param>
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

        public static bool Equals(PointF a, PointF b, float epsilon) {

            return (Math.Abs(a.X - b.X) <= epsilon
                && Math.Abs(a.Y - b.Y) <= epsilon);
        }

        #endregion
        #endregion
    }
}
