using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides a fixed-size generic collection of elements that are accessible by
    /// multi-dimensional index.</summary>
    /// <typeparam name="T">
    /// The type of all elements in the collection. If <typeparamref name="T"/> is a reference type,
    /// elements may be null references.</typeparam>
    /// <remarks><para>
    /// <b>ArrayEx</b> provides an <see cref="Array"/> of arbitrary rank. The implementation uses
    /// custom index transformation with a one-dimensional backing <see cref="Array"/>. Compared to
    /// the standard class, <b>ArrayEx</b> offers a few extra features:
    /// </para><list type="bullet"><item>
    /// One-dimensional indexing is available regardless of <see cref="ArrayEx{T}.Rank"/>, using the
    /// same order as enumeration through a multi-dimensional standard <see cref="Array"/>. <see
    /// cref="ArrayEx{T}.GetIndex"/> and <see cref="ArrayEx{T}.GetIndices"/> convert indices between
    /// their equivalent one-dimensional and multi-dimensional representations.
    /// </item><item>
    /// <see cref="ArrayEx{T}.AsReadOnly"/> returns a read-only wrapper that has the same public
    /// type as the original collection. Attempting to modify the collection through such a
    /// read-only view will raise a <see cref="NotSupportedException"/>.
    /// </item><item>
    /// <see cref="ArrayEx{T}.Copy"/> creates a deep copy of the collection by invoking <see
    /// cref="ICloneable.Clone"/> on each element. This feature requires that all copied elements
    /// implement the <see cref="ICloneable"/> interface.
    /// </item><item>
    /// <see cref="ArrayEx{T}.CopyFrom"/> copies elements from a specified one-dimensional <see
    /// cref="Array"/> or <see cref="IEnumerable{T}"/> collection to the <b>ArrayEx</b> collection.
    /// </item><item>
    /// <see cref="ArrayEx{T}.Empty"/> returns an immutable empty collection that is cached for
    /// repeated access.
    /// </item><item>
    /// <see cref="ArrayEx{T}.Equals"/> compares two collections with identical element types for
    /// value equality of all elements. The collections compare as equal if they contain the same
    /// elements in the same order.
    /// </item><item>
    /// <see cref="ArrayEx{T}.GetValueOrDefault"/> returns the default value for <typeparamref
    /// name="T"/> rather than throwing an exception when a specified index is invalid.
    /// </item><item>
    /// <see cref="ArrayEx{T}.ToArrayWithShape"/> copies the entire <b>ArrayEx</b> to a standard
    /// multi-dimensional <see cref="Array"/> with the same dimension lengths.
    /// </item><item>
    /// Many utility methods replicate those defined by the standard <see cref="Array"/> class.
    /// While the standard methods only accept one-dimensional arrays, the <b>ArrayEx</b> methods
    /// work regardless of <see cref="ArrayEx{T}.Rank"/>.
    /// </item></list><para>
    /// <b>ArrayEx</b> implements the <see cref="IList{T}"/> and <see cref="IList"/> interfaces. As
    /// with the standard <see cref="Array"/> class, any members that attempt to add or remove
    /// elements are implemented explicitly and always throw a <see cref="NotSupportedException"/>.
    /// Other members are implemented implicitly.
    /// </para><para>
    /// Unfortunately, <b>ArrayEx</b> also has several significant drawbacks compared to the
    /// standard <see cref="Array"/> class:
    /// </para><list type="bullet"><item>
    /// The indices of all dimensions must be zero-based. Arbitrary lower bounds are not supported.
    /// </item><item>
    /// You cannot use C# 3 collection initializers to define the elements of an <b>ArrayEx</b> upon
    /// construction because there is no working <b>Add</b> method.
    /// </item><item>
    /// Accessing individual elements is slower by a factor of two or more. Use <see
    /// cref="ArrayEx{T}.GetValueOrDefault"/> where possible to avoid double-checking indices.
    /// </item></list></remarks>

    [Serializable]
    public class ArrayEx<T>: IList<T>, IList, ICloneable {
        #region ArrayEx(Int32[])

        /// <overloads>
        /// Initializes a new instance of the <see cref="ArrayEx{T}"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayEx{T}"/> class with the specified
        /// dimension lengths.</summary>
        /// <param name="lengths">
        /// An <see cref="Array"/> containing the number of elements in each dimension.</param>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="lengths"/> is a null reference or an empty <see cref="Array"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="lengths"/> contains one or more negative values.</exception>
        /// <remarks>
        /// If any element in the <paramref name="lengths"/> equals zero, the new <see
        /// cref="ArrayEx{T}"/> will contain zero elements.</remarks>

        public ArrayEx(params int[] lengths) {
            int count = CheckLengths(lengths);

            Lengths = new int[lengths.Length];
            Array.Copy(lengths, Lengths, Rank);
            InnerArray = new T[count];
        }

        #endregion
        #region ArrayEx(Array, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayEx{T}"/> class with the dimension
        /// lengths of the specified <see cref="Array"/>, and optionally copies its elements.
        /// </summary>
        /// <param name="array">
        /// The <see cref="Array"/> whose dimension lengths to copy.</param>
        /// <param name="copying">
        /// <c>true</c> to also copy all elements from the specified <paramref name="array"/>;
        /// <c>false</c> to leave the elements of the new <see cref="ArrayEx{T}"/> at their default
        /// values.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <exception cref="InvalidCastException">
        /// <paramref name="copying"/> is <c>true</c>, and at least one element in <paramref
        /// name="array"/> cannot be cast to <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// The specified <paramref name="array"/> may have non-zero lower bounds in one or more
        /// dimensions. These are ignored, and the new <see cref="ArrayEx{T}"/> will contain the
        /// same number of elements starting from a zero-based index in each dimension.</remarks>

        public ArrayEx(Array array, bool copying) {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException("array");

            Lengths = new int[array.Rank];
            for (int i = 0; i < array.Rank; i++)
                Lengths[i] = array.GetLength(i);

            InnerArray = new T[array.Length];
            if (copying) {
                int i = 0;
                foreach (T item in array)
                    InnerArray[i++] = (T) item;
            }
        }

        #endregion
        #region ArrayEx(ArrayEx<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayEx{T}"/> class that has the same 
        /// dimension lengths and contains elements copied from the specified instance.</summary>
        /// <param name="array">
        /// The <see cref="ArrayEx{T}"/> collection whose elements are copied to the new collection.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <remarks>
        /// This constructor also copies all dimension lengths of the specified <paramref
        /// name="array"/>, but not the value of the <see cref="IsReadOnly"/> property.</remarks>

        public ArrayEx(ArrayEx<T> array) {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException("array");

            Lengths = new int[array.Rank];
            Array.Copy(array.Lengths, Lengths, array.Rank);

            InnerArray = new T[array.Length];
            Array.Copy(array.InnerArray, InnerArray, array.Length);
        }

        #endregion
        #region ArrayEx(ArrayEx<T>, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayEx{T}"/> class that is a read-only
        /// view of the specified instance.</summary>
        /// <param name="array">
        /// The <see cref="ArrayEx{T}"/> collection that provides the initial values for the <see
        /// cref="InnerArray"/> and <see cref="Lengths"/> fields.</param>
        /// <param name="readOnly">
        /// The initial value for the <see cref="IsReadOnly"/> property. This argument must be
        /// <c>true</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="readOnly"/> is <c>false</c>.</exception>
        /// <remarks>
        /// This constructor is used to create a read-only wrapper around an existing collection.
        /// The new instance shares the data of the specified <paramref name="array"/>.</remarks>

        protected ArrayEx(ArrayEx<T> array, bool readOnly) {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException("array");

            if (!readOnly)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "readOnly", Strings.ArgumentEquals, false);

            Lengths = array.Lengths;
            InnerArray = array.InnerArray;

            ReadOnlyFlag = true;
            ReadOnlyWrapper = this;
        }

        #endregion
        #region Protected Fields

        /// <summary>
        /// The one-dimensional <see cref="Array"/> that holds the <typeparamref name="T"/> elements
        /// of the <see cref="ArrayEx{T}"/>.</summary>

        protected readonly T[] InnerArray;

        /// <summary>
        /// The number of elements in all dimensions of the <see cref="ArrayEx{T}"/>.</summary>

        protected readonly int[] Lengths;

        /// <summary>
        /// Backs the <see cref="IsReadOnly"/> property.</summary>

        protected readonly bool ReadOnlyFlag;

        /// <summary>
        /// The read-only <see cref="ArrayEx{T}"/> collection that is returned by the <see
        /// cref="AsReadOnly"/> method.</summary>

        protected ArrayEx<T> ReadOnlyWrapper;

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="ArrayEx{T}"/>.</summary>
        /// <remarks>
        /// <b>Empty</b> is a read-only one-dimensional <see cref="ArrayEx{T}"/> whose only
        /// dimension length equals zero. <b>Empty</b> is therefore guaranteed to never change.
        /// </remarks>

        public static readonly ArrayEx<T> Empty = new ArrayEx<T>(0).AsReadOnly();

        #endregion
        #region Public Properties
        #region Count

        /// <summary>
        /// Gets the total number of elements in all dimensions of the <see cref="ArrayEx{T}"/>.
        /// </summary>
        /// <value>
        /// The total number of elements in all dimensions of the <see cref="ArrayEx{T}"/>.</value>
        /// <remarks>
        /// <b>Count</b> returns the value of the <see cref="Length"/> property.</remarks>

        public int Count {
            [DebuggerStepThrough]
            get { return InnerArray.Length; }
        }

        #endregion
        #region IsFixedSize

        /// <summary>
        /// Gets a value indicating whether the <see cref="ArrayEx{T}"/> has a fixed size.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="ArrayEx{T}"/> has a fixed size; otherwise, <c>false</c>.
        /// The default is <c>true</c>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="IList.IsFixedSize"/> for details.
        /// </para><para>
        /// This property always returns <c>true</c> since <see cref="ArrayEx{T}"/> instances always
        /// have a fixed size, even when they are not read-only.</para></remarks>

        public bool IsFixedSize {
            [DebuggerStepThrough]
            get { return true; }
        }

        #endregion
        #region IsReadOnly

        /// <summary>
        /// Gets a value indicating whether the <see cref="ArrayEx{T}"/> is read-only.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="ArrayEx{T}"/> is read-only; otherwise, <c>false</c>. The
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
        /// Gets a value indicating whether access to the <see cref="ArrayEx{T}"/> is synchronized
        /// (thread-safe).</summary>
        /// <value>
        /// <c>true</c> if access to the <see cref="ArrayEx{T}"/> is synchronized (thread-safe);
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
        #region Item[Int32]

        /// <overloads>
        /// Gets or sets the element at the specified index in the <see cref="ArrayEx{T}"/>.
        /// </overloads>
        /// <summary>
        /// Gets or sets the element at the specified one-dimensional index.</summary>
        /// <param name="index">
        /// The zero-based one-dimensional index of the element to get or set.</param>
        /// <value>
        /// The element at the specified <paramref name="index"/>.</value>
        /// <exception cref="IndexOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Length"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="IList{T}.this"/> for details. One-dimensional indexing is
        /// always possible, regardless of the <see cref="Rank"/> of the <see cref="ArrayEx{T}"/>.
        /// </remarks>

        public T this[int index] {
            [DebuggerStepThrough]
            get { return GetValue(index); }
            [DebuggerStepThrough]
            set { SetValue(value, index); }
        }

        /// <summary>
        /// Gets or sets the element at the specified one-dimensional index.</summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set.</param>
        /// <value>
        /// The element at the specified <paramref name="index"/>. When the property is set, this
        /// value must be compatible with <typeparamref name="T"/>.</value>
        /// <exception cref="IndexOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </para></exception>
        /// <exception cref="InvalidCastException">
        /// The property is set to a value that is not compatible with <typeparamref name="T"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="ArrayEx{T}"/> is read-only.</exception>

        object IList.this[int index] {
            [DebuggerStepThrough]
            get { return GetValue(index); }
            [DebuggerStepThrough]
            set { SetValue((T) value, index); }
        }

        #endregion
        #region Item[Int32, Int32]

        /// <summary>
        /// Gets or sets the element at the specified two-dimensional index.</summary>
        /// <param name="i">
        /// The zero-based first-dimension index of the element to get or set.</param>
        /// <param name="j">
        /// The zero-based second-dimension index of the element to get or set.</param>
        /// <value>
        /// The element at the index position specified by <paramref name="i"/> and <paramref
        /// name="j"/>.</value>
        /// <exception cref="ArgumentException">
        /// <see cref="Rank"/> does not equal two.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="i"/> or <paramref name="j"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="i"/> or <paramref name="j"/> is equal to or greater than the number of
        /// elements in the corresponding dimension of the <see cref="ArrayEx{T}"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Getting this property calls <see cref="GetValue(Int32, Int32)"/>, and setting this
        /// property calls <see cref="SetValue(T, Int32, Int32)"/>.</remarks>

        public T this[int i, int j] {
            [DebuggerStepThrough]
            get { return GetValue(i, j); }
            [DebuggerStepThrough]
            set { SetValue(value, i, j); }
        }

        #endregion
        #region Item[Int32, Int32, Int32]

        /// <summary>
        /// Gets or sets the element at the specified three-dimensional index.</summary>
        /// <param name="i">
        /// The zero-based first-dimension index of the element to get or set.</param>
        /// <param name="j">
        /// The zero-based second-dimension index of the element to get or set.</param>
        /// <param name="k">
        /// The zero-based third-dimension index of the element to get or set.</param>
        /// <value>
        /// The element at the index position specified by <paramref name="i"/>, <paramref
        /// name="j"/>, and <paramref name="k"/>.</value>
        /// <exception cref="ArgumentException">
        /// <see cref="Rank"/> does not equal three.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="i"/>, <paramref name="j"/>, or <paramref name="k"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="i"/>, <paramref name="j"/>, or <paramref name="k"/> is equal to or
        /// greater than the number of elements in the corresponding dimension of the <see
        /// cref="ArrayEx{T}"/>.</para></exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Getting this property calls <see cref="GetValue(Int32, Int32, Int32)"/>, and setting
        /// this property calls <see cref="SetValue(T, Int32, Int32, Int32)"/>.</remarks>

        public T this[int i, int j, int k] {
            [DebuggerStepThrough]
            get { return GetValue(i, j, k); }
            [DebuggerStepThrough]
            set { SetValue(value, i, j, k); }
        }

        #endregion
        #region Item[Int32[]]

        /// <summary>
        /// Gets or sets the element at the specified multi-dimensional index.</summary>
        /// <param name="indices">
        /// An <see cref="Array"/> containing one zero-based index for each dimension of the <see
        /// cref="ArrayEx{T}"/>.</param>
        /// <value>
        /// The element at the specified <paramref name="indices"/>.</value>
        /// <exception cref="ArgumentException">
        /// The number of elements in <paramref name="indices"/> differs from <see cref="Rank"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indices"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="indices"/> contains a value that is less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="indices"/> contains a value that is equal to or greater than the number
        /// of elements in the corresponding dimension of the <see cref="ArrayEx{T}"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException">
        /// The property is set, and the <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Getting this property calls <see cref="GetValue(Int32[])"/>, and setting this property
        /// calls <see cref="SetValue(T, Int32[])"/>.</remarks>

        public T this[params int[] indices] {
            [DebuggerStepThrough]
            get { return GetValue(indices); }
            [DebuggerStepThrough]
            set { SetValue(value, indices); }
        }

        #endregion
        #region Length

        /// <summary>
        /// Gets the total number of elements in all dimensions of the <see cref="ArrayEx{T}"/>.
        /// </summary>
        /// <value>
        /// The total number of elements in all dimensions of the <see cref="ArrayEx{T}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="Array.Length"/> for details.</remarks>

        public int Length {
            [DebuggerStepThrough]
            get { return InnerArray.Length; }
        }

        #endregion
        #region Rank

        /// <summary>
        /// Gets the rank (number of dimensions) of the <see cref="ArrayEx{T}"/>.</summary>
        /// <value>
        /// The rank (number of dimensions) of the <see cref="ArrayEx{T}"/>.</value>
        /// <remarks>
        /// Please refer to <see cref="Array.Rank"/> for details.</remarks>

        public int Rank {
            [DebuggerStepThrough]
            get { return Lengths.Length; }
        }

        #endregion
        #region SyncRoot

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ArrayEx{T}"/>.
        /// </summary>
        /// <value>
        /// An object that can be used to synchronize access to the <see cref="ArrayEx{T}"/>.
        /// </value>
        /// <remarks><para>
        /// Please refer to <see cref="ICollection.SyncRoot"/> for details.
        /// </para><para>
        /// When synchronizing multi-threaded access to the <see cref="ArrayEx{T}"/>, obtain a lock
        /// on the <b>SyncRoot</b> object rather than the collection itself. A read-only view always
        /// returns the same <b>SyncRoot</b> object as the underlying writable collection.
        /// </para></remarks>

        public object SyncRoot {
            [DebuggerStepThrough]
            get { return InnerArray.SyncRoot; }
        }

        #endregion
        #endregion
        #region Protected Methods
        #region CheckLengths

        /// <summary>
        /// Checks that the specified dimension lengths are valid.</summary>
        /// <param name="lengths">
        /// An <see cref="Array"/> containing the number of elements in each dimension.</param>
        /// <returns>
        /// The total number of elements in all dimensions of the specified <paramref
        /// name="lengths"/>.</returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="lengths"/> is a null reference or an empty <see cref="Array"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="lengths"/> contains one or more negative values.</exception>
        /// <remarks>
        /// <b>CheckLengths</b> returns the product of all values in the specified <paramref
        /// name="lengths"/>, and therefore zero if any such value equals zero.</remarks>

        protected int CheckLengths(params int[] lengths) {
            if (lengths == null || lengths.Length == 0)
                ThrowHelper.ThrowArgumentNullOrEmptyException("lengths");

            int count = 1;

            foreach (int length in lengths) {
                if (length < 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(
                        "lengths", lengths, Strings.ArgumentContainsNegative);

                count *= length;
            }

            return count;
        }

        #endregion
        #endregion
        #region Public Methods
        #region Add

        /// <summary>
        /// Adds an element to the end of the <see cref="ArrayEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to add.</param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only or has a fixed size.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="ICollection{T}.Add"/> for details.
        /// </para><para>
        /// <b>Add</b> always throws a <see cref="NotSupportedException"/> since <see
        /// cref="ArrayEx{T}"/> instances always have a fixed size.</para></remarks>

        void ICollection<T>.Add(T item) {
            throw new NotSupportedException(Strings.CollectionFixedSize);
        }

        /// <summary>
        /// Adds an element to the end of the <see cref="ArrayEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to add. This argument must be compatible with <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ArrayEx{T}"/> index at which the <paramref name="item"/> has been added.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="item"/> is not compatible with <typeparamref name="T"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only or has a fixed size.</exception>
        /// <remarks>
        /// <b>Add</b> always throws a <see cref="NotSupportedException"/> since <see
        /// cref="ArrayEx{T}"/> instances always have a fixed size.</remarks>

        int IList.Add(object item) {
            throw new NotSupportedException(Strings.CollectionFixedSize);
        }

        #endregion
        #region AsReadOnly

        /// <summary>
        /// Returns a read-only view of the <see cref="ArrayEx{T}"/>.</summary>
        /// <returns>
        /// A read-only wrapper around the <see cref="ArrayEx{T}"/>.</returns>
        /// <remarks><para>
        /// Attempting to modify the read-only wrapper returned by <b>AsReadOnly</b> will raise a
        /// <see cref="NotSupportedException"/>. Note that the original collection may still change,
        /// and any such changes will be reflected in the read-only view.
        /// </para><para>
        /// <b>AsReadOnly</b> buffers the newly created read-only wrapper when the method is first
        /// called, and returns the buffered value on subsequent calls.</para></remarks>

        public ArrayEx<T> AsReadOnly() {

            if (ReadOnlyWrapper == null)
                ReadOnlyWrapper = new ArrayEx<T>(this, true);

            return ReadOnlyWrapper;
        }

        #endregion
        #region BinarySearch(T)

        /// <overloads>
        /// Uses a binary search algorithm to locate a specific element in the sorted <see
        /// cref="ArrayEx{T}"/> or a portion of it.</overloads>
        /// <summary>
        /// Searches the entire sorted <see cref="ArrayEx{T}"/> for the specified element using the
        /// default comparer, and returns the zero-based index of the element.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="ArrayEx{T}"/>,
        /// if <paramref name="item"/> is found; otherwise, a negative number, which is the bitwise
        /// complement of the index of the next element that is larger than <paramref name="item"/>
        /// or, if there is no larger element, the bitwise complement of <see cref="Length"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The default <see cref="Comparer{T}"/> cannot find a generic or non-generic <see
        /// cref="IComparer"/> implementation for <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.BinarySearch{T}(T[], T)"/> for details.</remarks>

        public int BinarySearch(T item) {
            return Array.BinarySearch<T>(InnerArray, item);
        }

        #endregion
        #region BinarySearch(T, IComparer<T>)

        /// <summary>
        /// Searches the entire sorted <see cref="ArrayEx{T}"/> for the specified element using the
        /// specified comparer, and returns the zero-based index of the element.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> to use when comparing elements, or a null reference to
        /// use the default <see cref="Comparer{T}"/> for <typeparamref name="T"/>.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="ArrayEx{T}"/>,
        /// if <paramref name="item"/> is found; otherwise, a negative number, which is the bitwise
        /// complement of the index of the next element that is larger than <paramref name="item"/>
        /// or, if there is no larger element, the bitwise complement of <see cref="Length"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer"/> is a null reference, and the default <see
        /// cref="Comparer{T}"/> cannot find a generic or non-generic <see cref="IComparer"/>
        /// implementation for <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.BinarySearch{T}(T[], T, IComparer{T})"/> for details.
        /// </remarks>

        public int BinarySearch(T item, IComparer<T> comparer) {
            return Array.BinarySearch<T>(InnerArray, item, comparer);
        }

        #endregion
        #region BinarySearch(Int32, Int32, T, IComparer<T>)

        /// <summary>
        /// Searches a subrange of the sorted <see cref="ArrayEx{T}"/> for the specified element
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
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="ArrayEx{T}"/>,
        /// if <paramref name="item"/> is found; otherwise, a negative number, which is the bitwise
        /// complement of the index of the next element that is larger than <paramref name="item"/>
        /// or, if there is no larger element, the bitwise complement of <see cref="Length"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        /// <see cref="ArrayEx{T}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer"/> is a null reference, and the default <see
        /// cref="Comparer{T}"/> cannot find a generic or non-generic <see cref="IComparer"/>
        /// implementation for <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.BinarySearch{T}(T[], Int32, Int32, T, IComparer{T})"/>
        /// for details.</remarks>

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) {
            return Array.BinarySearch<T>(InnerArray, index, count, item, comparer);
        }

        #endregion
        #region Clear

        /// <summary>
        /// Sets all elements in the <see cref="ArrayEx{T}"/> to their default values.</summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.Clear"/> for details.</remarks>

        public void Clear() {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            Array.Clear(InnerArray, 0, Length);
        }

        #endregion
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="ArrayEx{T}"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="ArrayEx{T}"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> preserves all dimension lengths of the <see cref="ArrayEx{T}"/>, but not
        /// the value of the <see cref="IsReadOnly"/> property.</remarks>

        public virtual object Clone() {
            return new ArrayEx<T>(this);
        }

        #endregion
        #region Contains

        /// <summary>
        /// Determines whether the <see cref="ArrayEx{T}"/> contains the specified element.
        /// </summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="item"/> is found in the <see cref="ArrayEx{T}"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Contains"/> for details.</remarks>

        public bool Contains(T item) {
            return (Array.IndexOf<T>(InnerArray, item) >= 0);
        }

        /// <summary>
        /// Determines whether the <see cref="ArrayEx{T}"/> contains the specified element.
        /// </summary>
        /// <param name="item">
        /// The element to locate. This argument must be compatible with <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="item"/> is found in the <see cref="ArrayEx{T}"/>;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="item"/> is not compatible with <typeparamref name="T"/>.</exception>

        bool IList.Contains(object item) {
            return Contains((T) item);
        }

        #endregion
        #region Copy

        /// <summary>
        /// Creates a deep copy of the <see cref="ArrayEx{T}"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="ArrayEx{T}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="T"/> does not implement <see cref="ICloneable"/>.</exception>
        /// <remarks><para>
        /// <b>Copy</b> is similar to <see cref="Clone"/> but creates a deep copy the <see
        /// cref="ArrayEx{T}"/> by invoking <see cref="ICloneable.Clone"/> on all elements.
        /// </para><para>
        /// <b>Copy</b> preserves all dimension lengths of the <see cref="ArrayEx{T}"/>, but not the
        /// value of the <see cref="IsReadOnly"/> property.</para></remarks>

        public ArrayEx<T> Copy() {
            ArrayEx<T> copy = new ArrayEx<T>(Lengths);

            for (int i = 0; i < InnerArray.Length; i++) {
                T item = InnerArray[i];

                ICloneable cloneable = (ICloneable) item;
                if (cloneable != null)
                    item = (T) cloneable.Clone();

                copy.InnerArray[i] = item;
            }

            return copy;
        }

        #endregion
        #region CopyFrom(IEnumerable<T>)

        /// <overloads>
        /// Copies the elements of a specified collection to the <see cref="ArrayEx{T}"/>.
        /// </overloads>
        /// <summary>
        /// Copies the elements of the specified collection to the <see cref="ArrayEx{T}"/>,
        /// starting at the beginning of the target array.</summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> collection whose elements are copied to the <see
        /// cref="ArrayEx{T}"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <exception cref="IndexOutOfRangeException">
        /// The number of elements in the source <paramref name="collection"/> is greater than the
        /// available space in the destination <see cref="ArrayEx{T}"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// <b>CopyFrom</b> performs an inverse <see cref="Array.CopyTo"/> operation.</remarks>

        public void CopyFrom(IEnumerable<T> collection) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            int i = 0;
            foreach (T item in collection)
                InnerArray[i++] = item;
        }

        #endregion
        #region CopyFrom(IEnumerable<T>, Int32)

        /// <summary>
        /// Copies the elements of the specified collection to the <see cref="ArrayEx{T}"/>,
        /// starting at the specified index of the target array.</summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> collection whose elements are copied to the <see
        /// cref="ArrayEx{T}"/>.</param>
        /// <param name="arrayIndex">
        /// The zero-based index in the <see cref="ArrayEx{T}"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <exception cref="IndexOutOfRangeException"><para>
        /// <paramref name="arrayIndex"/> is less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="arrayIndex"/> is equal to or greater than <see cref="Length"/>.
        /// </para><para>-or-</para><para>
        /// The number of elements in the source <paramref name="collection"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination <see
        /// cref="ArrayEx{T}"/>.</para></exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// <b>CopyFrom</b> performs an inverse <see cref="Array.CopyTo"/> operation.</remarks>

        public void CopyFrom(IEnumerable<T> collection, int arrayIndex) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            foreach (T item in collection)
                InnerArray[arrayIndex++] = item;
        }

        #endregion
        #region CopyFrom(T[])

        /// <summary>
        /// Copies a one-dimensional <see cref="Array"/> to the <see cref="ArrayEx{T}"/>, starting
        /// at the beginning of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the source of the elements copied to the
        /// <see cref="ArrayEx{T}"/>. The <b>Array</b> must have zero-based indexing.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <paramref name="array"/> is greater than the
        /// available space in the destination <see cref="ArrayEx{T}"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// <b>CopyFrom</b> performs an inverse <see cref="Array.CopyTo"/> operation.</remarks>

        public void CopyFrom(T[] array) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            array.CopyTo(InnerArray, 0);
        }

        #endregion
        #region CopyFrom(T[], Int32)

        /// <summary>
        /// Copies a one-dimensional <see cref="Array"/> to the <see cref="ArrayEx{T}"/>, starting
        /// at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the source of the elements copied to the
        /// <see cref="ArrayEx{T}"/>. The <b>Array</b> must have zero-based indexing.</param>
        /// <param name="arrayIndex">
        /// The zero-based index in the <see cref="ArrayEx{T}"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="ArgumentException"><para>
        /// <paramref name="arrayIndex"/> is equal to or greater than <see cref="Length"/>.
        /// </para><para>-or-</para><para>
        /// The number of elements in the source <paramref name="array"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination <see
        /// cref="ArrayEx{T}"/>.</para></exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// <b>CopyFrom</b> performs an inverse <see cref="Array.CopyTo"/> operation.</remarks>

        public void CopyFrom(T[] array, int arrayIndex) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            array.CopyTo(InnerArray, arrayIndex);
        }

        #endregion
        #region CopyTo(T[])

        /// <overloads>
        /// Copies the entire <see cref="ArrayEx{T}"/> to a one-dimensional <see cref="Array"/>.
        /// </overloads>
        /// <summary>
        /// Copies the entire <see cref="ArrayEx{T}"/> to a one-dimensional <see cref="Array"/>,
        /// starting at the beginning of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from the <see cref="ArrayEx{T}"/>. The <b>Array</b> must have zero-based indexing.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is a null reference.</exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="ArrayEx{T}"/> is greater than the
        /// available space in the destination <paramref name="array"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.CopyTo"/> for details.</remarks>

        public void CopyTo(T[] array) {
            InnerArray.CopyTo(array, 0);
        }

        #endregion
        #region CopyTo(T[], Int32)

        /// <summary>
        /// Copies the entire <see cref="ArrayEx{T}"/> to a one-dimensional <see cref="Array"/>,
        /// starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from the <see cref="ArrayEx{T}"/>. The <b>Array</b> must have zero-based indexing.
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
        /// The number of elements in the source <see cref="ArrayEx{T}"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="Array.CopyTo"/> for details.</remarks>

        public void CopyTo(T[] array, int arrayIndex) {
            InnerArray.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the entire <see cref="ArrayEx{T}"/> to a one-dimensional <see cref="Array"/>,
        /// starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        /// from the <see cref="ArrayEx{T}"/>. The <b>Array</b> must have zero-based indexing.
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
        /// The number of elements in the source <see cref="ArrayEx{T}"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.</para></exception>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="T"/> cannot be cast automatically to the type of the destination
        /// <paramref name="array"/>.</exception>

        void ICollection.CopyTo(Array array, int arrayIndex) {
            InnerArray.CopyTo(array, arrayIndex);
        }

        #endregion
        #region Equals

        /// <summary>
        /// Determines whether the specified collection contains the same elements in the same order
        /// as the current <see cref="ArrayEx{T}"/>.</summary>
        /// <param name="collection">
        /// The <see cref="ICollection{T}"/> of elements to compare with the current <see
        /// cref="ArrayEx{T}"/>.</param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="collection"/> is another reference to this <see cref="ArrayEx{T}"/>.
        /// </item><item>
        /// <paramref name="collection"/> is not a null reference, contains the same number of
        /// elements as this <see cref="ArrayEx{T}"/>, and all elements compare as equal when
        /// retrieved in the enumeration sequence for each collection.</item></list></returns>
        /// <remarks>
        /// <b>Equals</b> calls <see cref="CollectionsUtility.SequenceEqual"/> to test the two
        /// collections for value equality.</remarks>

        public bool Equals(ICollection<T> collection) {
            return InnerArray.SequenceEqual(collection);
        }

        #endregion
        #region GetEnumerator

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that can iterate through the <see
        /// cref="ArrayEx{T}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> for the entire <see cref="ArrayEx{T}"/>.</returns>
        /// <remarks>
        /// Please refer to <see cref="Array.GetEnumerator"/> for details.</remarks>

        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>) InnerArray).GetEnumerator();
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that can iterate through the <see
        /// cref="ArrayEx{T}"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> for the entire <see cref="ArrayEx{T}"/>.</returns>

        IEnumerator IEnumerable.GetEnumerator() {
            return InnerArray.GetEnumerator();
        }

        #endregion
        #region GetIndex(Int32)

        /// <overloads>
        /// Gets the one-dimensional index in the <see cref="ArrayEx{T}"/> that corresponds to the
        /// specified multi-dimensional index.</overloads>
        /// <summary>
        /// Checks that the specified one-dimensional index is valid for the <see
        /// cref="ArrayEx{T}"/>.</summary>
        /// <param name="index">
        /// The zero-based index to check.</param>
        /// <returns>
        /// The specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Length"/>.
        /// </para></exception>
        /// <remarks>
        /// One-dimensional indexing is always possible, regardless of the <see cref="Rank"/> of the
        /// <see cref="ArrayEx{T}"/>.</remarks>

        public int GetIndex(int index) {

            if (index < 0 || index >= Length)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "index", index, Strings.ArgumentLessOrGreater, 0, Length - 1);

            return index;
        }

        #endregion
        #region GetIndex(Int32, Int32)

        /// <summary>
        /// Gets the one-dimensional index in the <see cref="ArrayEx{T}"/> that corresponds to the
        /// specified two-dimensional index.</summary>
        /// <param name="i">
        /// The zero-based index in the first dimension.</param>
        /// <param name="j">
        /// The zero-based index in the second dimension.</param>
        /// <returns>
        /// The one-dimensional index in the <see cref="ArrayEx{T}"/> that corresponds to the index
        /// position specified by <paramref name="i"/> and <paramref name="j"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="i"/> or <paramref name="j"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="i"/> or <paramref name="j"/> is equal to or greater than the number of
        /// elements in the corresponding dimension of the <see cref="ArrayEx{T}"/>.
        /// </para></exception>
        /// <exception cref="PropertyValueException">
        /// <see cref="Rank"/> does not equal two.</exception>
        /// <remarks>
        /// <b>GetIndex</b> establishes the same index order as enumeration through a
        /// multi-dimensional standard <see cref="Array"/>, i.e. elements are adjacent in the second
        /// dimension.</remarks>

        public int GetIndex(int i, int j) {
            if (Rank != 2)
                ThrowHelper.ThrowPropertyValueExceptionWithFormat(
                    "Rank", Rank, Strings.PropertyNotValue, 2);

            int length0 = Lengths[0], length1 = Lengths[1];

            if (i < 0 || i >= length0)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "i", i, Strings.ArgumentLessOrGreater, 0, length0 - 1);

            if (j < 0 || j >= length1)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "j", j, Strings.ArgumentLessOrGreater, 0, length1 - 1);

            return i * length1 + j;
        }

        #endregion
        #region GetIndex(Int32, Int32, Int32)

        /// <summary>
        /// Gets the one-dimensional index in the <see cref="ArrayEx{T}"/> that corresponds to the
        /// specified three-dimensional index.</summary>
        /// <param name="i">
        /// The zero-based index in the first dimension.</param>
        /// <param name="j">
        /// The zero-based index in the second dimension.</param>
        /// <param name="k">
        /// The zero-based index in the third dimension.</param>
        /// <returns>
        /// The one-dimensional index in the <see cref="ArrayEx{T}"/> that corresponds to the index
        /// position specified by <paramref name="i"/>, <paramref name="j"/>, and <paramref
        /// name="k"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <see cref="Rank"/> does not equal two.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="i"/>, <paramref name="j"/>, or <paramref name="k"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="i"/>, <paramref name="j"/>, or <paramref name="k"/> is equal to or
        /// greater than the number of elements in the corresponding dimension of the <see
        /// cref="ArrayEx{T}"/>.</para></exception>
        /// <exception cref="PropertyValueException">
        /// <see cref="Rank"/> does not equal three.</exception>
        /// <remarks>
        /// <b>GetIndex</b> establishes the same index order as enumeration through a
        /// multi-dimensional standard <see cref="Array"/>, i.e. elements are adjacent in the third
        /// dimension, and the first-dimension index is the slowest to change.</remarks>

        public int GetIndex(int i, int j, int k) {
            if (Rank != 3)
                ThrowHelper.ThrowPropertyValueExceptionWithFormat(
                    "Rank", Rank, Strings.PropertyNotValue, 3);

            int length0 = Lengths[0], length1 = Lengths[1], length2 = Lengths[2];

            if (i < 0 || i >= length0)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "i", i, Strings.ArgumentLessOrGreater, 0, length0 - 1);

            if (j < 0 || j >= length1)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "j", j, Strings.ArgumentLessOrGreater, 0, length1 - 1);

            if (k < 0 || k >= length2)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "k", k, Strings.ArgumentLessOrGreater, 0, length2 - 1);

            return (i * length1 + j) * length2 + k;
        }

        #endregion
        #region GetIndex(Int32[])

        /// <summary>
        /// Gets the one-dimensional index in the <see cref="ArrayEx{T}"/> that corresponds to the
        /// specified multi-dimensional index.</summary>
        /// <param name="indices">
        /// An <see cref="Array"/> containing one zero-based index for each dimension of the <see
        /// cref="ArrayEx{T}"/>.</param>
        /// <returns>
        /// The one-dimensional index in the <see cref="ArrayEx{T}"/> that corresponds to the
        /// specified <paramref name="indices"/>.</returns>
        /// <exception cref="ArgumentException">
        /// The number of elements in <paramref name="indices"/> differs from <see cref="Rank"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indices"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="indices"/> contains a value that is less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="indices"/> contains a value that is equal to or greater than the number
        /// of elements in the corresponding dimension of the <see cref="ArrayEx{T}"/>.
        /// </para></exception>
        /// <remarks>
        /// <b>GetIndex</b> establishes the same index order as enumeration through a
        /// multi-dimensional standard <see cref="Array"/>, i.e. elements are adjacent in the
        /// highest dimension, and indices of higher dimensions change faster than those of lower
        /// dimensions.</remarks>

        public int GetIndex(params int[] indices) {
            if (indices == null)
                ThrowHelper.ThrowArgumentNullException("indices");

            if (indices.Length != Rank)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "indices", Strings.ArgumentPropertyConflict, "Rank");

            int index = 0;

            for (int i = 0; i < indices.Length; i++) {
                int currentIndex = indices[i];
                int currentLength = Lengths[i];

                if (currentIndex < 0 || currentIndex >= currentLength)
                    ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                        "indices", indices, Strings.ArgumentLessOrGreater, 0, currentLength - 1);

                index = index * currentLength + currentIndex;
            }

            return index;
        }

        #endregion
        #region GetIndices

        /// <summary>
        /// Gets the multi-dimensional index in the <see cref="ArrayEx{T}"/> that corresponds to the
        /// specified one-dimensional index.</summary>
        /// <param name="index">
        /// A one-dimensional index in the <see cref="ArrayEx{T}"/>.</param>
        /// <returns>
        /// An <see cref="Array"/> containing the zero-based indices for each dimension of the <see
        /// cref="ArrayEx{T}"/> that correspond to the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Length"/>.
        /// </para></exception>
        /// <remarks>
        /// <b>GetIndices</b> establishes the same index order as enumeration through a
        /// multi-dimensional standard <see cref="Array"/>, i.e. indices of higher dimensions change
        /// faster than those of lower dimensions.</remarks>

        public int[] GetIndices(int index) {
            if (index < 0 || index >= Length)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "index", index, Strings.ArgumentLessOrGreater, 0, Length - 1);

            int[] indices = new int[Lengths.Length];

            for (int i = Lengths.Length - 1; i >= 0; i--) {
                int oldIndex = index, currentLength = Lengths[i];
                index /= currentLength;
                indices[i] = oldIndex - index * currentLength;
            }

            return indices;
        }

        #endregion
        #region GetLength

        /// <summary>
        /// Gets the number of elements in the specified dimension of the <see cref="ArrayEx{T}"/>.
        /// </summary>
        /// <param name="dimension">
        /// The zero-based dimension of the <see cref="ArrayEx{T}"/> whose length to determine.
        /// </param>
        /// <returns>
        /// The number of elements in the specified <paramref name="dimension"/>.</returns>
        /// <exception cref="IndexOutOfRangeException"><para>
        /// <paramref name="dimension"/> is less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="dimension"/> is equal to or greater than <see cref="Rank"/>.
        /// </para></exception>
        /// <remarks>
        /// Please refer to <see cref="Array.GetLength"/> for details.</remarks>

        public int GetLength(int dimension) {
            return Lengths[dimension];
        }

        #endregion
        #region GetValue(Int32)

        /// <overloads>
        /// Gets the element at the specified index in the <see cref="ArrayEx{T}"/>.</overloads>
        /// <summary>
        /// Gets the element at the specified one-dimensional index.</summary>
        /// <param name="index">
        /// The zero-based one-dimensional index of the element to get.</param>
        /// <returns>
        /// The element at the specified <paramref name="index"/>.</returns>
        /// <exception cref="IndexOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Length"/>.
        /// </para></exception>
        /// <remarks>
        /// Please refer to <see cref="Array.GetValue(Int32)"/> for details, but note that
        /// one-dimensional indexing is always possible, regardless of the <see cref="Rank"/> of the
        /// <see cref="ArrayEx{T}"/>.</remarks>

        public T GetValue(int index) {
            return InnerArray[index];
        }

        #endregion
        #region GetValue(Int32, Int32)

        /// <summary>
        /// Gets the element at the specified two-dimensional index.</summary>
        /// <param name="i">
        /// The zero-based first-dimension index of the element to get.</param>
        /// <param name="j">
        /// The zero-based second-dimension index of the element to get.</param>
        /// <returns>
        /// The element at the index position specified by <paramref name="i"/> and <paramref
        /// name="j"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <see cref="Rank"/> does not equal two.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="i"/> or <paramref name="j"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="i"/> or <paramref name="j"/> is equal to or greater than the number of
        /// elements in the corresponding dimension of the <see cref="ArrayEx{T}"/>.
        /// </para></exception>
        /// <remarks>
        /// Please refer to <see cref="Array.GetValue(Int32, Int32)"/> for details. The indexing
        /// order relative to one-dimensional indexing is established by <see cref="GetIndex"/>.
        /// </remarks>

        public T GetValue(int i, int j) {
            int index = GetIndex(i, j);
            return InnerArray[index];
        }

        #endregion
        #region GetValue(Int32, Int32, Int32)

        /// <summary>
        /// Gets the element at the specified three-dimensional index.</summary>
        /// <param name="i">
        /// The zero-based first-dimension index of the element to get.</param>
        /// <param name="j">
        /// The zero-based second-dimension index of the element to get.</param>
        /// <param name="k">
        /// The zero-based third-dimension index of the element to get.</param>
        /// <returns>
        /// The element at the index position specified by <paramref name="i"/>, <paramref
        /// name="j"/>, and <paramref name="k"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <see cref="Rank"/> does not equal three.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="i"/>, <paramref name="j"/>, or <paramref name="k"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="i"/>, <paramref name="j"/>, or <paramref name="k"/> is equal to or
        /// greater than the number of elements in the corresponding dimension of the <see
        /// cref="ArrayEx{T}"/>.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="Array.GetValue(Int32, Int32, Int32)"/> for details. The
        /// indexing order relative to one-dimensional indexing is established by <see
        /// cref="GetIndex"/>.</remarks>

        public T GetValue(int i, int j, int k) {
            int index = GetIndex(i, j, k);
            return InnerArray[index];
        }

        #endregion
        #region GetValue(Int32[])

        /// <summary>
        /// Gets the element at the specified multi-dimensional index.</summary>
        /// <param name="indices">
        /// An <see cref="Array"/> containing one zero-based index for each dimension of the <see
        /// cref="ArrayEx{T}"/>.</param>
        /// <returns>
        /// The element at the specified <paramref name="indices"/>.</returns>
        /// <exception cref="ArgumentException">
        /// The number of elements in <paramref name="indices"/> differs from <see cref="Rank"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indices"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="indices"/> contains a value that is less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="indices"/> contains a value that is equal to or greater than the number
        /// of elements in the corresponding dimension of the <see cref="ArrayEx{T}"/>.
        /// </para></exception>
        /// <remarks>
        /// Please refer to <see cref="Array.GetValue(Int32[])"/> for details. The indexing order
        /// relative to one-dimensional indexing is established by <see cref="GetIndex"/>.</remarks>

        public T GetValue(params int[] indices) {
            int index = GetIndex(indices);
            return InnerArray[index];
        }

        #endregion
        #region GetValueOrDefault(Int32)

        /// <overloads>
        /// Gets the element at the specified index in the <see cref="ArrayEx{T}"/>, or the default
        /// value for <typeparamref name="T"/> if not found.</overloads>
        /// <summary>
        /// Gets the element at the specified one-dimensional index, or the default value for
        /// <typeparamref name="T"/> if not found.</summary>
        /// <param name="index">
        /// The zero-based one-dimensional index of the element to get.</param>
        /// <returns><para>
        /// The element at the specified <paramref name="index"/>.
        /// </para><para>-or-</para><para>
        /// The default value for <typeparamref name="T"/> if the specified <paramref name="index"/>
        /// is less than zero, or equal to or greater than <see cref="Length"/>.</para></returns>
        /// <remarks>
        /// <b>GetValueOrDefault</b> is equivalent to <see cref="GetValue(Int32)"/> but never throws
        /// an <see cref="IndexOutOfRangeException"/> for an invalid <paramref name="index"/>.
        /// </remarks>

        public T GetValueOrDefault(int index) {
            if (index < 0 || index >= Length)
                return default(T);

            return InnerArray[index];
        }

        #endregion
        #region GetValueOrDefault(Int32, Int32)

        /// <summary>
        /// Gets the element at the specified two-dimensional index, or the default value for
        /// <typeparamref name="T"/> if not found.</summary>
        /// <param name="i">
        /// The zero-based first-dimension index of the element to get.</param>
        /// <param name="j">
        /// The zero-based second-dimension index of the element to get.</param>
        /// <returns><para>
        /// The element at the index position specified by <paramref name="i"/> and <paramref
        /// name="j"/>.
        /// </para><para>-or-</para><para>
        /// The default value for <typeparamref name="T"/> if <paramref name="i"/> or <paramref
        /// name="j"/> is less than zero, or equal to or greater than the number of elements in the
        /// corresponding dimension of the <see cref="ArrayEx{T}"/>.</para></returns>
        /// <exception cref="ArgumentException">
        /// <see cref="Rank"/> does not equal two.</exception>
        /// <remarks>
        /// <b>GetValueOrDefault</b> is equivalent to <see cref="GetValue(Int32, Int32)"/> but never
        /// throws an <see cref="ArgumentOutOfRangeException"/> for an invalid <paramref name="i"/>
        /// or <paramref name="j"/> index.</remarks>

        public T GetValueOrDefault(int i, int j) {
            if (Rank != 2)
                ThrowHelper.ThrowPropertyValueExceptionWithFormat(
                    "Rank", Rank, Strings.PropertyNotValue, 2);

            int length0 = Lengths[0], length1 = Lengths[1];

            if (i < 0 || i >= length0 || j < 0 || j >= length1)
                return default(T);

            int index = i * length1 + j;
            return InnerArray[index];
        }

        #endregion
        #region GetValueOrDefault(Int32, Int32, Int32)

        /// <summary>
        /// Gets the element at the specified three-dimensional index, or the default value for
        /// <typeparamref name="T"/> if not found.</summary>
        /// <param name="i">
        /// The zero-based first-dimension index of the element to get.</param>
        /// <param name="j">
        /// The zero-based second-dimension index of the element to get.</param>
        /// <param name="k">
        /// The zero-based third-dimension index of the element to get.</param>
        /// <returns><para>
        /// The element at the index position specified by <paramref name="i"/>, <paramref
        /// name="j"/>, and <paramref name="k"/>.
        /// </para><para>-or-</para><para>
        /// The default value for <typeparamref name="T"/> if <paramref name="i"/>, <paramref
        /// name="j"/>, or <paramref name="k"/> is less than zero, or equal to or greater than the
        /// number of elements in the corresponding dimension of the <see cref="ArrayEx{T}"/>.
        /// </para></returns>
        /// <exception cref="ArgumentException">
        /// <see cref="Rank"/> does not equal three.</exception>
        /// <remarks>
        /// <b>GetValueOrDefault</b> is equivalent to <see cref="GetValue(Int32, Int32, Int32)"/>
        /// but never throws an <see cref="ArgumentOutOfRangeException"/> for an invalid <paramref
        /// name="i"/>, <paramref name="j"/>, or <paramref name="k"/> index.</remarks>

        public T GetValueOrDefault(int i, int j, int k) {
            if (Rank != 3)
                ThrowHelper.ThrowPropertyValueExceptionWithFormat(
                    "Rank", Rank, Strings.PropertyNotValue, 3);

            int length0 = Lengths[0], length1 = Lengths[1], length2 = Lengths[2];

            if (i < 0 || i >= length0 || j < 0 || j >= length1 || k < 0 || k >= length2)
                return default(T);

            int index = (i * length1 + j) * length2 + k;
            return InnerArray[index];
        }

        #endregion
        #region GetValueOrDefault(Int32[])

        /// <summary>
        /// Gets the element at the specified multi-dimensional index, or the default value for
        /// <typeparamref name="T"/> if not found.</summary>
        /// <param name="indices">
        /// An <see cref="Array"/> containing one zero-based index for each dimension of the <see
        /// cref="ArrayEx{T}"/>.</param>
        /// <returns><para>
        /// The element at the specified <paramref name="indices"/>.
        /// </para><para>-or-</para><para>
        /// The default value for <typeparamref name="T"/> if <paramref name="indices"/> contains a
        /// value that is less than zero, or equal to or greater than the number of elements in the
        /// corresponding dimension of the <see cref="ArrayEx{T}"/>.</para></returns>
        /// <exception cref="ArgumentException">
        /// The number of elements in <paramref name="indices"/> differs from <see cref="Rank"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indices"/> is a null reference.</exception>
        /// <remarks>
        /// <b>GetValueOrDefault</b> is equivalent to <see cref="GetValue(Int32[])"/> but never
        /// throws an <see cref="ArgumentOutOfRangeException"/> for an invalid <paramref
        /// name="indices"/> element.</remarks>

        public T GetValueOrDefault(params int[] indices) {
            if (indices == null)
                ThrowHelper.ThrowArgumentNullException("indices");

            if (indices.Length != Rank)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "indices", Strings.ArgumentPropertyConflict, "Rank");

            int index = 0;

            for (int i = 0; i < indices.Length; i++) {
                int currentIndex = indices[i];
                int currentLength = Lengths[i];

                if (currentIndex < 0 || currentIndex >= currentLength)
                    return default(T);

                index = index * currentLength + currentIndex;
            }

            return InnerArray[index];
        }

        #endregion
        #region IndexOf(T)

        /// <overloads>
        /// Returns the zero-based index of the first occurrence of the specified element in the
        /// <see cref="ArrayEx{T}"/> or a portion of it.</overloads>
        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified element in the
        /// entire <see cref="ArrayEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> in the entire
        /// <see cref="ArrayEx{T}"/>, if found; otherwise, -1.</returns>
        /// <remarks>
        /// Please refer to <see cref="Array.IndexOf{T}(T[], T)"/> for details.</remarks>

        public int IndexOf(T item) {
            return Array.IndexOf<T>(InnerArray, item);
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified element in the
        /// <see cref="ArrayEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to locate. This argument must be compatible with <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> in the <see
        /// cref="ArrayEx{T}"/>, if found; otherwise, -1.</returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="item"/> is not compatible with <typeparamref name="T"/>.</exception>

        int IList.IndexOf(object item) {
            return IndexOf((T) item);
        }

        #endregion
        #region IndexOf(T, Int32)

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified element in the
        /// <see cref="ArrayEx{T}"/>, starting at the specified index.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <param name="index">
        /// The zero-based starting index of the search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> in the subrange
        /// of the <see cref="ArrayEx{T}"/> that starts at <paramref name="index"/>, if found;
        /// otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index for the <see cref="ArrayEx{T}"/>.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="Array.IndexOf{T}(T[], T, Int32)"/> for details.</remarks>

        public int IndexOf(T item, int index) {
            return Array.IndexOf<T>(InnerArray, item, index);
        }

        #endregion
        #region IndexOf(T, Int32, Int32)

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified element in a
        /// subrange of the <see cref="ArrayEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <param name="index">
        /// The zero-based starting index of the search.</param>
        /// <param name="count">
        /// The number of elements to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> in the subrange
        /// of the <see cref="ArrayEx{T}"/> that starts at <paramref name="index"/> and contains
        /// <paramref name="count"/> elements, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        /// <see cref="ArrayEx{T}"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.IndexOf{T}(T[], T, Int32, Int32)"/> for details.
        /// </remarks>

        public int IndexOf(T item, int index, int count) {
            return Array.IndexOf<T>(InnerArray, item, index, count);
        }

        #endregion
        #region Insert

        /// <summary>
        /// Inserts an element into the <see cref="ArrayEx{T}"/> at the specified index.</summary>
        /// <param name="index">
        /// The zero-based index at which to insert <paramref name="item"/>.</param>
        /// <param name="item">
        /// The element to insert.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than <see cref="Count"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only or has a fixed size.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="IList{T}.Insert"/> for details.
        /// </para><para>
        /// <b>Insert</b> always throws a <see cref="NotSupportedException"/> since <see
        /// cref="ArrayEx{T}"/> instances always have a fixed size.</para></remarks>

        void IList<T>.Insert(int index, T item) {
            throw new NotSupportedException(Strings.CollectionFixedSize);
        }

        /// <summary>
        /// Inserts an element into the <see cref="ArrayEx{T}"/> at the specified index.</summary>
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
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only or has a fixed size.</exception>
        /// <remarks>
        /// <b>Add</b> always throws a <see cref="NotSupportedException"/> since <see
        /// cref="ArrayEx{T}"/> instances always have a fixed size.</remarks>

        void IList.Insert(int index, object item) {
            throw new NotSupportedException(Strings.CollectionFixedSize);
        }

        #endregion
        #region LastIndexOf(T)

        /// <overloads>
        /// Returns the zero-based index of the last occurrence of the specified element in the <see
        /// cref="ArrayEx{T}"/> or a portion of it.</overloads>
        /// <summary>
        /// Returns the zero-based index of the last occurrence of the specified element in the
        /// entire <see cref="ArrayEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> in the entire
        /// <see cref="ArrayEx{T}"/>, if found; otherwise, -1.</returns>
        /// <remarks>
        /// Please refer to <see cref="Array.LastIndexOf{T}(T[], T)"/> for details.</remarks>

        public int LastIndexOf(T item) {
            return Array.LastIndexOf<T>(InnerArray, item);
        }

        #endregion
        #region LastIndexOf(T, Int32)

        /// <summary>
        /// Returns the zero-based index of the last occurrence of the specified element in the <see
        /// cref="ArrayEx{T}"/>, ending at the specified index.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <param name="index">
        /// The zero-based starting index of the backward search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> in the subrange
        /// of the <see cref="ArrayEx{T}"/> that ends at <paramref name="index"/>, if found;
        /// otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index for the <see cref="ArrayEx{T}"/>.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="Array.LastIndexOf{T}(T[], T, Int32)"/> for details.</remarks>

        public int LastIndexOf(T item, int index) {
            return Array.LastIndexOf<T>(InnerArray, item, index);
        }

        #endregion
        #region LastIndexOf(T, Int32, Int32)

        /// <summary>
        /// Returns the zero-based index of the last occurrence of the specified element in a
        /// subrange of the <see cref="ArrayEx{T}"/>.</summary>
        /// <param name="item">
        /// The element to locate.</param>
        /// <param name="index">
        /// The zero-based starting index of the backward search.</param>
        /// <param name="count">
        /// The number of elements to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> in the subrange
        /// of the <see cref="ArrayEx{T}"/> that contains <paramref name="count"/> elements and ends
        /// at <paramref name="index"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        /// <see cref="ArrayEx{T}"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.LastIndexOf{T}(T[], T, Int32, Int32)"/> for details.
        /// </remarks>

        public int LastIndexOf(T item, int index, int count) {
            return Array.LastIndexOf<T>(InnerArray, item, index, count);
        }

        #endregion
        #region Remove

        /// <summary>
        /// Removes the first occurrence of the specified element from the <see cref="ArrayEx{T}"/>.
        /// </summary>
        /// <param name="item">
        /// The element to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="item"/> was found and removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only or has a fixed size.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="ICollection{T}.Remove"/> for details.
        /// </para><para>
        /// <b>Remove</b> always throws a <see cref="NotSupportedException"/> since <see
        /// cref="ArrayEx{T}"/> instances always have a fixed size.</para></remarks>

        bool ICollection<T>.Remove(T item) {
            throw new NotSupportedException(Strings.CollectionFixedSize);
        }

        /// <summary>
        /// Removes the first occurrence of the specified element from the <see cref="ArrayEx{T}"/>.
        /// </summary>
        /// <param name="item">
        /// The element to remove. This argument must be compatible with <typeparamref name="T"/>.
        /// </param>
        /// <exception cref="InvalidCastException">
        /// <paramref name="item"/> is not compatible with <typeparamref name="T"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only or has a fixed size.</exception>
        /// <remarks>
        /// <b>Remove</b> always throws a <see cref="NotSupportedException"/> since <see
        /// cref="ArrayEx{T}"/> instances always have a fixed size.</remarks>

        void IList.Remove(object item) {
            throw new NotSupportedException(Strings.CollectionFixedSize);
        }

        #endregion
        #region RemoveAt

        /// <summary>
        /// Removes the element at the specified index in the <see cref="ArrayEx{T}"/>.</summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only or has a fixed size.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="IList{T}.RemoveAt"/> for details.
        /// </para><para>
        /// <b>RemoveAt</b> always throws a <see cref="NotSupportedException"/> since <see
        /// cref="ArrayEx{T}"/> instances always have a fixed size.</para></remarks>

        void IList<T>.RemoveAt(int index) {
            throw new NotSupportedException(Strings.CollectionFixedSize);
        }


        /// <summary>
        /// Removes the element at the specified index in the <see cref="ArrayEx{T}"/>.</summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only or has a fixed size.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="IList{T}.RemoveAt"/> for details.
        /// </para><para>
        /// <b>RemoveAt</b> always throws a <see cref="NotSupportedException"/> since <see
        /// cref="ArrayEx{T}"/> instances always have a fixed size.</para></remarks>

        void IList.RemoveAt(int index) {
            throw new NotSupportedException(Strings.CollectionFixedSize);
        }

        #endregion
        #region Reverse()

        /// <overloads>
        /// Reverses the order of the elements in the <see cref="ArrayEx{T}"/> or a portion of it.
        /// </overloads>
        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="ArrayEx{T}"/>.</summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.Reverse(Array)"/> for details.</remarks>

        public void Reverse() {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            Array.Reverse(InnerArray);
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
        /// elements in the <see cref="ArrayEx{T}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.Reverse(Array, Int32, Int32)"/> for details.</remarks>

        public void Reverse(int index, int count) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            Array.Reverse(InnerArray, index, count);
        }

        #endregion
        #region SetValue(T, Int32)

        /// <overloads>
        /// Sets the element at the specified index in the <see cref="ArrayEx{T}"/>.</overloads>
        /// <summary>
        /// Sets the element at the specified one-dimensional index.</summary>
        /// <param name="value">
        /// The new value for the element at the specified <paramref name="index"/>.</param>
        /// <param name="index">
        /// The zero-based one-dimensional index of the element to set.</param>
        /// <exception cref="IndexOutOfRangeException"><para>
        /// <paramref name="index"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="index"/> is equal to or greater than <see cref="Length"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.SetValue(Object, Int32)"/> for details, but note that
        /// one-dimensional indexing is always possible, regardless of the <see cref="Rank"/> of the
        /// <see cref="ArrayEx{T}"/>.</remarks>

        public void SetValue(T value, int index) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            InnerArray[index] = value;
        }

        #endregion
        #region SetValue(T, Int32, Int32)

        /// <summary>
        /// Sets the element at the specified two-dimensional index.</summary>
        /// <param name="value">
        /// The new value for the element at the index position specified by <paramref name="i"/>
        /// and <paramref name="j"/>.</param>
        /// <param name="i">
        /// The zero-based first-dimension index of the element to set.</param>
        /// <param name="j">
        /// The zero-based second-dimension index of the element to set.</param>
        /// <exception cref="ArgumentException">
        /// <see cref="Rank"/> does not equal two.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="i"/> or <paramref name="j"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="i"/> or <paramref name="j"/> is equal to or greater than the number of
        /// elements in the corresponding dimension of the <see cref="ArrayEx{T}"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.SetValue(Object, Int32, Int32)"/> for details. The
        /// indexing order relative to one-dimensional indexing is established by <see
        /// cref="GetIndex"/>.</remarks>

        public void SetValue(T value, int i, int j) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            int index = GetIndex(i, j);
            InnerArray[index] = value;
        }

        #endregion
        #region SetValue(T, Int32, Int32, Int32)

        /// <summary>
        /// Sets the element at the specified three-dimensional index.</summary>
        /// <param name="value">
        /// The new value for the element at the index position specified by <paramref name="i"/>,
        /// <paramref name="j"/>, and <paramref name="k"/>.</param>
        /// <param name="i">
        /// The zero-based first-dimension index of the element to set.</param>
        /// <param name="j">
        /// The zero-based second-dimension index of the element to set.</param>
        /// <param name="k">
        /// The zero-based third-dimension index of the element to set.</param>
        /// <exception cref="ArgumentException">
        /// <see cref="Rank"/> does not equal three.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="i"/>, <paramref name="j"/>, or <paramref name="k"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="i"/>, <paramref name="j"/>, or <paramref name="k"/> is equal to or
        /// greater than the number of elements in the corresponding dimension of the <see
        /// cref="ArrayEx{T}"/>.</para></exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.SetValue(Object, Int32, Int32, Int32)"/> for details.
        /// The indexing order relative to one-dimensional indexing is established by <see
        /// cref="GetIndex"/>.</remarks>

        public void SetValue(T value, int i, int j, int k) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            int index = GetIndex(i, j, k);
            InnerArray[index] = value;
        }

        #endregion
        #region SetValue(T, Int32[])

        /// <summary>
        /// Sets the element at the specified multi-dimensional index.</summary>
        /// <param name="value">
        /// The new value for the element at the specified <paramref name="indices"/>.</param>
        /// <param name="indices">
        /// An <see cref="Array"/> containing one zero-based index for each dimension of the <see
        /// cref="ArrayEx{T}"/>.</param>
        /// <exception cref="ArgumentException">
        /// The number of elements in <paramref name="indices"/> differs from <see cref="Rank"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indices"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="indices"/> contains a value that is less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="indices"/> contains a value that is equal to or greater than the number
        /// of elements in the corresponding dimension of the <see cref="ArrayEx{T}"/>.
        /// </para></exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.SetValue(Object, Int32[])"/> for details. The indexing
        /// order relative to one-dimensional indexing is established by <see cref="GetIndex"/>.
        /// </remarks>

        public void SetValue(T value, params int[] indices) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            int index = GetIndex(indices);
            InnerArray[index] = value;
        }

        #endregion
        #region Sort()

        /// <overloads>
        /// Sorts the <see cref="ArrayEx{T}"/> or a portion of it.</overloads>
        /// <summary>
        /// Sorts the entire <see cref="ArrayEx{T}"/> using the default comparer.</summary>
        /// <exception cref="InvalidOperationException">
        /// The default <see cref="Comparer{T}"/> cannot find a generic or non-generic <see
        /// cref="IComparer"/> implementation for <typeparamref name="T"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.Sort{T}(T[])"/> for details.</remarks>

        public void Sort() {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            Array.Sort<T>(InnerArray);
        }

        #endregion
        #region Sort(Comparison<T>)

        /// <summary>
        /// Sorts the entire <see cref="ArrayEx{T}"/> using the specified comparison method.
        /// </summary>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method to use when comparing elements.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="comparison"/> is a null reference.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.Sort{T}(T[], Comparison{T})"/> for details.</remarks>

        public void Sort(Comparison<T> comparison) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            Array.Sort<T>(InnerArray, comparison);
        }

        #endregion
        #region Sort(IComparer<T>)

        /// <summary>
        /// Sorts the entire <see cref="ArrayEx{T}"/> using the specified comparer.</summary>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> to use when comparing elements, or a null reference to
        /// use the default <see cref="Comparer{T}"/> for <typeparamref name="T"/>.</param>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer"/> is a null reference, and the default <see
        /// cref="Comparer{T}"/> cannot find a generic or non-generic <see cref="IComparer"/>
        /// implementation for <typeparamref name="T"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.Sort{T}(T[], IComparer{T})"/> for details.</remarks>

        public void Sort(IComparer<T> comparer) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            Array.Sort<T>(InnerArray, comparer);
        }

        #endregion
        #region Sort(Int32, Int32, IComparer<T>)

        /// <summary>
        /// Sorts a subrange of the <see cref="ArrayEx{T}"/> using the specified comparer.</summary>
        /// <param name="index">
        /// The zero-based starting index of the range to sort.</param>
        /// <param name="count">
        /// The length of the range to sort.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> to use when comparing elements, or a null reference to
        /// use the default <see cref="Comparer{T}"/> for <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the
        /// <see cref="ArrayEx{T}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> or <paramref name="count"/> is less than zero.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer"/> is a null reference, and the default <see
        /// cref="Comparer{T}"/> cannot find a generic or non-generic <see cref="IComparer"/>
        /// implementation for <typeparamref name="T"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayEx{T}"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.Sort{T}(T[], Int32, Int32, IComparer{T})"/> for
        /// details.</remarks>

        public void Sort(int index, int count, IComparer<T> comparer) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            Array.Sort<T>(InnerArray, index, count, comparer);
        }

        #endregion
        #region ToArray

        /// <summary>
        /// Copies the elements of the <see cref="ArrayEx{T}"/> to a new <see cref="Array"/>.
        /// </summary>
        /// <returns>
        /// A one-dimensional <see cref="Array"/> containing copies of the elements of the <see
        /// cref="ArrayEx{T}"/>.</returns>
        /// <remarks>
        /// Please refer to <see cref="List{T}.ToArray"/> for details.</remarks>

        public T[] ToArray() {
            T[] copy = new T[Length];
            Array.Copy(InnerArray, copy, Length);
            return copy;
        }

        #endregion
        #region ToArrayWithShape

        /// <summary>
        /// Copies the elements of the <see cref="ArrayEx{T}"/> to a new <see cref="Array"/> with
        /// the same dimension lengths.</summary>
        /// <returns>
        /// An <see cref="Array"/> with the same dimension lengths and containing copies of the
        /// elements of the <see cref="ArrayEx{T}"/>.</returns>
        /// <remarks>
        /// <b>ToArrayWithShape</b> returns an untyped <see cref="Array"/> object since we cannot
        /// specify the exact type of an <see cref="Array"/> of unknown rank. Clients should cast
        /// the return value to the desired target type.</remarks>

        public Array ToArrayWithShape() {
            Array copy = Array.CreateInstance(typeof(T), Lengths);

            for (int i = 0; i < Length; i++) {
                int[] indices = GetIndices(i);
                copy.SetValue(InnerArray[i], indices);
            }

            return copy;
        }

        #endregion
        #endregion
        #region Delegate Methods
        #region ConvertAll<TOutput>

        /// <summary>
        /// Converts the <see cref="ArrayEx{T}"/> to another element type.</summary>
        /// <typeparam name="TOutput">
        /// The type of all elements in the converted collection.</typeparam>
        /// <param name="converter">
        /// A <see cref="Converter{TInput, TOutput}"/> method that converts all elements from 
        /// <typeparamref name="T"/> to <typeparamref name="TOutput"/>.</param>
        /// <returns>
        /// A new one-dimensional <see cref="Array"/> containing the elements copied from the
        /// current collection and converted to <typeparamref name="TOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.ConvertAll{TInput, TOutput}"/> for details. Note that
        /// this method returns a standard <see cref="Array"/>, not a <see cref="ArrayEx{T}"/>.
        /// </remarks>

        public TOutput[] ConvertAll<TOutput>(Converter<T, TOutput> converter) {
            return Array.ConvertAll<T, TOutput>(InnerArray, converter);
        }

        #endregion
        #region Exists

        /// <summary>
        /// Determines whether the <see cref="ArrayEx{T}"/> contains elements that match the
        /// conditions defined by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="ArrayEx{T}"/> contains one or more elements that match the
        /// conditions defined by <paramref name="predicate"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.Exists{T}"/> for details.</remarks>

        public bool Exists(Predicate<T> predicate) {
            return Array.Exists(InnerArray, predicate);
        }

        #endregion
        #region Find

        /// <summary>
        /// Finds the first element in the <see cref="ArrayEx{T}"/> that matches the conditions
        /// defined by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The first element that matches the conditions defined by <paramref name="predicate"/>,
        /// if found; otherwise, the default value for <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.Find{T}"/> for details.</remarks>

        public T Find(Predicate<T> predicate) {
            return Array.Find(InnerArray, predicate);
        }

        #endregion
        #region FindAll

        /// <summary>
        /// Finds all elements in the <see cref="ArrayEx{T}"/> that match the conditions defined by
        /// the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// A new one-dimensional <see cref="Array"/> containing all elements that match the
        /// conditions defined by <paramref name="predicate"/>, if found; otherwise, an empty
        /// <b>Array</b>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.FindAll{T}"/> for details. Note that this method
        /// returns a standard <see cref="Array"/>, not a <see cref="ArrayEx{T}"/>.</remarks>

        public T[] FindAll(Predicate<T> predicate) {
            return Array.FindAll(InnerArray, predicate);
        }

        #endregion
        #region FindIndex(Predicate<T>)

        /// <overloads>
        /// Returns the zero-based index of the first element in the <see cref="ArrayEx{T}"/>, or a
        /// portion of it, that matches the conditions defined by the specified predicate.
        /// </overloads>
        /// <summary>
        /// Returns the zero-based index of the first element in the entire <see cref="ArrayEx{T}"/>
        /// that matches the conditions defined by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The zero-based index of the first element that matches the conditions defined by
        /// <paramref name="predicate"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.FindIndex{T}(T[], Predicate{T})"/> for details.
        /// </remarks>

        public int FindIndex(Predicate<T> predicate) {
            return Array.FindIndex(InnerArray, predicate);
        }

        #endregion
        #region FindIndex(Int32, Predicate<T>)

        /// <summary>
        /// Returns the zero-based index of the first element in the <see cref="ArrayEx{T}"/>,
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
        /// <paramref name="index"/> is not a valid index for the <see cref="ArrayEx{T}"/>.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="Array.FindIndex{T}(T[], Int32, Predicate{T})"/> for details.
        /// </remarks>

        public int FindIndex(int index, Predicate<T> predicate) {
            return Array.FindIndex(InnerArray, index, predicate);
        }

        #endregion
        #region FindIndex(Int32, Int32, Predicate<T>)

        /// <summary>
        /// Returns the zero-based index of the first element in a subrange of the <see
        /// cref="ArrayEx{T}"/> that matches the conditions defined by the specified predicate.
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
        /// <see cref="ArrayEx{T}"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.FindIndex{T}(T[], Int32, Int32, Predicate{T})"/> for
        /// details.</remarks>

        public int FindIndex(int index, int count, Predicate<T> predicate) {
            return Array.FindIndex(InnerArray, index, count, predicate);
        }

        #endregion
        #region FindLast

        /// <summary>
        /// Finds the last element in the <see cref="ArrayEx{T}"/> that matches the conditions
        /// defined by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The last element that matches the conditions defined by <paramref name="predicate"/>, if
        /// found; otherwise, the default value for <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.FindLast{T}"/> for details.</remarks>

        public T FindLast(Predicate<T> predicate) {
            return Array.FindLast(InnerArray, predicate);
        }

        #endregion
        #region FindLastIndex(Predicate<T>)

        /// <overloads>
        /// Returns the zero-based index of the last element in the <see cref="ArrayEx{T}"/>, or a
        /// portion of it, that matches the conditions defined by the specified predicate.
        /// </overloads>
        /// <summary>
        /// Returns the zero-based index of the last element in the entire <see cref="ArrayEx{T}"/>
        /// that matches the conditions defined by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// The zero-based index of the last element that matches the conditions defined by
        /// <paramref name="predicate"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.FindLastIndex{T}(T[], Predicate{T})"/> for details.
        /// </remarks>

        public int FindLastIndex(Predicate<T> predicate) {
            return Array.FindLastIndex(InnerArray, predicate);
        }

        #endregion
        #region FindLastIndex(Int32, Predicate<T>)

        /// <summary>
        /// Returns the zero-based index of the last element in the <see cref="ArrayEx{T}"/>, ending
        /// at the specified index, that matches the conditions defined by the specified predicate.
        /// </summary>
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
        /// <paramref name="index"/> is not a valid index for the <see cref="ArrayEx{T}"/>.
        /// </exception>
        /// <remarks>
        /// Please refer to <see cref="Array.FindLastIndex{T}(T[], Int32, Predicate{T})"/> for
        /// details.</remarks>

        public int FindLastIndex(int index, Predicate<T> predicate) {
            return Array.FindLastIndex(InnerArray, index, predicate);
        }

        #endregion
        #region FindLastIndex(Int32, Int32, Predicate<T>)

        /// <summary>
        /// Returns the zero-based index of the last element in a subrange of the <see
        /// cref="ArrayEx{T}"/> that matches the conditions defined by the specified predicate.
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
        /// <see cref="ArrayEx{T}"/>.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.FindLastIndex{T}(T[], Int32, Int32, Predicate{T})"/>
        /// for details.</remarks>

        public int FindLastIndex(int index, int count, Predicate<T> predicate) {
            return Array.FindLastIndex(InnerArray, index, count, predicate);
        }

        #endregion
        #region ForEach

        /// <summary>
        /// Performs the specified action on each element in the <see cref="ArrayEx{T}"/>.</summary>
        /// <param name="action">
        /// The <see cref="Action{T}"/> method to perform on each element.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.ForEach{T}"/> for details.</remarks>

        public void ForEach(Action<T> action) {
            Array.ForEach(InnerArray, action);
        }

        #endregion
        #region TrueForAll

        /// <summary>
        /// Determines whether all elements in the <see cref="ArrayEx{T}"/> match the conditions
        /// defined by the specified predicate.</summary>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> method that defines the search conditions.</param>
        /// <returns>
        /// <c>true</c> if all elements in the <see cref="ArrayEx{T}"/> match the conditions defined
        /// by <paramref name="predicate"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Array.TrueForAll{T}"/> for details.</remarks>

        public bool TrueForAll(Predicate<T> predicate) {
            return Array.TrueForAll(InnerArray, predicate);
        }

        #endregion
        #endregion
    }
}
