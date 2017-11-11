using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using Tektosyne.Collections;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Provides a fast search structure for a planar <see cref="Subdivision"/></summary>
    /// <remarks><para>
    /// <b>SubdivisionSearch</b> creates a search structure that achieves a query time of O(log n)
    /// for point location in a planar <see cref="Subdivision"/> with n full edges. However, the
    /// search structure itself occupies O(n) fairly big objects which require O(n log n) steps to
    /// construct, and must be recreated whenever the underlying <see cref="Subdivision"/> changes.
    /// </para><para>
    /// Moreover, <b>SubdivisionSearch</b> requires a minimum epsilon of 1e-10 for coordinate
    /// comparisons to reliably construct its search structure, and must use the construction
    /// epsilon for all point location queries. Use the brute force <see cref="Subdivision.Find"/>
    /// algorithm to avoid construction costs and/or perform searches with a different epsilon.
    /// </para><para>
    /// The algorithm to incrementally construct a search structure for the trapezoidal map of a
    /// planar subdivision was adapted from Mark de Berg et al., <em>Computational Geometry</em>
    /// (3rd ed.), Springer-Verlag 2008, p.122-137. This implementation uses null half-edges and
    /// <see cref="Double.MaxValue"/> to indicate the unbounded face, rather than placing an actual
    /// bounding rectangle around the <see cref="Subdivision"/>.</para></remarks>

    [Serializable]
    public class SubdivisionSearch {
        #region SubdivisionSearch(Subdivision, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdivisionSearch"/> class with the
        /// specified <see cref="Subdivision"/>.</summary>
        /// <param name="source">
        /// The <see cref="Subdivision"/> to search.</param>
        /// <param name="ordered">
        /// <c>true</c> to insert the <see cref="Subdivision.Edges"/> of <paramref name="source"/>
        /// in their original order; <c>false</c> to use a random permutation of that order. The
        /// default is <c>false</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is a null reference.</exception>
        /// <remarks>
        /// The <paramref name="ordered"/> parameter is intended for tests that require a known edge
        /// insertion order. A random permutation is usually preferable, as the original <see
        /// cref="Subdivision.Edges"/> order can result in worst-case performance for the
        /// <b>SubdivisionSearch</b> algorithm.</remarks>

        public SubdivisionSearch(Subdivision source, bool ordered = false) {
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");

            Source = source;
            Epsilon = Math.Max(1e-10, source.Epsilon);
            SubdivisionEdge[] edges = new SubdivisionEdge[source.Edges.Count / 2];
            int index = 0;

            // find all twin half-edges with lexicographically smaller origin
            foreach (SubdivisionEdge edge in Source.Edges.Values)
                if (PointDComparerX.CompareEpsilon(edge._origin, edge._twin._origin, Epsilon) < 0)
                    edges[index++] = edge;

            Debug.Assert(index == edges.Length);
            if (!ordered) CollectionsUtility.Randomize(edges);

            foreach (SubdivisionEdge edge in edges)
                InsertEdge(edge);
        }

        #endregion
        #region Private Fields

        // search graph containing nodes & trapezoids
        private object _tree = new Trapezoid();

        #endregion
        #region Epsilon

        /// <summary>
        /// The maximum absolute difference at which two coordinates should be considered equal.
        /// </summary>
        /// <remarks><para>
        /// <b>Epsilon</b> equals either 1e-10 or the comparison <see cref="Subdivision.Epsilon"/>
        /// of the associated <see cref="Source"/>, whichever is greater.
        /// </para><para>
        /// The <see cref="SubdivisionSearch"/> algorithm always uses a positive <b>Epsilon</b> to
        /// guard against <see cref="Subdivision.Vertices"/> with infinitesimal coordinate
        /// differences that might corrupt the search structure. If you encounter exceptions or
        /// incorrect search results for a given <see cref="Subdivision"/>, try increasing its
        /// comparison <see cref="Subdivision.Epsilon"/>.</para></remarks>

        public readonly double Epsilon;

        #endregion
        #region Source

        /// <summary>
        /// The <see cref="Subdivision"/> for which the <see cref="SubdivisionSearch"/> structure
        /// was created.</summary>
        /// <remarks>
        /// <b>Source</b> never returns a null reference. The <see cref="SubdivisionSearch"/>
        /// structure is not updated to reflect structural changes in the associated <b>Source</b>. 
        /// You must create a new <see cref="SubdivisionSearch"/> instance to receive correct
        /// results for a changed <b>Source</b>.</remarks>

        public readonly Subdivision Source;

        #endregion
        #region Find

        /// <summary>
        /// Finds the <see cref="SubdivisionElement"/> at the specified <see cref="PointD"/>
        /// coordinates within the associated <see cref="Source"/>.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <returns>
        /// The <see cref="SubdivisionElement"/> that coincides with <paramref name="q"/>.
        /// </returns>
        /// <remarks><para>
        /// <b>Find</b> returns <see cref="SubdivisionElement.NullFace"/> if <paramref name="q"/>
        /// lies within the unbounded face of the associated <see cref="Source"/>.
        /// </para><para>
        /// If <paramref name="q"/> coincides with a <see cref="SubdivisionEdge"/>, <b>Find</b>
        /// always returns the twin half-edge whose <see cref="SubdivisionEdge.Origin"/> is
        /// lexicographically smaller than its <see cref="SubdivisionEdge.Destination"/>, according
        /// to <see cref="PointDComparerX"/>.
        /// </para><para>
        /// <b>Find</b> always uses the default <see cref="Epsilon"/> to determine whether <paramref
        /// name="q"/> coincides with an <see cref="SubdivisionElement.Edge"/> or <see
        /// cref="SubdivisionElement.Vertex"/>. The <see cref="SubdivisionSearch"/> algorithm does
        /// not support a comparison epsilon that is significantly different from that which was
        /// used to construct the search structure.</para></remarks>

        public SubdivisionElement Find(PointD q) {
            if (_tree is Trapezoid)
                return SubdivisionElement.NullFace;

            return ((Node) _tree).Find(q);
        }

        #endregion
        #region Format

        /// <summary>
        /// Returns a <see cref="String"/> that represents the entire <see
        /// cref="SubdivisionSearch"/> graph.</summary>
        /// <returns>
        /// A <see cref="String"/> containing the <see cref="Object.ToString"/> results for all
        /// nodes of the <see cref="SubdivisionSearch"/> graph, traversed in breadth-first order.
        /// </returns>
        /// <remarks>
        /// <b>Format</b> is a new method, rather than overriding <see cref="Object.ToString"/>,
        /// because the returned <see cref="String"/> may be hundreds or thousands of lines long.
        /// </remarks>

        public string Format() {
            StringBuilder builder = new StringBuilder();
            builder.Append("Root: ");

            Queue queue = new Queue();
            queue.Enqueue(_tree);

            while (queue.Count > 0) {
                object obj = queue.Dequeue();

                builder.Append(obj.ToString());
                builder.Append("\n\n");

                Node node = obj as Node;
                if (node != null) {
                    if (node.Left != null) queue.Enqueue(node.Left);
                    if (node.Right != null) queue.Enqueue(node.Right);
                }
            }

            return builder.ToString();
        }

        #endregion
        #region Validate

        /// <summary>
        /// Validates the structure of the <see cref="SubdivisionSearch"/> graph.</summary>
        /// <exception cref="AssertionException">
        /// The structure of the <see cref="SubdivisionSearch"/> graph is invalid.</exception>
        /// <remarks>
        /// <b>Validate</b> performs a series of <see cref="ThrowHelper.Assert"/> calls that verify
        /// all structural invariants of the <see cref="SubdivisionSearch"/> graph.</remarks>

        public void Validate() {
            Queue queue = new Queue();
            queue.Enqueue(_tree);

            while (queue.Count > 0) {
                object obj = queue.Dequeue();

                VertexNode vertexNode = obj as VertexNode;
                if (vertexNode != null) {
                    VertexNode child = vertexNode.Left as VertexNode;
                    if (child != null)
                        ThrowHelper.Assert(PointDComparerX.CompareEpsilon(
                            vertexNode.Vertex, child.Vertex, Epsilon) > 0);

                    child = vertexNode.Right as VertexNode;
                    if (child != null)
                        ThrowHelper.Assert(PointDComparerX.CompareEpsilon(
                            vertexNode.Vertex, child.Vertex, Epsilon) < 0);
                }

                Node node = obj as Node;
                if (node != null) {
                    ThrowHelper.Assert(node.Left != null);
                    queue.Enqueue(node.Left);

                    ThrowHelper.Assert(node.Right != null);
                    queue.Enqueue(node.Right);

                    continue;
                }

                Trapezoid delta = (Trapezoid) obj;
                ThrowHelper.Assert(!delta.IsDeleted);
                ThrowHelper.Assert(_tree == delta || delta.Parents.Count > 0);
                foreach (Node parent in delta.Parents)
                    ThrowHelper.Assert(parent.Left == delta || parent.Right == delta);

                if (delta.UpperLeft != null) {
                    ThrowHelper.Assert(!delta.UpperLeft.IsDeleted);
                    ThrowHelper.Assert(delta.UpperLeft.UpperRight == delta);
                    ThrowHelper.Assert(delta.TopEdge == delta.UpperLeft.TopEdge);
                    ThrowHelper.Assert(delta.LeftVertex == delta.UpperLeft.RightVertex);
                }

                if (delta.UpperRight != null) {
                    ThrowHelper.Assert(!delta.UpperRight.IsDeleted);
                    ThrowHelper.Assert(delta.UpperRight.UpperLeft == delta);
                    ThrowHelper.Assert(delta.TopEdge == delta.UpperRight.TopEdge);
                    ThrowHelper.Assert(delta.RightVertex == delta.UpperRight.LeftVertex);
                }

                if (delta.LowerLeft != null) {
                    ThrowHelper.Assert(!delta.LowerLeft.IsDeleted);
                    ThrowHelper.Assert(delta.LowerLeft.LowerRight == delta);
                    ThrowHelper.Assert(delta.BottomEdge == delta.LowerLeft.BottomEdge);
                    ThrowHelper.Assert(delta.LeftVertex == delta.LowerLeft.RightVertex);
                }

                if (delta.LowerRight != null) {
                    ThrowHelper.Assert(!delta.LowerRight.IsDeleted);
                    ThrowHelper.Assert(delta.LowerRight.LowerLeft == delta);
                    ThrowHelper.Assert(delta.BottomEdge == delta.LowerRight.BottomEdge);
                    ThrowHelper.Assert(delta.RightVertex == delta.LowerRight.LeftVertex);
                }
            }
        }

        #endregion
        #region FindEdge

        /// <summary>
        /// Finds the list of <see cref="Trapezoid"/> objects that intersect the specified <see
        /// cref="Subdivision"/> edge.</summary>
        /// <param name="edge">
        /// The <see cref="Subdivision"/> edge to examine.</param>
        /// <returns>
        /// A <see cref="List{T}"/> containing each <see cref="Trapezoid"/> that intersects
        /// <paramref name="edge"/>, sorted by increasing x-coordinates.</returns>
        /// <remarks>
        /// The specified <paramref name="edge"/> must be oriented so that its <see
        /// cref="LineD.Start"/> point is lexicographically smaller than its <see cref="LineD.End"/>
        /// point, according to <see cref="PointDComparerX"/>.</remarks>

        private List<Trapezoid> FindEdge(LineD edge) {
            Debug.Assert(_tree is Node);

            Trapezoid delta = ((Node) _tree).FindEdge(edge);
            List<Trapezoid> deltas = new List<Trapezoid>();
            deltas.Add(delta);

            while (PointDComparerX.CompareEpsilon(edge.End, delta.RightVertex, Epsilon) > 0) {
                LineLocation result = edge.Locate(delta.RightVertex);
                switch (result) {

                    case LineLocation.Left:
                        delta = delta.LowerRight;
                        break;

                    case LineLocation.Right:
                        delta = delta.UpperRight;
                        break;

                    default:
                        ThrowHelper.ThrowInvalidOperationExceptionWithFormat(
                            Strings.MethodInvalidValue, "LineD.Locate");
                        break;
                }

                Debug.Assert(delta != null);
                deltas.Add(delta);
            }

            return deltas;
        }

        #endregion
        #region InsertEdge

        /// <summary>
        /// Inserts the specified <see cref="SubdivisionEdge"/> into the <see
        /// cref="SubdivisionSearch"/> graph.</summary>
        /// <param name="edge">
        /// The <see cref="SubdivisionEdge"/> to insert.</param>
        /// <remarks>
        /// The specified <paramref name="edge"/> must be oriented so that its <see
        /// cref="SubdivisionEdge.Origin"/> is lexicographically smaller than its <see
        /// cref="SubdivisionEdge.Destination"/>, according to <see cref="PointDComparerX"/>.
        /// </remarks>

        private void InsertEdge(SubdivisionEdge edge) {
            LineD edgeLine = edge.ToLine();
            Debug.Assert(PointDComparerX.CompareEpsilon(edgeLine.Start, edgeLine.End, Epsilon) < 0);

            List<Trapezoid> deltas = null;
            Trapezoid delta = _tree as Trapezoid;

            // find all trapezoids intersected by edge
            if (delta != null)
                deltas = new List<Trapezoid>(1) { delta };
            else
                deltas = FindEdge(edgeLine);

            // create y-node tree for each intersected trapezoid
            Node[] nodes = new Node[deltas.Count];
            for (int i = 0; i < nodes.Length; i++) {
                EdgeNode node = new EdgeNode(edge, edgeLine, Epsilon);
                nodes[i] = node;
                delta = deltas[i];

                // get previous top & bottom trapezoids, if any
                Trapezoid oldUpperLeaf = null, oldLowerLeaf = null;
                if (i > 0) {
                    oldUpperLeaf = (Trapezoid) nodes[i - 1].Left;
                    oldLowerLeaf = (Trapezoid) nodes[i - 1].Right;
                }

                // link to previous top trapezoid unless edge changed
                if (oldUpperLeaf != null && delta.TopEdge == oldUpperLeaf.TopEdge) {
                    oldUpperLeaf.RightVertex = delta.RightVertex;
                    node.SetLeft(oldUpperLeaf);
                } else {
                    Trapezoid upperLeaf = new Trapezoid() {
                        LeftVertex = delta.LeftVertex,
                        RightVertex = delta.RightVertex,
                        BottomEdge = edge,
                        TopEdge = delta.TopEdge
                    };

                    if (oldUpperLeaf != null) {
                        // establish link across new edge
                        upperLeaf.LowerLeft = oldUpperLeaf;
                        oldUpperLeaf.LowerRight = upperLeaf;

                        // copy links to existing trapezoids
                        upperLeaf.CopyUpperLeft(delta);
                        oldUpperLeaf.CopyUpperRight(deltas[i - 1]);
                    }
                    node.SetLeft(upperLeaf);
                }

                // link to previous bottom trapezoid unless edge changed
                if (oldLowerLeaf != null && delta.BottomEdge == oldLowerLeaf.BottomEdge) {
                    oldLowerLeaf.RightVertex = delta.RightVertex;
                    node.SetRight(oldLowerLeaf);
                } else {
                    Trapezoid lowerLeaf = new Trapezoid() {
                        LeftVertex = delta.LeftVertex,
                        RightVertex = delta.RightVertex,
                        BottomEdge = delta.BottomEdge,
                        TopEdge = edge._twin
                    };

                    if (oldLowerLeaf != null) {
                        // establish link across new edge
                        lowerLeaf.UpperLeft = oldLowerLeaf;
                        oldLowerLeaf.UpperRight = lowerLeaf;

                        // copy links to existing trapezoids
                        lowerLeaf.CopyLowerLeft(delta);
                        oldLowerLeaf.CopyLowerRight(deltas[i - 1]);
                    }
                    node.SetRight(lowerLeaf);
                }
            }

            // get trapezoids at extreme ends
            Trapezoid upperFirstLeaf = (Trapezoid) nodes[0].Left;
            Trapezoid lowerFirstLeaf = (Trapezoid) nodes[0].Right;
            Trapezoid upperLastLeaf, lowerLastLeaf;

            if (nodes.Length == 0) {
                upperLastLeaf = upperFirstLeaf;
                lowerLastLeaf = lowerFirstLeaf;
            } else {
                upperLastLeaf = (Trapezoid) nodes[nodes.Length - 1].Left;
                lowerLastLeaf = (Trapezoid) nodes[nodes.Length - 1].Right;
            }

            // create right x-node for new right vertex
            delta = deltas[deltas.Count - 1];
            if (delta.RightVertex != edgeLine.End) {
                upperLastLeaf.RightVertex = lowerLastLeaf.RightVertex = edgeLine.End;

                Trapezoid leaf = new Trapezoid() {
                    LeftVertex = edgeLine.End,
                    RightVertex = delta.RightVertex,
                    BottomEdge = delta.BottomEdge,
                    TopEdge = delta.TopEdge
                };
                leaf.CopyUpperRight(delta);
                leaf.CopyLowerRight(delta);

                upperLastLeaf.UpperRight = lowerLastLeaf.LowerRight = leaf;
                leaf.UpperLeft = upperLastLeaf;
                leaf.LowerLeft = lowerLastLeaf;

                VertexNode node = new VertexNode(edgeLine.End, Epsilon);
                node.SetRight(leaf);
                node.Left = nodes[nodes.Length - 1];
                nodes[nodes.Length - 1] = node;
            } else {
                upperLastLeaf.CopyUpperRight(delta);
                lowerLastLeaf.CopyLowerRight(delta);
            }

            // create left x-node for new left vertex
            delta = deltas[0];
            if (delta.LeftVertex != edgeLine.Start) {
                upperFirstLeaf.LeftVertex = lowerFirstLeaf.LeftVertex = edgeLine.Start;

                Trapezoid leaf = new Trapezoid() {
                    LeftVertex = delta.LeftVertex,
                    RightVertex = edgeLine.Start,
                    BottomEdge = delta.BottomEdge,
                    TopEdge = delta.TopEdge
                };
                leaf.CopyUpperLeft(delta);
                leaf.CopyLowerLeft(delta);

                upperFirstLeaf.UpperLeft = lowerFirstLeaf.LowerLeft = leaf;
                leaf.UpperRight = upperFirstLeaf;
                leaf.LowerRight = lowerFirstLeaf;

                VertexNode node = new VertexNode(edgeLine.Start, Epsilon);
                node.SetLeft(leaf);
                node.Right = nodes[0];
                nodes[0] = node;
            } else {
                upperFirstLeaf.CopyUpperLeft(delta);
                lowerFirstLeaf.CopyLowerLeft(delta);
            }

            // attach nodes to root or all previous parents
            if (_tree == deltas[0]) {
                _tree = nodes[0];
                deltas[0].IsDeleted = true;
            } else {
                for (int i = 0; i < nodes.Length; i++) {
                    delta = deltas[i];
                    foreach (Node parent in delta.Parents) {
                        if (parent.Left == delta) {
                            Debug.Assert(parent.Right != delta);
                            parent.Left = nodes[i];
                        } else {
                            Debug.Assert(parent.Right == delta);
                            parent.Right = nodes[i];
                        }
                    }
                    delta.IsDeleted = true;
                }
            }
        }

        #endregion
        #region Class Trapezoid

        /// <summary>
        /// Represents one of the elements of the trapezoidal map created from the <see
        /// cref="Subdivision"/> to search.</summary>

        [Serializable]
        private class Trapezoid {
            #region Parents

            /// <summary>
            /// A list containing all <see cref="Node"/> parents of the <see cref="Trapezoid"/>.
            /// </summary>
            /// <remarks>
            /// A fully initialized <see cref="Trapezoid"/> always has at least one valid <see
            /// cref="Node"/> parent, but may have more than one.</remarks>

            public readonly List<Node> Parents = new List<Node>(2);

            #endregion
            #region BottomEdge

            /// <summary>
            /// The <see cref="SubdivisionEdge"/> that forms the lower boundary of the <see
            /// cref="Trapezoid"/>, if any; otherwise, a null reference.</summary>
            /// <remarks>
            /// <b>BottomEdge</b> is a null reference if the lower side of the <see
            /// cref="Trapezoid"/> opens towards the unbounded <see cref="SubdivisionFace"/>.
            /// </remarks>

            public SubdivisionEdge BottomEdge;

            #endregion
            #region TopEdge

            /// <summary>
            /// The <see cref="SubdivisionEdge"/> that forms the upper boundary of the <see
            /// cref="Trapezoid"/>, if any; otherwise, a null reference.</summary>
            /// <remarks>
            /// <b>TopEdge</b> is a null reference if the upper side of the <see cref="Trapezoid"/>
            /// opens towards the unbounded <see cref="SubdivisionFace"/>.</remarks>

            public SubdivisionEdge TopEdge;

            #endregion
            #region Face

            /// <summary>
            /// Gets the <see cref="SubdivisionFace"/> that contains the <see cref="Trapezoid"/>.
            /// </summary>
            /// <value>
            /// A <see cref="SubdivisionElement"/> wrapping the <see cref="SubdivisionFace"/> that
            /// contains the <see cref="Trapezoid"/>.</value>
            /// <remarks>
            /// <b>Face</b> returns <see cref="SubdivisionElement.NullFace"/> if <see
            /// cref="TopEdge"/> and <see cref="BottomEdge"/> are both null references.</remarks>

            public SubdivisionElement Face {
                get {
                    if (TopEdge != null)
                        return new SubdivisionElement(TopEdge._face);

                    if (BottomEdge != null)
                        return new SubdivisionElement(BottomEdge._face);

                    return SubdivisionElement.NullFace;
                }
            }

            #endregion
            #region LeftVertex

            /// <summary>
            /// The <see cref="Subdivision"/> vertex that marks the left boundary of the <see
            /// cref="Trapezoid"/>.</summary>
            /// <remarks>
            /// The <see cref="PointD.X"/> coordinate of <b>LeftVertex</b> equals <see
            /// cref="Double.MinValue"/> if the left side of the <see cref="Trapezoid"/> opens
            /// towards the unbounded <see cref="SubdivisionFace"/>.</remarks>

            public PointD LeftVertex = new PointD(Double.MinValue, 0);

            #endregion
            #region RightVertex

            /// <summary>
            /// The <see cref="Subdivision"/> vertex that marks the right boundary of the <see
            /// cref="Trapezoid"/>.</summary>
            /// <remarks>
            /// The <see cref="PointD.X"/> coordinate of <b>RightVertex</b> equals <see
            /// cref="Double.MaxValue"/> if the right side of the <see cref="Trapezoid"/> opens
            /// towards the unbounded <see cref="SubdivisionFace"/>.</remarks>

            public PointD RightVertex = new PointD(Double.MaxValue, 0);

            #endregion
            #region LowerLeft

            /// <summary>
            /// The <see cref="Trapezoid"/> to the left of the current instance that shares the same
            /// <see cref="BottomEdge"/>, if any; otherwise, a null reference.</summary>

            public Trapezoid LowerLeft;

            #endregion
            #region LowerRight

            /// <summary>
            /// The <see cref="Trapezoid"/> to the right of the current instance that shares the
            /// same <see cref="BottomEdge"/>, if any; otherwise, a null reference.</summary>

            public Trapezoid LowerRight;

            #endregion
            #region UpperLeft

            /// <summary>
            /// The <see cref="Trapezoid"/> to the left of the current instance that shares the same
            /// <see cref="TopEdge"/>, if any; otherwise, a null reference.</summary>

            public Trapezoid UpperLeft;

            #endregion
            #region UpperRight

            /// <summary>
            /// The <see cref="Trapezoid"/> to the right of the current instance that shares the
            /// same <see cref="TopEdge"/>, if any; otherwise, a null reference.</summary>

            public Trapezoid UpperRight;

            #endregion
            #region IsDeleted

            /// <summary>
            /// Gets or sets a value indicating whether the <see cref="Trapezoid"/> has been removed
            /// from the <see cref="SubdivisionSearch"/> graph.</summary>
            /// <value>
            /// <c>true</c> if <see cref="TopEdge"/> equals a special <see cref="SubdivisionEdge"/>
            /// instance with an invalid <see cref="SubdivisionEdge.Key"/> of -1; otherwise,
            /// <c>false</c>. The default is <c>false</c>.</value>
            /// <exception cref="ArgumentOutOfRangeException">
            /// The property is set to <c>false</c>.</exception>
            /// <remarks>
            /// <see cref="Validate"/> checks <b>IsDeleted</b> to find obsolete <see
            /// cref="Trapezoid"/> instances that remain erroneously linked to the search graph, or
            /// to neighboring instances.</remarks>

            public bool IsDeleted {
                get { return (TopEdge == _deletedEdge); }
                set {
                    if (!value)
                        ThrowHelper.ThrowArgumentOutOfRangeException(
                            "value", value, Strings.ArgumentFalse);

                    TopEdge = _deletedEdge;
                }
            }

            private static readonly SubdivisionEdge _deletedEdge = new SubdivisionEdge(-1);

            #endregion
            #region CopyLowerLeft

            /// <summary>
            /// Copies the <see cref="LowerLeft"/> link from the specified <see cref="Trapezoid"/>.
            /// </summary>
            /// <param name="trapezoid">
            /// The <see cref="Trapezoid"/> whose <see cref="LowerLeft"/> link to copy.</param>
            /// <remarks>
            /// <b>CopyLowerLeft</b> also changes the <see cref="LowerRight"/> link of a valid <see
            /// cref="LowerLeft"/> neighbor to the current instance.</remarks>

            public void CopyLowerLeft(Trapezoid trapezoid) {
                if (trapezoid.LowerLeft != null) {
                    LowerLeft = trapezoid.LowerLeft;
                    Debug.Assert(LowerLeft.LowerRight == trapezoid);
                    LowerLeft.LowerRight = this;
                }
            }

            #endregion
            #region CopyLowerRight

            /// <summary>
            /// Copies the <see cref="LowerRight"/> link from the specified <see cref="Trapezoid"/>.
            /// </summary>
            /// <param name="trapezoid">
            /// The <see cref="Trapezoid"/> whose <see cref="LowerRight"/> link to copy.</param>
            /// <remarks>
            /// <b>CopyLowerRight</b> also changes the <see cref="LowerLeft"/> link of a valid <see
            /// cref="LowerRight"/> neighbor to the current instance.</remarks>

            public void CopyLowerRight(Trapezoid trapezoid) {
                if (trapezoid.LowerRight != null) {
                    LowerRight = trapezoid.LowerRight;
                    Debug.Assert(LowerRight.LowerLeft == trapezoid);
                    LowerRight.LowerLeft = this;
                }
            }

            #endregion
            #region CopyUpperLeft

            /// <summary>
            /// Copies the <see cref="UpperLeft"/> link from the specified <see cref="Trapezoid"/>.
            /// </summary>
            /// <param name="trapezoid">
            /// The <see cref="Trapezoid"/> whose <see cref="UpperLeft"/> link to copy.</param>
            /// <remarks>
            /// <b>CopyUpperLeft</b> also changes the <see cref="UpperRight"/> link of a valid <see
            /// cref="UpperLeft"/> neighbor to the current instance.</remarks>

            public void CopyUpperLeft(Trapezoid trapezoid) {
                if (trapezoid.UpperLeft != null) {
                    UpperLeft = trapezoid.UpperLeft;
                    Debug.Assert(UpperLeft.UpperRight == trapezoid);
                    UpperLeft.UpperRight = this;
                }
            }

            #endregion
            #region CopyUpperRight

            /// <summary>
            /// Copies the <see cref="UpperRight"/> link from the specified <see cref="Trapezoid"/>.
            /// </summary>
            /// <param name="trapezoid">
            /// The <see cref="Trapezoid"/> whose <see cref="UpperRight"/> link to copy.</param>
            /// <remarks>
            /// <b>CopyUpperRight</b> also changes the <see cref="UpperLeft"/> link of a valid <see
            /// cref="UpperRight"/> neighbor to the current instance.</remarks>

            public void CopyUpperRight(Trapezoid trapezoid) {
                if (trapezoid.UpperRight != null) {
                    UpperRight = trapezoid.UpperRight;
                    Debug.Assert(UpperRight.UpperLeft == trapezoid);
                    UpperRight.UpperLeft = this;
                }
            }

            #endregion
            #region ToString

            /// <summary>
            /// Returns a <see cref="String"/> that represents the <see cref="Trapezoid"/>.
            /// </summary>
            /// <returns>
            /// A <see cref="String"/> containing the associated <see cref="Subdivision"/> edges and
            /// vertices, and the <see cref="Object.GetHashCode"/> results for the <see
            /// cref="Trapezoid"/>, its neighbors, and its <see cref="Parents"/>.</returns>

            public override string ToString() {

                string parents = "(null)";
                if (Parents.Count > 0) {
                    StringBuilder builder = new StringBuilder();
                    foreach (Node parent in Parents) {
                        if (builder.Length > 0) builder.Append(", ");
                        builder.Append(parent.GetHashCode());
                    }
                    parents = builder.ToString();
                }

                Func<Object, String> showHashCode = (o) => (o == null ? "(null)" :
                    o.GetHashCode().ToString(CultureInfo.InvariantCulture));

                return String.Format(CultureInfo.InvariantCulture,
                    "{0} Trapezoid Parents {1} \n\tLeft {2} \n\tRight {3} \n\tTop {4} \n\t" +
                    "Bottom {5} \n\tLeft Upper {6}, Lower {7} \n\tRight Upper {8}, Lower {9}",
                    GetHashCode(), parents, LeftVertex, RightVertex,
                    StringUtility.Validate(TopEdge), StringUtility.Validate(BottomEdge),
                    showHashCode(UpperLeft), showHashCode(LowerLeft),
                    showHashCode(UpperRight), showHashCode(LowerRight));
            }

            #endregion
        }

        #endregion
        #region Class Node

        /// <summary>
        /// Represents one of the inner nodes of the search graph that spans the trapezoidal map
        /// created from the <see cref="Subdivision"/> to search.</summary>
        /// <remarks>
        /// The leaf nodes of the search graph are always <see cref="Trapezoid"/> objects.</remarks>

        [Serializable]
        private abstract class Node {
            #region Node(Double)

            /// <summary>
            /// Initializes a new instance of the <see cref="Node"/> class.</summary>
            /// <param name="epsilon"><para>
            /// The maximum absolute difference at which two coordinates should be considered equal.
            /// </para><para>-or-</para><para>
            /// Zero to use exact coordinate comparisons. This value cannot be negative.
            /// </para></param>

            public Node(double epsilon) {
                Debug.Assert(epsilon >= 0);
                Epsilon = epsilon;
            }

            #endregion
            #region Epsilon

            /// <summary>
            /// The maximum absolute difference at which two coordinates should be considered equal.
            /// </summary>
            /// <remarks>
            /// <b>Epsilon</b> is always equal to or greater than zero. This value is identical for
            /// all <see cref="Node"/> instances in the same <see cref="SubdivisionSearch"/> graph.
            /// </remarks>

            public readonly double Epsilon;

            #endregion
            #region Left

            /// <summary>
            /// The <see cref="Node"/> or <see cref="Trapezoid"/> that is the left descendant of the
            /// current instance.</summary>
            /// <remarks>
            /// <b>Left</b> is never a null reference for a fully initialized <see cref="Node"/>.
            /// All inner nodes of the search graph have two descendants.</remarks>

            public object Left;

            #endregion
            #region Right

            /// <summary>
            /// The <see cref="Node"/> or <see cref="Trapezoid"/> that is the right descendant of
            /// the current instance.</summary>
            /// <remarks>
            /// <b>Right</b> is never a null reference for a fully initialized <see cref="Node"/>.
            /// All inner nodes of the search graph have two descendants.</remarks>

            public object Right;

            #endregion
            #region Find

            /// <summary>
            /// Finds the <see cref="SubdivisionElement"/> at the specified <see cref="PointD"/>
            /// coordinates within the subtree starting at <see cref="Node"/>.</summary>
            /// <param name="q">
            /// The <see cref="PointD"/> coordinates to examine.</param>
            /// <returns>
            /// The <see cref="SubdivisionElement"/> that coincides with <paramref name="q"/>.
            /// </returns>

            public abstract SubdivisionElement Find(PointD q);

            #endregion
            #region FindEdge

            /// <summary>
            /// Finds the <see cref="Trapezoid"/> that contains the <see cref="LineD.Start"/> of the
            /// specified <see cref="Subdivision"/> edge.</summary>
            /// <param name="edge">
            /// The <see cref="Subdivision"/> edge to examine.</param>
            /// <returns>
            /// The <see cref="Trapezoid"/> that contains the <see cref="LineD.Start"/> of <paramref
            /// name="edge"/>.</returns>

            public abstract Trapezoid FindEdge(LineD edge);

            #endregion
            #region SetLeft

            /// <summary>
            /// Sets the <see cref="Left"/> descendant to the specified <see cref="Trapezoid"/>.
            /// </summary>
            /// <param name="trapezoid">
            /// The <see cref="Trapezoid"/> that is the new <see cref="Left"/> descendant.</param>
            /// <remarks>
            /// <b>SetLeft</b> also adds the <see cref="Node"/> to the <see
            /// cref="Trapezoid.Parents"/> of the specified <paramref name="trapezoid"/>.</remarks>

            public void SetLeft(Trapezoid trapezoid) {
                Left = trapezoid;
                trapezoid.Parents.Add(this);
            }

            #endregion
            #region SetRight

            /// <summary>
            /// Sets the <see cref="Right"/> descendant to the specified <see cref="Trapezoid"/>.
            /// </summary>
            /// <param name="trapezoid">
            /// The <see cref="Trapezoid"/> that is the new <see cref="Right"/> descendant.</param>
            /// <remarks>
            /// <b>SetRight</b> also adds the <see cref="Node"/> to the <see
            /// cref="Trapezoid.Parents"/> of the specified <paramref name="trapezoid"/>.</remarks>

            public void SetRight(Trapezoid trapezoid) {
                Right = trapezoid;
                trapezoid.Parents.Add(this);
            }

            #endregion
            #region ToString

            /// <summary>
            /// Returns a <see cref="String"/> that represents the <see cref="Node"/>.</summary>
            /// <returns>
            /// A <see cref="String"/> containing the <see cref="Object.GetHashCode"/> results for
            /// the <see cref="Left"/> and <see cref="Right"/> descendants.</returns>

            public override string ToString() {

                string left = (Left == null ? "(null)" :
                    Left.GetHashCode().ToString(CultureInfo.InvariantCulture));

                string right = (Left == null ? "(null)" :
                    Right.GetHashCode().ToString(CultureInfo.InvariantCulture));

                return String.Format(CultureInfo.InvariantCulture,
                    "Left {0}, Right {1}", left, right);
            }

            #endregion
        }

        #endregion
        #region Class EdgeNode

        /// <summary>
        /// Represents a search graph <see cref="Node"/> that divides its subtree along a <see
        /// cref="Subdivision"/> edge.</summary>
        /// <remarks>
        /// The <see cref="Node.Left"/> child of an <see cref="EdgeNode"/> contains all search graph
        /// nodes to the left of its <see cref="EdgeNode.Edge"/>, and the <see cref="Node.Right"/>
        /// child contains all nodes to the right, assuming that y-coordinates increase upward.
        /// </remarks>

        [Serializable]
        private sealed class EdgeNode: Node {
            #region EdgeNode(SubdivisionEdge, LineD, Double)

            /// <summary>
            /// Initializes a new instance of the <see cref="EdgeNode"/> class with the specified
            /// <see cref="SubdivisionEdge"/>.</summary>
            /// <param name="edge">
            /// The <see cref="SubdivisionEdge"/> that divides the subtree beginning at the <see
            /// cref="EdgeNode"/>.</param>
            /// <param name="edgeLine">
            /// The <see cref="LineD"/> representation of <paramref name="edge"/>.</param>
            /// <param name="epsilon"><para>
            /// The maximum absolute difference at which two coordinates should be considered equal.
            /// </para><para>-or-</para><para>
            /// Zero to use exact coordinate comparisons. This value cannot be negative.
            /// </para></param>
            /// <remarks>
            /// The specified <paramref name="edge"/> must be oriented so that its <see
            /// cref="SubdivisionEdge.Origin"/> is lexicographically smaller than its <see
            /// cref="SubdivisionEdge.Destination"/>, according to <see cref="PointDComparerX"/>.
            /// </remarks>

            public EdgeNode(SubdivisionEdge edge, LineD edgeLine, double epsilon): base(epsilon) {

                Debug.Assert(edge.ToLine() == edgeLine);
                Debug.Assert(PointDComparerX.CompareEpsilon(edgeLine.Start, edgeLine.End, epsilon) < 0);

                Edge = edge;
                EdgeLine = edgeLine;
            }

            #endregion
            #region Edge

            /// <summary>
            /// The <see cref="SubdivisionEdge"/> that divides the subtree beginning at the <see
            /// cref="EdgeNode"/>.</summary>
            /// <remarks>
            /// <b>Edge</b> is always oriented so that its <see cref="SubdivisionEdge.Origin"/> is
            /// lexicographically smaller than its <see cref="SubdivisionEdge.Destination"/>,
            /// according to <see cref="PointDComparerX"/>.</remarks>

            public readonly SubdivisionEdge Edge;

            #endregion
            #region EdgeLine

            /// <summary>
            /// The <see cref="LineD"/> representation of the associated <see cref="Edge"/>.
            /// </summary>

            public readonly LineD EdgeLine;

            #endregion
            #region Find

            /// <summary>
            /// Finds the <see cref="SubdivisionElement"/> at the specified <see cref="PointD"/>
            /// coordinates within the subtree starting at <see cref="Node"/>.</summary>
            /// <param name="q">
            /// The <see cref="PointD"/> coordinates to examine.</param>
            /// <returns>
            /// The <see cref="SubdivisionElement"/> that coincides with <paramref name="q"/>.
            /// </returns>

            public override sealed SubdivisionElement Find(PointD q) {
                object obj = null;
                LineLocation result = EdgeLine.Locate(q, Epsilon);

                switch (result) {
                    case LineLocation.Start:
                        // should have been pre-empted by VertexNode
                        return new SubdivisionElement(EdgeLine.Start);

                    case LineLocation.End:
                        // should have been pre-empted by VertexNode
                        return new SubdivisionElement(EdgeLine.End);

                    case LineLocation.Between:
                        return new SubdivisionElement(Edge);

                    case LineLocation.Left:
                    case LineLocation.Before:
                        obj = Left; break;

                    case LineLocation.Right:
                    case LineLocation.After:
                        obj = Right; break;
                }

                Node node = obj as Node;
                if (node != null) return node.Find(q);

                return ((Trapezoid) obj).Face;
            }

            #endregion
            #region FindEdge

            /// <summary>
            /// Finds the <see cref="Trapezoid"/> that contains the <see cref="LineD.Start"/> of the
            /// specified <see cref="Subdivision"/> edge.</summary>
            /// <param name="edge">
            /// The <see cref="Subdivision"/> edge to examine.</param>
            /// <returns>
            /// The <see cref="Trapezoid"/> that contains the <see cref="LineD.Start"/> of <paramref
            /// name="edge"/>.</returns>
            /// <remarks>
            /// The specified <paramref name="edge"/> must be oriented so that its <see
            /// cref="LineD.Start"/> point is lexicographically smaller than its <see
            /// cref="LineD.End"/> point, according to <see cref="PointDComparerX"/>.</remarks>

            public override sealed Trapezoid FindEdge(LineD edge) {
                Debug.Assert(PointDComparerX.CompareEpsilon(edge.Start, edge.End, Epsilon) < 0);
                object obj;
                LineLocation result = EdgeLine.Locate(edge.Start);

                if (result == LineLocation.Left)
                    obj = Left;
                else if (result == LineLocation.Right)
                    obj = Right;
                else {
                    /*
                     * Any Start point that lies on the current Edge must coincide with its Start
                     * point -- not its End point due to the lexicographic ordering of all edges,
                     * not a Between point since subdivision edges are non-intersecting, and not a
                     * Before or After point since that would be outside the current node’s range.
                     * 
                     * We now order the two edges by slope, with the greater slope interpreted as
                     * left/above. Straight up is maximum slope (positive infinity), horizontal to
                     * the right is slope zero, and almost straight down is minimum slope (close to
                     * negative infinity). Equal slopes are impossible, as that would imply
                     * overlapping subdivision edges.
                     */
                    Debug.Assert(result == LineLocation.Start);
                    if (Math.Abs(edge.Start.X - edge.End.X) <= Epsilon)
                        obj = Left;
                    else if (Math.Abs(EdgeLine.Start.X - EdgeLine.End.X) <= Epsilon)
                        obj = Right;
                    else
                        obj = (edge.Slope > EdgeLine.Slope ? Left : Right);
                }

                Node node = obj as Node;
                if (node != null) return node.FindEdge(edge);

                return (Trapezoid) obj;
            }

            #endregion
            #region ToString

            /// <summary>
            /// Returns a <see cref="String"/> that represents the <see cref="EdgeNode"/>.</summary>
            /// <returns>
            /// A <see cref="String"/> containing the <see cref="Object.GetHashCode"/> result for
            /// the <see cref="EdgeNode"/>, the associated <see cref="Edge"/>, and the result of the
            /// <see cref="Node"/> implementation of <see cref="Node.ToString"/>.</returns>

            public override string ToString() {
                return String.Format(CultureInfo.InvariantCulture,
                    "{0} Y-Node {1} \n\t{2} \n\t{3}",
                    GetHashCode(), EdgeLine, Edge, base.ToString());
            }

            #endregion
        }

        #endregion
        #region Class VertexNode

        /// <summary>
        /// Represents a search graph <see cref="Node"/> that divides its subtree across a <see
        /// cref="Subdivision"/> vertex.</summary>
        /// <remarks>
        /// The <see cref="Node.Left"/> child of a <see cref="VertexNode"/> contains all search
        /// graph nodes to the left of its <see cref="VertexNode.Vertex"/>, and the <see
        /// cref="Node.Right"/> child contains all nodes to the right.</remarks>

        [Serializable]
        private sealed class VertexNode: Node {
            #region VertexNode(PointD, Double)

            /// <summary>
            /// Initializes a new instance of the <see cref="VertexNode"/> class with the specified
            /// <see cref="Subdivision"/> vertex.</summary>
            /// <param name="vertex">
            /// The <see cref="Subdivision"/> vertex that divides the subtree beginning at the <see
            /// cref="VertexNode"/>.</param>
            /// <param name="epsilon"><para>
            /// The maximum absolute difference at which two coordinates should be considered equal.
            /// </para><para>-or-</para><para>
            /// Zero to use exact coordinate comparisons. This value cannot be negative.
            /// </para></param>

            public VertexNode(PointD vertex, double epsilon): base(epsilon) {
                Vertex = vertex;
            }

            #endregion
            #region Vertex

            /// <summary>
            /// The <see cref="Subdivision"/> vertex that divides the subtree beginning at the <see
            /// cref="VertexNode"/>.</summary>

            public readonly PointD Vertex;

            #endregion
            #region Find

            /// <summary>
            /// Finds the <see cref="SubdivisionElement"/> at the specified <see cref="PointD"/>
            /// coordinates within the subtree starting at <see cref="Node"/>.</summary>
            /// <param name="q">
            /// The <see cref="PointD"/> coordinates to examine.</param>
            /// <returns>
            /// The <see cref="SubdivisionElement"/> that coincides with <paramref name="q"/>.
            /// </returns>

            public override sealed SubdivisionElement Find(PointD q) {

                int result = PointDComparerX.CompareEpsilon(q, Vertex, Epsilon);
                if (result == 0) return new SubdivisionElement(Vertex);

                object obj = (result < 0 ? Left : Right);
                Node node = obj as Node;
                if (node != null) return node.Find(q);

                return ((Trapezoid) obj).Face;
            }

            #endregion
            #region FindEdge

            /// <summary>
            /// Finds the <see cref="Trapezoid"/> that contains the <see cref="LineD.Start"/> of the
            /// specified <see cref="Subdivision"/> edge.</summary>
            /// <param name="edge">
            /// The <see cref="Subdivision"/> edge to examine.</param>
            /// <returns>
            /// The <see cref="Trapezoid"/> that contains the <see cref="LineD.Start"/> of <paramref
            /// name="edge"/>.</returns>

            public override sealed Trapezoid FindEdge(LineD edge) {

                int result = PointDComparerX.CompareEpsilon(edge.Start, Vertex, Epsilon);
                object obj = (result < 0 ? Left : Right);

                Node node = obj as Node;
                if (node != null) return node.FindEdge(edge);

                return (Trapezoid) obj;
            }

            #endregion
            #region ToString

            /// <summary>
            /// Returns a <see cref="String"/> that represents the <see cref="VertexNode"/>.
            /// </summary>
            /// <returns>
            /// A <see cref="String"/> containing the <see cref="Object.GetHashCode"/> result for
            /// the <see cref="VertexNode"/>, the <see cref="Vertex"/> coordinates, and the result
            /// of the <see cref="Node"/> implementation of <see cref="Node.ToString"/>.</returns>

            public override string ToString() {
                return String.Format(CultureInfo.InvariantCulture,
                    "{0} X-Node {1} \n\t{2}", GetHashCode(), Vertex, base.ToString());
            }

            #endregion
        }

        #endregion
    }
}
