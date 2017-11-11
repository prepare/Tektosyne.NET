using System;
using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class RegularPolygonTest {

        RegularPolygon hexagonOnEdge = new RegularPolygon(10.0, 6, PolygonOrientation.OnEdge),
            hexagonOnVertex = new RegularPolygon(10.0, 6, PolygonOrientation.OnVertex),
            squareOnEdge = new RegularPolygon(10.0, 4, PolygonOrientation.OnEdge, true);

        [Test]
        public void HasTopIndex() {
            Assert.IsTrue(hexagonOnEdge.HasTopIndex);
            Assert.IsFalse(hexagonOnVertex.HasTopIndex);
            Assert.IsTrue(squareOnEdge.HasTopIndex);

            var polygon = new RegularPolygon(10.0, 5, PolygonOrientation.OnEdge);
            Assert.IsFalse(polygon.HasTopIndex);

            polygon = new RegularPolygon(10.0, 5, PolygonOrientation.OnVertex);
            Assert.IsTrue(polygon.HasTopIndex);
        }

        [Test]
        public void AngleToIndex() {
            Assert.AreEqual(0, hexagonOnEdge.AngleToIndex(-90.0));
            Assert.AreEqual(1, hexagonOnEdge.AngleToIndex(-30.0));
            Assert.AreEqual(2, hexagonOnEdge.AngleToIndex(30.0));
            Assert.AreEqual(3, hexagonOnEdge.AngleToIndex(90.0));
            Assert.AreEqual(4, hexagonOnEdge.AngleToIndex(150.0));
            Assert.AreEqual(5, hexagonOnEdge.AngleToIndex(210.0));

            Assert.AreEqual(0, hexagonOnVertex.AngleToIndex(-60.0));
            Assert.AreEqual(1, hexagonOnVertex.AngleToIndex(0.0));
            Assert.AreEqual(2, hexagonOnVertex.AngleToIndex(60.0));
            Assert.AreEqual(3, hexagonOnVertex.AngleToIndex(120.0));
            Assert.AreEqual(4, hexagonOnVertex.AngleToIndex(180.0));
            Assert.AreEqual(5, hexagonOnVertex.AngleToIndex(240.0));

            Assert.AreEqual(1, squareOnEdge.AngleToIndex(-23.0));
            Assert.AreEqual(2, squareOnEdge.AngleToIndex(-22.0));
            Assert.AreEqual(3, squareOnEdge.AngleToIndex(67.0));
            Assert.AreEqual(4, squareOnEdge.AngleToIndex(68.0));
            Assert.AreEqual(5, squareOnEdge.AngleToIndex(157.0));
            Assert.AreEqual(6, squareOnEdge.AngleToIndex(158.0));
            Assert.AreEqual(7, squareOnEdge.AngleToIndex(247.0));
            Assert.AreEqual(0, squareOnEdge.AngleToIndex(248.0));
        }

        [Test]
        public void CompassToIndex() {
            Assert.AreEqual(0, hexagonOnEdge.CompassToIndex(Compass.North));
            Assert.AreEqual(1, hexagonOnEdge.CompassToIndex(Compass.NorthEast));
            Assert.AreEqual(2, hexagonOnEdge.CompassToIndex(Compass.SouthEast));
            Assert.AreEqual(3, hexagonOnEdge.CompassToIndex(Compass.South));
            Assert.AreEqual(4, hexagonOnEdge.CompassToIndex(Compass.SouthWest));
            Assert.AreEqual(5, hexagonOnEdge.CompassToIndex(Compass.NorthWest));

            Assert.AreEqual(0, hexagonOnVertex.CompassToIndex(Compass.NorthEast));
            Assert.AreEqual(1, hexagonOnVertex.CompassToIndex(Compass.East));
            Assert.AreEqual(2, hexagonOnVertex.CompassToIndex(Compass.SouthEast));
            Assert.AreEqual(3, hexagonOnVertex.CompassToIndex(Compass.SouthWest));
            Assert.AreEqual(4, hexagonOnVertex.CompassToIndex(Compass.West));
            Assert.AreEqual(5, hexagonOnVertex.CompassToIndex(Compass.NorthWest));

            Assert.AreEqual(0, squareOnEdge.CompassToIndex(Compass.North));
            Assert.AreEqual(1, squareOnEdge.CompassToIndex(Compass.NorthEast));
            Assert.AreEqual(2, squareOnEdge.CompassToIndex(Compass.East));
            Assert.AreEqual(3, squareOnEdge.CompassToIndex(Compass.SouthEast));
            Assert.AreEqual(4, squareOnEdge.CompassToIndex(Compass.South));
            Assert.AreEqual(5, squareOnEdge.CompassToIndex(Compass.SouthWest));
            Assert.AreEqual(6, squareOnEdge.CompassToIndex(Compass.West));
            Assert.AreEqual(7, squareOnEdge.CompassToIndex(Compass.NorthWest));
        }

        [Test]
        public void IndexToAngle() {
            Assert.AreEqual(30.0, hexagonOnEdge.IndexToAngle(2));
            Assert.AreEqual(90.0, hexagonOnEdge.IndexToAngle(3));
            Assert.AreEqual(150.0, hexagonOnEdge.IndexToAngle(4));
            Assert.AreEqual(210.0, hexagonOnEdge.IndexToAngle(5));
            Assert.AreEqual(270.0, hexagonOnEdge.IndexToAngle(6));
            Assert.AreEqual(330.0, hexagonOnEdge.IndexToAngle(7));

            Assert.AreEqual(60.0, hexagonOnVertex.IndexToAngle(2));
            Assert.AreEqual(120.0, hexagonOnVertex.IndexToAngle(3));
            Assert.AreEqual(180.0, hexagonOnVertex.IndexToAngle(4));
            Assert.AreEqual(240.0, hexagonOnVertex.IndexToAngle(5));
            Assert.AreEqual(300.0, hexagonOnVertex.IndexToAngle(6));
            Assert.AreEqual(0.0, hexagonOnVertex.IndexToAngle(7));

            Assert.AreEqual(0.0, squareOnEdge.IndexToAngle(2));
            Assert.AreEqual(45.0, squareOnEdge.IndexToAngle(3));
            Assert.AreEqual(90.0, squareOnEdge.IndexToAngle(4));
            Assert.AreEqual(135.0, squareOnEdge.IndexToAngle(-3));
            Assert.AreEqual(180.0, squareOnEdge.IndexToAngle(-2));
            Assert.AreEqual(225.0, squareOnEdge.IndexToAngle(-1));
            Assert.AreEqual(270.0, squareOnEdge.IndexToAngle(0));
            Assert.AreEqual(315.0, squareOnEdge.IndexToAngle(1));
        }

        [Test]
        public void IndexToCompass() {
            Assert.AreEqual(Compass.SouthEast, hexagonOnEdge.IndexToCompass(2));
            Assert.AreEqual(Compass.South, hexagonOnEdge.IndexToCompass(3));
            Assert.AreEqual(Compass.SouthWest, hexagonOnEdge.IndexToCompass(4));
            Assert.AreEqual(Compass.NorthWest, hexagonOnEdge.IndexToCompass(5));
            Assert.AreEqual(Compass.North, hexagonOnEdge.IndexToCompass(6));
            Assert.AreEqual(Compass.NorthEast, hexagonOnEdge.IndexToCompass(7));

            Assert.AreEqual(Compass.SouthEast, hexagonOnVertex.IndexToCompass(2));
            Assert.AreEqual(Compass.SouthWest, hexagonOnVertex.IndexToCompass(3));
            Assert.AreEqual(Compass.West, hexagonOnVertex.IndexToCompass(4));
            Assert.AreEqual(Compass.NorthWest, hexagonOnVertex.IndexToCompass(5));
            Assert.AreEqual(Compass.NorthEast, hexagonOnVertex.IndexToCompass(6));
            Assert.AreEqual(Compass.East, hexagonOnVertex.IndexToCompass(7));

            Assert.AreEqual(Compass.East, squareOnEdge.IndexToCompass(2));
            Assert.AreEqual(Compass.SouthEast, squareOnEdge.IndexToCompass(3));
            Assert.AreEqual(Compass.South, squareOnEdge.IndexToCompass(4));
            Assert.AreEqual(Compass.SouthWest, squareOnEdge.IndexToCompass(-3));
            Assert.AreEqual(Compass.West, squareOnEdge.IndexToCompass(-2));
            Assert.AreEqual(Compass.NorthWest, squareOnEdge.IndexToCompass(-1));
            Assert.AreEqual(Compass.North, squareOnEdge.IndexToCompass(0));
            Assert.AreEqual(Compass.NorthEast, squareOnEdge.IndexToCompass(1));
        }
    }
}
