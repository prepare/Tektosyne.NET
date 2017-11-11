using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Tektosyne.Net {

    /// <summary>
    /// The exception that is thrown when an error occurs while using the MAPI or Simple MAPI e-mail
    /// protocols.</summary>
    /// <remarks>
    /// <b>MapiException</b> is thrown when an error occurs within a method of class <see
    /// cref="MapiMail"/>, which provides a wrapper for the Win32 API calls defined in the <see
    /// cref="Win32Api.Mapi"/> class.</remarks>

    [Serializable]
    public class MapiException: Exception {
        #region MapiException()

        /// <overloads>
        /// Initializes a new instance of the <see cref="MapiException"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="MapiException"/> class with default
        /// properties.</summary>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="MapiException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Code"/></term>
        /// <description><see cref="Win32Api.MapiError.SUCCESS_SUCCESS"/> (zero).</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Exception.Message"/></term>
        /// <description>A localized message indicating a successful operation.</description>
        /// </item></list></remarks>

        public MapiException(): this(Win32Api.MapiError.SUCCESS_SUCCESS) { }

        #endregion
        #region MapiException(String)

        /// <summary>
        /// Initializes a new instance of the <see cref="MapiException"/> class with the specified
        /// error message.</summary>
        /// <param name="message">
        /// An error message that specifies the reason for the exception.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="MapiException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Code"/></term>
        /// <description><see cref="Win32Api.MapiError.SUCCESS_SUCCESS"/> (zero).</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Exception.Message"/></term>
        /// <description>The specified <paramref name="message"/>.</description>
        /// </item></list></remarks>

        public MapiException(string message): base(message) {
            _code = Win32Api.MapiError.SUCCESS_SUCCESS;
        }

        #endregion
        #region MapiException(String, Exception)

        /// <summary>
        /// Initializes a new instance of the <see cref="MapiException"/> class with the specified
        /// error message and with the previous exception that is the cause of this <see
        /// cref="MapiException"/>.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current <see
        /// cref="MapiException"/>.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="MapiException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Code"/></term>
        /// <description><see cref="Win32Api.MapiError.SUCCESS_SUCCESS"/> (zero).</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>The specified <paramref name="innerException"/>.</description>
        /// </item><item>
        /// <term><see cref="Exception.Message"/></term>
        /// <description>The specified <paramref name="message"/>.</description>
        /// </item></list></remarks>

        public MapiException(string message, Exception innerException):
            base(message, innerException) {

            _code = Win32Api.MapiError.SUCCESS_SUCCESS;
        }

        #endregion
        #region MapiException(MapiError)

        /// <summary>
        /// Initializes a new instance of the <see cref="MapiException"/> class with the specified
        /// MAPI error code.</summary>
        /// <param name="code">
        /// The code of the MAPI or Simple MAPI error that caused the exception.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="MapiException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Code"/></term>
        /// <description>The specified <paramref name="code"/> which should be one of the error
        /// codes defined in <see cref="Win32Api.MapiError"/>.</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Exception.Message"/></term>
        /// <description>A localized description of the specified <paramref name="code"/>, followed
        /// by the numerical <paramref name="code"/> itself.</description>
        /// </item></list></remarks>

        public MapiException(Win32Api.MapiError code):
            base(MapiMail.GetErrorStringAndCode(code)) {

            _code = code;
        }

        #endregion
        #region MapiException(MapiError, String)

        /// <summary>
        /// Initializes a new instance of the <see cref="MapiException"/> class with the specified
        /// error message and MAPI error code.</summary>
        /// <param name="code">
        /// The code of the MAPI or Simple MAPI error that caused the exception.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="MapiException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Code"/></term>
        /// <description>The specified <paramref name="code"/> which should be one of the error
        /// codes defined in <see cref="Win32Api.MapiError"/>.</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Exception.Message"/></term>
        /// <description>The specified <paramref name="message"/>.</description>
        /// </item></list></remarks>

        public MapiException(Win32Api.MapiError code, string message): base(message) {
            _code = code;
        }

        #endregion
        #region MapiException(MapiError, String, Exception)

        /// <summary>
        /// Initializes a new instance of the <see cref="MapiException"/> class with the specified
        /// error message and MAPI error code, and with the previous exception that is the cause of
        /// this <see cref="MapiException"/>.</summary>
        /// <param name="code">
        /// The code of the MAPI or Simple MAPI error that caused the exception.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current <see
        /// cref="MapiException"/>.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="MapiException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Code"/></term>
        /// <description>The specified <paramref name="code"/> which should be one of the error
        /// codes defined in <see cref="Win32Api.MapiError"/>.</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>The specified <paramref name="innerException"/>.</description>
        /// </item><item>
        /// <term><see cref="Exception.Message"/></term>
        /// <description>The specified <paramref name="message"/>.</description>
        /// </item></list></remarks>

        public MapiException(Win32Api.MapiError code, string message, Exception innerException):
            base(message, innerException) {

            _code = code;
        }

        #endregion
        #region MapiException(SerializationInfo, StreamingContext)

        /// <summary>
        /// Initializes a new instance of the <see cref="MapiException"/> class with serialized
        /// data.</summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object providing serialized object data for the <see
        /// cref="MapiException"/>.</param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> object containing contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is a null reference.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="Exception(SerializationInfo, StreamingContext)"/> for
        /// details.
        /// </para><para>
        /// The value of the <see cref="Code"/> property is deserialized from an additional field,
        /// named "Code".</para></remarks>

        protected MapiException(SerializationInfo info, StreamingContext context):
            base(info, context) {

            _code = (Win32Api.MapiError) info.GetInt32("Code");
        }

        #endregion
        #region Private Fields

        // property backers
        private readonly Win32Api.MapiError _code;

        #endregion
        #region Abort

        /// <summary>
        /// The <see cref="Code"/> that is set when the user aborted the current MAPI operation.
        /// </summary>
        /// <remarks>
        /// When the user aborts a MAPI operation, the <see cref="MapiMail"/> methods communicate
        /// this fact back to the client by throwing a <see cref="MapiException"/> whose <see
        /// cref="Code"/> is <see cref="Win32Api.MapiError.MAPI_E_USER_ABORT"/>. <b>Abort</b>
        /// provides a convenient shortcut for this value.</remarks>

        public const Win32Api.MapiError Abort = Win32Api.MapiError.MAPI_E_USER_ABORT;

        #endregion
        #region Code

        /// <summary>
        /// Gets the MAPI error code of the exception.</summary>
        /// <value>
        /// The code of the MAPI or Simple MAPI error that caused the <see cref="MapiException"/>.
        /// </value>
        /// <remarks>
        /// <see cref="MapiException"/> objects are usually thrown in response to MAPI errors.
        /// <b>Code</b> is one of the error codes defined in <see cref="Win32Api.MapiError"/> which
        /// are identical to the constants defined in file MAPICode.h of the Win32 Platform SDK.
        /// </remarks>

        public Win32Api.MapiError Code {
            [DebuggerStepThrough]
            get { return _code; }
        }

        #endregion
        #region ISerializable Members

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> object with the data needed to serialize the
        /// exception.</summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object that receives the serialized object data of
        /// the <see cref="MapiException"/>.</param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> object containing contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is a null reference.</exception>
        /// <remarks><para>
        /// The value of the <see cref="Code"/> property is serialized to an additional field, named
        /// "Code".</para></remarks>

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {

            base.GetObjectData(info, context);
            info.AddValue("Code", (int) _code);
        }

        #endregion
    }
}
