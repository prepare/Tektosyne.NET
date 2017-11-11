using System;
using System.Collections.Generic;

using NUnit.Framework;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    [TestFixture]
    public class Int32HashSetTest {

        Int32HashSet _collection, _readOnly;

        [SetUp]
        public void SetUp() {
            _collection = new Int32HashSet();
            _readOnly = _collection.AsReadOnly();
            Assert.AreEqual(0, _collection.Count);

            _collection.Add(1);
            _collection.Add(2);
            _collection.Add(3);
            Assert.AreEqual(3, _collection.Count);
        }

        [TearDown]
        public void TearDown() {
            _collection.Clear();
            _collection = null;
        }

        [Test]
        public void Constructor() {
            var dictionary = new Int32HashSet();
            dictionary.Add(37);
            dictionary.Add(43);
            Assert.AreEqual(2, dictionary.Count);

            var clone = new Int32HashSet(dictionary);
            Assert.AreEqual(2, clone.Count);
        }

        [Test]
        public void Count() {
            _collection.Add(37);
            _collection.Add(43);
            Assert.AreEqual(5, _collection.Count);

            _collection.Remove(43);
            Assert.AreEqual(4, _collection.Count);
        }

        [Test]
        public void Empty() {
            Assert.AreSame(Int32HashSet.Empty, Int32HashSet.Empty);
            Assert.IsTrue(Int32HashSet.Empty.IsReadOnly);
            Assert.AreEqual(Int32HashSet.Empty.Count, 0);
        }

        [Test]
        public void IsFixedSize() {
            Assert.IsFalse(_collection.IsFixedSize);
            Assert.IsTrue(_readOnly.IsFixedSize);
        }

        [Test]
        public void IsReadOnly() {
            Assert.IsFalse(_collection.IsReadOnly);
            Assert.IsTrue(_readOnly.IsReadOnly);
        }

        [Test]
        public void Add() {
            Assert.Throws<ArgumentException>(() => _collection.Add(2));
            Assert.Throws<NotSupportedException>(() => _readOnly.Add(37));

            for (int i = 0; i < 20; i++) {
                int item = i + 37;
                _collection.Add(item);
                Assert.IsTrue(_collection.Contains(item));
            }
            Assert.AreEqual(23, _collection.Count);
        }

        [Test]
        public void AddRange() {
            var collection = new Int32HashSet();
            collection.Add(4);
            collection.Add(5);

            Assert.Throws<NotSupportedException>(() => _readOnly.AddRange(collection));
            _collection.AddRange(collection);

            Assert.AreEqual(5, _collection.Count);
            Assert.IsTrue(_collection.Contains(4));
            Assert.IsTrue(_collection.Contains(5));

            Assert.Throws<ArgumentException>(() => _collection.AddRange(collection));
        }

        [Test]
        public void AsReadOnly() {
            Assert.IsTrue(_collection.Equals(_readOnly));

            _collection.Add(6);
            Assert.IsTrue(_collection.Equals(_readOnly));

            _collection.Remove(2);
            Assert.IsTrue(_collection.Equals(_readOnly));
        }

        [Test]
        public void Clear() {
            Assert.Throws<NotSupportedException>(_readOnly.Clear);
            _collection.Clear();
            Assert.AreEqual(0, _collection.Count);
        }

        [Test]
        public void Clone() {
            var clone = (Int32HashSet) _readOnly.Clone();
            Assert.IsFalse(clone.IsReadOnly);

            Assert.AreEqual(_collection.Count, clone.Count);
            foreach (int item in clone)
                Assert.IsTrue(_collection.Contains(item));
        }

        [Test]
        public void Contains() {
            Assert.IsTrue(_collection.Contains(2));
            Assert.IsFalse(_collection.Contains(4));
        }

        [Test]
        public void CopyToEmpty() {
            _collection.Clear();
            int[] array = new int[0];
            _collection.CopyTo(array, 0);
        }

        [Test]
        public void CopyTo() {
            int[] array = new int[4];
            Assert.Throws<ArgumentException>(() => _collection.CopyTo(array, 3));

            _collection.CopyTo(array, 0);
            for (int i = 0; i < _collection.Count; i++)
                Assert.IsTrue(_collection.Contains(array[i]));

            _collection.CopyTo(array, 1);
            for (int i = 0; i < _collection.Count; i++)
                Assert.IsTrue(_collection.Contains(array[i + 1]));
        }

        [Test]
        public void Equals() {
            Int32HashSet collection = new Int32HashSet();
            collection.Add(1);
            collection.Add(2);
            collection.Add(3);
            Assert.IsTrue(_collection.Equals(collection));

            collection.Add(37);
            Assert.IsFalse(_collection.Equals(collection));
        }

        [Test]
        public void GetAny() {
            Assert.IsTrue(_collection.Contains(_collection.GetAny()));

            Int32HashSet collection = new Int32HashSet();
            Assert.Throws<InvalidOperationException>(() => collection.GetAny());
        }

        [Test]
        public void GetEnumerator() {
            foreach (int item in _collection)
                Assert.IsTrue(_collection.Contains(item));
        }

        [Test]
        public void Remove() {
            Assert.Throws<NotSupportedException>(() => _readOnly.Remove(2));
            Assert.IsTrue(_collection.Remove(2));
            Assert.IsFalse(_collection.Remove(4));
            Assert.AreEqual(2, _collection.Count);
        }

        [Test]
        public void ToArray() {
            int[] array = _collection.ToArray();
            Assert.AreEqual(_collection.Count, array.Length);
            for (int i = 0; i < _collection.Count; i++)
                Assert.IsTrue(_collection.Contains(array[i]));
        }

        [Test]
        public void Stress() {
            const int count = 3000;
            int[] array = new int[count];

            // multiples of prime number provoke hash collisions
            for (int i = 0; i < array.Length; i++)
                array[i] = i * 89;

            _collection.Clear();
            Assert.AreEqual(0, _collection.Count);

            // test adding elements
            foreach (int item in array)
                _collection.Add(item);
            Assert.AreEqual(array.Length, _collection.Count);

            // test finding elements
            foreach (int item in array)
                Assert.IsTrue(_collection.Contains(item));

            var standard = new HashSet<Int32>();
            foreach (int item in array) standard.Add(item);

            // test element enumeration
            foreach (int item in _collection) {
                Assert.IsTrue(standard.Contains(item));
                standard.Remove(item);
            }
            Assert.AreEqual(0, standard.Count);

            // test removing elements
            for (int i = array.Length - 1; i >= 0; i--)
                Assert.IsTrue(_collection.Remove(array[i]));
            Assert.AreEqual(0, _collection.Count);
        }
    }
}
