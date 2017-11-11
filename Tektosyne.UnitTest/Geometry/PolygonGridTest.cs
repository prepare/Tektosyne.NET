using System;
using System.Collections.Generic;

using NUnit.Framework;
using Tektosyne.Geometry;
using Tektosyne.Graph;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class PolygonGridTest {

        RegularPolygon element = new RegularPolygon(10.0, 6, PolygonOrientation.OnEdge);
        PolygonGrid grid, readOnly;

        [SetUp]
        public void SetUp() {
            grid = new PolygonGrid(element);
            readOnly = grid.AsReadOnly();
        }

        [Test]
        public void AsReadOnly() {
            Assert.IsFalse(grid.IsReadOnly);
            Assert.IsTrue(readOnly.IsReadOnly);
            Assert.AreSame(readOnly, grid.AsReadOnly());
            Assert.AreSame(readOnly, readOnly.AsReadOnly());
        }

        [Test]
        public void CompareData() {

            // settable properties
            Assert.AreEqual(grid.Element, readOnly.Element);
            Assert.AreEqual(grid.GridShift, readOnly.GridShift);
            Assert.AreEqual(grid.Size, readOnly.Size);

            // dependent properties
            Assert.AreEqual(grid.CenterDistance, readOnly.CenterDistance);
            Assert.AreEqual(grid.DisplayBounds, readOnly.DisplayBounds);
            Assert.AreEqual(grid.EdgeNeighborOffsets, readOnly.EdgeNeighborOffsets);
            Assert.AreEqual(grid.NeighborOffsets, readOnly.NeighborOffsets);
        }

        [Test]
        public void Element() {
            Assert.Throws<NotSupportedException>(() => readOnly.Element = grid.Element);

            grid.Element = new RegularPolygon(5.0, 4, PolygonOrientation.OnEdge);
            Assert.AreNotEqual(element, grid.Element);
            CompareData();
        }

        [Test]
        public void GridShift() {
            Assert.Throws<NotSupportedException>(() => readOnly.GridShift = grid.GridShift);

            Assert.AreEqual(PolygonGridShift.ColumnDown, grid.GridShift);
            grid.GridShift = PolygonGridShift.ColumnUp;
            CompareData();
        }

        [Test]
        public void Size() {
            Assert.Throws<NotSupportedException>(() => readOnly.Size = grid.Size);

            Assert.AreEqual(new SizeI(1, 1), grid.Size);
            grid.Size = new SizeI(5, 5);
            CompareData();
        }

        [Test]
        public void IGraph2DTest() {
            grid.Size = new SizeI(2, 2);
            var graph = grid as IGraph2D<PointI>;

            PointI[] locations = new[] {
                new PointI(0, 0), new PointI(0, 1), new PointI(1, 0), new PointI(1, 1) };

            Assert.AreEqual(6, graph.Connectivity);
            Assert.AreEqual(4, graph.NodeCount);
            foreach (PointI node in graph.Nodes)
                CollectionAssert.Contains(locations, node);

            double distance = graph.GetDistance(locations[0], locations[1]);
            foreach (PointI location in locations) {
                Assert.IsTrue(graph.Contains(location));

                PointD point = grid.GridToDisplay(location);
                Assert.AreEqual(point, graph.GetWorldLocation(location));
                PointD near = new PointD(point.X + 0.1, point.Y - 0.1);
                Assert.AreEqual(location, graph.GetNearestNode(near));

                PointI[] neighbors = null;
                if (location == locations[0] || location == locations[3])
                    neighbors = new[] { locations[1], locations[2] };
                else if (location == locations[1])
                    neighbors = new[] { locations[0], locations[2], locations[3] };
                else if (location == locations[2])
                    neighbors = new[] { locations[0], locations[1], locations[3] };

                CollectionAssert.AreEquivalent(neighbors, graph.GetNeighbors(location));
                foreach (PointI neighbor in neighbors)
                    Assert.AreEqual(distance, graph.GetDistance(location, neighbor));
            }
        }
    }
}
