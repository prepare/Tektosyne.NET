using System;
using System.Collections.Generic;

using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class PointDComparerYTest {

        [Test]
        public void Compare() {
            var comparer = new PointDComparerY();
            Assert.AreEqual(0.0, comparer.Epsilon);
            Assert.AreEqual(0, comparer.Compare(new PointD(1, 2), new PointD(1, 2)));
            Assert.AreEqual(-1, comparer.Compare(new PointD(1, 2), new PointD(2, 2)));
            Assert.AreEqual(-1, comparer.Compare(new PointD(1, 2), new PointD(1, 3)));
            Assert.AreEqual(+1, comparer.Compare(new PointD(1, 2), new PointD(0, 2)));
            Assert.AreEqual(+1, comparer.Compare(new PointD(1, 2), new PointD(1, 1)));

            comparer.Epsilon = 0.01;
            Assert.AreEqual(-1, comparer.Compare(new PointD(1, 2), new PointD(1.1, 2)));
            Assert.AreEqual(-1, comparer.Compare(new PointD(1, 2), new PointD(1, 2.1)));
            Assert.AreEqual(+1, comparer.Compare(new PointD(1, 2), new PointD(0.9, 2)));
            Assert.AreEqual(+1, comparer.Compare(new PointD(1, 2), new PointD(1, 1.9)));

            comparer.Epsilon = 0.5;
            Assert.AreEqual(0, comparer.Compare(new PointD(1, 2), new PointD(1.1, 2)));
            Assert.AreEqual(0, comparer.Compare(new PointD(1, 2), new PointD(1, 2.1)));
            Assert.AreEqual(0, comparer.Compare(new PointD(1, 2), new PointD(0.9, 2)));
            Assert.AreEqual(0, comparer.Compare(new PointD(1, 2), new PointD(1, 1.9)));
        }

        [Test]
        public void CompareEpsilon() {
            var comparer = new PointDComparerY();
            Assert.AreEqual(0.0, comparer.Epsilon);
            Assert.AreEqual(0, comparer.CompareEpsilon(new PointD(1, 2), new PointD(1, 2)));
            Assert.AreEqual(-1, comparer.CompareEpsilon(new PointD(1, 2), new PointD(2, 2)));
            Assert.AreEqual(-1, comparer.CompareEpsilon(new PointD(1, 2), new PointD(1, 3)));
            Assert.AreEqual(+1, comparer.CompareEpsilon(new PointD(1, 2), new PointD(0, 2)));
            Assert.AreEqual(+1, comparer.CompareEpsilon(new PointD(1, 2), new PointD(1, 1)));

            comparer.Epsilon = 0.01;
            Assert.AreEqual(-1, comparer.CompareEpsilon(new PointD(1, 2), new PointD(1.1, 2)));
            Assert.AreEqual(-1, comparer.CompareEpsilon(new PointD(1, 2), new PointD(1, 2.1)));
            Assert.AreEqual(+1, comparer.CompareEpsilon(new PointD(1, 2), new PointD(0.9, 2)));
            Assert.AreEqual(+1, comparer.CompareEpsilon(new PointD(1, 2), new PointD(1, 1.9)));

            comparer.Epsilon = 0.5;
            Assert.AreEqual(0, comparer.CompareEpsilon(new PointD(1, 2), new PointD(1.1, 2)));
            Assert.AreEqual(0, comparer.CompareEpsilon(new PointD(1, 2), new PointD(1, 2.1)));
            Assert.AreEqual(0, comparer.CompareEpsilon(new PointD(1, 2), new PointD(0.9, 2)));
            Assert.AreEqual(0, comparer.CompareEpsilon(new PointD(1, 2), new PointD(1, 1.9)));
        }

        [Test]
        public void CompareEpsilonStatic() {
            Assert.AreEqual(-1, PointDComparerY.CompareEpsilon(new PointD(1, 2), new PointD(1.1, 2), 0.01));
            Assert.AreEqual(-1, PointDComparerY.CompareEpsilon(new PointD(1, 2), new PointD(1, 2.1), 0.01));
            Assert.AreEqual(+1, PointDComparerY.CompareEpsilon(new PointD(1, 2), new PointD(0.9, 2), 0.01));
            Assert.AreEqual(+1, PointDComparerY.CompareEpsilon(new PointD(1, 2), new PointD(1, 1.9), 0.01));

            Assert.AreEqual(0, PointDComparerY.CompareEpsilon(new PointD(1, 2), new PointD(1.1, 2), 0.5));
            Assert.AreEqual(0, PointDComparerY.CompareEpsilon(new PointD(1, 2), new PointD(1, 2.1), 0.5));
            Assert.AreEqual(0, PointDComparerY.CompareEpsilon(new PointD(1, 2), new PointD(0.9, 2), 0.5));
            Assert.AreEqual(0, PointDComparerY.CompareEpsilon(new PointD(1, 2), new PointD(1, 1.9), 0.5));
        }

        [Test]
        public void CompareExact() {
            Assert.AreEqual(0, PointDComparerY.CompareExact(new PointD(1, 2), new PointD(1, 2)));
            Assert.AreEqual(-1, PointDComparerY.CompareExact(new PointD(1, 2), new PointD(2, 2)));
            Assert.AreEqual(-1, PointDComparerY.CompareExact(new PointD(1, 2), new PointD(1, 3)));
            Assert.AreEqual(+1, PointDComparerY.CompareExact(new PointD(1, 2), new PointD(0, 2)));
            Assert.AreEqual(+1, PointDComparerY.CompareExact(new PointD(1, 2), new PointD(1, 1)));

            Assert.AreEqual(+1, PointDComparerY.CompareExact(new PointD(1, 2), new PointD(2, 1)));
            Assert.AreEqual(-1, PointDComparerY.CompareExact(new PointD(2, 1), new PointD(1, 2)));
        }

        [Test]
        public void CompareObject() {
            var comparer = new PointDComparerY();
            Assert.AreEqual(0, comparer.Compare(null, null));
            Assert.AreEqual(-1, comparer.Compare(null, new PointD(1, 1)));
            Assert.AreEqual(+1, comparer.Compare(new PointD(1, 1), null));
        }

        [Test]
        public void CompareObjectFailed() {
            var comparer = new PointDComparerY();
            Assert.Throws<ArgumentException>(() => comparer.Compare("Hello", "World"));
            Assert.Throws<ArgumentException>(() => comparer.Compare("Hello", null));
            Assert.Throws<ArgumentException>(() => comparer.Compare(null, "World"));
            Assert.Throws<ArgumentException>(() => comparer.Compare("Hello", new PointD(1, 1)));
            Assert.Throws<ArgumentException>(() => comparer.Compare(new PointD(1, 1), "World"));
        }

        [Test]
        public void FindNearest() {
            var points = new PointD[100];
            for (int i = 0; i < points.Length; i++)
                points[i] = new PointD(i / 10, i % 10);

            var comparer = new PointDComparerY();
            Array.Sort<PointD>(points, comparer.Compare);

            for (int i = 0; i < points.Length; i++) {
                PointD q = points[i];
                Assert.AreEqual(i, comparer.FindNearest(points, q));

                q = new PointD(q.X + 0.1, q.Y - 0.1);
                Assert.AreEqual(i, comparer.FindNearest(points, q));
            }
        }

        [Test]
        public void FindNearestEpsilon() {
            var points = new PointD[100];
            for (int i = 0; i < points.Length; i++)
                points[i] = new PointD(i / 10, i % 10);

            var comparer = new PointDComparerY() { Epsilon = 0.2 };
            Array.Sort<PointD>(points, comparer.Compare);

            for (int i = 0; i < points.Length; i++) {
                PointD q = points[i];
                Assert.AreEqual(i, comparer.FindNearest(points, q));

                // equality given epsilon = 0.2
                q = new PointD(q.X + 0.1, q.Y - 0.1);
                Assert.AreEqual(i, comparer.FindNearest(points, q));

                // inequality given epsilon = 0.2
                q = new PointD(q.X - 0.4, q.Y + 0.4);
                Assert.AreEqual(i, comparer.FindNearest(points, q));
            }
        }

        [Test]
        public void FindNearestEpsilonOverlap() {
            var points = new PointD[100];
            for (int i = 0; i < points.Length; i++)
                points[i] = new PointD(i / 10, (i % 10) / 10.0);

            // unpredictable sorting since epsilon overlaps point distances
            var comparer = new PointDComparerY() { Epsilon = 0.2 };
            Array.Sort<PointD>(points, comparer.Compare);

            for (int i = 0; i < points.Length; i++) {
                PointD q = points[i];
                Assert.AreEqual(i, comparer.FindNearest(points, q));
                /*
                 * Since epsilon overlaps adjacent points, sorting is unpredictable
                 * and we cannot know which index contains the point nearest to a
                 * non-collection point. So we use brute force to find that index.
                 */
                q = new PointD(q.X + 0.1, q.Y - 0.1);
                int index = GeoAlgorithms.NearestPoint(points, q);
                Assert.AreEqual(index, comparer.FindNearest(points, q));

                q = new PointD(q.X - 0.4, q.Y + 0.4);
                index = GeoAlgorithms.NearestPoint(points, q);
                Assert.AreEqual(index, comparer.FindNearest(points, q));
            }
        }
    }
}
