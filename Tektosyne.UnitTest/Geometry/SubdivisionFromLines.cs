using System;
using System.Collections.Generic;

using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    using VertexPair = KeyValuePair<PointD, SubdivisionEdge>;

    [TestFixture]
    public class SubdivisionFromLines {

        /*
         *                  e2→
         *      (-1,+2)------------(+1,+2)
         *                 ←e3
         *
         *
         *
         *                  e0→
         *      (-1,-2)------------(+1,-2)
         *                 ←e1
         */
        [Test]
        public void FromLinesHorizontal() {
            PointD[] points = {
                new PointD(-1, -2), new PointD(-1, 2), new PointD(1, -2), new PointD(1, 2)
            };
            LineD[] lines = {
                new LineD(points[0], points[2]), new LineD(points[1], points[3])
            };
            var division = Subdivision.FromLines(lines);
            division.Validate();

            CollectionAssert.AreEquivalent(lines, division.ToLines());
            var edges = division.Edges.Values;
            var faces = division.Faces.Values;

            CollectionAssert.AreEqual(new[] {
                new VertexPair(points[0], edges[0]),
                new VertexPair(points[2], edges[1]),
                new VertexPair(points[1], edges[2]),
                new VertexPair(points[3], edges[3]),
            }, division.Vertices);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionEdge(0, points[0], edges[1], faces[0], edges[1], edges[1]),
                new SubdivisionEdge(1, points[2], edges[0], faces[0], edges[0], edges[0]),
                new SubdivisionEdge(2, points[1], edges[3], faces[0], edges[3], edges[3]),
                new SubdivisionEdge(3, points[3], edges[2], faces[0], edges[2], edges[2]),
            }, edges);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionFace(division, 0, null, new[] { edges[0], edges[2] }),
            }, faces);

            CheckFace(edges[0], new[] { points[0], points[2] }, 0, PointD.Empty);
            CheckFace(edges[2], new[] { points[1], points[3] }, 0, PointD.Empty);

            var cycles = division.GetZeroAreaCycles();
            Assert.AreEqual(2, cycles.Count);
            Assert.AreEqual(edges[0], cycles[0]);
            Assert.AreEqual(edges[2], cycles[1]);
        }

        /*
         *      (-1,+2)            (+1,+2)
         *         |                  |
         *         |                  |
         *     e0↑ | ↓e1          e2↑ | ↓e3
         *         |                  |
         *         |                  |
         *      (-1,-2)            (+1,-2)
         */

        [Test]
        public void FromLinesVertical() {
            PointD[] points = {
                new PointD(-1, -2), new PointD(-1, 2), new PointD(1, -2), new PointD(1, 2)
            };
            LineD[] lines = {
                new LineD(points[0], points[1]), new LineD(points[2], points[3])
            };
            var division = Subdivision.FromLines(lines);
            division.Validate();

            CollectionAssert.AreEquivalent(lines, division.ToLines());
            var edges = division.Edges.Values;
            var faces = division.Faces.Values;

            CollectionAssert.AreEqual(new[] {
                new VertexPair(points[0], edges[0]),
                new VertexPair(points[2], edges[2]),
                new VertexPair(points[1], edges[1]),
                new VertexPair(points[3], edges[3]),
            }, division.Vertices);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionEdge(0, points[0], edges[1], faces[0], edges[1], edges[1]),
                new SubdivisionEdge(1, points[1], edges[0], faces[0], edges[0], edges[0]),
                new SubdivisionEdge(2, points[2], edges[3], faces[0], edges[3], edges[3]),
                new SubdivisionEdge(3, points[3], edges[2], faces[0], edges[2], edges[2]),
            }, edges);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionFace(division, 0, null, new[] { edges[0], edges[2] }),
            }, faces);

            CheckFace(edges[0], new[] { points[0], points[1] }, 0, PointD.Empty);
            CheckFace(edges[2], new[] { points[2], points[3] }, 0, PointD.Empty);

            var cycles = division.GetZeroAreaCycles();
            Assert.AreEqual(2, cycles.Count);
            Assert.AreEqual(edges[0], cycles[0]);
            Assert.AreEqual(edges[2], cycles[1]);
        }

        /*
         *                  e2→
         *      (-1,+2)------------(+1,+2)
         *         |       ←e3        |
         *         |                  |
         *     e0↑ | ↓e1          e4↑ | ↓e5
         *         |                  |
         *         |        e6→       |
         *      (-1,-2)------------(+1,-2)
         *                 ←e7
         */

        [Test]
        public void FromLinesSquare() {
            PointD[] points = {
                new PointD(-1, -2), new PointD(-1, 2), new PointD(1, 2), new PointD(1, -2)
            };
            LineD[] lines = {
                new LineD(points[0], points[1]), new LineD(points[1], points[2]),
                new LineD(points[3], points[2]), new LineD(points[0], points[3])
            };
            var division = Subdivision.FromLines(lines);
            division.Validate();

            CollectionAssert.AreEquivalent(lines, division.ToLines());
            CheckLinesSquare(points, division);
        }

        [Test]
        public void FromLinesSquareEpsilon() {
            PointD[] points = {
                new PointD(-1, -2), new PointD(-1, 2), new PointD(1, 2), new PointD(1, -2)
            };
            LineD[] lines = {
                new LineD(points[0], points[1]),
                new LineD(new PointD(-1.1, 1.9), points[2]),
                new LineD(points[3], new PointD(0.9, 2.1)),
                new LineD(new PointD(-0.9, -2.1), new PointD(1.1, -1.9))
            };
            var division = Subdivision.FromLines(lines, 0.2);
            division.Validate();

            CollectionAssert.AreNotEquivalent(lines, division.ToLines());
            CheckLinesSquare(points, division);
        }

        private static void CheckLinesSquare(PointD[] points, Subdivision division) {
            var edges = division.Edges.Values;
            var faces = division.Faces.Values;

            CollectionAssert.AreEqual(new[] {
                new VertexPair(points[0], edges[0]),
                new VertexPair(points[3], edges[4]),
                new VertexPair(points[1], edges[1]),
                new VertexPair(points[2], edges[3]),
            }, division.Vertices);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionEdge(0, points[0], edges[1], faces[0], edges[2], edges[7]),
                new SubdivisionEdge(1, points[1], edges[0], faces[1], edges[6], edges[3]),
                new SubdivisionEdge(2, points[1], edges[3], faces[0], edges[5], edges[0]),
                new SubdivisionEdge(3, points[2], edges[2], faces[1], edges[1], edges[4]),
                new SubdivisionEdge(4, points[3], edges[5], faces[1], edges[3], edges[6]),
                new SubdivisionEdge(5, points[2], edges[4], faces[0], edges[7], edges[2]),
                new SubdivisionEdge(6, points[0], edges[7], faces[1], edges[4], edges[1]),
                new SubdivisionEdge(7, points[3], edges[6], faces[0], edges[0], edges[5]),
            }, edges);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionFace(division, 0, null, new[] { edges[0] }),
                new SubdivisionFace(division, 1, edges[1], null),
            }, faces);

            CheckFace(edges[0], points, -8, PointD.Empty);
            Array.Reverse(points);
            CheckFace(edges[4], points, +8, PointD.Empty);

            var cycles = division.GetZeroAreaCycles();
            Assert.AreEqual(0, cycles.Count);
        }

        /*
         *      (-1,+2)            (+1,+2)
         *            \            /
         *             \ ↓e3  e4↑ /
         *          e2↑ \        / ↓e5
         *               \      /
         *                (0, 0)
         *               /      \      
         *          e0↑ /        \ ↓e7
         *             / ↓e1  e6↑ \    
         *            /            \   
         *      (-1,-2)            (+1,-2)
         */

        [Test]
        public void FromLinesStar() {
            PointD[] points = {
                new PointD(-1, -2), new PointD(0, 0),
                new PointD(-1, 2), new PointD(1, 2), new PointD(1, -2)
            };
            LineD[] lines = {
                new LineD(points[0], points[1]), new LineD(points[1], points[2]),
                new LineD(points[1], points[3]), new LineD(points[4], points[1])
            };
            var division = Subdivision.FromLines(lines);
            division.Validate();

            CollectionAssert.AreEquivalent(lines, division.ToLines());
            CheckLinesStar(points, division);
        }

        [Test]
        public void FromLinesStarEpsilon() {
            PointD[] points = {
                new PointD(-1, -2), new PointD(0, 0),
                new PointD(-1, 2), new PointD(1, 2), new PointD(1, -2)
            };
            LineD[] lines = {
                new LineD(points[0], points[1]),
                new LineD(new PointD(0.1, -0.1), points[2]),
                new LineD(new PointD(-0.1, 0.1), points[3]),
                new LineD(points[4], new PointD(0.1, 0.1))
            };
            var division = Subdivision.FromLines(lines, 0.2);
            division.Validate();

            CollectionAssert.AreNotEquivalent(lines, division.ToLines());
            CheckLinesStar(points, division);
        }

        private static void CheckLinesStar(PointD[] points, Subdivision division) {
            var edges = division.Edges.Values;
            var faces = division.Faces.Values;

            CollectionAssert.AreEqual(new[] {
                new VertexPair(points[0], edges[0]),
                new VertexPair(points[4], edges[6]),
                new VertexPair(points[1], edges[1]),
                new VertexPair(points[2], edges[3]),
                new VertexPair(points[3], edges[5]),
            }, division.Vertices);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionEdge(0, points[0], edges[1], faces[0], edges[2], edges[1]),
                new SubdivisionEdge(1, points[1], edges[0], faces[0], edges[0], edges[6]),
                new SubdivisionEdge(2, points[1], edges[3], faces[0], edges[3], edges[0]),
                new SubdivisionEdge(3, points[2], edges[2], faces[0], edges[4], edges[2]),
                new SubdivisionEdge(4, points[1], edges[5], faces[0], edges[5], edges[3]),
                new SubdivisionEdge(5, points[3], edges[4], faces[0], edges[7], edges[4]),
                new SubdivisionEdge(6, points[4], edges[7], faces[0], edges[1], edges[7]),
                new SubdivisionEdge(7, points[1], edges[6], faces[0], edges[6], edges[5]),
            }, edges);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionFace(division, 0, null, new[] { edges[0] }),
            }, faces);

            CheckFace(edges[0], new[] {
                points[0], points[1], points[2], points[1],
                points[3], points[1], points[4], points[1]
            }, 0, PointD.Empty);

            var cycles = division.GetZeroAreaCycles();
            Assert.AreEqual(1, cycles.Count);
            Assert.AreEqual(edges[0], cycles[0]);
        }

        /*
         *                      e2→
         *      (-1,+2)--------------------(+1,+2)
         *         |  \        ←e3         /  |
         *         |   \↓e11          e12↑/   |
         *         |    e10↑\   f2   /↓e13    |
         *         |         \      /         |
         *     e0↑ | ↓e1  f1  (0, 0)  f3  e4↑ | ↓e5
         *         |         /      \         |
         *         |    e9↑_/   f4   \↓e14    |
         *         |   /↓e8           e15↑\   |
         *         |  /         e6→        \  |
         *      (-1,-2)--------------------(+1,-2)
         *                     ←e7
         */

        [Test]
        public void FromLinesSquareStar() {
            CreateSquareStar(true);
        }

        public static Subdivision CreateSquareStar(bool test) {
            PointD[] points = {
                new PointD(), new PointD(-1, -2), new PointD(-1, 2), new PointD(1, 2), new PointD(1, -2) 
            };
            LineD[] lines = {
                new LineD(points[1], points[2]), new LineD(points[2], points[3]),
                new LineD(points[4], points[3]), new LineD(points[1], points[4]),
                new LineD(points[0], points[1]), new LineD(points[0], points[2]),
                new LineD(points[0], points[3]), new LineD(points[0], points[4])
            };
            var division = Subdivision.FromLines(lines);
            division.Validate();
            if (!test) return division;

            var edges = division.Edges.Values;
            var faces = division.Faces.Values;
            Assert.AreEqual(16, edges.Count);
            Assert.AreEqual(5, faces.Count);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionEdge(0, points[1], edges[1], faces[0], edges[2], edges[7]),
                new SubdivisionEdge(1, points[2], edges[0], faces[1], edges[9], edges[10]),
                new SubdivisionEdge(2, points[2], edges[3], faces[0], edges[5], edges[0]),
                new SubdivisionEdge(3, points[3], edges[2], faces[2], edges[11], edges[12]),
                new SubdivisionEdge(4, points[4], edges[5], faces[3], edges[13], edges[14]),
                new SubdivisionEdge(5, points[3], edges[4], faces[0], edges[7], edges[2]),
                new SubdivisionEdge(6, points[1], edges[7], faces[4], edges[15], edges[8]),
                new SubdivisionEdge(7, points[4], edges[6], faces[0], edges[0], edges[5]),

                new SubdivisionEdge( 8, points[0], edges[9], faces[4], edges[6], edges[15]),
                new SubdivisionEdge( 9, points[1], edges[8], faces[1], edges[10], edges[1]),
                new SubdivisionEdge(10, points[0], edges[11], faces[1], edges[1], edges[9]),
                new SubdivisionEdge(11, points[2], edges[10], faces[2], edges[12], edges[3]),
                new SubdivisionEdge(12, points[0], edges[13], faces[2], edges[3], edges[11]),
                new SubdivisionEdge(13, points[3], edges[12], faces[3], edges[14], edges[4]),
                new SubdivisionEdge(14, points[0], edges[15], faces[3], edges[4], edges[13]),
                new SubdivisionEdge(15, points[4], edges[14], faces[4], edges[8], edges[6]),
            }, edges);


            var cycles = division.GetZeroAreaCycles();
            Assert.AreEqual(0, cycles.Count);
            return division;
        }

        /*
         *                 ----(0,+6)----
         *                /              \      
         *           e0↑ / ↓e1        e2↑ \ ↓e3
         *              /                  \   
         *             /         e10→       \
         *            / (-1,+2)------(+1,+2) \
         *           /    \     ←e11     /    \ 
         *          /      \            /      \
         *         /    e6↑ \ ↓e7  ↑e8 / e9↓    \
         *        /          \        /          \
         *       /            -(0, 0)-            \
         *      /                                  \ 
         *      |                e4→               |  
         *      (-5,-4)----------------------(+5,-4)
         *                      ←e5
         */

        [Test]
        public void FromLinesTriforce() {
            CreateTriforce(true);
        }

        public static Subdivision CreateTriforce(bool test) {
            PointD[] points = {
                new PointD(-5, -4), new PointD(0, 6), new PointD(5, -4),
                new PointD(0, 0), new PointD(-1, 2), new PointD(1, 2)
            };
            LineD[] lines = {
                new LineD(points[0], points[1]), new LineD(points[2], points[1]),
                new LineD(points[0], points[2]), new LineD(points[3], points[4]),
                new LineD(points[3], points[5]), new LineD(points[4], points[5])
            };

            Subdivision division = Subdivision.FromLines(lines);
            division.Validate();
            if (!test) return division;

            CollectionAssert.AreEquivalent(lines, division.ToLines());
            var edges = division.Edges.Values;
            var faces = division.Faces.Values;

            CollectionAssert.AreEqual(new[] {
                new VertexPair(points[0], edges[0]),
                new VertexPair(points[2], edges[2]),
                new VertexPair(points[3], edges[6]),
                new VertexPair(points[4], edges[7]),
                new VertexPair(points[5], edges[9]),
                new VertexPair(points[1], edges[1]),
            }, division.Vertices);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionEdge(0, points[0], edges[1], faces[0], edges[3], edges[5]),
                new SubdivisionEdge(1, points[1], edges[0], faces[1], edges[4], edges[2]),
                new SubdivisionEdge(2, points[2], edges[3], faces[1], edges[1], edges[4]),
                new SubdivisionEdge(3, points[1], edges[2], faces[0], edges[5], edges[0]),
                new SubdivisionEdge(4, points[0], edges[5], faces[1], edges[2], edges[1]),
                new SubdivisionEdge(5, points[2], edges[4], faces[0], edges[0], edges[3]),
                new SubdivisionEdge(6, points[3], edges[7], faces[1], edges[10], edges[9]),
                new SubdivisionEdge(7, points[4], edges[6], faces[2], edges[8], edges[11]),
                new SubdivisionEdge(8, points[3], edges[9], faces[2], edges[11], edges[7]),
                new SubdivisionEdge(9, points[5], edges[8], faces[1], edges[6], edges[10]),
                new SubdivisionEdge(10, points[4], edges[11], faces[1], edges[9], edges[6]),
                new SubdivisionEdge(11, points[5], edges[10], faces[2], edges[7], edges[8]),
            }, edges);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionFace(division, 0, null, new[] { edges[0] }),
                new SubdivisionFace(division, 1, edges[1], new[] { edges[6] }),
                new SubdivisionFace(division, 2, edges[7], null),
            }, faces);

            PointD centroid = new PointD(0, -2 / 3.0);
            CheckFace(edges[0], new[] { points[0], points[1], points[2] }, -50, centroid);
            CheckFace(edges[1], new[] { points[1], points[0], points[2] }, +50, centroid);

            centroid = new PointD(0, 4 / 3.0);
            CheckFace(edges[6], new[] { points[3], points[4], points[5] }, -2, centroid);
            CheckFace(edges[7], new[] { points[4], points[3], points[5] }, +2, centroid);

            var cycles = division.GetZeroAreaCycles();
            Assert.AreEqual(0, cycles.Count);
            return division;
        }

        /*
         *              -----(0,+4)-----
         *             /      /  \      \
         *        e2↑ / ↓e3  /    \  e14↑\ ↓e15
         *           /      /      \      \
         *          /      /        \      \
         *         /       |        |       \
         *        /    e6↑ |↓e7 e10↑| ↓e11   \
         *       /         |        |         \
         *    (-6,0)    (-3,0)    (+3,0)    (+6,0)
         *       \         |        |         /
         *        \    e4↑ |↓e5  e8↑| ↓e9    /
         *         \       |        |       /
         *          \      \        /      /
         *           \      \      /      /
         *        e0↑ \ ↓e1  \    /  e12↑/ ↓e13
         *             \      \  /      / 
         *              -----(0,-4)-----
         */

        [Test]
        public void FromLinesDiamond() {
            PointD[] points = {
                new PointD(0, -4), new PointD(-6, 0), new PointD(-3, 0),
                new PointD(3, 0), new PointD(6, 0), new PointD(0, 4)
            };
            LineD[] lines = {
                new LineD(points[0], points[1]), new LineD(points[1], points[5]),
                new LineD(points[0], points[2]), new LineD(points[2], points[5]),
                new LineD(points[0], points[3]), new LineD(points[3], points[5]),
                new LineD(points[0], points[4]), new LineD(points[4], points[5])
            };
            var division = Subdivision.FromLines(lines);
            division.Validate();

            CollectionAssert.AreEquivalent(lines, division.ToLines());
            var edges = division.Edges.Values;
            var faces = division.Faces.Values;

            CollectionAssert.AreEqual(new[] {
                new VertexPair(points[0], edges[0]),
                new VertexPair(points[1], edges[1]),
                new VertexPair(points[2], edges[5]),
                new VertexPair(points[3], edges[9]),
                new VertexPair(points[4], edges[13]),
                new VertexPair(points[5], edges[3]),
            }, division.Vertices);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionEdge(0, points[0], edges[1], faces[0], edges[2], edges[13]),
                new SubdivisionEdge(1, points[1], edges[0], faces[1], edges[4], edges[3]),
                new SubdivisionEdge(2, points[1], edges[3], faces[0], edges[15], edges[0]),
                new SubdivisionEdge(3, points[5], edges[2], faces[1], edges[1], edges[6]),
                new SubdivisionEdge(4, points[0], edges[5], faces[1], edges[6], edges[1]),
                new SubdivisionEdge(5, points[2], edges[4], faces[2], edges[8], edges[7]),
                new SubdivisionEdge(6, points[2], edges[7], faces[1], edges[3], edges[4]),
                new SubdivisionEdge(7, points[5], edges[6], faces[2], edges[5], edges[10]),
                new SubdivisionEdge(8, points[0], edges[9], faces[2], edges[10], edges[5]),
                new SubdivisionEdge(9, points[3], edges[8], faces[3], edges[12], edges[11]),
                new SubdivisionEdge(10, points[3], edges[11], faces[2], edges[7], edges[8]),
                new SubdivisionEdge(11, points[5], edges[10], faces[3], edges[9], edges[14]),
                new SubdivisionEdge(12, points[0], edges[13], faces[3], edges[14], edges[9]),
                new SubdivisionEdge(13, points[4], edges[12], faces[0], edges[0], edges[15]),
                new SubdivisionEdge(14, points[4], edges[15], faces[3], edges[11], edges[12]),
                new SubdivisionEdge(15, points[5], edges[14], faces[0], edges[13], edges[2]),
            }, edges);

            CollectionAssert.AreEqual(new[] {
                new SubdivisionFace(division, 0, null, new[] { edges[0] }),
                new SubdivisionFace(division, 1, edges[1], null),
                new SubdivisionFace(division, 2, edges[5], null),
                new SubdivisionFace(division, 3, edges[9], null),
            }, faces);

            CheckFace(edges[0], new[] { points[0], points[1], points[5], points[4] }, -48, PointD.Empty);
            CheckFace(edges[1], new[] { points[1], points[0], points[2], points[5] }, +12, new PointD(-3, 0));
            CheckFace(edges[5], new[] { points[2], points[0], points[3], points[5] }, +24, PointD.Empty);
            CheckFace(edges[9], new[] { points[3], points[0], points[4], points[5] }, +12, new PointD(3, 0));

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
