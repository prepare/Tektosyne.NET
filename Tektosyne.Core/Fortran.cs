using System;

namespace Tektosyne {

    /// <summary>
    /// Provides functions defined by the Fortran 90 standard.</summary>
    /// <remarks>
    /// <b>Fortran</b> supplements the anemic standard <see cref="System.Math"/> class with methods
    /// that mimic selected Fortran 90 functions. All methods provide several overloads for
    /// different numeric types to avoid type casting.</remarks>

    public static class Fortran {
        #region AInt(Decimal)

        /// <overloads>
        /// Returns the number nearest the specified value, rounded towards zero.</overloads>
        /// <summary>
        /// Returns the whole <see cref="Decimal"/> number nearest the specified value, rounded
        /// towards zero.</summary>
        /// <param name="n">
        /// A <see cref="Decimal"/> number to round.</param>
        /// <returns>
        /// The whole <see cref="Decimal"/> number nearest <paramref name="n"/> whose absolute value
        /// is less than or equal to <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>AInt</b> effectively removes all fractional digits from the specified number, as if
        /// it was cast to an integer type.</remarks>

        public static decimal AInt(decimal n) {
            return Math.Truncate(n);
        }

        #endregion
        #region AInt(Double)

        /// <summary>
        /// Returns the whole <see cref="Double"/> number nearest the specified value, rounded
        /// towards zero.</summary>
        /// <param name="n">
        /// A <see cref="Double"/> number to round.</param>
        /// <returns>
        /// The whole <see cref="Double"/> number nearest <paramref name="n"/> whose absolute value
        /// is less than or equal to <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>AInt</b> effectively removes all fractional digits from the specified number, as if
        /// it was cast to an integer type.</remarks>

        public static double AInt(double n) {
            return Math.Truncate(n);
        }

        #endregion
        #region AInt(Single)

        /// <summary>
        /// Returns the whole <see cref="Single"/> number nearest the specified value, rounded
        /// towards zero.</summary>
        /// <param name="n">
        /// A <see cref="Single"/> number to round.</param>
        /// <returns>
        /// The whole <see cref="Single"/> number nearest <paramref name="n"/> whose absolute value
        /// is less than or equal to <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>AInt</b> effectively removes all fractional digits from the specified number, as if
        /// it was cast to an integer type.</remarks>

        public static float AInt(float n) {
            // no float version of Truncate
            return (float) AInt((double) n);
        }

        #endregion
        #region ANInt(Decimal)

        /// <overloads>
        /// Returns the number nearest the specified value, using standard rounding.</overloads>
        /// <summary>
        /// Returns the whole <see cref="Decimal"/> number nearest the specified value, using
        /// standard rounding.</summary>
        /// <param name="n">
        /// A <see cref="Decimal"/> number to round.</param>
        /// <returns>
        /// The whole <see cref="Decimal"/> number nearest <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>ANInt</b> uses standard rounding, as specified by <see
        /// cref="MidpointRounding.AwayFromZero"/>.</remarks>

        public static decimal ANInt(decimal n) {
            return Math.Round(n, MidpointRounding.AwayFromZero);
        }

        #endregion
        #region ANInt(Double)

        /// <summary>
        /// Returns the whole <see cref="Double"/> number nearest the specified value, using
        /// standard rounding.</summary>
        /// <param name="n">
        /// A <see cref="Double"/> number to round.</param>
        /// <returns>
        /// The whole <see cref="Double"/> number nearest <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>ANInt</b> uses standard rounding, as specified by <see
        /// cref="MidpointRounding.AwayFromZero"/>.</remarks>

        public static double ANInt(double n) {
            return Math.Round(n, MidpointRounding.AwayFromZero);
        }

        #endregion
        #region ANInt(Single)

        /// <summary>
        /// Returns the whole <see cref="Single"/> number nearest the specified value, using
        /// standard rounding.</summary>
        /// <param name="n">
        /// A <see cref="Single"/> number to round.</param>
        /// <returns>
        /// The whole <see cref="Single"/> number nearest <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>ANInt</b> uses standard rounding, as specified by <see
        /// cref="MidpointRounding.AwayFromZero"/>.</remarks>

        public static float ANInt(float n) {
            // no float version of Round
            return (float) Math.Round((double) n, MidpointRounding.AwayFromZero);
        }

        #endregion
        #region ANInt(Decimal, Int32)

        /// <summary>
        /// Returns the <see cref="Decimal"/> number with the specified precision nearest the
        /// specified value, using standard rounding.</summary>
        /// <param name="n">
        /// A <see cref="Decimal"/> number to round.</param>
        /// <param name="digits">
        /// The number of significant fractional digits (precision) in the return value.</param>
        /// <returns>
        /// The <see cref="Decimal"/> number nearest <paramref name="n"/> with a precision equal to
        /// <paramref name="digits"/>.</returns>
        /// <remarks><para>
        /// The <paramref name="digits"/> parameter specifies the number of significant fractional
        /// digits in the return value. If <paramref name="digits"/> is zero, a whole number is
        /// returned.
        /// </para><para>
        /// <b>ANInt</b> uses standard rounding, as specified by <see
        /// cref="MidpointRounding.AwayFromZero"/>.</para></remarks>

        public static decimal ANInt(decimal n, int digits) {
            return Math.Round(n, digits, MidpointRounding.AwayFromZero);
        }

        #endregion
        #region ANInt(Double, Int32)

        /// <summary>
        /// Returns the <see cref="Double"/> number with the specified precision nearest the
        /// specified value, using standard rounding.</summary>
        /// <param name="n">
        /// A <see cref="Double"/> number to round.</param>
        /// <param name="digits">
        /// The number of significant fractional digits (precision) in the return value.</param>
        /// <returns>
        /// The <see cref="Double"/> number nearest <paramref name="n"/> with a precision equal to
        /// <paramref name="digits"/>.</returns>
        /// <remarks><para>
        /// The <paramref name="digits"/> parameter specifies the number of significant fractional
        /// digits in the return value. If <paramref name="digits"/> is zero, a whole number is
        /// returned.
        /// </para><para>
        /// <b>ANInt</b> uses standard rounding, as specified by <see
        /// cref="MidpointRounding.AwayFromZero"/>.</para></remarks>

        public static double ANInt(double n, int digits) {
            return Math.Round(n, digits, MidpointRounding.AwayFromZero);
        }

        #endregion
        #region ANInt(Single, Int32)

        /// <summary>
        /// Returns the <see cref="Single"/> number with the specified precision nearest the
        /// specified value, using standard rounding.</summary>
        /// <param name="n">
        /// A <see cref="Single"/> number to round.</param>
        /// <param name="digits">
        /// The number of significant fractional digits (precision) in the return value.</param>
        /// <returns>
        /// The <see cref="Single"/> number nearest <paramref name="n"/> with a precision equal to
        /// <paramref name="digits"/>.</returns>
        /// <remarks><para>
        /// The <paramref name="digits"/> parameter specifies the number of significant fractional
        /// digits in the return value. If <paramref name="digits"/> is zero, a whole number is
        /// returned.
        /// </para><para>
        /// <b>ANInt</b> uses standard rounding, as specified by <see
        /// cref="MidpointRounding.AwayFromZero"/>.</para></remarks>

        public static float ANInt(float n, int digits) {
            // no float version of Round
            return (float) Math.Round((double) n, digits, MidpointRounding.AwayFromZero);
        }

        #endregion
        #region Ceiling(Decimal)

        /// <overloads>
        /// Returns the <see cref="Int32"/> number nearest the specified value, rounded towards
        /// positive infinity.</overloads>
        /// <summary>
        /// Returns the <see cref="Int32"/> number nearest the specified <see cref="Decimal"/>
        /// value, rounded towards positive infinity.</summary>
        /// <param name="n">
        /// A <see cref="Decimal"/> number to round.</param>
        /// <returns>
        /// The <see cref="Int32"/> number nearest <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>Ceiling</b> is identical to <see cref="Decimal.Ceiling"/>, except that it returns an
        /// <see cref="Int32"/> number.</remarks>

        public static int Ceiling(decimal n) {
            return (int) Decimal.Ceiling(n);
        }

        #endregion
        #region Ceiling(Double)

        /// <summary>
        /// Returns the <see cref="Int32"/> number nearest the specified <see cref="Double"/> value,
        /// rounded towards positive infinity.</summary>
        /// <param name="n">
        /// A <see cref="Double"/> number to round.</param>
        /// <returns>
        /// The <see cref="Int32"/> number nearest <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>Ceiling</b> is identical to <see cref="Math.Ceiling"/>, except that it returns an
        /// <see cref="Int32"/> number.</remarks>

        public static int Ceiling(double n) {
            return (int) Math.Ceiling(n);
        }

        #endregion
        #region Ceiling(Single)

        /// <summary>
        /// Returns the <see cref="Int32"/> number nearest the specified <see cref="Single"/> value,
        /// rounded towards positive infinity.</summary>
        /// <param name="n">
        /// A <see cref="Single"/> number to round.</param>
        /// <returns>
        /// The <see cref="Int32"/> number nearest <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>Ceiling</b> is identical to <see cref="Math.Ceiling"/>, except that it returns an
        /// <see cref="Int32"/> number.</remarks>

        public static int Ceiling(float n) {
            // no float version of Ceiling
            return (int) Math.Ceiling((double) n);
        }

        #endregion
        #region Floor(Decimal)

        /// <overloads>
        /// Returns the <see cref="Int32"/> number nearest the specified value, rounded towards
        /// negative infinity.</overloads>
        /// <summary>
        /// Returns the <see cref="Int32"/> number nearest the specified <see cref="Decimal"/>
        /// value, rounded towards negative infinity.</summary>
        /// <param name="n">
        /// A <see cref="Decimal"/> number to round.</param>
        /// <returns>
        /// The <see cref="Int32"/> number nearest <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>Floor</b> is identical to <see cref="Math.Floor"/>, except that it returns an <see
        /// cref="Int32"/> number.</remarks>

        public static int Floor(decimal n) {
            return (int) Math.Floor(n);
        }

        #endregion
        #region Floor(Double)

        /// <summary>
        /// Returns the <see cref="Int32"/> number nearest the specified <see cref="Double"/> value,
        /// rounded towards negative infinity.</summary>
        /// <param name="n">
        /// A <see cref="Double"/> number to round.</param>
        /// <returns>
        /// The <see cref="Int32"/> number nearest <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>Floor</b> is identical to <see cref="Math.Floor"/>, except that it returns an <see
        /// cref="Int32"/> number.</remarks>

        public static int Floor(double n) {
            return (int) Math.Floor(n);
        }

        #endregion
        #region Floor(Single)

        /// <summary>
        /// Returns the <see cref="Int32"/> number nearest the specified <see cref="Single"/> value,
        /// rounded towards negative infinity.</summary>
        /// <param name="n">
        /// A <see cref="Single"/> number to round.</param>
        /// <returns>
        /// The <see cref="Int32"/> number nearest <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>Floor</b> is identical to <see cref="Math.Floor"/>, except that it returns an <see
        /// cref="Int32"/> number.</remarks>

        public static int Floor(float n) {
            // no float version of Floor
            return (int) Math.Floor((double) n);
        }

        #endregion
        #region Max(Decimal[])

        /// <overloads>
        /// Returns the largest of the specified <see cref="Array"/> of numbers.</overloads>
        /// <summary>
        /// Returns the largest of the specified <see cref="Array"/> of <see cref="Decimal"/>
        /// numbers.</summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Decimal"/> numbers to compare with each other.
        /// </param>
        /// <returns>
        /// The largest <see cref="Decimal"/> number found in <paramref name="array"/>.</returns>
        /// <remarks>
        /// <b>Max</b> returns <see cref="Decimal.MinValue"/> if <paramref name="array"/> is empty.
        /// </remarks>

        public static decimal Max(params decimal[] array) {
            decimal max = Decimal.MinValue;
            foreach (decimal n in array)
                if (n > max) max = n;
            return max;
        }

        #endregion
        #region Max(Double[])

        /// <summary>
        /// Returns the largest of the specified <see cref="Array"/> of <see cref="Double"/>
        /// numbers.</summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Double"/> numbers to compare with each other.
        /// </param>
        /// <returns>
        /// The largest <see cref="Double"/> number found in <paramref name="array"/>.</returns>
        /// <remarks>
        /// <b>Max</b> returns <see cref="Double.MinValue"/> if <paramref name="array"/> is empty.
        /// </remarks>

        public static double Max(params double[] array) {
            double max = Double.MinValue;
            foreach (double n in array)
                if (n > max) max = n;
            return max;
        }

        #endregion
        #region Max(Single[])

        /// <summary>
        /// Returns the largest of the specified <see cref="Array"/> of <see cref="Single"/>
        /// numbers.</summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Single"/> numbers to compare with each other.
        /// </param>
        /// <returns>
        /// The largest <see cref="Single"/> number found in <paramref name="array"/>.</returns>
        /// <remarks>
        /// <b>Max</b> returns <see cref="Single.MinValue"/> if <paramref name="array"/> is empty.
        /// </remarks>

        public static float Max(params float[] array) {
            float max = Single.MinValue;
            foreach (float n in array)
                if (n > max) max = n;
            return max;
        }

        #endregion
        #region Max(Int16[])

        /// <summary>
        /// Returns the largest of the specified <see cref="Array"/> of <see cref="Int16"/> numbers.
        /// </summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Int16"/> numbers to compare with each other.
        /// </param>
        /// <returns>
        /// The largest <see cref="Int16"/> number found in <paramref name="array"/>.</returns>
        /// <remarks>
        /// <b>Max</b> returns <see cref="Int16.MinValue"/> if <paramref name="array"/> is empty.
        /// </remarks>

        public static short Max(params short[] array) {
            short max = Int16.MinValue;
            foreach (short n in array)
                if (n > max) max = n;
            return max;
        }

        #endregion
        #region Max(Int32[])

        /// <summary>
        /// Returns the largest of the specified <see cref="Array"/> of <see cref="Int32"/> numbers.
        /// </summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Int32"/> numbers to compare with each other.
        /// </param>
        /// <returns>
        /// The largest <see cref="Int32"/> number found in <paramref name="array"/>.</returns>
        /// <remarks>
        /// <b>Max</b> returns <see cref="Int32.MinValue"/> if <paramref name="array"/> is empty.
        /// </remarks>

        public static int Max(params int[] array) {
            int max = Int32.MinValue;
            foreach (int n in array)
                if (n > max) max = n;
            return max;
        }

        #endregion
        #region Max(Int64[])

        /// <summary>
        /// Returns the largest of the specified <see cref="Array"/> of <see cref="Int64"/> numbers.
        /// </summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Int64"/> numbers to compare with each other.
        /// </param>
        /// <returns>
        /// The largest <see cref="Int64"/> number found in <paramref name="array"/>.</returns>
        /// <remarks>
        /// <b>Max</b> returns <see cref="Int64.MinValue"/> if <paramref name="array"/> is empty.
        /// </remarks>

        public static long Max(params long[] array) {
            long max = Int64.MinValue;
            foreach (long n in array)
                if (n > max) max = n;
            return max;
        }

        #endregion
        #region Min(Decimal[])

        /// <overloads>
        /// Returns the smallest of the specified <see cref="Array"/> of numbers.</overloads>
        /// <summary>
        /// Returns the smallest of the specified <see cref="Array"/> of <see cref="Decimal"/>
        /// numbers.</summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Decimal"/> numbers to compare with each other.
        /// </param>
        /// <returns>
        /// The smallest <see cref="Decimal"/> number found in <paramref name="array"/>.</returns>
        /// <remarks>
        /// <b>Min</b> returns <see cref="Decimal.MaxValue"/> if <paramref name="array"/> is empty.
        /// </remarks>

        public static decimal Min(params decimal[] array) {
            decimal min = Decimal.MaxValue;
            foreach (decimal n in array)
                if (n < min) min = n;
            return min;
        }

        #endregion
        #region Min(Double[])

        /// <summary>
        /// Returns the smallest of the specified <see cref="Array"/> of <see cref="Double"/>
        /// numbers.</summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Double"/> numbers to compare with each other.
        /// </param>
        /// <returns>
        /// The smallest <see cref="Double"/> number found in <paramref name="array"/>.</returns>
        /// <remarks>
        /// <b>Min</b> returns <see cref="Double.MaxValue"/> if <paramref name="array"/> is empty.
        /// </remarks>

        public static double Min(params double[] array) {
            double min = Double.MaxValue;
            foreach (double n in array)
                if (n < min) min = n;
            return min;
        }

        #endregion
        #region Min(Single[])

        /// <summary>
        /// Returns the smallest of the specified <see cref="Array"/> of <see cref="Single"/>
        /// numbers.</summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Single"/> numbers to compare with each other.
        /// </param>
        /// <returns>
        /// The smallest <see cref="Single"/> number found in <paramref name="array"/>.</returns>
        /// <remarks>
        /// <b>Min</b> returns <see cref="Single.MaxValue"/> if <paramref name="array"/> is empty.
        /// </remarks>

        public static float Min(params float[] array) {
            float min = Single.MaxValue;
            foreach (float n in array)
                if (n < min) min = n;
            return min;
        }

        #endregion
        #region Min(Int16)

        /// <summary>
        /// Returns the smallest of the specified <see cref="Array"/> of <see cref="Int16"/>
        /// numbers.</summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Int16"/> numbers to compare with each other.
        /// </param>
        /// <returns>
        /// The smallest <see cref="Int16"/> number found in <paramref name="array"/>.</returns>
        /// <remarks>
        /// <b>Min</b> returns <see cref="Int16.MaxValue"/> if <paramref name="array"/> is empty.
        /// </remarks>

        public static short Min(params short[] array) {
            short min = Int16.MaxValue;
            foreach (short n in array)
                if (n < min) min = n;
            return min;
        }

        #endregion
        #region Min(Int32)

        /// <summary>
        /// Returns the smallest of the specified <see cref="Array"/> of <see cref="Int32"/>
        /// numbers.</summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Int32"/> numbers to compare with each other.
        /// </param>
        /// <returns>
        /// The smallest <see cref="Int32"/> number found in <paramref name="array"/>.</returns>
        /// <remarks>
        /// <b>Min</b> returns <see cref="Int32.MaxValue"/> if <paramref name="array"/> is empty.
        /// </remarks>

        public static int Min(params int[] array) {
            int min = Int32.MaxValue;
            foreach (int n in array)
                if (n < min) min = n;
            return min;
        }

        #endregion
        #region Min(Int64)

        /// <summary>
        /// Returns the smallest of the specified <see cref="Array"/> of <see cref="Int64"/>
        /// numbers.</summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Int64"/> numbers to compare with each other.
        /// </param>
        /// <returns>
        /// The smallest <see cref="Int64"/> number found in <paramref name="array"/>.</returns>
        /// <remarks>
        /// <b>Min</b> returns <see cref="Int64.MaxValue"/> if <paramref name="array"/> is empty.
        /// </remarks>

        public static long Min(params long[] array) {
            long min = Int64.MaxValue;
            foreach (long n in array)
                if (n < min) min = n;
            return min;
        }

        #endregion
        #region Modulo(Decimal, Decimal)

        /// <overloads>
        /// Returns the first value modulo the second value.</overloads>
        /// <summary>
        /// Returns the first <see cref="Decimal"/> value modulo the second value.</summary>
        /// <param name="a">
        /// A <see cref="Decimal"/> number indicating the dividend.</param>
        /// <param name="p">
        /// A <see cref="Decimal"/> number indicating the divisor.</param>
        /// <returns>
        /// The <see cref="Decimal"/> number that equals <paramref name="a"/> modulo <paramref
        /// name="p"/>.</returns>
        /// <exception cref="DivideByZeroException">
        /// <paramref name="p"/> is zero.</exception>
        /// <remarks>
        /// <b>Modulo</b> returns <c>a - Floor(a/p) * p</c> whereas operator% returns <c>a - (int)
        /// (a/p) * p</c> which is actually the remainder. The result of <b>Modulo</b> is always in
        /// the interval [0, <paramref name="p"/>), regardless of the signs of <paramref name="a"/>
        /// and <paramref name="p"/>.</remarks>
        /// <example><para>
        /// The following table compares the results of <b>Modulo</b> and operator% for a dividend
        /// of 12, a divisor of 5, and all possible combinations of signs.
        /// </para><list type="table"><listheader>
        /// <term>Quotient</term>
        /// <description>Modulo</description><description>operator%</description>
        /// </listheader><item>
        /// <term>12 / 5</term><description>2</description><description>2</description>
        /// </item><item>
        /// <term>-12 / 5</term><description>3</description><description>-2</description>
        /// </item><item>
        /// <term>12 / -5</term><description>-3</description><description>2</description>
        /// </item><item>
        /// <term>-12 / -5</term><description>-2</description><description>-2</description>
        /// </item></list></example>

        public static decimal Modulo(decimal a, decimal p) {
            return a - Floor(a / p) * p;
        }

        #endregion
        #region Modulo(Double, Double)

        /// <summary>
        /// Returns the first <see cref="Double"/> value modulo the second value.</summary>
        /// <param name="a">
        /// A <see cref="Double"/> number indicating the dividend.</param>
        /// <param name="p">
        /// A <see cref="Double"/> number indicating the divisor.</param>
        /// <returns>
        /// The <see cref="Double"/> number that equals <paramref name="a"/> modulo <paramref
        /// name="p"/>.</returns>
        /// <exception cref="DivideByZeroException">
        /// <paramref name="p"/> is zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="Modulo(Decimal, Decimal)"/> for details.</remarks>

        public static double Modulo(double a, double p) {
            return a - Floor(a / p) * p;
        }

        #endregion
        #region Modulo(Single, Single)

        /// <summary>
        /// Returns the first <see cref="Single"/> value modulo the second value.</summary>
        /// <param name="a">
        /// A <see cref="Single"/> number indicating the dividend.</param>
        /// <param name="p">
        /// A <see cref="Single"/> number indicating the divisor.</param>
        /// <returns>
        /// The <see cref="Single"/> number that equals <paramref name="a"/> modulo <paramref
        /// name="p"/>.</returns>
        /// <exception cref="DivideByZeroException">
        /// <paramref name="p"/> is zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="Modulo(Decimal, Decimal)"/> for details.</remarks>

        public static float Modulo(float a, float p) {
            return a - Floor(a / p) * p;
        }

        #endregion
        #region Modulo(Int32, Int32)

        /// <summary>
        /// Returns the first <see cref="Int32"/> value modulo the second value.</summary>
        /// <param name="a">
        /// An <see cref="Int32"/> number indicating the dividend.</param>
        /// <param name="p">
        /// An <see cref="Int32"/> number indicating the divisor.</param>
        /// <returns>
        /// The <see cref="Int32"/> number that equals <paramref name="a"/> modulo <paramref
        /// name="p"/>.</returns>
        /// <exception cref="DivideByZeroException">
        /// <paramref name="p"/> is zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="Modulo(Decimal, Decimal)"/> for details.</remarks>

        public static int Modulo(int a, int p) {
            int quotient = a / p;
            int value = a - quotient * p;
            return (quotient < 0 ? value + p : value);
        }

        #endregion
        #region Modulo(Int64, Int64)

        /// <summary>
        /// Returns the first <see cref="Int64"/> value modulo the second value.</summary>
        /// <param name="a">
        /// An <see cref="Int64"/> number indicating the dividend.</param>
        /// <param name="p">
        /// An <see cref="Int64"/> number indicating the divisor.</param>
        /// <returns>
        /// The <see cref="Int64"/> number that equals <paramref name="a"/> modulo <paramref
        /// name="p"/>.</returns>
        /// <exception cref="DivideByZeroException">
        /// <paramref name="p"/> is zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="Modulo(Decimal, Decimal)"/> for details.</remarks>

        public static long Modulo(long a, long p) {
            long quotient = a / p;
            long value = a - quotient * p;
            return (quotient < 0 ? value + p : value);
        }

        #endregion
        #region NInt(Decimal)

        /// <overloads>
        /// Returns the <see cref="Int32"/> number nearest the specified value, using standard
        /// rounding.</overloads>
        /// <summary>
        /// Returns the <see cref="Int32"/> number nearest the specified <see cref="Decimal"/>
        /// value, using standard rounding.</summary>
        /// <param name="n">
        /// A <see cref="Decimal"/> number to round.</param>
        /// <returns>
        /// The <see cref="Int32"/> number nearest <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>NInt</b> is the equivalent of casting the result of <see cref="ANInt"/> to <see
        /// cref="Int32"/>.</remarks>

        public static int NInt(decimal n) {
            if (n > 0.0m)
                return (int) (n + 0.5m);
            else if (n < 0.0m)
                return (int) (n - 0.5m);
            else
                return 0;
        }

        #endregion
        #region NInt(Double)

        /// <summary>
        /// Returns the <see cref="Int32"/> number nearest the specified <see cref="Double"/> value,
        /// using standard rounding.</summary>
        /// <param name="n">
        /// A <see cref="Double"/> number to round.</param>
        /// <returns>
        /// The <see cref="Int32"/> number nearest <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>NInt</b> is the equivalent of casting the result of <see cref="ANInt"/> to <see
        /// cref="Int32"/>.</remarks>

        public static int NInt(double n) {
            if (n > 0.0)
                return (int) (n + 0.5);
            else if (n < 0.0)
                return (int) (n - 0.5);
            else
                return 0;
        }

        #endregion
        #region NInt(Single)

        /// <summary>
        /// Returns the <see cref="Int32"/> number nearest the specified <see cref="Single"/> value,
        /// using standard rounding.</summary>
        /// <param name="n">
        /// A <see cref="Single"/> number to round.</param>
        /// <returns>
        /// The <see cref="Int32"/> number nearest <paramref name="n"/>.</returns>
        /// <remarks>
        /// <b>NInt</b> is the equivalent of casting the result of <see cref="ANInt"/> to <see
        /// cref="Int32"/> but executes faster.</remarks>

        public static int NInt(float n) {
            if (n > 0.0f)
                return (int) (n + 0.5f);
            else if (n < 0.0f)
                return (int) (n - 0.5f);
            else
                return 0;
        }

        #endregion
        #region Sign(Decimal, Decimal)

        /// <overloads>
        /// Transfers the sign of one number to the absolute value of another.</overloads>
        /// <summary>
        /// Transfers the sign of one <see cref="Decimal"/> number to the absolute value of
        /// another.</summary>
        /// <param name="a">
        /// A <see cref="Decimal"/> number whose absolute value to combine with the sign of
        /// <paramref name="b"/>.</param>
        /// <param name="b">
        /// A <see cref="Decimal"/> number whose sign to combine with the absolute value of
        /// <paramref name="a"/>.</param>
        /// <returns>
        /// The <see cref="Decimal"/> number that equals the absolute value of <paramref name="a"/>
        /// with the sign of <paramref name="b"/>.</returns>
        /// <remarks>
        /// <b>Sign</b> assumes a positive sign if <paramref name="b"/> is zero, in accordance with
        /// the Fortran 90 standard.</remarks>

        public static decimal Sign(decimal a, decimal b) {
            if (b >= 0.0m)
                return (a >= 0.0m ? a : -a);
            else
                return (a >= 0.0m ? -a : a);
        }

        #endregion
        #region Sign(Double, Double)

        /// <summary>
        /// Transfers the sign of one <see cref="Double"/> number to the absolute value of another.
        /// </summary>
        /// <param name="a">
        /// A <see cref="Double"/> number whose absolute value to combine with the sign of <paramref
        /// name="b"/>.</param>
        /// <param name="b">
        /// A <see cref="Double"/> number whose sign to combine with the absolute value of <paramref
        /// name="a"/>.</param>
        /// <returns>
        /// The <see cref="Double"/> number that equals the absolute value of <paramref name="a"/>
        /// with the sign of <paramref name="b"/>.</returns>
        /// <remarks>
        /// <b>Sign</b> assumes a positive sign if <paramref name="b"/> is zero, in accordance with
        /// the Fortran 90 standard.</remarks>

        public static double Sign(double a, double b) {
            if (b >= 0.0)
                return (a >= 0.0 ? a : -a);
            else
                return (a >= 0.0 ? -a : a);
        }

        #endregion
        #region Sign(Single, Single)

        /// <summary>
        /// Transfers the sign of one <see cref="Single"/> number to the absolute value of another.
        /// </summary>
        /// <param name="a">
        /// A <see cref="Single"/> number whose absolute value to combine with the sign of <paramref
        /// name="b"/>.</param>
        /// <param name="b">
        /// A <see cref="Single"/> number whose sign to combine with the absolute value of <paramref
        /// name="a"/>.</param>
        /// <returns>
        /// The <see cref="Single"/> number that equals the absolute value of <paramref name="a"/>
        /// with the sign of <paramref name="b"/>.</returns>
        /// <remarks>
        /// <b>Sign</b> assumes a positive sign if <paramref name="b"/> is zero, in accordance with
        /// the Fortran 90 standard.</remarks>

        public static float Sign(float a, float b) {
            if (b >= 0.0f)
                return (a >= 0.0f ? a : -a);
            else
                return (a >= 0.0f ? -a : a);
        }

        #endregion
        #region Sum(Decimal[])

        /// <overloads>
        /// Returns the sum of the specified <see cref="Array"/> of numbers.</overloads>
        /// <summary>
        /// Returns the sum of the specified <see cref="Array"/> of <see cref="Decimal"/> numbers.
        /// </summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Decimal"/> numbers to sum up.</param>
        /// <returns>
        /// The <see cref="Decimal"/> number that is the sum of all values in <paramref
        /// name="array"/>.</returns>
        /// <remarks>
        /// <b>Sum</b> returns zero if <paramref name="array"/> is empty.</remarks>

        public static decimal Sum(params decimal[] array) {
            decimal sum = 0.0m;
            foreach (decimal n in array) sum += n;
            return sum;
        }

        #endregion
        #region Sum(Double[])

        /// <summary>
        /// Returns the sum of the specified <see cref="Array"/> of <see cref="Double"/> numbers.
        /// </summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Double"/> numbers to sum up.</param>
        /// <returns>
        /// The <see cref="Double"/> number that is the sum of all values in <paramref
        /// name="array"/>.</returns>
        /// <remarks>
        /// <b>Sum</b> returns zero if <paramref name="array"/> is empty.</remarks>

        public static double Sum(params double[] array) {
            double sum = 0.0;
            foreach (double n in array) sum += n;
            return sum;
        }

        #endregion
        #region Sum(Single[])

        /// <summary>
        /// Returns the sum of the specified <see cref="Array"/> of <see cref="Single"/> numbers.
        /// </summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Single"/> numbers to sum up.</param>
        /// <returns>
        /// The <see cref="Single"/> number that is the sum of all values in <paramref
        /// name="array"/>.</returns>
        /// <remarks>
        /// <b>Sum</b> returns zero if <paramref name="array"/> is empty.</remarks>

        public static float Sum(params float[] array) {
            float sum = 0.0f;
            foreach (float n in array) sum += n;
            return sum;
        }

        #endregion
        #region Sum(Int16[])

        /// <summary>
        /// Returns the sum of the specified <see cref="Array"/> of <see cref="Int16"/> numbers.
        /// </summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Int16"/> numbers to sum up.</param>
        /// <returns>
        /// The <see cref="Int16"/> number that is the sum of all values in <paramref
        /// name="array"/>.</returns>
        /// <remarks>
        /// <b>Sum</b> returns zero if <paramref name="array"/> is empty.</remarks>

        public static short Sum(params short[] array) {
            short sum = 0;
            foreach (short n in array) sum += n;
            return sum;
        }

        #endregion
        #region Sum(Int32[])

        /// <summary>
        /// Returns the sum of the specified <see cref="Array"/> of <see cref="Int32"/> numbers.
        /// </summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Int32"/> numbers to sum up.</param>
        /// <returns>
        /// The <see cref="Int32"/> number that is the sum of all values in <paramref
        /// name="array"/>.</returns>
        /// <remarks>
        /// <b>Sum</b> returns zero if <paramref name="array"/> is empty.</remarks>

        public static int Sum(params int[] array) {
            int sum = 0;
            foreach (int n in array) sum += n;
            return sum;
        }

        #endregion
        #region Sum(Int64[])

        /// <summary>
        /// Returns the sum of the specified <see cref="Array"/> of <see cref="Int64"/> numbers.
        /// </summary>
        /// <param name="array">
        /// The <see cref="Array"/> of <see cref="Int64"/> numbers to sum up.</param>
        /// <returns>
        /// The <see cref="Int64"/> number that is the sum of all values in <paramref
        /// name="array"/>.</returns>
        /// <remarks>
        /// <b>Sum</b> returns zero if <paramref name="array"/> is empty.</remarks>

        public static long Sum(params long[] array) {
            long sum = 0;
            foreach (long n in array) sum += n;
            return sum;
        }

        #endregion
    }
}
