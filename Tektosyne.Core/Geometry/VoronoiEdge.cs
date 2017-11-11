using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents one edge in the Voronoi diagram found by the <see cref="Voronoi"/> algorithm.
    /// </summary>
    /// <remarks><para>
    /// <b>VoronoiEdge</b> is an immutable structure that represents one element in the <see
    /// cref="VoronoiResults.VoronoiEdges"/> collection of the <see cref="VoronoiResults"/> class.
    /// </para><para>
    /// <b>VoronoiEdge</b> contains not only the indices of the two <see
    /// cref="VoronoiResults.VoronoiVertices"/> that terminate the edge but also the indices of the
    /// two <see cref="VoronoiResults.GeneratorSites"/> that are bisected by the edge. This data is
    /// required to construct the Voronoi region corresponding to each generator site.
    /// </para></remarks>

    [Serializable, StructLayout(LayoutKind.Auto)]
    public struct VoronoiEdge: IEquatable<VoronoiEdge> {
        #region VoronoiEdge(Int32, Int32, Int32, Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="VoronoiEdge"/> structure with the specified
        /// generator site and Voronoi vertex indices.</summary>
        /// <param name="site1">
        /// The index of the first of the two <see cref="VoronoiResults.GeneratorSites"/> that are
        /// bisected by the <see cref="VoronoiEdge"/>.</param>
        /// <param name="site2">
        /// The index of the second of the two <see cref="VoronoiResults.GeneratorSites"/> that are
        /// bisected by the <see cref="VoronoiEdge"/>.</param>
        /// <param name="vertex1">
        /// The index of the first of the two <see cref="VoronoiResults.VoronoiVertices"/> that are
        /// connected by the <see cref="VoronoiEdge"/>.</param>
        /// <param name="vertex2">
        /// The index of the second of the two <see cref="VoronoiResults.VoronoiVertices"/> that are
        /// connected by the <see cref="VoronoiEdge"/>.</param>

        internal VoronoiEdge(int site1, int site2, int vertex1, int vertex2) {
            Debug.Assert(vertex1 >= 0 && vertex2 >= 0);

            Site1 = site1;
            Site2 = site2;
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }

        #endregion
        #region Site1

        /// <summary>
        /// The index of the first of the two <see cref="VoronoiResults.GeneratorSites"/> that are
        /// bisected by the <see cref="VoronoiEdge"/>.</summary>

        public readonly int Site1;

        #endregion
        #region Site2

        /// <summary>
        /// The index of the second of the two <see cref="VoronoiResults.GeneratorSites"/> that are
        /// bisected by the <see cref="VoronoiEdge"/>.</summary>

        public readonly int Site2;

        #endregion
        #region Vertex1

        /// <summary>
        /// The index of the first of the two <see cref="VoronoiResults.VoronoiVertices"/> that are
        /// connected by the <see cref="VoronoiEdge"/>.</summary>

        public readonly int Vertex1;

        #endregion
        #region Vertex2

        /// <summary>
        /// The index of the second of the two <see cref="VoronoiResults.VoronoiVertices"/> that are
        /// connected by the <see cref="VoronoiEdge"/>.</summary>

        public readonly int Vertex2;

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="VoronoiEdge"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the values of the <see cref="Site1"/>, <see cref="Site2"/>,
        /// <see cref="Vertex1"/>, and <see cref="Vertex2"/> properties.</remarks>

        public override int GetHashCode() {
            unchecked { return Site1 ^ Site2 ^ Vertex1 ^ Vertex2; }
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="VoronoiEdge"/>.</summary>
        /// <returns>
        /// A <see cref="String"/> containing the values of the <see cref="Site1"/>, <see
        /// cref="Site2"/>, <see cref="Vertex1"/>, and <see cref="Vertex2"/> properties.</returns>

        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture,
                "{{Site1={0}, Site2={1}, Vertex1={2}, Vertex2={3}}}",
                Site1, Site2, Vertex1, Vertex2);
        }

        #endregion
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="VoronoiEdge"/> instances have the same value.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="VoronoiEdge"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="VoronoiEdge"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(VoronoiEdge)"/> method to test the two <see
        /// cref="VoronoiEdge"/> instances for value equality.</remarks>

        public static bool operator ==(VoronoiEdge x, VoronoiEdge y) {
            return x.Equals(y);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="VoronoiEdge"/> instances have different values.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="VoronoiEdge"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="VoronoiEdge"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is different from the value of
        /// <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(VoronoiEdge)"/> method to test the two <see
        /// cref="VoronoiEdge"/> instances for value inequality.</remarks>

        public static bool operator !=(VoronoiEdge x, VoronoiEdge y) {
            return !x.Equals(y);
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="VoronoiEdge"/> instances have the same value.
        /// </overloads>
        /// <summary>
        /// Determines whether this <see cref="VoronoiEdge"/> instance and a specified object, which
        /// must be a <see cref="VoronoiEdge"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="VoronoiEdge"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="VoronoiEdge"/> instance and
        /// its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="VoronoiEdge"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(VoronoiEdge)"/> overload to
        /// test the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is VoronoiEdge))
                return false;

            return Equals((VoronoiEdge) obj);
        }

        #endregion
        #region Equals(VoronoiEdge)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="VoronoiEdge"/> have the same
        /// value.</summary>
        /// <param name="edge">
        /// A <see cref="VoronoiEdge"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="edge"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="Site1"/>, <see cref="Site2"/>, <see
        /// cref="Vertex1"/>, and <see cref="Vertex2"/> properties of the two <see
        /// cref="VoronoiEdge"/> instances to test for value equality.</remarks>

        public bool Equals(VoronoiEdge edge) {
            return (Site1 == edge.Site1 && Site2 == edge.Site2
                && Vertex1 == edge.Vertex1 && Vertex2 == edge.Vertex2);
        }

        #endregion
        #region Equals(VoronoiEdge, VoronoiEdge)

        /// <summary>
        /// Determines whether two specified <see cref="VoronoiEdge"/> instances have the same
        /// value. </summary>
        /// <param name="x">
        /// The first <see cref="VoronoiEdge"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="VoronoiEdge"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(VoronoiEdge)"/> overload to test
        /// the two <see cref="VoronoiEdge"/> instances for value equality.</remarks>

        public static bool Equals(VoronoiEdge x, VoronoiEdge y) {
            return x.Equals(y);
        }

        #endregion
        #endregion
    }
}
