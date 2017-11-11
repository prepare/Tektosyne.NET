using System;
using NUnit.Framework;
using Tektosyne;

namespace Tektosyne.UnitTest {

    [TestFixture]
    public class StringUtilityTest {

        [Test]
        public void CompareNatural() {
            Assert.AreEqual(0, StringUtility.CompareNatural(null, null));
            Assert.AreEqual(1, StringUtility.CompareNatural("", null));
            Assert.AreEqual(-1, StringUtility.CompareNatural(null, ""));

            Assert.AreEqual(0, StringUtility.CompareNatural("", ""));
            Assert.AreEqual(1, StringUtility.CompareNatural("b", ""));
            Assert.AreEqual(-1, StringUtility.CompareNatural("", "c"));

            Assert.AreEqual(0, StringUtility.CompareNatural("b", "b"));
            Assert.AreEqual(1, StringUtility.CompareNatural("b", "a"));
            Assert.AreEqual(-1, StringUtility.CompareNatural("b", "1"));
            Assert.AreEqual(-1, StringUtility.CompareNatural("b", "c"));

            Assert.AreEqual(0, StringUtility.CompareNatural("2", "02"));
            Assert.AreEqual(1, StringUtility.CompareNatural("2", "01"));
            Assert.AreEqual(-1, StringUtility.CompareNatural("2", "03"));

            Assert.AreEqual(0, StringUtility.CompareNatural("b-2", "b-02"));
            Assert.AreEqual(1, StringUtility.CompareNatural("b-2-xa", "b-02-x"));
            Assert.AreEqual(1, StringUtility.CompareNatural("b-2", "b-01"));
            Assert.AreEqual(1, StringUtility.CompareNatural("b-2", "b-01-x"));
            Assert.AreEqual(-1, StringUtility.CompareNatural("b-2", "b-03"));

            Assert.AreEqual(0, StringUtility.CompareNatural("2-b", "02-b"));
            Assert.AreEqual(1, StringUtility.CompareNatural("02-b-2", "2-b-1"));
            Assert.AreEqual(1, StringUtility.CompareNatural("2-ba", "02-b"));
            Assert.AreEqual(1, StringUtility.CompareNatural("02-b", "02-ax"));
            Assert.AreEqual(-1, StringUtility.CompareNatural("2-b", "2-c"));
        }

        [Test]
        public void CompareOrdinal() {
            Assert.AreEqual(0, StringUtility.CompareOrdinal(null, null));
            Assert.AreEqual(1, StringUtility.CompareOrdinal("", null));
            Assert.AreEqual(-1, StringUtility.CompareOrdinal(null, ""));

            Assert.AreEqual(0, StringUtility.CompareOrdinal("", ""));
            Assert.AreEqual(1, StringUtility.CompareOrdinal("b", ""));
            Assert.AreEqual(-1, StringUtility.CompareOrdinal("", "c"));

            Assert.AreEqual(0, StringUtility.CompareOrdinal("b", "b"));
            Assert.AreEqual(1, StringUtility.CompareOrdinal("b", "a"));
            Assert.AreEqual(-1, StringUtility.CompareOrdinal("b", "1"));
            Assert.AreEqual(-1, StringUtility.CompareOrdinal("b", "c"));

            Assert.AreEqual(0, StringUtility.CompareOrdinal("2", "02"));
            Assert.AreEqual(1, StringUtility.CompareOrdinal("2", "01"));
            Assert.AreEqual(-1, StringUtility.CompareOrdinal("2", "03"));

            Assert.AreEqual(0, StringUtility.CompareOrdinal("b-2", "b-02"));
            Assert.AreEqual(1, StringUtility.CompareOrdinal("b-2-xa", "b-02-x"));
            Assert.AreEqual(1, StringUtility.CompareOrdinal("b-2", "b-01"));
            Assert.AreEqual(1, StringUtility.CompareOrdinal("b-2", "b-01-x"));
            Assert.AreEqual(-1, StringUtility.CompareOrdinal("b-2", "b-03"));

            Assert.AreEqual(0, StringUtility.CompareOrdinal("2-b", "02-b"));
            Assert.AreEqual(1, StringUtility.CompareOrdinal("02-b-2", "2-b-1"));
            Assert.AreEqual(1, StringUtility.CompareOrdinal("2-ba", "02-b"));
            Assert.AreEqual(1, StringUtility.CompareOrdinal("02-b", "02-ax"));
            Assert.AreEqual(-1, StringUtility.CompareOrdinal("2-b", "2-c"));
        }

        [Test]
        public void IsValidEmail() {
            Assert.IsFalse(StringUtility.IsValidEmail(null));
            Assert.IsFalse(StringUtility.IsValidEmail(""));

            Assert.IsFalse(StringUtility.IsValidEmail("user"));
            Assert.IsTrue(StringUtility.IsValidEmail("user@host.com"));
            Assert.IsTrue(StringUtility.IsValidEmail("user@127.0.0.1"));
        }

        [Test]
        public void Validate() {
            Assert.AreEqual("(null)", StringUtility.Validate(null));
            Assert.AreEqual("(empty)", StringUtility.Validate(""));
            Assert.AreEqual("Hello", StringUtility.Validate("Hello"));

            Assert.AreEqual("(null)", StringUtility.Validate<ValidateTestClass>(null));
            Assert.AreEqual("(empty)", StringUtility.Validate(new ValidateTestClass()));
            Assert.AreEqual("314", StringUtility.Validate<Double>(314));

            Assert.AreEqual("(replace)", StringUtility.Validate(null, "(replace)"));
            Assert.AreEqual("(replace)", StringUtility.Validate("", "(replace)"));
            Assert.AreEqual("Hello", StringUtility.Validate("Hello", "(replace)"));

            Assert.AreEqual("(replace)", StringUtility.Validate<ValidateTestClass>(null, "(replace)"));
            Assert.AreEqual("(replace)", StringUtility.Validate(new ValidateTestClass(), "(replace)"));
            Assert.AreEqual("314", StringUtility.Validate<Double>(314, "(replace)"));
        }

        [Test]
        public void ValidateCollection() {
            Assert.AreEqual("(null)", StringUtility.ValidateCollection<ValidateTestClass>(null));
            Assert.AreEqual("[(empty)]", StringUtility.ValidateCollection(new[] { new ValidateTestClass() }));

            Assert.AreEqual("[]", StringUtility.ValidateCollection(new double[] { }));
            Assert.AreEqual("[314]", StringUtility.ValidateCollection(new double[] { 314 }));
            Assert.AreEqual("[314, 526]", StringUtility.ValidateCollection(new double[] { 314, 526 }));
        }

        private class ValidateTestClass {
            public override string ToString() { return ""; }
        }

        [Test]
        public void ValidOrNull() {
            Assert.IsNull(StringUtility.ValidOrNull(null));
            Assert.IsNull(StringUtility.ValidOrNull(""));
            Assert.IsNull(StringUtility.ValidOrNull("  "));
            Assert.IsNull(StringUtility.ValidOrNull("Hello", "Hello"));
            Assert.AreEqual("Hello", StringUtility.ValidOrNull("Hello", "hello"));
        }
    }
}
