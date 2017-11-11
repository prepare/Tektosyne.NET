using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Tektosyne.Windows {

    /// <summary>
    /// Presents a WPF <see cref="Visual"/> within a <see cref="HostVisual"/> that resides on
    /// another thread.</summary>
    /// <remarks><para>
    /// <b>VisualSource</b> enables multithreaded rendering within a single top-level WPF <see
    /// cref="Window"/>. This requires two <see cref="Visual"/> objects:
    /// </para><list type="bullet"><item>
    /// The presentation <em>target</em> is a <see cref="HostVisual"/> that resides on the same
    /// thread as the <see cref="Window"/>, e.g. within a <see cref="ContainerVisualHost"/>. This
    /// object is supplied during construction.
    /// </item><item>
    /// The presentation <em>source</em> is an arbitrary <see cref="RootVisual"/>, e.g. a <see
    /// cref="DrawingVisual"/>, that resides on the same thread as the <see cref="VisualSource"/>.
    /// This object can be changed after construction.
    /// </item></list><para>
    /// Source and target thread must each be running a <see cref="Dispatcher"/> loop. When creating
    /// a background thread, simply call <see cref="Dispatcher.Run"/> at the end of its <see
    /// cref="ThreadStart"/> method. Once the connection is established, any new source content
    /// automatically appears within the presentation target.
    /// </para><para>
    /// <see cref="ConcurrentVisualHost"/> provides a convenient wrapper for <see
    /// cref="VisualSource"/> that implements all of the above functionality in a single class.
    /// </para><para>
    /// <b>VisualSource</b> was adapted from the post <a
    /// href="http://blogs.msdn.com/b/dwayneneed/archive/2007/04/26/multithreaded-ui-hostvisual.aspx">Multithreaded
    /// UI: HostVisual</a> on Dwayne Need’s weblog <a
    /// href="http://blogs.msdn.com/b/dwayneneed/">Presentation Source</a>. Please refer to that
    /// post for more details and a usage sample.</para></remarks>

    public sealed class VisualSource: PresentationSource, IDisposable {
        #region VisualSource(HostVisual)

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualSource"/> class.</summary>
        /// <param name="hostVisual">
        /// The <see cref="HostVisual"/> that is the target of the presentation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="hostVisual"/> is a null reference.</exception>
        /// <remarks>
        /// The specified <paramref name="hostVisual"/> may reside on a different thread than the
        /// <see cref="VisualSource"/> and its <see cref="RootVisual"/>. The necessary cross-thread
        /// marshalling is performed by the <see cref="VisualTarget"/> that is returned by <see
        /// cref="GetCompositionTargetCore"/>.</remarks>

        public VisualSource(HostVisual hostVisual) {
            // VisualTarget constructor performs null check
            _visualTarget = new VisualTarget(hostVisual);
        }

        #endregion
        #region Private Fields

        // property backers
        private bool _isDisposed;
        private readonly VisualTarget _visualTarget;

        #endregion
        #region RootVisual

        /// <summary>
        /// Gets or sets the root <see cref="Visual"/> that is the source of the presentation.
        /// </summary>
        /// <value>
        /// The root <see cref="Visual"/> that is the source of the presentation. The default is a
        /// null reference.</value>
        /// <remarks>
        /// <b>RootVisual</b> must reside on the same thread as the <see cref="VisualSource"/>.
        /// </remarks>

        public override Visual RootVisual {
            get { return _visualTarget.RootVisual; }
            set {
                // notify PresentationSource of changed Visual
                Visual oldRoot = _visualTarget.RootVisual;
                _visualTarget.RootVisual = value;
                RootChanged(oldRoot, value);

                // perform layout if supported
                UIElement rootElement = value as UIElement;
                if (rootElement != null) {
                    rootElement.Measure(new Size(Double.PositiveInfinity,
                                                 Double.PositiveInfinity));
                    rootElement.Arrange(new Rect(rootElement.DesiredSize));
                }
            }
        }

        #endregion
        #region GetCompositionTargetCore

        /// <summary>
        /// Gets the <see cref="CompositionTarget"/> that manages visual composition.</summary>
        /// <returns>
        /// The <see cref="VisualTarget"/> that connects source and target of the presentation.
        /// </returns>

        protected override CompositionTarget GetCompositionTargetCore() {
            return _visualTarget;
        }

        #endregion
        #region IDisposable Members
        #region IsDisposed

        /// <summary>
        /// Gets a value indicating whether the <see cref="VisualSource"/> has been disposed of.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Dispose"/> has been called; otherwise, <c>false</c>.</value>

        public override bool IsDisposed {
            [DebuggerStepThrough]
            get { return _isDisposed; }
        }

        #endregion
        #region Dispose

        /// <summary>
        /// Releases all resources used by the <see cref="VisualSource"/>.</summary>
        /// <remarks>
        /// <b>Dispose</b> calls <see cref="CompositionTarget.Dispose"/> on the <see
        /// cref="VisualTarget"/> that manages visual composition.</remarks>

        public void Dispose() {
            if (!_isDisposed) {
                _isDisposed = true;
                _visualTarget.Dispose();
            }
        }

        #endregion
        #endregion
    }
}
