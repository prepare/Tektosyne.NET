using System;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Maps the faces of a planar <see cref="Subdivision"/> to arbitrary objects.</summary>
    /// <typeparam name="T">
    /// The type of all objects to associate with the faces of the <see cref="Subdivision"/>.
    /// </typeparam>
    /// <remarks><para>
    /// <b>ISubdivisionMap</b> provides an application-specific mapping of all <see
    /// cref="Subdivision.Faces"/> of a planar <see cref="Subdivision"/> to arbitrary objects. The
    /// use of an interface allows clients to implement the most efficient mapping for their
    /// concrete <see cref="Subdivision"/> structure and object type <typeparamref name="T"/>.
    /// </para><para>
    /// Since the <see cref="Subdivision"/> has no knowledge of any <b>ISubdivisionMap</b> instances
    /// that reference it, you must manually update any such instances whenever the underlying <see
    /// cref="Subdivision"/> changes.</para></remarks>

    public interface ISubdivisionMap<T> {
        #region Source

        /// <summary>
        /// Gets the <see cref="Subdivision"/> that contains all mapped faces.</summary>
        /// <value>
        /// The <see cref="Subdivision"/> that contains all faces accepted and returned by the <see
        /// cref="FromFace"/> and <see cref="ToFace"/> methods, respectively.</value>
        /// <remarks>
        /// <b>Source</b> never returns a null reference. Multiple <see cref="ISubdivisionMap{T}"/>
        /// instances may refer to the same <b>Source</b>, mapping its <see
        /// cref="Subdivision.Faces"/> to objects of different types.</remarks>

        Subdivision Source { get; }

        #endregion
        #region Target

        /// <summary>
        /// Gets the object that defines all mapped <typeparamref name="T"/> objects.</summary>
        /// <value>
        /// The object that defines all <typeparamref name="T"/> objects returned and accepted by
        /// the <see cref="FromFace"/> and <see cref="ToFace"/> methods, respectively.</value>
        /// <remarks>
        /// <b>Target</b> may return a null reference if providing a container for all associated
        /// values is impossible or inconvenient.</remarks>

        object Target { get; }

        #endregion
        #region FromFace

        /// <summary>
        /// Converts the specified <see cref="SubdivisionFace"/> into the associated <typeparamref
        /// name="T"/> object.</summary>
        /// <param name="face">
        /// The <see cref="SubdivisionFace"/> to convert.</param>
        /// <returns>
        /// The <typeparamref name="T"/> object associated with <paramref name="face"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="face"/> was not found within <see cref="Source"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="face"/> is a null reference.</exception>

        T FromFace(SubdivisionFace face);

        #endregion
        #region ToFace

        /// <summary>
        /// Converts the specified <typeparamref name="T"/> object into the associated <see
        /// cref="SubdivisionFace"/>.</summary>
        /// <param name="value">
        /// The <typeparamref name="T"/> object to convert.</param>
        /// <returns>
        /// The <see cref="SubdivisionFace"/> associated with <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> does not map to any <see cref="SubdivisionFace"/> within <see
        /// cref="Source"/>.</exception>

        SubdivisionFace ToFace(T value);

        #endregion
    }
}
