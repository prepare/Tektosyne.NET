using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tektosyne.Collections {

    /// <summary>
    /// Provides sorting algorithms for <see cref="IList{T}"/> collections.</summary>
    /// <remarks><para>
    /// <b>Sorting</b> provides a variety of standard sorting algorithms that operate on any <see
    /// cref="IList{T}"/> collection. The implementations are based on Robert Sedgewick, "Algorithms
    /// in Java" (3rd ed.), Addison-Wesley 2003.
    /// </para><para>
    /// All <b>Sorting</b> algorithms return immediately if the specified collection contains less
    /// than two elements. The relatively complex algorithms <see cref="Sorting.HeapSort"/>, <see
    /// cref="Sorting.QuickSort"/>, and <see cref="Sorting.BestQuickSort"/> also defer collections
    /// with exactly two elements to the <see cref="CollectionsUtility.ConditionalSwap"/> method,
    /// and use an optimal algorithm for collections with exactly three elements.</para></remarks>

    public static class Sorting {
        #region Private Methods
        #region BinarySearchCore<T>

        /// <summary>
        /// Searches the specified sorted <see cref="IList{T}"/> collection for the specified
        /// element, using the specified <see cref="Comparison{T}"/> method.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The sorted <see cref="IList{T}"/> collection to search.</param>
        /// <param name="value">
        /// The element to locate. This argument may be a null reference if <typeparamref name="T"/>
        /// is a reference type.</param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method that defines the sorting order.</param>
        /// <returns>
        /// The zero-based index of <paramref name="value"/> in <paramref name="list"/>, if found;
        /// otherwise, a negative number that is the bitwise complement of the index of the first
        /// element larger than <paramref name="value"/>, if any; otherwise, the bitwise complement
        /// of the <see cref="ICollection{T}.Count"/> of <paramref name="list"/>.</returns>
        /// <remarks>
        /// <b>BinarySearchCore</b> implements the binary search algorithm used by the <see
        /// cref="BinarySearch"/> and <see cref="BestBinarySearch"/> methods.</remarks>

        private static int BinarySearchCore<T>(IList<T> list, T value, Comparison<T> comparison) {

            int left = 0, right = list.Count - 1, middle = 0;

            while (left <= right) {
                middle = (left + right) / 2;
                int result = comparison(value, list[middle]);

                if (result == 0) return middle;
                if (result < 0) right = middle - 1;
                else left = middle + 1;
            }

            return ~Math.Max(left, middle);
        }

        #endregion
        #region HeapSortSink<T>

        /// <summary>
        /// Fixes a heap for <see cref="HeapSort{T}"/> by sinking the specified element in the
        /// specified <see cref="IList{T}"/> collection.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method that defines the sorting order.</param>
        /// <param name="k">
        /// The one-based index of the <paramref name="list"/> element to sink.</param>
        /// <param name="n">
        /// The greatest one-based index in <paramref name="list"/> to compare against.</param>
        /// <remarks>
        /// <see cref="HeapSort{T}"/> internally uses one-based indices which must be adjusted to
        /// zero-based indices whenever <paramref name="list"/> elements are retrieved.</remarks>

        private static void HeapSortSink<T>(IList<T> list, Comparison<T> comparison, int k, int n) {
            while (2 * k <= n) {

                int j = 2 * k;
                if (j < n && comparison(list[j - 1], list[j]) < 0)
                    ++j;

                T kItem = list[k - 1], jItem = list[j - 1];
                if (comparison(kItem, jItem) >= 0)
                    break;

                list[k - 1] = jItem; list[j - 1] = kItem;
                k = j;
            }
        }

        #endregion
        #region QuickSortRange<T>

        /// <summary>
        /// Sorts the specified subrange of the specified <see cref="IList{T}"/> collection, using
        /// the Quicksort algorithm and the specified <see cref="Comparison{T}"/> method.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method that defines the sorting order.</param>
        /// <param name="l">
        /// The zero-based index of the first <paramref name="list"/> element to sort.</param>
        /// <param name="r">
        /// The zero-based index of the last <paramref name="list"/> element to sort.</param>
        /// <remarks>
        /// <b>QuickSortRange</b> implements partitioning and recursive partition sorting for <see
        /// cref="QuickSort{T}"/>. The range between the specified <paramref name="l"/> and
        /// <paramref name="r"/> indices must contain at least two elements, although our
        /// implementation ensures that the range contains at least four elements.</remarks>

        private static void QuickSortRange<T>(
            IList<T> list, Comparison<T> comparison, int l, int r) {

            // guaranteed by entry and cutoff conditions
            Debug.Assert(r - l >= 3);

            T item = list[r];
            int i = l - 1, j = r, p = l - 1, q = r, k;

            while (true) {
                do { ++i; } while (comparison(list[i], item) < 0);
                do { --j; } while (j > l && comparison(item, list[j]) < 0);
                if (i >= j) break;

                list.Swap(i, j);
                if (comparison(list[i], item) == 0) {
                    ++p; list.Swap(p, i);
                }
                if (comparison(item, list[j]) == 0) {
                    --q; list.Swap(q, j);
                }
            }

            list.Swap(i, r);
            j = i - 1; i = i + 1;

            for (k = l; k <= p; k++, j--) list.Swap(k, j);
            for (k = r - 1; k >= q; k--, i++) list.Swap(k, i);

            // recurse only for four or more elements
            int d = j - l;
            if (d > 0) {
                if (d == 1)
                    list.ConditionalSwap(comparison, l, j);
                else if (d == 2)
                    TripleSort(list, comparison, l);
                else
                    QuickSortRange(list, comparison, l, j);
            }

            d = r - i;
            if (d > 0) {
                if (d == 1)
                    list.ConditionalSwap(comparison, i, r);
                else if (d == 2)
                    TripleSort(list, comparison, i);
                else
                    QuickSortRange(list, comparison, i, r);
            }
        }

        #endregion
        #region TripleSort<T>

        /// <summary>
        /// Sorts three elements in the specified <see cref="IList{T}"/> collection, starting at the
        /// specified index and using the specified <see cref="Comparison{T}"/> method.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method that defines the sorting order.</param>
        /// <param name="i">
        /// The zero-based index of the first <paramref name="list"/> element to sort.</param>
        /// <remarks>
        /// <b>TripleSort</b> sorts the three <paramref name="list"/> elements starting at index
        /// position <paramref name="i"/> using two or at most three comparisons. This method is
        /// also called by <see cref="QuickSortRange{T}"/> for subranges of exactly three elements.
        /// </remarks>

        private static void TripleSort<T>(IList<T> list, Comparison<T> comparison, int i) {

            int j = i + 1, k = i + 2;
            T first = list[i], second = list[j], third = list[k];

            if (comparison(first, second) > 0) {
                if (comparison(first, third) > 0) {
                    if (comparison(second, third) > 0) {
                        list[i] = third; list[k] = first;
                    } else {
                        list[i] = second; list[j] = third; list[k] = first;
                    }
                } else {
                    list[i] = second; list[j] = first;
                }
            }
            else if (comparison(first, third) > 0) {
                list[i] = third; list[j] = first; list[k] = second;
            }
            else if (comparison(second, third) > 0) {
                list[j] = third; list[k] = second;
            }
        }

        #endregion
        #endregion
        #region BinarySearch<T>(IList<T>, T)

        /// <overloads>
        /// Searches the specified sorted <see cref="IList{T}"/> collection for the specified
        /// element.</overloads>
        /// <summary>
        /// Searches the specified sorted <see cref="IList{T}"/> collection for the specified
        /// element.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The sorted <see cref="IList{T}"/> collection to search.</param>
        /// <param name="value">
        /// The element to locate. This argument may be a null reference if <typeparamref name="T"/>
        /// is a reference type.</param>
        /// <returns>
        /// The zero-based index of <paramref name="value"/> in <paramref name="list"/>, if found;
        /// otherwise, a negative number that is the bitwise complement of the index of the first
        /// element larger than <paramref name="value"/>, if any; otherwise, the bitwise complement
        /// of the <see cref="ICollection{T}.Count"/> of <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks>
        /// This method calls the <see cref="BinarySearch{T}(IList{T}, T, Comparison{T})"/> overload
        /// that also takes a <see cref="Comparison{T}"/> argument, specifying a null reference for
        /// the second argument. Please see there for details.</remarks>

        public static int BinarySearch<T>(this IList<T> list, T value) {
            return BinarySearch<T>(list, value, null);
        }

        #endregion
        #region BinarySearch<T>(IList<T>, T, Comparison<T>)

        /// <summary>
        /// Searches the specified sorted <see cref="IList{T}"/> collection for the specified
        /// element, using the specified <see cref="Comparison{T}"/> method.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The sorted <see cref="IList{T}"/> collection to search.</param>
        /// <param name="value">
        /// The element to locate. This argument may be a null reference if <typeparamref name="T"/>
        /// is a reference type.</param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method that defines the sorting order. This argument may
        /// be a null reference.</param>
        /// <returns>
        /// The zero-based index of <paramref name="value"/> in <paramref name="list"/>, if found;
        /// otherwise, a negative number that is the bitwise complement of the index of the first
        /// element larger than <paramref name="value"/>, if any; otherwise, the bitwise complement
        /// of the <see cref="ICollection{T}.Count"/> of <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>BinarySearch</b> searches the specified <paramref name="list"/> using a binary search
        /// algorithm. To obtain correct results, <paramref name="list"/> must be sorted according
        /// to <paramref name="comparison"/>.
        /// </para><para>
        /// If <paramref name="comparison"/> is a null reference, <b>BinarySearch</b> uses the <see
        /// cref="Comparer{T}.Compare"/> method provided by the <see cref="Comparer{T}.Default"/>
        /// comparer for the specified type <typeparamref name="T"/> instead.</para></remarks>

        public static int BinarySearch<T>(this IList<T> list, T value, Comparison<T> comparison) {
            if (list == null)
                ThrowHelper.ThrowArgumentNullException("list");

            if (comparison == null)
                comparison = ComparerCache<T>.Comparer.Compare;

            return BinarySearchCore<T>(list, value, comparison);
        }

        #endregion
        #region BestBinarySearch<T>(IList<T>, T)

        /// <overloads>
        /// Searches the specified sorted <see cref="IList{T}"/> collection for the specified
        /// element, using the best binary search algorithm available for its concrete type.
        /// </overloads>
        /// <summary>
        /// Searches the specified sorted <see cref="IList{T}"/> collection for the specified
        /// element, using the best binary search algorithm available for its concrete type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The sorted <see cref="IList{T}"/> collection to search.</param>
        /// <param name="value">
        /// The element to locate. This argument may be a null reference if <typeparamref name="T"/>
        /// is a reference type.</param>
        /// <returns>
        /// The zero-based index of <paramref name="value"/> in <paramref name="list"/>, if found;
        /// otherwise, a negative number that is the bitwise complement of the index of the first
        /// element larger than <paramref name="value"/>, if any; otherwise, the bitwise complement
        /// of the <see cref="ICollection{T}.Count"/> of <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks>
        /// This method calls the <see cref="BestBinarySearch{T}(IList{T}, T, Comparison{T})"/>
        /// overload that also takes a <see cref="Comparison{T}"/> argument, specifying a null
        /// reference for the second argument. Please see there for details.</remarks>

        public static int BestBinarySearch<T>(this IList<T> list, T value) {
            return BestBinarySearch<T>(list, value, null);
        }

        #endregion
        #region BestBinarySearch<T>(IList<T>, T value, Comparison<T>)

        /// <summary>
        /// Searches the specified sorted <see cref="IList{T}"/> collection for the specified
        /// element, using the specified <see cref="Comparison{T}"/> method and the best binary
        /// search algorithm available for its concrete type.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The sorted <see cref="IList{T}"/> collection to search.</param>
        /// <param name="value">
        /// The element to locate. This argument may be a null reference if <typeparamref name="T"/>
        /// is a reference type.</param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method that defines the sorting order. This argument may
        /// be a null reference.</param>
        /// <returns>
        /// The zero-based index of <paramref name="value"/> in <paramref name="list"/>, if found;
        /// otherwise, a negative number that is the bitwise complement of the index of the first
        /// element larger than <paramref name="value"/>, if any; otherwise, the bitwise complement
        /// of the <see cref="ICollection{T}.Count"/> of <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>BinarySearch</b> searches the specified <paramref name="list"/> using a binary search
        /// algorithm. To obtain correct results, <paramref name="list"/> must be sorted according
        /// to <paramref name="comparison"/>.
        /// </para><para>
        /// <b>BestBinarySearch</b> selects the fastest binary search algorithm that is available
        /// for the concrete type of the specified <paramref name="list"/>, as follows:
        /// </para><list type="number"><item>
        /// If <paramref name="list"/> is an <see cref="Array"/>, use the <see
        /// cref="Array.BinarySearch{T}"/> method of the <see cref="Array"/> class.
        /// </item><item>
        /// If <paramref name="list"/> is a standard <see cref="List{T}"/>, use the <see
        /// cref="List{T}.BinarySearch"/> method of the <see cref="List{T}"/> class.
        /// </item><item>
        /// If <paramref name="list"/> is a <see cref="ListEx{T}"/>, use the <see
        /// cref="ListEx{T}.BinarySearch"/> method of the <see cref="ListEx{T}"/> class.
        /// </item><item>
        /// Otherwise, use the <see cref="BinarySearch{T}"/> method.
        /// </item></list><para>
        /// If <paramref name="comparison"/> is a null reference, <b>BestBinarySearch</b> uses the
        /// <see cref="Comparer{T}.Compare"/> method provided by the <see
        /// cref="Comparer{T}.Default"/> comparer for the specified type <typeparamref name="T"/>
        /// instead.</para></remarks>

        public static int BestBinarySearch<T>(
            this IList<T> list, T value, Comparison<T> comparison) {

            if (list == null)
                ThrowHelper.ThrowArgumentNullException("list");

            IComparer<T> comparer;
            if (comparison == null) {
                comparer = ComparerCache<T>.Comparer;
                comparison = comparer.Compare;
            } else
                comparer = new ComparerAdapter<T>(comparison);

            T[] first = list as T[];
            if (first != null)
                return Array.BinarySearch<T>(first, value, comparer);

            List<T> second = list as List<T>;
            if (second != null)
                return second.BinarySearch(value, comparer);

            ListEx<T> third = list as ListEx<T>;
            if (third != null)
                return third.BinarySearch(value, comparer);

            return BinarySearchCore<T>(list, value, comparison);
        }
        
        #endregion
        #region HeapSort<T>(IList<T>)

        /// <overloads>
        /// Sorts the specified <see cref="IList{T}"/> collection using the Heapsort algorithm.
        /// </overloads>
        /// <summary>
        /// Sorts the specified <see cref="IList{T}"/> collection using the Heapsort algorithm.
        /// </summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks>
        /// This method calls the <see cref="HeapSort{T}(IList{T}, Comparison{T})"/> overload that
        /// also takes a <see cref="Comparison{T}"/> argument, specifying a null reference for the
        /// second argument. Please see there for details.</remarks>

        public static void HeapSort<T>(this IList<T> list) {
            HeapSort<T>(list, null);
        }

        #endregion
        #region HeapSort<T>(IList<T>, Comparison<T>)

        /// <summary>
        /// Sorts the specified <see cref="IList{T}"/> collection using the Heapsort algorithm and
        /// the specified <see cref="Comparison{T}"/> method.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method that defines the sorting order. This argument may
        /// be a null reference.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>HeapSort</b> sorts the specified <paramref name="list"/> using the Heapsort
        /// algorithm. Like the Quicksort algorithm provided by the <see cref="List{T}"/> class,
        /// this is an unstable algorithm. That is, the relative order of any two elements for which
        /// the specified <paramref name="comparison"/> returns zero may change randomly.
        /// </para><para>
        /// If <paramref name="comparison"/> is a null reference, <b>HeapSort</b> uses the <see
        /// cref="Comparer{T}.Compare"/> method provided by the <see cref="Comparer{T}.Default"/>
        /// comparer for the specified type <typeparamref name="T"/> instead.</para></remarks>

        public static void HeapSort<T>(this IList<T> list, Comparison<T> comparison) {
            if (list == null)
                ThrowHelper.ThrowArgumentNullException("list");

            int n = list.Count;
            if (n < 2) return;

            if (comparison == null)
                comparison = ComparerCache<T>.Comparer.Compare;

            if (n == 2)
                list.ConditionalSwap(comparison, 0, 1);
            else if (n == 3)
                TripleSort(list, comparison, 0);
            else {
                for (int k = n / 2; k >= 1; k--)
                    HeapSortSink(list, comparison, k, n);

                while (--n > 0) {
                    list.Swap(0, n);
                    HeapSortSink(list, comparison, 1, n);
                }
            }
        }

        #endregion
        #region InsertionSort<T>(IList<T>)

        /// <overloads>
        /// Sorts the specified <see cref="IList{T}"/> collection using the insertion sort
        /// algorithm.</overloads>
        /// <summary>
        /// Sorts the specified <see cref="IList{T}"/> collection using the insertion sort
        /// algorithm.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks>
        /// This method calls the <see cref="InsertionSort{T}(IList{T}, Comparison{T})"/> overload
        /// that also takes a <see cref="Comparison{T}"/> argument, specifying a null reference for
        /// the second argument. Please see there for details.</remarks>

        public static void InsertionSort<T>(this IList<T> list) {
            InsertionSort<T>(list, null);
        }

        #endregion
        #region InsertionSort<T>(IList<T>, Comparison<T>)

        /// <summary>
        /// Sorts the specified <see cref="IList{T}"/> collection using the insertion sort algorithm
        /// and the specified <see cref="Comparison{T}"/> method.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method that defines the sorting order. This argument may
        /// be a null reference.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>InsertionSort</b> sorts the specified <paramref name="list"/> using the insertion
        /// sort algorithm. Unlike the Quicksort algorithm provided by the <see cref="List{T}"/>
        /// class, this is a stable algorithm. That is, the relative order of any two elements for
        /// which the specified <paramref name="comparison"/> returns zero remains unchanged.
        /// </para><para>
        /// If <paramref name="comparison"/> is a null reference, <b>InsertionSort</b> uses the <see
        /// cref="Comparer{T}.Compare"/> method provided by the <see cref="Comparer{T}.Default"/>
        /// comparer for the specified type <typeparamref name="T"/> instead.</para></remarks>

        public static void InsertionSort<T>(this IList<T> list, Comparison<T> comparison) {
            if (list == null)
                ThrowHelper.ThrowArgumentNullException("list");

            int count = list.Count;
            if (count < 2) return;

            if (comparison == null)
                comparison = ComparerCache<T>.Comparer.Compare;

            for (int j = 1; j < count; j++) {
                T item = list[j];

                int i = j - 1;
                for (; i >= 0 && comparison(list[i], item) > 0; i--)
                    list[i + 1] = list[i];

                list[i + 1] = item;
            }
        }

        #endregion
        #region QuickSort<T>(IList<T>)

        /// <overloads>
        /// Sorts the specified <see cref="IList{T}"/> collection using the Quicksort algorithm.
        /// </overloads>
        /// <summary>
        /// Sorts the specified <see cref="IList{T}"/> collection using the Quicksort algorithm.
        /// </summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks>
        /// This method calls the <see cref="QuickSort{T}(IList{T}, Comparison{T})"/> overload that
        /// also takes a <see cref="Comparison{T}"/> argument, specifying a null reference for the
        /// second argument. Please see there for details.</remarks>

        public static void QuickSort<T>(this IList<T> list) {
            QuickSort<T>(list, null);
        }

        #endregion
        #region QuickSort<T>(IList<T>, Comparison<T>)

        /// <summary>
        /// Sorts the specified <see cref="IList{T}"/> collection using the Quicksort algorithm and
        /// the specified <see cref="Comparison{T}"/> method.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method that defines the sorting order. This argument may
        /// be a null reference.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>QuickSort</b> sorts the specified <paramref name="list"/> using the Quicksort
        /// algorithm. This algorithm is unstable; that is, the relative order of any two elements
        /// for which the specified <paramref name="comparison"/> returns zero may change randomly.
        /// </para><para>
        /// The standard library already provides a Quicksort algorithm, but only for <see
        /// cref="List{T}"/> and <see cref="Array"/> instances, whereas <b>QuickSort</b> can operate
        /// on arbitrary <see cref="IList{T}"/> collections.
        /// </para><para>
        /// If <paramref name="comparison"/> is a null reference, <b>QuickSort</b> uses the <see
        /// cref="Comparer{T}.Compare"/> method provided by the <see cref="Comparer{T}.Default"/>
        /// comparer for the specified type <typeparamref name="T"/> instead.</para></remarks>

        public static void QuickSort<T>(this IList<T> list, Comparison<T> comparison) {
            if (list == null)
                ThrowHelper.ThrowArgumentNullException("list");

            int count = list.Count;
            if (count < 2) return;

            if (comparison == null)
                comparison = ComparerCache<T>.Comparer.Compare;

            if (count == 2)
                list.ConditionalSwap(comparison, 0, 1);
            else if (count == 3)
                TripleSort(list, comparison, 0);
            else
                QuickSortRange(list, comparison, 0, count - 1);
        }

        #endregion
        #region BestQuickSort<T>(IList<T>)

        /// <overloads>
        /// Sorts the specified <see cref="IList{T}"/> collection using the best Quicksort algorithm
        /// available for its concrete type.</overloads>
        /// <summary>
        /// Sorts the specified <see cref="IList{T}"/> collection using the best Quicksort algorithm
        /// available for its concrete type.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks>
        /// This method calls the <see cref="BestQuickSort{T}(IList{T}, Comparison{T})"/> overload
        /// that also takes a <see cref="Comparison{T}"/> argument, specifying a null reference for
        /// the second argument. Please see there for details.</remarks>

        public static void BestQuickSort<T>(this IList<T> list) {
            BestQuickSort<T>(list, null);
        }

        #endregion
        #region BestQuickSort<T>(IList<T>, Comparison<T>)

        /// <summary>
        /// Sorts the specified <see cref="IList{T}"/> collection, using the specified <see
        /// cref="Comparison{T}"/> method and the best Quicksort algorithm available for its
        /// concrete type.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method that defines the sorting order. This argument may
        /// be a null reference.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>BestQuickSort</b> sorts the specified <paramref name="list"/> using the Quicksort
        /// algorithm. This algorithm is unstable; that is, the relative order of any two elements
        /// for which the specified <paramref name="comparison"/> returns zero may change randomly.
        /// </para><para>
        /// <b>BestQuickSort</b> selects the fastest Quicksort algorithm that is available for the
        /// concrete type of the specified <paramref name="list"/>, as follows:
        /// </para><list type="number"><item>
        /// If <paramref name="list"/> is an <see cref="Array"/>, use the <see
        /// cref="Array.Sort{T}"/> method of the <see cref="Array"/> class.
        /// </item><item>
        /// If <paramref name="list"/> is a standard <see cref="List{T}"/>, use the <see
        /// cref="List{T}.Sort"/> method of the <see cref="List{T}"/> class.
        /// </item><item>
        /// If <paramref name="list"/> is a <see cref="ListEx{T}"/>, use the <see
        /// cref="ListEx{T}.Sort"/> method of the <see cref="ListEx{T}"/> class.
        /// </item><item>
        /// Otherwise, use the <see cref="QuickSort{T}"/> method.
        /// </item></list><para>
        /// If <paramref name="comparison"/> is a null reference, <b>BestQuickSort</b> uses the <see
        /// cref="Comparer{T}.Compare"/> method provided by the <see cref="Comparer{T}.Default"/>
        /// comparer for the specified type <typeparamref name="T"/> instead.</para></remarks>

        public static void BestQuickSort<T>(this IList<T> list, Comparison<T> comparison) {
            if (list == null)
                ThrowHelper.ThrowArgumentNullException("list");

            int count = list.Count;
            if (count < 2) return;

            if (comparison == null)
                comparison = ComparerCache<T>.Comparer.Compare;

            if (count == 2) {
                list.ConditionalSwap(comparison, 0, 1);
                return;
            }

            if (count == 3) {
                TripleSort(list, comparison, 0);
                return;
            }

            T[] first = list as T[];
            if (first != null) {
                Array.Sort<T>(first, comparison);
                return;
            }

            List<T> second = list as List<T>;
            if (second != null) {
                second.Sort(comparison);
                return;
            }

            ListEx<T> third = list as ListEx<T>;
            if (third != null) {
                third.Sort(comparison);
                return;
            }

            QuickSortRange(list, comparison, 0, count - 1);
        }

        #endregion
        #region ShellSort<T>(IList<T>)

        /// <overloads>
        /// Sorts the specified <see cref="IList{T}"/> collection using the Shellsort algorithm.
        /// </overloads>
        /// <summary>
        /// Sorts the specified <see cref="IList{T}"/> collection using the Shellsort algorithm.
        /// </summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks>
        /// This method calls the <see cref="ShellSort{T}(IList{T}, Comparison{T})"/> overload that
        /// also takes a <see cref="Comparison{T}"/> argument, specifying a null reference for the
        /// second argument. Please see there for details.</remarks>

        public static void ShellSort<T>(this IList<T> list) {
            ShellSort<T>(list, null);
        }

        #endregion
        #region ShellSort<T>(IList<T>, Comparison<T>)

        /// <summary>
        /// Sorts the specified <see cref="IList{T}"/> collection using the Shellsort algorithm and
        /// the specified <see cref="Comparison{T}"/> method.</summary>
        /// <typeparam name="T">
        /// The type of all elements in the collection.</typeparam>
        /// <param name="list">
        /// The <see cref="IList{T}"/> collection to sort.</param>
        /// <param name="comparison">
        /// The <see cref="Comparison{T}"/> method that defines the sorting order. This argument may
        /// be a null reference.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>ShellSort</b> sorts the specified <paramref name="list"/> using the Shellsort
        /// algorithm. Unlike the Quicksort algorithm provided by the <see cref="List{T}"/> class,
        /// this is a stable algorithm. That is, the relative order of any two elements for which
        /// the specified <paramref name="comparison"/> returns zero remains unchanged.
        /// </para><para>
        /// If <paramref name="comparison"/> is a null reference, <b>ShellSort</b> uses the <see
        /// cref="Comparer{T}.Compare"/> method provided by the <see cref="Comparer{T}.Default"/>
        /// comparer for the specified type <typeparamref name="T"/> instead.</para></remarks>

        public static void ShellSort<T>(this IList<T> list, Comparison<T> comparison) {
            if (list == null)
                ThrowHelper.ThrowArgumentNullException("list");

            int r = list.Count - 1;
            if (r < 1) return;

            if (comparison == null)
                comparison = ComparerCache<T>.Comparer.Compare;

            // determine increment sequence
            int h = 1;
            while (h <= r / 9) h = 3 * h + 1;

            for (; h > 0; h /= 3) {
                for (int i = h; i <= r; i++) {
                    T item = list[i];

                    int j = i;
                    while (j >= h && comparison(item, list[j - h]) < 0) {
                        list[j] = list[j - h];
                        j -= h;
                    }

                    list[j] = item;
                }
            }
        }

        #endregion
    }
}
