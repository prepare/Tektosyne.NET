using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides an unsorted generic collection of <see cref="Int32"/> keys and arbitrary values
    /// that are accessible by key.</summary>
    /// <typeparam name="TValue">
    /// The type of all values that are associated with the keys. If <typeparamref name="TValue"/>
    /// is a reference type, values may be null references.</typeparam>
    /// <remarks><para>
    /// <b>Int32Dictionary</b> is a complete reimplementation of the standard <see
    /// cref="Dictionary{TKey, TValue}"/>, which is a dynamic hashtable, so as to achieve the best
    /// possible performance for <see cref="Int32"/> keys.
    /// </para><list type="bullet"><item>
    /// Hashtables are ideally suited for <see cref="Int32"/> keys because such keys can act as
    /// their own hash codes, obviating the need to transform keys into hash codes for searches.
    /// This removes a potentially significant contribution to the cost of searches.
    /// </item><item>
    /// <see cref="Dictionary{TKey, TValue}"/> must route <see cref="Int32"/> keys through the same
    /// expensive mechanism as other generic keys, using an <see cref="EqualityComparer{T}"/> to
    /// obtain hash codes, and then a <see cref="Comparer{T}"/> to determine key equality.
    /// </item><item>
    /// <b>Int32Dictionary</b> directly uses <see cref="Int32"/> keys as their own hash codes, and
    /// directly compares them to stored keys using a single machine instruction. The resulting
    /// search speedup compared to a standard <see cref="Dictionary{TKey, TValue}"/> instantiated
    /// with <see cref="Int32"/> keys exceeds 60% in an optimized x64 build under .NET 4.0.
    /// </item></list><para>
    /// <b>Int32Dictionary</b> also contains the following extra features:
    /// </para><list type="bullet"><item>
    /// The <see cref="IKeyedValue{Int32}.Key"/> property of any <typeparamref name="TValue"/> that
    /// implements the <see cref="IKeyedValue{Int32}"/> interface is automatically checked against
    /// the associated dictionary key when a key or value is changed or inserted.
    /// </item><item>
    /// <see cref="Int32Dictionary{TValue}.AsReadOnly"/> returns a read-only wrapper that has the
    /// same public type as the original collection. Attempting to modify the collection through
    /// such a read-only view will raise a <see cref="NotSupportedException"/>.
    /// </item><item>
    /// <see cref="Int32Dictionary{TValue}.Copy"/> creates a deep copy of the collection by invoking
    /// <see cref="ICloneable.Clone"/> on each value. This feature requires that all copied values
    /// implement the <see cref="ICloneable"/> interface.
    /// </item><item>
    /// <see cref="Int32Dictionary{TValue}.Empty"/> returns an immutable empty collection that is
    /// cached for repeated access.
    /// </item><item>
    /// <see cref="Int32Dictionary{TValue}.Equals"/> compares two collections with identical element
    /// types for value equality of all elements. The collections compare as equal if they contain
    /// the same elements. The enumeration order of elements is ignored since the
    /// <b>Int32Dictionary</b> class does not establish any fixed element ordering.
    /// </item><item>
    /// <see cref="Int32Dictionary{TValue}.GetAny"/> returns an arbitrary key-and-value pair if the
    /// collection is not empty. This is equivalent to getting the first element yielded by an
    /// enumerator, but without actually creating the enumerator.
    /// </item></list><para>
    /// Moreover, several properties and methods that the standard class provides as explicit
    /// interface implementations have been elevated to public visibility. <b>Int32Dictionary</b>
    /// does <em>not</em> support a few rarely used features of the standard class:
    /// </para><list type="bullet"><item>
    /// <b>Int32Dictionary</b> does not implement the non-generic <see cref="IDictionary"/>
    /// interface. This would require implementing <see cref="IDictionaryEnumerator"/>, a peculiar
    /// enumerator interface that is incompatible with the C# <c>yield</c> keyword.
    /// </item><item>
    /// <see cref="Int32Dictionary{TValue}.CopyTo"/> does not support target arrays containing
    /// non-generic <see cref="DictionaryEntry"/> or <see cref="Object"/> elements.
    /// </item></list></remarks>

    [Serializable]
    public class Int32Dictionary<TValue>: IDictionary<Int32, TValue>, ICollection, ICloneable {
        #region Int32Dictionary()

        /// <overloads>
        /// Initializes a new instance of the <see cref="Int32Dictionary{TValue}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="Int32Dictionary{TValue}"/> class that is
        /// empty and has the default initial capacity.</summary>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}()"/> for details.</remarks>

        public Int32Dictionary(): this(0) { }

        #endregion
        #region Int32Dictionary(IDictionary<TKey, TValue>)

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32Dictionary{TValue}"/> class that
        /// contains elements copied from the specified collection.</summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{Int32, TValue}"/> whose elements are copied to the new
        /// collection.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dictionary"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="dictionary"/> contains an element whose <see cref="KeyValuePair{Int32,
        /// TValue}.Value"/> is an <see cref="IKeyedValue{Int32}"/> instance whose <see
        /// cref="IKeyedValue{Int32}.Key"/> differs from the associated <see
        /// cref="KeyValuePair{Int32, TValue}.Key"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}(IDictionary{TKey, TValue})"/> for
        /// details.</remarks>

        public Int32Dictionary(IDictionary<Int32, TValue> dictionary): this(0) {
            AddRange(dictionary);
        }

        #endregion
        #region Int32Dictionary(Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32Dictionary{TValue}"/> class that is
        /// empty and has the specified initial capacity.</summary>
        /// <param name="capacity">
        /// The number of elements that the new <see cref="Int32Dictionary{TValue}"/> is initially
        /// capable of storing.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}(Int32)"/> for details.</remarks>

        public Int32Dictionary(int capacity) {
            _data = new InstanceData(capacity);
        }

        #endregion
        #region Int32Dictionary(Int32Dictionary<Int32, TValue>, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32Dictionary{TValue}"/> class that is a
        /// read-only view of the specified instance.</summary>
        /// <param name="dictionary">
        /// The <see cref="Int32Dictionary{TValue}"/> collection that is wrapped by the new
        /// instance.</param>
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

        protected Int32Dictionary(Int32Dictionary<TValue> dictionary, bool readOnly) {
            if (dictionary == null)
                ThrowHelper.ThrowArgumentNullException("dictionary");

            if (!readOnly)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "readOnly", Strings.ArgumentEquals, false);

            _data = dictionary._data;
            ReadOnlyFlag = readOnly;
            ReadOnlyWrapper = this;
        }

        #endregion
        #region Private Fields

        /// <summary>
        /// The <see cref="InstanceData"/> object that holds the <see cref="Int32"/> keys and
        /// <typeparamref name="TValue"/> values of the <see cref="Int32Dictionary{TValue}"/>.
        /// </summary>

        private readonly InstanceData _data;

        /// <summary>Backs the <see cref="Keys"/> property.</summary>

        [NonSerialized]
        private KeyCollection _keys;

        /// <summary>Backs the <see cref="Values"/> property.</summary>

        [NonSerialized]
        private ValueCollection _values;

        #endregion
        #region Protected Fields

        /// <summary>
        /// Backs the <see cref="IsReadOnly"/> property.</summary>

        protected readonly bool ReadOnlyFlag;

        /// <summary>
        /// The read-only <see cref="Int32Dictionary{TValue}"/> collection that is returned by the
        /// <see cref="AsReadOnly"/> method.</summary>

        protected Int32Dictionary<TValue> ReadOnlyWrapper;

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="Int32Dictionary{TValue}"/>.</summary>
        /// <remarks>
        /// Attempting to modify the <b>Empty</b> collection will raise a <see
        /// cref="NotSupportedException"/>. The collection has zero capacity and is guaranteed to
        /// never change, as there are no writable references to the collection.</remarks>

        public static readonly Int32Dictionary<TValue> Empty =
            new Int32Dictionary<TValue>(0).AsReadOnly();

        #endregion
        #region Public Properties
        #region Count

        /// <summary>
        /// Gets the number of key-and-value pairs contained in the <see
        /// cref="Int32Dictionary{TValue}"/>.</summary>
        /// <value>
        /// The number of <see cref="KeyValuePair{Int32, TValue}"/> elements contained in the <see
        /// cref="Int32Dictionary{TValue}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}.Count"/> for details.</remarks>

        public int Count {
            [DebuggerStepThrough]
            get { return (_data.EntryCount - _data.FreeCount); }
        }

        #endregion
        #region IsFixedSize

        /// <summary>
        /// Gets a value indicating whether the <see cref="Int32Dictionary{TValue}"/> has a fixed
        /// size.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="Int32Dictionary{TValue}"/> has a fixed size; otherwise,
        /// <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="IDictionary.IsFixedSize"/> for details.
        /// </para><para>
        /// This property always returns the same value as the <see cref="IsReadOnly"/> property
        /// since any fixed-size <see cref="Int32Dictionary{TValue}"/> is also read-only, and vice
        /// versa.</para></remarks>

        public bool IsFixedSize {
            [DebuggerStepThrough]
            get { return ReadOnlyFlag; }
        }

        #endregion
        #region IsReadOnly

        /// <summary>
        /// Gets a value indicating whether the <see cref="Int32Dictionary{TValue}"/> is read-only.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="Int32Dictionary{TValue}"/> is read-only; otherwise,
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
        /// Gets a value indicating whether access to the <see cref="Int32Dictionary{TValue}"/> is
        /// synchronized (thread-safe).</summary>
        /// <value>
        /// <c>true</c> if access to the <see cref="Int32Dictionary{TValue}"/> is synchronized
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
        /// specified key and value to the <see cref="Int32Dictionary{TValue}"/>.</para></value>
        /// <exception cref="KeyMismatchException">
        /// The property is set to an <see cref="IKeyedValue{Int32}"/> instance whose <see
        /// cref="KeyValuePair{Int32, TValue}.Key"/> differs from the specified <paramref
        /// name="key"/>.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is read, and <paramref name="key"/> does not exist in the <see
        /// cref="Int32Dictionary{TValue}"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="Int32Dictionary{TValue}"/> is read-only.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}.this"/> for details.</remarks>

        public TValue this[Int32 key] {
            get {
                int index = _data.Find(key);
                if (index < 0)
                    ThrowHelper.ThrowKeyNotFoundException("key");

                return _data.Entries[index].Value;
            }
            set {
                if (ReadOnlyFlag)
                    ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);
                if (value is IKeyedValue<Int32>)
                    CollectionsUtility.ValidateKey(key, value);

                _data.Add(key, value, false);
            }
        }

        #endregion
        #region Keys

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="Int32Dictionary{TValue}"/>.
        /// </summary>
        /// <value>
        /// A read-only <see cref="ICollection{Int32}"/> containing the keys in the <see
        /// cref="Int32Dictionary{TValue}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}.Keys"/> for details.</remarks>

        public ICollection<Int32> Keys {
            [DebuggerStepThrough]
            get {
                if (_keys == null)
                    _keys = new KeyCollection(this);

                return _keys;
            }
        }

        #endregion
        #region SyncRoot

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see
        /// cref="Int32Dictionary{TValue}"/>.</summary>
        /// <value>
        /// An object that can be used to synchronize access to the <see
        /// cref="Int32Dictionary{TValue}"/>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="ICollection.SyncRoot"/> for details.
        /// </para><para>
        /// When synchronizing multi-threaded access to the <see cref="Int32Dictionary{TValue}"/>,
        /// obtain a lock on the <b>SyncRoot</b> object rather than the collection itself. A
        /// read-only view always returns the same <b>SyncRoot</b> object as the underlying writable
        /// collection.</para></remarks>

        public object SyncRoot {
            [DebuggerStepThrough]
            get { return _data; }
        }

        #endregion
        #region Values

        /// <summary>
        /// Gets a collection containing the values in the <see cref="Int32Dictionary{TValue}"/>.
        /// </summary>
        /// <value>
        /// A read-only <see cref="ICollection{TValue}"/> containing the values in the <see
        /// cref="Int32Dictionary{TValue}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}.Values"/> for details.</remarks>

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
        /// size of the <see cref="Int32Dictionary{TValue}"/>.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination for elements copied from
        /// the <see cref="Int32Dictionary{TValue}"/>. The <b>Array</b> must have zero-based
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
        /// The number of elements in the <see cref="Int32Dictionary{TValue}"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.</para></exception>

        protected void CheckTargetArray(Array array, int arrayIndex) {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException("array");
            if (array.Rank > 1)
                ThrowHelper.ThrowArgumentException("array", Strings.ArgumentMultidimensional);

            // skip length checks for empty collection and index zero
            if (arrayIndex == 0 && Count == 0) return;

            if (arrayIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "arrayIndex", arrayIndex, Strings.ArgumentNegative);

            if (arrayIndex >= array.Length)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "arrayIndex", arrayIndex, Strings.ArgumentNotLessValue, array.Length);

            if (Count > array.Length - arrayIndex)
                ThrowHelper.ThrowArgumentException("array", Strings.ArgumentSectionLessCollection);
        }

        #endregion
        #endregion
        #region Public Methods
        #region Add(TKey, TValue)

        /// <overloads>
        /// Adds the specified element to the <see cref="Int32Dictionary{TValue}"/>.</overloads>
        /// <summary>
        /// Adds the specified key and value to the <see cref="Int32Dictionary{TValue}"/>.</summary>
        /// <param name="key">
        /// The key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="key"/> already exists in the <see cref="Int32Dictionary{TValue}"/>.
        /// </exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="value"/> is an <see cref="IKeyedValue{Int32}"/> instance whose <see
        /// cref="IKeyedValue{Int32}.Key"/> differs from <paramref name="key"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Int32Dictionary{TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}.Add(TKey, TValue)"/> for details.
        /// </remarks>

        public void Add(int key, TValue value) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);
            if (value is IKeyedValue<Int32>)
                CollectionsUtility.ValidateKey(key, value);

            _data.Add(key, value, true);
        }

        #endregion
        #region Add(KeyValuePair<TKey, TValue>)

        /// <summary>
        /// Adds the specified key-and-value pair to the <see cref="Int32Dictionary{TValue}"/>.
        /// </summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{Int32, TValue}"/> element to add.</param>
        /// <exception cref="ArgumentException">
        /// The <see cref="KeyValuePair{Int32, TValue}.Key"/> component of <paramref name="pair"/>
        /// already exists in the <see cref="Int32Dictionary{TValue}"/>.</exception>
        /// <exception cref="KeyMismatchException">
        /// The <see cref="KeyValuePair{Int32, TValue}.Value"/> component of <paramref name="pair"/>
        /// is an <see cref="IKeyedValue{Int32}"/> instance whose <see
        /// cref="IKeyedValue{Int32}.Key"/> differs from the associated <see
        /// cref="KeyValuePair{Int32, TValue}.Key"/> component.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Int32Dictionary{TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Add"/> for details.</remarks>

        public void Add(KeyValuePair<Int32, TValue> pair) {
            Add(pair.Key, pair.Value);
        }

        #endregion
        #region AddRange

        /// <summary>
        /// Adds the elements of the specified collection to the <see
        /// cref="Int32Dictionary{TValue}"/>.</summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{Int32, TValue}"/> whose elements to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dictionary"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="dictionary"/> contains an element whose <see cref="KeyValuePair{Int32,
        /// TValue}.Value"/> is an <see cref="IKeyedValue{Int32}"/> instance whose <see
        /// cref="IKeyedValue{Int32}.Key"/> differs from the associated <see
        /// cref="KeyValuePair{Int32, TValue}.Key"/>.</exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="Int32Dictionary{TValue}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="Int32Dictionary{TValue}"/> already contains one or more keys in the
        /// specified <paramref name="dictionary"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="dictionary"/> contains one or more duplicate keys.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.AddRange"/> for details.</remarks>

        public void AddRange(IDictionary<Int32, TValue> dictionary) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);
            if (dictionary == null)
                ThrowHelper.ThrowArgumentNullException("dictionary");

            foreach (var pair in dictionary)
                Add(pair.Key, pair.Value);
        }

        #endregion
        #region AsReadOnly

        /// <summary>
        /// Returns a read-only view of the <see cref="Int32Dictionary{TValue}"/>.</summary>
        /// <returns>
        /// A read-only wrapper around the <see cref="Int32Dictionary{TValue}"/>.</returns>
        /// <remarks><para>
        /// Attempting to modify the read-only wrapper returned by <b>AsReadOnly</b> will raise a
        /// <see cref="NotSupportedException"/>. Note that the original collection may still change,
        /// and any such changes will be reflected in the read-only view.
        /// </para><para>
        /// <b>AsReadOnly</b> buffers the newly created read-only wrapper when the method is first
        /// called, and returns the buffered value on subsequent calls.</para></remarks>

        public Int32Dictionary<TValue> AsReadOnly() {

            if (ReadOnlyWrapper == null)
                ReadOnlyWrapper = new Int32Dictionary<TValue>(this, true);

            return ReadOnlyWrapper;
        }

        #endregion
        #region Clear

        /// <summary>
        /// Removes all elements from the <see cref="Int32Dictionary{TValue}"/>.</summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Int32Dictionary{TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}.Clear"/> for details.</remarks>

        public void Clear() {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            _data.Clear();
        }

        #endregion
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="Int32Dictionary{TValue}"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="Int32Dictionary{TValue}"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> does not preserve the enumeration order of the <see
        /// cref="Int32Dictionary{TValue}"/>, nor the values of the <see cref="IsFixedSize"/> and
        /// <see cref="IsReadOnly"/> properties.</remarks>

        public virtual object Clone() {
            return new Int32Dictionary<TValue>(this);
        }

        #endregion
        #region Contains

        /// <summary>
        /// Determines whether the <see cref="Int32Dictionary{TValue}"/> contains the specified
        /// key-and-value pair.</summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{Int32, TValue}"/> element to locate.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="pair"/> is found in the <see
        /// cref="Int32Dictionary{TValue}"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Contains"/> for details.</remarks>

        public bool Contains(KeyValuePair<Int32, TValue> pair) {
            int index = _data.Find(pair.Key);
            return (index >= 0 && ComparerCache<TValue>.
                EqualityComparer.Equals(pair.Value, _data.Entries[index].Value));
        }

        #endregion
        #region ContainsKey

        /// <summary>
        /// Determines whether the <see cref="Int32Dictionary{TValue}"/> contains the specified
        /// key.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Int32Dictionary{TValue}"/> contains an element with the
        /// specified <paramref name="key"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}.ContainsKey"/> for details.
        /// </remarks>

        public bool ContainsKey(int key) {
            return (_data.Find(key) >= 0);
        }

        #endregion
        #region ContainsValue

        /// <summary>
        /// Determines whether the <see cref="Int32Dictionary{TValue}"/> contains the specified
        /// value.</summary>
        /// <param name="value">
        /// The value to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Int32Dictionary{TValue}"/> contains an element with the
        /// specified <paramref name="value"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}.ContainsValue"/> for details.
        /// </remarks>

        public bool ContainsValue(TValue value) {
            var comparer = ComparerCache<TValue>.EqualityComparer;

            for (int i = 0; i < _data.EntryCount; i++)
                if (_data.Entries[i].IsValid && comparer.Equals(value, _data.Entries[i].Value))
                    return true;

            return false;
        }

        #endregion
        #region Copy

        /// <summary>
        /// Creates a deep copy of the <see cref="Int32Dictionary{TValue}"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="Int32Dictionary{TValue}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="TValue"/> does not implement <see cref="ICloneable"/>.</exception>
        /// <remarks><para>
        /// <b>Copy</b> is similar to <see cref="Clone"/> but creates a deep copy the <see
        /// cref="Int32Dictionary{TValue}"/> by invoking <see cref="ICloneable.Clone"/> on all 
        /// <typeparamref name="TValue"/> values.
        /// </para><para>
        /// <b>Copy</b> does not preserve the enumeration order of the <see
        /// cref="Int32Dictionary{TValue}"/>, nor the values of the <see cref="IsFixedSize"/> and
        /// <see cref="IsReadOnly"/> properties.</para></remarks>

        public Int32Dictionary<TValue> Copy() {
            var copy = new Int32Dictionary<TValue>(Count);

            foreach (var pair in this) {
                TValue value = pair.Value;

                ICloneable cloneable = (ICloneable) value;
                if (cloneable != null)
                    value = (TValue) cloneable.Clone();

                copy._data.Add(pair.Key, value, true);
            }

            return copy;
        }

        #endregion
        #region CopyTo

        /// <summary>
        /// Copies the entire <see cref="Int32Dictionary{TValue}"/> to a one-dimensional <see
        /// cref="Array"/>, starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the <see
        /// cref="KeyValuePair{Int32, TValue}"/> elements copied from the <see
        /// cref="Int32Dictionary{TValue}"/>. The <b>Array</b> must have zero-based indexing.
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
        /// The number of elements in the source <see cref="Int32Dictionary{TValue}"/> is greater
        /// than the available space from <paramref name="arrayIndex"/> to the end of the
        /// destination <paramref name="array"/>.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.CopyTo(T[], Int32)"/> for details.</remarks>

        public void CopyTo(KeyValuePair<Int32, TValue>[] array, int arrayIndex) {
            CheckTargetArray(array, arrayIndex);
            foreach (var pair in this)
                array[arrayIndex++] = pair;
        }

        /// <summary>
        /// Copies the entire <see cref="Int32Dictionary{TValue}"/> to a one-dimensional <see
        /// cref="Array"/>, starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the <see
        /// cref="KeyValuePair{Int32, TValue}"/> elements copied from the <see
        /// cref="Int32Dictionary{TValue}"/>. The <b>Array</b> must have zero-based indexing.
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
        /// The number of elements in the source <see cref="Int32Dictionary{TValue}"/> is greater
        /// than the available space from <paramref name="arrayIndex"/> to the end of the
        /// destination <paramref name="array"/>.</para></exception>
        /// <exception cref="InvalidCastException">
        /// <see cref="KeyValuePair{Int32, TValue}"/> cannot be cast automatically to the type of
        /// the destination <paramref name="array"/>.</exception>

        void ICollection.CopyTo(Array array, int arrayIndex) {
            CopyTo((KeyValuePair<Int32, TValue>[]) array, arrayIndex);
        }

        #endregion
        #region Equals

        /// <summary>
        /// Determines whether the specified collection contains the same key-and-value pairs as the
        /// current <see cref="Int32Dictionary{TValue}"/>.</summary>
        /// <param name="collection">
        /// The <see cref="ICollection{T}"/> of <see cref="KeyValuePair{Int32, TValue}"/> elements
        /// to compare with the current <see cref="Int32Dictionary{TValue}"/>.</param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="collection"/> is another reference to this <see
        /// cref="Int32Dictionary{TValue}"/>.
        /// </item><item>
        /// <paramref name="collection"/> is not a null reference, contains the same number of
        /// elements as this <see cref="Int32Dictionary{TValue}"/>, and each element compares as
        /// equal to the element with the same <see cref="KeyValuePair{Int32, TValue}.Key"/>.
        /// </item></list></returns>
        /// <remarks><para>
        /// <b>Equals</b> iterates over the specified <paramref name="collection"/> and calls <see
        /// cref="Contains"/> for each element to test the two collections for value equality.
        /// </para><para>
        /// <b>Equals</b> does not attempt to compare the enumeration order of both collections as
        /// the <see cref="Int32Dictionary{TValue}"/> class does not define a fixed enumeration
        /// order.</para></remarks>

        public bool Equals(ICollection<KeyValuePair<Int32, TValue>> collection) {

            if (collection == this) return true;
            if (collection == null || collection.Count != Count)
                return false;

            foreach (var pair in collection)
                if (!Contains(pair)) return false;

            return true;
        }

        #endregion
        #region GetAny

        /// <summary>
        /// Returns an arbitrary key-and-value pair in the <see cref="Int32Dictionary{TValue}"/>.
        /// </summary>
        /// <returns>
        /// An arbitrary <see cref="KeyValuePair{Int32, TValue}"/> element found in the <see
        /// cref="Int32Dictionary{TValue}"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Int32Dictionary{TValue}"/> is empty.</exception>
        /// <remarks>
        /// <b>GetAny</b> returns the first key-and-value pair found while traversing the <see
        /// cref="Int32Dictionary{TValue}"/> in enumeration order, but without actually creating an
        /// <see cref="IEnumerator{T}"/> instance.</remarks>

        public KeyValuePair<Int32, TValue> GetAny() {

            for (int i = 0; i < _data.EntryCount; i++)
                if (_data.Entries[i].IsValid)
                    return new KeyValuePair<Int32, TValue>(
                        _data.Entries[i].Key, _data.Entries[i].Value);

            ThrowHelper.ThrowInvalidOperationException(Strings.CollectionEmpty);
            return new KeyValuePair<Int32, TValue>();
        }

        #endregion
        #region GetAnyKey

        /// <summary>
        /// Returns an arbitrary key in the <see cref="Int32Dictionary{TValue}"/>.</summary>
        /// <returns>
        /// An arbitrary <see cref="Int32"/> key found in the <see cref="Int32Dictionary{TValue}"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Int32Dictionary{TValue}"/> is empty.</exception>
        /// <remarks>
        /// <b>GetAnyKey</b> returns the first key found while traversing the <see
        /// cref="Int32Dictionary{TValue}"/> in enumeration order, but without actually creating an
        /// <see cref="IEnumerator{T}"/> instance.</remarks>

        public Int32 GetAnyKey() {

            for (int i = 0; i < _data.EntryCount; i++)
                if (_data.Entries[i].IsValid)
                    return _data.Entries[i].Key;

            ThrowHelper.ThrowInvalidOperationException(Strings.CollectionEmpty);
            return 0;
        }

        #endregion
        #region GetAnyValue

        /// <summary>
        /// Returns an arbitrary value in the <see cref="Int32Dictionary{TValue}"/>.</summary>
        /// <returns>
        /// An arbitrary <typeparamref name="TValue"/> value found in the <see
        /// cref="Int32Dictionary{TValue}"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Int32Dictionary{TValue}"/> is empty.</exception>
        /// <remarks>
        /// <b>GetAnyValue</b> returns the first value found while traversing the <see
        /// cref="Int32Dictionary{TValue}"/> in enumeration order, but without actually creating an
        /// <see cref="IEnumerator{T}"/> instance.</remarks>

        public TValue GetAnyValue() {

            for (int i = 0; i < _data.EntryCount; i++)
                if (_data.Entries[i].IsValid)
                    return _data.Entries[i].Value;

            ThrowHelper.ThrowInvalidOperationException(Strings.CollectionEmpty);
            return default(TValue);
        }

        #endregion
        #region GetEnumerator

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that can iterate through the <see
        /// cref="Int32Dictionary{TValue}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> for the entire <see cref="Int32Dictionary{TValue}"/>.
        /// Each enumerated item is a <see cref="KeyValuePair{Int32, TValue}"/>.</returns>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}.GetEnumerator"/> for details.
        /// </remarks>

        public IEnumerator<KeyValuePair<Int32, TValue>> GetEnumerator() {
            for (int i = 0; i < _data.EntryCount; i++)
                if (_data.Entries[i].IsValid)
                    yield return new KeyValuePair<Int32, TValue>(
                        _data.Entries[i].Key, _data.Entries[i].Value);
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that can iterate through the <see
        /// cref="Int32Dictionary{TValue}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> for the entire <see cref="Int32Dictionary{TValue}"/>. Each
        /// enumerated item is a <see cref="KeyValuePair{Int32, TValue}"/>.</returns>

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
        #region Remove(Int32)

        /// <overloads>
        /// Removes the specified element from the <see cref="Int32Dictionary{TValue}"/>.
        /// </overloads>
        /// <summary>
        /// Removes the element with the specified key from the <see
        /// cref="Int32Dictionary{TValue}"/>.</summary>
        /// <param name="key">
        /// The key of the element to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="key"/> was found and the associated element was removed;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Int32Dictionary{TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Dictionary{TKey, TValue}.Remove"/> for details.</remarks>

        public bool Remove(int key) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            return _data.Remove(key);
        }

        #endregion
        #region Remove(KeyValuePair<Int32, TValue>)

        /// <summary>
        /// Removes the specified key-and-value pair from the <see cref="Int32Dictionary{TValue}"/>.
        /// </summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{Int32, TValue}"/> element to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="pair"/> was found and removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="KeyMismatchException">
        /// The <see cref="KeyValuePair{Int32, TValue}.Value"/> component of <paramref name="pair"/>
        /// is an <see cref="IKeyedValue{Int32}"/> instance whose <see
        /// cref="IKeyedValue{Int32}.Key"/> differs from the associated <see
        /// cref="KeyValuePair{Int32, TValue}.Key"/> component.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Int32Dictionary{TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Remove"/> for details.</remarks>

        public bool Remove(KeyValuePair<Int32, TValue> pair) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);
            if (pair.Value is IKeyedValue<Int32>)
                CollectionsUtility.ValidateKey(pair.Key, pair.Value);

            if (!Contains(pair)) return false;
            _data.Remove(pair.Key);
            return true;
        }

        #endregion
        #region ToArray

        /// <summary>
        /// Copies the key-and-value pairs of the <see cref="Int32Dictionary{TValue}"/> to a new
        /// <see cref="Array"/>.</summary>
        /// <returns>
        /// A one-dimensional <see cref="Array"/> containing copies of the <see
        /// cref="KeyValuePair{Int32, TValue}"/> elements of the <see
        /// cref="Int32Dictionary{TValue}"/>.</returns>
        /// <remarks>
        /// <b>ToArray</b> has the same effect as <see cref="CopyTo"/> with a starting index of
        /// zero, but also allocates the target array.</remarks>

        public KeyValuePair<Int32, TValue>[] ToArray() {
            var array = new KeyValuePair<Int32, TValue>[Count];

            int i = 0;
            foreach (var pair in this)
                array[i++] = pair;

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
        /// Please refer to <see cref="Dictionary{TKey, TValue}.TryGetValue"/> for details.
        /// </remarks>

        public bool TryGetValue(int key, out TValue value) {
            int index = _data.Find(key);

            if (index >= 0) {
                value = _data.Entries[index].Value;
                return true;
            } else {
                value = default(TValue);
                return false;
            }
        }

        #endregion
        #endregion
        #region Class KeyCollection

        private class KeyCollection: ICollection<Int32>, ICollection {

            private readonly Int32Dictionary<TValue> _dictionary;

            internal KeyCollection(Int32Dictionary<TValue> dictionary) {
                _dictionary = dictionary;
            }

            public int Count {
                [DebuggerStepThrough]
                get { return _dictionary.Count; }
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
                get { return _dictionary.SyncRoot; }
            }

            public void Add(int key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public void Clear() {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public bool Contains(int key) {
                return _dictionary.ContainsKey(key);
            }

            public void CopyTo(int[] array, int arrayIndex) {
                _dictionary.CheckTargetArray(array, arrayIndex);
                foreach (int key in this)
                    array[arrayIndex++] = key;
            }

            void ICollection.CopyTo(Array array, int arrayIndex) {
                CopyTo((int[]) array, arrayIndex);
            }

            public IEnumerator<Int32> GetEnumerator() {
                for (int i = 0; i < _dictionary._data.EntryCount; i++)
                    if (_dictionary._data.Entries[i].IsValid)
                        yield return _dictionary._data.Entries[i].Key;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public bool Remove(int key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }
        }

        #endregion
        #region Class ValueCollection

        private class ValueCollection: ICollection<TValue>, ICollection {

            private readonly Int32Dictionary<TValue> _dictionary;

            internal ValueCollection(Int32Dictionary<TValue> dictionary) {
                _dictionary = dictionary;
            }

            public int Count {
                [DebuggerStepThrough]
                get { return _dictionary.Count; }
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
                get { return _dictionary.SyncRoot; }
            }

            public void Add(TValue value) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public void Clear() {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public bool Contains(TValue value) {
                return _dictionary.ContainsValue(value);
            }

            public void CopyTo(TValue[] array, int arrayIndex) {
                _dictionary.CheckTargetArray(array, arrayIndex);
                foreach (TValue value in this)
                    array[arrayIndex++] = value;
            }

            void ICollection.CopyTo(Array array, int arrayIndex) {
                CopyTo((TValue[]) array, arrayIndex);
            }

            public IEnumerator<TValue> GetEnumerator() {
                for (int i = 0; i < _dictionary._data.EntryCount; i++) {
                    if (_dictionary._data.Entries[i].IsValid)
                        yield return _dictionary._data.Entries[i].Value;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public bool Remove(TValue value) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }
        }

        #endregion
        #region Struct Entry

        /// <summary>
        /// Represents one key-and-value pair in the <see cref="Int32Dictionary{TValue}"/>.
        /// </summary>
        /// <remarks>
        /// <b>Entry</b> is a simple data container for the key and value of one element in the <see
        /// cref="Int32Dictionary{TValue}"/>, along with auxiliary data for storage management.
        /// </remarks>

        [Serializable, StructLayout(LayoutKind.Auto)]
        private struct Entry {
            #region IsValid

            /// <summary>
            /// <c>true</c> if the <see cref="Entry"/> contains an existing key-and-value pair;
            /// <c>false</c> if the <see cref="Entry"/> is available for storing a new key-and-value
            /// pair. The default is <c>false</c>.</summary>>

            internal bool IsValid;

            #endregion
            #region Next

            /// <summary>
            /// The index of the next <see cref="Entry"/> in a linked list within the <see
            /// cref="InstanceData.Entries"/> collection, or -1 to indicate the end of the list.
            /// </summary>>
            /// <remarks>
            /// Chains of <b>Next</b> indices connect all <see cref="Entry"/> instances whose <see
            /// cref="Key"/> produces the same hash code, and likewise all unoccupied <see
            /// cref="Entry"/> instances within <see cref="InstanceData.EntryCount"/>.</remarks>

            internal int Next;

            #endregion
            #region Key

            /// <summary>
            /// The key of the key-and-value pair stored in the <see cref="Entry"/>.</summary>>
            /// <remarks>
            /// <b>Key</b> returns zero if <see cref="IsValid"/> is <c>false</c>.</remarks>

            internal int Key;

            #endregion
            #region Value

            /// <summary>
            /// The value of the key-and-value pair stored in the <see cref="Entry"/>.</summary>>
            /// <remarks>
            /// <b>Value</b> returns the default value for <typeparamref name="TValue"/> if <see
            /// cref="IsValid"/> is <c>false</c>.</remarks>

            internal TValue Value;

            #endregion
        }

        #endregion
        #region Class InstanceData

        /// <summary>
        /// Contains the key-and-value pairs of the <see cref="Int32Dictionary{TValue}"/>.</summary>
        /// <remarks>
        /// <b>InstanceData</b> provides and maintains the actual hashtable used by the <see
        /// cref="Int32Dictionary{TValue}"/> class.  When a read-only view is created, it shares the
        /// <b>InstanceData</b> of the original <see cref="Int32Dictionary{TValue}"/>. This allows
        /// the read-only view to reflect all changes to the original instance.</remarks>

        [Serializable]
        private class InstanceData {
            #region InstanceData(Int32)

            /// <summary>
            /// Initializes a new instance of the <see cref="InstanceData"/> class.</summary>
            /// <param name="capacity">
            /// The number of elements that the new <see cref="InstanceData"/> is initially capable
            /// of storing.</param>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="capacity"/> is less than zero.</exception>

            internal InstanceData(int capacity) {
                if (capacity < 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(
                        "capacity", capacity, Strings.ArgumentNegative);

                int prime = GetPrime(capacity);
                Buckets = new int[prime];
                for (int i = 0; i < Buckets.Length; i++)
                    Buckets[i] = -1;

                Entries = new Entry[prime];
                FreeList = -1;
            }

            #endregion
            #region SmallPrimes

            /// <summary>
            /// A list of small prime numbers chosen for optimal performance.</summary>
            /// <remarks>
            /// <b>SmallPrimes</b> contains the subset of prime numbers that was chosen by the BCL
            /// team for optimal reallaction performance of the standard <see cref="Dictionary{TKey,
            /// TValue}"/> class.</remarks>

            private static readonly int[] SmallPrimes = new int[]
            { 
                3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431,
                521, 631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839,
                7013, 8419, 10103, 12143, 14591, 17519, 21023, 25229, 30293, 36353, 43627, 52361,
                62851, 75431, 90523, 108631, 130363, 156437, 187751, 225307, 270371, 324449, 389357,
                467237, 560689, 672827, 807403, 968897, 1162687, 1395263, 1674319, 2009191, 2411033,
                2893249, 3471899, 4166287, 4999559, 5999471, 7199369
            };

            #endregion
            #region Buckets

            /// <summary>
            /// An <see cref="Array"/> containing the starting indices of linked lists within the
            /// <see cref="Entries"/> collection. The hash code of all elements within each linked
            /// list equals the corresponding <b>Buckets</b> index.</summary>>
            /// <remarks>
            /// Any <b>Buckets</b> element that has no matching <see cref="Entries"/> elements has
            /// the value -1.</remarks>

            private int[] Buckets;

            #endregion
            #region Entries

            /// <summary>
            /// An <see cref="Array"/> of <see cref="Entry"/> instances that contains all existing
            /// key-and-value pairs and/or storage for new key-and-value pairs.</summary>>
            /// <remarks>
            /// Any <b>Entries</b> element that is available for a new key-and-value pair has an
            /// <see cref="Entry.IsValid"/> flag which is <c>false</c>.</remarks>

            internal Entry[] Entries;

            #endregion
            #region EntryCount

            /// <summary>
            /// The number of <see cref="Entries"/> elements, starting from the first element, that
            /// may be currently occupied. The default is zero.</summary>>
            /// <remarks>
            /// <see cref="Entries"/> indices equal to or greater than <b>EntryCount</b> are
            /// guaranteed to be unoccupied. Indices less than <b>EntryCount</b> may also be free,
            /// in which case <see cref="FreeList"/> returns a valid index.</remarks>

            internal int EntryCount;

            #endregion
            #region FreeCount

            /// <summary>
            /// The number of <see cref="Entries"/> elements within <see cref="EntryCount"/> that
            /// are currently unoccupied. The default is zero.</summary>>

            internal int FreeCount;

            #endregion
            #region FreeList

            /// <summary>
            /// The index of the first <see cref="Entries"/> element within <see cref="EntryCount"/>
            /// that is currently unoccupied. The default is -1.</summary>
            /// <remarks>
            /// <b>FreeList</b> returns -1 when there are no free <see cref="Entries"/> within <see
            /// cref="EntryCount"/>. Otherwise, all unoccupied <see cref="Entries"/> starting at
            /// <b>FreeList</b> form a linked list.</remarks>

            private int FreeList;

            #endregion
            #region Add

            /// <summary>
            /// Adds the specified key-and-value pair to the <see cref="InstanceData"/>.</summary>
            /// <param name="key">
            /// The key of the element to add.</param>
            /// <param name="value">
            /// The value of the element to add.</param>
            /// <param name="adding">
            /// <c>true</c> to only allow adding a new element to the <see cref="InstanceData"/>;
            /// <c>false</c> to assign the specified <paramref name="value"/> to an existing element
            /// with the specified <paramref name="key"/>.</param>
            /// <exception cref="ArgumentException">
            /// <paramref name="adding"/> is <c>true</c>, and <paramref name="key"/> already exists
            /// in the <see cref="InstanceData"/>. </exception>

            internal void Add(int key, TValue value, bool adding) {

                int hashCode = key & 0x7fffffff;
                int bucket = hashCode % Buckets.Length;

                for (int i = Buckets[bucket]; i >= 0; i = Entries[i].Next) {
                    Debug.Assert(Entries[i].IsValid);
                    if (key == Entries[i].Key) {
                        if (adding)
                            ThrowHelper.ThrowArgumentException("key", Strings.ArgumentInCollection);

                        Entries[i].Value = value;
                        return;
                    }
                }

                int index;
                if (FreeCount > 0) {
                    index = FreeList;
                    FreeList = Entries[index].Next;
                    --FreeCount;
                } else {
                    if (EntryCount == Entries.Length) {
                        Resize();
                        bucket = hashCode % Buckets.Length;
                    }

                    index = EntryCount;
                    ++EntryCount;
                }

                Debug.Assert(!Entries[index].IsValid);

                Entries[index].IsValid = true;
                Entries[index].Next = Buckets[bucket];
                Entries[index].Key = key;
                Entries[index].Value = value;

                Buckets[bucket] = index;
            }

            #endregion
            #region Clear

            /// <summary>
            /// Removes all elements from the <see cref="InstanceData"/>.</summary>

            internal void Clear() {
                if (EntryCount == 0) return;

                for (int i = 0; i < Buckets.Length; i++)
                    Buckets[i] = -1;

                Array.Clear(Entries, 0, EntryCount);
                FreeList = -1;
                EntryCount = FreeCount = 0;
            }

            #endregion
            #region Find

            /// <summary>
            /// Finds the specified key within the <see cref="InstanceData"/>.</summary>
            /// <param name="key">
            /// The key to locate.</param>
            /// <returns>
            /// The index of the <see cref="Entries"/> element whose <see cref="Entry.Key"/> equals
            /// <paramref name="key"/>, if found; otherwise, -1.</returns>

            internal int Find(int key) {
                if (Buckets == null) return -1;

                int hashCode = key & 0x7fffffff;
                int bucket = hashCode % Buckets.Length;

                for (int i = Buckets[bucket]; i >= 0; i = Entries[i].Next) {
                    Debug.Assert(Entries[i].IsValid);
                    if (key == Entries[i].Key) return i;
                }

                return -1;
            }

            #endregion
            #region GetPrime

            /// <summary>
            /// Gets the smallest prime number that is equal to or greater than the specified
            /// value.</summary>
            /// <param name="min">
            /// The lower threshold for the returned prime number.</param>
            /// <returns>
            /// The smallest prime number that is equal to or greater than <paramref name="min"/>. 
            /// </returns>
            /// <remarks>
            /// <b>GetPrime</b> first chooses among the preselected <see cref="SmallPrimes"/>, and
            /// computes a new prime only when <paramref name="min"/> is greater than any <see
            /// cref="SmallPrimes"/> element.</remarks>

            private static int GetPrime(int min) {

                foreach (int prime in SmallPrimes)
                    if (prime >= min) return prime;

                for (int i = min | 1; i < 0x7fffffff; i += 2)
                    if (MathUtility.IsPrime((uint) i))
                        return i;

                return min;
            }

            #endregion
            #region Remove

            /// <summary>
            /// Removes the element with the specified key from the <see cref="InstanceData"/>.
            /// </summary>
            /// <param name="key">
            /// The key of the element to remove.</param>
            /// <returns>
            /// <c>true</c> if <paramref name="key"/> was found and the associated element was
            /// removed; otherwise, <c>false</c>.</returns>

            internal bool Remove(int key) {
                if (Buckets == null) return false;

                int hashCode = key & 0x7fffffff;    
                int bucket = hashCode % Buckets.Length, previous = -1;

                for (int i = Buckets[bucket]; i >= 0; i = Entries[i].Next) {
                    Debug.Assert(Entries[i].IsValid);

                    if (key == Entries[i].Key) {
                        if (previous < 0)
                            Buckets[bucket] = Entries[i].Next;
                        else
                            Entries[previous].Next = Entries[i].Next;

                        Entries[i].IsValid = false;
                        Entries[i].Next = FreeList;
                        Entries[i].Key = 0;
                        Entries[i].Value = default(TValue);

                        FreeList = i;
                        ++FreeCount;
                        return true;
                    }

                    previous = i;
                }

                return false;
            }

            #endregion
            #region Resize

            /// <summary>
            /// Resizes the <see cref="InstanceData"/> to accommodate twice the current <see
            /// cref="EntryCount"/>.</summary>
            /// <remarks>
            /// <b>Resize</b> is only called when the <see cref="FreeList"/> is empty.</remarks>

            private void Resize() {
                int prime = GetPrime(EntryCount * 2);

                int[] buckets = new int[prime];
                for (int i = 0; i < buckets.Length; i++)
                    buckets[i] = -1;

                Entry[] entries = new Entry[prime];
                Array.Copy(Entries, 0, entries, 0, EntryCount);

                for (int i = 0; i < EntryCount; i++) {
                    Debug.Assert(entries[i].IsValid);
                    int bucket = (entries[i].Key & 0x7fffffff) % prime;
                    entries[i].Next = buckets[bucket];
                    buckets[bucket] = i;
                }

                Buckets = buckets;
                Entries = entries;
            }

            #endregion
        }

        #endregion
    }
}
