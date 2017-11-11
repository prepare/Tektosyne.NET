using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    using Int32Keyed = KeyedType<Int32>;
    using Int32StringList = KeyedList<Int32, KeyedType<Int32>>;

    using StringKeyed = KeyedType<String>;
    using StringStringList = KeyedList<String, KeyedType<String>>;

    [TestFixture]
    public class KeyedListTest {

        StringStringList _list, _readOnly;

        [SetUp]
        public void SetUp() {
            _list = new StringStringList();
            _readOnly = _list.AsReadOnly();
            Assert.AreEqual(0, _list.Capacity);
            Assert.AreEqual(0, _list.Count);

            _list.Add(new StringKeyed("one", "first value"));
            _list.Add(new StringKeyed("two", "second value"));
            _list.Add(new StringKeyed("three", "third value"));
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
            list.Add(new StringKeyed("foo", "foo value"));
            list.Add(new StringKeyed("bar", "bar value"));
            Assert.AreEqual(567, list.Capacity);
            Assert.AreEqual(2, list.Count);

            StringKeyed[] array = new StringKeyed[4] { 
                new StringKeyed("a", "a value"),
                new StringKeyed("b", "b value"),
                new StringKeyed("c", "c value"),
                new StringKeyed("d", "d value")
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

            Assert.AreEqual(new StringKeyed("one", "first value"), _list["one"]);
            Assert.AreEqual(new StringKeyed("two", "second value"), _list["two"]);
            Assert.AreEqual(new StringKeyed("three", "third value"), _list["three"]);
        }

        [Test]
        public void ItemKeyInt32() {
            Int32StringList list = new Int32StringList();
            list.Add(new Int32Keyed(2, "two"));
            Assert.AreEqual(new Int32Keyed(2, "two"), ((ListEx<Int32Keyed>) list)[0]);
            Assert.AreEqual(new Int32Keyed(2, "two"), list[2]);
            Assert.AreEqual(new Int32Keyed(2, "two"), list.GetByKey(2));
        }

        [Test]
        public void AsReadOnly() {
            Assert.IsTrue(_list.Equals(_readOnly));

            _list[2] = new StringKeyed("two", "foo value");
            Assert.IsTrue(_list.Equals(_readOnly));

            _list[2] = new StringKeyed("two", "second value");
            Assert.IsTrue(_list.Equals(_readOnly));
        }

        [Test]
        public void Clone() {
            StringStringList clone = (StringStringList) _list.Clone();
            Assert.AreEqual(_list.Count, clone.Count);

            for (int i = 0; i < clone.Count; i++) {
                Assert.AreEqual(_list[i], clone[i]);
                Assert.AreSame(_list[i], clone[i]);
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
        public void Copy() {
            StringStringList copy = _list.Copy();
            Assert.AreEqual(_list.Count, copy.Count);

            for (int i = 0; i < copy.Count; i++) {
                Assert.AreEqual(_list[i], copy[i]);
                Assert.AreNotSame(_list[i], copy[i]);
            }
        }

        [Test]
        public void GetByIndex() {
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.GetByIndex(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.GetByIndex(3));

            Assert.AreEqual(new StringKeyed("one", "first value"), _list.GetByIndex(0));
            Assert.AreEqual(new StringKeyed("two", "second value"), _list.GetByIndex(1));
            Assert.AreEqual(new StringKeyed("three", "third value"), _list.GetByIndex(2));
        }

        [Test]
        public void GetByKey() {
            Assert.Throws<ArgumentNullException>(() => _list.GetByKey(null));
            Assert.Throws<KeyNotFoundException>(() => _list.GetByKey("four"));

            Assert.AreEqual(new StringKeyed("one", "first value"), _list.GetByKey("one"));
            Assert.AreEqual(new StringKeyed("two", "second value"), _list.GetByKey("two"));
            Assert.AreEqual(new StringKeyed("three", "third value"), _list.GetByKey("three"));
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
        public void SetByIndex() {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _list.SetByIndex(-1, new StringKeyed("four", "new fourth value")));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _list.SetByIndex(3, new StringKeyed("four", "new fourth value")));
            Assert.Throws<NotSupportedException>(() =>
                _readOnly.SetByIndex(0, new StringKeyed("one", "new first value")));

            _list.SetByIndex(0, new StringKeyed("one", "new first value"));
            Assert.AreEqual(new StringKeyed("one", "new first value"), _list[0]);
        }

        [Test]
        public void TryGetValue() {
            StringKeyed value;
            Assert.IsTrue(_list.TryGetValue("one", out value));
            Assert.AreEqual(new StringKeyed("one", "first value"), value);
            Assert.IsFalse(_list.TryGetValue("four", out value));
            Assert.AreEqual(null, value);
        }
    }
}
