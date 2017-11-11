using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Tektosyne {

    /// <summary>
    /// Provides auxiliary methods for <b>System.String</b>.</summary>

    public static class StringUtility {
        #region CompareNatural(String, String)

        /// <overloads>
        /// Compares two specified strings and returns an indication of their relative values,
        /// according to a natural sorting order.</overloads>
        /// <summary>
        /// Compares two specified strings and returns an indication of their relative values,
        /// according to a natural sorting order and using the sorting rules of the <see
        /// cref="StringComparison.CurrentCulture"/>.</summary>
        /// <param name="x">
        /// The first <see cref="String"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="String"/> to compare.</param>
        /// <returns><para>
        /// An <see cref="Int32"/> value indicating the relative order of <paramref name="x"/> and
        /// <paramref name="y"/>, as follows:
        /// </para><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description><paramref name="x"/> is less than <paramref name="y"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description><paramref name="x"/> equals <paramref name="y"/>.</description>
        /// </item><item>
        /// <term>Greater than zero</term>
        /// <description><paramref name="x"/> is greater than <paramref name="y"/>.</description>
        /// </item></list></returns>
        /// <remarks><para>
        /// <b>CompareNatural</b> is compatible with the <see cref="Comparison{T}"/> delegate and
        /// can be passed to various sorting methods.
        /// </para><para>
        /// <b>CompareNatural</b> invokes the second <see cref="CompareNatural(String, String,
        /// StringComparison)"/> overload with the specified arguments, using the sorting rules of
        /// the <see cref="StringComparison.CurrentCulture"/>. Please see there for details.
        /// </para></remarks>

        public static int CompareNatural(this string x, string y) {
            return CompareNatural(x, y, StringComparison.CurrentCulture);
        }

        #endregion
        #region CompareNatural(String, String, StringComparison)

        /// <summary>
        /// Compares two specified strings and returns an indication of their relative values,
        /// according to a natural sorting order and using the specified sorting rules.</summary>
        /// <param name="x">
        /// The first <see cref="String"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="String"/> to compare.</param>
        /// <param name="comparison">
        /// A <see cref="StringComparison"/> value indicating how sequences of non-digit characters
        /// should be compared.</param>
        /// <returns><para>
        /// An <see cref="Int32"/> value indicating the relative order of <paramref name="x"/> and
        /// <paramref name="y"/>, as follows:
        /// </para><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description><paramref name="x"/> is less than <paramref name="y"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description><paramref name="x"/> equals <paramref name="y"/>.</description>
        /// </item><item>
        /// <term>Greater than zero</term>
        /// <description><paramref name="x"/> is greater than <paramref name="y"/>.</description>
        /// </item></list></returns>
        /// <remarks><para>
        /// Either or both arguments may be null references or empty strings. A null reference is
        /// considered smaller than a valid string, whether empty or not, and two null references
        /// are considered equal. An empty string is considered smaller than a non-empty string, and
        /// two empty strings are considered equal.
        /// </para><para>
        /// Non-empty strings are broken into sequences of decimal digits and non-digit characters,
        /// as determined by <see cref="Char.IsDigit"/>. A digit sequence is considered greater than
        /// a non-digit sequence. Two digit sequences are compared by their numeric values which
        /// must fit in the <see cref="Int32"/> type. Two non-digit sequences are compared using the
        /// <see cref="String.Compare"/> method with the specified <paramref name="comparison"/>
        /// options. The return value of <b>CompareNatural</b> is determined by the first pair of
        /// sequences that are not considered equal.</para></remarks>

        public static int CompareNatural(this string x, string y, StringComparison comparison) {

            // check for null references
            if (x == null)
                return (y == null ? 0 : -1);
            else if (y == null)
                return 1;

            int x0 = 0, x1, y0 = 0, y1;

            while (true) {
                // check for end of string
                if (x0 == x.Length)
                    return (y0 == y.Length ? 0 : -1);
                else if (y0 == y.Length)
                    return 1;

                // determine digit-ness of current sequences
                bool dx = Char.IsDigit(x, x0);
                bool dy = Char.IsDigit(y, y0);

                // check for different digit-ness
                if (dx && !dy)
                    return 1;
                else if (!dx && dy)
                    return -1;

                // find longest sequences of equal digit-ness
                for (x1 = x0 + 1; x1 < x.Length && dx == Char.IsDigit(x, x1); x1++) ;
                for (y1 = y0 + 1; y1 < y.Length && dy == Char.IsDigit(y, y1); y1++) ;

                if (dx) {
                    // compare digit sequence by numeric value
                    int vx = 0, vy = 0;

                    unchecked {
                        for (; x0 < x1; x0++) vx = 10 * vx + (int) x[x0] - (int) '0';
                        for (; y0 < y1; y0++) vy = 10 * vy + (int) y[y0] - (int) '0';
                    }

                    if (vx != vy) return (vx - vy);
                }
                else {
                    // compare shortest common non-digit sequence
                    int count = Math.Min(x1 - x0, y1 - y0);
                    int result = String.Compare(x, x0, y, y0, count, comparison);

                    // compare by total length for equal prefixes
                    if (result == 0) result = ((x1 - x0) - (y1 - y0));
                    if (result != 0) return result;
                }

                // advance to next sequences
                x0 = x1; y0 = y1;
            }
        }

        #endregion
        #region CompareOrdinal(String, String)

        /// <summary>
        /// Compares two specified strings and returns an indication of their relative values,
        /// according to a natural sorting order and using <see cref="StringComparison.Ordinal"/>
        /// sorting rules.</summary>
        /// <param name="x">
        /// The first <see cref="String"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="String"/> to compare.</param>
        /// <returns><para>
        /// An <see cref="Int32"/> value indicating the relative order of <paramref name="x"/> and
        /// <paramref name="y"/>, as follows:
        /// </para><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description><paramref name="x"/> is less than <paramref name="y"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description><paramref name="x"/> equals <paramref name="y"/>.</description>
        /// </item><item>
        /// <term>Greater than zero</term>
        /// <description><paramref name="x"/> is greater than <paramref name="y"/>.</description>
        /// </item></list></returns>
        /// <remarks><para>
        /// <b>CompareOrdinal</b> is compatible with the <see cref="Comparison{T}"/> delegate and
        /// can be passed to various sorting methods.
        /// </para><para>
        /// <b>CompareOrdinal</b> invokes <see cref="CompareNatural(String, String,
        /// StringComparison)"/> overload with the specified arguments and the value <see
        /// cref="StringComparison.Ordinal"/>. Please see there for details.</para></remarks>

        public static int CompareOrdinal(this string x, string y) {
            return CompareNatural(x, y, StringComparison.Ordinal);
        }

        #endregion
        #region IsRichText

        /// <summary>
        /// Determines whether the specified <see cref="String"/> contains text in Rich Text Format
        /// (RTF).</summary>
        /// <param name="value">
        /// The <see cref="String"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="value"/> starts with the literal string
        /// "{\rtf"; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>IsRichText</b> returns <c>false</c> if the specified <paramref name="value"/> is a
        /// null reference or an empty string.</remarks>

        public static bool IsRichText(this string value) {
            return (value != null && value.StartsWith(@"{\rtf", StringComparison.Ordinal));
        }

        #endregion
        #region IsValidEmail

        /// <summary>
        /// Determines whether the specified <see cref="String"/> contains a valid e-mail address.
        /// </summary>
        /// <param name="value">
        /// The <see cref="String"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="value"/> contains a valid e-mail address;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>IsValidEmail</b> returns <c>false</c> if the specified <paramref name="value"/> is a
        /// null reference or an empty string.</remarks>

        public static bool IsValidEmail(this string value) {
            return (!String.IsNullOrEmpty(value) && Regex.IsMatch(value,
                @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|" +
                @"(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"));
        }

        #endregion
        #region PackSpace

        /// <summary>
        /// Replaces each sequence of whitespace in the specified <see cref="String"/> with a single
        /// space character.</summary>
        /// <param name="value">
        /// The <see cref="String"/> whose whitespace sequences should be packed.</param>
        /// <returns>
        /// A copy of <paramref name="value"/> in which each sequence of whitespace has been
        /// replaced by a single space character.</returns>
        /// <remarks><para>
        /// <b>PackSpace</b> uses the <see cref="Regex"/> expression "\s+" to identify sequences of
        /// whitespace. The replacement character is a standard space (Unicode character 32, SPACE).
        /// </para><para>
        /// <b>PackSpace</b> returns an empty string if <paramref name="value"/> is a null reference
        /// or an empty string.</para></remarks>

        public static string PackSpace(this string value) {
            return (String.IsNullOrEmpty(value) ? "" : Regex.Replace(value, @"\s+", " "));
        }

        #endregion
        #region Validate(String)

        /// <overloads>
        /// Returns the specified <see cref="String"/> if it is neither empty nor a null reference,
        /// otherwise a valid replacement text.</overloads>
        /// <summary>
        /// Returns the specified <see cref="String"/> if it is neither empty nor a null reference,
        /// otherwise a short debug text.</summary>
        /// <param name="value">
        /// The <see cref="String"/> to validate.</param>
        /// <returns>
        /// The literal string "(null)" if <paramref name="value"/> is a null reference; otherwise,
        /// the literal string "(empty)" if <paramref name="value"/> is an empty string; otherwise,
        /// the specified <paramref name="value"/>.</returns>
        /// <remarks>
        /// This <b>Validate</b> overload is intended for debugging.</remarks>

        public static string Validate(string value) {
            if (value == null) return "(null)";
            if (value.Length == 0) return "(empty)";
            return value;
        }

        #endregion
        #region Validate(String, String)

        /// <summary>
        /// Returns the specified <see cref="String"/> if it is neither empty nor a null reference,
        /// otherwise the specified replacement text.</summary>
        /// <param name="value">
        /// The <see cref="String"/> to validate.</param>
        /// <param name="replace">
        /// The <see cref="String"/> to return if <paramref name="value"/> is a null reference or an
        /// empty string.</param>
        /// <returns>
        /// The specified <paramref name="replace"/> text if <paramref name="value"/> is a null
        /// reference or an empty string; otherwise, the specified <paramref name="value"/>.
        /// </returns>
        /// <remarks>
        /// This <b>Validate</b> overload is intended for generating output visible to the user.
        /// </remarks>

        public static string Validate(string value, string replace) {
            return (String.IsNullOrEmpty(value) ? replace : value);
        }

        #endregion
        #region Validate<T>(T)

        /// <summary>
        /// Returns a <see cref="String"/> representation of the specified object if it is neither
        /// empty nor a null reference, otherwise a short debug text.</summary>
        /// <typeparam name="T">
        /// The type of the object to validate.</typeparam>
        /// <param name="value">
        /// The object to validate.</param>
        /// <returns>
        /// The literal string "(null)" if <paramref name="value"/> is a null reference; otherwise,
        /// the culture-invariant string representation of the specified <paramref name="value"/> if
        /// it is not an empty string; otherwise, the literal string "(empty)".</returns>
        /// <remarks>
        /// This <b>Validate</b> overload is intended for debugging.</remarks>

        public static string Validate<T>(T value) {
            if (value == null) return "(null)";

            string replace = String.Format(CultureInfo.InvariantCulture, "{0}", value);
            return (replace.Length == 0 ? "(empty)" : replace);
        }

        #endregion
        #region Validate<T>(T, String)

        /// <summary>
        /// Returns a <see cref="String"/> representation of the specified object if it is neither
        /// empty nor a null reference, otherwise the specified replacement text.</summary>
        /// <typeparam name="T">
        /// The type of the object to validate.</typeparam>
        /// <param name="value">
        /// The object to validate.</param>
        /// <param name="replace">
        /// The <see cref="String"/> to return if <paramref name="value"/> is a null reference, or 
        /// its string representation is a null reference or an empty string.</param>
        /// <returns>
        /// The specified <paramref name="replace"/> text if <paramref name="value"/> is a null
        /// reference, or its string representation is a null reference or an empty string;
        /// otherwise, the culture-invariant string representation of the specified <paramref
        /// name="value"/>.</returns>
        /// <remarks>
        /// This <b>Validate</b> overload is intended for generating output visible to the user.
        /// </remarks>

        public static string Validate<T>(T value, string replace) {
            if (value == null) return replace;

            string format = String.Format(CultureInfo.CurrentCulture, "{0}", value);
            return (String.IsNullOrEmpty(format) ? replace : format);
        }

        #endregion
        #region ValidateCollection<T>

        /// <summary>
        /// Returns a <see cref="String"/> that represents all elements in the specified collection,
        /// using a short debug text for null references and empty elements.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> collection whose elements to format.</param>
        /// <returns>
        /// A <see cref="String"/> containing the culture-invariant string representations of all
        /// elements in the specified <paramref name="collection"/>.</returns>
        /// <remarks><para>
        /// <b>ValidateCollection</b> returns the literal string "(null)" if the specified <paramref
        /// name="collection"/> is a null reference.
        /// </para><para>
        /// Otherwise, <b>ValidateCollection</b> returns a comma-separated list of the string
        /// representations of all <paramref name="collection"/> elements, surrounded by square
        /// brackets. All elements are formatted using <see cref="Validate{T}"/>.</para></remarks>

        public static string ValidateCollection<T>(IEnumerable<T> collection) {

            if (collection == null) return "(null)";
            var sb = new StringBuilder("[");

            foreach (T item in collection) {
                if (sb.Length > 1) sb.Append(", ");
                sb.Append(Validate(item));
            }

            sb.Append("]");
            return sb.ToString();
        }

        #endregion
        #region ValidOrNull

        /// <summary>
        /// Returns the specified <see cref="String"/> if it has any valid content, otherwise a null
        /// reference.</summary>
        /// <param name="value">
        /// The <see cref="String"/> to validate.</param>
        /// <param name="invalid">
        /// An optional <see cref="String"/> that should be considered invalid. The default is a
        /// null reference.</param>
        /// <returns>
        /// A null reference if <paramref name="value"/> is a null reference, an empty string, a
        /// string consisting only of white-space characters, or the specified <paramref
        /// name="invalid"/> text; otherwise, the specified <paramref name="value"/>.</returns>
        /// <remarks>
        /// <b>ValidOrNull</b> uses <see cref="StringComparison.Ordinal"/> sorting rules to compare
        /// the specified <paramref name="value"/> and <paramref name="invalid"/> text for equality,
        /// assuming <paramref name="invalid"/> is not a null reference.</remarks>

        public static string ValidOrNull(this string value, string invalid = null) {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            if (invalid != null && String.CompareOrdinal(value, invalid) == 0)
                return null;

            return value;
        }

        #endregion
    }
}
