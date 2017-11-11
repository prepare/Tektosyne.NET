using System;
using NUnit.Framework;
using Tektosyne;

namespace Tektosyne.UnitTest {

    [TestFixture]
    public class FortranTest {

        [Test]
        public void Ceiling() {
            Assert.AreEqual(-3, Fortran.Ceiling(-3.4m));
            Assert.AreEqual(-3, Fortran.Ceiling(-3.4d));
            Assert.AreEqual(-3, Fortran.Ceiling(-3.4f));
        }

        [Test]
        public void Floor() {
            Assert.AreEqual(-4, Fortran.Floor(-3.4m));
            Assert.AreEqual(-4, Fortran.Floor(-3.4d));
            Assert.AreEqual(-4, Fortran.Floor(-3.4f));
        }

        [Test]
        public void Modulo() {
            Assert.AreEqual(2m, Fortran.Modulo(12m, 5m));
            Assert.AreEqual(2d, Fortran.Modulo(12d, 5d));
            Assert.AreEqual(2f, Fortran.Modulo(12f, 5f));
            Assert.AreEqual(2, Fortran.Modulo(12, 5));
            Assert.AreEqual(2L, Fortran.Modulo(12L, 5L));

            Assert.AreEqual(3m, Fortran.Modulo(-12m, 5m));
            Assert.AreEqual(3d, Fortran.Modulo(-12d, 5d));
            Assert.AreEqual(3f, Fortran.Modulo(-12f, 5f));
            Assert.AreEqual(3, Fortran.Modulo(-12, 5));
            Assert.AreEqual(3L, Fortran.Modulo(-12L, 5L));

            Assert.AreEqual(-3m, Fortran.Modulo(12m, -5m));
            Assert.AreEqual(-3d, Fortran.Modulo(12d, -5d));
            Assert.AreEqual(-3f, Fortran.Modulo(12f, -5f));
            Assert.AreEqual(-3, Fortran.Modulo(12, -5));
            Assert.AreEqual(-3L, Fortran.Modulo(12L, -5L));

            Assert.AreEqual(-2m, Fortran.Modulo(-12m, -5m));
            Assert.AreEqual(-2d, Fortran.Modulo(-12d, -5d));
            Assert.AreEqual(-2f, Fortran.Modulo(-12f, -5f));
            Assert.AreEqual(-2, Fortran.Modulo(-12, -5));
            Assert.AreEqual(-2L, Fortran.Modulo(-12L, -5L));
        }
    }
}
