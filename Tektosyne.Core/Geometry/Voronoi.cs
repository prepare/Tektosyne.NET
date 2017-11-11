using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Provides a sweep line algorithm for Voronoi diagrams and Delaunay triangulations.</summary>
    /// <remarks><para>
    /// Call <see cref="Voronoi.FindAll"/> to find both the Voronoi diagram and the Delaunay
    /// triangulation for a given <see cref="PointD"/> set, or <see cref="Voronoi.FindDelaunay"/> to
    /// find only the Delaunay triangulation.
    /// </para><para>
    /// Since the outgoing edges of a Voronoi diagram continue indefinitely, <b>Voronoi</b> employs
    /// a clipping rectangle slightly larger than the bounding box of the specified point set.
    /// Voronoi edges that cross the clipping rectangle are terminated with a pseudo-vertex at the
    /// point of intersection. True Voronoi vertices beyond the clipping rectangle are not found.
    /// You may specify a larger clipping rectangle if desired.
    /// </para><para>
    /// <b>Voronoi</b> performs Fortune’s sweep line algorithm with an asymptotic runtime of O(n log
    /// n). This algorithm was first published in Steven J. Fortune, <em>A Sweepline Algorithm for
    /// Voronoi Diagrams,</em> Algorithmica 2 (1987), p.153-174. This C# implementation was adapted
    /// from Fortune’s own C implementation, available as <c>sweep2.gz</c> at the <a
    /// href="http://netlib.sandia.gov/voronoi/index.html">netlib/voronoi</a> archive page of Sandia
    /// National Laboratories. The following copyright statement is reproduced from the original C
    /// program, as required by the copyright conditions.
    /// </para><para>
    /// The author of this software is Steven Fortune.  Copyright (c) 1994 by AT&amp;T Bell
    /// Laboratories.
    /// </para><para>
    /// Permission to use, copy, modify, and distribute this software for any purpose without fee is
    /// hereby granted, provided that this entire notice is included in all copies of any software
    /// which is or includes a copy or modification of this software and in all copies of the
    /// supporting documentation for such software.
    /// </para><para>
    /// THIS SOFTWARE IS BEING PROVIDED "AS IS", WITHOUT ANY EXPRESS OR IMPLIED WARRANTY.  IN
    /// PARTICULAR, NEITHER THE AUTHORS NOR AT&amp;T MAKE ANY REPRESENTATION OR WARRANTY OF ANY KIND
    /// CONCERNING THE MERCHANTABILITY OF THIS SOFTWARE OR ITS FITNESS FOR ANY PARTICULAR PURPOSE.
    /// </para></remarks>

    [Serializable]
    public class Voronoi {
        #region FindAll(PointD[])

        /// <overloads>
        /// Finds the Voronoi diagram and the Delaunay triangulation for the specified set of <see
        /// cref="PointD"/> coordinates.</overloads>
        /// <summary>
        /// Finds the Voronoi diagram and the Delaunay triangulation for the specified set of <see
        /// cref="PointD"/> coordinates, using default clipping rectangle.</summary>
        /// <param name="points">
        /// An <see cref="Array"/> containing the <see cref="PointD"/> coordinates whose Voronoi
        /// diagram and Delaunay triangulation to find.</param>
        /// <returns>
        /// A <see cref="VoronoiResults"/> object containing the Voronoi diagram and Delaunay
        /// triangulation for the specified <paramref name="points"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="points"/> contains less than three elements.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="points"/> is a null reference.</exception>

        public static VoronoiResults FindAll(PointD[] points) {
            return FindAll(points, RectD.Empty);
        }

        #endregion
        #region FindAll(PointD[], RectD)

        /// <summary>
        /// Finds the Voronoi diagram and the Delaunay triangulation for the specified set of <see
        /// cref="PointD"/> coordinates, using the specified clipping rectangle.</summary>
        /// <param name="points">
        /// An <see cref="Array"/> containing the <see cref="PointD"/> coordinates whose Voronoi
        /// diagram and Delaunay triangulation to find.</param>
        /// <param name="clip">
        /// A <see cref="RectD"/> that indicates the clipping bounds for pseudo-vertices.</param>
        /// <returns>
        /// A <see cref="VoronoiResults"/> object containing the Voronoi diagram and Delaunay
        /// triangulation for the specified <paramref name="points"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="points"/> contains less than three elements.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="points"/> is a null reference.</exception>
        /// <remarks>
        /// The actual clipping rectangle always extends somewhat beyond the bounding rectangle of
        /// the specified <paramref name="points"/>. The specified <paramref name="clip"/> rectangle
        /// can only extend this area, not restrict it.</remarks>

        public static VoronoiResults FindAll(PointD[] points, RectD clip) {

            Voronoi voronoi = new Voronoi(points, ref clip, false);
            voronoi.SweepLine();

            return new VoronoiResults(clip, points,
                voronoi._voronoiVertices.ToArray(),
                voronoi._voronoiEdges.ToArray());
        }

        #endregion
        #region FindDelaunay

        /// <summary>
        /// Finds the Delaunay triangulation for the specified set of <see cref="PointD"/>
        /// coordinates.</summary>
        /// <param name="points">
        /// An <see cref="Array"/> containing the <see cref="PointD"/> coordinates whose Delaunay
        /// triangulation to find.</param>
        /// <returns>
        /// An <see cref="Array"/> containing all edges of the Delaunay triangulation, stored as
        /// index pairs relative to the specified <paramref name="points"/> array.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="points"/> contains less than three elements.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="points"/> is a null reference.</exception>
        /// <remarks><para>
        /// The <see cref="PointI.X"/> and <see cref="PointI.Y"/> components of each <see
        /// cref="PointI"/> element in the returned <see cref="Array"/> hold the indices of any two 
        /// <paramref name="points"/> elements that are connected by an edge in the Delaunay
        /// triangulation.
        /// </para><para>
        /// Use <see cref="LineD.FromIndexPoints"/> to convert <paramref name="points"/> and the
        /// returned index <see cref="Array"/> into a <see cref="LineD"/> array.</para></remarks>

        public static PointI[] FindDelaunay(PointD[] points) {

            RectD clip = RectD.Empty;
            Voronoi voronoi = new Voronoi(points, ref clip, true);
            voronoi.SweepLine();

            return voronoi._delaunayEdges.ToArray();
        }

        #endregion
        #region Voronoi(...)

        /// <summary>
        /// Initializes a new instance of the <see cref="Voronoi"/> class with the specified set of
        /// <see cref="PointD"/> coordinates and requested actions.</summary>
        /// <param name="points">
        /// An <see cref="Array"/> containing the <see cref="PointD"/> coordinates whose Voronoi
        /// diagram and/or Delaunay triangulation to find.</param>
        /// <param name="clip">
        /// A <see cref="RectD"/> that indicates the desired clipping bounds for pseudo-vertices,
        /// and returns the actual clipping bounds for the Voronoi diagram.</param>
        /// <param name="findDelaunay">
        /// <c>true</c> to find only the Delaunay triangulation for the specified <paramref
        /// name="points"/>; <c>false</c> to also find the Voronoi diagram.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="points"/> contains less than three elements.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="points"/> is a null reference.</exception>
        /// <remarks><para>
        /// Call <see cref="SweepLine"/> on the returned <see cref="Voronoi"/> object to actually
        /// compute the Voronoi diagram and/or Delaunay triangulation for the specified <paramref
        /// name="points"/>.
        /// </para><para>
        /// The specified <paramref name="clip"/> rectangle is ignored if <paramref
        /// name="findDelaunay"/> is <c>true</c>, or if its <see cref="RectD.Width"/> or <see
        /// cref="RectD.Height"/> component is not positive.</para></remarks>

        private Voronoi(PointD[] points, ref RectD clip, bool findDelaunay) {
            if (points == null)
                ThrowHelper.ThrowArgumentNullException("points");
            if (points.Length < 3)
                ThrowHelper.ThrowArgumentExceptionWithFormat(
                    "points.Length", Strings.ArgumentLessValue, 3);

            /*
             * Copy input array points[] into work array _sites[].
             * _sites[].Index values refer to original points[] array,
             * not to _sites[] which is subsequently sorted and modified.
             */

            _sites = new SiteVertex[points.Length];
            for (int i = 0; i < _sites.Length; i++)
                _sites[i] = new SiteVertex(points[i], i);

            // sort by ascending y-coordinates, then x-coordinates
            Comparison<SiteVertex> comparison = (s, t) => {
                if (s == t) return 0;

                if (s.Y < t.Y) return -1;
                if (s.Y > t.Y) return 1;
                if (s.X < t.X) return -1;
                if (s.X > t.X) return 1;

                return 0;
            };

            Array.Sort<SiteVertex>(_sites, comparison);

            // find minimum & maximum x-coordinates
            _minX = _maxX = _sites[0].X;
            for (int i = 1; i < _sites.Length; i++) {
                double x = _sites[i].X;
                if (x < _minX) _minX = x;
                if (x > _maxX) _maxX = x;
            }

            // sort defines minimum & maximum y-coordinates
            _minY = _sites[0].Y;
            _maxY = _sites[_sites.Length - 1].Y;

            /*
             * Voronoi diagrams and Delaunay triangulations contain at most 3n-6 edges,
             * and Voronoi diagrams contain at most 2n-5 vertices for n >= 3 input points.
             * 
             * We allocate space for 2n vertices to allow for additional pseudo-vertices.
             * This extra space is not required for the vertex index mapping array since
             * pseudo-vertices don’t use SiteVertex objects with index remapping.
             */

            int maxVertexCount = 2 * _sites.Length - 5;
            int maxEdgeCount = 3 * _sites.Length - 6;

            if (findDelaunay) {
                // only allocate Delaunay edge output storage
                _delaunayEdges = new List<PointI>(maxEdgeCount);
                return;
            }

            // calculate clipping region for Voronoi edges
            double dx = _maxX - _minX;
            double dy = _maxY - _minY;
            double d = Math.Max(dx, dy) * 1.1;

            _minClipX = _minX - (d - dx) / 2;
            _maxClipX = _maxX + (d - dx) / 2;
            _minClipY = _minY - (d - dy) / 2;
            _maxClipY = _maxY + (d - dy) / 2;

            // extend clipping region to specified bounds, if any
            if (clip.Width > 0 && clip.Height > 0) {
                _minClipX = Math.Min(_minClipX, clip.Left);
                _maxClipX = Math.Max(_maxClipX, clip.Right);
                _minClipY = Math.Min(_minClipY, clip.Top);
                _maxClipY = Math.Max(_maxClipY, clip.Bottom);
            }

            /*
            * Convert extended clipping region into RectD and re-extract clipping bounds.
            * 
            * This is necessary because RectD.Right & RectD.Bottom are computed, not stored.
            * Converting the clipping bounds into a RectD therefore may introduce very small
            * inequalities between the specified and the computed right & bottom coordinates.
            * Such inequalities would later wreak havoc when VoronoiResults compares vertex
            * coordinates against clipping bounds while closing the open VoronoiRegions.
            */

            clip = new RectD(
                new PointD(_minClipX, _minClipY),
                new PointD(_maxClipX, _maxClipY));

            _minClipX = clip.Left; _minClipY = clip.Top;
            _maxClipX = clip.Right; _maxClipY = clip.Bottom;

            // allocate Voronoi output & auxiliary storage
            _voronoiVertices = new List<PointD>(maxVertexCount + 5);
            _voronoiEdges = new List<VoronoiEdge>(maxEdgeCount);
            _vertexIndices = new int[maxVertexCount];
        }

        #endregion
        #region Private Fields

        // marks edge as deleted within edge list
        private readonly static FullEdge _deletedEdge = new FullEdge();

        // bounding rectangle of input coordinates
        private double _minX, _maxX, _minY, _maxY;

        // clipping rectangle for Voronoi output coordinates
        // (slightly larger than input to show unbounded edges)
        private double _minClipX, _maxClipX, _minClipY, _maxClipY;

        // edge list with its left & right ends
        private HalfEdge[] _edgeList;
        private HalfEdge _edgeListLeft, _edgeListRight;

        // priority queue with count & lower barrier
        private HalfEdge[] _priQueue;
        private int _priQueueCount, _priQueueMin;

        // generator sites for Voronoi diagram
        private SiteVertex[] _sites;

        // counter yielding unique internal index
        private int _vertexCount;

        // mapping of internal to output indices
        private int[] _vertexIndices;

        // Delaunay triangulation and Voronoi diagram
        private List<PointI> _delaunayEdges;
        private List<VoronoiEdge> _voronoiEdges;
        private List<PointD> _voronoiVertices;

        #endregion
        #region Private Methods
        #region SweepLine

        /// <summary>
        /// Performs Fortune’s sweep line algorithm on the current set of input coordinates.
        /// </summary>

        private void SweepLine() {

            SiteVertex lowSite, highSite, p, v;
            PointD minSite = PointD.Empty;
            HalfEdge leftHE, rightHE, prevHE, nextHE, bisectHE;
            FullEdge bisector;

            PriQueueInitialize();
            EdgeListInitialize();

            // get second input site
            int newSiteIndex = 1;
            SiteVertex newSite = _sites[newSiteIndex];

            while (true) {
                if (_priQueueCount != 0) minSite = PriQueuePeek();

                if (newSite != null &&
                    (_priQueueCount == 0 || newSite.Y < minSite.Y ||
                    (newSite.Y == minSite.Y && newSite.X < minSite.X))) {

                    // new site is smallest
                    leftHE = EdgeListLeftBound(newSite);
                    rightHE = leftHE.Right;
                    lowSite = GetRightSite(leftHE);
                    bisector = BisectSites(lowSite, newSite);
                    bisectHE = new HalfEdge(bisector, false);

                    EdgeListInsert(leftHE, bisectHE);
                    p = Intersect(leftHE, bisectHE);
                    if (p != null) {
                        PriQueueDelete(leftHE);
                        PriQueueInsert(leftHE, p, GetDistance(p, newSite));
                    }
                    leftHE = bisectHE;
                    bisectHE = new HalfEdge(bisector, true);

                    EdgeListInsert(leftHE, bisectHE);
                    p = Intersect(bisectHE, rightHE);
                    if (p != null)
                        PriQueueInsert(bisectHE, p, GetDistance(p, newSite));

                    newSite = null;
                    if (++newSiteIndex < _sites.Length)
                        newSite = _sites[newSiteIndex];
                }
                else if (_priQueueCount != 0) {

                    // intersection is smallest
                    leftHE = PriQueuePop();
                    rightHE = leftHE.Right;
                    prevHE = leftHE.Left;
                    nextHE = rightHE.Right;

                    lowSite = GetLeftSite(leftHE);
                    highSite = GetRightSite(rightHE);
                    v = leftHE.Vertex;
                    v.Index = _vertexCount++;

                    // create new Voronoi vertex if within plotting area
                    if (_voronoiEdges != null &&
                        v.X >= _minClipX && v.X <= _maxClipX &&
                        v.Y >= _minClipY && v.Y <= _maxClipY) {

                        _vertexIndices[v.Index] = _voronoiVertices.Count;
                        _voronoiVertices.Add(new PointD(v.X, v.Y));
                    }

                    AddVertex(leftHE.Edge, leftHE.IsRight, v);
                    AddVertex(rightHE.Edge, rightHE.IsRight, v);

                    EdgeListDelete(leftHE);
                    PriQueueDelete(rightHE);
                    EdgeListDelete(rightHE);

                    bool isRight = false;
                    if (lowSite.Y > highSite.Y) {
                        SiteVertex tmpSite = lowSite;
                        lowSite = highSite;
                        highSite = tmpSite;
                        isRight = true;
                    }

                    bisector = BisectSites(lowSite, highSite);
                    bisectHE = new HalfEdge(bisector, isRight);
                    EdgeListInsert(prevHE, bisectHE);
                    AddVertex(bisector, !isRight, v);

                    p = Intersect(prevHE, bisectHE);
                    if (p != null) {
                        PriQueueDelete(prevHE);
                        PriQueueInsert(prevHE, p, GetDistance(p, lowSite));
                    }

                    p = Intersect(bisectHE, nextHE);
                    if (p != null)
                        PriQueueInsert(bisectHE, p, GetDistance(p, lowSite));
                } else
                    break;
            }

            // output remaining Voronoi edges (those with only one vertex)
            if (_voronoiEdges != null)
                for (HalfEdge he = _edgeListLeft.Right;
                    he != _edgeListRight; he = he.Right)
                    StoreVoronoiEdge(he.Edge);
        }

        #endregion
        #region AddVertex

        /// <summary>
        /// Adds the specified vertex to the specified side of the specified edge.</summary>
        /// <param name="e">
        /// The <see cref="FullEdge"/> to which to add <paramref name="s"/>.</param>
        /// <param name="isRight">
        /// <c>true</c> to add <paramref name="s"/> to the right side of <paramref name="e"/>;
        /// <c>false</c> to add to the left side.</param>
        /// <param name="s">
        /// The <see cref="SiteVertex"/> that represents the vertex to add.</param>
        /// <remarks>
        /// When <paramref name="e"/> contains vertices on both sides, it is added to the Voronoi
        /// diagram.</remarks>

        private void AddVertex(FullEdge e, bool isRight, SiteVertex s) {
            e.SetVertex(isRight, s);
            if (_voronoiEdges != null && e.GetVertex(!isRight) != null)
                StoreVoronoiEdge(e);
        }

        #endregion
        #region BisectSites

        /// <summary>
        /// Creates a Voronoi edge that bisects the two specified sites.</summary>
        /// <param name="s">
        /// The <see cref="SiteVertex"/> that represents the first site to bisect.</param>
        /// <param name="t">
        /// The <see cref="SiteVertex"/> that represents the second site to bisect.</param>
        /// <returns>
        /// The new <see cref="FullEdge"/> the represents the Voronoi edge bisecting <paramref
        /// name="s"/> and <paramref name="t"/>.</returns>
        /// <remarks>
        /// <b>BisectSites</b> also creates a corresponding Delaunay edge if desired.</remarks>

        private FullEdge BisectSites(SiteVertex s, SiteVertex t) {

            FullEdge e = new FullEdge();
            e.LeftSite = s;
            e.RightSite = t;

            double dx = t.X - s.X;
            double dy = t.Y - s.Y;
            double adx = (dx > 0 ? dx : -dx);
            double ady = (dy > 0 ? dy : -dy);
            e.C = s.X * dx + s.Y * dy + (dx * dx + dy * dy) / 2;

            if (adx > ady) {
                // horizontal edge (+45...0...-45 degrees)
                e.A = 1;
                e.B = dy / dx;
                e.C = e.C / dx;
            } else {
                // vertical edge (-45...90...+45 degrees)
                e.A = dx / dy;
                e.B = 1;
                e.C = e.C / dy;
            }

            /*
             *  By definition, whenever two sites s and t have been bisected by a
             *  Voronoi edge e they also form an edge of the Delaunay triangulation.
             *  So if a triangulation is desired, the Delaunay edge is now stored.
             */

            if (_delaunayEdges != null)
                _delaunayEdges.Add(new PointI(s.Index, t.Index));

            return e;
        }

        #endregion
        #region GetDistance

        /// <summary>
        /// Computes the distance between the two specified sites or vertices.</summary>
        /// <param name="s">
        /// The first <see cref="SiteVertex"/> to examine.</param>
        /// <param name="t">
        /// The second <see cref="SiteVertex"/> to examine.</param>
        /// <returns>
        /// The distance between the <see cref="SiteVertex.X"/> and <see cref="SiteVertex.Y"/>
        /// coordinates of the specified <paramref name="s"/> and <paramref name="t"/>.</returns>

        private static double GetDistance(SiteVertex s, SiteVertex t) {
            double dx = s.X - t.X;
            double dy = s.Y - t.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        #endregion
        #region GetLeftSite

        /// <summary>
        /// Gets the site to the left of the specified edge.</summary>
        /// <param name="he">
        /// The <see cref="HalfEdge"/> to examine.</param>
        /// <returns>
        /// The <see cref="FullEdge.RightSite"/> of the <see cref="HalfEdge.Edge"/> of the specified
        /// <paramref name="he"/> if its <see cref="HalfEdge.IsRight"/> flag is <c>true</c>;
        /// otherwise, the <see cref="FullEdge.LeftSite"/>.</returns>

        private SiteVertex GetLeftSite(HalfEdge he) {
            if (he.Edge == null) return _sites[0];

            if (he.IsRight)
                return he.Edge.RightSite;
            else
                return he.Edge.LeftSite;
        }

        #endregion
        #region GetRightSite

        /// <summary>
        /// Gets the site to the right of the specified edge.</summary>
        /// <param name="he">
        /// The <see cref="HalfEdge"/> to examine.</param>
        /// <returns>
        /// The <see cref="FullEdge.RightSite"/> of the <see cref="HalfEdge.Edge"/> of the specified
        /// <paramref name="he"/> if its <see cref="HalfEdge.IsRight"/> flag is <c>false</c>;
        /// otherwise, the <see cref="FullEdge.LeftSite"/>.</returns>

        private SiteVertex GetRightSite(HalfEdge he) {
            if (he.Edge == null) return _sites[0];

            if (he.IsRight)
                return he.Edge.LeftSite;
            else
                return he.Edge.RightSite;
        }

        #endregion
        #region Intersect

        /// <summary>
        /// Creates a Voronoi vertex at the intersection of the specified edges.</summary>
        /// <param name="he1">
        /// The first <see cref="HalfEdge"/> to intersect.</param>
        /// <param name="he2">
        /// The second <see cref="HalfEdge"/> to intersect.</param>
        /// <returns>
        /// The new <see cref="SiteVertex"/> that represents the Voronoi vertex at the intersection
        /// of <paramref name="he1"/> and <paramref name="he2"/>, if found; otherwise, a null
        /// reference.</returns>

        private static SiteVertex Intersect(HalfEdge he1, HalfEdge he2) {

            FullEdge e1 = he1.Edge;
            FullEdge e2 = he2.Edge;
            if (e1 == null || e2 == null) return null;
            if (e1.RightSite == e2.RightSite) return null;

            double d = e1.A * e2.B - e1.B * e2.A;
            if (Math.Abs(d) < 1.0e-10) return null;

            double xint = (e1.C * e2.B - e2.C * e1.B) / d;
            double yint = (e2.C * e1.A - e1.C * e2.A) / d;

            HalfEdge el; FullEdge e;
            if ((e1.RightSite.Y < e2.RightSite.Y) ||
                (e1.RightSite.Y == e2.RightSite.Y && e1.RightSite.X < e2.RightSite.X)) {
                el = he1; e = e1;
            } else {
                el = he2; e = e2;
            }

            bool isRightOfSite = (xint >= e.RightSite.X);
            if ((isRightOfSite && !el.IsRight) || (!isRightOfSite && el.IsRight))
                return null;

            return new SiteVertex(xint, yint);
        }

        #endregion
        #region IsRightOf

        /// <summary>
        /// Determines whether the specified site is to the right of the specified edge.</summary>
        /// <param name="he">
        /// The <see cref="HalfEdge"/> to examine.</param>
        /// <param name="p">
        /// The <see cref="SiteVertex"/> that represents the site to examine.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="p"/> is to the right of <paramref name="he"/>; otherwise,
        /// <c>false</c>.</returns>

        private static bool IsRightOf(HalfEdge he, SiteVertex p) {

            FullEdge e = he.Edge;
            bool isRightOfSite = (p.X > e.RightSite.X);
            if (isRightOfSite && !he.IsRight) return true;
            if (!isRightOfSite && he.IsRight) return false;

            bool isAbove;
            if (e.A == 1) {
                double dyp = p.Y - e.RightSite.Y;
                double dxp = p.X - e.RightSite.X;
                bool isFast = false;

                if ((!isRightOfSite && e.B < 0) || (isRightOfSite && e.B >= 0)) {
                    isAbove = (dyp >= e.B * dxp);
                    isFast = isAbove;
                } else {
                    isAbove = (p.X + p.Y * e.B > e.C);
                    if (e.B < 0) isAbove = !isAbove;
                    if (!isAbove) isFast = true;
                }

                if (!isFast) {
                    double dxs = e.RightSite.X - e.LeftSite.X;
                    isAbove = (e.B * (dxp * dxp - dyp * dyp) <
                              dxs * dyp * (1 + 2 * dxp / dxs + e.B * e.B));
                    if (e.B < 0) isAbove = !isAbove;
                }
            } else {
                double yl = e.C - e.A * p.X;
                double t1 = p.Y - yl;
                double t2 = p.X - e.RightSite.X;
                double t3 = yl - e.RightSite.Y;
                isAbove = (t1 * t1 > t2 * t2 + t3 * t3);
            }

            return (he.IsRight ? !isAbove : isAbove);
        }

        #endregion
        #region StoreVoronoiEdge

        /// <summary>
        /// Stores the specified Voronoi edge, clipped to the desired output region.</summary>
        /// <param name="e">
        /// The <see cref="FullEdge"/> that represents the Voronoi edge to store.</param>
        /// <remarks>
        /// <b>StoreVoronoiEdge</b> also creates a new pseudo-vertex in the Voronoi diagram if 
        /// <paramref name="e"/> extends beyond the desired output region.</remarks>

        private void StoreVoronoiEdge(FullEdge e) {
            Debug.Assert(_voronoiEdges != null);

            SiteVertex s1, s2;
            double x1, x2, y1, y2;

            /*
             * e.LeftVertex stores the left vertex of e, and e.RightVertex stores the right vertex.
             *
             * For "vertical" edges (e.a != 1 && e.b == 1), this is all we need to know.
             * s1 is the left vertex, s2 is the right vertex, as in the "else" branch.
             *
             * For "horizontal" edges (e.a == 1), however, s1 should be the lower vertex and s2
             * the higher one. If the edge is pointing downward (e.b >= 0), the right vertex is
             * lower and the left one is higher. So we must reverse the vertices in the "if" branch.
             *
             * If the edge is pointing upward (e.b < 0), the left vertex is lower and the right
             * one is higher. We can reuse the "else" branch in this case.
             */

            if (e.A == 1 && e.B >= 0) {
                s1 = e.RightVertex;
                s2 = e.LeftVertex;
            } else {
                s1 = e.LeftVertex;
                s2 = e.RightVertex;
            }

            // the following holds true as per BisectSites
            Debug.Assert(e.A == 1 || e.B == 1);

            if (e.A == 1) {
                // horizontal edge (+45...0...-45 degrees)

                if (s1 != null && s1.Y > _minClipY) {
                    y1 = s1.Y;
                    // edge invisible if lower vertex above limits
                    if (y1 > _maxClipY) return;
                } else
                    y1 = _minClipY;

                if (s2 != null && s2.Y < _maxClipY) {
                    y2 = s2.Y;
                    // edge invisible if higher vertex below limits
                    if (y2 < _minClipY) return;
                } else
                    y2 = _maxClipY;

                x1 = e.C - e.B * y1;
                x2 = e.C - e.B * y2;

                if (x1 > _maxClipX) {
                    if (x2 > _maxClipX) return;
                    x1 = _maxClipX; y1 = (e.C - x1) / e.B;
                }
                else if (x1 < _minClipX) {
                    if (x2 < _minClipX) return;
                    x1 = _minClipX; y1 = (e.C - x1) / e.B;
                }

                if (x2 > _maxClipX) {
                    x2 = _maxClipX; y2 = (e.C - x2) / e.B;
                }
                else if (x2 < _minClipX) {
                    x2 = _minClipX; y2 = (e.C - x2) / e.B;
                }
            } else {
                // (e.a != 1), hence (e.b == 1): vertical edge (-45...90...+45 degrees)

                if (s1 != null && s1.X > _minClipX) {
                    x1 = s1.X;
                    // edge invisible if left vertex right of limits
                    if (x1 > _maxClipX) return;
                } else
                    x1 = _minClipX;

                if (s2 != null && s2.X < _maxClipX) {
                    x2 = s2.X;
                    // edge invisible if right vertex left of limits
                    if (x2 < _minClipX) return;
                } else
                    x2 = _maxClipX;

                y1 = e.C - e.A * x1;
                y2 = e.C - e.A * x2;

                if (y1 > _maxClipY) {
                    if (y2 > _maxClipY) return;
                    y1 = _maxClipY; x1 = (e.C - y1) / e.A;
                }
                else if (y1 < _minClipY) {
                    if (y2 < _minClipY) return;
                    y1 = _minClipY; x1 = (e.C - y1) / e.A;
                }

                if (y2 > _maxClipY) {
                    y2 = _maxClipY; x2 = (e.C - y2) / e.A;
                }
                else if (y2 < _minClipY) {
                    y2 = _minClipY; x2 = (e.C - y2) / e.A;
                }
            }

            /*
             * Voronoi vertices s1, s2 are output if they are defined (i.e. the edge is
             * closed on that end) and the vertex lies within the plotting region. Otherwise
             * a new pseudo-vertex is created, situated on one of the region boundaries.
             */

            int vertex1, vertex2;
            if (s1 != null &&
                s1.X >= _minClipX && s1.X <= _maxClipX &&
                s1.Y >= _minClipY && s1.Y <= _maxClipY) {
                // use existing vertex s1 to start new edge
                vertex1 = _vertexIndices[s1.Index];
            } else {
                // create new pseudo-vertex at (x1, y1)
                vertex1 = _voronoiVertices.Count;
                _voronoiVertices.Add(new PointD(x1, y1));
            }

            if (s2 != null &&
                s2.X >= _minClipX && s2.X <= _maxClipX &&
                s2.Y >= _minClipY && s2.Y <= _maxClipY) {
                // use existing vertex s2 to end new edge
                vertex2 = _vertexIndices[s2.Index];
            } else {
                // create new pseudo-vertex at (x2, y2)
                vertex2 = _voronoiVertices.Count;
                _voronoiVertices.Add(new PointD(x2, y2));
            }

            // add Voronoi vertex with indices of bisected sites
            var ve = new VoronoiEdge(e.LeftSite.Index, e.RightSite.Index, vertex1, vertex2);
            _voronoiEdges.Add(ve);
        }

        #endregion
        #region EdgeListInitialize

        /// <summary>
        /// Initializes the edge list.</summary>

        private void EdgeListInitialize() {

            int n = (int) (2 * Math.Sqrt(_sites.Length + 4));
            _edgeList = new HalfEdge[n];

            _edgeListLeft = new HalfEdge();
            _edgeListRight = new HalfEdge();

            _edgeListLeft.Right = _edgeListRight;
            _edgeListRight.Left = _edgeListLeft;

            _edgeList[0] = _edgeListLeft;
            _edgeList[n - 1] = _edgeListRight;
        }

        #endregion
        #region EdgeListInsert

        /// <summary>
        /// Inserts the specified <see cref="HalfEdge"/> at the specified position in the edge list.
        /// </summary>
        /// <param name="hePos">
        /// The <see cref="HalfEdge"/> in the edge list that will be the left neighbor of the
        /// inserted <paramref name="heNew"/>.</param>
        /// <param name="heNew">
        /// The <see cref="HalfEdge"/> to insert to the right of <paramref name="hePos"/>.</param>

        private static void EdgeListInsert(HalfEdge hePos, HalfEdge heNew) {
            heNew.Left = hePos;
            heNew.Right = hePos.Right;
            hePos.Right.Left = heNew;
            hePos.Right = heNew;
        }

        #endregion
        #region EdgeListDelete

        /// <summary>
        /// Deletes the specified <see cref="HalfEdge"/> from the edge list.</summary>
        /// <param name="he">
        /// The <see cref="HalfEdge"/> to delete.</param>

        private static void EdgeListDelete(HalfEdge he) {
            he.Left.Right = he.Right;
            he.Right.Left = he.Left;
            he.Edge = Voronoi._deletedEdge;
        }

        #endregion
        #region EdgeListHash

        /// <summary>
        /// Gets the <see cref="HalfEdge"/> at the specified hash bucket in the edge list.</summary>
        /// <param name="bucket">
        /// The hash bucket to search.</param>
        /// <returns>
        /// The <see cref="HalfEdge"/> at the specified hash bucket in the edge list, if any;
        /// otherwise, a null reference.</returns>
        /// <remarks>
        /// If the <see cref="HalfEdge"/> at the specified <paramref name="bucket"/> references a
        /// deleted <see cref="FullEdge"/>, <b>EdgeListHash</b> removes it from the edge list and
        /// returns a null reference.</remarks>

        private HalfEdge EdgeListHash(int bucket) {
            if (bucket < 0 || bucket >= _edgeList.Length)
                return null;

            HalfEdge he = _edgeList[bucket];
            if (he == null || he.Edge != Voronoi._deletedEdge)
                return he;

            // hashtable points to deleted half edge
            _edgeList[bucket] = null;
            return null;
        }

        #endregion
        #region EdgeListLeftBound

        /// <summary>
        /// Finds the left bound of the specified site in the edge list.</summary>
        /// <param name="s">
        /// The <see cref="SiteVertex"/> that represents the site to find.</param>
        /// <returns>
        /// The <see cref="HalfEdge"/> that represents the left bound of <paramref name="s"/>.
        /// </returns>

        private HalfEdge EdgeListLeftBound(SiteVertex s) {

            // use hash table to get close to desired half-edge
            int n = _edgeList.Length;
            int bucket = unchecked((int) ((s.X - _minX) / (_maxX - _minX) * n));
            if (bucket < 0) bucket = 0;
            if (bucket >= n) bucket = n - 1;

            HalfEdge he = EdgeListHash(bucket);
            if (he == null)
                for (int i = 1; true; i++) {
                    he = EdgeListHash(bucket - i); if (he != null) break;
                    he = EdgeListHash(bucket + i); if (he != null) break;
                }

            // now search linear list of half-edges for the correct one
            if (he == _edgeListLeft ||
                (he != _edgeListRight && IsRightOf(he, s))) {
                do
                    he = he.Right;
                while (he != _edgeListRight && IsRightOf(he, s));
                he = he.Left;
            } else {
                do
                    he = he.Left;
                while (he != _edgeListLeft && !IsRightOf(he, s));
            }

            // update hash table
            if (bucket > 0 && bucket < n - 1)
                _edgeList[bucket] = he;

            return he;
        }

        #endregion
        #region PriQueueBucket

        /// <summary>
        /// Gets the hash bucket for the specified <see cref="HalfEdge"/> in the priority queue.
        /// </summary>
        /// <param name="he">
        /// The <see cref="HalfEdge"/> whose hash bucket to return.</param>
        /// <returns>
        /// The hash bucket for the specified <paramref name="he"/>.</returns>

        private int PriQueueBucket(HalfEdge he) {

            int n = _priQueue.Length;
            int bucket = unchecked((int) ((he.YStar - _minY) / (_maxY - _minY) * n));

            if (bucket < 0) bucket = 0;
            if (bucket >= n) bucket = n - 1;

            if (bucket < _priQueueMin)
                _priQueueMin = bucket;

            return bucket;
        }

        #endregion
        #region PriQueueDelete

        /// <summary>
        /// Deletes the specified <see cref="HalfEdge"/> from the priority queue.</summary>
        /// <param name="he">
        /// The <see cref="HalfEdge"/> to delete.</param>

        private void PriQueueDelete(HalfEdge he) {
            if (he.Vertex == null) return;

            HalfEdge hash = _priQueue[PriQueueBucket(he)];
            while (hash.Next != he) hash = hash.Next;
            hash.Next = he.Next;

            --_priQueueCount;
            he.Vertex = null;
        }

        #endregion
        #region PriQueueInitialize

        /// <summary>
        /// Initializes the priority queue.</summary>

        private void PriQueueInitialize() {

            _priQueueCount = _priQueueMin = 0;
            int n = (int) (4 * Math.Sqrt(_sites.Length + 4));
            _priQueue = new HalfEdge[n];

            for (int i = 0; i < _priQueue.Length; i++)
                _priQueue[i] = new HalfEdge();
        }

        #endregion
        #region PriQueueInsert

        /// <summary>
        /// Inserts the specified <see cref="HalfEdge"/> with the specified vertex in the priority
        /// queue.</summary>
        /// <param name="he">
        /// The <see cref="HalfEdge"/> to insert.</param>
        /// <param name="v">
        /// The new value for the <see cref="HalfEdge.Vertex"/> of <paramref name="he"/>.</param>
        /// <param name="offset">
        /// The offset to add to the <see cref="HalfEdge.YStar"/> coordinate of <paramref
        /// name="he"/>.</param>

        private void PriQueueInsert(HalfEdge he, SiteVertex v, double offset) {
            he.Vertex = v;
            he.YStar = v.Y + offset;

            HalfEdge hash = _priQueue[PriQueueBucket(he)];
            HalfEdge next = hash.Next;

            while (next != null && (he.YStar > next.YStar ||
                (he.YStar == next.YStar && v.X > next.Vertex.X))) {
                hash = next;
                next = hash.Next;
            }

            he.Next = hash.Next;
            hash.Next = he;
            ++_priQueueCount;
        }

        #endregion
        #region PriQueuePeek

        /// <summary>
        /// Returns the coordinates of the first <see cref="HalfEdge"/> in the priority queue,
        /// without removing it.</summary>
        /// <returns>
        /// A <see cref="PointD"/> containing the <see cref="SiteVertex.X"/> and <see
        /// cref="HalfEdge.YStar"/> coordinates of the first <see cref="HalfEdge"/> in the priority
        /// queue.</returns>

        private PointD PriQueuePeek() {
            while (_priQueue[_priQueueMin].Next == null)
                ++_priQueueMin;

            return new PointD(
                _priQueue[_priQueueMin].Next.Vertex.X,
                _priQueue[_priQueueMin].Next.YStar);
        }
        
        #endregion
        #region PriQueuePop

        /// <summary>
        /// Removes and returns the first <see cref="HalfEdge"/> in the priority queue.</summary>
        /// <returns>
        /// The removed first <see cref="HalfEdge"/> in the priority queue.</returns>

        private HalfEdge PriQueuePop() {
            HalfEdge he = _priQueue[_priQueueMin].Next;
            _priQueue[_priQueueMin].Next = he.Next;
            --_priQueueCount;
            return he;
        }

        #endregion
        #endregion
        #region Class FullEdge

        /// <summary>
        /// Represents one edge in the Voronoi diagram.</summary>

        [Serializable]
        private class FullEdge {
            #region A

            /// <summary>
            /// The A component of the line equation for the <see cref="FullEdge"/> (ax + by = c).
            /// </summary>

            internal double A;

            #endregion
            #region B

            /// <summary>
            /// The B component of the line equation for the <see cref="FullEdge"/> (ax + by = c).
            /// </summary>

            internal double B;

            #endregion
            #region C

            /// <summary>
            /// The C component of the line equation for the <see cref="FullEdge"/> (ax + by = c).
            /// </summary>

            internal double C;

            #endregion
            #region LeftSite

            /// <summary>
            /// The <see cref="SiteVertex"/> that represents the left-hand generator site of the
            /// pair that is bisected by the <see cref="FullEdge"/>.</summary>

            internal SiteVertex LeftSite;

            #endregion
            #region LeftVertex

            /// <summary>
            /// The <see cref="SiteVertex"/> that represents the left-hand Voronoi vertex
            /// terminating the <see cref="FullEdge"/>.</summary>

            internal SiteVertex LeftVertex;

            #endregion
            #region RightSite

            /// <summary>
            /// The <see cref="SiteVertex"/> that represents the right-hand generator site of the
            /// pair that is bisected by the <see cref="FullEdge"/>.</summary>

            internal SiteVertex RightSite;

            #endregion
            #region RightVertex

            /// <summary>
            /// The <see cref="SiteVertex"/> that represents the right-hand Voronoi vertex
            /// terminating the <see cref="FullEdge"/>.</summary>

            internal SiteVertex RightVertex;

            #endregion
            #region GetVertex

            /// <summary>
            /// Gets the Voronoi vertex on the specified side of the <see cref="FullEdge"/>.
            /// </summary>
            /// <param name="isRight">
            /// <c>true</c> to get the <see cref="RightVertex"/>; <c>false</c> to get the <see
            /// cref="LeftVertex"/>.</param>
            /// <returns>
            /// The value of the <see cref="RightVertex"/> field if <paramref name="isRight"/> is
            /// <c>true</c>; otherwise, the value of the <see cref="LeftVertex"/> field.</returns>

            internal SiteVertex GetVertex(bool isRight) {
                return (isRight ? RightVertex : LeftVertex);
            }

            #endregion
            #region SetVertex

            /// <summary>
            /// Sets the Voronoi vertex on the specified side of the <see cref="FullEdge"/> to the
            /// specified <see cref="SiteVertex"/>.</summary>
            /// <param name="isRight">
            /// <c>true</c> to set the <see cref="RightVertex"/>; <c>false</c> to set the <see
            /// cref="LeftVertex"/>.</param>
            /// <param name="vertex">
            /// The new value for the <see cref="RightVertex"/> or <see cref="LeftVertex"/> field,
            /// depending on the specified <paramref name="isRight"/> flag.</param>

            internal void SetVertex(bool isRight, SiteVertex vertex) {
                if (isRight)
                    RightVertex = vertex;
                else
                    LeftVertex = vertex;
            }

            #endregion
        }

        #endregion
        #region Class HalfEdge

        /// <summary>
        /// Represents one side of an edge of the Voronoi diagram.</summary>

        [Serializable]
        private class HalfEdge {
            #region HalfEdge()

            /// <overloads>
            /// Initializes a new instance of the <see cref="HalfEdge"/> class.</overloads>
            /// <summary>
            /// Initializes a new instance of the <see cref="HalfEdge"/> class with default
            /// properties.</summary>

            internal HalfEdge() { }

            #endregion
            #region HalfEdge(FullEdge, Boolean)

            /// <summary>
            /// Initializes a new instance of the <see cref="HalfEdge"/> class with the
            /// specified <see cref="FullEdge"/> and direction flag.</summary>
            /// <param name="e">
            /// The <see cref="FullEdge"/> of which the <see cref="HalfEdge"/> is a part.</param>
            /// <param name="isRight">
            /// Indicates whether the <see cref="HalfEdge"/> is the right or left part of the
            /// associated <see cref="Edge"/>.</param>

            internal HalfEdge(FullEdge e, bool isRight) {
                Edge = e;
                IsRight = isRight;
            }

            #endregion
            #region Edge

            /// <summary>
            /// The <see cref="FullEdge"/> of which the <see cref="HalfEdge"/> is a part.</summary>

            internal FullEdge Edge;

            #endregion
            #region IsRight

            /// <summary>
            /// Indicates whether the <see cref="HalfEdge"/> is the right or left part of the
            /// associated <see cref="Edge"/>.</summary>

            internal readonly bool IsRight;

            #endregion
            #region Left

            /// <summary>
            /// The <see cref="HalfEdge"/> to the left of this instance in the edge list.</summary>

            internal HalfEdge Left;

            #endregion
            #region Next

            /// <summary>
            /// The <see cref="HalfEdge"/> following this instance in the priority queue.</summary>

            internal HalfEdge Next;

            #endregion
            #region Right

            /// <summary>
            /// The <see cref="HalfEdge"/> to the right of this instance in the edge list.</summary>

            internal HalfEdge Right;

            #endregion
            #region Vertex

            /// <summary>
            /// The <see cref="SiteVertex"/> that represents the Voronoi vertex terminating the <see
            /// cref="HalfEdge"/>.</summary>

            internal SiteVertex Vertex;

            #endregion
            #region YStar

            /// <summary>
            /// The modified y-coordinate of the <see cref="HalfEdge"/> (y + d(z)).</summary>

            internal double YStar;

            #endregion
        }

        #endregion
        #region Class SiteVertex

        /// <summary>
        /// Represents a generator site or vertex in the Voronoi diagram.</summary>

        [Serializable]
        private class SiteVertex {
            #region SiteVertex(Double, Double)

            /// <overloads>
            /// Initializes a new instance of the <see cref="SiteVertex"/> class.</overloads>
            /// <summary>
            /// Initializes a new instance of the <see cref="SiteVertex"/> class with the specified
            /// coordinates.</summary>
            /// <param name="x">
            /// The x-coordinate of the <see cref="SiteVertex"/>.</param>
            /// <param name="y">
            /// The y-coordinate of the <see cref="SiteVertex"/>.</param>

            internal SiteVertex(double x, double y) {
                X = x;
                Y = y;
            }

            #endregion
            #region SiteVertex(PointD, Int32)

            /// <summary>
            /// Initializes a new instance of the <see cref="SiteVertex"/> class with the specified
            /// coordinates and internal index.</summary>
            /// <param name="p">
            /// The <see cref="PointD"/> coordinates of the <see cref="SiteVertex"/>.</param>
            /// <param name="index">
            /// The unique internal index of the <see cref="SiteVertex"/>.</param>

            internal SiteVertex(PointD p, int index) {
                X = p.X;
                Y = p.Y;
                Index = index;
            }

            #endregion
            #region Index

            /// <summary>
            /// The unique internal index of the <see cref="SiteVertex"/>.</summary>

            internal int Index;

            #endregion
            #region X

            /// <summary>
            /// The x-coordinate of the <see cref="SiteVertex"/>.</summary>

            internal readonly double X;

            #endregion
            #region Y

            /// <summary>
            /// The y-coordinate of the <see cref="SiteVertex"/>.</summary>

            internal readonly double Y;

            #endregion
        }

        #endregion
    }
}
