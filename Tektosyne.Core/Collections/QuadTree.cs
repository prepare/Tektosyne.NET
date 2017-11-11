using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Tektosyne.Geometry;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides a generic collection of <see cref="PointD"/> keys and arbitrary values that are
    /// sorted in two dimensions using a quadrant tree.</summary>
    /// <typeparam name="TValue">
    /// The type of all values that are associated with the keys. If <typeparamref name="TValue"/>
    /// is a reference type, values may be null references.</typeparam>
    /// <remarks><para>
    /// <b>QuadTree</b> provides a two-dimensional search tree with <see cref="PointD"/> keys. The
    /// root node covers a specified rectangle (not necessarily a square), and each child node
    /// recursively covers one quadrant of its parent rectangle. Each leaf node holds one or more
    /// key-and-value pairs in a hashtable. Internal nodes hold no data.
    /// </para><list type="bullet"><item>
    /// The tree structure is exposed through the <see cref="QuadTreeNode{TValue}"/> class. You can
    /// find the node associated with any given key, or with any given tree level and quadrant grid
    /// coordinates, and follow a reference to its four descendants or parent node.
    /// </item><item>
    /// All tree nodes have a unique signature that doubles as their key in a hashtable, providing
    /// fast tree-wide enumeration and O(1) access to any node with a given level and quadrant grid
    /// coordinates. <see cref="QuadTree{TValue}.FindNode(PointD)"/> exploits this fact for a depth
    /// probe algorithm that greatly shortens search times in large trees.
    /// </item><item>
    /// <see cref="QuadTree{TValue}.FindRange"/> performs a two-dimensional range search that finds
    /// all elements within a given key range.
    /// </item></list><para>
    /// Like other <b>Tektosyne.Collections</b> classes, <b>QuadTree</b> also provides the following
    /// extra features:
    /// </para><list type="bullet"><item>
    /// The <see cref="IKeyedValue{TKey}.Key"/> property of any <typeparamref name="TValue"/> that
    /// implements the <see cref="IKeyedValue{TKey}"/> interface is automatically checked against
    /// the associated dictionary key when a key or value is changed or inserted.
    /// </item><item>
    /// <see cref="QuadTree{TValue}.Copy"/> creates a deep copy of the collection by invoking <see
    /// cref="ICloneable.Clone"/> on each value. This feature requires that all copied values
    /// implement the <see cref="ICloneable"/> interface.
    /// </item><item>
    /// <see cref="QuadTree{TValue}.Equals"/> compares two collections with identical element types
    /// for value equality of all elements. The collections compare as equal if they contain the
    /// same elements. The enumeration order of elements is ignored since the <b>QuadTree</b> class
    /// does not establish any fixed element ordering.
    /// </item></list><para>
    /// <b>QuadTree</b> was inspired by the <c>Quadtree</c> class by Michael J. Laszlo,
    /// <em>Computational Geometry and Computer Graphics in C++</em>, Prentice Hall 1996, p.231ff.
    /// </para></remarks>

    [Serializable]
    public class QuadTree<TValue>: IDictionary<PointD, TValue>, ICollection, ICloneable {
        #region QuadTree(RectD)

        /// <overloads>
        /// Initializes a new instance of the <see cref="QuadTree{TValue}"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTree{TValue}"/> class that is empty and
        /// has the specified bounds.</summary>
        /// <param name="bounds">
        /// The bounds of all keys in the <see cref="QuadTree{TValue}"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bounds"/> contains a <see cref="RectD.Width"/> or <see
        /// cref="RectD.Height"/> that is equal to or less than zero.</exception>
        /// <remarks>
        /// <see cref="Capacity"/> defaults to 128.</remarks>

        public QuadTree(RectD bounds): this(bounds, 128) { }

        #endregion
        #region QuadTree(RectD, Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTree{TValue}"/> class that is empty and
        /// has the specified bounds and leaf node capacity.</summary>
        /// <param name="bounds">
        /// The bounds of all keys in the <see cref="QuadTree{TValue}"/>.</param>
        /// <param name="capacity">
        /// The <see cref="Capacity"/> of all leaf <see cref="Nodes"/> above <see
        /// cref="MaxLevel"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="bounds"/> contains a <see cref="RectD.Width"/> or <see
        /// cref="RectD.Height"/> that is equal to or less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="capacity"/> is equal to or less than zero.</para></exception>

        public QuadTree(RectD bounds, int capacity) {
            if (bounds.Width <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "bounds.Width", bounds.Width, Strings.ArgumentNotPositive);

            if (bounds.Height <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "bounds.Height", bounds.Height, Strings.ArgumentNotPositive);

            if (capacity <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "capacity", capacity, Strings.ArgumentNegative);

            Bounds = bounds;
            Capacity = capacity;
            _nodes = new Int32Dictionary<QuadTreeNode<TValue>>();
            RootNode = new QuadTreeNode<TValue>(this, 0, null, bounds);
        }

        #endregion
        #region QuadTree(RectD, IDictionary<PointD, TValue>)

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTree{TValue}"/> class that has the
        /// specified bounds and contains elements copied from the specified collection.</summary>
        /// <param name="bounds">
        /// The bounds of all keys in the <see cref="QuadTree{TValue}"/>.</param>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{PointD, TValue}"/> whose elements are copied to the new
        /// collection.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dictionary"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="bounds"/> contains a <see cref="RectD.Width"/> or <see
        /// cref="RectD.Height"/> that is equal to or less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="dictionary"/> contains an element whose <see cref="KeyValuePair{PointD,
        /// TValue}.Key"/> is outside of <paramref name="bounds"/>.</para></exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="dictionary"/> contains an element whose <see cref="KeyValuePair{PointD,
        /// TValue}.Value"/> is an <see cref="IKeyedValue{PointD}"/> instance whose <see
        /// cref="IKeyedValue{PointD}.Key"/> differs from the associated <see
        /// cref="KeyValuePair{PointD, TValue}.Key"/>.</exception>
        /// <remarks>
        /// <see cref="Capacity"/> defaults to 128. This constructor calls <see cref="AddRange"/> to
        /// add all elements in the specified <paramref name="dictionary"/> to the new <see
        /// cref="QuadTree{TValue}"/>.</remarks>

        public QuadTree(RectD bounds, IDictionary<PointD, TValue> dictionary): this(bounds, 128) {
            AddRange(dictionary);
        }

        #endregion
        #region Private Fields

        /// <summary>
        /// The number of key-and-value pairs contained in the <see cref="QuadTree{TValue}"/>.
        /// </summary>

        private int _count;

        /// <summary>Backs the <see cref="Keys"/> property.</summary>

        [NonSerialized]
        private KeyCollection _keys;

        /// <summary>Backs the <see cref="Values"/> property.</summary>

        [NonSerialized]
        private ValueCollection _values;

        /// <summary>
        /// The status of the last depth probe conducted by <see cref="FindNode(PointD)"/>.</summary>

        [NonSerialized]
        private ProbeStatus _probe;

        #endregion
        #region Internal Fields

        /// <summary>
        /// The hashtable containing all <see cref="QuadTreeNode{TValue}"/> instances that are
        /// attached to the <see cref="QuadTree{TValue}"/>.</summary>

        internal readonly Int32Dictionary<QuadTreeNode<TValue>> _nodes;

        #endregion
        #region Capacity

        /// <summary>
        /// The maximum capacity for the <see cref="QuadTreeNode{T}.Data"/> collection of all leaf
        /// <see cref="Nodes"/> above <see cref="MaxLevel"/>.</summary>
        /// <remarks>
        /// <b>Capacity</b> usually indicates the maximum number of elements in the <see
        /// cref="QuadTreeNode{T}.Data"/> collection of a <see cref="QuadTreeNode{T}"/>. However,
        /// <b>Capacity</b> is ignored and the collection size is unbounded if the node’s <see
        /// cref="QuadTreeNode{T}.Level"/> equals <see cref="MaxLevel"/>.</remarks>

        public readonly int Capacity;

        #endregion
        #region MaxLevel

        /// <summary>
        /// The maximum level for any <see cref="QuadTree{TValue}"/>.</summary>
        /// <remarks><para>
        /// <b>MaxLevel</b> specifies the zero-based index of the deepest level in any <see
        /// cref="QuadTree{TValue}"/>. The maximum total number of levels therefore equals
        /// <b>MaxLevel</b> + 1, including the <see cref="RootNode"/> at level zero.
        /// </para><para>
        /// When a <see cref="QuadTreeNode{TValue}"/> is created on <b>MaxLevel</b>, the maximum
        /// <see cref="Capacity"/> is ignored so that its <see cref="QuadTreeNode{TValue}.Data"/>
        /// collection may grow unbounded.
        /// </para><para>
        /// <b>MaxLevel</b> is fixed at 14 so that each <see cref="QuadTreeNode{TValue}"/> can be
        /// uniquely identified by a 32-bit <see cref="QuadTreeNode{TValue}.Signature"/> containing
        /// a bitwise combination of the following indices:
        /// </para><list type="bullet"><item>
        /// The lowest 4 bits contain the node’s <see cref="QuadTreeNode{TValue}.Level"/>.
        /// </item><item>
        /// The middle 14 bits contain the node’s <see cref="QuadTreeNode{TValue}.GridX"/> index.
        /// </item><item>
        /// The highest 14 bits contain the node’s <see cref="QuadTreeNode{TValue}.GridY"/> index.
        /// </item></list><para>
        /// With a <b>MaxLevel</b> of 14, the deepest level can hold 16,384 x 16,384 nodes, and the
        /// entire <see cref="QuadTree{TValue}"/> can hold 357,913,941 nodes.</para></remarks>

        public const int MaxLevel = 14;

        #endregion
        #region ProbeLevel

        /// <summary>
        /// The minimum level at which <see cref="FindNode(PointD)"/> begins using a depth probe.
        /// </summary>
        /// <remarks><para>
        /// <see cref="FindNode(PointD)"/> switches from a normal tree search to a heuristic depth
        /// probe algorithm when the number of <see cref="Nodes"/> in the <see
        /// cref="QuadTree{TValue}"/> reaches 4^<b>ProbeLevel</b>, indicating that a large
        /// proportion of <see cref="Nodes"/> resides at or below that level.
        /// </para><para>
        /// <b>ProbeLevel</b> is currently fixed at four, so that the depth probe starts at 256 <see
        /// cref="Nodes"/>. <b>ProbeLevel</b> cannot be less than two since the depth probe ascends
        /// two levels at a time.</para></remarks>

        public const int ProbeLevel = 4;

        #endregion
        #region Public Properties
        #region Bounds

        /// <summary>
        /// The bounds of all keys in the <see cref="QuadTree{TValue}"/>.</summary>
        /// <remarks><para>
        /// <b>Bounds</b> holds a <see cref="RectD"/> whose area covers all <see cref="PointD"/> keys
        /// in the <see cref="QuadTree{TValue}"/>. <b>Bounds</b> always has a positive <see
        /// cref="RectD.Width"/> and <see cref="RectD.Height"/>.
        /// </para><para>
        /// The <see cref="QuadTree{TValue}"/> always divides both dimensions into the same number
        /// of grid cells at each <see cref="QuadTreeNode{TValue}.Level"/>, but the dimensions do
        /// not have to be equal.</para></remarks>

        public readonly RectD Bounds;

        #endregion
        #region Count

        /// <summary>
        /// Gets the number of key-and-value pairs contained in the <see cref="QuadTree{TValue}"/>.
        /// </summary>
        /// <value>
        /// The number of <see cref="KeyValuePair{PointD, TValue}"/> elements contained in the <see
        /// cref="QuadTree{TValue}"/>.</value>
        /// <remarks>
        /// <b>Count</b> returns a counter value maintained by the <see cref="Add"/> and <see
        /// cref="Remove"/> methods. Accessing this property is therefore an O(1) operation.
        /// </remarks>

        public int Count {
            [DebuggerStepThrough]
            get { return _count; }
        }

        #endregion
        #region IsFixedSize

        /// <summary>
        /// Gets a value indicating whether the <see cref="QuadTree{TValue}"/> has a fixed size.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="QuadTree{TValue}"/> has a fixed size; otherwise,
        /// <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="IDictionary.IsFixedSize"/> for details.
        /// </para><para>
        /// This property always returns the same value as the <see cref="IsReadOnly"/> property
        /// since any fixed-size <see cref="QuadTree{TValue}"/> is also read-only, and vice versa.
        /// </para></remarks>

        public bool IsFixedSize {
            [DebuggerStepThrough]
            get { return IsReadOnly; }
        }

        #endregion
        #region IsReadOnly

        /// <summary>
        /// Gets a value indicating whether the <see cref="QuadTree{TValue}"/> is read-only.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="QuadTree{TValue}"/> is read-only; otherwise, <c>false</c>.
        /// The default is <c>false</c>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="IDictionary.IsReadOnly"/> for details.
        /// </para><para>
        /// <b>IsReadOnly</b> always returns <c>false</c> since the <see cref="QuadTree{TValue}"/>
        /// class does not offer a read-only wrapper.</para></remarks>

        public bool IsReadOnly {
            [DebuggerStepThrough]
            get { return false; }
        }

        #endregion
        #region IsSynchronized

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="QuadTree{TValue}"/> is
        /// synchronized (thread-safe).</summary>
        /// <value>
        /// <c>true</c> if access to the <see cref="QuadTree{TValue}"/> is synchronized
        /// (thread-safe); otherwise, <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="ICollection.IsSynchronized"/> for details.
        /// </para><para>
        /// This property is provided for backwards compatibility only, as none of the
        /// <b>Tektosyne.Collections</b> classes support synchronized wrappers.</para></remarks>

        bool ICollection.IsSynchronized {
            [DebuggerStepThrough]
            get { return false; }
        }

        #endregion
        #region Item

        /// <summary>
        /// Gets or sets the value associated with the specified key.</summary>
        /// <param name="key">
        /// The key whose value to get or set.</param>
        /// <value><para>
        /// The value associated with the specified <paramref name="key"/>.
        /// </para><para>
        /// If <paramref name="key"/> is not found, attempting to get it throws a <see
        /// cref="KeyNotFoundException"/>, and attempting to set it adds a new element with the
        /// specified key and value to the <see cref="QuadTree{TValue}"/>.</para></value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The property is set, and <paramref name="key"/> is outside of <see cref="Bounds"/>.
        /// </exception>
        /// <exception cref="KeyMismatchException">
        /// The property is set to an <see cref="IKeyedValue{PointD}"/> instance whose <see
        /// cref="KeyValuePair{PointD, TValue}.Key"/> differs from the specified <paramref
        /// name="key"/>.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is read, and <paramref name="key"/> does not exist in the <see
        /// cref="QuadTree{TValue}"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="QuadTree{TValue}"/> is read-only.</exception>
        /// <remarks>
        /// This indexer calls <see cref="FindNode(PointD)"/> to find the <see
        /// cref="QuadTreeNode{TValue}"/> for the specified <paramref name="key"/>, but may then
        /// create one or more child nodes before adding a new element.</remarks>

        public TValue this[PointD key] {
            get {
                var node = FindNode(key);
                if (node == null || node._data == null)
                    ThrowHelper.ThrowKeyNotFoundException(key);

                TValue value;
                if (!node._data.TryGetValue(key, out value))
                    ThrowHelper.ThrowKeyNotFoundException(key);

                return value;
            }
            set {
                // CheckWritable();
                // ValidateKey is performed by Int32Dictionary
                // CollectionsUtility.ValidateKey(key, value);

                var node = FindNode(key);
                if (node == null)
                    ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                        "key", key, Strings.ArgumentPropertyConflict, "Bounds");

                if (node._data != null && node._data.ContainsKey(key))
                    node._data[key] = value;
                else
                    AddToNode(node, key, value);
            }
        }

        #endregion
        #region Keys

        /// <summary>
        /// Gets an <see cref="ICollection{PointD}"/> containing the keys in the <see
        /// cref="QuadTree{TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="ICollection{PointD}"/> containing the keys in the <see
        /// cref="QuadTree{TValue}"/>.</value>
        /// <remarks>
        /// <b>Keys</b> returns the keys stored with each attached <see
        /// cref="QuadTreeNode{TValue}"/> in the same order as <see cref="GetEnumerator"/>.
        /// </remarks>

        public ICollection<PointD> Keys {
            [DebuggerStepThrough]
            get {
                if (_keys == null)
                    _keys = new KeyCollection(this);

                return _keys;
            }
        }

        #endregion
        #region Nodes

        /// <summary>
        /// Gets a read-only view of the hashtable containing all <see cref="QuadTreeNode{TValue}"/>
        /// instances that are attached to the <see cref="QuadTree{TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="Int32Dictionary{TValue}"/> that maps <see
        /// cref="QuadTreeNode{TValue}.Signature"/> values to the corresponding <see
        /// cref="QuadTreeNode{TValue}"/> instances.</value>
        /// <remarks><para>
        /// <b>Nodes</b> always contains at least one element associated with a <see
        /// cref="QuadTreeNode{TValue}.Signature"/> of zero, which is the permanent <see
        /// cref="RootNode"/> of the <see cref="QuadTree{TValue}"/>.
        /// </para><para>
        /// <b>Nodes</b> generally contains fewer than <see cref="Count"/> elements since leaf nodes
        /// may contain up to <see cref="Capacity"/> key-and-value pairs, or more if they reside on
        /// <see cref="MaxLevel"/>.
        /// </para><para>
        /// <see cref="GetEnumerator"/> and the enumerators of the <see cref="Keys"/> and <see
        /// cref="Values"/> collections iterate over <b>Nodes</b>.</para></remarks>

        public Int32Dictionary<QuadTreeNode<TValue>> Nodes {
            [DebuggerStepThrough]
            get { return _nodes.AsReadOnly(); }
        }

        #endregion
        #region RootNode

        /// <summary>
        /// The <see cref="QuadTreeNode{TValue}"/> that is the root of the <see
        /// cref="QuadTree{TValue}"/>.</summary>
        /// <remarks><para>
        /// The <b>RootNode</b> is never removed from the <see cref="QuadTree{TValue}"/>. An empty
        /// <see cref="QuadTree{TValue}"/> contains only the <b>RootNode</b>. The <b>RootNode</b> is
        /// the only node whose <see cref="QuadTreeNode{TValue}.Data"/> collection may be empty when
        /// it has no descendants.
        /// </para><para>
        /// All other nodes are descendants of the <b>RootNode</b>. The chain of <see
        /// cref="QuadTreeNode{TValue}.Parent"/> references from any other node of the <see
        /// cref="QuadTree{TValue}"/> ends in the <b>RootNode</b>, whose <see
        /// cref="QuadTreeNode{TValue}.Parent"/> property is always a null reference.
        /// </para></remarks>

        public readonly QuadTreeNode<TValue> RootNode;

        #endregion
        #region SyncRoot

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see
        /// cref="QuadTree{TValue}"/>.</summary>
        /// <value>
        /// An object that can be used to synchronize access to the <see cref="QuadTree{TValue}"/>.
        /// </value>
        /// <remarks><para>
        /// Please refer to <see cref="ICollection.SyncRoot"/> for details.
        /// </para><para>
        /// When synchronizing multi-threaded access to the <see cref="QuadTree{TValue}"/>, obtain a
        /// lock on the <b>SyncRoot</b> object rather than the collection itself. A read-only view
        /// always returns the same <b>SyncRoot</b> object as the underlying writable collection.
        /// </para></remarks>

        public object SyncRoot {
            [DebuggerStepThrough]
            get { return RootNode; }
        }

        #endregion
        #region Values

        /// <summary>
        /// Gets an <see cref="ICollection{TValue}"/> containing the values in the <see
        /// cref="QuadTree{TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="ICollection{TValue}"/> containing the values in the <see
        /// cref="QuadTree{TValue}"/>.</value>
        /// <remarks>
        /// <b>Values</b> returns the values stored with each attached <see
        /// cref="QuadTreeNode{TValue}"/> in the same order as <see cref="GetEnumerator"/>.
        /// </remarks>

        public ICollection<TValue> Values {
            [DebuggerStepThrough]
            get {
                if (_values == null)
                    _values = new ValueCollection(this);

                return _values;
            }
        }

        #endregion
        #endregion
        #region Protected Methods
        #region AddToNode

        /// <summary>
        /// Adds the specified element to the subtree starting with the specified <see
        /// cref="QuadTreeNode{TValue}"/>.</summary>
        /// <param name="node">
        /// The <see cref="QuadTreeNode{TValue}"/> at the top of the subtree. The containing <see
        /// cref="QuadTreeNode{TValue}.Tree"/> of this argument must equal the current instance.
        /// </param>
        /// <param name="key">
        /// The key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add.</param>
        /// <remarks><para>
        /// <b>AddToNode</b> adds the specified <paramref name="key"/> and <paramref name="value"/>
        /// to the first <see cref="QuadTreeNode{TValue}"/> that has sufficient capacity in the
        /// subtree starting with the specified <paramref name="node"/>. The subtree is extended
        /// with new child nodes as needed.
        /// </para><para>
        /// <b>AddToNode</b> does not check whether <paramref name="key"/> already exists in the
        /// <see cref="QuadTree{TValue}"/>, or whether the <see cref="QuadTreeNode{TValue}.Bounds"/>
        /// of <paramref name="node"/> contain <paramref name="key"/>. The caller must ensure that
        /// these conditions hold.</para></remarks>

        protected void AddToNode(QuadTreeNode<TValue> node, PointD key, TValue value) {
            Debug.Assert(node.Tree == this);

            while (node.Level < MaxLevel && !node.HasCapacity) {
                if (node._data != null) node.Split();
                node = node.FindOrCreateChild(key);
            }

            node._data.Add(key, value);
            ++_count;
        }

        #endregion
        #region CheckTargetArray

        /// <summary>
        /// Checks the bounds of the specified array and the specified starting index against the
        /// size of the <see cref="QuadTree{TValue}"/>.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination for elements copied from
        /// the <see cref="QuadTree{TValue}"/>. The <b>Array</b> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="ArgumentException"><para>
        /// <paramref name="array"/> is multidimensional.
        /// </para><para>-or-</para><para>
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref
        /// name="array"/>.
        /// </para><para>-or-</para><para>
        /// The number of elements in the <see cref="QuadTree{TValue}"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.</para></exception>

        protected void CheckTargetArray(Array array, int arrayIndex) {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException("array");
            if (array.Rank > 1)
                ThrowHelper.ThrowArgumentException("array", Strings.ArgumentMultidimensional);

            // skip length checks for empty collection and index zero
            if (arrayIndex == 0 && _count == 0) return;

            if (arrayIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "arrayIndex", arrayIndex, Strings.ArgumentNegative);

            if (arrayIndex >= array.Length)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "arrayIndex", arrayIndex, Strings.ArgumentNotLessValue, array.Length);

            if (_count > array.Length - arrayIndex)
                ThrowHelper.ThrowArgumentException("array", Strings.ArgumentSectionLessCollection);
        }

        #endregion
        #endregion
        #region Public Methods
        #region Add(PointD, TValue)

        /// <overloads>
        /// Adds the specified element to the <see cref="QuadTree{TValue}"/>.</overloads>
        /// <summary>
        /// Adds the specified key and value to the <see cref="QuadTree{TValue}"/>.</summary>
        /// <param name="key">
        /// The key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> already exists in the <see cref="QuadTree{TValue}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="key"/> is outside of <see cref="Bounds"/>.</exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="value"/> is an <see cref="IKeyedValue{PointD}"/> instance whose <see
        /// cref="IKeyedValue{PointD}.Key"/> differs from <paramref name="key"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="QuadTree{TValue}"/> is read-only.</exception>
        /// <remarks>
        /// <b>Add</b> calls <see cref="FindNode(PointD)"/> to find the <see
        /// cref="QuadTreeNode{TValue}"/> for the specified <paramref name="key"/>, but may then 
        /// create one or more child nodes before adding the new element.</remarks>

        public void Add(PointD key, TValue value) {
            // CheckWritable();
            // ValidateKey is performed by Int32Dictionary
            // CollectionsUtility.ValidateKey(key, value);

            var node = FindNode(key);
            if (node == null)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "key", key, Strings.ArgumentPropertyConflict, "Bounds");

            if (node._data != null && node._data.ContainsKey(key))
                ThrowHelper.ThrowArgumentException("key", Strings.ArgumentInCollection);

            AddToNode(node, key, value);
        }

        #endregion
        #region Add(KeyValuePair<PointD, TValue>)

        /// <summary>
        /// Adds the specified key-and-value pair to the <see cref="QuadTree{TValue}"/>.</summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{PointD, TValue}"/> element to add.</param>
        /// <exception cref="ArgumentException">
        /// The <see cref="KeyValuePair{PointD, TValue}.Key"/> component of <paramref name="pair"/>
        /// already exists in the <see cref="QuadTree{TValue}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <see cref="KeyValuePair{PointD, TValue}.Key"/> component of <paramref name="pair"/>
        /// is outside of <see cref="Bounds"/>.</exception>
        /// <exception cref="KeyMismatchException">
        /// The <see cref="KeyValuePair{PointD, TValue}.Value"/> component of <paramref name="pair"/>
        /// is an <see cref="IKeyedValue{PointD}"/> instance whose <see
        /// cref="IKeyedValue{PointD}.Key"/> differs from the associated <see
        /// cref="KeyValuePair{PointD, TValue}.Key"/> component.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="QuadTree{TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Add(PointD, TValue)"/> for details.</remarks>

        public void Add(KeyValuePair<PointD, TValue> pair) {
            Add(pair.Key, pair.Value);
        }

        #endregion
        #region AddRange

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="QuadTree{TValue}"/>.
        /// </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{PointD, TValue}"/> whose elements to add.</param>
        /// <exception cref="ArgumentException"><para>
        /// The <see cref="QuadTree{PointD}"/> already contains one or more keys in the specified
        /// <paramref name="dictionary"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="dictionary"/> contains one or more duplicate keys.</para></exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dictionary"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="dictionary"/> contains an element whose <see cref="KeyValuePair{PointD,
        /// TValue}.Key"/> is outside of <see cref="Bounds"/>.</exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="dictionary"/> contains an element whose <see cref="KeyValuePair{PointD,
        /// TValue}.Value"/> is an <see cref="IKeyedValue{PointD}"/> instance whose <see
        /// cref="IKeyedValue{PointD}.Key"/> differs from the associated <see
        /// cref="KeyValuePair{PointD, TValue}.Key"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="QuadTree{TValue}"/> is read-only.</exception>
        /// <remarks>
        /// <b>AddRange</b> calls <see cref="Add"/> for each <see cref="KeyValuePair{PointD,
        /// TValue}"/> in the specified <paramref name="dictionary"/>.</remarks>

        public void AddRange(IDictionary<PointD, TValue> dictionary) {
            // CheckWritable();
            if (dictionary == null)
                ThrowHelper.ThrowArgumentNullException("dictionary");

            foreach (var pair in dictionary)
                Add(pair.Key, pair.Value);
        }

        #endregion
        #region Clear

        /// <summary>
        /// Removes all elements from the <see cref="QuadTree{TValue}"/>.</summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="QuadTree{TValue}"/> is read-only.</exception>
        /// <remarks><para>
        /// <b>Clear</b> resets <see cref="Count"/> to zero and removes any other <see
        /// cref="QuadTreeNode{TValue}"/> instances attached to the permanent <see
        /// cref="RootNode"/>. This is an O(1) operation.
        /// </para><note type="caution">
        /// Any removed <see cref="QuadTreeNode{TValue}"/> instances are <em>not</em> cleared,
        /// rendering all their tree structure properties invalid.</note></remarks>

        public void Clear() {
            // CheckWritable();
            RootNode.Clear();
            _count = 0;

            _nodes.Clear();
            _nodes.Add(0, RootNode);
        }

        #endregion
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="QuadTree{TValue}"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="QuadTree{TValue}"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> does not necessarily preserve the enumeration order of the <see
        /// cref="QuadTree{TValue}"/>. The tree structure is preserved, however.</remarks>

        public virtual object Clone() {
            return new QuadTree<TValue>(Bounds, this);
        }

        #endregion
        #region Contains

        /// <summary>
        /// Determines whether the <see cref="QuadTree{TValue}"/> contains the specified
        /// key-and-value pair.</summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{PointD, TValue}"/> element to locate.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="pair"/> is found in the <see cref="QuadTree{TValue}"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> succeeds if <see cref="FindNode(PointD)"/> finds the <see
        /// cref="KeyValuePair{PointD, TValue}.Key"/> of the specified <paramref name="pair"/>, and
        /// the resulting <see cref="QuadTreeNode{TValue}"/> also contains its <see
        /// cref="KeyValuePair{PointD, TValue}.Value"/>.</remarks>

        public bool Contains(KeyValuePair<PointD, TValue> pair) {
            var node = FindNode(pair.Key);
            return (node != null && node._data != null && node._data.Contains(pair));
        }

        #endregion
        #region ContainsKey(PointD)

        /// <overloads>
        /// Determines whether the <see cref="QuadTree{TValue}"/> contains the specified key.
        /// </overloads>
        /// <summary>
        /// Determines whether the <see cref="QuadTree{TValue}"/> contains the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="QuadTree{TValue}"/> contains an element with the specified
        /// <paramref name="key"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>ContainsKey</b> succeeds if <see cref="FindNode(PointD)"/> finds a <see
        /// cref="QuadTreeNode{TValue}"/> that contains the specified <paramref name="key"/>.
        /// </remarks>

        public bool ContainsKey(PointD key) {
            QuadTreeNode<TValue> node = FindNode(key);
            return (node != null && node._data != null && node._data.ContainsKey(key));
        }

        #endregion
        #region ContainsKey(PointD, QuadTreeNode<TValue>)

        /// <summary>
        /// Determines whether the <see cref="QuadTree{TValue}"/> contains the specified key, 
        /// searching the specified <see cref="QuadTreeNode{TValue}"/> first.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <param name="node"><para>
        /// An optional <see cref="QuadTreeNode{TValue}"/> to search for <paramref name="key"/>
        /// before conducting a full tree search.
        /// </para><para>
        /// On return, contains the result of <see cref="FindNode(PointD)"/> for <paramref
        /// name="key"/>, which may be a null reference.</para></param>
        /// <returns>
        /// <c>true</c> if the <see cref="QuadTree{TValue}"/> contains an element with the specified
        /// <paramref name="key"/>; otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>ContainsKey</b> succeeds if <see cref="FindNode(PointD)"/> finds a <see
        /// cref="QuadTreeNode{TValue}"/> that contains the specified <paramref name="key"/>.
        /// </para><para>
        /// If the specified <paramref name="node"/> is a valid leaf node that contains <paramref
        /// name="key"/>, <b>ContainsKey</b> succeeds without calling <see cref="FindNode(PointD)"/>
        /// and leaves <paramref name="node"/> unchanged. This reduces <b>ContainsKey</b> to an O(1)
        /// operation.</para></remarks>

        public bool ContainsKey(PointD key, ref QuadTreeNode<TValue> node) {

            if (node != null && node._data != null && node._data.ContainsKey(key))
                return true;

            node = FindNode(key);
            return (node != null && node._data != null && node._data.ContainsKey(key));
        }

        #endregion
        #region ContainsValue(TValue)

        /// <overloads>
        /// Determines whether the <see cref="QuadTree{PointD}"/> contains the specified value.
        /// </overloads>
        /// <summary>
        /// Determines whether the <see cref="QuadTree{PointD}"/> contains the specified value.
        /// </summary>
        /// <param name="value">
        /// The value to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="QuadTree{PointD}"/> contains an element with the specified
        /// <paramref name="value"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>ContainsValue</b> succeeds if <see cref="FindNodeByValue"/> finds a <see
        /// cref="QuadTreeNode{TValue}"/> that contains the specified <paramref name="value"/>.
        /// </remarks>

        public bool ContainsValue(TValue value) {
            return (FindNodeByValue(value) != null);
        }

        #endregion
        #region ContainsValue(TValue, QuadTreeNode<TValue>)

        /// <summary>
        /// Determines whether the <see cref="QuadTree{PointD}"/> contains the specified value, 
        /// searching the specified <see cref="QuadTreeNode{TValue}"/> first.</summary>
        /// <param name="value">
        /// The value to locate.</param>
        /// <param name="node"><para>
        /// An optional <see cref="QuadTreeNode{TValue}"/> to search for <paramref name="value"/>
        /// before conducting a full tree search.
        /// </para><para>
        /// On return, contains the result of <see cref="FindNodeByValue"/> for <paramref
        /// name="value"/>, which may be a null reference.</para></param>
        /// <returns>
        /// <c>true</c> if the <see cref="QuadTree{PointD}"/> contains an element with the specified
        /// <paramref name="value"/>; otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>ContainsValue</b> succeeds if <see cref="FindNodeByValue"/> finds a <see
        /// cref="QuadTreeNode{TValue}"/> that contains the specified <paramref name="value"/>.
        /// </para><para>
        /// If the specified <paramref name="node"/> is a valid leaf node that contains <paramref
        /// name="value"/>, <b>ContainsValue</b> succeeds without calling <see
        /// cref="FindNodeByValue"/> and leaves <paramref name="node"/> unchanged. This reduces
        /// <b>ContainsValue</b> to an O(k) operation, where k is the number of key-and-value pairs
        /// stored in <paramref name="node"/>.</para></remarks>

        public bool ContainsValue(TValue value, ref QuadTreeNode<TValue> node) {

            if (node != null && node._data != null && node._data.ContainsValue(value))
                return true;

            node = FindNodeByValue(value);
            return (node != null);
        }

        #endregion
        #region Copy

        /// <summary>
        /// Creates a deep copy of the <see cref="QuadTree{TValue}"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="QuadTree{TValue}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="TValue"/> does not implement <see cref="ICloneable"/>.</exception>
        /// <remarks><para>
        /// <b>Copy</b> is similar to <see cref="Clone"/> but creates a deep copy the <see
        /// cref="QuadTree{TValue}"/> by invoking <see cref="ICloneable.Clone"/> on all 
        /// <typeparamref name="TValue"/> values. The <see cref="PointD"/> keys are always duplicated
        /// by a shallow copy.
        /// </para><para>
        /// <b>Copy</b> does not necessarily preserve the enumeration order of the <see
        /// cref="QuadTree{TValue}"/>. The tree structure is preserved, however.</para></remarks>

        public QuadTree<TValue> Copy() {
            QuadTree<TValue> copy = new QuadTree<TValue>(Bounds);

            foreach (var pair in this) {
                TValue value = pair.Value;

                ICloneable cloneable = (ICloneable) value;
                if (cloneable != null)
                    value = (TValue) cloneable.Clone();

                copy.Add(pair.Key, value);
            }

            return copy;
        }

        #endregion
        #region CopyTo

        /// <summary>
        /// Copies the entire <see cref="QuadTree{TValue}"/> to a one-dimensional <see
        /// cref="Array"/>, starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the <see
        /// cref="KeyValuePair{PointD, TValue}"/> elements copied from the <see
        /// cref="QuadTree{TValue}"/>. The <b>Array</b> must have zero-based indexing.</param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="ArgumentException"><para>
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref
        /// name="array"/>.
        /// </para><para>-or-</para><para>
        /// The number of elements in the source <see cref="QuadTree{TValue}"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.</para></exception>
        /// <remarks>
        /// <b>CopyTo</b> copies elements using the <see cref="GetEnumerator"/> sequence.</remarks>

        public void CopyTo(KeyValuePair<PointD, TValue>[] array, int arrayIndex) {
            CheckTargetArray(array, arrayIndex);
            foreach (var pair in this)
                array[arrayIndex++] = pair;
        }

        /// <summary>
        /// Copies the entire <see cref="QuadTree{TValue}"/> to a one-dimensional <see
        /// cref="Array"/>, starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the <see
        /// cref="KeyValuePair{PointD, TValue}"/> elements copied from the <see
        /// cref="QuadTree{TValue}"/>. The <b>Array</b> must have zero-based indexing.</param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="ArgumentException"><para>
        /// <paramref name="array"/> is multidimensional.
        /// </para><para>-or-</para><para>
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref
        /// name="array"/>.
        /// </para><para>-or-</para><para>
        /// The number of elements in the source <see cref="QuadTree{TValue}"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.</para></exception>
        /// <exception cref="InvalidCastException">
        /// <see cref="KeyValuePair{PointD, TValue}"/> cannot be cast automatically to the type of
        /// the destination <paramref name="array"/>.</exception>

        void ICollection.CopyTo(Array array, int arrayIndex) {
            CopyTo((KeyValuePair<PointD, TValue>[]) array, arrayIndex);
        }

        #endregion
        #region Equals

        /// <summary>
        /// Determines whether the specified collection contains the same key-and-value pairs as the
        /// current <see cref="QuadTree{TValue}"/>.</summary>
        /// <param name="collection">
        /// The <see cref="ICollection{T}"/> of <see cref="KeyValuePair{PointD, TValue}"/> elements
        /// to compare with the current <see cref="QuadTree{TValue}"/>.</param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="collection"/> is another reference to this <see
        /// cref="QuadTree{TValue}"/>.
        /// </item><item>
        /// <paramref name="collection"/> is not a null reference, contains the same number of
        /// elements as this <see cref="QuadTree{TValue}"/>, and each element compares as equal to
        /// the element with the same <see cref="KeyValuePair{PointD, TValue}.Key"/>.
        /// </item></list></returns>
        /// <remarks><para>
        /// <b>Equals</b> iterates over the specified <paramref name="collection"/> and calls <see
        /// cref="Contains"/> for each element to test the two collections for value equality.
        /// </para><para>
        /// <b>Equals</b> does not attempt to compare the enumeration order of both collections as
        /// the <see cref="QuadTree{TValue}"/> class does not define a fixed enumeration order.
        /// </para></remarks>

        public bool Equals(ICollection<KeyValuePair<PointD, TValue>> collection) {

            if (collection == this) return true;
            if (collection == null || collection.Count != _count)
                return false;

            foreach (var pair in collection)
                if (!Contains(pair)) return false;

            return true;
        }

        #endregion
        #region FindNode(Int32, Int32, Int32)

        /// <overloads>
        /// Finds a <see cref="QuadTreeNode{TValue}"/> within the <see cref="QuadTree{TValue}"/>.
        /// </overloads>
        /// <summary>
        /// Finds the <see cref="QuadTreeNode{TValue}"/> at the specified level and grid coordinates
        /// within the <see cref="QuadTree{TValue}"/>.</summary>
        /// <param name="level">
        /// The <see cref="QuadTreeNode{TValue}.Level"/> to search.</param>
        /// <param name="gridX">
        /// The <see cref="QuadTreeNode{TValue}.GridX"/> coordinate to locate.</param>
        /// <param name="gridY">
        /// The <see cref="QuadTreeNode{TValue}.GridY"/> coordinate to locate.</param>
        /// <returns>
        /// The <see cref="QuadTreeNode{TValue}"/> at the specified <paramref name="gridX"/> and
        /// <paramref name="gridY"/> coordinates on the specified <paramref name="level"/>, if
        /// valid; otherwise, a null reference.</returns>
        /// <remarks><para>
        /// <b>FindNode</b> returns a null reference in the following cases:
        /// </para><list type="bullet"><item>
        /// The specified <paramref name="level"/> is less than zero or greater than <see
        /// cref="MaxLevel"/>.
        /// </item><item>
        /// The specified <paramref name="gridX"/> and/or <paramref name="gridY"/> coordinate is
        /// less than zero, or equal to or greater than the number of grid cells in the
        /// corresponding dimension at <paramref name="level"/>.
        /// </item><item>
        /// All three arguments are valid, but no <see cref="QuadTreeNode{TValue}"/> exists at the
        /// specified location within the <see cref="QuadTree{TValue}"/>.
        /// </item></list><para>
        /// <b>FindNode</b> combines the specified arguments into the unique <see
        /// cref="QuadTreeNode{TValue}.Signature"/> of the desired <see
        /// cref="QuadTreeNode{TValue}"/> which is used retrieve the node from the <see
        /// cref="Nodes"/> hashtable. This is an O(1) operation.</para></remarks>

        public QuadTreeNode<TValue> FindNode(int level, int gridX, int gridY) {

            // check if level is valid
            if (level < 0 || level > MaxLevel)
                return null;

            // check if grid coordinates are valid for level
            int sideCount = (1 << level);
            if (gridX < 0 || gridX >= sideCount || gridY < 0 || gridY >= sideCount)
                return null;

            // compose signature from level and grid coordinates
            int signature = unchecked(level | (gridX << 4) | (gridY << 18));

            QuadTreeNode<TValue> node;
            _nodes.TryGetValue(signature, out node);
            return node;
        }

        #endregion
        #region FindNode(PointD)

        /// <summary>
        /// Finds the <see cref="QuadTreeNode{TValue}"/> within the <see cref="QuadTree{TValue}"/>
        /// that should contain the specified key.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns><para>
        /// The <see cref="QuadTreeNode{TValue}"/> whose <see cref="QuadTreeNode{TValue}.Bounds"/>
        /// contain the specified <paramref name="key"/>, regardless of whether its <see
        /// cref="QuadTreeNode{TValue}.Data"/> already contains <paramref name="key"/> as well.
        /// </para><para>-or-</para><para>
        /// A null reference if the specified <paramref name="key"/> is outside the <see
        /// cref="Bounds"/> of the entire <see cref="QuadTree{TValue}"/>.</para></returns>
        /// <remarks><para>
        /// <b>FindNode</b> performs a range search within the <see cref="QuadTree{TValue}"/> for
        /// the specified <paramref name="key"/>, starting with the <see cref="RootNode"/>. This is
        /// usually an O(log m) operation where m is the number of <see cref="Nodes"/>.
        /// </para><para>
        /// If <see cref="Nodes"/> contains at least 4^<see cref="ProbeLevel"/> elements,
        /// <b>FindNode</b> first probes a deeper level of the <see cref="QuadTree{TValue}"/> to
        /// rapidly approach the <see cref="QuadTreeNode{TValue}"/> containing the specified
        /// <paramref name="key"/>.
        /// </para><para>
        /// The probe begins at level log4 m, where m is the number of <see cref="Nodes"/>, and
        /// ascends two levels at a time until we are either above <see cref="ProbeLevel"/> or have
        /// found a valid <see cref="QuadTreeNode{TValue}"/> at the grid coordinates that contain
        /// <paramref name="key"/>. <b>FindNode</b> then performs a regular tree search for the
        /// desired leaf node.
        /// </para><para>
        /// The depth probe is derived from a binary depth search algorithm given by Sariel
        /// Har-Peled in his (as yet) unpublished lecture on 17 March 2010, <em>Quadtrees –
        /// Hierarchical Grids</em>. This lecture is part of the series <a
        /// href="http://valis.cs.uiuc.edu/~sariel/teach/notes/aprx/">Approximation Algorithms in
        /// Geometry</a>.</para></remarks>

        public QuadTreeNode<TValue> FindNode(PointD key) {

            if (!Bounds.Contains(key)) return null;
            QuadTreeNode<TValue> node;

            // check if depth probe is worthwhile
            int count = (_nodes.Count >> (ProbeLevel << 1));
            if (count == 0)
                node = RootNode;
            else {
                // try to reuse last probe data
                int level = _probe.Level;
                int gridCount = (1 << level);

                if (count != _probe.NodeCount) {
#if QUADTREE_BITMASK
                    // determine probe level (favors higher node counts)
                    level = ((count & 0xF00000) != 0 ? 14 :
                            ((count & 0xFC0000) != 0 ? 13 :
                            ((count & 0xFF0000) != 0 ? 12 :
                            ((count & 0xFFC000) != 0 ? 11 :
                            ((count & 0xFFF000) != 0 ? 10 :
                            ((count & 0xFFFC00) != 0 ? 9 :
                            ((count & 0xFFFF00) != 0 ? 8 :
                            ((count & 0xFFFFC0) != 0 ? 7 :
                            ((count & 0xFFFFF0) != 0 ? 6 :
                            ((count & 0xFFFFFC) != 0 ? 5 : 4))))))))));
#else
                    // determine probe level (favors lower node counts)
                    for (level = 1; (count >> (level << 1)) > 0; ++level) ;
                    level += (ProbeLevel - 1);
                    if (level > MaxLevel) level = MaxLevel;
#endif
                    _probe.Level = level;
                    _probe.NodeCount = count;

                    // compute grid divisions at specified level
                    gridCount = (1 << level);
                    _probe.GridWidth = Bounds.Width / gridCount;
                    _probe.GridHeight = Bounds.Height / gridCount;
                }

                // compute grid coordinates for specified point
                int gridX = (int) ((key.X - Bounds.Left) / _probe.GridWidth);
                int gridY = (int) ((key.Y - Bounds.Top) / _probe.GridHeight);

                // map extreme bottom/right pixels to bottom/right cell
                if (gridX == gridCount) --gridX;
                if (gridY == gridCount) --gridY;

                Debug.Assert(gridX >= 0 && gridX < gridCount);
                Debug.Assert(gridY >= 0 && gridY < gridCount);

                while (true) {
                    // compose signature from level and grid coordinates
                    int signature = unchecked(level | (gridX << 4) | (gridY << 18));

                    /*
                     * Mathematically, a node found by signature should contain the key, but we
                     * need an extra bounds check to ward against floating-point inaccuracies.
                     */

                    // probe grid cell at current level for key
                    if (_nodes.TryGetValue(signature, out node) && node.Bounds.ContainsOpen(key))
                        break;

                    // probe parent grid cell two levels up
                    if (level < ProbeLevel) { node = RootNode; break; }
                    level -= 2; gridX >>= 2; gridY >>= 2;
                }
            }

            // perform normal tree search for key
            while (true) {
                var child = node.FindChild(key);
                if (child == null) return node;
                node = child;
            }
        }

        #endregion
        #region FindNodeByValue

        /// <summary>
        /// Finds a <see cref="QuadTreeNode{TValue}"/> within the <see cref="QuadTree{TValue}"/>
        /// that contains the specified value.</summary>
        /// <param name="value">
        /// The value to locate.</param>
        /// <returns>
        /// A <see cref="QuadTreeNode{TValue}"/> whose <see cref="QuadTreeNode{TValue}.Data"/>
        /// contains the specified <paramref name="value"/>, if found; otherwise, a null reference.
        /// </returns>
        /// <remarks>
        /// <b>FindNodeByValue</b> iterates over all attached <see cref="QuadTreeNode{TValue}"/>
        /// instances, using the <see cref="GetEnumerator"/> sequence, until the specified <paramref
        /// name="value"/> is found. This is an O(n) operation, where n equals <see cref="Count"/>.
        /// </remarks>

        public QuadTreeNode<TValue> FindNodeByValue(TValue value) {

            foreach (var node in _nodes.Values)
                if (node._data != null && node._data.ContainsValue(value))
                    return node;

            return null;
        }

        #endregion
        #region FindRange(PointD, Double)

        /// <overloads>
        /// Finds all key-and-value pairs within the <see cref="QuadTree{TValue}"/> whose keys lie
        /// within the specified range.</overloads>
        /// <summary>
        /// Finds all key-and-value pairs within the <see cref="QuadTree{TValue}"/> whose keys lie
        /// within the specified circular range.</summary>
        /// <param name="center">
        /// A <see cref="PointD"/> indicating the center of the key range to search.</param>
        /// <param name="radius">
        /// The radius of the key range around <paramref name="center"/> to search.</param>
        /// <returns>
        /// A <see cref="Dictionary{PointD, TValue}"/> containing all elements whose keys lie within
        /// <paramref name="radius"/> around <paramref name="center"/>.</returns>
        /// <remarks><para>
        /// <b>FindRange</b> immediately returns an empty collection if the square circumscribed
        /// around the indicated key range does not intersect with <see cref="Bounds"/>. Otherwise,
        /// <b>FindRange</b> performs a recursive search starting with the <see cref="RootNode"/>.
        /// </para><para>
        /// Depending on the size of the specified <paramref name="radius"/> relative to <see
        /// cref="Bounds"/>, the runtime of this operation ranges from O(log m) to O(<see
        /// cref="Count"/>), where m is the number of <see cref="Nodes"/>.</para></remarks>

        public Dictionary<PointD, TValue> FindRange(PointD center, double radius) {
            RectD range = new RectD(center.X - radius, center.Y - radius, 2 * radius, 2 * radius);

            var output = new Dictionary<PointD, TValue>();
            if (!Bounds.IntersectsWith(range)) return output;

            RootNode.FindRange(ref range, true, output);
            return output;
        }

        #endregion
        #region FindRange(RectD)

        /// <summary>
        /// Finds all key-and-value pairs within the <see cref="QuadTree{TValue}"/> whose keys lie
        /// within the specified rectangular range.</summary>
        /// <param name="range">
        /// A <see cref="RectD"/> indicating the key range to search.</param>
        /// <returns>
        /// A <see cref="Dictionary{PointD, TValue}"/> containing all elements whose keys lie within
        /// the specified <paramref name="range"/>.</returns>
        /// <remarks><para>
        /// <b>FindRange</b> immediately returns an empty collection if the specified <paramref
        /// name="range"/> does not intersect with <see cref="Bounds"/>. Otherwise, <b>FindRange</b>
        /// performs a recursive search starting with the <see cref="RootNode"/>.
        /// </para><para>
        /// Depending on the size of <paramref name="range"/> relative to <see cref="Bounds"/>, the
        /// runtime of this operation ranges from O(log m) to O(<see cref="Count"/>), where m is the
        /// number of <see cref="Nodes"/>.</para></remarks>

        public Dictionary<PointD, TValue> FindRange(RectD range) {

            var output = new Dictionary<PointD, TValue>();
            if (!Bounds.IntersectsWith(range)) return output;

            RootNode.FindRange(ref range, false, output);
            return output;
        }

        #endregion
        #region GetEnumerator

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that can iterate through the <see
        /// cref="QuadTree{TValue}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> for the entire <see cref="QuadTree{TValue}"/>. Each
        /// enumerated item is a <see cref="KeyValuePair{PointD, TValue}"/>.</returns>
        /// <remarks><para>
        /// The <see cref="IEnumerator{T}"/> follows the sequence in which the attached <see
        /// cref="QuadTreeNode{TValue}"/> instances and their data are stored in hashtables. This
        /// sequence is essentially unpredictable and does not establish any key ordering.
        /// </para><para>
        /// Each iteration step is an O(1) operation when processing leaf nodes, or an O(m)
        /// operation when skipping over empty internal nodes, where m is the number of <see
        /// cref="Nodes"/>.</para></remarks>

        public IEnumerator<KeyValuePair<PointD, TValue>> GetEnumerator() {
            foreach (var node in _nodes.Values)
                if (node._data != null)
                    foreach (var pair in node._data)
                        yield return pair;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that can iterate through the <see
        /// cref="QuadTree{TValue}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> for the entire <see cref="QuadTree{TValue}"/>. Each
        /// enumerated item is a <see cref="KeyValuePair{PointD, TValue}"/>.</returns>

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
        #region Move

        /// <summary>
        /// Moves the specified element to a different key within the <see
        /// cref="QuadTree{TValue}"/>.</summary>
        /// <param name="oldKey">
        /// The key of the element to move.</param>
        /// <param name="newKey">
        /// The new key where to move the element.</param>
        /// <param name="node">
        /// An optional <see cref="QuadTreeNode{TValue}"/> to search for <paramref name="oldKey"/>
        /// before conducting a full tree search. The default is a null reference.</param>
        /// <returns><para>
        /// The <see cref="QuadTreeNode{TValue}"/> that contained <paramref name="oldKey"/>.
        /// </para><para>-or-</para><para>
        /// A null reference if that <see cref="QuadTreeNode{TValue}"/> was removed from the <see
        /// cref="QuadTree{TValue}"/>.</para></returns>
        /// <exception cref="ArgumentException"><para>
        /// <paramref name="oldKey"/> was not found in the <see cref="QuadTree{TValue}"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="newKey"/> already exists in the <see cref="QuadTree{TValue}"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="node"/> is not a null reference, and its <see
        /// cref="QuadTreeNode{TValue}.Tree"/> property differs from this instance.
        /// </para></exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="newKey"/> is outside of <see cref="Bounds"/>.</exception>
        /// <exception cref="KeyMismatchException">
        /// The value associated with <paramref name="oldKey"/> is an <see
        /// cref="IKeyedValue{PointD}"/> instance whose <see cref="IKeyedValue{PointD}.Key"/>
        /// differs from <paramref name="newKey"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="QuadTree{TValue}"/> is read-only.</exception>
        /// <remarks><para>
        /// <b>Move</b> has the same effect as calling <see cref="Remove"/> with the specified
        /// <paramref name="oldKey"/>, followed by <see cref="Add"/> with the specified <paramref
        /// name="newKey"/>. However, <b>Move</b> introduces two shortcuts to avoid the O(log m)
        /// tree search performed by each method, where m is the number of <see cref="Nodes"/>.
        /// </para><list type="number"><item>
        /// If the specified <paramref name="node"/> is a valid leaf node that contains <paramref
        /// name="oldKey"/>, <b>Move</b> skips the first tree search for <paramref name="oldKey"/>.
        /// When moving multiple keys in close proximity, always set <paramref name="node"/> to the
        /// previous <b>Move</b> result.
        /// </item><item>
        /// If <paramref name="oldKey"/> and <paramref name="newKey"/> both fall within the <see
        /// cref="QuadTreeNode{TValue}.Bounds"/> of the same leaf node, <b>Move</b> skips the second
        /// tree search for <paramref name="newKey"/> and directly adjusts the leaf node’s
        /// hashtable.
        /// </item></list><para>
        /// Either shortcut avoids one O(log m) tree search. When moving nearby keys over a short
        /// distance, both shortcuts may apply and reduce <b>Move</b> to an O(1) operation.
        /// </para></remarks>

        public QuadTreeNode<TValue> Move(PointD oldKey, PointD newKey,
            QuadTreeNode<TValue> node = null) {

            // CheckWritable();
            // ValidateKey is performed by Int32Dictionary
            // CollectionsUtility.ValidateKey(key, value);

            if (node != null && node.Tree != this)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "node", Strings.ArgumentPropertyInvalid, "Tree");

            if (node == null || node._data == null || !node._data.ContainsKey(oldKey)) {
                node = FindNode(oldKey);
                if (node == null)
                    ThrowHelper.ThrowArgumentException("oldKey", Strings.ArgumentNotInCollection);
            }

            TValue value = node._data[oldKey];
            node._data.Remove(oldKey);

            // Right/Bottom may belong to neighboring leaf node
            if (node.Bounds.ContainsOpen(newKey))
                node._data.Add(newKey, value);
            else {
                --_count;
                if (node._data.Count == 0 && node.Parent != null) {
                    node.Parent.RemoveChild(node);
                    node = null;
                }
                Add(newKey, value);
            }

            return node;
        }

        #endregion
        #region Remove(PointD)

        /// <overloads>
        /// Removes the specified element from the <see cref="QuadTree{TValue}"/>.</overloads>
        /// <summary>
        /// Removes the element with the specified key from the <see cref="QuadTree{TValue}"/>.
        /// </summary>
        /// <param name="key">
        /// The key of the element to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="key"/> was found and the associated element was removed;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="NotSupportedException">
        /// The <see cref="QuadTree{TValue}"/> is read-only.</exception>
        /// <remarks><para>
        /// <b>Remove</b> calls <see cref="FindNode(PointD)"/> to find the <see
        /// cref="QuadTreeNode{TValue}"/> that contains the specified <paramref name="key"/>.
        /// </para><para>
        /// <b>Remove</b> also removes the <see cref="QuadTreeNode{TValue}"/> itself, and possibly
        /// recursively its chain of <see cref="QuadTreeNode{TValue}.Parent"/> nodes, if removing
        /// the specified <paramref name="key"/> leaves it empty.</para></remarks>

        public bool Remove(PointD key) {
            // CheckWritable();
            var node = FindNode(key);
            if (node == null || node._data == null || !node._data.Remove(key))
                return false;

            --_count;
            if (node._data.Count == 0 && node.Parent != null)
                node.Parent.RemoveChild(node);
            return true;
        }

        #endregion
        #region Remove(KeyValuePair<PointD, TValue>)

        /// <summary>
        /// Removes the specified key-and-value pair from the <see cref="QuadTree{TValue}"/>.
        /// </summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{PointD, TValue}"/> element to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="pair"/> was found and removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="KeyMismatchException">
        /// The <see cref="KeyValuePair{PointD, TValue}.Value"/> component of <paramref name="pair"/>
        /// is an <see cref="IKeyedValue{PointD}"/> instance whose <see
        /// cref="IKeyedValue{PointD}.Key"/> differs from the associated <see
        /// cref="KeyValuePair{PointD, TValue}.Key"/> component.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="QuadTree{TValue}"/> is read-only.</exception>
        /// <remarks><para>
        /// <b>Remove</b> calls <see cref="FindNode(PointD)"/> to find the <see
        /// cref="QuadTreeNode{TValue}"/> that contains the <see cref="KeyValuePair{PointD,
        /// TValue}.Key"/> of the specified <paramref name="pair"/>. The element is removed only if
        /// the key is associated with a matching <see cref="KeyValuePair{PointD, TValue}.Value"/>.
        /// </para><para>
        /// <b>Remove</b> also removes the <see cref="QuadTreeNode{TValue}"/> itself, and possibly
        /// recursively its chain of <see cref="QuadTreeNode{TValue}.Parent"/> nodes, if removing
        /// the specified <paramref name="pair"/> leaves it empty.</para></remarks>

        public bool Remove(KeyValuePair<PointD, TValue> pair) {
            // CheckWritable();
            // ValidateKey is performed by Int32Dictionary
            // CollectionsUtility.ValidateKey(pair.Key, pair.Value);

            var node = FindNode(pair.Key);
            if (node == null || node._data == null || !node._data.Remove(pair))
                return false;

            --_count;
            if (node._data.Count == 0 && node.Parent != null)
                node.Parent.RemoveChild(node);
            return true;
        }

        #endregion
        #region ToArray

        /// <summary>
        /// Copies the key-and-value pairs of the <see cref="QuadTree{TValue}"/> to a new <see
        /// cref="Array"/>.</summary>
        /// <returns>
        /// A one-dimensional <see cref="Array"/> containing copies of the <see
        /// cref="KeyValuePair{PointD, TValue}"/> elements of the <see cref="QuadTree{TValue}"/>.
        /// </returns>
        /// <remarks>
        /// <b>ToArray</b> has the same effect as <see cref="CopyTo"/> with a starting index of
        /// zero, but also allocates the target array.</remarks>

        public KeyValuePair<PointD, TValue>[] ToArray() {
            var array = new KeyValuePair<PointD, TValue>[_count];
            CopyTo(array, 0);
            return array;
        }

        #endregion
        #region TryGetValue

        /// <summary>
        /// Gets the value associated with the specified key.</summary>
        /// <param name="key">
        /// The key whose value to get.</param>
        /// <param name="value">
        /// The value associated with the first occurrence of <paramref name="key"/>, if found;
        /// otherwise, the default value for <typeparamref name="TValue"/>.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="key"/> was found; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>TryGetValue</b> calls <see cref="FindNode(PointD)"/> to find the <see
        /// cref="QuadTreeNode{TValue}"/> that contains the specified <paramref name="key"/>.
        /// </remarks>

        public bool TryGetValue(PointD key, out TValue value) {

            var node = FindNode(key);
            if (node != null && node._data != null)
                return node._data.TryGetValue(key, out value);

            value = default(TValue);
            return false;
        }

        #endregion
        #endregion
        #region Struct ProbeStatus

        /// <summary>
        /// Contains the status of the last depth probe conducted by <see cref="FindNode(PointD)"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="FindNode(PointD)"/> stores all level-specific data of the last depth probe,
        /// and reuses that data until the estimated starting level changes. This happens only when
        /// the number of <see cref="Nodes"/> changes by at least 256 elements, since depth probes
        /// are not conducted above tree level 4.</remarks>

        [StructLayout(LayoutKind.Auto)]
        private struct ProbeStatus {
            #region GridHeight

            /// <summary>
            /// The height of one grid cell at the recorded <see cref="Level"/>.</summary>

            internal double GridHeight;

            #endregion
            #region GridWidth

            /// <summary>
            /// The width of one grid cell at the recorded <see cref="Level"/>.</summary>

            internal double GridWidth;

            #endregion
            #region Level

            /// <summary>
            /// The starting level for the recorded <see cref="NodeCount"/>.</summary>

            internal int Level;

            #endregion
            #region NodeCount

            /// <summary>
            /// The number of <see cref="Nodes"/> at the last depth probe, right-shifted by 8 bits.
            /// </summary>

            internal int NodeCount;

            #endregion
        }

        #endregion
        #region Class KeyCollection

        private class KeyCollection: ICollection<PointD>, ICollection {

            private readonly QuadTree<TValue> _tree;

            internal KeyCollection(QuadTree<TValue> tree) {
                _tree = tree;
            }

            public int Count {
                [DebuggerStepThrough]
                get { return _tree._count; }
            }

            public bool IsReadOnly {
                [DebuggerStepThrough]
                get { return true; }
            }

            bool ICollection.IsSynchronized {
                [DebuggerStepThrough]
                get { return false; }
            }

            public object SyncRoot {
                [DebuggerStepThrough]
                get { return _tree.SyncRoot; }
            }

            public void Add(PointD key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public void Clear() {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public bool Contains(PointD key) {
                return _tree.ContainsKey(key);
            }

            public void CopyTo(PointD[] array, int arrayIndex) {
                _tree.CheckTargetArray(array, arrayIndex);
                foreach (PointD key in this)
                    array[arrayIndex++] = key;
            }

            void ICollection.CopyTo(Array array, int arrayIndex) {
                CopyTo((PointD[]) array, arrayIndex);
            }

            public IEnumerator<PointD> GetEnumerator() {
                foreach (var node in _tree._nodes.Values)
                    if (node._data != null)
                        foreach (var key in node._data.Keys)
                            yield return key;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public bool Remove(PointD key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }
        }

        #endregion
        #region Class ValueCollection

        private class ValueCollection: ICollection<TValue>, ICollection {

            private readonly QuadTree<TValue> _tree;

            internal ValueCollection(QuadTree<TValue> tree) {
                _tree = tree;
            }

            public int Count {
                [DebuggerStepThrough]
                get { return _tree._count; }
            }

            public bool IsReadOnly {
                [DebuggerStepThrough]
                get { return true; }
            }

            bool ICollection.IsSynchronized {
                [DebuggerStepThrough]
                get { return false; }
            }

            public object SyncRoot {
                [DebuggerStepThrough]
                get { return _tree.SyncRoot; }
            }

            public void Add(TValue value) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public void Clear() {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public bool Contains(TValue value) {
                return _tree.ContainsValue(value);
            }

            public void CopyTo(TValue[] array, int arrayIndex) {
                _tree.CheckTargetArray(array, arrayIndex);
                foreach (TValue value in this)
                    array[arrayIndex++] = value;
            }

            void ICollection.CopyTo(Array array, int arrayIndex) {
                CopyTo((TValue[]) array, arrayIndex);
            }

            public IEnumerator<TValue> GetEnumerator() {
                foreach (var node in _tree._nodes.Values)
                    if (node._data != null)
                        foreach (var value in node._data.Values)
                            yield return value;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public bool Remove(TValue value) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }
        }

        #endregion
    }
}
