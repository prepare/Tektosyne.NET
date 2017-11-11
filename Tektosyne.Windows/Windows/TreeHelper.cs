using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides auxiliary methods for navigating the logical and visual trees of
    /// <b>System.Windows</b> objects.</summary>

    public static class TreeHelper {
        #region FindContextMenuTarget

        /// <summary>
        /// Finds the <see cref="ContextMenu.PlacementTarget"/> for the <see cref="ContextMenu"/>
        /// that contains the specified object.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> that is part of the logical tree rooted in the <see
        /// cref="ContextMenu"/>, such as a clicked <see cref="MenuItem"/>.</param>
        /// <returns><para>
        /// The <see cref="ContextMenu.PlacementTarget"/> of the <see cref="ContextMenu"/> that
        /// contains <paramref name="obj"/> as part of its logical tree.
        /// </para><para>-or-</para><para>
        /// A null reference if <paramref name="obj"/> is not part of any <see cref="ContextMenu"/>.
        /// </para></returns>
        /// <remarks>
        /// To find the <see cref="UIElement"/> that owns an active <see cref="ContextMenu"/>, call
        /// <b>FindContextMenuTarget</b> with the <see cref="MenuItem"/> that sent the current <see
        /// cref="MenuItem.Click"/> event. Note that this assumes that the <see
        /// cref="ContextMenu.PlacementTarget"/> was not manually changed.</remarks>

        public static UIElement FindContextMenuTarget(object obj) {
            ContextMenu menu = FindLogicalParent<ContextMenu>(obj as DependencyObject);
            return (menu == null ? null : menu.PlacementTarget);
        }

        #endregion
        #region FindLogicalChild<T>

        /// <summary>
        /// Finds the nearest logical child of the specified type that is contained by the specified
        /// <see cref="DependencyObject"/>.</summary>
        /// <typeparam name="T">
        /// The type of the logical child to find.</typeparam>
        /// <param name="obj">
        /// A <see cref="DependencyObject"/> that is the parent of the logical tree containing the
        /// child.</param>
        /// <returns><para>
        /// The nearest child of type <typeparamref name="T"/> that is part of the logical tree
        /// rooted in the specified <paramref name="obj"/>.
        /// </para><para>-or-</para><para>
        /// A null reference if no such child was found.</para></returns>
        /// <remarks><para>
        /// <b>FindLogicalChild</b> calls <see cref="LogicalTreeHelper.GetChildren"/> to perform a
        /// breadth-first search of the logical tree from the specified <paramref name="obj"/> until
        /// either a child of type <typeparamref name="T"/> is found, or the tree has ended.
        /// </para><para>
        /// <b>FindLogicalChild</b> returns a null reference if <paramref name="obj"/> is a null
        /// reference, and <paramref name="obj"/> itself if its type is <typeparamref name="T"/>.
        /// </para></remarks>

        public static T FindLogicalChild<T>(DependencyObject obj) where T: DependencyObject {

            var queue = new Queue<DependencyObject>();
            if (obj != null) queue.Enqueue(obj);

            while (queue.Count > 0) {
                obj = queue.Dequeue();

                T child = obj as T;
                if (child != null) return child;

                foreach (object objChild in LogicalTreeHelper.GetChildren(obj)) {
                    DependencyObject depChild = objChild as DependencyObject;
                    if (depChild != null) queue.Enqueue(depChild);
                }
            }

            return null;
        }

        #endregion
        #region FindLogicalParent<T>

        /// <summary>
        /// Finds the nearest logical parent of the specified type that contains the specified <see
        /// cref="DependencyObject"/>.</summary>
        /// <typeparam name="T">
        /// The type of the logical parent to find.</typeparam>
        /// <param name="obj">
        /// A <see cref="DependencyObject"/> that is part of the logical tree rooted in the parent.
        /// </param>
        /// <returns><para>
        /// The nearest parent of type <typeparamref name="T"/> that contains the specified
        /// <paramref name="obj"/> as part of its logical tree.
        /// </para><para>-or-</para><para>
        /// A null reference if no such parent was found.</para></returns>
        /// <remarks><para>
        /// <b>FindLogicalParent</b> calls <see cref="LogicalTreeHelper.GetParent"/> to traverse the
        /// logical tree from the specified <paramref name="obj"/> until either a parent of type 
        /// <typeparamref name="T"/> is found, or the tree has ended.
        /// </para><para>
        /// <b>FindLogicalParent</b> returns a null reference if <paramref name="obj"/> is a null
        /// reference, and <paramref name="obj"/> itself if its type is <typeparamref name="T"/>.
        /// </para></remarks>

        public static T FindLogicalParent<T>(DependencyObject obj) where T: DependencyObject {

            while (obj != null) {
                T parent = obj as T;
                if (parent != null) return parent;
                obj = LogicalTreeHelper.GetParent(obj);
            }

            return null;
        }

        #endregion
        #region FindParentListItem

        /// <summary>
        /// Finds the <see cref="ListBoxItem"/> that contains the specified <see cref="Object"/>.
        /// </summary>
        /// <param name="obj">
        /// An <see cref="Object"/> that is part of the visual tree rooted in the <see
        /// cref="ListBoxItem"/> to find.</param>
        /// <returns><para>
        /// The nearest <see cref="ListBoxItem"/> that contains the specified <paramref name="obj"/>
        /// as part of its visual tree.
        /// </para><para>-or-</para><para>
        /// A null reference if no such <see cref="ListBoxItem"/> was found.</para></returns>
        /// <remarks><para>
        /// <b>FindParentListItem</b> returns the result of <see
        /// cref="FindVisualParent{ListBoxItem}"/> with the specified <paramref name="obj"/> cast to
        /// type <see cref="DependencyObject"/>, or a null reference if that cast fails.
        /// </para><para>
        /// <b>FindParentListItem</b> is a convenient shortcut for the frequent task of locating the
        /// <see cref="ListBoxItem"/> or <see cref="ListViewItem"/> that contains the untyped <see
        /// cref="RoutedEventArgs.Source"/> of a <see cref="RoutedEvent"/>.</para></remarks>

        public static ListBoxItem FindParentListItem(object obj) {
            return FindVisualParent<ListBoxItem>(obj as DependencyObject);
        }

        #endregion
        #region FindVisualChild<T>

        /// <summary>
        /// Finds the nearest visual child of the specified type that is contained by the specified
        /// <see cref="DependencyObject"/>.</summary>
        /// <typeparam name="T">
        /// The type of the visual child to find.</typeparam>
        /// <param name="obj">
        /// A <see cref="DependencyObject"/> that is the parent of the visual tree containing the
        /// child.</param>
        /// <returns><para>
        /// The nearest child of type <typeparamref name="T"/> that is part of the visual tree
        /// rooted in the specified <paramref name="obj"/>.
        /// </para><para>-or-</para><para>
        /// A null reference if no such child was found.</para></returns>
        /// <remarks><para>
        /// <b>FindVisualChild</b> calls <see cref="VisualTreeHelper.GetChild"/> to perform a
        /// breadth-first search of the visual tree from the specified <paramref name="obj"/> until
        /// either a child of type <typeparamref name="T"/> is found, or the tree has ended.
        /// </para><para>
        /// <b>FindVisualChild</b> returns a null reference if <paramref name="obj"/> is a null
        /// reference, and <paramref name="obj"/> itself if its type is <typeparamref name="T"/>.
        /// </para></remarks>

        public static T FindVisualChild<T>(DependencyObject obj) where T: DependencyObject {

            var queue = new Queue<DependencyObject>();
            if (obj != null) queue.Enqueue(obj);

            while (queue.Count > 0) {
                obj = queue.Dequeue();

                T child = obj as T;
                if (child != null) return child;

                if (obj is Visual || obj is Visual3D) {
                    int count = VisualTreeHelper.GetChildrenCount(obj);
                    for (int i = 0; i < count; i++)
                        queue.Enqueue(VisualTreeHelper.GetChild(obj, i));
                }
            }

            return null;
        }

        #endregion
        #region FindVisualParent<T>

        /// <summary>
        /// Finds the nearest visual parent of the specified type that contains the specified <see
        /// cref="DependencyObject"/>.</summary>
        /// <typeparam name="T">
        /// The type of the visual parent to find.</typeparam>
        /// <param name="obj">
        /// A <see cref="DependencyObject"/> that is part of the visual tree rooted in the parent.
        /// </param>
        /// <returns><para>
        /// The nearest parent of type <typeparamref name="T"/> that contains the specified
        /// <paramref name="obj"/> as part of its visual tree.
        /// </para><para>-or-</para><para>
        /// A null reference if no such parent was found.</para></returns>
        /// <remarks><para>
        /// <b>FindVisualParent</b> calls <see cref="VisualTreeHelper.GetParent"/> to traverse the
        /// visual tree from the specified <paramref name="obj"/> until either a parent of type 
        /// <typeparamref name="T"/> is found, or the tree has ended.
        /// </para><para>
        /// <b>FindVisualParent</b> returns a null reference if <paramref name="obj"/> is a null
        /// reference, and <paramref name="obj"/> itself if its type is <typeparamref name="T"/>.
        /// </para><para>
        /// <b>FindVisualParent</b> ends the search upon encountering an object whose type is not
        /// <see cref="Visual"/> or <see cref="Visual3D"/>, as the <see cref="VisualTreeHelper"/>
        /// class cannot process any other types.</para></remarks>

        public static T FindVisualParent<T>(DependencyObject obj) where T: DependencyObject {

            while (obj != null) {
                T parent = obj as T;
                if (parent != null) return parent;

                if (obj is Visual || obj is Visual3D)
                    obj = VisualTreeHelper.GetParent(obj);
                else
                    break;
            }

            return null;
        }

        #endregion
        #region GetLogicalChildren<T>

        /// <summary>
        /// Finds all logical children of the specified type that are contained by the specified
        /// <see cref="DependencyObject"/>.</summary>
        /// <typeparam name="T">
        /// The type of the logical children to find.</typeparam>
        /// <param name="obj">
        /// A <see cref="DependencyObject"/> that is the parent of the logical tree containing the
        /// children.</param>
        /// <returns>
        /// A <see cref="List{T}"/> containing all children of type <typeparamref name="T"/> that
        /// are part of the logical tree rooted in the specified <paramref name="obj"/>.</returns>
        /// <remarks><para>
        /// <b>GetLogicalChildren</b> calls <see cref="LogicalTreeHelper.GetChildren"/> to perform a
        /// breadth-first search of the logical tree from the specified <paramref name="obj"/> to
        /// find all children of type <typeparamref name="T"/>.
        /// </para><para>
        /// <b>GetLogicalChildren</b> returns an empty collection if <paramref name="obj"/> is a
        /// null reference, and <paramref name="obj"/> itself as the first collection element if its
        /// type is <typeparamref name="T"/>.</para></remarks>

        public static List<T> GetLogicalChildren<T>(DependencyObject obj) where T: DependencyObject {

            List<T> children = new List<T>();
            var queue = new Queue<DependencyObject>();
            if (obj != null) queue.Enqueue(obj);

            while (queue.Count > 0) {
                obj = queue.Dequeue();

                T child = obj as T;
                if (child != null) children.Add(child);

                foreach (object objChild in LogicalTreeHelper.GetChildren(obj)) {
                    DependencyObject depChild = objChild as DependencyObject;
                    if (depChild != null) queue.Enqueue(depChild);
                }
            }

            return children;
        }

        #endregion
        #region GetVisualChildren<T>

        /// <summary>
        /// Finds all visual children of the specified type that are contained by the specified <see
        /// cref="DependencyObject"/>.</summary>
        /// <typeparam name="T">
        /// The type of the visual children to find.</typeparam>
        /// <param name="obj">
        /// A <see cref="DependencyObject"/> that is the parent of the visual tree containing the
        /// children.</param>
        /// <returns>
        /// A <see cref="List{T}"/> containing all children of type <typeparamref name="T"/> that
        /// are part of the visual tree rooted in the specified <paramref name="obj"/>.</returns>
        /// <remarks><para>
        /// <b>GetVisualChildren</b> calls <see cref="VisualTreeHelper.GetChild"/> to perform a
        /// breadth-first search of the visual tree from the specified <paramref name="obj"/> to
        /// find all children of type <typeparamref name="T"/>.
        /// </para><para>
        /// <b>GetVisualChildren</b> returns an empty collection if <paramref name="obj"/> is a null
        /// reference, and <paramref name="obj"/> itself as the first collection element if its type
        /// is <typeparamref name="T"/>.</para></remarks>

        public static List<T> GetVisualChildren<T>(DependencyObject obj) where T: DependencyObject {

            List<T> children = new List<T>();
            var queue = new Queue<DependencyObject>();
            if (obj != null) queue.Enqueue(obj);

            while (queue.Count > 0) {
                obj = queue.Dequeue();

                T child = obj as T;
                if (child != null) children.Add(child);

                if (obj is Visual || obj is Visual3D) {
                    int count = VisualTreeHelper.GetChildrenCount(obj);
                    for (int i = 0; i < count; i++)
                        queue.Enqueue(VisualTreeHelper.GetChild(obj, i));
                }
            }

            return children;
        }

        #endregion
    }
}
