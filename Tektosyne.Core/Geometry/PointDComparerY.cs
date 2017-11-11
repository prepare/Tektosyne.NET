using System;
using System.Collections.Generic;
using System.Diagnostics;

using Tektosyne.Collections;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Provides methods that compare two <see cref="PointD"/> instances, preferring <see
    /// cref="PointD.Y"/> coordinates.</summary>
    /// <remarks><para>
    /// <b>PointDComparerY</b> defines a lexicographic ordering for <see cref="PointD"/> instances,
    /// sorting first by <see cref="PointD.Y"/> and then by <see cref="PointD.X"/> coordinates. Use
    /// <see cref="PointDComparerX"/> to sort first by <see cref="PointD.X"/> coordinates.
    /// </para><para>
    /// Coordinate comparisons may be performed precisely or with a specified epsilon. The actual
    /// comparisons are performed by two static methods, so you need to instantiate the
    /// <b>PointDComparerY</b> class only when required by a consumer.</para></remarks>

    [Serializable]
    public class PointDComparerY: IPointDComparer {
        #region Private Fields

        /// <summary>The epsilon used for coordinate comparisons.</summary>
        private double _epsilon;

        #endregion
        #region Epsilon

        /// <summary>
        /// Gets or sets the epsilon used for coordinate comparisons.</summary>
        /// <value><para>
        /// The maximum absolute difference at which coordinates should be considered equal.
        /// </para><para>-or-</para><para>
        /// Zero to use exact coordinate comparisons. The default is zero.</para></value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The property is set to a negative value.</exception>
        /// <remarks>
        /// <b>Epsilon</b> determines whether <see cref="Compare"/> dispatches to <see
        /// cref="CompareExact"/> or <see cref="CompareEpsilon"/>.</remarks>

        public double Epsilon {
            [DebuggerStepThrough]
            get { return _epsilon; }
            [DebuggerStepThrough]
            set {
                if (value < 0.0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(
                        "value", value, Strings.ArgumentNegative);

                _epsilon = value;
            }
        }

        #endregion
        #region CompareEpsilon(PointD, PointD)

        /// <overloads>
        /// Compares two specified <see cref="PointD"/> instances and returns an indication of their
        /// lexicographic ordering, given an epsilon for coordinate comparisons.</overloads>
        /// <summary>
        /// Compares two specified <see cref="PointD"/> instances and returns an indication of their
        /// lexicographic ordering, given the current <see cref="Epsilon"/> for coordinate
        /// comparisons.</summary>
        /// <param name="a">
        /// The first <see cref="PointD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="PointD"/> to compare.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description><paramref name="a"/> is sorted before <paramref name="b"/>, given the
        /// current <see cref="Epsilon"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description><paramref name="a"/> and <paramref name="b"/> are equal, given the current
        /// <see cref="Epsilon"/>.</description>
        /// </item><item>
        /// <term>Greater than zero</term>
        /// <description><paramref name="a"/> is sorted after <paramref name="b"/>, given the
        /// current <see cref="Epsilon"/>.</description>
        /// </item></list></returns>
        /// <remarks>
        /// <b>CompareEpsilon</b> is identical with <see cref="Compare"/> but always calls <see
        /// cref="MathUtility.Compare"/> with the current <see cref="Epsilon"/> for coordinate
        /// comparisons. This is slightly faster if <see cref="Epsilon"/> is known to be positive.
        /// </remarks>

        public int CompareEpsilon(PointD a, PointD b) {

            int result = MathUtility.Compare(a.Y, b.Y, _epsilon);
            if (result != 0) return result;
            return MathUtility.Compare(a.X, b.X, _epsilon);
        }

        #endregion
        #region CompareEpsilon(PointD, PointD, Double)

        /// <summary>
        /// Compares two specified <see cref="PointD"/> instances and returns an indication of their
        /// lexicographic ordering, given the specified epsilon for coordinate comparisons.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="PointD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="PointD"/> to compare.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which the coordinates of <paramref name="a"/> and
        /// <paramref name="b"/> should be considered equal.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description><paramref name="a"/> is sorted before <paramref name="b"/>, given the
        /// specified <paramref name="epsilon"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description><paramref name="a"/> and <paramref name="b"/> are equal, given the
        /// specified <paramref name="epsilon"/>.</description>
        /// </item><item>
        /// <term>Greater than zero</term>
        /// <description><paramref name="a"/> is sorted after <paramref name="b"/>, given the
        /// specified <paramref name="epsilon"/>.</description>
        /// </item></list></returns>
        /// <remarks><para>
        /// <b>CompareEpsilon</b> is identical with <see cref="CompareExact"/> but calls <see
        /// cref="MathUtility.Compare"/> with the specified <paramref name="epsilon"/> for
        /// coordinate comparisons.
        /// </para><para>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but
        /// <b>CompareEpsilon</b> does not check this condition.</para></remarks>

        public static int CompareEpsilon(PointD a, PointD b, double epsilon) {

            int result = MathUtility.Compare(a.Y, b.Y, epsilon);
            if (result != 0) return result;
            return MathUtility.Compare(a.X, b.X, epsilon);
        }

        #endregion
        #region CompareExact

        /// <summary>
        /// Compares two specified <see cref="PointD"/> instances and returns an indication of their
        /// lexicographic ordering, using exact coordinate comparisons.</summary>
        /// <param name="a">
        /// The first <see cref="PointD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="PointD"/> to compare.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description><paramref name="a"/> is sorted before <paramref name="b"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description><paramref name="a"/> and <paramref name="b"/> are equal.</description>
        /// </item><item>
        /// <term>Greater than zero</term>
        /// <description><paramref name="a"/> is sorted after <paramref name="b"/>.</description>
        /// </item></list></returns>
        /// <remarks>
        /// <b>CompareExact</b> determines the lexicographic ordering of <paramref name="a"/> and
        /// <paramref name="b"/> by first comparing their <see cref="PointD.Y"/> coordinates, and in
        /// case of equality then comparing their <see cref="PointD.X"/> coordinates. Smaller
        /// coordinates are sorted before greater coordinates.</remarks>

        public static int CompareExact(PointD a, PointD b) {

            if (a.Y < b.Y) return -1; if (a.Y > b.Y) return +1;
            if (a.X < b.X) return -1; if (a.X > b.X) return +1;
            return 0;
        }

        #endregion
        #region FindNearest

        /// <summary>
        /// Searches the specified sorted <see cref="PointD"/> collection for the element nearest to
        /// the specified coordinates, given the current <see cref="Epsilon"/> for coordinate
        /// comparisons.</summary>
        /// <param name="points">
        /// An <see cref="IList{T}"/> containing the <see cref="PointD"/> coordinates to search,
        /// sorted lexicographically using a <see cref="PointDComparerY"/> with the current <see
        /// cref="Epsilon"/>.</param>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to locate.</param>
        /// <returns>
        /// The zero-based index of any occurrence of <paramref name="q"/> in <paramref
        /// name="points"/>, if found; otherwise, the zero-based index of the <paramref
        /// name="points"/> element with the smallest Euclidean distance to <paramref name="q"/>.
        /// </returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="points"/> is a null reference or an empty collection.</exception>
        /// <remarks><para>
        /// <b>FindNearest</b> first approximates the index position of <paramref name="q"/> within
        /// the specified <paramref name="points"/> by a lexicographic binary search, using <see
        /// cref="PointDComparerY"/> methods with the current <see cref="Epsilon"/>.
        /// </para><para>
        /// <b>FindNearest</b> then expands the search to both increasing and decreasing index
        /// positions, using the Euclidean distance of the first approximation, or of any
        /// subsequently found nearer element, as the maximum search radius.
        /// </para><para>
        /// Once the vertical distances of the tested <paramref name="points"/> elements in both
        /// directions exceed the search radius, <b>FindNearest</b> returns the zero-based index of
        /// the element with the smallest Euclidean distance to <paramref name="q"/>.
        /// </para><para>
        /// <b>FindNearest</b> has a worst-case runtime of O(ld n + n), where n is the total number
        /// of <paramref name="points"/>. However, the runtime for an evenly distributed point set
        /// is close to O(ld n) since <b>FindNearest</b> can limit comparisons to a relatively
        /// narrow vertical distance around the initial approximation.</para></remarks>

        public int FindNearest(IList<PointD> points, PointD q) {
            if (points == null || points.Count == 0)
                ThrowHelper.ThrowArgumentNullOrEmptyException("points");

            int last = points.Count - 1;
            if (last == 0) return 0;

            /*
             * We can derive the initial approximation either from a lexicographic binary search
             * or from a simple comparison of the query y-coordinate to the total vertical range.
             * In benchmarks, both work nearly equally well, but binary search has a slight edge.
             * Apparently the closer approximation more than compensates for the additional work.
             */

#if POINTDCOMPARER_RANGE
            // determine range of y-coordinates
            double y0 = points[0].Y, y1 = points[last].Y;
            Debug.Assert(y1 >= y0);

            // approximate index of query y-coordinate
            int index;
            if (q.Y <= y0)
                index = 0;
            else if (q.Y >= y1)
                index = last;
            else {
                Debug.Assert(y0 < y1);
                index = (int) (points.Count * (q.Y - y0) / (y1 - y0));
                Debug.Assert(index >= 0 && index < points.Count);
            }
#else
            // use binary search for lexicographic approximation
            int index = (_epsilon == 0 ?
                points.BestBinarySearch(q, CompareExact) :
                points.BestBinarySearch(q, CompareEpsilon));

            /*
             * Return immediate binary search hit only if Epsilon is zero.
             * Otherwise, we still need to search for nearer points in the vicinity,
             * as we might have found a non-nearest point within Epsilon distance.
             */

            if (index < 0)
                index = Math.Min(~index, points.Count - 1);
            else if (_epsilon == 0)
                return index;
#endif

            // restrict search radius to first approximation
            PointD vector = points[index] - q;
            double minDistance = vector.LengthSquared;
            if (minDistance == 0) return index;

            int minIndex = index;
            double epsilon2 = 2 * _epsilon;

            // expand search in both directions until radius exceeded
            bool searchPlus = true, searchMinus = true;
            for (int search = 1; searchPlus || searchMinus; search++) {

                if (searchPlus) {
                    int i = index + search;
                    if (i >= points.Count)
                        searchPlus = false;
                    else {
                        // check if we exceeded search radius
                        vector = points[i] - q;
                        double y = Math.Abs(vector.Y) - epsilon2;
                        if (y * y - epsilon2 > minDistance)
                            searchPlus = false;
                        else {
                            // check if we found smaller distance
                            double distance = vector.LengthSquared;
                            if (minDistance > distance) {
                                if (distance == 0) return i;
                                minDistance = distance;
                                minIndex = i;
                            }
                        }
                    }
                }

                if (searchMinus) {
                    int i = index - search;
                    if (i < 0)
                        searchMinus = false;
                    else {
                        // check if we exceeded search radius
                        vector = points[i] - q;
                        double y = Math.Abs(vector.Y) - epsilon2;
                        if (y * y - epsilon2 > minDistance)
                            searchMinus = false;
                        else {
                            // check if we found smaller distance
                            double distance = vector.LengthSquared;
                            if (minDistance > distance) {
                                if (distance == 0) return i;
                                minDistance = distance;
                                minIndex = i;
                            }
                        }
                    }
                }
            }

            return minIndex;
        }

        #endregion
        #region IComparer Members

        /// <overloads>
        /// Compares two specified <see cref="PointD"/> instances and returns an indication of their
        /// lexicographic ordering.</overloads>
        /// <summary>
        /// Compares two specified objects, which must be <see cref="PointD"/> instances, and
        /// returns an indication of their lexicographic ordering.</summary>
        /// <param name="a">
        /// The first <see cref="Object"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="Object"/> to compare.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description><paramref name="a"/> is sorted before <paramref name="b"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description><paramref name="a"/> and <paramref name="b"/> are equal.</description>
        /// </item><item>
        /// <term>Greater than zero</term>
        /// <description><paramref name="a"/> is sorted after <paramref name="b"/>.</description>
        /// </item></list></returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="a"/> or <paramref name="b"/> is neither a <see cref="PointD"/> nor a
        /// null reference.</exception>
        /// <remarks><para>
        /// The specified <paramref name="a"/> and <paramref name="b"/> must both be either null
        /// references or <see cref="PointD"/> instances. Null references are always sorted before
        /// valid <see cref="PointD"/> instances. Two null references are considered equal.
        /// </para><para>
        /// <b>Compare</b> determines the relative order of the two instances by calling the
        /// strongly-typed <see cref="Compare(PointD, PointD)"/> overload.</para></remarks>

        public int Compare(object a, object b) {

            if (a == null) {
                if (b == null) return 0;
                if (!(b is PointD))
                    ThrowHelper.ThrowArgumentExceptionWithFormat(
                        "b", Strings.ArgumentTypeMismatch, "PointD");

                return -1;
            }

            if (!(a is PointD))
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "a", Strings.ArgumentTypeMismatch, "PointD");

            if (b == null) return 1;
            if (!(b is PointD))
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "b", Strings.ArgumentTypeMismatch, "PointD");

            return Compare((PointD) a, (PointD) b);
        }

        #endregion
        #region IComparer<PointD> Members

        /// <summary>
        /// Compares two specified <see cref="PointD"/> instances and returns an indication of their
        /// lexicographic ordering.</summary>
        /// <param name="a">
        /// The first <see cref="PointD"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="PointD"/> to compare.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description><paramref name="a"/> is sorted before <paramref name="b"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description><paramref name="a"/> and <paramref name="b"/> are equal.</description>
        /// </item><item>
        /// <term>Greater than zero</term>
        /// <description><paramref name="a"/> is sorted after <paramref name="b"/>.</description>
        /// </item></list></returns>
        /// <remarks>
        /// <b>Compare</b> returns the result of either <see cref="CompareExact"/> or <see
        /// cref="CompareEpsilon"/> for <paramref name="a"/> and <paramref name="b"/>, depending on
        /// whether the current <see cref="Epsilon"/> equals zero.</remarks>

        public int Compare(PointD a, PointD b) {
            return (_epsilon == 0 ? CompareExact(a, b) : CompareEpsilon(a, b, _epsilon));
        }

        #endregion
    }
}
