namespace Tektosyne.Geometry {

    /// <summary>
    /// Specifies the possible results of the <see cref="SubdivisionFace.MoveEdge"/> method of the
    /// <see cref="SubdivisionFace"/> class.</summary>

    internal enum MoveEdgeResult {

        /// <summary>
        /// Neither the half-edge nor its twin equals <see cref="SubdivisionFace.OuterEdge"/> or any
        /// <see cref="SubdivisionFace.InnerEdges"/> element. No properties were changed.</summary>

        Unchanged,

        /// <summary>
        /// The half-edge or its twin equals <see cref="SubdivisionFace.OuterEdge"/>, and that
        /// property was changed to another <see cref="SubdivisionEdge"/>.</summary>

        OuterChanged,

        /// <summary>
        /// The half-edge or its twin equals an <see cref="SubdivisionFace.InnerEdges"/> element,
        /// and that element was changed to another <see cref="SubdivisionEdge"/>.</summary>

        InnerChanged,

        /// <summary>
        /// The half-edge or its twin equals an <see cref="SubdivisionFace.InnerEdges"/> element,
        /// and that element was removed since its cycle contains no other half-edges.</summary>

        InnerRemoved
    }
}