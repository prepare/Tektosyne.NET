using System;
using System.Diagnostics;

using Tektosyne.Collections;

namespace Tektosyne.Graph {

    /// <summary>
    /// Provides a node for <see cref="AStar{T}"/> search paths.</summary>
    /// <typeparam name="T">
    /// The type of all nodes in the graph. If <typeparamref name="T"/> is a reference type, nodes
    /// cannot be null references.</typeparam>
    /// <remarks>
    /// <b>PathNode</b> associates an <see cref="IGraph2D{T}"/> node with various other data
    /// required by the <see cref="AStar{T}"/> pathfinding algorithm.</remarks>

    [Serializable]
    public class PathNode<T> {
        #region PathNode(T, Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="PathNode{T}"/> class.</summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node that the <see cref="PathNode{T}"/> represents.
        /// </param>
        /// <param name="children">
        /// The initial capacity of the <see cref="Children"/> collection.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="children"/> is less than zero.</exception>
        /// <remarks>
        /// The specified <paramref name="children"/> should equal the <see
        /// cref="IGraph2D{T}.Connectivity"/> of the <see cref="IGraph2D{T}"/> instance on which the
        /// path search is performed, so as to avoid costly reallocations.</remarks>

        internal PathNode(T node, int children) {
            Node = node;
            _children = new ListEx<PathNode<T>>(children);
        }

        #endregion
        #region Internal Fields

        /// <summary>
        /// A <see cref="ListEx{T}"/> containing all accessible direct neighbors of the current <see
        /// cref="Node"/> that were examined during the path search.</summary>

        internal ListEx<PathNode<T>> _children;

        /// <summary>
        /// The total cost from the source node to the current <see cref="Node"/>.</summary>

        internal double _g;

        /// <summary>
        /// The estimated total cost from the current <see cref="Node"/> to the target node.
        /// </summary>

        internal double _h;

        /// <summary>
        /// The next <see cref="PathNode{T}"/> in a linked list, or a null reference for the last
        /// list node or an unlinked node.</summary>

        internal PathNode<T> _next;

        /// <summary>
        /// The parent of the <see cref="PathNode{T}"/>.</summary>

        internal PathNode<T> _parent;

        #endregion
        #region Children

        /// <summary>
        /// Gets a read-only view of the children of the <see cref="PathNode{T}"/>.</summary>
        /// <value>
        /// A read-only <see cref="ListEx{T}"/> containing all accessible direct neighbors of the
        /// current <see cref="Node"/> that were examined during the path search.</value>
        /// <remarks>
        /// The <see cref="AStar{T}"/> pathfinding algorithm adds elements to the <see
        /// cref="Children"/> collection while expanding the current search path.</remarks>

        public ListEx<PathNode<T>> Children {
            [DebuggerStepThrough]
            get { return _children.AsReadOnly(); }
        }

        #endregion
        #region F

        /// <summary>
        /// Gets the estimated total cost of the search path.</summary>
        /// <value>
        /// The sum of the <see cref="G"/> and <see cref="H"/> properties.</value>
        /// <remarks>
        /// <b>F</b> returns the estimated total cost of the path from source to target node that
        /// leads across the current <see cref="Node"/>.</remarks>

        public double F {
            get { return _g + _h; }
        }

        #endregion
        #region G

        /// <summary>
        /// Gets the total cost of the search path up to the current <see cref="Node"/>.</summary>
        /// <value>
        /// The total cost from the source node to the current <see cref="Node"/>.</value>
        /// <remarks><para>
        /// <b>G</b> is a known quantity that represents the total cost to move from the source node
        /// to the current <see cref="Node"/>, along the path defined by the chain of <see
        /// cref="Parent"/> values.
        /// </para><para>
        /// <b>G</b> is zero if the current <see cref="Node"/> is the source node.</para></remarks>

        public double G {
            [DebuggerStepThrough]
            get { return _g; }
        }

        #endregion
        #region H

        /// <summary>
        /// Gets the estimated total cost of the search path that begins with the current <see
        /// cref="Node"/>.</summary>
        /// <value>
        /// The estimated total cost from the current <see cref="Node"/> to the target node.</value>
        /// <remarks><para>
        /// <b>H</b> is an estimated quantity that represents the total cost to move from the
        /// current <see cref="Node"/> to the target node. This estimate is usually obtained by
        /// calling <see cref="IGraph2D{T}.GetDistance"/>.
        /// </para><para>
        /// <b>H</b> is zero if the current <see cref="Node"/> is the target node.</para></remarks>

        public double H {
            [DebuggerStepThrough]
            get { return _h; }
        }

        #endregion
        #region Node

        /// <summary>
        /// The <see cref="IGraph2D{T}"/> node that the <see cref="PathNode{T}"/> represents.
        /// </summary>
        /// <remarks>
        /// The <b>Node</b> of the first <see cref="PathNode{T}"/> in a search path is the source
        /// node, and the <b>Node</b> of the last <see cref="PathNode{T}"/> is the target node,
        /// assuming that pathfinding was successful.</remarks>

        public readonly T Node;

        #endregion
        #region Parent

        /// <summary>
        /// Gets the parent of the <see cref="PathNode{T}"/>.</summary>
        /// <value>
        /// The preceding <see cref="PathNode{T}"/> in a path that starts at the source node, or a
        /// null reference for the source node itself.</value>
        /// <remarks>
        /// Tracing back through the <b>Parent</b> properties of all <see cref="PathNode{T}"/>
        /// objects eventually leads back to the source node.</remarks>

        public PathNode<T> Parent {
            [DebuggerStepThrough]
            get { return _parent; }
        }

        #endregion
    }
}
