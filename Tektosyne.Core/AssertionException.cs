using System;
using System.Runtime.Serialization;

namespace Tektosyne {

    /// <summary>
    /// Represents errors that occur as a result of assertion failures.</summary>
    /// <remarks>
    /// <b>AssertionException</b> is identical to the basic <see cref="Exception"/> class. Its sole
    /// purpose is to identify exceptions thrown by the <see cref="ThrowHelper.Assert"/> method.
    /// </remarks>

    [Serializable]
    public class AssertionException: Exception {
        #region AssertionException()

        /// <overloads>
        /// Initializes a new instance of the <see cref="AssertionException"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionException"/> class with default
        /// properties.</summary>
        /// <remarks>
        /// Please refer to <see cref="Exception()"/> for details.</remarks>

        public AssertionException(): base() { }

        #endregion
        #region AssertionException(String)

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionException"/> class with the
        /// specified error message.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <remarks>
        /// Please refer to <see cref="Exception(String)"/> for details.</remarks>

        public AssertionException(string message): base(message) { }

        #endregion
        #region AssertionException(String, Exception)

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionException"/> class with the
        /// specified error message and with the previous exception that is the cause of this <see
        /// cref="AssertionException"/>.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current <see
        /// cref="AssertionException"/>.</param>
        /// <remarks>
        /// Please refer to <see cref="Exception(String, Exception)"/> for details.</remarks>

        public AssertionException(string message, Exception innerException):
            base(message, innerException) { }

        #endregion
        #region AssertionException(SerializationInfo, StreamingContext)

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionException"/> class with serialized
        /// data.</summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object providing serialized object data for the <see
        /// cref="AssertionException"/>.</param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> object containing contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="Exception(SerializationInfo, StreamingContext)"/> for
        /// details.</remarks>

        protected AssertionException(SerializationInfo info, StreamingContext context):
            base(info, context) {
        }

        #endregion
    }
}
