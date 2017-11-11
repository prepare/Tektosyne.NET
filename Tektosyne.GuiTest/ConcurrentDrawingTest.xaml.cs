using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Tektosyne.Windows;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> for testing the <see cref="ConcurrentVisualHost"/> class.
    /// </summary>
    /// <remarks>
    /// <b>ConcurrentDrawingTest</b> shows four <see cref="ConcurrentVisualHost"/> instances and
    /// allows the user to redraw them either sequentially or concurrently. Please refer to the
    /// Tektosyne User’s Guide for a detailed explanation of this dialog.</remarks>

    public partial class ConcurrentDrawingTest: Window {
        #region ConcurrentDrawingTest()

        public ConcurrentDrawingTest() {
            InitializeComponent();
            _buttons = TreeHelper.GetLogicalChildren<Button>(this);

            // determine grid size defined in XAML
            int rows = HostGrid.RowDefinitions.Count;
            int columns = HostGrid.ColumnDefinitions.Count;

            // create one concurrent DrawingVisual per Grid cell
            _visualHosts = new ConcurrentVisualHost[rows * columns];
            Func<Visual> createVisual = () => { return new DrawingVisual(); };

            for (int i = 0; i < _visualHosts.Length; i++) {
                ConcurrentVisualHost host = new ConcurrentVisualHost(createVisual);
                _visualHosts[i] = host;

                HostGrid.Children.Add(host);
                Grid.SetColumn(host, i % columns);
                Grid.SetRow(host, i / rows);

                DrawingVisual visual = (DrawingVisual) host.WorkerVisual;
                host.BeginWork(() => DrawName(visual));
            }

            // concurrent threads require dedicated RNG instances
            int ticks = Environment.TickCount;
            _twisters = new MersenneTwister[_visualHosts.Length];
            for (int i = 0; i < _twisters.Length; i++)
                _twisters[i] = new MersenneTwister((uint) (ticks + i));
        }

        #endregion
        #region Private Fields

        // all buttons within dialog window
        private readonly List<Button> _buttons;

        // arrays with one element per grid cell
        private readonly ConcurrentVisualHost[] _visualHosts;
        private readonly MersenneTwister[] _twisters;

        // helpers for cell drawing algorithms
        private int _cellWidth, _cellHeight;
        private Typeface _cellTypeface;

        #endregion
        #region DrawAllContents

        private void DrawAllContents(bool isConcurrent) {

            // show how long GUI thread is inactive
            foreach (Button button in _buttons)
                button.IsEnabled = false;
            Dispatcher.DoEvents();

            // determine actual size of each grid cell
            _cellWidth = (int) HostGrid.ColumnDefinitions[0].ActualWidth;
            _cellHeight = (int) HostGrid.RowDefinitions[0].ActualHeight;

            for (int i = 0; i < _visualHosts.Length; i++) {
                ConcurrentVisualHost host = _visualHosts[i];
                DrawingVisual visual = (DrawingVisual) host.WorkerVisual;
                MersenneTwister twister = _twisters[i];

                Action action = () => DrawContent(visual, twister);
                if (isConcurrent)
                    host.WorkerDispatcher.BeginInvoke(action);
                else
                    host.WorkerDispatcher.Invoke(action);
            }

            // GUI thread becomes active again
            foreach (Button button in _buttons)
                button.IsEnabled = true;
        }

        #endregion
        #region DrawContent

        private void DrawContent(DrawingVisual visual, MersenneTwister twister) {

            // simulate heavy workload (see Tektosyne User's Guide)
            Thread.Sleep(1000);

            using (DrawingContext context = visual.RenderOpen()) {
                for (int i = 0; i < 100; i++) {

                    int width = twister.Next(4, 40);
                    int height = twister.Next(4, 40);
                    int x = twister.Next(_cellWidth - width);
                    int y = twister.Next(_cellHeight - height);
                    Rect rect = new Rect(x, y, width, height);

                    Brush brush = new SolidColorBrush(Color.FromArgb(
                        (byte) twister.Next(255), (byte) twister.Next(255),
                        (byte) twister.Next(255), (byte) twister.Next(255)));

                    Pen pen = new Pen(new SolidColorBrush(Color.FromArgb(
                        (byte) twister.Next(255), (byte) twister.Next(255),
                        (byte) twister.Next(255), (byte) twister.Next(255))), 1);

                    context.DrawRectangle(brush, pen, rect);
                }
            }
        }

        #endregion
        #region DrawName

        private void DrawName(DrawingVisual visual) {

            string name = String.Format(CultureInfo.InvariantCulture,
                "Thread “{0}”", visual.Dispatcher.Thread.Name);

            if (_cellTypeface == null)
                _cellTypeface = new Typeface(new FontFamily("Georgia"),
                    FontStyles.Italic, FontWeights.Normal, FontStretches.Normal);

            FormattedText text = new FormattedText(name,
                    CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                    _cellTypeface, 12, SystemColors.ControlTextBrush);

            using (DrawingContext context = visual.RenderOpen())
                context.DrawText(text, new Point(10, 10));
        }

        #endregion
        #region OnClosed

        protected override void OnClosed(EventArgs args) {
            base.OnClosed(args);

            // stop all worker dispatchers and background threads
            foreach (ConcurrentVisualHost host in _visualHosts)
                host.Dispose();
        }

        #endregion
        #region Event Handlers

        private void OnClear(object sender, RoutedEventArgs args) {
            args.Handled = true;

            foreach (ConcurrentVisualHost host in _visualHosts) {
                DrawingVisual visual = (DrawingVisual) host.WorkerVisual;
                host.BeginWork(() => DrawName(visual));
            }
        }

        private void OnConcurrent(object sender, RoutedEventArgs args) {
            args.Handled = true;

            OnClear(sender, args);
            DrawAllContents(true);
        }

        private void OnSequential(object sender, RoutedEventArgs args) {
            args.Handled = true;

            OnClear(sender, args);
            DrawAllContents(false);
        }

        #endregion
    }
}
