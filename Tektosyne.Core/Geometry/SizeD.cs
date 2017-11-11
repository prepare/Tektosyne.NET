using System;
using System.Globalization;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents an extension in two-dimensional space, using <see cref="Double"/> coordinates.
    /// </summary>
    /// <remarks><para>
    /// <b>SizeD</b> is an immutable structure whose two <see cref="Double"/> dimensions define an
    /// extension in two-dimensional space.
    /// </para><para>
    /// Use the <see cref="SizeI"/> structure to represent sizes with <see cref="Int32"/>
    /// dimensions, and the <see cref="SizeF"/> structure to represent sizes with <see
    /// cref="Single"/> dimensions. You can convert <see cref="SizeD"/> instances to and from <see
    /// cref="SizeI"/> and <see cref="SizeF"/> instances, rounding off the <see cref="Double"/>
    /// dimensions as necessary.</para></remarks>

    [Serializable]
    public struct SizeD: IEquatable<SizeD> {
        #region SizeD(Double, Double)

        /// <summary>
        /// Initializes a new instance of the <see cref="SizeD"/> structure with the specified
        /// dimensions.</summary>
        /// <param name="width">
        /// The <see cref="Width"/> of the <see cref="SizeD"/>.</param>
        /// <param name="height">
        /// The <see cref="Height"/> of the <see cref="SizeD"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is less than zero.</exception>

        public SizeD(double width, double height) {
            if (width < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "width", width, Strings.ArgumentNegative);

            if (height < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "height", height, Strings.ArgumentNegative);

            Width = width;
            Height = height;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="SizeD"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="SizeD"/> instance that was created with the default
        /// constructor.</remarks>

        public static readonly SizeD Empty = new SizeD();

        #endregion
        #region Width

        /// <summary>
        /// The horizontal extension of the <see cref="SizeD"/>.</summary>
        /// <remarks>
        /// <b>Width</b> is never less than zero.</remarks>

        public readonly double Width;

        #endregion
        #region Height

        /// <summary>
        /// The vertical extension of the <see cref="SizeD"/>.</summary>
        /// <remarks>
        /// <b>Height</b> is never less than zero.</remarks>

        public readonly double Height;

        #endregion
        #region Add

        /// <summary>
        /// Adds the dimensions of the specified <see cref="SizeD"/> to this instance.</summary>
        /// <param name="size">
        /// The <see cref="SizeD"/> whose dimensions to add to this instance.</param>
        /// <returns>
        /// A <see cref="SizeD"/> whose <see cref="Width"/> and <see cref="Height"/> properties
        /// equal the corresponding properties of the specified <paramref name="size"/> added to
        /// those of this instance.</returns>

        public SizeD Add(SizeD size) {
            return new SizeD(Width + size.Width, Height + size.Height);
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="SizeD"/> instance.</summary>
        /// <returns>
        /// An <see cref="Double"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the values of the <see cref="Width"/> and <see
        /// cref="Height"/> properties.</remarks>

        public override unsafe int GetHashCode() {
            unchecked {
                double w = Width, h = Height;
                long wi = *((long*) &w), hi = *((long*) &h);
                return (int) wi ^ (int) (wi >> 32) ^ (int) hi ^ (int) (hi >> 32);
            }
        }

        #endregion
        #region Restrict

        /// <summary>
        /// Restricts the <see cref="SizeD"/> to the specified extension range.</summary>
        /// <param name="minWidth">
        /// The smallest permissible <see cref="Width"/>.</param>
        /// <param name="minHeight">
        /// The smallest permissible <see cref="Height"/>.</param>
        /// <param name="maxWidth">
        /// The greatest permissible <see cref="Width"/>.</param>
        /// <param name="maxHeight">
        /// The greatest permissible <see cref="Height"/>.</param>
        /// <returns>
        /// A <see cref="SizeD"/> whose <see cref="Width"/> and <see cref="Height"/> equal those of
        /// this instance, restricted to the indicated extension range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxWidth"/> or <paramref name="maxHeight"/> is less than zero.
        /// </exception>

        public SizeD Restrict(double minWidth, double minHeight, double maxWidth, double maxHeight) {
            double height = Width, width = Height;

            if (height < minWidth) height = minWidth;
            else if (height > maxWidth) height = maxWidth;

            if (width < minHeight) width = minHeight;
            else if (width > maxHeight) width = maxHeight;

            return new SizeD(height, width);
        }

        #endregion
        #region Round

        /// <summary>
        /// Converts the <see cref="SizeD"/> to a <see cref="SizeI"/> by rounding dimensions to the
        /// nearest <see cref="Int32"/> values.</summary>
        /// <returns>
        /// A <see cref="SizeI"/> instance whose <see cref="SizeI.Width"/> and <see
        /// cref="SizeI.Height"/> properties equal the corresponding properties of the <see
        /// cref="SizeD"/>, rounded to the nearest <see cref="Int32"/> values.</returns>
        /// <remarks>
        /// The <see cref="Double"/> dimensions of the <see cref="SizeD"/> are converted to <see
        /// cref="Int32"/> dimensions using <see cref="Fortran.NInt"/> rounding.</remarks>

        public SizeI Round() {
            return new SizeI(Fortran.NInt(Width), Fortran.NInt(Height));
        }

        #endregion
        #region Subtract

        /// <summary>
        /// Subtracts the dimensions of the specified <see cref="SizeD"/> from this instance.
        /// </summary>
        /// <param name="size">
        /// The <see cref="SizeD"/> whose dimensions to subtract from this instance.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="size"/> contains a <see cref="Width"/> or <see cref="Height"/> that is
        /// greater than the corresponding property of this instance.</exception>
        /// <returns>
        /// A <see cref="SizeD"/> whose <see cref="Width"/> and <see cref="Height"/> properties
        /// equal the corresponding properties of the specified <paramref name="size"/> subtracted
        /// from those of this instance.</returns>

        public SizeD Subtract(SizeD size) {
            return new SizeD(Width - size.Width, Height - size.Height);
        }

        #endregion
        #region ToSizeF

        /// <summary>
        /// Converts the <see cref="SizeD"/> to a <see cref="SizeF"/> by casting dimensions to the
        /// equivalent <see cref="Single"/> values.</summary>
        /// <returns>
        /// A <see cref="SizeF"/> instance whose <see cref="SizeF.Width"/> and <see
        /// cref="SizeF.Height"/> properties equal the corresponding properties of the <see
        /// cref="SizeD"/>, cast to the equivalent <see cref="Single"/> values.</returns>

        public SizeF ToSizeF() {
            return new SizeF((float) Width, (float) Height);
        }

        #endregion
        #region ToSizeI

        /// <summary>
        /// Converts the <see cref="SizeD"/> to a <see cref="SizeI"/> by truncating dimensions to
        /// the nearest <see cref="Int32"/> values.</summary>
        /// <returns>
        /// A <see cref="SizeI"/> instance whose <see cref="SizeI.Width"/> and <see
        /// cref="SizeI.Height"/> properties equal the corresponding properties of the <see
        /// cref="SizeD"/>, truncated to the nearest <see cref="Int32"/> values.</returns>

        public SizeI ToSizeI() {
            return new SizeI((int) Width, (int) Height);
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="SizeD"/>.</summary>
        /// <returns>
        /// A <see cref="String"/> containing the values of the <see cref="Width"/> and <see
        /// cref="Height"/> properties.</returns>

        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture,
                "{{Width={0}, Height={1}}}", Width, Height);
        }

        #endregion
        #region Public Operators
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="SizeD"/> instances have the same value.</summary>
        /// <param name="a">
        /// The first <see cref="SizeD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="SizeD"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(SizeD)"/> method to test the two <see
        /// cref="SizeD"/> instances for value equality.</remarks>

        public static bool operator ==(SizeD a, SizeD b) {
            return a.Equals(b);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="SizeD"/> instances have different values.</summary>
        /// <param name="a">
        /// The first <see cref="SizeD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="SizeD"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different from the value of
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(SizeD)"/> method to test the two <see
        /// cref="SizeD"/> instances for value inequality.</remarks>

        public static bool operator !=(SizeD a, SizeD b) {
            return !a.Equals(b);
        }

        #endregion
        #region operator+

        /// <summary>
        /// Adds the dimensions of two <see cref="SizeD"/> instances.</summary>
        /// <param name="a">
        /// The first <see cref="SizeD"/> to add.</param>
        /// <param name="b">
        /// The second <see cref="SizeD"/> to add.</param>
        /// <returns>
        /// A <see cref="SizeD"/> whose <see cref="Width"/> and <see cref="Height"/> properties
        /// equal the corresponding properties of <paramref name="a"/> added to those of <paramref
        /// name="b"/>.</returns>
        /// <remarks>
        /// This operator invokes <see cref="Add"/> to add the dimensions of the two <see
        /// cref="SizeD"/> instances.</remarks>

        public static SizeD operator +(SizeD a, SizeD b) {
            return a.Add(b);
        }

        #endregion
        #region operator-

        /// <summary>
        /// Subtracts the dimensions of two <see cref="SizeD"/> instances.</summary>
        /// <param name="a">
        /// The <see cref="SizeD"/> to subtract from.</param>
        /// <param name="b">
        /// The <see cref="SizeD"/> to subtract.</param>
        /// <returns>
        /// A <see cref="SizeD"/> whose <see cref="Width"/> and <see cref="Height"/> properties
        /// equal the corresponding properties of <paramref name="a"/> subtracted from those of
        /// <paramref name="b"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="a"/> and <paramref name="b"/> contain <see cref="Width"/> or <see
        /// cref="Height"/> properties whose difference is less than zero.</exception>
        /// <remarks>
        /// This operator invokes <see cref="Subtract"/> to subtract the dimensions of the two <see
        /// cref="SizeD"/> instances.</remarks>

        public static SizeD operator -(SizeD a, SizeD b) {
            return a.Subtract(b);
        }

        #endregion
        #region SizeD(SizeF)

        /// <summary>
        /// Converts a <see cref="SizeF"/> to a <see cref="SizeD"/> with identical dimensions.
        /// </summary>
        /// <param name="size">
        /// The <see cref="SizeF"/> instance to convert into a <see cref="SizeD"/> instance.</param>
        /// <returns>
        /// A <see cref="SizeD"/> instance whose <see cref="Width"/> and <see cref="Height"/>
        /// properties equal the corresponding properties of the specified <paramref name="size"/>.
        /// </returns>

        public static implicit operator SizeD(SizeF size) {
            return new SizeD(size.Width, size.Height);
        }

        #endregion
        #region SizeD(SizeI)

        /// <summary>
        /// Converts a <see cref="SizeI"/> to a <see cref="SizeD"/> with identical dimensions.
        /// </summary>
        /// <param name="size">
        /// The <see cref="SizeI"/> instance to convert into a <see cref="SizeD"/> instance.</param>
        /// <returns>
        /// A <see cref="SizeD"/> instance whose <see cref="Width"/> and <see cref="Height"/>
        /// properties equal the corresponding properties of the specified <paramref name="size"/>.
        /// </returns>

        public static implicit operator SizeD(SizeI size) {
            return new SizeD(size.Width, size.Height);
        }

        #endregion
        #region SizeF(SizeD)

        /// <summary>
        /// Converts a <see cref="SizeD"/> to a <see cref="SizeF"/> by casting dimensions to the
        /// equivalent <see cref="Single"/> values.</summary>
        /// <param name="size">
        /// The <see cref="SizeD"/> instance to convert into a <see cref="SizeF"/> instance.</param>
        /// <returns>
        /// A <see cref="SizeF"/> instance whose <see cref="SizeF.Width"/> and <see
        /// cref="SizeF.Height"/> properties equal the corresponding properties of the specified
        /// <paramref name="size"/>, cast to the equivalent <see cref="Single"/> values.</returns>

        public static explicit operator SizeF(SizeD size) {
            return size.ToSizeF();
        }

        #endregion
        #region SizeI(SizeD)

        /// <summary>
        /// Converts a <see cref="SizeD"/> to a <see cref="SizeI"/> by truncating dimensions to the
        /// nearest <see cref="Int32"/> values.</summary>
        /// <param name="size">
        /// The <see cref="SizeD"/> instance to convert into a <see cref="SizeI"/> instance.</param>
        /// <returns>
        /// A <see cref="SizeI"/> instance whose <see cref="SizeI.Width"/> and <see
        /// cref="SizeI.Height"/> properties equal the corresponding properties of the specified
        /// <paramref name="size"/>, truncated to the nearest <see cref="Int32"/> values.</returns>

        public static explicit operator SizeI(SizeD size) {
            return size.ToSizeI();
        }

        #endregion
        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="SizeD"/> instances have the same value.</overloads>
        /// <summary>
        /// Determines whether this <see cref="SizeD"/> instance and a specified object, which must
        /// be a <see cref="SizeD"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="SizeD"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="SizeD"/> instance and its
        /// value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="SizeD"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(SizeD)"/> overload to test
        /// the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is SizeD))
                return false;

            return Equals((SizeD) obj);
        }

        #endregion
        #region Equals(SizeD)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="SizeD"/> have the same
        /// value.</summary>
        /// <param name="size">
        /// A <see cref="SizeD"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="size"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="Width"/> and <see cref="Height"/>
        /// properties of the two <see cref="SizeD"/> instances to test for value equality.
        /// </remarks>

        public bool Equals(SizeD size) {
            return (Width == size.Width && Height == size.Height);
        }

        #endregion
        #region Equals(SizeD, SizeD)

        /// <summary>
        /// Determines whether two specified <see cref="SizeD"/> instances have the same value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="SizeD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="SizeD"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(SizeD)"/> overload to test the
        /// two <see cref="SizeD"/> instances for value equality.</remarks>

        public static bool Equals(SizeD a, SizeD b) {
            return a.Equals(b);
        }

        #endregion
        #region Equals(SizeD, SizeD, Double)

        /// <summary>
        /// Determines whether two specified <see cref="SizeD"/> instances have the same value,
        /// given the specified epsilon.</summary>
        /// <param name="a">
        /// The first <see cref="SizeD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="SizeD"/> to compare.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which the dimensions of <paramref name="a"/> and
        /// <paramref name="b"/> should be considered equal.</param>
        /// <returns>
        /// <c>true</c> if the absolute difference between the dimensions of <paramref name="a"/>
        /// and <paramref name="b"/> is less than or equal to <paramref name="epsilon"/>; otherwise,
        /// <c>false</c>.</returns>
        /// <remarks>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but <b>Equals</b>
        /// does not check this condition.</remarks>

        public static bool Equals(SizeD a, SizeD b, double epsilon) {

            return (Math.Abs(a.Width - b.Width) <= epsilon
                && Math.Abs(a.Height - b.Height) <= epsilon);
        }

        #endregion
        #endregion
    }
}
