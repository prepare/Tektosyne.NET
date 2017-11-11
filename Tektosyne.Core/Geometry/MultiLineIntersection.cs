using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tektosyne.Collections;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Provides algorithms to find all intersections between multiple line segments.</summary>
    /// <remarks><para>
    /// <b>MultiLineIntersection</b> provides two algorithms to find all intersections between
    /// multiple line segments, specified as arrays of <see cref="LineD"/> instances:
    /// </para><list type="bullet"><item>
    /// <see cref="MultiLineIntersection.Find"/> provides a sweep line algorithm which is very
    /// efficient if the number of intersections is much smaller than the number of lines.
    /// </item><item>
    /// <see cref="MultiLineIntersection.FindSimple"/> provides a brute force algorithm which is
    /// better suited for a small number of lines or a large number of intersections.
    /// </item></list><para>
    /// Both algorithms sort their results lexicographically on the points of intersection, first by
    /// increasing y-coordinates and in case of equality by increasing x-coordinates. The <see
    /// cref="MultiLinePoint"/> instance created for each point identifies all line segments that
    /// contain the point. Please refer to the two methods for further details.
    /// </para><para>
    /// The sweep line algorithm implemented by <see cref="MultiLineIntersection.Find"/> was first
    /// described by J. L. Bentley and T. A. Ottmann, <em>Algorithms for reporting and counting
    /// geometric intersections</em>, IEEE Transactions on Computers C-28 (1979), p.643-647. An
    /// implementation outline is given by Mark de Berg et al., <em>Computational Geometry</em> (3rd
    /// ed.), Springer-Verlag 2008, p.20-29, and a limited C++ implementation by Michael J. Laszlo,
    /// <em>Computational Geometry and Computer Graphics in C++</em>, Prentice Hall 1996, p.173-181.
    /// </para><para>
    /// Our implementation supports an unlimited number of line segments meeting at any intersection
    /// point, and uses an improved sweep line comparer that raises the algorithm’s stability to the
    /// same level as that of the brute force algorithm. A comment block at the top of the source
    /// code file gives a detailed description of this comparer.</para></remarks>

    /*
     * The Sweep Line Comparer (Status.CompareLines)
     * ---------------------------------------------
     * A critical point in the sweep line algorithm is the design of the comparison function that
     * determines the order of line segments currently intersecting the moving sweep line. The
     * straightforward approach is to compute the intersection points and first order by those, and
     * then by line slopes when multiple lines intersect the sweep line at the same point.
     *
     * This approach has the obvious weakness that the "same" point is hard to identify, given the
     * precision limits of floating-point computation. Should the comparison method fail to sort
     * line segments correctly and consistently, the sweep line structure will become corrupted and
     * the algorithm will fail to output all intersections. One could use a comparison epsilon, but
     * that creates the risk of false positives when lines are near but do not (yet) cross.
     *
     * Laszlo’s solution is to compare line segments not on the sweep line itself, but slightly
     * above or below the sweep line. With an appropriate delta, the test point is never close to an
     * intersection and directly provides a consistent ordering. However, this method is also
     * susceptible to data corruption if the chosen delta is too large for very close event points,
     * or else too small for very narrow intersection angles.
     *
     * Our implementation takes a different approach. At each event point, we first remove all
     * ending and intersecting lines from the sweep line structure, then compute the intersections
     * of all remaining lines with the current sweep line, and then add (back) all starting and
     * intersecting lines. The intersection points of (re-)added lines are <em>not</em> computed but
     * simply copy the current event point location. This permits the use of exact coordinate
     * comparisons to reliably identify line intersections. The order of intersecting lines is
     * determined by their slopes, as usual.
     *
     * Our implementation performs about as well as Laszlo’s approach, but it is far more stable.
     * Testing shows the same perfect stability as for the brute force algorithm, even with large
     * input coordinates that reliably cause data corruption in the Laszlo implementation. The <see
     * cref="MultiLineIntersection.Find"/> method still checks for evidence of data corruption and
     * throws an <see cref="InvalidOperationException"/> upon detection, but this should not happen
     * in the current implementation. (The corruption tests do not impact performance.)
     */

    public static class MultiLineIntersection {
        #region Find

        /// <summary>
        /// Finds all intersections between the specified line segments, using a sweep line
        /// algorithm.</summary>
        /// <param name="lines">
        /// An <see cref="Array"/> containing the <see cref="LineD"/> instances to intersect.
        /// </param>
        /// <returns>
        /// A lexicographically sorted <see cref="Array"/> containing a <see cref="MultiLinePoint"/>
        /// for every point of intersection between the <paramref name="lines"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="lines"/> contains a <see cref="LineD"/> instance whose <see
        /// cref="LineD.Start"/> and <see cref="LineD.End"/> coordinates are equal.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="lines"/> is a null reference.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="lines"/> contains coordinates that caused corruption to an internal
        /// search structure.</exception>
        /// <remarks><para>
        /// <b>Find</b> moves a horizontal sweep line across the specified <paramref name="lines"/>,
        /// testing only those elements for intersection which are adjacent along the sweep line.
        /// The runtime is O((n + k) log n) where k is the number of discovered intersections.
        /// </para><para>
        /// <b>Find</b> is very efficient when there are no or few intersections, achieving its best
        /// performance if the specified <paramref name="lines"/> are horizontal parallels. However,
        /// the event point schedule and sweep line structure impose a large overhead if there are
        /// many candidate lines to consider. The worst case of O(n^2) intersections is much slower
        /// than the brute force algorithm implemented by <see cref="FindSimple"/>.
        /// </para><para>
        /// <b>Find</b> always uses exact coordinate comparisons. Epsilon comparisons would corrupt
        /// the internal search structures due to the merging of nearby event points. Call <see
        /// cref="FindSimple(LineD[], Double)"/> to use epsilon comparisons.</para></remarks>

        public static MultiLinePoint[] Find(LineD[] lines) {

            Status status = new Status();
            var crossings = status.FindCore(lines);
            return EventPoint.Convert(crossings);
        }

        #endregion
        #region FindSimple(LineD[])

        /// <overloads>
        /// Finds all intersections between the specified line segments, using a brute force
        /// algorithm.</overloads>
        /// <summary>
        /// Finds all intersections between the specified line segments, using a brute force
        /// algorithm and exact coordinate comparisons.</summary>
        /// <param name="lines">
        /// An <see cref="Array"/> containing the <see cref="LineD"/> instances to intersect.
        /// </param>
        /// <returns>
        /// A lexicographically sorted <see cref="Array"/> containing a <see cref="MultiLinePoint"/>
        /// for every point of intersection between the <paramref name="lines"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="lines"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>FindSimple</b> performs a pairwise intersection of every <paramref name="lines"/>
        /// element with every other element. The runtime is therefore always O(n^2), regardless of
        /// the number of intersections found.
        /// </para><para>
        /// However, the constant factor is low and O(n^2) intersections are found in optimal time
        /// because <b>FindSimple</b> performs no additional work to avoid testing for possible
        /// intersections. For a small number of <paramref name="lines"/> (n &lt; 50),
        /// <b>FindSimple</b> usually beats the sweep line algorithm implemented by <see
        /// cref="Find"/> regardless of the number of intersections.</para></remarks>

        public static MultiLinePoint[] FindSimple(LineD[] lines) {
            if (lines == null)
                ThrowHelper.ThrowArgumentNullException("lines");

            var crossings = new BraidedTree<PointD, EventPoint>(PointDComparerY.CompareExact);

            for (int i = 0; i < lines.Length - 1; i++)
                for (int j = i + 1; j < lines.Length; j++) {
                    var crossing = lines[i].Intersect(lines[j]);

                    if (crossing.Exists) {
                        PointD p = crossing.Shared.Value;
                        BraidedTreeNode<PointD, EventPoint> node;
                        crossings.TryAddNode(p, new EventPoint(p), out node);
                        node._value.TryAddLines(i, crossing.First, j, crossing.Second);
                    }
                }

            return EventPoint.Convert(crossings.Values);
        }

        #endregion
        #region FindSimple(LineD[], Double)

        /// <summary>
        /// Finds all intersections between the specified line segments, using a brute force
        /// algorithm and given the specified epsilon for coordinate comparisons.</summary>
        /// <param name="lines">
        /// An <see cref="Array"/> containing the <see cref="LineD"/> instances to intersect.
        /// </param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which two coordinates should be considered equal.
        /// </param>
        /// <returns>
        /// A lexicographically sorted <see cref="Array"/> containing a <see cref="MultiLinePoint"/>
        /// for every point of intersection between the <paramref name="lines"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="lines"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="epsilon"/> is equal to or less than zero.</exception>
        /// <remarks>
        /// <b>FindSimple</b> is identical with the basic <see cref="FindSimple(LineD[])"/> overload
        /// but uses the specified <paramref name="epsilon"/> to determine intersections between the
        /// specified <paramref name="lines"/> and to combine nearby intersections.</remarks>

        public static MultiLinePoint[] FindSimple(LineD[] lines, double epsilon) {
            if (lines == null)
                ThrowHelper.ThrowArgumentNullException("lines");
            if (epsilon <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "epsilon", epsilon, Strings.ArgumentNotPositive);

            var crossings = new BraidedTree<PointD, EventPoint>(
                (a, b) => PointDComparerY.CompareEpsilon(a, b, epsilon));

            for (int i = 0; i < lines.Length - 1; i++)
                for (int j = i + 1; j < lines.Length; j++) {
                    var crossing = lines[i].Intersect(lines[j], epsilon);

                    if (crossing.Exists) {
                        PointD p = crossing.Shared.Value;
                        BraidedTreeNode<PointD, EventPoint> node;
                        crossings.TryAddNode(p, new EventPoint(p), out node);
                        node._value.TryAddLines(i, crossing.First, j, crossing.Second);
                    }
                }

            return EventPoint.Convert(crossings.Values);
        }

        #endregion
        #region Split

        /// <summary>
        /// Splits the specified line segments on the specified intersection points.</summary>
        /// <param name="lines">
        /// An <see cref="Array"/> containing the <see cref="LineD"/> instances to split.</param>
        /// <param name="crossings">
        /// An <see cref="Array"/> of <see cref="MultiLinePoint"/> instances containing all points
        /// of intersection between the <paramref name="lines"/>.</param>
        /// <returns>
        /// An <see cref="Array"/> containing the <see cref="LineD"/> instances resulting from
        /// splitting all <paramref name="lines"/> on the matching <paramref name="crossings"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="lines"/> or <paramref name="crossings"/> is a null reference.
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="crossings"/> contains one or more <see cref="MultiLinePoint.Lines"/>
        /// indices that are equal to or greater than the number of <paramref name="lines"/>.
        /// </exception>
        /// <remarks><para>
        /// <b>Split</b> returns a collection of line segments that are guaranteed not to intersect,
        /// except at their <see cref="LineD.Start"/> or <see cref="LineD.End"/> points. The
        /// specified <paramref name="crossings"/> are usually the result of <see cref="Find"/> or
        /// <see cref="FindSimple"/> for the specified <paramref name="lines"/>. 
        /// </para><para>
        /// <b>Split</b> sets the <see cref="LineD.Start"/> or <see cref="LineD.End"/> point of any
        /// line segment that participates in a <see cref="LineLocation.Start"/> or <see
        /// cref="LineLocation.End"/> intersection to the <see cref="MultiLinePoint.Shared"/>
        /// coordinates of that intersection, so as to preserve coordinate identities that were
        /// established by a positive comparison epsilon.</para></remarks>

        public static LineD[] Split(LineD[] lines, MultiLinePoint[] crossings) {
            if (lines == null)
                ThrowHelper.ThrowArgumentNullException("lines");
            if (crossings == null)
                ThrowHelper.ThrowArgumentNullException("crossings");

            // list of split segments, or null to use original line
            int count = lines.Length;
            List<PointD>[] segmentPoints = new List<PointD>[count];

            // process all epsilon-matched intersection points
            foreach (MultiLinePoint crossing in crossings)
                for (int k = 0; k < crossing.Lines.Length; k++) {
                    int i = crossing.Lines[k];
                    List<PointD> points = segmentPoints[i];

                    // initialize split segment list with start & end points
                    if (points == null) {
                        points = new List<PointD>(4);
                        points.Add(lines[i].Start);
                        points.Add(lines[i].End);
                        segmentPoints[i] = points;
                    }

                    switch (crossing.Locations[k]) {
                    case LineLocation.Start:
                        // replace start point with epsilon-matched intersection
                        points[0] = crossing.Shared;
                        break;

                    case LineLocation.End:
                        // replace end point with epsilon-matched intersection
                        points[1] = crossing.Shared;
                        break;

                    case LineLocation.Between:
                        // add intersection point that defines new split segment
                        points.Add(crossing.Shared);
                        ++count;
                        break;
                    }
                }

            /*
             * Comparison function to sort split segment point list.
             * 
             * We cannot use lexicographic or other single-coordinate comparisons because
             * epsilon matching might cause coordinate aberrations in the wrong direction.
             * So we compare the squared distances of both points from the start point.
             */

            PointD start = PointD.Empty;
            Comparison<PointD> comparison = (a, b) => {

                double ax = a.X - start.X, ay = a.Y - start.Y;
                double bx = b.X - start.X, by = b.Y - start.Y;
                double d = (ax * ax + ay * ay) - (bx * bx + by * by);

                if (d < 0) return -1;
                if (d > 0) return +1;
                return 0;
            };

            LineD[] segments = new LineD[count];
            int index = 0;

            for (int i = 0; i < lines.Length; i++) {
                List<PointD> points = segmentPoints[i];
                if (points == null) {
                    // no intersections, store original line
                    segments[index++] = lines[i];
                } else {
                    Debug.Assert(points.Count > 2);

                    // sort points by distance from start point
                    start = points[0];
                    points.Sort(comparison);

                    // convert sorted points to split line segments
                    for (int j = 0; j < points.Count - 1; j++)
                        segments[index++] = new LineD(points[j], points[j + 1]);
                }
            }

            Debug.Assert(index == count);
            return segments;
        }

        #endregion
        #region Class Status

        /// <summary>
        /// Implements the sweep line algorithm exposed by <see cref="Find"/>.</summary>
        /// <remarks>
        /// <b>Status</b> and the <see cref="EventPoint"/> structure provide all methods and data
        /// structures required by the sweep line algorithm. Each call to <see cref="Find"/> creates
        /// a new <b>Status</b> instance.</remarks>

        private class Status {
            #region Status()

            /// <summary>
            /// Initializes a new instance of the <see cref="Status"/> class.</summary>

            internal Status() {
                Crossings = new List<EventPoint>();
                Schedule = new BraidedTree<PointD, EventPoint>(PointDComparerY.CompareExact);
                SweepLine = new BraidedTree<Int32, Int32>(CompareLines);
            }

            #endregion
            #region Cursor

            /// <summary>
            /// The coordinates of the current <see cref="EventPoint"/> on the sweep line.</summary>

            private PointD Cursor;

            #endregion
            #region Crossings

            /// <summary>
            /// All input line intersections that were discovered so far.</summary>

            private readonly List<EventPoint> Crossings;

            #endregion
            #region Lines

            /// <summary>
            /// All input lines whose intersections are to be found.</summary>

            private LineD[] Lines;

            #endregion
            #region Positions

            /// <summary>
            /// The position at which the corresponding input line intersected the sweep line at the
            /// last event that was not an end point event.</summary>
            /// <remarks>
            /// Each <b>Positions</b> element holds the x-coordinate where the <see cref="Lines"/>
            /// element with the same index intersects the current sweep line.</remarks>

            private double[] Positions;

            #endregion
            #region Schedule

            /// <summary>
            /// All event points that were discovered so far but not yet processed, sorted
            /// lexicographically by increasing y- and then x-coordinates.</summary>

            private readonly BraidedTree<PointD, EventPoint> Schedule;

            #endregion
            #region SweepLine

            /// <summary>
            /// All input lines that intersect the horizontal sweep line, sorted by increasing
            /// x-coordinates of the intersection, then by increasing slopes.</summary>

            private readonly BraidedTree<Int32, Int32> SweepLine;

            #endregion
            #region Slopes

            /// <summary>
            /// The precomputed inverse slope of the corresponding input line.</summary>
            /// <remarks>
            /// Each <b>Slopes</b> element holds the precomputed <see cref="LineD.InverseSlope"/> of
            /// the <see cref="Lines"/> element with the same index.</remarks>

            private double[] Slopes;

            #endregion
            #region FindCore

            /// <summary>
            /// Finds all intersections between the specified line segments, using a sweep line
            /// algorithm.</summary>
            /// <param name="lines">
            /// An <see cref="Array"/> containing the <see cref="LineD"/> segments to intersect.
            /// </param>
            /// <returns>
            /// A lexicographically sorted <see cref="List{T}"/> containing the final <see
            /// cref="EventPoint"/> for every point of intersection between two or more <paramref
            /// name="lines"/>.</returns>
            /// <exception cref="ArgumentException">
            /// <paramref name="lines"/> contains a <see cref="LineD"/> whose <see
            /// cref="LineD.Start"/> and <see cref="LineD.End"/> coordinates are equal.</exception>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="lines"/> is a null reference.</exception>
            /// <exception cref="InvalidOperationException">
            /// <paramref name="lines"/> contains coordinates that caused corruption to an internal
            /// search structure.</exception>
            /// <remarks>
            /// <b>FindCore</b> creates the intermediate output which is further processed by one of
            /// the <see cref="Find"/> overloads.</remarks>

            internal List<EventPoint> FindCore(LineD[] lines) {
                BuildSchedule(lines);

                while (Schedule.Count > 0) {
                    var node = Schedule.RootNode._next;
                    Schedule.RemoveNode(node);
                    HandleEvent(node._value);
                }

                if (SweepLine.Count > 0)
                    ThrowHelper.ThrowInvalidOperationException(Strings.SearchStructureCorrupted);

                return Crossings;
            }

            #endregion
            #region AddCrossing

            /// <summary>
            /// Adds an intersection <see cref="EventPoint"/> to the <see cref="Schedule"/> if the
            /// two specified <see cref="SweepLine"/> nodes indicate a line crossing.</summary>
            /// <param name="a">
            /// The first <see cref="SweepLine"/> node to examine.</param>
            /// <param name="b">
            /// The second <see cref="SweepLine"/> node to examine.</param>
            /// <param name="e">
            /// The current <see cref="EventPoint"/> which receives a detected crossing that occurs
            /// exactly at the <see cref="Cursor"/>.</param>
            /// <remarks>
            /// If the <see cref="Schedule"/> already contains an <see cref="EventPoint"/> for the
            /// computed intersection, <b>AddCrossing</b> adds the indicated lines to the existing
            /// <see cref="EventPoint"/> if they are not already present.</remarks>

            private void AddCrossing(BraidedTreeNode<Int32, Int32> a,
                BraidedTreeNode<Int32, Int32> b, EventPoint e) {

                int aIndex = a.Key, bIndex = b.Key;
                LineIntersection c = Lines[aIndex].Intersect(Lines[bIndex]);

                // ignore crossings that involve only start or end points,
                // as those line events have been scheduled during initialization
                if ((c.First == LineLocation.Between && LineIntersection.Contains(c.Second)) ||
                    (LineIntersection.Contains(c.First) && c.Second == LineLocation.Between)) {

                    // quit if crossing occurs before cursor
                    PointD p = c.Shared.Value;
                    int result = PointDComparerY.CompareExact(Cursor, p);
                    if (result > 0) return;

                    // update schedule if crossing occurs after cursor
                    if (result < 0) {
                        BraidedTreeNode<PointD, EventPoint> node;
                        Schedule.TryAddNode(p, new EventPoint(p), out node);
                        e = node._value;
                    }

                    // add crossing to current or scheduled event point
                    e.TryAddLines(aIndex, c.First, bIndex, c.Second);
                }
            }

            #endregion
            #region BuildSchedule

            /// <summary>
            /// Builds the <see cref="Schedule"/> and precomputes all <see cref="Slopes"/>.
            /// </summary>
            /// <param name="lines">
            /// An <see cref="Array"/> containing the <see cref="LineD"/> segments to intersect.
            /// </param>
            /// <exception cref="ArgumentException">
            /// <paramref name="lines"/> contains a <see cref="LineD"/> whose <see
            /// cref="LineD.Start"/> and <see cref="LineD.End"/> coordinates are equal.</exception>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="lines"/> is a null reference.</exception>

            private void BuildSchedule(LineD[] lines) {
                if (lines == null)
                    ThrowHelper.ThrowArgumentNullException("lines");

                Lines = lines;
                Positions = new double[lines.Length];
                Slopes = new double[lines.Length];

                for (int i = 0; i < lines.Length; i++) {
                    LineD line = lines[i];
                    Slopes[i] = line.InverseSlope;

                    int direction = PointDComparerY.CompareExact(line.Start, line.End);
                    if (direction == 0)
                        ThrowHelper.ThrowArgumentExceptionWithFormat(
                            "lines", Strings.ArgumentContainsEmpty, "LineD");

                    // start & end point events use lexicographic ordering
                    if (direction > 0) line = line.Reverse();
                    BraidedTreeNode<PointD, EventPoint> node;

                    // add start point event for current line
                    Schedule.TryAddNode(line.Start, new EventPoint(line.Start), out node);
                    node._value.AddLine(i, LineLocation.Start);

                    // add end point event for current line
                    Schedule.TryAddNode(line.End, new EventPoint(line.End), out node);
                    node._value.AddLine(i, LineLocation.End);
                }
            }

            #endregion
            #region CompareLines

            /// <summary>
            /// Compares two specified <see cref="Lines"/> indices and returns an indication of
            /// their <see cref="SweepLine"/> ordering.</summary>
            /// <param name="a">
            /// The zero-based index of the first <see cref="Lines"/> element to compare.</param>
            /// <param name="b">
            /// The zero-based index of the second <see cref="Lines"/> element to compare.</param>
            /// <returns><list type="table"><listheader>
            /// <term>Value</term><description>Condition</description>
            /// </listheader><item>
            /// <term>Less than zero</term><description>
            /// <paramref name="a"/> is sorted before <paramref name="b"/>.</description>
            /// </item><item>
            /// <term>Zero</term><description>
            /// <paramref name="a"/> and <paramref name="b"/> are equal.</description>
            /// </item><item>
            /// <term>Greater than zero</term><description>
            /// <paramref name="a"/> is sorted after <paramref name="b"/>.</description>
            /// </item></list></returns>

            private int CompareLines(int a, int b) {
                if (a == b) return 0;

                // sort on last intersection if possible
                double ax = Positions[a], bx = Positions[b];
                if (ax < bx) return -1;
                if (ax > bx) return +1;

                // else sort on slope above sweep line
                double aSlope = Slopes[a], bSlope = Slopes[b];
                if (aSlope < bSlope) return -1;
                if (aSlope > bSlope) return +1;

                return a - b;
            }

            #endregion
            #region HandleEvent

            /// <summary>
            /// Handles the specified <see cref="EventPoint"/> that was just removed from the <see
            /// cref="Schedule"/>.</summary>
            /// <param name="e">
            /// The <see cref="EventPoint"/> to handle.</param>
            /// <remarks>
            /// <b>HandleEvent</b> always updates the <see cref="SweepLine"/>, and possibly the <see
            /// cref="Schedule"/> via <see cref="AddCrossing"/>.</remarks>

            private void HandleEvent(EventPoint e) {
                BraidedTreeNode<Int32, Int32> node, previous = null, next = null;
                Cursor = e.Shared;
                bool adding = false;

                // remove end point & crossing nodes
                for (int i = 0; i < e.Locations.Count; i++)
                    switch (e.Locations[i]) {
                        case LineLocation.Start:
                            adding = true;
                            break;

                        case LineLocation.End:
                            node = SweepLine.FindNode(e.Lines[i]);
                            if (node == null)
                                ThrowHelper.ThrowInvalidOperationException(
                                    Strings.SearchStructureCorrupted);

                            // remember surrounding lines
                            previous = node._previous;
                            next = node._next;
                            SweepLine.RemoveNode(node);
                            break;

                        case LineLocation.Between:
                            if (!SweepLine.Remove(e.Lines[i]))
                                ThrowHelper.ThrowInvalidOperationException(
                                    Strings.SearchStructureCorrupted);
                            adding = true;
                            break;
                    }

                if (!adding) {
                    // intersect remaining neighbors of removed lines
                    if (previous._parent != null && next._parent != null)
                        AddCrossing(previous, next, e);

                    // record intersection event
                    var lines = e.Lines;
                    if (lines.Count < 2) return;

                    /*
                     * The sweep line algorithm would normally record TWO intersections for
                     * overlapping lines that share the same lexicographic end point: one for the
                     * start point, and one for the end point. So when we encounter an event that
                     * contains only end points, we must check that its line segments arrive from at
                     * least two different directions, and only then record an intersection.
                     */

                    double slope = Slopes[lines[0]];
                    for (int i = 1; i < lines.Count; i++)
                        if (slope != Slopes[lines[i]]) {
                            e.Normalize(Lines);
                            Crossings.Add(e);
                            break;
                        }

                    return;
                }

                // update remaining sweep line to prepare for insertion
                var root = SweepLine.RootNode;
                for (node = root._next; node != root; node = node._next) {
                    int index = node.Key;
                    double slope = Slopes[index];
                    if (slope != Double.MaxValue) {
                        PointD start = Lines[index].Start;
                        Positions[index] = slope * (Cursor.Y - start.Y) + start.X;
                    }
                }

                // (re-)insert start point & crossing nodes
                previous = next = null;
                for (int i = 0; i < e.Locations.Count; i++)
                    if (e.Locations[i] != LineLocation.End) {

                        int index = e.Lines[i];
                        Positions[index] = Cursor.X;
                        SweepLine.TryAddNode(index, 0, out node);

                        // remember surrounding lines
                        if (previous == null) {
                            previous = node._previous;
                            next = node._next;
                        }
                    }

                // intersect outermost added lines with existing neighbors
                if (previous._parent != null) AddCrossing(previous, previous._next, e);
                if (next._parent != null) AddCrossing(next._previous, next, e);

                // record intersection event
                if (e.Lines.Count > 1) {
                    e.Normalize(Lines);
                    Crossings.Add(e);
                }
            }

            #endregion
        }

        #endregion
        #region Class EventPoint

        /// <summary>
        /// Represents an event point encountered by any <see cref="MultiLineIntersection"/>
        /// algorithm.</summary>
        /// <remarks>
        /// <b>EventPoint</b> stores the immutable <see cref="EventPoint.Shared"/> coordinates at
        /// which an event occurs, along with growing collections of all intersecting <see
        /// cref="EventPoint.Lines"/> and their relative <see cref="EventPoint.Locations"/>.
        /// </remarks>

        private class EventPoint {
            #region EventPoint(PointD)

            /// <summary>
            /// Initializes a new instance of the <see cref="EventPoint"/> structure.</summary>
            /// <param name="shared">
            /// The coordinates of the <see cref="EventPoint"/>.</param>

            internal EventPoint(PointD shared) {
                Shared = shared;
            }

            #endregion
            #region Lines

            /// <summary>
            /// The indices of all input lines that intersect at the <see cref="EventPoint"/>. The
            /// default is a null reference.</summary>
            /// <remarks>
            /// Each <b>Lines</b> element holds the index of one of the input <see
            /// cref="Status.Lines"/> that intersect at the <see cref="Shared"/> coordinates.
            /// </remarks>

            internal List<Int32> Lines;

            #endregion
            #region Locations

            /// <summary>
            /// The locations of all intersecting <see cref="Lines"/> with the same index relative
            /// to the <see cref="EventPoint"/>. The default is a null reference.</summary>
            /// <remarks>
            /// Each <b>Locations</b> element holds the <see cref="LineLocation"/> value for the
            /// <see cref="Lines"/> element with the same index, relative to the <see
            /// cref="Shared"/> coordinates.</remarks>

            internal List<LineLocation> Locations;

            #endregion
            #region Shared

            /// <summary>
            /// The coordinates of the <see cref="EventPoint"/>.</summary>

            internal readonly PointD Shared;

            #endregion
            #region AddLine

            /// <summary>
            /// Adds the specified line event to the <see cref="EventPoint"/>.</summary>
            /// <param name="line">
            /// The element to add to the <see cref="Lines"/> collection.</param>
            /// <param name="location">
            /// The element to add to the <see cref="Locations"/> collection.</param>
            /// <remarks>
            /// <b>AddLine</b> does not check whether the <see cref="Lines"/> collection already
            /// contains the specified <paramref name="line"/>. The <see cref="Lines"/> and <see
            /// cref="Locations"/> collections are created if necessary.</remarks>

            internal void AddLine(int line, LineLocation location) {

                if (Lines == null) {
                    Lines = new List<Int32>(2);
                    Locations = new List<LineLocation>(2);
                }

                Lines.Add(line);
                Locations.Add(location);
            }

            #endregion
            #region Convert

            /// <summary>
            /// Converts the specified <see cref="ICollection{T}"/> of <see cref="EventPoint"/>
            /// instances into an <see cref="Array"/> of <see cref="MultiLinePoint"/> instances.
            /// </summary>
            /// <param name="crossings">
            /// The <see cref="ICollection{T}"/> of <see cref="EventPoint"/> instances to convert.
            /// </param>
            /// <returns>
            /// An <see cref="Array"/> of <see cref="MultiLinePoint"/> instances that contains the
            /// data from all elements found in <paramref name="crossings"/>, in the same order.
            /// </returns>

            internal static MultiLinePoint[] Convert(ICollection<EventPoint> crossings) {
                var output = new MultiLinePoint[crossings.Count];

                int i = 0;
                foreach (var e in crossings) {
                    var lines = e.Lines.ToArray();
                    var locations = e.Locations.ToArray();
                    output[i++] = new MultiLinePoint(e.Shared, lines, locations);
                }

                return output;
            }

            #endregion
            #region Normalize

            /// <summary>
            /// Normalizes all <see cref="Locations"/> to match the corresponding <see
            /// cref="Lines"/>.</summary>
            /// <param name="lines">
            /// An <see cref="Array"/> containing all input <see cref="LineD"/> segments.</param>
            /// <remarks><para>
            /// <b>Normalize</b> inverts any <see cref="LineLocation.Start"/> or <see
            /// cref="LineLocation.End"/> values in the <see cref="Locations"/> collection whose
            /// corresponding <see cref="Lines"/> element indicates a <see cref="LineD"/> whose <see
            /// cref="LineD.Start"/> and <see cref="LineD.End"/> points have the opposite orientation.
            /// </para><para>
            /// Therefore, the <see cref="Locations"/> collection no longer reflects the sweep line
            /// direction, but rather the point at which the corresponding <see cref="LineD"/>
            /// touches the <see cref="Shared"/> coordinates. Call this method to prepare for output
            /// generation, after the <see cref="EventPoint"/> has been removed from the schedule of
            /// the sweep line algorithm.</para></remarks>

            internal void Normalize(LineD[] lines) {

                for (int i = 0; i < Locations.Count; i++) {
                    LineLocation location = Locations[i];
                    const LineLocation startEnd = LineLocation.Start | LineLocation.End;

                    // check for start & end point events
                    if ((location & startEnd) != 0) {
                        LineD line = lines[Lines[i]];

                        // report orientation of inverted line
                        if (PointDComparerY.CompareExact(line.Start, line.End) > 0) {
                            location ^= startEnd;
                            Locations[i] = location;
                        }
                    }
                }
            }

            #endregion
            #region TryAddLines

            /// <summary>
            /// Adds the specified line events to the <see cref="EventPoint"/>, unless <see
            /// cref="Lines"/> already contains events for the same line.</summary>
            /// <param name="line1">
            /// The first element to add to the <see cref="Lines"/> collection.</param>
            /// <param name="location1">
            /// The first element to add to the <see cref="Locations"/> collection.</param>
            /// <param name="line2">
            /// The second element to add to the <see cref="Lines"/> collection.</param>
            /// <param name="location2">
            /// The second element to add to the <see cref="Locations"/> collection.</param>
            /// <remarks>
            /// <b>TryAddLines</b> skips either or both argument pairs if the <see cref="Lines"/>
            /// collection already contains the corresponding element. The <see cref="Lines"/> and
            /// <see cref="Locations"/> collections are created if necessary.</remarks>

            internal void TryAddLines(int line1, LineLocation location1,
                int line2, LineLocation location2) {

                if (Lines == null) {
                    Lines = new List<Int32>(2) { line1, line2 };
                    Locations = new List<LineLocation>(2) { location1, location2 };
                    return;
                }

                if (!Lines.Contains(line1)) {
                    Lines.Add(line1);
                    Locations.Add(location1);
                }

                if (!Lines.Contains(line2)) {
                    Lines.Add(line2);
                    Locations.Add(location2);
                }
            }

            #endregion
        }

        #endregion
    }
}
