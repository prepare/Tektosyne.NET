using System;
using System.Collections.Generic;

using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class SubdivisionSearchTest {

        [Test]
        public void Empty() {
            var search = new SubdivisionSearch(new Subdivision());
            search.Validate();

            for (int i = 0; i < 10; i++) {
                PointD q = GeoAlgorithms.RandomPoint(-100, -100, 200, 200);
                Assert.IsTrue(search.Find(q).IsUnboundedFace);
                Assert.IsTrue(search.Source.Find(q).IsUnboundedFace);
            }
        }

        [Test]
        public void SingleEdgeX() {
            var search = CheckSearch(new LineD(-5, 0, +5, 0));

            Assert.IsTrue(search.Find(new PointD(0, -1)).IsUnboundedFace);
            Assert.IsTrue(search.Find(new PointD(0, +1)).IsUnboundedFace);
            Assert.IsTrue(search.Find(new PointD(-6, 0)).IsUnboundedFace);
            Assert.IsTrue(search.Find(new PointD(+6, 0)).IsUnboundedFace);
        }

        [Test]
        public void SingleEdgeY() {
            var search = CheckSearch(new LineD(0, -5, 0, +5));

            Assert.IsTrue(search.Find(new PointD(-1, 0)).IsUnboundedFace);
            Assert.IsTrue(search.Find(new PointD(+1, 0)).IsUnboundedFace);
            Assert.IsTrue(search.Find(new PointD(0, -6)).IsUnboundedFace);
            Assert.IsTrue(search.Find(new PointD(0, +6)).IsUnboundedFace);
        }

        [Test]
        public void DoubleEdgeX() {
            LineD line = new LineD(-5, 0, +5, 0);

            // parallel horizontal edges
            CheckSearch(line, new LineD(-4, +2, +4, +2));
            CheckSearch(line, new LineD(-5, +2, +4, +2));
            CheckSearch(line, new LineD(-5, +2, +5, +2)); // multi-cell intersection
            CheckSearch(line, new LineD(-4, -2, +4, -2));
            CheckSearch(line, new LineD(-4, -2, +5, -2));
            CheckSearch(line, new LineD(-5, -2, +5, -2)); // multi-cell intersection

            // horizontal and vertical edge
            CheckSearch(line, new LineD(0, +1, 0, +4));
            CheckSearch(line, new LineD(0, -1, 0, -4));

            // horizontal and diagonal edge
            CheckSearch(line, new LineD(-5,  0, +4, +2));
            CheckSearch(line, new LineD(-5,  0, +5, +2)); // multi-cell intersection
            CheckSearch(line, new LineD(-5,  0, +5, -2));
            CheckSearch(line, new LineD(-5, +2, +5,  0));
            CheckSearch(line, new LineD(-4, -2, +5,  0));
            CheckSearch(line, new LineD(-5, -2, +5,  0)); // multi-cell intersection
        }

        [Test]
        public void DoubleEdgeY() {
            LineD line = new LineD(0, -5, 0, +5);

            // parallel vertical edges
            CheckSearch(line, new LineD(+2, -4, +2, +4));
            CheckSearch(line, new LineD(+2, -5, +2, +5));
            CheckSearch(line, new LineD(-2, -4, -2, +4));
            CheckSearch(line, new LineD(-2, -5, -2, +5));

            // vertical and horizontal edge
            CheckSearch(line, new LineD(+1, 0, +4, 0));
            CheckSearch(line, new LineD(-1, 0, -4, 0));

            // vertical and diagonal edge
            CheckSearch(line, new LineD(+1, -5, +2, +5));
            CheckSearch(line, new LineD( 0, -5, +2, +5)); // multi-cell intersection
            CheckSearch(line, new LineD( 0, -5, -2, +5));
            CheckSearch(line, new LineD(+2, -5,  0, +5));
        }

        [Test]
        public void FromLines() {
            Subdivision division = SubdivisionFromLines.CreateSquareStar(false);
            CheckSearch(division);

            division = SubdivisionFromLines.CreateTriforce(false);
            CheckSearch(division);
        }

        [Test]
        public void Polygon() {
            for (int i = 3; i < 9; i++) {
                var polygon = new RegularPolygon(30.0 / i, i, PolygonOrientation.OnEdge);
                LineD[] lines = GeoAlgorithms.ConnectPoints(true, polygon.Vertices);
                CheckSearch(lines);
            }
        }

        [Test]
        public void PolygonGrid() {
            var polygon = new RegularPolygon(5, 4, PolygonOrientation.OnEdge);
            var grid = new PolygonGrid(polygon) { Size = new SizeI(10, 10) };
            Subdivision division = grid.ToSubdivision(PointD.Empty).Source;
            CheckSearch(division);

            grid.Element = new RegularPolygon(5, 4, PolygonOrientation.OnVertex);
            division = grid.ToSubdivision(PointD.Empty).Source;
            CheckSearch(division);

            grid.Element = new RegularPolygon(5, 6, PolygonOrientation.OnEdge);
            division = grid.ToSubdivision(PointD.Empty).Source;
            CheckSearch(division);

            grid.Element = new RegularPolygon(5, 6, PolygonOrientation.OnVertex);
            division = grid.ToSubdivision(PointD.Empty).Source;
            CheckSearch(division);
        }

        private SubdivisionSearch CheckSearch(params LineD[] lines) {
            Subdivision division = Subdivision.FromLines(lines);
            division.Validate();
            SubdivisionSearch search = new SubdivisionSearch(division);
            search.Validate();
            CheckVertices(search);
            CheckEdges(search);

            PointD[] points = new[] { 
                new PointD(+1, +1), new PointD(+1, -1),
                new PointD(-1, +1), new PointD(-1, -1)
            };

            if (division.Faces.Count == 1) {
                foreach (PointD point in points) {
                    Assert.IsTrue(search.Find(point).IsUnboundedFace);
                    Assert.IsTrue(division.Find(point).IsUnboundedFace);
                }
            } else {
                var element = new SubdivisionElement(division.Faces[1]);
                foreach (PointD point in points) {
                    Assert.AreEqual(element, search.Find(point));
                    Assert.AreEqual(element, division.Find(point));
                }
            }

            points = new[] {
                new PointD(+10, +10), new PointD(+10, -10),
                new PointD(-10, +10), new PointD(-10, -10)
            };

            foreach (PointD point in points) {
                Assert.IsTrue(search.Find(point).IsUnboundedFace);
                Assert.IsTrue(division.Find(point).IsUnboundedFace);
            }
            return search;
        }

        private SubdivisionSearch CheckSearch(Subdivision division) {
            division.Validate();
            SubdivisionSearch search = new SubdivisionSearch(division);
            search.Validate();
            CheckVertices(search);
            CheckEdges(search);

            foreach (SubdivisionFace face in division.Faces.Values) {
                if (face.OuterEdge == null) continue;
                PointD centroid = face.OuterEdge.CycleCentroid;
                var element = new SubdivisionElement(face);
                Assert.AreEqual(element, search.Find(centroid));
                Assert.AreEqual(element, division.Find(centroid));
            }

            return search;
        }

        private void CheckVertices(SubdivisionSearch search) {

            foreach (PointD vertex in search.Source.Vertices.Keys) {
                var element = new SubdivisionElement(vertex);
                Assert.AreEqual(element, search.Find(vertex));
                Assert.AreEqual(element, search.Source.Find(vertex));

                // brute force search also supports comparison epsilon
                PointD offset = GeoAlgorithms.RandomPoint(-0.1, -0.1, 0.2, 0.2);
                Assert.AreEqual(element, search.Source.Find(vertex + offset, 0.25));
            }
        }

        private void CheckEdges(SubdivisionSearch search) {

            foreach (SubdivisionEdge edge in search.Source.Edges.Values) {
                var edgeElement = new SubdivisionElement(edge);
                var twinElement = new SubdivisionElement(edge._twin);

                // SubdivisionSearch always returns lexicographically increasing half-edges
                PointD start = edge._origin, end = edge._twin._origin;
                int result = PointDComparerX.CompareEpsilon(start, end, search.Source.Epsilon);
                var element = (result < 0 ? edgeElement : twinElement);

                PointD q = new PointD((start.X + end.X) / 2, (start.Y + end.Y) / 2);
                Assert.AreEqual(element, search.Find(q));

                // brute force search may return half-edge or its twin
                var found = search.Source.Find(q, 1e-10);
                Assert.IsTrue(found == edgeElement || found == twinElement);

                // brute force search also supports comparison epsilon
                PointD offset = GeoAlgorithms.RandomPoint(-0.1, -0.1, 0.2, 0.2);
                found = search.Source.Find(q + offset, 0.25);
                Assert.IsTrue(found == edgeElement || found == twinElement);
            }
        }
    }
}
