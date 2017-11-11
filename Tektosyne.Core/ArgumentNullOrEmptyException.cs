using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Tektosyne {

    /// <summary>
    /// The exception that is thrown when a null reference or an empty object is passed to a method
    /// that does not accept them as a valid argument.</summary>
    /// <remarks><para>
    /// <b>ArgumentNullOrEmptyException</b> offers the same functionality as the standard class <see
    /// cref="ArgumentNullException"/>.
    /// </para><para>
    /// Methods should raise this exception to indicate an invalid argument of a composite type that
    /// is either a null reference or does not contain any elements. Such composite types include
    /// the <see cref="String"/> and <see cref="Array"/> classes, as well as any collection classes.
    /// </para></remarks>

    [Serializable]
    public class ArgumentNullOrEmptyException: ArgumentException {
        #region ArgumentNullOrEmptyException()

        /// <overloads>
        /// Initializes a new instance of the <see cref="ArgumentNullOrEmptyException"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullOrEmptyException"/> class with
        /// default properties.</summary>
        /// <remarks>
        /// Please refer to <see cref="ArgumentNullException()"/> for details.</remarks>

        public ArgumentNullOrEmptyException(): base(Strings.ArgumentNullOrEmpty) { }

        #endregion
        #region ArgumentNullOrEmptyException(String)

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullOrEmptyException"/> class with
        /// the name of the parameter that caused the exception.</summary>
        /// <param name="paramName">
        /// The name of the parameter that caused the exception.</param>
        /// <remarks>
        /// Please refer to <see cref="ArgumentNullException(String)"/> for details.</remarks>

        public ArgumentNullOrEmptyException(string paramName):
            base(Strings.ArgumentNullOrEmpty, paramName) { }

        #endregion
        #region ArgumentNullOrEmptyException(String, Exception)

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullOrEmptyException"/> class with
        /// the specified error message and with the previous exception that is the cause of this
        /// <see cref="ArgumentNullOrEmptyException"/>.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current <see
        /// cref="ArgumentNullOrEmptyException"/>.</param>
        /// <remarks>
        /// Please refer to <see cref="ArgumentNullException(String, Exception)"/> for details.
        /// </remarks>

        public ArgumentNullOrEmptyException(string message, Exception innerException):
            base(message, innerException) { }

        #endregion
        #region ArgumentNullOrEmptyException(String, String)


        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullOrEmptyException"/> class with
        /// the name of the parameter that caused the exception and with the specified error
        /// message.</summary>
        /// <param name="paramName">
        /// The name of the parameter that caused the exception.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <remarks>
        /// Please refer to <see cref="ArgumentNullException(String, String)"/> for details.
        /// </remarks>

        public ArgumentNullOrEmptyException(string paramName, string message):
            base(message, paramName) { }

        #endregion
        #region ArgumentNullOrEmptyException(SerializationInfo, StreamingContext)

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullOrEmptyException"/> class with
        /// serialized data.</summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object providing serialized object data for the <see
        /// cref="ArgumentNullOrEmptyException"/>.</param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> object containing contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is a null reference.</exception>
        /// <remarks>
        /// Please refer to <see cref="ArgumentNullException(SerializationInfo,
        /// StreamingContext)"/> for details.</remarks>

        protected ArgumentNullOrEmptyException(SerializationInfo info, StreamingContext context):
            base(info, context) { }

        #endregion
    }
}
