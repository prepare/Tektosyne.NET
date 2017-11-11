namespace Tektosyne {

    /// <summary>
    /// Provides static methods for creating <see cref="ValueTuple"/> instances.</summary>
    /// <remarks>
    /// <b>ValueTuple</b> offers static <b>Create</b> methods for all <see cref="ValueTuple"/>
    /// variants to allow their instantation without having to specify generic type arguments.
    /// </remarks>

    public static class ValueTuple {
        #region Create<T1, T2>

        /// <summary>
        /// Creates a new <see cref="ValueTuple{T1,T2}"/>.</summary>
        /// <typeparam name="T1">
        /// The type of the first component.</typeparam>
        /// <typeparam name="T2">
        /// The type of the second component.</typeparam>
        /// <param name="item1">
        /// The first component of the <see cref="ValueTuple{T1,T2}"/>.</param>
        /// <param name="item2">
        /// The second component of the <see cref="ValueTuple{T1,T2}"/>.</param>
        /// <returns>
        /// A <see cref="ValueTuple{T1,T2}"/> containing the specified <paramref name="item1"/> and
        /// <paramref name="item2"/>.</returns>
        /// <remarks>
        /// <b>Create</b> allows you to create a new <see cref="ValueTuple{T1,T2}"/> without having
        /// to explicitly specify its generic type arguments.</remarks>

        public static ValueTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) {
            return new ValueTuple<T1, T2>(item1, item2);
        }

        #endregion
        #region Create<T1, T2, T3>

        /// <summary>
        /// Creates a new <see cref="ValueTuple{T1,T2,T3}"/>.</summary>
        /// <typeparam name="T1">
        /// The type of the first component.</typeparam>
        /// <typeparam name="T2">
        /// The type of the second component.</typeparam>
        /// <typeparam name="T3">
        /// The type of the third component.</typeparam>
        /// <param name="item1">
        /// The first component of the <see cref="ValueTuple{T1,T2,T3}"/>.</param>
        /// <param name="item2">
        /// The second component of the <see cref="ValueTuple{T1,T2,T3}"/>.</param>
        /// <param name="item3">
        /// The third component of the <see cref="ValueTuple{T1,T2,T3}"/>.</param>
        /// <returns>
        /// A <see cref="ValueTuple{T1,T2,T3}"/> containing the specified <paramref name="item1"/>,
        /// <paramref name="item2"/>, and <paramref name="item3"/>.</returns>
        /// <remarks>
        /// <b>Create</b> allows you to create a new <see cref="ValueTuple{T1,T2,T3}"/> without
        /// having to explicitly specify its generic type arguments.</remarks>

        public static ValueTuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) {
            return new ValueTuple<T1, T2, T3>(item1, item2, item3);
        }

        #endregion
        #region Create<T1, T2, T3, T4>

        /// <summary>
        /// Creates a new <see cref="ValueTuple{T1,T2,T3,T4}"/>.</summary>
        /// <typeparam name="T1">
        /// The type of the first component.</typeparam>
        /// <typeparam name="T2">
        /// The type of the second component.</typeparam>
        /// <typeparam name="T3">
        /// The type of the third component.</typeparam>
        /// <typeparam name="T4">
        /// The type of the fourth component.</typeparam>
        /// <param name="item1">
        /// The first component of the <see cref="ValueTuple{T1,T2,T3,T4}"/>.</param>
        /// <param name="item2">
        /// The second component of the <see cref="ValueTuple{T1,T2,T3,T4}"/>.</param>
        /// <param name="item3">
        /// The third component of the <see cref="ValueTuple{T1,T2,T3,T4}"/>.</param>
        /// <param name="item4">
        /// The fourth component of the <see cref="ValueTuple{T1,T2,T3,T4}"/>.</param>
        /// <returns>
        /// A <see cref="ValueTuple{T1,T2,T3,T4}"/> containing the specified <paramref
        /// name="item1"/>, <paramref name="item2"/>, <paramref name="item3"/>, and <paramref
        /// name="item4"/>.</returns>
        /// <remarks>
        /// <b>Create</b> allows you to create a new <see cref="ValueTuple{T1,T2,T3,T4}"/> without
        /// having to explicitly specify its generic type arguments.</remarks>

        public static ValueTuple<T1, T2, T3, T4>
            Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) {
            return new ValueTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }

        #endregion
    }
}
