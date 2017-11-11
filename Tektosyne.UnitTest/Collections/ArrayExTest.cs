using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    using StringArray = ArrayEx<CloneableType>;

    [TestFixture]
    public class ArrayExTest {

        StringArray _array, _readOnly;

        [SetUp]
        public void SetUp() {
            _array = new StringArray(2, 3);
            _readOnly = _array.AsReadOnly();

            Assert.AreEqual(6, _array.Length);
            Assert.AreEqual(2, _array.Rank);
            Assert.AreEqual(2, _array.GetLength(0));
            Assert.AreEqual(3, _array.GetLength(1));

            _array[0, 0] = "one"; _array[0, 1] = "two"; _array[0, 2] = "three";
            _array[1, 0] = "four"; _array[1, 1] = "five"; _array[1, 2] = "six";
        }

        [TearDown]
        public void TearDown() {
            _array = null;
        }

        [Test]
        public void Constructor() {
            var source = new CloneableType[4, 2]
                { { "a", "b" }, { "c", "d" }, { "e", "f" }, { "g", "h" } };
            StringArray array = new StringArray(source, true);

            Assert.AreEqual(8, array.Length);
            Assert.AreEqual(2, array.Rank);
            Assert.AreEqual(4, array.GetLength(0));
            Assert.AreEqual(2, array.GetLength(1));

            // check equality by multidimensional index
            for (int i = 0; i < array.Length; i++) {
                int[] indices = array.GetIndices(i);
                var value = source.GetValue(indices);
                Assert.AreEqual(value, array[i]);
                Assert.AreSame(value, array[i]);
            }

            // check equality by enumeration sequence
            int index = 0;
            foreach (CloneableType value in source) {
                Assert.AreEqual(value, array[index]);
                Assert.AreSame(value, array[index]);
                index++;
            }
        }

        [Test]
        public void Empty() {
            Assert.AreSame(StringArray.Empty, StringArray.Empty);
            Assert.IsTrue(StringArray.Empty.IsReadOnly);
            Assert.AreEqual(StringArray.Empty.Length, 0);
        }

        [Test]
        public void IsFixedSize() {
            Assert.IsTrue(_array.IsFixedSize);
        }

        [Test]
        public void IsReadOnly() {
            Assert.IsFalse(_array.IsReadOnly);
            Assert.IsTrue(_readOnly.IsReadOnly);
        }

        [Test]
        public void Item() {
            Assert.Throws<ArgumentOutOfRangeException>(() => { object item = _array[0, -1]; });
            Assert.Throws<ArgumentOutOfRangeException>(() => { object item = _array[4, 0]; });
            Assert.Throws<NotSupportedException>(() => _readOnly[0, 1] = "foo");

            Assert.AreEqual(new CloneableType("two"), _array[0, 1]);
            _array[0, 1] = "foo";
            Assert.AreEqual(new CloneableType("foo"), _array[0, 1]);
        }

        [Test]
        public void AsReadOnly() {
            Assert.AreEqual(_array.Rank, _readOnly.Rank);
            Assert.IsTrue(_array.Equals(_readOnly));

            _array[0, 1] = "foo value";
            Assert.IsTrue(_array.Equals(_readOnly));

            _array[0, 1] = "second value";
            Assert.IsTrue(_array.Equals(_readOnly));
        }

        [Test]
        public void Clear() {
            Assert.Throws<NotSupportedException>(() => _readOnly.Clear());

            _array.Clear();
            Assert.AreEqual(6, _array.Length);
            foreach (var item in _array)
                Assert.AreEqual(null, item);
        }

        [Test]
        public void Clone() {
            StringArray clone = (StringArray) _readOnly.Clone();
            Assert.AreEqual(false, clone.IsReadOnly);
            Assert.AreEqual(_array.Length, clone.Length);

            for (int i = 0; i < clone.Length; i++) {
                Assert.AreEqual(_array[i], clone[i]);
                Assert.AreSame(_array[i], clone[i]);
            }
        }

        [Test]
        public void Contains() {
            Assert.IsTrue(_array.Contains(new CloneableType("two")));
            Assert.IsFalse(_array.Contains(new CloneableType("seven")));
        }

        [Test]
        public void Copy() {
            StringArray copy = _readOnly.Copy();
            Assert.AreEqual(false, copy.IsReadOnly);
            Assert.AreEqual(_array.Length, copy.Length);

            for (int i = 0; i < copy.Length; i++) {
                Assert.AreEqual(_array[i], copy[i]);
                Assert.AreNotSame(_array[i], copy[i]);
            }
        }

        [Test]
        public void CopyFrom() {
            var source = new CloneableType[] { "a", "b", "c", "d" };
            Assert.Throws<NotSupportedException>(() => _readOnly.CopyFrom(source, 1));
            Assert.Throws<ArgumentException>(() => _array.CopyFrom(source, 3));

            _array.CopyFrom(source, 1);
            Assert.AreEqual(new CloneableType("one"), _array[0]);
            Assert.AreEqual(new CloneableType("a"), _array[1]);
            Assert.AreEqual(new CloneableType("b"), _array[2]);
            Assert.AreEqual(new CloneableType("c"), _array[3]);
            Assert.AreEqual(new CloneableType("d"), _array[4]);
            Assert.AreEqual(new CloneableType("six"), _array[5]);
        }

        [Test]
        public void CopyTo() {
            CloneableType[] array = new CloneableType[7];
            Assert.Throws<ArgumentException>(() => _array.CopyTo(array, 3));

            _array.CopyTo(array, 0);
            for (int i = 0; i < _array.Length; i++)
                Assert.AreEqual(_array[i], array[i]);

            _array.CopyTo(array, 1);
            for (int i = 0; i < _array.Length; i++)
                Assert.AreEqual(_array[i], array[i + 1]);
        }

        [Test]
        public void Equals() {
            StringArray array = new StringArray(2, 3);
            array[0, 0] = "one"; array[0, 1] = "two"; array[0, 2] = "three";
            array[1, 0] = "four"; array[1, 1] = "five"; array[1, 2] = "six";
            Assert.IsTrue(_array.Equals(array));

            array[0, 1] = "foo";
            Assert.IsFalse(_array.Equals(array));

            array[0, 1] = "two";
            Assert.IsTrue(_array.Equals(array));
        }

        [Test]
        public void EqualsArray() {
            CloneableType[,] array = new CloneableType[2, 3]
                { { "one", "two", "three" }, { "four", "five", "six" } };
            CompareToArray(array, false);
        }

        [Test]
        public void EqualsReadOnly() {
            Assert.IsTrue(_array.Equals(_readOnly));

            _array[0, 1] = "foo";
            Assert.IsTrue(_array.Equals(_readOnly));
        }

        [Test]
        public void GetEnumerator() {
            int i = 0;
            foreach (CloneableType entry in _array)
                Assert.AreEqual(_array[i++], entry);
        }

        [Test]
        public void GetIndex() {
            Assert.AreEqual(0, _array.GetIndex(0, 0));
            Assert.AreEqual(1, _array.GetIndex(0, 1));
            Assert.AreEqual(3, _array.GetIndex(1, 0));
            Assert.AreEqual(5, _array.GetIndex(1, 2));
        }

        [Test]
        public void GetIndex3D() {
            StringArray array = new StringArray(2, 3, 4);
            Assert.AreEqual(0, array.GetIndex(0, 0, 0));
            Assert.AreEqual(1, array.GetIndex(0, 0, 1));
            Assert.AreEqual(4, array.GetIndex(0, 1, 0));
            Assert.AreEqual(8, array.GetIndex(0, 2, 0));
            Assert.AreEqual(12, array.GetIndex(1, 0, 0));
            Assert.AreEqual(23, array.GetIndex(1, 2, 3));
        }

        public void GetIndexArray() {
            StringArray array = new StringArray(2, 3, 4);
            Assert.AreEqual(0, array.GetIndex(new int[] { 0, 0, 0 }));
            Assert.AreEqual(1, array.GetIndex(new int[] { 0, 0, 1 }));
            Assert.AreEqual(4, array.GetIndex(new int[] { 0, 1, 0 }));
            Assert.AreEqual(8, array.GetIndex(new int[] { 0, 2, 0 }));
            Assert.AreEqual(12, array.GetIndex(new int[] { 1, 0, 0 }));
            Assert.AreEqual(23, array.GetIndex(new int[] { 1, 2, 3 }));
        }

        [Test]
        public void GetIndices() {
            Assert.AreEqual(new int[] { 0, 0 }, _array.GetIndices(0));
            Assert.AreEqual(new int[] { 0, 1 }, _array.GetIndices(1));
            Assert.AreEqual(new int[] { 1, 0 }, _array.GetIndices(3));
            Assert.AreEqual(new int[] { 1, 2 }, _array.GetIndices(5));
        }

        [Test]
        public void GetIndices3D() {
            StringArray array = new StringArray(2, 3, 4);
            Assert.AreEqual(new int[] { 0, 0, 0 }, array.GetIndices(0));
            Assert.AreEqual(new int[] { 0, 0, 1 }, array.GetIndices(1));
            Assert.AreEqual(new int[] { 0, 1, 0 }, array.GetIndices(4));
            Assert.AreEqual(new int[] { 0, 2, 0 }, array.GetIndices(8));
            Assert.AreEqual(new int[] { 1, 0, 0 }, array.GetIndices(12));
            Assert.AreEqual(new int[] { 1, 2, 3 }, array.GetIndices(23));
        }

        [Test]
        public void GetValueOrDefault() {
            Assert.AreEqual(new CloneableType("five"), _array.GetValueOrDefault(4));
            Assert.AreEqual(new CloneableType("two"), _array.GetValueOrDefault(0, 1));
            Assert.AreEqual(new CloneableType("four"), _array.GetValueOrDefault(1, 0));

            Assert.AreEqual(null, _array.GetValueOrDefault(-1));
            Assert.AreEqual(null, _array.GetValueOrDefault(6));
            Assert.AreEqual(null, _array.GetValueOrDefault(3, 0));

            StringArray array = new StringArray(2, 3, 4);
            array.SetValue(new CloneableType("foo"), 1, 2, 3);
            Assert.AreEqual(new CloneableType("foo"), array.GetValueOrDefault(1, 2, 3));
            Assert.AreEqual(null, array.GetValueOrDefault(1, 3, 3));
        }

        [Test]
        public void IndexOf() {
            Assert.AreEqual(0, _array.IndexOf("one"));
            Assert.AreEqual(1, _array.IndexOf("two"));
            Assert.AreEqual(2, _array.IndexOf("three"));
            Assert.AreEqual(-1, _array.IndexOf("seven"));
        }


        [Test]
        public void ReverseFailed() {
            Assert.Throws<NotSupportedException>(_readOnly.Reverse);
            Assert.Throws<NotSupportedException>(() => _readOnly.Reverse(1,2));
        }

        [Test]
        public void SortFailed() {
            Assert.Throws<NotSupportedException>(_readOnly.Sort);
            Assert.Throws<NotSupportedException>(() => _readOnly.Sort((x, y) => 0));
            Assert.Throws<NotSupportedException>(() => _readOnly.Sort(Comparer<CloneableType>.Default));
            Assert.Throws<NotSupportedException>(() => _readOnly.Sort(0, 2, null));
        }

        [Test]
        public void ToArray() {
            CloneableType[] array = _array.ToArray();
            Assert.AreEqual(_array.Length, array.Length);
            for (int i = 0; i < _array.Length; i++)
                Assert.AreEqual(_array[i], array[i]);
        }

        [Test]
        public void ToArrayWithShape() {
            var array = (CloneableType[,]) _array.ToArrayWithShape();

            Assert.AreEqual(_array.Length, array.Length);
            Assert.AreEqual(_array.Rank, array.Rank);
            Assert.AreEqual(_array.GetLength(0), array.GetLength(0));
            Assert.AreEqual(_array.GetLength(1), array.GetLength(1));            

            CompareToArray(array, true);
        }

        private void CompareToArray(CloneableType[,] array, bool isSame) {

            // check equality by multidimensional index
            for (int i = 0; i < _array.GetLength(0); i++)
                for (int j = 0; j < _array.GetLength(1); j++) {
                    Assert.AreEqual(_array[i, j], array[i, j]);
                    if (isSame) Assert.AreSame(_array[i, j], array[i, j]);
                }

            // check equality by enumeration sequence
            int index = 0;
            foreach (CloneableType value in array) {
                Assert.AreEqual(value, _array[index]);
                if (isSame) Assert.AreSame(value, _array[index]);
                index++;
            }
        }
    }
}
