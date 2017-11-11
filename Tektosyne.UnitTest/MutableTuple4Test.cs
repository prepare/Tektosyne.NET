using System;
using NUnit.Framework;
using Tektosyne;

namespace Tektosyne.UnitTest {

    using TestTuple = MutableTuple<String, Double?, Int32, Int32?>;

    [TestFixture]
    public class MutableTuple4Test {

        [Test]
        public void EqualsTest() {
            Assert.AreEqual(new TestTuple("Hello", null, 0, null), new TestTuple("Hello", null, 0, null));
            Assert.AreEqual(new TestTuple(null, 3.14, 0, null), new TestTuple(null, 3.14, 0, null));
            Assert.AreEqual(new TestTuple(null, null, 42, null), new TestTuple(null, null, 42, null));
            Assert.AreEqual(new TestTuple(null, null, 0, 19), new TestTuple(null, null, 0, 19));
            Assert.AreEqual(new TestTuple("Hello", 3.14, 42, 19), new TestTuple("Hello", 3.14, 42, 19));

            Assert.AreNotEqual(new TestTuple(), new TestTuple("Hello", null, 0, null));
            Assert.AreNotEqual(new TestTuple("Hello", null, 0, null), new TestTuple(null, 3.14, 0, null));
            Assert.AreNotEqual(new TestTuple(null, 3.14, 0, null), new TestTuple(null, null, 42, null));
            Assert.AreNotEqual(new TestTuple(null, null, 42, null), new TestTuple("Hello", 3.14, 42, null));
            Assert.AreNotEqual(new TestTuple(null, null, 42, 19), new TestTuple("Hello", 3.14, 42, 19));
            Assert.AreNotEqual(new TestTuple("Hello", 3.14, 42, 19), new TestTuple("Hello", null, 0, 19));
        }

        [Test]
        public void EqualsOperatorTest() {
            Assert.IsTrue(new TestTuple("Hello", null, 0, null) == new TestTuple("Hello", null, 0, null));
            Assert.IsTrue(new TestTuple(null, 3.14, 0, null) == new TestTuple(null, 3.14, 0, null));
            Assert.IsTrue(new TestTuple(null, null, 42, null) == new TestTuple(null, null, 42, null));
            Assert.IsTrue(new TestTuple(null, null, 0, 19) == new TestTuple(null, null, 0, 19));
            Assert.IsTrue(new TestTuple("Hello", 3.14, 42, 19) == new TestTuple("Hello", 3.14, 42, 19));

            Assert.IsTrue(new TestTuple() != new TestTuple("Hello", null, 0, null));
            Assert.IsTrue(new TestTuple("Hello", null, 0, null) != new TestTuple(null, 3.14, 0, null));
            Assert.IsTrue(new TestTuple(null, 3.14, 0, null) != new TestTuple(null, null, 42, null));
            Assert.IsTrue(new TestTuple(null, null, 42, null) != new TestTuple("Hello", 3.14, 42, null));
            Assert.IsTrue(new TestTuple(null, null, 0, 19) != new TestTuple("Hello", 3.14, 42, 19));
            Assert.IsTrue(new TestTuple("Hello", 3.14, 42, 19) != new TestTuple("Hello", null, 0, null));
        }

        [Test]
        public void GetHashCodeTest() {
            Assert.AreEqual(0, new TestTuple().GetHashCode());
            unchecked {
                Assert.AreEqual("Hello".GetHashCode(), new TestTuple("Hello", null, 0, null).GetHashCode());
                Assert.AreEqual((3.14).GetHashCode(), new TestTuple(null, 3.14, 0, null).GetHashCode());
                Assert.AreEqual(42.GetHashCode(), new TestTuple(null, null, 42, null).GetHashCode());
                Assert.AreEqual(19.GetHashCode(), new TestTuple(null, null, 0, 19).GetHashCode());

                Assert.AreEqual(
                    "Hello".GetHashCode() ^ (3.14).GetHashCode() ^ 42.GetHashCode() ^ 19.GetHashCode(),
                    new TestTuple("Hello", 3.14, 42, 19).GetHashCode());
            }
        }

        [Test]
        public void ToStringTest() {
            Assert.AreEqual("((null), (null), 0, (null))", new TestTuple().ToString());
            Assert.AreEqual("(Hello, (null), 0, (null))", new TestTuple("Hello", null, 0, null).ToString());
            Assert.AreEqual("((null), 3.14, 0, (null))", new TestTuple(null, 3.14, 0, null).ToString());
            Assert.AreEqual("((null), (null), 42, (null))", new TestTuple(null, null, 42, null).ToString());
            Assert.AreEqual("((null), (null), 0, 19)", new TestTuple(null, null, 0, 19).ToString());
            Assert.AreEqual("(Hello, 3.14, 42, 19)", new TestTuple("Hello", 3.14, 42, 19).ToString());
        }
    }
}
