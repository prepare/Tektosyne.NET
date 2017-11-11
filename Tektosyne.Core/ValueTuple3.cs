using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

using Tektosyne.Collections;

namespace Tektosyne {

    /// <summary>
    /// Provides an immutable tuple of three arbitrary objects.</summary>
    /// <typeparam name="T1">
    /// The type of the first component.</typeparam>
    /// <typeparam name="T2">
    /// The type of the second component.</typeparam>
    /// <typeparam name="T3">
    /// The type of the third component.</typeparam>
    /// <remarks>
    /// <b>ValueTuple</b> resembles a standard <see cref="Tuple{T1,T2,T3}"/> but is implemented as an
    /// immutable structure with read-only fields, not as a class with properties.</remarks>

    [Serializable, StructLayout(LayoutKind.Auto)]
    public struct ValueTuple<T1, T2, T3>: IEquatable<ValueTuple<T1, T2, T3>> {
        #region ValueTuple(T1, T2, T3)

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTuple{T1,T2,T3}"/> structure.</summary>
        /// <param name="item1">
        /// The first component of the <see cref="ValueTuple{T1,T2,T3}"/>.</param>
        /// <param name="item2">
        /// The second component of the <see cref="ValueTuple{T1,T2,T3}"/>.</param>
        /// <param name="item3">
        /// The third component of the <see cref="ValueTuple{T1,T2,T3}"/>.</param>

        public ValueTuple(T1 item1, T2 item2, T3 item3) {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="ValueTuple{T1,T2,T3}"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="ValueTuple{T1,T2,T3}"/> instance that was created with
        /// the default constructor.</remarks>

        public static readonly ValueTuple<T1, T2, T3> Empty = new ValueTuple<T1, T2, T3>();

        #endregion
        #region Item1

        /// <summary>
        /// The first component of the <see cref="ValueTuple{T1,T2,T3}"/>.</summary>

        public readonly T1 Item1;

        #endregion
        #region Item2

        /// <summary>
        /// The second component of the <see cref="ValueTuple{T1,T2,T3}"/>.</summary>

        public readonly T2 Item2;

        #endregion
        #region Item3

        /// <summary>
        /// The third component of the <see cref="ValueTuple{T1,T2,T3}"/>.</summary>

        public readonly T3 Item3;

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="ValueTuple{T1,T2,T3}"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the results of <see cref="EqualityComparer{T}.GetHashCode"/>
        /// for the <see cref="Item1"/>, <see cref="Item2"/>, and <see cref="Item3"/> properties.
        /// </remarks>

        public override int GetHashCode() {
            unchecked {
                return ComparerCache<T1>.EqualityComparer.GetHashCode(Item1)
                    ^ ComparerCache<T2>.EqualityComparer.GetHashCode(Item2)
                    ^ ComparerCache<T3>.EqualityComparer.GetHashCode(Item3);
            }
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="ValueTuple{T1,T2,T3}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> containing the culture-invariant string representations of the
        /// <see cref="Item1"/>, <see cref="Item2"/>, and <see cref="Item3"/> properties.</returns>

        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2})",
                StringUtility.Validate<T1>(Item1),
                StringUtility.Validate<T2>(Item2),
                StringUtility.Validate<T3>(Item3));
        }

        #endregion
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="ValueTuple{T1,T2,T3}"/> instances have the same value.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="ValueTuple{T1,T2,T3}"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="ValueTuple{T1,T2,T3}"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(ValueTuple{T1,T2,T3})"/> method to test the
        /// two <see cref="ValueTuple{T1,T2,T3}"/> instances for value equality.</remarks>

        public static bool operator ==(ValueTuple<T1, T2, T3> x, ValueTuple<T1, T2, T3> y) {
            return x.Equals(y);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="ValueTuple{T1,T2,T3}"/> instances have different
        /// values.</summary>
        /// <param name="x">
        /// The first <see cref="ValueTuple{T1,T2,T3}"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="ValueTuple{T1,T2,T3}"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is different from the value of
        /// <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(ValueTuple{T1,T2,T3})"/> method to test the
        /// two <see cref="ValueTuple{T1,T2,T3}"/> instances for value inequality.</remarks>

        public static bool operator !=(ValueTuple<T1, T2, T3> x, ValueTuple<T1, T2, T3> y) {
            return !x.Equals(y);
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="ValueTuple{T1,T2,T3}"/> instances have the same value.
        /// </overloads>
        /// <summary>
        /// Determines whether this <see cref="ValueTuple{T1,T2,T3}"/> instance and a specified
        /// object, which must be a <see cref="ValueTuple{T1,T2,T3}"/>, have the same value.
        /// </summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="ValueTuple{T1,T2,T3}"/> instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="ValueTuple{T1,T2,T3}"/>
        /// instance and its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="ValueTuple{T1,T2,T3}"/>
        /// instance, <b>Equals</b> invokes the strongly-typed <see cref="Equals(ValueTuple{T1,T2,
        /// T3})"/> overload to test the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is ValueTuple<T1, T2, T3>))
                return false;

            return Equals((ValueTuple<T1, T2, T3>) obj);
        }

        #endregion
        #region Equals(ValueTuple<T1, T2, T3>)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="ValueTuple{T1,T2,T3}"/> have
        /// the same value.</summary>
        /// <param name="tuple">
        /// A <see cref="ValueTuple{T1,T2,T3}"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="tuple"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="Item1"/>, <see cref="Item2"/>, and
        /// <see cref="Item3"/> properties of the two <see cref="ValueTuple{T1,T2,T3}"/> instances to
        /// test for value equality.</remarks>

        public bool Equals(ValueTuple<T1, T2, T3> tuple) {

            return (ComparerCache<T1>.EqualityComparer.Equals(Item1, tuple.Item1)
                && ComparerCache<T2>.EqualityComparer.Equals(Item2, tuple.Item2)
                && ComparerCache<T3>.EqualityComparer.Equals(Item3, tuple.Item3));
        }

        #endregion
        #region Equals(ValueTuple<T1, T2, T3>, ValueTuple<T1, T2, T3>)

        /// <summary>
        /// Determines whether two specified <see cref="ValueTuple{T1,T2,T3}"/> instances have the
        /// same value.</summary>
        /// <param name="x">
        /// The first <see cref="ValueTuple{T1,T2,T3}"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="ValueTuple{T1,T2,T3}"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(ValueTuple{T1,T2,T3})"/> overload
        /// to test the two <see cref="ValueTuple{T1,T2,T3}"/> instances for value equality.
        /// </remarks>

        public static bool Equals(ValueTuple<T1, T2, T3> x, ValueTuple<T1, T2, T3> y) {
            return x.Equals(y);
        }

        #endregion
        #endregion
    }
}
