using System;
using System.Collections.Generic;

using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    using VertexPair = KeyValuePair<PointD, SubdivisionEdge>;

    [TestFixture]
    public class SubdivisionFromPolygons {

        /*
         *                  e2→           
         *      (-1,+2)------------(+1,+2)
         *         |       ←e3        |
         *         |                  |
         *     e0↑ | ↓e1          e5↑ | ↓e4
         *         |                  |
         *         |        e7→       |
         *      (-1,-2)------------(+1,-2)
         *                 ←e6
         */

        [Test]
        public void FromPolygonsSquare() {
            PointD[][] polygons = {
                new[] { new PointD(-1, -2), new PointD(-1, 2), new PointD(1, 2), new PointD(1, -2) }
            };
            var division = Subdivision.FromPolygons(polygons);
            division.Validate();

            var divisionPolygons = division.ToPolygons();
            Assert.AreEqual(polygons.Length, divisionPolygons.Length);
            CollectionAssert.AreEquivalent(polygons[0], divisionPolygons[0]);

            var edges = division.Edges.Values;
            var faces = division.Faces.Values;

            CollectionAssert.AreEqual(new[] {
                new VertexPair(polygons[0][0], edges[0]),
                new VertexPair(polygons[0][3], edges[5]),
                new VertexPair(polygons[0][1], edges[1]),
                new VertexPair(polygons[0][2], edges[3]),
            }, division.Vertices);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionEdge(0, polygons[0][0], edges[1], faces[0], edges[2], edges[6]),
                new SubdivisionEdge(1, polygons[0][1], edges[0], faces[1], edges[7], edges[3]),
                new SubdivisionEdge(2, polygons[0][1], edges[3], faces[0], edges[4], edges[0]),
                new SubdivisionEdge(3, polygons[0][2], edges[2], faces[1], edges[1], edges[5]),
                new SubdivisionEdge(4, polygons[0][2], edges[5], faces[0], edges[6], edges[2]),
                new SubdivisionEdge(5, polygons[0][3], edges[4], faces[1], edges[3], edges[7]),
                new SubdivisionEdge(6, polygons[0][3], edges[7], faces[0], edges[0], edges[4]),
                new SubdivisionEdge(7, polygons[0][0], edges[6], faces[1], edges[5], edges[1]),
            }, edges);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionFace(division, 0, null, new[] { edges[0] }),
                new SubdivisionFace(division, 1, edges[1], null),
            }, faces);

            CheckFace(edges[0], polygons[0], -8, PointD.Empty);
            Array.Reverse(polygons[0]);
            CheckFace(edges[5], polygons[0], +8, PointD.Empty);

            var cycles = division.GetZeroAreaCycles();
            Assert.AreEqual(0, cycles.Count);
        }

        /*
         *                 ----(0,+6)----
         *                /              \      
         *           e0↑ / ↓e1        e3↑ \ ↓e2
         *              /                  \   
         *             /         e8→       \
         *            / (-1,+2)------(+1,+2) \
         *           /    \     ←e9      /    \ 
         *          /      \            /      \
         *         /    e6↑ \ ↓e7  ↑e11/ e10↓   \
         *        /          \        /          \
         *       /            -(0, 0)-            \
         *      /                                  \ 
         *      |                e5→               |  
         *      (-5,-4)----------------------(+5,-4)
         *                      ←e4
         */

        [Test]
        public void FromPolygonsTriforce() {
            PointD[][] polygons = {
                new[] { new PointD(-5, -4), new PointD(0, 6), new PointD(5, -4) },
                new[] { new PointD(0, 0), new PointD(-1, 2), new PointD(1, 2) }
            };
            var division = Subdivision.FromPolygons(polygons);
            division.Validate();

            var divisionPolygons = division.ToPolygons();
            Assert.AreEqual(polygons.Length, divisionPolygons.Length);
            CollectionAssert.AreEquivalent(polygons[0], divisionPolygons[0]);
            CollectionAssert.AreEquivalent(polygons[1], divisionPolygons[1]);

            var edges = division.Edges.Values;
            var faces = division.Faces.Values;

            CollectionAssert.AreEqual(new[] {
                new VertexPair(polygons[0][0], edges[0]),
                new VertexPair(polygons[0][2], edges[3]),
                new VertexPair(polygons[1][0], edges[6]),
                new VertexPair(polygons[1][1], edges[7]),
                new VertexPair(polygons[1][2], edges[9]),
                new VertexPair(polygons[0][1], edges[1]),
            }, division.Vertices);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionEdge(0, polygons[0][0], edges[1], faces[0], edges[2], edges[4]),
                new SubdivisionEdge(1, polygons[0][1], edges[0], faces[1], edges[5], edges[3]),
                new SubdivisionEdge(2, polygons[0][1], edges[3], faces[0], edges[4], edges[0]),
                new SubdivisionEdge(3, polygons[0][2], edges[2], faces[1], edges[1], edges[5]),
                new SubdivisionEdge(4, polygons[0][2], edges[5], faces[0], edges[0], edges[2]),
                new SubdivisionEdge(5, polygons[0][0], edges[4], faces[1], edges[3], edges[1]),
                new SubdivisionEdge(6, polygons[1][0], edges[7], faces[1], edges[8], edges[10]),
                new SubdivisionEdge(7, polygons[1][1], edges[6], faces[2], edges[11], edges[9]),
                new SubdivisionEdge(8, polygons[1][1], edges[9], faces[1], edges[10], edges[6]),
                new SubdivisionEdge(9, polygons[1][2], edges[8], faces[2], edges[7], edges[11]),
                new SubdivisionEdge(10, polygons[1][2], edges[11], faces[1], edges[6], edges[8]),
                new SubdivisionEdge(11, polygons[1][0], edges[10], faces[2], edges[9], edges[7]),
            }, edges);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionFace(division, 0, null, new[] { edges[0] }),
                new SubdivisionFace(division, 1, edges[1], new[] { edges[6] }),
                new SubdivisionFace(division, 2, edges[7], null),
            }, faces);

            PointD centroid = new PointD(0, -2 / 3.0);
            CheckFace(edges[0], polygons[0], -50, centroid);
            Array.Reverse(polygons[0]);
            CheckFace(edges[3], polygons[0], +50, centroid);

            centroid = new PointD(0, 4 / 3.0);
            CheckFace(edges[6], polygons[1], -2, centroid);
            Array.Reverse(polygons[1]);
            CheckFace(edges[9], polygons[1], +2, centroid);

            var cycles = division.GetZeroAreaCycles();
            Assert.AreEqual(0, cycles.Count);
        }

        /*
         *              -----(0,+4)-----
         *             /      /  \      \
         *        e2↑ / ↓e3  /    \  e13↑\ ↓e12
         *           /      /      \      \
         *          /      /        \      \
         *         /       |        |       \
         *        /    e5↑ |↓e4  e9↑| ↓e8    \
         *       /         |        |         \
         *    (-6,0)    (-3,0)    (+3,0)    (+6,0)
         *       \         |        |         /
         *        \    e7↑ |↓e6 e11↑| ↓e10   /
         *         \       |        |       /
         *          \      \        /      /
         *           \      \      /      /
         *        e0↑ \ ↓e1  \    /  e15↑/ ↓e14
         *             \      \  /      / 
         *              -----(0,-4)-----
         */

        [Test]
        public void FromPolygonsDiamond() {
            PointD[][] polygons = {
                new[] { new PointD(0, -4), new PointD(-6, 0), new PointD(0, 4), new PointD(-3, 0) },
                new[] { new PointD(0, -4), new PointD(-3, 0), new PointD(0, 4), new PointD(3, 0) },
                new[] { new PointD(0, -4), new PointD(3, 0), new PointD(0, 4), new PointD(6, 0) }
            };
            var division = Subdivision.FromPolygons(polygons);
            division.Validate();

            var divisionPolygons = division.ToPolygons();
            Assert.AreEqual(polygons.Length, divisionPolygons.Length);
            CollectionAssert.AreEquivalent(polygons[0], divisionPolygons[0]);
            CollectionAssert.AreEquivalent(polygons[1], divisionPolygons[1]);
            CollectionAssert.AreEquivalent(polygons[2], divisionPolygons[2]);

            CheckPolygonsDiamond(polygons, division);
        }

        [Test]
        public void FromPolygonsDiamondEpsilon() {
            PointD[][] polygons = {
                new[] { new PointD(0, -4), new PointD(-6, 0), new PointD(0, 4), new PointD(-3, 0) },
                new[] { new PointD(0.1, -3.9), new PointD(-3, 0), new PointD(-0.1, 4.1), new PointD(3, 0) },
                new[] { new PointD(-0.1, -4.1), new PointD(2.9, 0.1), new PointD(0.1, 3.9), new PointD(6, 0) }
            };
            var division = Subdivision.FromPolygons(polygons, 0.2);
            division.Validate();

            var divisionPolygons = division.ToPolygons();
            Assert.AreEqual(polygons.Length, divisionPolygons.Length);
            CollectionAssert.AreEquivalent(polygons[0], divisionPolygons[0]);
            CollectionAssert.AreNotEquivalent(polygons[1], divisionPolygons[1]);
            CollectionAssert.AreNotEquivalent(polygons[2], divisionPolygons[2]);

            CheckPolygonsDiamond(polygons, division);
        }

        private static void CheckPolygonsDiamond(PointD[][] polygons, Subdivision division) {
            var edges = division.Edges.Values;
            var faces = division.Faces.Values;

            CollectionAssert.AreEqual(new[] {
                new VertexPair(polygons[0][0], edges[0]),
                new VertexPair(polygons[0][1], edges[1]),
                new VertexPair(polygons[0][3], edges[5]),
                new VertexPair(polygons[1][3], edges[9]),
                new VertexPair(polygons[2][3], edges[13]),
                new VertexPair(polygons[0][2], edges[3]),
            }, division.Vertices);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionEdge(0, polygons[0][0], edges[1], faces[0], edges[2], edges[14]),
                new SubdivisionEdge(1, polygons[0][1], edges[0], faces[1], edges[7], edges[3]),
                new SubdivisionEdge(2, polygons[0][1], edges[3], faces[0], edges[12], edges[0]),
                new SubdivisionEdge(3, polygons[0][2], edges[2], faces[1], edges[1], edges[5]),
                new SubdivisionEdge(4, polygons[0][2], edges[5], faces[2], edges[6], edges[9]),
                new SubdivisionEdge(5, polygons[0][3], edges[4], faces[1], edges[3], edges[7]),
                new SubdivisionEdge(6, polygons[0][3], edges[7], faces[2], edges[11], edges[4]),
                new SubdivisionEdge(7, polygons[0][0], edges[6], faces[1], edges[5], edges[1]),
                new SubdivisionEdge(8, polygons[0][2], edges[9], faces[3], edges[10], edges[13]),
                new SubdivisionEdge(9, polygons[1][3], edges[8], faces[2], edges[4], edges[11]),
                new SubdivisionEdge(10, polygons[1][3], edges[11], faces[3], edges[15], edges[8]),
                new SubdivisionEdge(11, polygons[0][0], edges[10], faces[2], edges[9], edges[6]),
                new SubdivisionEdge(12, polygons[0][2], edges[13], faces[0], edges[14], edges[2]),
                new SubdivisionEdge(13, polygons[2][3], edges[12], faces[3], edges[8], edges[15]),
                new SubdivisionEdge(14, polygons[2][3], edges[15], faces[0], edges[0], edges[12]),
                new SubdivisionEdge(15, polygons[0][0], edges[14], faces[3], edges[13], edges[10]),
            }, edges);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionFace(division, 0, null, new[] { edges[0] }),
                new SubdivisionFace(division, 1, edges[1], null),
                new SubdivisionFace(division, 2, edges[4], null),
                new SubdivisionFace(division, 3, edges[8], null),
            }, faces);

            CheckFace(edges[0], new[] {
                polygons[0][0], polygons[0][1], polygons[0][2], polygons[2][3] }, -48, PointD.Empty);
            CheckFace(edges[1], new[] {
                polygons[0][1], polygons[0][0], polygons[0][3], polygons[0][2] }, +12, new PointD(-3, 0));
            CheckFace(edges[6], new[] {
                polygons[0][3], polygons[0][0], polygons[1][3], polygons[0][2] }, +24, PointD.Empty);
            CheckFace(edges[10], new[] {
                polygons[1][3], polygons[0][0], polygons[2][3], polygons[0][2] }, +12, new PointD(3, 0));

            var cycles = division.GetZeroAreaCycles();
            Assert.AreEqual(0, cycles.Count);
        }

        private static void CheckFace(
            SubdivisionEdge edge, PointD[] polygon, double area, PointD centroid) {

            CollectionAssert.AreEqual(polygon, edge.CyclePolygon);
            Assert.AreEqual(area, edge.CycleArea);
            Assert.AreEqual((area == 0), edge.IsCycleAreaZero);
            if (area != 0) Assert.AreEqual(centroid, edge.CycleCentroid);
        }
    }
}
