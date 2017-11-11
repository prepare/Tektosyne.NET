using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Tektosyne.Collections;
using Tektosyne.Graph;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a planar subdivision as a doubly-connected edge list.</summary>
    /// <remarks><para>
    /// <b>Subdivision</b> represents a planar subdivision containing only straight bounded edges,
    /// i.e. a collection of line segments in two-dimensional space that do not intersect except at
    /// their end points. The vertices of the subdivision are the end points of all line segments.
    /// The entire structure of edges and vertices constitutes the planar embedding of a graph.
    /// </para><para>
    /// In addition to edges and vertices, <b>Subdivision</b> also stores the faces formed by all
    /// edges. Faces are any polygonal regions that are bounded by edges, whether on the inside, on
    /// the outside, or both. Edges are represented by the <see cref="SubdivisionEdge"/> class, and
    /// faces are represented by the <see cref="SubdivisionFace"/> class.
    /// </para><para>
    /// <b>Subdivision</b> supports generic graph algorithms through its implementation of the <see
    /// cref="IGraph2D{T}"/> interface. The graph nodes are the <see cref="PointD"/> coordinates of
    /// all vertices. Two nodes are considered connected if an edge exists between their
    /// corresponding vertices. The distance measure is the Euclidean distance between vertices.
    /// </para><para>
    /// The planar subdivision is implemented as the doubly-connected edge list described by Mark de
    /// Berg et al., <em>Computational Geometry</em> (3rd ed.), Springer-Verlag 2008, p.29-43. This
    /// implementation represents edges as "twin" pairs of half-edges.</para></remarks>

    [Serializable]
    public class Subdivision: ICloneable, IGraph2D<PointD> {
        #region Subdivision()

        /// <overloads>
        /// Initializes a new instance of the <see cref="Subdivision"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="Subdivision"/> class with default
        /// properties.</summary>
        /// <remarks>
        /// The <see cref="Edges"/> and <see cref="Vertices"/> properties are initialized to empty
        /// collections with a default initial capacity. The <see cref="Faces"/> property is
        /// initialized to a collection that contains only the unbounded face and has a default
        /// initial capacity.</remarks>

        public Subdivision() {
            _edges = new SortedListEx<Int32, SubdivisionEdge>();
            _faces = new SortedListEx<Int32, SubdivisionFace>();
            _vertices = new SortedListEx<PointD, SubdivisionEdge>(new PointDComparerY());
            _vertexRegions = new Dictionary<PointD, PointD[]>();

            var face = new SubdivisionFace(this, _nextFaceKey++);
            _faces.Add(face._key, face);
        }

        #endregion
        #region Subdivision(Int32, Int32, Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="Subdivision"/> class with the specified
        /// initial capacities.</summary>
        /// <param name="edgeCapacity">
        /// The initial <see cref="SortedListEx{TKey, TValue}.Capacity"/> for the <see
        /// cref="Edges"/> collection.</param>
        /// <param name="faceCapacity">
        /// The initial <see cref="SortedListEx{TKey, TValue}.Capacity"/> for the <see
        /// cref="Faces"/> collection.</param>
        /// <param name="vertexCapacity">
        /// The initial <see cref="SortedListEx{TKey, TValue}.Capacity"/> for the <see
        /// cref="Vertices"/> collection.</param>
        /// <remarks>
        /// The <see cref="Edges"/> and <see cref="Vertices"/> properties are initialized to empty
        /// collections with the specified initial capacities. The <see cref="Faces"/> property is
        /// initialized to a collection that contains only the unbounded face and has the specified
        /// initial capacity.</remarks>

        public Subdivision(int edgeCapacity, int faceCapacity, int vertexCapacity) {

            _edges = new SortedListEx<Int32, SubdivisionEdge>(edgeCapacity);
            _faces = new SortedListEx<Int32, SubdivisionFace>(faceCapacity);
            _vertices = new SortedListEx<PointD, SubdivisionEdge>(vertexCapacity, new PointDComparerY());
            _vertexRegions = new Dictionary<PointD, PointD[]>();

            var face = new SubdivisionFace(this, _nextFaceKey++);
            _faces.Add(face._key, face);
        }

        #endregion
        #region Private Fields

        /// <summary>
        /// The list of all half-edges in the <see cref="Subdivision"/>, sorted by key.</summary>

        private readonly SortedListEx<Int32, SubdivisionEdge> _edges;

        /// <summary>
        /// The list of all faces in the <see cref="Subdivision"/>, sorted by key.</summary>

        private readonly SortedListEx<Int32, SubdivisionFace> _faces;

        /// <summary>
        /// The lexicographically sorted list of all vertices in the <see cref="Subdivision"/> and
        /// one of their incident half-edges.</summary>

        private readonly SortedListEx<PointD, SubdivisionEdge> _vertices;

        /// <summary>
        /// The <see cref="IGraph2D{T}"/> regions associated with all vertices, if any.</summary>

        private readonly Dictionary<PointD, PointD[]> _vertexRegions;

        /// <summary>
        /// The maximum number of half-edges originating from any vertex.</summary>

        private int _connectivity;

        /// <summary>
        /// The epsilon used for coordinate comparisons within the <see cref="Vertices"/>
        /// collection.</summary>

        private double _epsilon;

        /// <summary>
        /// The unique key for the next <see cref="SubdivisionEdge"/> instance.</summary>

        private int _nextEdgeKey;

        /// <summary>
        /// The unique key for the next <see cref="SubdivisionFace"/> instance.</summary>

        private int _nextFaceKey;

        /// <summary>
        /// The current y-coordinate of the sweep line of a plane sweep algorithm.</summary>

        [NonSerialized]
        private double _cursorY;

        #endregion
        #region Edges

        /// <summary>
        /// Gets a read-only view of all half-edges in the <see cref="Subdivision"/>.</summary>
        /// <value>
        /// A read-only <see cref="SortedListEx{TKey, TValue}"/> that maps the <see
        /// cref="SubdivisionEdge.Key"/> of each half-edge in the <see cref="Subdivision"/> to the
        /// corresponding <see cref="SubdivisionEdge"/> object.</value>
        /// <remarks><para>
        /// <b>Edges</b> always contains an even number of elements since every edge in the <see
        /// cref="Subdivision"/> is comprised of two <see cref="SubdivisionEdge"/> objects.
        /// </para><para>
        /// <b>Edges</b> is provided for convenience, unit testing, and faster edge scanning. This
        /// collection is not strictly needed since a list of all half-edges is easily obtained by
        /// iterating over all <see cref="Vertices"/>, e.g. using <see cref="GetEdgesByOrigin"/>.
        /// </para><para>
        /// Maintaining the <b>Edges</b> collection consumes little extra runtime but a significant
        /// amount of memory, so an alternative <see cref="Subdivision"/> implementation might
        /// choose to remove this collection and create a new list of half-edges where necessary.
        /// </para></remarks>

        public SortedListEx<Int32, SubdivisionEdge> Edges {
            [DebuggerStepThrough]
            get { return _edges.AsReadOnly(); }
        }

        #endregion
        #region Epsilon

        /// <summary>
        /// Gets or sets the epsilon used for coordinate comparisons within the <see
        /// cref="Vertices"/> collection.</summary>
        /// <value><para>
        /// The maximum absolute difference at which vertex coordinates should be considered equal.
        /// </para><para>-or-</para><para>
        /// Zero to use exact coordinate comparisons. The default is zero.</para></value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The property is set to a negative value.</exception>
        /// <exception cref="PropertyValueException">
        /// The property is changed, and <see cref="Vertices"/> is not an empty collection.
        /// </exception>
        /// <remarks><para>
        /// <b>Epsilon</b> returns the comparison epsilon used by the <see cref="SortedListEx{TKey,
        /// TValue}.Comparer"/> that establishes the lexicographical ordering of <see
        /// cref="PointD"/> keys within the <see cref="Vertices"/> collection.
        /// </para><para>
        /// <b>Epsilon</b> cannot be set if the <see cref="Vertices"/> collection already contains
        /// one or more elements. Factory methods that create a new <see cref="Subdivision"/>
        /// typically offer a parameter to set this property upon construction.
        /// </para><note type="caution">
        /// Do not attempt to directly change the <see cref="PointDComparerY.Epsilon"/> of the <see
        /// cref="Vertices"/> comparer! This will cause incorrect search results and data corruption
        /// if the <see cref="Vertices"/> collection was not empty. Moreover, the <see
        /// cref="Subdivision"/> caches the current <b>Epsilon</b> internally, and changing the
        /// comparer epsilon directly will not update the cached value.</note></remarks>

        public double Epsilon {
            get { return _epsilon; }
            set {
                if (_epsilon == value) return;

                if (_vertices.Count > 0)
                    ThrowHelper.ThrowPropertyValueException("Vertices", Strings.PropertyNotEmpty);

                ((PointDComparerY) _vertices.Comparer).Epsilon = _epsilon = value;
            }
        }

        #endregion
        #region Faces

        /// <summary>
        /// Gets a read-only view of all faces in the <see cref="Subdivision"/>.</summary>
        /// <value>
        /// A read-only <see cref="SortedListEx{TKey, TValue}"/> that maps the <see
        /// cref="SubdivisionFace.Key"/> of each face in the <see cref="Subdivision"/> to the
        /// corresponding <see cref="SubdivisionFace"/> object.</value>
        /// <remarks>
        /// <b>Faces</b> always contains at least one element which is the unbounded face. This <see
        /// cref="SubdivisionFace"/> always remains at index position zero and always has a <see
        /// cref="SubdivisionFace.Key"/> of zero.</remarks>

        public SortedListEx<Int32, SubdivisionFace> Faces {
            [DebuggerStepThrough]
            get { return _faces.AsReadOnly(); }
        }

        #endregion
        #region IsEmpty

        /// <summary>
        /// Gets a value indicating whether the <see cref="Subdivision"/> is empty.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="Edges"/> collection is empty; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// For any valid <see cref="Subdivision"/>, the <see cref="Edges"/> collection is empty
        /// exactly if the <see cref="Vertices"/> collection is also empty, and the <see
        /// cref="Faces"/> collection contains only the unbounded face.</remarks>

        public bool IsEmpty {
            get { return (_edges.Count == 0); }
        }

        #endregion
        #region VertexRegions

        /// <summary>
        /// Gets a dictionary that maps <see cref="Vertices"/> to <see cref="IGraph2D{T}"/> regions.
        /// </summary>
        /// <value>
        /// A <see cref="Dictionary{TKey, TValue}"/> that maps <see cref="Vertices"/> elements, i.e.
        /// <see cref="IGraph2D{T}"/> nodes, to <see cref="IGraph2D{T}"/> regions. The default is an
        /// empty collection.</value>
        /// <remarks><para>
        /// <b>VertexRegions</b> always returns an empty collection by default, as a planar <see
        /// cref="Subdivision"/> does not inherently associate regions with <see cref="Vertices"/>.
        /// Clients must explicitly add any desired key-and-value pairs.
        /// </para><para>
        /// <see cref="GetWorldRegion"/> attempts to return the polygonal region that 
        /// <b>VertexRegions</b> associates with a specified <see cref="Vertices"/> element, i.e.
        /// <see cref="IGraph2D{T}"/> node, before returning a null reference.</para></remarks>

        public Dictionary<PointD, PointD[]> VertexRegions {
            [DebuggerStepThrough]
            get { return _vertexRegions; }
        }

        #endregion
        #region Vertices

        /// <summary>
        /// Gets a read-only view of all vertices in the <see cref="Subdivision"/>.</summary>
        /// <value>
        /// A read-only <see cref="SortedListEx{TKey, TValue}"/> that maps the <see cref="PointD"/>
        /// coordinates of all vertices in the <see cref="Subdivision"/> to a <see
        /// cref="SubdivisionEdge"/> that originates with the vertex.</value>
        /// <remarks><para>
        /// <b>Vertices</b> is sorted lexicographically by <see cref="PointD"/> keys, using the
        /// ordering established by the <see cref="PointDComparerY"/> class. That is, keys are
        /// sorted first by <see cref="PointD.Y"/> and then by <see cref="PointD.X"/> coordinates.
        /// </para><para>
        /// If multiple <see cref="SubdivisionEdge"/> instances originate from the same vertex, one
        /// is selected arbitrarily for the <b>Vertices</b> collection, depending on the
        /// construction order of the <see cref="Subdivision"/>.
        /// </para><para>
        /// Every <b>Vertices</b> key is associated with a valid <see cref="SubdivisionEdge"/>; that
        /// is, a <see cref="Subdivision"/> never contains isolated points, only edges.
        /// </para></remarks>

        public SortedListEx<PointD, SubdivisionEdge> Vertices {
            [DebuggerStepThrough]
            get { return _vertices.AsReadOnly(); }
        }

        #endregion
        #region Public Methods
        #region AddEdge(PointD, PointD)

        /// <overloads>
        /// Adds a new edge to the <see cref="Subdivision"/> that connects the specified vertices.
        /// </overloads>
        /// <summary>
        /// Adds a new edge to the <see cref="Subdivision"/> that connects the specified vertices.
        /// </summary>
        /// <param name="start">
        /// The <see cref="PointD"/> coordinates of the first vertex to connect.</param>
        /// <param name="end">
        /// The <see cref="PointD"/> coordinates of the second vertex to connect.</param>
        /// <returns>
        /// On success, the new <see cref="SubdivisionEdge"/> from <paramref name="start"/> to
        /// <paramref name="end"/>; otherwise, a null reference.</returns>
        /// <remarks><para>
        /// <b>AddEdge</b> first checks if the new <see cref="SubdivisionEdge"/> would intersect any
        /// existing <see cref="Edges"/>, except at the specified <paramref name="start"/> and
        /// <paramref name="end"/> coordinates, and returns a null reference if so.
        /// </para><para>
        /// Otherwise, <b>AddEdge</b> creates two new <see cref="Edges"/> elements, from <paramref
        /// name="start"/> to <paramref name="end"/> and vice versa. If <paramref name="start"/>
        /// and/or <paramref name="end"/> are not found in the <see cref="Vertices"/> collection,
        /// <b>AddEdge</b> adds the corresponding <see cref="Vertices"/> elements as well.
        /// </para><para>
        /// If the added <see cref="SubdivisionEdge"/> connects an inner cycle of the containing
        /// <see cref="SubdivisionFace"/> with its outer cycle, <b>AddEdge</b> removes the inner
        /// cycle. If the added <see cref="SubdivisionEdge"/> connects two inner cycles, one of them
        /// is arbitrarily chosen for removal.
        /// </para><para>
        /// If the added <see cref="SubdivisionEdge"/> connects two half-edges within the same inner
        /// cycle, <b>AddEdge</b> creates a new <see cref="SubdivisionFace"/> for the resulting
        /// enclosed area. If the added <see cref="SubdivisionEdge"/> connects two half-edges within
        /// the outer cycle, <b>AddEdge</b> creates a new <see cref="SubdivisionFace"/> for the part
        /// enclosing the smaller area.</para></remarks>

        public SubdivisionEdge AddEdge(PointD start, PointD end) {
            int changedFace, addedFace;
            return AddEdge(start, end, out changedFace, out addedFace);
        }

        #endregion
        #region AddEdge(PointD, PointD, Int32, Int32)

        /// <summary>
        /// Adds a new edge to the <see cref="Subdivision"/> that connects the specified vertices,
        /// and returns information on the changed and added <see cref="Faces"/>.</summary>
        /// <param name="start">
        /// The <see cref="PointD"/> coordinates of the first vertex to connect.</param>
        /// <param name="end">
        /// The <see cref="PointD"/> coordinates of the second vertex to connect.</param>
        /// <param name="changedFace">
        /// Returns the <see cref="SubdivisionFace.Key"/> of the <see cref="SubdivisionFace"/> that
        /// was changed by the edge creation, if any; otherwise, -1.</param>
        /// <param name="addedFace">
        /// Returns the <see cref="SubdivisionFace.Key"/> of the <see cref="SubdivisionFace"/> that
        /// was added along with the edge, if any; otherwise, -1.</param>
        /// <returns>
        /// On success, the new <see cref="SubdivisionEdge"/> from <paramref name="start"/> to
        /// <paramref name="end"/>; otherwise, a null reference.</returns>
        /// <remarks>
        /// Please refer to the basic <see cref="AddEdge(PointD, PointD)"/> overload for details.
        /// </remarks>

        public SubdivisionEdge AddEdge(
            PointD start, PointD end, out int changedFace, out int addedFace) {

            changedFace = addedFace = -1;
            int startIndex = _vertices.IndexOfKey(start);
            int endIndex = _vertices.IndexOfKey(end);

            // reacquire vertices if epsilon matching possible
            if (_epsilon > 0) {
                if (startIndex >= 0) start = _vertices.GetKey(startIndex);
                if (endIndex >= 0) end = _vertices.GetKey(endIndex);
            }
            if (start == end) return null;

            SubdivisionFace face = null;
            SubdivisionEdge nextStartEdge = null, nextEndEdge = null;

            if (startIndex < 0 && endIndex < 0)
                face = FindFace(start);
            else {
                SubdivisionEdge vertexEdge = _vertices.GetByIndex(startIndex);
                SubdivisionEdge previousStartEdge = null, previousEndEdge = null;

                // check for existing edge connecting vertices
                if (startIndex >= 0 && endIndex >= 0)
                    foreach (SubdivisionEdge edge in vertexEdge.OriginEdges)
                        if (edge._twin._origin == end) return null;

                // find neighboring edges in start vertex chain
                if (startIndex >= 0) {
                    vertexEdge.FindEdgePosition(end, out nextStartEdge, out previousStartEdge);
                    Debug.Assert(nextStartEdge._previous == previousStartEdge._twin);
                    face = nextStartEdge._face;
                }

                // find neighboring edges in end vertex chain
                if (endIndex >= 0) {
                    vertexEdge = _vertices.GetByIndex(endIndex);
                    vertexEdge.FindEdgePosition(start, out nextEndEdge, out previousEndEdge);
                    Debug.Assert(nextEndEdge._previous == previousEndEdge._twin);

                    if (face == null)
                        face = nextEndEdge._face;
                    else if (face != nextEndEdge._face)
                        return null;
                }

                Debug.Assert(nextStartEdge != nextEndEdge);
                Debug.Assert(previousStartEdge != previousEndEdge);
            }

            LineD line = new LineD(start, end);
            int startInnerCycle = -1, endInnerCycle = -1;

            if (face._outerEdge != null) {
                SubdivisionEdge edge = face._outerEdge;
                do {
                    // check for proper intersection with outer cycle edges
                    var result = line.Intersect(edge.ToLine(), _epsilon);
                    if (result.ExistsBetween) return null;

                    edge = edge._next;
                } while (edge != face._outerEdge);
            }

            if (face._innerEdges != null)
                for (int i = 0; i < face._innerEdges.Count; i++) {
                    SubdivisionEdge innerEdge = face._innerEdges[i];
                    SubdivisionEdge edge = innerEdge;
                    do {
                        // check for proper intersection with inner cycle edge
                        var result = line.Intersect(edge.ToLine(), _epsilon);
                        if (result.ExistsBetween) return null;

                        // record inner cycle index for neighboring edge
                        if (edge == nextStartEdge) startInnerCycle = i;
                        else if (edge == nextEndEdge) endInnerCycle = i;

                        edge = edge._next;
                    } while (edge != innerEdge);
                }

            // create edges and (if necessary) vertices
            SubdivisionEdge startEdge;
            CreateTwinEdges(start, end, out startEdge);
            startEdge._face = startEdge._twin._face = face;
            changedFace = face._key;

            /*
             * If the new edge connects two new vertices, we have a new single-edge inner cycle.
             * 
             * If the new edge connects an existing vertex and a new vertex, we have a new
             * zero-area protrusion of some existing cycle and don't need to do anything.
             * 
             * If the new edge connects two existing vertices and merges two different cycles,
             * one of them must be an inner cycle which is now obsolete and can be deleted.
             * 
             * If the new edge connects two existing vertices and establishes a new connection
             * within the same cycle, we have a new outer cycle that constitutes a new face.
             */

            if (startIndex < 0 && endIndex < 0) {
                face.AddInnerEdge(startEdge);
            }
            else if (startIndex >= 0 && endIndex >= 0) {
                if (startInnerCycle != endInnerCycle) {
                    if (endInnerCycle >= 0)
                        face._innerEdges.RemoveAt(endInnerCycle);
                    else {
                        Debug.Assert(startInnerCycle >= 0);
                        face._innerEdges.RemoveAt(startInnerCycle);
                    }
                } else {
                    Debug.Assert(startInnerCycle == endInnerCycle);
                    SubdivisionEdge newFaceEdge;

                    if (startInnerCycle < 0) {
                        double edgeArea = startEdge.CycleArea;
                        double twinArea = startEdge._twin.CycleArea;
                        Debug.Assert(edgeArea > 0 && twinArea > 0);

                        // face with greater area keeps old key
                        if (edgeArea < twinArea) {
                            newFaceEdge = startEdge;
                            face._outerEdge = startEdge._twin;
                        } else {
                            newFaceEdge = startEdge._twin;
                            face._outerEdge = startEdge;
                        }
                    } else {
                        SubdivisionEdge pivot = startEdge;
                        foreach (SubdivisionEdge edge in startEdge.CycleEdges)
                            if (_vertices.Comparer.Compare(pivot._origin, edge._origin) > 0)
                                pivot = edge;

                        // use pivot vertex to determine outer cycle
                        double length = pivot._previous._origin.
                            CrossProductLength(pivot._origin, pivot._next._origin);

                        if (length > 0) {
                            newFaceEdge = startEdge;
                            face._innerEdges[startInnerCycle] = startEdge._twin;
                        } else {
                            newFaceEdge = startEdge._twin;
                            face._innerEdges[startInnerCycle] = startEdge;
                        }
                    }

                    // create new face and update incident edges
                    var newFace = new SubdivisionFace(this, _nextFaceKey++, newFaceEdge, null);
                    _faces.Add(newFace._key, newFace);
                    newFaceEdge.SetAllFaces(newFace);
                    addedFace = newFace._key;

                    // move inner cycles to new face if necessary
                    if (face._innerEdges != null) {
                        int count = face._innerEdges.Count;
                        for (int i = count - 1; i >= 0; i--) {
                            if (startInnerCycle == i) continue;
                            SubdivisionEdge innerEdge = face._innerEdges[i];

                            var result = newFaceEdge.Locate(innerEdge._origin);
                            if (result == PolygonLocation.Inside) {
                                face._innerEdges.RemoveAt(i);
                                newFace.AddInnerEdge(innerEdge);
                            }
                        }

                        if (face._innerEdges.Count == 0)
                            face._innerEdges = null;
                    }
                }
            }

            _connectivity = 0;
            return startEdge;
        }

        #endregion
        #region Find

        /// <summary>
        /// Finds the <see cref="SubdivisionElement"/> at the specified <see cref="PointD"/>
        /// coordinates within the <see cref="Subdivision"/>.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <param name="epsilon"><para>
        /// The maximum absolute difference at which two coordinates should be considered equal.
        /// </para><para>-or-</para><para>
        /// Zero to use exact coordinate comparisons. The default is zero.</para></param>
        /// <returns>
        /// The <see cref="SubdivisionElement"/> that coincides with <paramref name="q"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="epsilon"/> is less than zero.</exception>
        /// <remarks><para>
        /// <b>Find</b> first calls <see cref="FindFace(PointD)"/> to determine the smallest <see
        /// cref="Faces"/> element that contains <paramref name="q"/>, and then checks all its <see
        /// cref="SubdivisionFace.OuterEdge"/> and <see cref="SubdivisionFace.InnerEdges"/> cycles
        /// to determine whether <paramref name="q"/> coincides with an incident <see cref="Edges"/>
        /// or <see cref="Vertices"/> element.
        /// </para><para>
        /// <b>Find</b> performs a slow brute-force search. For better performance, create a <see
        /// cref="SubdivisionSearch"/> structure for repeated searches within the same <see
        /// cref="Subdivision"/>, or examine the <see cref="Vertices"/> and <see cref="Edges"/>
        /// collections directly if you expect <paramref name="q"/> to coincide with one of their
        /// elements.</para></remarks>

        public SubdivisionElement Find(PointD q, double epsilon = 0) {
            if (epsilon < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "epsilon", epsilon, Strings.ArgumentNegative);

            SubdivisionFace face = FindFace(q);

            // check all outer cycle edges
            if (face._outerEdge != null) {
                SubdivisionEdge edge = face._outerEdge;
                do {
                    LineD line = edge.ToLine();
                    LineLocation result = (epsilon == 0 ?
                        line.Locate(q) : line.Locate(q, epsilon));

                    switch (result) {
                        case LineLocation.Start:
                            return new SubdivisionElement(line.Start);

                        case LineLocation.End:
                            return new SubdivisionElement(line.End);

                        case LineLocation.Between:
                            return new SubdivisionElement(edge);
                    }

                    edge = edge._next;
                } while (edge != face._outerEdge);
            }

            /*
             * Check all inner cycle edges.
             * 
             * Technically, we should not hit edges of inner cycles with a positive area,
             * as FindFace would have returned the corresponding nested face in that case.
             * But we must check for possible zero-area cycles, and also for epsilon-matching
             * of inner edges or vertices.
             */

            if (face._innerEdges != null)
                for (int i = 0; i < face._innerEdges.Count; i++) {
                    SubdivisionEdge innerEdge = face._innerEdges[i];
                    SubdivisionEdge edge = innerEdge;
                    do {
                        LineD line = edge.ToLine();
                        LineLocation result = (epsilon == 0 ?
                            line.Locate(q) : line.Locate(q, epsilon));

                        switch (result) {
                            case LineLocation.Start:
                                return new SubdivisionElement(line.Start);

                            case LineLocation.End:
                                return new SubdivisionElement(line.End);

                            case LineLocation.Between:
                                return new SubdivisionElement(edge);
                        }

                        edge = edge._next;
                    } while (edge != innerEdge);
                }

            return new SubdivisionElement(face);
        }

        #endregion
        #region FindEdge

        /// <summary>
        /// Finds the half-edge in the <see cref="Subdivision"/> with the specified origin and
        /// destination.</summary>
        /// <param name="origin">
        /// The <see cref="SubdivisionEdge.Origin"/> of the half-edge.</param>
        /// <param name="destination">
        /// The <see cref="SubdivisionEdge.Destination"/> of the half-edge.</param>
        /// <returns><para>
        /// The <see cref="SubdivisionEdge"/> with the specified <paramref name="origin"/> and
        /// <paramref name="destination"/>.
        /// </para><para>-or-</para><para>
        /// A null reference if no matching <see cref="SubdivisionEdge"/> was found.
        /// </para></returns>
        /// <remarks><para>
        /// <b>FindEdge</b> first attempts to find the specified <paramref name="origin"/> in the
        /// <see cref="Vertices"/> collection, and then calls <see
        /// cref="SubdivisionEdge.GetEdgeTo"/> to find the half-edge leading from <paramref
        /// name="origin"/> to the specified <paramref name="destination"/>.
        /// </para><para>
        /// This is an O(ld n + m) operation, where n is the number of <see cref="Vertices"/> and m
        /// is the number of half-edges originating from <paramref name="origin"/>. All coordinate
        /// comparisons use the current <see cref="Epsilon"/>.</para></remarks>

        public SubdivisionEdge FindEdge(PointD origin, PointD destination) {

            SubdivisionEdge edge;
            if (!_vertices.TryGetValue(origin, out edge))
                return null;

            return (_epsilon == 0 ?
                edge.GetEdgeTo(destination) :
                edge.GetEdgeTo(destination, _epsilon));
        }

        #endregion
        #region FindFace(PointD)

        /// <overloads>
        /// Finds the indicated face in the <see cref="Subdivision"/>.</overloads>
        /// <summary>
        /// Finds the smallest face in the <see cref="Subdivision"/> that contains the specified
        /// coordinates.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to locate.</param>
        /// <returns>
        /// The smallest <see cref="SubdivisionFace"/> that contains <paramref name="q"/>.</returns>
        /// <remarks><para>
        /// <b>FindFace</b> performs a linear search through all bounded <see cref="Faces"/> for an
        /// <see cref="SubdivisionFace.OuterEdge"/> boundary that contains <paramref name="q"/>;
        /// that is, <see cref="SubdivisionEdge.Locate(PointD)"/> does not return <see
        /// cref="PolygonLocation.Outside"/>.
        /// </para><para>
        /// <b>FindFace</b> immediately returns the first containing face that has no <see
        /// cref="SubdivisionFace.InnerEdges"/>. If there are multiple containing faces with one or
        /// more <see cref="SubdivisionFace.InnerEdges"/>, <b>FindFace</b> chooses the one with the
        /// smallest outer <see cref="SubdivisionEdge.CycleArea"/>. If no containing face was found,
        /// <b>FindFace</b> returns the unbounded face.
        /// </para><para>
        /// <b>FindFace</b> has an average runtime of O(n/2) where n is the number of bounded <see
        /// cref="Faces"/>, unless <see cref="Faces"/> with <see cref="SubdivisionFace.InnerEdges"/>
        /// are frequent in which case the runtime approaches O(n).</para></remarks>

        public SubdivisionFace FindFace(PointD q) {
            SubdivisionFace resultFace = null;
            double resultArea = 0;

            // check all bounded faces for containment
            for (int i = 1; i < _faces.Count; i++) {
                SubdivisionFace face = _faces.GetByIndex(i);
                if (face._outerEdge.Locate(q) == PolygonLocation.Outside)
                    continue;

                // succeed if no nested faces exist
                if (face._innerEdges == null) return face;

                if (resultFace == null)
                    resultFace = face;
                else {
                    // switch to nested face with smaller area
                    double area = face._outerEdge.CycleArea;
                    if (resultArea == 0)
                        resultArea = resultFace._outerEdge.CycleArea;

                    if (resultArea > area) {
                        resultArea = area;
                        resultFace = face;
                    }
                }
            }

            // default to unbounded face
            return (resultFace ?? _faces.GetByIndex(0));
        }

        #endregion
        #region FindFace(PointD[], Boolean)

        /// <summary>
        /// Finds the face in the <see cref="Subdivision"/> whose outer boundary equals the
        /// specified polygon.</summary>
        /// <param name="polygon">
        /// An <see cref="Array"/> whose <see cref="PointD"/> elements represent the consecutive
        /// vertices of the outer boundary.</param>
        /// <param name="verify">
        /// <c>true</c> to verify that the outer boundary is fully congruent with <paramref
        /// name="polygon"/>; <c>false</c> to return a result as soon as any alternative has been
        /// eliminated. The default is <c>false</c>.</param>
        /// <returns><para>
        /// The <see cref="SubdivisionFace"/> whose outer boundary equals the specified <paramref
        /// name="polygon"/>.
        /// </para><para>-or-</para><para>
        /// A null reference if no matching <see cref="SubdivisionFace"/> was found.
        /// </para></returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="polygon"/> is a null reference or contains less than three elements.
        /// </exception>
        /// <remarks><para>
        /// <b>FindFace</b> calls <see cref="FindEdge"/> on each pair of consecutive <paramref
        /// name="polygon"/> vertices, and immediately returns a null reference any such pair is not
        /// connected by a <see cref="SubdivisionEdge"/>. Otherwise, <b>FindFace</b> examines the
        /// <see cref="SubdivisionEdge.Face"/> pointers of both twin half-edges. If the <see
        /// cref="SubdivisionFace"/> on the same side of the <paramref name="polygon"/> ever
        /// changes, <b>FindFace</b> eliminates all half-edges on that side.
        /// </para><para>
        /// If <paramref name="verify"/> is <c>false</c>, <b>FindFace</b> immediately returns the
        /// <see cref="SubdivisionFace"/> on the remaining side when the other has been eliminated.
        /// Otherwise, <b>FindFace</b> continues checking the half-edges on the remaining side,
        /// verifying that they form a cycle around a single <see cref="SubdivisionFace"/> which
        /// also contains its <see cref="SubdivisionFace.OuterEdge"/>.
        /// </para><para>
        /// The specified <paramref name="polygon"/> may begin with any incident vertex, and the
        /// sequence of vertices may follow either the <see cref="SubdivisionEdge.Next"/> or the
        /// <see cref="SubdivisionEdge.Previous"/> pointers around the incident half-edges.
        /// </para><para>
        /// Depending on the <paramref name="verify"/> flag, <b>FindFace</b> has a runtime between
        /// O(ld n + 2m) and O(ld n + km), where n is the number of <see cref="Vertices"/>, m is the
        /// number of half-edges originating from each vertex, and k is the number of <paramref
        /// name="polygon"/> vertices. All coordinate comparisons use the current <see
        /// cref="Epsilon"/>.</para></remarks>

        public SubdivisionFace FindFace(PointD[] polygon, bool verify = false) {
            if (polygon == null || polygon.Length < 3)
                ThrowHelper.ThrowArgumentNullOrEmptyException("polygon");

            SubdivisionEdge edge = FindEdge(polygon[polygon.Length - 1], polygon[0]);
            if (edge == null) return null;

            // one edge provides two possible faces, one on each side
            SubdivisionFace face = edge._face, twinFace = edge._twin._face;
            bool isOuter = (edge == face._outerEdge);
            bool isTwinOuter = (edge._twin == twinFace._outerEdge);

            // check remaining edges to see which face is correct
            for (int i = 1; i < polygon.Length; i++) {

                edge = (_epsilon == 0 ?
                    edge._twin.GetEdgeTo(polygon[i]) :
                    edge._twin.GetEdgeTo(polygon[i], _epsilon));

                if (edge == null) return null;

                // eliminate side with two different cycles
                if (face != null) {
                    if (face != edge._face) {
                        if (!verify) return twinFace;
                        face = null;
                        if (twinFace == null) return null;
                    } else
                        isOuter |= (edge == face._outerEdge);
                }

                if (twinFace != null) {
                    if (twinFace != edge._twin._face) {
                        if (!verify) return face;
                        twinFace = null;
                        if (face == null) return null;
                    } else
                        isTwinOuter |= (edge._twin == twinFace._outerEdge);
                }
            }

            // only one face left, return other face
            if (face == null) {
                Debug.Assert(twinFace != null);
                return (isTwinOuter ? twinFace : null);
            }

            if (twinFace == null) {
                Debug.Assert(face != null);
                return (isOuter ? face : null);
            }

            // two faces left, check for outer boundary
            if (isOuter) return face;
            if (isTwinOuter) return twinFace;
            return null;
        }

        #endregion
        #region FindNearestEdge

        /// <summary>
        /// Finds the half-edge in the <see cref="Subdivision"/> that is nearest to and facing the
        /// specified coordinates.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to locate.</param>
        /// <param name="distance">
        /// Returns the distance between <paramref name="q"/> and the returned <see cref="Edges"/>
        /// element, if any; otherwise, <see cref="Double.MaxValue"/>.</param>
        /// <returns><para>
        /// The <see cref="Edges"/> element with the smallest distance to and facing <paramref
        /// name="q"/>.
        /// </para><para>-or-</para><para>
        /// A null reference if the <see cref="Edges"/> collection is empty.</para></returns>
        /// <remarks>
        /// <b>FindNearestEdge</b> first calls <see cref="FindFace(PointD)"/> to determine the <see
        /// cref="Faces"/> element that contains <paramref name="q"/>, and then calls <see
        /// cref="SubdivisionFace.FindNearestEdge"/> on that face to determine the nearest facing
        /// half-edge and its <paramref name="distance"/> from <paramref name="q"/>.</remarks>

        public SubdivisionEdge FindNearestEdge(PointD q, out double distance) {
            SubdivisionFace face = FindFace(q);
            return face.FindNearestEdge(q, out distance);
        }

        #endregion
        #region FindNearestVertex

        /// <summary>
        /// Finds the vertex in the <see cref="Subdivision"/> that is nearest to the specified
        /// coordinates.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to locate.</param>
        /// <returns>
        /// The zero-based index of <paramref name="q"/> in <see cref="Vertices"/>, if found;
        /// otherwise, the zero-based index of the <see cref="Vertices"/> key with the smallest
        /// Euclidean distance to <paramref name="q"/>.</returns>
        /// <remarks>
        /// <b>FindNearestVertex</b> returns the result of <see cref="PointDComparerY.FindNearest"/>
        /// for the <see cref="PointDComparerY"/> used to sort the <see cref="Vertices"/>
        /// collection. This is usually an O(ld n) operation, where n in the number of <see
        /// cref="Vertices"/>.</remarks>

        public int FindNearestVertex(PointD q) {
            PointDComparerY comparer = (PointDComparerY) _vertices.Comparer;
            return comparer.FindNearest(_vertices.Keys, q);
        }

        #endregion
        #region FromLines

        /// <summary>
        /// Creates a <see cref="Subdivision"/> from the specified line segments.</summary>
        /// <param name="lines">
        /// An <see cref="Array"/> of <see cref="LineD"/> instances that represent the <see
        /// cref="Edges"/> in the new <see cref="Subdivision"/>.</param>
        /// <param name="epsilon"><para>
        /// The maximum absolute difference at which coordinates should be considered equal.
        /// </para><para>-or-</para><para>
        /// Zero to use exact coordinate comparisons. The default is zero.</para></param>
        /// <returns>
        /// A new <see cref="Subdivision"/> instance whose <see cref="Edges"/> are the specified
        /// <paramref name="lines"/>, and whose <see cref="Vertices"/> are their <see
        /// cref="LineD.Start"/> and <see cref="LineD.End"/> points.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="lines"/> contains an element whose <see cref="LineD.Start"/> point
        /// equals its <see cref="LineD.End"/> point.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="lines"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="epsilon"/> is less than zero.</exception>
        /// <remarks><para>
        /// <b>FromLines</b> also determines the <see cref="Faces"/> that are formed by the <see
        /// cref="Edges"/> of the new <see cref="Subdivision"/>, and sets its comparison <see
        /// cref="Epsilon"/> to the specified <paramref name="epsilon"/>. The new <see
        /// cref="Subdivision"/> is empty if <paramref name="lines"/> is an empty array.
        /// </para><note type="caution">
        /// The specified <paramref name="lines"/> must not intersect or overlap anywhere except in
        /// their <see cref="LineD.Start"/> and <see cref="LineD.End"/> points. <b>FromLines</b>
        /// does not check this condition. If violated, the returned <see cref="Subdivision"/> will
        /// be invalid.</note></remarks>

        public static Subdivision FromLines(LineD[] lines, double epsilon = 0) {
            if (lines == null)
                ThrowHelper.ThrowArgumentNullException("lines");

            Subdivision division;
            if (lines.Length == 0) {
                division = new Subdivision();
                division.Epsilon = epsilon;
            } else {
                division = new Subdivision(lines.Length * 2, lines.Length / 4, lines.Length);
                division.Epsilon = epsilon;
                division.CreateAllFromLines(lines);
            }

            return division;
        }

        #endregion
        #region FromPolygons

        /// <summary>
        /// Creates a <see cref="Subdivision"/> from the specified polygons.</summary>
        /// <param name="polygons">
        /// An <see cref="IList{T}"/> of <see cref="PointD"/> arrays that represent the outer
        /// boundaries of all bounded <see cref="Faces"/> in the new <see cref="Subdivision"/>.
        /// </param>
        /// <param name="epsilon"><para>
        /// The maximum absolute difference at which coordinates should be considered equal.
        /// </para><para>-or-</para><para>
        /// Zero to use exact coordinate comparisons. The default is zero.</para></param>
        /// <returns>
        /// A new <see cref="Subdivision"/> instance whose bounded <see cref="Faces"/> are the
        /// specified <paramref name="polygons"/>, with the corresponding <see cref="Edges"/> and
        /// <see cref="Vertices"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="polygons"/> contains an <see cref="Array"/> that is a null reference or
        /// contains less than three elements or two consecutive identical elements.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="polygons"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="epsilon"/> is less than zero.</exception>
        /// <remarks><para>
        /// <b>FromPolygons</b> sets the comparison <see cref="Epsilon"/> of the new <see
        /// cref="Subdivision"/> to the specified <paramref name="epsilon"/>. The new <see
        /// cref="Subdivision"/> is empty if <paramref name="polygons"/> is an empty collection.
        /// </para><note type="caution">
        /// The specified <paramref name="polygons"/> may share common edges and vertices, and may
        /// be fully contained within one another, but must not otherwise intersect or overlap.
        /// <b>FromPolygons</b> does not check this condition. If violated, the returned <see
        /// cref="Subdivision"/> will be invalid.</note></remarks>

        public static Subdivision FromPolygons(IList<PointD[]> polygons, double epsilon = 0) {
            if (polygons == null)
                ThrowHelper.ThrowArgumentNullException("polygons");

            Subdivision division;
            if (polygons.Count == 0) {
                division = new Subdivision();
                division.Epsilon = epsilon;
            } else {
                division = new Subdivision(polygons.Count * 4, polygons.Count + 1, polygons.Count * 4);
                division.Epsilon = epsilon;
                division.CreateAllFromPolygons(polygons);
            }

            return division;
        }

        #endregion
        #region GetEdgesByOrigin

        /// <summary>
        /// Returns a list of all half-edges in the <see cref="Subdivision"/>, lexicographically
        /// sorted by origin.</summary>
        /// <returns>
        /// An <see cref="Array"/> containing all <see cref="Edges"/>, but sorted lexicographically
        /// by their <see cref="SubdivisionEdge.Origin"/> coordinates rather than by <see
        /// cref="SubdivisionEdge.Key"/>.</returns>
        /// <remarks>
        /// <b>GetEdgesByOrigin</b> does not use the <see cref="Edges"/> collection, but rather
        /// scans the <see cref="Vertices"/> collection by lexicographically ascending coordinates.
        /// All half-edges originating from the same vertex are stored in consecutive index
        /// positions, proceeding clockwise around the vertex.</remarks>

        public SubdivisionEdge[] GetEdgesByOrigin() {
            var edges = new SubdivisionEdge[_edges.Count];
            int edgeIndex = 0;

            foreach (SubdivisionEdge firstEdge in _vertices.Values) {
                SubdivisionEdge edge = firstEdge;
                do {
                    edges[edgeIndex++] = edge;
                    edge = edge._twin._next;
                } while (edge != firstEdge);
            }

            Debug.Assert(edgeIndex == edges.Length);
            return edges;
        }

        #endregion
        #region GetZeroAreaCycles

        /// <summary>
        /// Returns all half-edge cycles in the <see cref="Subdivision"/> that enclose no area.
        /// </summary>
        /// <returns>
        /// A <see cref="List{T}"/> containing one <see cref="SubdivisionEdge"/> from each cycle in
        /// the <see cref="Subdivision"/> that encloses no area.</returns>
        /// <remarks>
        /// <b>GetZeroAreaCycles</b> returns all <see cref="SubdivisionFace.InnerEdges"/> of all
        /// <see cref="Faces"/> for which <see cref="SubdivisionEdge.IsCycleAreaZero"/> succeeds.
        /// </remarks>

        public List<SubdivisionEdge> GetZeroAreaCycles() {
            var cycles = new List<SubdivisionEdge>();

            foreach (SubdivisionFace face in _faces.Values) {
                if (face._innerEdges == null) continue;
                foreach (SubdivisionEdge edge in face._innerEdges)
                    if (edge.IsCycleAreaZero) cycles.Add(edge);
            }

            return cycles;
        }

        #endregion
        #region Intersection

        /// <summary>
        /// Creates a <see cref="Subdivision"/> from the intersection of the two specified
        /// instances.</summary>
        /// <param name="division1">
        /// The first <see cref="Subdivision"/> to intersect.</param>
        /// <param name="division2">
        /// The second <see cref="Subdivision"/> to intersect.</param>
        /// <param name="faceKeys">
        /// Returns an <see cref="Array"/> that maps the <see cref="SubdivisionFace.Key"/> of each
        /// <see cref="SubdivisionFace"/> in the returned <see cref="Subdivision"/> to those of the
        /// containing <see cref="SubdivisionFace"/> in <paramref name="division1"/> and <paramref
        /// name="division2"/>, stored in the <see cref="ValueTuple{T1,T2}.Item1"/> and <see
        /// cref="ValueTuple{T1,T2}.Item2"/> components, respectively.</param>
        /// <returns>
        /// A new <see cref="Subdivision"/> instances that represents the intersection of <paramref
        /// name="division1"/> and <paramref name="division2"/>.</returns>
        /// <exception cref="ArgumentException"><para>
        /// Neither <paramref name="division1"/> nor <paramref name="division2"/> is empty, and the
        /// <see cref="Epsilon"/> of <paramref name="division1"/> is greater than that of <paramref
        /// name="division2"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="division1"/> or <paramref name="division2"/> is structurally invalid.
        /// </para></exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="division1"/> or <paramref name="division2"/> is a null reference.
        /// </exception>
        /// <remarks><para>
        /// <b>Intersection</b> first intersects the <see cref="Edges"/> of <paramref
        /// name="division1"/> with those of <paramref name="division2"/>, and then creates the
        /// resulting <see cref="Faces"/> with consecutive keys that equal their index positions.
        /// The original <see cref="Faces"/> that equal or contain the new <see cref="Faces"/> are
        /// indicated by <paramref name="faceKeys"/>.
        /// </para><para>
        /// <b>Intersection</b> uses the <see cref="Epsilon"/> of <paramref name="division1"/> to
        /// compare <see cref="Vertices"/> for equality. Therefore, <paramref name="division2"/>
        /// must use the same or a greater <see cref="Epsilon"/>; otherwise, some of its <see
        /// cref="Edges"/> might not be representable in the created <see cref="Subdivision"/>. 
        /// Moreover, the comparison epsilon used to detect edge intersections is raised to a
        /// minimum of 1e-10 for better numerical stability.
        /// </para><para>
        /// <b>Intersection</b> performs best if either <paramref name="division1"/> or <paramref
        /// name="division2"/> is empty, and worst if both instances are of equal size. That is
        /// because the algorithm intersects the <see cref="Edges"/> of <paramref name="division1"/>
        /// with those of <paramref name="division2"/>, rather than intersecting all <see
        /// cref="Edges"/> of the combined <see cref="Subdivision"/> with each other.
        /// </para></remarks>

        public static Subdivision Intersection(Subdivision division1,
            Subdivision division2, out ValueTuple<Int32, Int32>[] faceKeys) {

            if (division1 == null)
                ThrowHelper.ThrowArgumentNullException("division1");
            if (division2 == null)
                ThrowHelper.ThrowArgumentNullException("division2");

            if (division1._epsilon > division2._epsilon)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "division2", Strings.ArgumentPropertyConflict, "Epsilon");

            /*
             * Prepare dictionaries that map half-edge keys of the combined subdivision to the
             * corresponding incident face keys in each intersected subdivisions.
             * 
             * The first dictionary can be initialized directly since CloneEdges also copies the
             * keys of all copied half-edges. IntersectEdges may later add new elements if the
             * first-division edges are split, and will also store all second-division face keys
             * while computing their intersections.
             * 
             * We never store edges whose incident face key is zero, as the final face mapping array
             * is initialized to all zeroes anyway. Initial capacities equal input edge counts,
             * gambling that this optimization leaves enough vacant slots for any new elements.
             */

            var edgeToFace1 = new Int32Dictionary<Int32>(division1._edges.Count);
            var edgeToFace2 = new Int32Dictionary<Int32>(division2._edges.Count);

            foreach (SubdivisionEdge edge in division1._edges.Values)
                if (edge._face._key != 0)
                    edgeToFace1.Add(edge._key, edge._face._key);

            // combine all edges from both subdivisions
            Subdivision division = division1.CloneEdges();
            division.IntersectEdges(division2, edgeToFace1, edgeToFace2);

            // find all cycles and convert them into faces
            var cycles = division.FindCycles();
            division.CreateFacesFromCycles(cycles.Item1, cycles.Item2);

            Int32[] faceKeys1 = new Int32[division._faces.Count];
            Int32[] faceKeys2 = new Int32[division._faces.Count];

            // map created faces to containing intersected faces
            foreach (SubdivisionEdge edge in division._edges.Values) {
                int newFace = edge._face._key, oldFace;

                if (faceKeys1[newFace] == 0 && edgeToFace1.TryGetValue(edge._key, out oldFace)) {
                    Debug.Assert(oldFace != 0);
                    faceKeys1[newFace] = oldFace;
                }

                if (faceKeys2[newFace] == 0 && edgeToFace2.TryGetValue(edge._key, out oldFace)) {
                    Debug.Assert(oldFace != 0);
                    faceKeys2[newFace] = oldFace;
                }
            }

            faceKeys = new ValueTuple<Int32, Int32>[division._faces.Count];
            for (int i = 0; i < faceKeys.Length; i++)
                faceKeys[i] = ValueTuple.Create(faceKeys1[i], faceKeys2[i]);

            division._connectivity = 0;
            return division;
        }

        #endregion
        #region MoveVertex

        /// <summary>
        /// Moves the specified vertex to the specified coordinates.</summary>
        /// <param name="index">
        /// The zero-based index of the <see cref="Vertices"/> element to move.</param>
        /// <param name="vertex">
        /// The <see cref="PointD"/> coordinates to store at <paramref name="index"/>.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Vertices"/> element at <paramref name="index"/> was moved
        /// to <paramref name="vertex"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than the number of <see
        /// cref="Vertices"/>.</para></exception>
        /// <remarks><para>
        /// <b>MoveVertex</b> first checks whether the <see cref="Vertices"/> collection already
        /// contains the specified <paramref name="vertex"/>, or whether moving the <see
        /// cref="SubdivisionEdge.Origin"/> of any incident <see cref="SubdivisionEdge"/> to
        /// <paramref name="vertex"/> would create an intersection with any non-incident half-edge
        /// on any boundary of any incident <see cref="SubdivisionFace"/>.
        /// </para><para>
        /// If so, <b>MoveVertex</b> returns <c>false</c>. Otherwise, <b>MoveVertex</b> updates the
        /// <see cref="Vertices"/> collection and the <see cref="SubdivisionEdge.Origin"/> of all
        /// incident <see cref="Edges"/> and returns <c>true</c>.
        /// </para><note type="caution">
        /// On success, the index position of the specified <paramref name="vertex"/> may differ
        /// from the original <paramref name="index"/>. You must query the <see cref="Vertices"/>
        /// collection to determine the new index position.</note></remarks>

        public bool MoveVertex(int index, PointD vertex) {
            if (_vertices.ContainsKey(vertex))
                return false;

            int capacity = (_connectivity > 0 ? _connectivity : 8);
            var oldEdges = new List<SubdivisionEdge>(capacity);
            var faces = new List<SubdivisionFace>(capacity);

            // get incident edges and distinct incident faces
            SubdivisionEdge oldEdge = _vertices.GetByIndex(index);
            foreach (SubdivisionEdge edge in oldEdge.OriginEdges) {
                oldEdges.Add(edge);
                if (!faces.Contains(edge._face))
                    faces.Add(edge._face);
            }

            // create line segments that represent new edges
            LineD[] newEdges = new LineD[oldEdges.Count];
            for (int i = 0; i < newEdges.Length; i++)
                newEdges[i] = new LineD(vertex, oldEdges[i]._twin._origin);

            // check for intersections of new edges with any other edges
            foreach (SubdivisionFace face in faces)
                foreach (SubdivisionEdge edge in face.AllCycleEdges) {
                    if (oldEdges.Contains(edge) || oldEdges.Contains(edge._twin))
                        continue;

                    LineD edgeLine = edge.ToLine();
                    foreach (LineD line in newEdges) {
                        var result = line.Intersect(edgeLine, _epsilon);
                        if (result.ExistsBetween) return false;
                    }
                }

            // adjust vertex and incident edges
            _vertices.RemoveAt(index);
            _vertices.Add(vertex, oldEdge);

            foreach (SubdivisionEdge edge in oldEdges)
                edge._origin = vertex;

            return true;
        }

        #endregion
        #region RemoveEdge(Int32)

        /// <overloads>
        /// Removes the specified edge from the <see cref="Subdivision"/>.</overloads>
        /// <summary>
        /// Removes the specified edge from the <see cref="Subdivision"/>.</summary>
        /// <param name="edgeKey">
        /// The <see cref="SubdivisionEdge.Key"/> of one <see cref="SubdivisionEdge"/> to remove.
        /// Its <see cref="SubdivisionEdge.Twin"/> is removed as well.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="edgeKey"/> was found in the <see cref="Edges"/>
        /// collection and the associated edge was removed; otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// If the removed <see cref="SubdivisionEdge"/> and its <see cref="SubdivisionEdge.Twin"/>
        /// bound two different faces, <b>RemoveEdge</b> also removes the <see cref="Faces"/>
        /// element whose <em>outer</em> boundary contains a removed half-edge. If <em>both</em>
        /// removed half-edges constitute outer boundaries, <b>RemoveEdge</b> removes the <see
        /// cref="Faces"/> element with the greater <see cref="SubdivisionFace.Key"/>.
        /// </para><para>
        /// If the <see cref="SubdivisionEdge.Origin"/> or <see cref="SubdivisionEdge.Destination"/>
        /// of the removed <see cref="SubdivisionEdge"/> does not terminate any other <see
        /// cref="Edges"/>, <b>RemoveEdge</b> also removes the corresponding <see cref="Vertices"/>
        /// element(s).</para></remarks>

        public bool RemoveEdge(int edgeKey) {
            int changedFace, removedFace;
            return RemoveEdge(edgeKey, out changedFace, out removedFace);
        }

        #endregion
        #region RemoveEdge(Int32, Int32, Int32)

        /// <summary>
        /// Removes the specified edge from the <see cref="Subdivision"/>, and returns information
        /// on the changed and removed <see cref="Faces"/>.</summary>
        /// <param name="edgeKey">
        /// The <see cref="SubdivisionEdge.Key"/> of one <see cref="SubdivisionEdge"/> to remove.
        /// Its <see cref="SubdivisionEdge.Twin"/> is removed as well.</param>
        /// <param name="changedFace">
        /// Returns the <see cref="SubdivisionFace.Key"/> of the <see cref="SubdivisionFace"/> that
        /// was changed by the edge removal, if any; otherwise, -1.</param>
        /// <param name="removedFace">
        /// Returns the <see cref="SubdivisionFace.Key"/> of the <see cref="SubdivisionFace"/> that
        /// was removed along with the edge, if any; otherwise, -1.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="edgeKey"/> was found in the <see cref="Edges"/>
        /// collection and the associated edge was removed; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Please refer to the basic <see cref="RemoveEdge(Int32)"/> overload for details.
        /// </remarks>

        public bool RemoveEdge(int edgeKey, out int changedFace, out int removedFace) {
            changedFace = -1;
            removedFace = -1;

            SubdivisionEdge edge;
            if (!_edges.TryGetValue(edgeKey, out edge))
                return false;

            /*
             * 1. If both faces are equal, the edge is (part of) a zero-area protrusion of an outer
             *    or inner boundary. No face is removed. This is the only case where both half-edges
             *    may be inner boundaries, and may form a cycle containing no other half-edges.
             * 
             * 2. If both faces are different and both half-edges are outer boundaries, retain the
             *    face with the smaller key. The new combined boundary enlarges its outer boundary.
             *    All inner boundaries of the removed face are copied to the retained face.
             * 
             * 3. If both faces are different and one half-edge is an inner boundary, the other must
             *    be an outer boundary. Retain the face with the inner boundary. The new combined
             *    boundary enlarges that inner boundary. All inner boundaries of the removed face
             *    are copied to the retained face.
             */

            SubdivisionEdge twin = edge._twin;
            if (edge._face == twin._face) {
                changedFace = edge._face._key;
                edge._face.MoveEdge(edge);
            }
            else {
                // determine edge whose face has the smaller key
                SubdivisionEdge e0, e1;
                if (edge._face._key < twin._face._key) {
                    e0 = edge; e1 = twin;
                } else {
                    e0 = twin; e1 = edge;
                }

                // check if face with greater key is outer boundary
                SubdivisionFace e1Face = e1._face;
                if (e1Face._innerEdges == null) {
                    Debug.Assert(e1Face._outerEdge != null);
                    goto updateFaces;
                }

                if (e1Face._outerEdge != null) {
                    SubdivisionEdge cursor = e1;
                    do {
                        if (cursor == e1Face._outerEdge)
                            goto updateFaces;
                        cursor = cursor._next;
                    } while (cursor != e1);
                }

                // always keep face that is inner boundary
                e1Face = e0._face;
                e0 = e1;

            updateFaces:
                e0._face.MoveEdge(e0);
                e0._face.AddInnerEdges(e1Face._innerEdges);
                e1Face.SetAllEdgeFaces(e0._face);

                changedFace = e0._face._key;
                removedFace = e1Face._key;
                _faces.Remove(removedFace);
            }

            // remove half-edges from vertex chains
            RemoveAtOrigin(edge);
            RemoveAtOrigin(twin);

            /*
             * If both faces are equal, and we did not entirely remove an inner boundary or the
             * zero-area tip of a boundary, then we cut a zero-area protrusion or connection into
             * two parts. One part now forms a new inner cycle within the same face.
             * 
             * Note that we check removedFace rather than comparing edge.Face to twin.Face because
             * all incident faces have already been updated. At this point, edge.Face and twin.Face
             * both refer to changedFace rather than removedFace, even if the latter is valid.
             */

            if (removedFace < 0 && edge._next != twin && edge._previous != twin) {
                SubdivisionEdge outerEdge = edge._face._outerEdge;
                ListEx<SubdivisionEdge> innerEdges = edge._face._innerEdges;

                /*
                 * To find the new inner cycle, we explore the cycles starting after and before the
                 * removed half-edge. If we arrive at an existing InnerEdges element, we know that
                 * both cycles are inner cycles, and add the other cycle as a new inner cycle.
                 * 
                 * Otherwise, if we arrive at the existing OuterEdge, we must find and compare the
                 * lexicographically smallest vertices of both cycles to determine which one is the
                 * shortened outer cycle and which is added as a new inner cycle.
                 * 
                 * We check both conditions for both cycles in a single loop which we exit as soon
                 * as we match any InnerEdges element. We stop doing OuterEdge and InnerEdges
                 * comparisons as soon as we match OuterEdge in either cycle.
                 */

                SubdivisionEdge nextCursor = edge._next, prevCursor = edge._previous;
                PointD nextPivot = nextCursor._origin, prevPivot = prevCursor._origin;
                bool nextIsOuter = false, prevIsOuter = false;
                var vertexComparer = (PointDComparerY) _vertices.Comparer;

                do {
                    if (nextCursor != null) {
                        if (!nextIsOuter && !prevIsOuter) {
                            if (outerEdge == nextCursor)
                                nextIsOuter = true;
                            else if (innerEdges != null && innerEdges.Contains(nextCursor)) {
                                edge._face.AddInnerEdge(edge._previous);
                                break;
                            }
                        }

                        if (nextCursor == twin._previous)
                            nextCursor = null;
                        else {
                            nextCursor = nextCursor._next;
                            if (vertexComparer.Compare(nextPivot, nextCursor._origin) > 0)
                                nextPivot = nextCursor._origin;
                        }
                    }

                    if (prevCursor != null) {
                        if (!nextIsOuter && !prevIsOuter) {
                            if (outerEdge == prevCursor)
                                prevIsOuter = true;
                            else if (innerEdges != null && innerEdges.Contains(prevCursor)) {
                                edge._face.AddInnerEdge(edge._next);
                                break;
                            }
                        }

                        if (prevCursor == twin._next)
                            prevCursor = null;
                        else {
                            prevCursor = prevCursor._previous;
                            if (vertexComparer.Compare(prevPivot, prevCursor._origin) > 0)
                                prevPivot = prevCursor._origin;
                        }
                    }

                } while (nextCursor != null || prevCursor != null);

                /*
                 * If either cycle contains the existing OuterEdge, one of the two cycles is the
                 * shortened outer cycle -- but not necessarily the one containing OuterEdge.
                 * 
                 * We compare the lexicographically smallest pivot vertices in each cycle to find
                 * the actual outer cycle, switch OuterEdge to a half-edge within that cycle if
                 * necessary, and add the other cycle as a new inner cycle.
                 */

                if (nextIsOuter || prevIsOuter) {
                    int pivotCompare = vertexComparer.Compare(nextPivot, prevPivot);

                    if (pivotCompare < 0) {
                        if (prevIsOuter) edge._face._outerEdge = edge._next;
                        edge._face.AddInnerEdge(edge._previous);
                    } else {
                        Debug.Assert(pivotCompare > 0);
                        if (nextIsOuter) edge._face._outerEdge = edge._previous;
                        edge._face.AddInnerEdge(edge._next);
                    }
                }
            }

            _edges.Remove(edgeKey);
            _edges.Remove(twin._key);

            _connectivity = 0;
            return true;
        }

        #endregion
        #region RemoveVertex

        /// <summary>
        /// Removes the specified vertex by joining both incident edges.</summary>
        /// <param name="index">
        /// The zero-based index of the <see cref="Vertices"/> element to remove.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Vertices"/> element at <paramref name="index"/> and both
        /// incident <see cref="Edges"/> were removed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than the number of <see
        /// cref="Vertices"/>.</para></exception>
        /// <remarks><para>
        /// <b>RemoveVertex</b> first checks whether the <see cref="Vertices"/> element at the
        /// specified <paramref name="index"/> contains exactly two incident <see cref="Edges"/>, or
        /// whether joining them would disturb vertex chains or create an intersection with any
        /// non-incident half-edge on any boundary of the incident <see cref="SubdivisionFace"/>.
        /// </para><para>
        /// If so, <b>RemoveVertex</b> returns <c>false</c>. Otherwise, <b>RemoveVertex</b> links
        /// the twins of the incident <see cref="Edges"/> into a new half-edge pair, updates the
        /// <see cref="Vertices"/> and <see cref="Edges"/> collections, and returns <c>true</c>.
        /// </para></remarks>

        public bool RemoveVertex(int index) {

            // check for exactly two incident edges
            SubdivisionEdge edge1 = _vertices.GetByIndex(index), twin1 = edge1._twin;
            SubdivisionEdge edge2 = twin1._next, twin2 = edge2._twin;
            if (edge1 == edge2 || edge1._previous != twin2)
                return false;

            // check for existing connecting edge
            foreach (SubdivisionEdge edge in twin1.OriginEdges)
                if (edge._twin._origin == twin2._origin)
                    return false;

            // check joined edge position in vertex chains
            if (!twin1.IsCompatibleDestination(twin2._origin) ||
                !twin2.IsCompatibleDestination(twin1._origin))
                return false;

            // check for other edges intersecting joined edge
            double length = twin2._origin.CrossProductLength(edge1._origin, twin1._origin);
            if (Math.Abs(length) > _epsilon) {
                SubdivisionFace face = (length < 0) ? edge2._face : edge1._face;
                LineD line = new LineD(twin2._origin, twin1._origin);
                foreach (SubdivisionEdge edge in face.AllCycleEdges)
                    if (edge != edge1 && edge != twin1 && edge != edge2 && edge != twin2) {
                        var result = line.Intersect(edge.ToLine(), _epsilon);
                        if (result.ExistsBetween) return false;
                    }
            }

            edge1._face.MoveEdge(edge1, twin2);
            edge2._face.MoveEdge(edge2, twin1);

            // connect twins of incident edges
            twin1._twin = twin2;
            twin1._next = edge2._next;
            edge2._next._previous = twin1;

            twin2._twin = twin1;
            twin2._next = edge1._next;
            edge1._next._previous = twin2;

            // remove vertex and incident edges
            _edges.Remove(edge1._key);
            _edges.Remove(edge2._key);
            _vertices.RemoveAt(index);

            if (_connectivity == 2) _connectivity = 0;
            return true;
        }

        #endregion
        #region RenumberEdges

        /// <summary>
        /// Renumbers all <see cref="Edges"/> so that each <see cref="SubdivisionEdge.Key"/> equals
        /// the index position of the corresponding element.</summary>
        /// <returns>
        /// <c>true</c> if any <see cref="SubdivisionEdge.Key"/> was changed; otherwise,
        /// <c>false</c>.</returns>
        /// <remarks><para>
        /// Deleting <see cref="Edges"/> from an existing <see cref="Subdivision"/> may leave gaps
        /// in the <see cref="SubdivisionEdge.Key"/> sequence. <b>RenumberEdges</b> eliminates any
        /// such gaps, restoring the original equivalence of <see cref="SubdivisionEdge.Key"/> and
        /// index position in the <see cref="Edges"/> collection.
        /// </para><para>
        /// <b>RenumberEdges</b> does not change the sequence of <see cref="SubdivisionEdge"/>
        /// values stored in the <see cref="Edges"/> collection, only the <see cref="Int32"/> keys
        /// and the corresponding <see cref="SubdivisionEdge.Key"/> properties.</para></remarks>

        public bool RenumberEdges() {
            if (_edges.Count == _nextEdgeKey)
                return false;

            var edges = new SubdivisionEdge[_edges.Count];
            for (int i = 0; i < _edges.Count; i++) {
                SubdivisionEdge edge = _edges.GetByIndex(i);
                edges[i] = edge;
                edge._key = i;
            }

            _edges.Clear();
            foreach (SubdivisionEdge edge in edges)
                _edges.Add(edge._key, edge);

            _nextEdgeKey = edges.Length;
            return true;
        }

        #endregion
        #region RenumberFaces

        /// <summary>
        /// Renumbers all <see cref="Faces"/> so that each <see cref="SubdivisionFace.Key"/> equals
        /// the index position of the corresponding element.</summary>
        /// <returns>
        /// <c>true</c> if any <see cref="SubdivisionFace.Key"/> was changed; otherwise,
        /// <c>false</c>.</returns>
        /// <remarks><para>
        /// Deleting <see cref="Edges"/> from an existing <see cref="Subdivision"/> may leave gaps
        /// in the <see cref="SubdivisionFace.Key"/> sequence of the <see cref="Faces"/> collection.
        /// <b>RenumberFaces</b> eliminates any such gaps, restoring the original equivalence of
        /// <see cref="SubdivisionFace.Key"/> and index position.
        /// </para><para>
        /// <b>RenumberFaces</b> does not change the sequence of <see cref="SubdivisionFace"/>
        /// values stored in the <see cref="Faces"/> collection, only the <see cref="Int32"/> keys
        /// and the corresponding <see cref="SubdivisionFace.Key"/> properties. Note that this does
        /// invalidate any associated <see cref="ISubdivisionMap{T}"/> instances.</para></remarks>

        public bool RenumberFaces() {
            if (_faces.Count == _nextFaceKey)
                return false;

            var faces = new SubdivisionFace[_faces.Count];
            for (int i = 0; i < _faces.Count; i++) {
                SubdivisionFace face = _faces.GetByIndex(i);
                faces[i] = face;
                face._key = i;
            }

            _faces.Clear();
            foreach (SubdivisionFace face in faces)
                _faces.Add(face._key, face);

            _nextFaceKey = faces.Length;
            return true;
        }

        #endregion
        #region SplitEdge

        /// <summary>
        /// Splits the specified edge in half.</summary>
        /// <param name="edgeKey">
        /// The <see cref="SubdivisionEdge.Key"/> of one <see cref="SubdivisionEdge"/> to split. Its
        /// <see cref="SubdivisionEdge.Twin"/> is split as well.</param>
        /// <returns>
        /// On success, one of the two new <see cref="Edges"/> elements that originate from the new
        /// <see cref="Vertices"/> element; otherwise, a null reference.</returns>
        /// <remarks><para>
        /// <b>SplitEdge</b> returns a null reference if the specified <paramref name="edgeKey"/>
        /// was not found in the <see cref="Edges"/> collection, or if the new vertex would equal an
        /// existing <see cref="Vertices"/> element, given the current <see cref="Epsilon"/>.
        /// </para><para>
        /// Otherwise, <b>SplitEdge</b> creates a new <see cref="Vertices"/> element in the center
        /// of the split <see cref="SubdivisionEdge"/>, and two new <see cref="Edges"/> that
        /// originate from the new vertex. Each is paired with one of the original half-edge twins,
        /// effectively shortening them to end at the new vertex.</para></remarks>

        public SubdivisionEdge SplitEdge(int edgeKey) {
            SubdivisionEdge edge;
            if (!_edges.TryGetValue(edgeKey, out edge))
                return null;

            PointD a = edge._origin, b = edge._twin._origin;
            PointD vertex = new PointD((a.X + b.X) / 2, (a.Y + b.Y) / 2);
            if (_vertices.ContainsKey(vertex))
                return null;

            if (_connectivity < 2) _connectivity = 2;
            return SplitEdgeAtVertex(edge, vertex, -1);
        }

        #endregion
        #region StructureEquals

        /// <summary>
        /// Determines whether this instance and a specified <see cref="Subdivision"/> have the same
        /// structure.</summary>
        /// <param name="division">
        /// A <see cref="Subdivision"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the structure of <paramref name="division"/> is the same as this
        /// instance; otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>StructureEquals</b> compares the number, order, and internal structure of all <see
        /// cref="SubdivisionEdge"/> and <see cref="SubdivisionFace"/> objects in the two <see
        /// cref="Subdivision"/> instances to test for structural equality. Individual objects are
        /// compared using their own <b>StructureEquals</b> methods.
        /// </para><para>
        /// <b>StructureEquals</b> is intended for testing the <see cref="Subdivision.Clone"/>
        /// method which replicates <see cref="SubdivisionEdge"/> keys but not references.
        /// </para></remarks>

        public bool StructureEquals(Subdivision division) {

            // compare internal counters
            if (_nextEdgeKey != division._nextEdgeKey) return false;
            if (_nextFaceKey != division._nextFaceKey) return false;

            // compare collection counts
            if (_faces.Count != division._faces.Count) return false;
            if (_vertices.Count != division._vertices.Count) return false;

            // compare contents of vertex collection
            for (int i = 0; i < _vertices.Count; i++) {
                var edge = _vertices.GetByIndex(i);
                var otherEdge = division._vertices.GetByIndex(i);
                if (!edge.StructureEquals(otherEdge)) return false;
            }

            // compare contents of face collection
            for (int i = 0; i < _faces.Count; i++) {
                var face = _faces.GetByIndex(i);
                var otherFace = division._faces.GetByIndex(i);
                if (!face.StructureEquals(otherFace)) return false;
            }

            return true;
        }

        #endregion
        #region ToLines

        /// <summary>
        /// Converts all edges in the <see cref="Subdivision"/> to line segments.</summary>
        /// <returns>
        /// An <see cref="Array"/> of <see cref="LineD"/> instances that represent all <see
        /// cref="Edges"/> in the <see cref="Subdivision"/>.</returns>
        /// <remarks><para>
        /// The <see cref="LineD"/> elements of the returned <see cref="Array"/> are sorted by the
        /// smaller <see cref="SubdivisionEdge.Key"/> of the two <see cref="SubdivisionEdge"/>
        /// objects that constitute each edge, using the same ordering as the <see cref="Edges"/>
        /// collection.
        /// </para><para>
        /// The returned <see cref="Array"/> has half as many elements as the <see cref="Edges"/>
        /// collection since each <see cref="LineD"/> comprises two half-edges.</para></remarks>

        public LineD[] ToLines() {
            LineD[] lines = new LineD[_edges.Count / 2];
            int lineIndex = 0;

            // iterate over half-edges by ascending keys
            for (int i = 0; i < _edges.Count; i++) {
                SubdivisionEdge edge = _edges.GetByIndex(i);
                SubdivisionEdge twin = edge._twin;

                // add full edge when encountering first twin only;
                // that is, when current key is less than twin key
                if (edge._key < twin._key)
                    lines[lineIndex++] = new LineD(edge._origin, twin._origin);
            }

            Debug.Assert(lineIndex == lines.Length);
            return lines;
        }

        #endregion
        #region ToPolygons

        /// <summary>
        /// Converts the outer boundaries of all bounded <see cref="Faces"/> in the <see
        /// cref="Subdivision"/> to a list of polygons.</summary>
        /// <returns>
        /// An <see cref="Array"/> containing one nested <see cref="Array"/> of <see cref="PointD"/>
        /// instances for each bounded <see cref="Faces"/> element.</returns>
        /// <remarks><para>
        /// Each nested <see cref="PointD"/> array within the returned outer <see cref="Array"/>
        /// contains the <see cref="SubdivisionEdge.CyclePolygon"/> for the <see
        /// cref="SubdivisionFace.OuterEdge"/> of one bounded <see cref="Faces"/> element.
        /// </para><para>
        /// The <see cref="PointD"/> arrays are sorted by the <see cref="SubdivisionFace.Key"/> of
        /// the corresponding <see cref="SubdivisionFace"/>, using the same ordering as the <see
        /// cref="Faces"/> collection but excluding the unbounded face.</para></remarks>

        public PointD[][] ToPolygons() {
            var polygons = new PointD[_faces.Count - 1][];

            for (int i = 1; i < _faces.Count; i++) {
                SubdivisionFace face = _faces.GetByIndex(i);
                polygons[i - 1] = face._outerEdge.CyclePolygon;
            }

            return polygons;
        }

        #endregion
        #region Validate

        /// <summary>
        /// Validates the structure of the <see cref="Subdivision"/>.</summary>
        /// <exception cref="AssertionException">
        /// The structure of the <see cref="Subdivision"/> is invalid.</exception>
        /// <remarks>
        /// <b>Validate</b> performs a series of <see cref="ThrowHelper.Assert"/> calls that verify
        /// all structural invariants of the <see cref="Subdivision"/>.</remarks>

        public void Validate() {
            bool isEmpty = (_edges.Count == 0);

            // check vertex comparer epsilon against cached value
            ThrowHelper.Assert(((PointDComparerY) _vertices.Comparer).Epsilon == _epsilon);

            // check number of edges, vertices & faces
            ThrowHelper.Assert(_edges.Count % 2 == 0);
            ThrowHelper.Assert(
                (isEmpty && _vertices.Count == 0) ||
                (!isEmpty && _vertices.Count >= 2));
            ThrowHelper.Assert(_faces.Count > 0);
            ThrowHelper.Assert(_faces.GetByIndex(0)._key == 0);

            // check mandatory unbounded face
            SubdivisionFace faceZero = _faces.GetByIndex(0);
            ThrowHelper.Assert(faceZero._outerEdge == null);
            ThrowHelper.Assert(
                (isEmpty && faceZero._innerEdges == null) ||
                (!isEmpty && faceZero._innerEdges != null));
            ThrowHelper.Assert(faceZero._innerEdges == null || faceZero._innerEdges.Count > 0);

            // check incident faces and next/previous chains
            foreach (var pair in _edges) {
                SubdivisionEdge edge = pair.Value;
                ThrowHelper.Assert(pair.Key == edge._key);
                ThrowHelper.Assert(edge._face != null);
                ThrowHelper.Assert(edge._face.Owner == this);
                ThrowHelper.Assert(edge._twin._twin == edge);
                ThrowHelper.Assert(edge._next._previous == edge);
                ThrowHelper.Assert(edge._previous._next == edge);
            }

            // check incident edges and vertex chains
            foreach (var pair in _vertices) {
                SubdivisionEdge edge = pair.Value;
                do {
                    ThrowHelper.Assert(edge._origin == pair.Key);
                    edge = edge._twin._next;
                } while (edge != pair.Value);

                do {
                    ThrowHelper.Assert(edge._origin == pair.Key);
                    edge = edge._previous._twin;
                } while (edge != pair.Value);
            }

            foreach (var pair in _faces) {
                SubdivisionFace face = pair.Value;
                ThrowHelper.Assert(pair.Key == face._key);
                ThrowHelper.Assert(face.Owner == this);

                // check incident faces of outer cycle
                if (face._outerEdge != null) {
                    SubdivisionEdge edge = face._outerEdge;
                    do {
                        ThrowHelper.Assert(edge._face == face);
                        edge = edge._next;
                    } while (edge != face._outerEdge);

                    do {
                        ThrowHelper.Assert(edge._face == face);
                        edge = edge._previous;
                    } while (edge != face._outerEdge);
                }

                // check incident faces of all inner cycles
                if (face._innerEdges != null) {
                    foreach (SubdivisionEdge innerEdge in face._innerEdges) {
                        SubdivisionEdge edge = innerEdge;
                        do {
                            ThrowHelper.Assert(edge._face == face);
                            edge = edge._next;
                        } while (edge != innerEdge);

                        do {
                            ThrowHelper.Assert(edge._face == face);
                            edge = edge._previous;
                        } while (edge != innerEdge);
                    }
                }
            }
        }

        #endregion
        #endregion
        #region ICloneable Members
        #region Clone

        /// <summary>
        /// Creates a deep copy of the <see cref="Subdivision"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="Subdivision"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> replicates the entire structure of the <see cref="Subdivision"/>, creating
        /// a new <see cref="SubdivisionEdge"/> and <see cref="SubdivisionFace"/> instance for each
        /// corresponding instance found in the current structure. The new instances always have the
        /// same <b>Key</b> as the existing instances.</remarks>

        public object Clone() {
            var division = new Subdivision(_edges.Count, _faces.Count, _vertices.Count);

            // set epsilon on vertex comparer
            division.Epsilon = _epsilon;

            // copy internal counters
            division._connectivity = _connectivity;
            division._nextEdgeKey = _nextEdgeKey;
            division._nextFaceKey = _nextFaceKey;

            // copy edges with key and origin
            foreach (SubdivisionEdge oldEdge in _edges.Values) {
                var newEdge = new SubdivisionEdge(oldEdge._key);
                division._edges.Add(newEdge._key, newEdge);
                newEdge._origin = oldEdge._origin;
            }

            // copy vertices with new edge references
            foreach (SubdivisionEdge oldEdge in _vertices.Values)
                division._vertices.Add(oldEdge._origin, division._edges[oldEdge._key]);

            // copy unbounded face with new edge references
            SubdivisionFace oldFace = _faces.GetByIndex(0);
            if (oldFace._innerEdges != null) {
                SubdivisionFace newFace = division._faces.GetByIndex(0);
                newFace._innerEdges = new ListEx<SubdivisionEdge>(oldFace._innerEdges.Count);
                foreach (var oldEdge in oldFace._innerEdges)
                    newFace._innerEdges.Add(division._edges[oldEdge._key]);
            }

            // copy bounded faces with new edge references
            for (int i = 1; i < _faces.Count; i++) {
                oldFace = _faces.GetByIndex(i);
                var newFace = new SubdivisionFace(division, oldFace._key);
                division._faces.Add(newFace._key, newFace);

                newFace._outerEdge = division._edges[oldFace._outerEdge._key];
                if (oldFace._innerEdges != null) {
                    newFace._innerEdges = new ListEx<SubdivisionEdge>(oldFace._innerEdges.Count);
                    foreach (var oldEdge in oldFace._innerEdges)
                        newFace._innerEdges.Add(division._edges[oldEdge._key]);
                }
            }

            // update edge & face references of all edges
            foreach (SubdivisionEdge newEdge in division._edges.Values) {
                var oldEdge = _edges[newEdge._key];

                newEdge._face = division._faces[oldEdge._face._key];
                newEdge._twin = division._edges[oldEdge._twin._key];
                newEdge._next = division._edges[oldEdge._next._key];
                newEdge._previous = division._edges[oldEdge._previous._key];
            }

            return division;
        }

        #endregion
        #region CloneEdges

        /// <summary>
        /// Creates a deep copy of the <see cref="Subdivision"/>, except for the <see cref="Faces"/>
        /// collection.</summary>
        /// <returns>
        /// A deep copy of the <see cref="Subdivision"/>, except for the <see cref="Faces"/>
        /// collection.</returns>
        /// <remarks><para>
        /// <b>CloneEdges</b> replicates the entire structure of the <see cref="Subdivision"/>,
        /// creating a new <see cref="SubdivisionEdge"/> instance for each corresponding instance
        /// found in the current structure. The new instances always have the same <see
        /// cref="SubdivisionEdge.Key"/> as the existing instances.
        /// </para><para>
        /// Unlike <see cref="Clone"/>, the <see cref="Faces"/> collection and all related
        /// references are <em>not</em> copied. The returned <see cref="Subdivision"/> contains only
        /// the unbounded <see cref="SubdivisionFace"/>.</para></remarks>

        private Subdivision CloneEdges() {
            var division = new Subdivision(_edges.Count, _faces.Count, _vertices.Count);

            // set epsilon on vertex comparer
            division.Epsilon = _epsilon;

            // copy internal counters
            division._connectivity = _connectivity;
            division._nextEdgeKey = _nextEdgeKey;

            // copy edges with key and origin
            foreach (SubdivisionEdge oldEdge in _edges.Values) {
                var newEdge = new SubdivisionEdge(oldEdge._key);
                division._edges.Add(newEdge._key, newEdge);
                newEdge._origin = oldEdge._origin;
            }

            // copy vertices with new edge references
            foreach (SubdivisionEdge oldEdge in _vertices.Values)
                division._vertices.Add(oldEdge._origin, division._edges[oldEdge._key]);

            // update edge references of all edges
            foreach (SubdivisionEdge newEdge in division._edges.Values) {
                var oldEdge = _edges[newEdge._key];

                newEdge._twin = division._edges[oldEdge._twin._key];
                newEdge._next = division._edges[oldEdge._next._key];
                newEdge._previous = division._edges[oldEdge._previous._key];
            }

            return division;
        }

        #endregion
        #endregion
        #region IGraph2D<T> Members
        #region Connectivity

        /// <summary>
        /// Gets the maximum number of direct neighbors for any <see cref="IGraph2D{T}"/> node.
        /// </summary>
        /// <value>
        /// A positive <see cref="Int32"/> value indicating the maximum number of direct neighbors
        /// for any given <see cref="IGraph2D{T}"/> node.</value>
        /// <remarks><para>
        /// <b>Connectivity</b> returns the maximum number of half-edges that originate from any
        /// single <see cref="Vertices"/> element.
        /// </para><para>
        /// <b>Connectivity</b> scans the entire <see cref="Vertices"/> collection to determine its
        /// value on first access, and returns a cached value on subsequent accesses. The scan is
        /// repeated whenever the structure of the <see cref="Subdivision"/> changes.
        /// </para></remarks>

        public int Connectivity {
            get {
                if (_connectivity == 0 && _vertices.Count > 0) {
                    foreach (SubdivisionEdge firstEdge in _vertices.Values) {
                        int count = 0;

                        // count half-edges at current vertex
                        var edge = firstEdge;
                        do {
                            ++count;
                            edge = edge._twin._next;
                        } while (edge != firstEdge);

                        if (_connectivity < count)
                            _connectivity = count;
                    }
                }

                return _connectivity;
            }
        }

        #endregion
        #region NodeCount

        /// <summary>
        /// Gets the total number of <see cref="Nodes"/> in the <see cref="IGraph2D{T}"/>.</summary>
        /// <value>
        /// The total number of <see cref="Nodes"/> in the <see cref="IGraph2D{T}"/>.</value>
        /// <remarks>
        /// <b>NodeCount</b> returns the current number of <see cref="Vertices"/>.</remarks>

        public int NodeCount {
            get { return _vertices.Count; }
        }

        #endregion
        #region Nodes

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> collection that contains all nodes in the <see
        /// cref="IGraph2D{T}"/>.</summary>
        /// <value>
        /// An <see cref="IEnumerable{T}"/> collection that contains all nodes in the <see
        /// cref="IGraph2D{T}"/>.</value>
        /// <remarks>
        /// <b>Nodes</b> returns all <see cref="PointD"/> keys in the <see cref="Vertices"/>
        /// collection, using its current sorting order.</remarks>

        public IEnumerable<PointD> Nodes {
            get {
                foreach (PointD vertex in _vertices.Keys)
                    yield return vertex;
            }
        }

        #endregion
        #region Contains

        /// <summary>
        /// Determines whether the <see cref="IGraph2D{T}"/> contains the specified node.</summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="IGraph2D{T}"/> contains the specified <paramref
        /// name="node"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> returns <c>true</c> exactly if the <see cref="Vertices"/> collection
        /// contains the specified <paramref name="node"/>. This is an O(ld n) operation, where n is
        /// the number of <see cref="Vertices"/>.</remarks>

        public bool Contains(PointD node) {
            return _vertices.ContainsKey(node);
        }

        #endregion
        #region GetDistance

        /// <summary>
        /// Returns the distance between the two specified <see cref="IGraph2D{T}"/> nodes.
        /// </summary>
        /// <param name="source">
        /// The source node in the <see cref="IGraph2D{T}"/>.</param>
        /// <param name="target">
        /// The target node in the <see cref="IGraph2D{T}"/>.</param>
        /// <returns>
        /// The non-negative distance between <paramref name="source"/> and <paramref
        /// name="target"/>, measured in world coordinates.</returns>
        /// <remarks><para>
        /// <b>GetDistance</b> returns zero if the specified <paramref name="source"/> and <paramref
        /// name="target"/> are identical, and the Euclidean distance between <paramref
        /// name="source"/> and <paramref name="target"/> otherwise. This is equivalent to the
        /// absolute length of the edge that connects the two vertices.
        /// </para><para>
        /// <b>GetDistance</b> does not check whether the <see cref="Subdivision"/> actually
        /// contains the specified <paramref name="source"/> and <paramref name="target"/> nodes, or
        /// whether they are connected by an edge.</para></remarks>

        public double GetDistance(PointD source, PointD target) {
            if (source == target) return 0;

            double x = target.X - source.X;
            double y = target.Y - source.Y;

            return Math.Sqrt(x * x + y * y);
        }

        #endregion
        #region GetNearestNode

        /// <summary>
        /// Gets the <see cref="IGraph2D{T}"/> node that is nearest to the specified location, in
        /// world coordinates.</summary>
        /// <param name="location">
        /// The <see cref="PointD"/> location, in world coordinates, whose nearest <see
        /// cref="IGraph2D{T}"/> node to find.</param>
        /// <returns>
        /// The <see cref="IGraph2D{T}"/> node whose <see cref="GetWorldLocation"/> result is
        /// nearest to the specified <paramref name="location"/>.</returns>
        /// <remarks>
        /// <b>GetNearestNode</b> returns the <see cref="Vertices"/> element whose index is found by
        /// <see cref="FindNearestVertex"/> for the specified <paramref name="location"/>.</remarks>

        public PointD GetNearestNode(PointD location) {
            int index = FindNearestVertex(location);
            return _vertices.GetKey(index);
        }

        #endregion
        #region GetNeighbors

        /// <summary>
        /// Returns all direct neighbors of the specified <see cref="IGraph2D{T}"/> node.</summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node whose direct neighbors to return.</param>
        /// <returns>
        /// An <see cref="IList{T}"/> containing all valid <see cref="IGraph2D{T}"/> nodes that are
        /// directly connected with the specified <paramref name="node"/>. The number of elements is
        /// at most <see cref="Connectivity"/>.</returns>
        /// <remarks><para>
        /// <b>GetNeighbors</b> never returns a null reference, but it returns an empty <see
        /// cref="IList{T}"/> if the specified <paramref name="node"/> is not found in the <see
        /// cref="Vertices"/> collections.
        /// </para><para>
        /// Otherwise, <b>GetNeighbors</b> returns the destinations of all half-edges that originate
        /// from the specified <paramref name="node"/>. This is an O(ld n + m) operation, where n is
        /// the total number of <see cref="Vertices"/> and m is the number of half-edges originating
        /// from <paramref name="node"/>.</para></remarks>

        public IList<PointD> GetNeighbors(PointD node) {
            var neighbors = new List<PointD>();

            // find origin in vertex collection
            SubdivisionEdge firstEdge;
            if (!_vertices.TryGetValue(node, out firstEdge))
                return new List<PointD>(0);

            // find all destinations from origin
            var edge = firstEdge;
            do {
                var twin = edge._twin;
                neighbors.Add(twin._origin);
                edge = twin._next;
            } while (edge != firstEdge);

            return neighbors;
        }

        #endregion
        #region GetWorldLocation

        /// <summary>
        /// Gets the location of the specified <see cref="IGraph2D{T}"/> node, in world coordinates.
        /// </summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node whose location to return.</param>
        /// <returns>
        /// The <see cref="PointD"/> location of the specified <paramref name="node"/>, in world
        /// coordinates.</returns>
        /// <remarks>
        /// <b>GetWorldLocation</b> simply returns the specified <paramref name="node"/>, without
        /// checking whether it is actually part of the <see cref="Subdivision"/>.</remarks>

        public PointD GetWorldLocation(PointD node) {
            return node;
        }

        #endregion
        #region GetWorldRegion

        /// <summary>
        /// Gets the region covered by the specified <see cref="IGraph2D{T}"/> node, in world
        /// coordinates.</summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node whose region to return.</param>
        /// <returns><para>
        /// An <see cref="Array"/> containing the <see cref="PointD"/> vertices of the polygonal
        /// region covered by the specified <paramref name="node"/>, in world coordinates.
        /// </para><para>-or-</para><para>
        /// A null reference if <paramref name="node"/> does not define a polygonal region.
        /// </para></returns>
        /// <remarks>
        /// <b>GetWorldRegion</b> returns the polygonal region that <see cref="VertexRegions"/>
        /// associates with the specified <paramref name="node"/>, if found; otherwise, a null
        /// reference.</remarks>

        public PointD[] GetWorldRegion(PointD node) {
            PointD[] region;
            _vertexRegions.TryGetValue(node, out region);
            return region;
        }

        #endregion
        #endregion
        #region Private Methods
        #region CompareEdges

        /// <summary>
        /// Compares two specified <see cref="SubdivisionEdge"/> objects and returns an indication
        /// of their sweep line ordering.</summary>
        /// <param name="a">
        /// The first <see cref="SubdivisionEdge"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="SubdivisionEdge"/> to compare.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term><description>
        /// <paramref name="a"/> is sorted before <paramref name="b"/>.</description>
        /// </item><item>
        /// <term>Zero</term><description>
        /// <paramref name="a"/> and <paramref name="b"/> are equal.</description>
        /// </item><item>
        /// <term>Greater than zero</term><description>
        /// <paramref name="a"/> is sorted after <paramref name="b"/>.</description>
        /// </item></list></returns>
        /// <remarks>
        /// <b>CompareEdges</b> is used by the plane sweep algorithm performed by the <see
        /// cref="FindCycles"/> method.</remarks>

        private int CompareEdges(SubdivisionEdge a, SubdivisionEdge b) {
            if (a == b) return 0;

            // compute inverse slopes of intersecting half-edges
            LineD aLine = a.ToLine(), bLine = b.ToLine();
            double aSlope = aLine.InverseSlope, bSlope = bLine.InverseSlope;
            double ax, bx;

            /*
             * Compute x-coordinates of sweep line intersections.
             * 
             * We default horizontal edges to their destination, which is their left point
             * since we always use downward-pointing half-edges.
             * 
             * This means that a horizontal edge will get sorted AFTER any other edge touching
             * its destination (due to the maximum slope) and BEFORE any other edge touching its
             * origin (because the origin is lexicographically greater than the destination).
             * 
             * If an edge touches the sweep line with its origin, it extends below the sweep line.
             * We must then negate its slope for the possible follow-up comparison, so that the
             * sorting order remains consistent with earlier intersections below the sweep line.
             * 
             * We need no special treatment for computed sweep line intersection points, as in
             * the case of multiline intersection, because we know that no two edges intersect
             * or overlap except at their end points.
             */

            if (_cursorY == aLine.End.Y || aSlope == Double.MaxValue)
                ax = aLine.End.X;
            else if (_cursorY == aLine.Start.Y) {
                ax = aLine.Start.X;
                aSlope = -aSlope;
            } else
                ax = aLine.Start.X + (_cursorY - aLine.Start.Y) * aSlope;

            if (_cursorY == bLine.End.Y || bSlope == Double.MaxValue)
                bx = bLine.End.X;
            else if (_cursorY == bLine.Start.Y) {
                bx = bLine.Start.X;
                bSlope = -bSlope;
            } else
                bx = bLine.Start.X + (_cursorY - bLine.Start.Y) * bSlope;

            // sort on sweep line intersection if different
            if (ax < bx) return -1;
            if (ax > bx) return +1;

            // else sort on slope above sweep line
            if (aSlope < bSlope) return -1;
            if (aSlope > bSlope) return +1;

            return (a._key - b._key);
        }

        #endregion
        #region CreateAllFromLines

        /// <summary>
        /// Initializes the <see cref="Edges"/>, <see cref="Faces"/>, and <see cref="Vertices"/>
        /// collections from the specified line segments.</summary>
        /// <param name="lines">
        /// An <see cref="Array"/> of <see cref="LineD"/> instances that represent the <see
        /// cref="Edges"/> added to the <see cref="Subdivision"/>.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="lines"/> contains an element whose <see cref="LineD.Start"/> point
        /// equals its <see cref="LineD.End"/> point.</exception>
        /// <remarks>
        /// <b>CreateAllFromLines</b> requires that the <see cref="Edges"/> and <see
        /// cref="Vertices"/> collections are empty, and that the <see cref="Faces"/> collection
        /// contains only the unbounded face. They will be filled with the data extracted from the
        /// specified <paramref name="lines"/> when the method returns.</remarks>

        private void CreateAllFromLines(LineD[] lines) {

            Debug.Assert(_edges.Count == 0);
            Debug.Assert(_faces.Count == 1);
            Debug.Assert(_vertices.Count == 0);

            // convert all lines into half-edges
            SubdivisionEdge edge;
            foreach (LineD line in lines)
                CreateTwinEdges(line.Start, line.End, out edge);

            // find all inner and outer cycles
            var cycles = FindCycles();

            // convert cycles into face records
            CreateFacesFromCycles(cycles.Item1, cycles.Item2);
        }

        #endregion
        #region CreateAllFromPolygons

        /// <summary>
        /// Initializes the <see cref="Edges"/>, <see cref="Faces"/>, and <see cref="Vertices"/>
        /// collections from the specified polygons.</summary>
        /// <param name="polygons">
        /// An <see cref="IList{T}"/> of <see cref="PointD"/> arrays that represent the outer
        /// boundaries of all bounded faces added to the <see cref="Subdivision"/>.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="polygons"/> contains an <see cref="Array"/> that is a null reference or
        /// contains less than three elements or two consecutive identical elements.</exception>
        /// <remarks>
        /// <b>CreateAllFromPolygons</b> requires that the <see cref="Edges"/> and <see
        /// cref="Vertices"/> collections are empty, and that the <see cref="Faces"/> collection
        /// contains only the unbounded face. They will be filled with the data extracted from the
        /// specified <paramref name="polygons"/> when the method returns.</remarks>

        private void CreateAllFromPolygons(IList<PointD[]> polygons) {

            Debug.Assert(_edges.Count == 0);
            Debug.Assert(_faces.Count == 1);
            Debug.Assert(_vertices.Count == 0);

            // convert all polygon edges into half-edges
            SubdivisionEdge edge;
            for (int i = 0; i < polygons.Count; i++) {
                var polygon = polygons[i];
                for (int j = 0; j < polygon.Length; j++) {
                    PointD start = polygon[j], end = polygon[(j + 1) % polygon.Length];
                    CreateTwinEdges(start, end, out edge);
                }
            }

            // find all inner and outer cycles
            var cycles = FindCycles();

            // convert cycles into face records
            CreateFacesFromCycles(cycles.Item1, cycles.Item2);
        }

        #endregion
        #region CreateFacesFromCycles

        /// <summary>
        /// Initializes the <see cref="Faces"/> collection from the specified <see
        /// cref="EdgeCycle"/> collections.</summary>
        /// <param name="innerCycles">
        /// A <see cref="List{T}"/> containing all <see cref="EdgeCycle"/> lists that begin with
        /// inner cycles, and are therefore directly contained in the unbounded face.</param>
        /// <param name="outerCycles">
        /// A <see cref="List{T}"/> containing all <see cref="EdgeCycle"/> lists that begin with
        /// outer cycles, representing bounded faces which contain all subsequent inner cycles
        /// within the same <see cref="EdgeCycle"/> list.</param>
        /// <remarks>
        /// <b>CreateFacesFromCycles</b> requires that the <see cref="Faces"/> collection contains
        /// only the unbounded face.</remarks>

        private void CreateFacesFromCycles(
            List<EdgeCycle> innerCycles, List<EdgeCycle> outerCycles) {
            /*
             * The unbounded face receives all inner cycles that aren’t linked
             * to any outer cycles. Those inner cycles may still be linked to
             * other inner cycles, however, so we must traverse their Next chains.
             */

            Debug.Assert(_faces.Count == 1);
            SubdivisionFace face = _faces.GetByIndex(0);

            // add any unlinked inner cycles to unbounded face
            for (int i = 0; i < innerCycles.Count; i++)
                for (var cycle = innerCycles[i]; cycle != null; cycle = cycle.Next)
                    cycle.AddToFace(face, false);

            /*
             * Create zero or more bounded faces, one for each outer cycle.
             * Each bounded face also receives any inner cycles which are linked
             * to that outer cycle along the chain of Next references.
             */

            for (int i = 0; i < outerCycles.Count; i++) {
                var cycle = outerCycles[i];

                // create bounded face for outer cycle
                face = new SubdivisionFace(this, _nextFaceKey++);
                _faces.Add(face._key, face);
                cycle.AddToFace(face, true);

                // add any linked inner cycles to bounded face
                for (cycle = cycle.Next; cycle != null; cycle = cycle.Next)
                    cycle.AddToFace(face, false);
            }
        }

        #endregion
        #region CreateTwinEdges

        /// <summary>
        /// Finds or creates both twin half-edges between the specified <see cref="PointD"/>
        /// coordinates.</summary>
        /// <param name="start">
        /// The <see cref="SubdivisionEdge.Origin"/> of the first half-edge, and the <see
        /// cref="SubdivisionEdge.Destination"/> of the second half-edge.</param>
        /// <param name="end">
        /// The <see cref="SubdivisionEdge.Origin"/> of the second half-edge, and the <see
        /// cref="SubdivisionEdge.Destination"/> of the first half-edge.</param>
        /// <param name="startEdge">
        /// Returns the <see cref="SubdivisionEdge"/> from <paramref name="start"/> to <paramref
        /// name="end"/>.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="startEdge"/> is a newly created <see
        /// cref="SubdivisionEdge"/>; <c>false</c> if <paramref name="startEdge"/> is an existing
        /// <see cref="Edges"/> element.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="start"/> equals <paramref name="end"/>.</exception>
        /// <remarks><para>
        /// <b>CreateTwinEdges</b> adds the specified <paramref name="start"/> and <paramref
        /// name="end"/> coordinates to the <see cref="Vertices"/> collection if they are not
        /// already present. Otherwise, the <see cref="SubdivisionEdge.Origin"/> of a newly created
        /// half-edge is set to the corresponding <see cref="Vertices"/> element, which may differ
        /// from the specified <paramref name="start"/> or <paramref name="end"/> coordinates if the
        /// current comparison <see cref="Epsilon"/> is positive.
        /// </para><para>
        /// <b>CreateTwinEdges</b> does not change the <see cref="Subdivision"/> if the <see
        /// cref="Edges"/> collection already contains twin half-edges between <paramref
        /// name="start"/> and <paramref name="end"/>.</para></remarks>

        private bool CreateTwinEdges(PointD start, PointD end, out SubdivisionEdge startEdge) {
            if (start == end)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "lines", Strings.ArgumentContainsEmpty, "LineD");

            // search for vertices at start/end coordinates
            SubdivisionEdge oldStartEdge, oldEndEdge;
            _vertices.TryGetValue(start, out oldStartEdge);
            _vertices.TryGetValue(end, out oldEndEdge);

            // both vertices exist: check if half-edges also exist
            if (oldStartEdge != null && oldEndEdge != null) {
                startEdge = oldStartEdge;
                do {
                    if (startEdge._twin._origin == oldEndEdge._origin)
                        return false;

                    startEdge = startEdge._twin._next;
                } while (startEdge != oldStartEdge);
            }

            // create twin edges for line segment
            startEdge = new SubdivisionEdge(_nextEdgeKey++);
            _edges.Add(startEdge._key, startEdge);

            SubdivisionEdge endEdge = new SubdivisionEdge(_nextEdgeKey++);
            _edges.Add(endEdge._key, endEdge);

            // initialize Twin pointers (unchanged in this method)
            startEdge._twin = endEdge; endEdge._twin = startEdge;

            // initialize Next and Previous pointers
            // (may change if edges are linked to existing vertices)
            startEdge._next = startEdge._previous = endEdge;
            endEdge._next = endEdge._previous = startEdge;

            /*
             * Vertices might be performing searches with a non-zero epsilon, which means that
             * start/end might not exactly equal the vertex that is reported as identical.
             * Therefore, if a vertex already exists, we must assign the existing coordinates
             * to the half-edge’s Origin, and not the specified start/end coordinates.
             */

            // create or expand origin of start half-edge
            if (oldStartEdge == null) {
                startEdge._origin = start;
                _vertices.Add(start, startEdge);
            } else
                startEdge._origin = oldStartEdge._origin;

            // create or expand origin of end half-edge
            if (oldEndEdge == null) {
                endEdge._origin = end;
                _vertices.Add(end, endEdge);
            } else
                endEdge._origin = oldEndEdge._origin;

            // create links only after both Origins are set
            if (oldStartEdge != null) InsertAtEdge(startEdge, oldStartEdge);
            if (oldEndEdge != null) InsertAtEdge(endEdge, oldEndEdge);

            return true;
        }

        #endregion
        #region FindCycles

        /// <summary>
        /// Finds all inner and outer cycles formed by the half-edges in the <see
        /// cref="Subdivision"/>.</summary>
        /// <returns><para>
        /// A <see cref="ValueTuple{T1, T2}"/> containing the following collections:
        /// </para><para>
        /// <see cref="ValueTuple{T1, T2}.Item1"/> is a <see cref="List{T}"/> of <see
        /// cref="EdgeCycle"/> objects representing all unlinked inner cycles which are contained by
        /// the unbounded face.
        /// </para><para>
        /// <see cref="ValueTuple{T1, T2}.Item2"/> is a <see cref="List{T}"/> of <see
        /// cref="EdgeCycle"/> objects representing all outer cycles of bounded faces.
        /// </para></returns>

        private ValueTuple<List<EdgeCycle>, List<EdgeCycle>> FindCycles() {
            var vertexComparer = (PointDComparerY) _vertices.Comparer;

            // edge cycles containing all half-edges
            var edgeToCycle = new Int32Dictionary<EdgeCycle>(_edges.Count);

            // lexicographically smallest vertices of all inner cycles
            var pivotToInnerCycle = new Dictionary<PointD, EdgeCycle>(_edges.Count / 4);

            // output list containing all outer cycles
            var outerCycles = new List<EdgeCycle>(_edges.Count / 4);

            // one half-edge from each twin pair in a cycle
            Int32HashSet cycleTwinEdges = new Int32HashSet();

            // queue all half-edges for examination
            var edgeQueue = new Int32Dictionary<SubdivisionEdge>(_edges);
            while (edgeQueue.Count > 0) {

                // take arbitrary half-edge to start a cycle
                SubdivisionEdge edge = edgeQueue.GetAnyValue();
                SubdivisionEdge pivot = edge;
                EdgeCycle cycle = new EdgeCycle(edge);

                // prepare to check for zero-area cycles
                cycleTwinEdges.Clear();
                int cycleTwinCount = 0;

                do {
                    // map each half-edge to its containing cycle
                    edgeToCycle.Add(edge._key, cycle);

                    // count full twin pairs contained in the cycle
                    if (cycleTwinEdges.Contains(edge._twin._key))
                        ++cycleTwinCount;
                    else
                        cycleTwinEdges.Add(edge._key);

                    // continue cycle and determine lower-left vertex
                    edgeQueue.Remove(edge._key);
                    edge = edge._next;
                    if (vertexComparer.Compare(pivot._origin, edge._origin) > 0)
                        pivot = edge;

                } while (edge != cycle.FirstEdge);

                /*
                 * If the cycle contains only full twin pairs, it encloses no area at all.
                 * Therefore, all half-edges bound the same face, forming an inner cycle.
                 * Otherwise, the cycle does enclose an area and we must continue testing.
                 * 
                 * Compute cross-product length (CPL) of the pivot vertex (lower-left corner)
                 * with the previous and next vertex within the same cycle. This description
                 * assumes mathematical coordinates, i.e. y-coordinates increase upward.
                 * 
                 * Collinear (CPL = 0) means inner cycle, as the twin half-edges at the pivot
                 * vertex form a zero-area protrusion into a face on the outside of the cycle.
                 * 
                 * Right turn (CPL < 0) means inner cycle, as the incident faces are always
                 * to the left of the half-edges, and therefore on the outside of a right turn.
                 * 
                 * Left turn (CPL > 0) means outer cycle, as the incident faces are always
                 * to the left of the half-edges, and therefore on the inside of a left turn.
                 */

                bool isInnerCycle;
                if (cycleTwinCount == cycleTwinEdges.Count)
                    isInnerCycle = true;
                else {
                    double length = pivot._previous._origin.
                        CrossProductLength(pivot._origin, pivot._next._origin);
                    isInnerCycle = (length <= 0);
                }

                /*
                 * Add outer cycles directly to output list, but store inner cycles in a
                 * lookup table with their pivot vertex. They might get linked to other
                 * (inner or outer) cycles rather than directly to the unbounded face.
                 */

                if (isInnerCycle)
                    pivotToInnerCycle.Add(pivot._origin, cycle);
                else
                    outerCycles.Add(cycle);
            }

            // output list containing all unlinked inner cycles
            var innerCycles = new List<EdgeCycle>(pivotToInnerCycle.Count);
            int innerCyclesFound = 0;

            // current horizontal sweep line, moving upward (TValue is unused)
            var sweepLine = new BraidedTree<SubdivisionEdge, Boolean>(CompareEdges);

            foreach (SubdivisionEdge firstEdge in _vertices.Values) {
                _cursorY = firstEdge._origin.Y;

                /*
                 * Check if we are on the pivot vertex of an inner cycle, and if so,
                 * if another edge is directly to its left within the current sweep line.
                 * 
                 * Since all edges in the sweep line point downward, and each edge’s face
                 * points to the left relative to its direction, that edge’s face is either
                 * the outer cycle bounding the inner cycle or a neighboring inner cycle.
                 * 
                 * If there is such an edge, we link both cycles; otherwise, the inner cycle
                 * belongs to the unbounded face and is added to the general output list.
                 * 
                 * We can use any arbitrary edge that originates with the current vertex
                 * to query the sweep line, so we just use the first one. While we may find
                 * a previous node, we will never find an exact match since the pivot vertex
                 * is defined as the lexicographically smallest vertex of all attached edges,
                 * so none of these edges have been added to the sweep line yet.
                 */

                EdgeCycle innerCycle;
                if (pivotToInnerCycle.TryGetValue(firstEdge._origin, out innerCycle)) {

                    var leftNode = sweepLine.FindNodeOrPrevious(firstEdge._twin);
                    if (leftNode != sweepLine.RootNode) {
                        EdgeCycle leftCycle = edgeToCycle[leftNode.Key._key];
                        innerCycle.Next = leftCycle.Next;
                        leftCycle.Next = innerCycle;
                    } else
                        innerCycles.Add(innerCycle);

                    // we’re done if all pivot vertices were processed
                    if (++innerCyclesFound == pivotToInnerCycle.Count)
                        break;
                }

                /*
                 * Sweepline ordering is easier if all edges point in the same direction.
                 * Choose the twin half-edge that points downward, so that its face points
                 * toward the pivot vertex of any inner cycle to its right (see above).
                 * 
                 * The origin of a downward-pointing half-edge is its lexicographically
                 * greater point (= towards the upper-right), and the destination is its
                 * lexicographically smaller point (= towards the lower-left).
                 * 
                 * The sweep line moves upward (= increasing y-coordinates), which means:
                 * 
                 * 1. When we find an edge’s destination, we must have encountered that edge
                 *    for the first time and therefore need to add it to the sweep line.
                 * 
                 * 2. When we find an edge’s origin, we must have already added that edge at
                 *    its destination, and therefore need to remove it from the sweep line.
                 */

                SubdivisionEdge edge = firstEdge;
                do {
                    int direction = vertexComparer.Compare(edge._origin, edge._twin._origin);
                    Debug.Assert(direction != 0);
                    SubdivisionEdge downEdge = (direction < 0 ? edge._twin : edge);

                    if (downEdge._origin == firstEdge._origin) {
                        Debug.Assert(sweepLine.ContainsKey(downEdge));
                        sweepLine.Remove(downEdge);
                    } else {
                        Debug.Assert(downEdge._twin._origin == firstEdge._origin);
                        sweepLine.Add(downEdge, false);
                    }
                    edge = edge._twin._next;

                } while (edge != firstEdge);
            }

            Debug.Assert(innerCyclesFound == pivotToInnerCycle.Count);
            Debug.Assert(innerCycles.Count > 0);

            return ValueTuple.Create(innerCycles, outerCycles);
        }

        #endregion
        #region InsertAtEdge

        /// <summary>
        /// Inserts the specified <see cref="SubdivisionEdge"/> into the vertex chain that contains
        /// another specified instance.</summary>
        /// <param name="edge">
        /// The <see cref="SubdivisionEdge"/> to insert into the same vertex chain that contains
        /// <paramref name="oldEdge"/>.</param>
        /// <param name="oldEdge">
        /// A <see cref="SubdivisionEdge"/> that is already linked into the vertex chain around its
        /// <see cref="SubdivisionEdge.Origin"/>.</param>
        /// <remarks>
        /// <b>InsertAtEdge</b> requires that <paramref name="edge"/> and <paramref name="oldEdge"/>
        /// both have the same <see cref="SubdivisionEdge.Origin"/>.</remarks>

        private static void InsertAtEdge(SubdivisionEdge edge, SubdivisionEdge oldEdge ) {
            Debug.Assert(edge != oldEdge);
            Debug.Assert(edge._origin == oldEdge._origin);

            if (oldEdge._previous == oldEdge._twin) {
                // simple case: no other edge linked to old edge
                edge._previous = oldEdge._twin;
                edge._twin._next = oldEdge;
            } else {
                // find position of edge in existing vertex chain
                SubdivisionEdge nextEdge, previousEdge;
                oldEdge.FindEdgePosition(edge._twin._origin, out nextEdge, out previousEdge);
                edge._previous = previousEdge._twin;
                edge._twin._next = nextEdge;
            }

            // establish double-link invariants
            edge._previous._next = edge;
            edge._twin._next._previous = edge._twin;
        }

        #endregion
        #region InsertAtOrigin

        /// <summary>
        /// Inserts the specified <see cref="SubdivisionEdge"/> into the vertex chain around its
        /// <see cref="SubdivisionEdge.Origin"/>.</summary>
        /// <param name="edge">
        /// The <see cref="SubdivisionEdge"/> to insert into the vertex chain around its <see
        /// cref="SubdivisionEdge.Origin"/>.</param>
        /// <remarks>
        /// <b>InsertAtOrigin</b> also adds the <see cref="SubdivisionEdge.Origin"/> of <paramref
        /// name="edge"/> to the <see cref="Vertices"/> collection if not already present.</remarks>

        private void InsertAtOrigin(SubdivisionEdge edge) {

            // add new vertex with edge pair if not present
            int index = _vertices.IndexOfKey(edge._origin);
            if (index < 0) {
                _vertices.Add(edge._origin, edge);
                edge._previous = edge._twin;
                edge._twin._next = edge;
                return;
            }

            // get incident edge at existing vertex
            SubdivisionEdge oldEdge = _vertices.GetByIndex(index);

            // check if new edge already inserted
            SubdivisionEdge cursor = oldEdge;
            do {
                if (cursor == edge) return;
                cursor = cursor._twin._next;
            } while (cursor != oldEdge);

            // not found, insert edge as usual
            InsertAtEdge(edge, oldEdge);
        }

        #endregion
        #region IntersectEdges

        /// <summary>
        /// Intersects the <see cref="Edges"/> of this instance with those of the specified <see
        /// cref="Subdivision"/>.</summary>
        /// <param name="division">
        /// The <see cref="Subdivision"/> whose <see cref="Edges"/> to intersect with this instance.
        /// </param>
        /// <param name="edgeToFace1">
        /// An <see cref="Int32Dictionary{T}"/> that maps the keys of any existing <see
        /// cref="Edges"/> to the keys of the incident bounded <see cref="Faces"/> of the
        /// corresponding <see cref="Edges"/> in some pre-existing <see cref="Subdivision"/>.
        /// </param>
        /// <param name="edgeToFace2">
        /// An empty <see cref="Int32Dictionary{T}"/> that will, on return, map the keys of any
        /// existing and created <see cref="Edges"/> to the keys of the incident bounded <see
        /// cref="Faces"/> of the corresponding <see cref="Edges"/> in the intersecting <paramref
        /// name="division"/>.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="division"/> contains invalid <see cref="Edges"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="Subdivision"/> contains invalid <see cref="Edges"/>.</exception>
        /// <remarks><para>
        /// The <see cref="Epsilon"/> of the specified <paramref name="division"/> cannot be smaller
        /// than that of the current instance. See <see cref="Intersection"/> for details.
        /// </para><para>
        /// <b>IntersectEdges</b> adds new elements to the specified <paramref name="edgeToFace1"/>
        /// collection if any existing <see cref="Edges"/> are split by the intersection. All new
        /// elements will contain the same <see cref="SubdivisionFace"/> key as the element that
        /// refers to the split <see cref="SubdivisionEdge"/>.</para></remarks>

        private void IntersectEdges(Subdivision division, 
            Int32Dictionary<Int32> edgeToFace1, Int32Dictionary<Int32> edgeToFace2) {

            Debug.Assert(_epsilon <= division._epsilon);
            Debug.Assert(edgeToFace2.Count == 0);

            // minimum epsilon for edge intersection algorithm
            double epsilon = Math.Max(_epsilon, 1e-10);

            // store one twin out of each half-edge pair
            var edge1List = new List<SubdivisionEdge>(_edges.Count);
            foreach (SubdivisionEdge edge1 in _edges.Values)
                if (edge1._key < edge1._twin._key) edge1List.Add(edge1);

            /*
             * Intersect all firstEdges with all secondEdges, testing one half-edge per pair.
             * 
             * If any edge is split, we add one new half-edge to the corresponding collection for
             * future comparisons, some of which are redundant.
             * 
             * New first-instance edges are added to the first-instance list. New second-instance
             * edges are added to a temporary stack which is emptied before we move on to the next
             * original second-instance edge.
             * 
             * Any duplicate (congruent) edges are skipped in the second instance only. We never
             * completely delete any pre-existing first-instance edges, although they may be
             * shortened.
             * 
             * The current second-instance edge is immediately added to the first instance’s Edges
             * collection. If it turns out to be redundant during an edge split or origin move, it
             * is automatically removed again. Any newly created edges during a split are added
             * automatically to the Edges collection as well.
             */

            Stack<SubdivisionEdge> edge2Stack = new Stack<SubdivisionEdge>();
            int edge2Index = 0;
            while (true) {
                SubdivisionEdge edge2;

                if (edge2Stack.Count > 0) {
                    // process any newly split edges first
                    edge2 = edge2Stack.Pop();
                } else {
                    // we’re done if all second-instance edges are processed
                    if (edge2Index == division._edges.Count)
                        break;

                    // fetch one twin from each half-edge pair
                    SubdivisionEdge edge = division._edges.GetByIndex(edge2Index++);
                    if (edge._key > edge._twin._key) continue;

                    // recreate edge or find existing first-instance edge
                    bool created = CreateTwinEdges(edge._origin, edge._twin._origin, out edge2);

                    // store non-zero incident faces for face mapping
                    int face = edge._face._key;
                    if (face != 0) edgeToFace2.Add(edge2._key, face);
                    face = edge._twin._face._key;
                    if (face != 0) edgeToFace2.Add(edge2._twin._key, face);

                    // skip edge if duplicated by existing first-instance edge
                    if (!created) continue;
                }

                // compare all first-instance edges against current second-instance edge
                for (int edge1Index = 0; edge1Index < edge1List.Count; edge1Index++) {
                    SubdivisionEdge edge1 = edge1List[edge1Index];

                    // intersect first-instance and second-instance edge
                    LineIntersection crossing =  LineIntersection.Find(
                        edge1._origin, edge1._twin._origin,
                        edge2._origin, edge2._twin._origin, epsilon);

                    if (!crossing.Exists) continue;
                    SplitEdgeResult split;

                    if (crossing.Relation == LineRelation.Collinear) {
                        /*
                         * The various cases for overlapping collinear edges assume that both edges
                         * are equidirectional. We must ensure this condition using our intersection
                         * epsilon to handle infinitesimal y-differences in the wrong direction.
                         */
                        if (PointDComparerY.CompareEpsilon(
                            edge1._origin, edge1._twin._origin, epsilon) > 0)
                            edge1 = edge1._twin;

                        if (PointDComparerY.CompareEpsilon(
                            edge2._origin, edge2._twin._origin, epsilon) > 0)
                            edge2 = edge2._twin;

                        // relative locations of edge2’s origin & destination
                        var edge2Start = LineIntersection.LocateCollinear(
                            edge1._origin, edge1._twin._origin, edge2._origin, epsilon);

                        var edge2End = LineIntersection.LocateCollinear(
                            edge1._origin, edge1._twin._origin, edge2._twin._origin, epsilon);

                        switch (edge2Start) {
                        case LineLocation.Before:
                            switch (edge2End) {

                            case LineLocation.Before:
                                goto invalidResult;

                            case LineLocation.Start:
                                // edges already linked, nothing to do
                                continue;

                            case LineLocation.Between:
                                // edge1     --------->
                                // edge2 --------->
                                split = TrySplitEdge(edge1, edge2._twin._origin);
                                split.UpdateFaces(edge1, edgeToFace1, edgeToFace2);

                                if (split.CreatedEdge != null)
                                    edge1List.Add(split.CreatedEdge);
                                else if (split.IsEdgeDeleted)
                                    goto invalidSplit;

                                if (MoveOriginToEdge(edge2._twin, split.OriginEdge) != null)
                                    goto skipCurrentEdge2;
                                continue;

                            case LineLocation.End:
                                // edge1       ----->
                                // edge2 ----------->
                                if (MoveOriginToEdge(edge2._twin, edge1) != null)
                                    goto skipCurrentEdge2;
                                continue;

                            case LineLocation.After:
                                // edge1      ----->
                                // edge2 --------------->
                                split = TrySplitEdge(edge2, edge1._origin);
                                split.UpdateFaces(edge2, edgeToFace1, edgeToFace2);

                                if (split.CreatedEdge != null)
                                    edge2Stack.Push(split.CreatedEdge);
                                else if (split.IsEdgeDeleted)
                                    goto invalidSplit;

                                if (MoveOriginToEdge(split.DestinationEdge, edge1._twin) != null) {
                                    if (split.DestinationEdge == edge2)
                                        goto skipCurrentEdge2;
                                    else if (split.DestinationEdge == split.CreatedEdge)
                                        edge2Stack.Pop();
                                }
                                continue;
                            }
                            break;

                        case LineLocation.Start:
                            switch (edge2End) {

                            case LineLocation.Before:
                                goto invalidResult;

                            case LineLocation.Start:
                                goto invalidArgument;

                            case LineLocation.Between:
                                // edge1 ----------->
                                // edge2 ----->
                                if (MoveOriginToEdge(edge1, edge2._twin) != null)
                                    goto invalidMove;
                                continue;

                            case LineLocation.End:
                                // exactly congruent edges
                                goto invalidResult;

                            case LineLocation.After:
                                // edge1 ----->
                                // edge2 ----------->
                                if (MoveOriginToEdge(edge2, edge1._twin) != null)
                                    goto skipCurrentEdge2;
                                continue;
                            }
                            break;

                        case LineLocation.Between:
                            switch (edge2End) {

                            case LineLocation.Before:
                            case LineLocation.Start:
                                goto invalidResult;

                            case LineLocation.Between:
                                // edge1 --------------->
                                // edge2      ----->
                                split = TrySplitEdge(edge1, edge2._origin);
                                split.UpdateFaces(edge1, edgeToFace1, edgeToFace2);

                                if (split.CreatedEdge != null)
                                    edge1List.Add(split.CreatedEdge);
                                else if (split.IsEdgeDeleted)
                                    goto invalidSplit;

                                if (MoveOriginToEdge(split.DestinationEdge, edge2._twin) != null)
                                    goto invalidMove;
                                continue;

                            case LineLocation.End:
                                // edge1 ----------->
                                // edge2       ----->
                                if (MoveOriginToEdge(edge1._twin, edge2) != null)
                                    goto invalidMove;
                                continue;

                            case LineLocation.After:
                                // edge1 --------->
                                // edge2     --------->
                                split = TrySplitEdge(edge1, edge2._origin);
                                split.UpdateFaces(edge1, edgeToFace1, edgeToFace2);

                                if (split.CreatedEdge != null)
                                    edge1List.Add(split.CreatedEdge);
                                else if (split.IsEdgeDeleted)
                                    goto invalidSplit;

                                if (MoveOriginToEdge(edge2, split.DestinationEdge._twin) != null)
                                    goto skipCurrentEdge2;
                                continue;
                            }
                            break;

                        case LineLocation.End:
                            switch (edge2End) {

                            case LineLocation.Before:
                            case LineLocation.Start:
                            case LineLocation.Between:
                                goto invalidResult;

                            case LineLocation.End:
                                goto invalidArgument;

                            case LineLocation.After:
                                // edges already linked, nothing to do
                                continue;
                            }
                            break;

                        case LineLocation.After:
                            goto invalidResult;
                        }

                    invalidResult:
                        /*
                         * Both collinear edges point in opposite directions, which is impossible
                         * since they are sorted by lexicographically smaller origins; or else they
                         * are exactly congruent, which is also impossible since they were created
                         * by CreateTwinEdges which never creates duplicate edges.
                         */
                        ThrowHelper.ThrowInvalidOperationExceptionWithFormat(
                            Strings.MethodInvalidValue, "LocateCollinear");

                    invalidArgument:
                        // second instance contains zero-length edge
                        ThrowHelper.ThrowArgumentExceptionWithFormat("division",
                            Strings.ArgumentContainsEmpty, "SubdivisionEdge");

                    invalidMove:
                        // origin move detected overlapping first-division edges
                        ThrowHelper.ThrowInvalidOperationExceptionWithFormat(
                            Strings.MethodInvalidValue, "MoveOriginToEdge");

                    } else {
                        Debug.Assert(crossing.Relation == LineRelation.Divergent);
                        /*
                         * Divergent intersecting edges: We need only consider the case where either
                         * or both edges cross between their origin and destination, as the case
                         * with two edges touching at a common existing vertex is already covered by
                         * the vertex chain insertion performed by CreateTwinEdges.
                         */

                        if (crossing.First == LineLocation.Between) {
                            switch (crossing.Second) {

                            case LineLocation.Start:
                                split = TrySplitEdge(edge1, edge2._origin);
                                split.UpdateFaces(edge1, edgeToFace1, edgeToFace2);

                                if (split.CreatedEdge != null)
                                    edge1List.Add(split.CreatedEdge);
                                else if (split.IsEdgeDeleted)
                                    goto invalidSplit;
                                continue;

                            case LineLocation.End:
                                split = TrySplitEdge(edge1, edge2._twin._origin);
                                split.UpdateFaces(edge1, edgeToFace1, edgeToFace2);

                                if (split.CreatedEdge != null)
                                    edge1List.Add(split.CreatedEdge);
                                else if (split.IsEdgeDeleted)
                                    goto invalidSplit;
                                continue;

                            case LineLocation.Between:
                                split = TrySplitEdge(edge1, crossing.Shared.Value);
                                split.UpdateFaces(edge1, edgeToFace1, edgeToFace2);

                                if (split.CreatedEdge != null)
                                    edge1List.Add(split.CreatedEdge);
                                else if (split.IsEdgeDeleted)
                                    goto invalidSplit;

                                split = TrySplitEdge(edge2, split.DestinationEdge._origin);
                                split.UpdateFaces(edge2, edgeToFace1, edgeToFace2);

                                if (split.CreatedEdge != null)
                                    edge2Stack.Push(split.CreatedEdge);
                                else if (split.IsEdgeDeleted)
                                    goto skipCurrentEdge2;
                                continue;
                            }

                        } else if (crossing.Second == LineLocation.Between) {
                            switch (crossing.First) {

                            case LineLocation.Start:
                                split = TrySplitEdge(edge2, edge1._origin);
                                split.UpdateFaces(edge2, edgeToFace1, edgeToFace2);

                                if (split.CreatedEdge != null)
                                    edge2Stack.Push(split.CreatedEdge);
                                else if (split.IsEdgeDeleted)
                                    goto skipCurrentEdge2;
                                continue;

                            case LineLocation.End:
                                split = TrySplitEdge(edge2, edge1._twin._origin);
                                split.UpdateFaces(edge2, edgeToFace1, edgeToFace2);

                                if (split.CreatedEdge != null)
                                    edge2Stack.Push(split.CreatedEdge);
                                else if (split.IsEdgeDeleted)
                                    goto skipCurrentEdge2;
                                continue;
                            }
                        }

                        continue;
                    }

                invalidSplit:
                    // edge split detected overlapping first-division edges
                    ThrowHelper.ThrowInvalidOperationExceptionWithFormat(
                        Strings.MethodInvalidValue, "TrySplitEdge");
                }

            skipCurrentEdge2:
                continue;
            }
        }

        #endregion
        #region MoveOriginToEdge

        /// <summary>
        /// Moves the specified <see cref="SubdivisionEdge"/> to the vertex chain that contains
        /// another specified instance.</summary>
        /// <param name="edge">
        /// The <see cref="SubdivisionEdge"/> to move from its current vertex chain to that which
        /// contains <paramref name="oldEdge"/>.</param>
        /// <param name="oldEdge">
        /// A <see cref="SubdivisionEdge"/> that is already linked into the vertex chain around its
        /// <see cref="SubdivisionEdge.Origin"/>.</param>
        /// <returns><para>
        /// A null reference if <paramref name="edge"/> was successfully moved to the vertex chain
        /// that contains <paramref name="oldEdge"/>.
        /// </para><para>-or-</para><para>
        /// An existing <see cref="SubdivisionEdge"/> already linked into the vertex chain that
        /// contains <paramref name="oldEdge"/> whose <see cref="SubdivisionEdge.Destination"/>
        /// equals that of <paramref name="edge"/>.</para></returns>
        /// <remarks><para>
        /// <b>MoveOriginToEdge</b> changes the <see cref="SubdivisionEdge.Origin"/> of the
        /// specified <paramref name="edge"/>, and also updates all incident links on its old and
        /// new <see cref="SubdivisionEdge.Origin"/>.
        /// </para><para>
        /// <b>MoveOriginToEdge</b> deletes <paramref name="edge"/> and its <see
        /// cref="SubdivisionEdge.Twin"/> from the <see cref="Edges"/> collection if an existing
        /// element connects its <see cref="SubdivisionEdge.Destination"/> and the <see
        /// cref="SubdivisionEdge.Origin"/> of <paramref name="oldEdge"/>.</para></remarks>

        private SubdivisionEdge MoveOriginToEdge(SubdivisionEdge edge, SubdivisionEdge oldEdge) {

            RemoveAtOrigin(edge);
            SubdivisionEdge twin = edge._twin;

            // check for existing edge between vertices
            SubdivisionEdge cursor = oldEdge;
            do {
                if (cursor._twin._origin == twin._origin) {
                    RemoveAtOrigin(twin);
                    _edges.Remove(edge._key);
                    _edges.Remove(twin._key);
                    return cursor;
                }
                cursor = cursor._twin._next;
            } while (cursor != oldEdge);

            // re-insert edge at new origin
            edge._origin = oldEdge._origin;
            InsertAtEdge(edge, oldEdge);
            return null;
        }

        #endregion
        #region RemoveAtOrigin

        /// <summary>
        /// Removes the specified <see cref="SubdivisionEdge"/> from the vertex chain around its
        /// <see cref="SubdivisionEdge.Origin"/>.</summary>
        /// <param name="edge">
        /// The <see cref="SubdivisionEdge"/> to remove from the vertex chain around its <see
        /// cref="SubdivisionEdge.Origin"/>.</param>
        /// <remarks>
        /// <b>RemoveAtOrigin</b> also removes the <see cref="SubdivisionEdge.Origin"/> of the
        /// specified <paramref name="edge"/> from the <see cref="Vertices"/> collection if there
        /// are no other incident half-edges, and otherwise changes the incident half-edge in the
        /// <see cref="Vertices"/> collection if it equals <paramref name="edge"/>.</remarks>

        private void RemoveAtOrigin(SubdivisionEdge edge) {

            SubdivisionEdge twin = edge._twin;
            int index = _vertices.IndexOfKey(edge._origin);
            Debug.Assert(index >= 0);

            // remove vertex entirely if no other edges
            if (edge._previous == twin) {
                _vertices.RemoveAt(index);
                return;
            }

            // remove half-edge from vertex chain
            edge._previous._next = twin._next;
            twin._next._previous = edge._previous;

            // update incident half-edge if necessary
            if (_vertices.GetByIndex(index) == edge)
                _vertices.SetByIndex(index, edge._twin._next);
        }

        #endregion
        #region SplitEdgeAtVertex

        /// <summary>
        /// Splits the specified edge in two parts around the specified vertex.</summary>
        /// <param name="edge">
        /// One <see cref="SubdivisionEdge"/> to split. Its <see cref="SubdivisionEdge.Twin"/> is
        /// split as well.</param>
        /// <param name="vertex">
        /// The vertex around which to split <paramref name="edge"/>.</param>
        /// <param name="index">
        /// The zero-based index of <paramref name="vertex"/> in the <see cref="Vertices"/>
        /// collection, or a negative value to add <paramref name="vertex"/> to the collection.
        /// </param>
        /// <returns>
        /// One of the two new <see cref="Edges"/> elements that originate from <paramref
        /// name="vertex"/>.</returns>

        private SubdivisionEdge SplitEdgeAtVertex(SubdivisionEdge edge, PointD vertex, int index) {
            SubdivisionEdge twin = edge._twin;

            SubdivisionEdge newEdge = new SubdivisionEdge(_nextEdgeKey++);
            _edges.Add(newEdge._key, newEdge);

            newEdge._origin = vertex;
            newEdge._face = edge._face;
            newEdge._twin = twin; twin._twin = newEdge;
            newEdge._next = edge._next;
            newEdge._next._previous = newEdge;

            SubdivisionEdge newTwin = new SubdivisionEdge(_nextEdgeKey++);
            _edges.Add(newTwin._key, newTwin);

            newTwin._origin = vertex;
            newTwin._face = twin._face;
            newTwin._twin = edge; edge._twin = newTwin;
            newTwin._next = twin._next;
            twin._next._previous = newTwin;

            if (index < 0) {
                _vertices.Add(vertex, newEdge);
                edge._next = newEdge;
                newEdge._previous = edge;
                twin._next = newTwin;
                newTwin._previous = twin;
            } else {
                newEdge._previous = twin;
                twin._next = newEdge;
                newTwin._previous = edge;
                edge._next = newTwin;
                InsertAtOrigin(newEdge);
                InsertAtOrigin(newTwin);
            }

            return newEdge;
        }

        #endregion
        #region TrySplitEdge

        /// <summary>
        /// Attempts to split the specified edge in two parts around the specified vertex.</summary>
        /// <param name="edge">
        /// One <see cref="SubdivisionEdge"/> to split. Its <see cref="SubdivisionEdge.Twin"/> is
        /// split as well.</param>
        /// <param name="vertex">
        /// The vertex around which to split <paramref name="edge"/>.</param>
        /// <returns>
        /// A <see cref="SplitEdgeResult"/> instance containing the result of the operation.
        /// </returns>
        /// <remarks>
        /// <b>TrySplitEdge</b> adds the specified <paramref name="vertex"/> to the <see
        /// cref="Vertices"/> collection if not already present.</remarks>

        private SplitEdgeResult TrySplitEdge(SubdivisionEdge edge, PointD vertex) {

            SubdivisionEdge twin = edge._twin;
            Debug.Assert(vertex != edge._origin);
            Debug.Assert(vertex != twin._origin);
            SubdivisionEdge originEdge = null, destinationEdge = null;

            // check for existing edges between vertex and end points
            int index = _vertices.IndexOfKey(vertex);
            if (index >= 0) {
                SubdivisionEdge incidentEdge = _vertices.GetByIndex(index);
                SubdivisionEdge cursor = incidentEdge;
                do {
                    PointD origin = cursor._twin._origin;

                    if (origin == edge._origin) {
                        originEdge = cursor._twin;
                        if (destinationEdge != null) break;
                    }
                    else if (origin == twin._origin) {
                        destinationEdge = cursor;
                        if (originEdge != null) break;
                    }

                    cursor = cursor._twin._next;
                } while (cursor != incidentEdge);

                // set vertex to existing coordinates
                vertex = incidentEdge._origin;
            }

            // no connecting edges exist: create two new half-edges
            if (originEdge == null && destinationEdge == null) {
                SubdivisionEdge newEdge = SplitEdgeAtVertex(edge, vertex, index);
                return new SplitEdgeResult(edge, newEdge, newEdge, false);
            }

            // both connecting edges exist: delete edge to be split
            if (originEdge != null && destinationEdge != null) {
                RemoveAtOrigin(edge);
                RemoveAtOrigin(twin);
                _edges.Remove(edge._key);
                _edges.Remove(twin._key);
                return new SplitEdgeResult(originEdge, destinationEdge, null, true);
            }

            // one connecting edge exist: shorten edge for other part
            if (originEdge != null) {
                RemoveAtOrigin(edge);
                edge._origin = vertex;
                InsertAtEdge(edge, originEdge._twin);
                return new SplitEdgeResult(originEdge, edge, null, false);
            }
            else {
                Debug.Assert(destinationEdge != null);
                RemoveAtOrigin(edge._twin);
                edge._twin._origin = vertex;
                InsertAtEdge(edge._twin, destinationEdge);
                return new SplitEdgeResult(edge, destinationEdge, null, false);
            }
        }

        #endregion
        #endregion
        #region Class EdgeCycle

        /// <summary>
        /// Represents a cycle of half-edges that represents a <see cref="SubdivisionFace"/>.
        /// </summary>
        /// <remarks>
        /// There is one <b>EdgeCycle</b> for each chain of <see cref="SubdivisionEdge.Next"/>
        /// references that forms the inner or outer boundary of a single <see
        /// cref="SubdivisionFace"/>. Multiple <b>EdgeCycle</b> instances may also be linked to
        /// indicate one or more "holes" within the same outer boundary.</remarks>

        private class EdgeCycle {
            #region EdgeCycle(SubdivisionEdge)

            /// <overloads>
            /// Initializes a new instance of the <see cref="EdgeCycle"/> class.</overloads>
            /// <summary>
            /// Initializes a new instance of the <see cref="EdgeCycle"/> class with the specified
            /// incident half-edge.</summary>
            /// <param name="edge">
            /// A <see cref="SubdivisionEdge"/> that is part of the <see cref="EdgeCycle"/>.</param>

            public EdgeCycle(SubdivisionEdge edge) {
                Debug.Assert(edge != null);
                FirstEdge = edge;
            }

            #endregion
            #region FirstEdge

            /// <summary>
            /// A <see cref="SubdivisionEdge"/> that is part of the <see cref="EdgeCycle"/>.
            /// </summary>
            /// <remarks>
            /// Follow the chain of <see cref="SubdivisionEdge.Next"/> references starting with
            /// <b>FirstEdge</b> to visit the other half-edges in the <see cref="EdgeCycle"/>.
            /// </remarks>

            public readonly SubdivisionEdge FirstEdge;

            #endregion
            #region Next

            /// <summary>
            /// Another <see cref="EdgeCycle"/> that is an inner cycle, and either contained within
            /// or neighboring the current instance.</summary>
            /// <remarks><para>
            /// <b>Next</b> is either a null reference or an inner cycle. If the current instance is
            /// an outer cycle, <b>Next</b> is a "hole" contained within that outer cycle.
            /// </para><para>
            /// Otherwise, the current instance and <b>Next</b> are both neighboring "holes", either
            /// within the same outer cycle that begins the chain of <b>Next</b> references, or else
            /// within the unbounded face.</para></remarks>

            public EdgeCycle Next;

            #endregion
            #region AddToFace

            /// <summary>
            /// Adds the data of the <see cref="EdgeCycle"/> to the specified <see
            /// cref="SubdivisionFace"/>.</summary>
            /// <param name="face">
            /// The <see cref="SubdivisionFace"/> that receives the data of the <see
            /// cref="EdgeCycle"/>.</param>
            /// <param name="isOuter">
            /// <c>true</c> if the <see cref="EdgeCycle"/> represents an outer boundary;
            /// <c>false</c> if it represents an inner boundary.</param>
            /// <remarks>
            /// <b>AddToFace</b> updates the half-edge pointer of the specified <paramref
            /// name="face"/> that is indicated by <paramref name="isOuter"/>, and the face pointers
            /// of all half-edges in the chain that starts with <see cref="FirstEdge"/>.</remarks>

            public void AddToFace(SubdivisionFace face, bool isOuter) {

                // set edge pointer of face
                if (isOuter) {
                    Debug.Assert(face._outerEdge == null);
                    face._outerEdge = FirstEdge;
                } else
                    face.AddInnerEdge(FirstEdge);

                // set face pointers of all incident edges
                var edge = FirstEdge;
                do {
                    Debug.Assert(edge._face == null);
                    edge._face = face;
                    edge = edge._next;
                } while (edge != FirstEdge);
            }

            #endregion
        }

        #endregion
    }
}
