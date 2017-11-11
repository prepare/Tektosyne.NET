using System;

namespace Tektosyne {

    /// <summary>
    /// Provides a <see cref="String"/> with implicit natural sorting, using <see
    /// cref="StringComparison.Ordinal"/> sorting rules.</summary>
    /// <remarks><para>
    /// <b>OrdinalString</b> is an immmutable structure that relies on the <see
    /// cref="StringUtility.CompareOrdinal"/> method to provide implicit natural sorting for a
    /// wrapped <see cref="String"/>, using <see cref="StringComparison.Ordinal"/> sorting rules.
    /// </para><para>
    /// Use this structure when specifying an explicit <see cref="String"/> sorter is inconvenient.
    /// A similar structure, <see cref="NaturalString"/>, provides implicit natural sorting with the
    /// sorting rules of the <see cref="StringComparison.CurrentCulture"/>.</para></remarks>

    [Serializable]
    public struct OrdinalString:
        IComparable<OrdinalString>, IComparable, IEquatable<OrdinalString> {
        #region OrdinalString(String)

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdinalString"/> structure with the
        /// specified <see cref="String"/> value.</summary>
        /// <param name="value">
        /// The <see cref="String"/> value of the <see cref="OrdinalString"/>.</param>

        public OrdinalString(string value) {
            Value = value;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="OrdinalString"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="OrdinalString"/> instance that was created with the
        /// default constructor.</remarks>

        public static readonly OrdinalString Empty = new OrdinalString();

        #endregion
        #region Value

        /// <summary>
        /// The <see cref="String"/> value of the <see cref="OrdinalString"/>.</summary>
        /// <remarks>
        /// <b>Value</b> may be a null reference or an empty string.</remarks>

        public readonly string Value;

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this instance of <see cref="OrdinalString"/>.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code for the current <see cref="OrdinalString"/>.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> returns the result of <see cref="String.GetHashCode"/> for the
        /// current <see cref="Value"/>, if not a null reference; otherwise, zero.</remarks>

        public override int GetHashCode() {
            return (Value == null ? 0 : Value.GetHashCode());
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="OrdinalString"/>.
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
        /// Determines whether two <see cref="OrdinalString"/> objects have the same value.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="OrdinalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="OrdinalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(OrdinalString, OrdinalString)"/> method to
        /// test the two <see cref="OrdinalString"/> instances for value equality.</remarks>

        public static bool operator ==(OrdinalString x, OrdinalString y) {
            return Equals(x, y);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="OrdinalString"/> objects have different values.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="OrdinalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="OrdinalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is different from the value of
        /// <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(OrdinalString, OrdinalString)"/> method to
        /// test the two <see cref="OrdinalString"/> instances for value inequality.</remarks>

        public static bool operator !=(OrdinalString x, OrdinalString y) {
            return !Equals(x, y);
        }

        #endregion
        #region operator>

        /// <summary>
        /// Determines whether a specified <see cref="OrdinalString"/> is greater than another
        /// specified <see cref="OrdinalString"/>.</summary>
        /// <param name="x">
        /// The first <see cref="OrdinalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="OrdinalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is greater than the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="CompareTo(OrdinalString)"/> method to determine the
        /// relative values of the two <see cref="OrdinalString"/> instances.</remarks>

        public static bool operator >(OrdinalString x, OrdinalString y) {
            return (x.CompareTo(y) > 0);
        }

        #endregion
        #region operator<

        /// <summary>
        /// Determines whether a specified <see cref="OrdinalString"/> is less than another
        /// specified <see cref="OrdinalString"/>.</summary>
        /// <param name="x">
        /// The first <see cref="OrdinalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="OrdinalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is less than the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="CompareTo(OrdinalString)"/> method to determine the
        /// relative values of the two <see cref="OrdinalString"/> instances.</remarks>

        public static bool operator <(OrdinalString x, OrdinalString y) {
            return (x.CompareTo(y) < 0);
        }

        #endregion
        #region operator>=

        /// <summary>
        /// Determines whether a specified <see cref="OrdinalString"/> is greater than or equal to
        /// another specified <see cref="OrdinalString"/>.</summary>
        /// <param name="x">
        /// The first <see cref="OrdinalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="OrdinalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is greater than or equal to the value
        /// of <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="CompareTo(OrdinalString)"/> method to determine the
        /// relative values of the two <see cref="OrdinalString"/> instances.</remarks>

        public static bool operator >=(OrdinalString x, OrdinalString y) {
            return (x.CompareTo(y) >= 0);
        }

        #endregion
        #region operator<=

        /// <summary>
        /// Determines whether a specified <see cref="OrdinalString"/> is less than or equal to
        /// another specified <see cref="OrdinalString"/>.</summary>
        /// <param name="x">
        /// The first <see cref="OrdinalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="OrdinalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is less than or equal to the value of
        /// <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="CompareTo(OrdinalString)"/> method to determine the
        /// relative values of the two <see cref="OrdinalString"/> instances.</remarks>

        public static bool operator <=(OrdinalString x, OrdinalString y) {
            return (x.CompareTo(y) <= 0);
        }

        #endregion
        #region OrdinalString

        /// <summary>
        /// Converts a <see cref="String"/> to the equivalent <see cref="OrdinalString"/>.</summary>
        /// <param name="value">
        /// The <see cref="String"/> to wrap in an <see cref="OrdinalString"/>.</param>
        /// <returns>
        /// A new <see cref="OrdinalString"/> wrapping the specified <paramref name="value"/>.
        /// </returns>

        public static implicit operator OrdinalString(String value) {
            return new OrdinalString(value);
        }

        #endregion
        #region String

        /// <summary>
        /// Converts an <see cref="OrdinalString"/> to the equivalent <see cref="String"/>.
        /// </summary>
        /// <param name="ordinal">
        /// The <see cref="OrdinalString"/> instance whose <see cref="Value"/> property to extract.
        /// </param>
        /// <returns>
        /// The value of the <see cref="Value"/> property of the specified <paramref
        /// name="ordinal"/> instance.</returns>

        public static implicit operator String(OrdinalString ordinal) {
            return ordinal.Value;
        }

        #endregion
        #endregion
        #region IComparable Members
        #region CompareTo(Object)

        /// <overloads>
        /// Compares two <see cref="OrdinalString"/> objects and returns an indication of their
        /// relative values.</overloads>
        /// <summary>
        /// Compares this instance of <see cref="OrdinalString"/> and a specified object, which must
        /// be a <see cref="OrdinalString"/>, and returns an indication of their relative values.
        /// </summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this instance of <see cref="OrdinalString"/>.
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
        /// <paramref name="obj"/> is neither a <see cref="OrdinalString"/> nor a null reference.
        /// </exception>
        /// <remarks><para>
        /// The specified <paramref name="obj"/> must be either a null reference or an instance of
        /// <b>OrdinalString</b>.
        /// </para><para>
        /// <b>CompareTo</b> determines the relative order of the two instances by calling <see
        /// cref="StringUtility.CompareOrdinal"/>.</para></remarks>

        int IComparable.CompareTo(object obj) {
            if (obj == null) return 1;

            if (!(obj is OrdinalString))
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "obj", Strings.ArgumentTypeMismatch, "OrdinalString");

            return CompareTo((OrdinalString) obj);
        }

        #endregion
        #region CompareTo(OrdinalString)

        /// <summary>
        /// Compares this instance and a specified <see cref="OrdinalString"/> and returns an
        /// indication of their relative values.</summary>
        /// <param name="ordinal">
        /// A <see cref="OrdinalString"/> to compare to this instance.</param>
        /// <returns><para>
        /// An <see cref="Int32"/> value indicating the relative order of this instance and
        /// <paramref name="ordinal"/>, as follows:
        /// </para><list type="table"><listheader>
        /// <term>Return Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description>The <see cref="Value"/> property of this instance is less than that of
        /// <paramref name="ordinal"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description>The <see cref="Value"/> property of this instance equals that of <paramref
        /// name="ordinal"/>.</description>
        /// </item><item>
        /// <term>Greater than zero</term><description>
        /// The <see cref="Value"/> property of this instance is greater than that of <paramref
        /// name="ordinal"/>.</description>
        /// </item></list></returns>
        /// <remarks>
        /// <b>CompareTo</b> determines the relative order of the two instances by calling <see
        /// cref="StringUtility.CompareOrdinal"/>.</remarks>

        public int CompareTo(OrdinalString ordinal) {
            return Value.CompareOrdinal(ordinal.Value);
        }

        #endregion
        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="OrdinalString"/> objects have the same value.
        /// </overloads>
        /// <summary>
        /// Determines whether this instance of <see cref="OrdinalString"/> and a specified object,
        /// which must be a <see cref="OrdinalString"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this instance of <see cref="OrdinalString"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="OrdinalString"/> instance
        /// and its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="OrdinalString"/> instance,
        /// <b>Equals</b> calls <see cref="StringUtility.CompareOrdinal"/> to test the <see
        /// cref="Value"/> properties of the two <see cref="OrdinalString"/> instances for value
        /// equality.</remarks>

        public override bool Equals(object obj) {
            if (!(obj is OrdinalString))
                return false;

            return Equals((OrdinalString) obj);
        }

        #endregion
        #region Equals(OrdinalString)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="OrdinalString"/> have the
        /// same value.</summary>
        /// <param name="ordinal">
        /// A <see cref="OrdinalString"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="ordinal"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> calls <see cref="StringUtility.CompareOrdinal"/> to test the <see
        /// cref="Value"/> properties of the two <see cref="OrdinalString"/> instances for value
        /// equality.</remarks>

        public bool Equals(OrdinalString ordinal) {
            return (Value.CompareOrdinal(ordinal.Value) == 0);
        }

        #endregion
        #region Equals(OrdinalString, OrdinalString)

        /// <summary>
        /// Determines whether two specified <see cref="OrdinalString"/> objects have the same
        /// value.</summary>
        /// <param name="x">
        /// The first <see cref="OrdinalString"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="OrdinalString"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> calls <see cref="StringUtility.CompareOrdinal"/> to test the <see
        /// cref="Value"/> properties of the two <see cref="OrdinalString"/> instances for value
        /// equality.</remarks>

        public static bool Equals(OrdinalString x, OrdinalString y) {
            return (x.Value.CompareOrdinal(y.Value) == 0);
        }

        #endregion
        #endregion
    }
}
