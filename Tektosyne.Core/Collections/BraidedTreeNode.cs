using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides a generic tree node within a <see cref="BraidedTree{TKey, TValue}"/>.</summary>
    /// <typeparam name="TKey">
    /// The type of the key. Keys cannot be null references.</typeparam>
    /// <typeparam name="TValue">
    /// The type of the value that is associated with the key. If <typeparamref name="TValue"/> is a
    /// reference type, the value may be null references.</typeparam>
    /// <remarks><para>
    /// <b>BraidedTreeNode</b> represents a key-and-value pair within a <see cref="BraidedTree{TKey,
    /// TValue}"/>. All references to neighboring <b>BraidedTreeNode</b> instances within the tree
    /// structure or sorting order are exposed as read-only properties.
    /// </para><para>
    /// <b>BraidedTreeNode</b> implements the <c>RandomizedNode</c> class by Michael J. Laszlo,
    /// <em>Computational Geometry and Computer Graphics in C++</em>, Prentice Hall 1996, p.55ff.
    /// </para></remarks>

    [Serializable]
    public class BraidedTreeNode<TKey, TValue> {
        #region BraidedTreeNode(BraidedTree<TKey, TValue>)

        /// <overloads>
        /// Initializes a new instance of the <see cref="BraidedTreeNode{TKey, TValue}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BraidedTreeNode{TKey, TValue}"/> class with
        /// the specified tree structure.</summary>
        /// <param name="tree">
        /// The <see cref="BraidedTree{TKey, TValue}"/> that contains the <see
        /// cref="BraidedTreeNode{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tree"/> is a null reference.</exception>
        /// <remarks>
        /// <see cref="Priority"/> is set to -1.0. The <see cref="Key"/> property remains at its
        /// default value, which is a null reference if <typeparamref name="TKey"/> is a reference
        /// type. Use this constructor only for the <see cref="BraidedTree{TKey, TValue}.RootNode"/>
        /// of the specified <paramref name="tree"/>.</remarks>

        internal BraidedTreeNode(BraidedTree<TKey, TValue> tree) {
            if (tree == null)
                ThrowHelper.ThrowArgumentNullException("tree");

            _tree = tree;
            _next = _previous = this;
            _priority = -1.0;
        }

        #endregion
        #region BraidedTreeNode(BraidedTree<TKey, TValue>, TKey, TValue)

        /// <summary>
        /// Initializes a new instance of the <see cref="BraidedTreeNode{TKey, TValue}"/> class with
        /// the specified tree structure, key and value.</summary>
        /// <param name="tree">
        /// The <see cref="BraidedTree{TKey, TValue}"/> that contains the <see
        /// cref="BraidedTreeNode{TKey, TValue}"/>.</param>
        /// <param name="key">
        /// The key of the <see cref="BraidedTreeNode{TKey, TValue}"/>.</param>
        /// <param name="value">
        /// The value of the <see cref="BraidedTreeNode{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tree"/> or <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="KeyValuePair{TKey, TValue}.Key"/> differs from the specified <paramref
        /// name="key"/>.</exception>
        /// <remarks>
        /// <see cref="Priority"/> is set to a random value in the open interval [0, 1).</remarks>

        internal BraidedTreeNode(BraidedTree<TKey, TValue> tree, TKey key, TValue value) {
            if (tree == null)
                ThrowHelper.ThrowArgumentNullException("tree");

            if (value is IKeyedValue<TKey>)
                CollectionsUtility.ValidateKey(Key, value);
            else if (key == null)
                ThrowHelper.ThrowArgumentNullException("key");

            _tree = tree;
            _next = _previous = this;
            _priority = tree.Random.NextDouble();
            Key = key; _value = value;
        }

        #endregion
        #region Private Fields

        /// <summary>
        /// The <see cref="BraidedTree{TKey, TValue}"/> that contains the <see
        /// cref="BraidedTreeNode{TKey, TValue}"/>.</summary>

        private BraidedTree<TKey, TValue> _tree;

        /// <summary>
        /// The balancing priority of the <see cref="BraidedTreeNode{TKey, TValue}"/>.</summary>

        private double _priority;

        #endregion
        #region Internal Fields

        /// <summary>
        /// The left descendant of the <see cref="BraidedTreeNode{TKey, TValue}"/> in the tree
        /// structure.</summary>

        internal BraidedTreeNode<TKey, TValue> _left;

        /// <summary>
        /// The next <see cref="BraidedTreeNode{TKey, TValue}"/> in the sorting order.</summary>

        internal BraidedTreeNode<TKey, TValue> _next;

        /// <summary>
        /// The parent of the <see cref="BraidedTreeNode{TKey, TValue}"/> in the tree structure.
        /// </summary>

        internal BraidedTreeNode<TKey, TValue> _parent;

        /// <summary>
        /// The previous <see cref="BraidedTreeNode{TKey, TValue}"/> in the sorting order.</summary>

        internal BraidedTreeNode<TKey, TValue> _previous;

        /// <summary>
        /// The right descendant of the <see cref="BraidedTreeNode{TKey, TValue}"/> in the tree
        /// structure.</summary>

        internal BraidedTreeNode<TKey, TValue> _right;

        /// <summary>
        /// The value of the <see cref="BraidedTreeNode{TKey, TValue}"/>.</summary>

        internal TValue _value;

        #endregion
        #region Key

        /// <summary>
        /// The key of the <see cref="BraidedTreeNode{TKey, TValue}"/>.</summary>
        /// <remarks><para>
        /// <b>Key</b> never returns a null reference, except for the <see cref="BraidedTree{TKey,
        /// TValue}.RootNode"/> of the containing <see cref="Tree"/>.
        /// </para><para>
        /// The <b>Key</b> of a <see cref="BraidedTreeNode{TKey, TValue}"/> determines its location
        /// within the containing <see cref="Tree"/>.</para></remarks>

        public readonly TKey Key;

        #endregion
        #region Left

        /// <summary>
        /// Gets the left descendant of the <see cref="BraidedTreeNode{TKey, TValue}"/> in the tree
        /// structure.</summary>
        /// <value>
        /// The left descendant of the <see cref="BraidedTreeNode{TKey, TValue}"/>, if any. The
        /// default is a null reference.</value>
        /// <remarks>
        /// The chains of <b>Left</b>, <see cref="Right"/>, and <see cref="Parent"/> references
        /// determine the structure of the containing <see cref="Tree"/>.</remarks>

        public BraidedTreeNode<TKey, TValue> Left {
            [DebuggerStepThrough]
            get { return _left; }
        }

        #endregion
        #region Next

        /// <summary>
        /// Gets the next <see cref="BraidedTreeNode{TKey, TValue}"/> in the sorting order.
        /// </summary>
        /// <value>
        /// The <see cref="BraidedTreeNode{TKey, TValue}"/> whose <see cref="Key"/> is sorted
        /// immediately after this instance, if any. The default is the current instance.</value>
        /// <remarks>
        /// The chains of <see cref="Previous"/> and <b>Next</b> references determine the sorting
        /// order within the containing <see cref="Tree"/>. Both chains begin and end with the
        /// tree’s <see cref="BraidedTree{TKey, TValue}.RootNode"/>.</remarks>

        public BraidedTreeNode<TKey, TValue> Next {
            [DebuggerStepThrough]
            get { return _next; }
        }

        #endregion
        #region Parent

        /// <summary>
        /// Gets the parent of the <see cref="BraidedTreeNode{TKey, TValue}"/> in the tree
        /// structure.</summary>
        /// <value>
        /// The parent of the <see cref="BraidedTreeNode{TKey, TValue}"/>, if any. The default is a
        /// null reference.</value>
        /// <remarks><para>
        /// The chains of <see cref="Left"/>, <see cref="Right"/>, and <b>Parent</b> references
        /// determine the structure of the containing <see cref="Tree"/>.
        /// </para><para>
        /// Every <see cref="BraidedTreeNode{TKey, TValue}"/> that has a valid <see cref="Tree"/>
        /// and <see cref="Key"/> also has a valid <b>Parent</b>. The <b>Parent</b> of the topmost
        /// valid node is the permanent <see cref="BraidedTree{TKey, TValue}.RootNode"/> of the 
        /// containing <see cref="Tree"/> which has neither <see cref="Key"/> nor <see
        /// cref="Value"/>.</para></remarks>

        public BraidedTreeNode<TKey, TValue> Parent {
            [DebuggerStepThrough]
            get { return _parent; }
        }

        #endregion
        #region Previous

        /// <summary>
        /// Gets the previous <see cref="BraidedTreeNode{TKey, TValue}"/> in the sorting order.
        /// </summary>
        /// <value>
        /// The <see cref="BraidedTreeNode{TKey, TValue}"/> whose <see cref="Key"/> is sorted
        /// immediately before this instance, if any. The default is the current instance.</value>
        /// <remarks>
        /// The chains of <b>Previous</b> and <see cref="Next"/> references determine the sorting
        /// order within the containing <see cref="Tree"/>. Both chains begin and end with the
        /// tree’s <see cref="BraidedTree{TKey, TValue}.RootNode"/>.</remarks>

        public BraidedTreeNode<TKey, TValue> Previous {
            [DebuggerStepThrough]
            get { return _previous; }
        }

        #endregion
        #region Priority

        /// <summary>
        /// Gets the priority of the <see cref="BraidedTreeNode{TKey, TValue}"/>.</summary>
        /// <value>
        /// The balancing priority of the <see cref="BraidedTreeNode{TKey, TValue}"/>.</value>
        /// <remarks>
        /// <b>Priority</b> usually returns a random value in the interval [0, 1] that is used to
        /// balance the containing <see cref="Tree"/>. The <b>Priority</b> of the <see
        /// cref="BraidedTree{TKey, TValue}.RootNode"/> is always -1.</remarks>

        public double Priority {
            [DebuggerStepThrough]
            get { return _priority; }
        }

        #endregion
        #region Right

        /// <summary>
        /// Gets the right descendant of the <see cref="BraidedTreeNode{TKey, TValue}"/> in the tree
        /// structure.</summary>
        /// <value>
        /// The right descendant of the <see cref="BraidedTreeNode{TKey, TValue}"/>, if any. The
        /// default is a null reference.</value>
        /// <remarks>
        /// The chains of <see cref="Left"/>, <b>Right</b>, and <see cref="Parent"/> references
        /// determine the structure of the containing <see cref="Tree"/>.</remarks>

        public BraidedTreeNode<TKey, TValue> Right {
            [DebuggerStepThrough]
            get { return _right; }
        }

        #endregion
        #region Tree

        /// <summary>
        /// Gets the <see cref="BraidedTree{TKey, TValue}"/> that contains the <see
        /// cref="BraidedTreeNode{TKey, TValue}"/>.</summary>
        /// <value>
        /// The <see cref="BraidedTree{TKey, TValue}"/> that contains the <see
        /// cref="BraidedTreeNode{TKey, TValue}"/>, if any; otherwise, a null reference.</value>
        /// <remarks>
        /// <b>Tree</b> is set to a valid <see cref="BraidedTree{TKey, TValue}"/> upon construction,
        /// and to a null reference when removed from that tree structure. Once removed, the <see
        /// cref="BraidedTreeNode{TKey, TValue}"/> cannot be added to another tree structure, so
        /// <b>Tree</b> remains a null reference.</remarks>

        public BraidedTree<TKey, TValue> Tree {
            [DebuggerStepThrough]
            get { return _tree; }
        }

        #endregion
        #region Value

        /// <summary>
        /// Gets or sets the value of the <see cref="BraidedTreeNode{TKey, TValue}"/>.</summary>
        /// <value>
        /// The value of the <see cref="BraidedTreeNode{TKey, TValue}"/>. The default is the default
        /// value for <typeparamref name="TValue"/>.</value>
        /// <exception cref="KeyMismatchException">
        /// The property is set to an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="KeyValuePair{TKey, TValue}.Key"/> differs from the current <see cref="Key"/>.
        /// </exception>

        public TValue Value {
            [DebuggerStepThrough]
            get { return _value; }
            set {
                if (value is IKeyedValue<TKey>)
                    CollectionsUtility.ValidateKey(Key, value);

                _value = value;
            }
        }

        #endregion
        #region Private Methods
        #region AddList

        /// <summary>
        /// Adds the specified <see cref="BraidedTreeNode{TKey, TValue}"/> to the linked list,
        /// sorted after the current instance.</summary>
        /// <param name="node">
        /// The <see cref="BraidedTreeNode{TKey, TValue}"/> to add.</param>
        /// <remarks>
        /// <b>AddList</b> updates the chains of <see cref="Previous"/> and <see cref="Next"/>
        /// references so that the specified <paramref name="node"/> is sorted immediately after the
        /// current instance.</remarks>

        private void AddList(BraidedTreeNode<TKey, TValue> node) {
            node._next = _next;
            node._previous = this;
            _next = node;
            node._next._previous = node;
        }

        #endregion
        #region RemoveList

        /// <summary>
        /// Removes the <see cref="BraidedTreeNode{TKey, TValue}"/> from the linked list.</summary>
        /// <remarks>
        /// <b>RemoveList</b> updates the chains of <see cref="Previous"/> and <see cref="Next"/>
        /// references to exclude the current instance.</remarks>

        private void RemoveList() {
            _previous._next = _next;
            _next._previous = _previous;
            _next = _previous = this;
        }

        #endregion
        #region RotateLeft

        /// <summary>
        /// Rotates the <see cref="BraidedTreeNode{TKey, TValue}"/> to the left within the tree
        /// structure.</summary>
        /// <remarks>
        /// <b>RotateLeft</b> implements the <c>rotateLeft</c> algorithm by Michael J. Laszlo,
        /// <em>Computational Geometry and Computer Graphics in C++</em>, Prentice Hall 1996, p.58.
        /// </remarks>

        private void RotateLeft() {
            BraidedTreeNode<TKey, TValue> x = this, y = x._right, p = x._parent;

            x._right = y._left;
            if (x._right != null) x._right._parent = x;
            if (p._left == x) p._left = y; else p._right = y;

            y._parent = p;
            y._left = x;
            x._parent = y;
        }

        #endregion
        #region RotateRight

        /// <summary>
        /// Rotates the <see cref="BraidedTreeNode{TKey, TValue}"/> to the right within the tree
        /// structure.</summary>
        /// <remarks>
        /// <b>RotateRight</b> implements the <c>rotateRight</c> algorithm by Michael J. Laszlo,
        /// <em>Computational Geometry and Computer Graphics in C++</em>, Prentice Hall 1996, p.58.
        /// </remarks>

        private void RotateRight() {
            BraidedTreeNode<TKey, TValue> y = this, x = y._left, p = y._parent;

            y._left = x._right;
            if (y._left != null) y._left._parent = y;
            if (p._right == y) p._right = x; else p._left = x;

            x._parent = p;
            x._right = y;
            y._parent = x;
        }

        #endregion
        #endregion
        #region Internal Methods
        #region AddTree

        /// <summary>
        /// Adds the specified <see cref="BraidedTreeNode{TKey, TValue}"/> to the tree structure, as
        /// a leaf node of the current instance.</summary>
        /// <param name="node">
        /// The <see cref="BraidedTreeNode{TKey, TValue}"/> to add.</param>
        /// <param name="isRight">
        /// <c>true</c> to add <paramref name="node"/> as the <see cref="Right"/> descendant;
        /// <c>false</c> to add <paramref name="node"/> as the <see cref="Left"/> descendant. The
        /// corresponding property must be a null reference.</param>
        /// <remarks>
        /// <b>AddTree</b> also sets the <see cref="Parent"/> reference of the specified <paramref
        /// name="node"/> to the current instance, and updates the chains of <see cref="Previous"/>
        /// and <see cref="Next"/> references to include <paramref name="node"/>.</remarks>

        internal void AddTree(BraidedTreeNode<TKey, TValue> node, bool isRight) {

            Debug.Assert(node._parent == null);
            node._parent = this;

            if (isRight) {
                Debug.Assert(_right == null);
                _right = node;
                AddList(node);
            } else {
                Debug.Assert(_left == null);
                _left = node;
                _previous.AddList(node);
            }
        }

        #endregion
        #region BubbleDown

        /// <summary>
        /// Moves the <see cref="BraidedTreeNode{TKey, TValue}"/> towards the leaves of the tree
        /// structure.</summary>
        /// <remarks><para>
        /// <b>BubbleDown</b> recursively rotates the current instance downward until both of its
        /// descendants are null references.
        /// </para><para>
        /// <b>BubbleDown</b> implements the <c>bubbleDown</c> algorithm by Michael J. Laszlo,
        /// <em>Computational Geometry and Computer Graphics in C++</em>, Prentice Hall 1996, p.59.
        /// </para></remarks>

        internal void BubbleDown() {
            double leftPriority, rightPriority;

            if (_left == null) {
                if (_right == null) return;
                rightPriority = _right._priority;
                leftPriority = 2.0;
            } else {
                rightPriority = (_right == null ? 2.0 : _right._priority);
                leftPriority = _left._priority;
            }

            if (leftPriority < rightPriority)
                RotateRight();
            else
                RotateLeft();

            BubbleDown();
        }

        #endregion
        #region BubbleUp

        /// <summary>
        /// Moves the <see cref="BraidedTreeNode{TKey, TValue}"/> towards the root of the tree
        /// structure.</summary>
        /// <remarks><para>
        /// <b>BubbleUp</b> recursively rotates the current instance upward until its <see
        /// cref="Priority"/> is not less than that of its <see cref="Parent"/>.
        /// </para><para>
        /// <b>BubbleDown</b> implements the <c>bubbleDown</c> algorithm by Michael J. Laszlo,
        /// <em>Computational Geometry and Computer Graphics in C++</em>, Prentice Hall 1996, p.59.
        /// </para></remarks>

        internal void BubbleUp() {
            if (_priority >= _parent._priority) return;

            if (_parent._left == this)
                _parent.RotateRight();
            else
                _parent.RotateLeft();

            BubbleUp();
        }

        #endregion
        #region Clear

        /// <summary>
        /// Clears all references to other <see cref="BraidedTreeNode{TKey, TValue}"/> instances.
        /// </summary>
        /// <remarks>
        /// <b>Clear</b> resets the <see cref="Previous"/> and <see cref="Next"/> properties to the
        /// current instance, and the <see cref="Left"/>, <see cref="Right"/> and <see
        /// cref="Parent"/>, properties to null references. Other properties remain unchanged,
        /// including the <see cref="Tree"/> property.</remarks>

        internal void Clear() {
            _next = _previous = this;
            _left = _right = _parent = null;
        }

        #endregion
        #region RemoveTree

        /// <summary>
        /// Removes the <see cref="BraidedTreeNode{TKey, TValue}"/>, which must be a leaf node, from
        /// the tree structure.</summary>
        /// <remarks><para>
        /// <b>RemoveTree</b> sets the <see cref="Left"/> or <see cref="Right"/> reference of the
        /// <see cref="Parent"/> node, whichever matches the current instance, to a null reference,
        /// and updates the chains of <see cref="Previous"/> and <see cref="Next"/> references to
        /// exclude the current instance.
        /// </para><para>
        /// All <see cref="BraidedTreeNode{TKey, TValue}"/> references of the current instance are
        /// reset to default values, as with <see cref="Clear"/>. Moreover, <b>RemoveTree</b> sets
        /// the <see cref="Tree"/> property to a null reference.</para></remarks>

        internal void RemoveTree() {
            Debug.Assert(_left == null && _right == null);

            if (_parent._left == this)
                _parent._left = null;
            else {
                Debug.Assert(_parent._right == this);
                _parent._right = null;
            }

            _parent = null;
            RemoveList();
            _tree = null;
        }

        #endregion
        #endregion
    }
}
