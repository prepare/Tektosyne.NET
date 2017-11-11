namespace Tektosyne.Geometry {

    /// <summary>
    /// Specifies the spatial relationship between two line segments.</summary>

    public enum LineRelation {

        /// <summary>
        /// Specifies that the two line segments are parallel displacements of each other, and
        /// therefore cannot share any points.</summary>

        Parallel,

        /// <summary>
        /// Specifies that the two line segments are part of the same infinite line, and therefore
        /// may share some or all their points.</summary>

        Collinear,

        /// <summary>
        /// Specifies that the two line segments have different angles, and therefore may share a
        /// single point of intersection.</summary>

        Divergent
    }
}
