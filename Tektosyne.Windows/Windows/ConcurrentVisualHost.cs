using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides a <see cref="FrameworkElement"/> that hosts a single <see cref="Visual"/> living on
    /// a background thread.</summary>
    /// <remarks><para>
    /// <b>ConcurrentVisualHost</b> provides a <see cref="ContainerVisualHost"/> that internally
    /// hosts a <see cref="VisualSource"/> whose <see cref="VisualSource.RootVisual"/> lives on a
    /// background thread.
    /// </para><para>
    /// <b>ConcurrentVisualHost</b> automatically starts the background thread upon construction.
    /// The associated <see cref="WorkerDispatcher"/> and <see cref="WorkerVisual"/> are exposed to
    /// allow arbitrary content manipulation.
    /// </para><para>
    /// The background thread is manually created, rather than on the <see cref="ThreadPool"/>,
    /// since its lifetime is usually the same as the application’s. Its <see cref="Thread.Name"/>
    /// equals “WorkerVisual ” followed by the <see cref="Thread.ManagedThreadId"/>.
    /// </para><para>
    /// Unlike most WPF classes, <b>ConcurrentVisualHost</b> implements <see cref="IDisposable"/>.
    /// When deleting an instance before the application exits, call <see cref="Dispose"/> to stop
    /// the <see cref="WorkerDispatcher"/> and terminate the background thread.
    /// </para><note type="caution">
    /// Do not embed <b>ConcurrentVisualHost</b> in XAML! Visual Studio attempts to instantiate all
    /// declared objects when XAML pages are opened in design view. This starts unneeded background
    /// threads and can even freeze Visual Studio for several seconds. Always instantiate and attach
    /// <b>ConcurrentVisualHost</b> manually from code-behind.</note></remarks>

    public sealed class ConcurrentVisualHost: FrameworkElement, IDisposable {
        #region ConcurrentVisualHost()

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConcurrentVisualHost"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentVisualHost"/> class with default
        /// properties.</summary>

        public ConcurrentVisualHost(): this(null) { }

        #endregion
        #region ConcurrentVisualHost(Func<Visual>)

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentVisualHost"/> class with the
        /// specified method to create the <see cref="WorkerVisual"/>.</summary>
        /// <param name="createVisual">
        /// An optional <see cref="Func{Visual}"/> delegate that returns the initial value for the
        /// <see cref="WorkerVisual"/> property.</param>
        /// <remarks><para>
        /// <see cref="WorkerVisual"/> is initialized to a null reference if <paramref
        /// name="createVisual"/> is a null reference, returns a null reference, or throws any <see
        /// cref="Exception"/> when called.
        /// </para><para>
        /// <see cref="WorkerVisual"/> must be initialized through a <see cref="Func{Visual}"/>
        /// delegate, rather than directly with a <see cref="Visual"/> object, because that object
        /// must be created on the thread of the <see cref="WorkerDispatcher"/>.</para></remarks>

        public ConcurrentVisualHost(Func<Visual> createVisual) {

            // create host for worker visual
            _hostVisual = new HostVisual();
            AddVisualChild(_hostVisual);
            AddLogicalChild(_hostVisual);

            // prepare for synchronization with worker dispatcher
            _workerCreated = new ManualResetEventSlim(false);

            // create dispatcher thread to draw worker visual
            _workerThread = new Thread(StartWorkerDispatcher);
            _workerThread.Name = "WorkerVisual " + _workerThread.ManagedThreadId;
            _workerThread.SetApartmentState(ApartmentState.STA);
            _workerThread.IsBackground = true;
            _workerThread.Start(createVisual);

            // wait for creation of worker dispatcher
            _workerCreated.Wait();
            _workerCreated.Dispose();
            _workerCreated = null;
        }

        #endregion
        #region Private Fields

        // worker thread and synchronization flag
        private readonly Thread _workerThread;
        private readonly ManualResetEventSlim _workerCreated;

        // objects for cross-thread marshalling
        private readonly HostVisual _hostVisual;
        private VisualSource _visualSource;

        // property backers
        private Dispatcher _workerDispatcher;
        private Visual _workerVisual;
        private bool _isDisposed;

        #endregion
        #region WorkerDispatcher

        /// <summary>
        /// Gets the <see cref="Dispatcher"/> that runs on the background thread.</summary>
        /// <value>
        /// The <see cref="Dispatcher"/> that runs on the background thread.</value>
        /// <remarks>
        /// <b>WorkerDispatcher</b> never returns a null reference, but calling <see
        /// cref="Dispose"/> shuts down the <see cref="Dispatcher"/> and terminates the background
        /// thread. Use <see cref="IsDisposed"/> to test for this condition.</remarks>

        public Dispatcher WorkerDispatcher {
            [DebuggerStepThrough]
            get { return _workerDispatcher; }
        }

        #endregion
        #region WorkerVisual

        /// <summary>
        /// Gets or sets the <see cref="Visual"/> that lives on the background thread.</summary>
        /// <value>
        /// The <see cref="Visual"/> living on the thread of the <see cref="WorkerDispatcher"/>. The
        /// default is null reference.</value>
        /// <remarks>
        /// Changing <b>WorkerVisual</b> requires that the construction of the new property value
        /// and the property setter operation are both executed on the thread of the <see
        /// cref="WorkerDispatcher"/>.</remarks>

        public Visual WorkerVisual {
            [DebuggerStepThrough]
            get { return _workerVisual; }
            set {
                if (_workerVisual != value) {
                    _workerVisual = value;
                    _visualSource.RootVisual = _workerVisual;
                }
            }
        }

        #endregion
        #region VisualChildrenCount

        /// <summary>
        /// Gets the number of visual children of the <see cref="ConcurrentVisualHost"/>.</summary>
        /// <value>
        /// The number of visual children of the <see cref="ConcurrentVisualHost"/>.</value>
        /// <remarks>
        /// <b>VisualChildrenCount</b> returns the constant value one, as the <see
        /// cref="ConcurrentVisualHost"/> always directly contains only a single <see
        /// cref="HostVisual"/>.</remarks>

        protected override int VisualChildrenCount {
            get { return 1; }
        }

        #endregion
        #region BeginWork(Action, Object[])

        /// <overloads>
        /// Executes the specified <see cref="Action"/> asynchronously on the <see
        /// cref="WorkerDispatcher"/> thread.</overloads>
        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously on the <see
        /// cref="WorkerDispatcher"/> thread, using <see cref="DispatcherPriority.Normal"/>
        /// priority.</summary>
        /// <param name="action">
        /// The <see cref="Action"/> to execute asynchronously.</param>
        /// <param name="args">
        /// An optional <see cref="Array"/> of arguments for <paramref name="action"/>.</param>
        /// <returns>
        /// The <see cref="DispatcherOperation"/> returned by <see cref="Dispatcher.BeginInvoke"/>.
        /// </returns>
        /// <remarks>
        /// <b>BeginWork</b> is a helper method, provided for convenience, that merely calls <see
        /// cref="Dispatcher.BeginInvoke"/> on the <see cref="WorkerDispatcher"/> with the specified
        /// <paramref name="action"/> and <paramref name="args"/>.</remarks>

        public DispatcherOperation BeginWork(Action action, params object[] args) {
            return _workerDispatcher.BeginInvoke(action, args);
        }

        #endregion
        #region BeginWork(Action, DispatcherPriority, Object[])

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously on the <see
        /// cref="WorkerDispatcher"/> thread, using the specified priority.</summary>
        /// <param name="action">
        /// The <see cref="Action"/> to execute asynchronously.</param>
        /// <param name="priority">
        /// The <see cref="DispatcherPriority"/> at which to execute <paramref name="action"/>.
        /// </param>
        /// <param name="args">
        /// An optional <see cref="Array"/> of arguments for <paramref name="action"/>.</param>
        /// <returns>
        /// The <see cref="DispatcherOperation"/> returned by <see cref="Dispatcher.BeginInvoke"/>.
        /// </returns>
        /// <remarks>
        /// <b>BeginWork</b> is a helper method, provided for convenience, that merely calls <see
        /// cref="Dispatcher.BeginInvoke"/> on the <see cref="WorkerDispatcher"/> with the specified
        /// <paramref name="action"/>, <paramref name="priority"/>, and <paramref name="args"/>.
        /// </remarks>

        public DispatcherOperation BeginWork(
            Action action, DispatcherPriority priority, params object[] args) {

            return _workerDispatcher.BeginInvoke(action, priority, args);
        }

        #endregion
        #region GetVisualChild

        /// <summary>
        /// Returns the visual child element with the specified index.</summary>
        /// <param name="index">
        /// The zero-based index of the visual child element to return.</param>
        /// <returns>
        /// The visual child element with the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> does not equal zero.</exception>
        /// <remarks>
        /// <b>GetVisualChild</b> returns the associated <see cref="HostVisual"/> if the specified
        /// <paramref name="index"/> equals zero.</remarks>

        protected override Visual GetVisualChild(int index) {
            if (index != 0)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "index", index, Tektosyne.Strings.ArgumentNotEquals, "0");

            return _hostVisual;
        }

        #endregion
        #region MeasureOverride

        /// <summary>
        /// Measures the layout size required for the <see cref="ConcurrentVisualHost"/>.</summary>
        /// <param name="availableSize">
        /// The layout size available for the <see cref="ConcurrentVisualHost"/>.</param>
        /// <returns>
        /// The layout size required for the <see cref="ConcurrentVisualHost"/>.</returns>
        /// <remarks>
        /// <b>MeasureOverride</b> always returns the <see cref="ContainerVisual.ContentBounds"/>
        /// size of the associated <see cref="HostVisual"/> which reflects the size of the <see
        /// cref="WorkerVisual"/>. The specified <paramref name="availableSize"/> is ignored.
        /// </remarks>

        protected override Size MeasureOverride(Size availableSize) {
            return _hostVisual.ContentBounds.Size;
        }

        #endregion
        #region StartWorkerDispatcher

        /// <summary>
        /// Starts the <see cref="WorkerDispatcher"/>.</summary>
        /// <param name="obj">
        /// An optional <see cref="Func{Visual}"/> delegate that returns the initial value for the
        /// <see cref="WorkerVisual"/> property.</param>
        /// <remarks>
        /// <b>StartWorkerDispatcher</b> must run on a background thread.</remarks>

        private void StartWorkerDispatcher(object obj) {

            // initialize worker visual if desired
            Func<Visual> createVisual = obj as Func<Visual>;
            if (createVisual != null) {
                try { _workerVisual = createVisual(); }
                catch { _workerVisual = null; }
            }

            // establish WPF connection between threads
            _visualSource = new VisualSource(_hostVisual);
            _visualSource.RootVisual = _workerVisual;

            // create and publish worker dispatcher
            _workerDispatcher = Dispatcher.CurrentDispatcher;
            _workerCreated.Set();

            Dispatcher.Run();
        }

        #endregion
        #region IDisposable Members
        #region IsDisposed

        /// <summary>
        /// Gets a value indicating whether the <see cref="ConcurrentVisualHost"/> has been disposed
        /// of.</summary>
        /// <value>
        /// <c>true</c> if <see cref="Dispose"/> has been called; otherwise, <c>false</c>.</value>

        public bool IsDisposed {
            [DebuggerStepThrough]
            get { return _isDisposed; }
        }

        #endregion
        #region Dispose

        /// <summary>
        /// Releases all resources used by the <see cref="ConcurrentVisualHost"/>.</summary>
        /// <remarks>
        /// <b>Dispose</b> stops the <see cref="WorkerDispatcher"/> and allows the background thread
        /// to terminate.</remarks>

        public void Dispose() {
            if (!_isDisposed) {
                _isDisposed = true;

                // VisualSource must be disposed on background thread
                _workerDispatcher.Invoke(new Action(_visualSource.Dispose));

                _workerDispatcher.InvokeShutdown();
                Debug.Assert(_workerDispatcher.HasShutdownStarted);
            }
        }

        #endregion
        #endregion
    }
}
