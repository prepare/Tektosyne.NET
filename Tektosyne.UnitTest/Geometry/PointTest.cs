using System;
using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class PointTest {

        PointD pointD = new PointD(1, 2);
        PointF pointF = new PointF(1, 2);
        PointI pointI = new PointI(1, 2);

        [Test]
        public void AngleTest() {
            const double epsilon = 0.0001;
            const double angle = 63.435; // polar angle of (1,2)

            Assert.AreEqual(angle, pointD.Angle * Angle.RadiansToDegrees, epsilon);
            Assert.AreEqual(angle, pointF.Angle * Angle.RadiansToDegrees, epsilon);
            Assert.AreEqual(angle, pointI.Angle * Angle.RadiansToDegrees, epsilon);
        }

        [Test]
        public void Length() {
            Assert.AreEqual(Math.Sqrt(5), pointD.Length);
            Assert.AreEqual(Math.Sqrt(5), pointF.Length);
            Assert.AreEqual(Math.Sqrt(5), pointI.Length);
        }

        [Test]
        public void LengthSquared() {
            Assert.AreEqual(5, pointD.LengthSquared);
            Assert.AreEqual(5, pointF.LengthSquared);
            Assert.AreEqual(5, pointI.LengthSquared);
        }

        [Test]
        public void Conversion() {
            Assert.AreEqual(pointD, (PointD) pointF);
            Assert.AreEqual(pointD, (PointD) pointI);

            Assert.AreEqual(pointF, (PointF) pointD);
            Assert.AreEqual(pointF, (PointF) pointI);

            Assert.AreEqual(pointI, (PointI) pointD);
            Assert.AreEqual(pointI, (PointI) pointF);

            Assert.AreEqual(new PointI(0, 1), (PointI) new PointD(0.6, 1.6));
            Assert.AreEqual(new PointI(0, 1), (PointI) new PointF(0.6f, 1.6f));

            Assert.AreEqual(pointI, (PointI) new PointD(1.4, 2.4));
            Assert.AreEqual(pointI, (PointI) new PointF(1.4f, 2.4f));
        }

        [Test]
        public void EqualsEpsilon() {
            Assert.IsTrue(PointD.Equals(pointD, new PointD(1.1, 1.9), 0.2));
            Assert.IsTrue(PointF.Equals(pointF, new PointF(1.1f, 1.9f), 0.2f));
        }

        [Test]
        public void Add() {
            Assert.AreEqual(new PointD(4, 6), pointD + new PointD(3, 4));
            Assert.AreEqual(new PointF(4, 6), pointF + new PointF(3, 4));
            Assert.AreEqual(new PointI(4, 6), pointI + new PointI(3, 4));
        }

        [Test]
        public void AngleBetween() {
            const double epsilon = 0.0001;
            const double angle = 63.435; // polar angle of (1,2)

            Assert.AreEqual(0, pointD.AngleBetween(pointD));
            Assert.AreEqual(0, PointD.Empty.AngleBetween(pointD, pointD));
            Assert.AreEqual(Math.PI, PointD.Empty.AngleBetween(new PointD(1, 0), new PointD(-2, 0)));
            Assert.AreEqual(-angle, pointD.AngleBetween(new PointD(2, 0)) * Angle.RadiansToDegrees, epsilon);
            Assert.AreEqual(90.0 - angle, pointD.AngleBetween(new PointD(0, 2)) * Angle.RadiansToDegrees, epsilon);

            Assert.AreEqual(0, pointF.AngleBetween(pointF));
            Assert.AreEqual(0, PointF.Empty.AngleBetween(pointF, pointF));
            Assert.AreEqual(Math.PI, PointF.Empty.AngleBetween(new PointF(1, 0), new PointF(-2, 0)));
            Assert.AreEqual(-angle, pointF.AngleBetween(new PointF(2, 0)) * Angle.RadiansToDegrees, epsilon);
            Assert.AreEqual(90.0 - angle, pointF.AngleBetween(new PointF(0, 2)) * Angle.RadiansToDegrees, epsilon);

            Assert.AreEqual(0, pointI.AngleBetween(pointI));
            Assert.AreEqual(0, PointI.Empty.AngleBetween(pointI, pointI));
            Assert.AreEqual(Math.PI, PointI.Empty.AngleBetween(new PointI(1, 0), new PointI(-2, 0)));
            Assert.AreEqual(-angle, pointI.AngleBetween(new PointI(2, 0)) * Angle.RadiansToDegrees, epsilon);
            Assert.AreEqual(90.0 - angle, pointI.AngleBetween(new PointI(0, 2)) * Angle.RadiansToDegrees, epsilon);
        }

        [Test]
        public void CrossProductLength() {
            Assert.AreEqual(0, pointD.CrossProductLength(pointD));
            Assert.AreEqual(0, pointD.CrossProductLength(new PointD(-1, -2)));
            Assert.AreEqual(-4, pointD.CrossProductLength(new PointD(2, 0)));
            Assert.AreEqual(2, pointD.CrossProductLength(new PointD(0, 2)));
            Assert.AreEqual(-4, pointD.CrossProductLength(new PointD(2, 4), new PointD(1, -2)));

            Assert.AreEqual(0, pointF.CrossProductLength(pointF));
            Assert.AreEqual(0, pointF.CrossProductLength(new PointF(-1, -2)));
            Assert.AreEqual(-4, pointF.CrossProductLength(new PointF(2, 0)));
            Assert.AreEqual(2, pointF.CrossProductLength(new PointF(0, 2)));
            Assert.AreEqual(-4, pointF.CrossProductLength(new PointF(2, 4), new PointF(1, -2)));

            Assert.AreEqual(0, pointI.CrossProductLength(pointI));
            Assert.AreEqual(0, pointI.CrossProductLength(new PointI(-1, -2)));
            Assert.AreEqual(-4, pointI.CrossProductLength(new PointI(2, 0)));
            Assert.AreEqual(2, pointI.CrossProductLength(new PointI(0, 2)));
            Assert.AreEqual(-4, pointI.CrossProductLength(new PointI(2, 4), new PointI(1, -2)));
        }

        [Test]
        public void FromPolar() {
            const float epsilon = 0.0001f;

            Assert.IsTrue(PointD.Equals(pointD, PointD.FromPolar(pointD.Length, pointD.Angle), epsilon));
            Assert.IsTrue(PointF.Equals(pointF, PointF.FromPolar(pointF.Length, pointF.Angle), epsilon));
            Assert.AreEqual(pointI, PointI.FromPolar(pointI.Length, pointI.Angle));

            Assert.IsTrue(PointD.Equals(new PointD(-1, -2),
                PointD.FromPolar(-pointD.Length, pointD.Angle), epsilon));
            Assert.IsTrue(PointF.Equals(new PointF(-1, -2),
                PointF.FromPolar(-pointF.Length, pointF.Angle), epsilon));
            Assert.AreEqual(new PointI(-1, -2), PointI.FromPolar(-pointI.Length, pointI.Angle));
        }

        [Test]
        public void IsCollinear() {
            PointD ad = new PointD(2, 2);
            PointF af = new PointF(2, 2);
            PointI ai = new PointI(2, 2);

            Assert.IsTrue(PointD.Empty.IsCollinear(ad, PointD.Empty));
            Assert.IsTrue(PointD.Empty.IsCollinear(ad, ad));
            Assert.IsTrue(PointD.Empty.IsCollinear(ad, new PointD(1, 1)));
            Assert.IsFalse(PointD.Empty.IsCollinear(ad, new PointD(0, 1)));
            Assert.IsFalse(PointD.Empty.IsCollinear(ad, new PointD(1, 0)));

            Assert.IsTrue(PointF.Empty.IsCollinear(af, PointF.Empty));
            Assert.IsTrue(PointF.Empty.IsCollinear(af, af));
            Assert.IsTrue(PointF.Empty.IsCollinear(af, new PointF(1, 1)));
            Assert.IsFalse(PointF.Empty.IsCollinear(af, new PointF(0, 1)));
            Assert.IsFalse(PointF.Empty.IsCollinear(af, new PointF(1, 0)));

            Assert.IsTrue(PointI.Empty.IsCollinear(ai, PointI.Empty));
            Assert.IsTrue(PointI.Empty.IsCollinear(ai, ai));
            Assert.IsTrue(PointI.Empty.IsCollinear(ai, new PointI(1, 1)));
            Assert.IsFalse(PointI.Empty.IsCollinear(ai, new PointI(0, 1)));
            Assert.IsFalse(PointI.Empty.IsCollinear(ai, new PointI(1, 0)));
        }

        [Test]
        public void IsCollinearEpsilon() {
            PointD ad = new PointD(2, 2);
            PointF af = new PointF(2, 2);

            Assert.IsFalse(PointD.Empty.IsCollinear(ad, new PointD(0.9, 1.1), 0.01));
            Assert.IsFalse(PointD.Empty.IsCollinear(ad, new PointD(2.9, 3.1), 0.01));
            Assert.IsTrue(PointD.Empty.IsCollinear(ad, new PointD(0.9, 1.1), 0.5));
            Assert.IsTrue(PointD.Empty.IsCollinear(ad, new PointD(2.9, 3.1), 0.5));

            Assert.IsFalse(PointF.Empty.IsCollinear(af, new PointF(0.9f, 1.1f), 0.01f));
            Assert.IsFalse(PointF.Empty.IsCollinear(af, new PointF(2.9f, 3.1f), 0.01f));
            Assert.IsTrue(PointF.Empty.IsCollinear(af, new PointF(0.9f, 1.1f), 0.5f));
            Assert.IsTrue(PointF.Empty.IsCollinear(af, new PointF(2.9f, 3.1f), 0.5f));
        }

        [Test]
        public void Move() {
            double sqrt2 = Math.Sqrt(2);

            Assert.AreEqual(pointD, pointD.Move(pointD, 5));
            Assert.AreEqual(pointD, pointD.Move(new PointD(3, 4), 0));
            Assert.AreEqual(new PointD(2, 2), pointD.Move(new PointD(3, 2), 1));
            Assert.AreEqual(new PointD(0, 2), pointD.Move(new PointD(-1, 2), 1));
            Assert.AreEqual(new PointD(1, 3), pointD.Move(new PointD(1, 0), -1));
            Assert.AreEqual(new PointD(1, 1), pointD.Move(new PointD(1, 4), -1));
            Assert.AreEqual(new PointD(2, 3), pointD.Move(new PointD(3, 4), sqrt2));
            Assert.AreEqual(new PointD(0, 1), pointD.Move(new PointD(3, 4), -sqrt2));

            Assert.AreEqual(pointF, pointF.Move(pointF, 5));
            Assert.AreEqual(pointF, pointF.Move(new PointF(3, 4), 0));
            Assert.AreEqual(new PointF(2, 2), pointF.Move(new PointF(3, 2), 1));
            Assert.AreEqual(new PointF(0, 2), pointF.Move(new PointF(-1, 2), 1));
            Assert.AreEqual(new PointF(1, 3), pointF.Move(new PointF(1, 0), -1));
            Assert.AreEqual(new PointF(1, 1), pointF.Move(new PointF(1, 4), -1));
            Assert.AreEqual(new PointF(2, 3), pointF.Move(new PointF(3, 4), (float) sqrt2));
            Assert.AreEqual(new PointF(0, 1), pointF.Move(new PointF(3, 4), (float) -sqrt2));
        }

        [Test]
        public void Multiply() {
            Assert.AreEqual(11, pointD * new PointD(3, 4));
            Assert.AreEqual(pointD.LengthSquared, pointD * pointD);

            Assert.AreEqual(11, pointF * new PointF(3, 4));
            Assert.AreEqual(pointF.LengthSquared, pointF * pointF);

            Assert.AreEqual(11, pointI * new PointI(3, 4));
            Assert.AreEqual(pointI.LengthSquared, pointI * pointI);
        }

        [Test]
        public void Normalize() {
            const float epsilon = 0.0001f;

            Assert.IsTrue(PointD.Equals(new PointD(-1, 0), new PointD(-1, 0).Normalize(), epsilon));
            Assert.IsTrue(PointD.Equals(new PointD(0, -1), new PointD(0, -0.5).Normalize(), epsilon));
            Assert.IsTrue(PointD.Equals(new PointD(0.447213, 0.894428), pointD.Normalize(), epsilon));

            Assert.IsTrue(PointF.Equals(new PointF(-1, 0), new PointF(-1, 0).Normalize(), epsilon));
            Assert.IsTrue(PointF.Equals(new PointF(0, -1), new PointF(0, -0.5f).Normalize(), epsilon));
            Assert.IsTrue(PointF.Equals(new PointF(0.447213f, 0.894428f), pointF.Normalize(), epsilon));
        }

        [Test]
        public void Restrict() {
            Assert.AreEqual(pointD, new PointD(0, 0).Restrict(1, 2, 9, 9));
            Assert.AreEqual(pointD, new PointD(9, 9).Restrict(0, 0, 1, 2));

            Assert.AreEqual(pointF, new PointF(0, 0).Restrict(1, 2, 9, 9));
            Assert.AreEqual(pointF, new PointF(9, 9).Restrict(0, 0, 1, 2));

            Assert.AreEqual(pointI, new PointI(0, 0).Restrict(1, 2, 9, 9));
            Assert.AreEqual(pointI, new PointI(9, 9).Restrict(0, 0, 1, 2));
        }

        [Test]
        public void Round() {
            Assert.AreEqual(pointI, pointD.Round());
            Assert.AreEqual(pointI, new PointD(0.6, 1.6).Round());
            Assert.AreEqual(pointI, new PointD(1.4, 2.4).Round());

            Assert.AreEqual(pointI, pointF.Round());
            Assert.AreEqual(pointI, new PointF(0.6f, 1.6f).Round());
            Assert.AreEqual(pointI, new PointF(1.4f, 2.4f).Round());
        }

        [Test]
        public void Subtract() {
            Assert.AreEqual(new PointD(-2, -2), pointD - new PointD(3, 4));
            Assert.AreEqual(new PointF(-2, -2), pointF - new PointF(3, 4));
            Assert.AreEqual(new PointI(-2, -2), pointI - new PointI(3, 4));
        }
    }
}
