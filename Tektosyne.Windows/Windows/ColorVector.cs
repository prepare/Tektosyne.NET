using System;
using System.Globalization;
using System.Windows.Media;

namespace Tektosyne.Windows {

    /// <summary>
    /// Represents an immutable displacement in sRGB color space.</summary>
    /// <remarks><para>
    /// The <b>ColorVector</b> structure holds three <see cref="Int16"/> values, corresponding to
    /// the three sRGB channels of a <see cref="Color"/> instance. Each value may range from -255 to
    /// +255 and represents a displacement to the corresponding <see cref="Color"/> channel.
    /// </para><para>
    /// Unlike the <see cref="Color"/> structure, the <b>ColorVector</b> structure is immutable. Use
    /// the <see cref="ColorVector.Add"/> and <see cref="ColorVector.Subtract"/> methods to shift
    /// the color channels of a <see cref="Color"/> value or of another <b>ColorVector</b>.
    /// </para></remarks>

    [Serializable]
    public struct ColorVector: IEquatable<ColorVector> {
        #region ColorVector(Int16, Int16, Int16)

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorVector"/> structure with the specified
        /// values for each color channel.</summary>
        /// <param name="r">
        /// The sRGB red channel value of the <see cref="ColorVector"/>.</param>
        /// <param name="g">
        /// The sRGB green channel value of the <see cref="ColorVector"/>.</param>
        /// <param name="b">
        /// The sRGB blue channel value of the <see cref="ColorVector"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="r"/>, <paramref name="g"/>, or <paramref name="b"/> is less than -255 or
        /// greater than +255.</exception>

        public ColorVector(short r, short g, short b) {

            if (r < -255 || r > 255)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "r", r, Tektosyne.Strings.ArgumentLessOrGreater, -255, 255);

            if (g < -255 || g > 255)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "g", g, Tektosyne.Strings.ArgumentLessOrGreater, -255, 255);

            if (b < -255 || b > 255)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "b", b, Tektosyne.Strings.ArgumentLessOrGreater, -255, 255);

            R = r;
            G = g;
            B = b;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="ColorVector"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> holds a <see cref="ColorVector"/> instance that was created with the
        /// default constructor.</remarks>

        public static readonly ColorVector Empty = new ColorVector();

        #endregion
        #region IsEmpty

        /// <summary>
        /// Gets a value indicating whether the <see cref="ColorVector"/> is empty.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="R"/>, <see cref="G"/>, and <see cref="B"/> properties all
        /// equal zero; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// <b>IsEmpty</b> is always <c>true</c> for the <see cref="Empty"/> instance, or for any
        /// other default-constructed <see cref="ColorVector"/> instance.</remarks>

        public bool IsEmpty {
            get { return (R == 0 && G == 0 && B == 0); }
        }

        #endregion
        #region R

        /// <summary>
        /// The sRGB red channel value of the <see cref="ColorVector"/>, ranging from -255 to +255.
        /// </summary>>

        public readonly short R;

        #endregion
        #region G

        /// <summary>
        /// The sRGB green channel value of the <see cref="ColorVector"/>, ranging from -255 to
        /// +255.</summary>

        public readonly short G;

        #endregion
        #region B

        /// <summary>
        /// The sRGB blue channel value of the <see cref="ColorVector"/>, ranging from -255 to +255.
        /// </summary>>

        public readonly short B;

        #endregion
        #region Add(Color)

        /// <overloads>
        /// Adds the <see cref="ColorVector"/> to the specified <see cref="Color"/> or <see
        /// cref="ColorVector"/>.</overloads>
        /// <summary>
        /// Adds the <see cref="ColorVector"/> to the specified <see cref="Color"/>.</summary>
        /// <param name="color">
        /// The <see cref="Color"/> to which to add this <see cref="ColorVector"/>.</param>
        /// <remarks>
        /// <b>Add</b> adds the values of the <see cref="R"/>, <see cref="G"/>, and <see cref="B"/>
        /// properties to the corresponding sRGB color channels of the specified <paramref
        /// name="color"/>.</remarks>

        public void Add(Color color) {

            color.R = (byte) MathUtility.Restrict(color.R + R, 0, 255);
            color.G = (byte) MathUtility.Restrict(color.G + G, 0, 255);
            color.B = (byte) MathUtility.Restrict(color.B + B, 0, 255);
        }

        #endregion
        #region Add(ColorVector)

        /// <summary>
        /// Adds the <see cref="ColorVector"/> to the specified instance.</summary>
        /// <param name="vector">
        /// The <see cref="ColorVector"/> to which to add this instance.</param>
        /// <returns>
        /// A new <see cref="ColorVector"/> that represents the sum of the specified <paramref
        /// name="vector"/> and this instance.</returns>
        /// <remarks>
        /// <b>Add</b> adds the values of the <see cref="R"/>, <see cref="G"/>, and <see cref="B"/>
        /// properties of this instance to the specified <paramref name="vector"/>.</remarks>

        public ColorVector Add(ColorVector vector) {

            return new ColorVector(
                (short) MathUtility.Restrict(vector.R + R, -255, 255),
                (short) MathUtility.Restrict(vector.G + G, -255, 255),
                (short) MathUtility.Restrict(vector.B + B, -255, 255));
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="ColorVector"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the values of the <see cref="R"/>, <see cref="G"/>, and <see
        /// cref="B"/> properties.</remarks>

        public override int GetHashCode() {
            unchecked { return (R << 16) ^ (G << 8) ^ B; }
        }

        #endregion
        #region Subtract(Color)

        /// <overloads>
        /// Subtracts the <see cref="ColorVector"/> from the specified <see cref="Color"/> or <see
        /// cref="ColorVector"/>.</overloads>
        /// <summary>
        /// Subtracts the <see cref="ColorVector"/> from the specified <see cref="Color"/>.
        /// </summary>
        /// <param name="color">
        /// The <see cref="Color"/> from which to subtract this <see cref="ColorVector"/>.</param>
        /// <remarks>
        /// <b>Subtract</b> subtracts the values of the <see cref="R"/>, <see cref="G"/>, and <see
        /// cref="B"/> properties from the corresponding sRGB color channels of the specified
        /// <paramref name="color"/>.</remarks>

        public void Subtract(Color color) {

            color.R = (byte) MathUtility.Restrict(color.R - R, 0, 255);
            color.G = (byte) MathUtility.Restrict(color.G - G, 0, 255);
            color.B = (byte) MathUtility.Restrict(color.B - B, 0, 255);
        }

        #endregion
        #region Subtract(ColorVector)

        /// <summary>
        /// Subtracts the <see cref="ColorVector"/> from the specified instance.</summary>
        /// <param name="vector">
        /// The <see cref="ColorVector"/> from which to subtract this instance.</param>
        /// <returns>
        /// A new <see cref="ColorVector"/> that represents the difference between the specified
        /// <paramref name="vector"/> and this instance.</returns>
        /// <remarks>
        /// <b>Subtract</b> subtracts the values of the <see cref="R"/>, <see cref="G"/>, and <see
        /// cref="B"/> properties of this instance from the specified <paramref name="vector"/>.
        /// </remarks>

        public ColorVector Subtract(ColorVector vector) {

            return new ColorVector(
                (short) MathUtility.Restrict(vector.R - R, -255, 255),
                (short) MathUtility.Restrict(vector.G - G, -255, 255),
                (short) MathUtility.Restrict(vector.B - B, -255, 255));
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="ColorVector"/>.</summary>
        /// <returns>
        /// A <see cref="String"/> containing the values of the <see cref="R"/>, <see cref="G"/>,
        /// and <see cref="B"/> properties.</returns>

        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture, "{{R={0}, G={1}, B={2}}}", R, G, B);
        }

        #endregion
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="ColorVector"/> instances have the same value.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="ColorVector"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="ColorVector"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(ColorVector)"/> method to test the two <see
        /// cref="ColorVector"/> instances for value equality.</remarks>

        public static bool operator ==(ColorVector x, ColorVector y) {
            return x.Equals(y);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="ColorVector"/> instances have different values.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="ColorVector"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="ColorVector"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is different from the value of
        /// <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(ColorVector)"/> method to test the two <see
        /// cref="ColorVector"/> instances for value inequality.</remarks>

        public static bool operator !=(ColorVector x, ColorVector y) {
            return !x.Equals(y);
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="ColorVector"/> instances have the same value.
        /// </overloads>
        /// <summary>
        /// Determines whether this <see cref="ColorVector"/> instance and a specified object, which
        /// must be a <see cref="ColorVector"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="ColorVector"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="ColorVector"/> instance and
        /// its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="ColorVector"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(ColorVector)"/> overload to
        /// test the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is ColorVector))
                return false;

            return Equals((ColorVector) obj);
        }

        #endregion
        #region Equals(ColorVector)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="ColorVector"/> have the same
        /// value.</summary>
        /// <param name="vector">
        /// A <see cref="ColorVector"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="vector"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="R"/>, <see cref="G"/>, and <see
        /// cref="B"/> properties of the two <see cref="ColorVector"/> instances to test for value
        /// equality.</remarks>

        public bool Equals(ColorVector vector) {
            return (R == vector.R && G == vector.G && B == vector.B);
        }

        #endregion
        #region Equals(ColorVector, ColorVector)

        /// <summary>
        /// Determines whether two specified <see cref="ColorVector"/> instances have the same
        /// value.</summary>
        /// <param name="x">
        /// The first <see cref="ColorVector"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="ColorVector"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(ColorVector)"/> overload to test
        /// the two <see cref="ColorVector"/> instances for value equality.</remarks>

        public static bool Equals(ColorVector x, ColorVector y) {
            return x.Equals(y);
        }

        #endregion
        #endregion
    }
}
