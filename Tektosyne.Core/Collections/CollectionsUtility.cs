using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides auxiliary methods for <b>System.Collections</b>.</summary>
    /// <remarks>
    /// <b>CollectionsUtility</b> methods usually omit explicit argument checking for performance
    /// reasons if the error case would result in identical or similar exceptions within the method
    /// body.</remarks>

    public static class CollectionsUtility {
        #region AnyRandom<T>

        /// <summary>
        /// Determines whether any element of a generic <see cref="IList{T}"/> collection satisfies
        /// a condition, starting with a random element.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection containing all elements.</param>
        /// <param name="predicate">
        /// The <see cref="Predicate{T}"/> to test on one or more randomly selected <paramref
        /// name="list"/> elements.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="predicate"/> succeeded for any one <paramref
        /// name="list"/> element; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> or <paramref name="predicate"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>AnyRandom</b> first selects a random element within the specified <paramref
        /// name="list"/>, using the <see cref="MersenneTwister.Default"/> instance of the <see
        /// cref="MersenneTwister"/> class.
        /// </para><para>
        /// <b>AnyRandom</b> then tests the specified <paramref name="predicate"/> on the selected
        /// element. As long as the result is <c>false</c>, <b>AnyRandom</b> tests <paramref
        /// name="predicate"/> on each remaining <paramref name="list"/> element in turn.
        /// </para><para>
        /// <b>AnyRandom</b> returns <c>true</c> as soon as <paramref name="predicate"/> succeeds
        /// for any one element, and <c>false</c> if <paramref name="predicate"/> has failed for
        /// every <paramref name="list"/> element.</para></remarks>

        public static bool AnyRandom<T>(this IList<T> list, Predicate<T> predicate) {

            int count = list.Count;
            int index = MersenneTwister.Default.Next(0, count - 1);

            for (int i = index; i < index + count; i++)
                if (predicate(list[i % count]))
                    return true;

            return false;
        }

        #endregion
        #region ChangeKey<TKey, TValue>(IDictionary<TKey, TValue>, TKey, TKey)

        /// <overloads>
        /// Changes all occurrences of the specified key in a collection.</overloads>
        /// <summary>
        /// Changes the specified key in an <see cref="IDictionary{TKey, TValue}"/> collection to
        /// another key.</summary>
        /// <typeparam name="TKey">
        /// The type of all keys in the collection.</typeparam>
        /// <typeparam name="TValue">
        /// The type of all values in the collection.</typeparam>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> collection whose keys to change.</param>
        /// <param name="oldKey">
        /// The key to remove from <paramref name="dictionary"/>.</param>
        /// <param name="newKey">
        /// The key to store in <paramref name="dictionary"/> with the value of the <paramref
        /// name="oldKey"/> element.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dictionary"/>, <paramref name="oldKey"/>, or <paramref name="newKey"/>
        /// is a null reference.</exception>
        /// <remarks><para>
        /// <b>ChangeKey</b> removes the element with the specified <paramref name="oldKey"/> from
        /// the specified <paramref name="dictionary"/>, and then adds the element’s old <see
        /// cref="KeyValuePair{TKey, TValue}.Value"/> back with the specified <paramref
        /// name="newKey"/>, effectively changing the <see cref="KeyValuePair{TKey, TValue}.Key"/>
        /// component of an element.
        /// </para><para>
        /// If the <see cref="KeyValuePair{TKey, TValue}.Value"/> component of a changed element
        /// implements the <see cref="IMutableKeyedValue{TKey}"/> interface, its <see
        /// cref="IKeyedValue{T}.Key"/> property is also set to <paramref name="newKey"/>.
        /// </para><para>
        /// If the specified <paramref name="oldKey"/> does not exist in the <paramref
        /// name="dictionary"/>, <b>ChangeKey</b> silently adds an element with <paramref
        /// name="newKey"/> and the default value for <typeparamref name="TValue"/>.
        /// </para><para>
        /// If the specified <paramref name="newKey"/> already exists in the <paramref
        /// name="dictionary"/>, it is silently overwritten.</para></remarks>

        public static void ChangeKey<TKey, TValue>(
            IDictionary<TKey, TValue> dictionary, TKey oldKey, TKey newKey) {

            // retrieve value for old key
            TValue value = dictionary[oldKey];

            // update Key property if supported
            IMutableKeyedValue<TKey> keyedValue = value as IMutableKeyedValue<TKey>;
            if (keyedValue != null) keyedValue.SetKey(newKey);

            // remove old pair, add new pair
            dictionary.Remove(oldKey);
            dictionary[newKey] = value;
        }

        #endregion
        #region ChangeKey<TKey, TValue>(IList<T>, TKey, TKey)

        /// <summary>
        /// Changes all occurrences of the specified key in an <see cref="IList{T}"/> collection to
        /// another key.</summary>
        /// <typeparam name="TKey">
        /// The type of all keys in the collection.</typeparam>
        /// <typeparam name="TValue">
        /// The type of all values in the collection.</typeparam>
        /// <param name="list">
        /// An <see cref="IList{T}"/> collection containing the <see cref="KeyValuePair{TKey,
        /// TValue}"/> elements whose keys to change.</param>
        /// <param name="oldKey">
        /// The key to remove from <paramref name="list"/>.</param>
        /// <param name="newKey">
        /// The key to store in <paramref name="list"/> with the values of all <paramref
        /// name="oldKey"/> elements.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/>, <paramref name="oldKey"/>, or <paramref name="newKey"/> is a
        /// null reference.</exception>
        /// <remarks><para>
        /// <b>ChangeKey</b> changes all occurrences of the specified <paramref name="oldKey"/> in
        /// the <see cref="KeyValuePair{TKey, TValue}.Key"/> components of the specified <paramref
        /// name="list"/> to the specified <paramref name="newKey"/>. The index positions and <see
        /// cref="KeyValuePair{TKey, TValue}.Value"/> components of the elements containing
        /// <paramref name="oldKey"/> remain unchanged.
        /// </para><para>
        /// If the <see cref="KeyValuePair{TKey, TValue}.Value"/> component of a changed element
        /// implements the <see cref="IMutableKeyedValue{TKey}"/> interface, its <see
        /// cref="IKeyedValue{T}.Key"/> property is also set to <paramref name="newKey"/>.
        /// </para><para>
        /// If the specified <paramref name="oldKey"/> does not exist in the <paramref
        /// name="list"/>, <b>ChangeKey</b> silently adds an element with <paramref name="newKey"/>
        /// and the default value for <typeparamref name="TValue"/>.</para></remarks>

        public static void ChangeKey<TKey, TValue>(
            IList<KeyValuePair<TKey, TValue>> list, TKey oldKey, TKey newKey) {

            if (oldKey == null)
                ThrowHelper.ThrowArgumentNullException("oldKey");
            if (newKey == null)
                ThrowHelper.ThrowArgumentNullException("newKey");

            // change all matching elements to new key
            bool found = false;
            var comparer = ComparerCache<TKey>.EqualityComparer;

            for (int i = 0; i < list.Count; i++) {
                KeyValuePair<TKey, TValue> pair = list[i];
                if (comparer.Equals(oldKey, pair.Key)) {

                    // update Key property if supported
                    var keyedValue = pair.Value as IMutableKeyedValue<TKey>;
                    if (keyedValue != null) keyedValue.SetKey(newKey);

                    // store new key with current value
                    list[i] = new KeyValuePair<TKey, TValue>(newKey, pair.Value);
                    found = true;
                }
            }

            // add key with null value if not found
            if (!found)
                list.Add(new KeyValuePair<TKey, TValue>(newKey, default(TValue)));
        }

        #endregion
        #region ConditionalSwap<T>

        /// <summary>
        /// Conditionally swaps two elements in the specified <see cref="IList{T}"/> collection,
        /// based on the specified <see cref="Comparison{T}"/> method.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection whose elements to swap.</param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method that defines the sorting order.</param>
        /// <param name="first">
        /// The zero-based index of the first element to swap.</param>
        /// <param name="second">
        /// The zero-based index of the second element to swap.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> or <paramref name="comparison"/> is a null reference.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="first"/> or <paramref name="second"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="first"/> or <paramref name="second"/> is equal to or greater than the
        /// <see cref="ICollection{T}.Count"/> of the specified <paramref name="list"/>.
        /// </para></exception>
        /// <remarks>
        /// <b>ConditionalSwap</b> compares the elements at the <paramref name="first"/> and
        /// <paramref name="second"/> indices within the specified <paramref name="list"/> using the
        /// specified <paramref name="comparison"/>, and swaps them exactly if the <paramref
        /// name="first"/> element compares greater than the <paramref name="second"/> element.
        /// </remarks>

        public static void ConditionalSwap<T>(this IList<T> list,
            Comparison<T> comparison, int first, int second) {

            T firstItem = list[first];
            T secondItem = list[second];

            if (comparison(firstItem, secondItem) > 0) {
                list[first] = secondItem;
                list[second] = firstItem;
            }
        }

        #endregion
        #region CountKey<TKey, TValue>(IDictionary<TKey, TValue>, TKey)
        
        /// <overloads>
        /// Counts the all occurrences of the specified key in a collection.</overloads>
        /// <summary>
        /// Counts the occurrences of the specified key in an <see cref="IDictionary{TKey,
        /// TValue}"/> collection.</summary>
        /// <typeparam name="TKey">
        /// The type of all keys in the collection.</typeparam>
        /// <typeparam name="TValue">
        /// The type of all values in the collection.</typeparam>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> collection whose keys to count.</param>
        /// <param name="key">
        /// The key to count in <paramref name="dictionary"/>.</param>
        /// <returns>
        /// One if <paramref name="key"/> was found in <paramref name="dictionary"/>; otherwise,
        /// zero.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dictionary"/> or <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// <b>CountKey</b> returns the number of occurrences of the specified <paramref
        /// name="key"/> in the specified <paramref name="dictionary"/>. By definition, this number
        /// is either zero or one. The <paramref name="dictionary"/> remains unchanged.</remarks>

        public static int CountKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key) {
            return (dictionary.ContainsKey(key) ? 1 : 0);
        }

        #endregion
        #region CountKey<TKey, TValue>(IList<T>, TKey)
    
        /// <summary>
        /// Counts the occurrences of the specified key in an <see cref="IList{T}"/> collection.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of all keys in the collection.</typeparam>
        /// <typeparam name="TValue">
        /// The type of all values in the collection.</typeparam>
        /// <param name="list">
        /// An <see cref="IList{T}"/> collection containing the <see cref="KeyValuePair{TKey,
        /// TValue}"/> elements whose keys to count.</param>
        /// <param name="key">
        /// The key to count in <paramref name="list"/>.</param>
        /// <returns>
        /// The number of occurrences of <paramref name="key"/> in <paramref name="list"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> or <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// <b>CountKey</b> returns the number of occurrences of the specified <paramref
        /// name="key"/> in the specified <paramref name="list"/>. This number can be greater than
        /// one because <see cref="IList{T}"/> does not require unique <see cref="KeyValuePair{TKey,
        /// TValue}.Key"/> values. The <paramref name="list"/> remains unchanged.</remarks>

        public static int CountKey<TKey, TValue>(IList<KeyValuePair<TKey, TValue>> list, TKey key) {
            if (key == null)
                ThrowHelper.ThrowArgumentNullException("key");

            int count = 0;
            var comparer = ComparerCache<TKey>.EqualityComparer;

            // count elements with specified key
            for (int i = 0; i < list.Count; i++)
                if (comparer.Equals(key, list[i].Key))
                    ++count;

            return count;
        }

        #endregion
        #region DeleteKey<TKey, TValue>(IDictionary<TKey, TValue>, TKey)

        /// <overloads>
        /// Deletes all elements with the specified key from a collection.</overloads>
        /// <summary>
        /// Deletes the element with the specified key from an <see cref="IDictionary{TKey,
        /// TValue}"/> collection.</summary>
        /// <typeparam name="TKey">
        /// The type of all keys in the collection.</typeparam>
        /// <typeparam name="TValue">
        /// The type of all values in the collection.</typeparam>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> collection whose element to delete.</param>
        /// <param name="key">
        /// The key of the element to delete from <paramref name="dictionary"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dictionary"/> or <paramref name="key"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>DeleteKey</b> deletes the element with the specified <paramref name="key"/> by
        /// invoking <see cref="IDictionary{TKey, TValue}.Remove"/> on the specified <paramref
        /// name="dictionary"/>.
        /// </para><para>
        /// This method is provided only for symmetry with the <see cref="IList{T}"/> overload of
        /// <b>DeleteKey</b>.</para></remarks>

        public static void DeleteKey<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key) {
            dictionary.Remove(key);
        }

        #endregion
        #region DeleteKey<TKey, TValue>(IList<T>, TKey)
    
        /// <summary>
        /// Deletes all elements with the specified key from an <see cref="IList{T}"/> collection.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of all keys in the collection.</typeparam>
        /// <typeparam name="TValue">
        /// The type of all values in the collection.</typeparam>
        /// <param name="list">
        /// An <see cref="IList{T}"/> collection containing the <see cref="KeyValuePair{TKey,
        /// TValue}"/> elements to delete.</param>
        /// <param name="key">
        /// The key of the elements to delete from <paramref name="list"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> or <paramref name="key"/> is a null reference.</exception>
        /// <remarks>
        /// <b>DeleteKey</b> deletes all elements with the specified <paramref name="key"/> from the
        /// specified <paramref name="list"/>.</remarks>

        public static void DeleteKey<TKey, TValue>(
            IList<KeyValuePair<TKey, TValue>> list, TKey key) {

            if (key == null)
                ThrowHelper.ThrowArgumentNullException("key");

            var comparer = ComparerCache<TKey>.EqualityComparer;

            // remove all elements with specified key
            for (int i = list.Count - 1; i >= 0; i--)
                if (comparer.Equals(key, list[i].Key))
                    list.RemoveAt(i);
        }

        #endregion
        #region IndexArray

        /// <summary>
        /// Creates an <see cref="Array"/> of zero-based <see cref="Int32"/> indices with the
        /// specified element count.</summary>
        /// <param name="count">
        /// The number of elements in the returned <see cref="Array"/>.</param>
        /// <returns>
        /// An <see cref="Array"/> containing all <see cref="Int32"/> numbers from zero to <paramref
        /// name="count"/> - 1, stored in ascending order.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> is less than zero.</exception>
        /// <remarks>
        /// The <see cref="Int32"/> value stored at each index position in an <b>IndexArray</b>
        /// equals the index itself, i.e. a[i] = i for all i.</remarks>

        public static int[] IndexArray(int count) {
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "count", count, Strings.ArgumentNegative);

            int[] array = new int[count];

            for (int i = 0; i < array.Length; i++)
                array[i] = i;

            return array;
        }

        #endregion
        #region MoveItem<T>

        /// <summary>
        /// Moves the specified element in a generic <see cref="IList{T}"/> collection by a specific
        /// offset.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection containing the element to move.</param>
        /// <param name="item">
        /// The element to move.</param>
        /// <param name="offset">
        /// The number of index positions by which to move the specified <paramref name="item"/>.
        /// </param>
        /// <returns>
        /// The new index position of <paramref name="item"/> if it was found in the specified
        /// <paramref name="list"/>; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> or <paramref name="item"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>MoveItem</b> removes the specified <paramref name="item"/> from the specified
        /// <paramref name="list"/> and adds it back at the specified <paramref name="offset"/> from
        /// its original index position.
        /// </para><para>
        /// The <paramref name="item"/> is placed at the beginning or at the end of the <paramref
        /// name="list"/> if the new position would be less than zero or higher than the last
        /// position, respectively. The items in-between are shifted up or down accordingly.
        /// </para></remarks>

        public static int MoveItem<T>(IList<T> list, T item, int offset) {
            if (item == null)
                ThrowHelper.ThrowArgumentNullException("item");

            // determine source item index
            int source = list.IndexOf(item);
            if (source < 0) return source;

            // compute target item index
            int target = Math.Max(source + offset, 0);
            target = Math.Min(target, list.Count - 1);
            if (source == target) return source;

            // remove and re-insert item
            list.RemoveAt(source);
            if (target == list.Count)
                list.Add(item);
            else
                list.Insert(target, item);

            // return new item index
            return target;
        }

        #endregion
        #region MoveItemUntyped

        /// <summary>
        /// Moves the specified element in a non-generic <see cref="IList"/> collection by a
        /// specific offset.</summary>
        /// <param name="list">
        /// The <see cref="IList"/> collection containing the element to move.</param>
        /// <param name="item">
        /// The element to move.</param>
        /// <param name="offset">
        /// The number of index positions by which to move the specified <paramref name="item"/>.
        /// </param>
        /// <returns>
        /// The new index position of <paramref name="item"/> if it was found in the specified
        /// <paramref name="list"/>; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> or <paramref name="item"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>MoveItemUntyped</b> removes the specified <paramref name="item"/> from the specified
        /// <paramref name="list"/> and adds it back at the specified <paramref name="offset"/> from
        /// its original index position.
        /// </para><para>
        /// The <paramref name="item"/> is placed at the beginning or at the end of the <paramref
        /// name="list"/> if the new position would be less than zero or higher than the last
        /// position, respectively. The items in-between are shifted up or down accordingly.
        /// </para></remarks>

        public static int MoveItemUntyped(IList list, object item, int offset) {
            if (item == null)
                ThrowHelper.ThrowArgumentNullException("item");

            // determine source item index
            int source = list.IndexOf(item);
            if (source < 0) return source;

            // compute target item index
            int target = Math.Max(source + offset, 0);
            target = Math.Min(target, list.Count - 1);
            if (source == target) return source;

            // remove and re-insert item
            list.RemoveAt(source);
            if (target == list.Count)
                list.Add(item);
            else
                list.Insert(target, item);

            // return new item index
            return target;
        }

        #endregion
        #region ProcessKey<TKey, TValue>(IDictionary<TKey, TValue>, TKey, TKey)
        
        /// <overloads>
        /// Counts, changes, or deletes all occurrences of the specified key.</overloads>
        /// <summary>
        /// Counts, changes, or deletes the element with the specified key in an <see
        /// cref="IDictionary{TKey, TValue}"/> collection.</summary>
        /// <typeparam name="TKey">
        /// The type of all keys in the collection.</typeparam>
        /// <typeparam name="TValue">
        /// The type of all values in the collection.</typeparam>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey, TValue}"/> collection whose elements to process.
        /// </param>
        /// <param name="oldKey">
        /// The key to count, change, or delete in <paramref name="dictionary"/>.</param>
        /// <param name="newKey"><para>
        /// The same value as <paramref name="oldKey"/> to count the occurrences of <paramref
        /// name="oldKey"/>.
        /// </para><para>-or-</para><para>
        /// A different value than <paramref name="oldKey"/> to change any occurrences of <paramref
        /// name="oldKey"/> to <paramref name="newKey"/>. 
        /// </para><para>-or-</para><para>
        /// A null reference to delete the element with <paramref name="oldKey"/>.</para></param>
        /// <returns>
        /// One if <paramref name="oldKey"/> was found in <paramref name="dictionary"/>; otherwise,
        /// zero.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dictionary"/> or <paramref name="oldKey"/> is a null reference.
        /// </exception>
        /// <remarks>
        /// <b>ProcessKey</b> passes its arguments to <see cref="CountKey"/> and then to <see
        /// cref="ChangeKey"/> or <see cref="DeleteKey"/>, depending on the value of the specified
        /// <paramref name="newKey"/>.</remarks>

        public static int ProcessKey<TKey, TValue>(
            IDictionary<TKey, TValue> dictionary, TKey oldKey, TKey newKey) {

            int count = CountKey<TKey, TValue>(dictionary, oldKey);

            if (count > 0) {
                if (newKey == null)
                    DeleteKey(dictionary, oldKey);
                else if (!ComparerCache<TKey>.EqualityComparer.Equals(newKey, oldKey))
                    ChangeKey(dictionary, oldKey, newKey);
            }

            return count;
        }

        #endregion
        #region ProcessKey<TKey, TValue>(IList<T>, TKey, TKey)
    
        /// <summary>
        /// Counts, changes, or deletes all occurrences of the specified key in an <see
        /// cref="IList{T}"/> collection.</summary>
        /// <typeparam name="TKey">
        /// The type of all keys in the collection.</typeparam>
        /// <typeparam name="TValue">
        /// The type of all values in the collection.</typeparam>
        /// <param name="list">
        /// An <see cref="IList{T}"/> collection containing the <see cref="KeyValuePair{TKey,
        /// TValue}"/> elements to process.</param>
        /// <param name="oldKey">
        /// The key to count, change, or delete in <paramref name="list"/>.</param>
        /// <param name="newKey"><para>
        /// The same value as <paramref name="oldKey"/> to count the occurrences of <paramref
        /// name="oldKey"/>.
        /// </para><para>-or-</para><para>
        /// A different value than <paramref name="oldKey"/> to change all occurrences of <paramref
        /// name="oldKey"/> to <paramref name="newKey"/>. 
        /// </para><para>-or-</para><para>
        /// A null reference to delete all elements with <paramref name="oldKey"/>.</para></param>
        /// <returns>
        /// The number of occurrences of <paramref name="oldKey"/> in <paramref name="list"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> or <paramref name="oldKey"/> is a null reference.</exception>
        /// <remarks>
        /// <b>ProcessKey</b> passes its arguments to <see cref="CountKey"/> and then to <see
        /// cref="ChangeKey"/>, or <see cref="DeleteKey"/>, depending on the value of the specified
        /// <paramref name="newKey"/>.</remarks>

        public static int ProcessKey<TKey, TValue>(
            IList<KeyValuePair<TKey, TValue>> list, TKey oldKey, TKey newKey) {

            int count = CountKey<TKey, TValue>(list, oldKey);

            if (count > 0) {
                if (newKey == null)
                    DeleteKey(list, oldKey);
                else if (!ComparerCache<TKey>.EqualityComparer.Equals(newKey, oldKey))
                    ChangeKey(list, oldKey, newKey);
            }

            return count;
        }

        #endregion
        #region ProcessKey<TKey, TValue>(KeyValuePair<TKey,v>, TKey, TKey)

        /// <summary>
        /// Counts, changes, or deletes the the specified key of a <see cref="KeyValuePair{TKey,
        /// TValue}"/>.</summary>
        /// <typeparam name="TKey">
        /// The type of all keys in the collection.</typeparam>
        /// <typeparam name="TValue">
        /// The type of all values in the collection.</typeparam>
        /// <param name="pair">
        /// The <see cref="KeyValuePair{TKey, TValue}"/> instance to process.</param>
        /// <param name="oldKey">
        /// The key to count, change, or delete in <paramref name="pair"/>.</param>
        /// <param name="newKey"><para>
        /// The same value as <paramref name="oldKey"/> to count the occurrence of <paramref
        /// name="oldKey"/>.
        /// </para><para>-or-</para><para>
        /// A different value than <paramref name="oldKey"/> to change <paramref name="oldKey"/> to
        /// <paramref name="newKey"/>. 
        /// </para><para>-or-</para><para>
        /// A null reference to delete <paramref name="oldKey"/> and its value.</para></param>
        /// <returns>
        /// One if <paramref name="pair"/> contains <paramref name="oldKey"/>; otherwise, zero.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="oldKey"/> is a null reference.</exception>
        /// <remarks>
        /// <b>ProcessKey</b> replaces the specified <paramref name="pair"/> with a
        /// default-initialized <see cref="KeyValuePair{TKey, TValue}"/> instance if its <see
        /// cref="KeyValuePair{TKey, TValue}.Key"/> equals <paramref name="oldKey"/> and the
        /// specified <paramref name="newKey"/> is a null reference.</remarks>

        public static int ProcessKey<TKey, TValue>(
            ref KeyValuePair<TKey, TValue> pair, TKey oldKey, TKey newKey) {

            if (!ComparerCache<TKey>.EqualityComparer.Equals(oldKey, pair.Key))
                return 0;

            if (newKey == null)
                pair = new KeyValuePair<TKey, TValue>();
            else if (!ComparerCache<TKey>.EqualityComparer.Equals(newKey, oldKey))
                pair = new KeyValuePair<TKey, TValue>(newKey, pair.Value);

            return 1;
        }

        #endregion
        #region Randomize<T>

        /// <summary>
        /// Randomizes the element order of the specified <see cref="IList{T}"/> collection.
        /// </summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="List{T}"/> whose element order to randomize.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks>
        /// <b>Randomize</b> creates a random permutation of the elements in the specified <paramref
        /// name="list"/>, using the <see cref="MersenneTwister.Default"/> instance of the <see
        /// cref="MersenneTwister"/> class to determine random indices. This is an O(n) operation
        /// where n is the number of <paramref name="list"/> elements.</remarks>

        public static void Randomize<T>(this IList<T> list) {
            if (list.Count < 2) return;
            T swap;

            // randomly swap items down to second item
            for (int i = list.Count - 1; i >= 2; i--) {
                int j = MersenneTwister.Default.Next(i - 1);
                swap = list[i]; list[i] = list[j]; list[j] = swap;
            }

            // swap original first item with randomized second item
            swap = list[1]; list[1] = list[0]; list[0] = swap;
        }

        #endregion
        #region Restrict<T>

        /// <summary>
        /// Restricts the specified <see cref="IList{T}"/> collection to those elements that are
        /// also present in another collection.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to restrict.</param>
        /// <param name="restriction">
        /// The <see cref="IList{T}"/> collection by which to restrict the specified <paramref
        /// name="list"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> or <paramref name="restriction"/> is a null reference.
        /// </exception>
        /// <remarks>
        /// <b>Restrict</b> removes all elements from the specified <paramref name="list"/> that are
        /// not also present in the specified <paramref name="restriction"/> collection.</remarks>

        public static void Restrict<T>(IList<T> list, IList<T> restriction) {

            // shortcut if restricting to nothing
            if (restriction.Count == 0) {
                list.Clear();
                return;
            }

            // remove elements not present in restriction
            for (int i = list.Count - 1; i >= 0; i--)
                if (!restriction.Contains(list[i])) {
                    list.RemoveAt(i);

                    // quit if no elements remain
                    if (list.Count == 0) return;
                }
        }

        #endregion
        #region SequenceEqual<T>(ICollection<T>)

        /// <overloads>
        /// Determines whether two specified generic collections contain the same elements in the
        /// same order.</overloads>
        /// <summary>
        /// Determines whether two specified generic <see cref="ICollection{T}"/> collections
        /// contain the same elements in the same order.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="firstCollection">
        /// The first <see cref="ICollection{T}"/> collection to compare.</param>
        /// <param name="secondCollection">
        /// The second <see cref="ICollection{T}"/> collection to compare.</param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="firstCollection"/> and <paramref name="secondCollection"/> are both null
        /// references.
        /// </item><item>
        /// <paramref name="firstCollection"/> and <paramref name="secondCollection"/> are both
        /// references to the same valid collection.
        /// </item><item>
        /// <paramref name="firstCollection"/> and <paramref name="secondCollection"/> are both
        /// valid collections that contain the same number of elements, and all elements compare as
        /// equal when retrieved in the enumeration sequence for each collection.
        /// </item></list></returns>
        /// <remarks><para>
        /// <b>SequenceEqual</b> first checks the two specified collections for reference equality,
        /// then for a different element <see cref="ICollection{T}.Count"/>; and only then compares
        /// all elements of <paramref name="firstCollection"/> and <paramref
        /// name="secondCollection"/>, using the default <see cref="EqualityComparer{T}"/> for
        /// <typeparamref name="T"/>, until a difference is found or the collections are exhausted.
        /// </para><para>
        /// Elements are compared by obtaining an <see cref="IEnumerator{T}"/> for each collection. 
        /// Both enumerators are disposed of before <b>SequenceEqual</b> returns.</para></remarks>

        public static bool SequenceEqual<T>(
            this ICollection<T> firstCollection, ICollection<T> secondCollection) {

            if (firstCollection == secondCollection)
                return true;

            if (firstCollection == null || secondCollection == null)
                return false;

            int count = firstCollection.Count;
            if (count != secondCollection.Count)
                return false;

            using (IEnumerator<T> firstEnum = firstCollection.GetEnumerator(),
                secondEnum = secondCollection.GetEnumerator()) {
                var comparer = ComparerCache<T>.EqualityComparer;

                for (int i = 0; i < count; i++) {
                    firstEnum.MoveNext(); secondEnum.MoveNext();
                    if (!comparer.Equals(firstEnum.Current, secondEnum.Current))
                        return false;
                }
            }

            return true;
        }

        #endregion
        #region SequenceEqual<T>(IList<T>)

        /// <summary>
        /// Determines whether two specified generic <see cref="IList{T}"/> collections contain the
        /// same elements in the same order.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="firstList">
        /// The first <see cref="IList{T}"/> collection to compare.</param>
        /// <param name="secondList">
        /// The second <see cref="IList{T}"/> collection to compare.</param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="firstList"/> and <paramref name="secondList"/> are both null references.
        /// </item><item>
        /// <paramref name="firstList"/> and <paramref name="secondList"/> are both references to
        /// the same valid collection.
        /// </item><item>
        /// <paramref name="firstList"/> and <paramref name="secondList"/> are both valid
        /// collections that contain the same number of elements, and all elements at the same index
        /// position in each collection compare as equal.</item></list></returns>
        /// <remarks><para>
        /// <b>SequenceEqual</b> first checks the two specified collections for reference equality,
        /// then for a different element <see cref="ICollection{T}.Count"/>; and only then compares
        /// all elements of <paramref name="firstList"/> and <paramref name="secondList"/>, using
        /// the default <see cref="EqualityComparer{T}"/> for <typeparamref name="T"/>, until a
        /// difference is found or the collections are exhausted.
        /// </para><para>
        /// Elements are compared by retrieiving the element at the same index position in each
        /// collection. No enumerators are created.</para></remarks>

        public static bool SequenceEqual<T>(this IList<T> firstList, IList<T> secondList) {
            if (firstList == secondList)
                return true;

            if (firstList == null || secondList == null)
                return false;

            int count = firstList.Count;
            if (count != secondList.Count)
                return false;

            var comparer = ComparerCache<T>.EqualityComparer;
            for (int i = 0; i < count; i++)
                if (!comparer.Equals(firstList[i], secondList[i]))
                    return false;

            return true;
        }

        #endregion
        #region SequenceEqualUntyped(ICollection)

        /// <overloads>
        /// Determines whether two specified non-generic collections contain the same elements in
        /// the same order.</overloads>
        /// <summary>
        /// Determines whether two specified non-generic <see cref="ICollection"/> collections
        /// contain the same elements in the same order.</summary>
        /// <param name="firstCollection">
        /// The first <see cref="ICollection"/> collection to compare.</param>
        /// <param name="secondCollection">
        /// The second <see cref="ICollection"/> collection to compare.</param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="firstCollection"/> and <paramref name="secondCollection"/> are both null
        /// references.
        /// </item><item>
        /// <paramref name="firstCollection"/> and <paramref name="secondCollection"/> are both
        /// references to the same valid collection.
        /// </item><item>
        /// <paramref name="firstCollection"/> and <paramref name="secondCollection"/> are both
        /// valid collections that contain the same number of elements, and all elements compare as
        /// equal when retrieved in the enumeration sequence for each collection.
        /// </item></list></returns>
        /// <remarks><para>
        /// <b>SequenceEqualUntyped</b> first checks the two specified collections for reference
        /// equality, then for a different element <see cref="ICollection.Count"/>; and only then
        /// compares all elements of <paramref name="firstCollection"/> and <paramref
        /// name="secondCollection"/>, using <see cref="Object.Equals"/>, until a difference is
        /// found or the collections are exhausted.
        /// </para><para>
        /// Elements are compared by obtaining an <see cref="IEnumerator"/> for each collection. Any
        /// enumerators that implement <see cref="IDisposable"/> are disposed of before
        /// <b>SequenceEqualUntyped</b> returns.</para></remarks>

        public static bool SequenceEqualUntyped(
            this ICollection firstCollection, ICollection secondCollection) {

            if (firstCollection == secondCollection)
                return true;

            if (firstCollection == null || secondCollection == null)
                return false;

            int count = firstCollection.Count;
            if (count != secondCollection.Count)
                return false;

            IEnumerator firstEnum = firstCollection.GetEnumerator();
            IEnumerator secondEnum = secondCollection.GetEnumerator();

            try {
                for (int i = 0; i < count; i++) {
                    firstEnum.MoveNext(); secondEnum.MoveNext();
                    object x = firstEnum.Current, y = secondEnum.Current;
                    if (!Object.Equals(x, y)) return false;
                }
            }
            finally {
                IDisposable disposable = firstEnum as IDisposable;
                if (disposable != null) disposable.Dispose();

                disposable = secondEnum as IDisposable;
                if (disposable != null) disposable.Dispose();
            }

            return true;
        }

        #endregion
        #region SequenceEqualUntyped(IList)

        /// <summary>
        /// Determines whether two specified non-generic <see cref="IList"/> collections contain the
        /// same elements in the same order.</summary>
        /// <param name="firstList">
        /// The first <see cref="IList"/> collection to compare.</param>
        /// <param name="secondList">
        /// The second <see cref="IList"/> collection to compare.</param>
        /// <returns><para>
        /// <c>true</c> under the following conditions, otherwise <c>false</c>:
        /// </para><list type="bullet"><item>
        /// <paramref name="firstList"/> and <paramref name="secondList"/> are both null references.
        /// </item><item>
        /// <paramref name="firstList"/> and <paramref name="secondList"/> are both references to
        /// the same valid collection.
        /// </item><item>
        /// <paramref name="firstList"/> and <paramref name="secondList"/> are both valid
        /// collections that contain the same number of elements, and all elements at the same index
        /// position in each collection compare as equal.</item></list></returns>
        /// <remarks><para>
        /// <b>SequenceEqualUntyped</b> first checks the two specified collections for reference
        /// equality, then for a different element <see cref="ICollection.Count"/>; and only then
        /// compares all elements of <paramref name="firstList"/> and <paramref name="secondList"/>,
        /// using <see cref="Object.Equals"/>, until a difference is found or the collections are
        /// exhausted.
        /// </para><para>
        /// Elements are compared by retrieiving the element at the same index position in each
        /// collection. No enumerators are created.</para></remarks>

        public static bool SequenceEqualUntyped(this IList firstList, IList secondList) {
            if (firstList == secondList)
                return true;

            if (firstList == null || secondList == null)
                return false;

            int count = firstList.Count;
            if (count != secondList.Count)
                return false;

            for (int i = 0; i < count; i++) {
                object x = firstList[i], y = secondList[i];
                if (!Object.Equals(x, y)) return false;
            }

            return true;
        }

        #endregion
        #region Swap<T>

        /// <summary>
        /// Swaps two elements in the specified <see cref="IList{T}"/> collection.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection whose elements to swap.</param>
        /// <param name="first">
        /// The zero-based index of the first element to swap.</param>
        /// <param name="second">
        /// The zero-based index of the second element to swap.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para>
        /// <paramref name="first"/> or <paramref name="second"/> is less than zero. 
        /// </para><para>-or-</para><para>
        /// <paramref name="first"/> or <paramref name="second"/> is equal to or greater than the
        /// <see cref="ICollection{T}.Count"/> of the specified <paramref name="list"/>.
        /// </para></exception>
        /// <remarks>
        /// <b>Swap</b> exchanges the elements at the <paramref name="first"/> and <paramref
        /// name="second"/> indices within the specified <paramref name="list"/>.</remarks>

        public static void Swap<T>(this IList<T> list, int first, int second) {
            T item = list[first];
            list[first] = list[second];
            list[second] = item;
        }

        #endregion
        #region ValidateKey<TKey, TValue>

        /// <summary>
        /// Validates the specified key and the embedded key, if any, of the specified value.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key.</typeparam>
        /// <typeparam name="TValue">
        /// The type of the value.</typeparam>
        /// <param name="key">
        /// The <typeparamref name="TKey"/> key to validate.</param>
        /// <param name="value">
        /// The <typeparamref name="TValue"/> value to validate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is a null reference.</exception>
        /// <exception cref="KeyMismatchException">
        /// <paramref name="value"/> is an <see cref="IKeyedValue{TKey}"/> instance whose <see
        /// cref="IKeyedValue{TKey}.Key"/> differs from <paramref name="key"/>.</exception>
        /// <remarks>
        /// <b>ValidateKey</b> is called by the generic dictionary classes in the 
        /// <b>Tektosyne.Collections</b> namespace to ensure that dictionary keys are stored with
        /// identical <see cref="IKeyedValue{TKey}"/> keys only.</remarks>

        public static void ValidateKey<TKey, TValue>(TKey key, TValue value) {
            if (key == null)
                ThrowHelper.ThrowArgumentNullException("key");

            IKeyedValue<TKey> keyedValue = value as IKeyedValue<TKey>;
            if (keyedValue != null && !ComparerCache<TKey>.
                EqualityComparer.Equals(key, keyedValue.Key))
                ThrowHelper.ThrowKeyMismatchException(key, keyedValue.Key);
        }

        #endregion
    }
}
