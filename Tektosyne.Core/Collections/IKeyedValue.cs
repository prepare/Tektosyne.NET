using System;
using System.Collections.Generic;

namespace Tektosyne.Collections {
    #region IKeyedValue

    /// <summary>
    /// Associates an object with a key.</summary>
    /// <typeparam name="TKey">
    /// The type of the key.</typeparam>
    /// <remarks><para>
    /// <b>IKeyedValue</b> provides a generic read-only property, <see cref="IKeyedValue{T}.Key"/>,
    /// intended to identify the implementing object within a collection of compatible objects. The
    /// derived interface <see cref="IMutableKeyedValue{TKey}"/> adds a method to set the <b>Key</b>
    /// property. The <b>Key</b> of each object is typically unique within a collection, but that is
    /// not a requirement of the interface.
    /// </para><para>
    /// <b>IKeyedValue</b> instances that are stored in an <see cref="IList{T}"/> collection can be
    /// identified by their <b>Key</b> values – just as if they were stored in an <see
    /// cref="IDictionary{TKey, TValue}"/> collection, but without the need to maintain a separate
    /// <see cref="KeyValuePair{TKey, TValue}.Key"/> component for each element.
    /// </para><para>
    /// <b>IKeyedValue</b> instances can also be stored as the <see cref="KeyValuePair{TKey,
    /// TValue}.Value"/> components of an <see cref="IDictionary{TKey, TValue}"/> collection. In
    /// this case, their <b>Key</b> properties should mirror the associated <see
    /// cref="KeyValuePair{TKey, TValue}.Key"/> components. This key duplication allows clients to
    /// identify the <b>Value</b> components outside of the containing collection.</para></remarks>

    public interface IKeyedValue<TKey> {
        #region Key

        /// <summary>
        /// Gets the key associated with the <see cref="IKeyedValue{TKey}"/>.</summary>
        /// <value>
        /// An object that identifies the <see cref="IKeyedValue{TKey}"/> instance within any
        /// collection of compatible instances. This value is never a null reference.</value>
        /// <remarks>
        /// If the <see cref="IKeyedValue{TKey}"/> instance is stored as the <see
        /// cref="KeyValuePair{TKey, TValue}.Value"/> component of a <see cref="KeyValuePair{TKey,
        /// TValue}"/>, this property should have the same value as the <see
        /// cref="KeyValuePair{TKey, TValue}.Key"/> component of the <b>KeyValuePair</b>.</remarks>

        TKey Key { get; }

        #endregion
    }

    #endregion
    #region IMutableKeyedValue

    /// <summary>
    /// Associates an object with a mutable key.</summary>
    /// <typeparam name="TKey">
    /// The type of the mutable key.</typeparam>
    /// <remarks>
    /// <b>IMutableKeyedValue</b> extends the <see cref="IKeyedValue{TKey}"/> interface with an
    /// additional <see cref="IMutableKeyedValue{TKey}.SetKey"/> method that allows clients to
    /// change the <see cref="IKeyedValue{T}.Key"/> property of implementing classes.</remarks>

    public interface IMutableKeyedValue<TKey>: IKeyedValue<TKey> {
        #region SetKey

        /// <summary>
        /// Sets the <see cref="IKeyedValue{T}.Key"/> associated with the <see
        /// cref="IMutableKeyedValue{TKey}"/>.</summary>
        /// <param name="key">
        /// The new value for the <see cref="IKeyedValue{T}.Key"/> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>

        void SetKey(TKey key);

        #endregion
    }

    #endregion
}
