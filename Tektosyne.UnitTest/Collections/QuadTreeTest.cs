using System;
using System.Collections.Generic;

using NUnit.Framework;
using Tektosyne.Collections;
using Tektosyne.Geometry;

namespace Tektosyne.UnitTest.Collections {

    using PointStringPair = KeyValuePair<PointD, CloneableType>;
    using PointStringTree = QuadTree<CloneableType>;
    using PointStringNode = QuadTreeNode<CloneableType>;

    [TestFixture]
    public class QuadTreeTest {

        PointStringTree _tree;
        PointD firstKey = new PointD(0, 0),
            secondKey = new PointD(50, 0),
            thirdKey = new PointD(0, 50),
            fourthKey = new PointD(50, 50),
            invalidKey = new PointD(0, 300);

        [SetUp]
        public void SetUp() {
            _tree = new PointStringTree(new RectD(-100, -100, 200, 200));
            Assert.AreEqual(0, _tree.Count);
            Assert.AreEqual(1, _tree.Nodes.Count);

            _tree.Add(firstKey, "first value");
            _tree.Add(secondKey, "second value");
            _tree.Add(thirdKey, "third value");
            Assert.AreEqual(3, _tree.Count);
            Assert.AreEqual(1, _tree.Nodes.Count);
        }

        [TearDown]
        public void TearDown() {
            _tree.Clear();
            _tree = null;
        }

        [Test]
        public void Constructor() {
            var tree = new PointStringTree(new RectD(0, 0, 100, 100));
            tree.Add(new PointD(10, 10), "foo value");
            tree.Add(new PointD(20, 20), "bar value");
            Assert.AreEqual(2, tree.Count);

            var clone = new PointStringTree(new RectD(0, 0, 100, 100), tree);
            Assert.AreEqual(2, clone.Count);
        }

        [Test]
        public void Count() {
            _tree.Add(new PointD(-50, 0), "foo value");
            _tree.Add(new PointD(0, -50), "bar value");
            Assert.AreEqual(5, _tree.Count);

            _tree.Remove(new PointD(0, -50));
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
            Assert.Throws<KeyNotFoundException>(() => { var value = _tree[fourthKey]; });

            Assert.AreEqual(new CloneableType("second value"), _tree[secondKey]);
            _tree[secondKey] = "foo value";
            Assert.AreEqual(new CloneableType("foo value"), _tree[secondKey]);

            _tree[fourthKey] = "bar value";
            Assert.AreEqual(new CloneableType("bar value"), _tree[fourthKey]);
        }

        [Test]
        public void Keys() {
            Assert.AreEqual(_tree.Count, _tree.Keys.Count);
            foreach (PointD key in _tree.Keys)
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
            Assert.Throws<ArgumentException>(() => _tree.Add(secondKey, "another second value"));

            _tree.Add(fourthKey, null);
            Assert.IsTrue(_tree.ContainsValue(null));
            Assert.Throws<ArgumentException>(() => _tree.Add(secondKey, "new second value"));
            Assert.Throws<ArgumentOutOfRangeException>(() => _tree.Add(invalidKey, "invalid key"));
        }

        [Test]
        public void AddPair() {
            Assert.Throws<ArgumentException>(() =>
                _tree.Add(new PointStringPair(secondKey, "another second value")));

            _tree.Add(new PointStringPair(fourthKey, "fourth value"));
            Assert.AreEqual(4, _tree.Count);
            Assert.IsTrue(_tree.ContainsKey(fourthKey));
            Assert.IsTrue(_tree.ContainsValue("fourth value"));
        }

        [Test]
        public void AddRange() {
            var tree = new PointStringTree(_tree.Bounds);
            tree.Add(new PointStringPair(fourthKey, "fourth value"));
            tree.Add(new PointStringPair(new PointD(-50, -50), "fifth value"));
            _tree.AddRange(tree);

            Assert.AreEqual(5, _tree.Count);
            Assert.IsTrue(_tree.ContainsKey(fourthKey));
            Assert.IsTrue(_tree.ContainsValue("fourth value"));
            Assert.IsTrue(_tree.ContainsKey(new PointD(-50, -50)));
            Assert.IsTrue(_tree.ContainsValue("fifth value"));

            Assert.Throws<ArgumentException>(() => _tree.AddRange(tree));
        }

        [Test]
        public void Clear() {
            _tree.Clear();
            Assert.AreEqual(0, _tree.Count);
            Assert.AreEqual(0, _tree.RootNode.Data.Count);

            Assert.IsNull(_tree.RootNode.TopLeft);
            Assert.IsNull(_tree.RootNode.TopRight);
            Assert.IsNull(_tree.RootNode.BottomLeft);
            Assert.IsNull(_tree.RootNode.BottomRight);
        }

        [Test]
        public void Clone() {
            var clone = (PointStringTree) _tree.Clone();

            Assert.AreEqual(_tree.Count, clone.Count);
            foreach (PointStringPair pair in clone) {
                Assert.IsTrue(_tree.Contains(pair));
                Assert.AreEqual(_tree[pair.Key], pair.Value);
                Assert.AreSame(_tree[pair.Key], pair.Value);
            }
        }

        [Test]
        public void Contains() {
            Assert.IsTrue(_tree.Contains(new PointStringPair(firstKey, "first value")));
            Assert.IsTrue(_tree.Contains(new PointStringPair(secondKey, "second value")));
            Assert.IsTrue(_tree.Contains(new PointStringPair(thirdKey, "third value")));

            Assert.IsFalse(_tree.Contains(new PointStringPair(firstKey, "second value")));
            Assert.IsFalse(_tree.Contains(new PointStringPair(fourthKey, "fourth value")));
            Assert.IsFalse(_tree.Contains(new PointStringPair(invalidKey, null)));
        }

        [Test]
        public void ContainsKey() {
            Assert.IsTrue(_tree.ContainsKey(firstKey));
            Assert.IsTrue(_tree.ContainsKey(secondKey));
            Assert.IsTrue(_tree.ContainsKey(thirdKey));

            Assert.IsFalse(_tree.ContainsKey(fourthKey));
            Assert.IsFalse(_tree.ContainsKey(invalidKey));
        }

        [Test]
        public void ContainsValue() {
            Assert.IsTrue(_tree.ContainsValue("first value"));
            Assert.IsTrue(_tree.ContainsValue("second value"));
            Assert.IsTrue(_tree.ContainsValue("third value"));

            Assert.IsFalse(_tree.ContainsValue("fourth value"));
            Assert.IsFalse(_tree.ContainsValue(null));
        }

        [Test]
        public void ContainsWithNode() {
            var nodes = new PointStringNode[] { null,
                _tree.FindNode(firstKey),
                _tree.FindNode(secondKey),
                _tree.FindNode(thirdKey)
            };

            Assert.IsNotNull(nodes[1]);
            Assert.IsNotNull(nodes[2]);
            Assert.IsNotNull(nodes[3]);

            PointStringNode node;
            foreach (var startNode in nodes) {
                node = startNode; Assert.IsTrue(_tree.ContainsKey(firstKey, ref node));
                node = startNode; Assert.IsTrue(_tree.ContainsKey(secondKey, ref node));
                node = startNode; Assert.IsTrue(_tree.ContainsKey(thirdKey, ref node));

                node = startNode; Assert.IsFalse(_tree.ContainsKey(fourthKey, ref node));
                node = startNode; Assert.IsFalse(_tree.ContainsKey(invalidKey, ref node));

                node = startNode; Assert.IsTrue(_tree.ContainsValue("first value", ref node));
                node = startNode; Assert.IsTrue(_tree.ContainsValue("second value", ref node));
                node = startNode; Assert.IsTrue(_tree.ContainsValue("third value", ref node));

                node = startNode; Assert.IsFalse(_tree.ContainsValue("fourth value", ref node));
                node = startNode; Assert.IsFalse(_tree.ContainsValue(null, ref node));
            }
        }

        [Test]
        public void Copy() {
            PointStringTree copy = _tree.Copy();

            Assert.AreEqual(_tree.Count, copy.Count);
            foreach (PointStringPair pair in copy) {
                Assert.IsTrue(_tree.Contains(pair));
                Assert.AreEqual(_tree[pair.Key], pair.Value);
                Assert.AreNotSame(_tree[pair.Key], pair.Value);
            }
        }

        [Test]
        public void CopyTo() {
            var array = new PointStringPair[4];
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
            var array = new PointStringPair[0];
            _tree.CopyTo(array, 0);
        }

        [Test]
        public void Equals() {
            var tree = new PointStringTree(_tree.Bounds);
            tree.Add(firstKey, "first value");
            tree.Add(secondKey, "second value");
            tree.Add(thirdKey, "third value");
            Assert.IsTrue(_tree.Equals(tree));

            tree[secondKey] = "foo value";
            Assert.IsFalse(_tree.Equals(tree));

            tree[secondKey] = "second value";
            Assert.IsTrue(_tree.Equals(tree));

            tree[fourthKey] = "second value";
            Assert.IsFalse(_tree.Equals(tree));
        }

        [Test]
        public void FindRangeCircle() {
            Assert.IsEmpty(_tree.FindRange(new PointD(250, 250), 50));
            Assert.IsEmpty(_tree.FindRange(new PointD(75, 75), 25));

            var output = _tree.FindRange(new PointD(5, 5), 10);
            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("first value", output[firstKey].Text);

            output = _tree.FindRange(new PointD(0, 50), 60);
            Assert.AreEqual(2, output.Count);
            Assert.AreEqual("first value", output[firstKey].Text);
            Assert.AreEqual("third value", output[thirdKey].Text);
        }

        [Test]
        public void FindRangeRect() {
            Assert.IsEmpty(_tree.FindRange(new RectD(200, 200, 100, 100)));
            Assert.IsEmpty(_tree.FindRange(new RectD(50, 50, 100, 100)));

            var output = _tree.FindRange(new RectD(0, 0, 20, 20));
            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("first value", output[firstKey].Text);

            output = _tree.FindRange(new RectD(0, 0, 50, 50));
            Assert.AreEqual(3, output.Count);
            Assert.AreEqual("first value", output[firstKey].Text);
            Assert.AreEqual("second value", output[secondKey].Text);
            Assert.AreEqual("third value", output[thirdKey].Text);
        }

        [Test]
        public void GetEnumerator() {
            var enumerator = _tree.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(new PointStringPair(firstKey, "first value"), enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(new PointStringPair(secondKey, "second value"), enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(new PointStringPair(thirdKey, "third value"), enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void Move() {
            var node = _tree.Move(firstKey, new PointD(10, 10));
            Assert.IsFalse(_tree.ContainsKey(firstKey));
            Assert.AreEqual("first value", _tree[new PointD(10, 10)].Text);

            node = _tree.Move(secondKey, new PointD(30, 30), node);
            Assert.IsFalse(_tree.ContainsKey(secondKey));
            Assert.AreEqual("second value", _tree[new PointD(30, 30)].Text);
        }

        [Test]
        public void Remove() {
            Assert.IsTrue(_tree.Remove(secondKey));
            Assert.IsFalse(_tree.Remove(fourthKey));
            Assert.AreEqual(2, _tree.Count);

            Assert.IsTrue(_tree.Remove(firstKey));
            Assert.IsTrue(_tree.Remove(thirdKey));
            Assert.AreEqual(0, _tree.Count);
        }

        [Test]
        public void RemovePair() {
            Assert.IsTrue(_tree.Remove(new PointStringPair(secondKey, "second value")));
            Assert.IsFalse(_tree.Remove(new PointStringPair(fourthKey, "fourth value")));
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
            Assert.IsTrue(_tree.TryGetValue(firstKey, out value));
            Assert.AreEqual(new CloneableType("first value"), value);
            Assert.IsFalse(_tree.TryGetValue(invalidKey, out value));
            Assert.AreEqual(null, value);
        }

        [Test]
        public void WalkTree() {
            const int radius = 3000;
            var array = new KeyValuePair<PointD, CloneableType>[2 * radius];
            for (int i = 0; i < array.Length; i++)
                array[i] = new KeyValuePair<PointD, CloneableType>(
                    new PointD(i - radius, radius - i),
                    new CloneableType(String.Format("bar{0:D3} value", i)));

            var tree = new QuadTree<CloneableType>(
                new RectD(-2 * radius, -2 * radius, 4 * radius, 4 * radius));
            Assert.AreEqual(tree, tree.RootNode.Tree);
            Assert.AreEqual(1, tree.Nodes.Count);

            // test adding elements
            foreach (var pair in array)
                tree.Add(pair.Key, pair.Value);
            Assert.AreEqual(array.Length, tree.Count);
            Assert.AreEqual(145, tree.Nodes.Count);

            // test moving elements without hint node
            PointD offset = new PointD(0.1, 0.1);
            foreach (var pair in array)
                tree.Move(pair.Key, pair.Key + offset);

            // test moving elements with hint node
            QuadTreeNode<CloneableType> node = null;
            foreach (var pair in array)
                node = tree.Move(pair.Key + offset, pair.Key, node);

            Assert.AreEqual(array.Length, tree.Count);
            Assert.AreEqual(145, tree.Nodes.Count);

            // test finding elements
            foreach (var pair in array) {
                CloneableType value;
                Assert.IsTrue(tree.TryGetValue(pair.Key, out value));
                Assert.AreEqual(pair.Value, value);

                node = tree.FindNode(pair.Key);
                Assert.AreEqual(tree, node.Tree);
                Assert.IsTrue(node.Data.Contains(pair));

                var valueNode = tree.FindNodeByValue(pair.Value);
                Assert.AreEqual(node, valueNode);
            }

            // test finding elements in range
            var range = new RectD(-radius, 0, 2 * radius, 2 * radius);
            var elements = tree.FindRange(range);
            Assert.AreEqual(radius + 1, elements.Count);

            for (int i = 0; i <= radius; i++) {
                CloneableType value;
                Assert.IsTrue(elements.TryGetValue(array[i].Key, out value));
                Assert.AreEqual(array[i].Value, value);
            }

            // compare range search to BraidedTree
            var braidedTree = new BraidedTree<PointD, CloneableType>(PointDComparerY.CompareExact);
            foreach (var pair in array)
                braidedTree.Add(pair);

            // BraidedTree sorts by y-coordinates, so we must restrict x-coordinates
            var braidedElements = braidedTree.FindRange(range.TopLeft, range.BottomRight,
                n => (n.Key.X >= range.Left && n.Key.X <= range.Right));
            Assert.AreEqual(elements.Count, braidedElements.Count);

            foreach (var pair in elements) {
                CloneableType value;
                Assert.IsTrue(braidedElements.TryGetValue(pair.Key, out value));
                Assert.AreEqual(pair.Value, value);
            }

            // test element enumeration
            foreach (var pair in tree) {
                CloneableType value;
                Assert.IsTrue(braidedTree.TryGetValue(pair.Key, out value));
                Assert.AreEqual(value, pair.Value);
                braidedTree.Remove(pair.Key);
            }
            Assert.AreEqual(0, braidedTree.Count);

            // test removing elements
            foreach (var pair in array)
                Assert.IsTrue(tree.Remove(pair.Key));
            Assert.AreEqual(0, tree.Count);
            Assert.AreEqual(1, tree.Nodes.Count);
        }
    }
}
