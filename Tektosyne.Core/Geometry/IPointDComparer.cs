using System;
using System.Collections;
using System.Collections.Generic;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Provides methods that compare two <see cref="PointD"/> instances, given a specified epsilon
    /// for lexicographic coordinate comparisons.</summary>
    /// <remarks><para>
    /// <b>IPointDComparer</b> defines an <see cref="IComparer{T}"/> that lexicographically compares
    /// <see cref="PointD"/> coordinates with a given <see cref="IPointDComparer.Epsilon"/>, applied
    /// in each dimension. <see cref="PointDComparerX"/> and <see cref="PointDComparerY"/> provide
    /// implementations whose lexicographic ordering prefers x- and y-coordinates, respectively.
    /// </para><para>
    /// <b>IPointDComparer</b> also provides a <see cref="IPointDComparer.FindNearest"/> method to
    /// search a pre-sorted <see cref="PointD"/> collection for given coordinates. This method is
    /// defined here because it relies on the specific sorting order established by the same
    /// <b>IPointDComparer</b> instance.</para></remarks>

    public interface IPointDComparer: IComparer<PointD>, IComparer {
        #region Epsilon

        /// <summary>
        /// Gets or sets the epsilon used for coordinate comparisons.</summary>
        /// <value><para>
        /// The maximum absolute difference at which coordinates should be considered equal.
        /// </para><para>-or-</para><para>
        /// Zero to use exact coordinate comparisons. The default is zero.</para></value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The property is set to a negative value.</exception>

        double Epsilon { get; set; }

        #endregion
        #region CompareEpsilon

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

        int CompareEpsilon(PointD a, PointD b);

        #endregion
        #region FindNearest

        /// <summary>
        /// Searches the specified sorted <see cref="PointD"/> collection for the element nearest to
        /// the specified coordinates, given the current <see cref="Epsilon"/> for coordinate
        /// comparisons.</summary>
        /// <param name="points">
        /// An <see cref="IList{T}"/> containing the <see cref="PointD"/> coordinates to search,
        /// sorted lexicographically using the current <see cref="IPointDComparer"/>.</param>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to locate.</param>
        /// <returns>
        /// The zero-based index of any occurrence of <paramref name="q"/> in <paramref
        /// name="points"/>, if found; otherwise, the zero-based index of the <paramref
        /// name="points"/> element with the smallest Euclidean distance to <paramref name="q"/>.
        /// </returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="points"/> is a null reference or an empty collection.</exception>
        /// <remarks>
        /// <b>FindNearest</b> combines a fast initial approximation with a radius search, depending
        /// on the concrete implementation of <see cref="IPointDComparer"/>. Please refer to <see
        /// cref="PointDComparerX"/> and <see cref="PointDComparerY"/> for details.</remarks>

        int FindNearest(IList<PointD> points, PointD q);

        #endregion
    }
}
