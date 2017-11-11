using System.Collections.Generic;

namespace Tektosyne.Collections {

    /// <summary>
    /// Caches the default comparers for the type specified by the generic argument.</summary>
    /// <typeparam name="T">
    /// The type of all objects being compared.</typeparam>
    /// <remarks><para>
    /// The <see cref="System.Collections.Generic.Comparer{T}"/> and <see
    /// cref="System.Collections.Generic.EqualityComparer{T}"/> classes each provide a
    /// <b>Default</b> property that creates a default comparer for the specified type. These
    /// properties also cache the returned comparers, but accessing the property getter may still
    /// require a slow method call.
    /// </para><para>
    /// <b>ComparerCache</b> therefore provides a second cache level with read-only fields for
    /// immediate access to the default comparers for <typeparamref name="T"/>. This can accelerate
    /// comparisons by up to four times for value types with fast comparers, depending on the JIT
    /// compiler version and target platform.</para></remarks>

    public static class ComparerCache<T> {
        #region Comparer

        /// <summary>
        /// The default <see cref="System.Collections.Generic.Comparer{T}"/> for <typeparamref
        /// name="T"/>.</summary>

        public static readonly Comparer<T> Comparer = Comparer<T>.Default;

        #endregion
        #region EqualityComparer

        /// <summary>
        /// The default <see cref="System.Collections.Generic.EqualityComparer{T}"/> for
        /// <typeparamref name="T"/>.</summary>

        public static readonly EqualityComparer<T> EqualityComparer = EqualityComparer<T>.Default;

        #endregion
    }
}
