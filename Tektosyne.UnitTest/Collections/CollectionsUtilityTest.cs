using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    [TestFixture]
    public class CollectionsUtilityTest {

        [Test]
        public void IndexArray() {
            for (int count = 0; count < 10; count++) {
                int[] list = CollectionsUtility.IndexArray(count);
                for (int i = 0; i < list.Length; i++)
                    Assert.AreEqual(i, list[i]);
            }
        }

        [Test]
        public void Randomize() {
            for (int count = 0; count < 10; count++) {
                int[] list = CollectionsUtility.IndexArray(count);
                CollectionsUtility.Randomize(list);

                // check that all elements still occur exactly once
                bool[] found = new bool[count];
                for (int i = 0; i < list.Length; i++) {
                    Assert.IsFalse(found[list[i]]);
                    found[list[i]] = true;
                }
                for (int i = 0; i < list.Length; i++)
                    Assert.IsTrue(found[list[i]]);
            }
        }

        [Test]
        public void SequenceEqualColl() {

            Assert.IsTrue(CollectionsUtility.SequenceEqual(
                (ICollection<String>) null, (ICollection<String>) null));
            Assert.IsFalse(CollectionsUtility.SequenceEqual(
                (ICollection<String>) new List<String>(), (ICollection<String>) null));
            Assert.IsFalse(CollectionsUtility.SequenceEqual(
                (ICollection<String>) null, (ICollection<String>) new List<String>()));
            Assert.IsTrue(CollectionsUtility.SequenceEqual(
                (ICollection<String>) new List<String>(), (ICollection<String>) new List<String>()));

            Assert.IsTrue(CollectionsUtility.SequenceEqual(
                (ICollection<String>) new List<String>() { "One", "Two" },
                (ICollection<String>) new List<String>() { "One", "Two" }));

            Assert.IsFalse(CollectionsUtility.SequenceEqual(
                (ICollection<String>) new List<String>() { "One", "Two" },
                (ICollection<String>) new List<String>() { "Two", "One" }));
        }

        [Test]
        public void SequenceEqualList() {

            Assert.IsTrue(CollectionsUtility.SequenceEqual((IList<String>) null, null));
            Assert.IsFalse(CollectionsUtility.SequenceEqual(new List<String>(), null));
            Assert.IsFalse(CollectionsUtility.SequenceEqual(null, new List<String>()));
            Assert.IsTrue(CollectionsUtility.SequenceEqual(new List<String>(), new List<String>()));

            Assert.IsTrue(CollectionsUtility.SequenceEqual(
                new List<String>() { "One", "Two" }, new List<String>() { "One", "Two" }));
            Assert.IsFalse(CollectionsUtility.SequenceEqual(
                new List<String>() { "One", "Two" }, new List<String>() { "Two", "One" }));
        }

        [Test]
        public void SequenceEqualUntypedColl() {

            Assert.IsTrue(CollectionsUtility.SequenceEqualUntyped(
                (ICollection) null, (ICollection) null));
            Assert.IsFalse(CollectionsUtility.SequenceEqualUntyped(
                (ICollection) new ArrayList(), (ICollection) null));
            Assert.IsFalse(CollectionsUtility.SequenceEqualUntyped(
                (ICollection) null, (ICollection) new ArrayList()));
            Assert.IsTrue(CollectionsUtility.SequenceEqualUntyped(
                (ICollection) new ArrayList(), (ICollection) new ArrayList()));

            Assert.IsTrue(CollectionsUtility.SequenceEqualUntyped(
                (ICollection) new ArrayList() { "One", "Two" },
                (ICollection) new ArrayList() { "One", "Two" }));

            Assert.IsFalse(CollectionsUtility.SequenceEqualUntyped(
                (ICollection) new ArrayList() { "One", "Two" },
                (ICollection) new ArrayList() { "Two", "One" }));
        }

        [Test]
        public void SequenceEqualUntypedList() {

            Assert.IsTrue(CollectionsUtility.SequenceEqualUntyped(null, null));
            Assert.IsFalse(CollectionsUtility.SequenceEqualUntyped(new ArrayList(), null));
            Assert.IsFalse(CollectionsUtility.SequenceEqualUntyped(null, new ArrayList()));
            Assert.IsTrue(CollectionsUtility.SequenceEqualUntyped(new ArrayList(), new ArrayList()));

            Assert.IsTrue(CollectionsUtility.SequenceEqualUntyped(
                new ArrayList() { "One", "Two" }, new ArrayList() { "One", "Two" }));
            Assert.IsFalse(CollectionsUtility.SequenceEqualUntyped(
                new ArrayList() { "One", "Two" }, new ArrayList() { "Two", "One" }));
        }
    }
}
