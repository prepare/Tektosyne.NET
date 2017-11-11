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
    /// <b>SortedDictionaryEx</b> provides a <see cref="SortedDictionary{TKey, TValue}"/>, which is
    /// a red-black binary search tree, with a few extra features:
    /// </para><list type="bullet"><item>
    /// The <see cref="IKeyedValue{TKey}.Key"/> property of any <typeparamref name="TValue"/> that
    /// implements the <see cref="IKeyedValue{TKey}"/> interface is automatically checked against
    /// the associated dictionary key when a key or value is changed or inserted.
    /// </item><item>
    /// <see cref="SortedDictionaryEx{TKey, TValue}.AsReadOnly"/> returns a read-only wrapper that
    /// has the same public type as the original collection. Attempting to modify the collection
    /// through such a read-only view will raise a <see cref="NotSupportedException"/>.
    /// </item><item>
    /// <see cref="SortedDictionaryEx{TKey, TValue}.Copy"/> creates a deep copy of the collection by
    /// invoking <see cref="ICloneable.Clone"/> on each value. This feature requires that all copied
    /// values implement the <see cref="ICloneable"/> interface.
    /// </item><item>
    /// <see cref="SortedDictionaryEx{TKey, TValue}.Empty"/> returns an immutable empty collection
    /// that is cached for repeated access.
    /// </item><item>
    /// <see cref="SortedDictionaryEx{TKey, TValue}.Equals"/> compares two collections with
    /// identical element types for value equality of all elements. The collections compare as equal
    /// if they contain the same elements in the same order.
    /// </item></list><para>
    /// Moreover, several properties and methods that the standard class provides as explicit
    /// interface implementations have been elevated to public visibility.</para></remarks>

    [Serializable]
    public class SortedDictionaryEx<TKey, TValue>:
        IDictionary<TKey, TValue>, IDictionary, ICloneable {
        #region SortedDictionaryEx()

        /// <overloads>
        /// Initializes a new instance of the <see cref="SortedDictionaryEx{TKey, TValue}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionaryEx{TKey, TValue}"/> class
        /// that is empty and uses the default comparer for <typeparamref name="TKey"/>.</summary>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}()"/> for details.</remarks>

        public SortedDictionaryEx() {
            InnerDictionary = new SortedDictionary<TKey, TValue>();
        }

        #endregion
        #region SortedDictionaryEx(IComparer<TKey>)

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionaryEx{TKey, TValue}"/> class
        /// that is empty and uses the specified comparer for <typeparamref name="TKey"/>.</summary>
        /// <param name="comparer">
        /// The <see cref="IComparer{TKey}"/> to use when comparing keys, or a null reference to use
        /// the default <see cref="System.Collections.Generic.Comparer{T}"/> for <typeparamref
        /// name="TKey"/>.</param>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}(IComparer{TKey})"/> for
        /// details.</remarks>

        public SortedDictionaryEx(IComparer<TKey> comparer) {
            InnerDictionary = new SortedDictionary<TKey, TValue>(comparer);
        }

        #endregion
        #region SortedDictionaryEx(IDictionary<TKey, TValue>)

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionaryEx{TKey, TValue}"/> class
        /// that contains elements copied from the specified collection and uses the default
        /// comparer for <typeparamref name="TKey"/>.</summary>
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
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}(IDictionary{TKey, TValue})"/>
        /// for details.</remarks>

        public SortedDictionaryEx(IDictionary<TKey, TValue> dictionary) {
            if (dictionary == null)
                ThrowHelper.ThrowArgumentNullException("dictionary");

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
                CollectionsUtility.ValidateKey(pair.Key, pair.Value);

            InnerDictionary = new SortedDictionary<TKey, TValue>(dictionary);
        }

        #endregion
        #region SortedDictionaryEx(IDictionary<TKey, TValue>, IComparer<TKey>)

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionaryEx{TKey, TValue}"/> class
        /// that contains elements copied from the specified collection and uses the specified
        /// comparer for <typeparamref name="TKey"/>.</summary>
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
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}(IDictionary{TKey, TValue},
        /// IComparer{TKey})"/> for details.</remarks>

        public SortedDictionaryEx(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer) {
            if (dictionary == null)
                ThrowHelper.ThrowArgumentNullException("dictionary");

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
                CollectionsUtility.ValidateKey(pair.Key, pair.Value);

            InnerDictionary = new SortedDictionary<TKey, TValue>(dictionary, comparer);
        }

        #endregion
        #region SortedDictionaryEx(SortedDictionaryEx<TKey, TValue>, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionaryEx{TKey, TValue}"/> class
        /// that is a read-only view of the specified instance.</summary>
        /// <param name="dictionary">
        /// The <see cref="SortedDictionaryEx{TKey, TValue}"/> collection that provides the initial
        /// value for the <see cref="InnerDictionary"/> field.</param>
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

        protected SortedDictionaryEx(SortedDictionaryEx<TKey, TValue> dictionary, bool readOnly) {
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
        /// The <see cref="SortedDictionary{TKey, TValue}"/> collection that holds the <typeparamref
        /// name="TKey"/> keys and <typeparamref name="TValue"/> values of the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>

        protected readonly SortedDictionary<TKey, TValue> InnerDictionary;

        /// <summary>
        /// Backs the <see cref="IsReadOnly"/> property.</summary>

        protected readonly bool ReadOnlyFlag;

        /// <summary>
        /// The read-only <see cref="SortedDictionaryEx{TKey, TValue}"/> collection that is returned
        /// by the <see cref="AsReadOnly"/> method.</summary>

        protected SortedDictionaryEx<TKey, TValue> ReadOnlyWrapper;

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>
        /// <remarks>
        /// Attempting to modify the <b>Empty</b> collection will raise a <see
        /// cref="NotSupportedException"/>. The collection has zero capacity and is guaranteed to
        /// never change, as there are no writable references to the collection.</remarks>

        public static readonly SortedDictionaryEx<TKey, TValue> Empty =
            new SortedDictionaryEx<TKey, TValue>().AsReadOnly();

        #endregion
        #region Public Properties
        #region Comparer

        /// <summary>
        /// Gets the <see cref="IComparer{TKey}"/> that is used to determine the relative order of
        /// keys in the <see cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>
        /// <value>
        /// The <see cref="IComparer{TKey}"/> instance that is used to order keys.</value>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.Comparer"/> for details.
        /// </remarks>

        public IComparer<TKey> Comparer {
            [DebuggerStepThrough]
            get { return InnerDictionary.Comparer; }
        }

        #endregion
        #region Count

        /// <summary>
        /// Gets the number of key-and-value pairs contained in the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>
        /// <value>
        /// The number of <see cref="KeyValuePair{TKey, TValue}"/> elements contained in the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.Count"/> for details.
        /// </remarks>

        public int Count {
            [DebuggerStepThrough]
            get { return InnerDictionary.Count; }
        }

        #endregion
        #region IsFixedSize

        /// <summary>
        /// Gets a value indicating whether the <see cref="SortedDictionaryEx{TKey, TValue}"/> has a
        /// fixed size.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="SortedDictionaryEx{TKey, TValue}"/> has a fixed size;
        /// otherwise, <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="IDictionary.IsFixedSize"/> for details.
        /// </para><para>
        /// This property always returns the same value as the <see cref="IsReadOnly"/> property
        /// since any fixed-size <see cref="SortedDictionaryEx{TKey, TValue}"/> is also read-only,
        /// and vice versa.</para></remarks>

        public bool IsFixedSize {
            [DebuggerStepThrough]
            get { return ReadOnlyFlag; }
        }

        #endregion
        #region IsReadOnly

        /// <summary>
        /// Gets a value indicating whether the <see cref="SortedDictionaryEx{TKey, TValue}"/> is
        /// read-only.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="SortedDictionaryEx{TKey, TValue}"/> is read-only;
        /// otherwise, <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks>
        /// Please refer to <see cref="IDictionary.IsReadOnly"/> for details.</remarks>

        public bool IsReadOnly {
            [DebuggerStepThrough]
            get { return ReadOnlyFlag; }
        }

        #endregion
        #region IsSynchronized

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/> is synchronized (thread-safe).</summary>
        /// <value>
        /// <c>true</c> if access to the <see cref="SortedDictionaryEx{TKey, TValue}"/> is
        /// synchronized (thread-safe); otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
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
        /// specified key and value to the <see cref="SortedDictionaryEx{TKey, TValue}"/>.
        /// </para></value>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// The property is set to an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="KeyValuePair{TKey, TValue}.Key"/> differs from the specified <paramref
        /// name="key"/>.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is read, and <paramref name="key"/> does not exist in the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="SortedDictionaryEx{TKey, TValue}"/> is
        /// read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.this"/> for details.</remarks>

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
        /// specified key and value to the <see cref="SortedDictionaryEx{TKey, TValue}"/>.
        /// </para></value>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="InvalidCastException"><para>
        /// <paramref name="key"/> is not compatible with <typeparamref name="TKey"/>.
        /// </para><para>-or-</para><para>
        /// The property is set to a value that is not compatible with <typeparamref
        /// name="TValue"/>.</para></exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="SortedDictionaryEx{TKey, TValue}"/> is
        /// read-only.</exception>

        object IDictionary.this[object key] {
            [DebuggerStepThrough]
            get { return this[(TKey) key]; }
            [DebuggerStepThrough]
            set { this[(TKey) key] = (TValue) value; }
        }

        #endregion
        #region Keys

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="SortedDictionary{TKey, TValue}.KeyCollection"/> containing the
        /// keys in the <see cref="SortedDictionaryEx{TKey, TValue}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.Keys"/> for details.</remarks>

        public SortedDictionary<TKey, TValue>.KeyCollection Keys {
            [DebuggerStepThrough]
            get { return InnerDictionary.Keys; }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="ICollection{TKey}"/> containing the keys in the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</value>

        ICollection<TKey> IDictionary<TKey, TValue>.Keys {
            [DebuggerStepThrough]
            get { return InnerDictionary.Keys; }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="ICollection"/> containing the keys in the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</value>

        ICollection IDictionary.Keys {
            [DebuggerStepThrough]
            get { return InnerDictionary.Keys; }
        }

        #endregion
        #region SyncRoot

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>
        /// <value>
        /// An object that can be used to synchronize access to the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="ICollection.SyncRoot"/> for details.
        /// </para><para>
        /// When synchronizing multi-threaded access to the <see cref="SortedDictionaryEx{TKey,
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
        /// Gets a collection containing the values in the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="SortedDictionary{TKey, TValue}.ValueCollection"/> containing the
        /// values in the <see cref="SortedDictionaryEx{TKey, TValue}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.Values"/> for details.
        /// </remarks>

        public SortedDictionary<TKey, TValue>.ValueCollection Values {
            [DebuggerStepThrough]
            get { return InnerDictionary.Values; }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="ICollection{TValue}"/> containing the values in the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</value>

        ICollection<TValue> IDictionary<TKey, TValue>.Values {
            [DebuggerStepThrough]
            get { return InnerDictionary.Values; }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="ICollection"/> containing the values in the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</value>

        ICollection IDictionary.Values {
            [DebuggerStepThrough]
            get { return InnerDictionary.Values; }
        }

        #endregion
        #endregion
        #region Public Methods
        #region Add(TKey, TValue)

        /// <overloads>
        /// Adds the specified element to the <see cref="SortedDictionaryEx{TKey, TValue}"/>.
        /// </overloads>
        /// <summary>
        /// Adds the specified key and value to the <see cref="SortedDictionaryEx{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">
        /// The key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> already exists in the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="IKeyedValue{TKey}.Key"/> differs from <paramref name="key"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedDictionaryEx{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.Add(TKey, TValue)"/> for
        /// details.</remarks>

        public void Add(TKey key, TValue value) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);
            if (value is IKeyedValue<TKey>)
                CollectionsUtility.ValidateKey(key, value);

            InnerDictionary.Add(key, value);
        }

        /// <summary>
        /// Adds the specified key and value to the <see cref="SortedDictionaryEx{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">
        /// The key of the element to add. This argument must be compatible with <typeparamref
        /// name="TKey"/>.</param>
        /// <param name="value">
        /// The value of the element to add. This argument must be compatible with <typeparamref
        /// name="TValue"/>.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> already exists in the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="InvalidCastException"><para>
        /// <paramref name="key"/> is not compatible with <typeparamref name="TKey"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="value"/> is not compatible with <typeparamref name="TValue"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedDictionaryEx{TKey, TValue}"/> is read-only.</exception>

        void IDictionary.Add(object key, object value) {
            Add((TKey) key, (TValue) value);
        }

        #endregion
        #region Add(KeyValuePair<TKey, TValue>)

        /// <summary>
        /// Adds the specified key-and-value pair to the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>.</summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{TKey, TValue}"/> element to add.</param>
        /// <exception cref="ArgumentException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> component of <paramref name="pair"/>
        /// already exists in the <see cref="SortedDictionaryEx{TKey, TValue}"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> component of <paramref name="pair"/> is
        /// a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Value"/> component of <paramref name="pair"/>
        /// is an <see cref="IKeyedValue{TKey}"/> instance whose <see cref="IKeyedValue{TKey}.Key"/>
        /// differs from the associated <see cref="KeyValuePair{TKey, TValue}.Key"/> component.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedDictionaryEx{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Add"/> for details.</remarks>

        public void Add(KeyValuePair<TKey, TValue> pair) {
            Add(pair.Key, pair.Value);
        }

        #endregion
        #region AddRange

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="SortedDictionaryEx{TKey,
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
        /// The <see cref="SortedDictionaryEx{TKey, TValue}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="SortedDictionaryEx{TKey, TValue}"/> already contains one or more keys in
        /// the specified <paramref name="dictionary"/>.
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
        /// Returns a read-only view of the <see cref="SortedDictionaryEx{TKey, TValue}"/>.
        /// </summary>
        /// <returns>
        /// A read-only wrapper around the <see cref="SortedDictionaryEx{TKey, TValue}"/>.</returns>
        /// <remarks><para>
        /// Attempting to modify the read-only wrapper returned by <b>AsReadOnly</b> will raise a
        /// <see cref="NotSupportedException"/>. Note that the original collection may still change,
        /// and any such changes will be reflected in the read-only view.
        /// </para><para>
        /// <b>AsReadOnly</b> buffers the newly created read-only wrapper when the method is first
        /// called, and returns the buffered value on subsequent calls.</para></remarks>

        public SortedDictionaryEx<TKey, TValue> AsReadOnly() {

            if (ReadOnlyWrapper == null)
                ReadOnlyWrapper = new SortedDictionaryEx<TKey, TValue>(this, true);

            return ReadOnlyWrapper;
        }

        #endregion
        #region Clear

        /// <summary>
        /// Removes all elements from the <see cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedDictionaryEx{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.Clear"/> for details.
        /// </remarks>

        public void Clear() {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerDictionary.Clear();
        }

        #endregion
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="SortedDictionaryEx{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> does not preserve the values of the <see cref="IsFixedSize"/> and <see
        /// cref="IsReadOnly"/> properties.</remarks>

        public virtual object Clone() {
            return new SortedDictionaryEx<TKey, TValue>(this);
        }

        #endregion
        #region Contains

        /// <summary>
        /// Determines whether the <see cref="SortedDictionaryEx{TKey, TValue}"/> contains the
        /// specified key-and-value pair.</summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{TKey, TValue}"/> element to locate.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="pair"/> is found in the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Contains"/> for details.</remarks>

        public bool Contains(KeyValuePair<TKey, TValue> pair) {
            return ((ICollection<KeyValuePair<TKey, TValue>>) InnerDictionary).Contains(pair);
        }

        #endregion
        #region ContainsKey

        /// <summary>
        /// Determines whether the <see cref="SortedDictionaryEx{TKey, TValue}"/> contains the
        /// specified key.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="SortedDictionaryEx{TKey, TValue}"/> contains an element
        /// with the specified <paramref name="key"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.ContainsKey"/> for details.
        /// </remarks>

        public bool ContainsKey(TKey key) {
            return InnerDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the <see cref="SortedDictionaryEx{TKey, TValue}"/> contains the specified
        /// key.</summary>
        /// <param name="key">
        /// The key to locate. This argument must be compatible with <typeparamref name="TKey"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <see cref="SortedDictionaryEx{TKey, TValue}"/> contains an element with the
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
        /// Determines whether the <see cref="SortedDictionaryEx{TKey, TValue}"/> contains the
        /// specified value.</summary>
        /// <param name="value">
        /// The value to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="SortedDictionaryEx{TKey, TValue}"/> contains an element
        /// with the specified <paramref name="value"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.ContainsValue"/> for details.
        /// </remarks>

        public bool ContainsValue(TValue value) {
            return InnerDictionary.ContainsValue(value);
        }

        #endregion
        #region Copy

        /// <summary>
        /// Creates a deep copy of the <see cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="SortedDictionaryEx{TKey, TValue}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="TValue"/> does not implement <see cref="ICloneable"/>.</exception>
        /// <remarks><para>
        /// <b>Copy</b> is similar to <see cref="Clone"/> but creates a deep copy the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/> by invoking <see cref="ICloneable.Clone"/> on 
        /// all <typeparamref name="TValue"/> values. The <typeparamref name="TKey"/> keys are
        /// always duplicated by a shallow copy.
        /// </para><para>
        /// <b>Copy</b> does not preserve the values of the <see cref="IsFixedSize"/> and <see
        /// cref="IsReadOnly"/> properties.</para></remarks>

        public SortedDictionaryEx<TKey, TValue> Copy() {
            SortedDictionaryEx<TKey, TValue> copy = new SortedDictionaryEx<TKey, TValue>();

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
        /// Copies the entire <see cref="SortedDictionaryEx{TKey, TValue}"/> to a one-dimensional
        /// <see cref="Array"/>, starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the <see
        /// cref="KeyValuePair{TKey, TValue}"/> elements copied from the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>. The <b>Array</b> must have zero-based
        /// indexing.</param>
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
        /// The number of elements in the source <see cref="SortedDictionaryEx{TKey, TValue}"/> is
        /// greater than the available space from <paramref name="arrayIndex"/> to the end of the
        /// destination <paramref name="array"/>.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.CopyTo(KeyValuePair{TKey,
        /// TValue}[], Int32)"/> for details.</remarks>

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            InnerDictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the entire <see cref="SortedDictionaryEx{TKey, TValue}"/> to a one-dimensional
        /// <see cref="Array"/>, starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the <see
        /// cref="KeyValuePair{TKey, TValue}"/> elements copied from the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>. The <b>Array</b> must have zero-based
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
        /// The number of elements in the source <see cref="SortedDictionaryEx{TKey, TValue}"/> is
        /// greater than the available space from <paramref name="arrayIndex"/> to the end of the
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
        /// same order as the current <see cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>
        /// <param name="collection">
        /// The <see cref="ICollection{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/> elements to
        /// compare with the current <see cref="SortedDictionaryEx{TKey, TValue}"/>.</param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="collection"/> is another reference to this <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.
        /// </item><item>
        /// <paramref name="collection"/> is not a null reference, contains the same number of
        /// elements as this <see cref="SortedDictionaryEx{TKey, TValue}"/>, and all elements
        /// compare as equal when retrieved in the enumeration sequence for each collection.
        /// </item></list></returns>
        /// <remarks>
        /// <b>Equals</b> calls <see cref="CollectionsUtility.SequenceEqual"/> to test the two
        /// collections for value equality.</remarks>

        public bool Equals(ICollection<KeyValuePair<TKey, TValue>> collection) {
            return InnerDictionary.SequenceEqual(collection);
        }

        #endregion
        #region GetEnumerator

        /// <summary>
        /// Returns a <see cref="SortedDictionary{TKey, TValue}.Enumerator"/> that can iterate
        /// through the <see cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A <see cref="SortedDictionary{TKey, TValue}.Enumerator"/> for the entire <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>. Each enumerated item is a <see
        /// cref="KeyValuePair{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.GetEnumerator"/> for details.
        /// </remarks>

        public SortedDictionary<TKey, TValue>.Enumerator GetEnumerator() {
            return InnerDictionary.GetEnumerator();
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that can iterate through the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> for the entire <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>. Each enumerated item is a <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </returns>

        IEnumerator<KeyValuePair<TKey, TValue>>
            IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
            return InnerDictionary.GetEnumerator();
        }

        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator"/> that can iterate through the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator"/> for the entire <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>. Each enumerated item is a <see
        /// cref="KeyValuePair{TKey, TValue}"/>.</returns>

        IDictionaryEnumerator IDictionary.GetEnumerator() {
            return InnerDictionary.GetEnumerator();
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that can iterate through the <see
        /// cref="SortedDictionaryEx{TKey, TValue}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> for the entire <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>. Each enumerated item is a <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </returns>

        IEnumerator IEnumerable.GetEnumerator() {
            return InnerDictionary.GetEnumerator();
        }

        #endregion
        #region Remove(TKey)

        /// <overloads>
        /// Removes the specified element from the <see cref="SortedDictionaryEx{TKey, TValue}"/>.
        /// </overloads>
        /// <summary>
        /// Removes the element with the specified key from the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>.</summary>
        /// <param name="key">
        /// The key of the element to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="key"/> was found and the associated element was removed;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedDictionaryEx{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.Remove"/> for details.
        /// </remarks>

        public bool Remove(TKey key) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            return InnerDictionary.Remove(key);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>.</summary>
        /// <param name="key">
        /// The key of the element to remove. This argument must be compatible with <typeparamref
        /// name="TKey"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="InvalidCastException">
        /// <paramref name="key"/> is not compatible with <typeparamref name="TKey"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedDictionaryEx{TKey, TValue}"/> is read-only.</exception>

        void IDictionary.Remove(object key) {
            Remove((TKey) key);
        }

        #endregion
        #region Remove(KeyValuePair<TKey, TValue>)

        /// <summary>
        /// Removes the specified key-and-value pair from the <see cref="SortedDictionaryEx{TKey,
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
        /// The <see cref="SortedDictionaryEx{TKey, TValue}"/> is read-only.</exception>
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
        #region ToArray

        /// <summary>
        /// Copies the key-and-value pairs of the <see cref="SortedDictionaryEx{TKey, TValue}"/> to
        /// a new <see cref="Array"/>.</summary>
        /// <returns>
        /// A one-dimensional <see cref="Array"/> containing copies of the <see
        /// cref="KeyValuePair{TKey, TValue}"/> elements of the <see cref="SortedDictionaryEx{TKey,
        /// TValue}"/>.</returns>
        /// <remarks>
        /// <b>ToArray</b> has the same effect as <see cref="CopyTo"/> with a starting index of
        /// zero, but also allocates the target array.</remarks>

        public KeyValuePair<TKey, TValue>[] ToArray() {
            return InnerDictionary.ToArray<KeyValuePair<TKey, TValue>>();
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
        /// Please refer to <see cref="SortedDictionary{TKey, TValue}.TryGetValue"/> for details.
        /// </remarks>

        public bool TryGetValue(TKey key, out TValue value) {
            return InnerDictionary.TryGetValue(key, out value);
        }

        #endregion
        #endregion
    }
}
