using System;
using System.Collections.Generic;
using System.Diagnostics;

using Tektosyne.Collections;
using Tektosyne.Geometry;

namespace Tektosyne.Graph {

    /// <summary>
    /// Provides an A* pathfinding algorithm for <see cref="IGraph2D{T}"/> instances.</summary>
    /// <typeparam name="T">
    /// The type of all nodes in the graph. If <typeparamref name="T"/> is a reference type, nodes
    /// cannot be null references.</typeparam>
    /// <remarks><para>
    /// The <b>AStar</b> algorithm finds the best path to move an <see cref="IGraphAgent{T}"/> from
    /// one specified <see cref="IGraph2D{T}"/> node to another. The "best path" is the one whose
    /// total cost is minimal compared to all other connecting paths, where the total cost of a path
    /// is defined as the sum of all <see cref="IGraphAgent{T}.GetStepCost"/> results for each step
    /// between two adjacent path nodes.
    /// </para><para>
    /// This implementation of the A* algorithm is based on the CAStar class created by James
    /// Matthews. The original source code supplemented his article "Basic A* Pathfinding Made
    /// Simple", pages 105-113 in "AI Game Programming Wisdom", Charles River Media, 2002.
    /// </para><note type="caution">
    /// Multi-step movements are problematic when the <see cref="IGraphAgent{T}"/> uses a
    /// non-trivial <see cref="IGraphAgent{T}.CanOccupy"/> condition. After a successful path
    /// search, <see cref="AStar{T}.BestNode"/> is always valid and optimal for a single-step
    /// movement. However, calling <see cref="AStar{T}.GetLastNode"/> with less than the maximum
    /// path cost may yield a suboptimal intermediate node, or none at all. Since the algorithm does
    /// not know whether a given intermediate path node will be occupied permanently or merely
    /// traversed, the final path may contain many intermediate nodes for which <see
    /// cref="IGraphAgent{T}.CanOccupy"/> fails.</note></remarks>

    [Serializable]
    public class AStar<T>: IGraphPath<T> {
        #region AStar(IGraph2D<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="AStar{T}"/> class with the specified
        /// two-dimensional graph.</summary>
        /// <param name="graph">
        /// The <see cref="IGraph2D{T}"/> on which all searches are performed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="graph"/> is a null reference.</exception>

        public AStar(IGraph2D<T> graph) {
            if (graph == null)
                ThrowHelper.ThrowArgumentNullException("graph");

            Graph = graph;
        }

        #endregion
        #region Private Fields

        // search arguments
        private IGraphAgent<T> _agent;
        private T _source, _target;
        private double _relativeLimit;
        private PointD _targetWorld;

        // search results
        private double _absoluteLimit;
        private PathNode<T> _bestNode;
        private readonly ListEx<T> _nodes = new ListEx<T>(0);

        // node lists and parent stack
        private PathNode<T> _openList;
        private readonly Stack<PathNode<T>> _parents = new Stack<PathNode<T>>(0);
        private readonly Dictionary<T, PathNode<T>>
            _openTable = new Dictionary<T, PathNode<T>>(0),
            _closedTable = new Dictionary<T, PathNode<T>>(0);

        #endregion
        #region Public Properties
        #region AbsoluteLimit

        /// <summary>
        /// Gets the absolute limit on the search radius for the last path search.</summary>
        /// <value><para>
        /// The maximum sum of the distances, according to <see cref="IGraph2D{T}.GetDistance"/>,
        /// between any one <see cref="Graph"/> node in the search path and the source and target
        /// nodes. Nodes that exceed this sum are not added to the search path.
        /// </para><para>-or-</para><para>
        /// Zero to indicate that there was no limit on the search radius. The default is zero.
        /// </para></value>
        /// <remarks>
        /// This property is set by <see cref="FindBestPath"/>, depending on the current value of
        /// <see cref="RelativeLimit"/>. Please see there for further details.</remarks>

        public double AbsoluteLimit {
            [DebuggerStepThrough]
            get { return _absoluteLimit; }
        }

        #endregion
        #region Agent

        /// <summary>
        /// Gets the <see cref="IGraphAgent{T}"/> for the last path search.</summary>
        /// <value>
        /// The <see cref="IGraphAgent{T}"/> that was supplied to the last invocation of <see
        /// cref="FindBestPath"/>. The default is a null reference.</value>

        public IGraphAgent<T> Agent {
            [DebuggerStepThrough]
            get { return _agent; }
        }

        #endregion
        #region BestNode

        /// <summary>
        /// Gets the final node found by the last successful path search.</summary>
        /// <value>
        /// The <see cref="PathNode{T}"/> that represents the target node of the last successful
        /// call to <see cref="FindBestPath"/>. This <see cref="PathNode{T}"/> ends the best path.
        /// </value>
        /// <remarks><para>
        /// <b>BestNode</b> returns a null reference if the last call to <see cref="FindBestPath"/>
        /// returned <c>false</c>, or if the method has not yet been called.
        /// </para><para>
        /// The best path ended by <b>BestNode</b> is returned by the <see cref="Nodes"/> property.
        /// It is found by backtracking through the <see cref="PathNode{T}.Parent"/> properties of
        /// all connected <see cref="PathNode{T}"/> objects.</para></remarks>

        public PathNode<T> BestNode {
            [DebuggerStepThrough]
            get { return _bestNode; }
        }

        #endregion
        #region Graph

        /// <summary>
        /// The <see cref="IGraph2D{T}"/> on which all searches are performed.</summary>

        public readonly IGraph2D<T> Graph;

        #endregion
        #region Nodes

        /// <summary>
        /// Gets a list of all <see cref="Graph"/> nodes in the best path found by the last
        /// successful path search.</summary>
        /// <value>
        /// A read-only <see cref="ListEx{T}"/> containing all <see cref="Graph"/> nodes that
        /// constitute the best movement path found by the last successful call to <see
        /// cref="FindBestPath"/>, including source and target node.</value>
        /// <remarks><para>
        /// <b>Nodes</b> never returns a null reference, but it returns an empty collection if the
        /// last call to <see cref="FindBestPath"/> returned <c>false</c>, or if the method has not
        /// yet been called.
        /// </para><para>
        /// The first element in <b>Nodes</b> is always the source node, and the last element is
        /// always the <see cref="PathNode{T}.Node"/> represented by the <see cref="BestNode"/>.
        /// </para></remarks>

        public IList<T> Nodes {
            get {
                // create path if necessary and possible
                if (_nodes.Count == 0 && _bestNode != null) {

                    // traverse along parent relationships
                    for (var cursor = _bestNode; cursor != null; cursor = cursor._parent)
                        _nodes.Add(cursor.Node);

                    // reverse path for standard ordering
                    _nodes.Reverse();
                }

                return _nodes.AsReadOnly();
            }
        }

        #endregion
        #region RelativeLimit

        /// <summary>
        /// Gets or sets the limit on the search radius, relative to the distance between source and
        /// target node.</summary>
        /// <value><para>
        /// The factor to multiply with the distance between source and target node, according to
        /// <see cref="IGraph2D{T}.GetDistance"/>, to obtain the <see cref="AbsoluteLimit"/> on the
        /// search radius.
        /// </para><para>-or-</para><para>
        /// Zero to indicate that there is no limit on the search radius. The default is zero.
        /// </para></value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The property is set to a negative value, or to a value that is greater than zero and
        /// less than one.</exception>
        /// <remarks><para>
        /// Setting <b>RelativeLimit</b> to a positive value restricts the search path to an
        /// elliptical area. The source and target nodes are the focal points of the ellipse, and
        /// <b>RelativeLimit</b> defines its inverse eccentricity.
        /// </para><para>
        /// Please refer to <see cref="AbsoluteLimit"/> and <see cref="FindBestPath"/> for further
        /// details.</para></remarks>

        public double RelativeLimit {
            [DebuggerStepThrough]
            get { return _relativeLimit; }
            set {
                if (value < 0.0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(
                        "value", value, Strings.ArgumentNegative);

                if (value > 0.0 && value < 1.0)
                    ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                        "value", value, Strings.ArgumentGreaterAndLess, 0, 1);

                _relativeLimit = value;
            }
        }

        #endregion
        #region TotalCost

        /// <summary>
        /// Gets the total cost of the best path found by the last successful path search.</summary>
        /// <value>
        /// The value of the <see cref="PathNode{T}.G"/> property of <see cref="BestNode"/>. This is
        /// the sum of the <see cref="IGraphAgent{T}.GetStepCost"/> results for all <see
        /// cref="Nodes"/>.</value>
        /// <remarks>
        /// <b>TotalCost</b> returns -1 if <see cref="BestNode"/> is a null reference. This is the
        /// case if the last call to <see cref="FindBestPath"/> returned <c>false</c>, or if the
        /// method has not yet been called.</remarks>

        public double TotalCost {
            get { return (_bestNode == null ? -1 : _bestNode._g); }
        }

        #endregion
        #region UseWorldDistance

        /// <summary>
        /// <c>true</c> if the <see cref="AStar{T}"/> algorithm should prefer path nodes with a
        /// minimal distance from the target node, in world coordinates; otherwise, <c>false</c>.
        /// The default is <c>false</c>.</summary>
        /// <remarks><para>
        /// If <b>UseWorldDistance</b> is <c>true</c>, candidate path nodes whose path costs are
        /// identical are also compared for their distances from the target node, using the world
        /// coordinates returned by <see cref="IGraph2D{T}.GetWorldLocation"/>.
        /// </para><para>
        /// This option eliminates zero-cost oscillations in the final path, creating a smoother and
        /// more "natural" course. However, the additional calculations and comparisons may slow
        /// down pathfinding. <b>UseWorldDistance</b> has no effect if <see
        /// cref="IGraph2D{T}.GetDistance"/> already uses world coordinates.</para></remarks>

        public bool UseWorldDistance;

        #endregion
        #endregion
        #region FindBestPath

        /// <summary>
        /// Finds the best path to move the specified agent from one specified <see cref="Graph"/>
        /// node to another.</summary>
        /// <param name="agent">
        /// The <see cref="IGraphAgent{T}"/> that performs the movement.</param>
        /// <param name="source">
        /// The source node within <see cref="Graph"/>.</param>
        /// <param name="target">
        /// The target node within <see cref="Graph"/>.</param>
        /// <returns>
        /// <c>true</c> if a best path between <paramref name="source"/> and <paramref
        /// name="target"/> could be found; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="agent"/>, <paramref name="source"/>, or <paramref name="target"/> is a
        /// null reference.</exception>
        /// <remarks><para>
        /// <b>FindBestPath</b> returns <c>false</c> if either of the specified <paramref
        /// name="source"/> and <paramref name="target"/> nodes is invalid, or if no connecting path
        /// could be found.
        /// </para><para>
        /// Otherwise, <b>FindBestPath</b> returns <c>true</c> and sets the <see cref="BestNode"/>,
        /// <see cref="Nodes"/>, and <see cref="TotalCost"/> properties to the results of the path
        /// search.
        /// </para><para>
        /// <b>FindBestPath</b> calls <see cref="IGraphAgent{T}.CanOccupy"/> and <see
        /// cref="IGraphAgent{T}.IsNearTarget"/> on the specified <paramref name="agent"/> to
        /// determine whether a given <see cref="BestNode"/> candidate is acceptable. Depending on
        /// the implementation of <b>IsNearTarget</b>, the <see cref="PathNode{T}.Node"/> of the
        /// final <see cref="BestNode"/> may differ from the specified <paramref name="target"/>,
        /// and possibly equal the specified <paramref name="source"/>. <b>CanOccupy</b> is never
        /// called on path nodes that match the <paramref name="source"/> node.
        /// </para><para>
        /// <b>FindBestPath</b> operates with a <em>restricted search radius</em> if <see
        /// cref="RelativeLimit"/> is greater than zero. In this case, <see cref="AbsoluteLimit"/>
        /// is set to the product (rounded up) of <b>RelativeLimit</b> and the distance between
        /// <paramref name="source"/> and <paramref name="target"/>. Whenever a node is considered
        /// for inclusion in the search path, its distances from <paramref name="source"/> and
        /// <paramref name="target"/> are calculated, and the node is ignored if the sum exceeds
        /// <b>AbsoluteLimit</b>.</para></remarks>

        public bool FindBestPath(IGraphAgent<T> agent, T source, T target) {
            if (agent == null)
                ThrowHelper.ThrowArgumentNullException("agent");
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");
            if (target == null)
                ThrowHelper.ThrowArgumentNullException("target");

            _agent = agent;
            _source = source;
            _target = target;

            // clear previous results
            _absoluteLimit = 0;
            _bestNode = null;
            _nodes.Clear();
            _targetWorld = PointD.Empty;

            // fail if either node is invalid
            if (!Graph.Contains(source) || !Graph.Contains(target))
                return false;

            // compute absolute distance limit if desired
            double distance = Graph.GetDistance(source, target);
            if (_relativeLimit > 0) _absoluteLimit = distance * _relativeLimit;

            // compute world distance to target if desired
            if (UseWorldDistance) _targetWorld = Graph.GetWorldLocation(target);

            // initialize search list
            _openList = new PathNode<T>(source, Graph.Connectivity);
            _openList._h = distance;

            bool success = false;
            while (SetBestNode()) {
                T node = _bestNode.Node;

                // succeed if occupation target is in range
                if (_agent.IsNearTarget(node, target, _bestNode._h) &&
                    (ComparerCache<T>.EqualityComparer.Equals(source, node)
                    || _agent.CanOccupy(node))) {

                    success = true;
                    break;
                }

                // add children to search space
                CreateChildren(_bestNode);
            }

            // clear intermediate data
            _source = _target = default(T);
            Debug.Assert(_parents.Count == 0);
            _openList = null;
            _openTable.Clear();
            _closedTable.Clear();

            return success;
        }

        #endregion
        #region GetLastNode

        /// <summary>
        /// Returns the last <see cref="Graph"/> node in the best path whose total path cost does
        /// not exceed the specified maximum cost.</summary>
        /// <param name="maxCost">
        /// The maximum total path cost of the returned <see cref="Graph"/> node.</param>
        /// <returns>
        /// The <see cref="PathNode{T}.Node"/> of the last <see cref="Nodes"/> element whose total
        /// path cost does not exceed the specified <paramref name="maxCost"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxCost"/> is zero or negative.</exception>
        /// <exception cref="PropertyValueException">
        /// <see cref="BestNode"/> is a null reference.</exception>
        /// <remarks>
        /// <b>GetLastNode</b> returns the <see cref="PathNode{T}.Node"/> associated with the <see
        /// cref="PathNode{T}"/> found by <see cref="GetLastPathNode"/> for the specified <paramref
        /// name="maxCost"/>. Please see there for details.</remarks>

        public T GetLastNode(double maxCost) {
            return GetLastPathNode(maxCost).Node;
        }

        #endregion
        #region GetLastPathNode

        /// <summary>
        /// Returns the last <see cref="PathNode{T}"/> in the best path whose total path cost does
        /// not exceed the specified maximum cost.</summary>
        /// <param name="maxCost">
        /// The maximum total path cost of the returned <see cref="PathNode{T}"/>.</param>
        /// <returns>
        /// The <see cref="PathNode{T}"/> that is the last parent of <see cref="BestNode"/> whose
        /// total path cost does not exceed the specified <paramref name="maxCost"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxCost"/> is zero or negative.</exception>
        /// <exception cref="PropertyValueException">
        /// <see cref="BestNode"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>GetLastNode</b> always returns a <see cref="PathNode{T}"/> whose <see
        /// cref="PathNode{T}.Node"/> is an element of <see cref="Nodes"/>. The exact element
        /// depends on the specified <paramref name="maxCost"/>.
        /// </para><para>
        /// <b>GetLastNode</b> searches for the <see cref="PathNode{T}"/> that is the last <see
        /// cref="PathNode{T}.Parent"/> of <see cref="BestNode"/> whose <see cref="PathNode{T}.G"/>
        /// value does not exceed the specified <paramref name="maxCost"/>, and for which <see
        /// cref="IGraphAgent{T}.CanOccupy"/> succeeds with the moving <see cref="Agent"/>.
        /// </para><para>
        /// If <see cref="IGraphAgent{T}.RelaxedRange"/> is <c>true</c> for the moving <see
        /// cref="Agent"/>, the <see cref="PathNode{T}.G"/> value of the returned <b>PathNode</b>
        /// may exceed <paramref name="maxCost"/> if the <see cref="PathNode{T}.G"/> value of its
        /// <see cref="PathNode{T}.Parent"/> node is strictly less than <paramref name="maxCost"/>.
        /// </para><para>
        /// If the specified <paramref name="maxCost"/> exceeds the cost of all nodes, or if
        /// <b>CanOccupy</b> fails for all affordable nodes, <b>GetLastNode</b> returns the <see
        /// cref="PathNode{T}"/> that corresponds to the first <see cref="Nodes"/> element, i.e. the
        /// source node of the path search.</para></remarks>

        public PathNode<T> GetLastPathNode(double maxCost) {
            if (maxCost <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "maxCost", maxCost, Strings.ArgumentNotPositive);

            if (_bestNode == null)
                ThrowHelper.ThrowPropertyValueException("BestNode", Strings.PropertyNull);

            /*
             * Go backward starting at BestNode and check these conditions:
             * 
             * 1. cursor.Parent is null -- we have arrived at the source node and have
             *    nowhere else to go, so we return the source node.
             * 
             * 2. cursor.G <= maxCost -- the current node is no more expensive than maxCost.
             * 
             * 3. cursor.Parent.G < maxCost -- Agent.RelaxedRange means we can enter any node
             *    if we have even one movement point left, so we return the current node.
             * 
             * 4. Agent.CanOccupy(cursor.Node) -- always check that the Agent’s movement
             *    can end on the current node.
             */

            PathNode<T> cursor = _bestNode;
            bool relaxed = _agent.RelaxedRange;

            while (true) {
                PathNode<T> parent = cursor._parent;
                if (parent == null) return cursor;

                if ((cursor._g <= maxCost || (relaxed && parent._g < maxCost))
                    && _agent.CanOccupy(cursor.Node))
                    return cursor;

                cursor = parent;
            }
        }

        #endregion
        #region Private Methods
        #region CreateChildren

        /// <summary>
        /// Adds all valid neighbors as children to the specified parent node.</summary>
        /// <param name="parent">
        /// The <see cref="PathNode{T}"/> whose neighbors to examine.</param>

        private void CreateChildren(PathNode<T> parent) {
            T source = parent.Node;

            // compute direct neighbors of parent node
            IList<T> neighbors = Graph.GetNeighbors(source);

            // link all children that can be reached
            for (int i = 0; i < neighbors.Count; i++) {
                T neighbor = neighbors[i];

                if (_agent.CanMakeStep(source, neighbor))
                    LinkChild(parent, neighbor);
            }
        }

        #endregion
        #region GetWorldDistance

        /// <summary>
        /// Returns the squared world distance between the specified <see cref="PathNode{T}"/> and
        /// the target node.</summary>
        /// <param name="node">
        /// The <see cref="PathNode{T}"/> whose distance to the target node to compute.</param>
        /// <returns>
        /// The squared distance, in world coordinates, from the specified <paramref name="node"/>
        /// to the current target node.</returns>

        private double GetWorldDistance(PathNode<T> node) {

            PointD nodeWorld = Graph.GetWorldLocation(node.Node);
            double x = nodeWorld.X - _targetWorld.X;
            double y = nodeWorld.Y - _targetWorld.Y;

            return x * x + y * y;
        }

        #endregion
        #region LinkChild

        /// <summary>
        /// Links the specified child node to the specified parent node.</summary>
        /// <param name="parent">
        /// The parent node to link with <paramref name="graphChild"/>.</param>
        /// <param name="graphChild">
        /// The child node within <see cref="Graph"/> to link with <paramref name="parent"/>.
        /// </param>
        /// <remarks>
        /// <b>LinkChild</b> searches the open and closed tables for the specified <paramref
        /// name="graphChild"/>, and creates a new open list node if no matching node was found.
        /// </remarks>

        private void LinkChild(PathNode<T> parent, T graphChild) {

            // total cost to reach child via parent
            double g = parent._g + _agent.GetStepCost(parent.Node, graphChild);
            Debug.Assert(g > parent._g);

            // look for child node in open table
            PathNode<T> child;
            if (_openTable.TryGetValue(graphChild, out child)) {
                parent._children.Add(child);

                // switch to better route
                if (child._g > g) {
                    child._g = g;
                    child._parent = parent;
                }
                return;
            }

            // look for child node in closed table
            if (_closedTable.TryGetValue(graphChild, out child)) {
                parent._children.Add(child);

                // switch to better route
                if (child._g > g) {
                    child._g = g;
                    child._parent = parent;

                    // also update parents
                    UpdateParents(child);
                }
                return;
            }

            // compute distance from target
            double fromTarget = Graph.GetDistance(graphChild, _target);

            // apply distance limit if desired
            if (_absoluteLimit > 0) {

                // compute distance from source
                double fromSource = Graph.GetDistance(_source, graphChild);

                // ignore child if sum exceeds limit
                if (fromSource + fromTarget > _absoluteLimit)
                    return;
            }

            // create new path node for child
            child = new PathNode<T>(graphChild, Graph.Connectivity);
            parent._children.Add(child);

            child._g = g;
            child._h = fromTarget;
            child._parent = parent;

            // prepend child to open list & table
            child._next = _openList;
            _openList = child;
            _openTable[graphChild] = child;
        }

        #endregion
        #region SetBestNode

        /// <summary>
        /// Sets the <see cref="BestNode"/> property to the open list node with the lowest <see
        /// cref="PathNode{T}.F"/> value.</summary>
        /// <returns>
        /// <c>true</c> if the open list contained another node that was removed and stored as the
        /// new value of the <see cref="BestNode"/> property; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <see cref="BestNode"/> is a null reference exactly if <b>SetBestNode</b> returns
        /// <c>false</c>.</remarks>

        private bool SetBestNode() {

            // no nodes left to search
            if (_openList == null) {
                _bestNode = null;
                return false;
            }

            // get first open list node
            _bestNode = _openList;
            double bestF = _bestNode.F;
            PathNode<T> previous = null;

            if (UseWorldDistance) {
                double bestDistance = GetWorldDistance(_bestNode);

                // walk remaining list to find better node
                for (PathNode<T> preCursor = _openList, cursor = preCursor._next;
                    cursor != null; preCursor = cursor, cursor = cursor._next) {

                    double cursorDistance = GetWorldDistance(cursor);

                    // update to new best node
                    if (cursor.F < bestF ||
                        (cursor.F == bestF && cursorDistance < bestDistance)) {

                        previous = preCursor;
                        _bestNode = cursor;
                        bestF = cursor.F;
                        bestDistance = cursorDistance;
                    }
                }
            } else {
                // walk remaining list to find better node
                for (PathNode<T> preCursor = _openList, cursor = preCursor._next;
                    cursor != null; preCursor = cursor, cursor = cursor._next) {

                    // update to new best node
                    if (cursor.F < bestF) {
                        previous = preCursor;
                        _bestNode = cursor;
                        bestF = cursor.F;
                    }
                }
            }

            // remove best node from open list
            if (previous == null)
                _openList = _openList._next;
            else
                previous._next = _bestNode._next;

            // move best node to closed table
            T graphNode = _bestNode.Node;
            _openTable.Remove(graphNode);
            _closedTable[graphNode] = _bestNode;

            return true;
        }

        #endregion
        #region UpdateParents

        /// <summary>
        /// Updates all parents of all children of the specified node to reflect lowered path costs.
        /// </summary>
        /// <param name="node">
        /// A closed list node that has been reached by a new path.</param>
        /// <remarks>
        /// All possible paths are represented by <see cref="PathNode{T}.Parent"/> links through the
        /// current <see cref="PathNode{T}"/> collection. If an existing node has been reached by a
        /// new and cheaper path, the cost for all paths that involve this node must be updated
        /// accordingly.</remarks>

        private void UpdateParents(PathNode<T> node) {
            Debug.Assert(_parents.Count == 0);

            // specified node is first parent
            _parents.Push(node);

            // continue while we have parents
            while (_parents.Count > 0) {
                PathNode<T> parent = _parents.Pop();

                // check total costs of all children
                var children = parent._children;
                for (int i = 0; i < children.Count; i++) {
                    PathNode<T> child = children[i];

                    // skip children with better costs
                    if (child._g <= parent._g) continue;

                    // total cost to reach child via parent
                    double g = parent._g + _agent.GetStepCost(parent.Node, child.Node);
                    Debug.Assert(g > parent._g);

                    // switch to better route
                    if (child._g > g) {
                        child._g = g;
                        child._parent = parent;

                        // child is next parent
                        _parents.Push(child);
                    }
                }
            }
        }

        #endregion
        #endregion
    }
}
