namespace Tektosyne.Geometry {

    /// <summary>
    /// Specifies the location of a point relative to an arbitrary polygon.</summary>
    /// <remarks>
    /// <b>PolygonLocation</b> specifies the possible return values of the <see
    /// cref="GeoAlgorithms.PointInPolygon"/> algorithm.</remarks>

    public enum PolygonLocation {

        /// <summary>
        /// Specifies that the point is inside the polygon.</summary>

        Inside,

        /// <summary>
        /// Specifies that the point is outside the polygon.</summary>

        Outside,

        /// <summary>
        /// Specifies that the point coincides with an edge of the polygon.</summary>

        Edge,

        /// <summary>
        /// Specifies that the point coincides with a vertex of the polygon.</summary>

        Vertex
    }
}
