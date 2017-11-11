using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using Tektosyne.Collections;
using Tektosyne.Geometry;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> for benchmarking several Tektosyne algorithms.</summary>
    /// <remarks>
    /// <b>BenchmarkDialog</b> runs a selected benchmark suite in a background thread and appends
    /// any new results to a scrollable <see cref="TextBox"/>.</remarks>

    public partial class BenchmarkDialog: Window {
        #region BenchmarkDialog()

        public BenchmarkDialog() {
            InitializeComponent();
        }

        #endregion
        #region Private Fields

        private Thread _thread;
        private Exception _threadException;

        private static readonly TestCase[] _sortingTestCases =
        {
            new TestCase() { Name = "Insert", Sort = Sorting.InsertionSort },
            new TestCase() { Name = "Shell",  Sort = Sorting.ShellSort },
            new TestCase() { Name = "Heap",   Sort = Sorting.HeapSort },
            new TestCase() { Name = "Quick",  Sort = Sorting.QuickSort },
            new TestCase() { Name = "Array",  Sort = Sorting.BestQuickSort }
        };

        private static readonly TestCase[] _collectionTestCases =
        {
            new TestCase() { Name = "HashSet",    Collection = new HashSet<Int32>() },
            new TestCase() { Name = "IntHashSet", Collection = new Int32HashSet() }
        };

        private static readonly TestCase[] _dictionaryTestCases =
        {
            new TestCase() { Name = "BraidedTree",  Dictionary = new BraidedTree<Int32, String>() },
            new TestCase() { Name = "RedBlackTree", Dictionary = new SortedDictionary<Int32, String>() },
            new TestCase() { Name = "Hashtable",    Dictionary = new Dictionary<Int32, String>() },
            new TestCase() { Name = "IntHashtable", Dictionary = new Int32Dictionary<String>() }
        };

        private static readonly TestCase[] _geometryTestCases =
        {
            new TestCase() { Name = "ConvexHull", FindPoints = p => { GeoAlgorithms.ConvexHull(p); } },
            new TestCase() { Name = "Voronoi",    FindPoints = p => { Voronoi.FindAll(p); } },
            new TestCase() { Name = "Delaunay",   FindPoints = p => { Voronoi.FindDelaunay(p); } }
        };

        private static readonly TestCase[] _nearestPointTestCases = 
        {
            new TestCase() { Name = "Unsorted",
                FindPointIndex = (c, p, q) => GeoAlgorithms.NearestPoint(p, q) },
            new TestCase() { Name = "Sorted",
                FindPointIndex = (c, p, q) => c.FindNearest(p, q) }
        };

        private static readonly TestCase[] _intersectionTestCases =
        {
            new TestCase() { Name = "SweepLine",  FindLines = MultiLineIntersection.Find },
            new TestCase() { Name = "BruteForce", FindLines = MultiLineIntersection.FindSimple },
        };

        private static readonly TestCase[] _subdivisionTestCases = 
        {
            new TestCase() { Name = "0%–100%", Value = 0.0 },
            new TestCase() { Name = "10%–90%", Value = 0.1 },
            new TestCase() { Name = "50%–50%", Value = 0.5 },
            new TestCase() { Name = "90%–10%", Value = 0.9 },
            new TestCase() { Name = "100%–0%", Value = 1.0 }
        };

        private static readonly TestCase[] _rangeTreeTestCases =
        {
            new TestCase() { Name = "BraidedTree",
                RangeTree = new BraidedTree<PointD, String>(PointDComparerY.CompareExact), },
            new TestCase() { Name = "QuadTree",
                RangeTree = new QuadTree<String>(new RectD(0, 0, 10000, 10000)), }
        };

        private static readonly TestCase[] _subdivSearchTestCases =
        {
            new TestCase() { Name = "BruteForce" },
            new TestCase() { Name = "Ordered" },
            new TestCase() { Name = "Randomized" }
        };

        #endregion
        #region CollectionTest

        private void CollectionTest() {
            Stopwatch timer = new Stopwatch();
            var testCases = _collectionTestCases;

            Output(String.Format("{0,8}", " "));
            foreach (TestCase test in testCases)
                Output(String.Format("{0,14}", test.Name));
            Output("\n");

            // count units of size x operation in milliseconds,
            // rather than individual operations in microseconds
            const int outerLoop = 100, size = 200000;
            const int iterations = outerLoop * 1000;

            long[] addTicks = new long[testCases.Length],
                iterateTicks = new long[testCases.Length],
                searchTicks = new long[testCases.Length],
                removeTicks = new long[testCases.Length];

            // generate random permutation of items
            int[] array = CollectionsUtility.IndexArray(size);
            CollectionsUtility.Randomize(array);

            // trigger JIT compilation
            foreach (TestCase test in testCases) {
                test.Collection.Clear();
                foreach (int item in array)
                    test.Collection.Add(item);
            }

            for (int i = 0; i < outerLoop; i++) {
                for (int j = 0; j < testCases.Length; j++) {
                    TestCase test = testCases[j];
                    test.Collection.Clear();

                    timer.Restart();
                    foreach (int item in array)
                        test.Collection.Add(item);
                    timer.Stop();
                    addTicks[j] += timer.ElapsedTicks;

                    int sum = 0;
                    timer.Restart();
                    foreach (int item in test.Collection)
                        unchecked { sum += item; }
                    timer.Stop();
                    iterateTicks[j] += timer.ElapsedTicks;

                    int key = MersenneTwister.Default.Next(size - 1);
                    timer.Restart();
                    for (int k = 0; k < size; k++)
                        test.Collection.Contains((key + k) % size);
                    timer.Stop();
                    searchTicks[j] += timer.ElapsedTicks;

                    timer.Restart();
                    foreach (int item in array)
                        test.Collection.Remove(item);
                    timer.Stop();
                    removeTicks[j] += timer.ElapsedTicks;
                }
            }

            Output(String.Format("{0,8}", "Add"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(addTicks[i], iterations)));

            Output(String.Format("\n{0,8}", "Iterate"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(iterateTicks[i], iterations)));

            Output(String.Format("\n{0,8}", "Search"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(searchTicks[i], iterations)));

            Output(String.Format("\n{0,8}", "Remove"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(removeTicks[i], iterations)));

            Output(String.Format(
                "\n\nTimes are msec averages for {0:N0} integer items in random order.\n", size));
        }

        #endregion
        #region DictionaryTest

        private void DictionaryTest(bool random) {
            Stopwatch timer = new Stopwatch();
            var testCases = _dictionaryTestCases;

            Output(String.Format("{0,8}", " "));
            foreach (TestCase test in testCases)
                Output(String.Format("{0,14}", test.Name));
            Output("\n");

            // count units of size x operation in milliseconds,
            // rather than individual operations in microseconds
            const int outerLoop = 100, size = 60000;
            const int iterations = outerLoop * 1000;

            long[] addTicks = new long[testCases.Length],
                iterateTicks = new long[testCases.Length],
                searchTicks = new long[testCases.Length],
                removeTicks = new long[testCases.Length];

            // generate keys in ascending order
            var array = new KeyValuePair<Int32, String>[size];
            for (int i = 0; i < array.Length; i++)
                array[i] = new KeyValuePair<Int32, String>(i, null);

            // create random permutation of keys
            if (random) CollectionsUtility.Randomize(array);

            // trigger JIT compilation
            foreach (TestCase test in testCases) {
                test.Dictionary.Clear();
                foreach (var pair in array)
                    test.Dictionary.Add(pair.Key, pair.Value);
            }

            for (int i = 0; i < outerLoop; i++) {
                for (int j = 0; j < testCases.Length; j++) {
                    TestCase test = testCases[j];
                    test.Dictionary.Clear();

                    timer.Restart();
                    foreach (var pair in array)
                        test.Dictionary.Add(pair.Key, pair.Value);
                    timer.Stop();
                    addTicks[j] += timer.ElapsedTicks;

                    int sum = 0;
                    timer.Restart();
                    foreach (var pair in test.Dictionary)
                        unchecked { sum += pair.Key; }
                    timer.Stop();
                    iterateTicks[j] += timer.ElapsedTicks;

                    int key = MersenneTwister.Default.Next(size - 1);
                    timer.Restart();
                    for (int k = 0; k < size; k++)
                        test.Dictionary.ContainsKey((key + k) % size);
                    timer.Stop();
                    searchTicks[j] += timer.ElapsedTicks;

                    timer.Restart();
                    foreach (var pair in array)
                        test.Dictionary.Remove(pair.Key);
                    timer.Stop();
                    removeTicks[j] += timer.ElapsedTicks;
                }
            }

            Output(String.Format("{0,8}", "Add"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(addTicks[i], iterations)));

            Output(String.Format("\n{0,8}", "Iterate"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(iterateTicks[i], iterations)));

            Output(String.Format("\n{0,8}", "Search"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(searchTicks[i], iterations)));

            Output(String.Format("\n{0,8}", "Remove"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(removeTicks[i], iterations)));

            Output(String.Format("\n\nTimes are msec averages for {0:N0} integer keys in {1} order.\n",
                size, (random ? "random" : "ascending")));
        }

        #endregion
        #region GeometryBasicTest

        private void GeometryBasicTest() {
            Stopwatch timer = new Stopwatch();
            long polyTicks = 0, polyEpsilonTicks = 0, lineTicks = 0, lineEpsilonTicks = 0;

            const double epsilon = 1e-10;
            const int outerLoop = 10000, innerLoop = 1000;
            const int iterations = outerLoop * innerLoop;

            for (int i = 0; i < outerLoop; i++) {
                PointD[] polygon = GeoAlgorithms.RandomPolygon(0, 0, 1000, 1000);
                LineD line = GeoAlgorithms.RandomLine(0, 0, 1000, 1000);
                LineD line2 = GeoAlgorithms.RandomLine(0, 0, 1000, 1000);
                PointD q = GeoAlgorithms.RandomPoint(0, 0, 1000, 1000);

                // trigger JIT compilation
                if (i == 0) {
                    GeoAlgorithms.PointInPolygon(q, polygon);
                    GeoAlgorithms.PointInPolygon(q, polygon, epsilon);
                    line.Intersect(line2);
                    line.Intersect(line2, epsilon);
                }

                timer.Restart();
                for (int j = 0; j < innerLoop; j++)
                    line.Intersect(line2);
                timer.Stop();
                lineTicks += timer.ElapsedTicks;

                timer.Restart();
                for (int j = 0; j < innerLoop; j++)
                    line.Intersect(line2, epsilon);
                timer.Stop();
                lineEpsilonTicks += timer.ElapsedTicks;

                timer.Restart();
                for (int j = 0; j < innerLoop; j++)
                    GeoAlgorithms.PointInPolygon(q, polygon);
                timer.Stop();
                polyTicks += timer.ElapsedTicks;

                timer.Restart();
                for (int j = 0; j < innerLoop; j++)
                    GeoAlgorithms.PointInPolygon(q, polygon, epsilon);
                timer.Stop();
                polyEpsilonTicks += timer.ElapsedTicks;
            }

            Output("                  ");
            Output(String.Format("{0,12}", "Exact"));
            Output(String.Format("{0,12}", "Epsilon"));

            Output("\nLine Intersection ");
            Output(String.Format("{0,12:N2}", 1000 * AverageMicrosecs(lineTicks, iterations)));
            Output(String.Format("{0,12:N2}", 1000 * AverageMicrosecs(lineEpsilonTicks, iterations)));

            Output("\nPoint in Polygon  ");
            Output(String.Format("{0,12:N2}", 1000 * AverageMicrosecs(polyTicks, iterations)));
            Output(String.Format("{0,12:N2}", 1000 * AverageMicrosecs(polyEpsilonTicks, iterations)));

            Output("\nTimes are nsec averages for exact and epsilon comparisons.\n");
            Output("Point in Polygon uses random polygons with 3-60 vertices.\n");
        }

        #endregion
        #region GeometryTest

        private void GeometryTest() {
            Stopwatch timer = new Stopwatch();
            var testCases = _geometryTestCases;

            Output(String.Format("{0,6}", " "));
            foreach (TestCase test in testCases)
                Output(String.Format("{0,12}", test.Name));
            Output("\n");

            const int outerLoop = 100, innerLoop = 100;
            const int iterations = outerLoop * innerLoop;

            for (int size = 10; size <= 120; size += 10) {
                PointD[] points = new PointD[size];

                for (int i = 0; i < outerLoop; i++) {
                    for (int j = 0; j < points.Length; j++)
                        points[j] = GeoAlgorithms.RandomPoint(0, 0, 1000, 1000);

                    foreach (TestCase test in testCases) {
                        // trigger JIT compilation and reset ticks
                        if (i == 0) {
                            test.FindPoints(points);
                            test.Ticks = 0;
                        }

                        timer.Restart();
                        for (int k = 0; k < innerLoop; k++)
                            test.FindPoints(points);
                        timer.Stop();
                        test.Ticks += timer.ElapsedTicks;
                    }
                }

                Output(String.Format("{0,6}", size));
                foreach (TestCase test in testCases)
                    Output(String.Format("{0,12:N2}", AverageMicrosecs(test.Ticks, iterations)));
                Output("\n");
            }

            Output("\nTimes are µsec averages for point sets of the indicated size.\n");
        }

        #endregion
        #region IntersectionTest

        private void IntersectionTest() {
            Stopwatch timer = new Stopwatch();
            var testCases = _intersectionTestCases;

            Output(String.Format("{0,6}", " "));
            foreach (TestCase test in testCases) {
                Output(String.Format("{0,26}", test.Name));
                Output(String.Format("{0,10}", " "));
            }
            Output("\n");

            Output(String.Format("{0,6}", " "));
            foreach (TestCase test in testCases) {
                Output(String.Format("{0,12}", "0"));
                Output(String.Format("{0,12}", "n"));
                Output(String.Format("{0,12}", "(n^2)/4"));
            }
            Output("\n\n");

            const int iterations = 600;

            for (int size = 10; size <= 120; size += 10) {
                long[] ticks = new long[testCases.Length * 3];

                // create three non-random line sets per test case
                LineD[][] lines = new LineD[3][];
                for (int i = 0; i < lines.Length; i++)
                    lines[i] = new LineD[size];

                // Zero intersections: Set of near-horizontal lines
                // Best case for horizontal sweep line = O(n)
                for (int j = 0; j < size; j++) {
                    double y = j * 1000 / size;
                    lines[0][j] = new LineD(0, y, 1000, y + 2);
                }

                // Linear number of intersections (n):
                // Set of near-horizontal lines with one vertical line
                lines[1][0] = new LineD(500, 0, 500, 1000);
                for (int j = 1; j < size; j++) {
                    double y = j * 1000 / size;
                    lines[1][j] = new LineD(0, y, 1000, y + 2);
                }

                // Quadratic number of intersections (n^2 / 4):
                // Crosshatch of near-horizontal and near-vertical lines
                for (int j = 0; j < size / 2; j++) {
                    double xy = j * 2000 / size;
                    lines[2][j] = new LineD(0, xy, 1000, xy + 2);
                    lines[2][j + size / 2] = new LineD(xy, 0, xy + 2, 1000);
                }

                // reset ticks for all test cases
                Array.Clear(ticks, 0, ticks.Length);

                for (int testIndex = 0; testIndex < testCases.Length; testIndex++) {
                    var test = testCases[testIndex];

                    // trigger JIT compilation
                    test.FindLines(lines[lines.Length - 1]);

                    for (int j = 0; j < lines.Length; j++) {
                        timer.Restart();
                        for (int k = 0; k < iterations; k++)
                            test.FindLines(lines[j]);
                        timer.Stop();
                        ticks[testIndex * lines.Length + j] += timer.ElapsedTicks;
                    }
                }

                Output(String.Format("{0,6}", size));
                for (int testIndex = 0; testIndex < testCases.Length; testIndex++) {
                    var test = testCases[testIndex];
                    for (int j = 0; j < lines.Length; j++)
                        Output(String.Format("{0,12:N2}", AverageMicrosecs(
                            ticks[testIndex * lines.Length + j], iterations)));
                }
                Output("\n");
            }

            Output("\nTimes are µsec averages for line sets of the indicated size,\n");
            Output("and with the indicated relative number of intersections.\n");
        }

        #endregion
        #region NearestPointTest

        private void NearestPointTest() {
            Stopwatch timer = new Stopwatch();
            var testCases = _nearestPointTestCases;

            Output(String.Format("{0,6}", " "));
            foreach (TestCase test in testCases)
                Output(String.Format("{0,12}", test.Name));
            Output("\n");

            const int outerLoop = 100, innerLoop = 100;
            const int iterations = outerLoop * innerLoop;

            var comparer = new PointDComparerY();
            PointD[] query = new PointD[innerLoop];

            for (int size = 1000; size <= 12000; size += 1000) {
                PointD[] points = new PointD[size];
                for (int i = 0; i < points.Length; i++)
                    points[i] = GeoAlgorithms.RandomPoint(0, 0, 1000, 1000);
                Array.Sort<PointD>(points, comparer);

                // trigger JIT compilation and reset ticks
                foreach (TestCase test in testCases) {
                    test.FindPointIndex(comparer, points, PointD.Empty);
                    test.Ticks = 0;
                }

                for (int j = 0; j < outerLoop; j++) {
                    for (int k = 0; k < query.Length; k++)
                        query[k] = GeoAlgorithms.RandomPoint(0, 0, 1000, 1000);

                    foreach (TestCase test in testCases) {
                        timer.Restart();
                        for (int k = 0; k < query.Length; k++)
                            test.FindPointIndex(comparer, points, query[k]);

                        timer.Stop();
                        test.Ticks += timer.ElapsedTicks;
                    }
                }

                Output(String.Format("{0,6:N0}", size));
                foreach (TestCase test in testCases)
                    Output(String.Format("{0,12:N2}", AverageMicrosecs(test.Ticks, iterations)));
                Output("\n");
            }

            Output("\nTimes are µsec averages for point arrays of the indicated size.\n");
        }

        #endregion
        #region RangeTreeTest

        private void RangeTreeTest() {
            Stopwatch timer = new Stopwatch();
            var testCases = _rangeTreeTestCases;

            Output(String.Format("{0,8}", " "));
            foreach (TestCase test in testCases)
                Output(String.Format("{0,14}", test.Name));
            Output("\n");

            // count units of size x operation in milliseconds,
            // rather than individual operations in microseconds
            const int outerLoop = 10, innerLoop = 10;
            const int iterations = outerLoop * innerLoop * 1000;

            // bounds of search space, size of point set,
            // range & iterations for range search
            const int bounds = 10000, size = 60000;
            const int range = size / 80, rangeIterations = size / 120;

            long[] addTicks = new long[testCases.Length],
                iterateTicks = new long[testCases.Length],
                searchTicks = new long[testCases.Length],
                rangeTicks = new long[testCases.Length],
                removeTicks = new long[testCases.Length];

            var array = new KeyValuePair<PointD, String>[size];
            for (int i = 0; i < outerLoop; i++) {

                // generate random spatial keys
                for (int j = 0; j < array.Length; j++) {
                    var key = GeoAlgorithms.RandomPoint(0, 0, bounds, bounds);
                    array[j] = new KeyValuePair<PointD, String>(key, null);
                }

                // trigger JIT compilation
                if (i == 0)
                    foreach (TestCase test in testCases) {
                        test.RangeTree.Clear();
                        foreach (var pair in array)
                            test.RangeTree.Add(pair.Key, pair.Value);
                    }

                for (int j = 0; j < innerLoop; j++)
                    for (int k = 0; k < testCases.Length; k++) {
                        TestCase test = testCases[k];
                        test.RangeTree.Clear();

                        timer.Restart();
                        foreach (var pair in array)
                            test.RangeTree.Add(pair.Key, pair.Value);
                        timer.Stop();
                        addTicks[k] += timer.ElapsedTicks;

                        double sum = 0;
                        timer.Restart();
                        foreach (var pair in test.RangeTree)
                            sum += pair.Key.X;
                        timer.Stop();
                        iterateTicks[k] += timer.ElapsedTicks;

                        timer.Restart();
                        foreach (var pair in array)
                            test.RangeTree.ContainsKey(pair.Key);
                        timer.Stop();
                        searchTicks[k] += timer.ElapsedTicks;

                        /*
                         * BraidedTree performs one-dimensional range searches within a point set
                         * sorted by y-coordinates, using PointDComparerY. Therefore, we supply a
                         * condition that limits x-coordinates.
                         */

                        var braidedTree = test.RangeTree as BraidedTree<PointD, String>;
                        var quadTree = test.RangeTree as QuadTree<String>;

                        timer.Restart();
                        for (int l = 0; l < array.Length; l += size / rangeIterations) {
                            PointD p = array[l].Key;
                            RectD rect = new RectD(p.X, p.Y, range, range);

                            if (braidedTree != null) {
                                braidedTree.FindRange(rect.TopLeft, rect.BottomRight,
                                    n => (n.Key.X >= rect.Left && n.Key.X <= rect.Right));
                            } else if (quadTree != null)
                                quadTree.FindRange(rect);
                        }
                        timer.Stop();
                        rangeTicks[k] += timer.ElapsedTicks;

                        timer.Restart();
                        foreach (var pair in array)
                            test.RangeTree.Remove(pair.Key);
                        timer.Stop();
                        removeTicks[k] += timer.ElapsedTicks;
                    }
            }

            Output(String.Format("{0,8}", "Add"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(addTicks[i], iterations)));

            Output(String.Format("\n{0,8}", "Iterate"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(iterateTicks[i], iterations)));

            Output(String.Format("\n{0,8}", "Search"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(searchTicks[i], iterations)));

            Output(String.Format("\n{0,8}", "Range"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(rangeTicks[i], iterations)));

            Output(String.Format("\n{0,8}", "Remove"));
            for (int i = 0; i < testCases.Length; i++)
                Output(String.Format("{0,14:N2}", AverageMicrosecs(removeTicks[i], iterations)));

            const double share = range / (double) bounds;
            Output(String.Format("\n\nTimes are msec averages for {0:N0} random points.\n", size));
            Output(String.Format("Range search performs {0} iterations on {1:0.00%} of search space.\n",
                rangeIterations, share * share));
        }

        #endregion
        #region SortingTest

        private void SortingTest() {
            Stopwatch timer = new Stopwatch();
            var testCases = _sortingTestCases;

            Output(String.Format("{0,6}", " "));
            foreach (TestCase test in testCases)
                Output(String.Format("{0,12}", test.Name));
            Output("\n");

            const int outerLoop = 100, innerLoop = 100;
            const int iterations = outerLoop * innerLoop;

            for (int size = 20; size <= 240; size += 20) {
                int[] array = new int[size];
                int[] testArray = new int[size];
                for (int i = 0; i < testArray.Length; i++)
                    testArray[i] = MersenneTwister.Default.Next(1000);

                // trigger JIT compilation and reset ticks
                foreach (TestCase test in testCases) {
                    testArray.CopyTo(array, 0);
                    test.Sort(array);
                    test.Ticks = 0;
                }

                for (int j = 0; j < outerLoop; j++) {
                    for (int i = 0; i < testArray.Length; i++)
                        testArray[i] = MersenneTwister.Default.Next(1000);

                    foreach (TestCase test in testCases) {
                        timer.Restart();
                        for (int k = 0; k < innerLoop; k++) {
                            testArray.CopyTo(array, 0);
                            test.Sort(array);
                        }
                        timer.Stop();
                        test.Ticks += timer.ElapsedTicks;
                    }
                }

                Output(String.Format("{0,6}", size));
                foreach (TestCase test in testCases)
                    Output(String.Format("{0,12:N2}", AverageMicrosecs(test.Ticks, iterations)));
                Output("\n");
            }

            Output("\nTimes are µsec averages for integer arrays of the indicated size.\n");
        }

        #endregion
        #region SubdivisionTest

        private void SubdivisionTest() {
            Stopwatch timer = new Stopwatch();
            var testCases = _subdivisionTestCases;

            Output(String.Format("{0,6}", " "));
            foreach (TestCase test in testCases)
                Output(String.Format("{0,12}", test.Name));
            Output("\n");

            // ensure reasonable vertex spacing
            const double epsilon = 1e-6;
            const int outerLoop = 20, innerLoop = 20;
            const int iterations = outerLoop * innerLoop;
            Subdivision[] divisions = new Subdivision[2];

            for (int size = 20; size <= 240; size += 20) {
                for (int j = 0; j < outerLoop; j++) {
                    foreach (TestCase test in testCases) {

                        for (int s = 0; s < 2; s++) {
                            int count = (int) (size * (s == 0 ? test.Value : (1 - test.Value)));
                            divisions[s] = CreateSubdivision(count, epsilon);
                        }

                        // trigger JIT compilation and reset ticks
                        ValueTuple<Int32, Int32>[] faceKeys;
                        if (j == 0) {
                            Subdivision.Intersection(divisions[0], divisions[1], out faceKeys);
                            test.Ticks = 0;
                        }

                        timer.Restart();
                        for (int k = 0; k < innerLoop; k++) {
                            var division = Subdivision.Intersection(
                                divisions[0], divisions[1], out faceKeys);
                            division.Validate();
                        }

                        timer.Stop();
                        test.Ticks += timer.ElapsedTicks;
                    }
                }

                Output(String.Format("{0,6:N0}", size));
                foreach (TestCase test in testCases)
                    Output(String.Format("{0,12:N2}", AverageMicrosecs(test.Ticks, iterations)));
                Output("\n");
            }

            Output("\nTimes are µsec averages for two subdivisions with the indicated combined" +
                "\nedge count, distributed as indicated between the two subdivisions.\n");
        }

        #endregion
        #region SubdivisionSearchTest

        private void SubdivisionSearchTest(bool random) {
            Stopwatch timer = new Stopwatch();
            var testCases = _subdivSearchTestCases;

            Output(String.Format("{0,6}", " "));
            foreach (TestCase test in testCases)
                Output(String.Format("{0,12}", test.Name));
            Output("\n");

            const int outerLoop = 100, innerLoop = 200;
            const int iterations = outerLoop * innerLoop;
            PointD[] query = new PointD[innerLoop];

            PolygonGrid grid = null;
            int sizeMin, sizeMax, sizeStep;
            if (random) {
                sizeMin = 100; sizeMax = 1200; sizeStep = 100;
            } else {
                sizeMin = 6; sizeMax = 30; sizeStep = 2;
                RegularPolygon polygon = new RegularPolygon(10, 4, PolygonOrientation.OnEdge);
                grid = new PolygonGrid(polygon);
            }

            for (int size = sizeMin; size <= sizeMax; size += sizeStep) {
                Subdivision division;
                if (random) {
                    // create subdivision from random lines (few faces)
                    division = CreateSubdivision(size, 1e-10);
                } else {
                    // create subdivision from grid of diamonds (many faces)
                    grid.Element = new RegularPolygon(900 / size, 4, PolygonOrientation.OnEdge);
                    grid.Size = new SizeI(size, size);
                    division = grid.ToSubdivision(PointD.Empty).Source;
                }
                var ordered = new SubdivisionSearch(division, true);
                var randomized = new SubdivisionSearch(division, false);

                // test cases: BruteForce, Ordered, Randomized
                testCases[0].FindSubdivision = (q) => division.Find(q, division.Epsilon);
                testCases[1].FindSubdivision = (q) => ordered.Find(q);
                testCases[2].FindSubdivision = (q) => randomized.Find(q);

                // trigger JIT compilation and reset ticks
                foreach (TestCase test in testCases) {
                    test.FindSubdivision(PointD.Empty);
                    test.Ticks = 0;
                }

                for (int j = 0; j < outerLoop; j++) {
                    for (int k = 0; k < query.Length; k++)
                        query[k] = GeoAlgorithms.RandomPoint(0, 0, 1000, 1000);

                    foreach (TestCase test in testCases) {
                        timer.Restart();
                        for (int k = 0; k < query.Length; k++)
                            test.FindSubdivision(query[k]);

                        timer.Stop();
                        test.Ticks += timer.ElapsedTicks;
                    }
                }

                Output(String.Format("{0,6:N0}", division.Edges.Count / 2));
                foreach (TestCase test in testCases)
                    Output(String.Format("{0,12:N2}", AverageMicrosecs(test.Ticks, iterations)));
                Output("\n");
            }

            Output("\nTimes are µsec averages for subdivisions of the indicated edge count,\n");
            if (random)
                Output("based on random line sets (few faces, completely random edges).\n");
            else
                Output("based on grids of squares (many faces, strictly ordered edges).\n");
        }

        #endregion
        #region AverageMicrosecs

        /// <summary>
        /// Converts the specified <see cref="Stopwatch.ElapsedTicks"/> to microseconds, given the
        /// current <see cref="Stopwatch.Frequency"/> and the specified iteration count.</summary>
        /// <param name="ticks">
        /// The <see cref="Stopwatch.ElapsedTicks"/> value to convert.</param>
        /// <param name="iterations">
        /// The total number of iterations in the test loop.</param>
        /// <returns>
        /// The average number of microseconds spent on every single iteration.</returns>

        private double AverageMicrosecs(long ticks, int iterations) {
            return (ticks * 1000000.0) / (Stopwatch.Frequency * iterations);
        }

        #endregion
        #region CreateSubdivision

        /// <summary>
        /// Creates a random <see cref="Subdivision"/> with the specified number of full edges and
        /// comparison epsilon.</summary>
        /// <param name="size">
        /// The number of full edges, i.e. half the number of <see cref="Subdivision.Edges"/>, in
        /// the returned <see cref="Subdivision"/>.</param>
        /// <param name="epsilon">
        /// The maximum absolute difference at which two coordinates should be considered equal.
        /// </param>
        /// <returns>
        /// A new random <see cref="Subdivision"/> with the specified <paramref name="size"/> and
        /// <paramref name="epsilon"/>.</returns>

        private static Subdivision CreateSubdivision(int size, double epsilon) {
            LineD[] lines = new LineD[size];
            for (int i = 0; i < size; i++)
                lines[i] = GeoAlgorithms.RandomLine(0, 0, 1000, 1000);

            // split random set into non-intersecting line segments
            var crossings = MultiLineIntersection.FindSimple(lines, epsilon);
            var splitLines = MultiLineIntersection.Split(lines, crossings);
            Array.Copy(splitLines, lines, size);

            // re-randomize lines to eliminate split ordering
            CollectionsUtility.Randomize(lines);
            Subdivision division = Subdivision.FromLines(lines);
            division.Validate();
            return division;
        }

        #endregion
        #region Output

        private void Output(string text) {
            Dispatcher.Invoke(delegate {
                OutputBox.AppendText(text);
                OutputBox.ScrollToEnd();
            });
        }

        #endregion
        #region StartThread

        private void StartThread(string name, ThreadStart start) {
            if (_thread != null) return;

            TestCombo.IsEnabled = false;
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            Output(String.Format("{0} Test Started: {1}\n\n", name, DateTime.Now));
            _threadException = null;

            // protect against exceptions triggered by Thread.Abort
            ThreadStart wrapper = delegate {
                try { start(); }
                catch (Exception e) {
                    _threadException = e;
                }
                finally {
                    Dispatcher.BeginInvoke(StopThread);
                }
            };

            _thread = new Thread(wrapper);
            _thread.IsBackground = true;
            _thread.Start();
        }

        #endregion
        #region StopThread

        private void StopThread() {

            if (_thread != null) {
                _thread.Join();
                _thread = null;

                if (_threadException != null) {
                    if (_threadException is ThreadAbortException)
                        Output("Stopped at user request.\n");
                    else
                        Output(_threadException.ToString());
                }

                Output(String.Format("\nTest Complete: {0}\n\n", DateTime.Now));
            }

            TestCombo.IsEnabled = true;
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        #endregion
        #region OnClosing

        protected override void OnClosing(CancelEventArgs args) {
            base.OnClosing(args);

            if (_thread != null) {
                _thread.Abort();
                _thread.Join();
                _thread = null;
            }
        }

        #endregion
        #region OnStart

        private void OnStart(object sender, RoutedEventArgs args) {
            args.Handled = true;

            var selection = TestCombo.SelectedItem;
            if (selection == SortingItem)
                StartThread("Sorting", SortingTest);
            else if (selection == CollectionItem)
                StartThread("Collection", CollectionTest);
            else if (selection == DictionaryItem)
                StartThread("Dictionary", () => {
                    DictionaryTest(false);
                    Output("\n");
                    DictionaryTest(true);
                });
            else if (selection == GeometryItem)
                StartThread("Geometry", () => {
                    GeometryBasicTest();
                    Output("\n");
                    GeometryTest();
                });
            else if (selection == NearestPointItem)
                StartThread("Nearest Point", NearestPointTest);
            else if (selection == IntersectionItem)
                StartThread("Intersection", IntersectionTest);
            else if (selection == SubdivisionItem)
                StartThread("Subdivision", SubdivisionTest);
            else if (selection == RangeTreeItem)
                StartThread("Range Tree", RangeTreeTest);
            else if (selection == SubdivSearchItem)
                StartThread("Subdivision Search", () => {
                    SubdivisionSearchTest(false);
                    Output("\n");
                    SubdivisionSearchTest(true);
                });
        }

        #endregion
        #region OnStop

        private void OnStop(object sender, RoutedEventArgs args) {
            args.Handled = true;
            if (_thread != null) _thread.Abort();
        }

        #endregion
        #region Class TestCase

        private class TestCase {
            public string Name;
            public long Ticks;
            public double Value;

            public Action<IList<Int32>> Sort;
            public ICollection<Int32> Collection;
            public IDictionary<Int32, String> Dictionary;
            public Action<PointD[]> FindPoints;
            public Func<PointDComparerY, IList<PointD>, PointD, Int32> FindPointIndex;
            public Func<LineD[], MultiLinePoint[]> FindLines;
            public IDictionary<PointD, String> RangeTree;
            public Func<PointD, SubdivisionElement> FindSubdivision;
        }

        #endregion
    }
}
