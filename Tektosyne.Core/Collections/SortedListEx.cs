using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides a generic collection of keys and values that are sorted and accessible by key.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of all keys in the collection. Keys cannot be null references.</typeparam>
    /// <typeparam name="TValue">
    /// The type of all values that are associated with the keys. If <typeparamref name="TValue"/>
    /// is a reference type, values may be null references.</typeparam>
    /// <remarks><para>
    /// <b>SortedListEx</b> provides a <see cref="SortedList{TKey, TValue}"/> with a few extra
    /// features:
    /// </para><list type="bullet"><item>
    /// The <see cref="IKeyedValue{TKey}.Key"/> property of any <typeparamref name="TValue"/> that
    /// implements the <see cref="IKeyedValue{TKey}"/> interface is automatically checked against
    /// the associated dictionary key when a key or value is changed or inserted.
    /// </item><item>
    /// <see cref="SortedListEx{TKey, TValue}.AsReadOnly"/> returns a read-only wrapper that has the
    /// same public type as the original collection. Attempting to modify the collection through
    /// such a read-only view will raise a <see cref="NotSupportedException"/>.
    /// </item><item>
    /// <see cref="SortedListEx{TKey, TValue}.Copy"/> creates a deep copy of the collection by
    /// invoking <see cref="ICloneable.Clone"/> on each value. This feature requires that all copied
    /// values implement the <see cref="ICloneable"/> interface.
    /// </item><item>
    /// <see cref="SortedListEx{TKey, TValue}.Empty"/> returns an immutable empty collection that is
    /// cached for repeated access.
    /// </item><item>
    /// <see cref="SortedListEx{TKey, TValue}.Equals"/> compares two collections with identical
    /// element types for value equality of all elements. The collections compare as equal if they
    /// contain the same elements in the same order.
    /// </item><item>
    /// <see cref="SortedListEx{TKey, TValue}.GetByIndex"/>, <see cref="SortedListEx{TKey,
    /// TValue}.GetKey"/>, and <see cref="SortedListEx{TKey, TValue}.SetByIndex"/> are adopted from
    /// the non-generic <see cref="SortedList"/> class.
    /// </item></list><para>
    /// Moreover, several properties and methods that the standard class provides as explicit
    /// interface implementations have been elevated to public visibility.</para></remarks>

    [Serializable]
    public class SortedListEx<TKey, TValue>: IDictionary<TKey, TValue>, IDictionary, ICloneable {
        #region SortedListEx()

        /// <overloads>
        /// Initializes a new instance of the <see cref="SortedListEx{TKey, TValue}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx{TKey, TValue}"/> class that is
        /// empty, has the default initial capacity, and uses the default comparer for <typeparamref
        /// name="TKey"/>.</summary>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}()"/> for details.</remarks>

        public SortedListEx() {
            InnerDictionary = new SortedList<TKey, TValue>();
        }

        #endregion
        #region SortedListEx(IComparer<TKey>)

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx{TKey, TValue}"/> class that is
        /// empty, has the default initial capacity, and uses the specified comparer for
        /// <typeparamref name="TKey"/>.</summary>
        /// <param name="comparer">
        /// The <see cref="IComparer{TKey}"/> to use when comparing keys, or a null reference to use
        /// the default <see cref="System.Collections.Generic.Comparer{T}"/> for <typeparamref
        /// name="TKey"/>.</param>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}(IComparer{TKey})"/> for details.
        /// </remarks>

        public SortedListEx(IComparer<TKey> comparer) {
            InnerDictionary = new SortedList<TKey, TValue>(comparer);
        }

        #endregion
        #region SortedListEx(IDictionary<TKey, TValue>)

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx{TKey, TValue}"/> class that
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
        /// Please refer to <see cref="SortedList{TKey, TValue}(IDictionary{TKey, TValue})"/> for
        /// details.</remarks>

        public SortedListEx(IDictionary<TKey, TValue> dictionary) {
            if (dictionary == null)
                ThrowHelper.ThrowArgumentNullException("dictionary");

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
                CollectionsUtility.ValidateKey(pair.Key, pair.Value);

            InnerDictionary = new SortedList<TKey, TValue>(dictionary);
        }

        #endregion
        #region SortedListEx(IDictionary<TKey, TValue>, IComparer<TKey>)

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx{TKey, TValue}"/> class that
        /// contains elements copied from the specified collection and uses the specified comparer
        /// for <typeparamref name="TKey"/>.</summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new
        /// collection.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{TKey}"/> to use when comparing keys, or a null reference to use
        /// the default <see cref="System.Collections.Generic.Comparer{T}"/> for <typeparamref
        /// name="TKey"/>.</param>
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
        /// Please refer to <see cref="SortedList{TKey, TValue}(IDictionary{TKey, TValue},
        /// IComparer{TKey})"/> for details.</remarks>

        public SortedListEx(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer) {
            if (dictionary == null)
                ThrowHelper.ThrowArgumentNullException("dictionary");

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
                CollectionsUtility.ValidateKey(pair.Key, pair.Value);

            InnerDictionary = new SortedList<TKey, TValue>(dictionary, comparer);
        }

        #endregion
        #region SortedListEx(Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx{TKey, TValue}"/> class that is
        /// empty, has the specified initial capacity, and uses the default comparer for
        /// <typeparamref name="TKey"/>.</summary>
        /// <param name="capacity">
        /// The number of elements that the new <see cref="SortedListEx{TKey, TValue}"/> is
        /// initially capable of storing.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}(Int32)"/> for details.</remarks>

        public SortedListEx(int capacity) {
            InnerDictionary = new SortedList<TKey, TValue>(capacity);
        }

        #endregion
        #region SortedListEx(Int32, IComparer<TKey>)

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx{TKey, TValue}"/> class that is
        /// empty, has the specified initial capacity, and uses the specified comparer for
        /// <typeparamref name="TKey"/>.</summary>
        /// <param name="capacity">
        /// The number of elements that the new <see cref="SortedListEx{TKey, TValue}"/> is
        /// initially capable of storing.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{TKey}"/> to use when comparing keys, or a null reference to use
        /// the default <see cref="System.Collections.Generic.Comparer{T}"/> for <typeparamref
        /// name="TKey"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}(Int32, IComparer{TKey})"/> for
        /// details.</remarks>

        public SortedListEx(int capacity, IComparer<TKey> comparer) {
            InnerDictionary = new SortedList<TKey, TValue>(capacity, comparer);
        }

        #endregion
        #region SortedListEx(SortedListEx<TKey, TValue>, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedListEx{TKey, TValue}"/> class that is
        /// a read-only view of the specified instance.</summary>
        /// <param name="dictionary">
        /// The <see cref="SortedList{TKey, TValue}"/> collection that provides the initial value
        /// for the <see cref="InnerDictionary"/> field.</param>
        /// <param name="readOnly">
        /// The initial value for the <see cref="IsReadOnly"/> property. This argument must be
        /// <c>true</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dictionary"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="readOnly"/> is <c>false</c>.</exception>
        /// <remarks>
        /// This constructor is used to create a read-only wrapper around an existing collection.
        /// The new instance shares the data of the specified <paramref name="dictionary"/>.
        /// </remarks>

        protected SortedListEx(SortedListEx<TKey, TValue> dictionary, bool readOnly) {
            if (dictionary == null)
                ThrowHelper.ThrowArgumentNullException("dictionary");

            if (!readOnly)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "readOnly", Strings.ArgumentEquals, false);

            InnerDictionary = dictionary.InnerDictionary;
            ReadOnlyFlag = readOnly;
            ReadOnlyWrapper = this;
        }

        #endregion
        #region Protected Fields

        /// <summary>
        /// The <see cref="SortedList{TKey, TValue}"/> collection that holds the <typeparamref
        /// name="TKey"/> keys and <typeparamref name="TValue"/> values of the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</summary>

        protected readonly SortedList<TKey, TValue> InnerDictionary;

        /// <summary>
        /// Backs the <see cref="IsReadOnly"/> property.</summary>

        protected readonly bool ReadOnlyFlag;

        /// <summary>
        /// The read-only <see cref="SortedListEx{TKey, TValue}"/> collection that is returned by
        /// the <see cref="AsReadOnly"/> method.</summary>

        protected SortedListEx<TKey, TValue> ReadOnlyWrapper;

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="SortedListEx{TKey, TValue}"/>.</summary>
        /// <remarks>
        /// Attempting to modify the <b>Empty</b> collection will raise a <see
        /// cref="NotSupportedException"/>. The collection has zero capacity and is guaranteed to
        /// never change, as there are no writable references to the collection.</remarks>

        public static readonly SortedListEx<TKey, TValue> Empty =
            new SortedListEx<TKey, TValue>(0).AsReadOnly();

        #endregion
        #region Public Properties
        #region Capacity

        /// <summary>
        /// Gets or sets the capacity of the <see cref="SortedListEx{TKey, TValue}"/>.</summary>
        /// <value>
        /// The number of elements that the <see cref="SortedListEx{TKey, TValue}"/> can contain.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The property is set to a value that is less than <see cref="Count"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="SortedListEx{TKey, TValue}"/> is read-only.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.Capacity"/> for details.</remarks>

        public int Capacity {
            [DebuggerStepThrough]
            get { return InnerDictionary.Capacity; }
            set {
                if (ReadOnlyFlag)
                    ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

                InnerDictionary.Capacity = value;
            }
        }

        #endregion
        #region Comparer

        /// <summary>
        /// Gets the <see cref="IComparer{TKey}"/> that is used to determine the relative order of
        /// keys in the <see cref="SortedListEx{TKey, TValue}"/>.</summary>
        /// <value>
        /// The <see cref="IComparer{TKey}"/> instance that is used to order keys.</value>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.Comparer"/> for details.</remarks>

        public IComparer<TKey> Comparer {
            [DebuggerStepThrough]
            get { return InnerDictionary.Comparer; }
        }

        #endregion
        #region Count

        /// <summary>
        /// Gets the number of key-and-value pairs contained in the <see cref="SortedListEx{TKey,
        /// TValue}"/>.</summary>
        /// <value>
        /// The number of <see cref="KeyValuePair{TKey, TValue}"/> elements contained in the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.Count"/> for details.</remarks>

        public int Count {
            [DebuggerStepThrough]
            get { return InnerDictionary.Count; }
        }

        #endregion
        #region IsFixedSize

        /// <summary>
        /// Gets a value indicating whether the <see cref="SortedListEx{TKey, TValue}"/> has a fixed
        /// size.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="SortedListEx{TKey, TValue}"/> has a fixed size; otherwise,
        /// <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="IDictionary.IsFixedSize"/> for details.
        /// </para><para>
        /// This property always returns the same value as the <see cref="IsReadOnly"/> property
        /// since any fixed-size <see cref="SortedListEx{TKey, TValue}"/> is also read-only, and
        /// vice versa.</para></remarks>

        public bool IsFixedSize {
            [DebuggerStepThrough]
            get { return ReadOnlyFlag; }
        }

        #endregion
        #region IsReadOnly

        /// <summary>
        /// Gets a value indicating whether the <see cref="SortedListEx{TKey, TValue}"/> is
        /// read-only.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="SortedListEx{TKey, TValue}"/> is read-only; otherwise,
        /// <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks>
        /// Please refer to <see cref="IDictionary.IsReadOnly"/> for details.</remarks>

        public bool IsReadOnly {
            [DebuggerStepThrough]
            get { return ReadOnlyFlag; }
        }

        #endregion
        #region IsSynchronized

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="SortedListEx{TKey, TValue}"/>
        /// is synchronized (thread-safe).</summary>
        /// <value>
        /// <c>true</c> if access to the <see cref="SortedListEx{TKey, TValue}"/> is synchronized
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
        /// specified key and value to the <see cref="SortedListEx{TKey, TValue}"/>.</para></value>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// The property is set to an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="KeyValuePair{TKey, TValue}.Key"/> differs from the specified <paramref
        /// name="key"/>.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is read, and <paramref name="key"/> does not exist in the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="SortedListEx{TKey, TValue}"/> is read-only.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.this"/> for details.</remarks>

        public TValue this[TKey key] {
            [DebuggerStepThrough]
            get { return InnerDictionary[key]; }
            set {
                if (ReadOnlyFlag)
                    ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);
                if (value is IKeyedValue<TKey>)
                    CollectionsUtility.ValidateKey(key, value);

                InnerDictionary[key] = value;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.</summary>
        /// <param name="key">
        /// The key whose value to get or set. This argument must be compatible with <typeparamref
        /// name="TKey"/>.</param>
        /// <value><para>
        /// The value associated with the specified <paramref name="key"/>. When the property is
        /// set, this argument must be compatible with <typeparamref name="TValue"/>.
        /// </para><para>
        /// If <paramref name="key"/> is not found, attempting to get it throws a <see
        /// cref="KeyNotFoundException"/>, and attempting to set it adds a new element with the
        /// specified key and value to the <see cref="SortedListEx{TKey, TValue}"/>.</para></value>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="InvalidCastException"><para>
        /// <paramref name="key"/> is not compatible with <typeparamref name="TKey"/>.
        /// </para><para>-or-</para><para>
        /// The property is set to a value that is not compatible with <typeparamref
        /// name="TValue"/>.</para></exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="SortedListEx{TKey, TValue}"/> is read-only.
        /// </exception>

        object IDictionary.this[object key] {
            [DebuggerStepThrough]
            get { return this[(TKey) key]; }
            [DebuggerStepThrough]
            set { this[(TKey) key] = (TValue) value; }
        }

        #endregion
        #region Keys

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="SortedListEx{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        /// A read-only <see cref="IList{TKey}"/> containing the keys in the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.Keys"/> for details.</remarks>

        public IList<TKey> Keys {
            [DebuggerStepThrough]
            get { return InnerDictionary.Keys; }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="SortedListEx{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        /// A read-only <see cref="ICollection{TKey}"/> containing the keys in the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</value>

        ICollection<TKey> IDictionary<TKey, TValue>.Keys {
            [DebuggerStepThrough]
            get { return InnerDictionary.Keys; }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="SortedListEx{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        /// A read-only <see cref="ICollection"/> containing the keys in the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</value>

        ICollection IDictionary.Keys {
            [DebuggerStepThrough]
            get { return ((IDictionary) InnerDictionary).Keys; }
        }

        #endregion
        #region SyncRoot

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</summary>
        /// <value>
        /// An object that can be used to synchronize access to the <see cref="SortedListEx{TKey,
        /// TValue}"/>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="ICollection.SyncRoot"/> for details.
        /// </para><para>
        /// When synchronizing multi-threaded access to the <see cref="SortedListEx{TKey,
        /// TValue}"/>, obtain a lock on the <b>SyncRoot</b> object rather than the collection
        /// itself. A read-only view always returns the same <b>SyncRoot</b> object as the
        /// underlying writable collection.</para></remarks>

        public object SyncRoot {
            [DebuggerStepThrough]
            get { return ((ICollection) InnerDictionary).SyncRoot; }
        }

        #endregion
        #region Values

        /// <summary>
        /// Gets a collection containing the values in the <see cref="SortedListEx{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        /// A read-only <see cref="IList{TValue}"/> containing the values in the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.Values"/> for details.</remarks>

        public IList<TValue> Values {
            [DebuggerStepThrough]
            get { return InnerDictionary.Values; }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="SortedListEx{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        /// A read-only <see cref="ICollection{TValue}"/> containing the values in the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</value>

        ICollection<TValue> IDictionary<TKey, TValue>.Values {
            [DebuggerStepThrough]
            get { return InnerDictionary.Values; }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="SortedListEx{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        /// A read-only <see cref="ICollection"/> containing the values in the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</value>

        ICollection IDictionary.Values {
            [DebuggerStepThrough]
            get { return ((IDictionary) InnerDictionary).Values; }
        }

        #endregion
        #endregion
        #region Public Methods
        #region Add(TKey, TValue)

        /// <overloads>
        /// Adds the specified element to the <see cref="SortedListEx{TKey, TValue}"/>.</overloads>
        /// <summary>
        /// Adds the specified key and value to the <see cref="SortedListEx{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">
        /// The key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> already exists in the <see cref="SortedListEx{TKey, TValue}"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="IKeyedValue{TKey}.Key"/> differs from <paramref name="key"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedListEx{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.Add(TKey, TValue)"/> for details.
        /// </remarks>

        public void Add(TKey key, TValue value) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);
            if (value is IKeyedValue<TKey>)
                CollectionsUtility.ValidateKey(key, value);

            InnerDictionary.Add(key, value);
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="SortedListEx{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">
        /// The key of the element to add. This argument must be compatible with <typeparamref
        /// name="TKey"/>.</param>
        /// <param name="value">
        /// The value of the element to add. This argument must be compatible with <typeparamref
        /// name="TValue"/>.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> already exists in the <see cref="SortedListEx{TKey, TValue}"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="InvalidCastException"><para>
        /// <paramref name="key"/> is not compatible with <typeparamref name="TKey"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="value"/> is not compatible with <typeparamref name="TValue"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedListEx{TKey, TValue}"/> is read-only.</exception>

        void IDictionary.Add(object key, object value) {
            Add((TKey) key, (TValue) value);
        }

        #endregion
        #region Add(KeyValuePair<TKey, TValue>)

        /// <summary>
        /// Adds the specified key-and-value pair to the <see cref="SortedListEx{TKey, TValue}"/>.
        /// </summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{TKey, TValue}"/> element to add.</param>
        /// <exception cref="ArgumentException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> component of <paramref name="pair"/>
        /// already exists in the <see cref="SortedListEx{TKey, TValue}"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> component of <paramref name="pair"/> is
        /// a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Value"/> component of <paramref name="pair"/>
        /// is an <see cref="IKeyedValue{TKey}"/> instance whose <see cref="IKeyedValue{TKey}.Key"/>
        /// differs from the associated <see cref="KeyValuePair{TKey, TValue}.Key"/> component.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedListEx{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Add"/> for details.</remarks>

        public void Add(KeyValuePair<TKey, TValue> pair) {
            Add(pair.Key, pair.Value);
        }

        #endregion
        #region AddRange

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="SortedListEx{TKey,
        /// TValue}"/>. </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> whose elements to add.</param>
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
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="SortedListEx{TKey, TValue}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="SortedListEx{TKey, TValue}"/> already contains one or more keys in the
        /// specified <paramref name="dictionary"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="dictionary"/> contains one or more duplicate keys.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.AddRange"/> for details.</remarks>

        public void AddRange(IDictionary<TKey, TValue> dictionary) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);
            if (dictionary == null)
                ThrowHelper.ThrowArgumentNullException("dictionary");

            foreach (KeyValuePair<TKey, TValue> pair in dictionary) {
                TKey key = pair.Key; TValue value = pair.Value;

                if (value is IKeyedValue<TKey>)
                    CollectionsUtility.ValidateKey(key, value);

                InnerDictionary.Add(key, value);
            }
        }

        #endregion
        #region AsReadOnly

        /// <summary>
        /// Returns a read-only view of the <see cref="SortedListEx{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A read-only wrapper around the <see cref="SortedListEx{TKey, TValue}"/>.</returns>
        /// <remarks><para>
        /// Attempting to modify the read-only wrapper returned by <b>AsReadOnly</b> will raise a
        /// <see cref="NotSupportedException"/>. Note that the original collection may still change,
        /// and any such changes will be reflected in the read-only view.
        /// </para><para>
        /// <b>AsReadOnly</b> buffers the newly created read-only wrapper when the method is first
        /// called, and returns the buffered value on subsequent calls.</para></remarks>

        public SortedListEx<TKey, TValue> AsReadOnly() {

            if (ReadOnlyWrapper == null)
                ReadOnlyWrapper = new SortedListEx<TKey, TValue>(this, true);

            return ReadOnlyWrapper;
        }

        #endregion
        #region Clear

        /// <summary>
        /// Removes all elements from the <see cref="SortedListEx{TKey, TValue}"/>.</summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedListEx{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.Clear"/> for details.</remarks>

        public void Clear() {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerDictionary.Clear();
        }

        #endregion
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="SortedListEx{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="SortedListEx{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> does not preserve the values of the <see cref="IsFixedSize"/> and <see
        /// cref="IsReadOnly"/> properties.</remarks>

        public virtual object Clone() {
            return new SortedListEx<TKey, TValue>(this);
        }

        #endregion
        #region Contains

        /// <summary>
        /// Determines whether the <see cref="SortedListEx{TKey, TValue}"/> contains the specified
        /// key-and-value pair.</summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{TKey, TValue}"/> element to locate.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="pair"/> is found in the <see cref="SortedListEx{TKey,
        /// TValue}"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Contains"/> for details.</remarks>

        public bool Contains(KeyValuePair<TKey, TValue> pair) {
            return ((ICollection<KeyValuePair<TKey, TValue>>) InnerDictionary).Contains(pair);
        }

        #endregion
        #region ContainsKey

        /// <summary>
        /// Determines whether the <see cref="SortedListEx{TKey, TValue}"/> contains the specified
        /// key.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="SortedListEx{TKey, TValue}"/> contains an element with the
        /// specified <paramref name="key"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.ContainsKey"/> for details.
        /// </remarks>

        public bool ContainsKey(TKey key) {
            return InnerDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the <see cref="SortedListEx{TKey, TValue}"/> contains the specified
        /// key.</summary>
        /// <param name="key">
        /// The key to locate. This argument must be compatible with <typeparamref name="TKey"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <see cref="SortedListEx{TKey, TValue}"/> contains an element with the
        /// specified <paramref name="key"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="InvalidCastException">
        /// <paramref name="key"/> is not compatible with <typeparamref name="TKey"/>.</exception>

        bool IDictionary.Contains(object key) {
            return ContainsKey((TKey) key);
        }

        #endregion
        #region ContainsValue

        /// <summary>
        /// Determines whether the <see cref="SortedListEx{TKey, TValue}"/> contains the specified
        /// value.</summary>
        /// <param name="value">
        /// The value to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="SortedListEx{TKey, TValue}"/> contains an element with the
        /// specified <paramref name="value"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.ContainsValue"/> for details.
        /// </remarks>

        public bool ContainsValue(TValue value) {
            return InnerDictionary.ContainsValue(value);
        }

        #endregion
        #region Copy

        /// <summary>
        /// Creates a deep copy of the <see cref="SortedListEx{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="SortedListEx{TKey, TValue}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="TValue"/> does not implement <see cref="ICloneable"/>.</exception>
        /// <remarks><para>
        /// <b>Copy</b> is similar to <see cref="Clone"/> but creates a deep copy the <see
        /// cref="SortedListEx{TKey, TValue}"/> by invoking <see cref="ICloneable.Clone"/> on all 
        /// <typeparamref name="TValue"/> values. The <typeparamref name="TKey"/> keys are always
        /// duplicated by a shallow copy.
        /// </para><para>
        /// <b>Copy</b> does not preserve the values of the <see cref="IsFixedSize"/> and <see
        /// cref="IsReadOnly"/> properties.</para></remarks>

        public SortedListEx<TKey, TValue> Copy() {
            SortedListEx<TKey, TValue> copy = new SortedListEx<TKey, TValue>(Count);

            foreach (KeyValuePair<TKey, TValue> pair in InnerDictionary) {
                TValue value = pair.Value;

                ICloneable cloneable = (ICloneable) value;
                if (cloneable != null)
                    value = (TValue) cloneable.Clone();

                copy.InnerDictionary.Add(pair.Key, value);
            }

            return copy;
        }

        #endregion
        #region CopyTo

        /// <summary>
        /// Copies the entire <see cref="SortedListEx{TKey, TValue}"/> to a one-dimensional <see
        /// cref="Array"/>, starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the <see
        /// cref="KeyValuePair{TKey, TValue}"/> elements copied from the <see
        /// cref="SortedListEx{TKey, TValue}"/>. The <b>Array</b> must have zero-based indexing.
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
        /// The number of elements in the source <see cref="SortedListEx{TKey, TValue}"/> is greater
        /// than the available space from <paramref name="arrayIndex"/> to the end of the
        /// destination <paramref name="array"/>.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.CopyTo(T[], Int32)"/> for details.</remarks>

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            ((ICollection<KeyValuePair<TKey, TValue>>) InnerDictionary).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the entire <see cref="SortedListEx{TKey, TValue}"/> to a one-dimensional <see
        /// cref="Array"/>, starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the <see
        /// cref="KeyValuePair{TKey, TValue}"/> elements copied from the <see
        /// cref="SortedListEx{TKey, TValue}"/>. The <b>Array</b> must have zero-based indexing.
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
        /// The number of elements in the source <see cref="SortedListEx{TKey, TValue}"/> is greater
        /// than the available space from <paramref name="arrayIndex"/> to the end of the
        /// destination <paramref name="array"/>.</para></exception>
        /// <exception cref="InvalidCastException">
        /// <see cref="KeyValuePair{TKey, TValue}"/> cannot be cast automatically to the type of the
        /// destination <paramref name="array"/>.</exception>

        void ICollection.CopyTo(Array array, int arrayIndex) {
            ((ICollection) InnerDictionary).CopyTo(array, arrayIndex);
        }

        #endregion
        #region Equals

        /// <summary>
        /// Determines whether the specified collection contains the same key-and-value pairs in the
        /// same order as the current <see cref="SortedListEx{TKey, TValue}"/>.</summary>
        /// <param name="collection">
        /// The <see cref="ICollection{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/> elements to
        /// compare with the current <see cref="SortedListEx{TKey, TValue}"/>.</param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="collection"/> is another reference to this <see cref="SortedListEx{TKey,
        /// TValue}"/>.
        /// </item><item>
        /// <paramref name="collection"/> is not a null reference, contains the same number of
        /// elements as this <see cref="SortedListEx{TKey, TValue}"/>, and all elements compare as
        /// equal when retrieved in the enumeration sequence for each collection.
        /// </item></list></returns>
        /// <remarks>
        /// <b>Equals</b> calls <see cref="CollectionsUtility.SequenceEqual"/> to test the two
        /// collections for value equality.</remarks>

        public bool Equals(ICollection<KeyValuePair<TKey, TValue>> collection) {
            return InnerDictionary.SequenceEqual(collection);
        }

        #endregion
        #region GetByIndex

        /// <summary>
        /// Gets the value at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index of the value to get.</param>
        /// <returns>
        /// The value at the specified <paramref name="index"/> in the <see cref="SortedListEx{TKey,
        /// TValue}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="SortedListEx{TKey,
        /// TValue}.Count"/>.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList.GetByIndex"/> for details.</remarks>

        public TValue GetByIndex(int index) {
            return InnerDictionary.Values[index];
        }

        #endregion
        #region GetEnumerator

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that can iterate through the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> for the entire <see cref="SortedListEx{TKey, TValue}"/>.
        /// Each enumerated item is a <see cref="KeyValuePair{TKey, TValue}"/>.</returns>

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return InnerDictionary.GetEnumerator();
        }

        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator"/> that can iterate through the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</summary>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator"/> for the entire <see cref="SortedListEx{TKey,
        /// TValue}"/>. Each enumerated item is a <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </returns>

        IDictionaryEnumerator IDictionary.GetEnumerator() {
            return ((IDictionary) InnerDictionary).GetEnumerator();
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that can iterate through the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> for the entire <see cref="SortedListEx{TKey, TValue}"/>.
        /// Each enumerated item is a <see cref="KeyValuePair{TKey, TValue}"/>.</returns>

        IEnumerator IEnumerable.GetEnumerator() {
            return InnerDictionary.GetEnumerator();
        }

        #endregion
        #region GetKey

        /// <summary>
        /// Gets the key at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index of the key to get.</param>
        /// <returns>
        /// The key at the specified <paramref name="index"/> in the <see cref="SortedListEx{TKey,
        /// TValue}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="SortedListEx{TKey,
        /// TValue}.Count"/>.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList.GetKey"/> for details.</remarks>

        public TKey GetKey(int index) {
            return InnerDictionary.Keys[index];
        }

        #endregion
        #region IndexOfKey

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified key.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="key"/> in the <see
        /// cref="SortedListEx{TKey, TValue}"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.IndexOfKey"/> for details.</remarks>

        public int IndexOfKey(TKey key) {
            return InnerDictionary.IndexOfKey(key);
        }

        #endregion
        #region IndexOfValue

        /// <summary>
        /// Returns the zero-based index of first occurrence of the specified value.</summary>
        /// <param name="value">
        /// The value to locate.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="value"/> in the <see
        /// cref="SortedListEx{TKey, TValue}"/>, if found; otherwise, -1.</returns>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.IndexOfValue"/> for details.
        /// </remarks>

        public int IndexOfValue(TValue value) {
            return InnerDictionary.IndexOfValue(value);
        }

        #endregion
        #region Remove(TKey)

        /// <overloads>
        /// Removes the specified element from the <see cref="SortedListEx{TKey, TValue}"/>.
        /// </overloads>
        /// <summary>
        /// Removes the element with the specified key from the <see cref="SortedListEx{TKey,
        /// TValue}"/>.</summary>
        /// <param name="key">
        /// The key of the element to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="key"/> was found and the associated element was removed;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedListEx{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.Remove"/> for details.</remarks>

        public bool Remove(TKey key) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            return InnerDictionary.Remove(key);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="SortedListEx{TKey,
        /// TValue}"/>.</summary>
        /// <param name="key">
        /// The key of the element to remove. This argument must be compatible with <typeparamref
        /// name="TKey"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="InvalidCastException">
        /// <paramref name="key"/> is not compatible with <typeparamref name="TKey"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedListEx{TKey, TValue}"/> is read-only.</exception>

        void IDictionary.Remove(object key) {
            Remove((TKey) key);
        }

        #endregion
        #region Remove(KeyValuePair<TKey, TValue>)

        /// <summary>
        /// Removes the specified key-and-value pair from the <see cref="SortedListEx{TKey,
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
        /// The <see cref="SortedListEx{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Remove"/> for details.</remarks>

        public bool Remove(KeyValuePair<TKey, TValue> pair) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);
            if (pair.Value is IKeyedValue<TKey>)
                CollectionsUtility.ValidateKey(pair.Key, pair.Value);

            return ((ICollection<KeyValuePair<TKey, TValue>>) InnerDictionary).Remove(pair);
        }

        #endregion
        #region RemoveAt

        /// <summary>
        /// Removes the element at the specified index in the <see cref="SortedListEx{TKey,
        /// TValue}"/>.</summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedListEx{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.RemoveAt"/> for details.</remarks>

        public void RemoveAt(int index) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerDictionary.RemoveAt(index);
        }

        #endregion
        #region SetByIndex

        /// <summary>
        /// Sets the value at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index of the value to set.</param>
        /// <param name="value">
        /// The value to store at the specified <paramref name="index"/> in the <see
        /// cref="SortedListEx{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="SortedListEx{TKey,
        /// TValue}.Count"/>.</para></exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="IKeyedValue{TKey}.Key"/> differs from the key at the specified <paramref
        /// name="index"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedListEx{TKey, TValue}"/> is read-only.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="SortedList.SetByIndex"/> for details.
        /// </para><para>
        /// <b>SetByIndex</b> must perform a key search since the generic <see
        /// cref="SortedList{TKey, TValue}"/> class does not provide direct write access by
        /// positional indexing. Unlike the non-generic equivalent, this method is therefore
        /// <em>less</em> efficient than using the key indexer.</para></remarks>

        public void SetByIndex(int index, TValue value) {
            TKey key = InnerDictionary.Keys[index];
            this[key] = value;
        }

        #endregion
        #region ToArray

        /// <summary>
        /// Copies the key-and-value pairs of the <see cref="SortedListEx{TKey, TValue}"/> to a new
        /// <see cref="Array"/>.</summary>
        /// <returns>
        /// A one-dimensional <see cref="Array"/> containing copies of the <see
        /// cref="KeyValuePair{TKey, TValue}"/> elements of the <see cref="SortedListEx{TKey,
        /// TValue}"/>.</returns>
        /// <remarks>
        /// <b>ToArray</b> has the same effect as <see cref="CopyTo"/> with a starting index of
        /// zero, but also allocates the target array.</remarks>

        public KeyValuePair<TKey, TValue>[] ToArray() {
            return InnerDictionary.ToArray<KeyValuePair<TKey, TValue>>();
        }

        #endregion
        #region TrimExcess

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="SortedListEx{TKey,
        /// TValue}"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedListEx{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.TrimExcess"/> for details.</remarks>

        public void TrimExcess() {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerDictionary.TrimExcess();
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
        /// Please refer to <see cref="SortedList{TKey, TValue}.TryGetValue"/> for details.
        /// </remarks>

        public bool TryGetValue(TKey key, out TValue value) {
            return InnerDictionary.TryGetValue(key, out value);
        }

        #endregion
        #endregion
    }
}
