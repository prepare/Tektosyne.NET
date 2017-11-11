using System;
using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class AngleTest {

        [Test]
        public void DegreesToCompass() {
            Assert.AreEqual(Compass.North, Angle.DegreesToCompass(-22.0));
            Assert.AreEqual(Compass.North, Angle.DegreesToCompass(0.0));
            Assert.AreEqual(Compass.North, Angle.DegreesToCompass(22.0));

            Assert.AreEqual(Compass.NorthWest, Angle.DegreesToCompass(-23.0));
            Assert.AreEqual(Compass.NorthEast, Angle.DegreesToCompass(23.0));
        }

        [Test]
        public void DistanceDegrees() {
            for (double a = 0; a < 360; a++)
                for (double b = 0; b < 360; b++) {
                    double dist = Angle.DistanceDegrees(a, b);
                    Assert.IsTrue(-180 < dist && dist <= 180);
                    Assert.AreEqual(b, Angle.NormalizeDegrees(a + dist));
                }
        }

        [Test]
        public void DistanceRadians() {
            for (double a = 0; a < 2 * Math.PI; a += Angle.DegreesToRadians)
                for (double b = 0; b < 2 * Math.PI; b += Angle.DegreesToRadians) {
                    double dist = Angle.DistanceRadians(a, b);
                    Assert.IsTrue(-Math.PI < dist && dist <= Math.PI);
                    Assert.AreEqual(b, Angle.NormalizeRadians(a + dist), Angle.DegreesToRadians / 2);
                }
        }

        [Test]
        public void NormalizeDegrees() {
            const double epsilon = 0.0001;

            Assert.AreEqual(0, Angle.NormalizeDegrees(0), epsilon);
            Assert.AreEqual(0.4, Angle.NormalizeDegrees(0.4), epsilon);
            Assert.AreEqual(359.6, Angle.NormalizeDegrees(-0.4), epsilon);

            Assert.AreEqual(0, Angle.NormalizeDegrees(360), epsilon);
            Assert.AreEqual(0.4, Angle.NormalizeDegrees(360.4), epsilon);
            Assert.AreEqual(359.4, Angle.NormalizeDegrees(-0.6), epsilon);

            Assert.AreEqual(180, Angle.NormalizeDegrees(180), epsilon);
            Assert.AreEqual(180, Angle.NormalizeDegrees(540), epsilon);
            Assert.AreEqual(180, Angle.NormalizeDegrees(-180), epsilon);
            Assert.AreEqual(180, Angle.NormalizeDegrees(-540), epsilon);
        }

        [Test]
        public void NormalizeRoundedDegrees() {
            Assert.AreEqual(0, Angle.NormalizeRoundedDegrees(0));
            Assert.AreEqual(0, Angle.NormalizeRoundedDegrees(0.4));
            Assert.AreEqual(0, Angle.NormalizeRoundedDegrees(-0.4));

            Assert.AreEqual(0, Angle.NormalizeRoundedDegrees(360));
            Assert.AreEqual(0, Angle.NormalizeRoundedDegrees(360.4));
            Assert.AreEqual(359, Angle.NormalizeRoundedDegrees(-0.6));

            Assert.AreEqual(180, Angle.NormalizeRoundedDegrees(180));
            Assert.AreEqual(180, Angle.NormalizeRoundedDegrees(540));
            Assert.AreEqual(180, Angle.NormalizeRoundedDegrees(-180));
            Assert.AreEqual(180, Angle.NormalizeRoundedDegrees(-540));
        }

        [Test]
        public void NormalizeRadians() {
            const double epsilon = 0.0001;

            Assert.AreEqual(0, Angle.NormalizeRadians(0), epsilon);
            Assert.AreEqual(0.4, Angle.NormalizeRadians(0.4), epsilon);
            Assert.AreEqual(2 * Math.PI - 0.4, Angle.NormalizeRadians(-0.4), epsilon);

            Assert.AreEqual(0, Angle.NormalizeRadians(2 * Math.PI), epsilon);
            Assert.AreEqual(0.4, Angle.NormalizeRadians(2 * Math.PI + 0.4), epsilon);
            Assert.AreEqual(2 * Math.PI - 0.6, Angle.NormalizeRadians(-0.6), epsilon);

            Assert.AreEqual(Math.PI, Angle.NormalizeRadians(Math.PI), epsilon);
            Assert.AreEqual(Math.PI, Angle.NormalizeRadians(3 * Math.PI), epsilon);
            Assert.AreEqual(Math.PI, Angle.NormalizeRadians(-Math.PI), epsilon);
            Assert.AreEqual(Math.PI, Angle.NormalizeRadians(-3 * Math.PI), epsilon);
        }
    }
}
