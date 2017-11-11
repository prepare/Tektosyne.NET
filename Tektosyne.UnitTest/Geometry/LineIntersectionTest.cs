using System;
using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class LineIntersectionTest {

        [Test]
        public void Intersect() {

            var result = new LineD(0, 0, 0.9, 0.9).Intersect(new LineD(1, 1, 2, 2));
            Assert.AreEqual(null, result.Shared);
            Assert.AreEqual(LineLocation.None, result.First);
            Assert.AreEqual(LineLocation.None, result.Second);
            Assert.AreEqual(LineRelation.Collinear, result.Relation);

            result = new LineD(0, 1, 2, 3).Intersect(new LineD(1, 0, 3, 2));
            Assert.AreEqual(null, result.Shared);
            Assert.AreEqual(LineLocation.None, result.First);
            Assert.AreEqual(LineLocation.None, result.Second);
            Assert.AreEqual(LineRelation.Parallel, result.Relation);

            result = new LineD(0, 0, 0.9, 0.9).Intersect(new LineD(0, 2, 0.9, 1.1));
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.After, result.First);
            Assert.AreEqual(LineLocation.After, result.Second);
            Assert.AreEqual(LineRelation.Divergent, result.Relation);

            result = new LineD(0.9, 0.9, 0, 0).Intersect(new LineD(1.1, 0.9, 2, 0));
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.Before, result.First);
            Assert.AreEqual(LineLocation.Before, result.Second);
            Assert.AreEqual(LineRelation.Divergent, result.Relation);
        }

        [Test]
        public void IntersectCollinear() {

            var result = new LineD(1, 1, 0, 0).Intersect(new LineD(1, 1, 0, 0));
            Assert.IsTrue(PointD.Equals(new PointD(0, 0), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.End, result.First);
            Assert.AreEqual(LineLocation.End, result.Second);
            Assert.AreEqual(LineRelation.Collinear, result.Relation);

            result = new LineD(0, 0, 1, 1).Intersect(new LineD(1, 1, 2, 2));
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.End, result.First);
            Assert.AreEqual(LineLocation.Start, result.Second);
            Assert.AreEqual(LineRelation.Collinear, result.Relation);

            result = new LineD(1, 1, 0, 0).Intersect(new LineD(2, 2, 1, 1));
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.Start, result.First);
            Assert.AreEqual(LineLocation.End, result.Second);
            Assert.AreEqual(LineRelation.Collinear, result.Relation);

            result = new LineD(0, 0, 2, 2).Intersect(new LineD(3, 3, 1, 1));
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.Between, result.First);
            Assert.AreEqual(LineLocation.End, result.Second);
            Assert.AreEqual(LineRelation.Collinear, result.Relation);
        }

        [Test]
        public void IntersectDivergent() {

            var result = new LineD(0, 0, 1, 1).Intersect(new LineD(1, 1, 2, 0));
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.End, result.First);
            Assert.AreEqual(LineLocation.Start, result.Second);
            Assert.AreEqual(LineRelation.Divergent, result.Relation);

            result = new LineD(1, 1, 0, 0).Intersect(new LineD(2, 0, 1, 1));
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.Start, result.First);
            Assert.AreEqual(LineLocation.End, result.Second);
            Assert.AreEqual(LineRelation.Divergent, result.Relation);

            result = new LineD(0, 0, 2, 2).Intersect(new LineD(1, 1, 2, 0));
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.Between, result.First);
            Assert.AreEqual(LineLocation.Start, result.Second);
            Assert.AreEqual(LineRelation.Divergent, result.Relation);

            result = new LineD(0, 0, 1, 1).Intersect(new LineD(0, 2, 2, 0));
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.End, result.First);
            Assert.AreEqual(LineLocation.Between, result.Second);
            Assert.AreEqual(LineRelation.Divergent, result.Relation);

            result = new LineD(0, 0, 2, 2).Intersect(new LineD(0, 2, 2, 0));
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.Between, result.First);
            Assert.AreEqual(LineLocation.Between, result.Second);
            Assert.AreEqual(LineRelation.Divergent, result.Relation);
        }

        [Test]
        public void IntersectEpsilon() {

            var result = new LineD(0, 0, 0.9, 0.9).Intersect(new LineD(1, 1, 2, 2), 0.01);
            Assert.AreEqual(null, result.Shared);
            Assert.AreEqual(LineLocation.None, result.First);
            Assert.AreEqual(LineLocation.None, result.Second);
            Assert.AreEqual(LineRelation.Collinear, result.Relation);

            result = new LineD(0, 0, 0.9, 0.9).Intersect(new LineD(1, 1, 2, 2), 0.5);
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.End, result.First);
            Assert.AreEqual(LineLocation.Start, result.Second);
            Assert.AreEqual(LineRelation.Collinear, result.Relation);

            result = new LineD(0.9, 0.9, 0.1, 0.1).Intersect(new LineD(1.1, 1.1, 0, 0), 0.5);
            Assert.IsTrue(PointD.Equals(new PointD(0, 0), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.End, result.First);
            Assert.AreEqual(LineLocation.End, result.Second);
            Assert.AreEqual(LineRelation.Collinear, result.Relation);

            result = new LineD(0, 0, 0.9, 0.9).Intersect(new LineD(1, 1, 2, 0), 0.01);
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.After, result.First);
            Assert.AreEqual(LineLocation.Start, result.Second);
            Assert.AreEqual(LineRelation.Divergent, result.Relation);

            result = new LineD(0, 0, 0.9, 0.9).Intersect(new LineD(1, 1, 2, 0), 0.5);
            Assert.IsTrue(PointD.Equals(new PointD(0.9, 0.9), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.End, result.First);
            Assert.AreEqual(LineLocation.Start, result.Second);
            Assert.AreEqual(LineRelation.Divergent, result.Relation);

            result = new LineD(0, 0, 2, 2).Intersect(new LineD(0.9, 1.1, 2, 0), 0.01);
            Assert.IsTrue(PointD.Equals(new PointD(1, 1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.Between, result.First);
            Assert.AreEqual(LineLocation.Between, result.Second);
            Assert.AreEqual(LineRelation.Divergent, result.Relation);

            result = new LineD(0, 0, 2, 2).Intersect(new LineD(0.9, 1.1, 2, 0), 0.5);
            Assert.IsTrue(PointD.Equals(new PointD(0.9, 1.1), result.Shared.Value, 0.01));
            Assert.AreEqual(LineLocation.Between, result.First);
            Assert.AreEqual(LineLocation.Start, result.Second);
            Assert.AreEqual(LineRelation.Divergent, result.Relation);
        }
    }
}
