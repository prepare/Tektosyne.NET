using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides a generic collection of keys and values that are sorted using a braided search
    /// tree.</summary>
    /// <typeparam name="TKey">
    /// The type of all keys in the collection. Keys cannot be null references.</typeparam>
    /// <typeparam name="TValue">
    /// The type of all values that are associated with the keys. If <typeparamref name="TValue"/>
    /// is a reference type, values may be null references.</typeparam>
    /// <remarks><para>
    /// <b>BraidedTree</b> provides a binary search tree, similar to the standard class <see
    /// cref="SortedDictionary{TKey, TValue}"/>, but with the following differences:
    /// </para><list type="bullet"><item>
    /// The tree structure is exposed through the <see cref="BraidedTreeNode{TKey, TValue}"/> class.
    /// You can find the node associated with any given key and follow a reference to its left 
    /// descendant, right descendant, or parent node.
    /// </item><item>
    /// The key sorting order is represented not only indirectly by the tree structure, but also
    /// directly by a doubly-linked list that connects all nodes – hence the term "braided". A
    /// single step reaches the smallest or greatest node from the root node, or the next-smaller or
    /// next-greater node from any tree node. <see cref="BraidedTree{TKey, TValue}.FindRange"/>
    /// exploits this fact to quickly find all elements within a given key range.
    /// </item><item>
    /// The tree structure is balanced for minimal tree height by associating each node with a
    /// random priority, rather than by alternating node "colors" as in a red-black tree.
    /// </item></list><para>
    /// Like other <b>Tektosyne.Collections</b> classes, <b>BraidedTree</b> also provides the
    /// following extra features:
    /// </para><list type="bullet"><item>
    /// The <see cref="IKeyedValue{TKey}.Key"/> property of any <typeparamref name="TValue"/> that
    /// implements the <see cref="IKeyedValue{TKey}"/> interface is automatically checked against
    /// the associated dictionary key when a key or value is changed or inserted.
    /// </item><item>
    /// <see cref="BraidedTree{TKey, TValue}.Copy"/> creates a deep copy of the collection by
    /// invoking <see cref="ICloneable.Clone"/> on each value. This feature requires that all copied
    /// values implement the <see cref="ICloneable"/> interface.
    /// </item><item>
    /// <see cref="BraidedTree{TKey, TValue}.Equals"/> compares two collections with identical
    /// element types for value equality of all elements. The collections compare as equal if they
    /// contain the same elements in the same sorting order. The internal tree structure is
    /// irrelevant for determining equality.
    /// </item></list><para>
    /// <b>BraidedTree</b> implements the <c>RandomizedSearchTree</c> class by Michael J. Laszlo,
    /// <em>Computational Geometry and Computer Graphics in C++</em>, Prentice Hall 1996, p.55ff.
    /// </para></remarks>

    [Serializable]
    public class BraidedTree<TKey, TValue>: IDictionary<TKey, TValue>, ICollection, ICloneable {
        #region BraidedTree()

        /// <overloads>
        /// Initializes a new instance of the <see cref="BraidedTree{TKey, TValue}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BraidedTree{TKey, TValue}"/> class that is
        /// empty and uses the default comparer for <typeparamref name="TKey"/>.</summary>

        public BraidedTree() {
            Comparison = ComparerCache<TKey>.Comparer.Compare;
            RootNode = new BraidedTreeNode<TKey, TValue>(this);
        }

        #endregion
        #region BraidedTree(Comparison<TKey>)

        /// <summary>
        /// Initializes a new instance of the <see cref="BraidedTree{TKey, TValue}"/> class that is
        /// empty and uses the specified comparer for <typeparamref name="TKey"/>.</summary>
        /// <param name="comparison">
        /// The <see cref="System.Comparison{T}"/> method to use when comparing keys, or a null
        /// reference to use the default <see cref="Comparer{T}"/> for <typeparamref name="TKey"/>.
        /// </param>

        public BraidedTree(Comparison<TKey> comparison) {
            Comparison = comparison ?? ComparerCache<TKey>.Comparer.Compare;
            RootNode = new BraidedTreeNode<TKey, TValue>(this);
        }

        #endregion
        #region BraidedTree(IDictionary<TKey, TValue>)

        /// <summary>
        /// Initializes a new instance of the <see cref="BraidedTree{TKey, TValue}"/> class that
        /// contains elements copied from the specified collection and uses the default comparer for
        /// <typeparamref name="TKey"/>.</summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new
        /// collection.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <exception cref="ArgumentNullException"><para>
        /// <paramref name="dictionary"/> is a null reference.
        /// </para><para>-or-</para><para>
        /// <paramref name="dictionary"/> contains an element whose <see cref="KeyValuePair{TKey,
        /// TValue}.Key"/> is a null reference. </para></exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="dictionary"/> contains an element whose <see cref="KeyValuePair{TKey,
        /// TValue}.Value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="IKeyedValue{TKey}.Key"/> differs from the associated <see cref="KeyValuePair{TKey,
        /// TValue}.Key"/>.</exception>
        /// <remarks>
        /// This constructor calls <see cref="AddRange"/> to add all elements in the specified
        /// <paramref name="dictionary"/> to the new <see cref="BraidedTree{TKey, TValue}"/>.
        /// </remarks>

        public BraidedTree(IDictionary<TKey, TValue> dictionary): this() {
            AddRange(dictionary);
        }

        #endregion
        #region BraidedTree(IDictionary<TKey, TValue>, Comparison<TKey>)

        /// <summary>
        /// Initializes a new instance of the <see cref="BraidedTree{TKey, TValue}"/> class that
        /// contains elements copied from the specified collection and uses the specified comparer
        /// for <typeparamref name="TKey"/>.</summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new
        /// collection.</param>
        /// <param name="comparison">
        /// The <see cref="System.Comparison{T}"/> method to use when comparing keys, or a null
        /// reference to use the default <see cref="Comparer{T}"/> for <typeparamref name="TKey"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <exception cref="ArgumentNullException"><para>
        /// <paramref name="dictionary"/> is a null reference.
        /// </para><para>-or-</para><para>
        /// <paramref name="dictionary"/> contains an element whose <see cref="KeyValuePair{TKey,
        /// TValue}.Key"/> is a null reference.</para></exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="dictionary"/> contains an element whose <see cref="KeyValuePair{TKey,
        /// TValue}.Value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="IKeyedValue{TKey}.Key"/> differs from the associated <see cref="KeyValuePair{TKey,
        /// TValue}.Key"/>.</exception>
        /// <remarks>
        /// This constructor calls <see cref="AddRange"/> to add all elements in the specified
        /// <paramref name="dictionary"/> to the new <see cref="BraidedTree{TKey, TValue}"/>.
        /// </remarks>

        public BraidedTree(IDictionary<TKey, TValue> dictionary,
            Comparison<TKey> comparison): this(comparison) {

            AddRange(dictionary);
        }

        #endregion
        #region Private Fields

        /// <summary>
        /// The number of key-and-value pairs contained in the <see cref="BraidedTree{TKey,
        /// TValue}"/>.</summary>

        private int _count;

        /// <summary>Backs the <see cref="Keys"/> property.</summary>

        [NonSerialized]
        private KeyCollection _keys;

        /// <summary>Backs the <see cref="Values"/> property.</summary>

        [NonSerialized]
        private ValueCollection _values;

        /// <summary>
        /// Used to generate random <see cref="BraidedTreeNode{TKey, TValue}.Priority"/> values.
        /// </summary>

        [NonSerialized]
        internal readonly MersenneTwister Random = new MersenneTwister();

        #endregion
        #region Public Properties
        #region Comparison

        /// <summary>
        /// The <see cref="System.Comparison{T}"/> method that is used to determine the relative
        /// order of keys in the <see cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <remarks>
        /// <b>Comparison</b> is used to determine the order of <see cref="BraidedTreeNode{TKey,
        /// TValue}"/> objects within the binary <see cref="BraidedTree{TKey, TValue}"/> structure,
        /// and also within the chain of <see cref="BraidedTreeNode{TKey, TValue}.Next"/> and <see
        /// cref="BraidedTreeNode{TKey, TValue}.Previous"/> references.</remarks>

        public readonly Comparison<TKey> Comparison;

        #endregion
        #region Count

        /// <summary>
        /// Gets the number of key-and-value pairs contained in the <see cref="BraidedTree{TKey,
        /// TValue}"/>.</summary>
        /// <value>
        /// The number of <see cref="KeyValuePair{TKey, TValue}"/> elements contained in the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</value>
        /// <remarks>
        /// <b>Count</b> returns a counter value maintained by the <see cref="TryAddNode"/> and <see
        /// cref="RemoveNode"/> methods. Accessing this property is therefore an O(1) operation.
        /// </remarks>

        public int Count {
            [DebuggerStepThrough]
            get { return _count; }
        }

        #endregion
        #region FirstNode

        /// <summary>
        /// Gets the smallest <see cref="BraidedTreeNode{TKey, TValue}"/> within the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <value><para>
        /// The smallest <see cref="BraidedTreeNode{TKey, TValue}"/> within the <see
        /// cref="BraidedTree{TKey, TValue}"/>.
        /// </para><para>-or-</para><para>
        /// <see cref="RootNode"/> if the <see cref="BraidedTree{TKey, TValue}"/> is empty.
        /// </para></value>
        /// <remarks>
        /// <b>FirstNode</b> returns the value of the <see cref="BraidedTreeNode{TKey,
        /// TValue}.Next"/> property of the <see cref="RootNode"/>. This is an O(1) operation.
        /// </remarks>

        public BraidedTreeNode<TKey, TValue> FirstNode {
            get { return RootNode._next; }
        }

        #endregion
        #region IsFixedSize

        /// <summary>
        /// Gets a value indicating whether the <see cref="BraidedTree{TKey, TValue}"/> has a fixed
        /// size.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="BraidedTree{TKey, TValue}"/> has a fixed size; otherwise,
        /// <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="IDictionary.IsFixedSize"/> for details.
        /// </para><para>
        /// This property always returns the same value as the <see cref="IsReadOnly"/> property
        /// since any fixed-size <see cref="BraidedTree{TKey, TValue}"/> is also read-only, and vice
        /// versa.</para></remarks>

        public bool IsFixedSize {
            [DebuggerStepThrough]
            get { return IsReadOnly; }
        }

        #endregion
        #region IsReadOnly

        /// <summary>
        /// Gets a value indicating whether the <see cref="BraidedTree{TKey, TValue}"/> is
        /// read-only.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="BraidedTree{TKey, TValue}"/> is read-only; otherwise,
        /// <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="IDictionary.IsReadOnly"/> for details.
        /// </para><para>
        /// <b>IsReadOnly</b> always returns <c>false</c> since the <see cref="BraidedTree{TKey,
        /// TValue}"/> class does not offer a read-only wrapper.</para></remarks>

        public bool IsReadOnly {
            [DebuggerStepThrough]
            get { return false; }
        }

        #endregion
        #region IsSynchronized

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="BraidedTree{TKey, TValue}"/> is
        /// synchronized (thread-safe).</summary>
        /// <value>
        /// <c>true</c> if access to the <see cref="BraidedTree{TKey, TValue}"/> is synchronized
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
        /// specified key and value to the <see cref="BraidedTree{TKey, TValue}"/>.</para></value>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// The property is set to an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="KeyValuePair{TKey, TValue}.Key"/> differs from the specified <paramref
        /// name="key"/>.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is read, and <paramref name="key"/> does not exist in the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="BraidedTree{TKey, TValue}"/> is read-only.
        /// </exception>
        /// <remarks>
        /// This indexer calls <see cref="FindNode(TKey)"/> to find the specified <paramref
        /// name="key"/>, and <see cref="TryAddNode"/> to add a new element or set the <see
        /// cref="BraidedTreeNode{TKey, TValue}.Value"/> of an existing element.</remarks>

        public TValue this[TKey key] {
            get {
                var node = FindNode(key);
                if (node == null)
                    ThrowHelper.ThrowKeyNotFoundException(key);

                return node._value;
            }
            set {
                BraidedTreeNode<TKey, TValue> node;
                if (!TryAddNode(key, value, out node))
                    node.Value = value;
            }
        }

        #endregion
        #region Keys

        /// <summary>
        /// Gets an <see cref="ICollection{TKey}"/> containing the keys in the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="ICollection{TKey}"/> containing the keys in the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</value>
        /// <remarks>
        /// <b>Keys</b> starts at <see cref="FirstNode"/> and follows the chain of <see
        /// cref="BraidedTreeNode{TKey, TValue}.Next"/> references. Each step is an O(1) operation.
        /// </remarks>

        public ICollection<TKey> Keys {
            [DebuggerStepThrough]
            get {
                if (_keys == null)
                    _keys = new KeyCollection(this);

                return _keys;
            }
        }

        #endregion
        #region LastNode

        /// <summary>
        /// Gets the greatest <see cref="BraidedTreeNode{TKey, TValue}"/> within the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <value><para>
        /// The greatest <see cref="BraidedTreeNode{TKey, TValue}"/> within the <see
        /// cref="BraidedTree{TKey, TValue}"/>.
        /// </para><para>-or-</para><para>
        /// <see cref="RootNode"/> if the <see cref="BraidedTree{TKey, TValue}"/> is empty.
        /// </para></value>
        /// <remarks>
        /// <b>LastNode</b> returns the value of the <see cref="BraidedTreeNode{TKey,
        /// TValue}.Previous"/> property of the <see cref="RootNode"/>. This is an O(1) operation.
        /// </remarks>

        public BraidedTreeNode<TKey, TValue> LastNode {
            get { return RootNode._previous; }
        }

        #endregion
        #region RootNode

        /// <summary>
        /// The <see cref="BraidedTreeNode{TKey, TValue}"/> that is the root of the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <remarks><para>
        /// The <b>RootNode</b> holds no key or value, and is never removed from the <see
        /// cref="BraidedTree{TKey, TValue}"/>. An empty <see cref="BraidedTree{TKey, TValue}"/>
        /// contains only the <b>RootNode</b>.
        /// </para><para>
        /// All other nodes are descendants of the <b>RootNode</b>. The chain of <see
        /// cref="BraidedTreeNode{TKey, TValue}.Parent"/> references from any other node of the <see
        /// cref="BraidedTree{TKey, TValue}"/> ends in the <b>RootNode</b>, whose <see
        /// cref="BraidedTreeNode{TKey, TValue}.Parent"/> property is always a null reference.
        /// </para><para>
        /// The chains of <see cref="BraidedTreeNode{TKey, TValue}.Previous"/> and <see
        /// cref="BraidedTreeNode{TKey, TValue}.Next"/> references always begin and end with the
        /// <b>RootNode</b>. When iterating through the <see cref="BraidedTree{TKey, TValue}"/>
        /// starting from <see cref="FirstNode"/> or <see cref="LastNode"/>, reaching the
        /// <b>RootNode</b> indicates the end of the iteration.</para></remarks>

        public readonly BraidedTreeNode<TKey, TValue> RootNode;

        #endregion
        #region SyncRoot

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <value>
        /// An object that can be used to synchronize access to the <see cref="BraidedTree{TKey,
        /// TValue}"/>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="ICollection.SyncRoot"/> for details.
        /// </para><para>
        /// When synchronizing multi-threaded access to the <see cref="BraidedTree{TKey, TValue}"/>,
        /// obtain a lock on the <b>SyncRoot</b> object rather than the collection itself. A
        /// read-only view always returns the same <b>SyncRoot</b> object as the underlying writable
        /// collection.</para></remarks>

        public object SyncRoot {
            [DebuggerStepThrough]
            get { return RootNode; }
        }

        #endregion
        #region Values

        /// <summary>
        /// Gets an <see cref="ICollection{TValue}"/> containing the values in the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="ICollection{TValue}"/> containing the values in the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</value>
        /// <remarks>
        /// <b>Values</b> starts at <see cref="FirstNode"/> and follows the chain of <see
        /// cref="BraidedTreeNode{TKey, TValue}.Next"/> references. Each step is an O(1) operation.
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
        #region CheckTargetArray

        /// <summary>
        /// Checks the bounds of the specified array and the specified starting index against the
        /// size of the <see cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination for elements copied from
        /// the <see cref="BraidedTree{TKey, TValue}"/>. The <b>Array</b> must have zero-based
        /// indexing.</param>
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
        /// The number of elements in the <see cref="BraidedTree{TKey, TValue}"/> is greater than
        /// the available space from <paramref name="arrayIndex"/> to the end of the destination
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
        #region Add(TKey, TValue)

        /// <overloads>
        /// Adds the specified element to the <see cref="BraidedTree{TKey, TValue}"/>.</overloads>
        /// <summary>
        /// Adds the specified key and value to the <see cref="BraidedTree{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">
        /// The key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> already exists in the <see cref="BraidedTree{TKey, TValue}"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="IKeyedValue{TKey}.Key"/> differs from <paramref name="key"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="BraidedTree{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// <b>Add</b> calls <see cref="TryAddNode"/> with the specified <paramref name="key"/> and
        /// <paramref name="value"/>, but throws an <see cref="ArgumentException"/> if <paramref
        /// name="key"/> already exists in the <see cref="BraidedTree{TKey, TValue}"/>.</remarks>

        public void Add(TKey key, TValue value) {
            BraidedTreeNode<TKey, TValue> node;
            if (!TryAddNode(key, value, out node))
                ThrowHelper.ThrowArgumentException("key", Strings.ArgumentInCollection);
        }

        #endregion
        #region Add(KeyValuePair<TKey, TValue>)

        /// <summary>
        /// Adds the specified key-and-value pair to the <see cref="BraidedTree{TKey, TValue}"/>.
        /// </summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{TKey, TValue}"/> element to add.</param>
        /// <exception cref="ArgumentException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> component of <paramref name="pair"/>
        /// already exists in the <see cref="BraidedTree{TKey, TValue}"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> component of <paramref name="pair"/> is
        /// a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Value"/> component of <paramref name="pair"/>
        /// is an <see cref="IKeyedValue{TKey}"/> instance whose <see cref="IKeyedValue{TKey}.Key"/>
        /// differs from the associated <see cref="KeyValuePair{TKey, TValue}.Key"/> component.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="BraidedTree{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Add(TKey, TValue)"/> for details.</remarks>

        public void Add(KeyValuePair<TKey, TValue> pair) {
            Add(pair.Key, pair.Value);
        }

        #endregion
        #region AddRange

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="BraidedTree{TKey,
        /// TValue}"/>. </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> whose elements to add.</param>
        /// <exception cref="ArgumentException"><para>
        /// The <see cref="BraidedTree{TKey, TValue}"/> already contains one or more keys in the
        /// specified <paramref name="dictionary"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="dictionary"/> contains one or more duplicate keys.</para></exception>
        /// <exception cref="ArgumentNullException"><para>
        /// <paramref name="dictionary"/> is a null reference.
        /// </para><para>-or-</para><para>
        /// <paramref name="dictionary"/> contains an element whose <see cref="KeyValuePair{TKey,
        /// TValue}.Key"/> is a null reference. </para></exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="dictionary"/> contains an element whose <see cref="KeyValuePair{TKey,
        /// TValue}.Value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="IKeyedValue{TKey}.Key"/> differs from the associated <see cref="KeyValuePair{TKey,
        /// TValue}.Key"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="BraidedTree{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// <b>AddRange</b> calls <see cref="Add"/> for each <see cref="KeyValuePair{TKey,
        /// TValue}"/> in the specified <paramref name="dictionary"/>.</remarks>

        public void AddRange(IDictionary<TKey, TValue> dictionary) {
            // CheckWritable();
            if (dictionary == null)
                ThrowHelper.ThrowArgumentNullException("dictionary");

            foreach (var pair in dictionary)
                Add(pair.Key, pair.Value);
        }

        #endregion
        #region Clear

        /// <summary>
        /// Removes all elements from the <see cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="BraidedTree{TKey, TValue}"/> is read-only.</exception>
        /// <remarks><para>
        /// <b>Clear</b> resets <see cref="Count"/> to zero and removes any other <see
        /// cref="BraidedTreeNode{TKey, TValue}"/> instances attached to the permanent <see
        /// cref="RootNode"/>. This is an O(1) operation.
        /// </para><note type="caution">
        /// Any removed <see cref="BraidedTreeNode{TKey, TValue}"/> instances are <em>not</em>
        /// cleared, rendering all their tree structure properties invalid.</note></remarks>

        public void Clear() {
            // CheckWritable();
            _count = 0;
            RootNode.Clear();
        }

        #endregion
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="BraidedTree{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> does not necessarily preserve the internal structure of the <see
        /// cref="BraidedTree{TKey, TValue}"/>, due to randomized tree rebalancing. The sorting
        /// order of all elements is preserved, however.</remarks>

        public virtual object Clone() {
            return new BraidedTree<TKey, TValue>(this, Comparison);
        }

        #endregion
        #region Contains

        /// <summary>
        /// Determines whether the <see cref="BraidedTree{TKey, TValue}"/> contains the specified
        /// key-and-value pair.</summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{TKey, TValue}"/> element to locate.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="pair"/> is found in the <see cref="BraidedTree{TKey,
        /// TValue}"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> succeeds if <see cref="FindNode(TKey)"/> finds the <see
        /// cref="KeyValuePair{TKey, TValue}.Key"/> of the specified <paramref name="pair"/>, and
        /// the resulting <see cref="BraidedTreeNode{TKey, TValue}"/> also contains its <see
        /// cref="KeyValuePair{TKey, TValue}.Value"/>.</remarks>

        public bool Contains(KeyValuePair<TKey, TValue> pair) {
            var node = FindNode(pair.Key);
            return (node != null && ComparerCache<TValue>.
                EqualityComparer.Equals(node._value, pair.Value));
        }

        #endregion
        #region ContainsKey

        /// <summary>
        /// Determines whether the <see cref="BraidedTree{TKey, TValue}"/> contains the specified
        /// key.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="BraidedTree{TKey, TValue}"/> contains an element with the
        /// specified <paramref name="key"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// <b>ContainsKey</b> succeeds if <see cref="FindNode(TKey)"/> finds a <see
        /// cref="BraidedTreeNode{TKey, TValue}"/> for the specified <paramref name="key"/>.
        /// </remarks>

        public bool ContainsKey(TKey key) {
            return (FindNode(key) != null);
        }

        #endregion
        #region ContainsValue

        /// <summary>
        /// Determines whether the <see cref="BraidedTree{TKey, TValue}"/> contains the specified
        /// value.</summary>
        /// <param name="value">
        /// The value to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="BraidedTree{TKey, TValue}"/> contains an element with the
        /// specified <paramref name="value"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>ContainsValue</b> succeeds if <see cref="FindNodeByValue"/> finds a <see
        /// cref="BraidedTreeNode{TKey, TValue}"/> for the specified <paramref name="value"/>.
        /// </remarks>

        public bool ContainsValue(TValue value) {
            return (FindNodeByValue(value) != null);
        }

        #endregion
        #region Copy

        /// <summary>
        /// Creates a deep copy of the <see cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="BraidedTree{TKey, TValue}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="TValue"/> does not implement <see cref="ICloneable"/>.</exception>
        /// <remarks><para>
        /// <b>Copy</b> is similar to <see cref="Clone"/> but creates a deep copy the <see
        /// cref="BraidedTree{TKey, TValue}"/> by invoking <see cref="ICloneable.Clone"/> on all 
        /// <typeparamref name="TValue"/> values. The <typeparamref name="TKey"/> keys are always
        /// duplicated by a shallow copy.
        /// </para><para>
        /// <b>Copy</b> does not necessarily preserve the internal structure of the <see
        /// cref="BraidedTree{TKey, TValue}"/>, due to randomized tree rebalancing. The sorting
        /// order of all elements is preserved, however.</para></remarks>

        public BraidedTree<TKey, TValue> Copy() {
            BraidedTree<TKey, TValue> copy = new BraidedTree<TKey, TValue>(Comparison);

            for (var node = RootNode._next; node != RootNode; node = node._next) {
                TValue value = node._value;

                ICloneable cloneable = (ICloneable) value;
                if (cloneable != null)
                    value = (TValue) cloneable.Clone();

                copy.Add(node.Key, value);
            }

            return copy;
        }

        #endregion
        #region CopyTo

        /// <summary>
        /// Copies the entire <see cref="BraidedTree{TKey, TValue}"/> to a one-dimensional <see
        /// cref="Array"/>, starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the <see
        /// cref="KeyValuePair{TKey, TValue}"/> elements copied from the <see
        /// cref="BraidedTree{TKey, TValue}"/>. The <b>Array</b> must have zero-based indexing.
        /// </param>
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
        /// The number of elements in the source <see cref="BraidedTree{TKey, TValue}"/> is greater
        /// than the available space from <paramref name="arrayIndex"/> to the end of the
        /// destination <paramref name="array"/>.</para></exception>
        /// <remarks>
        /// <b>CopyTo</b> starts at <see cref="FirstNode"/> and follows the chain of <see
        /// cref="BraidedTreeNode{TKey, TValue}.Next"/> references. Each step is an O(1) operation.
        /// </remarks>

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            CheckTargetArray(array, arrayIndex);
            foreach (var pair in this)
                array[arrayIndex++] = pair;
        }

        /// <summary>
        /// Copies the entire <see cref="BraidedTree{TKey, TValue}"/> to a one-dimensional <see
        /// cref="Array"/>, starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the <see
        /// cref="KeyValuePair{TKey, TValue}"/> elements copied from the <see
        /// cref="BraidedTree{TKey, TValue}"/>. The <b>Array</b> must have zero-based indexing.
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
        /// The number of elements in the source <see cref="BraidedTree{TKey, TValue}"/> is greater
        /// than the available space from <paramref name="arrayIndex"/> to the end of the
        /// destination <paramref name="array"/>.</para></exception>
        /// <exception cref="InvalidCastException">
        /// <see cref="KeyValuePair{TKey, TValue}"/> cannot be cast automatically to the type of the
        /// destination <paramref name="array"/>.</exception>

        void ICollection.CopyTo(Array array, int arrayIndex) {
            CopyTo((KeyValuePair<TKey, TValue>[]) array, arrayIndex);
        }

        #endregion
        #region Equals

        /// <summary>
        /// Determines whether the specified collection contains the same key-and-value pairs in the
        /// same order as the current <see cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <param name="collection">
        /// The <see cref="ICollection{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/> elements to
        /// compare with the current <see cref="BraidedTree{TKey, TValue}"/>.</param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="collection"/> is another reference to this <see cref="BraidedTree{TKey,
        /// TValue}"/>.
        /// </item><item>
        /// <paramref name="collection"/> is not a null reference, contains the same number of
        /// elements as this <see cref="BraidedTree{TKey, TValue}"/>, and all elements compare as
        /// equal when retrieved in the enumeration sequence for each collection.
        /// </item></list></returns>
        /// <remarks>
        /// <b>Equals</b> calls <see cref="CollectionsUtility.SequenceEqual"/> to test the two
        /// collections for value equality.</remarks>

        public bool Equals(ICollection<KeyValuePair<TKey, TValue>> collection) {
            return CollectionsUtility.SequenceEqual(this, collection);
        }

        #endregion
        #region FindNode

        /// <summary>
        /// Finds the <see cref="BraidedTreeNode{TKey, TValue}"/> within the <see
        /// cref="BraidedTree{TKey, TValue}"/> that contains the specified key.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns>
        /// The <see cref="BraidedTreeNode{TKey, TValue}"/> whose <see cref="BraidedTreeNode{TKey,
        /// TValue}.Key"/> equals the specified <paramref name="key"/>, if found; otherwise, a null
        /// reference.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// <b>FindNode</b> performs a binary search within the <see cref="BraidedTree{TKey,
        /// TValue}"/> for the specified <paramref name="key"/>. This is an O(ld n) operation.
        /// </remarks>

        public BraidedTreeNode<TKey, TValue> FindNode(TKey key) {
            if (key == null)
                ThrowHelper.ThrowArgumentNullException("key");

            BraidedTreeNode<TKey, TValue> node = RootNode._right;

            while (node != null) {
                int result = Comparison(key, node.Key);

                if (result < 0) node = node._left;
                else if (result > 0) node = node._right;
                else break;
            }

            return node;
        }

        #endregion
        #region FindNodeByValue

        /// <summary>
        /// Finds the smallest <see cref="BraidedTreeNode{TKey, TValue}"/> within the <see
        /// cref="BraidedTree{TKey, TValue}"/> that contains the specified value.</summary>
        /// <param name="value">
        /// The value to locate.</param>
        /// <returns>
        /// The smallest <see cref="BraidedTreeNode{TKey, TValue}"/> whose <see
        /// cref="BraidedTreeNode{TKey, TValue}.Value"/> equals the specified <paramref
        /// name="value"/>, if found; otherwise, a null reference.</returns>
        /// <remarks>
        /// <b>FindNodeByValue</b> starts with <see cref="FirstNode"/> and follows the chain of <see
        /// cref="BraidedTreeNode{TKey, TValue}.Next"/> references until the specified <paramref
        /// name="value"/> is found. This is an O(n) operation.</remarks>

        public BraidedTreeNode<TKey, TValue> FindNodeByValue(TValue value) {
            var comparer = ComparerCache<TValue>.EqualityComparer;

            for (var node = RootNode._next; node != RootNode; node = node._next)
                if (comparer.Equals(value, node._value))
                    return node;

            return null;
        }

        #endregion
        #region FindNodeOrPrevious

        /// <summary>
        /// Finds the <see cref="BraidedTreeNode{TKey, TValue}"/> within the <see
        /// cref="BraidedTree{TKey, TValue}"/> that contains the specified key, or the previous <see
        /// cref="BraidedTreeNode{TKey, TValue}"/> if not found.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns><para>
        /// The <see cref="BraidedTreeNode{TKey, TValue}"/> whose <see cref="BraidedTreeNode{TKey,
        /// TValue}.Key"/> equals the specified <paramref name="key"/>, if found.
        /// </para><para>-or-</para><para>
        /// The <see cref="BraidedTreeNode{TKey, TValue}"/> with the greatest <see
        /// cref="BraidedTreeNode{TKey, TValue}.Key"/> that is not greater than the specified
        /// <paramref name="key"/>, if any.
        /// </para><para>-or-</para><para>
        /// The <see cref="RootNode"/> if neither the specified <paramref name="key"/> nor a
        /// previous <see cref="BraidedTreeNode{TKey, TValue}"/> was found.</para></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// <b>FindNodeOrPrevious</b> performs a binary search within the <see
        /// cref="BraidedTree{TKey, TValue}"/> for the specified <paramref name="key"/>. This is an
        /// O(ld n) operation.</remarks>

        public BraidedTreeNode<TKey, TValue> FindNodeOrPrevious(TKey key) {
            if (key == null)
                ThrowHelper.ThrowArgumentNullException("key");

            BraidedTreeNode<TKey, TValue> previous = RootNode, node = RootNode._right;

            while (node != null) {
                int result = Comparison(key, node.Key);

                if (result < 0)
                    node = node._left;
                else if (result > 0) {
                    previous = node;
                    node = node._right;
                } else
                    return node;
            }

            return previous;
        }

        #endregion
        #region FindRange(TKey, TKey)

        /// <overloads>
        /// Finds all key-and-value pairs within the <see cref="BraidedTree{TKey, TValue}"/> whose
        /// keys lie within the specified range.</overloads>
        /// <summary>
        /// Finds all key-and-value pairs within the <see cref="BraidedTree{TKey, TValue}"/> whose
        /// keys lie within the specified range.</summary>
        /// <param name="lower">
        /// The lower bound of the key range to search.</param>
        /// <param name="upper">
        /// The upper bound of the key range to search.</param>
        /// <returns>
        /// A <see cref="SortedList{TKey, TValue}"/> ordered by <see cref="Comparison"/> that
        /// contains all elements whose keys are equal to or greater than <paramref name="lower"/>,
        /// and equal to or smaller than <paramref name="upper"/>.</returns>
        /// <remarks><para>
        /// <b>FindRange</b> immediately returns an empty collection if <paramref name="lower"/> is
        /// greater than <paramref name="upper"/>.
        /// </para><para>
        /// Otherwise, <b>FindRange</b> calls <see cref="FindNodeOrPrevious"/> to determine the <see
        /// cref="BraidedTreeNode{TKey, TValue}"/> whose key is equal to or less than <paramref
        /// name="lower"/>, and then follows the chain of <see cref="BraidedTreeNode{TKey,
        /// TValue}.Next"/> references until <paramref name="upper"/> is exceeded.
        /// </para><para>
        /// Depending on the distance between <paramref name="lower"/> and <paramref name="upper"/>,
        /// the runtime of this operation ranges from O(ld n) to O(n).</para></remarks>

        public SortedList<TKey, TValue> FindRange(TKey lower, TKey upper) {
            return FindRange(lower, upper, null);
        }

        #endregion
        #region FindRange(TKey, TKey, Predicate<...>)

        /// <summary>
        /// Finds all key-and-value pairs within the <see cref="BraidedTree{TKey, TValue}"/> whose
        /// keys lie within the specified range, filtered by the specified condition.</summary>
        /// <param name="lower">
        /// The lower bound of the key range to search.</param>
        /// <param name="upper">
        /// The upper bound of the key range to search.</param>
        /// <param name="condition"><para>
        /// The <see cref="Predicate{T}"/> that specifies an additional condition which must hold
        /// for each <see cref="BraidedTree{TKey, TValue}"/> found within the key range.
        /// </para><para>-or-</para><para>
        /// A null reference if there is no additional condition.</para></param>
        /// <returns>
        /// A <see cref="SortedList{TKey, TValue}"/> ordered by <see cref="Comparison"/> that
        /// contains all elements for which <paramref name="condition"/> holds and whose keys are
        /// equal to or greater than <paramref name="lower"/>, and equal to or smaller than
        /// <paramref name="upper"/>.</returns>
        /// <remarks>
        /// <b>FindRange</b> is identical with the basic <see cref="FindRange(TKey, TKey)"/>
        /// overload but also tests that the specified <paramref name="condition"/> holds before
        /// adding a found key-and-value pair to the output collection.</remarks>

        public SortedList<TKey, TValue> FindRange(TKey lower, TKey upper,
            Predicate<BraidedTreeNode<TKey, TValue>> condition) {

            var comparer = new ComparerAdapter<TKey>(Comparison);
            var output = new SortedList<TKey, TValue>(comparer);

            // check for inverted range
            if (Comparison(lower, upper) > 0) return output;

            // find first node at or after lower limit
            var node = FindNodeOrPrevious(lower);
            if (node == RootNode || Comparison(node.Key, lower) < 0)
                node = node._next;

            // append all nodes that do not exceed upper limit
            while (node != RootNode && Comparison(node.Key, upper) <= 0) {

                if (condition == null || condition(node))
                    output.Add(node.Key, node._value);

                node = node._next;
            }

            return output;
        }

        #endregion
        #region GetEnumerator

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that can iterate through the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> for the entire <see cref="BraidedTree{TKey, TValue}"/>.
        /// Each enumerated item is a <see cref="KeyValuePair{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// The <see cref="IEnumerator{T}"/> starts at <see cref="FirstNode"/> and follows the chain
        /// of <see cref="BraidedTreeNode{TKey, TValue}.Next"/> references. Each iteration step is
        /// an O(1) operation.</remarks>

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            for (var node = RootNode._next; node != RootNode; node = node._next)
                yield return new KeyValuePair<TKey, TValue>(node.Key, node._value);
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that can iterate through the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> for the entire <see cref="BraidedTree{TKey, TValue}"/>.
        /// Each enumerated item is a <see cref="KeyValuePair{TKey, TValue}"/>.</returns>

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
        #region Remove(TKey)

        /// <overloads>
        /// Removes the specified element from the <see cref="BraidedTree{TKey, TValue}"/>.
        /// </overloads>
        /// <summary>
        /// Removes the element with the specified key from the <see cref="BraidedTree{TKey,
        /// TValue}"/>.</summary>
        /// <param name="key">
        /// The key of the element to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="key"/> was found and the associated element was removed;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="BraidedTree{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// <b>Remove</b> calls <see cref="FindNode(TKey)"/> to find the specified <paramref
        /// name="key"/>, and then <see cref="RemoveNode"/> with the resulting <see
        /// cref="BraidedTreeNode{TKey, TValue}"/> which may be a null reference.</remarks>

        public bool Remove(TKey key) {
            var node = FindNode(key);
            return RemoveNode(node);
        }

        #endregion
        #region Remove(KeyValuePair<TKey, TValue>)

        /// <summary>
        /// Removes the specified key-and-value pair from the <see cref="BraidedTree{TKey,
        /// TValue}"/>.</summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{TKey, TValue}"/> element to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="pair"/> was found and removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> component of <paramref name="pair"/> is
        /// a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Value"/> component of <paramref name="pair"/>
        /// is an <see cref="IKeyedValue{TKey}"/> instance whose <see cref="IKeyedValue{TKey}.Key"/>
        /// differs from the associated <see cref="KeyValuePair{TKey, TValue}.Key"/> component.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="BraidedTree{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// <b>Remove</b> calls <see cref="FindNode(TKey)"/> to find the <see
        /// cref="KeyValuePair{TKey, TValue}.Key"/> of the specified <paramref name="pair"/>, and
        /// then <see cref="RemoveNode"/> if a matching <see cref="BraidedTreeNode{TKey, TValue}"/>
        /// is found and also contains a matching <see cref="BraidedTreeNode{TKey, TValue}.Value"/>.
        /// </remarks>

        public bool Remove(KeyValuePair<TKey, TValue> pair) {
            if (pair.Value is IKeyedValue<TKey>)
                CollectionsUtility.ValidateKey(pair.Key, pair.Value);

            var node = FindNode(pair.Key);
            if (node == null || !ComparerCache<TValue>.
                EqualityComparer.Equals(pair.Value, node._value))
                return false;

            return RemoveNode(node);
        }

        #endregion
        #region RemoveNode

        /// <summary>
        /// Removes the specified <see cref="BraidedTreeNode{TKey, TValue}"/> from the <see
        /// cref="BraidedTree{TKey, TValue}"/>.</summary>
        /// <param name="node">
        /// The <see cref="BraidedTreeNode{TKey, TValue}"/> to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="node"/> is valid and was removed; otherwise,
        /// <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="node"/> indicates a <see cref="BraidedTreeNode{TKey, TValue}.Tree"/>
        /// that is a null reference or differs from the current instance.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="BraidedTree{TKey, TValue}"/> is read-only.</exception>
        /// <remarks><para>
        /// <b>RemoveNode</b> rebalances the <see cref="BraidedTree{TKey, TValue}"/> to account for
        /// the removal of the specified <paramref name="node"/>. All structural properties of the
        /// specified <paramref name="node"/> are cleared, but its <see cref="BraidedTreeNode{TKey,
        /// TValue}.Key"/> and <see cref="BraidedTreeNode{TKey, TValue}.Value"/> remain intact.
        /// </para><para>
        /// <b>RemoveNode</b> returns <c>false</c> and does nothing if the specified <paramref
        /// name="node"/> is a null reference or the <see cref="RootNode"/>, and throws an exception
        /// if <paramref name="node"/> belongs to a different tree structure, or to none at all.
        /// </para></remarks>

        public bool RemoveNode(BraidedTreeNode<TKey, TValue> node) {
            // CheckWritable();
            if (node == null) return false;
            if (node.Tree != this)
                ThrowHelper.ThrowArgumentException("node", Strings.ArgumentNotInCollection);
            if (node == RootNode) return false;

            node.BubbleDown();
            node.RemoveTree();

            --_count;
            return true;
        }

        #endregion
        #region ToArray

        /// <summary>
        /// Copies the key-and-value pairs of the <see cref="BraidedTree{TKey, TValue}"/> to a new
        /// <see cref="Array"/>.</summary>
        /// <returns>
        /// A one-dimensional <see cref="Array"/> containing copies of the <see
        /// cref="KeyValuePair{TKey, TValue}"/> elements of the <see cref="BraidedTree{TKey,
        /// TValue}"/>.</returns>
        /// <remarks>
        /// <b>ToArray</b> has the same effect as <see cref="CopyTo"/> with a starting index of
        /// zero, but also allocates the target array.</remarks>

        public KeyValuePair<TKey, TValue>[] ToArray() {
            var array = new KeyValuePair<TKey, TValue>[_count];

            int i = 0;
            for (var node = RootNode._next; node != RootNode; node = node._next, i++)
                array[i] = new KeyValuePair<TKey, TValue>(node.Key, node._value);

            return array;
        }

        #endregion
        #region TryAddNode

        /// <summary>
        /// Adds the specified key and value to the <see cref="BraidedTree{TKey, TValue}"/>, and
        /// returns the added <see cref="BraidedTreeNode{TKey, TValue}"/>.</summary>
        /// <param name="key">
        /// The key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add.</param>
        /// <param name="node"><para>
        /// On success, returns the new <see cref="BraidedTreeNode{TKey, TValue}"/> that was added
        /// to the <see cref="BraidedTree{TKey, TValue}"/>.
        /// </para><para>
        /// On failure, returns the unchanged existing <see cref="BraidedTreeNode{TKey, TValue}"/>
        /// that was found for <paramref name="key"/>.</para></param>
        /// <returns>
        /// <c>true</c> if a new <see cref="BraidedTreeNode{TKey, TValue}"/> was added; <c>false</c>
        /// if <paramref name="key"/> already exists in the <see cref="BraidedTree{TKey, TValue}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="IKeyedValue{TKey}.Key"/> differs from <paramref name="key"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="BraidedTree{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// <b>AddNode</b> performs a binary search for the specified <paramref name="key"/>. This
        /// is an O(ld n) operation, regardless of whether <paramref name="key"/> is found or not.
        /// </remarks>

        public bool TryAddNode(TKey key, TValue value, out BraidedTreeNode<TKey, TValue> node) {
            // CheckWritable();
            int result = 1;
            BraidedTreeNode<TKey, TValue> parent = RootNode, search = RootNode._right;

            while (search != null) {
                parent = search;
                result = Comparison(key, search.Key);

                if (result < 0)
                    search = parent._left;
                else if (result > 0)
                    search = parent._right;
                else {
                    node = search;
                    return false;
                }
            }

            search = new BraidedTreeNode<TKey, TValue>(this, key, value);
            parent.AddTree(search, (result > 0));
            search.BubbleUp();

            ++_count;
            node = search;
            return true;
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// <b>TryGetValue</b> calls <see cref="FindNode(TKey)"/> to find the <paramref
        /// name="value"/> associated with the specified <paramref name="key"/>.</remarks>

        public bool TryGetValue(TKey key, out TValue value) {
            var node = FindNode(key);

            if (node != null) {
                value = node._value;
                return true;
            } else {
                value = default(TValue);
                return false;
            }
        }

        #endregion
        #endregion
        #region Class KeyCollection

        private class KeyCollection: ICollection<TKey>, ICollection {

            private readonly BraidedTree<TKey, TValue> _tree;

            internal KeyCollection(BraidedTree<TKey, TValue> tree) {
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

            public void Add(TKey key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public void Clear() {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public bool Contains(TKey key) {
                return _tree.ContainsKey(key);
            }

            public void CopyTo(TKey[] array, int arrayIndex) {
                _tree.CheckTargetArray(array, arrayIndex);
                foreach (var key in this)
                    array[arrayIndex++] = key;
            }

            void ICollection.CopyTo(Array array, int arrayIndex) {
                CopyTo((TKey[]) array, arrayIndex);
            }

            public IEnumerator<TKey> GetEnumerator() {
                var root = _tree.RootNode;
                for (var node = root._next; node != root; node = node._next)
                    yield return node.Key;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public bool Remove(TKey key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }
        }

        #endregion
        #region Class ValueCollection

        private class ValueCollection: ICollection<TValue>, ICollection {

            private readonly BraidedTree<TKey, TValue> _tree;

            internal ValueCollection(BraidedTree<TKey, TValue> tree) {
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
                foreach (var value in this)
                    array[arrayIndex++] = value;
            }

            void ICollection.CopyTo(Array array, int arrayIndex) {
                CopyTo((TValue[]) array, arrayIndex);
            }

            public IEnumerator<TValue> GetEnumerator() {
                var root = _tree.RootNode;
                for (var node = root._next; node != root; node = node._next)
                    yield return node._value;
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
