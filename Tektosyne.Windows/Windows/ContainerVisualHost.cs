using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Tektosyne.Windows {

    /// <summary>
    /// Provides a <see cref="FrameworkElement"/> that hosts a single <see cref="ContainerVisual"/>
    /// child element.</summary>
    /// <remarks>
    /// <b>ContainerVisualHost</b> is a simple adapter class for hosting <see
    /// cref="ContainerVisual"/> objects within WPF data structures that accept only <see
    /// cref="UIElement"/> or <see cref="FrameworkElement"/> objects.</remarks>

    public class ContainerVisualHost: FrameworkElement {
        #region ContainerVisualHost()

        /// <overloads>
        /// Initializes a new instance of the <see cref="ContainerVisualHost"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerVisualHost"/> class with default
        /// properties.</summary>
        /// <remarks>
        /// The <see cref="VisualChild"/> property is set to an empty <see cref="DrawingVisual"/>
        /// object.</remarks>

        public ContainerVisualHost() {
            VisualChild = new DrawingVisual();
        }

        #endregion
        #region ContainerVisualHost(ContainerVisual)

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerVisualHost"/> class with the
        /// specified <see cref="ContainerVisual"/> object.</summary>
        /// <param name="visualChild">
        /// The single visual child element of the <see cref="ContainerVisualHost"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="visualChild"/> is a null reference.</exception>

        public ContainerVisualHost(ContainerVisual visualChild) {
            VisualChild = visualChild;
        }

        #endregion
        #region Private Fields

        // property backers
        private ContainerVisual _visualChild;

        #endregion
        #region VisualChild

        /// <summary>
        /// Gets or sets the single visual child element of the <see cref="ContainerVisualHost"/>.
        /// </summary>
        /// <value>
        /// The single visual child element of the <see cref="ContainerVisualHost"/>. The default is
        /// an empty <see cref="DrawingVisual"/> object.</value>
        /// <exception cref="ArgumentNullException">
        /// The property is set to a null reference.</exception>

        public ContainerVisual VisualChild {
            [DebuggerStepThrough]
            get { return _visualChild; }
            set {
                if (value == null)
                    ThrowHelper.ThrowArgumentNullException("value");

                if (_visualChild != null) {
                    RemoveVisualChild(_visualChild);
                    RemoveLogicalChild(_visualChild);
                }

                _visualChild = value;

                AddVisualChild(_visualChild);
                AddLogicalChild(_visualChild);
            }
        }

        #endregion
        #region VisualChildrenCount

        /// <summary>
        /// Gets the number of visual children of the <see cref="ContainerVisualHost"/>.</summary>
        /// <value>
        /// The number of visual children of the <see cref="ContainerVisualHost"/>.</value>
        /// <remarks>
        /// <b>VisualChildrenCount</b> returns the constant value one, as the <see
        /// cref="ContainerVisualHost"/> always directly contains only a single <see
        /// cref="VisualChild"/>.</remarks>

        protected override int VisualChildrenCount {
            get { return 1; }
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
        /// <b>GetVisualChild</b> returns the value of the <see cref="VisualChild"/> property if the
        /// specified <paramref name="index"/> equals zero.</remarks>

        protected override Visual GetVisualChild(int index) {
            if (index != 0)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "index", index, Tektosyne.Strings.ArgumentNotEquals, "0");

            return VisualChild;
        }

        #endregion
        #region MeasureOverride

        /// <summary>
        /// Measures the layout size required for the <see cref="ContainerVisualHost"/>.</summary>
        /// <param name="availableSize">
        /// The layout size available for the <see cref="ContainerVisualHost"/>.</param>
        /// <returns>
        /// The layout size required for the <see cref="ContainerVisualHost"/>.</returns>
        /// <remarks>
        /// <b>MeasureOverride</b> always returns the <see cref="ContainerVisual.ContentBounds"/>
        /// size of the hosted <see cref="VisualChild"/>. The specified <paramref
        /// name="availableSize"/> is ignored.</remarks>

        protected override Size MeasureOverride(Size availableSize) {
            return VisualChild.ContentBounds.Size;
        }

        #endregion
    }
}
