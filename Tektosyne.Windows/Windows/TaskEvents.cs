using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;

using SysDispatcher = System.Windows.Threading.Dispatcher;

namespace Tektosyne.Windows {

    using ExceptionEventArgs = EventArgs<Exception>;
    using ExceptionEventHandler = EventHandler<EventArgs<Exception>>;

    using MessageEventArgs = EventArgs<String>;
    using MessageEventHandler = EventHandler<EventArgs<String>>;

    using ProgressEventArgs = EventArgs<Int32>;
    using ProgressEventHandler = EventHandler<EventArgs<Int32>>;

    /// <summary>
    /// Provides events to communicate the status of a task.</summary>
    /// <remarks><para>
    /// <b>TaskEvents</b> bundles several general-purpose events that allow a worker algorithm to
    /// broadcast the status of a given task to any listeners, as follows:
    /// </para><list type="bullet"><item>
    /// Two events to notify any listeners that the task was completed, or that an exception has
    /// occurred.
    /// </item><item>
    /// Two events to send text messages and progress indicators to any listeners.
    /// </item><item>
    /// A timer that clients can query to determine when to raise message and progress events.
    /// </item></list><para>
    /// Moreover, <b>TaskEvents</b> allows the worker algorithm and all listeners to live on
    /// different threads. Clients may specify a <see cref="SysDispatcher"/> on which all event
    /// handlers are invoked so that the calls are marshalled to the GUI thread.</para></remarks>

    public class TaskEvents {
        #region TaskEvents()

        /// <overloads>
        /// Initializes a new instance of the <see cref="TaskEvents"/> class. </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskEvents"/> class with default
        /// properties.</summary>

        public TaskEvents(): this(null) { }

        #endregion
        #region TaskEvents(Dispatcher)

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskEvents"/> class with the specified <see
        /// cref="SysDispatcher"/>.</summary>
        /// <param name="dispatcher">
        /// The <see cref="SysDispatcher"/> used to marshal calls to the foreground thread. This
        /// argument may be a null reference.</param>

        public TaskEvents(SysDispatcher dispatcher) {
            Dispatcher = dispatcher;

            // create & start timer
            Timer = new Stopwatch();
            Timer.Start();
        }

        #endregion
        #region Dispatcher

        /// <summary>
        /// The <see cref="SysDispatcher"/> used to marshal calls to the foreground thread. The
        /// default is a null reference, indicating that no cross-thread marshalling is desired.
        /// </summary>
        /// <remarks>
        /// The event methods check <b>Dispatcher</b> to determine how to raise their respective
        /// events. If <b>Dispatcher</b> is a null reference or its <see
        /// cref="SysDispatcher.CheckAccess"/> method succeeds, the event handlers are invoked
        /// directly; otherwise, they are invoked by calling <see cref="SysDispatcher.BeginInvoke"/>
        /// or <see cref="SysDispatcher.Invoke"/> on the <b>Dispatcher</b>.</remarks>

        public readonly SysDispatcher Dispatcher;

        #endregion
        #region Priority

        /// <summary>
        /// The <see cref="DispatcherPriority"/> for asynchronous cross-thread calls marshalled by
        /// the <see cref="Dispatcher"/>. The default is <see cref="DispatcherPriority.Normal"/>.
        /// </summary>
        /// <remarks>
        /// <b>Priority</b> is ignored by <see cref="OnTaskException"/> which always performs
        /// synchronous calls with <see cref="DispatcherPriority.Send"/> priority.</remarks>

        public DispatcherPriority Priority = DispatcherPriority.Normal;

        #endregion
        #region Timer

        /// <summary>
        /// The <see cref="Stopwatch"/> timer used to control timed progress display.</summary>
        /// <remarks>
        /// <b>Timer</b> is created and started during construction of the <see cref="TaskEvents"/>
        /// object. Clients may read or reset the <b>Timer</b> as needed to determine when to raise
        /// notification events. Call <see cref="RestartTimer"/> to restart the <b>Timer</b> when a
        /// specific amount of time has elapsed.</remarks>

        public readonly Stopwatch Timer;

        #endregion
        #region Public Methods
        #region OnTaskComplete

        /// <summary>
        /// Raises the <see cref="TaskComplete"/> event.</summary>
        /// <param name="sender">
        /// The <see cref="Object"/> sending the event.</param>
        /// <remarks><para>
        /// <b>OnTaskComplete</b> raises the <see cref="TaskComplete"/> event. Use this method to
        /// notify any listeners that work on the current task is complete.
        /// </para><para>
        /// If <see cref="Dispatcher"/> is valid and <see cref="SysDispatcher.CheckAccess"/> fails,
        /// <b>OnTaskComplete</b> performs an asynchronous delegate invocation via <see
        /// cref="SysDispatcher.BeginInvoke"/> and with the current <see cref="Priority"/>.
        /// </para></remarks>

        public void OnTaskComplete(object sender) {

            var handler = TaskComplete;
            if (handler != null) {
                if (Dispatcher != null && !Dispatcher.CheckAccess()) {
                    foreach (EventHandler eh in handler.GetInvocationList())
                        Dispatcher.BeginInvoke(Priority, eh, sender, EventArgs.Empty);
                } else
                    handler(sender, EventArgs.Empty);
            }
        }

        #endregion
        #region OnTaskException

        /// <summary>
        /// Raises the <see cref="TaskException"/> event with the specified <see cref="Exception"/>.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="Object"/> sending the event.</param>
        /// <param name="exception">
        /// The <see cref="Exception"/> that occurred while working on the task.</param>
        /// <remarks><para>
        /// <b>OnTaskException</b> raises the <see cref="TaskException"/> event with an <see
        /// cref="ExceptionEventArgs"/> object that wraps the specified <paramref
        /// name="exception"/>. Use this method to notify any listeners of an unrecoverable error
        /// that occurred while working on the current task.
        /// </para><para>
        /// Unlike the other callback methods, <b>OnTaskException</b> invokes its associated event
        /// handlers <em>synchronously</em> with a fixed priority of <see
        /// cref="DispatcherPriority.Send"/> if <see cref="Dispatcher"/> is valid and <see
        /// cref="SysDispatcher.CheckAccess"/> fails. This allows clients to examine the situation
        /// and decide on further actions while the background thread is blocked.</para></remarks>

        public void OnTaskException(object sender, Exception exception) {

            var handler = TaskException;
            if (handler != null) {
                ExceptionEventArgs args = new ExceptionEventArgs(exception);

                if (Dispatcher != null && !Dispatcher.CheckAccess()) {
                    foreach (ExceptionEventHandler eeh in handler.GetInvocationList())
                        Dispatcher.Invoke(DispatcherPriority.Send, eeh, sender, args);
                } else
                    handler(sender, args);
            }
        }

        #endregion
        #region OnTaskMessage(Object, String)

        /// <overloads>
        /// Raises the <see cref="TaskMessage"/> event with the specified message.</overloads>
        /// <summary>
        /// Raises the <see cref="TaskMessage"/> event with the specified message.</summary>
        /// <param name="sender">
        /// The <see cref="Object"/> sending the event.</param>
        /// <param name="message">
        /// A <see cref="String"/> containing the message text.</param>
        /// <remarks><para>
        /// <b>OnTaskMessage</b> raises the <see cref="TaskMessage"/> event with a <see
        /// cref="MessageEventArgs"/> object that wraps the specified <paramref name="message"/>.
        /// Use this method and its overloads to send any listeners a text message concerning the
        /// current task.
        /// </para><para>
        /// If <see cref="Dispatcher"/> is valid and <see cref="SysDispatcher.CheckAccess"/> fails,
        /// <b>OnTaskMessage</b> performs an asynchronous delegate invocation via <see
        /// cref="SysDispatcher.BeginInvoke"/> and with the current <see cref="Priority"/>.
        /// </para></remarks>

        public void OnTaskMessage(object sender, string message) {

            var handler = TaskMessage;
            if (handler != null) {
                MessageEventArgs args = new MessageEventArgs(message);

                if (Dispatcher != null && !Dispatcher.CheckAccess()) {
                    foreach (MessageEventHandler meh in handler.GetInvocationList())
                        Dispatcher.BeginInvoke(Priority, meh, sender, args);
                } else
                    handler(sender, args);
            }
        }

        #endregion
        #region OnTaskMessage(Object, String, Object[])

        /// <summary>
        /// Raises the <see cref="TaskMessage"/> event with a formatted message containing the
        /// values of the specified objects.</summary>
        /// <param name="sender">
        /// The <see cref="Object"/> sending the event.</param>
        /// <param name="format">
        /// A <see cref="String"/> containing zero or more format specifications.</param>
        /// <param name="args">
        /// An <see cref="Array"/> of zero or more <see cref="Object"/> instances to be formatted.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> or <paramref name="args"/> is a null reference.</exception>
        /// <exception cref="FormatException"><para>
        /// <paramref name="format"/> contains an invalid format specification.
        /// </para><para>-or-</para><para>
        /// A number indicating an argument to be formatted is less than zero, or greater than or
        /// equal to the length of the <paramref name="args"/> array.</para></exception>
        /// <remarks>
        /// <b>OnTaskMessage</b> invokes <see cref="String.Format"/> with the specified <paramref
        /// name="format"/> and <paramref name="args"/>, and then calls the basic <see
        /// cref="OnTaskMessage(Object, String)"/> overload to raise the <see cref="TaskMessage"/>
        /// event with the resulting formatted message.</remarks>

        public void OnTaskMessage(object sender, string format, params object[] args) {
            if (TaskMessage != null)
                OnTaskMessage(sender, String.Format(CultureInfo.CurrentCulture, format, args));
        }

        #endregion
        #region OnTaskMessage(Object, IFormatProvider, String, Object[])

        /// <summary>
        /// Raises the <see cref="TaskMessage"/> event with a formatted message containing the
        /// values of the specified objects, using the specified culture-specific formatting
        /// information.</summary>
        /// <param name="sender">
        /// The <see cref="Object"/> sending the event.</param>
        /// <param name="provider">
        /// An <see cref="IFormatProvider"/> object that supplies culture-specific formatting
        /// information.</param>
        /// <param name="format">
        /// A <see cref="String"/> containing zero or more format specifications.</param>
        /// <param name="args">
        /// An <see cref="Array"/> of zero or more <see cref="Object"/> instances to be formatted.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> or <paramref name="args"/> is a null reference.</exception>
        /// <exception cref="FormatException"><para>
        /// <paramref name="format"/> contains an invalid format specification.
        /// </para><para>-or-</para><para>
        /// A number indicating an argument to be formatted is less than zero, or greater than or
        /// equal to the length of the <paramref name="args"/> array.</para></exception>
        /// <remarks>
        /// <b>OnTaskMessage</b> invokes <see cref="String.Format"/> with the specified <paramref
        /// name="provider"/>, <paramref name="format"/>, and <paramref name="args"/>, and then
        /// calls the basic <see cref="OnTaskMessage(Object, String)"/> overload to raise the <see
        /// cref="TaskMessage"/> event with the resulting formatted message.</remarks>

        public void OnTaskMessage(object sender,
            IFormatProvider provider, string format, params object[] args) {

            if (TaskMessage != null)
                OnTaskMessage(sender, String.Format(provider, format, args));
        }

        #endregion
        #region OnTaskProgress

        /// <summary>
        /// Raises the <see cref="TaskProgress"/> event with the specified progress indicator.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="Object"/> sending the event.</param>
        /// <param name="progress">
        /// An <see cref="Int32"/> value indicating the progress made on the task.</param>
        /// <remarks><para>
        /// <b>OnTaskProgress</b> raises the <see cref="TaskProgress"/> event with a <see
        /// cref="ProgressEventArgs"/> object that wraps the specified <paramref name="progress"/>.
        /// Use this method to send any listeners a numerical indicator for the progress made on the
        /// current task.
        /// </para><para>
        /// If <see cref="Dispatcher"/> is valid and <see cref="SysDispatcher.CheckAccess"/> fails,
        /// <b>OnTaskProgress</b> performs an asynchronous delegate invocation via <see
        /// cref="SysDispatcher.BeginInvoke"/> and with the current <see cref="Priority"/>.
        /// </para></remarks>

        public void OnTaskProgress(object sender, int progress) {

            var handler = TaskProgress;
            if (handler != null) {
                ProgressEventArgs args = new ProgressEventArgs(progress);

                if (Dispatcher != null && !Dispatcher.CheckAccess()) {
                    foreach (ProgressEventHandler peh in handler.GetInvocationList())
                        Dispatcher.BeginInvoke(Priority, peh, sender, args);
                } else
                    handler(sender, args);
            }
        }

        #endregion
        #region RestartTimer

        /// <summary>
        /// Restarts the <see cref="Timer"/> if the specified amount of time has elapsed.</summary>
        /// <param name="elapsed">
        /// The amount of time, in milliseconds, to compare against the <see
        /// cref="Stopwatch.ElapsedMilliseconds"/> property of the <see cref="Timer"/>.</param>
        /// <returns>
        /// <c>true</c> if at least <paramref name="elapsed"/> milliseconds have accumulated on the
        /// <see cref="Timer"/> since it was last reset; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="elapsed"/> is less than zero.</exception>
        /// <remarks>
        /// <b>RestartTimer</b> calls <see cref="Stopwatch.Restart"/> on the associated <see
        /// cref="Timer"/> when returning <c>true</c>; otherwise, the <see cref="Timer"/> remains
        /// unchanged.</remarks>

        public bool RestartTimer(long elapsed) {
            if (elapsed < 0L)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "elapsed", elapsed, Strings.ArgumentNegative);

            if (Timer.ElapsedMilliseconds < elapsed)
                return false;

            Timer.Restart();
            return true;
        }

        #endregion
        #endregion
        #region TaskComplete

        /// <summary>
        /// Occurs when the task is complete.</summary>
        /// <remarks>
        /// <b>TaskComplete</b> is raised by the <see cref="OnTaskComplete"/> method to indicate
        /// that work on the current task is complete.</remarks>

        public event EventHandler TaskComplete;

        #endregion
        #region TaskException

        /// <summary>
        /// Occurs when an exception was raised.</summary>
        /// <remarks>
        /// <b>TaskException</b> is raised by the <see cref="OnTaskException"/> method to indicate 
        /// that an unrecoverable error occurred while working on the current task.</remarks>

        public event ExceptionEventHandler TaskException;

        #endregion
        #region TaskMessage

        /// <summary>
        /// Occurs when a text message is sent.</summary>
        /// <remarks>
        /// <b>TaskMessage</b> is raised by the <see cref="OnTaskMessage"/> method to broadcast a
        /// text message concerning the current task.</remarks>

        public event MessageEventHandler TaskMessage;

        #endregion
        #region TaskProgress

        /// <summary>
        /// Occurs when progress is made on the task.</summary>
        /// <remarks>
        /// <b>TaskProgress</b> is raised by the <see cref="OnTaskProgress"/> method to broadcast a
        /// numerical indicator for the progress made on the current task.</remarks>

        public event ProgressEventHandler TaskProgress;

        #endregion
    }
}
