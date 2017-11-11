using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides an unsorted generic collection of <see cref="Int32"/> elements.</summary>
    /// <remarks><para>
    /// <b>Int32HashSet</b> implements a dynamic hashtable optimized for <see cref="Int32"/> keys,
    /// just like <see cref="Int32Dictionary{TValue}"/>, but without support for value association.
    /// The functionality is similar to the standard <see cref="HashSet{T}"/> but more limited;
    /// specifically, <b>Int32HashSet</b> does not implement the <see cref="ISet{T}"/> interface.
    /// </para><para>
    /// <b>Int32HashSet</b> directly uses <see cref="Int32"/> keys as their own hash codes, and
    /// directly compares them to stored keys using a single machine instruction. The resulting
    /// search speedup compared to a standard <see cref="HashSet{T}"/> instantiated with <see
    /// cref="Int32"/> keys approaches 50% in an optimized x64 build under .NET 4.0.
    /// </para><para>
    /// <b>Int32HashSet</b> also contains the following extra features:
    /// </para><list type="bullet"><item>
    /// <see cref="Int32HashSet.AsReadOnly"/> returns a read-only wrapper that has the same public
    /// type as the original collection. Attempting to modify the collection through such a
    /// read-only view will raise a <see cref="NotSupportedException"/>.
    /// </item><item>
    /// <see cref="Int32HashSet.Empty"/> returns an immutable empty collection that is cached for
    /// repeated access.
    /// </item><item>
    /// <see cref="Int32HashSet.Equals"/> compares two collections with identical element types for
    /// value equality of all elements. The collections compare as equal if they contain the same
    /// elements. The enumeration order of elements is ignored since the <b>Int32HashSet</b> class
    /// does not establish any fixed element ordering.
    /// </item><item>
    /// <see cref="Int32HashSet.GetAny"/> returns an arbitrary element if the collection is not
    /// empty. This is equivalent to getting the first element yielded by an enumerator, but without
    /// actually creating the enumerator.
    /// </item></list><para>
    /// Moreover, several properties and methods that the standard class provides as explicit
    /// interface implementations have been elevated to public visibility.</para></remarks>

    [Serializable]
    public class Int32HashSet: ICollection<Int32>, ICollection, ICloneable {
        #region Int32HashSet()

        /// <overloads>
        /// Initializes a new instance of the <see cref="Int32HashSet"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="Int32HashSet"/> class that is empty and has
        /// the default initial capacity.</summary>
        /// <remarks>
        /// Please refer to <see cref="List{T}()"/> for details.</remarks>

        public Int32HashSet(): this(0) { }

        #endregion
        #region Int32HashSet(IEnumerable<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32HashSet"/> class that contains
        /// elements copied from the specified collection.</summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{Int32}"/> whose elements are copied to the new collection.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="collection"/> contains one or more duplicate elements.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(IEnumerable{T})"/> for details.</remarks>

        public Int32HashSet(IEnumerable<Int32> collection): this(0) {
            AddRange(collection);
        }

        #endregion
        #region Int32HashSet(Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32HashSet"/> class that is empty and has
        /// the specified initial capacity.</summary>
        /// <param name="capacity">
        /// The number of elements that the new <see cref="Int32HashSet"/> is initially capable of
        /// storing.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>

        public Int32HashSet(int capacity) {
            _data = new InstanceData(capacity);
        }

        #endregion
        #region Int32HashSet(Int32HashSet, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32HashSet"/> class that is a read-only
        /// view of the specified instance.</summary>
        /// <param name="collection">
        /// The <see cref="Int32HashSet"/> collection that is wrapped by the new instance.</param>
        /// <param name="readOnly">
        /// The initial value for the <see cref="IsReadOnly"/> property. This argument must be
        /// <c>true</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="readOnly"/> is <c>false</c>.</exception>
        /// <remarks>
        /// This constructor is used to create a read-only wrapper around an existing collection.
        /// The new instance shares the data of the specified <paramref name="collection"/>.
        /// </remarks>

        protected Int32HashSet(Int32HashSet collection, bool readOnly) {
            if (collection == null)
                ThrowHelper.ThrowArgumentNullException("collection");

            if (!readOnly)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "readOnly", Strings.ArgumentEquals, false);

            _data = collection._data;
            ReadOnlyFlag = readOnly;
            ReadOnlyWrapper = this;
        }

        #endregion
        #region Private Fields

        /// <summary>
        /// The <see cref="InstanceData"/> object that holds the <see cref="Int32"/> elements of the
        /// <see cref="Int32HashSet"/>.</summary>

        private readonly InstanceData _data;

        #endregion
        #region Protected Fields

        /// <summary>
        /// Backs the <see cref="IsReadOnly"/> property.</summary>

        protected readonly bool ReadOnlyFlag;

        /// <summary>
        /// The read-only <see cref="Int32HashSet"/> collection that is returned by the <see
        /// cref="AsReadOnly"/> method.</summary>

        protected Int32HashSet ReadOnlyWrapper;

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="Int32HashSet"/>.</summary>
        /// <remarks>
        /// Attempting to modify the <b>Empty</b> collection will raise a <see
        /// cref="NotSupportedException"/>. The collection has zero capacity and is guaranteed to
        /// never change, as there are no writable references to the collection.</remarks>

        public static readonly Int32HashSet Empty = new Int32HashSet(0).AsReadOnly();

        #endregion
        #region Public Properties
        #region Count

        /// <summary>
        /// Gets the number of elements contained in the <see cref="Int32HashSet"/>.</summary>
        /// <value>
        /// The number of <see cref="Int32"/> elements contained in the <see cref="Int32HashSet"/>.
        /// </value>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Count"/> for details.</remarks>

        public int Count {
            [DebuggerStepThrough]
            get { return (_data.EntryCount - _data.FreeCount); }
        }

        #endregion
        #region IsFixedSize

        /// <summary>
        /// Gets a value indicating whether the <see cref="Int32HashSet"/> has a fixed size.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="Int32HashSet"/> has a fixed size; otherwise, <c>false</c>.
        /// The default is <c>false</c>.</value>
        /// <remarks><para>
        /// Please refer to <see cref="IList.IsFixedSize"/> for details.
        /// </para><para>
        /// This property always returns the same value as the <see cref="IsReadOnly"/> property
        /// since any fixed-size <see cref="Int32HashSet"/> is also read-only, and vice versa.
        /// </para></remarks>

        public bool IsFixedSize {
            [DebuggerStepThrough]
            get { return ReadOnlyFlag; }
        }

        #endregion
        #region IsReadOnly

        /// <summary>
        /// Gets a value indicating whether the <see cref="Int32HashSet"/> is read-only.</summary>
        /// <value>
        /// <c>true</c> if the <see cref="Int32HashSet"/> is read-only; otherwise, <c>false</c>. The
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
        /// Gets a value indicating whether access to the <see cref="Int32HashSet"/> is synchronized
        /// (thread-safe).</summary>
        /// <value>
        /// <c>true</c> if access to the <see cref="Int32HashSet"/> is synchronized (thread-safe);
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
        #region SyncRoot

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="Int32HashSet"/>.
        /// </summary>
        /// <value>
        /// An object that can be used to synchronize access to the <see cref="Int32HashSet"/>.
        /// </value>
        /// <remarks><para>
        /// Please refer to <see cref="ICollection.SyncRoot"/> for details.
        /// </para><para>
        /// When synchronizing multi-threaded access to the <see cref="Int32HashSet"/>, obtain a
        /// lock on the <b>SyncRoot</b> object rather than the collection itself. A read-only view
        /// always returns the same <b>SyncRoot</b> object as the underlying writable collection.
        /// </para></remarks>

        public object SyncRoot {
            [DebuggerStepThrough]
            get { return _data; }
        }

        #endregion
        #endregion
        #region Protected Methods
        #region CheckTargetArray

        /// <summary>
        /// Checks the bounds of the specified array and the specified starting index against the
        /// size of the <see cref="Int32HashSet"/>.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination for elements copied from
        /// the <see cref="Int32HashSet"/>. The <b>Array</b> must have zero-based indexing.</param>
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
        /// The number of elements in the <see cref="Int32HashSet"/> is greater than the available
        /// space from <paramref name="arrayIndex"/> to the end of the destination <paramref
        /// name="array"/>.</para></exception>

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
        #region Add

        /// <summary>
        /// Adds the specified element to the <see cref="Int32HashSet"/>.</summary>
        /// <param name="item">
        /// The <see cref="Int32"/> element to add.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="item"/> already exists in the <see cref="Int32HashSet"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Int32HashSet"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Add"/> for details.</remarks>

        public void Add(int item) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            _data.Add(item);
        }

        #endregion
        #region AddRange

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="Int32HashSet"/>.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{Int32}"/> collection whose elements to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <exception cref="NotSupportedException"><para>
        /// The <see cref="Int32HashSet"/> is read-only.
        /// </para><para>-or-</para><para>
        /// The <see cref="Int32HashSet"/> already contains one or more elements in the specified
        /// <paramref name="collection"/>.
        /// </para><para>-or-</para><para>
        /// <paramref name="collection"/> contains one or more duplicate elements.
        /// </para></exception>
        /// <remarks>
        /// Please refer to <see cref="List{T}.AddRange"/> for details.</remarks>

        public void AddRange(IEnumerable<Int32> collection) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);
            if (collection == null)
                ThrowHelper.ThrowArgumentNullException("collection");

            foreach (int item in collection)
                Add(item);
        }

        #endregion
        #region AsReadOnly

        /// <summary>
        /// Returns a read-only view of the <see cref="Int32HashSet"/>.</summary>
        /// <returns>
        /// A read-only wrapper around the <see cref="Int32HashSet"/>.</returns>
        /// <remarks><para>
        /// Attempting to modify the read-only wrapper returned by <b>AsReadOnly</b> will raise a
        /// <see cref="NotSupportedException"/>. Note that the original collection may still change,
        /// and any such changes will be reflected in the read-only view.
        /// </para><para>
        /// <b>AsReadOnly</b> buffers the newly created read-only wrapper when the method is first
        /// called, and returns the buffered value on subsequent calls.</para></remarks>

        public Int32HashSet AsReadOnly() {

            if (ReadOnlyWrapper == null)
                ReadOnlyWrapper = new Int32HashSet(this, true);

            return ReadOnlyWrapper;
        }

        #endregion
        #region Clear

        /// <summary>
        /// Removes all elements from the <see cref="Int32HashSet"/>.</summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Int32HashSet"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Clear"/> for details.</remarks>

        public void Clear() {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            _data.Clear();
        }

        #endregion
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="Int32HashSet"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="Int32HashSet"/>.</returns>
        /// <remarks>
        /// <b>Clone</b> does not preserve the enumeration order of the <see cref="Int32HashSet"/>,
        /// nor the values of the <see cref="IsFixedSize"/> and <see cref="IsReadOnly"/> properties.
        /// </remarks>

        public virtual object Clone() {
            return new Int32HashSet(this);
        }

        #endregion
        #region Contains

        /// <summary>
        /// Determines whether the <see cref="Int32HashSet"/> contains the specified element.
        /// </summary>
        /// <param name="item">
        /// The <see cref="Int32"/> element to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Int32HashSet"/> contains the specified <paramref
        /// name="item"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Contains"/> for details.</remarks>

        public bool Contains(int item) {
            return (_data.Find(item) >= 0);
        }

        #endregion
        #region CopyTo

        /// <summary>
        /// Copies the entire <see cref="Int32HashSet"/> to a one-dimensional <see cref="Array"/>,
        /// starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the <see
        /// cref="Int32"/> elements copied from the <see cref="Int32HashSet"/>. The <b>Array</b>
        /// must have zero-based indexing.</param>
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
        /// The number of elements in the source <see cref="Int32HashSet"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.</para></exception>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.CopyTo(T[], Int32)"/> for details.</remarks>

        public void CopyTo(int[] array, int arrayIndex) {
            CheckTargetArray(array, arrayIndex);
            foreach (int item in this)
                array[arrayIndex++] = item;
        }

        /// <summary>
        /// Copies the entire <see cref="Int32HashSet"/> to a one-dimensional <see cref="Array"/>,
        /// starting at the specified index of the target array.</summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the <see
        /// cref="Int32"/> elements copied from the <see cref="Int32HashSet"/>. The <b>Array</b>
        /// must have zero-based indexing.</param>
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
        /// The number of elements in the source <see cref="Int32HashSet"/> is greater than the
        /// available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.</para></exception>
        /// <exception cref="InvalidCastException">
        /// <see cref="Int32"/> cannot be cast automatically to the type of the destination
        /// <paramref name="array"/>.</exception>

        void ICollection.CopyTo(Array array, int arrayIndex) {
            CopyTo((int[]) array, arrayIndex);
        }

        #endregion
        #region Equals

        /// <summary>
        /// Determines whether the specified collection contains the same elements as the current
        /// <see cref="Int32HashSet"/>.</summary>
        /// <param name="collection">
        /// The <see cref="ICollection{T}"/> of <see cref="Int32"/> elements to compare with the
        /// current <see cref="Int32HashSet"/>.</param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="collection"/> is another reference to this <see cref="Int32HashSet"/>.
        /// </item><item>
        /// <paramref name="collection"/> is not a null reference and contains exactly the same 
        /// elements as this <see cref="Int32HashSet"/>.</item></list></returns>
        /// <remarks><para>
        /// <b>Equals</b> iterates over the specified <paramref name="collection"/> and calls <see
        /// cref="Contains"/> for each element to test the two collections for value equality.
        /// </para><para>
        /// <b>Equals</b> does not attempt to compare the enumeration order of both collections as
        /// the <see cref="Int32HashSet"/> class does not define a fixed enumeration order.
        /// </para></remarks>

        public bool Equals(ICollection<Int32> collection) {

            if (collection == this) return true;
            if (collection == null || collection.Count != Count)
                return false;

            foreach (int item in collection)
                if (!Contains(item)) return false;

            return true;
        }

        #endregion
        #region GetAny

        /// <summary>
        /// Returns an arbitrary element in the <see cref="Int32HashSet"/>.</summary>
        /// <returns>
        /// An arbitrary <see cref="Int32"/> element found in the <see cref="Int32HashSet"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Int32HashSet"/> is empty.</exception>
        /// <remarks>
        /// <b>GetAny</b> returns the first element found while traversing the <see
        /// cref="Int32HashSet"/> in enumeration order, but without actually creating an <see
        /// cref="IEnumerator{T}"/> instance.</remarks>

        public int GetAny() {

            for (int i = 0; i < _data.EntryCount; i++)
                if (_data.Entries[i].IsValid)
                    return _data.Entries[i].Key;

            ThrowHelper.ThrowInvalidOperationException(Strings.CollectionEmpty);
            return 0;
        }

        #endregion
        #region GetEnumerator

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that can iterate through the <see
        /// cref="Int32HashSet"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}"/> for the entire <see cref="Int32HashSet"/>. Each
        /// enumerated item is an <see cref="Int32"/> value.</returns>
        /// <remarks>
        /// Please refer to <see cref="List{T}.GetEnumerator"/> for details.</remarks>

        public IEnumerator<Int32> GetEnumerator() {
            for (int i = 0; i < _data.EntryCount; i++)
                if (_data.Entries[i].IsValid)
                    yield return _data.Entries[i].Key;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> that can iterate through the <see
        /// cref="Int32HashSet"/>.</summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> for the entire <see cref="Int32HashSet"/>. Each enumerated
        /// item is an <see cref="Int32"/> value.</returns>

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
        #region Remove

        /// <summary>
        /// Removes the specified element from the <see cref="Int32HashSet"/>.</summary>
        /// <param name="item">
        /// The <see cref="Int32"/> element to remove.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="item"/> was found and the associated element was removed;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Int32HashSet"/> is read-only.</exception>
        /// <remarks>
        /// Please refer to <see cref="ICollection{T}.Remove"/> for details.</remarks>

        public bool Remove(int item) {
            if (ReadOnlyFlag)
                ThrowHelper.ThrowNotSupportedException(Strings.CollectionReadOnly);

            return _data.Remove(item);
        }

        #endregion
        #region ToArray

        /// <summary>
        /// Copies the key-and-value pairs of the <see cref="Int32HashSet"/> to a new <see
        /// cref="Array"/>.</summary>
        /// <returns>
        /// A one-dimensional <see cref="Array"/> containing copies of the <see cref="Int32"/>
        /// elements of the <see cref="Int32HashSet"/>.</returns>
        /// <remarks>
        /// <b>ToArray</b> has the same effect as <see cref="CopyTo"/> with a starting index of
        /// zero, but also allocates the target array.</remarks>

        public int[] ToArray() {
            int[] array = new int[Count];

            int i = 0;
            foreach (int item in this)
                array[i++] = item;

            return array;
        }

        #endregion
        #endregion
        #region Struct Entry

        /// <summary>
        /// Represents one element in the <see cref="Int32HashSet"/>.</summary>
        /// <remarks>
        /// <b>Entry</b> is a simple data container for one <see cref="Int32"/> element in the <see
        /// cref="Int32HashSet"/>, along with auxiliary data for storage management.</remarks>

        [Serializable, StructLayout(LayoutKind.Auto)]
        private struct Entry {
            #region IsValid

            /// <summary>
            /// <c>true</c> if the <see cref="Entry"/> contains an existing element; <c>false</c> if
            /// the <see cref="Entry"/> is available for storing a new element. The default is
            /// <c>false</c>.</summary>>

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
            /// The <see cref="Int32"/> element stored in the <see cref="Entry"/>.</summary>>
            /// <remarks>
            /// <b>Key</b> returns zero if <see cref="IsValid"/> is <c>false</c>.</remarks>

            internal int Key;

            #endregion
        }

        #endregion
        #region Class InstanceData

        /// <summary>
        /// Contains the elements of the <see cref="Int32HashSet"/>.</summary>
        /// <remarks>
        /// <b>InstanceData</b> provides and maintains the actual hashtable used by the <see
        /// cref="Int32HashSet"/> class.  When a read-only view is created, it shares the
        /// <b>InstanceData</b> of the original <see cref="Int32HashSet"/>. This allows the
        /// read-only view to reflect all changes to the original instance.</remarks>

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
            /// elements and/or storage for new elements.</summary>>
            /// <remarks>
            /// Any <b>Entries</b> element that is available for a new element has an <see
            /// cref="Entry.IsValid"/> flag which is <c>false</c>.</remarks>

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
            /// Adds the specified element to the <see cref="InstanceData"/>.</summary>
            /// <param name="item">
            /// The <see cref="Int32"/> element to add.</param>
            /// <exception cref="ArgumentException">
            /// <paramref name="item"/> already exists in the <see cref="InstanceData"/>.
            /// </exception>

            internal void Add(int item) {

                int hashCode = item & 0x7fffffff;
                int bucket = hashCode % Buckets.Length;

                for (int i = Buckets[bucket]; i >= 0; i = Entries[i].Next) {
                    Debug.Assert(Entries[i].IsValid);
                    if (item == Entries[i].Key)
                        ThrowHelper.ThrowArgumentException("item", Strings.ArgumentInCollection);
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
                Entries[index].Key = item;

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
            /// Finds the specified element within the <see cref="InstanceData"/>.</summary>
            /// <param name="item">
            /// The <see cref="Int32"/> element to locate.</param>
            /// <returns>
            /// The index of the <see cref="Entries"/> element whose <see cref="Entry.Key"/> equals
            /// <paramref name="item"/>, if found; otherwise, -1.</returns>

            internal int Find(int item) {
                if (Buckets == null) return -1;

                int hashCode = item & 0x7fffffff;
                int bucket = hashCode % Buckets.Length;

                for (int i = Buckets[bucket]; i >= 0; i = Entries[i].Next) {
                    Debug.Assert(Entries[i].IsValid);
                    if (item == Entries[i].Key) return i;
                }

                return -1;
            }

            #endregion
            #region GetPrime

            /// <summary>
            /// Gets the smallest prime number that is equal to or greater than the specified value.
            /// </summary>
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
            /// Removes the specified element from the <see cref="InstanceData"/>.</summary>
            /// <param name="item">
            /// The <see cref="Int32"/> element to remove.</param>
            /// <returns>
            /// <c>true</c> if <paramref name="item"/> was found and removed; otherwise,
            /// <c>false</c>.</returns>

            internal bool Remove(int item) {
                if (Buckets == null) return false;

                int hashCode = item & 0x7fffffff;
                int bucket = hashCode % Buckets.Length, previous = -1;

                for (int i = Buckets[bucket]; i >= 0; i = Entries[i].Next) {
                    Debug.Assert(Entries[i].IsValid);

                    if (item == Entries[i].Key) {
                        if (previous < 0)
                            Buckets[bucket] = Entries[i].Next;
                        else
                            Entries[previous].Next = Entries[i].Next;

                        Entries[i].IsValid = false;
                        Entries[i].Next = FreeList;
                        Entries[i].Key = 0;

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
