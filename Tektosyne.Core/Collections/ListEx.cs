using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides a generic collection of elements that are accessible by index.</summary>
    /// <typeparam name="T">
    /// The type of all elements in the collection. If <typeparamref name="T"/> is a reference type,
    /// elements may be null references.</typeparam>
    /// <remarks><para>
    /// <b>ListEx</b> provides a <see cref="List{T}"/> with a few extra features:
    /// </para><list type="bullet"><item>
    /// <see cref="ListEx{T}.IsUnique"/> provides a set-like collection which ensures that all
    /// elements have unique values. Attempting to insert a duplicate value will raise a <see
    /// cref="NotSupportedException"/>. This property must be set during construction.
    /// </item><item>
    /// <see cref="ListEx{T}.AsReadOnly"/> returns a read-only wrapper that has the same public type
    /// as the original collection. Attempting to modify the collection through such a read-only
    /// view will raise a <see cref="NotSupportedException"/>.
    /// </item><item>
    /// <see cref="ListEx{T}.Copy"/> creates a deep copy of the collection by invoking <see
    /// cref="ICloneable.Clone"/> on each element. This feature requires that all copied elements
    /// implement the <see cref="ICloneable"/> interface.
    /// </item><item>
    /// <see cref="ListEx{T}.Empty"/> returns an immutable empty collection that is cached for
    /// repeated access.
    /// </item><item>
    /// <see cref="ListEx{T}.Equals"/> compares two collections with identical element types for
    /// value equality of all elements. The collections compare as equal if they contain the same
    /// elements in the same order.
    /// </item></list><para>
    /// Moreover, several properties that the standard class provides as explicit interface
    /// implementations have been elevated to public visibility.</para></remarks>

    [Serializable]
    public class ListEx<T>: IList<T>, IList, ICloneable {
        #region ListEx()

        /// <overloads>
        /// Initializes a new instance of the <see cref="ListEx{T}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ListEx{T}"/> class that is empty and has
        /// the default initial capacity.</summary>
        /// <remarks>
        /// Please refer to <see cref="List{T}()"/> for details.</remarks>

        public ListEx(): this(false) { }

        #endregion
        #region ListEx(Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEx{T}"/> class that is empty, has the
        /// default initial capacity, and optionally ensures that all elements are unique.</summary>
        /// <param name="isUnique">
        /// The initial value for the <see cref="IsUnique"/> property.</param>
        /// <remarks>
        /// Please refer to <see cref="List{T}()"/> for details.</remarks>

        public ListEx(bool isUnique) {
            InnerList = new List<T>();
            UniqueFlag = isUnique;
        }

        #endregion
        #region ListEx(IEnumerable<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEx{T}"/> class that contains elements
        /// copied from the specified collection and has sufficient capacity to accommodate the
        /// number of elements copied.</summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> collection whose elements are copied to the new
        /// collection.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(IEnumerable{T})"/> for details.</remarks>

        public ListEx(IEnumerable<T> collection) {
            InnerList = new List<T>(collection);
        }

        #endregion
        #region ListEx(Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEx{T}"/> class that is empty and has
        /// the specified initial capacity.</summary>
        /// <param name="capacity">
        /// The number of elements that the new <see cref="ListEx{T}"/> is initially capable of
        /// storing.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>

        public ListEx(int capacity): this(capacity, false) { }

        #endregion
        #region ListEx(Int32, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEx{T}"/> class that is empty, has the
        /// specified initial capacity, and optionally ensures that all elements are unique.
        /// </summary>
        /// <param name="capacity">
        /// The number of elements that the new <see cref="ListEx{T}"/> is initially capable of
        /// storing.</param>
        /// <param name="isUnique">
        /// The initial value for the <see cref="IsUnique"/> property.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>

        public ListEx(int capacity, bool isUnique) {
            InnerList = new List<T>(capacity);
            UniqueFlag = isUnique;
        }

        #endregion
        #region ListEx(ListEx<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEx{T}"/> class that contains elements
        /// copied from the specified instance and has sufficient capacity to accommodate the
        /// number of elements copied.</summary>
        /// <param name="list">
        /// The <see cref="ListEx{T}"/> collection whose elements are copied to the new collection.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(IEnumerable{T})"/> for details. This constructor also
        /// copies the value of the <see cref="IsUnique"/> property.</remarks>

        public ListEx(ListEx<T> list) {
            if (list == null)
                ThrowHelper.ThrowArgumentNullException("list");

            InnerList = new List<T>(list.InnerList);
            UniqueFlag = list.UniqueFlag;
        }

        #endregion
        #region ListEx(ListEx<T>, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEx{T}"/> class that is a read-only view
        /// of the specified instance.</summary>
        /// <param name="list">
        /// The <see cref="ListEx{T}"/> collection that provides the initial values for the <see
        /// cref="InnerList"/> field and for the <see cref="IsUnique"/> property.</param>
        /// <param name="readOnly">
        /// The initial value for the <see cref="IsReadOnly"/> property. This argument must be
        /// <c>true</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="readOnly"/> is <c>false</c>.</exception>
        /// <remarks>
        /// This constructor is used to create a read-only wrapper around an existing collection.
        /// The new instance shares the data of the specified <paramref name="list"/>.</remarks>

        protected ListEx(ListEx<T> list, bool readOnly) {
            if (list == null)
                ThrowHelper.ThrowArgumentNullException("list");

            if (!readOnly)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "readOnly", Strings.ArgumentEquals, false);

            InnerList = list.InnerList;
            UniqueFlag = list.UniqueFlag;
            ReadOnlyFlag = true;
            ReadOnlyWrapper = this;
        }

        #endregion
        #region Protected Fields

        /// <summary>
        /// The <see cref="List{T}"/> collection that holds the <typeparamref name="T"/> elements of
        /// the <see cref="ListEx{T}"/>.</summary>

        protected readonly List<T> InnerList;

        /// <summary>
        /// Backs the <see cref="IsReadOnly"/> property.</summary>

        protected readonly bool ReadOnlyFlag;

        /// <summary>
        /// The read-only <see cref="ListEx{T}"/> collection that is returned by the <see
        /// cref="AsReadOnly"/> method.</summary>

        protected ListEx<T> ReadOnlyWrapper;

        /// <summary>
        /// Backs the <see cref="IsUnique"/> property.</summary>

        protected readonly bool UniqueFlag;

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="ListEx{T}"/>.</summary>
        /// <remarks>
        /// Attempting to modify the <b>Empty</b> collection will raise a <see
        /// cref="NotSupportedException"/>. The collection has zero capacity and is guaranteed to
        /// never change, as there are no writable references to the collection.</remarks>

        public static readonly ListEx<T> Empty = new ListEx<T>(0).AsReadOnly();

        #endregion
        #region Public Properties
        #region Capacity

        /// <summary>
        /// Gets or sets the capacity of the <see cref="ListEx{T}"/>.</summary>
        /// <value>
        /// The number of elements that the <see cref="ListEx{T}"/> can contain.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The property is set to a value that is less than <see cref="Count"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="ListEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Capacity"/> for details.</remarks>

        public int Capacity {
            [DebuggerStepThrough]
            get { return InnerList.Capacity; }
            set {
                if (ReadOnlyFlag)
                    ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

                InnerList.Capacity = value;
            }
        }

        #endregion
        #region Count

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ListEx{T}"/>.</summary>
        /// <value>
        /// The number of elements contained in the <see cref="ListEx{T}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Count"/> for details.</remarks>

        public int Count {
            [DebuggerStepThrough]
            get { return InnerList.Count; }
        }

        #endregion
        #region IsFixedSize

        /// <summary>
        /// Gets a value indicating whether the <see cref="ListEx{T}"/> has a fixed size.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="ListEx{T}"/> has a fixed size; otherwise, <c>false</c>.
        /// The default is <c>false</c>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="IList.IsFixedSize"/> for details.
        /// </para><para>
        /// This property always returns the same value as the <see cref="IsReadOnly"/> property
        /// since any fixed-size <see cref="ListEx{T}"/> is also read-only, and vice versa.
        /// </para></remarks>

        public bool IsFixedSize {
            [DebuggerStepThrough]
            get { return ReadOnlyFlag; }
        }

        #endregion
        #region IsReadOnly

        /// <summary>
        /// Gets a value indicating whether the <see cref="ListEx{T}"/> is read-only.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="ListEx{T}"/> is read-only; otherwise, <c>false</c>. The
        /// default is <c>false</c>.</value>
        /// <remarks>
        /// Please refer to <see cref="IList.IsReadOnly"/> for details.</remarks>

        public bool IsReadOnly {
            [DebuggerStepThrough]
            get { return ReadOnlyFlag; }
        }

        #endregion
        #region IsSynchronized

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ListEx{T}"/> is synchronized
        /// (thread-safe).</summary>
        /// <value>
        /// <c>true</c> if access to the <see cref="ListEx{T}"/> is synchronized (thread-safe);
        /// otherwise, <c>false</c>. The default is <c>false</c>.</value>
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
        #region IsUnique

        /// <summary>
        /// Gets a value indicating whether the <see cref="ListEx{T}"/> ensures that all elements
        /// are unique.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="ListEx{T}"/> ensures that all elements are unique;
        /// otherwise, <c>false</c>. The default is <c>false</c>.</value>
        /// <remarks><para>
        /// <b>IsUnique</b> provides a set-like collection by ensuring that all elements in the <see
        /// cref="ListEx{T}"/> are unique. When the property is <c>true</c>, any attempt to add an
        /// element that is already contained in the <see cref="ListEx{T}"/> will cause a <see
        /// cref="NotSupportedException"/>.
        /// </para><note type="implementnotes">
        /// Due to the necessary test for duplicate elements, the addition of a new element becomes
        /// an O(<em>N</em>) operation when <b>IsUnique</b> is <c>true</c>.</note></remarks>

        public bool IsUnique {
            [DebuggerStepThrough]
            get { return UniqueFlag; }
        }

        #endregion
        #region Item

        /// <summary>
        /// Gets or sets the element at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set.</param>
        /// <value>
        /// The element at the specified <paramref name="index"/>.</value>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException"><para>
        /// The property is set, and the <see cref="ListEx{T}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The property is set, the <see cref="ListEx{T}"/> already contains the specified element
        /// at a different index, and the <see cref="ListEx{T}"/> ensures that all elements are
        /// unique.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.this"/> for details.</remarks>

        public T this[int index] {
            [DebuggerStepThrough]
            get { return InnerList[index]; }
            set {
                CheckWritable(index, value);
                InnerList[index] = value;
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set.</param>
        /// <value>
        /// The element at the specified <paramref name="index"/>. When the property is set, this
        /// value must be compatible with <typeparamref name="T"/>.</value>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </para></exception>
        /// <exception cref="InvalidCastException">
        /// The property is set to a value that is not compatible with <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="NotSupportedException"><para>
        /// The property is set, and the <see cref="ListEx{T}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The property is set, the <see cref="ListEx{T}"/> already contains the specified element
        /// at a different index, and the <see cref="ListEx{T}"/> ensures that all elements are
        /// unique.</para></exception>

        object IList.this[int index] {
            [DebuggerStepThrough]
            get { return this[index]; }
            [DebuggerStepThrough]
            set { this[index] = (T) value; }
        }

        #endregion
        #region SyncRoot

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ListEx{T}"/>.
        /// </summary>
        /// <value>
        /// An object that can be used to synchronize access to the <see cref="ListEx{T}"/>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="ICollection.SyncRoot"/> for details.
        /// </para><para>
        /// When synchronizing multi-threaded access to the <see cref="ListEx{T}"/>, obtain a lock
        /// on the <b>SyncRoot</b> object rather than the collection itself. A read-only view always
        /// returns the same <b>SyncRoot</b> object as the underlying writable collection.
        /// </para></remarks>

        public object SyncRoot {
            [DebuggerStepThrough]
            get { return ((ICollection) InnerList).SyncRoot; }
        }

        #endregion
        #endregion
        #region Protected Methods
        #region CheckWritable(T)

        /// <overloads>
        /// Checks that the specified element can be added to the <see cref="ListEx{T}"/>.
        /// </overloads>
        /// <summary>
        /// Checks that the specified element can be added to the end of the <see
        /// cref="ListEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to add.</param>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="ListEx{T}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="ListEx{T}"/> already contains the specified <paramref name="item"/>, and
        /// the <see cref="ListEx{T}"/> ensures that all elements are unique.</para></exception>

        protected virtual void CheckWritable(T item) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            if (UniqueFlag && InnerList.IndexOf(item) >= 0)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionUnique);
        }

        #endregion
        #region CheckWritable(Int32, T)

        /// <summary>
        /// Checks that the specified element can be inserted into the <see cref="ListEx{T}"/> at
        /// the specified index.</summary>
        /// <param name="index">
        /// The zero-based index at which to insert <paramref name="item"/>.</param>
        /// <param name="item">
        /// The element to insert.</param>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="ListEx{T}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="ListEx{T}"/> already contains the specified <paramref name="item"/> at a
        /// different index, and the <see cref="ListEx{T}"/> ensures that all elements are unique.
        /// </para></exception>
        /// <remarks>
        /// <b>CheckWritable</b> does not test for duplicate elements if the specified <paramref
        /// name="index"/> is less than zero, or equal to or greater than <see cref="Count"/>.
        /// </remarks>

        protected virtual void CheckWritable(int index, T item) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            if (UniqueFlag && index >= 0 && index < Count) {
                int existing = InnerList.IndexOf(item);
                if (existing >= 0 && existing != index)
                    ThrowHelper.ThrowNotSupportedException(Strings.CollectionUnique);
            }
        }

        #endregion
        #endregion
        #region Public Methods
        #region Add

        /// <summary>
        /// Adds an element to the end of the <see cref="ListEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to add.</param>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="ListEx{T}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="ListEx{T}"/> already contains the specified <paramref name="item"/>, and
        /// the <see cref="ListEx{T}"/> ensures that all elements are unique.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Add"/> for details.</remarks>

        public void Add(T item) {
            CheckWritable(item);
            InnerList.Add(item);
        }

        /// <summary>
        /// Adds an element to the end of the <see cref="ListEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to add. This argument must be compatible with <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ListEx{T}"/> index at which the <paramref name="item"/> has been added.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="item"/> is not compatible with <typeparamref name="T"/>.</exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="ListEx{T}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="ListEx{T}"/> already contains the specified <paramref name="item"/>, and
        /// the <see cref="ListEx{T}"/> ensures that all elements are unique.</para></exception>

        int IList.Add(object item) {
            T typedItem = (T) item;
            Add(typedItem);
            return InnerList.Count - 1;
        }

        #endregion
        #region AddRange

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ListEx{T}"/>.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> collection whose elements to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="ListEx{T}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="ListEx{T}"/> already contains one or more elements in the specified 
        /// <paramref name="collection"/>, and the <see cref="ListEx{T}"/> ensures that all elements
        /// are unique.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.AddRange"/> for details.</remarks>

        public void AddRange(IEnumerable<T> collection) {
            if (collection == null)
                ThrowHelper.ThrowArgumentNullException("collection");

            foreach (T item in collection) {
                CheckWritable(item);
                InnerList.Add(item);
            }
        }

        #endregion
        #region AsReadOnly

        /// <summary>
        /// Returns a read-only view of the <see cref="ListEx{T}"/>.</summary>
        /// <returns>
        /// A read-only wrapper around the <see cref="ListEx{T}"/>.</returns>
        /// <remarks><para>
        /// Attempting to modify the read-only wrapper returned by <b>AsReadOnly</b> will raise a
        /// <see cref="NotSupportedException"/>. Note that the original collection may still change,
        /// and any such changes will be reflected in the read-only view.
        /// </para><para>
        /// <b>AsReadOnly</b> buffers the newly created read-only wrapper when the method is first
        /// called, and returns the buffered value on subsequent calls.</para></remarks>

        public ListEx<T> AsReadOnly() {

            if (ReadOnlyWrapper == null)
                ReadOnlyWrapper = new ListEx<T>(this, true);

            return ReadOnlyWrapper;
        }

        #endregion
        #region BinarySearch(T)

        /// <overloads>
        /// Uses a binary search algorithm to locate a specific element in the sorted <see
        /// cref="ListEx{T}"/> or a portion of it.</overloads>
        /// <summary>
        /// Searches the entire sorted <see cref="ListEx{T}"/> for the specified element using the
        /// default comparer, and returns the zero-based index of the element.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="ListEx{T}"/>,
        /// if <paramref name="item"/> is found; otherwise, a negative number, which is the bitwise
        /// complement of the index of the next element that is larger than <paramref name="item"/>
        /// or, if there is no larger element, the bitwise complement of <see cref="Count"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The default <see cref="Comparer{T}"/> cannot find a generic or non-generic <see
        /// cref="IComparer"/> implementation for <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.BinarySearch(T)"/> for details.</remarks>

        public int BinarySearch(T item) {
            return InnerList.BinarySearch(item);
        }

        #endregion
        #region BinarySearch(T, IComparer<T>)

        /// <summary>
        /// Searches the entire sorted <see cref="ListEx{T}"/> for the specified element using the
        /// specified comparer, and returns the zero-based index of the element.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> to use when comparing elements, or a null reference to
        /// use the default <see cref="Comparer{T}"/> for <typeparamref name="T"/>.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="ListEx{T}"/>,
        /// if <paramref name="item"/> is found; otherwise, a negative number, which is the bitwise
        /// complement of the index of the next element that is larger than <paramref name="item"/>
        /// or, if there is no larger element, the bitwise complement of <see cref="Count"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer"/> is a null reference, and the default <see
        /// cref="Comparer{T}"/> cannot find a generic or non-generic <see cref="IComparer"/>
        /// implementation for <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.BinarySearch(T, IComparer{T})"/> for details.
        /// </remarks>

        public int BinarySearch(T item, IComparer<T> comparer) {
            return InnerList.BinarySearch(item, comparer);
        }

        #endregion
        #region BinarySearch(Int32, Int32, T, IComparer<T>)

        /// <summary>
        /// Searches a subrange of the sorted <see cref="ListEx{T}"/> for the specified element
        /// using the specified comparer, and returns the zero-based index of the element.</summary>
        /// <param name="index">
        /// The zero-based starting index of the range to search.</param>
        /// <param name="count">
        /// The length of the range to search.</param>
        /// <param name="item">
        /// The element to locate.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> to use when comparing elements, or a null reference to
        /// use the default <see cref="Comparer{T}"/> for <typeparamref name="T"/>.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="ListEx{T}"/>,
        /// if <paramref name="item"/> is found; otherwise, a negative number, which is the bitwise
        /// complement of the index of the next element that is larger than <paramref name="item"/>
        /// or, if there is no larger element, the bitwise complement of <see cref="Count"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        /// <see cref="ListEx{T}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer"/> is a null reference, and the default <see
        /// cref="Comparer{T}"/> cannot find a generic or non-generic <see cref="IComparer"/>
        /// implementation for <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.BinarySearch(Int32, Int32, T, IComparer{T})"/> for
        /// details.</remarks>

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) {
            return InnerList.BinarySearch(index, count, item, comparer);
        }

        #endregion
        #region Clear

        /// <summary>
        /// Removes all elements from the <see cref="ListEx{T}"/>.</summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Clear"/> for details.</remarks>

        public void Clear() {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerList.Clear();
        }

        #endregion
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="ListEx{T}"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="ListEx{T}"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> preserves the value of the <see cref="IsUnique"/> property, but not the 
        /// values of the <see cref="IsFixedSize"/> and <see cref="IsReadOnly"/> properties.
        /// </remarks>

        public virtual object Clone() {
            return new ListEx<T>(this);
        }

        #endregion
        #region Contains

        /// <summary>
        /// Determines whether the <see cref="ListEx{T}"/> contains the specified element.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="item"/> is found in the <see cref="ListEx{T}"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Contains"/> for details.</remarks>

        public bool Contains(T item) {
            return InnerList.Contains(item);
        }

        /// <summary>
        /// Determines whether the <see cref="ListEx{T}"/> contains the specified element.</summary>
        /// <param name="item">
        /// The element to locate. This argument must be compatible with <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="item"/> is found in the <see cref="ListEx{T}"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="item"/> is not compatible with <typeparamref name="T"/>.</exception>

        bool IList.Contains(object item) {
            return Contains((T) item);
        }

        #endregion
        #region Copy

        /// <summary>
        /// Creates a deep copy of the <see cref="ListEx{T}"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="ListEx{T}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="T"/> does not implement <see cref="ICloneable"/>.</exception>
        /// <remarks><para>
        /// <b>Copy</b> is similar to <see cref="Clone"/> but creates a deep copy the <see
        /// cref="ListEx{T}"/> by invoking <see cref="ICloneable.Clone"/> on all elements.
        /// </para><para>
        /// <b>Copy</b> preserves the value of the <see cref="IsUnique"/> property, but not the
        /// values of the <see cref="IsFixedSize"/> and <see cref="IsReadOnly"/> properties.
        /// </para></remarks>

        public ListEx<T> Copy() {

            int count = InnerList.Count;
            ListEx<T> copy = new ListEx<T>(count, UniqueFlag);

            for (int i = 0; i < count; i++) {
                T item = InnerList[i];

                ICloneable cloneable = (ICloneable) item;
                if (cloneable != null)
                    item = (T) cloneable.Clone();

                copy.InnerList.Add(item);
            }

            return copy;
        }

        #endregion
        #region CopyTo(T[])

        /// <overloads>
        /// Copies the entire <see cref="ListEx{T}"/> to a one-dimensional <see cref="Array"/>.
        /// </overloads>
        /// <summary>
        /// Copies the entire <see cref="ListEx{T}"/> to a one-dimensional <see cref="Array"/>,
        /// starting at the beginning of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from the <see cref="ListEx{T}"/>. The <b>Array</b> must have zero-based indexing.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="ListEx{T}"/> is greater than the
        /// available space in the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.CopyTo(T[])"/> for details.</remarks>

        public void CopyTo(T[] array) {
            InnerList.CopyTo(array, 0);
        }

        #endregion
        #region CopyTo(T[], Int32)

        /// <summary>
        /// Copies the entire <see cref="ListEx{T}"/> to a one-dimensional <see cref="Array"/>,
        /// starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from the <see cref="ListEx{T}"/>. The <b>Array</b> must have zero-based indexing.
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
        /// The number of elements in the source <see cref="ListEx{T}"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.CopyTo(T[], Int32)"/> for details.</remarks>

        public void CopyTo(T[] array, int arrayIndex) {
            InnerList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the entire <see cref="ListEx{T}"/> to a one-dimensional <see cref="Array"/>,
        /// starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from the <see cref="ListEx{T}"/>. The <b>Array</b> must have zero-based indexing.
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
        /// The number of elements in the source <see cref="ListEx{T}"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.</para></exception>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="T"/> cannot be cast automatically to the type of the destination
        /// <paramref name="array"/>.</exception>

        void ICollection.CopyTo(Array array, int arrayIndex) {
            ((ICollection) InnerList).CopyTo(array, arrayIndex);
        }

        #endregion
        #region CopyTo(Int32, T[], Int32, Int32)

        /// <summary>
        /// Copies a subrange of the <see cref="ListEx{T}"/> to a one-dimensional <see
        /// cref="Array"/>, starting at the specified index of the target array.</summary>
        /// <param name="index">
        /// The zero-based index in the <see cref="ListEx{T}"/> at which copying begins.</param>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from the <see cref="ListEx{T}"/>. The <b>Array</b> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="count">
        /// The number of elements to copy.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="arrayIndex"/> is less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="count"/> is less than zero.</para></exception>
        /// <exception cref="ArgumentException"><para>
        /// <paramref name="index"/> is equal to or greater than the <see cref="Count"/> of the <see
        /// cref="ListEx{T}"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="count"/> is greater than the number of elements from <paramref
        /// name="index"/> to the <see cref="Count"/> of the <see cref="ListEx{T}"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref
        /// name="array"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="count"/> is greater than the available space from <paramref
        /// name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </para></exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.CopyTo(Int32, T[], Int32, Int32)"/> for details.
        /// </remarks>

        public void CopyTo(int index, T[] array, int arrayIndex, int count) {
            InnerList.CopyTo(index, array, arrayIndex, count);
        }

        #endregion
        #region Equals

        /// <summary>
        /// Determines whether the specified collection contains the same elements in the same order
        /// as the current <see cref="ListEx{T}"/>.</summary>
        /// <param name="collection">
        /// The <see cref="ICollection{T}"/> of elements to compare with the current <see
        /// cref="ListEx{T}"/>.</param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="collection"/> is another reference to this <see cref="ListEx{T}"/>.
        /// </item><item>
        /// <paramref name="collection"/> is not a null reference, contains the same number of
        /// elements as this <see cref="ListEx{T}"/>, and all elements compare as equal when
        /// retrieved in the enumeration sequence for each collection.</item></list></returns>
        /// <remarks>
        /// <b>Equals</b> calls <see cref="CollectionsUtility.SequenceEqual"/> to test the two
        /// collections for value equality.</remarks>

        public bool Equals(ICollection<T> collection) {
            return InnerList.SequenceEqual(collection);
        }

        #endregion
        #region GetEnumerator

        /// <summary>
        /// Returns a <see cref="List{T}.Enumerator"/> that can iterate through the <see
        /// cref="ListEx{T}"/>.</summary>
        /// <returns>
        /// A <see cref="List{T}.Enumerator"/> for the entire <see cref="ListEx{T}"/>.</returns>
        /// <remarks>
        /// Please refer to <see cref="List{T}.GetEnumerator"/> for details.</remarks>

        public List<T>.Enumerator GetEnumerator() {
            return InnerList.GetEnumerator();
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that can iterate through the <see
        /// cref="ListEx{T}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> for the entire <see cref="ListEx{T}"/>.</returns>

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return InnerList.GetEnumerator();
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that can iterate through the <see
        /// cref="ListEx{T}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> for the entire <see cref="ListEx{T}"/>.</returns>

        IEnumerator IEnumerable.GetEnumerator() {
            return InnerList.GetEnumerator();
        }

        #endregion
        #region GetRange

        /// <summary>
        /// Creates a shallow copy of a subrange of the <see cref="ListEx{T}"/>.</summary>
        /// <param name="index">
        /// The zero-based starting index of the range of elements to copy.</param>
        /// <param name="count">
        /// The number of elements to copy.</param>
        /// <returns>
        /// A shallow copy of the subrange of the <see cref="ListEx{T}"/> that starts at <paramref
        /// name="index"/> and contains <paramref name="count"/> elements.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        /// <see cref="ListEx{T}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.GetRange"/> for details. Note that this method
        /// returns a standard <see cref="List{T}"/>, not a <see cref="ListEx{T}"/>.</remarks>

        public List<T> GetRange(int index, int count) {
            return InnerList.GetRange(index, count);
        }

        #endregion
        #region IndexOf(T)

        /// <overloads>
        /// Returns the zero-based index of the first occurrence of the specified element in the
        /// <see cref="ListEx{T}"/> or a portion of it.</overloads>
        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified element in the
        /// entire <see cref="ListEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> in the entire
        /// <see cref="ListEx{T}"/>, if found; otherwise, -1.</returns>
        /// <remarks>
        /// Please refer to <see cref="List{T}.IndexOf(T)"/> for details.</remarks>

        public int IndexOf(T item) {
            return InnerList.IndexOf(item);
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified element in the
        /// <see cref="ListEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to locate. This argument must be compatible with <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> in the <see
        /// cref="ListEx{T}"/>, if found; otherwise, -1.</returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="item"/> is not compatible with <typeparamref name="T"/>.</exception>

        int IList.IndexOf(object item) {
            return IndexOf((T) item);
        }

        #endregion
        #region IndexOf(T, Int32)

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified element in the
        /// <see cref="ListEx{T}"/>, starting at the specified index.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <param name="index">
        /// The zero-based starting index of the search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> in the subrange
        /// of the <see cref="ListEx{T}"/> that starts at <paramref name="index"/>, if found;
        /// otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index for the <see cref="ListEx{T}"/>.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.IndexOf(T, Int32)"/> for details.</remarks>

        public int IndexOf(T item, int index) {
            return InnerList.IndexOf(item, index);
        }

        #endregion
        #region IndexOf(T, Int32, Int32)

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified element in a
        /// subrange of the <see cref="ListEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <param name="index">
        /// The zero-based starting index of the search.</param>
        /// <param name="count">
        /// The number of elements to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> in the subrange
        /// of the <see cref="ListEx{T}"/> that starts at <paramref name="index"/> and contains
        /// <paramref name="count"/> elements, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        /// <see cref="ListEx{T}"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.IndexOf(T, Int32, Int32)"/> for details.</remarks>

        public int IndexOf(T item, int index, int count) {
            return InnerList.IndexOf(item, index, count);
        }

        #endregion
        #region Insert

        /// <summary>
        /// Inserts an element into the <see cref="ListEx{T}"/> at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index at which to insert <paramref name="item"/>.</param>
        /// <param name="item">
        /// The element to insert.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than <see cref="Count"/>.
        /// </exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="ListEx{T}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="ListEx{T}"/> already contains the specified <paramref name="item"/>, and
        /// the <see cref="ListEx{T}"/> ensures that all elements are unique.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Insert"/> for details.</remarks>

        public void Insert(int index, T item) {
            CheckWritable(item);
            InnerList.Insert(index, item);
        }

        /// <summary>
        /// Inserts an element into the <see cref="ListEx{T}"/> at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index at which to insert <paramref name="item"/>.</param>
        /// <param name="item">
        /// The element to insert. This argument must be compatible with <typeparamref name="T"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than <see cref="Count"/>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// <paramref name="item"/> is not compatible with <typeparamref name="T"/>.</exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="ListEx{T}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="ListEx{T}"/> already contains the specified <paramref name="item"/>, and
        /// the <see cref="ListEx{T}"/> ensures that all elements are unique.</para></exception>

        void IList.Insert(int index, object item) {
            Insert(index, (T) item);
        }

        #endregion
        #region InsertRange

        /// <summary>
        /// Inserts the elements of the specified collection into the <see cref="ListEx{T}"/> at the
        /// specified index.</summary>
        /// <param name="index">
        /// The zero-based index at which to insert <paramref name="collection"/>.</param>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> collection whose elements to insert.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than <see cref="Count"/>.
        /// </exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="ListEx{T}"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="ListEx{T}"/> already contains one or more elements in the specified 
        /// <paramref name="collection"/>, and the <see cref="ListEx{T}"/> ensures that all elements
        /// are unique.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.InsertRange"/> for details.</remarks>

        public void InsertRange(int index, IEnumerable<T> collection) {
            if (collection == null)
                ThrowHelper.ThrowArgumentNullException("collection");

            foreach (T item in collection) {
                CheckWritable(item);
                InnerList.Insert(index++, item);
            }
        }

        #endregion
        #region LastIndexOf(T)

        /// <overloads>
        /// Returns the zero-based index of the last occurrence of the specified element in the
        /// <see cref="ListEx{T}"/> or a portion of it.</overloads>
        /// <summary>
        /// Returns the zero-based index of the last occurrence of the specified element in the
        /// entire <see cref="ListEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> in the entire
        /// <see cref="ListEx{T}"/>, if found; otherwise, -1.</returns>
        /// <remarks>
        /// Please refer to <see cref="List{T}.LastIndexOf(T)"/> for details.</remarks>

        public int LastIndexOf(T item) {
            return InnerList.LastIndexOf(item);
        }

        #endregion
        #region LastIndexOf(T, Int32)

        /// <summary>
        /// Returns the zero-based index of the last occurrence of the specified element in the
        /// <see cref="ListEx{T}"/>, ending at the specified index.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <param name="index">
        /// The zero-based starting index of the backward search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> in the subrange
        /// of the <see cref="ListEx{T}"/> that ends at <paramref name="index"/>, if found;
        /// otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index for the <see cref="ListEx{T}"/>.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.LastIndexOf(T, Int32)"/> for details.</remarks>

        public int LastIndexOf(T item, int index) {
            return InnerList.LastIndexOf(item, index);
        }

        #endregion
        #region LastIndexOf(T, Int32, Int32)

        /// <summary>
        /// Returns the zero-based index of the last occurrence of the specified element in a
        /// subrange of the <see cref="ListEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <param name="index">
        /// The zero-based starting index of the backward search.</param>
        /// <param name="count">
        /// The number of elements to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> in the subrange
        /// of the <see cref="ListEx{T}"/> that contains <paramref name="count"/> elements and ends
        /// at <paramref name="index"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        /// <see cref="ListEx{T}"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.LastIndexOf(T, Int32, Int32)"/> for details.</remarks>

        public int LastIndexOf(T item, int index, int count) {
            return InnerList.LastIndexOf(item, index, count);
        }

        #endregion
        #region Remove

        /// <summary>
        /// Removes the first occurrence of the specified element from the <see cref="ListEx{T}"/>.
        /// </summary>
        /// <param name="item">
        /// The element to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="item"/> was found and removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Remove"/> for details.</remarks>

        public bool Remove(T item) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            return InnerList.Remove(item);
        }

        /// <summary>
        /// Removes the first occurrence of the specified element from the <see cref="ListEx{T}"/>.
        /// </summary>
        /// <param name="item">
        /// The element to remove. This argument must be compatible with <typeparamref name="T"/>.
        /// </param>
        /// <exception cref="InvalidCastException">
        /// <paramref name="item"/> is not compatible with <typeparamref name="T"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListEx{T}"/> is read-only.</exception>

        void IList.Remove(object item) {
            Remove((T) item);
        }

        #endregion
        #region RemoveAt

        /// <summary>
        /// Removes the element at the specified index in the <see cref="ListEx{T}"/>.</summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.RemoveAt"/> for details.</remarks>

        public void RemoveAt(int index) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerList.RemoveAt(index);
        }

        #endregion
        #region RemoveRange

        /// <summary>
        /// Removes a range of elements from the <see cref="ListEx{T}"/>.</summary>
        /// <param name="index">
        /// The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">
        /// The number of elements to remove.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        /// <see cref="ListEx{T}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.RemoveRange"/> for details.</remarks>

        public void RemoveRange(int index, int count) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerList.RemoveRange(index, count);
        }

        #endregion
        #region Reverse()

        /// <overloads>
        /// Reverses the order of the elements in the <see cref="ListEx{T}"/> or a portion of it.
        /// </overloads>
        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="ListEx{T}"/>.</summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Reverse"/> for details.</remarks>

        public void Reverse() {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerList.Reverse();
        }

        #endregion
        #region Reverse(Int32, Int32)

        /// <summary>
        /// Reverses the order of the elements in the specified range.</summary>
        /// <param name="index">
        /// The zero-based starting index of the range of elements to reverse.</param>
        /// <param name="count">
        /// The number of elements to reverse.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range of
        /// elements in the <see cref="ListEx{T}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Reverse"/> for details.</remarks>

        public void Reverse(int index, int count) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerList.Reverse(index, count);
        }

        #endregion
        #region Sort()

        /// <overloads>
        /// Sorts the <see cref="ListEx{T}"/> or a portion of it.</overloads>
        /// <summary>
        /// Sorts the entire <see cref="ListEx{T}"/> using the default comparer.</summary>
        /// <exception cref="InvalidOperationException">
        /// The default <see cref="Comparer{T}"/> cannot find a generic or non-generic <see
        /// cref="IComparer"/> implementation for <typeparamref name="T"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Sort()"/> for details.</remarks>

        public void Sort() {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerList.Sort();
        }

        #endregion
        #region Sort(Comparison<T>)

        /// <summary>
        /// Sorts the entire <see cref="ListEx{T}"/> using the specified comparison method.
        /// </summary>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method to use when comparing elements.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="comparison"/> is a null reference.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Sort(Comparison{T})"/> for details.</remarks>

        public void Sort(Comparison<T> comparison) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerList.Sort(comparison);
        }

        #endregion
        #region Sort(IComparer<T>)

        /// <summary>
        /// Sorts the entire <see cref="ListEx{T}"/> using the specified comparer.</summary>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> to use when comparing elements, or a null reference to
        /// use the default <see cref="Comparer{T}"/> for <typeparamref name="T"/>.</param>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer"/> is a null reference, and the default <see
        /// cref="Comparer{T}"/> cannot find a generic or non-generic <see cref="IComparer"/>
        /// implementation for <typeparamref name="T"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Sort(IComparer{T})"/> for details.</remarks>

        public void Sort(IComparer<T> comparer) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerList.Sort(comparer);
        }

        #endregion
        #region Sort(Int32, Int32, IComparer<T>)

        /// <summary>
        /// Sorts a subrange of the <see cref="ListEx{T}"/> using the specified comparer.</summary>
        /// <param name="index">
        /// The zero-based starting index of the range to sort.</param>
        /// <param name="count">
        /// The length of the range to sort.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> to use when comparing elements, or a null reference to
        /// use the default <see cref="Comparer{T}"/> for <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        /// <see cref="ListEx{T}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer"/> is a null reference, and the default <see
        /// cref="Comparer{T}"/> cannot find a generic or non-generic <see cref="IComparer"/>
        /// implementation for <typeparamref name="T"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Sort(Int32, Int32, IComparer{T})"/> for details.
        /// </remarks>

        public void Sort(int index, int count, IComparer<T> comparer) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerList.Sort(index, count, comparer);
        }

        #endregion
        #region ToArray

        /// <summary>
        /// Copies the elements of the <see cref="ListEx{T}"/> to a new <see cref="Array"/>.
        /// </summary>
        /// <returns>
        /// A one-dimensional <see cref="Array"/> containing copies of the elements of the <see
        /// cref="ListEx{T}"/>.</returns>
        /// <remarks>
        /// Please refer to <see cref="List{T}.ToArray"/> for details.</remarks>

        public T[] ToArray() {
            return InnerList.ToArray();
        }

        #endregion
        #region TrimExcess

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="ListEx{T}"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ListEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.TrimExcess"/> for details.</remarks>

        public void TrimExcess() {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerList.TrimExcess();
        }

        #endregion
        #endregion
        #region Delegate Methods
        #region ConvertAll<TOutput>

        /// <summary>
        /// Converts the <see cref="ListEx{T}"/> to another element type.</summary>
        /// <typeparam name="TOutput">
        /// The type of all elements in the converted collection.</typeparam>
        /// <param name="converter">
        /// A <see cref="Converter{TInput, TOutput}"/> method that converts all elements from 
        /// <typeparamref name="T"/> to <typeparamref name="TOutput"/>.</param>
        /// <returns>
        /// A new <see cref="List{T}"/> containing the elements copied from the current collection
        /// and converted to <typeparamref name="TOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.ConvertAll{TOutput}"/> for details. Note that this
        /// method returns a standard <see cref="List{T}"/>, not a <see cref="ListEx{T}"/>.
        /// </remarks>

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) {
            return InnerList.ConvertAll<TOutput>(converter);
        }

        #endregion
        #region Exists

        /// <summary>
        /// Determines whether the <see cref="ListEx{T}"/> contains elements that match the
        /// conditions defined by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="ListEx{T}"/> contains one or more elements that match the
        /// conditions defined by <paramref name="predicate"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Exists"/> for details.</remarks>

        public bool Exists(Predicate<T> predicate) {
            return InnerList.Exists(predicate);
        }

        #endregion
        #region Find

        /// <summary>
        /// Finds the first element in the <see cref="ListEx{T}"/> that matches the conditions
        /// defined by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The first element that matches the conditions defined by <paramref name="predicate"/>,
        /// if found; otherwise, the default value for <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.Find"/> for details.</remarks>

        public T Find(Predicate<T> predicate) {
            return InnerList.Find(predicate);
        }

        #endregion
        #region FindAll

        /// <summary>
        /// Finds all elements in the <see cref="ListEx{T}"/> that match the conditions defined by
        /// the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// A new <see cref="List{T}"/> containing all elements that match the conditions defined by
        /// <paramref name="predicate"/>, if found; otherwise, an empty <b>List</b>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.FindAll"/> for details. Note that this method
        /// returns a standard <see cref="List{T}"/>, not a <see cref="ListEx{T}"/>.</remarks>

        public List<T> FindAll(Predicate<T> predicate) {
            return InnerList.FindAll(predicate);
        }

        #endregion
        #region FindIndex(Predicate<T>)

        /// <overloads>
        /// Returns the zero-based index of the first element in the <see cref="ListEx{T}"/>, or a
        /// portion of it, that matches the conditions defined by the specified predicate.
        /// </overloads>
        /// <summary>
        /// Returns the zero-based index of the first element in the entire <see cref="ListEx{T}"/>
        /// that matches the conditions defined by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The zero-based index of the first element that matches the conditions defined by
        /// <paramref name="predicate"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.FindIndex(Predicate{T})"/> for details.</remarks>

        public int FindIndex(Predicate<T> predicate) {
            return InnerList.FindIndex(predicate);
        }

        #endregion
        #region FindIndex(Int32, Predicate<T>)

        /// <summary>
        /// Returns the zero-based index of the first element in the <see cref="ListEx{T}"/>,
        /// starting at the specified index, that matches the conditions defined by the specified
        /// predicate.</summary>
        /// <param name="index">
        /// The zero-based starting index of the search.</param>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The zero-based index of the first element that matches the conditions defined by
        /// <paramref name="predicate"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index for the <see cref="ListEx{T}"/>.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.FindIndex(Int32, Predicate{T})"/> for details.
        /// </remarks>

        public int FindIndex(int index, Predicate<T> predicate) {
            return InnerList.FindIndex(index, predicate);
        }

        #endregion
        #region FindIndex(Int32, Int32, Predicate<T>)

        /// <summary>
        /// Returns the zero-based index of the first element in a subrange of the <see
        /// cref="ListEx{T}"/> that matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the search.</param>
        /// <param name="count">
        /// The number of elements to search.</param>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The zero-based index of the first element that matches the conditions defined by
        /// <paramref name="predicate"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        /// <see cref="ListEx{T}"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.FindIndex(Int32, Int32, Predicate{T})"/> for details.
        /// </remarks>

        public int FindIndex(int index, int count, Predicate<T> predicate) {
            return InnerList.FindIndex(index, count, predicate);
        }

        #endregion
        #region FindLast

        /// <summary>
        /// Finds the last element in the <see cref="ListEx{T}"/> that matches the conditions
        /// defined by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The last element that matches the conditions defined by <paramref name="predicate"/>, if
        /// found; otherwise, the default value for <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.FindLast"/> for details.</remarks>

        public T FindLast(Predicate<T> predicate) {
            return InnerList.FindLast(predicate);
        }

        #endregion
        #region FindLastIndex(Predicate<T>)

        /// <overloads>
        /// Returns the zero-based index of the last element in the <see cref="ListEx{T}"/>, or a
        /// portion of it, that matches the conditions defined by the specified predicate.
        /// </overloads>
        /// <summary>
        /// Returns the zero-based index of the last element in the entire <see cref="ListEx{T}"/>
        /// that matches the conditions defined by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The zero-based index of the last element that matches the conditions defined by
        /// <paramref name="predicate"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.FindLastIndex(Predicate{T})"/> for details.</remarks>

        public int FindLastIndex(Predicate<T> predicate) {
            return InnerList.FindLastIndex(predicate);
        }

        #endregion
        #region FindLastIndex(Int32, Predicate<T>)

        /// <summary>
        /// Returns the zero-based index of the last element in the <see cref="ListEx{T}"/>,
        /// ending at the specified index, that matches the conditions defined by the specified
        /// predicate.</summary>
        /// <param name="index">
        /// The zero-based starting index of the backward search.</param>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The zero-based index of the last element that matches the conditions defined by
        /// <paramref name="predicate"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index for the <see cref="ListEx{T}"/>.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.FindLastIndex(Int32, Predicate{T})"/> for details.
        /// </remarks>

        public int FindLastIndex(int index, Predicate<T> predicate) {
            return InnerList.FindLastIndex(index, predicate);
        }

        #endregion
        #region FindLastIndex(Int32, Int32, Predicate<T>)

        /// <summary>
        /// Returns the zero-based index of the last element in a subrange of the <see
        /// cref="ListEx{T}"/> that matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the backward search.</param>
        /// <param name="count">
        /// The number of elements to search.</param>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The zero-based index of the last element that matches the conditions defined by
        /// <paramref name="predicate"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        /// <see cref="ListEx{T}"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.FindLastIndex(Int32, Int32, Predicate{T})"/> for
        /// details.</remarks>

        public int FindLastIndex(int index, int count, Predicate<T> predicate) {
            return InnerList.FindLastIndex(index, count, predicate);
        }

        #endregion
        #region ForEach

        /// <summary>
        /// Performs the specified action on each element in the <see cref="ListEx{T}"/>.</summary>
        /// <param name="action">
        /// The <see cref="Action{T}"/> method to perform on each element.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.ForEach"/> for details.</remarks>

        public void ForEach(Action<T> action) {
            InnerList.ForEach(action);
        }

        #endregion
        #region RemoveAll

        /// <summary>
        /// Removes all elements from the <see cref="ListEx{T}"/> that match the conditions defined
        /// by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The number of elements that were removed from the <see cref="ListEx{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.RemoveAll"/> for details.</remarks>

        public int RemoveAll(Predicate<T> predicate) {
            return InnerList.RemoveAll(predicate);
        }

        #endregion
        #region TrueForAll

        /// <summary>
        /// Determines whether all elements in the <see cref="ListEx{T}"/> match the conditions
        /// defined by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// <c>true</c> if all elements in the <see cref="ListEx{T}"/> match the conditions defined
        /// by <paramref name="predicate"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.TrueForAll"/> for details.</remarks>

        public bool TrueForAll(Predicate<T> predicate) {
            return InnerList.TrueForAll(predicate);
        }

        #endregion
        #endregion
    }
}
