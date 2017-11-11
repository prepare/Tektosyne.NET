using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    using StringQueue = QueueEx<CloneableType>;

    [TestFixture]
    public class QueueExTest {

        StringQueue _queue;

        [SetUp]
        public void SetUp() {
            _queue = new StringQueue(16);
            Assert.AreEqual(0, _queue.Count);

            _queue.Enqueue("one");
            _queue.Enqueue("two");
            _queue.Enqueue("three");
            Assert.AreEqual(3, _queue.Count);
        }

        [TearDown]
        public void TearDown() {
            _queue.Clear();
            _queue = null;
        }

        [Test]
        public void Constructor() {
            StringQueue queue = new StringQueue(567);
            queue.Enqueue("foo"); queue.Enqueue("bar");
            Assert.AreEqual(2, queue.Count);

            CloneableType[] array = new CloneableType[4] { "a", "b", "c", "d" };
            queue = new StringQueue(array);
            Assert.AreEqual(4, queue.Count);
            Assert.IsTrue(queue.Equals(array));
        }

        [Test]
        public void Count() {
            _queue.Enqueue("foo");
            _queue.Enqueue("bar");
            Assert.AreEqual(5, _queue.Count);

            _queue.Dequeue();
            Assert.AreEqual(4, _queue.Count);
        }

        [Test]
        public void Clear() {
            _queue.Clear();
            Assert.AreEqual(0, _queue.Count);
        }

        [Test]
        public void Clone() {
            StringQueue clone = (StringQueue) _queue.Clone();
            Assert.AreEqual(_queue.Count, clone.Count);

            int count = _queue.Count;
            for (int i = 0; i < count; i++) {
                CloneableType x = _queue.Dequeue(), y = clone.Dequeue();
                Assert.AreEqual(x, y);
                Assert.AreSame(x, y);
            }
        }

        [Test]
        public void Contains() {
            Assert.IsTrue(_queue.Contains("one"));
            Assert.IsTrue(_queue.Contains("two"));
            Assert.IsTrue(_queue.Contains("three"));
            Assert.IsFalse(_queue.Contains("four"));
        }

        [Test]
        public void Copy() {
            StringQueue copy = _queue.Copy();
            Assert.AreEqual(_queue.Count, copy.Count);

            int count = _queue.Count;
            for (int i = 0; i < count; i++) {
                CloneableType x = _queue.Dequeue(), y = copy.Dequeue();
                Assert.AreEqual(x, y);
                Assert.AreNotSame(x, y);
            }
        }

        [Test]
        public void CopyToEmpty() {
            _queue.Clear();
            CloneableType[] array = new CloneableType[0];
            _queue.CopyTo(array, 0);
        }

        [Test]
        public void CopyToZero() {
            CloneableType[] array = new CloneableType[4];
            _queue.CopyTo(array, 0);

            int count = _queue.Count;
            for (int i = 0; i < count; i++)
                Assert.AreEqual(_queue.Dequeue(), array[i]);
        }

        [Test]
        public void CopyToOne() {
            CloneableType[] array = new CloneableType[4];
            Assert.Throws<ArgumentException>(() => _queue.CopyTo(array, 3));

            _queue.CopyTo(array, 1);
            int count = _queue.Count;
            for (int i = 0; i < count; i++)
                Assert.AreEqual(_queue.Dequeue(), array[i + 1]);
        }

        [Test]
        public void Dequeue() {
            Assert.AreEqual(new CloneableType("one"), _queue.Dequeue());
            Assert.AreEqual(2, _queue.Count);

            Assert.AreEqual(new CloneableType("two"), _queue.Dequeue());
            Assert.AreEqual(1, _queue.Count);

            Assert.AreEqual(new CloneableType("three"), _queue.Dequeue());
            Assert.AreEqual(0, _queue.Count);

            Assert.Throws<InvalidOperationException>(() => _queue.Dequeue());
        }

        [Test]
        public void Enqueue() {
            _queue.Clear();
            Assert.AreEqual(0, _queue.Count);

            for (int i = 0; i < 14; i++)
                _queue.Enqueue("bar" + i.ToString());
            Assert.AreEqual(14, _queue.Count);

            for (int i = 0; i < 14; i++)
                Assert.AreEqual(new CloneableType("bar" + i.ToString()), _queue.Dequeue());
            Assert.AreEqual(0, _queue.Count);
        }

        [Test]
        public void EnqueueNull() {
            _queue.Enqueue(null);
            Assert.IsTrue(_queue.Contains(null));
        }

        [Test]
        public void Equals() {
            StringQueue queue = new StringQueue();
            queue.Enqueue("one");
            queue.Enqueue("two");
            queue.Enqueue("three");
            Assert.IsTrue(_queue.Equals(queue));

            queue.Enqueue("one");
            Assert.IsFalse(_queue.Equals(queue));

            queue.Dequeue();
            Assert.IsFalse(_queue.Equals(queue));
        }

        [Test]
        public void GetEnumerator() {
            CloneableType[] array = new CloneableType[3];
            _queue.CopyTo(array, 0);

            int i = 0;
            foreach (CloneableType item in _queue)
                Assert.AreEqual(array[i++], item);
        }

        [Test]
        public void Peek() {
            Assert.AreEqual(new CloneableType("one"), _queue.Peek());
            Assert.AreEqual(new CloneableType("one"), _queue.Peek());
            _queue.Dequeue();
            Assert.AreEqual(new CloneableType("two"), _queue.Peek());

            _queue.Clear();
            Assert.Throws<InvalidOperationException>(() => _queue.Peek());
        }

        [Test]
        public void ToArray() {
            CloneableType[] array = _queue.ToArray();
            Assert.AreEqual(_queue.Count, array.Length);

            int count = _queue.Count;
            for (int i = 0; i < count; i++)
                Assert.AreEqual(_queue.Dequeue(), array[i]);
        }
    }
}
