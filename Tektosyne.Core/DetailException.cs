using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

using Tektosyne.Xml;

namespace Tektosyne {

    /// <summary>
    /// Represents errors that occur during application execution, providing additional information
    /// on technical details.</summary>
    /// <remarks><para>
    /// <b>DetailException</b> extends the <see cref="Exception"/> class with an additional <see
    /// cref="DetailException.Detail"/> property that holds technical details about the error.
    /// </para><para>
    /// The <b>Detail</b> property is intended to fill the gap between the standard <see
    /// cref="Exception.Message"/> text, which is usually very brief and unspecific, and the full
    /// <see cref="Exception.ToString"/> representation of the exception, which is usually extremely
    /// lengthy and hard to read.</para></remarks>

    [Serializable]
    public class DetailException: Exception {
        #region DetailException()

        /// <overloads>
        /// Initializes a new instance of the <see cref="DetailException"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailException"/> class with default
        /// properties.</summary>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="DetailException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Detail"/></term>
        /// <description>An empty string.</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Exception.Message"/></term>
        /// <description>A system-supplied localized description.</description>
        /// </item></list></remarks>

        public DetailException(): base() { }

        #endregion
        #region DetailException(String)

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailException"/> class with the specified
        /// error message.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="DetailException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Detail"/></term>
        /// <description>An empty string.</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Exception.Message"/></term>
        /// <description>The specified <paramref name="message"/>.</description>
        /// </item></list></remarks>

        public DetailException(string message): base(message) { }

        #endregion
        #region DetailException(String, Exception)

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailException"/> class with the specified
        /// error message and with the previous exception that is the cause of this <see
        /// cref="DetailException"/>.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current <see
        /// cref="DetailException"/>.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="DetailException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Detail"/></term>
        /// <description>The value of the <see cref="Exception.Message"/> property of the specified
        /// <paramref name="innerException"/>.</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>The specified <paramref name="innerException"/>.</description>
        /// </item><item>
        /// <term><see cref="Exception.Message"/></term>
        /// <description>The specified <paramref name="message"/>.</description>
        /// </item></list><para>
        /// The constructor calls <see cref="XmlUtility.GetXmlMessage"/> to retrieve the value of
        /// the <see cref="Exception.Message"/> property of the specified <paramref
        /// name="innerException"/>. This means that a localized error location statement will be
        /// prepended to the new value of the <see cref="Detail"/> property if <paramref
        /// name="innerException"/> is an object of type <see cref="System.Xml.XmlException"/> or
        /// <see cref="System.Xml.Schema.XmlSchemaException"/>.</para></remarks>

        public DetailException(string message, Exception innerException):
            base(message, innerException) {

            // store inner exception message in Detail property
            _detail = XmlUtility.GetXmlMessage(innerException);
        }

        #endregion
        #region DetailException(String, String)

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailException"/> class with the specified
        /// error message and technical details.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="details">
        /// A message that provides technical details about the exception.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="DetailException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Detail"/></term>
        /// <description>The specified <paramref name="details"/>.</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Exception.Message"/></term>
        /// <description>The specified <paramref name="message"/>.</description>
        /// </item></list></remarks>

        public DetailException(string message, string details): base(message) {
            _detail = details;
        }

        #endregion
        #region DetailException(String, String, Exception)

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailException"/> class with the specified
        /// error message and technical details, and with the previous exception that is the cause
        /// of this <see cref="DetailException"/>.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="details">
        /// A message that provides technical details about the exception.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current <see
        /// cref="DetailException"/>.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="DetailException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Detail"/></term>
        /// <description>The specified <paramref name="details"/>.</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>The specified <paramref name="innerException"/>.</description>
        /// </item><item>
        /// <term><see cref="Exception.Message"/></term>
        /// <description>The specified <paramref name="message"/>.</description>
        /// </item></list></remarks>

        public DetailException(string message, string details, Exception innerException):
            base(message, innerException) {

            _detail = details;
        }

        #endregion
        #region DetailException(SerializationInfo, StreamingContext)

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailException"/> class with serialized
        /// data.</summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object providing serialized object data for the <see
        /// cref="DetailException"/>.</param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> object containing contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is a null reference.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="Exception(SerializationInfo, StreamingContext)"/> for
        /// details.
        /// </para><para>
        /// The value of the <see cref="Detail"/> property is deserialized from an additional field,
        /// named "Detail".</para></remarks>

        protected DetailException(SerializationInfo info, StreamingContext context):
            base(info, context) {

            _detail = info.GetString("Detail");
        }

        #endregion
        #region Private Fields

        // backer for Detail property
        private string _detail;

        #endregion
        #region Detail

        /// <summary>
        /// Gets technical details about the exception.</summary>
        /// <value>
        /// A <see cref="String"/> containing technical details about the <see
        /// cref="DetailException"/>. The default is an empty string.</value>
        /// <remarks>
        /// <b>Detail</b> returns an empty string when set to a null reference during construction.
        /// </remarks>

        public string Detail {
            [DebuggerStepThrough]
            get { return _detail ?? ""; }
        }

        #endregion
        #region ISerializable Members

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> object with the data needed to serialize the
        /// exception.</summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object that receives the serialized object data of
        /// the <see cref="DetailException"/>.</param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> object containing contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is a null reference.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="Exception.GetObjectData"/> for details.
        /// </para><para>
        /// The value of the <see cref="Detail"/> property is serialized to an additional field,
        /// named "Detail".</para></remarks>

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {

            base.GetObjectData(info, context);
            info.AddValue("Detail", _detail);
        }

        #endregion
    }
}
