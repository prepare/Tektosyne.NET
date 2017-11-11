using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class GeoAlgorithmsTest {

        [Test]
        public void ConnectPoints() {
            PointD p0 = new PointD(0, 0), p1 = new PointD(1, 1), p2 = new PointD(2, 0);

            CollectionAssert.AreEqual(new LineD[0], GeoAlgorithms.ConnectPoints(false));
            CollectionAssert.AreEqual(new LineD[0], GeoAlgorithms.ConnectPoints(true));
            CollectionAssert.AreEqual(new LineD[0], GeoAlgorithms.ConnectPoints(false, p0));
            CollectionAssert.AreEqual(new LineD[0], GeoAlgorithms.ConnectPoints(true, p0));

            CollectionAssert.AreEqual(new[] { new LineD(p0, p1) },
                GeoAlgorithms.ConnectPoints(false, p0, p1));
            CollectionAssert.AreEqual(new[] { new LineD(p0, p1), new LineD(p1, p0) },
                GeoAlgorithms.ConnectPoints(true, p0, p1));

            CollectionAssert.AreEqual(new[] { new LineD(p0, p1), new LineD(p1, p2) },
                GeoAlgorithms.ConnectPoints(false, p0, p1, p2));
            CollectionAssert.AreEqual(new[] { new LineD(p0, p1), new LineD(p1, p2), new LineD(p2, p0) },
                GeoAlgorithms.ConnectPoints(true, p0, p1, p2));
        }

        [Test]
        public void ConvexHull() {
            PointD p0 = new PointD(0, 0), p1 = new PointD(1, 1), p2 = new PointD(2, 0);

            CollectionAssert.AreEqual(new[] { p0 }, GeoAlgorithms.ConvexHull(p0));
            CollectionAssert.AreEqual(new[] { p0 }, GeoAlgorithms.ConvexHull(p0, p0));
            CollectionAssert.AreEqual(new[] { p0 }, GeoAlgorithms.ConvexHull(p0, p0, p0));

            CollectionAssert.AreEqual(new[] { p0, p1 }, GeoAlgorithms.ConvexHull(p0, p1));
            CollectionAssert.AreEqual(new[] { p1, p0 }, GeoAlgorithms.ConvexHull(p0, p1, p0));
            CollectionAssert.AreEqual(new[] { p1, p0 }, GeoAlgorithms.ConvexHull(p0, p0, p1));
            CollectionAssert.AreEqual(new[] { p1, p0 }, GeoAlgorithms.ConvexHull(p0, p1, p1));

            CollectionAssert.AreEqual(new[] { p1, p2, p0 }, GeoAlgorithms.ConvexHull(p0, p1, p2));
            CollectionAssert.AreEqual(new[] { p1, p2, p0 }, GeoAlgorithms.ConvexHull(p1, p0, p2));

            PointD p3 = new PointD(1, 0);
            CollectionAssert.AreEqual(new[] { p1, p2, p0 }, GeoAlgorithms.ConvexHull(p3, p1, p0, p2));
        }

        [Test]
        public void PointInPolygon() {
            PointD[] p = { new PointD(0, 0), new PointD(1, 1), new PointD(2, 0) };
            Assert.AreEqual(PolygonLocation.Inside, GeoAlgorithms.PointInPolygon(new PointD(1.0, 0.5), p));

            Assert.AreEqual(PolygonLocation.Outside, GeoAlgorithms.PointInPolygon(new PointD(0.0, 0.5), p));
            Assert.AreEqual(PolygonLocation.Outside, GeoAlgorithms.PointInPolygon(new PointD(2.0, 0.5), p));
            Assert.AreEqual(PolygonLocation.Outside, GeoAlgorithms.PointInPolygon(new PointD(1.0, -0.5), p));
            Assert.AreEqual(PolygonLocation.Outside, GeoAlgorithms.PointInPolygon(new PointD(1.0, 2.5), p));

            Assert.AreEqual(PolygonLocation.Edge, GeoAlgorithms.PointInPolygon(new PointD(1.0, 0.0), p));
            Assert.AreEqual(PolygonLocation.Edge, GeoAlgorithms.PointInPolygon(new PointD(0.5, 0.5), p));
            Assert.AreEqual(PolygonLocation.Edge, GeoAlgorithms.PointInPolygon(new PointD(1.5, 0.5), p));

            Assert.AreEqual(PolygonLocation.Vertex, GeoAlgorithms.PointInPolygon(new PointD(0.0, 0.0), p));
            Assert.AreEqual(PolygonLocation.Vertex, GeoAlgorithms.PointInPolygon(new PointD(1.0, 1.0), p));
            Assert.AreEqual(PolygonLocation.Vertex, GeoAlgorithms.PointInPolygon(new PointD(2.0, 0.0), p));
        }

        [Test]
        public void PointInPolygonEpsilon() {
            PointD[] p = { new PointD(0, 0), new PointD(1, 1), new PointD(2, 0) };
            Assert.AreEqual(PolygonLocation.Inside, GeoAlgorithms.PointInPolygon(new PointD(1.0, 0.5), p, 0.2));

            Assert.AreEqual(PolygonLocation.Outside, GeoAlgorithms.PointInPolygon(new PointD(1.0, -0.5), p, 0.2));
            Assert.AreEqual(PolygonLocation.Outside, GeoAlgorithms.PointInPolygon(new PointD(0.0, 0.5), p, 0.2));
            Assert.AreEqual(PolygonLocation.Outside, GeoAlgorithms.PointInPolygon(new PointD(2.0, 0.5), p, 0.2));

            Assert.AreEqual(PolygonLocation.Vertex, GeoAlgorithms.PointInPolygon(new PointD(1.0, 0.9), p, 0.2));
            Assert.AreEqual(PolygonLocation.Vertex, GeoAlgorithms.PointInPolygon(new PointD(0.0, 0.1), p, 0.2));
            Assert.AreEqual(PolygonLocation.Vertex, GeoAlgorithms.PointInPolygon(new PointD(2.1, 0.0), p, 0.2));

            Assert.AreEqual(PolygonLocation.Edge, GeoAlgorithms.PointInPolygon(new PointD(1.0, -0.1), p, 0.2));
            Assert.AreEqual(PolygonLocation.Edge, GeoAlgorithms.PointInPolygon(new PointD(0.6, 0.5), p, 0.2));
            Assert.AreEqual(PolygonLocation.Edge, GeoAlgorithms.PointInPolygon(new PointD(1.4, 0.5), p, 0.2));
        }

        [Test]
        public void PolygonArea() {
            PointD p0 = new PointD(0, 0), p1 = new PointD(1, 1), p2 = new PointD(2, 0);
            PointD p3 = new PointD(0, 2), p4 = new PointD(2, 2);

            // triangles in both orientations
            Assert.AreEqual(-1, GeoAlgorithms.PolygonArea(p0, p1, p2));
            Assert.AreEqual(+1, GeoAlgorithms.PolygonArea(p2, p1, p0));

            // squares in both orientations
            Assert.AreEqual(-4, GeoAlgorithms.PolygonArea(p0, p3, p4, p2));
            Assert.AreEqual(+4, GeoAlgorithms.PolygonArea(p2, p4, p3, p0));

            // collinear points and star shape
            Assert.AreEqual(0, GeoAlgorithms.PolygonArea(p0, p1, p4));
            Assert.AreEqual(0, GeoAlgorithms.PolygonArea(p0, p1, p3, p1, p2, p1, p4, p1));
        }

        [Test]
        public void PolygonCentroid() {
            PointD p0 = new PointD(0, 0), p1 = new PointD(1, 1), p2 = new PointD(2, 0);
            PointD p3 = new PointD(0, 2), p4 = new PointD(2, 2);

            Assert.AreEqual(new PointD(1, 1 / 3.0), GeoAlgorithms.PolygonCentroid(p0, p1, p2));
            Assert.AreEqual(new PointD(1, 1 / 3.0), GeoAlgorithms.PolygonCentroid(p2, p1, p0));

            Assert.AreEqual(p1, GeoAlgorithms.PolygonCentroid(p0, p3, p4, p2));
            Assert.AreEqual(p1, GeoAlgorithms.PolygonCentroid(p2, p4, p3, p0));
        }

        [Test]
        public void RandomPoints() {
            RectD bounds = new RectD(-100, -100, 200, 200);
            PointD[] points = GeoAlgorithms.RandomPoints(100, bounds);
            foreach (PointD p in points)
                Assert.IsTrue(bounds.Contains(p));
        }

        [Test]
        public void RandomPointsComparer() {
            RandomPointsComparer(new PointDComparerX());
            RandomPointsComparer(new PointDComparerY());
        }

        [Test]
        public void RandomPointsComparerEpsilon() {
            RandomPointsComparer(new PointDComparerX() { Epsilon = 0.5 });
            RandomPointsComparer(new PointDComparerY() { Epsilon = 0.5 });
        }

        private static void RandomPointsComparer(IPointDComparer comparer) {
            RectD bounds = new RectD(-100, -100, 200, 200);
            PointD[] points = GeoAlgorithms.RandomPoints(100, bounds, comparer, 2);

            for (int i = 0; i < points.Length; i++) {
                PointD p = points[i];
                Assert.IsTrue(bounds.Contains(p));
                if (i > 0)
                    Assert.AreEqual(+1, comparer.Compare(p, points[i - 1]));
                if (i < points.Length - 1)
                    Assert.AreEqual(-1, comparer.Compare(p, points[i + 1]));

                for (int j = 0; j < points.Length; j++) {
                    if (i == j) continue;
                    double distance = p.Subtract(points[j]).LengthSquared;
                    Assert.GreaterOrEqual(distance, 4);
                }
            }
        }
    }
}
