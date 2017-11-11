using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    using Int32StringPair = KeyValuePair<Int32, CloneableType>;
    using Int32StringDictionary = SortedDictionaryEx<Int32, CloneableType>;

    using StringStringPair = KeyValuePair<String, CloneableType>;
    using StringStringDictionary = SortedDictionaryEx<String, CloneableType>;

    [TestFixture]
    public class SortedDictionaryExTest {

        StringStringDictionary _dictionary, _readOnly;

        [SetUp]
        public void SetUp() {
            _dictionary = new StringStringDictionary();
            _readOnly = _dictionary.AsReadOnly();
            Assert.AreEqual(0, _dictionary.Count);

            _dictionary.Add("one", "first value");
            _dictionary.Add("two", "second value");
            _dictionary.Add("three", "third value");
            Assert.AreEqual(3, _dictionary.Count);
        }

        [TearDown]
        public void TearDown() {
            _dictionary.Clear();
            _dictionary = null;
        }

        [Test]
        public void Constructor() {
            StringStringDictionary dictionary = new StringStringDictionary();
            dictionary.Add("foo", "foo value");
            dictionary.Add("bar", "bar value");
            Assert.AreEqual(2, dictionary.Count);

            StringStringDictionary clone = new StringStringDictionary(dictionary);
            Assert.AreEqual(2, clone.Count);
        }

        [Test]
        public void Count() {
            _dictionary.Add("foo", "foo value");
            _dictionary.Add("bar", "bar value");
            Assert.AreEqual(5, _dictionary.Count);

            _dictionary.Remove("bar");
            Assert.AreEqual(4, _dictionary.Count);
        }

        [Test]
        public void Empty() {
            Assert.AreSame(StringStringDictionary.Empty, StringStringDictionary.Empty);
            Assert.IsTrue(StringStringDictionary.Empty.IsReadOnly);
            Assert.AreEqual(StringStringDictionary.Empty.Count, 0);
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
            Assert.Throws<ArgumentNullException>(() => { var value = _dictionary[null]; });
            Assert.Throws<KeyNotFoundException>(() => { var value = _dictionary["bar"]; });
            Assert.Throws<NotSupportedException>(() => { _readOnly["two"] = "foo value"; });

            Assert.AreEqual(new CloneableType("second value"), _dictionary["two"]);
            _dictionary["two"] = "foo value";
            Assert.AreEqual(new CloneableType("foo value"), _dictionary["two"]);

            _dictionary["bar"] = "bar value";
            Assert.AreEqual(new CloneableType("bar value"), _dictionary["bar"]);
        }

        [Test]
        public void Keys() {
            Assert.AreEqual(_dictionary.Count, _dictionary.Keys.Count);
            foreach (string key in _dictionary.Keys)
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
            Assert.Throws<ArgumentNullException>(() => _dictionary.Add(null, "a null key"));
            Assert.Throws<ArgumentException>(() => _dictionary.Add("two", "another second value"));
            Assert.Throws<NotSupportedException>((() => _readOnly.Add("bar0", "bar value")));

            for (int i = 0; i < 20; i++) {
                string key = String.Format("bar{0}", i);
                _dictionary.Add(key, "bar value");
                Assert.IsTrue(_dictionary.Contains(new StringStringPair(key, "bar value")));
            }
            Assert.AreEqual(23, _dictionary.Count);
        }

        [Test]
        public void AddNull() {
            _dictionary.Add("null", null);
            Assert.IsTrue(_dictionary.ContainsValue(null));
        }

        [Test]
        public void AddPair() {
            Assert.Throws<ArgumentNullException>(() =>
                _dictionary.Add(new StringStringPair(null, "a null key")));
            Assert.Throws<ArgumentException>(() =>
                _dictionary.Add(new StringStringPair("two", "another second value")));
            Assert.Throws<NotSupportedException>(() =>
                _readOnly.Add(new StringStringPair("four", "fourth value")));

            _dictionary.Add(new StringStringPair("four", "fourth value"));
            Assert.AreEqual(4, _dictionary.Count);
            Assert.IsTrue(_dictionary.ContainsKey("four"));
            Assert.IsTrue(_dictionary.ContainsValue("fourth value"));
        }

        [Test]
        public void AddRange() {
            StringStringDictionary dictionary = new StringStringDictionary();
            dictionary.Add(new StringStringPair("four", "fourth value"));
            dictionary.Add(new StringStringPair("five", "fifth value"));

            Assert.Throws<NotSupportedException>(() => _readOnly.AddRange(dictionary));
            _dictionary.AddRange(dictionary);

            Assert.AreEqual(5, _dictionary.Count);
            Assert.IsTrue(_dictionary.ContainsKey("four"));
            Assert.IsTrue(_dictionary.ContainsValue("fourth value"));
            Assert.IsTrue(_dictionary.ContainsKey("five"));
            Assert.IsTrue(_dictionary.ContainsValue("fifth value"));

            Assert.Throws<ArgumentException>(() => _dictionary.AddRange(dictionary));
        }

        [Test]
        public void AsReadOnly() {
            Assert.IsTrue(_dictionary.Equals(_readOnly));

            _dictionary["two"] = "foo value";
            Assert.IsTrue(_dictionary.Equals(_readOnly));

            _dictionary["two"] = "second value";
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
            StringStringDictionary clone = (StringStringDictionary) _readOnly.Clone();
            Assert.IsFalse(clone.IsReadOnly);

            Assert.AreEqual(_dictionary.Count, clone.Count);
            foreach (StringStringPair item in clone) {
                Assert.IsTrue(_dictionary.Contains(item));
                Assert.AreEqual(_dictionary[item.Key], item.Value);
                Assert.AreSame(_dictionary[item.Key], item.Value);
            }
        }

        [Test]
        public void Contains() {
            Assert.IsTrue(_dictionary.Contains(new StringStringPair("two", "second value")));
            Assert.IsFalse(_dictionary.Contains(new StringStringPair("four", "fourth value")));
        }

        [Test]
        public void ContainsKey() {
            Assert.Throws<ArgumentNullException>(() => _dictionary.ContainsKey(null));
            Assert.IsTrue(_dictionary.ContainsKey("one"));
            Assert.IsTrue(_dictionary.ContainsKey("two"));
            Assert.IsTrue(_dictionary.ContainsKey("three"));
            Assert.IsFalse(_dictionary.ContainsKey("four"));
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
            StringStringDictionary copy = _readOnly.Copy();
            Assert.IsFalse(copy.IsReadOnly);

            Assert.AreEqual(_dictionary.Count, copy.Count);
            foreach (StringStringPair item in copy) {
                Assert.IsTrue(_dictionary.Contains(item));
                Assert.AreEqual(_dictionary[item.Key], item.Value);
                Assert.AreNotSame(_dictionary[item.Key], item.Value);
            }
        }

        [Test]
        public void CopyToEmpty() {
            _dictionary.Clear();
            StringStringPair[] array = new StringStringPair[0];
            _dictionary.CopyTo(array, 0);
        }

        [Test]
        public void CopyTo() {
            StringStringPair[] array = new StringStringPair[4];
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
            StringStringDictionary dictionary = new StringStringDictionary();
            dictionary.Add("one", "first value");
            dictionary.Add("two", "second value");
            dictionary.Add("three", "third value");
            Assert.IsTrue(_dictionary.Equals(dictionary));

            dictionary["two"] = "foo value";
            Assert.IsFalse(_dictionary.Equals(dictionary));

            dictionary["two"] = "second value";
            Assert.IsTrue(_dictionary.Equals(dictionary));

            dictionary["foo"] = "second value";
            Assert.IsFalse(_dictionary.Equals(dictionary));
        }

        [Test]
        public void GetEnumerator() {
            foreach (StringStringPair entry in _dictionary)
                Assert.IsTrue(_dictionary.Contains(entry));
        }

        [Test]
        public void Remove() {
            Assert.Throws<NotSupportedException>(() => _readOnly.Remove("two"));
            Assert.IsTrue(_dictionary.Remove("two"));
            Assert.IsFalse(_dictionary.Remove("four"));
            Assert.AreEqual(2, _dictionary.Count);
        }

        [Test]
        public void RemovePair() {
            Assert.Throws<NotSupportedException>(() =>
                _readOnly.Remove(new StringStringPair("two", "second value")));

            Assert.IsTrue(_dictionary.Remove(new StringStringPair("two", "second value")));
            Assert.IsFalse(_dictionary.Remove(new StringStringPair("four", "fourth value")));
            Assert.AreEqual(2, _dictionary.Count);
        }

        [Test]
        public void ToArray() {
            StringStringPair[] array = _dictionary.ToArray();
            Assert.AreEqual(_dictionary.Count, array.Length);
            for (int i = 0; i < _dictionary.Count; i++)
                Assert.IsTrue(_dictionary.Contains(array[i]));
        }

        [Test]
        public void TryGetValue() {
            CloneableType value;
            Assert.IsTrue(_dictionary.TryGetValue("one", out value));
            Assert.AreEqual(new CloneableType("first value"), value);
            Assert.IsFalse(_dictionary.TryGetValue("four", out value));
            Assert.AreEqual(null, value);
        }
    }
}
