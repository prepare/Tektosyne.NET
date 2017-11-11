using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    using StringStack = StackEx<CloneableType>;

    [TestFixture]
    public class StackExTest {

        StringStack _stack;

        [SetUp]
        public void SetUp() {
            _stack = new StringStack(16);
            Assert.AreEqual(0, _stack.Count);

            _stack.Push("one");
            _stack.Push("two");
            _stack.Push("three");
            Assert.AreEqual(3, _stack.Count);
        }

        [TearDown]
        public void TearDown() {
            _stack.Clear();
            _stack = null;
        }

        [Test]
        public void Constructor() {
            StringStack stack = new StringStack(567);
            stack.Push("foo"); stack.Push("bar");
            Assert.AreEqual(2, stack.Count);

            CloneableType[] array = new CloneableType[4] { "a", "b", "c", "d" };
            stack = new StringStack(array);
            Assert.AreEqual(4, stack.Count);
            Assert.IsFalse(stack.Equals(array));
            Assert.IsTrue(stack.EqualsReverse(array));
        }

        [Test]
        public void Count() {
            _stack.Push("foo");
            _stack.Push("bar");
            Assert.AreEqual(5, _stack.Count);

            _stack.Pop();
            Assert.AreEqual(4, _stack.Count);
        }

        [Test]
        public void Clear() {
            _stack.Clear();
            Assert.AreEqual(0, _stack.Count);
        }

        [Test]
        public void Clone() {
            StringStack clone = (StringStack) _stack.Clone();
            Assert.AreEqual(_stack.Count, clone.Count);

            int count = _stack.Count;
            for (int i = 0; i < count; i++) {
                CloneableType x = _stack.Pop(), y = clone.Pop();
                Assert.AreEqual(x, y);
                Assert.AreSame(x, y);
            }
        }

        [Test]
        public void Contains() {
            Assert.IsTrue(_stack.Contains("one"));
            Assert.IsTrue(_stack.Contains("two"));
            Assert.IsTrue(_stack.Contains("three"));
            Assert.IsFalse(_stack.Contains("four"));
        }

        [Test]
        public void Copy() {
            StringStack copy = _stack.Copy();
            Assert.AreEqual(_stack.Count, copy.Count);

            int count = _stack.Count;
            for (int i = 0; i < count; i++) {
                CloneableType x = _stack.Pop(), y = copy.Pop();
                Assert.AreEqual(x, y);
                Assert.AreNotSame(x, y);
            }
        }

        [Test]
        public void CopyToEmpty() {
            _stack.Clear();
            CloneableType[] array = new CloneableType[0];
            _stack.CopyTo(array, 0);
        }

        [Test]
        public void CopyToZero() {
            CloneableType[] array = new CloneableType[4];
            _stack.CopyTo(array, 0);

            int count = _stack.Count;
            for (int i = 0; i < count; i++)
                Assert.AreEqual(_stack.Pop(), array[i]);
        }

        [Test]
        public void CopyToOne() {
            CloneableType[] array = new CloneableType[4];
            Assert.Throws<ArgumentException>(() => _stack.CopyTo(array, 3));

            _stack.CopyTo(array, 1);
            int count = _stack.Count;
            for (int i = 0; i < count; i++)
                Assert.AreEqual(_stack.Pop(), array[i + 1]);
        }

        [Test]
        public void Equals() {
            StringStack stack = new StringStack();
            stack.Push("one");
            stack.Push("two");
            stack.Push("three");
            Assert.IsTrue(_stack.Equals(stack));
            Assert.IsFalse(_stack.EqualsReverse(stack));

            stack.Push("one");
            Assert.IsFalse(_stack.Equals(stack));

            stack.Pop();
            Assert.IsTrue(_stack.Equals(stack));
        }

        [Test]
        public void EqualsReverse() {
            List<CloneableType> list = new List<CloneableType>();
            list.AddRange(new CloneableType[] { "one", "two", "three" });
            Assert.IsFalse(_stack.Equals(list));
            Assert.IsTrue(_stack.EqualsReverse(list));

            list.Add("one");
            Assert.IsFalse(_stack.EqualsReverse(list));

            list.RemoveAt(list.Count - 1);
            Assert.IsTrue(_stack.EqualsReverse(list));
        }

        [Test]
        public void GetEnumerator() {
            CloneableType[] array = new CloneableType[3];
            _stack.CopyTo(array, 0);

            int i = 0;
            foreach (CloneableType item in _stack)
                Assert.AreEqual(array[i++], item);
        }

        [Test]
        public void Peek() {
            Assert.AreEqual(new CloneableType("three"), _stack.Peek());
            Assert.AreEqual(new CloneableType("three"), _stack.Peek());
            _stack.Pop();
            Assert.AreEqual(new CloneableType("two"), _stack.Peek());

            _stack.Clear();
            Assert.Throws<InvalidOperationException>(() => _stack.Peek());
        }

        [Test]
        public void Pop() {
            Assert.AreEqual(new CloneableType("three"), _stack.Pop());
            Assert.AreEqual(2, _stack.Count);

            Assert.AreEqual(new CloneableType("two"), _stack.Pop());
            Assert.AreEqual(1, _stack.Count);

            Assert.AreEqual(new CloneableType("one"), _stack.Pop());
            Assert.AreEqual(0, _stack.Count);

            Assert.Throws<InvalidOperationException>(() => _stack.Pop());
        }

        [Test]
        public void Push() {
            _stack.Clear();
            Assert.AreEqual(0, _stack.Count);

            for (int i = 0; i < 14; i++)
                _stack.Push("bar" + i.ToString());
            Assert.AreEqual(14, _stack.Count);

            for (int i = 13; i >= 0; i--)
                Assert.AreEqual(new CloneableType("bar" + i.ToString()), _stack.Pop());
            Assert.AreEqual(0, _stack.Count);
        }

        [Test]
        public void PushNull() {
            _stack.Push(null);
            Assert.IsTrue(_stack.Contains(null));
        }

        [Test]
        public void ToArray() {
            CloneableType[] array = _stack.ToArray();
            Assert.AreEqual(_stack.Count, array.Length);

            int count = _stack.Count;
            for (int i = 0; i < count; i++)
                Assert.AreEqual(_stack.Pop(), array[i]);
        }
    }
}
