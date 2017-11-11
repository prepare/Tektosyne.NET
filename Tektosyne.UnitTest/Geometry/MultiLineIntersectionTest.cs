using System;
using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class MultiLineIntersectionTest {

        [Test]
        public void Central() {
            LineD[] lines = new LineD[100];
            for (int i = 0; i < lines.Length; i++)
                lines[i] = new LineD(i * 10, 0, 1000 - i * 10, 1000);

            var results = FindBoth(lines);
            Assert.AreEqual(1, results.Length);
            Assert.AreEqual(lines.Length, results[0].Lines.Length);
            Assert.AreEqual(new PointD(500, 500), results[0].Shared);
            foreach (var location in results[0].Locations)
                Assert.AreEqual(LineLocation.Between, location);
        }

        [Test]
        public void Collinear() {
            LineD[] lines = new LineD[] {
                new LineD(0, 0, 3, 0), new LineD(1, 0, 4, 0),
                new LineD(0, 1, 0, 3), new LineD(0, 2, 0, 4),
                new LineD(3, 3, 1, 1), new LineD(2, 2, 4, 4),
            };

            var results = FindBoth(lines);
            Assert.AreEqual(3, results.Length);

            LineD[] sharedEndLines = new LineD[] {
                new LineD(0, 0, 3, 0), new LineD(1, 0, 3, 0),
                new LineD(0, 1, 0, 3), new LineD(0, 2, 0, 3),
                new LineD(3, 3, 1, 1), new LineD(2, 2, 3, 3),
            };

            var sharedEndResults = FindBoth(sharedEndLines);
            CompareResults(results, sharedEndResults);

            var result = results[0];
            Assert.AreEqual(2, result.Lines.Length);
            Assert.AreEqual(new PointD(1, 0), result.Shared);
            Assert.AreEqual(0, result.Lines[0]);
            Assert.AreEqual(LineLocation.Between, result.Locations[0]);
            Assert.AreEqual(1, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);

            result = results[1];
            Assert.AreEqual(2, result.Lines.Length);
            Assert.AreEqual(new PointD(0, 2), result.Shared);
            Assert.AreEqual(2, result.Lines[0]);
            Assert.AreEqual(LineLocation.Between, result.Locations[0]);
            Assert.AreEqual(3, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);

            result = results[2];
            Assert.AreEqual(2, result.Lines.Length);
            Assert.AreEqual(new PointD(2, 2), result.Shared);
            Assert.AreEqual(4, result.Lines[0]);
            Assert.AreEqual(LineLocation.Between, result.Locations[0]);
            Assert.AreEqual(5, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);
        }

        [Test]
        public void Congruent() {
            LineD[] lines = new LineD[] {
                new LineD(0, 0, 3, 0), new LineD(0, 0, 4, 0),
                new LineD(0, 1, 0, 3), new LineD(0, 4, 0, 1),
                new LineD(3, 3, 1, 1), new LineD(4, 4, 1, 1),
            };

            var results = FindBoth(lines);
            Assert.AreEqual(3, results.Length);

            LineD[] sharedEndLines = new LineD[] {
                new LineD(0, 0, 3, 0), new LineD(0, 0, 3, 0),
                new LineD(0, 1, 0, 3), new LineD(0, 3, 0, 1),
                new LineD(3, 3, 1, 1), new LineD(3, 3, 1, 1),
            };

            var sharedEndResults = FindBoth(sharedEndLines);
            CompareResults(results, sharedEndResults);

            var result = results[0];
            Assert.AreEqual(2, result.Lines.Length);
            Assert.AreEqual(new PointD(0, 0), result.Shared);
            Assert.AreEqual(0, result.Lines[0]);
            Assert.AreEqual(LineLocation.Start, result.Locations[0]);
            Assert.AreEqual(1, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);

            result = results[1];
            Assert.AreEqual(2, result.Lines.Length);
            Assert.AreEqual(new PointD(0, 1), result.Shared);
            Assert.AreEqual(2, result.Lines[0]);
            Assert.AreEqual(LineLocation.Start, result.Locations[0]);
            Assert.AreEqual(3, result.Lines[1]);
            Assert.AreEqual(LineLocation.End, result.Locations[1]);

            result = results[2];
            Assert.AreEqual(2, result.Lines.Length);
            Assert.AreEqual(new PointD(1, 1), result.Shared);
            Assert.AreEqual(4, result.Lines[0]);
            Assert.AreEqual(LineLocation.End, result.Locations[0]);
            Assert.AreEqual(5, result.Lines[1]);
            Assert.AreEqual(LineLocation.End, result.Locations[1]);
        }

        [Test]
        public void CongruentShared() {
            LineD[] lines = new LineD[] {
                new LineD(3, 3, 0, 0), new LineD(0, 0, 3, 3),
                new LineD(0, 1, 3, 3), new LineD(3, 3, 0, 1)
            };

            var results = FindBoth(lines);
            Assert.AreEqual(3, results.Length);

            var result = results[0];
            Assert.AreEqual(2, result.Lines.Length);
            Assert.AreEqual(new PointD(0, 0), result.Shared);
            Assert.AreEqual(0, result.Lines[0]);
            Assert.AreEqual(LineLocation.End, result.Locations[0]);
            Assert.AreEqual(1, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);

            result = results[1];
            Assert.AreEqual(2, result.Lines.Length);
            Assert.AreEqual(new PointD(0, 1), result.Shared);
            Assert.AreEqual(2, result.Lines[0]);
            Assert.AreEqual(LineLocation.Start, result.Locations[0]);
            Assert.AreEqual(3, result.Lines[1]);
            Assert.AreEqual(LineLocation.End, result.Locations[1]);

            result = results[2];
            Assert.AreEqual(4, result.Lines.Length);
            Assert.AreEqual(new PointD(3, 3), result.Shared);
            Assert.AreEqual(0, result.Lines[0]);
            Assert.AreEqual(LineLocation.Start, result.Locations[0]);
            Assert.AreEqual(1, result.Lines[1]);
            Assert.AreEqual(LineLocation.End, result.Locations[1]);
            Assert.AreEqual(2, result.Lines[2]);
            Assert.AreEqual(LineLocation.End, result.Locations[2]);
            Assert.AreEqual(3, result.Lines[3]);
            Assert.AreEqual(LineLocation.Start, result.Locations[3]);
        }

        [Test]
        public void Empty() {
            LineD[] lines = new LineD[] { };
            var results = FindBoth(lines);
            Assert.AreEqual(0, results.Length);

            lines = new LineD[] { new LineD(1, 2, 3, 4) };
            results = FindBoth(lines);
            Assert.AreEqual(0, results.Length);
        }

        [Test]
        public void StartEnd() {
            LineD[] lines = new LineD[] {
                new LineD(0, 0, 5, 0), new LineD(5, 0, 5, 5),
                new LineD(5, 5, 0, 5), new LineD(0, 5, 0, 0)
            };

            var results = FindBoth(lines);
            Assert.AreEqual(4, results.Length);
            for (int i = 0; i < results.Length; i++)
                Assert.AreEqual(2, results[i].Lines.Length);

            MultiLinePoint result = results[0];
            Assert.AreEqual(new PointD(0, 0), result.Shared);
            Assert.AreEqual(0, result.Lines[0]);
            Assert.AreEqual(LineLocation.Start, result.Locations[0]);
            Assert.AreEqual(3, result.Lines[1]);
            Assert.AreEqual(LineLocation.End, result.Locations[1]);

            result = results[1];
            Assert.AreEqual(new PointD(5, 0), result.Shared);
            Assert.AreEqual(0, result.Lines[0]);
            Assert.AreEqual(LineLocation.End, result.Locations[0]);
            Assert.AreEqual(1, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);

            result = results[2];
            Assert.AreEqual(new PointD(0, 5), result.Shared);
            Assert.AreEqual(2, result.Lines[0]);
            Assert.AreEqual(LineLocation.End, result.Locations[0]);
            Assert.AreEqual(3, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);

            result = results[3];
            Assert.AreEqual(new PointD(5, 5), result.Shared);
            Assert.AreEqual(1, result.Lines[0]);
            Assert.AreEqual(LineLocation.End, result.Locations[0]);
            Assert.AreEqual(2, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);
        }

        [Test]
        public void StartBetween() {
            LineD[] lines = new LineD[] {
                new LineD(0, 0, 6, 0), new LineD(5, 0, 5, 6),
                new LineD(5, 5, -1, 5), new LineD(0, 5, 0, -1)
            };

            var results = FindBoth(lines);
            Assert.AreEqual(4, results.Length);
            for (int i = 0; i < results.Length; i++)
                Assert.AreEqual(2, results[i].Lines.Length);

            MultiLinePoint result = results[0];
            Assert.AreEqual(new PointD(0, 0), result.Shared);
            Assert.AreEqual(0, result.Lines[0]);
            Assert.AreEqual(LineLocation.Start, result.Locations[0]);
            Assert.AreEqual(3, result.Lines[1]);
            Assert.AreEqual(LineLocation.Between, result.Locations[1]);

            result = results[1];
            Assert.AreEqual(new PointD(5, 0), result.Shared);
            Assert.AreEqual(0, result.Lines[0]);
            Assert.AreEqual(LineLocation.Between, result.Locations[0]);
            Assert.AreEqual(1, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);

            result = results[2];
            Assert.AreEqual(new PointD(0, 5), result.Shared);
            Assert.AreEqual(2, result.Lines[0]);
            Assert.AreEqual(LineLocation.Between, result.Locations[0]);
            Assert.AreEqual(3, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);

            result = results[3];
            Assert.AreEqual(new PointD(5, 5), result.Shared);
            Assert.AreEqual(1, result.Lines[0]);
            Assert.AreEqual(LineLocation.Between, result.Locations[0]);
            Assert.AreEqual(2, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);
        }

        [Test]
        public void EndBetween() {
            LineD[] lines = new LineD[] {
                new LineD(6, 0, 0, 0), new LineD(5, 6, 5, 0),
                new LineD(-1, 5, 5, 5), new LineD(0, -1, 0, 5)
            };

            var results = FindBoth(lines);
            Assert.AreEqual(4, results.Length);
            for (int i = 0; i < results.Length; i++)
                Assert.AreEqual(2, results[i].Lines.Length);

            var result = results[0];
            Assert.AreEqual(new PointD(0, 0), result.Shared);
            Assert.AreEqual(0, result.Lines[0]);
            Assert.AreEqual(LineLocation.End, result.Locations[0]);
            Assert.AreEqual(3, result.Lines[1]);
            Assert.AreEqual(LineLocation.Between, result.Locations[1]);

            result = results[1];
            Assert.AreEqual(new PointD(5, 0), result.Shared);
            Assert.AreEqual(0, result.Lines[0]);
            Assert.AreEqual(LineLocation.Between, result.Locations[0]);
            Assert.AreEqual(1, result.Lines[1]);
            Assert.AreEqual(LineLocation.End, result.Locations[1]);

            result = results[2];
            Assert.AreEqual(new PointD(0, 5), result.Shared);
            Assert.AreEqual(2, result.Lines[0]);
            Assert.AreEqual(LineLocation.Between, result.Locations[0]);
            Assert.AreEqual(3, result.Lines[1]);
            Assert.AreEqual(LineLocation.End, result.Locations[1]);

            result = results[3];
            Assert.AreEqual(new PointD(5, 5), result.Shared);
            Assert.AreEqual(1, result.Lines[0]);
            Assert.AreEqual(LineLocation.Between, result.Locations[0]);
            Assert.AreEqual(2, result.Lines[1]);
            Assert.AreEqual(LineLocation.End, result.Locations[1]);
        }

        [Test]
        public void Epsilon() {
            LineD[] lines = new LineD[] { new LineD(0, 2, 5, 2), new LineD(3, 2.1, 5, 4) };
            var results = FindBoth(lines);
            Assert.AreEqual(0, results.Length);
            results = MultiLineIntersection.FindSimple(lines, 1.0);
            Assert.AreEqual(1, results.Length);

            var result = results[0];
            Assert.IsTrue(PointD.Equals(new PointD(3, 2), result.Shared, 1.0));
            Assert.AreEqual(2, result.Lines.Length);
            Assert.AreEqual(0, result.Lines[0]);
            Assert.AreEqual(LineLocation.Between, result.Locations[0]);
            Assert.AreEqual(1, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);

            lines = new LineD[] { new LineD(3, 1, 1, 1), new LineD(1, 1.1, 3, 3), new LineD(1, 0.9, 3, -2) };
            results = FindBoth(lines);
            Assert.AreEqual(0, results.Length);
            results = MultiLineIntersection.FindSimple(lines, 1.0);
            Assert.AreEqual(1, results.Length);

            result = results[0];
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared, 1.0));
            Assert.AreEqual(3, result.Lines.Length);
            Assert.AreEqual(0, result.Lines[0]);
            Assert.AreEqual(LineLocation.End, result.Locations[0]);
            Assert.AreEqual(1, result.Lines[1]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);
            Assert.AreEqual(2, result.Lines[2]);
            Assert.AreEqual(LineLocation.Start, result.Locations[1]);
        }

        [Test]
        public void Random() {
            LineD[] lines = new LineD[100];
            for (int i = 0; i < lines.Length; i++)
                lines[i] = GeoAlgorithms.RandomLine(0, 0, 1000000, 1000000);

            FindBoth(lines);
        }
    
        private MultiLinePoint[] FindBoth(LineD[] lines) {
            var brute = MultiLineIntersection.FindSimple(lines);
            var sweep = MultiLineIntersection.Find(lines);
            CompareResults(brute, sweep);
            return brute;
        }

        private void CompareResults(MultiLinePoint[] brute, MultiLinePoint[] sweep) {
            Assert.AreEqual(brute.Length, sweep.Length);
            for (int i = 0; i < sweep.Length; i++)
                CompareResultPoints(brute[i], sweep[i]);
        }

        private void CompareResultPoints(MultiLinePoint brute, MultiLinePoint sweep) {
            Assert.IsTrue(PointD.Equals(brute.Shared, sweep.Shared, 1e-6));

            Int32[] bruteLines = brute.Lines, sweepLines = sweep.Lines;
            LineLocation[] bruteLocations = brute.Locations, sweepLocations = sweep.Locations;
            Assert.AreEqual(bruteLines.Length, bruteLocations.Length);
            Assert.AreEqual(sweepLines.Length, sweepLocations.Length);

            Array.Sort(bruteLines, bruteLocations); Array.Sort(sweepLines, sweepLocations);
            CollectionAssert.AreEqual(bruteLines, sweepLines);
            CollectionAssert.AreEqual(bruteLocations, sweepLocations);
        }
    }
}
