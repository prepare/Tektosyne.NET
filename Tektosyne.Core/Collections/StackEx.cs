using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides a generic last-in, first-out (LIFO) collection of items.</summary>
    /// <typeparam name="T">
    /// The type of all items in the stack. If <typeparamref name="T"/> is a reference type, items
    /// may be null references.</typeparam>
    /// <remarks><para>
    /// <b>StackEx</b> is identical with the standard class <see cref="Stack{T}"/> from which it
    /// derives, except for a few extra features:
    /// </para><list type="bullet"><item>
    /// <see cref="StackEx{T}.SyncRoot"/> is available as a public property, rather than an explicit
    /// <see cref="ICollection"/> implementation.
    /// </item><item>
    /// <see cref="StackEx{T}.Clone"/> creates a shallow copy of the stack, using the <see
    /// cref="StackEx{T}(IEnumerable{T})"/> constructor that takes a collection argument.
    /// </item><item>
    /// <see cref="StackEx{T}.Copy"/> creates a deep copy of the stack by invoking <see
    /// cref="ICloneable.Clone"/> on each <typeparamref name="T"/> element. This feature requires
    /// that all copied elements implement the <see cref="ICloneable"/> interface.
    /// </item><item>
    /// <see cref="StackEx{T}.Equals"/> and <see cref="StackEx{T}.EqualsReverse"/> compare two
    /// collections with identical element types for value equality of all elements. The collections
    /// compare as equal if they contain the same elements in the same or in reverse order, 
    /// respectively.</item></list></remarks>

    [Serializable]
    public class StackEx<T>: Stack<T>, ICloneable {
        #region StackEx<T>()

        /// <overloads>
        /// Initializes a new instance of the <see cref="StackEx{T}"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="StackEx{T}"/> class that is empty and has
        /// the default initial capacity.</summary>
        /// <remarks>
        /// Please refer to <see cref="Stack{T}()"/> for details.</remarks>

        public StackEx() : base() { }

        #endregion
        #region StackEx<T>(IEnumerable<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="StackEx{T}"/> class that contains elements
        /// copied from the specified collection and has sufficient capacity to accommodate the
        /// number of elements copied.</summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> collection whose elements are copied to the new
        /// collection.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Stack{T}(IEnumerable{T})"/> for details.</remarks>

        public StackEx(IEnumerable<T> collection) : base(collection) { }

        #endregion
        #region StackEx<T>(Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="StackEx{T}"/> class that is empty and has
        /// the specified initial capacity.</summary>
        /// <param name="capacity">
        /// The initial number of elements that the new <see cref="StackEx{T}"/> can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="Stack{T}(Int32)"/> for details.</remarks>

        public StackEx(int capacity) : base(capacity) { }

        #endregion
        #region SyncRoot

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="StackEx{T}"/>.
        /// </summary>
        /// <value>
        /// An object that can be used to synchronize access to the <see cref="StackEx{T}"/>.
        /// </value>
        /// <remarks><para>
        /// Please refer to <see cref="Stack.SyncRoot"/> for details.
        /// </para><para>
        /// When synchronizing multi-threaded access to the <see cref="StackEx{T}"/>, obtain a lock
        /// on the <b>SyncRoot</b> object rather than the collection itself. A read-only view always
        /// returns the same <b>SyncRoot</b> object as the underlying writable collection.
        /// </para></remarks>

        public object SyncRoot {
            [DebuggerStepThrough]
            get { return ((ICollection) this).SyncRoot; }
        }

        #endregion
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="StackEx{T}"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="StackEx{T}"/>.</returns>

        public virtual object Clone() {

            // reverse enumeration order
            T[] array = ToArray();
            Array.Reverse(array);

            return new StackEx<T>(array);
        }

        #endregion
        #region Copy

        /// <summary>
        /// Creates a deep copy of the <see cref="StackEx{T}"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="StackEx{T}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="T"/> does not implement <see cref="ICloneable"/>.</exception>
        /// <remarks>
        /// <b>Copy</b> is similar to <see cref="Clone"/> but creates a deep copy the <see
        /// cref="StackEx{T}"/> by invoking <see cref="ICloneable.Clone"/> on all <typeparamref
        /// name="T"/> elements.</remarks>

        public StackEx<T> Copy() {
            StackEx<T> copy = new StackEx<T>(Count);

            // reverse enumeration order
            T[] array = ToArray();
            Array.Reverse(array);

            foreach (T item in array) {
                ICloneable cloneable = (ICloneable) item;

                if (cloneable != null)
                    copy.Push((T) cloneable.Clone());
                else
                    copy.Push(item);
            }

            return copy;
        }

        #endregion
        #region Equals

        /// <summary>
        /// Determines whether the specified collection contains the same elements in the same order
        /// as the current <see cref="StackEx{T}"/>.</summary>
        /// <param name="collection">
        /// The <see cref="ICollection"/> to compare with the current <see cref="StackEx{T}"/>.
        /// </param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="collection"/> is another reference to this <see cref="StackEx{T}"/>.
        /// </item><item>
        /// <paramref name="collection"/> is not a null reference, contains the same number of
        /// elements as this <see cref="StackEx{T}"/>, and all elements compare as equal when
        /// retrieved in the enumeration sequence for each collection.</item></list></returns>
        /// <remarks><para>
        /// <b>Equals</b> calls <see cref="CollectionsUtility.SequenceEqualUntyped"/> to test the
        /// two collections for value equality.
        /// </para><note type="implementnotes">
        /// <para>The <see cref="StackEx{T}"/> is a last-in, first-out (LIFO) collection, which
        /// means that its enumeration sequence is reversed compared to a first-in, first-out (FIFO)
        /// collection that contains the same elements in the same original insertion order.
        /// </para><para>
        /// When comparing to a non-LIFO collection, you must first reverse its element order, or
        /// that of the <see cref="StackEx{T}"/>, for a successful comparison. Use the alternative
        /// method <see cref="EqualsReverse"/> to automatically perform this inversion.
        /// </para></note></remarks>

        public bool Equals(ICollection collection) {
            return CollectionsUtility.SequenceEqualUntyped(this, collection);
        }

        #endregion
        #region EqualsReverse

        /// <summary>
        /// Determines whether the specified collection contains the same elements in the reverse
        /// order as the current <see cref="StackEx{T}"/>.</summary>
        /// <param name="collection">
        /// The <see cref="ICollection"/> to compare with the current <see cref="StackEx{T}"/>.
        /// </param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="collection"/> is not a null reference, contains the same number of
        /// elements as this <see cref="StackEx{T}"/>, and all elements compare as equal when
        /// retrieved in the original enumeration sequence for the <paramref name="collection"/>,
        /// and in the reverse enumeration sequence for this <see cref="StackEx{T}"/>.
        /// </item></list></returns>
        /// <remarks><para>
        /// <b>Equals</b> calls <see cref="CollectionsUtility.SequenceEqualUntyped"/> to test the
        /// two collections for value equality.
        /// </para><note type="implementnotes">
        /// <para>The <see cref="StackEx{T}"/> is a last-in, first-out (LIFO) collection, which
        /// means that its enumeration sequence is reversed compared to a first-in, first-out (FIFO)
        /// collection that contains the same elements in the same original insertion order.
        /// </para><para>
        /// When comparing to a non-LIFO collection, you must first reverse its element order, or
        /// that of the <see cref="StackEx{T}"/>, for a successful comparison. <b>EqualsReverse</b>
        /// automatically performs this inversion. Use the alternative method <see cref="Equals"/>
        /// when comparing to another LIFO collection.</para></note></remarks>

        public bool EqualsReverse(ICollection collection) {

            // comparing against the StackEx itself fails since we reverse the order
            if (collection == null || collection == this || collection.Count != Count)
                return false;

            T[] array = ToArray();
            Array.Reverse(array);
            return CollectionsUtility.SequenceEqualUntyped(array, collection);
        }

        #endregion
    }
}
