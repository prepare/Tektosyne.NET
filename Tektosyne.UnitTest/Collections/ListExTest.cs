using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    using StringList = ListEx<CloneableType>;

    [TestFixture]
    public class ListExTest {

        StringList _list, _readOnly, _unique;

        [SetUp]
        public void SetUp() {
            _list = new StringList();
            _readOnly = _list.AsReadOnly();
            Assert.AreEqual(0, _list.Capacity);
            Assert.AreEqual(0, _list.Count);

            _list.Add("one");
            _list.Add("two");
            _list.Add("three");
            Assert.AreEqual(3, _list.Count);

            _unique = new StringList(true);
            _unique.AddRange(_list);
            Assert.AreEqual(3, _list.Count);
        }

        [TearDown]
        public void TearDown() {
            _list.Clear();
            _list = null;
        }

        [Test]
        public void Constructor() {
            StringList list = new StringList(567);
            list.Add("foo");
            list.Add("bar");
            Assert.AreEqual(567, list.Capacity);
            Assert.AreEqual(2, list.Count);

            list = new StringList(new CloneableType[4] { "a", "b", "c", "d" });
            Assert.AreEqual(4, list.Capacity);
            Assert.AreEqual(4, list.Count);
        }

        [Test]
        public void Capacity() {
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.Capacity = 0);
            Assert.Throws<NotSupportedException>(() => _readOnly.Capacity = _list.Capacity);

            _list.Capacity = 238;
            Assert.AreEqual(238, _list.Capacity);

            _list.Clear();
            _list.Capacity = 0;
            Assert.AreEqual(0, _list.Capacity);
        }

        [Test]
        public void Count() {
            _list.Add("foo");
            _list.Add("bar");
            Assert.AreEqual(5, _list.Count);

            _list.RemoveAt(4);
            Assert.AreEqual(4, _list.Count);
        }

        [Test]
        public void Empty() {
            Assert.AreSame(StringList.Empty, StringList.Empty);
            Assert.IsTrue(StringList.Empty.IsReadOnly);
            Assert.AreEqual(StringList.Empty.Count, 0);
        }

        [Test]
        public void IsFixedSize() {
            Assert.IsFalse(_list.IsFixedSize);
            Assert.IsTrue(_readOnly.IsFixedSize);
            Assert.IsFalse(_unique.IsFixedSize);
        }

        [Test]
        public void IsReadOnly() {
            Assert.IsFalse(_list.IsReadOnly);
            Assert.IsTrue(_readOnly.IsReadOnly);
            Assert.IsFalse(_unique.IsReadOnly);
        }

        [Test]
        public void IsUnique() {
            Assert.IsFalse(_list.IsUnique);
            Assert.IsFalse(_readOnly.IsUnique);
            Assert.IsTrue(_unique.IsUnique);
        }

        [Test]
        public void Item() {
            Assert.Throws<ArgumentOutOfRangeException>(() => { object item = _list[-1]; });
            Assert.Throws<ArgumentOutOfRangeException>(() => { object item = _list[4]; });
            Assert.Throws<NotSupportedException>(() => _readOnly[1] = "four");
            Assert.Throws<NotSupportedException>(() => _unique[1] = "three");

            Assert.AreEqual(new CloneableType("two"), _list[1]);
            _list[1] = "foo";
            Assert.AreEqual(new CloneableType("foo"), _list[1]);
        }

        [Test]
        public void Add() {
            Assert.Throws<NotSupportedException>(() => _readOnly.Add("four"));
            Assert.Throws<NotSupportedException>(() => _unique.Add("three"));

            for (int i = 0; i < 20; i++) {
                _list.Add("bar");
                Assert.AreEqual(new CloneableType("bar"), _list[3 + i]);
            }
            Assert.AreEqual(23, _list.Count);
            Assert.AreEqual(32, _list.Capacity);
        }

        [Test]
        public void AddNull() {
            _list.Add(null);
            Assert.IsTrue(_list.Contains(null));
        }

        [Test]
        public void AddRange() {
            var array = new CloneableType[3] { "a", "b", "c" };
            Assert.Throws<NotSupportedException>(() => _readOnly.AddRange(_list));
            Assert.Throws<NotSupportedException>(() => _unique.AddRange(_list));

            _list.AddRange(array);
            Assert.AreEqual(new CloneableType("a"), _list[3]);
            Assert.AreEqual(new CloneableType("b"), _list[4]);
            Assert.AreEqual(new CloneableType("c"), _list[5]);
        }

        [Test]
        public void AsReadOnly() {
            Assert.IsTrue(_list.Equals(_readOnly));

            _list[2] = "foo value";
            Assert.IsTrue(_list.Equals(_readOnly));

            _list[2] = "second value";
            Assert.IsTrue(_list.Equals(_readOnly));
        }

        [Test]
        public void Clear() {
            Assert.Throws<NotSupportedException>(_readOnly.Clear);

            _list.Clear();
            Assert.AreEqual(4, _list.Capacity);
            Assert.AreEqual(0, _list.Count);
        }

        [Test]
        public void Clone() {
            StringList clone = (StringList) _readOnly.Clone();
            Assert.AreEqual(false, clone.IsReadOnly);
            Assert.AreEqual(_list.Count, clone.Count);

            for (int i = 0; i < clone.Count; i++) {
                Assert.AreEqual(_list[i], clone[i]);
                Assert.AreSame(_list[i], clone[i]);
            }
        }

        [Test]
        public void CloneUnique() {
            StringList clone = (StringList) _unique.Clone();
            Assert.IsTrue(clone.IsUnique);
            Assert.AreEqual(_list.Count, clone.Count);

            for (int i = 0; i < clone.Count; i++) {
                Assert.AreEqual(_list[i], clone[i]);
                Assert.AreSame(_list[i], clone[i]);
            }
        }

        [Test]
        public void Contains() {
            Assert.IsTrue(_list.Contains(new CloneableType("two")));
            Assert.IsFalse(_list.Contains(new CloneableType("four")));
        }

        [Test]
        public void Copy() {
            StringList copy = _readOnly.Copy();
            Assert.AreEqual(false, copy.IsReadOnly);
            Assert.AreEqual(_list.Count, copy.Count);

            for (int i = 0; i < copy.Count; i++) {
                Assert.AreEqual(_list[i], copy[i]);
                Assert.AreNotSame(_list[i], copy[i]);
            }
        }

        [Test]
        public void CopyUnique() {
            StringList copy = _unique.Copy();
            Assert.IsTrue(copy.IsUnique);
            Assert.AreEqual(_list.Count, copy.Count);

            for (int i = 0; i < copy.Count; i++) {
                Assert.AreEqual(_list[i], copy[i]);
                Assert.AreNotSame(_list[i], copy[i]);
            }
        }

        [Test]
        public void CopyToEmpty() {
            _list.Clear();
            CloneableType[] array = new CloneableType[0];
            _list.CopyTo(array, 0);
        }

        [Test]
        public void CopyTo() {
            CloneableType[] array = new CloneableType[4];
            Assert.Throws<ArgumentException>(() => _list.CopyTo(array, 3));

            _list.CopyTo(array, 0);
            for (int i = 0; i < _list.Count; i++)
                Assert.AreEqual(_list[i], array[i]);

            _list.CopyTo(array, 1);
            for (int i = 0; i < _list.Count; i++)
                Assert.AreEqual(_list[i], array[i + 1]);
        }

        [Test]
        public void Equals() {
            Assert.IsTrue(_list.Equals(_readOnly));

            StringList list = new StringList();
            list.Add("one");
            list.Add("two");
            list.Add("three");
            Assert.IsTrue(_list.Equals(list));
            Assert.IsTrue(_list.Equals(_readOnly));

            list[1] = "foo";
            Assert.IsFalse(_list.Equals(list));
            Assert.IsTrue(_list.Equals(_readOnly));

            list[1] = "two";
            Assert.IsTrue(_list.Equals(list));
            Assert.IsTrue(_list.Equals(_readOnly));
        }

        [Test]
        public void GetEnumerator() {
            int i = 0;
            foreach (CloneableType entry in _list)
                Assert.AreEqual(_list[i++], entry);
        }

        [Test]
        public void GetRange() {
            Assert.Throws<ArgumentException>(() => _list.GetRange(1, 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.GetRange(-1, 4));

            List<CloneableType> range = _list.GetRange(1, 2);
            Assert.AreEqual(2, range.Count);
            Assert.AreEqual(range[0], _list[1]);
            Assert.AreEqual(range[1], _list[2]);
        }

        [Test]
        public void IndexOf() {
            Assert.AreEqual(0, _list.IndexOf("one"));
            Assert.AreEqual(1, _list.IndexOf("two"));
            Assert.AreEqual(2, _list.IndexOf("three"));
            Assert.AreEqual(-1, _list.IndexOf("four"));
        }

        [Test]
        public void Insert() {
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.Insert(4, "five"));
            Assert.Throws<NotSupportedException>(() => _readOnly.Insert(2, "four"));
            Assert.Throws<NotSupportedException>(() => _unique.Insert(1, "two"));

            _list.Insert(2, "four");
            Assert.AreEqual(4, _list.Count);
            Assert.AreEqual(new CloneableType("four"), _list[2]);
            Assert.AreEqual(new CloneableType("three"), _list[3]);

            _list.Insert(4, "five");
            Assert.AreEqual(5, _list.Count);
            Assert.AreEqual(new CloneableType("five"), _list[4]);
        }

        [Test]
        public void InsertRange() {
            var array = new CloneableType[3] { "a", "b", "c" };
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.InsertRange(4, array));
            Assert.Throws<NotSupportedException>(() => _readOnly.InsertRange(2, array));
            Assert.Throws<NotSupportedException>(() => _unique.InsertRange(1, _list));

            _list.InsertRange(1, array);
            Assert.AreEqual(new CloneableType("a"), _list[1]);
            Assert.AreEqual(new CloneableType("b"), _list[2]);
            Assert.AreEqual(new CloneableType("c"), _list[3]);
        }

        [Test]
        public void Remove() {
            Assert.Throws<NotSupportedException>(() => _readOnly.Remove("two"));

            Assert.IsTrue(_list.Remove("two"));
            Assert.IsFalse(_list.Remove("four"));
            Assert.AreEqual(2, _list.Count);
            Assert.AreEqual(new CloneableType("three"), _list[1]);
        }

        [Test]
        public void RemoveAt() {
            Assert.Throws<ArgumentOutOfRangeException>(() => _list.RemoveAt(3));
            Assert.Throws<NotSupportedException>(() => _readOnly.RemoveAt(0));

            _list.RemoveAt(0);
            Assert.AreEqual(2, _list.Count);
            Assert.AreEqual(new CloneableType("two"), _list[0]);
            Assert.AreEqual(new CloneableType("three"), _list[1]);
        }

        [Test]
        public void RemoveRange() {
            Assert.Throws<ArgumentException>(() => _list.RemoveRange(3, 1));
            Assert.Throws<NotSupportedException>(() => _readOnly.RemoveRange(1, 2));

            _list.RemoveRange(0, 2);
            Assert.AreEqual(1, _list.Count);
            Assert.AreEqual(new CloneableType("three"), _list[0]);
        }

        [Test]
        public void ReverseFailed() {
            Assert.Throws<NotSupportedException>(_readOnly.Reverse);
            Assert.Throws<NotSupportedException>(() => _readOnly.Reverse(1, 2));
        }

        [Test]
        public void SortFailed() {
            Assert.Throws<NotSupportedException>(_readOnly.Sort);
            Assert.Throws<NotSupportedException>(() => _readOnly.Sort((x, y)  => 0));
            Assert.Throws<NotSupportedException>(() => _readOnly.Sort(Comparer<CloneableType>.Default));
            Assert.Throws<NotSupportedException>(() => _readOnly.Sort(0, 2, null));
        }

        [Test]
        public void ToArray() {
            CloneableType[] array = _list.ToArray();
            Assert.AreEqual(_list.Count, array.Length);
            for (int i = 0; i < _list.Count; i++)
                Assert.AreEqual(_list[i], array[i]);
        }

        [Test]
        public void TrimExcess() {
            Assert.Throws<NotSupportedException>(_readOnly.TrimExcess);
            _list.TrimExcess();
            Assert.AreEqual(4, _list.Capacity);
        }
    }
}
