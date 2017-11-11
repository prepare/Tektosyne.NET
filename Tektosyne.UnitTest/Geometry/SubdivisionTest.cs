using System;
using System.Collections.Generic;

using NUnit.Framework;
using Tektosyne.Geometry;
using Tektosyne.Graph;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class SubdivisionTest {

        [Test]
        public void IGraph2DTest() {
            PointD[] points = {
                new PointD(0, 0), new PointD(-1, -2),
                new PointD(-1, 2), new PointD(1, 2), new PointD(1, -2)
            };
            LineD[] lines = {
                new LineD(points[0], points[1]), new LineD(points[0], points[2]),
                new LineD(points[0], points[3]), new LineD(points[4], points[0])
            };
            var division = Subdivision.FromLines(lines);
            division.Validate();
            var graph = division as IGraph2D<PointD>;

            Assert.AreEqual(4, graph.Connectivity);
            Assert.AreEqual(5, graph.NodeCount);
            foreach (PointD node in graph.Nodes)
                CollectionAssert.Contains(points, node);

            foreach (PointD point in points) {
                Assert.IsTrue(graph.Contains(point));
                Assert.AreEqual(point, graph.GetWorldLocation(point));

                PointD near = new PointD(point.X + 0.1, point.Y - 0.1);
                Assert.IsFalse(graph.Contains(near));
                Assert.AreEqual(point, graph.GetNearestNode(near));
            }

            var center = new[] { points[0] };
            var neighbors = new PointD[points.Length - 1];
            double distance = Math.Sqrt(5.0);

            for (int i = 1; i < points.Length; i++) {
                Assert.AreEqual(distance, graph.GetDistance(points[0], points[i]));
                CollectionAssert.AreEquivalent(center, graph.GetNeighbors(points[i]));
                neighbors[i - 1] = points[i];
            }

            CollectionAssert.AreEquivalent(neighbors, graph.GetNeighbors(points[0]));
        }

        [Test]
        public void FindEdge() {
            PointD[] points = {
                new PointD(0, 0), new PointD(-1, -2), 
                new PointD(-1, 2), new PointD(1, 2), new PointD(1, -2)
            };
            LineD[] lines = {
                new LineD(points[1], points[0]), new LineD(points[0], points[2]),
                new LineD(points[0], points[3]), new LineD(points[4], points[0])
            };
            var division = Subdivision.FromLines(lines);
            division.Validate();

            // check all existing half-edges
            foreach (SubdivisionEdge edge in division.Edges.Values)
                Assert.AreSame(edge, division.FindEdge(edge.Origin, edge.Destination));

            for (int i = 0; i < points.Length; i++) {
                // check half-edges including one non-existing vertex
                Assert.IsNull(division.FindEdge(points[i], new PointD(1, 0)));
                Assert.IsNull(division.FindEdge(new PointD(1, 0), points[i]));

                // check nonexistent half-edges between vertices
                if (i == 0) continue;
                for (int j = 1; j < points.Length; j++)
                    if (j != i) Assert.IsNull(division.FindEdge(points[i], points[j]));
            }
        }

        [Test]
        public void FindEdgeEpsilon() {
            PointD[] points = {
                new PointD(0, 0), new PointD(-1, -2), 
                new PointD(-1, 2), new PointD(1, 2), new PointD(1, -2)
            };
            LineD[] lines = {
                new LineD(points[1], points[0]), new LineD(points[0], points[2]),
                new LineD(points[0], points[3]), new LineD(points[4], points[0])
            };
            var division = Subdivision.FromLines(lines, 0.2);
            division.Validate();

            // check all existing half-edges
            PointD offset = new PointD(0.1, -0.1);
            foreach (SubdivisionEdge edge in division.Edges.Values) {
                Assert.AreSame(edge, division.FindEdge(edge.Origin, edge.Destination));
                Assert.AreSame(edge, division.FindEdge(
                    edge.Origin.Add(offset), edge.Destination.Add(offset)));
            }

            for (int i = 0; i < points.Length; i++) {
                // check half-edges including one non-existing vertex
                Assert.IsNull(division.FindEdge(points[i], new PointD(1, 0)));
                Assert.IsNull(division.FindEdge(new PointD(1, 0), points[i]));

                // check nonexistent half-edges between vertices
                if (i == 0) continue;
                for (int j = 1; j < points.Length; j++)
                    if (j != i) Assert.IsNull(division.FindEdge(points[i], points[j]));
            }
        }

        [Test]
        public void FindFacePoint() {
            var division = SubdivisionFromLines.CreateSquareStar(false);
            var faces = division.Faces.Values;

            Assert.AreEqual(faces[0], division.FindFace(new PointD(-2, 0)));
            Assert.AreEqual(faces[1], division.FindFace(new PointD(-0.5, 0)));
            Assert.AreEqual(faces[2], division.FindFace(new PointD(0, 0.5)));
            Assert.AreEqual(faces[3], division.FindFace(new PointD(0.5, 0)));
            Assert.AreEqual(faces[4], division.FindFace(new PointD(0, -0.5)));

            // four nested triangles
            PointD[][] polygons = {
                new[] { new PointD(-8, -8), new PointD(0, 8), new PointD(8, -8) },
                new[] { new PointD(-6, -6), new PointD(0, 6), new PointD(6, -6) },
                new[] { new PointD(-4, -4), new PointD(0, 4), new PointD(4, -4) },
                new[] { new PointD(-2, -2), new PointD(0, 2), new PointD(2, -2) }
            };
            division = Subdivision.FromPolygons(polygons);
            division.Validate();
            faces = division.Faces.Values;

            Assert.AreEqual(faces[0], division.FindFace(new PointD(0, 10)));
            Assert.AreEqual(faces[1], division.FindFace(new PointD(0, 7)));
            Assert.AreEqual(faces[2], division.FindFace(new PointD(0, 5)));
            Assert.AreEqual(faces[3], division.FindFace(new PointD(0, 3)));
            Assert.AreEqual(faces[4], division.FindFace(new PointD(0, 1)));
        }
        
        [Test]
        public void FindFacePolygon() {
            var polygon = new[] { new PointD(0, 0), new PointD(1, 1), new PointD(2, 0) };
            var division = Subdivision.FromPolygons(new PointD[][] { polygon });
            division.Validate();
            var face = division.Faces[1];

            // original sequence, any starting vertex
            Assert.AreSame(face, division.FindFace(new[] { polygon[0], polygon[1], polygon[2] }));
            Assert.AreSame(face, division.FindFace(new[] { polygon[1], polygon[2], polygon[0] }));
            Assert.AreSame(face, division.FindFace(new[] { polygon[2], polygon[0], polygon[1] }));

            // inverted sequence, any starting vertex
            Assert.AreSame(face, division.FindFace(new[] { polygon[2], polygon[1], polygon[0] }));
            Assert.AreSame(face, division.FindFace(new[] { polygon[0], polygon[2], polygon[1] }));
            Assert.AreSame(face, division.FindFace(new[] { polygon[1], polygon[0], polygon[2] }));

            // sequence including nonexistent point
            var point = new PointD(2, 1);
            Assert.IsNull(division.FindFace(new[] { point, polygon[1], polygon[2] }));
            Assert.IsNull(division.FindFace(new[] { polygon[0], point, polygon[2] }));
            Assert.IsNull(division.FindFace(new[] { polygon[0], polygon[1], point }));
        }

        [Test]
        public void FindNearestEdge() {
            var division = SubdivisionFromLines.CreateSquareStar(false);
            var edges = division.Edges.Values;
            var faces = division.Faces.Values;

            double distance;
            Assert.AreEqual(edges[0], division.FindNearestEdge(new PointD(-1.1, 0), out distance));
            Assert.AreEqual(edges[1], division.FindNearestEdge(new PointD(-0.9, 0), out distance));
            Assert.AreEqual(edges[2], division.FindNearestEdge(new PointD(0, 2.1), out distance));
            Assert.AreEqual(edges[3], division.FindNearestEdge(new PointD(0, 1.9), out distance));
            Assert.AreEqual(edges[4], division.FindNearestEdge(new PointD(0.9, 0), out distance));
            Assert.AreEqual(edges[5], division.FindNearestEdge(new PointD(1.1, 0), out distance));
            Assert.AreEqual(edges[6], division.FindNearestEdge(new PointD(0, -1.9), out distance));
            Assert.AreEqual(edges[7], division.FindNearestEdge(new PointD(0, -2.1), out distance));

            Assert.AreEqual(edges[ 8], division.FindNearestEdge(new PointD(-0.5, -1.1), out distance));
            Assert.AreEqual(edges[ 9], division.FindNearestEdge(new PointD(-0.5, -0.9), out distance));
            Assert.AreEqual(edges[10], division.FindNearestEdge(new PointD(-0.5, 0.9), out distance));
            Assert.AreEqual(edges[11], division.FindNearestEdge(new PointD(-0.5, 1.1), out distance));
            Assert.AreEqual(edges[12], division.FindNearestEdge(new PointD(0.5, 1.1), out distance));
            Assert.AreEqual(edges[13], division.FindNearestEdge(new PointD(0.5, 0.9), out distance));
            Assert.AreEqual(edges[14], division.FindNearestEdge(new PointD(0.5, -0.9), out distance));
            Assert.AreEqual(edges[15], division.FindNearestEdge(new PointD(0.5, -1.1), out distance));
        }

        [Test]
        public void LocateInFace() {
            var polygon = new[] { new PointD(0, 0), new PointD(1, 1), new PointD(2, 0) };
            var division = Subdivision.FromPolygons(new PointD[][] { polygon });
            division.Validate();

            var edge = division.Faces[1].OuterEdge;
            Assert.AreEqual(PolygonLocation.Inside, edge.Locate(new PointD(1.0, 0.5)));

            Assert.AreEqual(PolygonLocation.Outside, edge.Locate(new PointD(0.0, 0.5)));
            Assert.AreEqual(PolygonLocation.Outside, edge.Locate(new PointD(2.0, 0.5)));
            Assert.AreEqual(PolygonLocation.Outside, edge.Locate(new PointD(1.0, -0.5)));
            Assert.AreEqual(PolygonLocation.Outside, edge.Locate(new PointD(1.0, 2.5)));

            Assert.AreEqual(PolygonLocation.Edge, edge.Locate(new PointD(1.0, 0.0)));
            Assert.AreEqual(PolygonLocation.Edge, edge.Locate(new PointD(0.5, 0.5)));
            Assert.AreEqual(PolygonLocation.Edge, edge.Locate(new PointD(1.5, 0.5)));

            Assert.AreEqual(PolygonLocation.Vertex, edge.Locate(new PointD(0.0, 0.0)));
            Assert.AreEqual(PolygonLocation.Vertex, edge.Locate(new PointD(1.0, 1.0)));
            Assert.AreEqual(PolygonLocation.Vertex, edge.Locate(new PointD(2.0, 0.0)));
        }

        [Test]
        public void LocateInFaceEpsilon() {
            var polygon = new[] { new PointD(0, 0), new PointD(1, 1), new PointD(2, 0) };
            var division = Subdivision.FromPolygons(new PointD[][] { polygon });
            division.Validate();

            var edge = division.Faces[1].OuterEdge;
            Assert.AreEqual(PolygonLocation.Inside, edge.Locate(new PointD(1.0, 0.5), 0.2));

            Assert.AreEqual(PolygonLocation.Outside, edge.Locate(new PointD(1.0, -0.5), 0.2));
            Assert.AreEqual(PolygonLocation.Outside, edge.Locate(new PointD(0.0, 0.5), 0.2));
            Assert.AreEqual(PolygonLocation.Outside, edge.Locate(new PointD(2.0, 0.5), 0.2));

            Assert.AreEqual(PolygonLocation.Vertex, edge.Locate(new PointD(1.0, 0.9), 0.2));
            Assert.AreEqual(PolygonLocation.Vertex, edge.Locate(new PointD(0.0, 0.1), 0.2));
            Assert.AreEqual(PolygonLocation.Vertex, edge.Locate(new PointD(2.1, 0.0), 0.2));

            Assert.AreEqual(PolygonLocation.Edge, edge.Locate(new PointD(1.0, -0.1), 0.2));
            Assert.AreEqual(PolygonLocation.Edge, edge.Locate(new PointD(0.6, 0.5), 0.2));
            Assert.AreEqual(PolygonLocation.Edge, edge.Locate(new PointD(1.4, 0.5), 0.2));
        }

        [Test]
        public void StructureEquals() {
            var division = SubdivisionFromLines.CreateTriforce(false);
            division.Validate();

            // check overall structural equality
            var clone = (Subdivision) division.Clone();
            clone.Validate();
            Assert.IsTrue(division.StructureEquals(clone));

            // check individual edges
            Assert.AreEqual(division.Edges.Count, clone.Edges.Count);
            for (int i = 0; i < division.Edges.Count; i++) {
                var edge = division.Edges.GetByIndex(i);
                var cloneEdge = clone.Edges.GetByIndex(i);

                Assert.AreNotEqual(edge, cloneEdge);
                Assert.IsTrue(edge.StructureEquals(cloneEdge));
            }

            // check individual faces
            var faces = division.Faces.Values;
            var cloneFaces = clone.Faces.Values;
            Assert.AreEqual(faces.Count, cloneFaces.Count);

            for (int i = 0; i < faces.Count; i++) {
                Assert.AreNotEqual(faces[i], cloneFaces[i]);
                Assert.IsTrue(faces[i].StructureEquals(cloneFaces[i]));
            }
        }

        [Test]
        public void PolygonGrid() {
            var grid = new PolygonGrid(new RegularPolygon(10, 4, PolygonOrientation.OnEdge));
            grid.Size = new SizeI(6, 4);
            var division = grid.ToSubdivision(PointD.Empty);
            CheckGridDivision(division);

            grid = new PolygonGrid(new RegularPolygon(10, 4, PolygonOrientation.OnVertex));
            grid.Size = new SizeI(4, 6);
            division = grid.ToSubdivision(PointD.Empty);
            CheckGridDivision(division);

            grid = new PolygonGrid(new RegularPolygon(10, 6, PolygonOrientation.OnEdge));
            grid.Size = new SizeI(6, 4);
            division = grid.ToSubdivision(PointD.Empty);
            CheckGridDivision(division);

            grid = new PolygonGrid(new RegularPolygon(10, 6, PolygonOrientation.OnVertex));
            grid.Size = new SizeI(4, 6);
            division = grid.ToSubdivision(PointD.Empty);
            CheckGridDivision(division);
        }

        private static void CheckGridDivision(PolygonGrid.SubdivisionMap map) {
            map.Source.Validate();

            // test finding vertices with FindNearestVertex
            for (int i = 0; i < map.Source.Vertices.Count; i++) {
                PointD q = map.Source.Vertices.GetKey(i) + GeoAlgorithms.RandomPoint(-2, -2, 4, 4);
                Assert.AreEqual(i, map.Source.FindNearestVertex(q));
            }

            // test GetElementVertices and face mapping
            for (int x = 0; x < map.Target.Size.Width; x++)
                for (int y = 0; y < map.Target.Size.Height; y++) {

                    var polygon = map.Target.GetElementVertices(x, y);
                    var face = map.Source.FindFace(polygon, true);

                    Assert.AreSame(face, map.ToFace(new PointI(x, y)));
                    Assert.AreEqual(new PointI(x, y), map.FromFace(face));
                }
        }

        [Test]
        public void VoronoiTest() {
            int count = MersenneTwister.Default.Next(10, 100);
            var points = new PointD[count];
            for (int i = 0; i < points.Length; i++)
                points[i] = GeoAlgorithms.RandomPoint(-1000, -1000, 2000, 2000);

            var results = Voronoi.FindAll(points, new RectD(-1000, -1000, 2000, 2000));
            var delaunay = results.ToDelaunySubdivision();
            delaunay.Validate();
            var voronoi = results.ToVoronoiSubdivision();
            voronoi.Source.Validate();

            // compare original and subdivision’s Delaunay edges
            var delaunayEdges = delaunay.ToLines();
            Assert.AreEqual(results.DelaunayEdges.Length, delaunayEdges.Length);

            foreach (LineD edge in results.DelaunayEdges)
                if (PointDComparerY.CompareExact(edge.Start, edge.End) > 0)
                    Assert.Contains(edge.Reverse(), delaunayEdges);
                else
                    Assert.Contains(edge, delaunayEdges);

            // compare original and subdivision’s Voronoi regions
            var voronoiFaces = voronoi.Source.Faces;
            Assert.AreEqual(results.VoronoiRegions.Length, voronoiFaces.Count - 1);

            foreach (var face in voronoiFaces.Values) {
                if (face.OuterEdge == null) continue;
                int index = voronoi.FromFace(face);

                PointD[] polygon = results.VoronoiRegions[index];
                CollectionAssert.AreEquivalent(polygon, face.OuterEdge.CyclePolygon);

                PointD site = results.GeneratorSites[index];
                Assert.AreNotEqual(PolygonLocation.Outside, face.OuterEdge.Locate(site));
            }
        }
    }
}
