using System;
using NUnit.Framework;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Geometry {

    [TestFixture]
    public class SizeTest {

        SizeD sizeD = new SizeD(1, 2);
        SizeF sizeF = new SizeF(1, 2);
        SizeI sizeI = new SizeI(1, 2);

        [Test]
        public void Constructor() {
            Assert.Throws<ArgumentOutOfRangeException>(() => { var size = new SizeD(-1, 2); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var size = new SizeD(1, -2); });

            Assert.Throws<ArgumentOutOfRangeException>(() => { var size = new SizeF(-1, 2); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var size = new SizeF(1, -2); });
            
            Assert.Throws<ArgumentOutOfRangeException>(() => { var size = new SizeI(-1, 2); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var size = new SizeI(1, -2); });
        }

        [Test]
        public void Conversion() {
            Assert.AreEqual(sizeD, (SizeD) sizeF);
            Assert.AreEqual(sizeD, (SizeD) sizeI);

            Assert.AreEqual(sizeF, (SizeF) sizeD);
            Assert.AreEqual(sizeF, (SizeF) sizeI);

            Assert.AreEqual(sizeI, (SizeI) sizeD);
            Assert.AreEqual(sizeI, (SizeI) sizeF);

            Assert.AreEqual(new SizeI(0, 1), (SizeI) new SizeD(0.6, 1.6));
            Assert.AreEqual(new SizeI(0, 1), (SizeI) new SizeF(0.6f, 1.6f));

            Assert.AreEqual(sizeI, (SizeI) new SizeD(1.4, 2.4));
            Assert.AreEqual(sizeI, (SizeI) new SizeF(1.4f, 2.4f));
        }
        
        [Test]
        public void EqualsEpsilon() {
            Assert.IsTrue(SizeD.Equals(sizeD, new SizeD(1.1, 1.9), 0.2));
            Assert.IsTrue(SizeF.Equals(sizeF, new SizeF(1.1f, 1.9f), 0.2f));
        }

        [Test]
        public void Add() {
            Assert.AreEqual(new SizeD(4, 6), sizeD + new SizeD(3, 4));
            Assert.AreEqual(new SizeF(4, 6), sizeF + new SizeF(3, 4));
            Assert.AreEqual(new SizeI(4, 6), sizeI + new SizeI(3, 4));
        }

        [Test]
        public void Restrict() {
            Assert.AreEqual(sizeD, new SizeD(0, 0).Restrict(1, 2, 9, 9));
            Assert.AreEqual(sizeD, new SizeD(9, 9).Restrict(0, 0, 1, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => { sizeD.Restrict(0, 0, -1, -2); });

            Assert.AreEqual(sizeF, new SizeF(0, 0).Restrict(1, 2, 9, 9));
            Assert.AreEqual(sizeF, new SizeF(9, 9).Restrict(0, 0, 1, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => { sizeF.Restrict(0, 0, -1, -2); });

            Assert.AreEqual(sizeI, new SizeI(0, 0).Restrict(1, 2, 9, 9));
            Assert.AreEqual(sizeI, new SizeI(9, 9).Restrict(0, 0, 1, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => { sizeI.Restrict(0, 0, -1, -2); });
        }

        [Test]
        public void Round() {
            Assert.AreEqual(sizeI, sizeD.Round());
            Assert.AreEqual(sizeI, new SizeD(0.6, 1.6).Round());
            Assert.AreEqual(sizeI, new SizeD(1.4, 2.4).Round());

            Assert.AreEqual(sizeI, sizeF.Round());
            Assert.AreEqual(sizeI, new SizeF(0.6f, 1.6f).Round());
            Assert.AreEqual(sizeI, new SizeF(1.4f, 2.4f).Round());
        }

        [Test]
        public void Subtract() {
            Assert.AreEqual(new SizeD(2, 2), new SizeD(3, 4) - sizeD);
            Assert.AreEqual(new SizeF(2, 2), new SizeF(3, 4) - sizeF);
            Assert.AreEqual(new SizeI(2, 2), new SizeI(3, 4) - sizeI);

            Assert.Throws<ArgumentOutOfRangeException>(() => { var size = sizeD - new SizeD(3, 4); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var size = sizeF - new SizeF(3, 4); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var size = sizeI - new SizeI(3, 4); });
        }
    }
}
