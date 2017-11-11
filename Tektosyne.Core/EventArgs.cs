using System;
using System.Globalization;

namespace Tektosyne {

    /// <summary>
    /// Provides an event argument that contains a single arbitrary value.</summary>
    /// <typeparam name="T">
    /// The type of the <see cref="EventArgs{T}.Value"/> transmitted by the event.</typeparam>
    /// <remarks>
    /// <b>EventArgs</b> extends the standard <see cref="EventArgs"/> class to allow the
    /// transmission of a single arbitrary value when an event is raised.</remarks>

    [Serializable]
    public class EventArgs<T>: EventArgs {
        #region EventArgs(T)

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T}"/> class with the specified
        /// value.</summary>
        /// <param name="value">
        /// The <typeparamref name="T"/> object transmitted by the event.</param>

        public EventArgs(T value) {
            Value = value;
        }

        #endregion
        #region Value

        /// <summary>
        /// The <typeparamref name="T"/> object transmitted by the event.</summary>

        public readonly T Value;

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="EventArgs{T}"/> object.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> containing the culture-invariant string representations of the
        /// instance type <typeparamref name="T"/> and of the <see cref="Value"/> property.
        /// </returns>

        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture,
                "EventArgs<{0}> (Value = {1})", typeof(T), StringUtility.Validate<T>(Value));
        }

        #endregion
    }
}
