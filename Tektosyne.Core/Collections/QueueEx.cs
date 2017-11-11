using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides a generic first-in, first-out (FIFO) collection of items.</summary>
    /// <typeparam name="T">
    /// The type of all items in the queue. If <typeparamref name="T"/> is a reference type, items
    /// may be null references.</typeparam>
    /// <remarks><para>
    /// <b>QueueEx</b> is identical with the standard class <see cref="Queue{T}"/> from which it
    /// derives, except for a few extra features:
    /// </para><list type="bullet"><item>
    /// <see cref="QueueEx{T}.SyncRoot"/> is available as a public property, rather than an explicit
    /// <see cref="ICollection"/> implementation.
    /// </item><item>
    /// <see cref="QueueEx{T}.Clone"/> creates a shallow copy of the queue, using the <see
    /// cref="QueueEx{T}(IEnumerable{T})"/> constructor that takes a collection argument.
    /// </item><item>
    /// <see cref="QueueEx{T}.Copy"/> creates a deep copy of the queue by invoking <see
    /// cref="ICloneable.Clone"/> on each <typeparamref name="T"/> element. This feature requires
    /// that all copied elements implement the <see cref="ICloneable"/> interface.
    /// </item><item>
    /// <see cref="QueueEx{T}.Equals"/> compares two collections with identical element types for
    /// value equality of all elements. The collections compare as equal if they contain the same
    /// elements in the same order.</item></list></remarks>

    [Serializable]
    public class QueueEx<T>: Queue<T>, ICloneable {
        #region QueueEx<T>()

        /// <overloads>
        /// Initializes a new instance of the <see cref="QueueEx{T}"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueEx{T}"/> class that is empty and has
        /// the default initial capacity.</summary>
        /// <remarks>
        /// Please refer to <see cref="Queue{T}()"/> for details.</remarks>

        public QueueEx(): base() { }

        #endregion
        #region QueueEx<T>(IEnumerable<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueEx{T}"/> class that contains elements
        /// copied from the specified collection and has sufficient capacity to accommodate the
        /// number of elements copied.</summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable{T}"/> collection whose elements are copied to the new
        /// collection.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Queue{T}(IEnumerable{T})"/> for details.</remarks>

        public QueueEx(IEnumerable<T> collection): base(collection) { }

        #endregion
        #region QueueEx<T>(Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueEx{T}"/> class that is empty and has
        /// the specified initial capacity.</summary>
        /// <param name="capacity">
        /// The initial number of elements that the new <see cref="QueueEx{T}"/> can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>
        /// Please refer to <see cref="Queue{T}(Int32)"/> for details.</remarks>

        public QueueEx(int capacity): base(capacity) { }

        #endregion
        #region SyncRoot

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="QueueEx{T}"/>.
        /// </summary>
        /// <value>
        /// An object that can be used to synchronize access to the <see cref="QueueEx{T}"/>.
        /// </value>
        /// <remarks><para>
        /// Please refer to <see cref="Queue.SyncRoot"/> for details.
        /// </para><para>
        /// When synchronizing multi-threaded access to the <see cref="QueueEx{T}"/>, obtain a lock
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
        /// Creates a shallow copy of the <see cref="QueueEx{T}"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="QueueEx{T}"/>.</returns>

        public virtual object Clone() {
            return new QueueEx<T>(this);
        }

        #endregion
        #region Copy

        /// <summary>
        /// Creates a deep copy of the <see cref="QueueEx{T}"/>.</summary>
        /// <returns>
        /// A deep copy of the <see cref="QueueEx{T}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <typeparamref name="T"/> does not implement <see cref="ICloneable"/>.</exception>
        /// <remarks>
        /// <b>Copy</b> is similar to <see cref="Clone"/> but creates a deep copy the <see
        /// cref="QueueEx{T}"/> by invoking <see cref="ICloneable.Clone"/> on all <typeparamref
        /// name="T"/> elements.</remarks>

        public QueueEx<T> Copy() {
            QueueEx<T> copy = new QueueEx<T>(Count);

            foreach (T item in this) {
                ICloneable cloneable = (ICloneable) item;

                if (cloneable != null)
                    copy.Enqueue((T) cloneable.Clone());
                else
                    copy.Enqueue(item);
            }

            return copy;
        }

        #endregion
        #region Equals

        /// <summary>
        /// Determines whether the specified collection contains the same elements in the same order
        /// as the current <see cref="QueueEx{T}"/>.</summary>
        /// <param name="collection">
        /// The <see cref="ICollection"/> to compare with the current <see cref="QueueEx{T}"/>.
        /// </param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="collection"/> is another reference to this <see cref="QueueEx{T}"/>.
        /// </item><item>
        /// <paramref name="collection"/> is not a null reference, contains the same number of
        /// elements as this <see cref="QueueEx{T}"/>, and all elements compare as equal when
        /// retrieved in the enumeration sequence for each collection.</item></list></returns>
        /// <remarks>
        /// <b>Equals</b> calls <see cref="CollectionsUtility.SequenceEqualUntyped"/> to test the
        /// two collections for value equality.</remarks>

        public bool Equals(ICollection collection) {
            return CollectionsUtility.SequenceEqualUntyped(this, collection);
        }

        #endregion
    }
}
