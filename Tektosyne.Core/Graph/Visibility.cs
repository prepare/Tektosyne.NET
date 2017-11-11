using System;
using System.Collections.Generic;
using System.Diagnostics;

using Tektosyne.Collections;
using Tektosyne.Geometry;

namespace Tektosyne.Graph {

    /// <summary>
    /// Provides a line-of-sight algorithm for <see cref="IGraph2D{T}"/> instances.</summary>
    /// <typeparam name="T">
    /// The type of all nodes in the graph. If <typeparamref name="T"/> is a reference type, nodes
    /// cannot be null references.</typeparam>
    /// <remarks><para>
    /// The <b>Visibility</b> algorithm starts on a specified starting node and recursively finds
    /// any adjacent nodes, up to a specified maximum world distance, whose line of sight is not
    /// blocked by any other examined nodes closer to the source. The opacity of a given node is
    /// determined by a specified <see cref="Predicate{Int32}"/> delegate.
    /// </para><para>
    /// For each tested node, we consider its polygonal world region as defined by <see
    /// cref="IGraph2D{T}.GetWorldRegion"/>. We draw the two tangents from the source node’s world
    /// location to the extreme points of that region. An opaque node blocks visibility, across the
    /// angle between its tangents, for any node whose nearest world region vertex is farther from
    /// the source than that of the opaque node. A node is considered visible as long as a certain
    /// fraction of its tangential arc remains unobscured, as defined by the <see
    /// cref="Visibility{T}.Threshold"/> property.
    /// </para><para>
    /// If an opaque node obscures only the middle part of another node’s tangential arc, but leaves
    /// partial arcs on both ends visible, only the greater of these partial arcs is considered
    /// visible whereas the smaller is considered obscured. This simplifies visibility testing,
    /// although very rarely visible nodes may be misclassified as obscured if the size of world
    /// regions varies greatly among <see cref="IGraph2D{T}"/> nodes.
    /// </para><para>
    /// Any graph node for which <see cref="IGraph2D{T}.GetWorldRegion"/> returns a null reference
    /// is assigned a default tangential arc, spanning one degree around its world location. This
    /// assumption allows the <b>Visibility</b> algorithm to process graphs that do not define world
    /// regions for all nodes, although the results are likely not very useful.</para></remarks>

    [Serializable]
    public class Visibility<T> {
        #region Visibility(IGraph2D<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="Visibility{T}"/> class with the specified
        /// two-dimensional graph.</summary>
        /// <param name="graph">
        /// The <see cref="IGraph2D{T}"/> on which all searches are performed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="graph"/> is a null reference.</exception>

        public Visibility(IGraph2D<T> graph) {
            if (graph == null)
                ThrowHelper.ThrowArgumentNullException("graph");

            Graph = graph;
        }

        #endregion
        #region Private Fields

        // search arguments
        private T _source;
        private PointD _sourceWorld;
        private Predicate<T> _isOpaque;
        private double _distance;

        // property backers
        private double _threshold = 1.0 / 3.0;
        private readonly ListEx<T> _nodes = new ListEx<T>();
        private readonly DictionaryEx<T, NodeArc> _nodeArcs = new DictionaryEx<T, NodeArc>();

        // visible nodes that obscure the view on other nodes
        private readonly Dictionary<T, NodeArc> _obscuringNodes = new Dictionary<T, NodeArc>();

        // nodes to remove from collection of obscuring nodes
        private readonly List<T> _removeNodes = new List<T>();


        #endregion
        #region Graph

        /// <summary>
        /// The <see cref="IGraph2D{T}"/> on which all searches are performed.</summary>

        public readonly IGraph2D<T> Graph;

        #endregion
        #region Nodes

        /// <summary>
        /// Gets the list of all <see cref="Graph"/> nodes that were reached by the last successful
        /// search.</summary>
        /// <value>
        /// A read-only <see cref="ListEx{T}"/> containing all <see cref="Graph"/> nodes that were
        /// reached by the last successful call to <see cref="FindVisible"/>, not including the
        /// source node.</value>
        /// <remarks><para>
        /// <b>Nodes</b> never returns a null reference, but it returns an empty collection if the
        /// last call to <see cref="FindVisible"/> returned <c>false</c>, or if the method has not
        /// yet been called.
        /// </para><para>
        /// <b>Nodes</b> contains those <see cref="Graph"/> nodes in the <see cref="NodeArcs"/>
        /// collection whose <see cref="NodeArc.VisibleFraction"/> equals or exceeds the current
        /// <see cref="Threshold"/>.</para></remarks>

        public ListEx<T> Nodes {
            [DebuggerStepThrough]
            get { return _nodes.AsReadOnly(); }
        }

        #endregion
        #region NodeArcs

        /// <summary>
        /// Gets the source distances, tangential arcs, and visible fractions for all <see
        /// cref="Graph"/> nodes that were examined by the last search.</summary>
        /// <value>
        /// A read-only <see cref="DictionaryEx{TKey, TValue}"/> that maps all visited <see
        /// cref="Graph"/> nodes to the corresponding <see cref="NodeArc"/> instances.</value>
        /// <remarks><para>
        /// <b>NodeArcs</b> never returns a null reference, but it returns an empty collection if
        /// <see cref="FindVisible"/> has not yet been called.
        /// </para><para>
        /// <b>NodeArcs</b> contains all <see cref="Graph"/> nodes that were examined by <see
        /// cref="FindVisible"/>, including partly or fully obscured nodes that were not added to
        /// the <see cref="Nodes"/> collection, but excluding the source node.</para></remarks>

        public DictionaryEx<T, NodeArc> NodeArcs {
            [DebuggerStepThrough]
            get { return _nodeArcs.AsReadOnly(); }
        }

        #endregion
        #region Threshold

        /// <summary>
        /// Gets or sets the visibility threshold for any <see cref="Graph"/> node, as a fraction of
        /// the sweep angle of its tangential arc.</summary>
        /// <value>
        /// The minimum <see cref="NodeArc.VisibleFraction"/> for the <see cref="NodeArc"/> of any
        /// <see cref="Graph"/> node that should be considered visible. The default is 1/3.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The property is set to a value that is less than zero or greater than one.</exception>
        /// <remarks><para>
        /// <b>Threshold</b> returns <see cref="Double.Epsilon"/> when set to zero. The effect is
        /// that a <see cref="Graph"/> node is considered visible while even the smallest fraction
        /// of its tangential arc remains unobscured.
        /// </para><para>
        /// Setting <b>Threshold</b> to one has the opposite effect. A <see cref="Graph"/> node is
        /// considered visible only if its entire tangential arc remains unobscured. Values between
        /// zero and one allow greater or lesser degrees of obscuration.</para></remarks>

        public double Threshold {
            [DebuggerStepThrough]
            get { return _threshold; }
            set {
                if (value < 0 || value > 1)
                    ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                        "value", value, Strings.ArgumentLessOrGreater, "0", "1");

                if (value == 0) value = Double.Epsilon;
                _threshold = value;
            }
        }

        #endregion
        #region FindVisible

        /// <summary>
        /// Finds all contiguous <see cref="Graph"/> nodes within the specified maximum world
        /// distance that are visible from the specified node.</summary>
        /// <param name="isOpaque">
        /// The <see cref="Predicate{T}"/> delegate that determines whether a <see cref="Graph"/>
        /// node blocks the line of sight.</param>
        /// <param name="source">
        /// The source node within <see cref="Graph"/> where the search starts.</param>
        /// <param name="distance"><para>
        /// The maximum world distance from the specified <paramref name="source"/> to search.
        /// </para><para>-or-</para><para>
        /// Zero to search the entire <see cref="Graph"/>. The default is zero.</para></param>
        /// <returns>
        /// <c>true</c> if one or more nodes are visible from <paramref name="source"/> within the
        /// specified <paramref name="distance"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="isOpaque"/> or <paramref name="source"/> is a null reference.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="distance"/> is less than zero.</exception>
        /// <remarks><para>
        /// <b>FindVisible</b> returns <c>false</c> if the specified <paramref name="source"/> node
        /// is invalid, or if there are no visible nodes. Otherwise, <b>FindVisible</b> returns
        /// <c>true</c> and sets the <see cref="Nodes"/> and <see cref="NodeArcs"/> properties to
        /// the result of the visibility search.
        /// </para><para>
        /// All nodes within the specified maximum <paramref name="distance"/> are considered
        /// visible, except for those that are obscured by a node for which <paramref
        /// name="isOpaque"/> succeeds, as described for the <see cref="Visibility{T}"/> class.
        /// </para><para>
        /// If <paramref name="distance"/> is positive, any visible node must be reachable by a path
        /// that only includes other nodes within <paramref name="distance"/>; otherwise, it will
        /// not be found. This condition holds for any <see cref="PolygonGrid"/>, and for any <see
        /// cref="Subdivision"/> that was created from a Delaunay triangulation.</para></remarks>

        public bool FindVisible(Predicate<T> isOpaque, T source, double distance = 0) {
            if (isOpaque == null)
                ThrowHelper.ThrowArgumentNullException("isOpaque");
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");
            if (distance < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "distance", distance, Strings.ArgumentNegative);

            // clear previous results
            _nodes.Clear();
            _nodeArcs.Clear();

            // fail if source node is invalid
            if (!Graph.Contains(source)) return false;

            _isOpaque = isOpaque;
            _source = source;
            _distance = distance;

            // compute world coordinates of source node
            _sourceWorld = Graph.GetWorldLocation(source);

            // expand visibility area from source
            FindObscuringNodes(source);
            FindVisibleNodes();

            // clear intermediate data
            _isOpaque = null;
            _source = default(T);
            _obscuringNodes.Clear();

            // succeed if any nodes reached
            return (_nodes.Count > 0);
        }

        #endregion
        #region Private Methods
        #region CreateNodeArc

        /// <summary>
        /// Creates the <see cref="NodeArc"/> for the specified target node.</summary>
        /// <param name="target">
        /// The <see cref="Graph"/> node whose <see cref="NodeArc"/> to create.</param>
        /// <returns>
        /// A new <see cref="NodeArc"/> for the specified <paramref name="target"/> node.</returns>

        private NodeArc CreateNodeArc(T target) {

            // compute world coordinates of target node
            PointD targetWorld = Graph.GetWorldLocation(target);
            PointD[] region = Graph.GetWorldRegion(target);

            // alpha is angle from source to center of target node
            LineD line = new LineD(_sourceWorld, targetWorld);
            double alpha = line.Angle;

            // use central location with 1° arc if no region available
            if (region == null) {
                const double arc = Angle.DegreesToRadians;
                alpha -= arc / 2;
                if (alpha <= -Math.PI) alpha += 2 * Math.PI;
                return new NodeArc(alpha, arc, line.Length);
            }

            double distance = Double.MaxValue;
            double minBeta = 0, maxBeta = 0;

            foreach (PointD vertex in region) {
                // find extreme relative angles for any region vertex
                double beta = _sourceWorld.AngleBetween(targetWorld, vertex);
                if (beta <= minBeta)
                    minBeta = beta;
                else if (beta >= maxBeta)
                    maxBeta = beta;

                // find smallest distance for any region vertex
                double vertexDistance = (vertex - _sourceWorld).Length;
                if (distance > vertexDistance)
                    distance = vertexDistance;
            }

            return new NodeArc(alpha + minBeta, maxBeta - minBeta, distance);
        }

        #endregion
        #region FindObscuringNodes

        /// <summary>
        /// Expands the current collection of obscuring <see cref="Graph"/> nodes with all neighbors
        /// of the specified node, within maximum world distance from the source node.</summary>
        /// <param name="node">
        /// The <see cref="Graph"/> node whose neighbors to examine.</param>
        /// <remarks><para>
        /// <b>FindObscuringNodes</b> recursively visits all directly connected nodes, and adds them
        /// to an internal collection of obscuring nodes if they are opaque. Nodes which are fully
        /// obscured by other obscuring nodes are removed from the collection.
        /// </para><para>
        /// <b>FindObscuringNodes</b> never revisits nodes that were already examined. All visited
        /// nodes are added to <see cref="NodeArcs"/> for later processing by <see
        /// cref="FindVisibleNodes"/>.</para></remarks>

        private void FindObscuringNodes(T node) {

            // get valid neighbors of current node
            IList<T> neighbors = Graph.GetNeighbors(node);

            // recurse into all valid neighbors
            for (int i = 0; i < neighbors.Count; i++) {
                T neighbor = neighbors[i];

                // skip source and previously visited nodes
                if (ComparerCache<T>.EqualityComparer.Equals(_source, neighbor)
                    || _nodeArcs.ContainsKey(neighbor))
                    continue;

                // compute tangential arc and source distance
                NodeArc arc = CreateNodeArc(neighbor);

                // skip nodes beyond maximum distance
                if (_distance > 0 && arc.Distance > _distance)
                    continue;

                // record visited node with tangential arc
                _nodeArcs.Add(neighbor, arc);

                // nothing else to do for transparent nodes
                if (!_isOpaque(neighbor))
                    goto nextNeighbor;

                /*
                 * Try adding current opaque node to list of all obscuring nodes recorded so far.
                 * 
                 * If any single recorded node completely obscures the current node, we skip it.
                 * If the current node completely obscures any recorded nodes, we delete those.
                 * 
                 * We also clear the VisiblityFraction for all completely obscured nodes (current
                 * or recorded) so we won't waste time testing them again in FindVisibleNodes.
                 */

                foreach (var pair in _obscuringNodes) { 
                    int result = arc.IsObscured(pair.Value);

                    if (result < 0) {
                        arc._visibleFraction = 0;
                        goto nextNeighbor;
                    }
                    if (result > 0) {
                        pair.Value._visibleFraction = 0;
                        _removeNodes.Add(pair.Key);
                    }
                }

                // remove obscuring nodes that were themselves obscured
                for (int j = 0; j < _removeNodes.Count; j++)
                    _obscuringNodes.Remove(_removeNodes[j]);
                _removeNodes.Clear();

                // add neighbor to obscuring nodes
                _obscuringNodes.Add(neighbor, arc);

            nextNeighbor:
                FindObscuringNodes(neighbor);
            }
        }

        #endregion
        #region FindVisibleNodes

        /// <summary>
        /// Expands the current visibility area with all visible <see cref="Graph"/> nodes, within
        /// maximum world distance from the source node.</summary>
        /// <remarks><para>
        /// <b>FindVisibleNodes</b> iterates over all <see cref="NodeArcs"/> found by <see
        /// cref="FindObscuringNodes"/>, and adjusts their <see cref="NodeArc.VisibleFraction"/> 
        /// according to the collection of obscuring nodes also created by that method.
        /// </para><para>
        /// Any node whose <see cref="NodeArc"/> remains unobscured by at least the current <see
        /// cref="Threshold"/> is added to the <see cref="Nodes"/> collection.</para></remarks>

        private void FindVisibleNodes() {

            // iterate over all visited nodes that may be visible
            foreach (var pair in _nodeArcs) {
                NodeArc arc = pair.Value;

                if (arc._visibleFraction == 0) continue;
                double start = arc.Start, sweep = arc.Sweep;

                // compare visited node to all obscuring nodes (except itself)
                foreach (var obscuringPair in _obscuringNodes) {
                    if (ComparerCache<T>.EqualityComparer.Equals(pair.Key, obscuringPair.Key))
                        continue;

                    // ignore obscuring nodes at a greater source distance
                    NodeArc obscuringAngle = obscuringPair.Value;
                    if (obscuringAngle.Distance > arc.Distance)
                        continue;

                    // obscure visibility arc of visited node
                    obscuringAngle.Obscure(ref start, ref sweep);
                    arc._visibleFraction = sweep / arc.Sweep;

                    // check if arc is sufficiently obscured
                    if (arc._visibleFraction < _threshold)
                        goto nextVisited;
                }

                // add visible node to search results
                _nodes.Add(pair.Key);

            nextVisited:
                continue;
            }
        }

        #endregion
        #endregion
    }
}
