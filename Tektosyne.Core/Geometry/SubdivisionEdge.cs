using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

using Tektosyne.Collections;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents a half-edge in a planar <see cref="Subdivision"/>.</summary>
    /// <remarks><para>
    /// <b>SubdivisionEdge</b> does not represent a complete edge in a <see cref="Subdivision"/> but
    /// rather a half-edge. Any given <b>SubdivisionEdge</b> holds one end point of an edge, and a
    /// pointer to its twin half-edge which holds the other end point.
    /// </para><para>
    /// Every <b>SubdivisionEdge</b> is part of a cycle of half-edges that is connected by the <see
    /// cref="SubdivisionEdge.Next"/> and <see cref="SubdivisionEdge.Previous"/> pointers. Assuming
    /// y-coordinates increase upward, a clockwise cycle forms the inner boundary of the incident
    /// <see cref="SubdivisionFace"/>, and a counter-clockwise cycle forms its outer boundary. A
    /// <b>SubdivisionEdge</b> may form a cycle with its own twin half-edge; such a zero-area cycle
    /// always forms an inner boundary.</para></remarks>

    [Serializable]
    public sealed class SubdivisionEdge: IEquatable<SubdivisionEdge>, IKeyedValue<Int32> {
        #region SubdivisionEdge(Int32)

        /// <overloads>
        /// Initializes a new instance of the <see cref="SubdivisionEdge"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="SubdivisionEdge"/> class with the specified
        /// unique key.</summary>
        /// <param name="key">
        /// The unique key of the <see cref="SubdivisionEdge"/> within its containing <see
        /// cref="Subdivision"/>.</param>

        internal SubdivisionEdge(int key) {
            _key = key;
        }

        #endregion
        #region SubdivisionEdge(Int32, PointD, ...)

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdivisionEdge"/> class with the specified
        /// unique key, origin, incident face, twin, next and previous half-edge.</summary>
        /// <param name="key">
        /// The unique key of the <see cref="SubdivisionEdge"/> within its containing <see
        /// cref="Subdivision"/>.</param>
        /// <param name="origin">
        /// The coordinates where the <see cref="SubdivisionEdge"/> begins.</param>
        /// <param name="twin">
        /// The <see cref="SubdivisionEdge"/> that is the twin of the current instance.</param>
        /// <param name="face">
        /// The <see cref="SubdivisionFace"/> that is bounded by the <see cref="SubdivisionEdge"/>.
        /// </param>
        /// <param name="next">
        /// The next <see cref="SubdivisionEdge"/> that bounds the same <paramref name="face"/>.
        /// </param>
        /// <param name="previous">
        /// The previous <see cref="SubdivisionEdge"/> that bounds the same <paramref name="face"/>.
        /// </param>
        /// <remarks>
        /// This constructor is intended for unit testing.</remarks>

        internal SubdivisionEdge(int key, PointD origin, SubdivisionEdge twin,
            SubdivisionFace face, SubdivisionEdge next, SubdivisionEdge previous) {

            _key = key;
            _origin = origin;
            _twin = twin;
            _face = face;
            _next = next;
            _previous = previous;
        }

        #endregion
        #region Internal Fields

        /// <summary>
        /// The unique key of the <see cref="SubdivisionEdge"/>.</summary>

        internal int _key;

        /// <summary>
        /// The coordinates where the <see cref="SubdivisionEdge"/> begins.</summary>

        internal PointD _origin;

        /// <summary>
        /// The <see cref="SubdivisionFace"/> that is bounded by the <see cref="SubdivisionEdge"/>.
        /// </summary>

        internal SubdivisionFace _face;

        /// <summary>
        /// The next <see cref="SubdivisionEdge"/> that bounds the same <see cref="Face"/>.
        /// </summary>

        internal SubdivisionEdge _next;

        /// <summary>
        /// The previous <see cref="SubdivisionEdge"/> that bounds the same <see cref="Face"/>.
        /// </summary>

        internal SubdivisionEdge _previous;

        /// <summary>
        /// The <see cref="SubdivisionEdge"/> that is the twin of the current instance.</summary>

        internal SubdivisionEdge _twin;

        #endregion
        #region Public Properties
        #region Key

        /// <summary>
        /// Gets the unique key of the <see cref="SubdivisionEdge"/>.</summary>
        /// <value>
        /// The unique key of the <see cref="SubdivisionEdge"/> within its containing <see
        /// cref="Subdivision"/>.</value>
        /// <remarks><para>
        /// <b>Key</b> begins at zero for the first <see cref="SubdivisionEdge"/> instance in a <see
        /// cref="Subdivision"/>, and is incremented by one whenever an additional instance is
        /// created. The <b>Key</b> values of all <see cref="SubdivisionEdge"/> instances thus
        /// reflect the order in which they were created.
        /// </para><para>
        /// <b>Key</b> is usually immutable, unless <see cref="Subdivision.RenumberEdges"/> is
        /// called on the containing <see cref="Subdivision"/>.</para></remarks>

        public int Key {
            [DebuggerStepThrough]
            get { return _key; }
        }

        #endregion
        #region Origin

        /// <summary>
        /// Gets the coordinates where the <see cref="SubdivisionEdge"/> begins.</summary>
        /// <value>
        /// The <see cref="PointD"/> coordinates of the origin of the half-edge represented by the
        /// <see cref="SubdivisionEdge"/>.</value>

        public PointD Origin {
            [DebuggerStepThrough]
            get { return _origin; }
        }

        #endregion
        #region Destination

        /// <summary>
        /// Gets the coordinates where the <see cref="SubdivisionEdge"/> ends.</summary>
        /// <value>
        /// The <see cref="Origin"/> of the <see cref="Twin"/> of the <see cref="SubdivisionEdge"/>.
        /// </value>

        public PointD Destination {
            get { return _twin._origin; }
        }

        #endregion
        #region Face

        /// <summary>
        /// Gets the <see cref="SubdivisionFace"/> that is bounded by the <see
        /// cref="SubdivisionEdge"/>.</summary>
        /// <value>
        /// The <see cref="SubdivisionFace"/> that lies to the left of the half-edge represented by
        /// the <see cref="SubdivisionEdge"/>, viewed from its <see cref="Origin"/> and assuming
        /// that y-coordinates increase upward.</value>
        /// <remarks>
        /// <b>Face</b> defaults to a null reference while a <see cref="Subdivision"/> is being
        /// created, but never returns a null reference after construction is finished.</remarks>

        public SubdivisionFace Face {
            [DebuggerStepThrough]
            get { return _face; }
        }

        #endregion
        #region Next

        /// <summary>
        /// Gets the next <see cref="SubdivisionEdge"/> that bounds the same <see cref="Face"/>.
        /// </summary>
        /// <value>
        /// The <see cref="SubdivisionEdge"/> that begins at the <see cref="Destination"/> of the
        /// current instance and bounds the same <see cref="Face"/>.</value>
        /// <remarks><para>
        /// <b>Next</b> returns the <see cref="Twin"/> of the current instance if no other <see
        /// cref="SubdivisionEdge"/> begins at its <see cref="Destination"/>.
        /// </para><para>
        /// If there are multiple eligible <see cref="SubdivisionEdge"/> instances, <b>Next</b>
        /// returns the nearest in clockwise direction, assuming that y-coordinates increase upward.
        /// </para><para>
        /// <b>Next</b> defaults to a null reference while a <see cref="Subdivision"/> is being
        /// created, but never returns a null reference after construction is finished.
        /// </para></remarks>

        public SubdivisionEdge Next {
            [DebuggerStepThrough]
            get { return _next; }
        }

        #endregion
        #region Previous

        /// <summary>
        /// Gets the previous <see cref="SubdivisionEdge"/> that bounds the same <see
        /// cref="Face"/>.</summary>
        /// <value>
        /// The <see cref="SubdivisionEdge"/> that ends at the <see cref="Origin"/> of the current
        /// instance and bounds the same <see cref="Face"/>.</value>
        /// <remarks><para>
        /// <b>Previous</b> returns the <see cref="Twin"/> of the current instance if no other <see
        /// cref="SubdivisionEdge"/> ends at its <see cref="Origin"/>.
        /// </para><para>
        /// If there are multiple eligible <see cref="SubdivisionEdge"/> instances, <b>Previous</b>
        /// returns the nearest in counter-clockwise direction, assuming that y-coordinates increase
        /// upward.
        /// </para><para>
        /// <b>Previous</b> defaults to a null reference while a <see cref="Subdivision"/> is being
        /// created, but never returns a null reference after construction is finished.
        /// </para></remarks>

        public SubdivisionEdge Previous {
            [DebuggerStepThrough]
            get { return _previous; }
        }

        #endregion
        #region Twin

        /// <summary>
        /// Gets the <see cref="SubdivisionEdge"/> that is the twin of the current instance.
        /// </summary>
        /// <value>
        /// The <see cref="SubdivisionEdge"/> that begins at the <see cref="Destination"/> and ends
        /// at the <see cref="Origin"/> of the current instance.</value>
        /// <remarks><para>
        /// A <see cref="SubdivisionEdge"/> and its <b>Twin</b> combine to form one edge in a <see
        /// cref="Subdivision"/>, corresponding to a single <see cref="LineD"/> instance.
        /// </para><para>
        /// <b>Twin</b> defaults to a null reference while a <see cref="Subdivision"/> is being
        /// created, but never returns a null reference after construction is finished.
        /// </para></remarks>

        public SubdivisionEdge Twin {
            [DebuggerStepThrough]
            get { return _twin; }
        }

        #endregion
        #region CycleArea

        /// <summary>
        /// Gets the area within the boundary of the incident <see cref="Face"/> which contains the
        /// <see cref="SubdivisionEdge"/>.</summary>
        /// <value>
        /// The area of the <see cref="CyclePolygon"/>, with a sign that indicates the orientation
        /// of its vertices.</value>
        /// <remarks><para>
        /// The absolute value of <b>CycleArea</b> equals the area of the <see
        /// cref="CyclePolygon"/>. The sign indicates the orientation of its vertices, as follows:
        /// </para><list type="table"><listheader>
        /// <term>Value</term><description>Relationship</description>
        /// </listheader><item>
        /// <term>Less than zero</term><description>
        /// The vertices are ordered clockwise, assuming y-coordinates increase upward. The <see
        /// cref="CyclePolygon"/> forms an inner boundary of the incident <see cref="Face"/>.
        /// </description></item><item>
        /// <term>Zero</term><description>
        /// All vertices are collinear, or otherwise enclose no area. The <see cref="CyclePolygon"/>
        /// forms an inner boundary of the incident <see cref="Face"/>.
        /// </description></item><item>
        /// <term>Greater than zero</term><description>
        /// The vertices are ordered counter-clockwise, assuming y-coordinates increase upward. The
        /// <see cref="CyclePolygon"/> forms the outer boundary of the incident <see cref="Face"/>.
        /// </description></item></list></remarks>

        public double CycleArea {
            get {
                double area = 0;

                var edge = this;
                do {
                    var next = edge._next;
                    area += (edge._origin.X * next._origin.Y - next._origin.X * edge._origin.Y);
                    edge = next;
                } while (edge != this);

                return area / 2.0;
            }
        }

        #endregion
        #region CycleCentroid

        /// <summary>
        /// Gets the centroid of the boundary of the incident <see cref="Face"/> which contains the
        /// <see cref="SubdivisionEdge"/>.</summary>
        /// <value>
        /// The centroid (center of gravity) of the <see cref="CyclePolygon"/>.</value>
        /// <remarks>
        /// <b>CycleCentroid</b> is undefined if the <see cref="CycleArea"/> is zero.</remarks>

        public PointD CycleCentroid {
            get {
                double area = 0, x = 0, y = 0;

                var edge = this;
                do {
                    var next = edge._next;
                    double factor = edge._origin.X * next._origin.Y - next._origin.X * edge._origin.Y;

                    area += factor;
                    x += (edge._origin.X + next._origin.X) * factor;
                    y += (edge._origin.Y + next._origin.Y) * factor;

                    edge = next;
                } while (edge != this);

                area *= 3.0;
                return new PointD(x / area, y / area);
            }
        }

        #endregion
        #region CycleEdges

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> collection that contains all half-edges on the
        /// boundary of the incident <see cref="Face"/>.</summary>
        /// <value>
        /// An <see cref="IEnumerable{T}"/> collection that contains each <see
        /// cref="SubdivisionEdge"/> on the boundary of the incident <see cref="Face"/>.</value>
        /// <remarks>
        /// <b>CycleEdges</b> begins with the current <see cref="SubdivisionEdge"/> and follows the
        /// chain of <see cref="Next"/> pointers until the cycle is complete, yielding each
        /// encountered <see cref="SubdivisionEdge"/> in turn.</remarks>

        public IEnumerable<SubdivisionEdge> CycleEdges {
            get {
                SubdivisionEdge cursor = this;
                do {
                    yield return cursor;
                    cursor = cursor._next;
                } while (cursor != this);
            }
        }

        #endregion
        #region CyclePolygon

        /// <summary>
        /// Gets a polygon that represents the boundary of the incident <see cref="Face"/> which
        /// contains the <see cref="SubdivisionEdge"/>.</summary>
        /// <value>
        /// An <see cref="Array"/> containing the <see cref="Origin"/> coordinates of all half-edges
        /// in the cycle that begins with this <see cref="SubdivisionEdge"/> and continues along the
        /// chain of <see cref="Next"/> pointers.</value>
        /// <remarks>
        /// <b>CyclePolygon</b> represents the outer boundary of the incident <see cref="Face"/> if
        /// its vertices contain a positive area and are ordered counter-clockwise, assuming
        /// y-coordinates increase upward. Otherwise, <b>CyclePolygon</b> represents one of the
        /// inner boundaries of the incident <see cref="Face"/>.</remarks>

        public PointD[] CyclePolygon {
            get {
                // count half-edges in cycle
                int index = 0;
                var edge = this;
                do {
                    ++index;
                    edge = edge._next;
                } while (edge != this);

                // copy cycle vertices to array
                var points = new PointD[index];
                for (index = 0; index < points.Length; index++) {
                    points[index] = edge._origin;
                    edge = edge._next;
                }

                return points;
            }
        }

        #endregion
        #region IsCycleAreaZero

        /// <summary>
        /// Gets a value indicating whether the boundary of the incident <see cref="Face"/> which
        /// contains the <see cref="SubdivisionEdge"/> encloses no area.</summary>
        /// <value>
        /// <c>true</c> if the vertices of the <see cref="CyclePolygon"/> enclose no area;
        /// otherwise, <c>false</c>.</value>
        /// <remarks><para>
        /// <b>IsCycleAreaZero</b> returns <c>true</c> exactly if the twins of all half-edges in the
        /// current cycle bound the same <see cref="Face"/> as the current instance. This implies a
        /// half-edge cycle that comprises only complete <see cref="Twin"/> pairs. Such a cycle
        /// cannot enclose any area, as that would require some twins bounding a different <see
        /// cref="Face"/>.
        /// </para><para>
        /// <see cref="CycleArea"/> should equal zero if <b>IsCycleAreaZero</b> returns <c>true</c>,
        /// but this may not be the case due to floating-point inaccuracies. <b>IsCycleAreaZero</b>
        /// is both faster and more precise than <see cref="CycleArea"/> if the actual area is not
        /// required.</para></remarks>

        public bool IsCycleAreaZero {
            get {
                var edge = this;
                do {
                    if (edge._twin._face != _face)
                        return false;

                    edge = edge._next;
                } while (edge != this);

                return true;
            }
        }

        #endregion
        #region OriginEdges

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> collection that contains all half-edges with the
        /// same <see cref="Origin"/>.</summary>
        /// <value>
        /// An <see cref="IEnumerable{T}"/> collection that contains each <see
        /// cref="SubdivisionEdge"/> with the same <see cref="Origin"/>.</value>
        /// <remarks>
        /// <b>OriginEdges</b> begins with the current <see cref="SubdivisionEdge"/> and follows the
        /// chain of <see cref="Twin"/> and <see cref="Next"/> pointers until the sequence is
        /// complete, yielding each encountered <see cref="SubdivisionEdge"/> in turn.</remarks>

        public IEnumerable<SubdivisionEdge> OriginEdges {
            get {
                SubdivisionEdge cursor = this;
                do {
                    yield return cursor;
                    cursor = cursor._twin._next;
                } while (cursor != this);
            }
        }

        #endregion
        #endregion
        #region Public Methods
        #region GetEdgeTo(PointD)

        /// <summary>
        /// Returns the half-edge with the same origin and the specified destination.</summary>
        /// <summary>
        /// Returns the half-edge with the same origin and the specified destination, using exact
        /// coordinate comparisons.</summary>
        /// <param name="destination">
        /// The <see cref="Destination"/> of the half-edge.</param>
        /// <returns><para>
        /// The <see cref="SubdivisionEdge"/> with the same <see cref="Origin"/> as the current
        /// instance, and with the specified <paramref name="destination"/>.
        /// </para><para>-or-</para><para>
        /// A null reference if no matching <see cref="SubdivisionEdge"/> was found.
        /// </para></returns>
        /// <remarks>
        /// <b>GetEdgeTo</b> is an O(m) operation, where m is the number of half-edges originating
        /// from the current <see cref="Origin"/>.</remarks>

        public SubdivisionEdge GetEdgeTo(PointD destination) {
            var edge = this;

            do {
                var twin = edge._twin;
                if (twin._origin == destination)
                    return edge;

                edge = twin._next;
            } while (edge != this);

            return null;
        }

        #endregion
        #region GetEdgeTo(PointD, Double)

        /// <summary>
        /// Returns the half-edge with the same origin and the specified destination, given the
        /// specified epsilon for coordinate comparisons.</summary>
        /// <param name="destination">
        /// The <see cref="Destination"/> of the half-edge.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which coordinates should be considered equal.</param>
        /// <returns><para>
        /// The <see cref="SubdivisionEdge"/> with the same <see cref="Origin"/> as the current
        /// instance, and with the specified <paramref name="destination"/>.
        /// </para><para>-or-</para><para>
        /// A null reference if no matching <see cref="SubdivisionEdge"/> was found.
        /// </para></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="epsilon"/> is less than zero.</exception>
        /// <remarks>
        /// <b>GetEdgeTo</b> is identical with the basic <see cref="GetEdgeTo(PointD)"/> overload
        /// but uses the specified <paramref name="epsilon"/> to compare the specified <paramref
        /// name="destination"/> against existing <see cref="Subdivision.Vertices"/>.</remarks>

        public SubdivisionEdge GetEdgeTo(PointD destination, double epsilon) {
            var edge = this;

            do {
                var twin = edge._twin;
                if (PointD.Equals(twin._origin, destination, epsilon))
                    return edge;

                edge = twin._next;
            } while (edge != this);

            return null;
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="SubdivisionEdge"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> returns the value of the <see cref="Key"/> property, which is
        /// guaranteed to be unique within the containing <see cref="Subdivision"/>.</remarks>

        public override int GetHashCode() {
            return _key;
        }

        #endregion
        #region Locate(PointD)

        /// <overloads>
        /// Finds the location of the specified <see cref="PointD"/> relative to the boundary of the
        /// incident <see cref="Face"/> which contains the <see cref="SubdivisionEdge"/>.
        /// </overloads>
        /// <summary>
        /// Finds the location of the specified <see cref="PointD"/> relative to the boundary of the
        /// incident <see cref="Face"/> that contains the <see cref="SubdivisionEdge"/>, using exact
        /// coordinate comparisons.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to locate.</param>
        /// <returns>
        /// A <see cref="PolygonLocation"/> value that indicates the location of <paramref
        /// name="q"/> relative to the <see cref="CyclePolygon"/>.</returns>
        /// <remarks>
        /// <b>Locate</b> performs a ray crossings algorithm with an asymptotic runtime of O(n).
        /// This is equivalent to <see cref="GeoAlgorithms.PointInPolygon"/> operating on the <see
        /// cref="Origin"/> coordinates of all half-edges in the cycle that begins with this <see
        /// cref="SubdivisionEdge"/> and continues along the chain of <see cref="Next"/> pointers.
        /// </remarks>

        public PolygonLocation Locate(PointD q) {

            // number of right & left crossings of edge & ray
            int rightCrossings = 0, leftCrossings = 0;

            // get starting point for first edge
            var edge = this;
            double x1 = edge._origin.X - q.X;
            double y1 = edge._origin.Y - q.Y;

            do {
                // get end point for current edge
                edge = edge._next;
                double x0 = edge._origin.X - q.X;
                double y0 = edge._origin.Y - q.Y;

                // check if q matches current vertex
                if (x0 == 0 && y0 == 0)
                    return PolygonLocation.Vertex;

                // check if current edge straddles x-axis
                bool rightStraddle = ((y0 > 0) != (y1 > 0));
                bool leftStraddle = ((y0 < 0) != (y1 < 0));

                // determine intersection of edge with x-axis
                if (rightStraddle || leftStraddle) {
                    double x = (x0 * y1 - x1 * y0) / (y1 - y0);
                    if (rightStraddle && x > 0) ++rightCrossings;
                    if (leftStraddle && x < 0) ++leftCrossings;
                }

                // move starting point for next edge
                x1 = x0; y1 = y0;

            } while (edge != this);

            // q is on edge if crossings are of different parity
            if (rightCrossings % 2 != leftCrossings % 2)
                return PolygonLocation.Edge;

            // q is inside for an odd number of crossings, else outside
            return (rightCrossings % 2 == 1 ?
                PolygonLocation.Inside : PolygonLocation.Outside);
        }

        #endregion
        #region Locate(PointD, Double)

        /// <summary>
        /// Finds the location of the specified <see cref="PointD"/> relative to the boundary of the
        /// incident <see cref="Face"/> which contains the <see cref="SubdivisionEdge"/>, given the
        /// specified epsilon for coordinate comparisons.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to locate.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which two coordinates should be considered equal.
        /// </param>
        /// <returns>
        /// A <see cref="PolygonLocation"/> value that indicates the location of <paramref
        /// name="q"/> relative to the <see cref="CyclePolygon"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="epsilon"/> is equal to or less than zero.</exception>
        /// <remarks>
        /// <b>Locate</b> is identical with the basic <see cref="Locate(PointD)"/> overload but
        /// calls <see cref="MathUtility.Compare"/> with the specified <paramref name="epsilon"/> to
        /// determine whether <paramref name="q"/> coincides with any edge or vertex of the <see
        /// cref="CyclePolygon"/>.</remarks>

        public PolygonLocation Locate(PointD q, double epsilon) {
            if (epsilon <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "epsilon", epsilon, Strings.ArgumentNotPositive);

            // number of right & left crossings of edge & ray
            int rightCrossings = 0, leftCrossings = 0;

            // get starting point for first edge
            var edge = this;
            double x1 = edge._origin.X - q.X;
            double y1 = edge._origin.Y - q.Y;
            int dy1 = MathUtility.Compare(y1, 0, epsilon);

            do {
                // get end point for current edge
                edge = edge._next;
                double x0 = edge._origin.X - q.X;
                double y0 = edge._origin.Y - q.Y;

                int dx0 = MathUtility.Compare(x0, 0, epsilon);
                int dy0 = MathUtility.Compare(y0, 0, epsilon);

                // check if q matches current vertex
                if (dx0 == 0 && dy0 == 0)
                    return PolygonLocation.Vertex;

                // check if current edge straddles x-axis
                bool rightStraddle = ((dy0 > 0) != (dy1 > 0));
                bool leftStraddle = ((dy0 < 0) != (dy1 < 0));

                // determine intersection of edge with x-axis
                if (rightStraddle || leftStraddle) {
                    double x = (x0 * y1 - x1 * y0) / (y1 - y0);
                    int dx = MathUtility.Compare(x, 0, epsilon);

                    if (rightStraddle && dx > 0) ++rightCrossings;
                    if (leftStraddle && dx < 0) ++leftCrossings;
                }

                // move starting point for next edge
                x1 = x0; y1 = y0; dy1 = dy0;
            } while (edge != this);

            // q is on edge if crossings are of different parity
            if (rightCrossings % 2 != leftCrossings % 2)
                return PolygonLocation.Edge;

            // q is inside for an odd number of crossings, else outside
            return (rightCrossings % 2 == 1 ?
                PolygonLocation.Inside : PolygonLocation.Outside);
        }

        #endregion
        #region ToLine

        /// <summary>
        /// Converts the <see cref="SubdivisionEdge"/> to a <see cref="LineD"/> with the same
        /// direction.</summary>
        /// <returns>
        /// A <see cref="LineD"/> whose <see cref="LineD.Start"/> point equals the <see
        /// cref="Origin"/> and whose <see cref="LineD.End"/> point equals the <see
        /// cref="Destination"/> of the <see cref="SubdivisionEdge"/>.</returns>

        public LineD ToLine() {
            return new LineD(_origin, _twin._origin);
        }

        #endregion
        #region ToLineReverse

        /// <summary>
        /// Converts the <see cref="SubdivisionEdge"/> to a <see cref="LineD"/> with the opposite
        /// direction.</summary>
        /// <returns>
        /// A <see cref="LineD"/> whose <see cref="LineD.Start"/> point equals the <see
        /// cref="Destination"/> and whose <see cref="LineD.End"/> point equals the <see
        /// cref="Origin"/> of the <see cref="SubdivisionEdge"/>.</returns>

        public LineD ToLineReverse() {
            return new LineD(_twin._origin, _origin);
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="SubdivisionEdge"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> containing the culture-invariant string representations of the
        /// <see cref="Key"/> and <see cref="Origin"/> properties, as well as the <see cref="Key"/>
        /// values of the <see cref="Face"/>, <see cref="Twin"/>, <see cref="Next"/>, and <see
        /// cref="Previous"/> properties. The value -1 is substituted for any properties that are
        /// null references.</returns>

        public override string ToString() {
            Func<SubdivisionFace, Int32> faceKey = (f => f == null ? -1 : f._key);
            Func<SubdivisionEdge, Int32> edgeKey = (e => e == null ? -1 : e._key);

            return String.Format(CultureInfo.InvariantCulture,
                "Edge {0}: {1}, Twin={2}, Face={3}, Next={4}, Previous={5}",
                _key, _origin, edgeKey(_twin), faceKey(_face),
                edgeKey(_next), edgeKey(_previous));
        }

        #endregion
        #endregion
        #region Internal Methods
        #region FindEdgePosition

        /// <summary>
        /// Finds the position of a half-edge to the specified <see cref="Destination"/> within the
        /// vertex chain around <see cref="Origin"/>.</summary>
        /// <param name="destination">
        /// The <see cref="Destination"/> of the half-edge whose position to find.</param>
        /// <param name="nextEdge">
        /// Returns the <see cref="SubdivisionEdge"/> that is <see cref="Next"/> from the <see
        /// cref="Twin"/> of a half-edge to <paramref name="destination"/> within the vertex chain
        /// around <see cref="Origin"/>.</param>
        /// <param name="previousEdge">
        /// Returns the <see cref="SubdivisionEdge"/> whose <see cref="Twin"/> is <see
        /// cref="Previous"/> from a half-edge to <paramref name="destination"/> within the vertex
        /// chain around <see cref="Origin"/>.</param>

        internal void FindEdgePosition(PointD destination,
            out SubdivisionEdge nextEdge, out SubdivisionEdge previousEdge) {

            nextEdge = this;
            previousEdge = this;

            // determine reference angle between new and existing edge
            PointD pivot = _twin._origin;
            double firstAngle = _origin.AngleBetween(destination, pivot);
            double angle = firstAngle;

            if (firstAngle > 0) {
                // positive angle: decrease until first negative angle found,
                // or until angle wraps around to greater than starting value
                do {
                    previousEdge = nextEdge;
                    nextEdge = nextEdge._twin._next;
                    pivot = nextEdge._twin._origin;
                    angle = _origin.AngleBetween(destination, pivot);
                } while (angle > 0 && angle < firstAngle);
            } else {
                // negative angle: increase until first positive angle found,
                // or until angle wraps around to smaller than starting value
                do {
                    nextEdge = previousEdge;
                    previousEdge = previousEdge._previous._twin;
                    pivot = previousEdge._twin._origin;
                    angle = _origin.AngleBetween(destination, pivot);
                } while (angle < 0 && angle > firstAngle);
            }
        }

        #endregion
        #region GetOtherCycleEdge

        /// <summary>
        /// Returns a half-edge on the same boundary of the incident <see cref="Face"/> that differs
        /// from the specified <see cref="SubdivisionEdge"/> and its <see cref="Twin"/>.</summary>
        /// <param name="edge">
        /// The <see cref="SubdivisionEdge"/> to avoid.</param>
        /// <returns><list type="table"><listheader>
        /// <term>Return Value</term><description>Condition</description>
        /// </listheader><item>
        /// <term><see cref="Next"/></term>
        /// <description><para>
        /// This instance equals <paramref name="edge"/>, and <see cref="Next"/> does not equal <see
        /// cref="Twin"/> of <paramref name="edge"/>.
        /// </para><para>-or-</para><para>
        /// This instance equals <see cref="Twin"/> of <paramref name="edge"/>, and <see
        /// cref="Next"/> does not equal <paramref name="edge"/>.</para></description>
        /// </item><item>
        /// <term><see cref="Previous"/></term>
        /// <description><para>
        /// This instance equals <paramref name="edge"/>, and <see cref="Next"/> equals <see
        /// cref="Twin"/> of <paramref name="edge"/> but <see cref="Previous"/> does not.
        /// </para><para>-or-</para><para>
        /// This instance equals <see cref="Twin"/> of <paramref name="edge"/>, and <see
        /// cref="Next"/> equals <paramref name="edge"/> but <see cref="Previous"/> does not.
        /// </para></description>
        /// </item><item>
        /// <term>A null reference</term>
        /// <description><para>
        /// This instance equals <paramref name="edge"/>, and <see cref="Next"/> and <see
        /// cref="Previous"/> both equal <see cref="Twin"/> of <paramref name="edge"/>.
        /// </para><para>-or-</para><para>
        /// This instance equals <see cref="Twin"/> of <paramref name="edge"/>, and <see
        /// cref="Next"/> and <see cref="Previous"/> both equal <paramref name="edge"/>.
        /// </para></description>
        /// </item><item>
        /// <term>This instance</term><description>
        /// This instance equals neither <paramref name="edge"/> nor its <see cref="Twin"/>.
        /// </description></item></list></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="edge"/> is a null reference.</exception>

        internal SubdivisionEdge GetOtherCycleEdge(SubdivisionEdge edge) {

            if (this == edge) {
                if (_next != _twin) return _next;
                if (_previous != _twin) return _previous;
                return null;
            }

            if (this == edge._twin) {
                if (_next != edge) return _next;
                if (_previous != edge) return _previous;
                return null;
            }

            return this;
        }

        #endregion
        #region IsCompatibleDestination

        /// <summary>
        /// Determines whether the specified <see cref="Destination"/> is compatible with the vertex
        /// chain around <see cref="Origin"/>.</summary>
        /// <param name="destination">
        /// The <see cref="PointD"/> coordinates of the new <see cref="Destination"/>.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="destination"/> is compatible with the vertex chain around
        /// <see cref="Origin"/>; otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>IsCompatibleDestination</b> always returns <c>true</c> if the <see
        /// cref="SubdivisionEdge"/> is the only incident half-edge at its <see cref="Origin"/>, or
        /// if the specified <paramref name="destination"/> equals the current <see
        /// cref="Destination"/>.
        /// </para><para>
        /// Otherwise, <b>IsCompatibleDestination</b> returns <c>true</c> exactly if rotating the
        /// <see cref="SubdivisionEdge"/> to <paramref name="destination"/>, in the direction that
        /// minimizes angular distance, would not traverse any neighboring edge in the vertex chain
        /// around <see cref="Origin"/>.</para></remarks>

        internal bool IsCompatibleDestination(PointD destination) {

            // succeed if only one edge or same destination
            if (_next == _twin) return true;
            PointD pivot = _twin._origin;
            if (pivot == destination) return true;

            // compute angles to destination and previous edge
            double pivotAngle = _origin.AngleBetween(pivot, destination);
            double prevAngle = _origin.AngleBetween(pivot, _previous._origin);

            // compute angle to next edge, if different
            SubdivisionEdge next = _twin._next._twin;
            double nextAngle = (_previous == next ? prevAngle :
                _origin.AngleBetween(pivot, next._origin));

            // adjust signs of neighboring angles
            if (prevAngle > 0) prevAngle -= 2 * Math.PI;
            if (nextAngle < 0) nextAngle += 2 * Math.PI;

            if (pivotAngle < 0) {
                if (prevAngle < 0)
                    return (pivotAngle > prevAngle);
                else {
                    Debug.Assert(nextAngle < 0);
                    return (pivotAngle > nextAngle);
                }
            } else {
                if (prevAngle > 0)
                    return (pivotAngle < prevAngle);
                else {
                    Debug.Assert(nextAngle > 0);
                    return (pivotAngle < nextAngle);
                }
            }
        }

        #endregion
        #region SetAllFaces

        /// <summary>
        /// Sets the <see cref="Face"/> property of this <see cref="SubdivisionEdge"/> and all other
        /// half-edges in the same cycle to the specified value.</summary>
        /// <param name="face">
        /// The new value for the <see cref="Face"/> property of each <see cref="SubdivisionEdge"/>.
        /// </param>

        internal void SetAllFaces(SubdivisionFace face) {
            Debug.Assert(face != null);

            SubdivisionEdge cursor = this;
            do {
                cursor._face = face;
                cursor = cursor._next;
            } while (cursor != this);
        }

        #endregion
        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="SubdivisionEdge"/> instances have the same value.
        /// </overloads>
        /// <summary>
        /// Determines whether this <see cref="SubdivisionEdge"/> instance and a specified object,
        /// which must be a <see cref="SubdivisionEdge"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="SubdivisionEdge"/> instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="SubdivisionEdge"/> instance
        /// and its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="SubdivisionEdge"/>
        /// instance, or an instance of a derived class, <b>Equals</b> invokes the strongly-typed
        /// <see cref="Equals(SubdivisionEdge)"/> overload to test the two instances for value
        /// equality.</remarks>

        public override bool Equals(object obj) {
            return Equals(obj as SubdivisionEdge);
        }

        #endregion
        #region Equals(SubdivisionEdge)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="SubdivisionEdge"/> have the
        /// same value.</summary>
        /// <param name="edge">
        /// A <see cref="SubdivisionEdge"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="edge"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>Equals</b> compares the values all properties of the two <see
        /// cref="SubdivisionEdge"/> instances to test for value equality. Properties of type <see
        /// cref="SubdivisionEdge"/> and <see cref="SubdivisionFace"/> are compared using <see
        /// cref="Object.ReferenceEquals"/>.
        /// </para><para>
        /// <b>Equals</b> is intended for unit testing, as any two <see cref="SubdivisionEdge"/>
        /// instances created during normal operation are never equal.</para></remarks>

        public bool Equals(SubdivisionEdge edge) {

            if (Object.ReferenceEquals(edge, this)) return true;
            if (Object.ReferenceEquals(edge, null)) return false;

            return (_origin.Equals(edge._origin)
                && Object.ReferenceEquals(_face, edge._face)
                && Object.ReferenceEquals(_twin, edge._twin)
                && Object.ReferenceEquals(_next, edge._next)
                && Object.ReferenceEquals(_previous, edge._previous));
        }

        #endregion
        #region Equals(SubdivisionEdge, SubdivisionEdge)

        /// <summary>
        /// Determines whether two specified <see cref="SubdivisionEdge"/> instances have the same
        /// value.</summary>
        /// <param name="x">
        /// The first <see cref="SubdivisionEdge"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="SubdivisionEdge"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(SubdivisionEdge)"/> overload to
        /// test the two <see cref="SubdivisionEdge"/> instances for value equality.</remarks>

        public static bool Equals(SubdivisionEdge x, SubdivisionEdge y) {

            if (Object.ReferenceEquals(x, null))
                return Object.ReferenceEquals(y, null);

            return x.Equals(y);
        }

        #endregion
        #region StructureEquals

        /// <summary>
        /// Determines whether this instance and a specified <see cref="SubdivisionEdge"/> have the
        /// same structure.</summary>
        /// <param name="edge">
        /// A <see cref="SubdivisionEdge"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the structure of <paramref name="edge"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks><para>
        /// <b>StructureEquals</b> compares the values all properties of the two <see
        /// cref="SubdivisionEdge"/> instances to test for structural equality. Properties of type
        /// <see cref="SubdivisionEdge"/> and <see cref="SubdivisionFace"/> are compared by their
        /// <see cref="Key"/> values.
        /// </para><para>
        /// <b>StructureEquals</b> is intended for testing the <see cref="Subdivision.Clone"/> 
        /// method which replicate <see cref="SubdivisionEdge"/> and <see cref="SubdivisionFace"/>
        /// keys but not references.</para></remarks>

        public bool StructureEquals(SubdivisionEdge edge) {

            if (Object.ReferenceEquals(edge, this)) return true;
            if (Object.ReferenceEquals(edge, null)) return false;

            return (_key == edge._key
                && _origin == edge._origin
                && _face._key == edge._face._key
                && _twin._key == edge._twin._key
                && _next._key == edge._next._key
                && _previous._key == edge._previous._key);
        }

        #endregion
        #endregion
    }
}
