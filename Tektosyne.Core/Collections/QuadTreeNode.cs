using System;
using System.Collections.Generic;
using System.Diagnostics;

using Tektosyne.Geometry;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides a generic tree node within a <see cref="QuadTree{TValue}"/>.</summary>
    /// <typeparam name="TValue">
    /// The type of the values that are associated with each <see cref="PointD"/> key. If
    /// <typeparamref name="TValue"/> is a reference type, the value may be null references.
    /// </typeparam>
    /// <remarks><para>
    /// <b>QuadTreeNode</b> represents a collection of key-and-value pairs within a <see
    /// cref="QuadTree{TValue}"/>. This collection and all references to related <b>QuadTreeNode</b>
    /// instances within the tree structure are exposed as read-only properties.
    /// </para><para>
    /// <b>QuadTreeNode</b> was inspired by the <c>QuadtreeNode</c> class by Michael J. Laszlo,
    /// <em>Computational Geometry and Computer Graphics in C++</em>, Prentice Hall 1996, p.236ff.
    /// </para></remarks>

    [Serializable]
    public class QuadTreeNode<TValue> {
        #region QuadTreeNode(...)

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTreeNode{TValue}"/> class.</summary>
        /// <param name="tree">
        /// The <see cref="QuadTree{TValue}"/> that contains the <see cref="QuadTreeNode{TValue}"/>.
        /// </param>
        /// <param name="signature">
        /// The signature of the <see cref="QuadTreeNode{TValue}"/>.</param>
        /// <param name="parent">
        /// The parent of the <see cref="QuadTreeNode{TValue}"/> in the tree structure.</param>
        /// <param name="bounds">
        /// The bounds of all keys in the <see cref="QuadTreeNode{TValue}"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tree"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bounds"/> contains a <see cref="RectD.Width"/> or <see
        /// cref="RectD.Height"/> that is equal to or less than zero.</exception>

        internal QuadTreeNode(QuadTree<TValue> tree,
            int signature, QuadTreeNode<TValue> parent, RectD bounds) {

            if (tree == null)
                ThrowHelper.ThrowArgumentNullException("tree");

            if (bounds.Width <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "bounds.Width", bounds.Width, Strings.ArgumentNotPositive);

            if (bounds.Height <= 0.0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "bounds.Height", bounds.Height, Strings.ArgumentNotPositive);

            Tree = tree;
            Signature = signature;
            Parent = parent;
            Bounds = bounds;
            Center = new PointD(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);

            tree._nodes.Add(signature, this);
            _data = new DictionaryEx<PointD, TValue>(tree.Capacity);
        }

        #endregion
        #region Private Fields

        /// <summary>
        /// The bottom left child of the <see cref="QuadTreeNode{TValue}"/> in the tree structure.
        /// </summary>

        private QuadTreeNode<TValue> _bottomLeft;

        /// <summary>
        /// The bottom right child of the <see cref="QuadTreeNode{TValue}"/> in the tree structure.
        /// </summary>

        private QuadTreeNode<TValue> _bottomRight;

        /// <summary>
        /// The top left child of the <see cref="QuadTreeNode{TValue}"/> in the tree structure.
        /// </summary>

        private QuadTreeNode<TValue> _topLeft;

        /// <summary>
        /// The top right child of the <see cref="QuadTreeNode{TValue}"/> in the tree structure.
        /// </summary>

        private QuadTreeNode<TValue> _topRight;

        #endregion
        #region Internal Fields

        /// <summary>
        /// The key-and-value pairs stored in the <see cref="QuadTreeNode{TValue}"/>.</summary>

        internal DictionaryEx<PointD, TValue> _data;

        #endregion
        #region Public Properties
        #region BottomLeft

        /// <summary>
        /// Gets the bottom left child of the <see cref="QuadTreeNode{TValue}"/> in the tree
        /// structure.</summary>
        /// <value>
        /// The bottom left child of the <see cref="QuadTreeNode{TValue}"/>, if any. The default is
        /// a null reference.</value>
        /// <remarks><para>
        /// <b>BottomLeft</b> returns the child node that covers the <see cref="RectD.BottomLeft"/>
        /// quadrant of the current <see cref="Bounds"/>.
        /// </para><para>
        /// The chains of <see cref="TopLeft"/>, <see cref="TopRight"/>, <b>BottomLeft</b>, <see
        /// cref="BottomRight"/>, and <see cref="Parent"/> references determine the structure of the
        /// containing <see cref="Tree"/>.</para></remarks>

        public QuadTreeNode<TValue> BottomLeft {
            [DebuggerStepThrough]
            get { return _bottomLeft; }
        }

        #endregion
        #region BottomRight

        /// <summary>
        /// Gets the bottom right child of the <see cref="QuadTreeNode{TValue}"/> in the tree
        /// structure.</summary>
        /// <value>
        /// The bottom right child of the <see cref="QuadTreeNode{TValue}"/>, if any. The default is
        /// a null reference.</value>
        /// <remarks><para>
        /// <b>BottomRight</b> returns the child node that covers the <see cref="RectD.BottomRight"/>
        /// quadrant of the current <see cref="Bounds"/>.
        /// </para><para>
        /// The chains of <see cref="TopLeft"/>, <see cref="TopRight"/>, <see cref="BottomLeft"/>, 
        /// <b>BottomRight</b>, and <see cref="Parent"/> references determine the structure of the
        /// containing <see cref="Tree"/>.</para></remarks>

        public QuadTreeNode<TValue> BottomRight {
            [DebuggerStepThrough]
            get { return _bottomRight; }
        }

        #endregion
        #region Bounds

        /// <summary>
        /// The bounds of all keys in the <see cref="QuadTreeNode{TValue}"/>.</summary>
        /// <remarks><para>
        /// <b>Bounds</b> holds a <see cref="RectD"/> indicating the subrange of the containing <see
        /// cref="Tree"/> that is covered by the <see cref="QuadTreeNode{TValue}"/> and its
        /// children. <b>Bounds</b> always has a positive <see cref="RectD.Width"/> and <see
        /// cref="RectD.Height"/>. The two dimensions are not necessarily equal.
        /// </para><para>
        /// Any <see cref="DictionaryEx{PointD, TValue}.Keys"/> stored in the associated <see
        /// cref="Data"/> collection lie within <b>Bounds</b>. The extreme <see cref="RectD.Right"/>
        /// and <see cref="RectD.Bottom"/> coordinates are considered part of the neighboring <see
        /// cref="QuadTreeNode{TValue}"/> on that side, if there is one.</para></remarks>

        public readonly RectD Bounds;

        #endregion
        #region Center

        /// <summary>
        /// The center of the <see cref="Bounds"/> of the <see cref="QuadTreeNode{TValue}"/>.
        /// </summary>
        /// <remarks>
        /// <b>Center</b> divides the associated <see cref="Bounds"/> into four equal-sized
        /// quadrants, corresponding to the <see cref="TopLeft"/>, <see cref="TopRight"/>, <see
        /// cref="BottomLeft"/>, and <see cref="BottomRight"/> child nodes. <b>Center</b> is
        /// precomputed to speed up the traversal of the tree structure.</remarks>

        public readonly PointD Center;

        #endregion
        #region Data

        /// <summary>
        /// Gets a read-only view of the key-and-value pairs stored in the <see
        /// cref="QuadTreeNode{TValue}"/>.</summary>
        /// <value><para>
        /// A read-only <see cref="DictionaryEx{PointD, TValue}"/> containing any key-and-value
        /// pairs stored in the <see cref="QuadTreeNode{TValue}"/> if <see cref="IsLeaf"/> is
        /// <c>true</c>.
        /// </para><para>-or-</para><para>
        /// A null reference if <see cref="IsLeaf"/> is <c>false</c>.</para></value>
        /// <remarks><para>
        /// <b>Data</b> usually contains up to <see cref="QuadTree{TValue}.Capacity"/> elements when
        /// not a null reference. If <see cref="Level"/> equals <see
        /// cref="QuadTree{TValue}.MaxLevel"/>, the number of elements is unbounded.
        /// </para><para>
        /// <b>Data</b> returns a read-only view of the <see cref="DictionaryEx{PointD, TValue}"/>.
        /// Use the containing <see cref="Tree"/> to add, change, or remove keys and values.
        /// </para></remarks>

        public DictionaryEx<PointD, TValue> Data {
            [DebuggerStepThrough]
            get { return (_data == null ? null : _data.AsReadOnly()); }
        }

        #endregion
        #region GridX

        /// <summary>
        /// Gets the x-coordinate of the <see cref="QuadTreeNode{TValue}"/> in the tree structure.
        /// </summary>
        /// <value>
        /// The x-coordinate of the <see cref="QuadTreeNode{TValue}"/> within the grid of its <see
        /// cref="Level"/>, ranging from zero to 2^<see cref="Level"/>.</value>
        /// <remarks>
        /// <b>GridX</b> returns the middle 14 bits of <see cref="Signature"/>.</remarks>

        public int GridX {
            [DebuggerStepThrough]
            get { return (int) (((uint) Signature & 0x0003FFF0) >> 4); }
        }

        #endregion
        #region GridY

        /// <summary>
        /// Gets the y-coordinate of the <see cref="QuadTreeNode{TValue}"/> in the tree structure.
        /// </summary>
        /// <value>
        /// The y-coordinate of the <see cref="QuadTreeNode{TValue}"/> within the grid of its <see
        /// cref="Level"/>, ranging from zero to 2^<see cref="Level"/>.</value>
        /// <remarks>
        /// <b>GridY</b> returns the highest 14 bits of <see cref="Signature"/>.</remarks>

        public int GridY {
            [DebuggerStepThrough]
            get { return (int) (((uint) Signature & 0xFFFC0000) >> 18); }
        }

        #endregion
        #region HasCapacity

        /// <summary>
        /// Gets a value indicating whether the <see cref="QuadTreeNode{TValue}"/> has any remaining
        /// <see cref="Data"/> capacity. </summary>
        /// <value>
        /// <c>true</c> if <see cref="IsLeaf"/> is <c>true</c> and the number of elements in the
        /// <see cref="Data"/> collection is less than <see cref="QuadTree{TValue}.Capacity"/>;
        /// otherwise, <c>false</c>.</value>
        /// <remarks>
        /// <b>HasCapacity</b> does not check whether <see cref="Level"/> equals <see
        /// cref="QuadTree{TValue}.MaxLevel"/>, in which case the number of <see cref="Data"/>
        /// elements may exceed <see cref="QuadTree{TValue}.Capacity"/>.</remarks>

        public bool HasCapacity {
            get { return (_data != null && _data.Count < Tree.Capacity); }
        }

        #endregion
        #region IsLeaf

        /// <summary>
        /// Gets a value indicating whether the <see cref="QuadTreeNode{TValue}"/> is a leaf node.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="QuadTreeNode{TValue}"/> is a leaf node; <c>false</c> if it
        /// is an internal node. The default is <c>true</c>.</value>
        /// <remarks><para>
        /// <b>IsLeaf</b> is <c>true</c> if <see cref="Data"/> is a valid collection. In that case,
        /// the <see cref="TopLeft"/>, <see cref="TopRight"/>, <see cref="BottomLeft"/>, and <see
        /// cref="BottomRight"/> properties are all null references.
        /// </para><para>
        /// <b>IsLeaf</b> is <c>false</c> if <see cref="Data"/> is a null reference. In that case,
        /// at least one of the <see cref="TopLeft"/>, <see cref="TopRight"/>, <see
        /// cref="BottomLeft"/>, and <see cref="BottomRight"/> properties is a valid child node.
        /// </para></remarks>

        public bool IsLeaf {
            [DebuggerStepThrough]
            get { return (_data != null); }
        }

        #endregion
        #region Level

        /// <summary>
        /// Gets the level of the <see cref="QuadTreeNode{TValue}"/> in the tree structure.
        /// </summary>
        /// <value>
        /// The level of the <see cref="QuadTreeNode{TValue}"/> in the containing <see
        /// cref="Tree"/>, ranging from zero to <see cref="QuadTree{TValue}.MaxLevel"/>.</value>
        /// <remarks>
        /// <b>Level</b> returns the lowest 4 bits of <see cref="Signature"/>.</remarks>

        public int Level {
            [DebuggerStepThrough]
            get { return (Signature & 0x0000000F); }
        }

        #endregion
        #region Parent

        /// <summary>
        /// The parent of the <see cref="QuadTreeNode{TValue}"/> in the tree structure.</summary>
        /// <remarks><para>
        /// <b>Parent</b> never holds a null reference, except on the permanent <see
        /// cref="QuadTree{TValue}.RootNode"/> of the containing <see cref="Tree"/>.
        /// </para><para>
        /// The chains of <see cref="TopLeft"/>, <see cref="TopRight"/>, <see cref="BottomLeft"/>,
        /// <see cref="BottomRight"/>, and <b>Parent</b> references determine the structure of the
        /// containing <see cref="Tree"/>.</para></remarks>

        public readonly QuadTreeNode<TValue> Parent;
        
        #endregion
        #region Signature

        /// <summary>
        /// The signature of the <see cref="QuadTreeNode{TValue}"/>.</summary>
        /// <remarks><para>
        /// <b>Signature</b> holds an <see cref="Int32"/> value containing a bitwise combination of
        /// the <see cref="Level"/>, <see cref="GridX"/>, and <see cref="GridY"/> values.
        /// </para><para>
        /// <b>Signature</b> uniquely identifies the position of each <see
        /// cref="QuadTreeNode{TValue}"/> in the containing <see cref="Tree"/>, and also serves as
        /// its key within the <see cref="QuadTree{TValue}.Nodes"/> hashtable.</para></remarks>

        public readonly int Signature;

        #endregion
        #region TopLeft

        /// <summary>
        /// Gets the top left child of the <see cref="QuadTreeNode{TValue}"/> in the tree structure.
        /// </summary>
        /// <value>
        /// The top left child of the <see cref="QuadTreeNode{TValue}"/>, if any. The default is a
        /// null reference.</value>
        /// <remarks><para>
        /// <b>TopLeft</b> returns the child node that covers the <see cref="RectD.TopLeft"/>
        /// quadrant of the current <see cref="Bounds"/>.
        /// </para><para>
        /// The chains of <b>TopLeft</b>, <see cref="TopRight"/>, <see cref="BottomLeft"/>, <see
        /// cref="BottomRight"/>, and <see cref="Parent"/> references determine the structure of the
        /// containing <see cref="Tree"/>.</para></remarks>

        public QuadTreeNode<TValue> TopLeft {
            [DebuggerStepThrough]
            get { return _topLeft; }
        }

        #endregion
        #region TopRight

        /// <summary>
        /// Gets the top right child of the <see cref="QuadTreeNode{TValue}"/> in the tree
        /// structure.</summary>
        /// <value>
        /// The top right child of the <see cref="QuadTreeNode{TValue}"/>, if any. The default is a
        /// null reference.</value>
        /// <remarks><para>
        /// <b>TopRight</b> returns the child node that covers the <see cref="RectD.TopRight"/>
        /// quadrant of the current <see cref="Bounds"/>.
        /// </para><para>
        /// The chains of <see cref="TopLeft"/>, <b>TopRight</b>, <see cref="BottomLeft"/>, <see
        /// cref="BottomRight"/>, and <see cref="Parent"/> references determine the structure of the
        /// containing <see cref="Tree"/>.</para></remarks>

        public QuadTreeNode<TValue> TopRight {
            [DebuggerStepThrough]
            get { return _topRight; }
        }

        #endregion
        #region Tree

        /// <summary>
        /// The <see cref="QuadTree{TValue}"/> that contains the <see cref="QuadTreeNode{TValue}"/>.
        /// </summary>
        /// <remarks>
        /// <b>Tree</b> never holds a null reference.</remarks>

        public readonly QuadTree<TValue> Tree;

        #endregion
        #endregion
        #region Internal Methods
        #region Clear

        /// <summary>
        /// Clears the <see cref="QuadTreeNode{TValue}"/>.</summary>
        /// <remarks><para>
        /// <b>Clear</b> resets the <see cref="TopLeft"/>, <see cref="TopRight"/>, <see
        /// cref="BottomLeft"/>, and <see cref="BottomRight"/> properties to null references, and
        /// <see cref="Data"/> to an empty <see cref="DictionaryEx{PointD, TValue}"/>.
        /// </para><para>
        /// <b>Clear</b> does not clear any previously attached child nodes, or transfer their <see
        /// cref="Data"/>. Call this method only on the <see cref="QuadTree{TValue}.RootNode"/> of
        /// the containing <see cref="Tree"/> to destroy the entire tree structure.</para></remarks>

        internal void Clear() {
            Debug.Assert(this == Tree.RootNode);

            _topLeft = _topRight = _bottomLeft = _bottomRight = null;
            _data = new DictionaryEx<PointD, TValue>(Tree.Capacity);
        }

        #endregion
        #region FindChild

        /// <summary>
        /// Finds the child node of the <see cref="QuadTreeNode{TValue}"/> that contains the
        /// specified key.</summary>
        /// <param name="key">
        /// The key to locate. This argument must lie within <see cref="Bounds"/>.</param>
        /// <returns>
        /// The child of the <see cref="QuadTreeNode{TValue}"/> that contains <paramref
        /// name="key"/>, or a null reference if the child does not yet exist.</returns>
        /// <remarks>
        /// <b>FindChild</b> compares the specified <paramref name="key"/> to the <see
        /// cref="Center"/> point to determine the containing quadrant.</remarks>

        internal QuadTreeNode<TValue> FindChild(PointD key) {
            Debug.Assert(Bounds.Contains(key));

            double relX = key.X - Center.X;
            double relY = key.Y - Center.Y;

            return (relX < 0 ?
                (relY < 0 ? _topLeft : _bottomLeft) :
                (relY < 0 ? _topRight : _bottomRight));
        }

        #endregion
        #region FindOrCreateChild

        /// <summary>
        /// Finds the child node of the <see cref="QuadTreeNode{TValue}"/> that contains the
        /// specified key, creating the child node if necessary.</summary>
        /// <param name="key">
        /// The key to locate. This argument must lie within <see cref="Bounds"/>.</param>
        /// <returns>
        /// The child of the <see cref="QuadTreeNode{TValue}"/> that contains <paramref
        /// name="key"/>. The child is created if it does not yet exist.</returns>
        /// <remarks>
        /// <b>FindOrCreateChild</b> compares the specified <paramref name="key"/> to the <see
        /// cref="Center"/> point to determine the containing quadrant.</remarks>

        internal QuadTreeNode<TValue> FindOrCreateChild(PointD key) {
            Debug.Assert(Bounds.Contains(key));

            double relX = key.X - Center.X;
            double relY = key.Y - Center.Y;

            if (relX < 0) {
                if (relY < 0) {
                    if (_topLeft == null)
                        _topLeft = CreateChild(0, 0);
                    return _topLeft;
                } else {
                    if (_bottomLeft == null)
                        _bottomLeft = CreateChild(0, 1);
                    return _bottomLeft;
                }
            } else {
                if (relY < 0) {
                    if (_topRight == null)
                        _topRight = CreateChild(1, 0);
                    return _topRight;
                } else {
                    if (_bottomRight == null)
                        _bottomRight = CreateChild(1, 1);
                    return _bottomRight;
                }
            }
        }

        #endregion
        #region FindRange

        /// <summary>
        /// Finds all key-and-value pairs within the specified key range that are stored in the <see
        /// cref="QuadTreeNode{TValue}"/> or its child nodes.</summary>
        /// <param name="range">
        /// A <see cref="RectD"/> indicating the key range to search. This argument must intersect
        /// with <see cref="Bounds"/>, and it must be a square if <paramref name="useCircle"/> is
        /// <c>true</c>.</param>
        /// <param name="useCircle">
        /// <c>true</c> to search only the circle inscribed within <paramref name="range"/>;
        /// <c>false</c> to search the entire <paramref name="range"/>.</param>
        /// <param name="output">
        /// A <see cref="Dictionary{PointD, TValue}"/> to which any elements are added whose key
        /// lies within the specified <paramref name="range"/>.</param>

        internal void FindRange(ref RectD range, bool useCircle, Dictionary<PointD, TValue> output) {
            Debug.Assert(Bounds.IntersectsWith(range));

            if (_data != null) {
                if (useCircle) {
                    double radius = range.Width / 2;
                    double x = range.X + radius, y = range.Y + radius;

                    foreach (var pair in _data) {
                        if (range.Contains(pair.Key)) {
                            double dx = pair.Key.X - x, dy = pair.Key.Y - y;
                            if (dx * dx + dy * dy <= radius * radius)
                                output.Add(pair.Key, pair.Value);
                        }
                    }
                } else {
                    foreach (var pair in _data)
                        if (range.Contains(pair.Key))
                            output.Add(pair.Key, pair.Value);
                }
                return;
            }

            bool topRange = (range.Top < Center.Y);
            bool bottomRange = (range.Bottom >= Center.Y);
            bool leftRange = (range.Left < Center.X);
            bool rightRange = (range.Right >= Center.X);

            if (topRange) {
                if (leftRange && _topLeft != null)
                    _topLeft.FindRange(ref range, useCircle, output);
                if (rightRange && _topRight != null)
                    _topRight.FindRange(ref range, useCircle, output);
            }

            if (bottomRange) {
                if (leftRange && _bottomLeft != null)
                    _bottomLeft.FindRange(ref range, useCircle, output);
                if (rightRange && _bottomRight != null)
                    _bottomRight.FindRange(ref range, useCircle, output);
            }
        }

        #endregion
        #region RemoveChild

        /// <summary>
        /// Removes the specified child node from the <see cref="QuadTreeNode{TValue}"/>.</summary>
        /// <param name="child">
        /// The value of the <see cref="TopLeft"/>, <see cref="TopRight"/>, <see
        /// cref="BottomLeft"/>, or <see cref="BottomRight"/> property.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="child"/> does not equal one of the four child nodes.</exception>
        /// <exception cref="NullReferenceException">
        /// <paramref name="child"/> is a null reference.</exception>
        /// <remarks>
        /// <b>RemoveChild</b> recursively removes the current instance from its <see
        /// cref="Parent"/> if the specified <paramref name="child"/> was its last valid child. If
        /// <see cref="Parent"/> is a null reference, <b>RemoveChild</b> recreates an empty <see
        /// cref="Data"/> collection instead.</remarks>

        internal void RemoveChild(QuadTreeNode<TValue> child) {

            // remove specified child node
            if (child == _topLeft) {
                Tree._nodes.Remove(_topLeft.Signature);
                _topLeft = null;
            }
            else if (child == _topRight) {
                Tree._nodes.Remove(_topRight.Signature);
                _topRight = null;
            }
            else if (child == _bottomLeft) {
                Tree._nodes.Remove(_bottomLeft.Signature);
                _bottomLeft = null;
            }
            else if (child == _bottomRight) {
                Tree._nodes.Remove(_bottomRight.Signature);
                _bottomRight = null;
            } else
                ThrowHelper.ThrowArgumentException("child", Strings.ArgumentNotInCollection);

            // remove empty node if all children removed
            if (_topLeft == null && _bottomLeft == null &&
                _topRight == null && _bottomRight == null) {

                // root reverts to leaf node instead
                if (Parent == null)
                    _data = new DictionaryEx<PointD, TValue>(Tree.Capacity);
                else
                    Parent.RemoveChild(this);
            }
        }

        #endregion
        #region Split

        /// <summary>
        /// Splits the <see cref="QuadTreeNode{TValue}"/> into child nodes.</summary>
        /// <remarks>
        /// <b>Split</b> transfers all <see cref="Data"/> of the current instance to newly created
        /// children. Children that would receive no <see cref="Data"/> are not created.</remarks>

        internal void Split() {

            foreach (var pair in _data) {
                QuadTreeNode<TValue> child = FindOrCreateChild(pair.Key);
                child._data.Add(pair);
            }

            _data = null;
        }

        #endregion
        #endregion
        #region CreateChild

        /// <summary>
        /// Creates the indicated child node of the <see cref="QuadTreeNode{TValue}"/>.</summary>
        /// <param name="deltaX">
        /// The offset for the <see cref="GridX"/> coordinate on the next <see cref="Level"/>. This
        /// value must be zero or one.</param>
        /// <param name="deltaY">
        /// The offset for the <see cref="GridY"/> coordinate on the next <see cref="Level"/>. This
        /// value must be zero or one.</param>
        /// <returns>
        /// A new <see cref="QuadTreeNode{TValue}"/> that is the child of the current instance and
        /// has the indicated <see cref="Bounds"/> and grid coordinates.</returns>

        private QuadTreeNode<TValue> CreateChild(int deltaX, int deltaY) {

            Debug.Assert(deltaX == 0 || deltaX == 1);
            Debug.Assert(deltaY == 0 || deltaY == 1);

            // compute grid coordinates for child node
            int x = (GridX << 1) + deltaX;
            int y = (GridY << 1) + deltaY;

            // compose signature from level and grid coordinates
            int signature = unchecked((Level + 1) | (x << 4) | (y << 18));

            // compute bounding rectangle for child node
            double left, top, width, height;
            if (deltaX == 0) {
                left = Bounds.Left;
                width = Center.X - left;
            } else {
                left = Center.X;
                width = Bounds.Right - left;
            }
            if (deltaY == 0) {
                top = Bounds.Top;
                height = Center.Y - top;
            } else {
                top = Center.Y;
                height = Bounds.Bottom - top;
            }

            return new QuadTreeNode<TValue>(Tree, signature,
                this, new RectD(left, top, width, height));
        }

        #endregion
    }
}
