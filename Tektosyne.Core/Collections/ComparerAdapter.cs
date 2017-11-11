using System;
using System.Collections;
using System.Collections.Generic;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides an adapter class that wraps a <see cref="System.Comparison{T}"/> method in the <see
    /// cref="IComparer"/> and <see cref="IComparer{T}"/> interfaces.</summary>
    /// <typeparam name="T">
    /// The type of all objects to compare.</typeparam>
    /// <remarks><para>
    /// The BCL offers two choices for establishing a custom sorting order: the functional way using
    /// the <see cref="System.Comparison{T}"/> delegate, and the object-oriented way using the <see
    /// cref="IComparer"/> and <see cref="IComparer{T}"/> interfaces.
    /// </para><para>
    /// Converting interfaces to delegates is simple, as <see cref="IComparer{T}"/> already defines
    /// a conforming <see cref="IComparer{T}.Compare"/> method. <b>ComparerAdapter</b> provides the
    /// opposite conversion by wrapping the <see cref="IComparer"/> and <see cref="IComparer{T}"/>
    /// interfaces around a specified <see cref="System.Comparison{T}"/> method.</para></remarks>

    [Serializable]
    public class ComparerAdapter<T>: IComparer<T>, IComparer {
        #region ComparerAdapter()

        /// <overloads>
        /// Initializes a new instance of the <see cref="ComparerAdapter{T}"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ComparerAdapter{T}"/> class with the
        /// default comparer for <typeparamref name="T"/>.</summary>

        public ComparerAdapter(): this(null) { }

        #endregion
        #region ComparerAdapter(Comparison<T>)

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparerAdapter{T}"/> class with the
        /// specified comparer for <typeparamref name="T"/>.</summary>
        /// <param name="comparison">
        /// The <see cref="System.Comparison{T}"/> method to use when comparing <typeparamref
        /// name="T"/> instances, or a null reference to use the default comparer for <typeparamref
        /// name="T"/>.</param>

        public ComparerAdapter(Comparison<T> comparison) {
            Comparison = comparison ?? ComparerCache<T>.Comparer.Compare;
        }

        #endregion
        #region Comparison

        /// <summary>
        /// The <see cref="System.Comparison{T}"/> method to use when comparing <typeparamref
        /// name="T"/> instances.</summary>
        /// <remarks>
        /// <b>Comparison</b> never returns a null reference. The default is the default comparer
        /// for <typeparamref name="T"/>.</remarks>

        public readonly Comparison<T> Comparison;

        #endregion
        #region IComparer Members

        /// <overloads>
        /// Compares two specified objects and returns an indication of their relative values.
        /// </overloads>
        /// <summary>
        /// Compares two specified objecs, which must be <typeparamref name="T"/> instances, and
        /// returns an indication of their relative values.</summary>
        /// <param name="x">
        /// The first object to compare.</param>
        /// <param name="y">
        /// The second object to compare.</param>
        /// <returns><para>
        /// An <see cref="Int32"/> value indicating the relative order of <paramref name="x"/> and
        /// <paramref name="y"/>, as follows:
        /// </para><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description><paramref name="x"/> is less than <paramref name="y"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description><paramref name="x"/> equals <paramref name="y"/>.</description>
        /// </item><item>
        /// <term>Greater than zero</term>
        /// <description><paramref name="x"/> is greater than <paramref name="y"/>.</description>
        /// </item></list></returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="x"/> or <paramref name="y"/> cannot be cast to <typeparamref name="T"/>.
        /// </exception>
        /// <remarks>
        /// <b>Compare</b> returns the result of the <see cref="Comparison"/> method for the
        /// specified <paramref name="x"/> and <paramref name="y"/>.</remarks>

        public int Compare(object x, object y) {
            return Comparison((T) x, (T) y);
        }

        #endregion
        #region IComparer<T> Members

        /// <summary>
        /// Compares two specified <typeparamref name="T"/> instances and returns an indication of
        /// their relative values.</summary>
        /// <param name="x">
        /// The first <typeparamref name="T"/> instance to compare.</param>
        /// <param name="y">
        /// The second <typeparamref name="T"/> instance to compare.</param>
        /// <returns><para>
        /// An <see cref="Int32"/> value indicating the relative order of <paramref name="x"/> and
        /// <paramref name="y"/>, as follows:
        /// </para><list type="table"><listheader>
        /// <term>Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term>Less than zero</term>
        /// <description><paramref name="x"/> is less than <paramref name="y"/>.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description><paramref name="x"/> equals <paramref name="y"/>.</description>
        /// </item><item>
        /// <term>Greater than zero</term>
        /// <description><paramref name="x"/> is greater than <paramref name="y"/>.</description>
        /// </item></list></returns>
        /// <remarks>
        /// <b>Compare</b> returns the result of the <see cref="Comparison"/> method for the
        /// specified <paramref name="x"/> and <paramref name="y"/>.</remarks>

        public int Compare(T x, T y) {
            return Comparison(x, y);
        }

        #endregion
    }
}
