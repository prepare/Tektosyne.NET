using System;
using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class RectTest {

        RectD rectD = new RectD(1, 2, 4, 5);
        RectF rectF = new RectF(1, 2, 4, 5);
        RectI rectI = new RectI(1, 2, 4, 5);

        [Test]
        public void Constructor() {
            Assert.AreEqual(rectD, new RectD(rectD.TopRight, rectD.BottomLeft));
            Assert.AreEqual(rectF, new RectF(rectF.TopRight, rectF.BottomLeft));

            Assert.Throws<ArgumentOutOfRangeException>(() => { var rect = new RectD(0, 0, -1, 2); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var rect = new RectD(0, 0, 1, -2); });

            Assert.Throws<ArgumentOutOfRangeException>(() => { var rect = new RectF(0, 0, -1, 2); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var rect = new RectF(0, 0, 1, -2); });

            Assert.Throws<ArgumentOutOfRangeException>(() => { var rect = new RectI(0, 0, -1, 2); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var rect = new RectI(0, 0, 1, -2); });
        }

        [Test]
        public void Conversion() {
            Assert.AreEqual(rectD, (RectD) rectF);
            Assert.AreEqual(rectD, (RectD) rectI);

            Assert.AreEqual(rectF, (RectF) rectD);
            Assert.AreEqual(rectF, (RectF) rectI);

            Assert.AreEqual(rectI, (RectI) rectD);
            Assert.AreEqual(rectI, (RectI) rectF);

            Assert.AreEqual(new RectI(0, 1, 3, 4), (RectI) new RectD(0.6, 1.6, 3.6, 4.6));
            Assert.AreEqual(new RectI(0, 1, 3, 4), (RectI) new RectF(0.6f, 1.6f, 3.6f, 4.6f));

            Assert.AreEqual(rectI, (RectI) new RectD(1.4, 2.4, 4.4, 5.4));
            Assert.AreEqual(rectI, (RectI) new RectF(1.4f, 2.4f, 4.4f, 5.4f));
        }

        [Test]
        public void EqualsEpsilon() {
            Assert.IsTrue(RectD.Equals(rectD, new RectD(1.1, 1.9, 3.9, 5.1), 0.2));
            Assert.IsTrue(RectF.Equals(rectF, new RectF(1.1f, 1.9f, 3.9f, 5.1f), 0.2f));
        }

        [Test]
        public void Circumscribe() {
            Assert.AreEqual(rectD, RectD.Circumscribe(
                rectD.TopLeft, rectD.TopRight, rectD.BottomLeft, rectD.BottomRight));

            Assert.AreEqual(rectF, RectF.Circumscribe(
                rectF.TopLeft, rectF.TopRight, rectF.BottomLeft, rectF.BottomRight));

            // RectI.Circumscribe extends one past the specified points
            Assert.AreEqual(new RectI(1, 2, 5, 6), RectI.Circumscribe(
                rectI.TopLeft, rectI.TopRight, rectI.BottomLeft, rectI.BottomRight));
        }

        [Test]
        public void CircumscribeRect() {
            Assert.AreEqual(rectI, rectD.Circumscribe());
            Assert.AreEqual(rectI, new RectD(1.6, 2.4, 3.2, 4.1).Circumscribe());
            Assert.AreEqual(new RectI(-3, -4, 20, 30), new RectD(-2.1, -3.2, 19.8, 29.1).Circumscribe());

            Assert.AreEqual(rectI, rectF.Circumscribe());
            Assert.AreEqual(rectI, new RectF(1.6f, 2.4f, 3.2f, 4.1f).Circumscribe());
            Assert.AreEqual(new RectI(-3, -4, 20, 30), new RectF(-2.1f, -3.2f, 19.8f, 29.1f).Circumscribe());
        }

        [Test]
        public void ContainsPoint() {
            Assert.IsTrue(rectD.Contains(rectD.TopLeft));
            Assert.IsTrue(rectD.Contains(rectD.TopRight));
            Assert.IsTrue(rectD.Contains(rectD.BottomLeft));
            Assert.IsTrue(rectD.Contains(rectD.BottomRight));
            Assert.IsTrue(rectD.Contains(3, 4));
            Assert.IsFalse(rectD.Contains(0, 1));

            Assert.IsTrue(rectF.Contains(rectF.TopLeft));
            Assert.IsTrue(rectF.Contains(rectF.TopRight));
            Assert.IsTrue(rectF.Contains(rectF.BottomLeft));
            Assert.IsTrue(rectF.Contains(rectF.BottomRight));
            Assert.IsTrue(rectF.Contains(3, 4));
            Assert.IsFalse(rectF.Contains(0, 1));

            Assert.IsTrue(rectI.Contains(rectI.TopLeft));
            Assert.IsFalse(rectI.Contains(rectI.TopRight));
            Assert.IsFalse(rectI.Contains(rectI.BottomLeft));
            Assert.IsFalse(rectI.Contains(rectI.BottomRight));
            Assert.IsTrue(rectI.Contains(3, 4));
            Assert.IsFalse(rectI.Contains(0, 1));
        }

        [Test]
        public void ContainsInverse() {
            Assert.IsTrue(rectD.ContainsOpen(rectD.TopLeft));
            Assert.IsFalse(rectD.ContainsOpen(rectD.TopRight));
            Assert.IsFalse(rectD.ContainsOpen(rectD.BottomLeft));
            Assert.IsFalse(rectD.ContainsOpen(rectD.BottomRight));
            Assert.IsTrue(rectD.Contains(3, 4));
            Assert.IsFalse(rectD.ContainsOpen(0, 1));

            Assert.IsTrue(rectF.ContainsOpen(rectF.TopLeft));
            Assert.IsFalse(rectF.ContainsOpen(rectF.TopRight));
            Assert.IsFalse(rectF.ContainsOpen(rectF.BottomLeft));
            Assert.IsFalse(rectF.ContainsOpen(rectF.BottomRight));
            Assert.IsTrue(rectF.Contains(3, 4));
            Assert.IsFalse(rectF.ContainsOpen(0, 1));

            Assert.IsTrue(rectI.ContainsClosed(rectI.TopLeft));
            Assert.IsTrue(rectI.ContainsClosed(rectI.TopRight));
            Assert.IsTrue(rectI.ContainsClosed(rectI.BottomLeft));
            Assert.IsTrue(rectI.ContainsClosed(rectI.BottomRight));
            Assert.IsTrue(rectI.Contains(3, 4));
            Assert.IsFalse(rectI.ContainsClosed(0, 1));
        }

        [Test]
        public void ContainsRect() {
            Assert.IsTrue(rectD.Contains(rectD));
            Assert.IsTrue(rectD.Contains(new RectD(2, 3, 2, 3)));
            Assert.IsFalse(rectD.Contains(new RectD(0, 1, 4, 5)));
            Assert.IsFalse(rectD.Contains(new RectD(10, 2, 4, 5)));

            Assert.IsTrue(rectF.Contains(rectF));
            Assert.IsTrue(rectF.Contains(new RectF(2, 3, 2, 3)));
            Assert.IsFalse(rectF.Contains(new RectF(0, 1, 4, 5)));
            Assert.IsFalse(rectF.Contains(new RectF(10, 2, 4, 5)));

            Assert.IsTrue(rectI.Contains(rectI));
            Assert.IsTrue(rectI.Contains(new RectI(2, 3, 2, 3)));
            Assert.IsFalse(rectI.Contains(new RectI(0, 1, 4, 5)));
            Assert.IsFalse(rectI.Contains(new RectI(10, 2, 4, 5)));
        }

        [Test]
        public void GetDistanceVector() {
            Assert.AreEqual(new PointD(-1, -2), rectD.GetDistanceVector(PointD.Empty));
            Assert.AreEqual(PointD.Empty, rectD.GetDistanceVector(rectD.TopLeft));
            Assert.AreEqual(PointD.Empty, rectD.GetDistanceVector(new PointD(3, 6)));
            Assert.AreEqual(PointD.Empty, rectD.GetDistanceVector(rectD.BottomRight));
            Assert.AreEqual(new PointD(+1, +2), rectD.GetDistanceVector(new PointD(6, 9)));

            Assert.AreEqual(new PointF(-1, -2), rectF.GetDistanceVector(PointF.Empty));
            Assert.AreEqual(PointF.Empty, rectF.GetDistanceVector(rectF.TopLeft));
            Assert.AreEqual(PointF.Empty, rectF.GetDistanceVector(new PointF(3, 6)));
            Assert.AreEqual(PointF.Empty, rectF.GetDistanceVector(rectF.BottomRight));
            Assert.AreEqual(new PointF(+1, +2), rectF.GetDistanceVector(new PointF(6, 9)));

            Assert.AreEqual(new PointI(-1, -2), rectI.GetDistanceVector(PointI.Empty));
            Assert.AreEqual(PointI.Empty, rectI.GetDistanceVector(rectI.TopLeft));
            Assert.AreEqual(PointI.Empty, rectI.GetDistanceVector(new PointI(3, 6)));
            Assert.AreEqual(PointI.Empty, rectI.GetDistanceVector(rectI.BottomRight));
            Assert.AreEqual(new PointI(+1, +2), rectI.GetDistanceVector(new PointI(6, 9)));
        }

        [Test]
        public void IntersectLine() {
            LineD resultD;
            Assert.IsTrue(rectD.Intersect(new LineD(1, 2, 5, 7), out resultD));
            Assert.AreEqual(new LineD(1, 2, 5, 7), resultD);
            Assert.IsTrue(rectD.Intersect(new LineD(2, 6, 4, 3), out resultD));
            Assert.AreEqual(new LineD(2, 6, 4, 3), resultD);
            Assert.IsTrue(rectD.Intersect(new LineD(3, 1, 3, 8), out resultD));
            Assert.AreEqual(new LineD(3, 2, 3, 7), resultD);
            Assert.IsTrue(rectD.Intersect(new LineD(0, 4, 6, 4), out resultD));
            Assert.AreEqual(new LineD(1, 4, 5, 4), resultD);

            Assert.IsFalse(rectD.Intersect(new LineD(0, 1, 0, 8), out resultD));
            Assert.IsFalse(rectD.Intersect(new LineD(0, 1, 6, 1), out resultD));
            Assert.IsFalse(rectD.Intersect(new LineD(6, 1, 6, 8), out resultD));
            Assert.IsFalse(rectD.Intersect(new LineD(0, 8, 6, 8), out resultD));
            Assert.IsFalse(rectD.Intersect(new LineD(-2, 3, 2, -3), out resultD));

            LineF resultF;
            Assert.IsTrue(rectF.Intersect(new LineF(1, 2, 5, 7), out resultF));
            Assert.AreEqual(new LineF(1, 2, 5, 7), resultF);
            Assert.IsTrue(rectF.Intersect(new LineF(2, 6, 4, 3), out resultF));
            Assert.AreEqual(new LineF(2, 6, 4, 3), resultF);
            Assert.IsTrue(rectF.Intersect(new LineF(3, 1, 3, 8), out resultF));
            Assert.AreEqual(new LineF(3, 2, 3, 7), resultF);
            Assert.IsTrue(rectF.Intersect(new LineF(0, 4, 6, 4), out resultF));
            Assert.AreEqual(new LineF(1, 4, 5, 4), resultF);

            Assert.IsFalse(rectF.Intersect(new LineF(0, 1, 0, 8), out resultF));
            Assert.IsFalse(rectF.Intersect(new LineF(0, 1, 6, 1), out resultF));
            Assert.IsFalse(rectF.Intersect(new LineF(6, 1, 6, 8), out resultF));
            Assert.IsFalse(rectF.Intersect(new LineF(0, 8, 6, 8), out resultF));
            Assert.IsFalse(rectF.Intersect(new LineF(-2, 3, 2, -3), out resultF));
        }

        [Test]
        public void IntersectPolygon() {
            PointD[] resultD, polyD = new[] { new PointD(3, 3) };
            Assert.IsTrue(rectD.Intersect(polyD, out resultD));
            CollectionAssert.AreEqual(polyD, resultD);

            polyD = new[] { rectD.BottomLeft, rectD.BottomRight, rectD.TopRight, rectD.TopLeft };
            Assert.IsTrue(rectD.Intersect(polyD, out resultD));
            CollectionAssert.AreEqual(polyD, resultD);

            polyD = new[] { rectD.BottomLeft, rectD.TopRight, rectD.TopLeft };
            Assert.IsTrue(rectD.Intersect(polyD, out resultD));
            CollectionAssert.AreEqual(polyD, resultD);

            polyD = new[] { new PointD(0, 1), new PointD(6, 1), new PointD(6, 8), new PointD(0, 8) };
            Assert.IsTrue(rectD.Intersect(polyD, out resultD));
            CollectionAssert.AreEqual(new[] {
                rectD.BottomLeft, rectD.TopLeft, rectD.TopRight, rectD.BottomRight }, resultD);

            polyD = new[] { new PointD(2, 0), new PointD(4, 0), new PointD(4, 8), new PointD(2, 8) };
            Assert.IsTrue(rectD.Intersect(polyD, out resultD));
            CollectionAssert.AreEqual(new[] {
                new PointD(2, 7), new PointD(2, 2), new PointD(4, 2), new PointD(4, 7) }, resultD);

            polyD = new[] { new PointD(0, 3), new PointD(0, 6), new PointD(6, 6), new PointD(6, 3) };
            Assert.IsTrue(rectD.Intersect(polyD, out resultD));
            CollectionAssert.AreEqual(new[] {
                new PointD(5, 3), new PointD(1, 3), new PointD(1, 6), new PointD(5, 6) }, resultD);

            polyD = new[] { new PointD(6, 3), new PointD(6, 6), new PointD(8, 5) };
            Assert.IsFalse(rectD.Intersect(polyD, out resultD));
            CollectionAssert.AreEqual(new PointD[] { }, resultD);

            PointF[] resultF, polyF = new[] { new PointF(3, 3) };
            Assert.IsTrue(rectF.Intersect(polyF, out resultF));
            CollectionAssert.AreEqual(polyF, resultF);

            polyF = new[] { rectF.BottomLeft, rectF.BottomRight, rectF.TopRight, rectF.TopLeft };
            Assert.IsTrue(rectF.Intersect(polyF, out resultF));
            CollectionAssert.AreEqual(polyF, resultF);

            polyF = new[] { rectF.BottomLeft, rectF.TopRight, rectF.TopLeft };
            Assert.IsTrue(rectF.Intersect(polyF, out resultF));
            CollectionAssert.AreEqual(polyF, resultF);

            polyF = new[] { new PointF(0, 1), new PointF(6, 1), new PointF(6, 8), new PointF(0, 8) };
            Assert.IsTrue(rectF.Intersect(polyF, out resultF));
            CollectionAssert.AreEqual(new[] {
                rectF.BottomLeft, rectF.TopLeft, rectF.TopRight, rectF.BottomRight }, resultF);

            polyF = new[] { new PointF(2, 0), new PointF(4, 0), new PointF(4, 8), new PointF(2, 8) };
            Assert.IsTrue(rectF.Intersect(polyF, out resultF));
            CollectionAssert.AreEqual(new[] {
                new PointF(2, 7), new PointF(2, 2), new PointF(4, 2), new PointF(4, 7) }, resultF);

            polyF = new[] { new PointF(0, 3), new PointF(0, 6), new PointF(6, 6), new PointF(6, 3) };
            Assert.IsTrue(rectF.Intersect(polyF, out resultF));
            CollectionAssert.AreEqual(new[] {
                new PointF(5, 3), new PointF(1, 3), new PointF(1, 6), new PointF(5, 6) }, resultF);

            polyF = new[] { new PointF(6, 3), new PointF(6, 6), new PointF(8, 5) };
            Assert.IsFalse(rectF.Intersect(polyF, out resultF));
            CollectionAssert.AreEqual(new PointF[] { }, resultF);
        }

        [Test]
        public void IntersectRect() {
            RectD resultD;
            Assert.IsTrue(rectD.Intersect(rectD, out resultD));
            Assert.AreEqual(rectD, resultD);
            Assert.IsTrue(rectD.Intersect(new RectD(2, 3, 2, 3), out resultD));
            Assert.AreEqual(new RectD(2, 3, 2, 3), resultD);
            Assert.IsTrue(rectD.Intersect(new RectD(0, 1, 4, 5), out resultD));
            Assert.AreEqual(new RectD(1, 2, 3, 4), resultD);
            Assert.IsFalse(rectD.Intersect(new RectD(10, 3, 4, 5), out resultD));

            RectF resultF;
            Assert.IsTrue(rectF.Intersect(rectF, out resultF));
            Assert.AreEqual(rectF, resultF);
            Assert.IsTrue(rectF.Intersect(new RectF(2, 3, 2, 3), out resultF));
            Assert.AreEqual(new RectF(2, 3, 2, 3), resultF);
            Assert.IsTrue(rectF.Intersect(new RectF(0, 1, 4, 5), out resultF));
            Assert.AreEqual(new RectF(1, 2, 3, 4), resultF);
            Assert.IsFalse(rectF.Intersect(new RectF(10, 3, 4, 5), out resultF));
        }

        [Test]
        public void IntersectsWithLine() {
            Assert.IsTrue(rectD.IntersectsWith(new LineD(1, 2, 5, 7)));
            Assert.IsTrue(rectD.IntersectsWith(new LineD(3, 1, 3, 8)));
            Assert.IsTrue(rectD.IntersectsWith(new LineD(0, 4, 6, 4)));
            Assert.IsFalse(rectD.IntersectsWith(new LineD(0, 1, 0, 8)));
            Assert.IsFalse(rectD.IntersectsWith(new LineD(0, 1, 6, 1)));
            Assert.IsFalse(rectD.IntersectsWith(new LineD(-2, 3, 2, -3)));

            Assert.IsTrue(rectF.IntersectsWith(new LineF(1, 2, 5, 7)));
            Assert.IsTrue(rectF.IntersectsWith(new LineF(3, 1, 3, 8)));
            Assert.IsTrue(rectF.IntersectsWith(new LineF(0, 4, 6, 4)));
            Assert.IsFalse(rectF.IntersectsWith(new LineF(0, 1, 0, 8)));
            Assert.IsFalse(rectF.IntersectsWith(new LineF(0, 1, 6, 1)));
            Assert.IsFalse(rectF.IntersectsWith(new LineF(-2, 3, 2, -3)));
        }

        [Test]
        public void IntersectsWithRect() {
            Assert.IsTrue(rectD.IntersectsWith(rectD));
            Assert.IsTrue(rectD.IntersectsWith(new RectD(2, 3, 2, 3)));
            Assert.IsTrue(rectD.IntersectsWith(new RectD(0, 1, 4, 5)));
            Assert.IsFalse(rectD.IntersectsWith(new RectD(10, 3, 4, 5)));

            Assert.IsTrue(rectF.IntersectsWith(rectF));
            Assert.IsTrue(rectF.IntersectsWith(new RectF(2, 3, 2, 3)));
            Assert.IsTrue(rectF.IntersectsWith(new RectF(0, 1, 4, 5)));
            Assert.IsFalse(rectF.IntersectsWith(new RectF(10, 3, 4, 5)));

            Assert.IsTrue(rectI.IntersectsWith(rectI));
            Assert.IsTrue(rectI.IntersectsWith(new RectI(2, 3, 2, 3)));
            Assert.IsTrue(rectI.IntersectsWith(new RectI(0, 1, 4, 5)));
            Assert.IsFalse(rectI.IntersectsWith(new RectI(10, 3, 4, 5)));
        }

        [Test]
        public void Locate() {
            Assert.AreEqual(RectLocation.BeforeX | RectLocation.BeforeY, rectD.Locate(new PointD(0, 1)));
            Assert.AreEqual(RectLocation.StartX | RectLocation.StartY, rectD.Locate(new PointD(1, 2)));
            Assert.AreEqual(RectLocation.InsideX | RectLocation.InsideY, rectD.Locate(new PointD(3, 4)));
            Assert.AreEqual(RectLocation.EndX | RectLocation.EndY, rectD.Locate(new PointD(5, 7)));
            Assert.AreEqual(RectLocation.AfterX | RectLocation.AfterY, rectD.Locate(new PointD(6, 8)));

            Assert.AreEqual(RectLocation.BeforeX | RectLocation.BeforeY, rectF.Locate(new PointF(0, 1)));
            Assert.AreEqual(RectLocation.StartX | RectLocation.StartY, rectF.Locate(new PointF(1, 2)));
            Assert.AreEqual(RectLocation.InsideX | RectLocation.InsideY, rectF.Locate(new PointF(3, 4)));
            Assert.AreEqual(RectLocation.EndX | RectLocation.EndY, rectF.Locate(new PointF(5, 7)));
            Assert.AreEqual(RectLocation.AfterX | RectLocation.AfterY, rectF.Locate(new PointF(6, 8)));

            Assert.AreEqual(RectLocation.BeforeX | RectLocation.BeforeY, rectI.Locate(new PointI(0, 1)));
            Assert.AreEqual(RectLocation.StartX | RectLocation.StartY, rectI.Locate(new PointI(1, 2)));
            Assert.AreEqual(RectLocation.InsideX | RectLocation.InsideY, rectI.Locate(new PointI(3, 4)));
            Assert.AreEqual(RectLocation.EndX | RectLocation.EndY, rectI.Locate(new PointI(5 - 1, 7 - 1)));
            Assert.AreEqual(RectLocation.AfterX | RectLocation.AfterY, rectI.Locate(new PointI(6, 8)));
        }

        [Test]
        public void LocateEpsilon() {
            Assert.AreEqual(RectLocation.BeforeX | RectLocation.BeforeY, rectD.Locate(new PointD(0.1, 0.9), 0.2));
            Assert.AreEqual(RectLocation.StartX | RectLocation.StartY, rectD.Locate(new PointD(0.9, 2.1), 0.2));
            Assert.AreEqual(RectLocation.InsideX | RectLocation.InsideY, rectD.Locate(new PointD(3.1, 3.9), 0.2));
            Assert.AreEqual(RectLocation.EndX | RectLocation.EndY, rectD.Locate(new PointD(4.9, 7.1), 0.2));
            Assert.AreEqual(RectLocation.AfterX | RectLocation.AfterY, rectD.Locate(new PointD(5.9, 8.1), 0.2));

            Assert.AreEqual(RectLocation.BeforeX | RectLocation.BeforeY, rectF.Locate(new PointF(0.1f, 0.9f), 0.2f));
            Assert.AreEqual(RectLocation.StartX | RectLocation.StartY, rectF.Locate(new PointF(0.9f, 2.1f), 0.2f));
            Assert.AreEqual(RectLocation.InsideX | RectLocation.InsideY, rectF.Locate(new PointF(3.1f, 3.9f), 0.2f));
            Assert.AreEqual(RectLocation.EndX | RectLocation.EndY, rectF.Locate(new PointF(4.9f, 7.1f), 0.2f));
            Assert.AreEqual(RectLocation.AfterX | RectLocation.AfterY, rectF.Locate(new PointF(5.9f, 8.1f), 0.2f));
        }

        [Test]
        public void LocateInverse() {
            Assert.AreEqual(RectLocation.BeforeX | RectLocation.BeforeY, rectI.LocateClosed(new PointI(0, 1)));
            Assert.AreEqual(RectLocation.StartX | RectLocation.StartY, rectI.LocateClosed(new PointI(1, 2)));
            Assert.AreEqual(RectLocation.InsideX | RectLocation.InsideY, rectI.LocateClosed(new PointI(3, 4)));
            Assert.AreEqual(RectLocation.EndX | RectLocation.EndY, rectI.LocateClosed(new PointI(5, 7)));
            Assert.AreEqual(RectLocation.AfterX | RectLocation.AfterY, rectI.LocateClosed(new PointI(6, 8)));
        }

        [Test]
        public void Offset() {
            Assert.AreEqual(new RectD(4, 6, 4, 5), rectD.Offset(new PointD(3, 4)));
            Assert.AreEqual(new RectF(4, 6, 4, 5), rectF.Offset(new PointF(3, 4)));
            Assert.AreEqual(new RectI(4, 6, 4, 5), rectI.Offset(new PointI(3, 4)));
        }

        [Test]
        public void Round() {
            Assert.AreEqual(rectI, rectD.Round());
            Assert.AreEqual(rectI, new RectD(0.6, 1.6, 3.6, 4.6).Round());
            Assert.AreEqual(rectI, new RectD(1.4, 2.4, 4.4, 5.4).Round());

            Assert.AreEqual(rectI, rectF.Round());
            Assert.AreEqual(rectI, new RectF(0.6f, 1.6f, 3.6f, 4.6f).Round());
            Assert.AreEqual(rectI, new RectF(1.4f, 2.4f, 4.4f, 5.4f).Round());
        }

        [Test]
        public void Union() {
            Assert.AreEqual(rectD, rectD.Union(rectD));
            Assert.AreEqual(rectD, rectD.Union(new RectD(2, 3, 2, 3)));
            Assert.AreEqual(new RectD(0, 1, 5, 6), rectD.Union(new RectD(0, 1, 4, 5)));
            Assert.AreEqual(new RectD(1, 2, 13, 6), rectD.Union(new RectD(10, 3, 4, 5)));

            Assert.AreEqual(rectF, rectF.Union(rectF));
            Assert.AreEqual(rectF, rectF.Union(new RectF(2, 3, 2, 3)));
            Assert.AreEqual(new RectF(0, 1, 5, 6), rectF.Union(new RectF(0, 1, 4, 5)));
            Assert.AreEqual(new RectF(1, 2, 13, 6), rectF.Union(new RectF(10, 3, 4, 5)));

            Assert.AreEqual(rectI, rectI.Union(rectI));
            Assert.AreEqual(rectI, rectI.Union(new RectI(2, 3, 2, 3)));
            Assert.AreEqual(new RectI(0, 1, 5, 6), rectI.Union(new RectI(0, 1, 4, 5)));
            Assert.AreEqual(new RectI(1, 2, 13, 6), rectI.Union(new RectI(10, 3, 4, 5)));
        }
    }
}
