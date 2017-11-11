using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides a generic collection of <see cref="IKeyedValue{TKey}"/> elements that are
    /// accessible by index and by key.</summary>
    /// <typeparam name="TKey">
    /// The type of the <see cref="IKeyedValue{TKey}.Key"/> property that all elements in the
    /// collection inherit from the <see cref="IKeyedValue{TKey}"/> interface. Keys cannot be null
    /// references.</typeparam>
    /// <typeparam name="TValue">
    /// The type of all elements in the collection. This type must implement <see
    /// cref="IKeyedValue{TKey}"/> with the <typeparamref name="TKey"/> type. If <typeparamref
    /// name="TValue"/> is a reference type, elements may be null references.</typeparam>
    /// <remarks><para>
    /// <b>KeyedList</b> provides a <see cref="ListEx{T}"/> whose elements contain an embedded key,
    /// as defined by the <see cref="IKeyedValue{TKey}"/> interface. Several additional methods
    /// adopted from the <see cref="SortedListEx{TKey, TValue}"/> class allow direct access by key.
    /// </para><para>
    /// The collection may contain multiple identical keys, unless <see cref="ListEx{T}.IsUnique"/>
    /// is <c>true</c>. All key access methods return the first occurrence of the specified key.
    /// Access by index is an O(1) operation but access by key is an O(<em>N</em>) operation, where
    /// <em>N</em> is the number of elements in the collection.
    /// </para><para>
    /// <b>KeyedList</b> also provides several extra features inherited from <see
    /// cref="ListEx{T}"/>; please see there for details. Note that <see cref="ListEx{T}.IsUnique"/>
    /// now checks for unique keys, rather than unique key-and-value pairs.</para></remarks>

    [Serializable]
    public class KeyedList<TKey, TValue>: ListEx<TValue> where TValue: IKeyedValue<TKey> {
        #region KeyedList()

        /// <overloads>
        /// Initializes a new instance of the <see cref="KeyedList{TKey, TValue}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedList{TKey, TValue}"/> class that is
        /// empty and has the default initial capacity.</summary>
        /// <remarks>
        /// Please refer to <see cref="List{T}()"/> for details.</remarks>

        public KeyedList(): this(false) { }

        #endregion
        #region KeyedList(Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedList{TKey, TValue}"/> class that is
        /// empty, has the default initial capacity, and optionally ensures that all keys are
        /// unique.</summary>
        /// <param name="isUnique">
        /// The initial value for the <see cref="ListEx{T}.IsUnique"/> property.</param>
        /// <remarks>
        /// Please refer to <see cref="List{T}()"/> for details.</remarks>

        public KeyedList(bool isUnique): base(isUnique) { }

        #endregion
        #region KeyedList(IEnumerable<TValue>)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedList{TKey, TValue}"/> class that
        /// contains elements copied from the specified collection and has sufficient capacity to 
        /// accommodate the number of elements copied.</summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> collection whose <see cref="KeyValuePair{TKey,
        /// TValue}"/> elements are copied to the new collection.</param>
        /// <exception cref="ArgumentNullException"><para>
        /// <paramref name="collection"/> is a null reference.
        /// </para><para>-or-</para><para>
        /// <paramref name="collection"/> contains an element whose embedded key is a null
        /// reference.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(IEnumerable{T})"/> for details.</remarks>

        public KeyedList(IEnumerable<TValue> collection): base(collection) {

            foreach (TValue value in collection)
                if (value != null && value.Key == null)
                    ThrowHelper.ThrowArgumentNullException(
                        "collection", Strings.ArgumentContainsNullKey);
        }

        #endregion
        #region KeyedList(Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedList{TKey, TValue}"/> class that is
        /// empty and has the specified initial capacity.</summary>
        /// <param name="capacity">
        /// The number of elements that the new <see cref="KeyedList{TKey, TValue}"/> is initially
        /// capable of storing.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>

        public KeyedList(int capacity): this(capacity, false) { }

        #endregion
        #region KeyedList(Int32, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedList{TKey, TValue}"/> class that is
        /// empty, has the specified initial capacity, and optionally ensures that all keys are
        /// unique.</summary>
        /// <param name="capacity">
        /// The number of elements that the new <see cref="KeyedList{TKey, TValue}"/> is initially
        /// capable of storing.</param>
        /// <param name="isUnique">
        /// The initial value for the <see cref="ListEx{T}.IsUnique"/> property.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>

        public KeyedList(int capacity, bool isUnique): base(capacity, isUnique) { }

        #endregion
        #region KeyedList(KeyedList<TKey, TValue>)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedList{TKey, TValue}"/> class that
        /// contains elements copied from the specified instance and has sufficient capacity to 
        /// accommodate the number of elements copied.</summary>
        /// <param name="list">
        /// The <see cref="KeyedList{TKey, TValue}"/> collection whose elements are copied to the
        /// new collection.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(IEnumerable{T})"/> for details. This constructor also
        /// copies the value of the <see cref="ListEx{T}.IsUnique"/> property.</remarks>

        public KeyedList(KeyedList<TKey, TValue> list): base(list) { }

        #endregion
        #region KeyedList(KeyedList<TKey, TValue>, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedList{TKey, TValue}"/> class that is a
        /// read-only view of the specified instance.</summary>
        /// <param name="list">
        /// The <see cref="KeyedList{TKey, TValue}"/> collection that provides the initial values
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

        protected KeyedList(KeyedList<TKey, TValue> list, bool readOnly):
            base(list, readOnly) { }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="KeyedList{TKey, TValue}"/>.</summary>
        /// <remarks>
        /// Attempting to modify the <b>Empty</b> collection will raise a <see
        /// cref="NotSupportedException"/>. The collection has zero capacity and is guaranteed to
        /// never change, as there are no writable references to the collection.</remarks>

        public static readonly new KeyedList<TKey, TValue> Empty =
            new KeyedList<TKey, TValue>(0).AsReadOnly();

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
        /// <paramref name="key"/> does not exist in the <see cref="KeyedList{TKey, TValue}"/>.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="SortedList{TKey, TValue}.this"/> for details.</remarks>

        public TValue this[TKey key] {
            [DebuggerStepThrough]
            get { return GetByKey(key); }
        }

        #endregion
        #region Protected Methods
        #region CheckWritable(TValue)

        /// <summary>
        /// Checks that the specified value can be added to the <see cref="KeyedList{TKey,
        /// TValue}"/>.</summary>
        /// <param name="value">
        /// The value to add.</param>
        /// <exception cref="ArgumentNullException">
        /// The key embedded in <paramref name="value"/> is a null reference.</exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="KeyedList{TKey, TValue}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="KeyedList{TKey, TValue}"/> already contains the key embedded in <paramref
        /// name="value"/>, and the <see cref="KeyedList{TKey, TValue}"/> ensures that all keys are
        /// unique.</para></exception>

        protected override void CheckWritable(TValue value) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            if (value != null) {
                if (value.Key == null)
                    ThrowHelper.ThrowArgumentNullException("value.Key");

                if (UniqueFlag && IndexOfKey(value.Key) >= 0)
                    ThrowHelper.ThrowNotSupportedException(Strings.CollectionUnique);
            }
        }

        #endregion
        #region CheckWritable(Int32, TValue)

        /// <summary>
        /// Checks that the specified value can be inserted into the <see cref="KeyedList{TKey,
        /// TValue}"/> at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index at which to insert <paramref name="value"/>.</param>
        /// <param name="value">
        /// The value to insert.</param>
        /// <exception cref="ArgumentNullException">
        /// The key embedded in <paramref name="value"/> is a null reference.</exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="KeyedList{TKey, TValue}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="KeyedList{TKey, TValue}"/> already contains the key embedded in <paramref
        /// name="value"/> at a different index, and the <see cref="KeyedList{TKey, TValue}"/>
        /// ensures that all keys are unique.</para></exception>
        /// <remarks>
        /// <b>CheckWritable</b> does not test for duplicate keys if the specified <paramref
        /// name="index"/> is less than zero, or equal to or greater than <see
        /// cref="ListEx{T}.Count"/>.</remarks>

        protected override void CheckWritable(int index, TValue value) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            if (value != null) {
                if (value.Key == null)
                    ThrowHelper.ThrowArgumentNullException("value.Key");

                if (UniqueFlag && index >= 0 && index < Count) {
                    int existing = IndexOfKey(value.Key);
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
        /// Creates a read-only view of the <see cref="KeyedList{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A read-only wrapper around the <see cref="KeyedList{TKey, TValue}"/>.</returns>
        /// <remarks><para>
        /// Attempting to modify the read-only wrapper returned by <b>AsReadOnly</b> will raise a
        /// <see cref="NotSupportedException"/>. Note that the original collection may still change,
        /// and any such changes will be reflected in the read-only view.
        /// </para><para>
        /// <b>AsReadOnly</b> buffers the newly created read-only wrapper when the method is first
        /// called, and returns the buffered value on subsequent calls.</para></remarks>

        public new KeyedList<TKey, TValue> AsReadOnly() {

            if (ReadOnlyWrapper == null)
                ReadOnlyWrapper = new KeyedList<TKey, TValue>(this, true);

            return (KeyedList<TKey, TValue>) ReadOnlyWrapper;
        }

        #endregion
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="KeyedList{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="KeyedList{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> preserves the value of the <see cref="ListEx{T}.IsUnique"/> property, but
        /// not the values of the <see cref="ListEx{T}.IsFixedSize"/> and <see
        /// cref="ListEx{T}.IsReadOnly"/> properties.</remarks>

        public override object Clone() {
            return new KeyedList<TKey, TValue>(this);
        }

        #endregion
        #region ContainsKey

        /// <summary>
        /// Determines whether the <see cref="KeyedList{TKey, TValue}"/> contains the specified
        /// key.</summary>
        /// <param name="key">
        /// The key to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="KeyedList{TKey, TValue}"/> contains an element with
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
        /// Creates a deep copy of the <see cref="KeyedList{TKey, TValue}"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="KeyedList{TKey, TValue}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="TValue"/> does not implement <see cref="ICloneable"/>.</exception>
        /// <remarks><para>
        /// <b>Copy</b> is similar to <see cref="Clone"/> but creates a deep copy the <see
        /// cref="KeyedList{TKey, TValue}"/> by invoking <see cref="ICloneable.Clone"/> on all
        /// <typeparamref name="TValue"/> elements.
        /// </para><para>
        /// <b>Copy</b> preserves the value of the <see cref="ListEx{T}.IsUnique"/> property, but
        /// not the values of the <see cref="ListEx{T}.IsFixedSize"/> and <see
        /// cref="ListEx{T}.IsReadOnly"/> properties.</para></remarks>

        public new KeyedList<TKey, TValue> Copy() {

            int count = InnerList.Count;
            KeyedList<TKey, TValue> copy = new KeyedList<TKey, TValue>(count, UniqueFlag);

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
        /// The value at the specified <paramref name="index"/> in the <see cref="KeyedList{TKey,
        /// TValue}"/>.</returns>
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
        /// cref="KeyedList{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyNotFoundException">
        /// <paramref name="key"/> was not found in the <see cref="KeyedList{TKey, TValue}"/>.
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

        /// <summary>
        /// Gets the key at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index of the key to get.</param>
        /// <returns>
        /// The key at the specified <paramref name="index"/> in the <see cref="KeyedList{TKey,
        /// TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <see cref="KeyedList{TKey, TValue}"/> element at the specified <paramref
        /// name="index"/> is a null reference.</exception>
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
        /// cref="KeyedList{TKey, TValue}"/>, if found; otherwise, -1.</returns>
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
                if (value != null && comparer.Equals(key, value.Key))
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
        /// cref="KeyedList{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// The key embedded in <paramref name="value"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="ListEx{T}.Count"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="KeyedList{TKey, TValue}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="KeyedList{TKey, TValue}"/> already contains the key embedded in <paramref 
        /// name="value"/> at a different index, and the <see cref="KeyedList{TKey, TValue}"/>
        /// ensures that all keys are unique.</para></exception>
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
