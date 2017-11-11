using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Tektosyne.Collections;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Contains the results of the <see cref="MultiLineIntersection"/> algorithms.</summary>
    /// <remarks><para>
    /// <b>MultiLinePoint</b> is an immutable structure that represents one intersection point found
    /// by the <see cref="MultiLineIntersection"/> algorithms. The complete results are stored as a
    /// collection of zero or more <b>MultiLinePoint</b> instances.
    /// </para><para>
    /// <b>MultiLinePoint</b> holds the input indices of all intersecting <see
    /// cref="MultiLinePoint.Lines"/>, as well as their <see cref="MultiLinePoint.Locations"/>
    /// relative to the <see cref="MultiLinePoint.Shared"/> coordinates.</para></remarks>

    [Serializable, StructLayout(LayoutKind.Auto)]
    public struct MultiLinePoint: IEquatable<MultiLinePoint> {
        #region MultiLinePoint(PointD, Int32[], LineLocation[])

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLinePoint"/> structure with the
        /// specified shared coordinates, intersecting lines, and relative locations.</summary>
        /// <param name="shared">
        /// The <see cref="PointD"/> coordinates shared by all <paramref name="lines"/>.</param>
        /// <param name="lines">
        /// The input indices of all line segments that intersect at the <paramref name="shared"/>
        /// coordinates.</param>
        /// <param name="locations">
        /// The locations of the <paramref name="shared"/> coordinates relative to the <paramref
        /// name="lines"/> element with the same index.</param>
        /// <remarks>
        /// The specified <paramref name="lines"/> and <paramref name="locations"/> must contain the
        /// same number of elements, and that number must be greater than zero.</remarks>

        internal MultiLinePoint(PointD shared, int[] lines, LineLocation[] locations) {
            Debug.Assert(lines.Length > 0);
            Debug.Assert(lines.Length == locations.Length);

            Shared = shared;
            Lines = lines;
            Locations = locations;
        }

        #endregion
        #region Lines

        /// <summary>
        /// The input indices of all line segments that intersect at the <see cref="Shared"/>
        /// coordinates.</summary>
        /// <remarks><para>
        /// <b>Lines</b> is an <see cref="Array"/> of zero-based indices, relative to the original
        /// input collection, which indicates the line segments that intersect at the <see
        /// cref="Shared"/> coordinates. The default is a null reference.
        /// </para><para>
        /// For any <see cref="MultiLinePoint"/> created by a <see cref="MultiLineIntersection"/>
        /// algorithm, <b>Lines</b> is always a valid <see cref="Array"/> with at least one element,
        /// and the same actual number of elements as <see cref="Locations"/>.</para></remarks>

        public readonly int[] Lines;

        #endregion
        #region Locations

        /// <summary>
        /// The locations of the <see cref="Shared"/> coordinates relative to the <see
        /// cref="Lines"/> element with the same index.</summary>
        /// <remarks><para>
        /// <b>Locations</b> is an <see cref="Array"/> of <see cref="LineLocation"/> values
        /// indicating the locations of the <see cref="Shared"/> coordinates relative to the <see
        /// cref="Lines"/> element with the same index. The default is a null reference.
        /// </para><para>
        /// For any <see cref="MultiLinePoint"/> created by a <see cref="MultiLineIntersection"/>
        /// algorithm, <b>Locations</b> is always a valid <see cref="Array"/> with at least one
        /// element, and the same actual number of elements as <see cref="Lines"/>.
        /// </para><para>
        /// Since all <see cref="Lines"/> are guaranteed to intersect at the <see cref="Shared"/>
        /// coordinates, each <b>Locations</b> element is either <see cref="LineLocation.Start"/>,
        /// <see cref="LineLocation.Between"/>, or <see cref="LineLocation.End"/>.</para></remarks>

        public readonly LineLocation[] Locations;

        #endregion
        #region Shared

        /// <summary>
        /// The <see cref="PointD"/> coordinates shared by all <see cref="Lines"/>.</summary>
        /// <remarks><para>
        /// <b>Shared</b> holds the coordinates where all <see cref="Lines"/> elements intersect.
        /// These coordinates are always computed (rather than copied) if all <see
        /// cref="Locations"/> values equal <see cref="LineLocation.Between"/>.
        /// </para><para>
        /// Otherwise, the <b>Shared</b> coordinates are either computed, or copied from the <see
        /// cref="LineD.Start"/> or <see cref="LineD.End"/> point of a <see cref="Lines"/> element
        /// whose corresponding <see cref="Locations"/> value equals <see
        /// cref="LineLocation.Start"/> or <see cref="LineLocation.End"/>, respectively.
        /// </para></remarks>

        public readonly PointD Shared;

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="MultiLinePoint"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> returns the result of <see cref="PointD.GetHashCode"/> for the <see
        /// cref="Shared"/> property.</remarks>

        public override unsafe int GetHashCode() {
            unchecked {
                double x = Shared.X, y = Shared.Y;
                long xi = *((long*) &x), yi = *((long*) &y);
                return (int) xi ^ (int) (xi >> 32) ^ (int) yi ^ (int) (yi >> 32);
            }
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="MultiLinePoint"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> containing the value of the <see cref="Shared"/> property, and
        /// the <see cref="Array.Length"/> of the <see cref="Lines"/> and <see cref="Locations"/>
        /// properties.</returns>

        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture,
                "{{Shared={0}, Lines.Length={1}, Locations.Length={2}}}", Shared,
                (Lines == null ? 0 : Lines.Length), (Locations == null ? 0: Locations.Length));
        }

        #endregion
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="MultiLinePoint"/> instances have the same value.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="MultiLinePoint"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="MultiLinePoint"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(MultiLinePoint)"/> method to test the two
        /// <see cref="MultiLinePoint"/> instances for value equality.</remarks>

        public static bool operator ==(MultiLinePoint x, MultiLinePoint y) {
            return x.Equals(y);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="MultiLinePoint"/> instances have different values.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="MultiLinePoint"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="MultiLinePoint"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is different from the value of
        /// <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(MultiLinePoint)"/> method to test the two
        /// <see cref="MultiLinePoint"/> instances for value inequality.</remarks>

        public static bool operator !=(MultiLinePoint x, MultiLinePoint y) {
            return !x.Equals(y);
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="MultiLinePoint"/> instances have the same value.
        /// </overloads>
        /// <summary>
        /// Determines whether this <see cref="MultiLinePoint"/> instance and a specified object,
        /// which must be a <see cref="MultiLinePoint"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="MultiLinePoint"/> instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="MultiLinePoint"/> instance
        /// and its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="MultiLinePoint"/>
        /// instance, <b>Equals</b> invokes the strongly-typed <see cref="Equals(MultiLinePoint)"/>
        /// overload to test the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is MultiLinePoint))
                return false;

            return Equals((MultiLinePoint) obj);
        }

        #endregion
        #region Equals(MultiLinePoint)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="MultiLinePoint"/> have the
        /// same value.</summary>
        /// <param name="other">
        /// A <see cref="MultiLinePoint"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="other"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="Shared"/>, <see cref="Lines"/>, and
        /// <see cref="Locations"/> properties of the two <see cref="MultiLinePoint"/> instances to
        /// test for value equality.</remarks>

        public bool Equals(MultiLinePoint other) {
            return (Shared == other.Shared &&
                CollectionsUtility.SequenceEqual(Lines, other.Lines) &&
                CollectionsUtility.SequenceEqual(Locations, other.Locations));
        }

        #endregion
        #region Equals(MultiLinePoint, MultiLinePoint)

        /// <summary>
        /// Determines whether two specified <see cref="MultiLinePoint"/> instances have the same
        /// value.</summary>
        /// <param name="x">
        /// The first <see cref="MultiLinePoint"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="MultiLinePoint"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(MultiLinePoint)"/> overload to
        /// test the two <see cref="MultiLinePoint"/> instances for value equality.</remarks>

        public static bool Equals(MultiLinePoint x, MultiLinePoint y) {
            return x.Equals(y);
        }

        #endregion
        #endregion
    }
}
