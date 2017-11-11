using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Contains the results of the <see cref="Voronoi"/> algorithm.</summary>
    /// <remarks>
    /// <b>VoronoiResults</b> holds the Voronoi diagram and Delaunay triangulation found by the <see
    /// cref="Voronoi"/> algorithm when both results were requested.</remarks>

    [Serializable]
    public class VoronoiResults {
        #region VoronoiResults(...)

        /// <summary>
        /// Initializes a new instance of the <see cref="VoronoiResults"/> class with the specified
        /// clipping bounds, generator sites, and Voronoi diagram.</summary>
        /// <param name="clippingBounds">
        /// The clipping bounds for the entire Voronoi diagram.</param>
        /// <param name="generatorSites">
        /// The generator sites for the Voronoi diagram and Delaunay triangulation.</param>
        /// <param name="voronoiVertices">
        /// The vertex list for the Voronoi diagram.</param>
        /// <param name="voronoiEdges">
        /// The edge list for the Voronoi diagram.</param>

        internal VoronoiResults(RectD clippingBounds, PointD[] generatorSites,
            PointD[] voronoiVertices, VoronoiEdge[] voronoiEdges) {

            ClippingBounds = clippingBounds;
            GeneratorSites = generatorSites;
            VoronoiVertices = voronoiVertices;
            VoronoiEdges = voronoiEdges;
        }

        #endregion
        #region Private Fields

        /// <summary>
        /// Backs the <see cref="VoronoiRegions"/> property.</summary>

        [NonSerialized]
        private PointD[][] _voronoiRegions;

        #endregion
        #region ClippingBounds

        /// <summary>
        /// The clipping bounds for the entire Voronoi diagram.</summary>
        /// <remarks><para>
        /// <b>ClippingBounds</b> contains the clipping rectangle that was actually used by the <see
        /// cref="Voronoi"/> algorithm, which may be larger than the clipping rectangle originally
        /// supplied to <see cref="Voronoi.FindAll(PointD[], RectD)"/>.
        /// </para><para>
        /// All Voronoi edges are terminated with a pseudo-vertex in <see cref="VoronoiVertices"/>
        /// when they intersect the <see cref="ClippingBounds"/>. No <see cref="VoronoiVertices"/>
        /// lie outside the <see cref="ClippingBounds"/>.
        /// </para><para>
        /// Moreover, the four corners of the <see cref="ClippingBounds"/> are always part of some
        /// <see cref="VoronoiRegions"/> that were originally unbounded. Usually, the <see
        /// cref="VoronoiVertices"/> list does not contain these corner vertices.</para></remarks>

        public readonly RectD ClippingBounds;

        #endregion
        #region DelaunayEdges

        /// <summary>
        /// Gets the edge list for the Delaunay triangulation.</summary>
        /// <value>
        /// An <see cref="Array"/> containing all <see cref="LineD"/> edges of the Delaunay
        /// triangulation.</value>
        /// <remarks>
        /// <b>DelaunayEdges</b> returns an <see cref="Array"/> of the same size as <see
        /// cref="VoronoiEdges"/>. Each element is a <see cref="LineD"/> connecting the <see
        /// cref="GeneratorSites"/> indicated by <see cref="VoronoiEdge.Site1"/> and <see
        /// cref="VoronoiEdge.Site2"/> of the <see cref="VoronoiEdge"/> at the same index position.
        /// </remarks>

        public LineD[] DelaunayEdges {
            get {
                LineD[] edges = new LineD[VoronoiEdges.Length];

                for (int i = 0; i < VoronoiEdges.Length; i++)
                    edges[i] = new LineD(
                        GeneratorSites[VoronoiEdges[i].Site1],
                        GeneratorSites[VoronoiEdges[i].Site2]);

                return edges;
            }
        }

        #endregion
        #region GeneratorSites

        /// <summary>
        /// The generator sites for the Voronoi diagram and Delaunay triangulation.</summary>
        /// <remarks><para>
        /// <b>GeneratorSites</b> holds the <see cref="PointD"/> coordinates whose Voronoi diagram
        /// and Delaunay triangulation are provided by the <see cref="VoronoiResults"/>.
        /// </para><para>
        /// <b>GeneratorSites</b> is the original <see cref="PointD"/> array that was supplied to the
        /// <see cref="Voronoi"/> algorithm and resulted in this <see cref="VoronoiResults"/>
        /// object. This property is provided merely for convenience.</para></remarks>

        public readonly PointD[] GeneratorSites;

        #endregion
        #region VoronoiEdges

        /// <summary>
        /// The edge list for the Voronoi diagram.</summary>
        /// <remarks><para>
        /// <b>VoronoiEdges</b> holds all edges in the Voronoi diagram, stored as double index pairs
        /// relative to the <see cref="GeneratorSites"/> and <see cref="VoronoiVertices"/> arrays.
        /// </para><para>
        /// The complete Voronoi diagram is defined both by the <see cref="VoronoiVertices"/> and by
        /// the <b>VoronoiEdges</b> that connect the vertices. All coordinates are bounded by the
        /// current <see cref="ClippingBounds"/>.</para></remarks>

        public readonly VoronoiEdge[] VoronoiEdges;

        #endregion
        #region VoronoiRegions

        /// <summary>
        /// Gets the regions of the Voronoi diagram.</summary>
        /// <value>
        /// An <see cref="Array"/> which contains, for each index in <see cref="GeneratorSites"/>, 
        /// the vertices of the corresponding Voronoi region as an <see cref="Array"/> of <see
        /// cref="PointD"/> coordinates.</value>
        /// <remarks><para>
        /// The <b>VoronoiRegions</b> are calculated when the property is first accessed, and then
        /// cached for repeated access. All coordinates are bounded by the current <see
        /// cref="ClippingBounds"/>.
        /// </para><para>
        /// The <see cref="PointD"/> array for each generator site contains the vertices of a convex
        /// polygon. The last vertex is implicitly assumed to be connected with the first vertex.
        /// Most vertices also appear in <see cref="VoronoiVertices"/>, except for the four corners
        /// of the <see cref="ClippingBounds"/> which terminate the outermost regions.
        /// </para></remarks>

        public PointD[][] VoronoiRegions {
            get {
                if (_voronoiRegions == null)
                    CreateRegions();

                return _voronoiRegions;
            }
        }

        #endregion
        #region VoronoiVertices

        /// <summary>
        /// The vertex list for the Voronoi diagram.</summary>
        /// <remarks><para>
        /// <b>VoronoiVertices</b> holds the <see cref="PointD"/> coordinates of all vertices in the
        /// Voronoi diagram.
        /// </para><para>
        /// The complete Voronoi diagram is defined both by the <b>VoronoiVertices</b> and by the
        /// <see cref="VoronoiEdges"/> that connect the vertices. All coordinates are bounded by the
        /// current <see cref="ClippingBounds"/>.</para></remarks>

        public readonly PointD[] VoronoiVertices;

        #endregion
        #region ClearVoronoiRegions

        /// <summary>
        /// Clears the <see cref="VoronoiRegions"/> property.</summary>
        /// <remarks><para>
        /// <b>ClearVoronoiRegions</b> resets the <see cref="VoronoiRegions"/> property to a null
        /// reference, causing its value to be recalculated when the property is next accessed.
        /// </para><para>
        /// Call this method to reduce memory consumption when the <see cref="VoronoiRegions"/> are
        /// no longer required, e.g. after executing <see cref="ToVoronoiSubdivision"/>.
        /// </para></remarks>

        public void ClearVoronoiRegions() {
            _voronoiRegions = null;
        }

        #endregion
        #region ClipDelaunayEdges

        /// <summary>
        /// Clips the edge list for the Delaunay triangulation to the specified bounds.</summary>
        /// <param name="bounds">
        /// A <see cref="RectD"/> that indicates the clipping bounds for all <see
        /// cref="DelaunayEdges"/>.</param>
        /// <returns>
        /// An <see cref="Array"/> containing all <see cref="DelaunayEdges"/> which intersect the
        /// specified <paramref name="bounds"/>, as defined below.</returns>
        /// <remarks><para>
        /// <b>ClipDelaunayEdges</b> returns all <see cref="DelaunayEdges"/> whose corresponding
        /// <see cref="VoronoiEdges"/> element fulfils two conditions:
        /// </para><list type="bullet"><item>
        /// <see cref="VoronoiEdge.Site1"/> and <see cref="VoronoiEdge.Site2"/> both fall within the
        /// specified <paramref name="bounds"/>.
        /// </item><item>
        /// The line segment indicated by <see cref="VoronoiEdge.Vertex1"/> and <see
        /// cref="VoronoiEdge.Vertex2"/> intersects the specified <paramref name="bounds"/>.
        /// </item></list><para>
        /// In other words, <b>ClipDelaunayEdges</b> selects those <see cref="DelaunayEdges"/> that
        /// fall entirely within <paramref name="bounds"/>, and which connect two <see
        /// cref="VoronoiRegions"/> that share a common border within <paramref name="bounds"/>.
        /// </para></remarks>

        public LineD[] ClipDelaunayEdges(RectD bounds) {
            List<LineD> delaunayEdges = new List<LineD>(VoronoiEdges.Length);

            foreach (VoronoiEdge edge in VoronoiEdges) {
                PointD s1 = GeneratorSites[edge.Site1];
                PointD s2 = GeneratorSites[edge.Site2];

                if (bounds.Contains(s1) && bounds.Contains(s2)) {
                    PointD v1 = VoronoiVertices[edge.Vertex1];
                    PointD v2 = VoronoiVertices[edge.Vertex2];

                    if (bounds.IntersectsWith(new LineD(v1, v2)))
                        delaunayEdges.Add(new LineD(s1, s2));
                }
            }

            return delaunayEdges.ToArray();
        }

        #endregion
        #region ToDelaunySubdivision(Boolean)

        /// <overloads>
        /// Converts all <see cref="DelaunayEdges"/> to a planar <see cref="Subdivision"/>.
        /// </overloads>
        /// <summary>
        /// Converts all <see cref="DelaunayEdges"/> to a planar <see cref="Subdivision"/>, using
        /// the default <see cref="ClippingBounds"/>.</summary>
        /// <param name="addRegions">
        /// <c>true</c> to add all <see cref="VoronoiRegions"/> with the corresponding <see
        /// cref="GeneratorSites"/> to the <see cref="Subdivision.VertexRegions"/> collection of the
        /// new <see cref="Subdivision"/>; otherwise, <c>false</c>.</param>
        /// <returns>
        /// A new <see cref="Subdivision"/> whose <see cref="Subdivision.Edges"/> correspond to the
        /// <see cref="DelaunayEdges"/> of the <see cref="VoronoiResults"/>.</returns>

        public Subdivision ToDelaunySubdivision(bool addRegions = false) {
            Subdivision division = Subdivision.FromLines(DelaunayEdges);

            if (addRegions)
                for (int i = 0; i < GeneratorSites.Length; i++)
                    division.VertexRegions.Add(GeneratorSites[i], VoronoiRegions[i]);

            return division;
        }

        #endregion
        #region ToDelaunySubdivision(RectD, Boolean)

        /// <summary>
        /// Converts all <see cref="DelaunayEdges"/> to a planar <see cref="Subdivision"/>, using
        /// the specified clipping bounds.</summary>
        /// <param name="bounds">
        /// A <see cref="RectD"/> that indicates the clipping bounds for all <see
        /// cref="DelaunayEdges"/>.</param>
        /// <param name="addRegions">
        /// <c>true</c> to add all <see cref="VoronoiRegions"/> with the corresponding <see
        /// cref="GeneratorSites"/> to the <see cref="Subdivision.VertexRegions"/> collection of the
        /// new <see cref="Subdivision"/>; otherwise, <c>false</c>.</param>
        /// <returns>
        /// A new <see cref="Subdivision"/> whose <see cref="Subdivision.Edges"/> correspond to the
        /// <see cref="DelaunayEdges"/> of the <see cref="VoronoiResults"/>.</returns>
        /// <remarks><para>
        /// The specified <paramref name="bounds"/> determine the subset of <see
        /// cref="GeneratorSites"/> and <see cref="DelaunayEdges"/> that is stored in the new <see
        /// cref="Subdivision"/>, as described in <see cref="ClipDelaunayEdges"/>.
        /// </para><para>
        /// If <paramref name="addRegions"/> is <c>true</c>, the polygons added to the <see
        /// cref="Subdivision.VertexRegions"/> collection are also clipped the specified <paramref
        /// name="bounds"/>.</para></remarks>

        public Subdivision ToDelaunySubdivision(RectD bounds, bool addRegions = false) {

            LineD[] edges = ClipDelaunayEdges(bounds);
            Subdivision division = Subdivision.FromLines(edges);

            if (addRegions)
                for (int i = 0; i < GeneratorSites.Length; i++) {
                    PointD site = GeneratorSites[i];
                    if (!bounds.Contains(site)) continue;

                    PointD[] region;
                    if (bounds.Intersect(VoronoiRegions[i], out region))
                        division.VertexRegions.Add(site, region);
                }

            return division;
        }

        #endregion
        #region ToVoronoiSubdivision

        /// <summary>
        /// Converts all <see cref="VoronoiRegions"/> to a planar <see cref="Subdivision"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="SubdivisionMap"/> containing a new <see cref="Subdivision"/> whose bounded
        /// <see cref="Subdivision.Faces"/> correspond to the <see cref="VoronoiRegions"/> of the
        /// <see cref="VoronoiResults"/>.</returns>
        /// <remarks><para>
        /// The returned <see cref="SubdivisionMap"/> also provides a mapping between the zero-based
        /// indices of all <see cref="GeneratorSites"/> and <see cref="VoronoiRegions"/> and the
        /// corresponding <see cref="Subdivision.Faces"/>. This mapping will become invalid as soon
        /// as the created <see cref="Subdivision"/> is changed.
        /// </para><para>
        /// <b>ToVoronoiSubdivision</b> creates all <see cref="VoronoiRegions"/> if they have not
        /// yet been calculated. Call <see cref="ClearVoronoiRegions"/> if you no longer require
        /// this property after <b>ToVoronoiSubdivision</b> has returned.</para></remarks>

        public SubdivisionMap ToVoronoiSubdivision() {
            var division = Subdivision.FromPolygons(VoronoiRegions);

            int regionCount = VoronoiRegions.Length;
            Debug.Assert(GeneratorSites.Length == regionCount);
            Debug.Assert(division.Faces.Count == regionCount + 1);

            var faceToSite = new int[regionCount];
            var siteToFace = new SubdivisionFace[regionCount];

            // determine equivalence of faces and VoronoiRegions indices
            // (which are in turn equivalent to GeneratorSites indices)
            for (int i = 0; i < VoronoiRegions.Length; i++) {
                PointD[] polygon = VoronoiRegions[i];
                SubdivisionFace face = division.FindFace(polygon);

                // bounded faces start at creation index one
                faceToSite[face._key - 1] = i;
                siteToFace[i] = face;
            }

            return new SubdivisionMap(division, this, faceToSite, siteToFace);
        }

        #endregion
        #region Private Methods
        #region CloseCornerRegion

        /// <summary>
        /// Closes the specified Voronoi region by adding pseudo-edges across one or two <see
        /// cref="ClippingBounds"/> corner between the specified Voronoi vertices.</summary>
        /// <param name="region">
        /// A <see cref="LinkedList{PointI}"/> containing the known Voronoi vertex indices of the
        /// Voronoi region to close.</param>
        /// <param name="v1">
        /// The <see cref="PointD"/> coordinates of the first known vertex in the specified
        /// <paramref name="region"/>.</param>
        /// <param name="v2">
        /// The <see cref="PointD"/> coordinates of the last known vertex in the specified <paramref
        /// name="region"/>. Both components must be different from <paramref name="v1"/>.</param>
        /// <remarks>
        /// <b>CloseCornerRegion</b> adds one or two <see cref="PointI"/> pseudo-edges beginning
        /// with <see cref="CornerIndex"/> values (other than <see cref="CornerIndex.None"/>) to the
        /// specified <paramref name="region"/>.</remarks>

        private void CloseCornerRegion(LinkedList<PointI> region, PointD v1, PointD v2) {
            Debug.Assert(v1.X != v2.X && v1.Y != v2.Y);

            if (v2.X == ClippingBounds.Left || v2.X == ClippingBounds.Right) {

                if (v1.Y == ClippingBounds.Top || v1.Y == ClippingBounds.Bottom) {
                    // p is on adjacent border, spanning one corner
                    PointD corner = new PointD(v2.X, v1.Y);
                    region.AddLast(CreateCornerEdge(corner));
                } else {
                    // p is on opposite vertical border, spanning two corners
                    // region opens to top or bottom clipping border
                    PointD corner = new PointD(v2.X, FindHorizontalBorder(v1, v2));
                    PointD secondCorner = new PointD(v1.X, corner.Y);

                    region.AddLast(CreateCornerEdge(corner));
                    region.AddLast(CreateCornerEdge(secondCorner));
                }
            } else {
                Debug.Assert(v2.Y == ClippingBounds.Top || v2.Y == ClippingBounds.Bottom);

                if (v1.X == ClippingBounds.Left || v1.X == ClippingBounds.Right) {
                    // p is on adjacent border, spanning one corner
                    PointD corner = new PointD(v1.X, v2.Y);
                    region.AddLast(CreateCornerEdge(corner));
                } else {
                    // p is on opposite horizontal border, spanning two corners
                    // region opens to left or right clipping border
                    PointD corner = new PointD(FindVerticalBorder(v1, v2), v2.Y);
                    PointD secondCorner = new PointD(corner.X, v1.Y);

                    region.AddLast(CreateCornerEdge(corner));
                    region.AddLast(CreateCornerEdge(secondCorner));
                }
            }
        }

        #endregion
        #region ConnectCandidates

        /// <summary>
        /// Connects the specified candidate edges to the specified Voronoi region by inserting
        /// edges that span a border or corner of the <see cref="ClippingBounds"/>.</summary>
        /// <param name="candidates">
        /// A <see cref="LinkedList{PointI}"/> containing the candidate edges that must be connected
        /// to the specified <paramref name="region"/>.</param>
        /// <param name="region">
        /// A <see cref="LinkedList{PointI}"/> containing the sorted and connected edges of the
        /// Voronoi region. The first and last vertex must touch the <see cref="ClippingBounds"/>.
        /// </param>
        /// <remarks>
        /// <b>ConnectCandidates</b> adds one or two edges at the beginning of the specified
        /// <paramref name="candidates"/> list. If two edges are added, both will contain one
        /// pseudo-vertex represented by a <see cref="CornerIndex"/> value.</remarks>

        private void ConnectCandidates(LinkedList<PointI> candidates, LinkedList<PointI> region) {

            int firstIndex = region.First.Value.X;
            int lastIndex = region.Last.Value.Y;

            PointD firstVertex = GetVertex(firstIndex);
            PointD lastVertex = GetVertex(lastIndex);

            Debug.Assert(IsAtClippingBounds(firstVertex));
            Debug.Assert(IsAtClippingBounds(lastVertex));

            // outer vertex indices of connecting edge(s)
            int connect1 = -1, connect2 = -1;

            foreach (PointI candidate in candidates) {
                connect1 = candidate.X;
                PointD connectVertex = GetVertex(connect1);

                if (MeetAtClippingBounds(connectVertex, firstVertex)) {
                    connect2 = firstIndex; break;
                }
                if (MeetAtClippingBounds(connectVertex, lastVertex)) {
                    connect2 = lastIndex; break;
                }

                connect1 = candidate.Y;
                connectVertex = GetVertex(connect1);

                if (MeetAtClippingBounds(connectVertex, firstVertex)) {
                    connect2 = firstIndex; break;
                }
                if (MeetAtClippingBounds(connectVertex, lastVertex)) {
                    connect2 = lastIndex; break;
                }
            }

            if (connect2 >= 0) {
                // add connecting edge on same clipping border
                candidates.AddFirst(new PointI(connect1, connect2));
                return;
            }

            connect1 = -1; connect2 = -1;
            CornerIndex corner = CornerIndex.None;

            foreach (PointI candidate in candidates) {
                connect1 = candidate.X;
                PointD connectVertex = GetVertex(connect1);

                corner = GetCornerIndex(connectVertex, firstVertex);
                if (corner != CornerIndex.None) {
                    connect2 = firstIndex; break;
                }
                corner = GetCornerIndex(connectVertex, lastVertex);
                if (corner != CornerIndex.None) {
                    connect2 = lastIndex; break;
                }

                connect1 = candidate.Y;
                connectVertex = GetVertex(connect1);

                corner = GetCornerIndex(connectVertex, firstVertex);
                if (corner != CornerIndex.None) {
                    connect2 = firstIndex; break;
                }
                corner = GetCornerIndex(connectVertex, lastVertex);
                if (corner != CornerIndex.None) {
                    connect2 = lastIndex; break;
                }
            }

            // add connecting edges across clipping corner
            Debug.Assert(connect2 >= 0);
            candidates.AddFirst(new PointI((int) corner, connect2));
            candidates.AddFirst(new PointI(connect1, (int) corner));
        }

        #endregion
        #region CreateCornerEdge

        /// <summary>
        /// Creates a pseudo-edge that begins with the <see cref="CornerIndex"/> corresponding to
        /// the specified <see cref="PointD"/> coordinates.</summary>
        /// <param name="p">
        /// A <see cref="PointD"/> that coincides with one of the four corners of the <see
        /// cref="ClippingBounds"/>.</param>
        /// <returns>
        /// A <see cref="PointI"/> whose <see cref="PointI.X"/> component contains the result of
        /// <see cref="GetCornerIndex"/> for <paramref name="p"/>, and whose <see cref="PointI.Y"/>
        /// component equals <see cref="Int32.MinValue"/>.</returns>
        /// <remarks>
        /// The <see cref="PointI.Y"/> component of the returned pseudo-edge is deliberately invalid
        /// because <b>CreateCornerEdge</b> is only used in the last stage of <see
        /// cref="CreateRegions"/> when <see cref="PointI.Y"/> components are ignored.</remarks>

        private PointI CreateCornerEdge(PointD p) {
            int index = (int) GetCornerIndex(p);
            Debug.Assert(index != (int) CornerIndex.None);
            return new PointI((int) index, Int32.MinValue);
        }

        #endregion
        #region CreateRegions

        /// <summary>
        /// Creates the regions of the Voronoi diagram.</summary>
        /// <remarks>
        /// <b>CreateRegions</b> stores its output in the <see cref="VoronoiRegions"/> property.
        /// Please see there for details.</remarks>

        private void CreateRegions() {
            /*
             * 1. Create list of unsorted edges for each region
             * ================================================
             * First we accumulate the raw material for each Voronoi region.
             * All edges are stored as indices into VoronoiVertices here.
             */

            var listRegions = new LinkedList<PointI>[GeneratorSites.Length];
            for (int i = 0; i < listRegions.Length; i++)
                listRegions[i] = new LinkedList<PointI>();

            foreach (VoronoiEdge edge in VoronoiEdges) {
                PointI vertex = new PointI(edge.Vertex1, edge.Vertex2);
                listRegions[edge.Site1].AddLast(vertex);
                listRegions[edge.Site2].AddLast(vertex);
            }

            /*
             * 2. Sort and complete list of edges for each region
             * ==================================================
             * Sort each edge list so that an edge’s second vertex index equals
             * the first vertex index of the subsequent edge in the region list.
             * We may need to swap an edge’s vertex indices to allow this connection.
             * 
             * Because all Voronoi edges are terminated with a pseudo-vertex where they
             * intersect the clipping rectangle, a Voronoi region may appear as several
             * internally connected, but mutually disconnected series of edges. To get
             * a single connected list, we must then insert pseudo-edges that span either
             * a border or even a corner of the clipping rectangle.
             */

            for (int i = 0; i < listRegions.Length; i++) {
                var candidates = listRegions[i];
                var listRegion = new LinkedList<PointI>();

                // start with first unsorted edge
                listRegion.AddFirst(candidates.First.Value);
                candidates.RemoveFirst();

                var candidate = candidates.First;
                bool wasEdgeAdded = false;

                while (candidate != null) {
                    // save next candidate before removing current one
                    var nextCandidate = candidate.Next;

                    PointI vertex = candidate.Value;
                    for (var node = listRegion.First; node != null; node = node.Next) {
                        Debug.Assert(vertex != node.Value); // all vertices are distinct

                        // invert edges with out-of-order indices
                        if (vertex.X == node.Value.X || vertex.Y == node.Value.Y)
                            vertex = new PointI(vertex.Y, vertex.X);

                        // move preceding edge to sorted list
                        if (vertex.Y == node.Value.X) {
                            candidates.Remove(candidate);
                            listRegion.AddBefore(node, vertex);
                            wasEdgeAdded = true;
                            break;
                        }

                        // move succeeding edge to sorted list
                        if (node.Value.Y == vertex.X) {
                            candidates.Remove(candidate);
                            listRegion.AddAfter(node, vertex);
                            wasEdgeAdded = true;
                            break;
                        }
                    }

                    // try next unsorted edge
                    candidate = nextCandidate;
                    if (candidate == null && candidates.Count > 0) {

                        // connection across border or corner required
                        if (!wasEdgeAdded)
                            ConnectCandidates(candidates, listRegion);

                        // start over with first candidate node
                        candidate = candidates.First;
                        wasEdgeAdded = false;
                    }
                }

                // replace unsorted with sorted list
                listRegions[i] = listRegion;
            }

            /*
             * 3. Transform index list into polygon for each region
             * ====================================================
             * We now have sorted lists containing all edges of each Voronoi region.
             * For closed (interior) regions, the last edge connects to the first,
             * and we store exactly one vertex per edge (we always choose the first).
             * 
             * For open (exterior) regions, the first edge begins and the last edge ends
             * with two different pseudo-vertices. We must now close them in the same way
             * in which we connected separate sublists in step 2.
             * 
             * That is, we add one or more pseudo-edges that connect the outer vertices
             * of the list across a border or corner of the clipping rectangle. Unlike
             * step 2, we may need to extend the connection across two corners.
             */

            _voronoiRegions = new PointD[GeneratorSites.Length][];

            for (int i = 0; i < listRegions.Length; i++) {
                LinkedList<PointI> listRegion = listRegions[i];

                int firstIndex = listRegion.First.Value.X;
                int lastIndex = listRegion.Last.Value.Y;

                if (firstIndex != lastIndex) {
                    // extend region to last pseudo-vertex
                    listRegion.AddLast(new PointI(lastIndex, Int32.MinValue));

                    PointD firstVertex = GetVertex(firstIndex);
                    PointD lastVertex = GetVertex(lastIndex);

                    // check if pseudo-vertices span one or two corners of clipping region
                    if (firstVertex.X != lastVertex.X && firstVertex.Y != lastVertex.Y)
                        CloseCornerRegion(listRegion, firstVertex, lastVertex);
                }

                PointD[] region = new PointD[listRegion.Count];
                _voronoiRegions[i] = region;

                // store coordinates for first vertex of each edge
                int j = 0;
                for (var edge = listRegion.First; edge != null; edge = edge.Next, j++)
                    region[j] = GetVertex(edge.Value.X);
            }
        }

        #endregion
        #region FindHorizontalBorder

        /// <summary>
        /// Finds the horizontal border of the <see cref="ClippingBounds"/> towards which the
        /// Voronoi region containing the specified vertical border coordinates is open.</summary>
        /// <param name="p">
        /// The first <see cref="PointD"/> to examine.</param>
        /// <param name="q">
        /// The second <see cref="PointD"/> to examine.</param>
        /// <returns>
        /// The <see cref="RectD.Top"/> or <see cref="RectD.Bottom"/> border of the <see
        /// cref="ClippingBounds"/>, depending on the specified <paramref name="p"/> and <paramref
        /// name="q"/>.</returns>
        /// <remarks>
        /// The specified <paramref name="p"/> and <paramref name="q"/> must lie on opposite
        /// vertical borders of the <see cref="ClippingBounds"/>, and their Voronoi region must open
        /// to one of the horizontal borders, including both adjacent corners.</remarks>

        private double FindHorizontalBorder(PointD p, PointD q) {

            // line from left to right of clipping bounds
            LineD line = (p.X < q.X) ? new LineD(p, q) : new LineD(q, p);
            Debug.Assert(line.Start.X == ClippingBounds.Left);
            Debug.Assert(line.End.X == ClippingBounds.Right);

            // check for vertex on either side of line
            // (LineLocation assumes y-coordinates increase upward!)
            foreach (PointD vertex in VoronoiVertices)
                switch (line.Locate(vertex)) {
                    case LineLocation.Left: return ClippingBounds.Top;
                    case LineLocation.Right: return ClippingBounds.Bottom;
                }

            Debug.Fail("Cannot identify open side of Voronoi region.");
            return ClippingBounds.Top;
        }

        #endregion
        #region FindVerticalBorder

        /// <summary>
        /// Finds the vertical border of the <see cref="ClippingBounds"/> towards which the Voronoi
        /// region containing the specified horizontal border coordinates is open.</summary>
        /// <param name="p">
        /// The first <see cref="PointD"/> to examine.</param>
        /// <param name="q">
        /// The second <see cref="PointD"/> to examine.</param>
        /// <returns>
        /// The <see cref="RectD.Left"/> or <see cref="RectD.Right"/> border of the <see
        /// cref="ClippingBounds"/>, depending on the specified <paramref name="p"/> and <paramref
        /// name="q"/>.</returns>
        /// <remarks>
        /// The specified <paramref name="p"/> and <paramref name="q"/> must lie on opposite
        /// horizontal borders of the <see cref="ClippingBounds"/>, and their Voronoi region must
        /// open to one of the vertical borders, including both adjacent corners.</remarks>

        private double FindVerticalBorder(PointD p, PointD q) {

            // line from top to bottom of clipping bounds
            LineD line = (p.Y < q.Y) ? new LineD(p, q) : new LineD(q, p);
            Debug.Assert(line.Start.Y == ClippingBounds.Top);
            Debug.Assert(line.End.Y == ClippingBounds.Bottom);

            // check for vertex on either side of line
            // (LineLocation assumes y-coordinates increase upward!)
            foreach (PointD vertex in VoronoiVertices)
                switch (line.Locate(vertex)) {
                    case LineLocation.Left: return ClippingBounds.Right;
                    case LineLocation.Right: return ClippingBounds.Left;
                }

            Debug.Fail("Cannot identify open side of Voronoi region.");
            return ClippingBounds.Left;
        }

        #endregion
        #region GetCornerIndex(PointD)

        /// <overloads>
        /// Gets the index of the corner of the <see cref="ClippingBounds"/> that corresponds to the
        /// specified coordinates.</overloads>
        /// <summary>
        /// Gets the index of the corner of the <see cref="ClippingBounds"/> that coincides with the
        /// specified <see cref="PointD"/> coordinates.</summary>
        /// <param name="p">
        /// The <see cref="PointD"/> to examine.</param>
        /// <returns><para>
        /// A <see cref="CornerIndex"/> value indicating the corner of the <see
        /// cref="ClippingBounds"/> that coincides with <paramref name="p"/>.
        /// </para><para>-or-</para><para>
        /// <see cref="CornerIndex.None"/> if <paramref name="p"/> does not coincide with any corner
        /// of the <see cref="ClippingBounds"/>.</para></returns>

        private CornerIndex GetCornerIndex(PointD p) {

            if (p.X == ClippingBounds.Left) {
                if (p.Y == ClippingBounds.Top) return CornerIndex.TopLeft;
                if (p.Y == ClippingBounds.Bottom) return CornerIndex.BottomLeft;
            }
            else if (p.X == ClippingBounds.Right) {
                if (p.Y == ClippingBounds.Top) return CornerIndex.TopRight;
                if (p.Y == ClippingBounds.Bottom) return CornerIndex.BottomRight;
            }

            return CornerIndex.None;
        }

        #endregion
        #region GetCornerIndex(PointD, PointD)

        /// <summary>
        /// Gets the index of the corner of the <see cref="ClippingBounds"/> that separates the
        /// specified two <see cref="PointD"/> coordinates.</summary>
        /// <param name="p">
        /// The first <see cref="PointD"/> to examine.</param>
        /// <param name="q">
        /// The second <see cref="PointD"/> to examine.</param>
        /// <returns><para>
        /// A <see cref="CornerIndex"/> value indicating the corner of the <see
        /// cref="ClippingBounds"/> that separates <paramref name="p"/> and <paramref name="q"/>.
        /// </para><para>-or-</para><para>
        /// <see cref="CornerIndex.None"/> if <paramref name="p"/> and <paramref name="q"/> do not
        /// lie on adjacent borders of the <see cref="ClippingBounds"/>.</para></returns>

        private CornerIndex GetCornerIndex(PointD p, PointD q) {

            if (p.X == ClippingBounds.Left) {
                if (q.Y == ClippingBounds.Top) return CornerIndex.TopLeft;
                if (q.Y == ClippingBounds.Bottom) return CornerIndex.BottomLeft;
            }
            else if (p.X == ClippingBounds.Right) {
                if (q.Y == ClippingBounds.Top) return CornerIndex.TopRight;
                if (q.Y == ClippingBounds.Bottom) return CornerIndex.BottomRight;
            }
            else if (p.Y == ClippingBounds.Top) {
                if (q.X == ClippingBounds.Left) return CornerIndex.TopLeft;
                if (q.X == ClippingBounds.Right) return CornerIndex.TopRight;
            }
            else if (p.Y == ClippingBounds.Bottom) {
                if (q.X == ClippingBounds.Left) return CornerIndex.BottomLeft;
                if (q.X == ClippingBounds.Right) return CornerIndex.BottomRight;
            }

            return CornerIndex.None;
        }

        #endregion
        #region GetVertex

        /// <summary>
        /// Gets the Voronoi vertex with the specified index.</summary>
        /// <param name="index">
        /// The index of the Voronoi vertex to retrieve. This may be either a <see
        /// cref="VoronoiVertices"/> index or a <see cref="CornerIndex"/> value.</param>
        /// <returns>
        /// One of the four corners of the <see cref="ClippingBounds"/> if <paramref name="index"/>
        /// equals a <see cref="CornerIndex"/> value other than <see cref="CornerIndex.None"/>;
        /// otherwise, the <see cref="VoronoiVertices"/> element with the specified <paramref
        /// name="index"/>.</returns>

        private PointD GetVertex(int index) {
            switch (index) {

                case (int) CornerIndex.TopLeft:     return ClippingBounds.TopLeft;
                case (int) CornerIndex.TopRight:    return ClippingBounds.TopRight;
                case (int) CornerIndex.BottomLeft:  return ClippingBounds.BottomLeft;
                case (int) CornerIndex.BottomRight: return ClippingBounds.BottomRight;

                default: return VoronoiVertices[index];
            }
        }

        #endregion
        #region IsAtClippingBounds

        /// <summary>
        /// Determines whether the specified <see cref="PointD"/> coordinates touch the borders of
        /// the <see cref="ClippingBounds"/>.</summary>
        /// <param name="p">
        /// The <see cref="PointD"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="PointD.X"/> component of <paramref name="p"/> equals the
        /// <see cref="RectD.Left"/> or <see cref="RectD.Right"/> border of the <see
        /// cref="ClippingBounds"/>, or if the <see cref="PointD.Y"/> component equals the <see
        /// cref="RectD.Top"/> or <see cref="RectD.Bottom"/> border; otherwise, <c>false</c>.
        /// </returns>

        private bool IsAtClippingBounds(PointD p) {

            return (p.X == ClippingBounds.Left ||
                p.X == ClippingBounds.Right ||
                p.Y == ClippingBounds.Top ||
                p.Y == ClippingBounds.Bottom);
        }

        #endregion
        #region MeetAtClippingBounds

        /// <summary>
        /// Determines whether the specified two <see cref="PointD"/> coordinates lie on the same
        /// border of the <see cref="ClippingBounds"/>.</summary>
        /// <param name="p">
        /// The first <see cref="PointD"/> to examine.</param>
        /// <param name="q">
        /// The second <see cref="PointD"/> to examine.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="PointD.X"/> components of <paramref name="p"/> and
        /// <paramref name="q"/> both equal the <see cref="RectD.Left"/> or <see
        /// cref="RectD.Right"/> border of the <see cref="ClippingBounds"/>, or if their <see
        /// cref="PointD.Y"/> components both equal the <see cref="RectD.Top"/> or <see
        /// cref="RectD.Bottom"/> border; otherwise, <c>false</c>.</returns>

        private bool MeetAtClippingBounds(PointD p, PointD q) {

            return ((p.X == q.X && (p.X == ClippingBounds.Left || p.X == ClippingBounds.Right))
                || (p.Y == q.Y && (p.Y == ClippingBounds.Top || p.Y == ClippingBounds.Bottom)));
        }

        #endregion
        #endregion
        #region Enum CornerIndex

        /// <summary>
        /// Specifies the four corners of the <see cref="ClippingBounds"/>.</summary>
        /// <remarks>
        /// The four corners of the <see cref="ClippingBounds"/> rectangle appear as vertices of the
        /// <see cref="VoronoiRegions"/> but do not have corresponding indices in the <see
        /// cref="VoronoiVertices"/> array. <b>CornerIndex</b> therefore provides negative
        /// pseudo-indices for these four vertices that are accepted by <see cref="GetVertex"/>.
        /// </remarks>

        private enum CornerIndex {

            /// <summary>
            /// Specifies none of the four corners.</summary>
            None = 0,

            /// <summary>
            /// Specifies the <see cref="RectD.TopLeft"/> corner.</summary>
            TopLeft = -1,

            /// <summary>
            /// Specifies the <see cref="RectD.TopRight"/> corner.</summary>
            TopRight = -2,

            /// <summary>
            /// Specifies the <see cref="RectD.BottomLeft"/> corner.</summary>
            BottomLeft = -3,

            /// <summary>
            /// Specifies the <see cref="RectD.BottomRight"/> corner.</summary>
            BottomRight = -4
        }

        #endregion
        #region Class SubdivisionMap

        /// <summary>
        /// Maps the faces of a planar <see cref="Subdivision"/> to <see cref="GeneratorSites"/>
        /// indices.</summary>
        /// <remarks><para>
        /// <b>SubdivisionMap</b> provides a mapping between all faces of a planar <see
        /// cref="Subdivision"/> and the zero-based indices of the <see cref="GeneratorSites"/> of
        /// the <see cref="VoronoiResults"/> from which the <see cref="Subdivision"/> was created.
        /// </para><para>
        /// The mapping is realized by a pair of arrays for optimal runtime efficiency. However,
        /// <b>SubdivisionMap</b> will not reflect changes to the underlying <see
        /// cref="Subdivision"/>.</para></remarks>

        [Serializable]
        public class SubdivisionMap: ISubdivisionMap<Int32> {
            #region SubdivisionMap(...)

            /// <summary>
            /// Initializes a new instance of the <see cref="SubdivisionMap"/> class.</summary>
            /// <param name="source">
            /// The <see cref="Subdivision"/> that contains all mapped faces.</param>
            /// <param name="target">
            /// The <see cref="VoronoiResults"/> that contain all mapped indices.</param>
            /// <param name="faceToSite">
            /// A one-dimensional <see cref="Array"/> that maps <see cref="SubdivisionFace"/> keys
            /// to <see cref="GeneratorSites"/> indices.</param>
            /// <param name="siteToFace">
            /// A two-dimensional <see cref="Array"/> that maps <see cref="GeneratorSites"/> indices
            /// to <see cref="SubdivisionFace"/> objects.</param>

            internal SubdivisionMap(Subdivision source, VoronoiResults target,
                int[] faceToSite, SubdivisionFace[] siteToFace) {

                Debug.Assert(source != null);
                Debug.Assert(target != null);

                Debug.Assert(faceToSite != null);
                Debug.Assert(siteToFace != null);
                Debug.Assert(faceToSite.Length == siteToFace.Length);

                _source = source;
                _target = target;
                _faceToSite = faceToSite;
                _siteToFace = siteToFace;
            }

            #endregion
            #region Private Fields

            // property backers
            private readonly Subdivision _source;
            private readonly VoronoiResults _target;

            // mapping arrays
            private readonly int[] _faceToSite;
            private readonly SubdivisionFace[] _siteToFace;

            #endregion
            #region Target

            /// <summary>
            /// Gets the <see cref="VoronoiResults"/> object that define all mapped <see
            /// cref="GeneratorSites"/> indices.</summary>
            /// <value>
            /// The <see cref="VoronoiResults"/> object that define all <see cref="GeneratorSites"/>
            /// indices returned and accepted by the <see cref="FromFace"/> and <see cref="ToFace"/>
            /// methods, respectively.</value>

            public VoronoiResults Target {
                get { return _target; }
            }

            /// <summary>
            /// Gets the <see cref="VoronoiResults"/> object that define all mapped <see
            /// cref="GeneratorSites"/> indices.</summary>
            /// <value>
            /// The <see cref="VoronoiResults"/> object that define all <see cref="GeneratorSites"/>
            /// indices returned and accepted by the <see cref="FromFace"/> and <see cref="ToFace"/>
            /// methods, respectively.</value>

            object ISubdivisionMap<Int32>.Target {
                get { return _target; }
            }

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
            #region FromFace

            /// <summary>
            /// Converts the specified <see cref="SubdivisionFace"/> into the associated <see
            /// cref="GeneratorSites"/> index.</summary>
            /// <param name="face">
            /// The <see cref="SubdivisionFace"/> to convert.</param>
            /// <returns>
            /// The zero-based <see cref="GeneratorSites"/> index associated with <paramref
            /// name="face"/>.</returns>
            /// <exception cref="IndexOutOfRangeException">
            /// <paramref name="face"/> contains a <see cref="SubdivisionFace.Key"/> that is less
            /// than one or greater than the number of <see cref="GeneratorSites"/>.</exception>
            /// <exception cref="NullReferenceException">
            /// <paramref name="face"/> is a null reference.</exception>

            public int FromFace(SubdivisionFace face) {
                return _faceToSite[face._key - 1];
            }

            #endregion
            #region ToFace

            /// <summary>
            /// Converts the specified <see cref="GeneratorSites"/> index into the associated <see
            /// cref="SubdivisionFace"/>.</summary>
            /// <param name="value">
            /// The zero-based <see cref="GeneratorSites"/> index to convert.</param>
            /// <returns>
            /// The <see cref="SubdivisionFace"/> associated with <paramref name="value"/>.
            /// </returns>
            /// <exception cref="IndexOutOfRangeException">
            /// <paramref name="value"/> is not a valid <see cref="GeneratorSites"/> index.
            /// </exception>

            public SubdivisionFace ToFace(int value) {
                return _siteToFace[value];
            }

            #endregion
        }

        #endregion
    }
}
