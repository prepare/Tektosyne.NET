using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tektosyne.Collections;

namespace Tektosyne.UnitTest.Collections {

    [TestFixture]
    public class SortingTest {

        [Test]
        public void BinarySearch() {

            const int size = 100, step = 10;
            var array = new int[size];
            for (int i = 0; i < array.Length; i++)
                array[i] = step * i;

            BinarySearchTest(array, step);

            var list = new List<Int32>(array);
            BinarySearchTest(list, step);

            var listEx = new ListEx<Int32>(array);
            BinarySearchTest(listEx, step);
        }

        private void BinarySearchTest(IList<Int32> list, int step) {
            int[] array = list as int[];
            int offset, hit, miss, count = list.Count;

            for (int i = 0; i < count; i++) {
                offset = MersenneTwister.Default.Next(1, step - 1);
                hit = step * i; miss = hit - offset;

                // reference result from Array.BinarySearch
                if (array != null) {
                    Assert.AreEqual(i, Array.BinarySearch(array, hit));
                    Assert.AreEqual(-i - 1, Array.BinarySearch(array, miss));
                }

                // bitwise complement (~i) equals (-i - 1)
                Assert.AreEqual(i, Sorting.BinarySearch(list, hit));
                Assert.AreEqual(-i - 1, Sorting.BinarySearch(list, miss));

                Assert.AreEqual(i, Sorting.BestBinarySearch(list, hit));
                Assert.AreEqual(-i - 1, Sorting.BestBinarySearch(list, miss));
            }

            offset = MersenneTwister.Default.Next(step - 1);
            miss = step * count + offset;

            if (array != null) Assert.AreEqual(-count - 1, Array.BinarySearch(array, miss));
            Assert.AreEqual(-count - 1, Sorting.BinarySearch(list, miss));
            Assert.AreEqual(-count - 1, Sorting.BestBinarySearch(list, miss));
        }

        [Test]
        public void HeapSort() {

            SortAndCompare(new String[] { }, new String[] { }, Sorting.HeapSort);
            SortAndCompare(new String[] { "One" }, new String[] { "One" }, Sorting.HeapSort);
            SortAndCompare(new String[] { "One", "Two" }, new String[] { "Two", "One" }, Sorting.HeapSort);

            // case-sensitive sorting changes order of "Two" and "two"
            // (case-insensitive sorting results in undefined order)
            SortAndCompare(new String[] { "One", "two", "Two" },
                new String[] { "Two", "two", "One" }, Sorting.HeapSort);

            RandomSort(Sorting.HeapSort);
        }

        [Test]
        public void InsertionSort() {

            SortAndCompare(new String[] { }, new String[] { }, Sorting.InsertionSort);
            SortAndCompare(new String[] { "One" }, new String[] { "One" }, Sorting.InsertionSort);
            SortAndCompare(new String[] { "One", "Two" }, new String[] { "Two", "One" }, Sorting.InsertionSort);

            // case-sensitive sorting changes order of "Two" and "two"
            SortAndCompare(new String[] { "One", "two", "Two" },
                new String[] { "Two", "two", "One" }, Sorting.InsertionSort);

            // case-insensitive sorting preserves order of "Two" and "two"
            SortAndCompare<String>(new String[] { "One", "Two", "two" },
                new String[] { "Two", "two", "One" },
                Sorting.InsertionSort, StringComparer.OrdinalIgnoreCase.Compare);

            RandomSort(Sorting.InsertionSort);
        }

        [Test]
        public void QuickSort() {

            SortAndCompare(new String[] { }, new String[] { }, Sorting.QuickSort);
            SortAndCompare(new String[] { "One" }, new String[] { "One" }, Sorting.QuickSort);
            SortAndCompare(new String[] { "One", "Two" }, new String[] { "Two", "One" }, Sorting.QuickSort);

            // case-sensitive sorting changes order of "Two" and "two"
            // (case-insensitive sorting results in undefined order)
            SortAndCompare(new String[] { "One", "two", "Two" },
                new String[] { "Two", "two", "One" }, Sorting.QuickSort);

            RandomSort(Sorting.QuickSort);
        }

        [Test]
        public void BestQuickSort() {

            SortAndCompare(new String[] { }, new String[] { }, Sorting.BestQuickSort);
            SortAndCompare(new String[] { "One" }, new String[] { "One" }, Sorting.BestQuickSort);
            SortAndCompare(new String[] { "One", "Two" }, new String[] { "Two", "One" }, Sorting.BestQuickSort);

            // case-sensitive sorting changes order of "Two" and "two"
            // (case-insensitive sorting results in undefined order)
            SortAndCompare(new String[] { "One", "two", "Two" },
                new String[] { "Two", "two", "One" }, Sorting.BestQuickSort);

            RandomSort(Sorting.BestQuickSort);
        }

        [Test]
        public void ShellSort() {

            SortAndCompare(new String[] { }, new String[] { }, Sorting.ShellSort);
            SortAndCompare(new String[] { "One" }, new String[] { "One" }, Sorting.ShellSort);
            SortAndCompare(new String[] { "One", "Two" }, new String[] { "Two", "One" }, Sorting.ShellSort);

            // case-sensitive sorting changes order of "Two" and "two"
            SortAndCompare(new String[] { "One", "two", "Two" },
                new String[] { "Two", "two", "One" }, Sorting.ShellSort);

            // case-insensitive sorting preserves order of "Two" and "two"
            SortAndCompare<String>(new String[] { "One", "Two", "two" },
                new String[] { "Two", "two", "One" },
                Sorting.ShellSort, StringComparer.OrdinalIgnoreCase.Compare);

            RandomSort(Sorting.ShellSort);
        }

        private void RandomSort(Action<IList<Int32>> action) {
            Random random = new Random();
            
            int[] array = new int[1000];
            for (int i = 0; i < array.Length; i++)
                array[i] = random.Next(1000);

            action(array);
            for (int i = 0; i < array.Length - 1; i++)
                Assert.IsTrue(array[i] <= array[i+1]);
        }

        private void SortAndCompare<T>(IList<T> first, IList<T> second, Action<IList<T>> action) {
            action(second);
            CollectionAssert.AreEqual(first, second);
        }

        private void SortAndCompare<T>(IList<T> first, IList<T> second,
            Action<IList<T>, Comparison<T>> action, Comparison<T> comparison) {
            action(second, comparison);
            CollectionAssert.AreEqual(first, second);
        }
    }
}
