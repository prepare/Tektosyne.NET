using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides a generic collection of key-and-value pairs that retain their insertion order and
    /// are accessible by index and by key.</summary>
    /// <typeparam name="TKey">
    /// The type of all keys in the collection. Keys cannot be null references.</typeparam>
    /// <typeparam name="TValue">
    /// The type of all values that are associated with the keys. If <typeparamref name="TValue"/>
    /// is a reference type, values may be null references.</typeparam>
    /// <remarks><para>
    /// <b>KeyValueList</b> provides a <see cref="ListEx{T}"/> that contains <see
    /// cref="KeyValuePair{TKey, TValue}"/> elements and implements the <see cref="IDictionary{TKey,
    /// TValue}"/> interface. Several additional methods adopted from the <see
    /// cref="SortedListEx{TKey, TValue}"/> class allow direct access by key and by value.
    /// </para><para>
    /// The collection may contain multiple identical keys. All key access methods return the first
    /// occurrence of the specified key. Access by index is an O(1) operation but access by key or
    /// by value are both O(<em>N</em>) operations, where <em>N</em> is the number of key-and-value
    /// pairs in the collection.
    /// </para><para>
    /// The <see cref="IKeyedValue{TKey}.Key"/> property of any <typeparamref name="TValue"/> that
    /// implements the <see cref="IKeyedValue{TKey}"/> interface is automatically checked against
    /// the associated collection key when a key or value is changed or inserted.
    /// </para><para>
    /// <b>KeyValueList</b> also provides several extra features inherited from <see
    /// cref="ListEx{T}"/>; please see there for details. Note that <see cref="ListEx{T}.IsUnique"/>
    /// checks for unique key-and-value pairs, rather than for unique keys.</para></remarks>

    [Serializable]
    public class KeyValueList<TKey, TValue>:
        ListEx<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue> {
        #region KeyValueList()

        /// <overloads>
        /// Initializes a new instance of the <see cref="KeyValueList{TKey, TValue}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueList{TKey, TValue}"/> class that is
        /// empty and has the default initial capacity.</summary>
        /// <remarks>
        /// Please refer to <see cref="List{T}()"/> for details.</remarks>

        public KeyValueList(): this(false) { }

        #endregion
        #region KeyValueList(Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueList{TKey, TValue}"/> class that
        /// is empty, has the default initial capacity, and optionally ensures that all keys are
        /// unique.</summary>
        /// <param name="isUnique">
        /// The initial value for the <see cref="ListEx{T}.IsUnique"/> property.</param>
        /// <remarks>
        /// Please refer to <see cref="List{T}()"/> for details.</remarks>

        public KeyValueList(bool isUnique): base(isUnique) { }

        #endregion
        #region KeyValueList(IEnumerable<KeyValuePair<TKey, TValue>>)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueList{TKey, TValue}"/> class that
        /// contains elements copied from the specified collection and has sufficient capacity to 
        /// accommodate the number of elements copied.</summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> collection whose <see cref="KeyValuePair{TKey,
        /// TValue}"/> elements are copied to the new collection.</param>
        /// <exception cref="ArgumentNullException"><para>
        /// <paramref name="collection"/> is a null reference.
        /// </para><para>-or-</para><para>
        /// <paramref name="collection"/> contains an element whose <see cref="KeyValuePair{TKey,
        /// TValue}.Key"/> component is a null reference.</para></exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="collection"/> contains an element whose <see cref="KeyValuePair{TKey,
        /// TValue}.Value"/> component is an <see cref="IKeyedValue{TKey}"/> instance, and whose
        /// <see cref="KeyValuePair{TKey, TValue}.Key"/> component differs from that instance’s
        /// <see cref="IKeyedValue{TKey}.Key"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(IEnumerable{T})"/> for details.</remarks>

        public KeyValueList(IEnumerable<KeyValuePair<TKey, TValue>> collection):
            base(collection) {

            foreach (KeyValuePair<TKey, TValue> pair in InnerList)
                CollectionsUtility.ValidateKey(pair.Key, pair.Value);
        }

        #endregion
        #region KeyValueList(Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueList{TKey, TValue}"/> class that is
        /// empty and has the specified initial capacity.</summary>
        /// <param name="capacity">
        /// The number of elements that the new <see cref="KeyValueList{TKey, TValue}"/> is
        /// initially capable of storing.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>

        public KeyValueList(int capacity): this(capacity, false) { }

        #endregion
        #region KeyValueList(Int32, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueList{TKey, TValue}"/> class that
        /// is empty, has the specified initial capacity, and optionally ensures that all keys are
        /// unique.</summary>
        /// <param name="capacity">
        /// The number of elements that the new <see cref="KeyValueList{TKey, TValue}"/> is
        /// initially capable of storing.</param>
        /// <param name="isUnique">
        /// The initial value for the <see cref="ListEx{T}.IsUnique"/> property.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>

        public KeyValueList(int capacity, bool isUnique): base(capacity, isUnique) { }

        #endregion
        #region KeyValueList(KeyValueList<TKey, TValue>)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueList{TKey, TValue}"/> class that
        /// contains elements copied from the specified instance and has sufficient capacity to 
        /// accommodate the number of elements copied.</summary>
        /// <param name="list">
        /// The <see cref="KeyValueList{TKey, TValue}"/> collection whose elements are copied to the
        /// new collection.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(IEnumerable{T})"/> for details. This constructor also
        /// copies the value of the <see cref="ListEx{T}.IsUnique"/> property.</remarks>

        public KeyValueList(KeyValueList<TKey, TValue> list): base(list) { }

        #endregion
        #region KeyValueList(KeyValueList<TKey, TValue>, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueList{TKey, TValue}"/> class that is
        /// a read-only view of the specified instance.</summary>
        /// <param name="list">
        /// The <see cref="KeyValueList{TKey, TValue}"/> collection that provides the initial values
        /// for the <see cref="ListEx{T}.InnerList"/> field and for the <see
        /// cref="ListEx{T}.IsUnique"/> property.</param>
        /// <param name="readOnly">
        /// The initial value for the <see cref="ListEx{T}.IsReadOnly"/> property. This argument
        /// must be <c>true</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="readOnly"/> is <c>false</c>.</exception>
        /// <remarks>
        /// This constructor is used to create a read-only wrapper around an existing collection.
        /// The new instance shares the data of the specified <paramref name="list"/>.</remarks>

        protected KeyValueList(KeyValueList<TKey, TValue> list, bool readOnly):
            base(list, readOnly) { }

        #endregion
        #region Private Fields

        /// <summary>Backs the <see cref="Keys"/> property.</summary>

        [NonSerialized]
        private KeyList _keys;

        /// <summary>Backs the <see cref="Values"/> property.</summary>

        [NonSerialized]
        private ValueList _values;

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="KeyValueList{TKey, TValue}"/>.</summary>
        /// <remarks>
        /// Attempting to modify the <b>Empty</b> collection will raise a <see
        /// cref="NotSupportedException"/>. The collection has zero capacity and is guaranteed to
        /// never change, as there are no writable references to the collection.</remarks>

        public static readonly new KeyValueList<TKey, TValue> Empty =
            new KeyValueList<TKey, TValue>(0).AsReadOnly();

        #endregion
        #region Item[TKey]

        /// <overloads>
        /// Gets or sets the element at the specified index or with the specified key.</overloads>
        /// <summary>
        /// Gets or sets the value associated with the first occurrence of the specified key.
        /// </summary>
        /// <param name="key">
        /// The key whose value to get or set.</param>
        /// <value><para>
        /// The value associated with the first occurrence of the specified <paramref name="key"/>.
        /// </para><para>
        /// If <paramref name="key"/> is not found, attempting to get it throws a <see
        /// cref="KeyNotFoundException"/>, and attempting to set it adds a new element with the
        /// specified key and value to the end of the <see cref="KeyValueList{TKey, TValue}"/>.
        /// </para></value>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// The property is set to an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="KeyValuePair{TKey, TValue}.Key"/> differs from the specified <paramref
        /// name="key"/>.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is read, and <paramref name="key"/> does not exist in the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="KeyValueList{TKey, TValue}"/> is read-only.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.this"/> for details.</remarks>

        public TValue this[TKey key] {
            [DebuggerStepThrough]
            get { return GetByKey(key); }
            [DebuggerStepThrough]
            set { SetByKey(key, value); }
        }

        #endregion
        #region Keys

        /// <summary>
        /// Gets an <see cref="IList{TKey}"/> containing the keys in the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="IList{TKey}"/> containing the keys in the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.Keys"/> for details.</remarks>

        public IList<TKey> Keys {
            [DebuggerStepThrough]
            get {
                if (_keys == null)
                    _keys = new KeyList(this);

                return _keys;
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="KeyValueList{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        /// A read-only <see cref="ICollection{TKey}"/> containing the keys in the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</value>

        ICollection<TKey> IDictionary<TKey, TValue>.Keys {
            [DebuggerStepThrough]
            get { return Keys; }
        }

        #endregion
        #region Values

        /// <summary>
        /// Gets an <see cref="IList{TValue}"/> containing the values in the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</summary>
        /// <value>
        /// A read-only <see cref="IList{TValue}"/> containing the values in the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.Values"/> for details.</remarks>

        public IList<TValue> Values {
            [DebuggerStepThrough]
            get {
                if (_values == null)
                    _values = new ValueList(this);

                return _values;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="KeyValueList{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        /// A read-only <see cref="ICollection{TValue}"/> containing the values in the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</value>

        ICollection<TValue> IDictionary<TKey, TValue>.Values {
            [DebuggerStepThrough]
            get { return Values; }
        }

        #endregion
        #region Private Methods
        #region CheckTargetArray

        /// <summary>
        /// Checks the bounds of the specified array and the specified starting index against the
        /// size of the <see cref="ListEx{T}.InnerList"/>.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination for elements copied from
        /// the <see cref="ListEx{T}.InnerList"/>. The <b>Array</b> must have zero-based indexing.
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
        /// The number of elements in the <see cref="ListEx{T}.InnerList"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.</para></exception>

        private void CheckTargetArray(Array array, int arrayIndex) {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException("array");
            if (array.Rank > 1)
                ThrowHelper.ThrowArgumentException("array", Strings.ArgumentMultidimensional);

            // skip length checks for empty collection and index zero
            if (arrayIndex == 0 && InnerList.Count == 0)
                return;

            if (arrayIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "arrayIndex", arrayIndex, Strings.ArgumentNegative);

            if (arrayIndex >= array.Length)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "arrayIndex", arrayIndex, Strings.ArgumentNotLessValue, array.Length);

            if (InnerList.Count > array.Length - arrayIndex)
                ThrowHelper.ThrowArgumentException("array", Strings.ArgumentSectionLessCollection);
        }

        #endregion
        #endregion
        #region Protected Methods
        #region CheckWritable(KeyValuePair<TKey, TValue>)

        /// <summary>
        /// Checks that the specified key-and-value pair can be added to the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</summary>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{TKey, TValue}"/> element to add.</param>
        /// <exception cref="ArgumentNullException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> component of <paramref name="pair"/> is
        /// a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Value"/> component of <paramref name="pair"/>
        /// is an <see cref="IKeyedValue{TKey}"/> instance whose <see cref="IKeyedValue{TKey}.Key"/>
        /// differs from the associated <see cref="KeyValuePair{TKey, TValue}.Key"/> component.
        /// </exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="KeyValueList{TKey, TValue}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="KeyValueList{TKey, TValue}"/> already contains the <see
        /// cref="KeyValuePair{TKey, TValue}.Key"/> component of <paramref name="pair"/>, and the
        /// <see cref="MultiKeyedList{TKey, TValue}"/> ensures that all keys are unique.
        /// </para></exception>

        protected override void CheckWritable(KeyValuePair<TKey, TValue> pair) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            if (pair.Value is IKeyedValue<TKey>)
                CollectionsUtility.ValidateKey(pair.Key, pair.Value);

            if (UniqueFlag && IndexOfKey(pair.Key) >= 0)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionUnique);
        }

        #endregion
        #region CheckWritable(Int32, KeyValuePair<TKey, TValue>)

        /// <summary>
        /// Checks that the specified key-and-value pair can be inserted into the <see
        /// cref="MultiKeyedList{TKey, TValue}"/> at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index at which to insert <paramref name="pair"/>.</param>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{TKey, TValue}"/> element to add.</param>
        /// <exception cref="ArgumentNullException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> component of <paramref name="pair"/> is
        /// a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Value"/> component of <paramref name="pair"/>
        /// is an <see cref="IKeyedValue{TKey}"/> instance whose <see cref="IKeyedValue{TKey}.Key"/>
        /// differs from the associated <see cref="KeyValuePair{TKey, TValue}.Key"/> component.
        /// </exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="KeyValueList{TKey, TValue}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="KeyValueList{TKey, TValue}"/> already contains the <see
        /// cref="KeyValuePair{TKey, TValue}.Key"/> component of <paramref name="pair"/> at a
        /// different index, and the <see cref="MultiKeyedList{TKey, TValue}"/> ensures that all
        /// keys are unique.</para></exception>
        /// <remarks>
        /// <b>CheckWritable</b> does not test for duplicate keys if the specified <paramref
        /// name="index"/> is less than zero, or equal to or greater than <see
        /// cref="ListEx{T}.Count"/>.</remarks>

        protected override void CheckWritable(int index, KeyValuePair<TKey, TValue> pair) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            if (pair.Value is IKeyedValue<TKey>)
                CollectionsUtility.ValidateKey(pair.Key, pair.Value);

            if (UniqueFlag && index >= 0 && index < Count) {
                int existing = IndexOfKey(pair.Key);
                if (existing >= 0 && existing != index)
                    ThrowHelper.ThrowNotSupportedException(Strings.CollectionUnique);
            }
        }

        #endregion
        #endregion
        #region Public Methods
        #region Add(TKey, TValue)

        /// <overloads>
        /// Adds an element to the end of the <see cref="KeyValueList{TKey, TValue}"/>.</overloads>
        /// <summary>
        /// Adds an element with the specified key and value to the end of the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</summary>
        /// <param name="key">
        /// The key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="IKeyedValue{TKey}.Key"/> differs from <paramref name="key"/>.</exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="KeyValueList{TKey, TValue}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="KeyValueList{TKey, TValue}"/> already contains the specified
        /// key-and-value pair, and the <see cref="KeyValueList{TKey, TValue}"/> ensures that all
        /// elements are unique.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.Add(TKey, TValue)"/> for details.
        /// </remarks>

        public void Add(TKey key, TValue value) {
            if (value is IKeyedValue<TKey>)
                CollectionsUtility.ValidateKey(key, value);
            else if (key == null)
                ThrowHelper.ThrowArgumentNullException("key");

            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        #endregion
        #region AsReadOnly

        /// <summary>
        /// Creates a read-only view of the <see cref="KeyValueList{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A read-only wrapper around the <see cref="KeyValueList{TKey, TValue}"/>.</returns>
        /// <remarks><para>
        /// Attempting to modify the read-only wrapper returned by <b>AsReadOnly</b> will raise a
        /// <see cref="NotSupportedException"/>. Note that the original collection may still change,
        /// and any such changes will be reflected in the read-only view.
        /// </para><para>
        /// <b>AsReadOnly</b> buffers the newly created read-only wrapper when the method is first
        /// called, and returns the buffered value on subsequent calls.</para></remarks>

        public new KeyValueList<TKey, TValue> AsReadOnly() {

            if (ReadOnlyWrapper == null)
                ReadOnlyWrapper = new KeyValueList<TKey, TValue>(this, true);

            return (KeyValueList<TKey, TValue>) ReadOnlyWrapper;
        }

        #endregion
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="KeyValueList{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="KeyValueList{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> preserves the value of the <see cref="ListEx{T}.IsUnique"/> property, but
        /// not the values of the <see cref="ListEx{T}.IsFixedSize"/> and <see
        /// cref="ListEx{T}.IsReadOnly"/> properties.</remarks>

        public override object Clone() {
            return new KeyValueList<TKey, TValue>(this);
        }

        #endregion
        #region ContainsKey

        /// <summary>
        /// Determines whether the <see cref="KeyValueList{TKey, TValue}"/> contains the specified
        /// key.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="KeyValueList{TKey, TValue}"/> contains an element with the
        /// specified <paramref name="key"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.ContainsKey"/> for details.
        /// </remarks>

        public bool ContainsKey(TKey key) {
            return (IndexOfKey(key) >= 0);
        }

        #endregion
        #region ContainsValue

        /// <summary>
        /// Determines whether the <see cref="KeyValueList{TKey, TValue}"/> contains the specified
        /// value.</summary>
        /// <param name="value">
        /// The value to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="KeyValueList{TKey, TValue}"/> contains an element with the
        /// specified <paramref name="value"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.ContainsValue"/> for details.
        /// </remarks>

        public bool ContainsValue(TValue value) {
            return (IndexOfValue(value) >= 0);
        }

        #endregion
        #region Copy
        
        /// <summary>
        /// Creates a deep copy of the <see cref="KeyValueList{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="KeyValueList{TKey, TValue}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="TValue"/> does not implement <see cref="ICloneable"/>.</exception>
        /// <remarks><para>
        /// <b>Copy</b> is similar to <see cref="Clone"/> but creates a deep copy the <see
        /// cref="KeyValueList{TKey, TValue}"/> by invoking <see cref="ICloneable.Clone"/> on all
        /// <typeparamref name="TValue"/> values. The <typeparamref name="TKey"/> keys are always
        /// duplicated by a shallow copy.
        /// </para><para>
        /// <b>Copy</b> preserves the value of the <see cref="ListEx{T}.IsUnique"/> property, but
        /// not the values of the <see cref="ListEx{T}.IsFixedSize"/> and <see
        /// cref="ListEx{T}.IsReadOnly"/> properties.</para></remarks>

        public new KeyValueList<TKey, TValue> Copy() {

            int count = InnerList.Count;
            KeyValueList<TKey, TValue> copy = new KeyValueList<TKey, TValue>(count, UniqueFlag);

            for (int i = 0; i < count; i++) {
                KeyValuePair<TKey, TValue> pair = InnerList[i];

                ICloneable cloneable = (ICloneable) pair.Value;
                if (cloneable != null) 
                    pair = new KeyValuePair<TKey, TValue>(pair.Key, (TValue) cloneable.Clone());

                copy.InnerList.Add(pair);
            }

            return copy;
        }
        
        #endregion
        #region GetByIndex

        /// <summary>
        /// Gets the value at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index of the value to get.</param>
        /// <returns>
        /// The value at the specified <paramref name="index"/> in the <see cref="KeyValueList{TKey,
        /// TValue}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="ListEx{T}.Count"/>.
        /// </para></exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList.GetByIndex"/> for details.</remarks>

        public TValue GetByIndex(int index) {
            return InnerList[index].Value;
        }

        #endregion
        #region GetByKey

        /// <summary>
        /// Gets the value associated with the first occurrence of the specified key.</summary>
        /// <param name="key">
        /// The key whose value to get.</param>
        /// <returns>
        /// The value associated with the first occurrence of <paramref name="key"/> in the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyNotFoundException">
        /// <paramref name="key"/> was not found in the <see cref="KeyValueList{TKey, TValue}"/>.
        /// </exception>
        /// <remarks>
        /// <b>GetByKey</b> has the same effect as reading the key indexer, <see
        /// cref="this[TKey]"/>.</remarks>

        public TValue GetByKey(TKey key) {

            int index = IndexOfKey(key);
            if (index < 0)
                ThrowHelper.ThrowKeyNotFoundException(key);

            return InnerList[index].Value;
        }

        #endregion
        #region GetKey

        /// <summary>
        /// Gets the key at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index of the key to get.</param>
        /// <returns>
        /// The key at the specified <paramref name="index"/> in the <see cref="KeyValueList{TKey,
        /// TValue}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="ListEx{T}.Count"/>.
        /// </para></exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList.GetKey"/> for details.</remarks>

        public TKey GetKey(int index) {
            return InnerList[index].Key;
        }

        #endregion
        #region IndexOfKey

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified key.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="key"/> in the <see
        /// cref="KeyValueList{TKey, TValue}"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.IndexOfKey"/> for details.</remarks>

        public int IndexOfKey(TKey key) {
            if (key == null)
                ThrowHelper.ThrowArgumentNullException("key");

            var comparer = ComparerCache<TKey>.EqualityComparer;

            for (int i = 0; i < InnerList.Count; i++)
                if (comparer.Equals(key, InnerList[i].Key))
                    return i;

            return -1;
        }

        #endregion
        #region IndexOfValue

        /// <summary>
        /// Returns the zero-based index of first occurrence of the specified value.</summary>
        /// <param name="value">
        /// The value to locate.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="value"/> in the <see
        /// cref="KeyValueList{TKey, TValue}"/>, if found; otherwise, -1.</returns>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.IndexOfValue"/> for details.
        /// </remarks>

        public int IndexOfValue(TValue value) {
            var comparer = ComparerCache<TValue>.EqualityComparer;

            for (int i = 0; i < InnerList.Count; i++)
                if (comparer.Equals(value, InnerList[i].Value))
                    return i;

            return -1;
        }

        #endregion
        #region Remove(TKey)

        /// <overloads>
        /// Removes the first occurrence of specified element from the <see cref="KeyValueList{TKey,
        /// TValue}"/>.</overloads>
        /// <summary>
        /// Removes the first occurrence of the element with the specified key from the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</summary>
        /// <param name="key">
        /// The key of the element to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="key"/> was found and the first associated element was
        /// removed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="KeyValueList{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.Remove"/> for details.</remarks>

        public bool Remove(TKey key) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            int index = IndexOfKey(key);
            if (index < 0) return false;

            InnerList.RemoveAt(index);
            return true;
        }

        #endregion
        #region SetByIndex

        /// <summary>
        /// Sets the value at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index of the value to set.</param>
        /// <param name="value">
        /// The value to store at the specified <paramref name="index"/> in the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="ListEx{T}.Count"/>.
        /// </para></exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="IKeyedValue{TKey}.Key"/> component differs from the key at the specified <paramref
        /// name="index"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="KeyValueList{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList.SetByIndex"/> for details.</remarks>

        public void SetByIndex(int index, TValue value) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            TKey key = InnerList[index].Key;
            if (value is IKeyedValue<TKey>)
                CollectionsUtility.ValidateKey(key, value);

            InnerList[index] = new KeyValuePair<TKey, TValue>(key, value);
        }

        #endregion
        #region SetByKey

        /// <summary>
        /// Sets the value associated with the first occurrence of the specified key.</summary>
        /// <param name="key">
        /// The key whose value to set.</param>
        /// <param name="value"><para>
        /// The value to associate with the first occurrence of <paramref name="key"/>.
        /// </para><para>
        /// If <paramref name="key"/> is not found, <b>SetByKey</b> adds a new element with the
        /// specified <paramref name="key"/> and <paramref name="value"/> to the end of the <see
        /// cref="KeyValueList{TKey, TValue}"/>.</para></param>
        /// <returns>
        /// The <see cref="KeyValueList{TKey, TValue}"/> index of the element that was changed or
        /// added.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="IKeyedValue{TKey}.Key"/> differs from <paramref name="key"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="KeyValueList{TKey, TValue}"/> is read-only.</exception>
        /// <remarks>
        /// <b>SetByKey</b> has the same effect as setting the key indexer, <see
        /// cref="this[TKey]"/>.</remarks>

        public int SetByKey(TKey key, TValue value) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            if (value is IKeyedValue<TKey>)
                CollectionsUtility.ValidateKey(key, value);
            else if (key == null)
                ThrowHelper.ThrowArgumentNullException("key");

            var pair = new KeyValuePair<TKey, TValue>(key, value);
            int index = IndexOfKey(key);

            if (index >= 0) {
                InnerList[index] = pair;
                return index;
            }
            else {
                InnerList.Add(pair);
                return InnerList.Count - 1;
            }
        }

        #endregion
        #region TryGetValue

        /// <summary>
        /// Gets the value associated with the first occurrence of the specified key.</summary>
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

            int index = IndexOfKey(key);
            if (index >= 0) {
                value = InnerList[index].Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        #endregion
        #endregion
        #region Class KeyList

        private class KeyList: IList<TKey>, IList {

            private readonly KeyValueList<TKey, TValue> _list;

            internal KeyList(KeyValueList<TKey, TValue> list) {
                _list = list;
            }

            public int Count {
                [DebuggerStepThrough]
                get { return _list.Count; }
            }

            public bool IsReadOnly {
                [DebuggerStepThrough]
                get { return true; }
            }

            public bool IsFixedSize {
                [DebuggerStepThrough]
                get { return true; }
            }

            bool ICollection.IsSynchronized {
                [DebuggerStepThrough]
                get { return false; }
            }

            public TKey this[int index] {
                [DebuggerStepThrough]
                get { return _list[index].Key; }
                set { throw new NotSupportedException(Strings.CollectionReadOnly); }
            }

            object IList.this[int index] {
                [DebuggerStepThrough]
                get { return this[index]; }
                set { throw new NotSupportedException(Strings.CollectionReadOnly); }
            }

            public object SyncRoot {
                [DebuggerStepThrough]
                get { return _list.SyncRoot; }
            }

            public void Add(TKey key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            int IList.Add(object key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public void Clear() {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public bool Contains(TKey key) {
                return _list.ContainsKey(key);
            }

            bool IList.Contains(object key) {
                return Contains((TKey) key);
            }

            public void CopyTo(TKey[] array, int arrayIndex) {
                _list.CheckTargetArray(array, arrayIndex);
                foreach (var key in this)
                    array[arrayIndex++] = key;
            }

            void ICollection.CopyTo(Array array, int arrayIndex) {
                CopyTo((TKey[]) array, arrayIndex);
            }

            public IEnumerator<TKey> GetEnumerator() {
                for (int i = 0; i < _list.Count; i++)
                    yield return _list[i].Key;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public int IndexOf(TKey key) {
                return _list.IndexOfKey(key);
            }

            int IList.IndexOf(object key) {
                return IndexOf((TKey) key);
            }

            public void Insert(int index, TKey key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            void IList.Insert(int index, object key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public bool Remove(TKey key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            void IList.Remove(object key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public void RemoveAt(int index) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }
        }

        #endregion
        #region Class ValueList

        private class ValueList: IList<TValue>, IList {

            private readonly KeyValueList<TKey, TValue> _list;

            internal ValueList(KeyValueList<TKey, TValue> list) {
                _list = list;
            }

            public int Count {
                [DebuggerStepThrough]
                get { return _list.Count; }
            }

            public bool IsReadOnly {
                [DebuggerStepThrough]
                get { return true; }
            }

            public bool IsFixedSize {
                [DebuggerStepThrough]
                get { return true; }
            }

            bool ICollection.IsSynchronized {
                [DebuggerStepThrough]
                get { return false; }
            }

            public TValue this[int index] {
                [DebuggerStepThrough]
                get { return _list[index].Value; }
                set { throw new NotSupportedException(Strings.CollectionReadOnly); }
            }

            object IList.this[int index] {
                [DebuggerStepThrough]
                get { return this[index]; }
                set { throw new NotSupportedException(Strings.CollectionReadOnly); }
            }

            public object SyncRoot {
                [DebuggerStepThrough]
                get { return _list.SyncRoot; }
            }

            public void Add(TValue value) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            int IList.Add(object value) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public void Clear() {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public bool Contains(TValue value) {
                return _list.ContainsValue(value);
            }

            bool IList.Contains(object value) {
                return Contains((TValue) value);
            }

            public void CopyTo(TValue[] array, int arrayIndex) {
                _list.CheckTargetArray(array, arrayIndex);
                foreach (var value in this)
                    array[arrayIndex++] = value;
            }

            void ICollection.CopyTo(Array array, int arrayIndex) {
                CopyTo((TValue[]) array, arrayIndex);
            }

            public IEnumerator<TValue> GetEnumerator() {
                for (int i = 0; i < _list.Count; i++)
                    yield return _list[i].Value;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public int IndexOf(TValue value) {
                return _list.IndexOfValue(value);
            }

            int IList.IndexOf(object key) {
                return IndexOf((TValue) key);
            }

            public void Insert(int index, TValue value) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            void IList.Insert(int index, object key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public bool Remove(TValue value) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            void IList.Remove(object key) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

            public void RemoveAt(int index) {
                throw new NotSupportedException(Strings.CollectionReadOnly);
            }

        }

        #endregion
    }
}
