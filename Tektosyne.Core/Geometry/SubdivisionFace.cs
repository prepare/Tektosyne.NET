using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using Tektosyne.Collections;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a face in a planar <see cref="Subdivision"/>.</summary>
    /// <remarks><para>
    /// <b>SubdivisionFace</b> represents any polygonal region that is bounded by the edges of a
    /// <see cref="Subdivision"/>, whether on the inside, on the outside, or both. There is always
    /// exactly one face without an outer boundary, called the unbounded face, which comprises the
    /// entire plane outside of the <see cref="Subdivision"/>.
    /// </para><para>
    /// A <b>SubdivisionFace</b> stores one <see cref="SubdivisionEdge"/> for each of its outer and
    /// inner boundaries. The corresponding polygonal region can be reconstructed from the cycle of
    /// half-edges that begins with an incident <see cref="SubdivisionFace.OuterEdge"/> or <see
    /// cref="SubdivisionFace.InnerEdges"/> element. Use the <b>Cycle…</b> properties of these
    /// half-edges to obtain face boundaries and related data.</para></remarks>

    [Serializable]
    public sealed class SubdivisionFace: IEquatable<SubdivisionFace>, IKeyedValue<Int32> {
        #region SubdivisionFace(Int32)

        /// <overloads>
        /// Initializes a new instance of the <see cref="SubdivisionFace"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="SubdivisionFace"/> class with the specified
        /// containing <see cref="Subdivision"/> and unique key.</summary>
        /// <param name="owner">
        /// The <see cref="Subdivision"/> that contains the <see cref="SubdivisionFace"/>.</param>
        /// <param name="key">
        /// The unique key of the <see cref="SubdivisionFace"/> within its <paramref name="owner"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="owner"/> is a null reference.</exception>

        internal SubdivisionFace(Subdivision owner, int key) {
            if (owner == null)
                ThrowHelper.ThrowArgumentNullException("owner");

            Owner = owner;
            _key = key;
        }

        #endregion
        #region SubdivisionFace(Int32, SubdivisionEdge, IEnumerable<SubdivisionEdge>)

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdivisionFace"/> class with the specified
        /// containing <see cref="Subdivision"/>, unique key, and outer and inner boundaries.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="Subdivision"/> that contains the <see cref="SubdivisionFace"/>.</param>
        /// <param name="key">
        /// The unique key of the <see cref="SubdivisionFace"/> within its <paramref name="owner"/>.
        /// </param>
        /// <param name="outerEdge">
        /// A <see cref="SubdivisionEdge"/> on the outer boundary of the <see
        /// cref="SubdivisionFace"/>. This argument may be a null reference.</param>
        /// <param name="innerEdges">
        /// An <see cref="IEnumerable{T}"/> collection containing one <see cref="SubdivisionEdge"/>
        /// on each inner boundary of the <see cref="SubdivisionFace"/>. This argument may be a null
        /// reference.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="owner"/> is a null reference.</exception>
        /// <remarks>
        /// This constructor is intended for unit testing.</remarks>

        internal SubdivisionFace(Subdivision owner, int key,
            SubdivisionEdge outerEdge, IEnumerable<SubdivisionEdge> innerEdges): this(owner, key) {

            _outerEdge = outerEdge;
            if (innerEdges != null)
                _innerEdges = new ListEx<SubdivisionEdge>(innerEdges);
        }

        #endregion
        #region Internal Fields

        /// <summary>
        /// The unique key of the <see cref="SubdivisionFace"/>.</summary>

        internal int _key;

        /// <summary>
        /// A <see cref="SubdivisionEdge"/> on the outer boundary of the <see
        /// cref="SubdivisionFace"/>.</summary>

        internal SubdivisionEdge _outerEdge;

        /// <summary>
        /// A list containing one <see cref="SubdivisionEdge"/> on each inner boundary of the <see
        /// cref="SubdivisionFace"/>.</summary>
        /// <remarks>
        /// This field default to a null reference to save memory in the frequent case that the <see
        /// cref="SubdivisionFace"/> contains no inner boundaries.</remarks>

        internal ListEx<SubdivisionEdge> _innerEdges;

        #endregion
        #region Key

        /// <summary>
        /// Gets the unique key of the <see cref="SubdivisionFace"/>.</summary>
        /// <value>
        /// The unique key of the <see cref="SubdivisionFace"/> within its <see cref="Owner"/>.
        /// </value>
        /// <remarks><para>
        /// <b>Key</b> begins at zero for the first <see cref="SubdivisionFace"/> instance in a <see
        /// cref="Subdivision"/>, and is incremented by one whenever an additional instance is
        /// created. The <b>Key</b> values of all <see cref="SubdivisionFace"/> instances thus
        /// reflect the order in which they were created.
        /// </para><para>
        /// <b>Key</b> is usually immutable, unless <see cref="Subdivision.RenumberFaces"/> is
        /// called on the containing <see cref="Subdivision"/>.</para></remarks>

        public int Key {
            [DebuggerStepThrough]
            get { return _key; }
        }

        #endregion
        #region Owner

        /// <summary>
        /// The <see cref="Subdivision"/> that contains the <see cref="SubdivisionFace"/>.</summary>

        public readonly Subdivision Owner;

        #endregion
        #region AllCycleEdges

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> collection that contains all half-edges on all
        /// boundaries of the <see cref="SubdivisionFace"/>.</summary>
        /// <value>
        /// An <see cref="IEnumerable{T}"/> collection that contains each <see
        /// cref="SubdivisionEdge"/> on all boundaries of the <see cref="SubdivisionFace"/>.</value>
        /// <remarks>
        /// <b>AllCycleEdges</b> begins with the <see cref="OuterEdge"/>, if any, and follows the
        /// chain of <see cref="SubdivisionEdge.Next"/> pointers until the cycle is complete,
        /// yielding each encountered <see cref="SubdivisionEdge"/> in turn. <b>AllCycleEdges</b>
        /// then repeats this process for each <see cref="InnerEdges"/> element.</remarks>

        public IEnumerable<SubdivisionEdge> AllCycleEdges {
            get {
                // yield all outer cycle edges
                if (_outerEdge != null) {
                    SubdivisionEdge edge = _outerEdge;
                    do {
                        yield return edge;
                        edge = edge._next;
                    } while (edge != _outerEdge);
                }

                if (_innerEdges == null)
                    yield break;

                // yield all inner cycle edges
                for (int i = 0; i < _innerEdges.Count; i++) {
                    SubdivisionEdge innerEdge = _innerEdges[i];
                    SubdivisionEdge edge = innerEdge;
                    do {
                        yield return edge;
                        edge = edge._next;
                    } while (edge != innerEdge);
                }
            }
        }

        #endregion
        #region OuterEdge

        /// <summary>
        /// Gets a <see cref="SubdivisionEdge"/> on the outer boundary of the <see
        /// cref="SubdivisionFace"/>.</summary>
        /// <value><para>
        /// An inward-facing <see cref="SubdivisionEdge"/> on the outer boundary of the <see
        /// cref="SubdivisionFace"/>.
        /// </para><para>-or-</para><para>
        /// A null reference if the <see cref="SubdivisionFace"/> has no outer boundary. The default
        /// is a null reference.</para></value>

        public SubdivisionEdge OuterEdge {
            [DebuggerStepThrough]
            get { return _outerEdge; }
        }

        #endregion
        #region InnerEdges

        /// <summary>
        /// Gets a read-only list containing one <see cref="SubdivisionEdge"/> on each inner
        /// boundary of the <see cref="SubdivisionFace"/>.</summary>
        /// <value>
        /// A read-only <see cref="ListEx{T}"/> containing one outward-facing <see
        /// cref="SubdivisionEdge"/> on each disconnected inner boundary of the <see
        /// cref="SubdivisionFace"/>.</value>
        /// <remarks>
        /// <b>InnerEdges</b> returns an empty collection if the <see cref="SubdivisionFace"/>
        /// contains no inner boundaries, or "holes".</remarks>

        public ListEx<SubdivisionEdge> InnerEdges {
            [DebuggerStepThrough]
            get {
                return (_innerEdges == null ?
                    ListEx<SubdivisionEdge>.Empty : _innerEdges.AsReadOnly());
            }
        }

        #endregion
        #region FindNearestEdge

        /// <summary>
        /// Finds the half-edge bounding the <see cref="SubdivisionFace"/> that is nearest to and
        /// facing the specified coordinates.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to locate.</param>
        /// <param name="distance">
        /// Returns the distance between <paramref name="q"/> and the returned <see
        /// cref="SubdivisionEdge"/>, if any; otherwise, <see cref="Double.MaxValue"/>.</param>
        /// <returns><para>
        /// The <see cref="SubdivisionEdge"/> on any outer or inner boundaries of the <see
        /// cref="SubdivisionFace"/> with the smallest distance to and facing <paramref name="q"/>.
        /// </para><para>-or-</para><para>
        /// A null reference if the <see cref="SubdivisionFace"/> is completely unbounded.
        /// </para></returns>
        /// <remarks><para>
        /// <b>FindNearestEdge</b> traverses the <see cref="OuterEdge"/> boundary and all <see
        /// cref="InnerEdges"/> boundaries, computing the distance from <paramref name="q"/> to each
        /// <see cref="SubdivisionEdge"/>. This is an O(n) operation where n is the number of
        /// half-edges incident on the <see cref="SubdivisionFace"/>.
        /// </para><para>
        /// If <paramref name="q"/> is nearest to an edge that belongs to a zero-area protrusion
        /// into the <see cref="SubdivisionFace"/>, <b>FindNearestEdge</b> returns the twin
        /// half-edge that faces <paramref name="q"/>, according to its <see
        /// cref="SubdivisionEdge.Face"/> orientation.</para></remarks>

        public SubdivisionEdge FindNearestEdge(PointD q, out double distance) {
            distance = Double.MaxValue;
            SubdivisionEdge nearestEdge = null;

            // find smallest distance to any outer cycle edge
            if (_outerEdge != null) {
                SubdivisionEdge edge = _outerEdge;
                do {
                    double d = edge.ToLine().DistanceSquared(q);
                    if (distance > d) {
                        distance = d;
                        if (d == 0) return edge;
                        nearestEdge = edge;
                    }
                    edge = edge._next;
                } while (edge != _outerEdge);
            }

            // find smallest distance to any inner cycle edge
            if (_innerEdges != null)
                for (int i = 0; i < _innerEdges.Count; i++) {
                    SubdivisionEdge innerEdge = _innerEdges[i];
                    SubdivisionEdge edge = innerEdge;
                    do {
                        double d = edge.ToLine().DistanceSquared(q);
                        if (distance > d) {
                            distance = d;
                            if (d == 0) return edge;
                            nearestEdge = edge;
                        }
                        edge = edge._next;
                    } while (edge != innerEdge);
                }

            if (nearestEdge == null) return null;

            // check twin in case of zero-area protrusion
            if (nearestEdge._twin._face == this) {
                LineLocation location = nearestEdge.ToLine().Locate(q);
                if (location == LineLocation.Right)
                    nearestEdge = nearestEdge._twin;
            }

            distance = Math.Sqrt(distance);
            return nearestEdge;
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="SubdivisionFace"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> returns the value of the <see cref="Key"/> property, which is
        /// guaranteed to be unique within the containing <see cref="Subdivision"/>.</remarks>

        public override int GetHashCode() {
            return _key;
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="SubdivisionFace"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> containing the culture-invariant string representation of the
        /// <see cref="Key"/> property, followed by the <see cref="SubdivisionEdge.Key"/> values of
        /// the <see cref="OuterEdge"/> and <see cref="InnerEdges"/> properties. The value -1 is
        /// substituted for any properties or elements that are null references.</returns>

        public override string ToString() {
            Func<SubdivisionEdge, Int32> edgeKey = (e => e == null ? -1 : e._key);
            var innerIds = InnerEdges.Select(edgeKey);

            return String.Format(CultureInfo.InvariantCulture,
                "Face {0}: OuterEdge={1}, InnerEdges={2}",
                _key, edgeKey(_outerEdge), StringUtility.ValidateCollection(innerIds));
        }

        #endregion
        #region Internal Methods
        #region AddInnerEdge

        /// <summary>
        /// Adds the specified <see cref="SubdivisionEdge"/> to the <see cref="InnerEdges"/>
        /// collection.</summary>
        /// <param name="edge">
        /// The <see cref="SubdivisionEdge"/> to add.</param>
        /// <remarks>
        /// <b>AddInnerEdge</b> first creates a new collection that backs the <see
        /// cref="InnerEdges"/> property, if necessary.</remarks>

        internal void AddInnerEdge(SubdivisionEdge edge) {
            Debug.Assert(edge != null);

            if (_innerEdges == null)
                _innerEdges = new ListEx<SubdivisionEdge>();

            _innerEdges.Add(edge);
        }

        #endregion
        #region AddInnerEdges

        /// <summary>
        /// Adds the specified <see cref="SubdivisionEdge"/> collection to the <see
        /// cref="InnerEdges"/> collection.</summary>
        /// <param name="edges">
        /// A <see cref="ListEx{T}"/> containing the <see cref="SubdivisionEdge"/> elements to add.
        /// </param>
        /// <remarks>
        /// <b>AddInnerEdge</b> first creates a new collection that backs the <see
        /// cref="InnerEdges"/> property, if necessary.</remarks>

        internal void AddInnerEdges(ListEx<SubdivisionEdge> edges) {
            if (edges == null || edges.Count == 0)
                return;

            if (_innerEdges == null)
                _innerEdges = new ListEx<SubdivisionEdge>();

            _innerEdges.AddRange(edges);
        }

        #endregion
        #region MoveEdge(SubdivisionEdge)

        /// <overloads>
        /// Moves the incident half-edge on one of the boundaries of the <see
        /// cref="SubdivisionFace"/>.</overloads>
        /// <summary>
        /// Moves the incident half-edge on one of the boundaries of the <see
        /// cref="SubdivisionFace"/> away from the specified <see cref="SubdivisionEdge"/>.
        /// </summary>
        /// <param name="oldEdge">
        /// The incident <see cref="SubdivisionEdge"/> to replace with another half-edge on the same
        /// boundary of the <see cref="SubdivisionFace"/>.</param>
        /// <returns>
        /// A <see cref="MoveEdgeResult"/> value indicating which <see cref="SubdivisionFace"/>
        /// properties were changed, if any.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="oldEdge"/> is a null reference.</exception>
        /// <remarks><para>
        /// If <see cref="OuterEdge"/> equals the specified <paramref name="oldEdge"/> or its twin,
        /// <b>MoveEdge</b> sets <see cref="OuterEdge"/> to the result of <see
        /// cref="SubdivisionEdge.GetOtherCycleEdge"/> for the specified <paramref name="oldEdge"/>.
        /// </para><para>
        /// Otherwise, <b>MoveEdge</b> searches for an <see cref="InnerEdges"/> element that equals
        /// <paramref name="oldEdge"/> or its twin. On success, <b>MoveEdge</b> removes that element
        /// if the cycle contains no other half-edges; otherwise, <b>MoveEdge</b> sets that element
        /// to the result of <see cref="SubdivisionEdge.GetOtherCycleEdge"/> for <paramref
        /// name="oldEdge"/>.</para></remarks>

        internal MoveEdgeResult MoveEdge(SubdivisionEdge oldEdge) {
            SubdivisionEdge oldTwin = oldEdge._twin;

            if (_outerEdge == oldEdge || _outerEdge == oldTwin) {
                _outerEdge = _outerEdge.GetOtherCycleEdge(oldEdge);
                Debug.Assert(_outerEdge != null);
                return MoveEdgeResult.OuterChanged;
            }

            if (_innerEdges != null) {
                for (int i = 0; i < _innerEdges.Count; i++) {
                    SubdivisionEdge innerEdge = _innerEdges[i];
                    if (innerEdge == oldEdge || innerEdge == oldTwin) {

                        if (oldEdge._next == oldTwin && oldEdge._previous == oldTwin) {
                            _innerEdges.RemoveAt(i);
                            if (_innerEdges.Count == 0)
                                _innerEdges = null;
                            return MoveEdgeResult.InnerRemoved;
                        }

                        _innerEdges[i] = innerEdge.GetOtherCycleEdge(oldEdge);
                        Debug.Assert(_innerEdges[i] != null);
                        return MoveEdgeResult.InnerChanged;
                    }
                }
            }

            return MoveEdgeResult.Unchanged;
        }

        #endregion
        #region MoveEdge(SubdivisionEdge, SubdivisionEdge)

        /// <summary>
        /// Moves the incident half-edge on one of the boundaries of the <see
        /// cref="SubdivisionFace"/> from the specified <see cref="SubdivisionEdge"/> to another
        /// specified instance.</summary>
        /// <param name="oldEdge">
        /// The incident <see cref="SubdivisionEdge"/> to replace with <paramref name="newEdge"/>.
        /// </param>
        /// <param name="newEdge">
        /// The incident <see cref="SubdivisionEdge"/> that replaces <paramref name="oldEdge"/>.
        /// </param>
        /// <returns>
        /// A <see cref="MoveEdgeResult"/> value indicating which <see cref="SubdivisionFace"/>
        /// properties were changed, if any.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="oldEdge"/> or <paramref name="newEdge"/> is a null reference.
        /// </exception>
        /// <remarks><para>
        /// If <see cref="OuterEdge"/> equals the specified <paramref name="oldEdge"/>,
        /// <b>MoveEdge</b> sets that property to the specified <paramref name="newEdge"/>.
        /// Otherwise, <b>MoveEdge</b> sets the first <see cref="InnerEdges"/> element that equals
        /// <paramref name="oldEdge"/> to <paramref name="newEdge"/>, if any.
        /// </para><para>
        /// Unlike the other <see cref="MoveEdge(SubdivisionEdge)"/> overload, this overload does
        /// not check the <see cref="SubdivisionEdge.Twin"/> of <paramref name="oldEdge"/>, nor
        /// remove single-edge cycles.</para></remarks>

        internal MoveEdgeResult MoveEdge(SubdivisionEdge oldEdge, SubdivisionEdge newEdge) {

            if (_outerEdge == oldEdge) {
                _outerEdge = newEdge;
                return MoveEdgeResult.OuterChanged;
            }

            if (_innerEdges != null)
                for (int i = 0; i < _innerEdges.Count; i++)
                    if (_innerEdges[i] == oldEdge) {
                        _innerEdges[i] = newEdge;
                        return MoveEdgeResult.InnerChanged;
                    }

            return MoveEdgeResult.Unchanged;
        }

        #endregion
        #region SetAllEdgeFaces

        /// <summary>
        /// Sets the <see cref="SubdivisionEdge.Face"/> property of each <see
        /// cref="SubdivisionEdge"/> in the <see cref="OuterEdge"/> cycle and all <see
        /// cref="InnerEdges"/> cycles to the specified value.</summary>
        /// <param name="face">
        /// The new value for the <see cref="SubdivisionEdge.Face"/> property of each <see
        /// cref="SubdivisionEdge"/>.</param>

        internal void SetAllEdgeFaces(SubdivisionFace face) {
            Debug.Assert(face != null);

            if (_outerEdge != null)
                _outerEdge.SetAllFaces(face);

            if (_innerEdges != null)
                for (int i = 0; i < _innerEdges.Count; i++)
                    _innerEdges[i].SetAllFaces(face);
        }

        #endregion
        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="SubdivisionFace"/> instances have the same value.
        /// </overloads>
        /// <summary>
        /// Determines whether this <see cref="SubdivisionFace"/> instance and a specified object,
        /// which must be a <see cref="SubdivisionFace"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="SubdivisionFace"/> instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="SubdivisionFace"/> instance
        /// and its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="SubdivisionFace"/>
        /// instance, or an instance of a derived class, <b>Equals</b> invokes the strongly-typed
        /// <see cref="Equals(SubdivisionFace)"/> overload to test the two instances for value
        /// equality.</remarks>

        public override bool Equals(object obj) {
            return Equals(obj as SubdivisionFace);
        }

        #endregion
        #region Equals(SubdivisionFace)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="SubdivisionFace"/> have the
        /// same value.</summary>
        /// <param name="face">
        /// A <see cref="SubdivisionFace"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="face"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>Equals</b> compares the values all properties of the two <see
        /// cref="SubdivisionFace"/> instances to test for value equality. Properties of type <see
        /// cref="SubdivisionEdge"/> are compared using <see cref="Object.ReferenceEquals"/>.
        /// </para><para>
        /// <b>Equals</b> is intended for unit testing, as any two <see cref="SubdivisionFace"/>
        /// instances created during normal operation are never equal.</para></remarks>

        public bool Equals(SubdivisionFace face) {

            if (Object.ReferenceEquals(face, this)) return true;
            if (Object.ReferenceEquals(face, null)) return false;

            if (_key != face._key ||
                !Object.ReferenceEquals(Owner, face.Owner) ||
                !Object.ReferenceEquals(_outerEdge, face._outerEdge))
                return false;

            if (_innerEdges == null) {
                if (face._innerEdges != null) return false;
            } else {
                if (face._innerEdges == null || _innerEdges.Count != face._innerEdges.Count)
                    return false;

                for (int i = 0; i < _innerEdges.Count; i++)
                    if (!Object.ReferenceEquals(_innerEdges[i], face._innerEdges[i]))
                        return false;
            }

            return true;
        }

        #endregion
        #region Equals(SubdivisionFace, SubdivisionFace)

        /// <summary>
        /// Determines whether two specified <see cref="SubdivisionFace"/> instances have the same
        /// value.</summary>
        /// <param name="x">
        /// The first <see cref="SubdivisionFace"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="SubdivisionFace"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(SubdivisionFace)"/> overload to
        /// test the two <see cref="SubdivisionFace"/> instances for value equality.</remarks>

        public static bool Equals(SubdivisionFace x, SubdivisionFace y) {

            if (Object.ReferenceEquals(x, null))
                return Object.ReferenceEquals(y, null);

            return x.Equals(y);
        }

        #endregion
        #region StructureEquals

        /// <summary>
        /// Determines whether this instance and a specified <see cref="SubdivisionFace"/> have the
        /// same structure.</summary>
        /// <param name="face">
        /// A <see cref="SubdivisionFace"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the structure of <paramref name="face"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>StructureEquals</b> compares the values all properties of the two <see
        /// cref="SubdivisionFace"/> instances, except for <see cref="Owner"/>, to test for
        /// structural equality. Properties of type <see cref="SubdivisionEdge"/> are compared by
        /// their <see cref="Key"/> values.
        /// </para><para>
        /// <b>StructureEquals</b> is intended for testing the <see cref="Subdivision.Clone"/> 
        /// method which replicates <see cref="SubdivisionEdge"/> keys but not references.
        /// </para></remarks>

        public bool StructureEquals(SubdivisionFace face) {

            if (Object.ReferenceEquals(face, this)) return true;
            if (Object.ReferenceEquals(face, null)) return false;

            if (_key != face._key) return false;

            if (_outerEdge == null) {
                if (face._outerEdge != null) return false;
            } else {
                if (face._outerEdge == null || _outerEdge._key != face._outerEdge._key)
                    return false;
            }

            if (_innerEdges == null) {
                if (face._innerEdges != null) return false;
            } else {
                if (face._innerEdges == null || _innerEdges.Count != face._innerEdges.Count)
                    return false;

                for (int i = 0; i < _innerEdges.Count; i++)
                    if (_innerEdges[i]._key != face._innerEdges[i]._key)
                        return false;
            }

            return true;
        }

        #endregion
        #endregion
    }
}
