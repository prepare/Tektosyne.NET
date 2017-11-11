using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using Tektosyne.Collections;
using Tektosyne.Graph;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a rectangular grid composed of regular polygons.</summary>
    /// <remarks><para>
    /// <b>PolygonGrid</b> represents a mesh of identical squares or regular hexagons. The shape of 
    /// each element is described by a <see cref="RegularPolygon"/> which must have either four or
    /// six <see cref="RegularPolygon.Sides"/>.
    /// </para><para>
    /// Each polygonal element within the <b>PolygonGrid</b> corresponds to a coordinate pair within
    /// an integer rectangle. That is, coordinates range from zero to a user-defined width and
    /// height. The exact mapping of elements to coordinates depends on the underlying <see
    /// cref="RegularPolygon"/> and on the associated <see cref="PolygonGridShift"/> value.
    /// </para><para>
    /// <b>PolygonGrid</b> supports generic graph algorithms through its implementation of the <see
    /// cref="IGraph2D{T}"/> interface. The graph nodes are the <see cref="PointI"/> coordinates of
    /// all grid locations. Two nodes are considered connected if they correspond to neighboring
    /// grid locations. The distance measure is the number of intervening grid locations.
    /// </para><para>
    /// Other methods provide topological information on grid locations, conversion to and from
    /// display coordinates, and the creation of a read-only wrapper similar to those provided by
    /// various <b>Tektosyne.Collections</b> classes.</para></remarks>

    [Serializable]
    public class PolygonGrid: ICloneable, IGraph2D<PointI> {
        #region PolygonGrid(RegularPolygon)

        /// <overloads>
        /// Initializes a new instance of the <see cref="PolygonGrid"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonGrid"/> class with the specified
        /// element shape.</summary>
        /// <param name="element">
        /// The <see cref="RegularPolygon"/> object that constitutes an element of the <see
        /// cref="PolygonGrid"/>.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="element"/> is neither a square nor a hexagon.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element"/> is a null reference.</exception>
        /// <remarks><para>
        /// The <see cref="GridShift"/> property is initialized to an appropriate default value that
        /// depends on the specified <paramref name="element"/>.
        /// </para><para>
        /// The <see cref="Size"/> property is initialized to (1,1). All dependent sizes are
        /// initialized according to the specified property values.</para></remarks>

        public PolygonGrid(RegularPolygon element) {
            if (element == null)
                ThrowHelper.ThrowArgumentNullException("element");

            _data = new InstanceData();
            Element = element;
            Size = new SizeI(1, 1);
        }

        #endregion
        #region PolygonGrid(RegularPolygon, PolygonGridShift)

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonGrid"/> class with the specified
        /// element shape and row or column shifting.</summary>
        /// <param name="element">
        /// The <see cref="RegularPolygon"/> object that constitutes an element of the <see
        /// cref="PolygonGrid"/>.</param>
        /// <param name="gridShift">
        /// A <see cref="PolygonGridShift"/> value indicating the shifting of rows or columns in the
        /// <see cref="PolygonGrid"/>.</param>
        /// <exception cref="ArgumentException"><para>
        /// <paramref name="element"/> is neither a square nor a hexagon.
        /// </para><para>-or-</para><para>
        /// <paramref name="gridShift"/> is not compatible with <paramref name="element"/>.
        /// </para></exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element"/> is a null reference.</exception>
        /// <exception cref="InvalidEnumArgumentException">
        /// <paramref name="gridShift"/> is not a valid <see cref="PolygonGridShift"/> value.
        /// </exception>
        /// <remarks>
        /// The <see cref="Size"/> property is initialized to (1,1). All dependent sizes are
        /// initialized according to the specified property values.</remarks>

        public PolygonGrid(RegularPolygon element, PolygonGridShift gridShift): this(element) {
            GridShift = gridShift;
        }

        #endregion
        #region PolygonGrid(PolygonGrid)

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonGrid"/> class that is a shallow copy
        /// of the specified instance.</summary>
        /// <param name="grid">
        /// The <see cref="PolygonGrid"/> object whose property values should be copied to the new
        /// instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="grid"/> is a null reference.</exception>
        /// <remarks><para>
        /// This "copy constructor" does not need to perform a deep copy as the associated <see
        /// cref="RegularPolygon"/> object is immutable, and all other properties contain either
        /// structures or read-only values that depend on other properties.
        /// </para><note type="caution">
        /// Some properties return arrays that <em>can</em> be changed by clients, and any such
        /// changes would be reflected in both the specified <paramref name="grid"/> and the new
        /// <see cref="PolygonGrid"/> instance. Never modify arrays that are returned by properties!
        /// </note></remarks>

        public PolygonGrid(PolygonGrid grid) {
            if (grid == null)
                ThrowHelper.ThrowArgumentNullException("grid");

            _data = new InstanceData(grid._data);
        }

        #endregion
        #region PolygonGrid(InstanceData)

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonGrid"/> class that is a read-only
        /// view of the specified <see cref="InstanceData"/>.</summary>
        /// <param name="data">
        /// The <see cref="InstanceData"/> object that the new instance should share.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data"/> is a null reference.</exception>
        /// <remarks>
        /// The <see cref="IsReadOnly"/> property is initialized to <c>true</c>. The new instance is
        /// its own read-only wrapper, i.e. <see cref="AsReadOnly"/> will return the instance it is
        /// invoked on.</remarks>

        private PolygonGrid(InstanceData data) {
            if (data == null)
                ThrowHelper.ThrowArgumentNullException("data");

            _data = data;
            IsReadOnly = true;
            _readOnlyWrapper = this;
        }

        #endregion
        #region Private Fields

        // support for read-only wrapper
        private readonly InstanceData _data;
        private PolygonGrid _readOnlyWrapper;

        // precomputed value for DisplayToGrid
        private static readonly double _sqrt3 = Math.Sqrt(3.0);

        #region Squares on Edge

        private static readonly PointI[][][] _squareEdgeOffsets = {
            new PointI[][] {
                new PointI[] {
                    // edge connections only
                    new PointI( 0, -1),  // North
                    new PointI( 1,  0),  // East
                    new PointI( 0,  1),  // South
                    new PointI(-1,  0),  // West
                }
            },
            new PointI[][] {
                new PointI[] {
                    // edge & vertex connections
                    new PointI( 0, -1),  // North
                    new PointI( 1, -1),  // North-East
                    new PointI( 1,  0),  // East
                    new PointI( 1,  1),  // South-East
                    new PointI( 0,  1),  // South
                    new PointI(-1,  1),  // South-West
                    new PointI(-1,  0),  // West
                    new PointI(-1, -1),  // North-West
                }
            }
        };

        #endregion
        #region Squares on Vertex (Columns Shifted)

        private static readonly PointI[][][] _squareVertexColumnOffsets = {
            new PointI[][] {
                new PointI[] {
                    // edge only, column up
                    new PointI( 1, -1),  // North-East
                    new PointI( 1,  0),  // South-East
                    new PointI(-1,  0),  // South-West
                    new PointI(-1, -1),  // North-West
                },
                new PointI[] {
                    // edge only, column down
                    new PointI( 1,  0),  // North-East
                    new PointI( 1,  1),  // South-East
                    new PointI(-1,  1),  // South-West
                    new PointI(-1,  0),  // North-West
                }
            },
            new PointI[][] {
                new PointI[] {
                    // edge & vertex, column up
                    new PointI( 0, -1),  // North
                    new PointI( 1, -1),  // North-East
                    new PointI( 2,  0),  // East
                    new PointI( 1,  0),  // South-East
                    new PointI( 0,  1),  // South
                    new PointI(-1,  0),  // South-West
                    new PointI(-2,  0),  // West
                    new PointI(-1, -1),  // North-West
                },
                new PointI[] {
                    // edge & vertex, column down
                    new PointI( 0, -1),  // North
                    new PointI( 1,  0),  // North-East
                    new PointI( 2,  0),  // East
                    new PointI( 1,  1),  // South-East
                    new PointI( 0,  1),  // South
                    new PointI(-1,  1),  // South-West
                    new PointI(-2,  0),  // West
                    new PointI(-1,  0),  // North-West
                }
            }
        };

        #endregion
        #region Squares on Vertex (Rows Shifted)

        private static readonly PointI[][][] _squareVertexRowOffsets = {
            new PointI[][] {
                new PointI[] {
                    // edge only, row left
                    new PointI( 0, -1),  // North-East
                    new PointI( 0,  1),  // South-East
                    new PointI(-1,  1),  // South-West
                    new PointI(-1, -1),  // North-West
                },
                new PointI[] {
                    // edge only, row right
                    new PointI( 1, -1),  // North-East
                    new PointI( 1,  1),  // South-East
                    new PointI( 0,  1),  // South-West
                    new PointI( 0, -1),  // North-West
                }
            },
            new PointI[][] {
                new PointI[] {
                    // edge & vertex, row left
                    new PointI( 0, -2),  // North
                    new PointI( 0, -1),  // North-East
                    new PointI( 1,  0),  // East
                    new PointI( 0,  1),  // South-East
                    new PointI( 0,  2),  // South
                    new PointI(-1,  1),  // South-West
                    new PointI(-1,  0),  // West
                    new PointI(-1, -1),  // North-West
                },
                new PointI[] {
                    // edge & vertex, row right
                    new PointI( 0, -2),  // North
                    new PointI( 1, -1),  // North-East
                    new PointI( 1,  0),  // East
                    new PointI( 1,  1),  // South-East
                    new PointI( 0,  2),  // South
                    new PointI( 0,  1),  // South-West
                    new PointI(-1,  0),  // West
                    new PointI( 0, -1),  // North-West
                }
            }
        };

        #endregion
        #region Hexagons on Edge

        private static readonly PointI[][] _hexagonEdgeOffsets = {
            new PointI[] {
                // column up
                new PointI( 0, -1),  // North
                new PointI( 1, -1),  // North-East
                new PointI( 1,  0),  // South-East
                new PointI( 0,  1),  // South
                new PointI(-1,  0),  // South-West
                new PointI(-1, -1),  // North-West
            },
            new PointI[] {
                // column down
                new PointI( 0, -1),  // North
                new PointI( 1,  0),  // North-East
                new PointI( 1,  1),  // South-East
                new PointI( 0,  1),  // South
                new PointI(-1,  1),  // South-West
                new PointI(-1,  0),  // North-West
            }
        };

        #endregion
        #region Hexagons on Vertex

        private static readonly PointI[][] _hexagonVertexOffsets = {
            new PointI[] {
                // row left
                new PointI( 0, -1),  // North-East
                new PointI( 1,  0),  // East
                new PointI( 0,  1),  // South-East
                new PointI(-1,  1),  // South-West
                new PointI(-1,  0),  // West
                new PointI(-1, -1),  // North-West
            },
            new PointI[] {
                // row right
                new PointI( 1, -1),  // North-East
                new PointI( 1,  0),  // East
                new PointI( 1,  1),  // South-East
                new PointI( 0,  1),  // South-West
                new PointI(-1,  0),  // West
                new PointI( 0, -1),  // North-West
            }
        };

        #endregion
        #endregion
        #region InvalidLocation

        /// <summary>
        /// Represents an invalid <see cref="PolygonGrid"/> location.</summary>
        /// <remarks><para>
        /// <b>InvalidLocation</b> holds a <see cref="PointI"/> value whose <see cref="PointI.X"/>
        /// and <see cref="PointI.Y"/> components are both -1. This represents a location outside of
        /// any <see cref="PolygonGrid"/> since column and row indices are always zero-based.
        /// </para><para>
        /// Various <see cref="PolygonGrid"/> methods return <b>InvalidLocation</b> to indicate that
        /// a valid <see cref="PolygonGrid"/> location could not be found. Clients are encouraged to
        /// use this read-only field for the same purpose.</para></remarks>

        public static readonly PointI InvalidLocation = new PointI(-1, -1);

        #endregion
        #region Public Properties
        #region AreColumnsShifted

        /// <summary>
        /// Gets a value indicating whether the columns of the <see cref="PolygonGrid"/> are
        /// shifted.</summary>
        /// <value>
        /// <c>true</c> if <see cref="GridShift"/> equals <see cref="PolygonGridShift.ColumnUp"/> or
        /// <see cref="PolygonGridShift.ColumnDown"/>; otherwise, <c>false</c>.</value>

        public bool AreColumnsShifted {
            get {
                return (_data.GridShift == PolygonGridShift.ColumnUp
                    || _data.GridShift == PolygonGridShift.ColumnDown);
            }
        }

        #endregion
        #region AreRowsShifted

        /// <summary>
        /// Gets a value indicating whether the rows of the <see cref="PolygonGrid"/> are shifted.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="GridShift"/> equals <see cref="PolygonGridShift.RowLeft"/> or
        /// <see cref="PolygonGridShift.RowRight"/>; otherwise, <c>false</c>.</value>

        public bool AreRowsShifted {
            get {
                return (_data.GridShift == PolygonGridShift.RowLeft
                    || _data.GridShift == PolygonGridShift.RowRight);
            }
        }

        #endregion
        #region CenterDistance

        /// <summary>
        /// Gets the display distance between the center points of neighboring <see cref="Element"/>
        /// shapes.</summary>
        /// <value>
        /// A <see cref="SizeD"/> value indicating the display distance between the center points of
        /// neighboring <see cref="Element"/> shapes, given the current values of the <see
        /// cref="Element"/> and <see cref="GridShift"/> properties.</value>
        /// <remarks><para>
        /// The <see cref="SizeD.Width"/> component of <b>CenterDistance</b> holds the horizontal 
        /// distance between neighboring elements within the same row, and the <see
        /// cref="SizeD.Height"/> component holds the vertical distance between neighboring elements
        /// within the same column.
        /// </para><para>
        /// <b>CenterDistance</b> is recalculated automatically whenever the <see cref="Element"/>
        /// or <see cref="GridShift"/> properties change.</para></remarks>

        public SizeD CenterDistance {
            [DebuggerStepThrough]
            get { return _data.CenterDistance; }
        }

        #endregion
        #region DisplayBounds

        /// <summary>
        /// Gets the display bounds of the <see cref="PolygonGrid"/>.</summary>
        /// <value>
        /// A <see cref="RectD"/> indicating the display bounds of the <see cref="PolygonGrid"/>,
        /// given the current values of the <see cref="Element"/>, <see cref="GridShift"/>, and <see
        /// cref="Size"/> properties.</value>
        /// <remarks>
        /// The upper-left corner of <b>DisplayBounds</b> is always (0,0). The lower-right corner is
        /// automatically recalculated whenever the <see cref="Element"/>, <see cref="GridShift"/>,
        /// or <see cref="Size"/> properties change.</remarks>

        public RectD DisplayBounds {
            [DebuggerStepThrough]
            get { return _data.DisplayBounds; }
        }

        #endregion
        #region EdgeNeighborOffsets

        /// <summary>
        /// Gets a list of all coordinate offsets to reach a neighboring location on a shared edge.
        /// </summary>
        /// <value>
        /// A jagged <see cref="Array"/> containing 1 x 4, 2 x 4, or 2 x 6 <see cref="PointI"/>
        /// values whose coordinates range from -1 to +1.</value>
        /// <remarks><para>
        /// <b>EdgeNeighborOffsets</b> is identical to <see cref="NeighborOffsets"/> except that its
        /// inner arrays never contain offsets to neighboring locations on shared vertices, even if
        /// <see cref="RegularPolygon.VertexNeighbors"/> is <c>true</c> for the current <see
        /// cref="Element"/>. Use <see cref="GetEdgeNeighborOffsets"/> to determine the correct
        /// inner array for a given location.
        /// </para><para>
        /// <b>EdgeNeighborOffsets</b> is recalculated automatically whenever the <see
        /// cref="Element"/> or <see cref="GridShift"/> properties change.</para></remarks>

        public PointI[][] EdgeNeighborOffsets {
            [DebuggerStepThrough]
            get { return _data.EdgeNeighborOffsets; }
        }

        #endregion
        #region Element

        /// <summary>
        /// Gets or sets the <see cref="RegularPolygon"/> that constitutes an element of the <see
        /// cref="PolygonGrid"/>.</summary>
        /// <value>
        /// The <see cref="RegularPolygon"/> object that constitutes an element of the <see
        /// cref="PolygonGrid"/>.</value>
        /// <exception cref="ArgumentException">
        /// The property is set to a <see cref="RegularPolygon"/> that is neither a square nor a
        /// hexagon.</exception>
        /// <exception cref="ArgumentNullException">
        /// The property is set to a null reference.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="PolygonGrid"/> is read-only.</exception>
        /// <remarks>
        /// Setting <b>Element</b> also resets <see cref="GridShift"/> to an appropriate default
        /// value if the current value is incompatible with the new <b>Element</b>, and recalculates
        /// the <see cref="CenterDistance"/>, <see cref="DisplayBounds"/>, <see
        /// cref="EdgeNeighborOffsets"/>, and <see cref="NeighborOffsets"/> properties accordingly.
        /// </remarks>

        public RegularPolygon Element {
            [DebuggerStepThrough]
            get { return _data.Element; }
            set {
                if (IsReadOnly)
                    ThrowHelper.ThrowNotSupportedException(Strings.ViewReadOnly);

                // Check for incompatible GridShift value.
                // AreCompatible also checks "value" for null and invalid Sides.

                if (!AreCompatible(value, GridShift)) {
                    bool onEdge = (value.Orientation == PolygonOrientation.OnEdge);

                    if (value.Sides == 4) {
                        _data.GridShift = (onEdge ?
                            PolygonGridShift.None : PolygonGridShift.RowRight);
                    }
                    else {
                        Debug.Assert(value.Sides == 6);
                        _data.GridShift = (onEdge ?
                            PolygonGridShift.ColumnDown : PolygonGridShift.RowRight);
                    }
                }

                _data.Element = value;
                OnGeometryChanged();
            }
        }

        #endregion
        #region GridShift

        /// <summary>
        /// Gets or sets the shifting of rows or columns in the <see cref="PolygonGrid"/>.</summary>
        /// <value>
        /// A <see cref="PolygonGridShift"/> value indicating the shifting of <see cref="Element"/>
        /// rows or columns in the <see cref="PolygonGrid"/>.</value>
        /// <exception cref="ArgumentException">
        /// The property is set to a <see cref="PolygonGridShift"/> value that is not compatible
        /// with the current <see cref="Element"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException">
        /// The property is set to an invalid <see cref="PolygonGridShift"/> value.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="PolygonGrid"/> is read-only.</exception>
        /// <remarks><para>
        /// Setting <b>GridShift</b> also recalculates the <see cref="CenterDistance"/>, <see
        /// cref="DisplayBounds"/>, <see cref="EdgeNeighborOffsets"/>, and <see
        /// cref="NeighborOffsets"/> properties.
        /// </para><para>
        /// <b>GridShift</b> is automatically reset to an appropriate default value whenever the
        /// <see cref="Element"/> property changes to a <see cref="RegularPolygon"/> that is
        /// incompatible with the current <b>GridShift</b> value.
        /// </para><para>
        /// The following table shows the default <b>GridShift</b> values for all possible <see
        /// cref="Element"/> shapes:</para>
        /// <list type="table"><listheader>
        /// <term>Element</term><description>GridShift</description>
        /// </listheader><item>
        /// <term>Square on Edge</term>
        /// <description><see cref="PolygonGridShift.None"/></description>
        /// </item><item>
        /// <term>Square on Vertex</term>
        /// <description><see cref="PolygonGridShift.RowRight"/></description>
        /// </item><item>
        /// <term>Hexagon on Edge</term>
        /// <description><see cref="PolygonGridShift.ColumnDown"/></description>
        /// </item><item>
        /// <term>Hexagon on Vertex</term>
        /// <description><see cref="PolygonGridShift.RowRight"/></description>
        /// </item></list></remarks>

        public PolygonGridShift GridShift {
            [DebuggerStepThrough]
            get { return _data.GridShift; }
            set {
                if (IsReadOnly)
                    ThrowHelper.ThrowNotSupportedException(Strings.ViewReadOnly);

                if (!AreCompatible(Element, value))
                    ThrowHelper.ThrowArgumentExceptionWithFormat(
                        "value", Strings.ArgumentPropertyConflict, "Element");

                _data.GridShift = value;
                OnGeometryChanged();
            }
        }

        #endregion
        #region IsReadOnly

        /// <summary>
        /// <c>true</c> if the <see cref="PolygonGrid"/> is read-only; otherwise, <c>false</c>. The
        /// default is <c>false</c>.</summary>
        /// <remarks>
        /// Attempting to modify a read-only <see cref="PolygonGrid"/> will raise a <see
        /// cref="NotSupportedException"/>. Use <see cref="AsReadOnly"/> to create a read-only
        /// wrapper around a given <see cref="PolygonGrid"/>.</remarks>

        public readonly bool IsReadOnly;

        #endregion
        #region NeighborOffsets

        /// <summary>
        /// Gets a list of all coordinate offsets to reach a neighboring location.</summary>
        /// <value>
        /// A jagged <see cref="Array"/> containing 1 x 4, 2 x 4, 2 x 6, or 2 x 8 <see
        /// cref="PointI"/> values whose coordinates range from -2 to +2.</value>
        /// <remarks><para>
        /// The outer array of <b>NeighborOffsets</b> contains either a single array if <see
        /// cref="GridShift"/> is <see cref="PolygonGridShift.None"/>, or two arrays for any other
        /// <see cref="GridShift"/> value. In that case, the first inner array contains offsets for
        /// left-shifted rows or up-shifted columns, and the second inner array contains offsets for
        /// right-shifted rows or down-shifted columns.
        /// </para><para>
        /// The inner arrays of <b>NeighborOffsets</b> contain the number of index positions 
        /// indicated by the <see cref="Connectivity"/> of the current <see cref="Element"/>.
        /// </para><para>
        /// The array element at index position [<em>i</em>][<em>j</em>] contains the coordinate
        /// offsets to reach the neighboring location on edge <em>j</em> when the current location
        /// resides in an odd- or even-numbered row or column, as indicated by <em>i</em>. Counting
        /// starts at the topmost edge if <see cref="RegularPolygon.HasTopIndex"/> is <c>true</c>
        /// and with the edge to the right of the topmost vertex otherwise, continuing clockwise.
        /// </para><para>
        /// If <see cref="RegularPolygon.VertexNeighbors"/> is <c>true</c> for the current <see
        /// cref="Element"/>, the inner arrays instead contain the offsets to the neighboring
        /// locations on all edges and vertices in an alternating sequence. Counting starts with the
        /// topmost edge for <see cref="PolygonOrientation.OnEdge"/> orientation and with the
        /// topmost vertex otherwise, continuing clockwise.
        /// </para><para>
        /// Use <see cref="GetNeighborOffsets"/> to determine the correct inner array for a given
        /// location. You may also call the <see cref="GetNeighbor"/> and <see cref="GetNeighbors"/>
        /// methods to directly find one or more neighbors of a given location.
        /// </para><para>
        /// <b>NeighborOffsets</b> is recalculated automatically whenever the <see cref="Element"/>
        /// or <see cref="GridShift"/> properties change.</para></remarks>

        public PointI[][] NeighborOffsets {
            [DebuggerStepThrough]
            get { return _data.NeighborOffsets; }
        }

        #endregion
        #region Size

        /// <summary>
        /// Gets or sets the number of rows and columns in the <see cref="PolygonGrid"/>.</summary>
        /// <value>
        /// A <see cref="SizeI"/> value indicating the number of <see cref="Element"/> rows and
        /// columns in the <see cref="PolygonGrid"/>.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The property is set to a <see cref="SizeI"/> whose <see cref="SizeI.Width"/> or <see
        /// cref="SizeI.Height"/> is zero or negative.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="PolygonGrid"/> is read-only.</exception>
        /// <remarks>
        /// Setting <b>Size</b> also recalculates the <see cref="DisplayBounds"/> property.
        /// </remarks>

        public SizeI Size {
            [DebuggerStepThrough]
            get { return _data.Size; }
            set {
                if (IsReadOnly)
                    ThrowHelper.ThrowNotSupportedException(Strings.ViewReadOnly);

                if (value.Width <= 0 || value.Height <= 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(
                        "value", value, Strings.ArgumentContainsNegative);

                _data.Size = value;
                OnSizeChanged();
            }
        }

        #endregion
        #endregion
        #region Private Methods
        #region DisplayToGridCore

        /// <summary>
        /// Converts the specified coordinate pair from display to <see cref="PolygonGrid"/>
        /// coordinates.</summary>
        /// <param name="x">
        /// The x-coordinate to convert.</param>
        /// <param name="y">
        /// The y-coordinate to convert.</param>
        /// <returns>
        /// The <see cref="PolygonGrid"/> location of the <see cref="Element"/> whose shape contains
        /// the specified <paramref name="x"/> and <paramref name="y"/> display coordinates.
        /// </returns>
        /// <remarks><para>
        /// <b>DisplayToGridCore</b> does not check whether the specified <paramref name="x"/> and
        /// <paramref name="y"/> coordinates are within the current <see cref="DisplayBounds"/>.
        /// </para><para>
        /// Therefore, the returned grid coordinates may be less than zero or greater than the
        /// current <see cref="Size"/> minus one. In that case, they indicate elements on a 
        /// hypothetical extension of the actual <see cref="PolygonGrid"/>.</para></remarks>

        private PointI DisplayToGridCore(double x, double y) {

            int column, row, index;
            double width = Element.Bounds.Width, width2 = width / 2.0,
                height = Element.Bounds.Height, height2 = height / 2.0;

            double assertEpsilon = 0.0;
#if DEBUG
            // determine suitable comparison epsilon for assertions
            assertEpsilon = Math.Min(width / 100.0, height / 100.0);
#endif

            if (Element.Sides == 4) {

                // squares on edge: all coordinates are valid
                if (Element.Orientation == PolygonOrientation.OnEdge)
                    return new PointI((int) (x / width), (int) (y / height));

                if (AreColumnsShifted) {
                    // determine closest element
                    column = (int) (x / width2);
                    if (IsDownColumn(column)) {
                        index = 1;
                        row = (int) ((y - height2) / height);
                    }
                    else {
                        index = 0;
                        row = (int) (y / height);
                    }

                    // offset to center of element
                    x -= (column + 1) * width2;
                    y -= (2 * row + index + 1) * height2;

                    Debug.Assert(MathUtility.Compare(x, -width2, assertEpsilon) >= 0);
                    Debug.Assert(MathUtility.Compare(x, 0, assertEpsilon) <= 0);

                    // check if we hit element or neighbor
                    if (Math.Abs(y) <= x + width2)
                        return new PointI(column, row);
                    else {
                        PointI offset = EdgeNeighborOffsets[index][y < 0 ? 3 : 2];
                        return new PointI(column + offset.X, row + offset.Y);
                    }
                }
                else {
                    // determine closest element
                    row = (int) (y / height2);
                    if (IsRightRow(row)) {
                        index = 1;
                        column = (int) ((x - width2) / width);
                    }
                    else {
                        index = 0;
                        column = (int) (x / width);
                    }

                    // offset to center of element
                    x -= (2 * column + index + 1) * width2;
                    y -= (row + 1) * height2;

                    Debug.Assert(MathUtility.Compare(y, -height2, assertEpsilon) >= 0);
                    Debug.Assert(MathUtility.Compare(y, 0, assertEpsilon) <= 0);

                    // check if we hit element or neighbor
                    if (Math.Abs(x) <= y + height2)
                        return new PointI(column, row);
                    else {
                        PointI offset = EdgeNeighborOffsets[index][x < 0 ? 3 : 0];
                        return new PointI(column + offset.X, row + offset.Y);
                    }
                }
            }
            else {
                Debug.Assert(Element.Sides == 6);
                double width4 = width / 4.0, height4 = height / 4.0;

                if (AreColumnsShifted) {
                    // determine closest element
                    column = (int) (x / (3.0 * width4));
                    if (IsDownColumn(column)) {
                        index = 1;
                        row = (int) ((y - height2) / height);
                    }
                    else {
                        index = 0;
                        row = (int) (y / height);
                    }

                    // offset to center of element
                    x -= (3 * column + 2) * width4;
                    y -= (2 * row + index + 1) * height2;

                    Debug.Assert(MathUtility.Compare(x, -width2, assertEpsilon) >= 0);
                    Debug.Assert(MathUtility.Compare(x, width4, assertEpsilon) <= 0);

                    // check if we hit element or neighbor
                    if (Math.Abs(y) <= (x + width2) * PolygonGrid._sqrt3)
                        return new PointI(column, row);
                    else {
                        PointI offset = EdgeNeighborOffsets[index][y < 0 ? 5 : 4];
                        return new PointI(column + offset.X, row + offset.Y);
                    }
                }
                else {
                    // determine closest element
                    row = (int) (y / (3.0 * height4));
                    if (IsRightRow(row)) {
                        index = 1;
                        column = (int) ((x - width2) / width);
                    }
                    else {
                        index = 0;
                        column = (int) (x / width);
                    }

                    // offset to center of element
                    x -= (2 * column + index + 1) * width2;
                    y -= (3 * row + 2) * height4;

                    Debug.Assert(MathUtility.Compare(y, -height2, assertEpsilon) >= 0);
                    Debug.Assert(MathUtility.Compare(y, height4, assertEpsilon) <= 0);

                    // check if we hit element or neighbor
                    if (Math.Abs(x) <= (y + height2) * PolygonGrid._sqrt3)
                        return new PointI(column, row);
                    else {
                        PointI offset = EdgeNeighborOffsets[index][x < 0 ? 5 : 0];
                        return new PointI(column + offset.X, row + offset.Y);
                    }
                }
            }
        }

        #endregion
        #region OnGeometryChanged

        /// <summary>
        /// Updates all properties that depend on <see cref="Element"/> and <see cref="GridShift"/>.
        /// </summary>
        /// <remarks><para>
        /// <b>OnGeometryChanged</b> recalculates the <see cref="CenterDistance"/>, <see
        /// cref="EdgeNeighborOffsets"/>, and <see cref="NeighborOffsets"/> properties, based on the
        /// current values of the <see cref="Element"/> and <see cref="GridShift"/> properties.
        /// </para><para>
        /// <b>OnGeometryChanged</b> also invokes <see cref="OnSizeChanged"/> to recalculate all
        /// dependent properties accordingly.</para></remarks>

        private void OnGeometryChanged() {

            if (Element.Sides == 4) {
                PointI[][][] offsets;

                if (Element.Orientation == PolygonOrientation.OnEdge) {
                    double side = Element.Length;
                    _data.CenterDistance = new SizeD(side, side);
                    offsets = PolygonGrid._squareEdgeOffsets;
                }
                else {
                    SizeD element = Element.Bounds.Size;

                    if (AreColumnsShifted) {
                        _data.CenterDistance = new SizeD(element.Width / 2.0, element.Height);
                        offsets = PolygonGrid._squareVertexColumnOffsets;
                    }
                    else {
                        _data.CenterDistance = new SizeD(element.Width, element.Height / 2.0);
                        offsets = PolygonGrid._squareVertexRowOffsets;
                    }
                }

                _data.NeighborOffsets = offsets[Element.VertexNeighbors ? 1 : 0];
                _data.EdgeNeighborOffsets = offsets[0];
            }
            else {
                Debug.Assert(Element.Sides == 6);

                double edgeDistance = 2.0 * Element.InnerRadius;
                double vertexDistance = (3.0 * Element.OuterRadius) / 2.0;

                if (AreColumnsShifted) {
                    _data.CenterDistance = new SizeD(vertexDistance, edgeDistance);
                    _data.NeighborOffsets = PolygonGrid._hexagonEdgeOffsets;
                }
                else {
                    _data.CenterDistance = new SizeD(edgeDistance, vertexDistance);
                    _data.NeighborOffsets = PolygonGrid._hexagonVertexOffsets;
                }

                _data.EdgeNeighborOffsets = _data.NeighborOffsets;
            }

            OnSizeChanged();
        }

        #endregion
        #region OnSizeChanged

        /// <summary>
        /// Updates all properties that depend on <see cref="Element"/>, <see cref="GridShift"/>,
        /// and <see cref="Size"/>.</summary>
        /// <remarks>
        /// <b>OnSizeChanged</b> recalculates the <see cref="DisplayBounds"/> property, based on the
        /// current values of the <see cref="Element"/>, <see cref="GridShift"/>, and <see
        /// cref="Size"/> properties.</remarks>

        private void OnSizeChanged() {

            // check for calls before Size was set
            if (Size == SizeI.Empty) {
                _data.DisplayBounds = RectD.Empty;
                return;
            }

            // compute display bounds without overhang
            SizeD element = Element.Bounds.Size;
            double width = element.Width + (Size.Width - 1) * CenterDistance.Width,
                height = element.Height + (Size.Height - 1) * CenterDistance.Height;

            // add overhang for shifted rows or columns
            switch (GridShift) {

                case PolygonGridShift.ColumnUp:
                case PolygonGridShift.ColumnDown:
                    height += element.Height / 2.0;
                    break;

                case PolygonGridShift.RowLeft:
                case PolygonGridShift.RowRight:
                    width += element.Width / 2.0;
                    break;
            }

            _data.DisplayBounds = new RectD(0.0, 0.0, width, height);
        }

        #endregion
        #endregion
        #region Public Methods
        #region AreCompatible

        /// <summary>
        /// Determines whether the specified <see cref="RegularPolygon"/> is compatible with the
        /// specified <see cref="PolygonGridShift"/> value.</summary>
        /// <param name="polygon">
        /// The <see cref="RegularPolygon"/> to test.</param>
        /// <param name="gridShift">
        /// The <see cref="PolygonGridShift"/> value to test.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="polygon"/> and <paramref name="gridShift"/>
        /// value could be assigned to the <see cref="Element"/> and <see cref="GridShift"/>
        /// properties of the same <see cref="PolygonGrid"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="polygon"/> is neither a square nor a hexagon.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="polygon"/> is a null reference.</exception>
        /// <exception cref="InvalidEnumArgumentException">
        /// <paramref name="gridShift"/> is not a valid <see cref="PolygonGridShift"/> value.
        /// </exception>

        public static bool AreCompatible(RegularPolygon polygon, PolygonGridShift gridShift) {
            if (polygon == null)
                ThrowHelper.ThrowArgumentNullException("polygon");

            bool isSquare = (polygon.Sides == 4), isHexagon = (polygon.Sides == 6),
                onEdge = (polygon.Orientation == PolygonOrientation.OnEdge),
                onVertex = (polygon.Orientation == PolygonOrientation.OnVertex);

            if (!isSquare && !isHexagon)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "polygon", Strings.ArgumentPropertyInvalid, "Sides");

            switch (gridShift) {

                case PolygonGridShift.None:
                    return (isSquare && onEdge);

                case PolygonGridShift.ColumnUp:
                case PolygonGridShift.ColumnDown:
                    return ((isSquare && onVertex) || (isHexagon && onEdge));

                case PolygonGridShift.RowLeft:
                case PolygonGridShift.RowRight:
                    return (onVertex && (isSquare || isHexagon));

                default:
                    ThrowHelper.ThrowInvalidEnumArgumentException(
                        "gridShift", (int) gridShift, typeof(PolygonGridShift));
                    return false;
            }
        }

        #endregion
        #region AsReadOnly

        /// <summary>
        /// Returns a read-only view of the <see cref="PolygonGrid"/>.</summary>
        /// <returns>
        /// A read-only wrapper around the <see cref="PolygonGrid"/>.</returns>
        /// <remarks><para>
        /// Attempting to modify the read-only wrapper returned by <b>AsReadOnly</b> will raise a
        /// <see cref="NotSupportedException"/>. Note that the original <see cref="PolygonGrid"/>
        /// may still change, and any such changes will be reflected in the read-only view.
        /// </para><para>
        /// <b>AsReadOnly</b> buffers the newly created read-only wrapper when the method is first
        /// called, and returns the buffered value on subsequent calls. When invoked on a read-only
        /// view, <b>AsReadOnly</b> returns the instance it is invoked on.</para></remarks>

        public PolygonGrid AsReadOnly() {

            if (_readOnlyWrapper == null)
                _readOnlyWrapper = new PolygonGrid(_data);

            return _readOnlyWrapper;
        }

        #endregion
        #region CreateArray<T>

        /// <summary>
        /// Creates a two-dimensional <see cref="Array"/> with the same <see cref="Size"/> as the
        /// <see cref="PolygonGrid"/>.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the returned <see cref="Array"/>.</typeparam>
        /// <returns>
        /// A two-dimensional <see cref="Array"/> of element type <typeparamref name="T"/> whose
        /// dimensions equal the current <see cref="Size"/>.</returns>
        /// <exception cref="NotSupportedException">
        /// <typeparamref name="T"/> is not supported for <see cref="Array"/> creation.</exception>
        /// <remarks>
        /// <b>CreateArray</b> maps the <see cref="SizeI.Width"/> of the <see cref="PolygonGrid"/>
        /// to the first dimension of the returned <see cref="Array"/>, and the <see
        /// cref="SizeI.Height"/> to the second dimension.</remarks>

        public T[,] CreateArray<T>() {
            return (T[,]) Array.CreateInstance(typeof(T), Size.Width, Size.Height);
        }

        #endregion
        #region CreateArrayEx<T>

        /// <summary>
        /// Creates a two-dimensional <see cref="ArrayEx{T}"/> with the same <see cref="Size"/> as
        /// the <see cref="PolygonGrid"/>.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the returned <see cref="ArrayEx{T}"/>.</typeparam>
        /// <returns>
        /// A two-dimensional <see cref="ArrayEx{T}"/> of element type <typeparamref name="T"/>
        /// whose dimensions equal the current <see cref="Size"/>.</returns>
        /// <exception cref="NotSupportedException">
        /// <typeparamref name="T"/> is not supported for <see cref="Array"/> creation.</exception>
        /// <remarks>
        /// <b>CreateArrayEx</b> maps the <see cref="SizeI.Width"/> of the <see cref="PolygonGrid"/>
        /// to the first dimension of the returned <see cref="ArrayEx{T}"/>, and the <see
        /// cref="SizeI.Height"/> to the second dimension.</remarks>

        public ArrayEx<T> CreateArrayEx<T>() {
            return new ArrayEx<T>(Size.Width, Size.Height);
        }

        #endregion
        #region DisplayToGrid(Double, Double)

        /// <overloads>
        /// Converts the specified display location to <see cref="PolygonGrid"/> coordinates.
        /// </overloads>
        /// <summary>
        /// Converts the specified coordinate pair from display to <see cref="PolygonGrid"/>
        /// coordinates.</summary>
        /// <param name="x">
        /// The x-coordinate to convert.</param>
        /// <param name="y">
        /// The y-coordinate to convert.</param>
        /// <returns><para>
        /// The <see cref="PolygonGrid"/> location of the <see cref="Element"/> whose shape contains
        /// the specified <paramref name="x"/> and <paramref name="y"/> display coordinates.
        /// </para><para>-or-</para><para>
        /// The constant value <see cref="InvalidLocation"/> if no such <see cref="Element"/>
        /// exists.</para></returns>
        /// <remarks>
        /// <b>DisplayToGrid</b> returns coordinates between (0,0) and one less than the current
        /// <see cref="Size"/> in either dimension if the specified coordinates are within the
        /// current <see cref="DisplayBounds"/>, and the constant value <see
        /// cref="InvalidLocation"/> otherwise.</remarks>

        public PointI DisplayToGrid(double x, double y) {
            /*
             * We first check for obviously invalid display coordinates.
             * 
             * However, DisplayBounds may include a small number of pixels
             * between convex border polygons that don’t hit any polygon,
             * so we need to check again when DisplayToGridCore returns.
             */

            if (!DisplayBounds.Contains(x, y))
                return InvalidLocation;

            PointI element = DisplayToGridCore(x, y);
            return (Contains(element) ? element : InvalidLocation);
        }

        #endregion
        #region DisplayToGrid(PointD)

        /// <summary>
        /// Converts the specified <see cref="PointD"/> from display to <see cref="PolygonGrid"/> 
        /// coordinates.</summary>
        /// <param name="location">
        /// The <see cref="PointD"/> to convert.</param>
        /// <returns><para>
        /// The <see cref="PolygonGrid"/> location of the <see cref="Element"/> whose shape contains
        /// the specified display <paramref name="location"/>.
        /// </para><para>-or-</para><para>
        /// The constant value <see cref="InvalidLocation"/> if no such <see cref="Element"/>
        /// exists.</para></returns>
        /// <remarks>
        /// <b>DisplayToGrid</b> returns coordinates between (0,0) and one less than the current
        /// <see cref="Size"/> in either dimension if the specified <paramref name="location"/> is
        /// within the current <see cref="DisplayBounds"/>, and the constant value <see
        /// cref="InvalidLocation"/> otherwise.</remarks>

        public PointI DisplayToGrid(PointD location) {
            return DisplayToGrid(location.X, location.Y);
        }

        #endregion
        #region DisplayToGridClipped(Double, Double)

        /// <overloads>
        /// Converts the specified display location to <see cref="PolygonGrid"/> coordinates,
        /// clipping to the nearest element if necessary.</overloads>
        /// <summary>
        /// Converts the specified coordinate pair from display to <see cref="PolygonGrid"/>
        /// coordinates, clipping to the nearest element if necessary.</summary>
        /// <param name="x">
        /// The x-coordinate to convert.</param>
        /// <param name="y">
        /// The y-coordinate to convert.</param>
        /// <returns><para>
        /// The <see cref="PolygonGrid"/> location of the <see cref="Element"/> whose shape contains
        /// the specified <paramref name="x"/> and <paramref name="y"/> display coordinates.
        /// </para><para>-or-</para><para>
        /// The <see cref="PolygonGrid"/> location nearest to the specified <paramref name="x"/> and
        /// <paramref name="y"/> display coordinates if no such <see cref="Element"/> exists.
        /// </para></returns>
        /// <remarks>
        /// <b>DisplayToGridClipped</b> always returns coordinates between (0,0) and one less than
        /// the current <see cref="Size"/> in either dimension, regardless of whether the specified
        /// coordinates are within the current <see cref="DisplayBounds"/>.</remarks>

        public PointI DisplayToGridClipped(double x, double y) {
            /*
             * DisplayToGridCore maps invalid display coordinates to elements
             * in a hypothetically extended grid. When the resulting grid
             * coordinates are clipped, the resulting actual elements may not 
             * be the visually nearest to the original display coordinates.
             * 
             * We get better results by moving display coordinates near the grid
             * border inward by half a polygon diameter before translating them.
             * This makes them valid and usually guarantees that they get mapped
             * to the visually nearest actual grid elements.
             */

            double marginX = Element.Bounds.Width / 2.0,
                marginY = Element.Bounds.Height / 2.0;

            if (x <= marginX) x = marginX + 1.0;
            else if (x >= DisplayBounds.Width - marginX)
                x = DisplayBounds.Width - marginX - 1.0;

            if (y <= marginY) y = marginY + 1.0;
            else if (y >= DisplayBounds.Height - marginY)
                y = DisplayBounds.Height - marginY - 1.0;

            PointI element = DisplayToGridCore(x, y);
            return element.Restrict(0, 0, Size.Width - 1, Size.Height - 1);
        }

        #endregion
        #region DisplayToGridClipped(PointD)

        /// <summary>
        /// Converts the specified <see cref="PointD"/> from display to <see cref="PolygonGrid"/> 
        /// coordinates, clipping to the nearest element if necessary.</summary>
        /// <param name="location">
        /// The <see cref="PointD"/> to convert.</param>
        /// <returns><para>
        /// The <see cref="PolygonGrid"/> location of the <see cref="Element"/> whose shape contains
        /// the specified display <paramref name="location"/>.
        /// </para><para>-or-</para><para>
        /// The <see cref="PolygonGrid"/> location nearest to the specified display <paramref
        /// name="location"/> if no such <see cref="Element"/> exists.</para></returns>
        /// <remarks>
        /// <b>DisplayToGridClipped</b> always returns coordinates between (0,0) and one less than
        /// the current <see cref="Size"/> in either dimension, regardless of whether the specified
        /// <paramref name="location"/> is within the current <see cref="DisplayBounds"/>.</remarks>

        public PointI DisplayToGridClipped(PointD location) {
            return DisplayToGridClipped(location.X, location.Y);
        }

        #endregion
        #region GetEdgeNeighborOffsets

        /// <summary>
        /// Gets the inner array within <see cref="EdgeNeighborOffsets"/> that matches the specified
        /// location.</summary>
        /// <param name="location">
        /// The coordinates of the location to examine.</param>
        /// <returns>
        /// The inner array within <see cref="EdgeNeighborOffsets"/> that matches the specified
        /// <paramref name="location"/>.</returns>
        /// <remarks><para>
        /// <b>GetEdgeNeighborOffsets</b> does not check whether the specified <paramref
        /// name="location"/> is actually within the <see cref="PolygonGrid"/>.
        /// </para><para>
        /// Please refer to <see cref="EdgeNeighborOffsets"/> and <see cref="NeighborOffsets"/> for
        /// a description of the storage format used for neighbor coordinate offsets.
        /// </para></remarks>

        public PointI[] GetEdgeNeighborOffsets(PointI location) {
            int index = ((IsRightRow(location.Y) || IsDownColumn(location.X)) ? 1 : 0);
            return EdgeNeighborOffsets[index];
        }

        #endregion
        #region GetElementBounds(Int32, Int32)

        /// <overloads>
        /// Gets the bounding rectangle of the <see cref="Element"/> at the specified <see
        /// cref="PolygonGrid"/> location.</overloads>
        /// <summary>
        /// Gets the bounding rectangle of the <see cref="Element"/> at the specified column and row
        /// indices.</summary>
        /// <param name="column">
        /// The zero-based index of a <see cref="PolygonGrid"/> column.</param>
        /// <param name="row">
        /// The zero-based index of a <see cref="PolygonGrid"/> row.</param>
        /// <returns>
        /// The <see cref="RectD"/> that circumscribes the <see cref="Element"/> shape at the
        /// specified <paramref name="column"/> and <paramref name="row"/>.</returns>
        /// <remarks>
        /// <b>GetElementBounds</b> always returns a region within the current <see
        /// cref="DisplayBounds"/> if the specified coordinates are within the <see
        /// cref="PolygonGrid"/>, but does not check whether that is the case.</remarks>

        public RectD GetElementBounds(int column, int row) {

            // determine center of specified element
            PointD center = GridToDisplay(column, row);

            // offset element bounds by center point
            return Element.Bounds.Offset(center);
        }

        #endregion
        #region GetElementBounds(PointI)

        /// <summary>
        /// Gets the bounding rectangle of the <see cref="Element"/> at the specified <see
        /// cref="PointI"/> location.</summary>
        /// <param name="location">
        /// The coordinates of the <see cref="PolygonGrid"/> location to examine.</param>
        /// <returns>
        /// The <see cref="RectD"/> that circumscribes the <see cref="Element"/> shape at the
        /// specified <paramref name="location"/>.</returns>
        /// <remarks>
        /// <b>GetElementBounds</b> always returns a region within the current <see
        /// cref="DisplayBounds"/> if the specified <paramref name="location"/> is within the <see
        /// cref="PolygonGrid"/>, but does not check whether that is the case.</remarks>

        public RectD GetElementBounds(PointI location) {
            return GetElementBounds(location.X, location.Y);
        }

        #endregion
        #region GetElementBounds(RectI)

        /// <summary>
        /// Gets the bounding rectangle of all <see cref="Element"/> shapes within the specified
        /// region.</summary>
        /// <param name="region">
        /// A <see cref="RectI"/> comprising the coordinates of all <see cref="PolygonGrid"/>
        /// locations to examine.</param>
        /// <returns>
        /// The <see cref="RectD"/> that circumscribes all <see cref="Element"/> shapes within the
        /// specified <paramref name="region"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="region"/> contains a <see cref="RectI.Width"/> or <see
        /// cref="RectI.Height"/> that is zero or negative.</exception>
        /// <remarks>
        /// <b>GetElementBounds</b> always returns a region within the current <see
        /// cref="DisplayBounds"/> if the specified <paramref name="region"/> is fully within the
        /// <see cref="PolygonGrid"/>, but does not check whether that is the case.</remarks>

        public RectD GetElementBounds(RectI region) {
            if (region.Width <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "region.Width", region.Width, Strings.ArgumentNotPositive);

            if (region.Height <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "region.Height", region.Height, Strings.ArgumentNotPositive);

            // check for single-element region
            if (region.Width == 1 && region.Height == 1)
                return GetElementBounds(region.X, region.Y);

            double width = Element.Bounds.Width, width2 = width / 2.0,
                height = Element.Bounds.Height, height2 = height / 2.0;

            // compute bounding rectangle without overhang
            PointD location = GridToDisplay(region.X, region.Y);
            double left = location.X - width2;
            double top = location.Y - height2;
            width += (region.Width - 1) * CenterDistance.Width;
            height += (region.Height - 1) * CenterDistance.Height;

            // add overhang for shifted rows or columns
            switch (GridShift) {

                case PolygonGridShift.ColumnUp:
                case PolygonGridShift.ColumnDown:
                    if (region.Width > 1) {
                        height += height2;
                        if (IsDownColumn(region.X)) top -= height2;
                    }
                    break;

                case PolygonGridShift.RowLeft:
                case PolygonGridShift.RowRight:
                    if (region.Height > 1) {
                        width += width2;
                        if (IsRightRow(region.Y)) left -= width2;
                    }
                    break;
            }

            return new RectD(left, top, width, height);
        }

        #endregion
        #region GetElementVertices

        /// <summary>
        /// Gets the polygon vertices of the <see cref="Element"/> at the specified <see
        /// cref="PolygonGrid"/> location.</summary>
        /// <param name="column">
        /// The zero-based index of a <see cref="PolygonGrid"/> column.</param>
        /// <param name="row">
        /// The zero-based index of a <see cref="PolygonGrid"/> row.</param>
        /// <returns>
        /// The <see cref="RegularPolygon.Vertices"/> of the <see cref="Element"/> shape at the
        /// specified <paramref name="column"/> and <paramref name="row"/>.</returns>
        /// <remarks>
        /// <b>GetElementVertices</b> shifts all <see cref="RegularPolygon.Vertices"/> of an <see
        /// cref="Element"/> by the result of <see cref="GridToDisplay"/> for the specified
        /// <paramref name="column"/> and <paramref name="row"/>. The grid location is not checked
        /// against the bounds of the <see cref="PolygonGrid"/>.</remarks>

        public PointD[] GetElementVertices(int column, int row) {

            PointD[] vertices = Element.Vertices;
            PointD[] shiftedVertices = new PointD[vertices.Length];

            // shift vertices to center of specified element
            PointD center = GridToDisplay(column, row);
            for (int i = 0; i < vertices.Length; i++)
                shiftedVertices[i] = vertices[i] + center;

            return shiftedVertices;
        }

        #endregion
        #region GetNeighbor

        /// <summary>
        /// Returns the coordinates of the location that borders the specified location on the
        /// specified edge or vertex.</summary>
        /// <param name="location">
        /// The coordinates of the location whose neighbor to return.</param>
        /// <param name="index">
        /// A zero-based index for the inner arrays within <see cref="NeighborOffsets"/>, indicating
        /// an edge or vertex of the specified <paramref name="location"/>.</param>
        /// <returns>
        /// The coordinates of the location that borders the specified <paramref name="location"/>
        /// on the edge or vertex indicated by the specified <paramref name="index"/>.</returns>
        /// <remarks><para>
        /// <b>GetNeighbor</b> does not check whether the specified <paramref name="location"/> or
        /// the returned coordinates are actually within the <see cref="PolygonGrid"/>. You must
        /// perform your own coordinate validation if desired.
        /// </para><para>
        /// The specified <paramref name="index"/> is taken <see cref="Fortran.Modulo"/> the length
        /// of the inner arrays within <see cref="NeighborOffsets"/>, and may therefore be negative
        /// or greater than the maximum index.
        /// </para><para>
        /// Please refer to <see cref="NeighborOffsets"/> for a description of the index order.
        /// </para></remarks>

        public PointI GetNeighbor(PointI location, int index) {

            // offset location by normalized index in its group
            PointI[] offsets = GetNeighborOffsets(location);
            index = Fortran.Modulo(index, offsets.Length);
            return location + offsets[index];
        }

        #endregion
        #region GetNeighborIndex

        /// <summary>
        /// Returns the edge or vertex on which the specified location borders another neighboring
        /// location.</summary>
        /// <param name="location">
        /// The coordinates of the location whose edge or vertex to return.</param>
        /// <param name="neighbor">
        /// The coordinates of a neighboring location.</param>
        /// <returns>
        /// A zero-based index for the inner arrays within <see cref="NeighborOffsets"/>, indicating
        /// the edge or vertex of the specified <paramref name="location"/> on which it borders the
        /// specified <paramref name="neighbor"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="location"/> and <paramref name="neighbor"/> are not coordinates of
        /// neighboring locations in the <see cref="PolygonGrid"/>.</exception>
        /// <remarks><para>
        /// <b>GetNeighborIndex</b> does not check whether the specified <paramref name="location"/> 
        /// or <paramref name="neighbor"/> are actually within the <see cref="PolygonGrid"/>. You
        /// must perform your own coordinate validation if desired.
        /// </para><para>
        /// <b>GetNeighborIndex</b> is the inverse of the <see cref="GetNeighbor"/> method. That is,
        /// the following relations hold for all locations <em>p</em> with valid neighbors
        /// <em>q</em> and neighbor indices <em>i</em>:
        /// </para><para><code>
        /// GetNeighbor(p, GetNeighborIndex(p, q)) == q;<br/>
        /// GetNeighborIndex(p, GetNeighbor(p, i)) == i;</code>
        /// </para><para>
        /// Please refer to <see cref="NeighborOffsets"/> for a description of the index order.
        /// </para></remarks>

        public int GetNeighborIndex(PointI location, PointI neighbor) {

            // determine actual offset of specified neighbor
            PointI offset = new PointI(neighbor.X - location.X, neighbor.Y - location.Y);

            // determine offset group for given location
            PointI[] offsets = GetNeighborOffsets(location);

            // try to find neighbor offset in group
            for (int i = 0; i < offsets.Length; i++)
                if (offsets[i] == offset)
                    return i;

            ThrowHelper.ThrowArgumentException("neighbor", Strings.ArgumentNotNeighbor);
            return -1;
        }

        #endregion
        #region GetNeighborOffsets

        /// <summary>
        /// Gets the inner array within <see cref="NeighborOffsets"/> that matches the specified
        /// location.</summary>
        /// <param name="location">
        /// The coordinates of the location to examine.</param>
        /// <returns>
        /// The inner array within <see cref="NeighborOffsets"/> that matches the specified
        /// <paramref name="location"/>.</returns>
        /// <remarks><para>
        /// <b>GetNeighborOffsets</b> does not check whether the specified <paramref
        /// name="location"/> is actually within the <see cref="PolygonGrid"/>.
        /// </para><para>
        /// Please refer to <see cref="NeighborOffsets"/> for a description of the storage format
        /// used for neighbor coordinate offsets.</para></remarks>

        public PointI[] GetNeighborOffsets(PointI location) {
            int index = ((IsRightRow(location.Y) || IsDownColumn(location.X)) ? 1 : 0);
            return NeighborOffsets[index];
        }

        #endregion
        #region GetStepDistance

        /// <summary>
        /// Returns the distance between the two specified locations, in movement steps.</summary>
        /// <param name="source">
        /// The coordinates of the source location.</param>
        /// <param name="target">
        /// The coordinates of the target location.</param>
        /// <returns>
        /// The non-negative distance between <paramref name="source"/> and <paramref
        /// name="target"/>, measured in movement steps.</returns>
        /// <remarks><para>
        /// <b>GetStepDistance</b> returns zero if the specified <paramref name="source"/> and
        /// <paramref name="target"/> are identical, and the minimum number of location transitions
        /// required to move from <paramref name="source"/> to <paramref name="target"/> otherwise.
        /// </para><para>
        /// <b>GetStepDistance</b> does not check whether the <see cref="PolygonGrid"/> actually
        /// contains the specified <paramref name="source"/> and <paramref name="target"/>
        /// locations.
        /// </para><para>
        /// All distance calculations are O(1) operations, regardless of the concrete values of
        /// <paramref name="source"/> and <paramref name="target"/>. The calculations for hexagon
        /// grids were adopted from a Usenet post by Matthew V. Jessick.</para></remarks>

        public int GetStepDistance(PointI source, PointI target) {
            if (source == target) return 0;

            int signDeltaX = target.X - source.X;
            int signDeltaY = target.Y - source.Y;

            if (Element.Sides == 4) {
                int deltaX = Math.Abs(signDeltaX);
                int deltaY = Math.Abs(signDeltaY);

                if (Element.Orientation == PolygonOrientation.OnEdge)
                    return (Element.VertexNeighbors ?
                        Math.Max(deltaX, deltaY) : deltaX + deltaY);

                if (AreColumnsShifted) {
                    if (Element.VertexNeighbors) {
                        int adjustX = (IsDownColumn(source.X) ?
                            (signDeltaY <= 0 ? 1 : 0) :
                            (signDeltaY >= 0 ? 1 : 0));

                        return deltaX / 2 + deltaY + adjustX * (deltaX % 2);
                    } else {
                        if (2 * deltaY <= deltaX) return deltaX;

                        Debug.Assert(signDeltaY != 0);
                        int adjustX = (IsDownColumn(source.X) ?
                            (signDeltaY < 0 ? 1 : -1) :
                            (signDeltaY > 0 ? 1 : -1));

                        return 2 * deltaY + adjustX * (deltaX % 2);
                    }
                } else {
                    if (Element.VertexNeighbors) {
                        int adjustY = (IsRightRow(source.Y) ?
                            (signDeltaX <= 0 ? 1 : 0) :
                            (signDeltaX >= 0 ? 1 : 0));

                        return deltaY / 2 + deltaX + adjustY * (deltaY % 2);
                    } else {
                        if (2 * deltaX <= deltaY) return deltaY;

                        Debug.Assert(signDeltaX != 0);
                        int adjustY = (IsRightRow(source.Y) ?
                            (signDeltaX < 0 ? 1 : -1) :
                            (signDeltaX > 0 ? 1 : -1));

                        return 2 * deltaX + adjustY * (deltaY % 2);
                    }
                }
            } else {
                Debug.Assert(Element.Sides == 6);

                if (AreColumnsShifted) {
                    int deltaX = Math.Abs(signDeltaX);
                    signDeltaY -= (IsUpColumn(source.X) ? deltaX : deltaX + 1) / 2;
                    int deltaY = Math.Abs(signDeltaY);

                    return (signDeltaY < 0 ? Math.Max(deltaX, deltaY) : deltaX + deltaY);
                } else {
                    int deltaY = Math.Abs(signDeltaY);
                    signDeltaX -= (IsLeftRow(source.Y) ? deltaY : deltaY + 1) / 2;
                    int deltaX = Math.Abs(signDeltaX);

                    return (signDeltaX < 0 ? Math.Max(deltaX, deltaY) : deltaX + deltaY);
                }
            }
        }

        #endregion
        #region GridToDisplay(Int32, Int32)

        /// <overloads>
        /// Converts the specified <see cref="PolygonGrid"/> location to display coordinates.
        /// </overloads>
        /// <summary>
        /// Converts the specified column and row indices to display coordinates.</summary>
        /// <param name="column">
        /// The zero-based index of a <see cref="PolygonGrid"/> column.</param>
        /// <param name="row">
        /// The zero-based index of a <see cref="PolygonGrid"/> row.</param>
        /// <returns>
        /// The display coordinates of the center of the <see cref="Element"/> shape at the
        /// specified <paramref name="column"/> and <paramref name="row"/>.</returns>
        /// <remarks>
        /// <b>GridToDisplay</b> always returns coordinates within the current <see
        /// cref="DisplayBounds"/> if the specified coordinates are within the <see
        /// cref="PolygonGrid"/>, but does not check whether that is the case.</remarks>

        public PointD GridToDisplay(int column, int row) {
            PointI factor; // to be multipled by (width/4, height/4)

            if (Element.Sides == 4) {
                if (Element.Orientation == PolygonOrientation.OnEdge)
                    factor = new PointI(4 * column + 2, 4 * row + 2);
                else
                    factor = (AreColumnsShifted ?
                        new PointI(2 * column + 2, 4 * row + (IsDownColumn(column) ? 4 : 2)) :
                        new PointI(4 * column + (IsRightRow(row) ? 4 : 2), 2 * row + 2));
            }
            else {
                Debug.Assert(Element.Sides == 6);

                factor = (AreColumnsShifted ?
                    new PointI(3 * column + 2, 4 * row + (IsDownColumn(column) ? 4 : 2)) :
                    new PointI(4 * column + (IsRightRow(row) ? 4 : 2), 3 * row + 2));
            }

            return new PointD(
                factor.X * Element.Bounds.Width / 4.0,
                factor.Y * Element.Bounds.Height / 4.0);
        }

        #endregion
        #region GridToDisplay(PointI)

        /// <summary>
        /// Converts the specified <see cref="PolygonGrid"/> location to display coordinates.
        /// </summary>
        /// <param name="location">
        /// The coordinates of the <see cref="PolygonGrid"/> location to convert.</param>
        /// <returns>
        /// The display coordinates of the center of the <see cref="Element"/> shape at the
        /// specified <paramref name="location"/> in the <see cref="PolygonGrid"/>.</returns>
        /// <remarks>
        /// <b>GridToDisplay</b> always returns coordinates within the current <see
        /// cref="DisplayBounds"/> if the specified <paramref name="location"/> is within the <see
        /// cref="PolygonGrid"/>, but does not check whether that is the case.</remarks>

        public PointD GridToDisplay(PointI location) {
            return GridToDisplay(location.X, location.Y);
        }

        #endregion
        #region IsDownColumn

        /// <summary>
        /// Determines whether the specified column is shifted down compared to the neighboring
        /// columns in the <see cref="PolygonGrid"/>.</summary>
        /// <param name="column">
        /// The zero-based index of a <see cref="PolygonGrid"/> column.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="column"/> is shifted downward compared to its neighbors,
        /// given the current <see cref="GridShift"/>; otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>IsDownColumn</b> does not check whether the specified <paramref name="column"/> is a
        /// valid column index within the <see cref="PolygonGrid"/>.
        /// </para><para>
        /// <b>IsDownColumn</b> returns <c>false</c> for all <paramref name="column"/> values if the
        /// current <see cref="GridShift"/> value is neither <see cref="PolygonGridShift.ColumnUp"/>
        /// nor <see cref="PolygonGridShift.ColumnDown"/>.</para></remarks>

        public bool IsDownColumn(int column) {
            switch (GridShift) {

                case PolygonGridShift.ColumnUp:
                    return ((column % 2) == 0);

                case PolygonGridShift.ColumnDown:
                    return ((column % 2) != 0);

                default:
                    return false;
            }
        }

        #endregion
        #region IsLeftRow

        /// <summary>
        /// Determines whether the specified row is shifted left compared to the neighboring rows in
        /// the <see cref="PolygonGrid"/>.</summary>
        /// <param name="row">
        /// The zero-based index of a <see cref="PolygonGrid"/> row.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="row"/> is shifted to the left compared to its neighbors,
        /// given the current <see cref="GridShift"/>; otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>IsLeftRow</b> does not check whether the specified <paramref name="row"/> is a valid
        /// column index within the <see cref="PolygonGrid"/>.
        /// </para><para>
        /// <b>IsLeftRow</b> returns <c>false</c> for all <paramref name="row"/> values if the
        /// current <see cref="GridShift"/> value is neither <see cref="PolygonGridShift.RowLeft"/>
        /// nor <see cref="PolygonGridShift.RowRight"/>.</para></remarks>

        public bool IsLeftRow(int row) {
            switch (GridShift) {

                case PolygonGridShift.RowLeft:
                    return ((row % 2) != 0);

                case PolygonGridShift.RowRight:
                    return ((row % 2) == 0);

                default:
                    return false;
            }
        }

        #endregion
        #region IsRightRow

        /// <summary>
        /// Determines whether the specified row is shifted right compared to the neighboring rows
        /// in the <see cref="PolygonGrid"/>.</summary>
        /// <param name="row">
        /// The zero-based index of a <see cref="PolygonGrid"/> row.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="row"/> is shifted to the right compared to its neighbors,
        /// given the current <see cref="GridShift"/>; otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>IsRightRow</b> does not check whether the specified <paramref name="row"/> is a valid
        /// column index within the <see cref="PolygonGrid"/>.
        /// </para><para>
        /// <b>IsRightRow</b> returns <c>false</c> for all <paramref name="row"/> values if the
        /// current <see cref="GridShift"/> value is neither <see cref="PolygonGridShift.RowLeft"/>
        /// nor <see cref="PolygonGridShift.RowRight"/>.</para></remarks>

        public bool IsRightRow(int row) {
            switch (GridShift) {

                case PolygonGridShift.RowLeft:
                    return ((row % 2) == 0);

                case PolygonGridShift.RowRight:
                    return ((row % 2) != 0);

                default:
                    return false;
            }
        }

        #endregion
        #region IsUpColumn

        /// <summary>
        /// Determines whether the specified column is shifted up compared to the neighboring
        /// columns in the <see cref="PolygonGrid"/>.</summary>
        /// <param name="column">
        /// The zero-based index of a <see cref="PolygonGrid"/> column.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="column"/> is shifted upward compared to its neighbors,
        /// given the current <see cref="GridShift"/>; otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>IsUpColumn</b> does not check whether the specified <paramref name="column"/> is a
        /// valid column index within the <see cref="PolygonGrid"/>.
        /// </para><para>
        /// <b>IsUpColumn</b> returns <c>false</c> for all <paramref name="column"/> values if the
        /// current <see cref="GridShift"/> value is neither <see cref="PolygonGridShift.ColumnUp"/>
        /// nor <see cref="PolygonGridShift.ColumnDown"/>.</para></remarks>

        public bool IsUpColumn(int column) {
            switch (GridShift) {

                case PolygonGridShift.ColumnUp:
                    return ((column % 2) != 0);

                case PolygonGridShift.ColumnDown:
                    return ((column % 2) == 0);

                default:
                    return false;
            }
        }

        #endregion
        #region ToSubdivision

        /// <summary>
        /// Converts the <see cref="PolygonGrid"/> to a planar <see cref="Subdivision"/>.</summary>
        /// <param name="offset">
        /// The offset by which to shift the <see cref="PolygonGrid"/>.</param>
        /// <param name="epsilon"><para>
        /// The maximum absolute difference at which two coordinates should be considered equal.
        /// </para><para>-or-</para><para>
        /// Zero to use a default value that depends on the current <see cref="Element"/> size.
        /// </para></param>
        /// <returns>
        /// A <see cref="SubdivisionMap"/> containing a new <see cref="Subdivision"/> whose bounded
        /// <see cref="Subdivision.Faces"/> each correspond to one <see cref="Element"/> of the <see
        /// cref="PolygonGrid"/>, shifted by the specified <paramref name="offset"/>.</returns>
        /// <remarks><para>
        /// <b>ToSubdivision</b> shifts the <see cref="RegularPolygon.Vertices"/> of each <see
        /// cref="Element"/> in the <see cref="PolygonGrid"/> by the <see cref="GridToDisplay"/>
        /// result for its grid location, plus the specified <paramref name="offset"/>. The <see
        /// cref="Subdivision"/> is then created from the resulting list of polygons.
        /// </para><para>
        /// The <see cref="Subdivision.Epsilon"/> of the new <see cref="Subdivision"/> is set to the
        /// specified <paramref name="epsilon"/>, if positive; otherwise, to one millionth of the
        /// <see cref="RegularPolygon.Length"/> of the current <see cref="Element"/>. We cannot use
        /// exact coordinate comparisons because shared vertices of adjacent grid elements are
        /// unlikely to evaluate to the exact same coordinates in all cases.
        /// </para><para>
        /// The returned <see cref="SubdivisionMap"/> also provides a mapping between <see
        /// cref="PolygonGrid"/> locations and the corresponding <see cref="Subdivision.Faces"/>.
        /// This mapping will become invalid as soon as either the <see cref="PolygonGrid"/> or the
        /// created <see cref="Subdivision"/> are changed.</para></remarks>

        public SubdivisionMap ToSubdivision(PointD offset, double epsilon = 0) {

            int width = Size.Width, height = Size.Height;
            var polygons = new PointD[width * height][];
            PointD[] vertices = Element.Vertices;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++) {
                    PointD[] polygon = new PointD[vertices.Length];

                    // shift polygon vertices by grid coordinates plus offset
                    PointD element = GridToDisplay(x, y);
                    for (int i = 0; i < polygon.Length; i++)
                        polygon[i] = vertices[i] + element + offset;

                    polygons[x * height + y] = polygon;
                }

            // use epsilon to match vertices of neighboring polygons
            if (epsilon <= 0) epsilon = Element.Length * 1e-6;
            var division = Subdivision.FromPolygons(polygons, epsilon);

            var faceToGrid = new PointI[width * height];
            var gridToFace = new SubdivisionFace[width, height];

            // determine equivalence of faces and grid elements
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++) {
                    PointD[] polygon = polygons[x * height + y];
                    SubdivisionFace face = division.FindFace(polygon);

                    // bounded faces start at creation index one
                    faceToGrid[face._key - 1] = new PointI(x, y);
                    gridToFace[x, y] = face;
                }

            return new SubdivisionMap(division, this, faceToGrid, gridToFace);
        }

        #endregion
        #endregion
        #region ICloneable Members

        /// <summary>
        /// Creates a shallow copy of the <see cref="PolygonGrid"/>.</summary>
        /// <returns>
        /// A shallow copy of the <see cref="PolygonGrid"/>.</returns>
        /// <remarks><para>
        /// <b>Clone</b> invokes the "copy constructor", <see cref="PolygonGrid(PolygonGrid)"/>, to
        /// create a shallow copy of the current instance.
        /// </para><para>
        /// <b>Clone</b> does not preserve the value of the <see cref="IsReadOnly"/> property. The
        /// new <see cref="PolygonGrid"/> will be mutable even if the current instance is read-only.
        /// </para></remarks>

        public object Clone() {
            return new PolygonGrid(this);
        }

        #endregion
        #region IGraph2D<T> Members
        #region Connectivity

        /// <summary>
        /// Gets the maximum number of direct neighbors for any <see cref="IGraph2D{T}"/> node.
        /// </summary>
        /// <value>
        /// The <see cref="RegularPolygon.Connectivity"/> of the current <see cref="Element"/>.
        /// </value>
        /// <remarks>
        /// <b>Connectivity</b> also equals the size of an inner array within <see
        /// cref="NeighborOffsets"/>.</remarks>

        public int Connectivity {
            get { return Element.Connectivity; }
        }

        #endregion
        #region NodeCount

        /// <summary>
        /// Gets the total number of <see cref="Nodes"/> in the <see cref="IGraph2D{T}"/>.</summary>
        /// <value>
        /// The total number of <see cref="Nodes"/> in the <see cref="IGraph2D{T}"/>.</value>
        /// <remarks>
        /// <b>NodeCount</b> returns the product of the <see cref="SizeI.Width"/> and <see
        /// cref="SizeI.Height"/> of the current <see cref="Size"/>.</remarks>

        public int NodeCount {
            get { return Size.Width * Size.Height; }
        }

        #endregion
        #region Nodes

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> collection that contains all nodes in the <see
        /// cref="IGraph2D{T}"/>.</summary>
        /// <value>
        /// An <see cref="IEnumerable{T}"/> collection that contains all nodes in the <see
        /// cref="IGraph2D{T}"/>.</value>
        /// <remarks>
        /// <b>Nodes</b> returns all <see cref="PointI"/> locations in the <see
        /// cref="PolygonGrid"/>, starting at (0,0) and incrementing x-coordinates before
        /// y-coordinates.</remarks>

        public IEnumerable<PointI> Nodes {
            get {
                for (int x = 0; x < Size.Width; x++)
                    for (int y = 0; y < Size.Height; y++)
                        yield return new PointI(x, y);
            }
        }

        #endregion
        #region Contains(Int32, Int32)

        /// <overloads>
        /// Determines whether the <see cref="PolygonGrid"/> contains the specified locations.
        /// </overloads>
        /// <summary>
        /// Determines whether the <see cref="PolygonGrid"/> contains the specified coordinate pair.
        /// </summary>
        /// <param name="x">
        /// The x-coordinate to examine.</param>
        /// <param name="y">
        /// The y-coordinate to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="PolygonGrid"/> contains the specified <paramref name="x"/>
        /// and <paramref name="y"/> coordinates; otherwise, <c>false</c>.</returns>

        public bool Contains(int x, int y) {
            return (x >= 0 && x < Size.Width && y >= 0 && y < Size.Height);
        }

        #endregion
        #region Contains(PointI)

        /// <summary>
        /// Determines whether the <see cref="IGraph2D{T}"/> contains the specified node.</summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="IGraph2D{T}"/> contains the specified <paramref
        /// name="node"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> returns the result of <see cref="Contains(Int32, Int32)"/> for the
        /// specified <paramref name="node"/>.</remarks>

        public bool Contains(PointI node) {
            return Contains(node.X, node.Y);
        }

        #endregion
        #region Contains(RectI)

        /// <summary>
        /// Determines whether the <see cref="PolygonGrid"/> entirely contains the specified <see
        /// cref="RectI"/>.</summary>
        /// <param name="rect">
        /// The <see cref="RectI"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="PolygonGrid"/> entirely contains the specified <paramref
        /// name="rect"/>; otherwise, <c>false</c>.</returns>

        public bool Contains(RectI rect) {
            return Contains(rect.Left, rect.Top) && Contains(rect.Right - 1, rect.Bottom - 1);
        }

        #endregion
        #region GetDistance

        /// <summary>
        /// Returns the distance between the two specified <see cref="IGraph2D{T}"/> nodes.
        /// </summary>
        /// <param name="source">
        /// The source node in the <see cref="IGraph2D{T}"/>.</param>
        /// <param name="target">
        /// The target node in the <see cref="IGraph2D{T}"/>.</param>
        /// <returns>
        /// The non-negative distance between <paramref name="source"/> and <paramref
        /// name="target"/>, measured in movement steps.</returns>
        /// <remarks>
        /// <b>GetDistance</b> returns the result of <see cref="GetStepDistance"/> for the specified
        /// <paramref name="source"/> and <paramref name="target"/>, which is always an <see
        /// cref="Int32"/> value converted to <see cref="Double"/>.</remarks>

        public double GetDistance(PointI source, PointI target) {
            return GetStepDistance(source, target);
        }

        #endregion
        #region GetNearestNode

        /// <summary>
        /// Gets the <see cref="IGraph2D{T}"/> node that is nearest to the specified location, in
        /// world coordinates.</summary>
        /// <param name="location">
        /// The <see cref="PointD"/> location, in world coordinates, whose nearest <see
        /// cref="IGraph2D{T}"/> node to find.</param>
        /// <returns>
        /// The <see cref="IGraph2D{T}"/> node whose <see cref="GetWorldLocation"/> result is
        /// nearest to the specified <paramref name="location"/>.</returns>
        /// <remarks>
        /// <b>GetNearestNode</b> returns the result of <see cref="DisplayToGridClipped"/> for the
        /// specified <paramref name="location"/>.</remarks>

        public PointI GetNearestNode(PointD location) {
            return DisplayToGridClipped(location.X, location.Y);
        }

        #endregion
        #region GetNeighbors(PointI)

        /// <overloads>
        /// Returns all neighbors of the specified <see cref="IGraph2D{T}"/> node.</overloads>
        /// <summary>
        /// Returns all direct neighbors of the specified <see cref="IGraph2D{T}"/> node.</summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node whose neighbors to return.</param>
        /// <returns>
        /// An <see cref="IList{T}"/> containing all valid <see cref="IGraph2D{T}"/> nodes that are
        /// directly connected with the specified <paramref name="node"/>. The number of elements is
        /// at most <see cref="Connectivity"/>.</returns>
        /// <remarks><para>
        /// <b>GetNeighbors</b> never returns a null reference, but it returns an empty <see
        /// cref="IList{PointI}"/> if the specified <paramref name="node"/> or all its neighbors are
        /// outside the <see cref="PolygonGrid"/>.
        /// </para><para>
        /// Otherwise, <b>GetNeighbors</b> calculates all neighboring locations by adding each
        /// element in the appropriate inner array within <see cref="NeighborOffsets"/> to the
        /// specified <paramref name="node"/>, omitting any neighbors outside the <see
        /// cref="PolygonGrid"/>.</para></remarks>

        public IList<PointI> GetNeighbors(PointI node) {

            // do nothing if specified location invalid
            if (!Contains(node)) return new List<PointI>(0);

            // determine offset group for given location
            PointI[] offsets = GetNeighborOffsets(node);
            List<PointI> neighbors = new List<PointI>(offsets.Length);

            // build list containing all valid neighbors
            for (int index = 0; index < offsets.Length; index++) {
                PointI neighbor = node + offsets[index];
                if (Contains(neighbor)) neighbors.Add(neighbor);
            }

            return neighbors;
        }

        #endregion
        #region GetNeighbors(PointI, Int32)

        /// <summary>
        /// Returns all <see cref="IGraph2D{T}"/> nodes within the specified step distance of the
        /// specified node.</summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node whose neighbors to return.</param>
        /// <param name="distance">
        /// The distance around the specified <paramref name="node"/>, in movement steps, in which
        /// another node is considered a neighbor.</param>
        /// <returns>
        /// An <see cref="IList{Int32}"/> containing all valid <see cref="IGraph2D{T}"/> nodes whose
        /// step distance from the specified <paramref name="node"/> is greater than zero, and equal
        /// to or less than <paramref name="distance"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="distance"/> is zero or negative.</exception>
        /// <remarks><para>
        /// <b>GetNeighbors</b> never returns a null reference, but it returns an empty <see
        /// cref="IList{PointI}"/> if the specified <paramref name="node"/> or all its neighbors are
        /// outside the <see cref="PolygonGrid"/>. Otherwise, the elements of the returned <see
        /// cref="IList{PointI}"/> are ordered by increasing x- and y-coordinates. 
        /// </para><para>
        /// Note that the specified <paramref name="distance"/> refers to the result of <see
        /// cref="GetStepDistance"/>, not <see cref="GetDistance"/>. This overload is a specialized
        /// variant of the <see cref="IGraph2D{T}.GetNeighbors"/> method defined by the <see
        /// cref="IGraph2D{T}"/> interface.</para></remarks>

        public IList<PointI> GetNeighbors(PointI node, int distance) {
            if (distance <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "distance", distance, Strings.ArgumentNotPositive);

            // use fast overload for immediate neighbors
            if (distance == 1) return GetNeighbors(node);

            // do nothing if specified location invalid
            if (!Contains(node)) return new List<PointI>(0);

            List<PointI> neighbors = new List<PointI>();
            int distanceX = distance, distanceY = distance;

            // standing squares: double or halve coordinate distances
            if (Element.Sides == 4 && Element.Orientation == PolygonOrientation.OnVertex) {
                if (AreColumnsShifted) {
                    if (Element.VertexNeighbors)
                        distanceX *= 2;
                    else
                        distanceY = (distanceY + 1) / 2;
                } else {
                    if (Element.VertexNeighbors)
                        distanceY *= 2;
                    else
                        distanceX = (distanceX + 1) / 2;
                }
            }

            // compute rectangle covering potential neighbors
            int minX = node.X - distanceX;
            int maxX = node.X + distanceX;
            int minY = node.Y - distanceY;
            int maxY = node.Y + distanceY;

            // restrict rectangle to current grid size
            if (minX < 0) minX = 0;
            if (maxX >= Size.Width) maxX = Size.Width - 1;
            if (minY < 0) minY = 0;
            if (maxY >= Size.Height) maxY = Size.Height - 1;

            // add all rectangle locations within step distance
            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++) {
                    PointI neighbor = new PointI(x, y);
                    int delta = GetStepDistance(node, neighbor);
                    if (delta > 0 && delta <= distance)
                        neighbors.Add(neighbor);
                }

            return neighbors;
        }

        #endregion
        #region GetWorldLocation

        /// <summary>
        /// Gets the location of the specified <see cref="IGraph2D{T}"/> node, in world coordinates.
        /// </summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node whose location to return.</param>
        /// <returns>
        /// The <see cref="PointD"/> location of the specified <paramref name="node"/>, in world
        /// coordinates.</returns>
        /// <remarks>
        /// <b>GetWorldLocation</b> returns the result of <see cref="GridToDisplay"/> for the
        /// specified <paramref name="node"/>. That is, world coordinates are equivalent to display
        /// coordinates.</remarks>

        public PointD GetWorldLocation(PointI node) {
            return GridToDisplay(node.X, node.Y);
        }

        #endregion
        #region GetWorldRegion

        /// <summary>
        /// Gets the region covered by the specified <see cref="IGraph2D{T}"/> node, in world
        /// coordinates.</summary>
        /// <param name="node">
        /// The <see cref="IGraph2D{T}"/> node whose region to return.</param>
        /// <returns>
        /// An <see cref="Array"/> containing the <see cref="PointD"/> vertices of the polygonal
        /// region covered by the specified <paramref name="node"/>, in world coordinates.</returns>
        /// <remarks>
        /// <b>GetWorldRegion</b> returns the result of <see cref="GetElementVertices"/> for the
        /// specified <paramref name="node"/>. That is, node regions in world coordinates are
        /// equivalent to <see cref="Element"/> bounds in display coordinates.</remarks>

        public PointD[] GetWorldRegion(PointI node) {
            return GetElementVertices(node.X, node.Y);
        }

        #endregion
        #endregion
        #region Class InstanceData

        /// <summary>
        /// Contains the values of most instance properties of a <see cref="PolygonGrid"/>.
        /// </summary>
        /// <remarks>
        /// <b>InstanceData</b> is a simple data container whose fields back most of the instance
        /// properties of an associated <see cref="PolygonGrid"/> object. When a read-only view is
        /// created, it shares the <b>InstanceData</b> of the original <see cref="PolygonGrid"/>.
        /// This allows the read-only view to reflect all changes to the original instance.
        /// </remarks>

        [Serializable]
        private class InstanceData {
            #region InstanceData()

            /// <overloads>
            /// Initializes a new instance of the <see cref="InstanceData"/> class.</overloads>
            /// <summary>
            /// Initializes a new instance of the <see cref="InstanceData"/> class with default
            /// properties.</summary>

            internal InstanceData() { }

            #endregion
            #region InstanceData(InstanceData)

            /// <summary>
            /// Initializes a new instance of the <see cref="InstanceData"/> class that is a shallow
            /// copy of the specified instance.</summary>
            /// <param name="data">
            /// The <see cref="InstanceData"/> object whose field values should be copied to the new
            /// instance.</param>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="data"/> is a null reference.</exception>
            /// <remarks>
            /// Please refer to <see cref="PolygonGrid(PolygonGrid)"/> for details.</remarks>

            internal InstanceData(InstanceData data) {
                if (data == null)
                    ThrowHelper.ThrowArgumentNullException("data");

                // settable PolygonGrid properties
                Element = data.Element;
                GridShift = data.GridShift;
                Size = data.Size;

                // dependent PolygonGrid properties
                CenterDistance = data.CenterDistance;
                DisplayBounds = data.DisplayBounds;
                EdgeNeighborOffsets = data.EdgeNeighborOffsets;
                NeighborOffsets = data.NeighborOffsets;
            }

            #endregion
            #region Internal Fields

            // settable PolygonGrid properties
            internal RegularPolygon Element;
            internal PolygonGridShift GridShift;
            internal SizeI Size;

            // dependent PolygonGrid properties
            internal SizeD CenterDistance;
            internal RectD DisplayBounds;
            internal PointI[][] EdgeNeighborOffsets, NeighborOffsets;

            #endregion
        }

        #endregion
        #region Class SubdivisionMap

        /// <summary>
        /// Maps the faces of a planar <see cref="Subdivision"/> to <see cref="PolygonGrid"/>
        /// locations.</summary>
        /// <remarks><para>
        /// <b>SubdivisionMap</b> provides a mapping between all faces of a planar <see
        /// cref="Subdivision"/> and the <see cref="PointI"/> locations of the <see
        /// cref="PolygonGrid"/> from which the <see cref="Subdivision"/> was created.
        /// </para><para>
        /// The mapping is realized by a pair of arrays for optimal runtime efficiency. However,
        /// <b>SubdivisionMap</b> will not reflect changes to the underlying <see
        /// cref="Subdivision"/> or <see cref="PolygonGrid"/>.</para></remarks>

        [Serializable]
        public class SubdivisionMap: ISubdivisionMap<PointI> {
            #region SubdivisionMap(...)

            /// <summary>
            /// Initializes a new instance of the <see cref="SubdivisionMap"/> class.</summary>
            /// <param name="source">
            /// The <see cref="Subdivision"/> that contains all mapped faces.</param>
            /// <param name="target">
            /// The <see cref="PolygonGrid"/> that defines all mapped locations.</param>
            /// <param name="faceToGrid">
            /// A one-dimensional <see cref="Array"/> that maps <see cref="SubdivisionFace"/> keys
            /// to <see cref="PolygonGrid"/> locations.</param>
            /// <param name="gridToFace">
            /// A two-dimensional <see cref="Array"/> that maps <see cref="PolygonGrid"/> locations
            /// to <see cref="SubdivisionFace"/> objects.</param>

            internal SubdivisionMap(Subdivision source, PolygonGrid target,
                PointI[] faceToGrid, SubdivisionFace[,] gridToFace) {

                Debug.Assert(source != null);
                Debug.Assert(target != null);

                Debug.Assert(faceToGrid != null);
                Debug.Assert(gridToFace != null);
                Debug.Assert(faceToGrid.Length == gridToFace.GetLength(0) * gridToFace.GetLength(1));

                _source = source;
                _target = target;
                _faceToGrid = faceToGrid;
                _gridToFace = gridToFace;
            }

            #endregion
            #region Private Fields

            // property backers
            private readonly Subdivision _source;
            private readonly PolygonGrid _target;

            // mapping arrays
            private readonly PointI[] _faceToGrid;
            private readonly SubdivisionFace[,] _gridToFace;

            #endregion
            #region Source

            /// <summary>
            /// Gets the <see cref="Subdivision"/> that contains all mapped faces.</summary>
            /// <value>
            /// The <see cref="Subdivision"/> that contains all faces accepted and returned by the
            /// <see cref="FromFace"/> and <see cref="ToFace"/> methods, respectively.</value>

            public Subdivision Source {
                get { return _source; }
            }

            #endregion
            #region Target

            /// <summary>
            /// Gets the <see cref="PolygonGrid"/> that defines all mapped locations.</summary>
            /// <value>
            /// The <see cref="PolygonGrid"/> that defines all locations returned and accepted by
            /// the <see cref="FromFace"/> and <see cref="ToFace"/> methods, respectively.</value>

            public PolygonGrid Target {
                get { return _target; }
            }

            /// <summary>
            /// Gets the <see cref="PolygonGrid"/> that defines all mapped locations.</summary>
            /// <value>
            /// The <see cref="PolygonGrid"/> that defines all locations returned and accepted by
            /// the <see cref="FromFace"/> and <see cref="ToFace"/> methods, respectively.</value>

            object ISubdivisionMap<PointI>.Target {
                get { return _target; }
            }

            #endregion
            #region FromFace

            /// <summary>
            /// Converts the specified <see cref="SubdivisionFace"/> into the associated <see
            /// cref="PolygonGrid"/> location.</summary>
            /// <param name="face">
            /// The <see cref="SubdivisionFace"/> to convert.</param>
            /// <returns>
            /// The <see cref="PolygonGrid"/> location associated with <paramref name="face"/>.
            /// </returns>
            /// <exception cref="IndexOutOfRangeException">
            /// <paramref name="face"/> contains a <see cref="SubdivisionFace.Key"/> that is less
            /// than one or greater than the number of <see cref="PolygonGrid"/> locations.
            /// </exception>
            /// <exception cref="NullReferenceException">
            /// <paramref name="face"/> is a null reference.</exception>

            public PointI FromFace(SubdivisionFace face) {
                return _faceToGrid[face._key - 1];
            }

            #endregion
            #region ToFace

            /// <summary>
            /// Converts the specified <see cref="PolygonGrid"/> location into the associated <see
            /// cref="SubdivisionFace"/>.</summary>
            /// <param name="value">
            /// The <see cref="PolygonGrid"/> location to convert.</param>
            /// <returns>
            /// The <see cref="SubdivisionFace"/> associated with <paramref name="value"/>.
            /// </returns>
            /// <exception cref="IndexOutOfRangeException">
            /// <paramref name="value"/> is not a valid <see cref="PolygonGrid"/> location.
            /// </exception>

            public SubdivisionFace ToFace(PointI value) {
                return _gridToFace[value.X, value.Y];
            }

            #endregion
        }

        #endregion
    }
}
