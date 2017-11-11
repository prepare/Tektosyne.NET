using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    using Int32StringPair = KeyValuePair<Int32, CloneableType>;
    using Int32StringDictionary = Int32Dictionary<CloneableType>;

    [TestFixture]
    public class Int32DictionaryTest {

        Int32StringDictionary _dictionary, _readOnly;

        [SetUp]
        public void SetUp() {
            _dictionary = new Int32StringDictionary();
            _readOnly = _dictionary.AsReadOnly();
            Assert.AreEqual(0, _dictionary.Count);

            _dictionary.Add(1, "first value");
            _dictionary.Add(2, "second value");
            _dictionary.Add(3, "third value");
            Assert.AreEqual(3, _dictionary.Count);
        }

        [TearDown]
        public void TearDown() {
            _dictionary.Clear();
            _dictionary = null;
        }

        [Test]
        public void Constructor() {
            var dictionary = new Int32StringDictionary();
            dictionary.Add(37, "foo value");
            dictionary.Add(43, "bar value");
            Assert.AreEqual(2, dictionary.Count);

            var clone = new Int32StringDictionary(dictionary);
            Assert.AreEqual(2, clone.Count);
        }

        [Test]
        public void Count() {
            _dictionary.Add(37, "foo value");
            _dictionary.Add(43, "bar value");
            Assert.AreEqual(5, _dictionary.Count);

            _dictionary.Remove(43);
            Assert.AreEqual(4, _dictionary.Count);
        }

        [Test]
        public void Empty() {
            Assert.AreSame(Int32StringDictionary.Empty, Int32StringDictionary.Empty);
            Assert.IsTrue(Int32StringDictionary.Empty.IsReadOnly);
            Assert.AreEqual(Int32StringDictionary.Empty.Count, 0);
        }

        [Test]
        public void IsFixedSize() {
            Assert.IsFalse(_dictionary.IsFixedSize);
            Assert.IsTrue(_readOnly.IsFixedSize);
        }

        [Test]
        public void IsReadOnly() {
            Assert.IsFalse(_dictionary.IsReadOnly);
            Assert.IsTrue(_readOnly.IsReadOnly);
        }

        [Test]
        public void Item() {
            Assert.Throws<KeyNotFoundException>(() => { var value = _dictionary[43]; });
            Assert.Throws<NotSupportedException>(() => { _readOnly[2] = "foo value"; });

            Assert.AreEqual(new CloneableType("second value"), _dictionary[2]);
            _dictionary[2] = "foo value";
            Assert.AreEqual(new CloneableType("foo value"), _dictionary[2]);

            _dictionary[43] = "bar value";
            Assert.AreEqual(new CloneableType("bar value"), _dictionary[43]);
        }

        [Test]
        public void Keys() {
            Assert.AreEqual(_dictionary.Count, _dictionary.Keys.Count);
            foreach (int key in _dictionary.Keys)
                Assert.IsTrue(_dictionary.ContainsKey(key));
        }

        [Test]
        public void Values() {
            Assert.AreEqual(_dictionary.Count, _dictionary.Values.Count);
            foreach (CloneableType value in _dictionary.Values)
                Assert.IsTrue(_dictionary.ContainsValue(value));
        }

        [Test]
        public void Add() {
            Assert.Throws<ArgumentException>(() => _dictionary.Add(2, "another second value"));
            Assert.Throws<NotSupportedException>(() => _readOnly.Add(37, "bar value"));

            for (int i = 0; i < 20; i++) {
                int key = i + 37;
                _dictionary.Add(key, "bar value");
                Assert.IsTrue(_dictionary.Contains(new Int32StringPair(key, "bar value")));
            }
            Assert.AreEqual(23, _dictionary.Count);
        }

        [Test]
        public void AddNull() {
            _dictionary.Add(37, null);
            Assert.IsTrue(_dictionary.ContainsValue(null));
        }

        [Test]
        public void AddPair() {
            Assert.Throws<ArgumentException>(() =>
                _dictionary.Add(new Int32StringPair(2, "another second value")));
            Assert.Throws<NotSupportedException>(() =>
                _readOnly.Add(new Int32StringPair(4, "fourth value")));

            _dictionary.Add(new Int32StringPair(4, "fourth value"));
            Assert.AreEqual(4, _dictionary.Count);
            Assert.IsTrue(_dictionary.ContainsKey(4));
            Assert.IsTrue(_dictionary.ContainsValue("fourth value"));
        }

        [Test]
        public void AddRange() {
            var dictionary = new Int32StringDictionary();
            dictionary.Add(new Int32StringPair(4, "fourth value"));
            dictionary.Add(new Int32StringPair(5, "fifth value"));

            Assert.Throws<NotSupportedException>(() => _readOnly.AddRange(dictionary));
            _dictionary.AddRange(dictionary);

            Assert.AreEqual(5, _dictionary.Count);
            Assert.IsTrue(_dictionary.ContainsKey(4));
            Assert.IsTrue(_dictionary.ContainsValue("fourth value"));
            Assert.IsTrue(_dictionary.ContainsKey(5));
            Assert.IsTrue(_dictionary.ContainsValue("fifth value"));

            Assert.Throws<ArgumentException>(() => _dictionary.AddRange(dictionary));
        }

        [Test]
        public void AsReadOnly() {
            Assert.IsTrue(_dictionary.Equals(_readOnly));

            _dictionary[2] = "foo value";
            Assert.IsTrue(_dictionary.Equals(_readOnly));

            _dictionary[2] = "second value";
            Assert.IsTrue(_dictionary.Equals(_readOnly));
        }

        [Test]
        public void Clear() {
            Assert.Throws<NotSupportedException>(_readOnly.Clear);
            _dictionary.Clear();
            Assert.AreEqual(0, _dictionary.Count);
        }

        [Test]
        public void Clone() {
            var clone = (Int32StringDictionary) _readOnly.Clone();
            Assert.IsFalse(clone.IsReadOnly);

            Assert.AreEqual(_dictionary.Count, clone.Count);
            foreach (Int32StringPair item in clone) {
                Assert.IsTrue(_dictionary.Contains(item));
                Assert.AreEqual(_dictionary[item.Key], item.Value);
                Assert.AreSame(_dictionary[item.Key], item.Value);
            }
        }

        [Test]
        public void Contains() {
            Assert.IsTrue(_dictionary.Contains(new Int32StringPair(2, "second value")));
            Assert.IsFalse(_dictionary.Contains(new Int32StringPair(4, "fourth value")));
        }

        [Test]
        public void ContainsKey() {
            Assert.IsTrue(_dictionary.ContainsKey(1));
            Assert.IsTrue(_dictionary.ContainsKey(2));
            Assert.IsTrue(_dictionary.ContainsKey(3));
            Assert.IsFalse(_dictionary.ContainsKey(4));
        }

        [Test]
        public void ContainsValue() {
            Assert.IsFalse(_dictionary.ContainsValue(null));
            Assert.IsTrue(_dictionary.ContainsValue("first value"));
            Assert.IsTrue(_dictionary.ContainsValue("second value"));
            Assert.IsTrue(_dictionary.ContainsValue("third value"));
            Assert.IsFalse(_dictionary.ContainsValue("fourth value"));
        }

        [Test]
        public void Copy() {
            Int32StringDictionary copy = _readOnly.Copy();
            Assert.IsFalse(copy.IsReadOnly);

            Assert.AreEqual(_dictionary.Count, copy.Count);
            foreach (Int32StringPair item in copy) {
                Assert.IsTrue(_dictionary.Contains(item));
                Assert.AreEqual(_dictionary[item.Key], item.Value);
                Assert.AreNotSame(_dictionary[item.Key], item.Value);
            }
        }

        [Test]
        public void CopyToEmpty() {
            _dictionary.Clear();
            Int32StringPair[] array = new Int32StringPair[0];
            _dictionary.CopyTo(array, 0);
        }

        [Test]
        public void CopyTo() {
            Int32StringPair[] array = new Int32StringPair[4];
            Assert.Throws<ArgumentException>(() => _dictionary.CopyTo(array, 3));

            _dictionary.CopyTo(array, 0);
            for (int i = 0; i < _dictionary.Count; i++)
                Assert.IsTrue(_dictionary.Contains(array[i]));

            _dictionary.CopyTo(array, 1);
            for (int i = 0; i < _dictionary.Count; i++)
                Assert.IsTrue(_dictionary.Contains(array[i + 1]));
        }

        [Test]
        public void Equals() {
            Int32StringDictionary dictionary = new Int32StringDictionary();
            dictionary.Add(1, "first value");
            dictionary.Add(2, "second value");
            dictionary.Add(3, "third value");
            Assert.IsTrue(_dictionary.Equals(dictionary));

            dictionary[2] = "foo value";
            Assert.IsFalse(_dictionary.Equals(dictionary));

            dictionary[2] = "second value";
            Assert.IsTrue(_dictionary.Equals(dictionary));

            dictionary[37] = "second value";
            Assert.IsFalse(_dictionary.Equals(dictionary));
        }

        [Test]
        public void GetAny() {
            Assert.IsTrue(_dictionary.Contains(_dictionary.GetAny()));
            Assert.IsTrue(_dictionary.ContainsKey(_dictionary.GetAnyKey()));
            Assert.IsTrue(_dictionary.ContainsValue(_dictionary.GetAnyValue()));

            Int32StringDictionary dictionary = new Int32StringDictionary();
            Assert.Throws<InvalidOperationException>(() => dictionary.GetAny());
            Assert.Throws<InvalidOperationException>(() => dictionary.GetAnyKey());
            Assert.Throws<InvalidOperationException>(() => dictionary.GetAnyValue());
        }

        [Test]
        public void GetEnumerator() {
            foreach (Int32StringPair entry in _dictionary)
                Assert.IsTrue(_dictionary.Contains(entry));
        }

        [Test]
        public void Remove() {
            Assert.Throws<NotSupportedException>(() => _readOnly.Remove(2));
            Assert.IsTrue(_dictionary.Remove(2));
            Assert.IsFalse(_dictionary.Remove(4));
            Assert.AreEqual(2, _dictionary.Count);
        }

        [Test]
        public void RemovePair() {
            Assert.Throws<NotSupportedException>(() =>
                _readOnly.Remove(new Int32StringPair(2, "second value")));

            Assert.IsTrue(_dictionary.Remove(new Int32StringPair(2, "second value")));
            Assert.IsFalse(_dictionary.Remove(new Int32StringPair(4, "fourth value")));
            Assert.AreEqual(2, _dictionary.Count);
        }

        [Test]
        public void ToArray() {
            Int32StringPair[] array = _dictionary.ToArray();
            Assert.AreEqual(_dictionary.Count, array.Length);
            for (int i = 0; i < _dictionary.Count; i++)
                Assert.IsTrue(_dictionary.Contains(array[i]));
        }

        [Test]
        public void TryGetValue() {
            CloneableType value;
            Assert.IsTrue(_dictionary.TryGetValue(1, out value));
            Assert.AreEqual(new CloneableType("first value"), value);
            Assert.IsFalse(_dictionary.TryGetValue(4, out value));
            Assert.AreEqual(null, value);
        }

        [Test]
        public void Stress() {
            const int count = 3000;
            var array = new KeyValuePair<Int32, CloneableType>[count];

            // multiples of prime number provoke hash collisions
            for (int i = 0; i < array.Length; i++)
                array[i] = new KeyValuePair<Int32, CloneableType>(i * 89,
                    new CloneableType(String.Format("bar{0:D3} value", i)));

            _dictionary.Clear();
            Assert.AreEqual(0, _dictionary.Count);

            // test adding elements
            foreach (var pair in array)
                _dictionary.Add(pair.Key, pair.Value);
            Assert.AreEqual(array.Length, _dictionary.Count);

            // test finding elements
            foreach (var pair in array) {
                CloneableType value;
                Assert.IsTrue(_dictionary.TryGetValue(pair.Key, out value));
                Assert.AreEqual(pair.Value, value);
            }

            var standard = new Dictionary<Int32, CloneableType>(count);
            foreach (var pair in array) standard.Add(pair.Key, pair.Value);

            // test element enumeration
            foreach (var pair in _dictionary) {
                CloneableType value;
                Assert.IsTrue(standard.TryGetValue(pair.Key, out value));
                Assert.AreEqual(value, pair.Value);
                standard.Remove(pair.Key);
            }
            Assert.AreEqual(0, standard.Count);

            // test removing elements
            for (int i = array.Length - 1; i >= 0; i--)
                Assert.IsTrue(_dictionary.Remove(array[i]));
            Assert.AreEqual(0, _dictionary.Count);
        }
    }
}
