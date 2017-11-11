using System;
using NUnit.Framework;
using Tektosyne;

namespace Tektosyne.UnitTest {

    [TestFixture]
    public class MathUtilityTest {

        [Test]
        public void CompareDouble() {
            Assert.AreEqual( 0, MathUtility.Compare(0.0, 0.0, Double.Epsilon));
            Assert.AreEqual( 0, MathUtility.Compare(0.0, Double.Epsilon, Double.Epsilon));
            Assert.AreEqual( 0, MathUtility.Compare(0.0, -Double.Epsilon, Double.Epsilon));

            Assert.AreEqual(-1, MathUtility.Compare(0.0, 1.0, Double.Epsilon));
            Assert.AreEqual(-1, MathUtility.Compare(-Double.Epsilon, Double.Epsilon, Double.Epsilon));
            Assert.AreEqual(+1, MathUtility.Compare(0.0, -1.0, Double.Epsilon));
            Assert.AreEqual(+1, MathUtility.Compare(Double.Epsilon, -Double.Epsilon, Double.Epsilon));
        }

        [Test]
        public void CompareDoubleEpsilon() {
            const double epsilon = 0.5;
            Assert.AreEqual( 0, MathUtility.Compare(0.0, 0.0, epsilon));

            Assert.AreEqual(-1, MathUtility.Compare(0.0, 1.0, epsilon));
            Assert.AreEqual( 0, MathUtility.Compare(-Double.Epsilon, Double.Epsilon, epsilon));
            Assert.AreEqual(+1, MathUtility.Compare(0.0, -1.0, epsilon));
            Assert.AreEqual( 0, MathUtility.Compare(Double.Epsilon, -Double.Epsilon, epsilon));
        }

        [Test]
        public void CompareSingle() {
            Assert.AreEqual( 0, MathUtility.Compare(0f, 0f, Single.Epsilon));
            Assert.AreEqual( 0, MathUtility.Compare(0f, Single.Epsilon, Single.Epsilon));
            Assert.AreEqual( 0, MathUtility.Compare(0f, -Single.Epsilon, Single.Epsilon));

            Assert.AreEqual(-1, MathUtility.Compare(0f, 1f, Single.Epsilon));
            Assert.AreEqual(-1, MathUtility.Compare(-Single.Epsilon, Single.Epsilon, Single.Epsilon));
            Assert.AreEqual(+1, MathUtility.Compare(0f, -1f, Single.Epsilon));
            Assert.AreEqual(+1, MathUtility.Compare(Single.Epsilon, -Single.Epsilon, Single.Epsilon));
        }

        [Test]
        public void CompareSingleEpsilon() {
            float epsilon = 0.5f;
            Assert.AreEqual( 0, MathUtility.Compare(0f, 0f, epsilon));

            Assert.AreEqual(-1, MathUtility.Compare(0f, 1f, epsilon));
            Assert.AreEqual( 0, MathUtility.Compare(-Single.Epsilon, Single.Epsilon, epsilon));
            Assert.AreEqual(+1, MathUtility.Compare(0f, -1f, epsilon));
            Assert.AreEqual( 0, MathUtility.Compare(Single.Epsilon, -Single.Epsilon, epsilon));
        }

        [Test]
        public void EqualsDouble() {
            Assert.IsTrue(MathUtility.Equals(0.0, 0.0, Double.Epsilon));
            Assert.IsTrue(MathUtility.Equals(0.0, Double.Epsilon, Double.Epsilon));
            Assert.IsTrue(MathUtility.Equals(0.0, -Double.Epsilon, Double.Epsilon));

            Assert.IsFalse(MathUtility.Equals(0.0, 1.0, Double.Epsilon));
            Assert.IsFalse(MathUtility.Equals(-Double.Epsilon, Double.Epsilon, Double.Epsilon));
            Assert.IsFalse(MathUtility.Equals(0.0, -1.0, Double.Epsilon));
            Assert.IsFalse(MathUtility.Equals(Double.Epsilon, -Double.Epsilon, Double.Epsilon));
        }

        [Test]
        public void EqualsDoubleEpsilon() {
            const double epsilon = 0.5;
            Assert.IsTrue(MathUtility.Equals(0.0, 0.0, epsilon));

            Assert.IsFalse(MathUtility.Equals(0.0, 1.0, epsilon));
            Assert.IsTrue(MathUtility.Equals(-Double.Epsilon, Double.Epsilon, epsilon));
            Assert.IsFalse(MathUtility.Equals(0.0, -1.0, epsilon));
            Assert.IsTrue(MathUtility.Equals(Double.Epsilon, -Double.Epsilon, epsilon));
        }

        [Test]
        public void EqualsSingle() {
            Assert.IsTrue(MathUtility.Equals(0f, 0f, Single.Epsilon));
            Assert.IsTrue(MathUtility.Equals(0f, Single.Epsilon, Single.Epsilon));
            Assert.IsTrue(MathUtility.Equals(0f, -Single.Epsilon, Single.Epsilon));

            Assert.IsFalse(MathUtility.Equals(0f, 1f, Single.Epsilon));
            Assert.IsFalse(MathUtility.Equals(-Single.Epsilon, Single.Epsilon, Single.Epsilon));
            Assert.IsFalse(MathUtility.Equals(0f, -1f, Single.Epsilon));
            Assert.IsFalse(MathUtility.Equals(Single.Epsilon, -Single.Epsilon, Single.Epsilon));
        }

        [Test]
        public void EqualsSingleEpsilon() {
            float epsilon = 0.5f;
            Assert.IsTrue(MathUtility.Equals(0f, 0f, epsilon));

            Assert.IsFalse(MathUtility.Equals(0f, 1f, epsilon));
            Assert.IsTrue(MathUtility.Equals(-Single.Epsilon, Single.Epsilon, epsilon));
            Assert.IsFalse(MathUtility.Equals(0f, -1f, epsilon));
            Assert.IsTrue(MathUtility.Equals(Single.Epsilon, -Single.Epsilon, epsilon));
        }

        [Test]
        public void IsPrime() {
            Assert.IsTrue(MathUtility.IsPrime(1));
            Assert.IsTrue(MathUtility.IsPrime(2));
            Assert.IsTrue(MathUtility.IsPrime(3));
            Assert.IsFalse(MathUtility.IsPrime(4));

            Assert.Throws<ArgumentOutOfRangeException>(() => MathUtility.IsPrime(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => MathUtility.IsPrime(-1));

            Assert.IsTrue(MathUtility.IsPrime(3559));
            Assert.IsFalse(MathUtility.IsPrime(3561));
            Assert.IsTrue(MathUtility.IsPrime(4052687u));
            Assert.IsFalse(MathUtility.IsPrime(4052689u));
        }
    }
}
