using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Provides algorithms to find the intersection of two line segments.</summary>
    /// <remarks><para>
    /// <b>LineIntersection</b> is an immutable structure that describes the intersection of two
    /// line segments. This includes the absolute and relative locations of the intersection point
    /// and the spatial relationship between the intersected line segments.
    /// </para><para>
    /// <b>LineIntersection</b> also provides a static <see cref="LineIntersection.Find"/> method 
    /// that determines the intersection of two specified line segments, defined by the <see
    /// cref="PointD"/> coordinates of their start and end points.</para></remarks>

    [Serializable, StructLayout(LayoutKind.Auto)]
    public struct LineIntersection: IEquatable<LineIntersection> {
        #region LineIntersection(LineRelation)

        /// <overloads>
        /// Initializes a new instance of the <see cref="LineIntersection"/> structure.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="LineIntersection"/> structure with the
        /// specified spatial relationship between the intersected lines.</summary>
        /// <param name="relation">
        /// The spatial relationship between the two line segments.</param>
        /// <remarks>
        /// The <see cref="First"/>, <see cref="Second"/>, and <see cref="Shared"/> properties
        /// remain at their default values. Use this constructor for collinear or parallel line
        /// segments that share no points.</remarks>

        public LineIntersection(LineRelation relation): this(null, 0, 0, relation) { }

        #endregion
        #region LineIntersection(PointD, LineLocation, LineLocation, LineRelation)

        /// <summary>
        /// Initializes a new instance of the <see cref="LineIntersection"/> structure with the
        /// specified shared coordinates, relative locations, and spatial relationship between the
        /// intersected lines.</summary>
        /// <param name="shared">
        /// The <see cref="PointD"/> coordinates shared by the two line segments or their infinite
        /// extensions.</param>
        /// <param name="first">
        /// The location of the <see cref="Shared"/> coordinates relative to the first line segment.
        /// </param>
        /// <param name="second">
        /// The location of the <see cref="Shared"/> coordinates relative to the second line
        /// segment.</param>
        /// <param name="relation">
        /// The spatial relationship between the two line segments.</param>

        public LineIntersection(PointD? shared,
            LineLocation first, LineLocation second, LineRelation relation) {

            Shared = shared;
            First = first;
            Second = second;
            Relation = relation;
        }

        #endregion
        #region Exists

        /// <summary>
        /// Gets a value indicating whether both line segments contain the <see cref="Shared"/>
        /// coordinates.</summary>
        /// <value>
        /// <c>true</c> if both <see cref="First"/> and <see cref="Second"/> equal either <see
        /// cref="LineLocation.Start"/>, <see cref="LineLocation.Between"/>, or <see
        /// cref="LineLocation.End"/>; otherwise, <c>false</c>.</value>
        /// <remarks><para>
        /// <b>Exists</b> requires that both <see cref="First"/> and <see cref="Second"/> equal one
        /// of the indicated <see cref="LineLocation"/> values, but not necessarily the same value.
        /// </para><para>
        /// <b>Exists</b> indicates whether the two line segments themselves intersect. The <see
        /// cref="Shared"/> coordinates may be valid even if <b>Exists</b> is <c>false</c>,
        /// indicating an intersection of the infinite extensions of either or both line segments.
        /// </para></remarks>

        public bool Exists {
            get { return (Contains(First) && Contains(Second)); }
        }

        #endregion
        #region ExistsBetween

        /// <summary>
        /// Gets a value indicating whether both line segments contain the <see cref="Shared"/>
        /// coordinates, excluding the end points of at least one line segment.</summary>
        /// <value>
        /// <c>true</c> if either <see cref="First"/> or <see cref="Second"/> equals <see
        /// cref="LineLocation.Between"/>, and the other property equals either <see
        /// cref="LineLocation.Start"/>, <see cref="LineLocation.Between"/>, or <see
        /// cref="LineLocation.End"/>; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// <b>ExistsBetween</b> indicates whether the two line segments themselves intersect.
        /// Unlike <see cref="Exists"/>, at least one line segment must be properly intersected by
        /// the other, i.e. not just touched at an end point.</remarks>

        public bool ExistsBetween {
            get {
                return ((Contains(First) && Second == LineLocation.Between)
                    || (First == LineLocation.Between && Contains(Second)));
            }
        }

        #endregion
        #region First

        /// <summary>
        /// The location of the <see cref="Shared"/> coordinates relative to the first line segment.
        /// </summary>
        /// <remarks>
        /// <b>First</b> holds a <see cref="LineLocation"/> value indicating the location of the
        /// <see cref="Shared"/> coordinates relative to the first line segment, or <see
        /// cref="LineLocation.None"/> if no intersection was found. The default is <see
        /// cref="LineLocation.None"/>.</remarks>

        public readonly LineLocation First;

        #endregion
        #region Relation

        /// <summary>
        /// The spatial relationship between the two line segments.</summary>
        /// <remarks>
        /// <b>Relation</b> holds a <see cref="LineRelation"/> value indicating the spatial
        /// relationship between the two line segments. The default is <see
        /// cref="LineRelation.Parallel"/>.</remarks>

        public readonly LineRelation Relation;

        #endregion
        #region Second

        /// <summary>
        /// The location of the <see cref="Shared"/> coordinates relative to the second line
        /// segment.</summary>
        /// <remarks>
        /// <b>Second</b> holds a <see cref="LineLocation"/> value indicating the location of the
        /// <see cref="Shared"/> coordinates relative to the second line segment, or <see
        /// cref="LineLocation.None"/> if no intersection was found. The default is <see
        /// cref="LineLocation.None"/>.</remarks>

        public readonly LineLocation Second;

        #endregion
        #region Shared

        /// <summary>
        /// The <see cref="PointD"/> coordinates shared by the two line segments or their infinite
        /// extensions.</summary>
        /// <remarks><para>
        /// <b>Shared</b> holds the <see cref="PointD"/> coordinates shared by the two line segments
        /// or their infinite extensions, or a null reference if no intersection was found. The
        /// default is a null reference.
        /// </para><para>
        /// Valid <b>Shared</b> coordinates are generally computed, but may be copied from the <see
        /// cref="LineD.Start"/> or <see cref="LineD.End"/> points of an intersecting line if <see
        /// cref="First"/> and/or <see cref="Second"/> equals <see cref="LineLocation.Start"/> or
        /// <see cref="LineLocation.End"/>.
        /// </para><para>
        /// <b>Shared</b> holds the following special values if <see cref="Relation"/> equals <see
        /// cref="LineRelation.Collinear"/>:
        /// </para><list type="bullet"><item>
        /// If the two line segments overlap, <b>Shared</b> is not computed but set directly to the
        /// <see cref="LineD.Start"/> or <see cref="LineD.End"/> point of the second line segment,
        /// whichever is contained by the first line segment and is lexicographically smaller,
        /// according to <see cref="PointDComparerY"/>.
        /// </item><item>
        /// Otherwise, <b>Shared</b> is set to a null reference, even though the infinite extensions
        /// of the line segments share all their points.</item></list></remarks>

        public readonly PointD? Shared;

        #endregion
        #region Contains

        /// <summary>
        /// Determines whether the specified <see cref="LineLocation"/> value specifies that the
        /// tested line segment contains the tested point.</summary>
        /// <param name="location">
        /// The <see cref="LineLocation"/> value to examine.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="location"/> equals <see cref="LineLocation.Start"/>, <see
        /// cref="LineLocation.Between"/>, or <see cref="LineLocation.End"/>; otherwise,
        /// <c>false</c>.</returns>
        /// <remarks>
        /// <b>Contains</b> uses a bit mask for efficient testing of the specified <paramref
        /// name="location"/> for the three acceptable <see cref="LineLocation"/> values.</remarks>

        public static bool Contains(LineLocation location) {
            const LineLocation mask = LineLocation.Start | LineLocation.Between | LineLocation.End;
            return ((location & mask) != 0);
        }

        #endregion
        #region Find(PointD, PointD, PointD, PointD)

        /// <overloads>
        /// Finds the intersection of the specified line segments.</overloads>
        /// <summary>
        /// Finds the intersection of the specified line segments, using exact coordinate
        /// comparisons.</summary>
        /// <param name="a">
        /// The <see cref="LineD.Start"/> point of the first line segment.</param>
        /// <param name="b">
        /// The <see cref="LineD.End"/> point of the first line segment.</param>
        /// <param name="c">
        /// The <see cref="LineD.Start"/> point of the second line segment.</param>
        /// <param name="d">
        /// The <see cref="LineD.End"/> point of the second line segment.</param>
        /// <returns>
        /// A <see cref="LineIntersection"/> instance that describes if and how the line segments
        /// from <paramref name="a"/> to <paramref name="b"/> and from <paramref name="c"/> to
        /// <paramref name="d"/> intersect.</returns>
        /// <remarks><para>
        /// <b>Find</b> was adapted from the <c>Segments-Intersect</c> algorithm by Thomas H. Cormen
        /// et al., <em>Introduction to Algorithms</em> (3rd ed.), The MIT Press 2009, p.1018, for
        /// intersection testing; and from the <c>SegSegInt</c> and <c>ParallelInt</c> algorithms by
        /// Joseph O’Rourke, <em>Computational Geometry in C</em> (2nd ed.), Cambridge University
        /// Press 1998, p.224f, for line relationships and shared coordinates.
        /// </para><para>
        /// Cormen’s intersection testing first examines the <see cref="PointD.CrossProductLength"/>
        /// for each triplet of specified points. If that is insufficient, O’Rourke’s algorithm then
        /// examines the parameters of both line equations. This is mathematically redundant since
        /// O’Rourke’s algorithm alone should produce all desired information, but the combination
        /// of both algorithms proved much more resilient against misjudging line relationships due
        /// to floating-point inaccuracies.
        /// </para><para>
        /// Although most comparisons in this overload are exact, cross-product testing is always
        /// performed with a minimum epsilon of 1e-10. Moreover, <b>Find</b> will return the result
        /// of the other <see cref="Find(PointD, PointD, PointD, PointD, Double)"/> overload with an
        /// epsilon of 2e-10 if cross-product testing contradicts line equation testing. Subsequent
        /// contradictions result in further recursive calls, each time with a doubled epsilon,
        /// until an intersection can be determined without contradictions.</para></remarks>

        public static LineIntersection Find(PointD a, PointD b, PointD c, PointD d) {
            const double epsilon = 1e-10;
            LineLocation first, second;

            double bax = b.X - a.X, bay = b.Y - a.Y;
            double dcx = d.X - c.X, dcy = d.Y - c.Y;

            // compute cross-products for all end points
            double d1 = (a.X - c.X) * dcy - (a.Y - c.Y) * dcx;
            double d2 = (b.X - c.X) * dcy - (b.Y - c.Y) * dcx;
            double d3 = (c.X - a.X) * bay - (c.Y - a.Y) * bax;
            double d4 = (d.X - a.X) * bay - (d.Y - a.Y) * bax;

            //Debug.Assert(d1 == c.CrossProductLength(a, d));
            //Debug.Assert(d2 == c.CrossProductLength(b, d));
            //Debug.Assert(d3 == a.CrossProductLength(c, b));
            //Debug.Assert(d4 == a.CrossProductLength(d, b));

            /*
             * Some cross-products are zero: corresponding end point triplets are collinear.
             * 
             * The infinite lines intersect on the corresponding end points. The lines are collinear
             * exactly if all cross-products are zero; otherwise, the lines are divergent and we
             * need to check whether the finite line segments also intersect on the end points.
             * 
             * We always perform epsilon comparisons on cross-products, even in the exact overload,
             * because almost-zero cases are very frequent, especially for collinear lines.
             */
            if (Math.Abs(d1) <= epsilon && Math.Abs(d2) <= epsilon &&
                Math.Abs(d3) <= epsilon && Math.Abs(d4) <= epsilon) {

                // find lexicographically first point where segments overlap
                if (PointDComparerY.CompareExact(c, d) < 0) {
                    first = LocateCollinear(a, b, c);
                    if (Contains(first))
                        return new LineIntersection(c, first, LineLocation.Start, LineRelation.Collinear);

                    first = LocateCollinear(a, b, d);
                    if (Contains(first))
                        return new LineIntersection(d, first, LineLocation.End, LineRelation.Collinear);
                } else {
                    first = LocateCollinear(a, b, d);
                    if (Contains(first))
                        return new LineIntersection(d, first, LineLocation.End, LineRelation.Collinear);

                    first = LocateCollinear(a, b, c);
                    if (Contains(first))
                        return new LineIntersection(c, first, LineLocation.Start, LineRelation.Collinear);
                }

                // collinear line segments without overlapping points
                return new LineIntersection(LineRelation.Collinear);
            }

            // check for divergent lines with end point intersection
            if (Math.Abs(d1) <= epsilon) {
                second = LocateCollinear(c, d, a);
                return new LineIntersection(a, LineLocation.Start, second, LineRelation.Divergent);
            }
            if (Math.Abs(d2) <= epsilon) {
                second = LocateCollinear(c, d, b);
                return new LineIntersection(b, LineLocation.End, second, LineRelation.Divergent);
            }
            if (Math.Abs(d3) <= epsilon) {
                first = LocateCollinear(a, b, c);
                return new LineIntersection(c, first, LineLocation.Start, LineRelation.Divergent);
            }
            if (Math.Abs(d4) <= epsilon) {
                first = LocateCollinear(a, b, d);
                return new LineIntersection(d, first, LineLocation.End, LineRelation.Divergent);
            }

            /*
             * All cross-products are non-zero: divergent or parallel lines.
             * 
             * The lines and segments might intersect, but not on any end point.
             * Compute parameters of both line equations to determine intersections.
             * Zero denominator indicates parallel lines (but not collinear, see above).
             */
            double denom = dcx * bay - bax * dcy;
            if (Math.Abs(denom) <= epsilon)
                return new LineIntersection(LineRelation.Parallel);

            /*
             * Compute position of intersection point relative to line segments, and also perform
             * sanity checks for floating-point inaccuracies. If a check fails, we cannot give a
             * reliable result at the current precision and must recurse with a greater epsilon.
             * 
             * Cross-products have pairwise opposite signs exactly if the corresponding line segment
             * straddles the infinite extension of the other line segment, implying a line equation
             * parameter between zero and one. Pairwise identical signs imply a parameter less than
             * zero or greater than one. Parameters cannot be exactly zero or one, as that indicates
             * end point intersections which were already ruled out by cross-product testing.
             */
            double snum = a.X * dcy - a.Y * dcx - c.X * d.Y + c.Y * d.X;
            double s = snum / denom;

            if ((d1 < 0 && d2 < 0) || (d1 > 0 && d2 > 0)) {
                if (s < 0) first = LineLocation.Before;
                else if (s > 1) first = LineLocation.After;
                else return Find(a, b, c, d, 2 * epsilon);
            } else {
                if (s > 0 && s < 1) first = LineLocation.Between;
                else return Find(a, b, c, d, 2 * epsilon);
            }

            double tnum = c.Y * bax - c.X * bay + a.X * b.Y - a.Y * b.X;
            double t = tnum / denom;

            if ((d3 < 0 && d4 < 0) || (d3 > 0 && d4 > 0)) {
                if (t < 0) second = LineLocation.Before;
                else if (t > 1) second = LineLocation.After;
                else return Find(a, b, c, d, 2 * epsilon);
            } else {
                if (t > 0 && t < 1) second = LineLocation.Between;
                else return Find(a, b, c, d, 2 * epsilon);
            }

            PointD shared = new PointD(a.X + s * bax, a.Y + s * bay);
            return new LineIntersection(shared, first, second, LineRelation.Divergent);
        }

        #endregion
        #region Find(PointD, PointD, PointD, PointD, Double)

        /// <summary>
        /// Finds the intersection of the specified line segments, given the specified epsilon for
        /// coordinate comparisons.</summary>
        /// <param name="a">
        /// The <see cref="LineD.Start"/> point of the first line segment.</param>
        /// <param name="b">
        /// The <see cref="LineD.End"/> point of the first line segment.</param>
        /// <param name="c">
        /// The <see cref="LineD.Start"/> point of the second line segment.</param>
        /// <param name="d">
        /// The <see cref="LineD.End"/> point of the second line segment.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which coordinates and intermediate results should be
        /// considered equal. This value is always raised to a minium of 1e-10.</param>
        /// <returns>
        /// A <see cref="LineIntersection"/> instance that describes if and how the line segments
        /// from <paramref name="a"/> to <paramref name="b"/> and from <paramref name="c"/> to
        /// <paramref name="d"/> intersect.</returns>
        /// <remarks><para>
        /// <b>Find</b> is identical with the basic <see cref="Find(PointD, PointD, PointD,
        /// PointD)"/> overload but uses the specified <paramref name="epsilon"/> to compare
        /// coordinates and intermediate results.
        /// </para><para>
        /// <b>Find</b> always raises the specified <paramref name="epsilon"/> to a minimum of 1e-10
        /// because the algorithm is otherwise too unstable, and would initiate multiple recursions
        /// with a greater epsilon anyway.</para></remarks>

        public static LineIntersection Find(PointD a, PointD b, PointD c, PointD d, double epsilon) {
            if (epsilon < 1e-10) epsilon = 1e-10;
            LineLocation first, second;

            double bax = b.X - a.X, bay = b.Y - a.Y;
            double dcx = d.X - c.X, dcy = d.Y - c.Y;

            // compute cross-products for all end points
            double d1 = (a.X - c.X) * dcy - (a.Y - c.Y) * dcx;
            double d2 = (b.X - c.X) * dcy - (b.Y - c.Y) * dcx;
            double d3 = (c.X - a.X) * bay - (c.Y - a.Y) * bax;
            double d4 = (d.X - a.X) * bay - (d.Y - a.Y) * bax;

            //Debug.Assert(d1 == c.CrossProductLength(a, d));
            //Debug.Assert(d2 == c.CrossProductLength(b, d));
            //Debug.Assert(d3 == a.CrossProductLength(c, b));
            //Debug.Assert(d4 == a.CrossProductLength(d, b));

            // check for collinear (but not parallel) lines
            if (Math.Abs(d1) <= epsilon && Math.Abs(d2) <= epsilon &&
                Math.Abs(d3) <= epsilon && Math.Abs(d4) <= epsilon) {

                // find lexicographically first point where segments overlap
                if (PointDComparerY.CompareExact(c, d) < 0) {
                    first = LocateCollinear(a, b, c, epsilon);
                    if (Contains(first))
                        return new LineIntersection(c, first, LineLocation.Start, LineRelation.Collinear);

                    first = LocateCollinear(a, b, d, epsilon);
                    if (Contains(first))
                        return new LineIntersection(d, first, LineLocation.End, LineRelation.Collinear);
                } else {
                    first = LocateCollinear(a, b, d, epsilon);
                    if (Contains(first))
                        return new LineIntersection(d, first, LineLocation.End, LineRelation.Collinear);

                    first = LocateCollinear(a, b, c, epsilon);
                    if (Contains(first))
                        return new LineIntersection(c, first, LineLocation.Start, LineRelation.Collinear);
                }

                // collinear line segments without overlapping points
                return new LineIntersection(LineRelation.Collinear);
            }

            // check for divergent lines with end point intersection
            if (Math.Abs(d1) <= epsilon) {
                second = LocateCollinear(c, d, a, epsilon);
                return new LineIntersection(a, LineLocation.Start, second, LineRelation.Divergent);
            }
            if (Math.Abs(d2) <= epsilon) {
                second = LocateCollinear(c, d, b, epsilon);
                return new LineIntersection(b, LineLocation.End, second, LineRelation.Divergent);
            }
            if (Math.Abs(d3) <= epsilon) {
                first = LocateCollinear(a, b, c, epsilon);
                return new LineIntersection(c, first, LineLocation.Start, LineRelation.Divergent);
            }
            if (Math.Abs(d4) <= epsilon) {
                first = LocateCollinear(a, b, d, epsilon);
                return new LineIntersection(d, first, LineLocation.End, LineRelation.Divergent);
            }

            // compute parameters of line equations
            double denom = dcx * bay - bax * dcy;
            if (Math.Abs(denom) <= epsilon)
                return new LineIntersection(LineRelation.Parallel);

            double snum = a.X * dcy - a.Y * dcx - c.X * d.Y + c.Y * d.X;
            double s = snum / denom;

            if ((d1 < 0 && d2 < 0) || (d1 > 0 && d2 > 0)) {
                if (s < 0) first = LineLocation.Before;
                else if (s > 1) first = LineLocation.After;
                else return Find(a, b, c, d, 2 * epsilon);
            } else {
                if (s > 0 && s < 1) first = LineLocation.Between;
                else return Find(a, b, c, d, 2 * epsilon);
            }

            double tnum = c.Y * bax - c.X * bay + a.X * b.Y - a.Y * b.X;
            double t = tnum / denom;

            if ((d3 < 0 && d4 < 0) || (d3 > 0 && d4 > 0)) {
                if (t < 0) second = LineLocation.Before;
                else if (t > 1) second = LineLocation.After;
                else return Find(a, b, c, d, 2 * epsilon);
            } else {
                if (t > 0 && t < 1) second = LineLocation.Between;
                else return Find(a, b, c, d, 2 * epsilon);
            }

            PointD shared = new PointD(a.X + s * bax, a.Y + s * bay);

            /*
             * Epsilon comparisons of cross products (or line equation parameters) might miss
             * epsilon-close end point intersections of very long line segments. We compensate by
             * directly comparing the computed intersection point against the four end points.
             */

            if (PointD.Equals(a, shared, epsilon))
                first = LineLocation.Start;
            else if (PointD.Equals(b, shared, epsilon))
                first = LineLocation.End;

            if (PointD.Equals(c, shared, epsilon))
                second = LineLocation.Start;
            else if (PointD.Equals(d, shared, epsilon))
                second = LineLocation.End;

            return new LineIntersection(shared, first, second, LineRelation.Divergent);
        }

        #endregion
        #region LocateCollinear(PointD, PointD, PointD)

        /// <overloads>
        /// Determines the location of the specified <see cref="PointD"/> coordinates relative to
        /// the specified line segment, assuming they are collinear.</overloads>
        /// <summary>
        /// Determines the location of the specified <see cref="PointD"/> coordinates relative to
        /// the specified line segment, assuming they are collinear and using exact coordinate
        /// comparisons.</summary>
        /// <param name="a">
        /// The <see cref="LineD.Start"/> point of the line segment.</param>
        /// <param name="b">
        /// The <see cref="LineD.End"/> point of the line segment.</param>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <returns>
        /// A <see cref="LineLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the line segment from <paramref name="a"/> to <paramref name="b"/>.
        /// </returns>
        /// <remarks>
        /// <b>LocateCollinear</b> is identical with the <see cref="LineD.LocateCollinear(PointD)"/> 
        /// instance method of the <see cref="LineD"/> structure but takes the start and end points
        /// of the line segment as explicit parameters.</remarks>

        public static LineLocation LocateCollinear(PointD a, PointD b, PointD q) {

            if (q == a) return LineLocation.Start;
            if (q == b) return LineLocation.End;

            double ax = b.X - a.X, ay = b.Y - a.Y;
            double bx = q.X - a.X, by = q.Y - a.Y;

            if (ax * bx < 0 || ay * by < 0)
                return LineLocation.Before;
            if (ax * ax + ay * ay < bx * bx + by * by)
                return LineLocation.After;

            return LineLocation.Between;
        }

        #endregion
        #region LocateCollinear(PointD, PointD, PointD, Double)

        /// <summary>
        /// Determines the location of the specified <see cref="PointD"/> coordinates relative to
        /// the specified line segment, assuming they are collinear and given the specified epsilon
        /// for coordinate comparisons.</summary>
        /// <param name="a">
        /// The <see cref="LineD.Start"/> point of the line segment.</param>
        /// <param name="b">
        /// The <see cref="LineD.End"/> point of the line segment.</param>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to examine.</param>
        /// <param name="epsilon">
        /// The maximum absolute value at which intermediate results should be considered zero.
        /// </param>
        /// <returns>
        /// A <see cref="LineLocation"/> value indicating the location of <paramref name="q"/>
        /// relative to the line segment from <paramref name="a"/> to <paramref name="b"/>.
        /// </returns>
        /// <remarks><para>
        /// <b>LocateCollinear</b> is identical with the <see cref="LineD.LocateCollinear(PointD,
        /// Double)"/> instance method of the <see cref="LineD"/> structure but takes the start and
        /// end points of the line segment as explicit parameters.
        /// </para><para>
        /// The specified <paramref name="epsilon"/> must be greater than zero, but
        /// <b>LocateCollinear</b> does not check this condition.</para></remarks>

        public static LineLocation LocateCollinear(PointD a, PointD b, PointD q, double epsilon) {

            if (PointD.Equals(q, a, epsilon)) return LineLocation.Start;
            if (PointD.Equals(q, b, epsilon)) return LineLocation.End;

            double ax = b.X - a.X, ay = b.Y - a.Y;
            double bx = q.X - a.X, by = q.Y - a.Y;

            if (ax * bx < 0 || ay * by < 0)
                return LineLocation.Before;
            if (ax * ax + ay * ay < bx * bx + by * by)
                return LineLocation.After;

            return LineLocation.Between;
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="LineIntersection"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> returns the result of <see cref="PointD.GetHashCode"/> for the <see
        /// cref="Shared"/> property, if valid; otherwise, zero.</remarks>

        public override unsafe int GetHashCode() {
            unchecked {
                PointD shared = Shared.GetValueOrDefault();
                double x = shared.X, y = shared.Y;
                long xi = *((long*) &x), yi = *((long*) &y);
                return (int) xi ^ (int) (xi >> 32) ^ (int) yi ^ (int) (yi >> 32);
            }
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="LineIntersection"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> containing the values of the <see cref="Shared"/>, <see
        /// cref="First"/>, <see cref="Second"/>, and <see cref="Relation"/> properties.</returns>

        public override string ToString() {
            string shared = (Shared.HasValue ? Shared.ToString() : "null");

            return String.Format(CultureInfo.InvariantCulture,
                "{{Shared={0}, First={1}, Second={2}, Relation={3}}}",
                shared, First, Second, Relation);
        }

        #endregion
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="LineIntersection"/> instances have the same value.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="LineIntersection"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="LineIntersection"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(LineIntersection)"/> method to test the two
        /// <see cref="LineIntersection"/> instances for value equality.</remarks>

        public static bool operator ==(LineIntersection x, LineIntersection y) {
            return x.Equals(y);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="LineIntersection"/> instances have different values.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="LineIntersection"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="LineIntersection"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is different from the value of
        /// <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(LineIntersection)"/> method to test the two
        /// <see cref="LineIntersection"/> instances for value inequality.</remarks>

        public static bool operator !=(LineIntersection x, LineIntersection y) {
            return !x.Equals(y);
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="LineIntersection"/> instances have the same value.
        /// </overloads>
        /// <summary>
        /// Determines whether this <see cref="LineIntersection"/> instance and a specified object,
        /// which must be a <see cref="LineIntersection"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="LineIntersection"/> instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="LineIntersection"/> instance
        /// and its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="LineIntersection"/>
        /// instance, <b>Equals</b> invokes the strongly-typed <see
        /// cref="Equals(LineIntersection)"/> overload to test the two instances for value equality.
        /// </remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is LineIntersection))
                return false;

            return Equals((LineIntersection) obj);
        }

        #endregion
        #region Equals(LineIntersection)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="LineIntersection"/> have the
        /// same value.</summary>
        /// <param name="other">
        /// A <see cref="LineIntersection"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="other"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="First"/>, <see cref="Second"/>, <see
        /// cref="Relation"/>, and <see cref="Shared"/> properties of the two <see
        /// cref="LineIntersection"/> instances to test for value equality.</remarks>

        public bool Equals(LineIntersection other) {
            return (First == other.First && Second == other.Second &&
                Relation == other.Relation && Shared == other.Shared);
        }

        #endregion
        #region Equals(LineIntersection, LineIntersection)

        /// <summary>
        /// Determines whether two specified <see cref="LineIntersection"/> instances have the same
        /// value.</summary>
        /// <param name="x">
        /// The first <see cref="LineIntersection"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="LineIntersection"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(LineIntersection)"/> overload to
        /// test the two <see cref="LineIntersection"/> instances for value equality.</remarks>

        public static bool Equals(LineIntersection x, LineIntersection y) {
            return x.Equals(y);
        }

        #endregion
        #endregion
    }
}
