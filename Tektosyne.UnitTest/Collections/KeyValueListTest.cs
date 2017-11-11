using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    using Int32StringPair = KeyValuePair<Int32, CloneableType>;
    using Int32StringList = KeyValueList<Int32, CloneableType>;

    using StringStringPair = KeyValuePair<String, CloneableType>;
    using StringStringList = KeyValueList<String, CloneableType>;

    [TestFixture]
    public class KeyValueListTest {

        StringStringList _list, _readOnly;

        [SetUp]
        public void SetUp() {
            _list = new StringStringList();
            _readOnly = _list.AsReadOnly();
            Assert.AreEqual(0, _list.Capacity);
            Assert.AreEqual(0, _list.Count);

            _list.Add("one", "first value");
            _list.Add("two", "second value");
            _list.Add("three", "third value");
            Assert.AreEqual(3, _list.Count);
        }

        [TearDown]
        public void TearDown() {
            _list.Clear();
            _list = null;
        }

        [Test]
        public void Constructor() {
            StringStringList list = new StringStringList(567);
            list.Add("foo", "foo value");
            list.Add("bar", "bar value");
            Assert.AreEqual(567, list.Capacity);
            Assert.AreEqual(2, list.Count);

            StringStringPair[] array = new StringStringPair[4] { 
                new StringStringPair("a", "a value"),
                new StringStringPair("b", "b value"),
                new StringStringPair("c", "c value"),
                new StringStringPair("d", "d value")
            };

            list = new StringStringList(array);
            Assert.AreEqual(4, list.Capacity);
            Assert.AreEqual(4, list.Count);
        }

        [Test]
        public void Empty() {
            Assert.AreSame(StringStringList.Empty, StringStringList.Empty);
            Assert.IsTrue(StringStringList.Empty.IsReadOnly);
            Assert.AreEqual(StringStringList.Empty.Count, 0);
        }

        [Test]
        public void ItemKey() {
            Assert.Throws<ArgumentNullException>(() => { var value = _list[null]; });
            Assert.Throws<KeyNotFoundException>(() => { var value = _list["bar"]; });
            Assert.Throws<NotSupportedException>(() => { _readOnly["two"] = "foo value"; });

            Assert.AreEqual(new CloneableType("second value"), _list["two"]);
            _list["two"] = "foo value";
            Assert.AreEqual(new CloneableType("foo value"), _list["two"]);

            _list["bar"] = "bar value";
            Assert.AreEqual(new CloneableType("bar value"), _list["bar"]);
        }

        [Test]
        public void ItemKeyInt32() {
            Int32StringList list = new Int32StringList();
            list.Add(2, "two");
            Assert.AreEqual(new Int32StringPair(2, "two"), ((ListEx<Int32StringPair>) list)[0]);
            Assert.AreEqual(new CloneableType("two"), list[2]);
            Assert.AreEqual(new CloneableType("two"), list.GetByKey(2));
        }

        [Test]
        public void Keys() {
            Assert.AreEqual(_list.Count, _list.Keys.Count);

            int index = 0;
            foreach (string item in _list.Keys)
                Assert.AreEqual(_list[index++].Key, item);
        }

        [Test]
        public void Values() {
            Assert.AreEqual(_list.Count, _list.Values.Count);

            int index = 0;
            foreach (CloneableType item in _list.Values)
                Assert.AreEqual(_list[index++].Value, item);
        }

        [Test]
        public void Add() {
            Assert.Throws<ArgumentNullException>(() => _list.Add(null, "a null key"));
            Assert.Throws<NotSupportedException>((() => _readOnly.Add("bar", "bar value")));

            for (int i = 0; i < 20; i++) {
                _list.Add("bar", "bar value");
                Assert.AreEqual(new StringStringPair("bar", "bar value"), _list[3 + i]);
            }
            Assert.AreEqual(23, _list.Count);
            Assert.AreEqual(32, _list.Capacity);
        }

        [Test]
        public void AddNull() {
            _list.Add("null", null);
            Assert.IsTrue(_list.ContainsValue(null));
        }

        [Test]
        public void AsReadOnly() {
            Assert.IsTrue(_list.Equals(_readOnly));

            _list[2] = new StringStringPair("two", "foo value");
            Assert.IsTrue(_list.Equals(_readOnly));

            _list[2] = new StringStringPair("two", "second value");
            Assert.IsTrue(_list.Equals(_readOnly));
        }

        [Test]
        public void Clone() {
            StringStringList clone = (StringStringList) _list.Clone();
            Assert.AreEqual(_list.Count, clone.Count);

            for (int i = 0; i < clone.Count; i++) {
                Assert.AreEqual(_list[i], clone[i]);
                Assert.AreSame(_list[i].Value, clone[i].Value);
            }
        }

        [Test]
        public void ContainsKey() {
            Assert.Throws<ArgumentNullException>(() => _list.ContainsKey(null));
            Assert.IsTrue(_list.ContainsKey("one"));
            Assert.IsTrue(_list.ContainsKey("two"));
            Assert.IsTrue(_list.ContainsKey("three"));
            Assert.IsFalse(_list.ContainsKey("four"));
        }

        [Test]
        public void ContainsValue() {
            Assert.IsFalse(_list.ContainsValue(null));
            Assert.IsTrue(_list.ContainsValue("first value"));
            Assert.IsTrue(_list.ContainsValue("second value"));
            Assert.IsTrue(_list.ContainsValue("third value"));
            Assert.IsFalse(_list.ContainsValue("fourth value"));
        }

        [Test]
        public void Copy() {
            StringStringList copy = _list.Copy();
            Assert.AreEqual(_list.Count, copy.Count);

            for (int i = 0; i < copy.Count; i++) {
                Assert.AreEqual(_list[i], copy[i]);
                Assert.AreNotSame(_list[i].Value, copy[i].Value);
            }
        }

        [Test]
        public void GetByIndex() {
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.GetByIndex(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.GetByIndex(3));

            Assert.AreEqual(new CloneableType("first value"), _list.GetByIndex(0));
            Assert.AreEqual(new CloneableType("second value"), _list.GetByIndex(1));
            Assert.AreEqual(new CloneableType("third value"), _list.GetByIndex(2));
        }

        [Test]
        public void GetByKey() {
            Assert.Throws<ArgumentNullException>(() => _list.GetByKey(null));
            Assert.Throws<KeyNotFoundException>(() => _list.GetByKey("four"));

            Assert.AreEqual(new CloneableType("first value"), _list.GetByKey("one"));
            Assert.AreEqual(new CloneableType("second value"), _list.GetByKey("two"));
            Assert.AreEqual(new CloneableType("third value"), _list.GetByKey("three"));
        }

        [Test]
        public void GetKey() {
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.GetKey(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.GetKey(3));

            Assert.AreEqual("one", _list.GetKey(0));
            Assert.AreEqual("two", _list.GetKey(1));
            Assert.AreEqual("three", _list.GetKey(2));
        }

        [Test]
        public void IndexOfKey() {
            Assert.AreEqual(0, _list.IndexOfKey("one"));
            Assert.AreEqual(1, _list.IndexOfKey("two"));
            Assert.AreEqual(2, _list.IndexOfKey("three"));
            Assert.AreEqual(-1, _list.IndexOfKey("four"));
        }

        [Test]
        public void IndexOfValue() {
            Assert.AreEqual(0, _list.IndexOfValue("first value"));
            Assert.AreEqual(1, _list.IndexOfValue("second value"));
            Assert.AreEqual(2, _list.IndexOfValue("third value"));
            Assert.AreEqual(-1, _list.IndexOfValue("fourth value"));
        }

        [Test]
        public void Remove() {
            Assert.Throws<NotSupportedException>(() => _readOnly.Remove("two"));
            Assert.IsTrue(_list.Remove("two"));
            Assert.IsFalse(_list.Remove("four"));
            Assert.AreEqual(2, _list.Count);
        }

        [Test]
        public void SetByIndex() {
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.SetByIndex(-1, "new fourth value"));
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.SetByIndex(3, "new fourth value"));
            Assert.Throws<NotSupportedException>(() => _readOnly.SetByIndex(0, "new first value"));

            _list.SetByIndex(0, "new first value");
            Assert.AreEqual(new StringStringPair("one", "new first value"), _list[0]);
        }

        [Test]
        public void SetByKey() {
            Assert.Throws<ArgumentNullException>(() => _list.SetByKey(null, "new null value"));
            Assert.Throws<NotSupportedException>(() => _readOnly.SetByKey("one", "new first value"));

            Assert.AreEqual(0, _list.SetByKey("one", "new first value"));
            Assert.AreEqual(new StringStringPair("one", "new first value"), _list[0]);

            Assert.AreEqual(3, _list.SetByKey("four", "new fourth value"));
            Assert.AreEqual(new StringStringPair("four", "new fourth value"), _list[3]);
        }

        [Test]
        public void TryGetValue() {
            CloneableType value;
            Assert.IsTrue(_list.TryGetValue("one", out value));
            Assert.AreEqual(new CloneableType("first value"), value);
            Assert.IsFalse(_list.TryGetValue("four", out value));
            Assert.AreEqual(null, value);
        }
    }
}
