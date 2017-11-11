using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides a generic collection of elements that are accessible by index and by one or more
    /// user-defined keys.</summary>
    /// <typeparam name="TKey">
    /// The type of the all user-defined keys in the collection. Keys cannot be null references.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The type of all elements in the collection. If <typeparamref name="TValue"/> is a reference
    /// type, elements may be null references.</typeparam>
    /// <remarks><para>
    /// <b>MultiKeyedList</b> provides a <see cref="ListEx{T}"/> whose elements must define at least
    /// one embedded key of type <typeparamref name="TKey"/>. Several additional methods adopted
    /// from the <see cref="SortedListEx{TKey, TValue}"/> class allow direct access by key.
    /// </para><para>
    /// The key of a given collection element is extracted by a user-defined <see
    /// cref="MultiKeyedList{TKey, TValue}.KeyConverter"/> method. This method must be specified
    /// during construction but may be changed at any time, allowing clients to use multiple keys
    /// with the same collection. All keys must be of type <typeparamref name="TKey"/>, however.
    /// </para><para>
    /// The collection may contain multiple identical keys, unless <see cref="ListEx{T}.IsUnique"/>
    /// is <c>true</c>. All key access methods return the first occurrence of the specified key.
    /// Access by index is an O(1) operation but access by key is an O(<em>N</em>) operation, where
    /// <em>N</em> is the number of elements in the collection.
    /// </para><para>
    /// <b>MultiKeyedList</b> also provides several extra features inherited from <see
    /// cref="ListEx{T}"/>; please see there for details. Note that <see cref="ListEx{T}.IsUnique"/>
    /// now checks for unique keys, rather than unique key-and-value pairs.</para></remarks>

    [Serializable]
    public class MultiKeyedList<TKey, TValue>: ListEx<TValue> {
        #region MultiKeyedList(Converter<TValue, TKey>)

        /// <overloads>
        /// Initializes a new instance of the <see cref="MultiKeyedList{TKey, TValue}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyedList{TKey, TValue}"/> class that
        /// is empty and has the default initial capacity.</summary>
        /// <param name="keyConverter">
        /// The <see cref="Converter{TValue, TKey}"/> method that converts a <typeparamref
        /// name="TValue"/> element into its associated <typeparamref name="TKey"/> key.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyConverter"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}()"/> for details.</remarks>

        public MultiKeyedList(Converter<TValue, TKey> keyConverter): this(keyConverter, false) { }

        #endregion
        #region MultiKeyedList(Converter<TValue, TKey>, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyedList{TKey, TValue}"/> class that
        /// is empty, has the default initial capacity, and optionally ensures that all keys are
        /// unique.</summary>
        /// <param name="keyConverter">
        /// The <see cref="Converter{TValue, TKey}"/> method that converts a <typeparamref
        /// name="TValue"/> element into its associated <typeparamref name="TKey"/> key.</param>
        /// <param name="isUnique">
        /// The initial value for the <see cref="ListEx{T}.IsUnique"/> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyConverter"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}()"/> for details.</remarks>

        public MultiKeyedList(Converter<TValue, TKey> keyConverter, bool isUnique): base(isUnique) {
            if (keyConverter == null)
                ThrowHelper.ThrowArgumentNullException("keyConverter");

            _keyConverter = keyConverter;
        }

        #endregion
        #region MultiKeyedList(IEnumerable<TValue>, Converter<TValue, TKey>)

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyedList{TKey, TValue}"/> class that
        /// contains elements copied from the specified collection and has sufficient capacity to 
        /// accommodate the number of elements copied.</summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> collection whose <see cref="KeyValuePair{TKey,
        /// TValue}"/> elements are copied to the new collection.</param>
        /// <param name="keyConverter">
        /// The <see cref="Converter{TValue, TKey}"/> method that converts a <typeparamref
        /// name="TValue"/> element into its associated <typeparamref name="TKey"/> key.</param>
        /// <exception cref="ArgumentNullException"><para>
        /// <paramref name="collection"/> or <paramref name="keyConverter"/>is a null reference.
        /// </para><para>-or-</para><para>
        /// <paramref name="collection"/> contains an element whose embedded key is a null
        /// reference.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(IEnumerable{T})"/> for details.</remarks>

        public MultiKeyedList(IEnumerable<TValue> collection,
            Converter<TValue, TKey> keyConverter): base(collection) {

            if (keyConverter == null)
                ThrowHelper.ThrowArgumentNullException("keyConverter");

            _keyConverter = keyConverter;

            foreach (TValue value in collection)
                if (value != null && keyConverter(value) == null)
                    ThrowHelper.ThrowArgumentNullException(
                        "collection", Strings.ArgumentContainsNullKey);
        }

        #endregion
        #region MultiKeyedList(Int32, Converter<TValue, TKey>)

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyedList{TKey, TValue}"/> class that
        /// is empty and has the specified initial capacity.</summary>
        /// <param name="capacity">
        /// The number of elements that the new <see cref="MultiKeyedList{TKey, TValue}"/> is
        /// initially capable of storing.</param>
        /// <param name="keyConverter">
        /// The <see cref="Converter{TValue, TKey}"/> method that converts a <typeparamref
        /// name="TValue"/> element into its associated <typeparamref name="TKey"/> key.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyConverter"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>

        public MultiKeyedList(int capacity, Converter<TValue, TKey> keyConverter):
            this(capacity, keyConverter, false) { }

        #endregion
        #region MultiKeyedList(Int32, Converter<TValue, TKey>, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyedList{TKey, TValue}"/> class that
        /// is empty, has the specified initial capacity, and optionally ensures that all keys are
        /// unique.</summary>
        /// <param name="capacity">
        /// The number of elements that the new <see cref="MultiKeyedList{TKey, TValue}"/> is
        /// initially capable of storing.</param>
        /// <param name="keyConverter">
        /// The <see cref="Converter{TValue, TKey}"/> method that converts a <typeparamref
        /// name="TValue"/> element into its associated <typeparamref name="TKey"/> key.</param>
        /// <param name="isUnique">
        /// The initial value for the <see cref="ListEx{T}.IsUnique"/> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyConverter"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>

        public MultiKeyedList(int capacity, Converter<TValue, TKey> keyConverter, bool isUnique):
            base(capacity, isUnique) {

            if (keyConverter == null)
                ThrowHelper.ThrowArgumentNullException("keyConverter");

            _keyConverter = keyConverter;
        }

        #endregion
        #region MultiKeyedList(MultiKeyedList<TKey, TValue>)

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyedList{TKey, TValue}"/> class that
        /// contains elements copied from the specified instance and has sufficient capacity to 
        /// accommodate the number of elements copied.</summary>
        /// <param name="list">
        /// The <see cref="MultiKeyedList{TKey, TValue}"/> collection whose elements are copied to
        /// the new collection.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(IEnumerable{T})"/> for details. This constructor also
        /// copies the values of the <see cref="ListEx{T}.IsUnique"/> and <see cref="KeyConverter"/>
        /// properties.</remarks>

        public MultiKeyedList(MultiKeyedList<TKey, TValue> list): base(list) {
            _keyConverter = list._keyConverter;
        }

        #endregion
        #region MultiKeyedList(MultiKeyedList<TKey, TValue>, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiKeyedList{TKey, TValue}"/> class that
        /// is a read-only view of the specified instance.</summary>
        /// <param name="list">
        /// The <see cref="MultiKeyedList{TKey, TValue}"/> collection that provides the initial
        /// values for the <see cref="ListEx{T}.InnerList"/> and <see cref="ListEx{T}.IsUnique"/>
        /// fields and for the <see cref="KeyConverter"/> property.</param>
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

        protected MultiKeyedList(MultiKeyedList<TKey, TValue> list, bool readOnly):
            base(list, readOnly) {

            _keyConverter = list._keyConverter;
        }

        #endregion
        #region Private Fields

        /// <summary>
        /// The <see cref="Converter{TValue, TKey}"/> method that converts a specified <typeparamref
        /// name="TValue"/> element into its associated <typeparamref name="TKey"/> key.</summary>

        private Converter<TValue, TKey> _keyConverter;

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="MultiKeyedList{TKey, TValue}"/>.</summary>
        /// <remarks><para>
        /// Attempting to modify the <b>Empty</b> collection will raise a <see
        /// cref="NotSupportedException"/>. The collection has zero capacity and is guaranteed to
        /// never change, as there are no writable references to the collection.
        /// </para><para>
        /// The <see cref="Converter{TKey, TValue}"/> used by <b>Empty</b> is a pseudo-converter
        /// that maps all values to the default key.</para></remarks>

        public static readonly new MultiKeyedList<TKey, TValue> Empty =
             new MultiKeyedList<TKey, TValue>(0, v => default(TKey)).AsReadOnly();

        #endregion
        #region Item[TKey]

        /// <summary>
        /// Gets the value associated with the first occurrence of the specified key.</summary>
        /// <param name="key">
        /// The key whose value to get.</param>
        /// <value>
        /// The value associated with the first occurrence of the specified <paramref name="key"/>.
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyNotFoundException">
        /// <paramref name="key"/> does not exist in the <see cref="MultiKeyedList{TKey, TValue}"/>.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.this"/> for details.</remarks>

        public TValue this[TKey key] {
            [DebuggerStepThrough]
            get { return GetByKey(key); }
        }

        #endregion
        #region KeyConverter

        /// <summary>
        /// Gets or sets the <see cref="Converter{TValue, TKey}"/> that converts an element in the
        /// <see cref="MultiKeyedList{TKey, TValue}"/> into its associated key.</summary>
        /// <value>
        /// The <see cref="Converter{TValue, TKey}"/> method that converts a specified <typeparamref
        /// name="TValue"/> element into its associated <typeparamref name="TKey"/> key.</value>
        /// <exception cref="ArgumentNullException">
        /// The property is set to a null reference.</exception>
        /// <remarks>
        /// <b>KeyConverter</b> never returns a null reference. The method should throw an <see
        /// cref="ArgumentNullException"/> when invoked with a null reference.</remarks>

        public Converter<TValue, TKey> KeyConverter {
            [DebuggerStepThrough]
            get { return _keyConverter; }
            [DebuggerStepThrough]
            set {
                if (value == null)
                    ThrowHelper.ThrowArgumentNullException("value");

                _keyConverter = value;
            }
        }

        #endregion
        #region Protected Methods
        #region CheckWritable(TValue)

        /// <summary>
        /// Checks that the specified value can be added to the <see cref="MultiKeyedList{TKey,
        /// TValue}"/>.</summary>
        /// <param name="value">
        /// The value to add.</param>
        /// <exception cref="ArgumentNullException">
        /// The key embedded in <paramref name="value"/> is a null reference.</exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="MultiKeyedList{TKey, TValue}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="MultiKeyedList{TKey, TValue}"/> already contains the key embedded in
        /// <paramref name="value"/>, and the <see cref="MultiKeyedList{TKey, TValue}"/> ensures
        /// that all keys are unique.</para></exception>

        protected override void CheckWritable(TValue value) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            if (value != null) {
                TKey key = _keyConverter(value);
                if (key == null)
                    ThrowHelper.ThrowArgumentNullException("KeyConverter(value)");

                if (UniqueFlag && IndexOfKey(key) >= 0)
                    ThrowHelper.ThrowNotSupportedException(Strings.CollectionUnique);
            }
        }

        #endregion
        #region CheckWritable(Int32, TValue)

        /// <summary>
        /// Checks that the specified value can be inserted into the <see cref="MultiKeyedList{TKey,
        /// TValue}"/> at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index at which to insert <paramref name="value"/>.</param>
        /// <param name="value">
        /// The value to insert.</param>
        /// <exception cref="ArgumentNullException">
        /// The key embedded in <paramref name="value"/> is a null reference.</exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="MultiKeyedList{TKey, TValue}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="MultiKeyedList{TKey, TValue}"/> already contains the key embedded in
        /// <paramref name="value"/> at a different index, and the <see cref="MultiKeyedList{TKey,
        /// TValue}"/> ensures that all keys are unique.</para></exception>
        /// <remarks>
        /// <b>CheckWritable</b> does not test for duplicate keys if the specified <paramref
        /// name="index"/> is less than zero, or equal to or greater than <see
        /// cref="ListEx{T}.Count"/>.</remarks>

        protected override void CheckWritable(int index, TValue value) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            if (value != null) {
                TKey key = _keyConverter(value);
                if (key == null)
                    ThrowHelper.ThrowArgumentNullException("KeyConverter(value)");

                if (UniqueFlag && index >= 0 && index < Count) {
                    int existing = IndexOfKey(key);
                    if (existing >= 0 && existing != index)
                        ThrowHelper.ThrowNotSupportedException(Strings.CollectionUnique);
                }
            }
        }

        #endregion
        #endregion
        #region Public Methods
        #region AsReadOnly

        /// <summary>
        /// Creates a read-only view of the <see cref="MultiKeyedList{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A read-only wrapper around the <see cref="MultiKeyedList{TKey, TValue}"/>.</returns>
        /// <remarks><para>
        /// Attempting to modify the read-only wrapper returned by <b>AsReadOnly</b> will raise a
        /// <see cref="NotSupportedException"/>. Note that the original collection may still change,
        /// and any such changes will be reflected in the read-only view.
        /// </para><para>
        /// <b>AsReadOnly</b> buffers the newly created read-only wrapper when the method is first
        /// called, and returns the buffered value on subsequent calls.</para></remarks>

        public new MultiKeyedList<TKey, TValue> AsReadOnly() {

            if (ReadOnlyWrapper == null)
                ReadOnlyWrapper = new MultiKeyedList<TKey, TValue>(this, true);

            return (MultiKeyedList<TKey, TValue>) ReadOnlyWrapper;
        }

        #endregion
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="MultiKeyedList{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="MultiKeyedList{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> preserves the values of the <see cref="ListEx{T}.IsUnique"/> and <see
        /// cref="KeyConverter"/> properties, but not the values of the <see
        /// cref="ListEx{T}.IsFixedSize"/> and <see cref="ListEx{T}.IsReadOnly"/> properties.
        /// </remarks>

        public override object Clone() {
            return new MultiKeyedList<TKey, TValue>(this);
        }

        #endregion
        #region ContainsKey

        /// <summary>
        /// Determines whether the <see cref="MultiKeyedList{TKey, TValue}"/> contains the specified
        /// key.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="MultiKeyedList{TKey, TValue}"/> contains an element with
        /// the specified <paramref name="key"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.ContainsKey"/> for details.
        /// </remarks>

        public bool ContainsKey(TKey key) {
            return (IndexOfKey(key) >= 0);
        }

        #endregion
        #region Copy

        /// <summary>
        /// Creates a deep copy of the <see cref="MultiKeyedList{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="MultiKeyedList{TKey, TValue}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="TValue"/> does not implement <see cref="ICloneable"/>.</exception>
        /// <remarks><para>
        /// <b>Copy</b> is similar to <see cref="Clone"/> but creates a deep copy the <see
        /// cref="MultiKeyedList{TKey, TValue}"/> by invoking <see cref="ICloneable.Clone"/> on all
        /// <typeparamref name="TValue"/> elements.
        /// </para><para>
        /// <b>Copy</b> preserves the values of the <see cref="ListEx{T}.IsUnique"/> and <see
        /// cref="KeyConverter"/> properties, but not the values of the <see
        /// cref="ListEx{T}.IsFixedSize"/> and <see cref="ListEx{T}.IsReadOnly"/> properties.
        /// </para></remarks>

        public new MultiKeyedList<TKey, TValue> Copy() {

            int count = InnerList.Count;
            var copy = new MultiKeyedList<TKey, TValue>(count, _keyConverter, UniqueFlag);

            for (int i = 0; i < count; i++) {
                TValue value = InnerList[i];

                ICloneable cloneable = (ICloneable) value;
                if (cloneable != null)
                    value = (TValue) cloneable.Clone();

                copy.InnerList.Add(value);
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
        /// The value at the specified <paramref name="index"/> in the <see
        /// cref="MultiKeyedList{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="ListEx{T}.Count"/>.
        /// </para></exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList.GetByIndex"/> for details.</remarks>

        public TValue GetByIndex(int index) {
            return InnerList[index];
        }

        #endregion
        #region GetByKey

        /// <summary>
        /// Gets the value associated with the first occurrence of the specified key.</summary>
        /// <param name="key">
        /// The key whose value to get.</param>
        /// <returns>
        /// The value associated with the first occurrence of <paramref name="key"/> in the <see
        /// cref="MultiKeyedList{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyNotFoundException">
        /// <paramref name="key"/> was not found in the <see cref="MultiKeyedList{TKey, TValue}"/>.
        /// </exception>
        /// <remarks>
        /// <b>GetByKey</b> has the same effect as reading the key indexer, <see
        /// cref="this[TKey]"/>.</remarks>

        public TValue GetByKey(TKey key) {

            int index = IndexOfKey(key);
            if (index < 0)
                ThrowHelper.ThrowKeyNotFoundException(key);

            return InnerList[index];
        }

        #endregion
        #region GetKey

        /// <overloads>
        /// Gets the key associated with the specified value.</overloads>
        /// <summary>
        /// Gets the key at the specified index of the <see cref="MultiKeyedList{TKey, TValue}"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the key to get.</param>
        /// <returns>
        /// The key at the specified <paramref name="index"/> of the <see cref="MultiKeyedList{TKey,
        /// TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <see cref="MultiKeyedList{TKey, TValue}"/> element at the specified <paramref
        /// name="index"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="ListEx{T}.Count"/>.
        /// </para></exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList.GetKey"/> for details.</remarks>

        public TKey GetKey(int index) {
            return _keyConverter(InnerList[index]);
        }

        #endregion
        #region IndexOfKey

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified key.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="key"/> in the <see
        /// cref="MultiKeyedList{TKey, TValue}"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.IndexOfKey"/> for details.</remarks>

        public int IndexOfKey(TKey key) {
            if (key == null)
                ThrowHelper.ThrowArgumentNullException("key");

            var comparer = ComparerCache<TKey>.EqualityComparer;

            for (int i = 0; i < InnerList.Count; i++) {
                TValue value = InnerList[i];
                if (value != null && comparer.Equals(key, _keyConverter(value)))
                    return i;
            }

            return -1;
        }

        #endregion
        #region SetByIndex

        /// <summary>
        /// Sets the value at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index of the value to set.</param>
        /// <param name="value">
        /// The value to store at the specified <paramref name="index"/> in the <see
        /// cref="MultiKeyedList{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// The key embedded in <paramref name="value"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="ListEx{T}.Count"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="MultiKeyedList{TKey, TValue}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="MultiKeyedList{TKey, TValue}"/> already contains the key embedded in 
        /// <paramref name="value"/> at a different index, and the <see cref="MultiKeyedList{TKey,
        /// TValue}"/> ensures that all keys are unique.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList.SetByIndex"/> for details.</remarks>

        public void SetByIndex(int index, TValue value) {
            CheckWritable(index, value);
            InnerList[index] = value;
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
                value = InnerList[index];
                return true;
            }

            value = default(TValue);
            return false;
        }

        #endregion
        #endregion
    }
}
