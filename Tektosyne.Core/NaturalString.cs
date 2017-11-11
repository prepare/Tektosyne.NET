using System;

namespace Tektosyne {

    /// <summary>
    /// Provides a <see cref="String"/> with implicit natural sorting, using the sorting rules of
    /// the <see cref="StringComparison.CurrentCulture"/>.</summary>
    /// <remarks><para>
    /// <b>NaturalString</b> is an immmutable structure that relies on the <see
    /// cref="StringUtility.CompareNatural(String, String)"/> method to provide implicit natural
    /// sorting for a wrapped <see cref="String"/>, using the sorting rules of the <see
    /// cref="StringComparison.CurrentCulture"/>.
    /// </para><para>
    /// Use this structure when specifying an explicit <see cref="String"/> sorter is inconvenient.
    /// A similar structure, <see cref="OrdinalString"/>, provides implicit natural sorting with
    /// <see cref="StringComparison.Ordinal"/> sorting rules.</para></remarks>

    [Serializable]
    public struct NaturalString:
        IComparable<NaturalString>, IComparable, IEquatable<NaturalString> {
        #region NaturalString(String)

        /// <summary>
        /// Initializes a new instance of the <see cref="NaturalString"/> structure with the
        /// specified <see cref="String"/> value.</summary>
        /// <param name="value">
        /// The <see cref="String"/> value of the <see cref="NaturalString"/>.</param>

        public NaturalString(string value) {
            Value = value;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="NaturalString"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="NaturalString"/> instance that was created with the
        /// default constructor.</remarks>

        public static readonly NaturalString Empty = new NaturalString();

        #endregion
        #region Value

        /// <summary>
        /// The <see cref="String"/> value of the <see cref="NaturalString"/>.</summary>
        /// <remarks>
        /// <b>Value</b> may be a null reference or an empty string.</remarks>

        public readonly string Value;

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this instance of <see cref="NaturalString"/>.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code for the current <see cref="NaturalString"/>.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> returns the result of <see cref="String.GetHashCode"/> for the
        /// current <see cref="Value"/>, if not a null reference; otherwise, zero.</remarks>

        public override int GetHashCode() {
            return (Value == null ? 0 : Value.GetHashCode());
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="NaturalString"/>.
        /// </summary>
        /// <returns>
        /// The result of the <see cref="StringUtility.Validate"/> for the value of the <see
        /// cref="Value"/> property.</returns>

        public override string ToString() {
            return StringUtility.Validate(Value);
        }

        #endregion
        #region Public Operators
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="NaturalString"/> objects have the same value.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="NaturalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="NaturalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(NaturalString, NaturalString)"/> method to
        /// test the two <see cref="NaturalString"/> instances for value equality.</remarks>

        public static bool operator ==(NaturalString x, NaturalString y) {
            return Equals(x, y);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="NaturalString"/> objects have different values.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="NaturalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="NaturalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is different from the value of
        /// <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(NaturalString, NaturalString)"/> method to
        /// test the two <see cref="NaturalString"/> instances for value inequality.</remarks>

        public static bool operator !=(NaturalString x, NaturalString y) {
            return !Equals(x, y);
        }

        #endregion
        #region operator>

        /// <summary>
        /// Determines whether a specified <see cref="NaturalString"/> is greater than another
        /// specified <see cref="NaturalString"/>.</summary>
        /// <param name="x">
        /// The first <see cref="NaturalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="NaturalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is greater than the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="CompareTo(NaturalString)"/> method to determine the
        /// relative values of the two <see cref="NaturalString"/> instances.</remarks>

        public static bool operator >(NaturalString x, NaturalString y) {
            return (x.CompareTo(y) > 0);
        }

        #endregion
        #region operator<

        /// <summary>
        /// Determines whether a specified <see cref="NaturalString"/> is less than another
        /// specified <see cref="NaturalString"/>.</summary>
        /// <param name="x">
        /// The first <see cref="NaturalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="NaturalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is less than the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="CompareTo(NaturalString)"/> method to determine the
        /// relative values of the two <see cref="NaturalString"/> instances.</remarks>

        public static bool operator <(NaturalString x, NaturalString y) {
            return (x.CompareTo(y) < 0);
        }

        #endregion
        #region operator>=

        /// <summary>
        /// Determines whether a specified <see cref="NaturalString"/> is greater than or equal to
        /// another specified <see cref="NaturalString"/>.</summary>
        /// <param name="x">
        /// The first <see cref="NaturalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="NaturalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is greater than or equal to the value
        /// of <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="CompareTo(NaturalString)"/> method to determine the
        /// relative values of the two <see cref="NaturalString"/> instances.</remarks>

        public static bool operator >=(NaturalString x, NaturalString y) {
            return (x.CompareTo(y) >= 0);
        }

        #endregion
        #region operator<=

        /// <summary>
        /// Determines whether a specified <see cref="NaturalString"/> is less than or equal to
        /// another specified <see cref="NaturalString"/>.</summary>
        /// <param name="x">
        /// The first <see cref="NaturalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="NaturalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is less than or equal to the value of
        /// <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="CompareTo(NaturalString)"/> method to determine the
        /// relative values of the two <see cref="NaturalString"/> instances.</remarks>

        public static bool operator <=(NaturalString x, NaturalString y) {
            return (x.CompareTo(y) <= 0);
        }

        #endregion
        #region NaturalString

        /// <summary>
        /// Converts a <see cref="String"/> to the equivalent <see cref="NaturalString"/>.</summary>
        /// <param name="value">
        /// The <see cref="String"/> to wrap in an <see cref="NaturalString"/>.</param>
        /// <returns>
        /// A new <see cref="NaturalString"/> wrapping the specified <paramref name="value"/>.
        /// </returns>

        public static implicit operator NaturalString(String value) {
            return new NaturalString(value);
        }

        #endregion
        #region String

        /// <summary>
        /// Converts a <see cref="NaturalString"/> to the equivalent <see cref="String"/>.</summary>
        /// <param name="natural">
        /// The <see cref="NaturalString"/> instance whose <see cref="Value"/> property to extract.
        /// </param>
        /// <returns>
        /// The value of the <see cref="Value"/> property of the specified <paramref
        /// name="natural"/> instance.</returns>

        public static implicit operator String(NaturalString natural) {
            return natural.Value;
        }

        #endregion
        #endregion
        #region IComparable Members
        #region CompareTo(Object)

        /// <overloads>
        /// Compares two <see cref="NaturalString"/> objects and returns an indication of their
        /// relative values.</overloads>
        /// <summary>
        /// Compares this instance of <see cref="NaturalString"/> and a specified object, which must
        /// be a <see cref="NaturalString"/>, and returns an indication of their relative values.
        /// </summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this instance of <see cref="NaturalString"/>.
        /// </param>
        /// <returns><para>
        /// An <see cref="Int32"/> value indicating the relative order of this instance and
        /// <paramref name="obj"/>, as follows:
        /// </para><list type="table"><listheader>
        /// <term>Return Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description>The <see cref="Value"/> property of this instance is less than that of
        /// <paramref name="obj"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description>The <see cref="Value"/> property of this instance equals that of <paramref
        /// name="obj"/>.</description>
        /// </item><item>
        /// <term>Greater than zero</term><description><para>
        /// The <see cref="Value"/> property of this instance is greater than that of <paramref
        /// name="obj"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="obj"/> is a null reference.</para></description>
        /// </item></list></returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="obj"/> is neither a <see cref="NaturalString"/> nor a null reference.
        /// </exception>
        /// <remarks><para>
        /// The specified <paramref name="obj"/> must be either a null reference or an instance of
        /// <b>NaturalString</b>.
        /// </para><para>
        /// <b>CompareTo</b> determines the relative order of the two instances by calling <see
        /// cref="StringUtility.CompareNatural(String, String)"/>.</para></remarks>

        int IComparable.CompareTo(object obj) {
            if (obj == null) return 1;

            if (!(obj is NaturalString))
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "obj", Strings.ArgumentTypeMismatch, "NaturalString");

            return CompareTo((NaturalString) obj);
        }

        #endregion
        #region CompareTo(NaturalString)

        /// <summary>
        /// Compares this instance and a specified <see cref="NaturalString"/> and returns an
        /// indication of their relative values.</summary>
        /// <param name="natural">
        /// A <see cref="NaturalString"/> to compare to this instance.</param>
        /// <returns><para>
        /// An <see cref="Int32"/> value indicating the relative order of this instance and
        /// <paramref name="natural"/>, as follows:
        /// </para><list type="table"><listheader>
        /// <term>Return Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description>The <see cref="Value"/> property of this instance is less than that of
        /// <paramref name="natural"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description>The <see cref="Value"/> property of this instance equals that of <paramref
        /// name="natural"/>.</description>
        /// </item><item>
        /// <term>Greater than zero</term><description>
        /// The <see cref="Value"/> property of this instance is greater than that of <paramref
        /// name="natural"/>.</description>
        /// </item></list></returns>
        /// <remarks>
        /// <b>CompareTo</b> determines the relative order of the two instances by calling <see
        /// cref="StringUtility.CompareNatural"/>, using the sorting rules of the <see
        /// cref="StringComparison.CurrentCulture"/>.</remarks>

        public int CompareTo(NaturalString natural) {
            return Value.CompareNatural(natural.Value, StringComparison.CurrentCulture);
        }

        #endregion
        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="NaturalString"/> objects have the same value.
        /// </overloads>
        /// <summary>
        /// Determines whether this instance of <see cref="NaturalString"/> and a specified object,
        /// which must be a <see cref="NaturalString"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this instance of <see cref="NaturalString"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="NaturalString"/> instance
        /// and its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="NaturalString"/> instance,
        /// <b>Equals</b> calls <see cref="StringUtility.CompareNatural(String, String)"/> to test
        /// the <see cref="Value"/> properties of the two <see cref="NaturalString"/> instances for
        /// value equality.</remarks>

        public override bool Equals(object obj) {
            if (!(obj is NaturalString))
                return false;

            return Equals((NaturalString) obj);
        }

        #endregion
        #region Equals(NaturalString)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="NaturalString"/> have the
        /// same value.</summary>
        /// <param name="natural">
        /// A <see cref="NaturalString"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="natural"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> calls <see cref="StringUtility.CompareNatural"/> to test the <see
        /// cref="Value"/> properties of the two <see cref="NaturalString"/> instances for value
        /// equality, using the sorting rules of the <see cref="StringComparison.CurrentCulture"/>.
        /// </remarks>

        public bool Equals(NaturalString natural) {
            return (Value.CompareNatural(natural.Value, StringComparison.CurrentCulture) == 0);
        }

        #endregion
        #region Equals(NaturalString, NaturalString)

        /// <summary>
        /// Determines whether two specified <see cref="NaturalString"/> objects have the same
        /// value.</summary>
        /// <param name="x">
        /// The first <see cref="NaturalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="NaturalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> calls <see cref="StringUtility.CompareNatural"/> to test the <see
        /// cref="Value"/> properties of the two <see cref="NaturalString"/> instances for value
        /// equality, using the sorting rules of the <see cref="StringComparison.CurrentCulture"/>.
        /// </remarks>

        public static bool Equals(NaturalString x, NaturalString y) {
            return (x.Value.CompareNatural(y.Value, StringComparison.CurrentCulture) == 0);
        }

        #endregion
        #endregion
    }
}
