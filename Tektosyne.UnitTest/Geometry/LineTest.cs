using System;
using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class LineTest {

        LineD diagD = new LineD(0, 0, 2, 2);
        LineF diagF = new LineF(0, 0, 2, 2);
        LineI diagI = new LineI(0, 0, 2, 2);

        LineD lineD = new LineD(1, 3, 4, 5);
        LineF lineF = new LineF(1, 3, 4, 5);
        LineI lineI = new LineI(1, 3, 4, 5);

        LineD mirrorLineD = new LineD(1, -3, 4, -5);
        LineF mirrorLineF = new LineF(1, -3, 4, -5);
        LineI mirrorLineI = new LineI(1, -3, 4, -5);

        [Test]
        public void AngleTest() {
            const double epsilon = 0.0001;
            const double angle = 33.69 * Angle.DegreesToRadians;

            Assert.AreEqual(angle, lineD.Angle, epsilon);
            Assert.AreEqual(angle, lineF.Angle, epsilon);
            Assert.AreEqual(angle, lineI.Angle, epsilon);

            Assert.AreEqual(-angle, mirrorLineD.Angle, epsilon);
            Assert.AreEqual(-angle, mirrorLineF.Angle, epsilon);
            Assert.AreEqual(-angle, mirrorLineI.Angle, epsilon);
        }

        [Test]
        public void InverseSlope() {
            Assert.AreEqual(1.5, lineD.InverseSlope);
            Assert.AreEqual(-1.5, mirrorLineD.InverseSlope);

            Assert.AreEqual(1.5f, lineF.InverseSlope);
            Assert.AreEqual(-1.5f, mirrorLineF.InverseSlope);

            Assert.AreEqual(1.5, lineI.InverseSlope);
            Assert.AreEqual(-1.5, mirrorLineI.InverseSlope);
        }

        [Test]
        public void Length() {
            Assert.AreEqual(Math.Sqrt(13), lineD.Length);
            Assert.AreEqual(Math.Sqrt(13), lineF.Length);
            Assert.AreEqual(Math.Sqrt(13), lineI.Length);

            Assert.AreEqual(Math.Sqrt(13), mirrorLineD.Length);
            Assert.AreEqual(Math.Sqrt(13), mirrorLineF.Length);
            Assert.AreEqual(Math.Sqrt(13), mirrorLineI.Length);
        }

        [Test]
        public void LengthSquared() {
            Assert.AreEqual(13, lineD.LengthSquared);
            Assert.AreEqual(13, lineF.LengthSquared);
            Assert.AreEqual(13, lineI.LengthSquared);

            Assert.AreEqual(13, mirrorLineD.LengthSquared);
            Assert.AreEqual(13, mirrorLineF.LengthSquared);
            Assert.AreEqual(13, mirrorLineI.LengthSquared);
        }

        [Test]
        public void Slope() {
            Assert.AreEqual(2.0 / 3.0, lineD.Slope);
            Assert.AreEqual(-2.0 / 3.0, mirrorLineD.Slope);

            Assert.AreEqual(2f / 3f, lineF.Slope);
            Assert.AreEqual(-2f / 3f, mirrorLineF.Slope);

            Assert.AreEqual(2.0 / 3.0, lineI.Slope);
            Assert.AreEqual(-2.0 / 3.0, mirrorLineI.Slope);
        }

        [Test]
        public void Vector() {
            Assert.AreEqual(new PointD(3, 2), lineD.Vector);
            Assert.AreEqual(new PointF(3, 2), lineF.Vector);
            Assert.AreEqual(new PointI(3, 2), lineI.Vector);

            Assert.AreEqual(new PointD(3, -2), mirrorLineD.Vector);
            Assert.AreEqual(new PointF(3, -2), mirrorLineF.Vector);
            Assert.AreEqual(new PointI(3, -2), mirrorLineI.Vector);

            const double epsilon = 0.0001;
            Assert.AreEqual(lineD.Angle, lineD.Vector.Angle, epsilon);
            Assert.AreEqual(lineF.Angle, lineF.Vector.Angle, epsilon);
            Assert.AreEqual(lineI.Angle, lineI.Vector.Angle, epsilon);

            Assert.AreEqual(lineD.Length, lineD.Vector.Length, epsilon);
            Assert.AreEqual(lineF.Length, lineF.Vector.Length, epsilon);
            Assert.AreEqual(lineI.Length, lineI.Vector.Length, epsilon);
        }

        [Test]
        public void Conversion() {
            Assert.AreEqual(lineD, (LineD) lineF);
            Assert.AreEqual(lineD, (LineD) lineI);

            Assert.AreEqual(lineF, (LineF) lineD);
            Assert.AreEqual(lineF, (LineF) lineI);

            Assert.AreEqual(lineI, (LineI) lineD);
            Assert.AreEqual(lineI, (LineI) lineF);

            Assert.AreEqual(new LineI(0, 1, 3, 4), (LineI) new LineD(0.6, 1.6, 3.6, 4.6));
            Assert.AreEqual(new LineI(0, 1, 3, 4), (LineI) new LineF(0.6f, 1.6f, 3.6f, 4.6f));

            Assert.AreEqual(lineI, (LineI) new LineD(1.4, 3.4, 4.4, 5.4));
            Assert.AreEqual(lineI, (LineI) new LineF(1.4f, 3.4f, 4.4f, 5.4f));
        }

        [Test]
        public void DistanceSquared() {
            Assert.AreEqual(0, diagD.DistanceSquared(diagD.Start));
            Assert.AreEqual(0, diagD.DistanceSquared(diagD.End));
            Assert.AreEqual(0, diagD.DistanceSquared(new PointD(1, 1)));

            Assert.AreEqual(2, diagD.DistanceSquared(new PointD(-1, -1)));
            Assert.AreEqual(2, diagD.DistanceSquared(new PointD(0, 2)));
            Assert.AreEqual(2, diagD.DistanceSquared(new PointD(2, 0)));
            Assert.AreEqual(2, diagD.DistanceSquared(new PointD(3, 3)));

            Assert.AreEqual(0, diagF.DistanceSquared(diagF.Start));
            Assert.AreEqual(0, diagF.DistanceSquared(diagF.End));
            Assert.AreEqual(0, diagF.DistanceSquared(new PointF(1, 1)));

            Assert.AreEqual(2, diagF.DistanceSquared(new PointF(-1, -1)));
            Assert.AreEqual(2, diagF.DistanceSquared(new PointF(0, 2)));
            Assert.AreEqual(2, diagF.DistanceSquared(new PointF(2, 0)));
            Assert.AreEqual(2, diagF.DistanceSquared(new PointF(3, 3)));

            Assert.AreEqual(0, diagI.DistanceSquared(diagI.Start));
            Assert.AreEqual(0, diagI.DistanceSquared(diagI.End));
            Assert.AreEqual(0, diagI.DistanceSquared(new PointI(1, 1)));

            Assert.AreEqual(2, diagI.DistanceSquared(new PointI(-1, -1)));
            Assert.AreEqual(2, diagI.DistanceSquared(new PointI(0, 2)));
            Assert.AreEqual(2, diagI.DistanceSquared(new PointI(2, 0)));
            Assert.AreEqual(2, diagI.DistanceSquared(new PointI(3, 3)));
        }

        [Test]
        public void EqualsEpsilon() {
            Assert.IsTrue(LineD.Equals(lineD, new LineD(1.1, 2.9, 3.9, 5.1), 0.2));
            Assert.IsTrue(LineF.Equals(lineF, new LineF(1.1f, 2.9f, 3.9f, 5.1f), 0.2f));
        }

        [Test]
        public void FindX() {
            Assert.AreEqual(-0.5, lineD.FindX(2));
            Assert.AreEqual(1, lineD.FindX(3));
            Assert.AreEqual(2.5, lineD.FindX(4));
            Assert.AreEqual(4, lineD.FindX(5));
            Assert.AreEqual(5.5, lineD.FindX(6));

            Assert.AreEqual(-0.5f, lineF.FindX(2));
            Assert.AreEqual(1, lineF.FindX(3));
            Assert.AreEqual(2.5f, lineF.FindX(4));
            Assert.AreEqual(4, lineF.FindX(5));
            Assert.AreEqual(5.5f, lineF.FindX(6));

            Assert.AreEqual(-0.5, lineI.FindX(2));
            Assert.AreEqual(1, lineI.FindX(3));
            Assert.AreEqual(2.5, lineI.FindX(4));
            Assert.AreEqual(4, lineI.FindX(5));
            Assert.AreEqual(5.5, lineI.FindX(6));
        }

        [Test]
        public void FindY() {
            Assert.AreEqual(2, lineD.FindY(-0.5));
            Assert.AreEqual(3, lineD.FindY(1));
            Assert.AreEqual(4, lineD.FindY(2.5));
            Assert.AreEqual(5, lineD.FindY(4));
            Assert.AreEqual(6, lineD.FindY(5.5));

            Assert.AreEqual(2, lineF.FindY(-0.5f));
            Assert.AreEqual(3, lineF.FindY(1));
            Assert.AreEqual(4, lineF.FindY(2.5f));
            Assert.AreEqual(5, lineF.FindY(4));
            Assert.AreEqual(6, lineF.FindY(5.5f));

            Assert.AreEqual(2, lineI.FindY(-0.5));
            Assert.AreEqual(3, lineI.FindY(1));
            Assert.AreEqual(4, lineI.FindY(2.5));
            Assert.AreEqual(5, lineI.FindY(4));
            Assert.AreEqual(6, lineI.FindY(5.5));
        }

        [Test]
        public void Intersect() {
            Assert.AreEqual(diagD.Start, diagD.Intersect(diagD.Start));
            Assert.AreEqual(diagD.End, diagD.Intersect(diagD.End));

            Assert.AreEqual(diagF.Start, diagF.Intersect(diagF.Start));
            Assert.AreEqual(diagF.End, diagF.Intersect(diagF.End));

            Assert.AreEqual((PointD) diagI.Start, diagI.Intersect(diagI.Start));
            Assert.AreEqual((PointD) diagI.End, diagI.Intersect(diagI.End));

            for (int d = -2; d <= 4; d++) {
                Assert.AreEqual(new PointD(d, d), diagD.Intersect(new PointD(d, d)));
                Assert.AreEqual(new PointD(d, d), diagD.Intersect(new PointD(d-1, d+1)));
                Assert.AreEqual(new PointD(d, d), diagD.Intersect(new PointD(d+1, d-1)));

                Assert.AreEqual(new PointF(d, d), diagF.Intersect(new PointF(d, d)));
                Assert.AreEqual(new PointF(d, d), diagF.Intersect(new PointF(d - 1, d + 1)));
                Assert.AreEqual(new PointF(d, d), diagF.Intersect(new PointF(d + 1, d - 1)));

                Assert.AreEqual(new PointD(d, d), diagI.Intersect(new PointI(d, d)));
                Assert.AreEqual(new PointD(d, d), diagI.Intersect(new PointI(d - 1, d + 1)));
                Assert.AreEqual(new PointD(d, d), diagI.Intersect(new PointI(d + 1, d - 1)));
            }
        }

        [Test]
        public void Locate() {
            Assert.AreEqual(LineLocation.Start, diagD.Locate(new PointD(0, 0)));
            Assert.AreEqual(LineLocation.End, diagD.Locate(new PointD(2, 2)));
            Assert.AreEqual(LineLocation.Before, diagD.Locate(new PointD(-1, -1)));
            Assert.AreEqual(LineLocation.Between, diagD.Locate(new PointD(1, 1)));
            Assert.AreEqual(LineLocation.After, diagD.Locate(new PointD(3, 3)));
            Assert.AreEqual(LineLocation.Left, diagD.Locate(new PointF(0, 1)));
            Assert.AreEqual(LineLocation.Right, diagD.Locate(new PointF(1, 0)));

            Assert.AreEqual(LineLocation.Start, diagF.Locate(new PointF(0, 0)));
            Assert.AreEqual(LineLocation.End, diagF.Locate(new PointF(2, 2)));
            Assert.AreEqual(LineLocation.Before, diagF.Locate(new PointF(-1, -1)));
            Assert.AreEqual(LineLocation.Between, diagF.Locate(new PointF(1, 1)));
            Assert.AreEqual(LineLocation.After, diagF.Locate(new PointF(3, 3)));
            Assert.AreEqual(LineLocation.Left, diagF.Locate(new PointI(0, 1)));
            Assert.AreEqual(LineLocation.Right, diagF.Locate(new PointI(1, 0)));

            Assert.AreEqual(LineLocation.Start, diagI.Locate(new PointI(0, 0)));
            Assert.AreEqual(LineLocation.End, diagI.Locate(new PointI(2, 2)));
            Assert.AreEqual(LineLocation.Before, diagI.Locate(new PointI(-1, -1)));
            Assert.AreEqual(LineLocation.Between, diagI.Locate(new PointI(1, 1)));
            Assert.AreEqual(LineLocation.After, diagI.Locate(new PointI(3, 3)));
            Assert.AreEqual(LineLocation.Left, diagI.Locate(new PointI(0, 1)));
            Assert.AreEqual(LineLocation.Right, diagI.Locate(new PointI(1, 0)));
        }

        [Test]
        public void LocateReverse() {
            LineD reverseDiagD = diagD.Reverse();
            Assert.AreEqual(LineLocation.End, reverseDiagD.Locate(new PointD(0, 0)));
            Assert.AreEqual(LineLocation.Start, reverseDiagD.Locate(new PointD(2, 2)));
            Assert.AreEqual(LineLocation.After, reverseDiagD.Locate(new PointD(-1, -1)));
            Assert.AreEqual(LineLocation.Between, reverseDiagD.Locate(new PointD(1, 1)));
            Assert.AreEqual(LineLocation.Before, reverseDiagD.Locate(new PointD(3, 3)));
            Assert.AreEqual(LineLocation.Right, reverseDiagD.Locate(new PointF(0, 1)));
            Assert.AreEqual(LineLocation.Left, reverseDiagD.Locate(new PointF(1, 0)));

            LineF reverseDiagF = diagF.Reverse();
            Assert.AreEqual(LineLocation.End, reverseDiagF.Locate(new PointF(0, 0)));
            Assert.AreEqual(LineLocation.Start, reverseDiagF.Locate(new PointF(2, 2)));
            Assert.AreEqual(LineLocation.After, reverseDiagF.Locate(new PointF(-1, -1)));
            Assert.AreEqual(LineLocation.Between, reverseDiagF.Locate(new PointF(1, 1)));
            Assert.AreEqual(LineLocation.Before, reverseDiagF.Locate(new PointF(3, 3)));
            Assert.AreEqual(LineLocation.Right, reverseDiagF.Locate(new PointI(0, 1)));
            Assert.AreEqual(LineLocation.Left, reverseDiagF.Locate(new PointI(1, 0)));

            LineI reverseDiagI = diagI.Reverse();
            Assert.AreEqual(LineLocation.End, reverseDiagI.Locate(new PointI(0, 0)));
            Assert.AreEqual(LineLocation.Start, reverseDiagI.Locate(new PointI(2, 2)));
            Assert.AreEqual(LineLocation.After, reverseDiagI.Locate(new PointI(-1, -1)));
            Assert.AreEqual(LineLocation.Between, reverseDiagI.Locate(new PointI(1, 1)));
            Assert.AreEqual(LineLocation.Before, reverseDiagI.Locate(new PointI(3, 3)));
            Assert.AreEqual(LineLocation.Right, reverseDiagI.Locate(new PointI(0, 1)));
            Assert.AreEqual(LineLocation.Left, reverseDiagI.Locate(new PointI(1, 0)));
        }

        [Test]
        public void LocateEpsilon() {
            Assert.AreEqual(LineLocation.Before, diagD.Locate(new PointD(-0.1, -0.1), 0.01));
            Assert.AreEqual(LineLocation.Start, diagD.Locate(new PointD(-0.1, -0.1), 0.5));
            Assert.AreEqual(LineLocation.After, diagD.Locate(new PointD(2.1, 2.1), 0.01));
            Assert.AreEqual(LineLocation.End, diagD.Locate(new PointD(2.1, 2.1), 0.5));

            Assert.AreEqual(LineLocation.Left, diagD.Locate(new PointD(0.9, 1.1), 0.01));
            Assert.AreEqual(LineLocation.Between, diagD.Locate(new PointD(0.9, 1.1), 0.5));
            Assert.AreEqual(LineLocation.Right, diagD.Locate(new PointD(1.1, 0.9), 0.01));
            Assert.AreEqual(LineLocation.Between, diagD.Locate(new PointD(1.1, 0.9), 0.5));

            Assert.AreEqual(LineLocation.Before, diagF.Locate(new PointF(-0.1f, -0.1f), 0.01f));
            Assert.AreEqual(LineLocation.Start, diagF.Locate(new PointF(-0.1f, -0.1f), 0.5f));
            Assert.AreEqual(LineLocation.After, diagF.Locate(new PointF(2.1f, 2.1f), 0.01f));
            Assert.AreEqual(LineLocation.End, diagF.Locate(new PointF(2.1f, 2.1f), 0.5f));

            Assert.AreEqual(LineLocation.Left, diagF.Locate(new PointF(0.9f, 1.1f), 0.01f));
            Assert.AreEqual(LineLocation.Between, diagF.Locate(new PointF(0.9f, 1.1f), 0.5f));
            Assert.AreEqual(LineLocation.Right, diagF.Locate(new PointF(1.1f, 0.9f), 0.01f));
            Assert.AreEqual(LineLocation.Between, diagF.Locate(new PointF(1.1f, 0.9f), 0.5f));
        }

        [Test]
        public void LocateCollinear() {
            Assert.AreEqual(LineLocation.Start, diagD.LocateCollinear(new PointD(0, 0)));
            Assert.AreEqual(LineLocation.End, diagD.LocateCollinear(new PointD(2, 2)));
            Assert.AreEqual(LineLocation.Before, diagD.LocateCollinear(new PointD(-1, -1)));
            Assert.AreEqual(LineLocation.Between, diagD.LocateCollinear(new PointD(1, 1)));
            Assert.AreEqual(LineLocation.After, diagD.LocateCollinear(new PointD(3, 3)));
            Assert.AreEqual(LineLocation.Between, diagD.LocateCollinear(new PointD(0, 1)));
            Assert.AreEqual(LineLocation.Between, diagD.LocateCollinear(new PointD(1, 0)));

            Assert.AreEqual(LineLocation.Start, diagF.LocateCollinear(new PointF(0, 0)));
            Assert.AreEqual(LineLocation.End, diagF.LocateCollinear(new PointF(2, 2)));
            Assert.AreEqual(LineLocation.Before, diagF.LocateCollinear(new PointF(-1, -1)));
            Assert.AreEqual(LineLocation.Between, diagF.LocateCollinear(new PointF(1, 1)));
            Assert.AreEqual(LineLocation.After, diagF.LocateCollinear(new PointF(3, 3)));
            Assert.AreEqual(LineLocation.Between, diagF.LocateCollinear(new PointF(0, 1)));
            Assert.AreEqual(LineLocation.Between, diagF.LocateCollinear(new PointF(1, 0)));

            Assert.AreEqual(LineLocation.Start, diagI.LocateCollinear(new PointI(0, 0)));
            Assert.AreEqual(LineLocation.End, diagI.LocateCollinear(new PointI(2, 2)));
            Assert.AreEqual(LineLocation.Before, diagI.LocateCollinear(new PointI(-1, -1)));
            Assert.AreEqual(LineLocation.Between, diagI.LocateCollinear(new PointI(1, 1)));
            Assert.AreEqual(LineLocation.After, diagI.LocateCollinear(new PointI(3, 3)));
            Assert.AreEqual(LineLocation.Between, diagI.LocateCollinear(new PointI(0, 1)));
            Assert.AreEqual(LineLocation.Between, diagI.LocateCollinear(new PointI(1, 0)));
        }

        [Test]
        public void LocateCollinearReverse() {
            LineD reverseDiagD = diagD.Reverse();
            Assert.AreEqual(LineLocation.End, reverseDiagD.LocateCollinear(new PointD(0, 0)));
            Assert.AreEqual(LineLocation.Start, reverseDiagD.LocateCollinear(new PointD(2, 2)));
            Assert.AreEqual(LineLocation.After, reverseDiagD.LocateCollinear(new PointD(-1, -1)));
            Assert.AreEqual(LineLocation.Between, reverseDiagD.LocateCollinear(new PointD(1, 1)));
            Assert.AreEqual(LineLocation.Before, reverseDiagD.LocateCollinear(new PointD(3, 3)));
            Assert.AreEqual(LineLocation.Between, reverseDiagD.LocateCollinear(new PointD(0, 1)));
            Assert.AreEqual(LineLocation.Between, reverseDiagD.LocateCollinear(new PointD(1, 0)));

            LineF reverseDiagF = diagF.Reverse();
            Assert.AreEqual(LineLocation.End, reverseDiagF.LocateCollinear(new PointF(0, 0)));
            Assert.AreEqual(LineLocation.Start, reverseDiagF.LocateCollinear(new PointF(2, 2)));
            Assert.AreEqual(LineLocation.After, reverseDiagF.LocateCollinear(new PointF(-1, -1)));
            Assert.AreEqual(LineLocation.Between, reverseDiagF.LocateCollinear(new PointF(1, 1)));
            Assert.AreEqual(LineLocation.Before, reverseDiagF.LocateCollinear(new PointF(3, 3)));
            Assert.AreEqual(LineLocation.Between, reverseDiagF.LocateCollinear(new PointF(0, 1)));
            Assert.AreEqual(LineLocation.Between, reverseDiagF.LocateCollinear(new PointF(1, 0)));

            LineI reverseDiagI = diagI.Reverse();
            Assert.AreEqual(LineLocation.End, reverseDiagI.LocateCollinear(new PointI(0, 0)));
            Assert.AreEqual(LineLocation.Start, reverseDiagI.LocateCollinear(new PointI(2, 2)));
            Assert.AreEqual(LineLocation.After, reverseDiagI.LocateCollinear(new PointI(-1, -1)));
            Assert.AreEqual(LineLocation.Between, reverseDiagI.LocateCollinear(new PointI(1, 1)));
            Assert.AreEqual(LineLocation.Before, reverseDiagI.LocateCollinear(new PointI(3, 3)));
            Assert.AreEqual(LineLocation.Between, reverseDiagI.LocateCollinear(new PointI(0, 1)));
            Assert.AreEqual(LineLocation.Between, reverseDiagI.LocateCollinear(new PointI(1, 0)));
        }

        [Test]
        public void LocateCollinearEpsilon() {
            Assert.AreEqual(LineLocation.Before, diagD.LocateCollinear(new PointD(-0.1, -0.1), 0.01));
            Assert.AreEqual(LineLocation.Start, diagD.LocateCollinear(new PointD(-0.1, -0.1), 0.5));
            Assert.AreEqual(LineLocation.After, diagD.LocateCollinear(new PointD(2.1, 2.1), 0.01));
            Assert.AreEqual(LineLocation.End, diagD.LocateCollinear(new PointD(2.1, 2.1), 0.5));
            Assert.AreEqual(LineLocation.Between, diagD.LocateCollinear(new PointD(0.9, 1.1), 0.01));
            Assert.AreEqual(LineLocation.Between, diagD.LocateCollinear(new PointD(1.1, 0.9), 0.01));

            Assert.AreEqual(LineLocation.Before, diagF.LocateCollinear(new PointF(-0.1f, -0.1f), 0.01f));
            Assert.AreEqual(LineLocation.Start, diagF.LocateCollinear(new PointF(-0.1f, -0.1f), 0.5f));
            Assert.AreEqual(LineLocation.After, diagF.LocateCollinear(new PointF(2.1f, 2.1f), 0.01f));
            Assert.AreEqual(LineLocation.End, diagF.LocateCollinear(new PointF(2.1f, 2.1f), 0.5f));
            Assert.AreEqual(LineLocation.Between, diagF.LocateCollinear(new PointF(0.9f, 1.1f), 0.01f));
            Assert.AreEqual(LineLocation.Between, diagF.LocateCollinear(new PointF(1.1f, 0.9f), 0.01f));
        }

        [Test]
        public void Reverse() {
            Assert.AreEqual(new LineD(4, 5, 1, 3), lineD.Reverse());
            Assert.AreEqual(new LineF(4, 5, 1, 3), lineF.Reverse());
            Assert.AreEqual(new LineI(4, 5, 1, 3), lineI.Reverse());
        }

        [Test]
        public void Round() {
            Assert.AreEqual(lineI, lineD.Round());
            Assert.AreEqual(lineI, new LineD(0.6, 2.6, 3.6, 4.6).Round());
            Assert.AreEqual(lineI, new LineD(1.4, 3.4, 4.4, 5.4).Round());

            Assert.AreEqual(lineI, lineF.Round());
            Assert.AreEqual(lineI, new LineF(0.6f, 2.6f, 3.6f, 4.6f).Round());
            Assert.AreEqual(lineI, new LineF(1.4f, 3.4f, 4.4f, 5.4f).Round());
        }
    }
}
