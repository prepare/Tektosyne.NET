using System;
using NUnit.Framework;
using Tektosyne;

namespace Tektosyne.UnitTest {

    [TestFixture]
    public class NaturalStringTest {

        [Test]
        public void Compare() {
            Assert.AreEqual(new NaturalString(null), new NaturalString(null));
            Assert.Greater(new NaturalString(""), new NaturalString(null));
            Assert.Less(new NaturalString(null), new NaturalString(""));

            Assert.AreEqual(new NaturalString(""), new NaturalString(""));
            Assert.Greater(new NaturalString("b"), new NaturalString(""));
            Assert.Less(new NaturalString(""), new NaturalString("c"));

            Assert.AreEqual(new NaturalString("b"), new NaturalString("b"));
            Assert.Greater(new NaturalString("b"), new NaturalString("a"));
            Assert.Less(new NaturalString("b"), new NaturalString("1"));
            Assert.Less(new NaturalString("b"), new NaturalString("c"));

            Assert.AreEqual(new NaturalString("2"), new NaturalString("02"));
            Assert.Greater(new NaturalString("2"), new NaturalString("01"));
            Assert.Less(new NaturalString("2"), new NaturalString("03"));

            Assert.AreEqual(new NaturalString("b-2"), new NaturalString("b-02"));
            Assert.Greater(new NaturalString("b-2-xa"), new NaturalString("b-02-x"));
            Assert.Greater(new NaturalString("b-2"), new NaturalString("b-01"));
            Assert.Greater(new NaturalString("b-2"), new NaturalString("b-01-x"));
            Assert.Less(new NaturalString("b-2"), new NaturalString("b-03"));

            Assert.AreEqual(new NaturalString("2-b"), new NaturalString("02-b"));
            Assert.Greater(new NaturalString("02-b-2"), new NaturalString("2-b-1"));
            Assert.Greater(new NaturalString("2-ba"), new NaturalString("02-b"));
            Assert.Greater(new NaturalString("02-b"), new NaturalString("02-ax"));
            Assert.Less(new NaturalString("2-b"), new NaturalString("2-c"));
        }
    }
}
