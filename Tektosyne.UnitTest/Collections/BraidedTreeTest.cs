using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    using Int32StringPair = KeyValuePair<Int32, CloneableType>;
    using Int32StringTree = BraidedTree<Int32, CloneableType>;

    using StringStringPair = KeyValuePair<String, CloneableType>;
    using StringStringTree = BraidedTree<String, CloneableType>;

    [TestFixture]
    public class BraidedTreeTest {

        StringStringTree _tree;

        [SetUp]
        public void SetUp() {
            _tree = new StringStringTree();
            Assert.AreEqual(0, _tree.Count);

            _tree.Add("one", "first value");
            _tree.Add("two", "second value");
            _tree.Add("three", "third value");
            Assert.AreEqual(3, _tree.Count);
        }

        [TearDown]
        public void TearDown() {
            _tree.Clear();
            _tree = null;
        }

        [Test]
        public void Constructor() {
            var tree = new StringStringTree();
            tree.Add("foo", "foo value");
            tree.Add("bar", "bar value");
            Assert.AreEqual(2, tree.Count);

            var clone = new StringStringTree(tree);
            Assert.AreEqual(2, clone.Count);
        }

        [Test]
        public void Count() {
            _tree.Add("foo", "foo value");
            _tree.Add("bar", "bar value");
            Assert.AreEqual(5, _tree.Count);

            _tree.Remove("bar");
            Assert.AreEqual(4, _tree.Count);
        }

        [Test]
        public void IsFixedSize() {
            Assert.IsFalse(_tree.IsFixedSize);
        }

        [Test]
        public void IsReadOnly() {
            Assert.IsFalse(_tree.IsReadOnly);
        }

        [Test]
        public void Item() {
            Assert.Throws<ArgumentNullException>(() => { var value = _tree[null]; });
            Assert.Throws<KeyNotFoundException>(() => { var value = _tree["bar"]; });

            Assert.AreEqual(new CloneableType("second value"), _tree["two"]);
            _tree["two"] = "foo value";
            Assert.AreEqual(new CloneableType("foo value"), _tree["two"]);

            _tree["bar"] = "bar value";
            Assert.AreEqual(new CloneableType("bar value"), _tree["bar"]);
        }

        [Test]
        public void Keys() {
            Assert.AreEqual(_tree.Count, _tree.Keys.Count);
            foreach (string key in _tree.Keys)
                Assert.IsTrue(_tree.ContainsKey(key));
        }

        [Test]
        public void Values() {
            Assert.AreEqual(_tree.Count, _tree.Values.Count);
            foreach (CloneableType value in _tree.Values)
                Assert.IsTrue(_tree.ContainsValue(value));
        }

        [Test]
        public void Add() {
            Assert.Throws<ArgumentNullException>(() => _tree.Add(null, "a null key"));
            Assert.Throws<ArgumentException>(() => _tree.Add("two", "another second value"));

            for (int i = 0; i < 20; i++) {
                string key = String.Format("bar{0}", i);
                _tree.Add(key, "bar value");
                Assert.IsTrue(_tree.Contains(new StringStringPair(key, "bar value")));
            }
            Assert.AreEqual(23, _tree.Count);
        }

        [Test]
        public void AddNull() {
            _tree.Add("null", null);
            Assert.IsTrue(_tree.ContainsValue(null));
        }

        [Test]
        public void AddPair() {
            Assert.Throws<ArgumentNullException>(() =>
                _tree.Add(new StringStringPair(null, "a null key")));
            Assert.Throws<ArgumentException>(() =>
                _tree.Add(new StringStringPair("two", "another second value")));

            _tree.Add(new StringStringPair("four", "fourth value"));
            Assert.AreEqual(4, _tree.Count);
            Assert.IsTrue(_tree.ContainsKey("four"));
            Assert.IsTrue(_tree.ContainsValue("fourth value"));
        }

        [Test]
        public void AddRange() {
            var tree = new StringStringTree();
            tree.Add(new StringStringPair("four", "fourth value"));
            tree.Add(new StringStringPair("five", "fifth value"));
            _tree.AddRange(tree);

            Assert.AreEqual(5, _tree.Count);
            Assert.IsTrue(_tree.ContainsKey("four"));
            Assert.IsTrue(_tree.ContainsValue("fourth value"));
            Assert.IsTrue(_tree.ContainsKey("five"));
            Assert.IsTrue(_tree.ContainsValue("fifth value"));

            Assert.Throws<ArgumentException>(() => _tree.AddRange(tree));
        }

        [Test]
        public void Clear() {
            _tree.Clear();
            Assert.AreEqual(0, _tree.Count);

            Assert.IsNull(_tree.RootNode.Left);
            Assert.IsNull(_tree.RootNode.Right);
            Assert.AreEqual(_tree.RootNode, _tree.RootNode.Previous);
            Assert.AreEqual(_tree.RootNode, _tree.RootNode.Next);
        }

        [Test]
        public void Clone() {
            var clone = (StringStringTree) _tree.Clone();

            Assert.AreEqual(_tree.Count, clone.Count);
            foreach (StringStringPair pair in clone) {
                Assert.IsTrue(_tree.Contains(pair));
                Assert.AreEqual(_tree[pair.Key], pair.Value);
                Assert.AreSame(_tree[pair.Key], pair.Value);
            }
        }

        [Test]
        public void Contains() {
            Assert.IsTrue(_tree.Contains(new StringStringPair("two", "second value")));
            Assert.IsFalse(_tree.Contains(new StringStringPair("four", "fourth value")));
        }

        [Test]
        public void ContainsKey() {
            Assert.Throws<ArgumentNullException>(() => _tree.ContainsKey(null));
            Assert.IsTrue(_tree.ContainsKey("one"));
            Assert.IsTrue(_tree.ContainsKey("two"));
            Assert.IsTrue(_tree.ContainsKey("three"));
            Assert.IsFalse(_tree.ContainsKey("four"));
        }

        [Test]
        public void ContainsValue() {
            Assert.IsFalse(_tree.ContainsValue(null));
            Assert.IsTrue(_tree.ContainsValue("first value"));
            Assert.IsTrue(_tree.ContainsValue("second value"));
            Assert.IsTrue(_tree.ContainsValue("third value"));
            Assert.IsFalse(_tree.ContainsValue("fourth value"));
        }

        [Test]
        public void Copy() {
            StringStringTree copy = _tree.Copy();

            Assert.AreEqual(_tree.Count, copy.Count);
            foreach (StringStringPair pair in copy) {
                Assert.IsTrue(_tree.Contains(pair));
                Assert.AreEqual(_tree[pair.Key], pair.Value);
                Assert.AreNotSame(_tree[pair.Key], pair.Value);
            }
        }

        [Test]
        public void CopyTo() {
            var array = new StringStringPair[4];
            Assert.Throws<ArgumentException>(() => _tree.CopyTo(array, 3));

            _tree.CopyTo(array, 0);
            for (int i = 0; i < _tree.Count; i++)
                Assert.IsTrue(_tree.Contains(array[i]));

            _tree.CopyTo(array, 1);
            for (int i = 0; i < _tree.Count; i++)
                Assert.IsTrue(_tree.Contains(array[i + 1]));
        }

        [Test]
        public void CopyToEmpty() {
            _tree.Clear();
            var array = new StringStringPair[0];
            _tree.CopyTo(array, 0);
        }

        [Test]
        public void Equals() {
            var tree = new StringStringTree();
            tree.Add("one", "first value");
            tree.Add("two", "second value");
            tree.Add("three", "third value");
            Assert.IsTrue(_tree.Equals(tree));

            tree["two"] = "foo value";
            Assert.IsFalse(_tree.Equals(tree));

            tree["two"] = "second value";
            Assert.IsTrue(_tree.Equals(tree));

            tree["foo"] = "second value";
            Assert.IsFalse(_tree.Equals(tree));
        }

        [Test]
        public void FindRange() {
            Assert.IsEmpty(_tree.FindRange("a", "b"));
            Assert.IsEmpty(_tree.FindRange("p", "s"));

            var output = _tree.FindRange("o", "p");
            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("first value", output["one"].Text);

            output = _tree.FindRange("m", "u");
            Assert.AreEqual(3, output.Count);
            Assert.AreEqual("first value", output["one"].Text);
            Assert.AreEqual("second value", output["two"].Text);
            Assert.AreEqual("third value", output["three"].Text);
        }

        [Test]
        public void FindRangeCondition() {
            Assert.IsEmpty(_tree.FindRange("o", "p", (n => n.Value.Text == "Hello World")));

            var output = _tree.FindRange("m", "u", (n => n.Key != "one"));
            Assert.AreEqual(2, output.Count);
            Assert.AreEqual("second value", output["two"].Text);
            Assert.AreEqual("third value", output["three"].Text);
        }

        [Test]
        public void GetEnumerator() {
            var enumerator = _tree.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(new StringStringPair("one", "first value"), enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(new StringStringPair("three", "third value"), enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(new StringStringPair("two", "second value"), enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void Remove() {
            Assert.IsTrue(_tree.Remove("two"));
            Assert.IsFalse(_tree.Remove("four"));
            Assert.AreEqual(2, _tree.Count);
        }

        [Test]
        public void RemovePair() {
            Assert.IsTrue(_tree.Remove(new StringStringPair("two", "second value")));
            Assert.IsFalse(_tree.Remove(new StringStringPair("four", "fourth value")));
            Assert.AreEqual(2, _tree.Count);
        }

        [Test]
        public void ToArray() {
            var array = _tree.ToArray();
            Assert.AreEqual(_tree.Count, array.Length);
            for (int i = 0; i < _tree.Count; i++)
                Assert.IsTrue(_tree.Contains(array[i]));
        }

        [Test]
        public void TryGetValue() {
            CloneableType value;
            Assert.IsTrue(_tree.TryGetValue("one", out value));
            Assert.AreEqual(new CloneableType("first value"), value);
            Assert.IsFalse(_tree.TryGetValue("four", out value));
            Assert.AreEqual(null, value);
        }

        [Test]
        public void RemoveNode() {
            int count = _tree.Count;
            Assert.IsFalse(_tree.RemoveNode(null));
            Assert.IsFalse(_tree.RemoveNode(_tree.RootNode));
            Assert.AreEqual(count, _tree.Count);

            var node = _tree.FirstNode;
            Assert.IsTrue(_tree.RemoveNode(node));
            Assert.AreEqual(count - 1, _tree.Count);
            Assert.Throws<ArgumentException>(() => _tree.RemoveNode(node));

            var tree = new StringStringTree();
            tree.Add(node.Key, node.Value);
            Assert.Throws<ArgumentException>(() => _tree.RemoveNode(tree.RootNode));
            Assert.Throws<ArgumentException>(() => _tree.RemoveNode(tree.FirstNode));
        }

        [Test]
        public void WalkTree() {
            var array = new KeyValuePair<String, CloneableType>[200];
            for (int i = 0; i < array.Length; i++)
                array[i] = new KeyValuePair<String,CloneableType>(
                    String.Format("bar{0:D3}", i),
                    new CloneableType(String.Format("bar{0:D3} value", i)));

            _tree.Clear();
            Assert.AreEqual(_tree, _tree.RootNode.Tree);
            Assert.AreEqual(_tree.RootNode, _tree.FirstNode);
            Assert.AreEqual(_tree.RootNode, _tree.LastNode);

            // test adding & finding elements
            foreach (var pair in array) {
                _tree.Add(pair.Key, pair.Value);
                CloneableType value;
                Assert.IsTrue(_tree.TryGetValue(pair.Key, out value));
                Assert.AreEqual(pair.Value, value);
            }
            Assert.AreEqual(array.Length, _tree.Count);

            // test finding elements in range
            var elements = _tree.FindRange("bar020", "bar060");
            Assert.AreEqual(41, elements.Count);
            for (int i = 20; i <= 60; i++) {
                CloneableType value;
                Assert.IsTrue(elements.TryGetValue(array[i].Key, out value));
                Assert.AreEqual(array[i].Value, value);
            }

            // follow Next references from FirstNode
            var node = _tree.FirstNode;
            foreach (var pair in array) {
                Assert.AreEqual(pair.Key, node.Key);
                Assert.AreEqual(pair.Value, node.Value);
                node = node.Next;
            }
            Assert.AreEqual(_tree.RootNode, node);

            // follow Previous references from LastNode
            node = _tree.LastNode;
            for (int i = array.Length - 1; i >= 0; i--) {
                Assert.AreEqual(array[i].Key, node.Key);
                Assert.AreEqual(array[i].Value, node.Value);
                node = node.Previous;
            }
            Assert.AreEqual(_tree.RootNode, node);

            // test node access and removing elements
            foreach (var pair in array) {
                node = _tree.FindNode(pair.Key);
                Assert.AreEqual(_tree, node.Tree);
                Assert.AreEqual(pair.Key, node.Key);
                Assert.AreEqual(pair.Value, node.Value);

                Assert.IsTrue(_tree.RemoveNode(node));
                Assert.IsNull(node.Tree);
                Assert.IsNull(_tree.FindNode(pair.Key));
            }
            Assert.AreEqual(0, _tree.Count);
        }
    }
}
