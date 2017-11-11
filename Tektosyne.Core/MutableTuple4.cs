using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

using Tektosyne.Collections;

namespace Tektosyne {

    /// <summary>
    /// Provides a mutable tuple of four arbitrary objects.</summary>
    /// <typeparam name="T1">
    /// The type of the first component.</typeparam>
    /// <typeparam name="T2">
    /// The type of the second component.</typeparam>
    /// <typeparam name="T3">
    /// The type of the third component.</typeparam>
    /// <typeparam name="T4">
    /// The type of the fourth component.</typeparam>
    /// <remarks>
    /// <b>MutableTuple</b> resembles a standard <see cref="Tuple{T1,T2,T3,T4}"/> but provides
    /// public setters for all component properties. This allows using <b>MutableTuple</b> in
    /// scenarios such as two-way data binding using WPF controls.</remarks>

    [Serializable]
    public class MutableTuple<T1, T2, T3, T4>: IEquatable<MutableTuple<T1, T2, T3, T4>> {
        #region MutableTuple()

        /// <overloads>
        /// Initializes a new instance of the <see cref="MutableTuple{T1,T2,T3,T4}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTuple{T1,T2,T3,T4}"/> class with
        /// default properties.</summary>

        public MutableTuple() { }

        #endregion
        #region MutableTuple(T1, T2, T3, T4)

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableTuple{T1,T2,T3,T4}"/> class with the
        /// specified components.</summary>
        /// <param name="item1">
        /// The first component of the <see cref="MutableTuple{T1,T2,T3,T4}"/>.</param>
        /// <param name="item2">
        /// The second component of the <see cref="MutableTuple{T1,T2,T3,T4}"/>.</param>
        /// <param name="item3">
        /// The third component of the <see cref="MutableTuple{T1,T2,T3,T4}"/>.</param>
        /// <param name="item4">
        /// The fourth component of the <see cref="MutableTuple{T1,T2,T3,T4}"/>.</param>

        public MutableTuple(T1 item1, T2 item2, T3 item3, T4 item4) {
            _item1 = item1;
            _item2 = item2;
            _item3 = item3;
            _item4 = item4;
        }

        #endregion
        #region Private Fields

        // property backers
        private T1 _item1;
        private T2 _item2;
        private T3 _item3;
        private T4 _item4;

        #endregion
        #region Item1

        /// <summary>
        /// Gets or sets the first component of the <see cref="MutableTuple{T1,T2,T3,T4}"/>.
        /// </summary>
        /// <value>
        /// The first component of the <see cref="MutableTuple{T1,T2,T3,T4}"/>.</value>

        public T1 Item1 {
            [DebuggerStepThrough]
            get { return _item1; }
            [DebuggerStepThrough]
            set { _item1 = value; }
        }

        #endregion
        #region Item2

        /// <summary>
        /// Gets or sets the second component of the <see cref="MutableTuple{T1,T2,T3,T4}"/>.
        /// </summary>
        /// <value>
        /// The second component of the <see cref="MutableTuple{T1,T2,T3,T4}"/>.</value>

        public T2 Item2 {
            [DebuggerStepThrough]
            get { return _item2; }
            [DebuggerStepThrough]
            set { _item2 = value; }
        }

        #endregion
        #region Item3

        /// <summary>
        /// Gets or sets the third component of the <see cref="MutableTuple{T1,T2,T3,T4}"/>.
        /// </summary>
        /// <value>
        /// The third component of the <see cref="MutableTuple{T1,T2,T3,T4}"/>.</value>

        public T3 Item3 {
            [DebuggerStepThrough]
            get { return _item3; }
            [DebuggerStepThrough]
            set { _item3 = value; }
        }

        #endregion
        #region Item4

        /// <summary>
        /// Gets or sets the third component of the <see cref="MutableTuple{T1,T2,T3,T4}"/>.
        /// </summary>
        /// <value>
        /// The fourth component of the <see cref="MutableTuple{T1,T2,T3,T4}"/>.</value>

        public T4 Item4 {
            [DebuggerStepThrough]
            get { return _item4; }
            [DebuggerStepThrough]
            set { _item4 = value; }
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="MutableTuple{T1,T2,T3,T4}"/> instance.
        /// </summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the results of <see cref="EqualityComparer{T}.GetHashCode"/>
        /// for the <see cref="Item1"/>, <see cref="Item2"/>, <see cref="Item3"/>, and <see
        /// cref="Item4"/> properties.</remarks>

        public override int GetHashCode() {
            unchecked {
                return ComparerCache<T1>.EqualityComparer.GetHashCode(_item1)
                    ^ ComparerCache<T2>.EqualityComparer.GetHashCode(_item2)
                    ^ ComparerCache<T3>.EqualityComparer.GetHashCode(_item3)
                    ^ ComparerCache<T4>.EqualityComparer.GetHashCode(_item4);
            }
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see
        /// cref="MutableTuple{T1,T2,T3,T4}"/>.</summary>
        /// <returns>
        /// A <see cref="String"/> containing the culture-invariant string representations of the
        /// <see cref="Item1"/>, <see cref="Item2"/>, <see cref="Item3"/>, and <see cref="Item4"/>
        /// properties.</returns>

        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2}, {3})",
                StringUtility.Validate<T1>(_item1),
                StringUtility.Validate<T2>(_item2),
                StringUtility.Validate<T3>(_item3),
                StringUtility.Validate<T4>(_item4));
        }

        #endregion
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="MutableTuple{T1,T2,T3,T4}"/> instances have the same
        /// value.</summary>
        /// <param name="x">
        /// The first <see cref="MutableTuple{T1,T2,T3,T4}"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="MutableTuple{T1,T2,T3,T4}"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(MutableTuple{T1,T2,T3,T4},
        /// MutableTuple{T1,T2,T3,T4})"/> method to test the two <see
        /// cref="MutableTuple{T1,T2,T3,T4}"/> instances for value equality.</remarks>

        public static bool operator ==(
            MutableTuple<T1, T2, T3, T4> x, MutableTuple<T1, T2, T3, T4> y) {

            return Equals(x, y);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="MutableTuple{T1,T2,T3,T4}"/> instances have different
        /// values.</summary>
        /// <param name="x">
        /// The first <see cref="MutableTuple{T1,T2,T3,T4}"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="MutableTuple{T1,T2,T3,T4}"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is different from the value of
        /// <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(MutableTuple{T1,T2,T3,T4},
        /// MutableTuple{T1,T2,T3,T4})"/> method to test the two <see
        /// cref="MutableTuple{T1,T2,T3,T4}"/> instances for value inequality.</remarks>

        public static bool operator !=(
            MutableTuple<T1, T2, T3, T4> x, MutableTuple<T1, T2, T3, T4> y) {

            return !Equals(x, y);
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="MutableTuple{T1,T2,T3,T4}"/> instances have the same
        /// value.</overloads>
        /// <summary>
        /// Determines whether this <see cref="MutableTuple{T1,T2,T3,T4}"/> instance and a specified
        /// object, which must be a <see cref="MutableTuple{T1,T2,T3,T4}"/>, have the same value.
        /// </summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="MutableTuple{T1,T2,T3,T4}"/>
        /// instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="MutableTuple{T1,T2,T3,T4}"/>
        /// instance and its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see
        /// cref="MutableTuple{T1,T2,T3,T4}"/> instance, or an instance of a derived class,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(MutableTuple{T1,T2,T3,T4})"/>
        /// overload to test the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            return Equals(obj as MutableTuple<T1, T2, T3, T4>);
        }

        #endregion
        #region Equals(MutableTuple<T1, T2, T3, T4>)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="MutableTuple{T1,T2,T3,T4}"/>
        /// have the same value.</summary>
        /// <param name="tuple">
        /// A <see cref="MutableTuple{T1,T2,T3,T4}"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="tuple"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="Item1"/>, <see cref="Item2"/>, <see
        /// cref="Item3"/>, and <see cref="Item4"/> properties of the two <see
        /// cref="MutableTuple{T1,T2,T3,T4}"/> instances to test for value equality.</remarks>

        public bool Equals(MutableTuple<T1, T2, T3, T4> tuple) {

            return (!Object.ReferenceEquals(tuple, null)
                && ComparerCache<T1>.EqualityComparer.Equals(_item1, tuple._item1)
                && ComparerCache<T2>.EqualityComparer.Equals(_item2, tuple._item2)
                && ComparerCache<T3>.EqualityComparer.Equals(_item3, tuple._item3)
                && ComparerCache<T4>.EqualityComparer.Equals(_item4, tuple._item4));
        }

        #endregion
        #region Equals(MutableTuple<T1, T2, T3, T4>, MutableTuple<T1, T2, T3, T4>)

        /// <summary>
        /// Determines whether two specified <see cref="MutableTuple{T1,T2,T3,T4}"/> instances have
        /// the same value.</summary>
        /// <param name="x">
        /// The first <see cref="MutableTuple{T1,T2,T3,T4}"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="MutableTuple{T1,T2,T3,T4}"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(MutableTuple{T1,T2,T3,T4})"/>
        /// overload to test the two <see cref="MutableTuple{T1,T2,T3,T4}"/> instances for value
        /// equality.</remarks>

        public static bool Equals(MutableTuple<T1, T2, T3, T4> x, MutableTuple<T1, T2, T3, T4> y) {

            if (Object.ReferenceEquals(x, null))
                return Object.ReferenceEquals(y, null);

            return x.Equals(y);
        }

        #endregion
        #endregion
    }
}
