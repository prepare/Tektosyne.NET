using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Provides standard algorithms and auxiliary methods for computational geometry.</summary>
    /// <remarks>
    /// All <b>Random…</b> methods use the <see cref="MersenneTwister.Default"/> instance of the
    /// <see cref="MersenneTwister"/> class, and are therefore <em>not</em> thread-safe.</remarks>

    public static class GeoAlgorithms {
        #region ConnectPoints

        /// <summary>
        /// Connects the specified <see cref="PointD"/> coordinates with <see cref="LineD"/>
        /// instances.</summary>
        /// <param name="isClosed">
        /// <c>true</c> to create a <see cref="LineD"/> instance from the last to the first <see
        /// cref="PointD"/> coordinates; otherwise, <c>false</c>.</param>
        /// <param name="points">
        /// An <see cref="Array"/> containing the <see cref="PointD"/> coordinates the connect.
        /// </param>
        /// <returns>
        /// An <see cref="Array"/> containing the <see cref="LineD"/> instances that connect all
        /// <paramref name="points"/> in the specified order.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="points"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>ConnectPoints</b> returns an empty <see cref="Array"/> if <paramref name="points"/>
        /// contains less than two elements. Otherwise, <b>ConnectPoints</b> returns an <see
        /// cref="Array"/> with the same number of elements as <paramref name="points"/> if
        /// <paramref name="isClosed"/> is <c>true</c>, and one element less if <paramref
        /// name="isClosed"/> is <c>false</c>.
        /// </para><para>
        /// <b>ConnectPoints</b> does not check for identical adjacent <paramref name="points"/>, or
        /// for congruent <see cref="LineD"/> instances. For example, if <paramref name="points"/>
        /// contains two elements and <paramref name="isClosed"/> is <c>true</c>, the returned <see
        /// cref="Array"/> will contain two <see cref="LineD"/> instances with identical coordinates
        /// but opposite directions.</para></remarks>

        public static LineD[] ConnectPoints(bool isClosed, params PointD[] points) {
            if (points == null)
                ThrowHelper.ThrowArgumentNullException("points");

            if (points.Length < 2)
                return new LineD[0];

            LineD[] lines = new LineD[isClosed ? points.Length : points.Length - 1];
            
            for (int i = 0; i < points.Length - 1; i++)
                lines[i] = new LineD(points[i], points[i + 1]);

            if (isClosed)
                lines[lines.Length - 1] = new LineD(points[points.Length - 1], points[0]);

            return lines;
        }

        #endregion
        #region ConvexHull

        /// <summary>
        /// Finds the convex hull for the specified set of <see cref="PointD"/> coordinates.
        /// </summary>
        /// <param name="points">
        /// An <see cref="Array"/> containing the <see cref="PointD"/> coordinates whose convex hull
        /// to find.</param>
        /// <returns>
        /// An <see cref="Array"/> containing the subset of the specified <paramref name="points"/>
        /// that represent the vertices of their convex hull.</returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="points"/> is a null reference or an empty array.</exception>
        /// <remarks><para>
        /// If the specified <paramref name="points"/> array contains only one or two elements,
        /// <b>ConvexHull</b> returns a new array containing the same elements. Points that are
        /// coincident or collinear with other hull vertices are always removed from the returned
        /// array, however. A <paramref name="points"/> array containing the same <see
        /// cref="PointD"/> twice will return an array containing that <see cref="PointD"/> once.
        /// </para><para>
        /// <b>ConvexHull</b> performs a Graham scan with an asymptotic runtime of O(n log n). This
        /// C# implementation was adapted from the <c>Graham</c> algorithm by Joseph O’Rourke,
        /// <em>Computational Geometry in C</em> (2nd ed.), Cambridge University Press 1998, p.72ff.
        /// </para></remarks>

        public static PointD[] ConvexHull(params PointD[] points) {
            if (points == null || points.Length == 0)
                ThrowHelper.ThrowArgumentNullOrEmptyException("points");

            // handle trivial edge cases
            switch (points.Length) {
                case 1: return new PointD[] { points[0] };

                case 2:
                    if (points[0] == points[1])
                        goto case 1;
                    else
                        return new PointD[] { points[0], points[1] };
            }

            /*
             * Set index n to lowest vertex. Unlike O’Rourke, we immediately mark duplicates
             * of the current lowest vertex for deletion. This eliminates some corner cases
             * of multiple duplicates that are missed by ConvexHullVertexComparer.Compare.
             */

            var p = new ConvexHullVertex[points.Length];
            PointD pnv = points[0];
            p[0] = new ConvexHullVertex(pnv, 0);

            int i, n = 0;
            for (i = 1; i < p.Length; i++) {
                PointD piv = points[i];
                p[i] = new ConvexHullVertex(piv, i);

                int result = PointDComparerY.CompareExact(piv, pnv);
                if (result < 0) {
                    n = i; pnv = piv;
                } else if (result == 0)
                    p[i].Delete = true;
            }

            // move lowest vertex to index 0
            if (n > 0) {
                var swap = p[0]; p[0] = p[n]; p[n] = swap;
            }

            // sort and mark collinear/coincident vertices for deletion
            var comparer = new ConvexHullVertexComparer(p[0]);
            Array.Sort(p, 1, p.Length - 1, comparer);

            // delete marked vertices (n is remaining count)
            for (i = 0, n = 0; i < p.Length; i++)
                if (!p[i].Delete) p[n++] = p[i];

            // quit if only one unique vertex remains
            if (n == 1) return new[] { p[0].Vertex };

            // begin stack of convex hull vertices
            var top = p[1]; top.Next = p[0];
            int hullCount = 2;

            // first two vertices are permanent, now examine others
            for (i = 2; i < n; ) {
                ConvexHullVertex pi = p[i];

                if (top.Next.Vertex.CrossProductLength(top.Vertex, pi.Vertex) > 0) {
                    // push p[i] on stack
                    pi.Next = top;
                    top = pi; ++i;
                    ++hullCount;
                } else {
                    // pop top from stack
                    top = top.Next;
                    --hullCount;
                }
            }

            // convert vertex stack to point array
            PointD[] hull = new PointD[hullCount];
            for (i = 0; i < hull.Length; i++) {
                hull[i] = top.Vertex;
                top = top.Next;
            }
            Debug.Assert(top == null);

            return hull;
        }

        #endregion
        #region NearestPoint

        /// <summary>
        /// Searches the specified <see cref="PointD"/> collection for the element nearest to the
        /// specified coordinates.</summary>
        /// <param name="points">
        /// An <see cref="IList{T}"/> containing the <see cref="PointD"/> coordinates to search.
        /// </param>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to locate.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="q"/> in <paramref
        /// name="points"/>, if found; otherwise, the zero-based index of the <paramref
        /// name="points"/> element with the smallest Euclidean distance to <paramref name="q"/>.
        /// </returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="points"/> is a null reference or an empty collection.</exception>
        /// <remarks><para>
        /// <b>NearestPoint</b> performs a linear search within the specified <paramref
        /// name="points"/> to find the element with the smallest Euclidean distance to <paramref
        /// name="q"/>. This is always an O(n) operation, where n is the total number of <paramref
        /// name="points"/>, unless an exact match for <paramref name="q"/> is encountered.
        /// </para><para>
        /// If the specified <paramref name="points"/> are already sorted lexicographically, the
        /// <see cref="PointDComparerX"/> and <see cref="PointDComparerY"/> classes provide a much
        /// faster <b>FindNearest</b> method.</para></remarks>

        public static int NearestPoint(IList<PointD> points, PointD q) {
            if (points == null || points.Count == 0)
                ThrowHelper.ThrowArgumentNullOrEmptyException("points");

            PointD vector = q - points[0];
            double minDistance = vector.LengthSquared;
            if (minDistance == 0) return 0;
            int minIndex = 0;

            for (int i = 1; i < points.Count; i++) {
                vector = q - points[i];
                double distance = vector.LengthSquared;

                if (minDistance > distance) {
                    if (distance == 0) return i;
                    minDistance = distance;
                    minIndex = i;
                }
            }

            return minIndex;
        }

        #endregion
        #region PointInPolygon(PointD, PointD[])

        /// <overloads>
        /// Finds the location of the specified <see cref="PointD"/> relative to the specified
        /// arbitrary polygon.</overloads>
        /// <summary>
        /// Finds the location of the specified <see cref="PointD"/> relative to the specified
        /// arbitrary polygon, using exact coordinate comparisons.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to locate.</param>
        /// <param name="polygon">
        /// An <see cref="Array"/> containing <see cref="PointD"/> coordinates that are the vertices
        /// of an arbitrary polygon.</param>
        /// <returns>
        /// A <see cref="PolygonLocation"/> value that indicates the location of <paramref
        /// name="q"/> relative to the specified <paramref name="polygon"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="polygon"/> contains less than three elements.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="polygon"/> is a null reference.</exception>
        /// <remarks><para>
        /// The specified <paramref name="polygon"/> is implicitly assumed to be closed, with an
        /// edge connecting its first and last vertex. Therefore, all vertices should be different.
        /// </para><para>
        /// <b>PointInPolygon</b> performs a ray crossings algorithm with an asymptotic runtime of
        /// O(n). This C# implementation was adapted from the <c>InPoly1</c> algorithm by Joseph
        /// O’Rourke, <em>Computational Geometry in C</em> (2nd ed.), Cambridge University Press
        /// 1998, p.244.</para></remarks>

        public static PolygonLocation PointInPolygon(PointD q, PointD[] polygon) {
            if (polygon == null)
                ThrowHelper.ThrowArgumentNullException("polygon");
            if (polygon.Length < 3)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "polygon.Length", Strings.ArgumentLessValue, 3);

            // number of right & left crossings of edge & ray
            int rightCrossings = 0, leftCrossings = 0;

            // last vertex is starting point for first edge
            int lastIndex = polygon.Length - 1;
            double x1 = polygon[lastIndex].X - q.X, y1 = polygon[lastIndex].Y - q.Y;

            for (int i = 0; i < polygon.Length; i++) {
                double x0 = polygon[i].X - q.X, y0 = polygon[i].Y - q.Y;

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
            }

            // q is on edge if crossings are of different parity
            if (rightCrossings % 2 != leftCrossings % 2)
                return PolygonLocation.Edge;

            // q is inside for an odd number of crossings, else outside
            return (rightCrossings % 2 == 1 ?
                PolygonLocation.Inside : PolygonLocation.Outside);
        }

        #endregion
        #region PointInPolygon(PointD, PointD[], Double)

        /// <summary>
        /// Finds the location of the specified <see cref="PointD"/> relative to the specified
        /// arbitrary polygon, given the specified epsilon for coordinate comparisons.</summary>
        /// <param name="q">
        /// The <see cref="PointD"/> coordinates to locate.</param>
        /// <param name="polygon">
        /// An <see cref="Array"/> containing <see cref="PointD"/> coordinates that are the vertices
        /// of an arbitrary polygon.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which two coordinates should be considered equal.
        /// </param>
        /// <returns>
        /// A <see cref="PolygonLocation"/> value that indicates the location of <paramref
        /// name="q"/> relative to the specified <paramref name="polygon"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="polygon"/> contains less than three elements.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="polygon"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="epsilon"/> is equal to or less than zero.</exception>
        /// <remarks>
        /// <b>PointInPolygon</b> is identical with the basic <see cref="PointInPolygon(PointD,
        /// PointD[])"/> overload but calls <see cref="MathUtility.Compare"/> with the specified
        /// <paramref name="epsilon"/> to determine whether <paramref name="q"/> coincides with any
        /// edge or vertex of the specified <paramref name="polygon"/>.</remarks>

        public static PolygonLocation PointInPolygon(PointD q, PointD[] polygon, double epsilon) {
            if (polygon == null)
                ThrowHelper.ThrowArgumentNullException("polygon");
            if (polygon.Length < 3)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "polygon.Length", Strings.ArgumentLessValue, 3);

            if (epsilon <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "epsilon", epsilon, Strings.ArgumentNotPositive);

            // number of right & left crossings of edge & ray
            int rightCrossings = 0, leftCrossings = 0;

            // last vertex is starting point for first edge
            int lastIndex = polygon.Length - 1;
            double x1 = polygon[lastIndex].X - q.X, y1 = polygon[lastIndex].Y - q.Y;
            int dy1 = MathUtility.Compare(y1, 0, epsilon);

            for (int i = 0; i < polygon.Length; i++) {
                double x0 = polygon[i].X - q.X, y0 = polygon[i].Y - q.Y;

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
            }

            // q is on edge if crossings are of different parity
            if (rightCrossings % 2 != leftCrossings % 2)
                return PolygonLocation.Edge;

            // q is inside for an odd number of crossings, else outside
            return (rightCrossings % 2 == 1 ?
                PolygonLocation.Inside : PolygonLocation.Outside);
        }

        #endregion
        #region PolygonArea

        /// <summary>
        /// Computes the area of the specified polygon.</summary>
        /// <param name="polygon">
        /// An <see cref="Array"/> containing <see cref="PointD"/> coordinates that are the vertices
        /// of an arbitrary polygon.</param>
        /// <returns>
        /// The area of the specified <paramref name="polygon"/>, with a sign that indicates the
        /// orientation of its vertices.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="polygon"/> contains less than three elements.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="polygon"/> is a null reference.</exception>
        /// <remarks><para>
        /// The specified <paramref name="polygon"/> is implicitly assumed to be closed, with an
        /// edge connecting its first and last vertex. Therefore, all vertices should be different.
        /// Moreover, <paramref name="polygon"/> must not self-intersect.
        /// </para><para>
        /// The absolute value of <b>PolygonArea</b> equals the area of the specified <paramref
        /// name="polygon"/>. The sign indicates the orientation of its vertices, as follows:
        /// </para><list type="table"><listheader>
        /// <term>Return Value</term><description>Relationship</description>
        /// </listheader><item>
        /// <term>Less than zero</term><description>
        /// The vertices are specified in clockwise order, assuming y-coordinates increase upward.
        /// </description></item><item>
        /// <term>Zero</term><description>
        /// All vertices are collinear, or otherwise enclose no area.
        /// </description></item><item>
        /// <term>Greater than zero</term><description>
        /// The vertices are specified in counter-clockwise order, assuming y-coordinates increase
        /// upward.</description></item></list></remarks>

        public static double PolygonArea(params PointD[] polygon) {
            if (polygon == null)
                ThrowHelper.ThrowArgumentNullException("polygon");
            if (polygon.Length < 3)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "polygon.Length", Strings.ArgumentLessValue, 3);

            double area = 0;

            for (int i = polygon.Length - 1, j = 0; j < polygon.Length; i = j++)
                area += (polygon[i].X * polygon[j].Y - polygon[j].X * polygon[i].Y);

            return area / 2.0;
        }

        #endregion
        #region PolygonCentroid

        /// <summary>
        /// Computes the centroid of the specified polygon.</summary>
        /// <param name="polygon">
        /// An <see cref="Array"/> containing <see cref="PointD"/> coordinates that are the vertices
        /// of an arbitrary polygon.</param>
        /// <returns>
        /// The centroid (center of gravity) of the specified <paramref name="polygon"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="polygon"/> contains less than three elements.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="polygon"/> is a null reference.</exception>
        /// <remarks><para>
        /// The specified <paramref name="polygon"/> is implicitly assumed to be closed, with an
        /// edge connecting its first and last vertex. Therefore, all vertices should be different.
        /// </para><para>
        /// Moreover, <paramref name="polygon"/> must not self-intersect, and its vertices cannot be
        /// collinear, i.e. <see cref="PolygonArea"/> cannot be zero.</para></remarks>

        public static PointD PolygonCentroid(params PointD[] polygon) {
            if (polygon == null)
                ThrowHelper.ThrowArgumentNullException("polygon");
            if (polygon.Length < 3)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "polygon.Length", Strings.ArgumentLessValue, 3);

            double area = 0, x = 0, y = 0;

            for (int i = polygon.Length - 1, j = 0; j < polygon.Length; i = j++) {
                double factor = polygon[i].X * polygon[j].Y - polygon[j].X * polygon[i].Y;

                area += factor;
                x += (polygon[i].X + polygon[j].X) * factor;
                y += (polygon[i].Y + polygon[j].Y) * factor;
            }

            area *= 3.0;
            return new PointD(x / area, y / area);
        }

        #endregion
        #region RandomLine

        /// <summary>
        /// Creates a random <see cref="LineD"/> within the specified area.</summary>
        /// <param name="x">
        /// The smallest x-coordinate for any <see cref="LineD"/> coordinate.</param>
        /// <param name="y">
        /// The smallest y-coordinate for any <see cref="LineD"/> coordinate.</param>
        /// <param name="width">
        /// The width of the area containing all <see cref="LineD"/> coordinates.</param>
        /// <param name="height">
        /// The height of the area containing all <see cref="LineD"/> coordinates.</param>
        /// <returns>
        /// A randomly created <see cref="LineD"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is equal to or less than zero.
        /// </exception>

        public static LineD RandomLine(double x, double y, double width, double height) {

            if (width <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "width", width, Strings.ArgumentNotPositive);
            if (height <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "height", height, Strings.ArgumentNotPositive);

            return new LineD(
                x + MersenneTwister.Default.NextDouble() * width,
                y + MersenneTwister.Default.NextDouble() * height,
                x + MersenneTwister.Default.NextDouble() * width,
                y + MersenneTwister.Default.NextDouble() * height);
        }

        #endregion
        #region RandomPoint

        /// <summary>
        /// Creates a random <see cref="PointD"/> within the specified area.</summary>
        /// <param name="x">
        /// The smallest x-coordinate for any <see cref="PointD"/>.</param>
        /// <param name="y">
        /// The smallest y-coordinate for any <see cref="PointD"/>.</param>
        /// <param name="width">
        /// The width of the area containing all <see cref="PointD"/> coordinates.</param>
        /// <param name="height">
        /// The height of the area containing all <see cref="PointD"/> coordinates.</param>
        /// <returns>
        /// A randomly created <see cref="PointD"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is equal to or less than zero.
        /// </exception>

        public static PointD RandomPoint(double x, double y, double width, double height) {

            if (width <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "width", width, Strings.ArgumentNotPositive);
            if (height <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "height", height, Strings.ArgumentNotPositive);

            return new PointD(
                x + MersenneTwister.Default.NextDouble() * width,
                y + MersenneTwister.Default.NextDouble() * height);
        }

        #endregion
        #region RandomPoints(Int32, RectD)

        /// <overloads>
        /// Creates an <see cref="Array"/> of random <see cref="PointD"/> coordinates within the
        /// specified area.</overloads>
        /// <summary>
        /// Creates an <see cref="Array"/> of random <see cref="PointD"/> coordinates within the
        /// specified area.</summary>
        /// <param name="count">
        /// The number of <see cref="PointD"/> coordinates to create.</param>
        /// <param name="bounds">
        /// The coordinates of the area containing all <see cref="PointD"/> coordinates.</param>
        /// <returns>
        /// An <see cref="Array"/> containing <paramref name="count"/> randomly created <see
        /// cref="PointD"/> coordinates.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="count"/> is less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="bounds"/> contains a <see cref="RectD.Width"/> or <see
        /// cref="RectD.Height"/> that is equal to or less than zero.</para></exception>
        /// <remarks>
        /// The returned <see cref="Array"/> is unsorted and may contain duplicate <see
        /// cref="PointD"/> coordinates.</remarks>

        public static PointD[] RandomPoints(int count, RectD bounds) {

            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "count", count, Strings.ArgumentNegative);
            if (bounds.Width <= 0 || bounds.Height <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "bounds", bounds, Strings.ArgumentCoordinatesInvalid);

            PointD[] points = new PointD[count];

            for (int i = 0; i < points.Length; i++)
                points[i] = new PointD(
                    bounds.X + MersenneTwister.Default.NextDouble() * bounds.Width,
                    bounds.Y + MersenneTwister.Default.NextDouble() * bounds.Height);

            return points;
        }

        #endregion
        #region RandomPoints(Int32, RectD, IPointDComparer, Double)

        /// <summary>
        /// Creates an <see cref="Array"/> of random <see cref="PointD"/> coordinates within the
        /// specified area, ensuring a specified pairwise minimum distance.</summary>
        /// <param name="count">
        /// The number of <see cref="PointD"/> coordinates to create.</param>
        /// <param name="bounds">
        /// The coordinates of the area containing all <see cref="PointD"/> coordinates.</param>
        /// <param name="comparer">
        /// The <see cref="IPointDComparer"/> instance used to sort and search the collection of
        /// <see cref="PointD"/> coordinates.</param>
        /// <param name="distance">
        /// The smallest Euclidean distance between any two <see cref="PointD"/> coordinates.
        /// </param>
        /// <returns>
        /// An <see cref="Array"/> containing <paramref name="count"/> randomly created <see
        /// cref="PointD"/> coordinates whose pairwise distance is equal to or greater than
        /// <paramref name="distance"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="comparer"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="count"/> is less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="bounds"/> contains a <see cref="RectD.Width"/> or <see
        /// cref="RectD.Height"/> that is equal to or less than zero.
        /// </para><para>-or-</para><para>
        /// <paramref name="distance"/> is equal to or less than zero.</para></exception>
        /// <remarks><para>
        /// The returned <see cref="Array"/> is sorted using the specified <paramref
        /// name="comparer"/>, and never contains duplicate <see cref="PointD"/> coordinates.
        /// </para><note type="caution">
        /// <b>RandomPoints</b> may enter an endless loop if <paramref name="distance"/> is too
        /// great relative to <paramref name="count"/> and <paramref name="bounds"/>.
        /// </note></remarks>

        public static PointD[] RandomPoints(int count,
            RectD bounds, IPointDComparer comparer, double distance) {

            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "count", count, Strings.ArgumentNegative);
            if (bounds.Width <= 0 || bounds.Height <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "bounds", bounds, Strings.ArgumentCoordinatesInvalid);

            if (comparer == null)
                ThrowHelper.ThrowArgumentNullException("comparer");
            if (distance <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "distance", distance, Strings.ArgumentNotPositive);

            distance *= distance;
            List<PointD> points = new List<PointD>(count);

            for (int i = 0; i < count; i++) {
            createPoint:
                    PointD point = new PointD(
                        bounds.X + MersenneTwister.Default.NextDouble() * bounds.Width,
                        bounds.Y + MersenneTwister.Default.NextDouble() * bounds.Height);

                    if (points.Count > 0) {
                        int index = comparer.FindNearest(points, point);
                        if (point.Subtract(points[index]).LengthSquared < distance)
                            goto createPoint;
                    }

                    points.Add(point);
                    points.Sort(comparer);
            }

            return points.ToArray();
        }

        #endregion
        #region RandomPolygon

        /// <summary>
        /// Creates a random simple polygon within the specified area.</summary>
        /// <param name="x">
        /// The smallest x-coordinate for any polygon vertex.</param>
        /// <param name="y">
        /// The smallest y-coordinate for any polygon vertex.</param>
        /// <param name="width">
        /// The width of the area containing all polygon vertices.</param>
        /// <param name="height">
        /// The height of the area containing all polygon vertices.</param>
        /// <returns>
        /// An <see cref="Array"/> containing the <see cref="PointD"/> coordinates that are the
        /// vertices of the created polygon.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is equal to or less than zero.
        /// </exception>
        /// <remarks>
        /// <b>RandomPolygon</b> moves in a full circle around the center of the specified area,
        /// placing vertices at random angles and radii within the area. Any two vertices are
        /// separated by a minimum angular distance of 6 degrees. The resulting polygon is simple,
        /// i.e. covering a single contiguous space without self-intersections.</remarks>

        public static PointD[] RandomPolygon(double x, double y, double width, double height) {

            if (width <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "width", width, Strings.ArgumentNotPositive);
            if (height <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "height", height, Strings.ArgumentNotPositive);

            // drawing range extending from center of drawing area
            SizeD range = new SizeD(width / 2.0, height / 2.0);
            PointD center = new PointD(x + range.Width, y + range.Height);

            // radius of circle circumscribed around drawing area
            double radius = Math.Sqrt(range.Width * range.Width + range.Height * range.Height);

            // create vertices as series of random polar coordinates
            var polygon = new List<PointD>();
            for (double degrees = 0; degrees < 360; degrees += 6.0) {

                // random increment of angle for next vertex
                degrees += MersenneTwister.Default.NextDouble() * 110;
                if (degrees >= 360) break;
                double radians = degrees * Angle.DegreesToRadians;

                // axis projections of circumscribed radius at current angle
                double dx = Math.Cos(radians) * radius;
                double dy = Math.Sin(radians) * radius;

                // shorten total radius where it extends beyond area
                double dxLimit = range.Width / Math.Abs(dx);
                double dyLimit = range.Height / Math.Abs(dy);
                double factor = Math.Min(dxLimit, dyLimit);

                // additional random shortening to determine vertex
                factor *= MersenneTwister.Default.NextDouble();
                polygon.Add(new PointD(center.X + dx * factor, center.Y + dy * factor));
            }

            return polygon.ToArray();
        }

        #endregion
        #region Class ConvexHullVertex

        /// <summary>
        /// Represents a potential vertex of a <see cref="ConvexHull"/> under construction.
        /// </summary>
        /// <remarks>
        /// <b>ConvexHullVertex</b> implements the <c>tPointStructure</c> and <c>tStackCell</c>
        /// structures by Joseph O’Rourke, <em>Computational Geometry in C</em> (2nd ed.), Cambridge
        /// University Press 1998, p.78.</remarks>

        private class ConvexHullVertex {
            #region ConvexHullVertex(PointD, Int32)

            /// <summary>
            /// Initializes a new instance of the <see cref="ConvexHullVertex"/> class with the
            /// specified <see cref="Vertex"/> and <see cref="Index"/>.</summary>
            /// <param name="vertex">
            /// The coordinates of the <see cref="ConvexHullVertex"/>.</param>
            /// <param name="index">
            /// The unique index of the <see cref="ConvexHullVertex"/>.</param>

            internal ConvexHullVertex(PointD vertex, int index) {
                Vertex = vertex;
                Index = index;
            }

            #endregion
            #region Delete

            /// <summary>
            /// <c>true</c> if the <see cref="ConvexHullVertex"/> is not part of the convex hull and
            /// should be deleted; otherwise, <c>false</c>. The default is <c>false</c>.</summary>

            internal bool Delete;

            #endregion
            #region Index

            /// <summary>
            /// An <see cref="Int32"/> value that uniquely identifies the <see
            /// cref="ConvexHullVertex"/> within the convex hull.</summary>

            internal readonly int Index;

            #endregion
            #region Next

            /// <summary>
            /// The next <see cref="ConvexHullVertex"/> in the convex hull. The default is a null
            /// reference.</summary>
            /// <remarks>
            /// <b>Next</b> is a null reference if the convex hull ends with this <see
            /// cref="ConvexHullVertex"/>, or if the vertices have not yet been linked.</remarks>

            internal ConvexHullVertex Next;

            #endregion
            #region Vertex

            /// <summary>
            /// The coordinates of the <see cref="ConvexHullVertex"/>.</summary>

            internal readonly PointD Vertex;

            #endregion
        }

        #endregion
        #region Class ConvexHullVertexComparer

        /// <summary>
        /// Provides a method that compares two <see cref="ConvexHullVertex"/> instances.</summary>
        /// <remarks>
        /// <b>ConvexHullVertexComparer</b> implements the <c>Compare</c> algorithm by Joseph
        /// O’Rourke, <em>Computational Geometry in C</em> (2nd ed.), Cambridge University Press
        /// 1998, p.82.</remarks>

        private class ConvexHullVertexComparer: IComparer<ConvexHullVertex> {
            #region ConvexHullVertexComparer(ConvexHullVertex)

            /// <summary>
            /// Initializes a new instance of the <see cref="ConvexHullVertexComparer"/> class with
            /// the specified initial vertex.</summary>
            /// <param name="p0">
            /// The <see cref="ConvexHullVertex"/> object that represents the first vertex in the
            /// convex hull under construction.</param>

            internal ConvexHullVertexComparer(ConvexHullVertex p0) {
                P0 = p0.Vertex;
            }

            #endregion
            #region P0

            /// <summary>
            /// The first <see cref="ConvexHullVertex.Vertex"/> in the convex hull under
            /// construction.</summary>

            private readonly PointD P0;

            #endregion
            #region Compare

            /// <summary>
            /// Compares two specified <see cref="ConvexHullVertex"/> objects and returns an
            /// indication of their precedence in the convex hull under construction.</summary>
            /// <param name="pi">
            /// The first <see cref="ConvexHullVertex"/> to compare.</param>
            /// <param name="pj">
            /// The second <see cref="ConvexHullVertex"/> to compare.</param>
            /// <returns><list type="table"><listheader>
            /// <term>Value</term><description>Condition</description>
            /// </listheader><item>
            /// <term>Less than zero</term><description>
            /// <paramref name="pi"/> is sorted before <paramref name="pj"/>.</description>
            /// </item><item>
            /// <term>Zero</term><description>
            /// <paramref name="pi"/> and <paramref name="pj"/> are the same object or contain the
            /// same <see cref="ConvexHullVertex.Vertex"/> coordinates.</description>
            /// </item><item>
            /// <term>Greater than zero</term><description>
            /// <paramref name="pi"/> is sorted after <paramref name="pj"/>.</description>
            /// </item></list></returns>
            /// <remarks><para>
            /// <b>Compare</b> sets the <see cref="ConvexHullVertex.Delete"/> flag on either the
            /// specified <paramref name="pi"/> or <paramref name="pj"/> if the two vertices are
            /// collinear or coincident.
            /// </para><para>
            /// <b>Compare</b> implements the <c>Compare</c> algorithm by Joseph O’Rourke,
            /// <em>Computational Geometry in C</em> (2nd ed.), Cambridge University Press 1998,
            /// p.82. See there for an explanation of the established sorting order.
            /// </para><para>
            /// Our implementation also compares <paramref name="pi"/> and <paramref name="pj"/> for
            /// reference equality before doing anything else. This is necessary because sorting
            /// algorithms may supply the same object for both parameters. </para></remarks>

            public int Compare(ConvexHullVertex pi, ConvexHullVertex pj) {
                if (pi == pj) return 0;
                PointD piv = pi.Vertex, pjv = pj.Vertex;

                // check if coordinate triplet constitutes a turn
                double length = P0.CrossProductLength(piv, pjv);
                if (length > 0) return -1;
                else if (length < 0) return +1;

                // pi and pj are collinear with p0, delete one of them
                double x = Math.Abs(piv.X - P0.X) - Math.Abs(pjv.X - P0.X);
                double y = Math.Abs(piv.Y - P0.Y) - Math.Abs(pjv.Y - P0.Y);

                if (x < 0 || y < 0) {
                    pi.Delete = true;
                    return -1;
                }
                if (x > 0 || y > 0) {
                    pj.Delete = true;
                    return +1;
                }

                // pi and pj are coincident
                if (pi.Index > pj.Index)
                    pj.Delete = true;
                else
                    pi.Delete = true;

                return 0;
            }

            #endregion
        }

        #endregion
    }
}
