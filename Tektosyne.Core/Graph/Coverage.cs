using System;
using System.Collections.Generic;
using System.Diagnostics;

using Tektosyne.Collections;

namespace Tektosyne.Graph {

    /// <summary>
    /// Provides a path coverage algorithm for <see cref="IGraph2D{T}"/> instances.</summary>
    /// <typeparam name="T">
    /// The type of all nodes in the graph. If <typeparamref name="T"/> is a reference type, nodes
    /// cannot be null references.</typeparam>
    /// <remarks><para>
    /// The <b>Coverage</b> algorithm finds all <see cref="IGraph2D{T}"/> nodes that an <see
    /// cref="IGraphAgent{T}"/> can reach by any movement path that starts on a specified node, and
    /// whose total cost does not exceed a specified maximum cost.
    /// </para><para>
    /// The total path cost is defined as the sum of all <see cref="IGraphAgent{T}.GetStepCost"/>
    /// results for each movement step between two adjacent path nodes. Only nodes for which <see
    /// cref="IGraphAgent{T}.CanOccupy"/> succeeds are considered reachable.
    /// </para><note type="caution">
    /// Multi-step movements are problematic when the <see cref="IGraphAgent{T}"/> uses a
    /// non-trivial <see cref="IGraphAgent{T}.CanOccupy"/> condition. After a successful path
    /// search, all found <see cref="Coverage{T}.Nodes"/> are valid end points for a single-step
    /// movement. However, attempting to reach any <b>Nodes</b> element with multiple movement steps
    /// may prove impossible. Since the algorithm does not know whether a given intermediate path
    /// node will be occupied permanently or merely traversed, the path by which a <b>Nodes</b>
    /// element was reached may contain many intermediate nodes for which <see
    /// cref="IGraphAgent{T}.CanOccupy"/> fails.</note></remarks>

    [Serializable]
    public class Coverage<T> {
        #region Coverage(IGraph2D<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="Coverage{T}"/> class with the specified
        /// two-dimensional graph.</summary>
        /// <param name="graph">
        /// The <see cref="IGraph2D{T}"/> on which all searches are performed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="graph"/> is a null reference.</exception>

        public Coverage(IGraph2D<T> graph) {
            if (graph == null)
                ThrowHelper.ThrowArgumentNullException("graph");

            Graph = graph;
        }

        #endregion
        #region Private Fields

        // search arguments & results
        private IGraphAgent<T> _agent;
        private double _maxCost;
        private readonly ListEx<T> _nodes = new ListEx<T>(0);

        // hashtable holding minimum path costs
        private readonly Dictionary<T, Double> _pathCosts = new Dictionary<T, Double>(0);

        #endregion
        #region Agent

        /// <summary>
        /// Gets the <see cref="IGraphAgent{T}"/> for the last search.</summary>
        /// <value>
        /// The <see cref="IGraphAgent{T}"/> that was supplied to the last invocation of <see
        /// cref="FindReachable"/>. The default is a null reference.</value>

        public IGraphAgent<T> Agent {
            [DebuggerStepThrough]
            get { return _agent; }
        }

        #endregion
        #region Graph

        /// <summary>
        /// The <see cref="IGraph2D{T}"/> on which all searches are performed.</summary>

        public readonly IGraph2D<T> Graph;

        #endregion
        #region Nodes

        /// <summary>
        /// Gets a list of all <see cref="Graph"/> nodes that were reached by the last successful
        /// search.</summary>
        /// <value>
        /// A read-only <see cref="ListEx{Int32}"/> containing all <see cref="Graph"/> nodes that
        /// were reached by the last successful call to <see cref="FindReachable"/>, not including
        /// the source node.</value>
        /// <remarks>
        /// <b>Nodes</b> never returns a null reference, but it returns an empty collection if the
        /// last call to <see cref="FindReachable"/> returned <c>false</c>, or if the method has not
        /// yet been called.</remarks>

        public ListEx<T> Nodes {
            [DebuggerStepThrough]
            get { return _nodes.AsReadOnly(); }
        }

        #endregion
        #region FindReachable

        /// <summary>
        /// Finds all <see cref="Graph"/> nodes that the specified agent can reach from the
        /// specified node with the specified maximum path cost.</summary>
        /// <param name="agent">
        /// The <see cref="IGraphAgent{T}"/> that performs the movement.</param>
        /// <param name="source">
        /// The source node within <see cref="Graph"/> where the movement starts.</param>
        /// <param name="maxCost">
        /// The maximum total cost of the best path from the specified <paramref name="source"/> to
        /// any reachable <see cref="Graph"/> node.</param>
        /// <returns>
        /// <c>true</c> if one or more <see cref="Graph"/> nodes could be reached from the specified
        /// <paramref name="source"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="agent"/> or <paramref name="source"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxCost"/> is zero or negative.</exception>
        /// <remarks><para>
        /// <b>FindReachable</b> returns <c>false</c> if the specified <paramref name="source"/>
        /// node is invalid, or if there are no reachable nodes whose total path cost is equal to or
        /// less than the specified <paramref name="maxCost"/>.
        /// </para><para>
        /// Otherwise, <b>FindReachable</b> returns <c>true</c> and sets the <see cref="Nodes"/>
        /// property to the result of the path search. This collection contains only those reachable
        /// nodes for which <see cref="IGraphAgent{T}.CanOccupy"/> has succeeded.
        /// </para><para>
        /// If <see cref="IGraphAgent{T}.RelaxedRange"/> is <c>true</c> for the specified <paramref
        /// name="agent"/>, the <b>Nodes</b> collection includes those nodes whose total path cost
        /// exceeds <paramref name="maxCost"/>, but which can be reached from a neighbor whose total
        /// path cost is less than (but not equal to) <paramref name="maxCost"/>. These nodes are
        /// considered reachable regardless of their actual step costs.</para></remarks>

        public bool FindReachable(IGraphAgent<T> agent, T source, double maxCost) {
            if (agent == null)
                ThrowHelper.ThrowArgumentNullException("agent");
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");
            if (maxCost <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "maxCost", maxCost, Strings.ArgumentNotPositive);

            _agent = agent;
            _maxCost = maxCost;

            // clear previous results
            _nodes.Clear();

            // fail if source node is invalid
            if (!Graph.Contains(source)) return false;

            // mark source node as visited
            _pathCosts.Add(source, -1);

            // expand coverage to maxCost
            ExpandArea(source, 0);

            // clear intermediate data
            _pathCosts.Clear();

            // succeed if any nodes reached
            return (_nodes.Count > 0);
        }

        #endregion
        #region ExpandArea

        /// <summary>
        /// Expands the current coverage area with all neighbors of the specified <see
        /// cref="Graph"/> node that can be reached by the current <see cref="Agent"/>.</summary>
        /// <param name="node">
        /// The <see cref="Graph"/> node whose neighbors to examine.</param>
        /// <param name="cost">
        /// The total path cost to reach <paramref name="node"/>, measured as the sum of all <see
        /// cref="IGraphAgent{T}.GetStepCost"/> results for each movement step between neighboring
        /// nodes.</param>
        /// <remarks><para>
        /// <b>ExpandArea</b> recursively computes all possible movement paths for the current <see
        /// cref="Agent"/>, adding all valid nodes in any affordable path to the <see cref="Nodes"/>
        /// collection.
        /// </para><para>
        /// <b>ExpandArea</b> never revisits nodes that were already reached by a better path. The
        /// source node specified in the <see cref="FindReachable"/> call is never added to the
        /// <b>Nodes</b> collection.</para></remarks>

        private void ExpandArea(T node, double cost) {

            // get valid neighbors of current node
            IList<T> neighbors = Graph.GetNeighbors(node);

            // recurse into all valid neighbors
            for (int i = 0; i < neighbors.Count; i++) {
                T neighbor = neighbors[i];
                double minCost;

                // skip nodes with better path
                _pathCosts.TryGetValue(neighbor, out minCost);
                if (minCost != 0 && minCost <= cost)
                    continue;

                // skip unreachable nodes
                if (!_agent.CanMakeStep(node, neighbor))
                    continue;

                // get cost for next movement step
                double stepCost = _agent.GetStepCost(node, neighbor);
                Debug.Assert(stepCost > 0);

                // skip unaffordable nodes
                if (!_agent.RelaxedRange && cost + stepCost > _maxCost)
                    continue;

                // skip nodes with better path
                if (minCost != 0 && minCost <= cost + stepCost)
                    continue;

                // add newly reached neighbor if possible
                if (minCost == 0 && _agent.CanOccupy(neighbor))
                    _nodes.Add(neighbor);

                // store new minimum path cost
                _pathCosts[neighbor] = cost + stepCost;

                // visit neighbors if still affordable
                if (cost + stepCost < _maxCost)
                    ExpandArea(neighbor, cost + stepCost);
            }
        }

        #endregion
    }
}
