namespace Tektosyne.Geometry {

    /// <summary>
    /// Specifies the shifting of rows or columns in a <see cref="PolygonGrid"/>.</summary>
    /// <remarks><para>
    /// <b>PolygonGridShift</b> specifies if and how even-numbered rows or columns in a rectangular
    /// <see cref="PolygonGrid"/> are shifted, relative to odd-numbered rows or columns. The valid
    /// choices for a given <see cref="PolygonGrid"/> depend on the underlying <see
    /// cref="RegularPolygon"/>.
    /// </para><note type="caution">
    /// The terms "even-numbered" and "odd-numbered" refer to a counting sequence that starts at one
    /// for the <see cref="PolygonGrid"/> row or column at index position zero. This first row or
    /// column is therefore considered <em>odd-numbered</em> rather than even-numbered.
    /// </note></remarks>

    public enum PolygonGridShift {
        #region None

        /// <summary>
        /// Specifies that no <see cref="PolygonGrid"/> rows or columns are shifted.</summary>
        /// <remarks><para>
        /// <b>None</b> is the only valid choice for a <see cref="PolygonGrid"/> of squares with
        /// <see cref="PolygonOrientation.OnEdge"/> orientation.
        /// </para><para>
        /// <b>None</b> is not a valid choice for any other <see cref="RegularPolygon"/> type. All
        /// <see cref="PolygonGridShift"/> values other than <b>None</b> are valid for a <see
        /// cref="PolygonGrid"/> of squares with <see cref="PolygonOrientation.OnVertex"/>
        /// orientation.</para></remarks>

        None,

        #endregion
        #region ColumnUp

        /// <summary>
        /// Specifies that even-numbered <see cref="PolygonGrid"/> columns are shifted upward.
        /// </summary>
        /// <remarks>
        /// <b>ColumnUp</b> and <see cref="ColumnDown"/> are the only valid choices for a <see
        /// cref="PolygonGrid"/> of hexagons with <see cref="PolygonOrientation.OnEdge"/>
        /// orientation.</remarks>

        ColumnUp,

        #endregion
        #region ColumnDown

        /// <summary>
        /// Specifies that even-numbered <see cref="PolygonGrid"/> columns are shifted downward.
        /// </summary>
        /// <remarks>
        /// <see cref="ColumnUp"/> and <b>ColumnDown</b> are the only valid choices for a <see
        /// cref="PolygonGrid"/> of hexagons with <see cref="PolygonOrientation.OnEdge"/>
        /// orientation.</remarks>

        ColumnDown,

        #endregion
        #region RowRight

        /// <summary>
        /// Specifies that even-numbered <see cref="PolygonGrid"/> rows are shifted to the right.
        /// </summary>
        /// <remarks>
        /// <see cref="RowLeft"/> and <b>RowRight</b> are the only valid choices for a <see
        /// cref="PolygonGrid"/> of hexagons with <see cref="PolygonOrientation.OnVertex"/>
        /// orientation.</remarks>

        RowRight,

        #endregion
        #region RowLeft

        /// <summary>
        /// Specifies that even-numbered <see cref="PolygonGrid"/> rows are shifted to the left.
        /// </summary>
        /// <remarks>
        /// <b>RowLeft</b> and <see cref="RowRight"/> are the only valid choices for a <see
        /// cref="PolygonGrid"/> of hexagons with <see cref="PolygonOrientation.OnVertex"/>
        /// orientation.</remarks>

        RowLeft,

        #endregion
    }
}
