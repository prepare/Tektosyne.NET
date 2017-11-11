using System;
using System.Diagnostics;

using Tektosyne.Collections;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents the result of splitting a <see cref="SubdivisionEdge"/> into two parts.</summary>
    /// <remarks>
    /// <b>SplitEdgeResult</b> is an immutable structure containing two or three <see
    /// cref="SubdivisionEdge"/> instances that describe the result of splitting another instance 
    /// into two parts. If <see cref="SplitEdgeResult.CreatedEdge"/> is valid then <see
    /// cref="SplitEdgeResult.IsEdgeDeleted"/> is <c>false</c>, and vice versa.</remarks>

    internal struct SplitEdgeResult {
        #region SplitEdgeResult(...)

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitEdgeResult"/> structure with the
        /// specified <see cref="SubdivisionEdge"/> instances.</summary>
        /// <param name="originEdge">
        /// The <see cref="SubdivisionEdge"/> with the same <see cref="SubdivisionEdge.Origin"/> as
        /// the instance that has been split.</param>
        /// <param name="destinationEdge">
        /// The <see cref="SubdivisionEdge"/> with the same <see
        /// cref="SubdivisionEdge.Destination"/> as the instance that has been split.</param>
        /// <param name="createdEdge">
        /// The <see cref="SubdivisionEdge"/> that has been newly created for one of the two parts
        /// resulting from the split, if any; otherwise, a null reference.</param>
        /// <param name="isEdgeDeleted">
        /// <c>true</c> if the <see cref="SubdivisionEdge"/> to be split has been deleted because
        /// both parts were duplicated by existing instances; otherwise, <c>false</c>.</param>

        public SplitEdgeResult(
            SubdivisionEdge originEdge, SubdivisionEdge destinationEdge,
            SubdivisionEdge createdEdge, bool isEdgeDeleted) {

            OriginEdge = originEdge;
            DestinationEdge = destinationEdge;
            CreatedEdge = createdEdge;
            IsEdgeDeleted = isEdgeDeleted;
        }

        #endregion
        #region Public Fields

        /// <summary>
        /// The <see cref="SubdivisionEdge"/> with the same <see cref="SubdivisionEdge.Origin"/> as
        /// the instance that has been split.</summary>

        public readonly SubdivisionEdge OriginEdge;

        /// <summary>
        /// The <see cref="SubdivisionEdge"/> with the same <see
        /// cref="SubdivisionEdge.Destination"/> as the instance that has been split.</summary>

        public readonly SubdivisionEdge DestinationEdge;

        /// <summary>
        /// The <see cref="SubdivisionEdge"/> that has been newly created for one of the two parts
        /// resulting from the split, if any; otherwise, a null reference.</summary>

        public readonly SubdivisionEdge CreatedEdge;

        /// <summary>
        /// <c>true</c> if the <see cref="SubdivisionEdge"/> to be split has been deleted because
        /// both parts were duplicated by existing instances; otherwise, <c>false</c>.</summary>

        public readonly bool IsEdgeDeleted;

        #endregion
        #region UpdateFaces

        /// <summary>
        /// Updates the <see cref="SubdivisionFace"/> keys in the specified dictionaries after the
        /// specified <see cref="SubdivisionEdge"/> has been split.</summary>
        /// <param name="edge">
        /// The <see cref="SubdivisionEdge"/> whose splitting resulted in the current <see
        /// cref="SplitEdgeResult"/>.</param>
        /// <param name="edgeToFace1">
        /// An <see cref="Int32Dictionary{T}"/> that maps the keys of any existing half-edges to the
        /// keys of the incident bounded <see cref="Subdivision.Faces"/> of the corresponding <see
        /// cref="Subdivision.Edges"/> in a first <see cref="Subdivision"/>.</param>
        /// <param name="edgeToFace2">
        /// An <see cref="Int32Dictionary{T}"/> that maps the keys of any existing half-edges to the
        /// keys of the incident bounded <see cref="Subdivision.Faces"/> of the corresponding <see
        /// cref="Subdivision.Edges"/> in a second <see cref="Subdivision"/>.</param>
        /// <remarks>
        /// <b>UpdateFaces</b> ensures that the mapping between original and intersected faces
        /// established by the <see cref="Subdivision.Intersection"/> algorithm is kept up-to-date
        /// when edge splitting results in a valid <see cref="CreatedEdge"/>.</remarks>

        public void UpdateFaces(SubdivisionEdge edge,
            Int32Dictionary<Int32> edgeToFace1, Int32Dictionary<Int32> edgeToFace2) {

            if (CreatedEdge == null) return;
            int face;

            if (edgeToFace1.TryGetValue(edge._key, out face)) {
                Debug.Assert(face != 0);
                edgeToFace1.Add(CreatedEdge._key, face);
            }

            if (edgeToFace2.TryGetValue(edge._key, out face)) {
                Debug.Assert(face != 0);
                edgeToFace2.Add(CreatedEdge._key, face);
            }
        }

        #endregion
    }
}
