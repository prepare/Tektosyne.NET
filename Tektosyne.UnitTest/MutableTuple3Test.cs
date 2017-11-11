using System;
using NUnit.Framework;
using Tektosyne;

namespace Tektosyne.UnitTest {

    using TestTuple = MutableTuple<String, Double?, Int32>;

    [TestFixture]
    public class MutableTuple3Test {

        [Test]
        public void EqualsTest() {
            Assert.AreEqual(new TestTuple("Hello", null, 0), new TestTuple("Hello", null, 0));
            Assert.AreEqual(new TestTuple(null, 3.14, 0), new TestTuple(null, 3.14, 0));
            Assert.AreEqual(new TestTuple(null, null, 42), new TestTuple(null, null, 42));
            Assert.AreEqual(new TestTuple("Hello", 3.14, 42), new TestTuple("Hello", 3.14, 42));

            Assert.AreNotEqual(new TestTuple(), new TestTuple("Hello", null, 0));
            Assert.AreNotEqual(new TestTuple("Hello", null, 0), new TestTuple(null, 3.14, 0));
            Assert.AreNotEqual(new TestTuple(null, 3.14, 0), new TestTuple(null, null, 42));
            Assert.AreNotEqual(new TestTuple(null, null, 42), new TestTuple("Hello", 3.14, 42));
            Assert.AreNotEqual(new TestTuple("Hello", 3.14, 42), new TestTuple("Hello", null, 0));
        }

        [Test]
        public void EqualsOperatorTest() {
            Assert.IsTrue(new TestTuple("Hello", null, 0) == new TestTuple("Hello", null, 0));
            Assert.IsTrue(new TestTuple(null, 3.14, 0) == new TestTuple(null, 3.14, 0));
            Assert.IsTrue(new TestTuple(null, null, 42) == new TestTuple(null, null, 42));
            Assert.IsTrue(new TestTuple("Hello", 3.14, 42) == new TestTuple("Hello", 3.14, 42));

            Assert.IsTrue(new TestTuple() != new TestTuple("Hello", null, 0));
            Assert.IsTrue(new TestTuple("Hello", null, 0) != new TestTuple(null, 3.14, 0));
            Assert.IsTrue(new TestTuple(null, 3.14, 0) != new TestTuple(null, null, 42));
            Assert.IsTrue(new TestTuple(null, null, 42) != new TestTuple("Hello", 3.14, 42));
            Assert.IsTrue(new TestTuple("Hello", 3.14, 42) != new TestTuple("Hello", null, 0));
        }

        [Test]
        public void GetHashCodeTest() {
            Assert.AreEqual(0, new TestTuple().GetHashCode());
            unchecked {
                Assert.AreEqual("Hello".GetHashCode(), new TestTuple("Hello", null, 0).GetHashCode());
                Assert.AreEqual((3.14).GetHashCode(), new TestTuple(null, 3.14, 0).GetHashCode());
                Assert.AreEqual(42.GetHashCode(), new TestTuple(null, null, 42).GetHashCode());
                Assert.AreEqual("Hello".GetHashCode() ^ (3.14).GetHashCode() ^ 42.GetHashCode(),
                    new TestTuple("Hello", 3.14, 42).GetHashCode());
            }
        }

        [Test]
        public void ToStringTest() {
            Assert.AreEqual("((null), (null), 0)", new TestTuple().ToString());
            Assert.AreEqual("(Hello, (null), 0)", new TestTuple("Hello", null, 0).ToString());
            Assert.AreEqual("((null), 3.14, 0)", new TestTuple(null, 3.14, 0).ToString());
            Assert.AreEqual("((null), (null), 42)", new TestTuple(null, null, 42).ToString());
            Assert.AreEqual("(Hello, 3.14, 42)", new TestTuple("Hello", 3.14, 42).ToString());
        }
    }
}
