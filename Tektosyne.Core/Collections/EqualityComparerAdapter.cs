using System;
using System.Collections;
using System.Collections.Generic;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides an adapter class that wraps an <see cref="IEqualityComparer{T}.Equals"/> method in
    /// the <see cref="IEqualityComparer"/> and <see cref="IEqualityComparer{T}"/> interfaces.
    /// </summary>
    /// <typeparam name="T">
    /// The type of all objects to compare.</typeparam>
    /// <remarks><para>
    /// <b>EqualityComparerAdapter</b> wraps the <see cref="IEqualityComparer"/> and <see
    /// cref="IEqualityComparer{T}"/> interfaces around a specified <see
    /// cref="IEqualityComparer{T}.Equals"/> method, satisfying BCL classes and methods that expect
    /// such interface implementations.
    /// </para><para>
    /// <b>EqualityComparerAdapter</b> also accepts a custom <see
    /// cref="IEqualityComparer{T}.GetHashCode"/> method, and otherwise provides a safe default
    /// implementation which returns the constant value zero.</para></remarks>

    [Serializable]
    public class EqualityComparerAdapter<T>: IEqualityComparer<T>, IEqualityComparer {
        #region EqualityComparerAdapter()

        /// <overloads>
        /// Initializes a new instance of the <see cref="EqualityComparerAdapter{T}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualityComparerAdapter{T}"/> class with
        /// the default equality comparer for <typeparamref name="T"/> and the default hash
        /// function.</summary>

        public EqualityComparerAdapter(): this(null, null) { }

        #endregion
        #region EqualityComparerAdapter(Equals<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualityComparerAdapter{T}"/> class with
        /// the specified equality comparer for <typeparamref name="T"/> and the default hash
        /// function.</summary>
        /// <param name="comparison">
        /// The method to use when comparing <typeparamref name="T"/> instances for equality, or a
        /// null reference to use the default equality comparer for <typeparamref name="T"/>.
        /// </param>

        public EqualityComparerAdapter(Func<T, T, Boolean> comparison): this(comparison, null) { }

        #endregion
        #region EqualityComparerAdapter(Equals<T>, GetHashCode<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualityComparerAdapter{T}"/> class with
        /// the specified equality comparer and hash function for <typeparamref name="T"/>.
        /// </summary>
        /// <param name="comparison">
        /// The method to use when comparing <typeparamref name="T"/> instances for equality, or a
        /// null reference to use the default equality comparer for <typeparamref name="T"/>.
        /// </param>
        /// <param name="hashing">
        /// The method to use when computing the hash code for a <typeparamref name="T"/> instance,
        /// or a null reference to use the default hash function.</param>

        public EqualityComparerAdapter(Func<T, T, Boolean> comparison, Func<T, Int32> hashing) {

            Comparison = comparison ?? ComparerCache<T>.EqualityComparer.Equals;
            Hashing = hashing ?? (t => 0);
        }

        #endregion
        #region Comparison

        /// <summary>
        /// The method to use when comparing <typeparamref name="T"/> instances for equality.
        /// </summary>
        /// <remarks>
        /// <b>Comparison</b> never returns a null reference. The default is the default equality
        /// comparer for <typeparamref name="T"/>.</remarks>

        public readonly Func<T, T, Boolean> Comparison;

        #endregion
        #region Hashing

        /// <summary>
        /// The method to use when computing the hash code for a <typeparamref name="T"/> instance.
        /// </summary>
        /// <remarks>
        /// <b>Hashing</b> never returns a null reference. The default is a method that returns the
        /// constant value zero, ignoring the specified <typeparamref name="T"/> instance.</remarks>

        public readonly Func<T, Int32> Hashing;

        #endregion
        #region IEqualityComparer Members
        #region Equals(Object, Object)

        /// <overloads>
        /// Determines whether two specified objects are equal.</overloads>
        /// <summary>
        /// Determines whether two specified objects, which must be <typeparamref name="T"/>
        /// instances, are equal.</summary>
        /// <param name="x">
        /// The first object to compare.</param>
        /// <param name="y">
        /// The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="x"/> and <paramref name="y"/> are equal; otherwise,
        /// <c>false</c>.</returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="x"/> or <paramref name="y"/> cannot be cast to <typeparamref name="T"/>.
        /// </exception>
        /// <remarks>
        /// <b>Equals</b> returns the result of the <see cref="Comparison"/> method for the
        /// specified <paramref name="x"/> and <paramref name="y"/>.</remarks>

        public new bool Equals(object x, object y) {
            return Comparison((T) x, (T) y);
        }

        #endregion
        #region GetHashCode(Object)

        /// <overloads>
        /// Returns a hash code for the specified object.</overloads>
        /// <summary>
        /// Returns a hash code for the specified object, which must be a <typeparamref name="T"/>
        /// instance.</summary>
        /// <param name="obj">
        /// The object whose hash code to determine.</param>
        /// <returns>
        /// An <see cref="Int32"/> hash code for <paramref name="obj"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="obj"/> cannot be cast to <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// <b>GetHashCode</b> returns the result of the <see cref="Hashing"/> method for the
        /// specified <paramref name="obj"/>.</remarks>

        public int GetHashCode(object obj) {
            return Hashing((T) obj);
        }

        #endregion
        #endregion
        #region IEqualityComparer<T> Members
        #region Equals(T, T)

        /// <summary>
        /// Determines whether two specified <typeparamref name="T"/> instances are equal.</summary>
        /// <param name="x">
        /// The first <typeparamref name="T"/> instance to compare.</param>
        /// <param name="y">
        /// The second <typeparamref name="T"/> instance to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="x"/> and <paramref name="y"/> are equal; otherwise,
        /// <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> returns the result of the <see cref="Comparison"/> method for the
        /// specified <paramref name="x"/> and <paramref name="y"/>.</remarks>

        public bool Equals(T x, T y) {
            return Comparison(x, y);
        }

        #endregion
        #region GetHashCode(T)

        /// <summary>
        /// Returns a hash code for the specified <typeparamref name="T"/> instance.</summary>
        /// <param name="value">
        /// The <typeparamref name="T"/> instance whose hash code to determine.</param>
        /// <returns>
        /// An <see cref="Int32"/> hash code for <paramref name="value"/>.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> returns the result of the <see cref="Hashing"/> method for the
        /// specified <paramref name="value"/>.</remarks>
        
        public int GetHashCode(T value) {
            return Hashing(value);
        }

        #endregion
        #endregion
    }
}
