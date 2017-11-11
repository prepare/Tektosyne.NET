using System;
using System.Collections.Generic;
using System.Diagnostics;

using Tektosyne.Collections;

namespace Tektosyne.Graph {

    /// <summary>
    /// Provides a flood fill algorithm for <see cref="IGraph2D{T}"/> instances.</summary>
    /// <typeparam name="T">
    /// The type of all nodes in the graph. If <typeparamref name="T"/> is a reference type, nodes
    /// cannot be null references.</typeparam>
    /// <remarks>
    /// The <b>FloodFill</b> algorithm starts on a specified starting node, and recursively finds
    /// any adjacent nodes that match the conditions defined by a specified <see
    /// cref="Predicate{T}"/> delegate.</remarks>

    [Serializable]
    public class FloodFill<T> {
        #region FloodFill(IGraph2D<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="FloodFill{T}"/> class with the specified
        /// two-dimensional graph.</summary>
        /// <param name="graph">
        /// The <see cref="IGraph2D{T}"/> on which all searches are performed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="graph"/> is a null reference.</exception>

        public FloodFill(IGraph2D<T> graph) {
            if (graph == null)
                ThrowHelper.ThrowArgumentNullException("graph");

            Graph = graph;
        }

        #endregion
        #region Private Fields

        // search arguments & results
        private Predicate<T> _match;
        private readonly ListEx<T> _nodes = new ListEx<T>(0);

        // hash set holding visited nodes
        private readonly HashSet<T> _visited = new HashSet<T>();

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
        /// A read-only <see cref="ListEx{T}"/> containing all <see cref="Graph"/> nodes that were
        /// reached by the last successful call to <see cref="FindMatching"/>, not including the
        /// source node.</value>
        /// <remarks>
        /// <b>Nodes</b> never returns a null reference, but it returns an empty collection if the
        /// last call to <see cref="FindMatching"/> returned <c>false</c>, or if the method has not
        /// yet been called.</remarks>

        public ListEx<T> Nodes {
            [DebuggerStepThrough]
            get { return _nodes.AsReadOnly(); }
        }

        #endregion
        #region FindMatching

        /// <summary>
        /// Finds all contiguous <see cref="Graph"/> nodes that match the specified conditions,
        /// starting from the specified node.</summary>
        /// <param name="match">
        /// The <see cref="Predicate{T}"/> delegate that defines the conditions each <see
        /// cref="Graph"/> node must match.</param>
        /// <param name="source">
        /// The source node within <see cref="Graph"/> where the search starts.</param>
        /// <returns>
        /// <c>true</c> if one or more <see cref="Graph"/> nodes could be reached from the specified
        /// <paramref name="source"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="match"/> or <paramref name="source"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>FindMatching</b> returns <c>false</c> if the specified <paramref name="source"/> node
        /// is invalid, or if there are no contiguous nodes for which <paramref name="match"/>
        /// succeeds.
        /// </para><para>
        /// Otherwise, <b>FindMatching</b> returns <c>true</c> and sets the <see cref="Nodes"/>
        /// property to the result of the flood fill.</para></remarks>

        public bool FindMatching(Predicate<T> match, T source) {
            if (match == null)
                ThrowHelper.ThrowArgumentNullException("match");
            if (source == null)
                ThrowHelper.ThrowArgumentNullException("source");

            _match = match;

            // clear previous results
            _nodes.Clear();

            // fail if source node is invalid
            if (!Graph.Contains(source)) return false;

            // mark source node as visited
            _visited.Add(source);

            // expand area around source
            ExpandArea(source);

            // clear intermediate data
            _match = null;
            _visited.Clear();

            // succeed if any nodes reached
            return (_nodes.Count > 0);
        }

        #endregion
        #region ExpandArea

        /// <summary>
        /// Expands the current fill area with all neighbors of the specified <see cref="Graph"/>
        /// node for which the matching predicate succeeds.</summary>
        /// <param name="node">
        /// The <see cref="Graph"/> node whose neighbors to examine.</param>
        /// <remarks><para>
        /// <b>ExpandArea</b> recursively visits all contiguous nodes for which the matching
        /// predicate succeeds, and adds them to the <see cref="Nodes"/> collection.
        /// </para><para>
        /// <b>ExpandArea</b> never revisits nodes that were already added or rejected. The source
        /// node specified in the <see cref="FindMatching"/> call is never added to the <b>Nodes</b>
        /// collection.</para></remarks>

        private void ExpandArea(T node) {

            // get valid neighbors of current node
            IList<T> neighbors = Graph.GetNeighbors(node);

            // recurse into all valid neighbors
            for (int i = 0; i < neighbors.Count; i++) {
                T neighbor = neighbors[i];

                // skip visited nodes
                if (_visited.Contains(neighbor)) continue;
                _visited.Add(neighbor);

                // add match and visit neighbors
                if (_match(neighbor)) {
                    _nodes.Add(neighbor);
                    ExpandArea(neighbor);
                }
            }
        }

        #endregion
    }
}
