using System;
using NUnit.Framework;
using Tektosyne;

namespace Tektosyne.UnitTest {

    [TestFixture]
    public class OrdinalStringTest {

        [Test]
        public void Compare() {
            Assert.AreEqual(new OrdinalString(null), new OrdinalString(null));
            Assert.Greater(new OrdinalString(""), new OrdinalString(null));
            Assert.Less(new OrdinalString(null), new OrdinalString(""));

            Assert.AreEqual(new OrdinalString(""), new OrdinalString(""));
            Assert.Greater(new OrdinalString("b"), new OrdinalString(""));
            Assert.Less(new OrdinalString(""), new OrdinalString("c"));

            Assert.AreEqual(new OrdinalString("b"), new OrdinalString("b"));
            Assert.Greater(new OrdinalString("b"), new OrdinalString("a"));
            Assert.Less(new OrdinalString("b"), new OrdinalString("1"));
            Assert.Less(new OrdinalString("b"), new OrdinalString("c"));

            Assert.AreEqual(new OrdinalString("2"), new OrdinalString("02"));
            Assert.Greater(new OrdinalString("2"), new OrdinalString("01"));
            Assert.Less(new OrdinalString("2"), new OrdinalString("03"));

            Assert.AreEqual(new OrdinalString("b-2"), new OrdinalString("b-02"));
            Assert.Greater(new OrdinalString("b-2-xa"), new OrdinalString("b-02-x"));
            Assert.Greater(new OrdinalString("b-2"), new OrdinalString("b-01"));
            Assert.Greater(new OrdinalString("b-2"), new OrdinalString("b-01-x"));
            Assert.Less(new OrdinalString("b-2"), new OrdinalString("b-03"));

            Assert.AreEqual(new OrdinalString("2-b"), new OrdinalString("02-b"));
            Assert.Greater(new OrdinalString("02-b-2"), new OrdinalString("2-b-1"));
            Assert.Greater(new OrdinalString("2-ba"), new OrdinalString("02-b"));
            Assert.Greater(new OrdinalString("02-b"), new OrdinalString("02-ax"));
            Assert.Less(new OrdinalString("2-b"), new OrdinalString("2-c"));
        }
    }
}
