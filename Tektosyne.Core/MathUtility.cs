using System;

namespace Tektosyne {

    /// <summary>
    /// Provides auxiliary methods for <b>System.Math</b>.</summary>

    public static class MathUtility {
        #region Compare(Double, Double, Double)

        /// <overloads>
        /// Compares two specified floating-point numbers and returns an indication of their
        /// relative magnitudes, given the specified epsilon.</overloads>
        /// <summary>
        /// Compares two specified <see cref="Double"/> numbers and returns an indication of their
        /// relative magnitudes, given the specified epsilon.</summary>
        /// <param name="a">
        /// The first <see cref="Double"/> number to compare.</param>
        /// <param name="b">
        /// The second <see cref="Double"/> number to compare.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which <paramref name="a"/> and <paramref name="b"/>
        /// should be considered equal.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term><description><paramref name="a"/> is less than <paramref
        /// name="b"/> by more than <paramref name="epsilon"/>.</description>
        /// </item><item>
        /// <term>Zero</term><description>
        /// The absolute difference between <paramref name="a"/> and <paramref name="b"/> is less
        /// than or equal to <paramref name="epsilon"/>.</description>
        /// </item><item>
        /// <term>Greater than zero</term><description><paramref name="a"/> is greater than
        /// <paramref name="b"/> by more than <paramref name="epsilon"/>.</description>
        /// </item></list></returns>
        /// <remarks>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but <b>Compare</b>
        /// does not check this condition.</remarks>

        public static int Compare(double a, double b, double epsilon) {

            double delta = a - b;
            if (Math.Abs(delta) <= epsilon) return 0;
            return (delta < 0 ? -1 : 1);
        }

        #endregion
        #region Compare(Single, Single, Single)

        /// <summary>
        /// Compares two specified <see cref="Single"/> numbers and returns an indication of their
        /// relative magnitudes, given the specified epsilon.</summary>
        /// <param name="a">
        /// The first <see cref="Single"/> number to compare.</param>
        /// <param name="b">
        /// The second <see cref="Single"/> number to compare.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which <paramref name="a"/> and <paramref name="b"/>
        /// should be considered equal.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term><description><paramref name="a"/> is less than <paramref
        /// name="b"/> by more than <paramref name="epsilon"/>.</description>
        /// </item><item>
        /// <term>Zero</term><description>
        /// The absolute difference between <paramref name="a"/> and <paramref name="b"/> is less
        /// than or equal to <paramref name="epsilon"/>.</description>
        /// </item><item>
        /// <term>Greater than zero</term><description><paramref name="a"/> is greater than
        /// <paramref name="b"/> by more than <paramref name="epsilon"/>.</description>
        /// </item></list></returns>
        /// <remarks>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but <b>Compare</b>
        /// does not check this condition.</remarks>

        public static int Compare(float a, float b, float epsilon) {

            float delta = a - b;
            if (Math.Abs(delta) <= epsilon) return 0;
            return (delta < 0 ? -1 : 1);
        }

        #endregion
        #region Equals(Double, Double, Double)

        /// <overloads>
        /// Determines whether two specified floating-point numbers are equal, given the specified
        /// epsilon.</overloads>
        /// <summary>
        /// Determines whether two specified <see cref="Double"/> numbers are equal, given the
        /// specified epsilon.</summary>
        /// <param name="a">
        /// The first <see cref="Double"/> number to compare.</param>
        /// <param name="b">
        /// The second <see cref="Double"/> number to compare.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which <paramref name="a"/> and <paramref name="b"/>
        /// should be considered equal.</param>
        /// <returns>
        /// <c>true</c> if the absolute difference between <paramref name="a"/> and <paramref
        /// name="b"/> is equal to or less than <paramref name="epsilon"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but <b>Equals</b>
        /// does not check this condition.</remarks>

        public static bool Equals(double a, double b, double epsilon) {
            return (Math.Abs(a - b) <= epsilon);
        }

        #endregion
        #region Equals(Single, Single, Single)

        /// <summary>
        /// Determines whether two specified <see cref="Single"/> numbers are equal, given the
        /// specified epsilon.</summary>
        /// <param name="a">
        /// The first <see cref="Single"/> number to compare.</param>
        /// <param name="b">
        /// The second <see cref="Single"/> number to compare.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which <paramref name="a"/> and <paramref name="b"/>
        /// should be considered equal.</param>
        /// <returns>
        /// <c>true</c> if the absolute difference between <paramref name="a"/> and <paramref
        /// name="b"/> is equal to or less than <paramref name="epsilon"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but <b>Equals</b>
        /// does not check this condition.</remarks>

        public static bool Equals(float a, float b, float epsilon) {
            return (Math.Abs(a - b) <= epsilon);
        }

        #endregion
        #region IsPrime(Int32)

        /// <overloads>
        /// Determines whether the specified number is prime.</overloads>
        /// <summary>
        /// Determines whether the specified <see cref="Int32"/> number is prime.</summary>
        /// <param name="candidate">
        /// The <see cref="Int32"/> number to examine.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="candidate"/> is prime; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="candidate"/> is equal to or less than zero.</exception>
        /// <remarks>
        /// <b>IsPrime</b> performs trial divisions of the specified <paramref name="candidate"/>
        /// against any number between two and its square root.</remarks>

        public static bool IsPrime(int candidate) {
            if (candidate <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "candidate", candidate, Strings.ArgumentNotPositive);

            if ((candidate & 1) == 0)
                return (candidate == 2);

            int root = (int) Math.Sqrt(candidate);
            for (int i = 3; i <= root; i += 2)
                if ((candidate % i) == 0)
                    return false;

            return true;
        }

        #endregion
        #region IsPrime(UInt32)

        /// <summary>
        /// Determines whether the specified <see cref="UInt32"/> number is prime.</summary>
        /// <param name="candidate">
        /// The <see cref="UInt32"/> number to examine.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="candidate"/> is prime; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>IsPrime</b> performs trial divisions of the specified <paramref name="candidate"/>
        /// against any number between two and its square root.</remarks>

        [CLSCompliant(false)]
        public static bool IsPrime(uint candidate) {
            if ((candidate & 1) == 0)
                return (candidate == 2);

            uint root = (uint) Math.Sqrt(candidate);
            for (uint i = 3; i <= root; i += 2)
                if ((candidate % i) == 0)
                    return false;

            return true;
        }

        #endregion
        #region Normalize(Double[])

        /// <overloads>
        /// Normalizes the specified <see cref="Array"/> of non-negative numbers.</overloads>
        /// <summary>
        /// Normalizes the specified <see cref="Array"/> of non-negative <see cref="Double"/>
        /// numbers.</summary>
        /// <param name="array">
        /// The <see cref="Array"/> of non-negative <see cref="Double"/> numbers to normalize.
        /// </param>
        /// <returns>
        /// The sum of all values in <paramref name="array"/> before normalization.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="array"/> contains a negative value.</exception>
        /// <remarks><para>
        /// <b>Normalize</b> divides all values in <paramref name="array"/> by the sum of all
        /// values, thus normalizing the list of values to a partitioning of the standard interval
        /// [0,1].
        /// </para><para>
        /// If the sum of all values is zero, <b>Normalize</b> sets all values to the quotient
        /// 1/<see cref="Array.Length"/> instead.</para></remarks>

        public static double Normalize(double[] array) {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException("array");

            double sum = 0.0;
            foreach (double value in array) {
                if (value < 0.0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(
                        "array", value, Strings.ArgumentContainsNegative);

                sum += value;
            }

            if (sum != 0.0) {
                for (int i = 0; i < array.Length; i++)
                    array[i] /= sum;
            } else {
                double value = 1.0 / array.Length;
                for (int i = 0; i < array.Length; i++)
                    array[i] = value;
            }

            return sum;
        }

        #endregion
        #region Normalize(Single[])

        /// <summary>
        /// Normalizes the specified <see cref="Array"/> of non-negative <see cref="Single"/>
        /// numbers.</summary>
        /// <param name="array">
        /// The <see cref="Array"/> of non-negative <see cref="Single"/> numbers to normalize.
        /// </param>
        /// <returns>
        /// The sum of all values in <paramref name="array"/> before normalization.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="array"/> contains a negative value.</exception>
        /// <remarks><para>
        /// <b>Normalize</b> divides all values in <paramref name="array"/> by the sum of all
        /// values, thus normalizing the list of values to a partitioning of the standard interval
        /// [0,1].
        /// </para><para>
        /// If the sum of all values is zero, <b>Normalize</b> sets all values to the quotient
        /// 1/<see cref="Array.Length"/> instead.</para></remarks>

        public static float Normalize(float[] array) {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException("array");

            float sum = 0f;
            foreach (float value in array) {
                if (value < 0f)
                    ThrowHelper.ThrowArgumentOutOfRangeException(
                        "array", value, Strings.ArgumentContainsNegative);

                sum += value;
            }

            if (sum != 0f) {
                for (int i = 0; i < array.Length; i++)
                    array[i] /= sum;
            } else {
                float value = 1f / array.Length;
                for (int i = 0; i < array.Length; i++)
                    array[i] = value;
            }

            return sum;
        }

        #endregion
        #region Restrict(Decimal, Decimal, Decimal)

        /// <overloads>
        /// Restricts the specified number to the specified range.</overloads>
        /// <summary>
        /// Restricts the specified <see cref="Decimal"/> number to the specified range.</summary>
        /// <param name="a">
        /// The <see cref="Decimal"/> number to restrict.</param>
        /// <param name="min">
        /// An <see cref="Decimal"/> number indicating the smallest permissible value.</param>
        /// <param name="max">
        /// An <see cref="Decimal"/> number indicating the greatest permissible value.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term><paramref name="min"/></term><description>
        /// <paramref name="a"/> is equal to or less than <paramref name="min"/>.</description>
        /// </item><item>
        /// <term><paramref name="a"/></term><description>
        /// <paramref name="a"/> is greater than <paramref name="min"/> and less than <paramref
        /// name="max"/>.</description>
        /// </item><item>
        /// <term><paramref name="max"/></term><description>
        /// <paramref name="a"/> is equal to or greater than <paramref name="max"/>.</description>
        /// </item></list></returns>

        public static decimal Restrict(this decimal a, decimal min, decimal max) {
            return (a < min ? min : (a > max ? max : a));
        }

        #endregion
        #region Restrict(Double, Double, Double)

        /// <summary>
        /// Restricts the specified <see cref="Double"/> number to the specified range.</summary>
        /// <param name="a">
        /// The <see cref="Double"/> number to restrict.</param>
        /// <param name="min">
        /// An <see cref="Double"/> number indicating the smallest permissible value.</param>
        /// <param name="max">
        /// An <see cref="Double"/> number indicating the greatest permissible value.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term><paramref name="min"/></term><description>
        /// <paramref name="a"/> is equal to or less than <paramref name="min"/>.</description>
        /// </item><item>
        /// <term><paramref name="a"/></term><description>
        /// <paramref name="a"/> is greater than <paramref name="min"/> and less than <paramref
        /// name="max"/>.</description>
        /// </item><item>
        /// <term><paramref name="max"/></term><description>
        /// <paramref name="a"/> is equal to or greater than <paramref name="max"/>.</description>
        /// </item></list></returns>

        public static double Restrict(this double a, double min, double max) {
            return (a < min ? min : (a > max ? max : a));
        }

        #endregion
        #region Restrict(Single, Single, Single)

        /// <summary>
        /// Restricts the specified <see cref="Single"/> number to the specified range.</summary>
        /// <param name="a">
        /// The <see cref="Single"/> number to restrict.</param>
        /// <param name="min">
        /// An <see cref="Single"/> number indicating the smallest permissible value.</param>
        /// <param name="max">
        /// An <see cref="Single"/> number indicating the greatest permissible value.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term><paramref name="min"/></term><description>
        /// <paramref name="a"/> is equal to or less than <paramref name="min"/>.</description>
        /// </item><item>
        /// <term><paramref name="a"/></term><description>
        /// <paramref name="a"/> is greater than <paramref name="min"/> and less than <paramref
        /// name="max"/>.</description>
        /// </item><item>
        /// <term><paramref name="max"/></term><description>
        /// <paramref name="a"/> is equal to or greater than <paramref name="max"/>.</description>
        /// </item></list></returns>

        public static float Restrict(this float a, float min, float max) {
            return (a < min ? min : (a > max ? max : a));
        }

        #endregion
        #region Restrict(Int16, Int16, Int16)

        /// <summary>
        /// Restricts the specified <see cref="Int16"/> number to the specified range.</summary>
        /// <param name="a">
        /// The <see cref="Int16"/> number to restrict.</param>
        /// <param name="min">
        /// An <see cref="Int16"/> number indicating the smallest permissible value.</param>
        /// <param name="max">
        /// An <see cref="Int16"/> number indicating the greatest permissible value.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term><paramref name="min"/></term><description>
        /// <paramref name="a"/> is equal to or less than <paramref name="min"/>.</description>
        /// </item><item>
        /// <term><paramref name="a"/></term><description>
        /// <paramref name="a"/> is greater than <paramref name="min"/> and less than <paramref
        /// name="max"/>.</description>
        /// </item><item>
        /// <term><paramref name="max"/></term><description>
        /// <paramref name="a"/> is equal to or greater than <paramref name="max"/>.</description>
        /// </item></list></returns>

        public static short Restrict(this short a, short min, short max) {
            return (a < min ? min : (a > max ? max : a));
        }

        #endregion
        #region Restrict(Int32, Int32, Int32)

        /// <summary>
        /// Restricts the specified <see cref="Int32"/> number to the specified range.</summary>
        /// <param name="a">
        /// The <see cref="Int32"/> number to restrict.</param>
        /// <param name="min">
        /// An <see cref="Int32"/> number indicating the smallest permissible value.</param>
        /// <param name="max">
        /// An <see cref="Int32"/> number indicating the greatest permissible value.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term><paramref name="min"/></term><description>
        /// <paramref name="a"/> is equal to or less than <paramref name="min"/>.</description>
        /// </item><item>
        /// <term><paramref name="a"/></term><description>
        /// <paramref name="a"/> is greater than <paramref name="min"/> and less than <paramref
        /// name="max"/>.</description>
        /// </item><item>
        /// <term><paramref name="max"/></term><description>
        /// <paramref name="a"/> is equal to or greater than <paramref name="max"/>.</description>
        /// </item></list></returns>

        public static int Restrict(this int a, int min, int max) {
            return (a < min ? min : (a > max ? max : a));
        }

        #endregion
        #region Restrict(Int64, Int64, Int64)

        /// <summary>
        /// Restricts the specified <see cref="Int64"/> number to the specified range.</summary>
        /// <param name="a">
        /// The <see cref="Int64"/> number to restrict.</param>
        /// <param name="min">
        /// An <see cref="Int64"/> number indicating the smallest permissible value.</param>
        /// <param name="max">
        /// An <see cref="Int64"/> number indicating the greatest permissible value.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term><paramref name="min"/></term><description>
        /// <paramref name="a"/> is equal to or less than <paramref name="min"/>.</description>
        /// </item><item>
        /// <term><paramref name="a"/></term><description>
        /// <paramref name="a"/> is greater than <paramref name="min"/> and less than <paramref
        /// name="max"/>.</description>
        /// </item><item>
        /// <term><paramref name="max"/></term><description>
        /// <paramref name="a"/> is equal to or greater than <paramref name="max"/>.</description>
        /// </item></list></returns>

        public static long Restrict(this long a, long min, long max) {
            return (a < min ? min : (a > max ? max : a));
        }

        #endregion
    }
}
