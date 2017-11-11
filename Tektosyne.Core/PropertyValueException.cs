using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Tektosyne {

    /// <summary>
    /// The exception that is thrown when a method call is invalid for the current value of a
    /// property.</summary>
    /// <remarks><para>
    /// <b>PropertyValueException</b> extends the <see cref="InvalidOperationException"/> class with
    /// two additional properties, <see cref="PropertyValueException.PropertyName"/> and <see
    /// cref="PropertyValueException.ActualValue"/>, holding the name and value of the property that
    /// caused the exception.
    /// </para><para>
    /// <b>PropertyValueException</b> duplicates the functionality of <see
    /// cref="ArgumentOutOfRangeException"/> for errors that are caused by invalid property values.
    /// In my experience, this is the most common reason for <b>InvalidOperationException</b>
    /// errors.</para></remarks>

    [Serializable]
    public class PropertyValueException: InvalidOperationException {
        #region PropertyValueException()

        /// <overloads>
        /// Initializes a new instance of the <see cref="PropertyValueException"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueException"/> class with
        /// default properties.</summary>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="PropertyValueException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Message"/></term>
        /// <description>A localized message indicating an invalid property value.</description>
        /// </item><item>
        /// <term><see cref="PropertyName"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="ActualValue"/></term>
        /// <description>A null reference.</description>
        /// </item></list></remarks>

        public PropertyValueException(): this(null, null, null) { }

        #endregion
        #region PropertyValueException(String)

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueException"/> class with the
        /// name of the property that caused the exception.</summary>
        /// <param name="propertyName">
        /// The name of the property that caused the exception.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="PropertyValueException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="ActualValue"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Message"/></term>
        /// <description>A localized message indicating an invalid property value.</description>
        /// </item><item>
        /// <term><see cref="PropertyName"/></term>
        /// <description>The specified <paramref name="propertyName"/>.</description>
        /// </item></list></remarks>

        public PropertyValueException(string propertyName): this(propertyName, null, null) { }

        #endregion
        #region PropertyValueException(String, Exception)

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueException"/> class with the
        /// specified error message and with the previous exception that is the cause of this <see
        /// cref="PropertyValueException"/>.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current <see
        /// cref="PropertyValueException"/>.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="PropertyValueException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>The specified <paramref name="innerException"/>.</description>
        /// </item><item>
        /// <term><see cref="Message"/></term>
        /// <description>The specified <paramref name="message"/>.</description>
        /// </item><item>
        /// <term><see cref="PropertyName"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="ActualValue"/></term>
        /// <description>A null reference.</description>
        /// </item></list></remarks>

        public PropertyValueException(string message, Exception innerException):
            base(message, innerException) { }

        #endregion
        #region PropertyValueException(String, String)

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueException"/> class with the
        /// name of the property that caused the exception and with the specified error message.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that caused the exception.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="PropertyValueException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="ActualValue"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Message"/></term>
        /// <description>The specified <paramref name="message"/>, followed by the specified
        /// <paramref name="propertyName"/>.</description>
        /// </item><item>
        /// <term><see cref="PropertyName"/></term>
        /// <description>The specified <paramref name="propertyName"/>.</description>
        /// </item></list><para>
        /// If the specified <paramref name="message"/> is a null reference or an empty string, the
        /// <b>Message</b> property will contain a localized message indicating an invalid property
        /// value.</para></remarks>

        public PropertyValueException(string propertyName, string message):
            this(propertyName, null, message) { }

        #endregion
        #region PropertyValueException(String, Object, String)

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueException"/> class with the
        /// name and value of the property that caused the exception and with the specified error
        /// message.</summary>
        /// <param name="propertyName">
        /// The name of the property that caused the exception.</param>
        /// <param name="actualValue">
        /// The value of the property that caused the exception.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="PropertyValueException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="ActualValue"/></term>
        /// <description>The specified <paramref name="actualValue"/>.</description>
        /// </item><item>
        /// <term><see cref="Message"/></term>
        /// <description>The specified <paramref name="message"/>, followed by the specified
        /// <paramref name="propertyName"/> and <paramref name="actualValue"/>.</description>
        /// </item><item>
        /// <term><see cref="PropertyName"/></term>
        /// <description>The specified <paramref name="propertyName"/>.</description>
        /// </item></list><para>
        /// If the specified <paramref name="message"/> is a null reference or an empty string, the
        /// <b>Message</b> property will contain a localized message indicating an invalid property
        /// value.</para></remarks>

        public PropertyValueException(string propertyName, object actualValue, string message):
            base(StringUtility.Validate(message, Strings.PropertyInvalidValue)) {

            _actualValue = actualValue;
            _propertyName = propertyName;
        }

        #endregion
        #region PropertyValueException(SerializationInfo, StreamingContext)

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueException"/> class with
        /// serialized data.</summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object providing serialized object data for the <see
        /// cref="PropertyValueException"/>.</param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> object containing contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is a null reference.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="InvalidOperationException(SerializationInfo,
        /// StreamingContext)"/> for details.
        /// </para><para>
        /// The values of the <see cref="ActualValue"/> and <see cref="PropertyName"/> properties
        /// are deserialized from two additional fields, named "ActualValue" and "PropertyName".
        /// </para></remarks>

        protected PropertyValueException(SerializationInfo info, StreamingContext context):
            base(info, context) {

            _actualValue = info.GetValue("ActualValue", typeof(object));
            _propertyName = info.GetString("PropertyName");
        }

        #endregion
        #region Private Fields

        // property backers
        private readonly object _actualValue;
        private readonly string _propertyName;

        #endregion
        #region ActualValue

        /// <summary>
        /// Gets the value of the property that caused the exception.</summary>
        /// <value>
        /// The invalid property value that caused the <see cref="PropertyValueException"/>.</value>
        /// <remarks>
        /// A <see cref="PropertyValueException"/> should carry the invalid property value that
        /// caused the exception if it is relevant and cannot be inferred from the error <see
        /// cref="Message"/>.</remarks>

        public object ActualValue {
            [DebuggerStepThrough]
            get { return _actualValue; }
        }

        #endregion
        #region Message

        /// <summary>
        /// Gets the error message, followed by the property name and value if available.</summary>
        /// <value>
        /// The error message passed to the constructor, followed by the <see cref="PropertyName"/>
        /// if it is not a null reference or an empty string, and by the <see cref="ActualValue"/>
        /// if it is not a null reference.</value>
        /// <remarks>
        /// The error message should be localized.</remarks>

        public override string Message {
            get {
                if (String.IsNullOrEmpty(_propertyName))
                    return base.Message;

                StringBuilder sb = new StringBuilder();
                sb.Append(base.Message);

                if (!String.IsNullOrEmpty(_propertyName)) {
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat(Strings.PropertyName, _propertyName);
                }

                if (_actualValue != null) {
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat(Strings.ActualValue, _actualValue);
                }

                return sb.ToString();
            }
        }

        #endregion
        #region PropertyName

        /// <summary>
        /// Gets the name of the property that caused the exception.</summary>
        /// <value>
        /// The name of the property that caused the <see cref="PropertyValueException"/>.</value>
        /// <remarks>
        /// Every <see cref="PropertyValueException"/> should carry the name of the property whose
        /// invalid value caused the exception. This name should not be localized.</remarks>

        public string PropertyName {
            [DebuggerStepThrough]
            get { return _propertyName; }
        }

        #endregion
        #region ISerializable Members

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> object with the data needed to serialize the
        /// exception.</summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object that receives the serialized object data of
        /// the <see cref="PropertyValueException"/>.</param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> object containing contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is a null reference.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="Exception.GetObjectData"/> for details.
        /// </para><para>
        /// The values of the <see cref="ActualValue"/> and <see cref="PropertyName"/> properties
        /// are serialized to two additional fields, named "ActualValue" and "PropertyName".
        /// </para></remarks>

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {

            base.GetObjectData(info, context);
            info.AddValue("ActualValue", _actualValue);
            info.AddValue("PropertyName", _propertyName);
        }

        #endregion
    }
}
