namespace Tektosyne.Geometry {

    /// <summary>
    /// Specifies the possible orientations of a <see cref="RegularPolygon"/>.</summary>
    /// <remarks>
    /// <b>PolygonOrientation</b> specifies the two possible orientations of a regular polygon as
    /// represented by the <see cref="RegularPolygon"/> class.</remarks>

    public enum PolygonOrientation {

        /// <summary>
        /// Specifies that the <see cref="RegularPolygon"/> is lying on an edge.</summary>

        OnEdge,

        /// <summary>
        /// Specifies that the <see cref="RegularPolygon"/> is standing on a vertex.</summary>

        OnVertex
    }
}
