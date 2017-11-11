using System;
using System.Globalization;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents an extension in two-dimensional space, using <see cref="Int32"/> coordinates.
    /// </summary>
    /// <remarks><para>
    /// <b>SizeI</b> is an immutable structure whose two <see cref="Int32"/> dimensions define an
    /// extension in two-dimensional space.
    /// </para><para>
    /// Use the <see cref="SizeF"/> structure to represent sizes with <see cref="Single"/>
    /// dimensions, and the <see cref="SizeD"/> structure to represent sizes with <see
    /// cref="Double"/> dimensions.</para></remarks>

    [Serializable]
    public struct SizeI: IEquatable<SizeI> {
        #region SizeI(Int32, Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="SizeI"/> structure with the specified
        /// dimensions.</summary>
        /// <param name="width">
        /// The <see cref="Width"/> of the <see cref="SizeI"/>.</param>
        /// <param name="height">
        /// The <see cref="Height"/> of the <see cref="SizeI"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is less than zero.</exception>

        public SizeI(int width, int height) {
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
        /// An empty read-only <see cref="SizeI"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="SizeI"/> instance that was created with the default
        /// constructor.</remarks>

        public static readonly SizeI Empty = new SizeI();

        #endregion
        #region Width

        /// <summary>
        /// The horizontal extension of the <see cref="SizeI"/>.</summary>
        /// <remarks>
        /// <b>Width</b> is never less than zero.</remarks>

        public readonly int Width;

        #endregion
        #region Height

        /// <summary>
        /// The vertical extension of the <see cref="SizeI"/>.</summary>
        /// <remarks>
        /// <b>Height</b> is never less than zero.</remarks>

        public readonly int Height;

        #endregion
        #region Add

        /// <summary>
        /// Adds the dimensions of the specified <see cref="SizeI"/> to this instance.</summary>
        /// <param name="size">
        /// The <see cref="SizeI"/> whose dimensions to add to this instance.</param>
        /// <returns>
        /// A <see cref="SizeI"/> whose <see cref="Width"/> and <see cref="Height"/> properties
        /// equal the corresponding properties of the specified <paramref name="size"/> added to
        /// those of this instance.</returns>

        public SizeI Add(SizeI size) {
            return new SizeI(Width + size.Width, Height + size.Height);
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="SizeI"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the values of the <see cref="Width"/> and <see
        /// cref="Height"/> properties.</remarks>

        public override int GetHashCode() {
            unchecked { return Width ^ Height; }
        }

        #endregion
        #region Restrict

        /// <summary>
        /// Restricts the <see cref="SizeI"/> to the specified extension range.</summary>
        /// <param name="minWidth">
        /// The smallest permissible <see cref="Width"/>.</param>
        /// <param name="minHeight">
        /// The smallest permissible <see cref="Height"/>.</param>
        /// <param name="maxWidth">
        /// The greatest permissible <see cref="Width"/>.</param>
        /// <param name="maxHeight">
        /// The greatest permissible <see cref="Height"/>.</param>
        /// <returns>
        /// A <see cref="SizeI"/> whose <see cref="Width"/> and <see cref="Height"/> equal those of
        /// this instance, restricted to the indicated extension range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxWidth"/> or <paramref name="maxHeight"/> is less than zero.
        /// </exception>

        public SizeI Restrict(int minWidth, int minHeight, int maxWidth, int maxHeight) {
            int height = Width, width = Height;

            if (height < minWidth) height = minWidth;
            else if (height > maxWidth) height = maxWidth;

            if (width < minHeight) width = minHeight;
            else if (width > maxHeight) width = maxHeight;

            return new SizeI(height, width);
        }

        #endregion
        #region Subtract

        /// <summary>
        /// Subtracts the dimensions of the specified <see cref="SizeI"/> from this instance.
        /// </summary>
        /// <param name="size">
        /// The <see cref="SizeI"/> whose dimensions to subtract from this instance.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="size"/> contains a <see cref="Width"/> or <see cref="Height"/> that is
        /// greater than the corresponding property of this instance.</exception>
        /// <returns>
        /// A <see cref="SizeI"/> whose <see cref="Width"/> and <see cref="Height"/> properties
        /// equal the corresponding properties of the specified <paramref name="size"/> subtracted
        /// from those of this instance.</returns>

        public SizeI Subtract(SizeI size) {
            return new SizeI(Width - size.Width, Height - size.Height);
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="SizeI"/>.</summary>
        /// <returns>
        /// A <see cref="String"/> containing the values of the <see cref="Width"/> and <see
        /// cref="Height"/> properties.</returns>

        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture,
                "{{Width={0}, Height={1}}}", Width, Height);
        }

        #endregion
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="SizeI"/> instances have the same value.</summary>
        /// <param name="a">
        /// The first <see cref="SizeI"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="SizeI"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(SizeI)"/> method to test the two <see
        /// cref="SizeI"/> instances for value equality.</remarks>

        public static bool operator ==(SizeI a, SizeI b) {
            return a.Equals(b);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="SizeI"/> instances have different values.</summary>
        /// <param name="a">
        /// The first <see cref="SizeI"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="SizeI"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different from the value of
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(SizeI)"/> method to test the two <see
        /// cref="SizeI"/> instances for value inequality.</remarks>

        public static bool operator !=(SizeI a, SizeI b) {
            return !a.Equals(b);
        }

        #endregion
        #region operator+

        /// <summary>
        /// Adds the dimensions of two <see cref="SizeI"/> instances.</summary>
        /// <param name="a">
        /// The first <see cref="SizeI"/> to add.</param>
        /// <param name="b">
        /// The second <see cref="SizeI"/> to add.</param>
        /// <returns>
        /// A <see cref="SizeI"/> whose <see cref="Width"/> and <see cref="Height"/> properties
        /// equal the corresponding properties of <paramref name="a"/> added to those of <paramref
        /// name="b"/>.</returns>
        /// <remarks>
        /// This operator invokes <see cref="Add"/> to add the dimensions of the two <see
        /// cref="SizeI"/> instances.</remarks>

        public static SizeI operator +(SizeI a, SizeI b) {
            return a.Add(b);
        }

        #endregion
        #region operator-

        /// <summary>
        /// Subtracts the dimensions of two <see cref="SizeI"/> instances.</summary>
        /// <param name="a">
        /// The <see cref="SizeI"/> to subtract from.</param>
        /// <param name="b">
        /// The <see cref="SizeI"/> to subtract.</param>
        /// <returns>
        /// A <see cref="SizeI"/> whose <see cref="Width"/> and <see cref="Height"/> properties
        /// equal the corresponding properties of <paramref name="a"/> subtracted from those of
        /// <paramref name="b"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="a"/> and <paramref name="b"/> contain <see cref="Width"/> or <see
        /// cref="Height"/> properties whose difference is less than zero.</exception>
        /// <remarks>
        /// This operator invokes <see cref="Subtract"/> to subtract the dimensions of the two <see
        /// cref="SizeI"/> instances.</remarks>

        public static SizeI operator -(SizeI a, SizeI b) {
            return a.Subtract(b);
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="SizeI"/> instances have the same value.</overloads>
        /// <summary>
        /// Determines whether this <see cref="SizeI"/> instance and a specified object, which must
        /// be a <see cref="SizeI"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="SizeI"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="SizeI"/> instance and its
        /// value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="SizeI"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(SizeI)"/> overload to test
        /// the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is SizeI))
                return false;

            return Equals((SizeI) obj);
        }

        #endregion
        #region Equals(SizeI)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="SizeI"/> have the same
        /// value.</summary>
        /// <param name="size">
        /// A <see cref="SizeI"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="size"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="Width"/> and <see cref="Height"/>
        /// properties of the two <see cref="SizeI"/> instances to test for value equality.
        /// </remarks>

        public bool Equals(SizeI size) {
            return (Width == size.Width && Height == size.Height);
        }

        #endregion
        #region Equals(SizeI, SizeI)

        /// <summary>
        /// Determines whether two specified <see cref="SizeI"/> instances have the same value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="SizeI"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="SizeI"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(SizeI)"/> overload to test the
        /// two <see cref="SizeI"/> instances for value equality.</remarks>

        public static bool Equals(SizeI a, SizeI b) {
            return a.Equals(b);
        }

        #endregion
        #endregion
    }
}
