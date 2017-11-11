using System;
using NUnit.Framework;
using Tektosyne;

namespace Tektosyne.UnitTest {

    using TestTuple = MutableTuple<String, Int32>;

    [TestFixture]
    public class MutableTuple2Test {

        [Test]
        public void EqualsTest() {
            Assert.AreEqual(new TestTuple("Hello", 0), new TestTuple("Hello", 0));
            Assert.AreEqual(new TestTuple(null, 42), new TestTuple(null, 42));
            Assert.AreEqual(new TestTuple("Hello", 42), new TestTuple("Hello", 42));

            Assert.AreNotEqual(new TestTuple(), new TestTuple("Hello", 0));
            Assert.AreNotEqual(new TestTuple("Hello", 0), new TestTuple(null, 42));
            Assert.AreNotEqual(new TestTuple(null, 42), new TestTuple("Hello", 42));
            Assert.AreNotEqual(new TestTuple("Hello", 42), new TestTuple("Hello", 0));
        }

        [Test]
        public void EqualsOperatorTest() {
            Assert.IsTrue(new TestTuple("Hello", 0) == new TestTuple("Hello", 0));
            Assert.IsTrue(new TestTuple(null, 42) == new TestTuple(null, 42));
            Assert.IsTrue(new TestTuple("Hello", 42) == new TestTuple("Hello", 42));

            Assert.IsTrue(new TestTuple() != new TestTuple("Hello", 0));
            Assert.IsTrue(new TestTuple("Hello", 0) != new TestTuple(null, 42));
            Assert.IsTrue(new TestTuple(null, 42) != new TestTuple("Hello", 42));
            Assert.IsTrue(new TestTuple("Hello", 42) != new TestTuple("Hello", 0));
        }

        [Test]
        public void GetHashCodeTest() {
            Assert.AreEqual(0, new TestTuple().GetHashCode());
            unchecked {
                Assert.AreEqual("Hello".GetHashCode(), new TestTuple("Hello", 0).GetHashCode());
                Assert.AreEqual(42.GetHashCode(), new TestTuple(null, 42).GetHashCode());
                Assert.AreEqual("Hello".GetHashCode() ^ 42.GetHashCode(),
                    new TestTuple("Hello", 42).GetHashCode());
            }
        }

        [Test]
        public void ToStringTest() {
            Assert.AreEqual("((null), 0)", new TestTuple().ToString());
            Assert.AreEqual("(Hello, 0)", new TestTuple("Hello", 0).ToString());
            Assert.AreEqual("((null), 42)", new TestTuple(null, 42).ToString());
            Assert.AreEqual("(Hello, 42)", new TestTuple("Hello", 42).ToString());
        }
    }
}
