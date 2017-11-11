using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Tektosyne.Collections {

    /// <summary>
    /// The exception that is thrown when dictionary and <see cref="IKeyedValue{TKey}"/> keys are
    /// mismatched.</summary>
    /// <remarks><para>
    /// <b>KeyMismatchException</b> extends the <see cref="InvalidOperationException"/> class with
    /// two additional properties, <see cref="KeyMismatchException.Key"/> and <see
    /// cref="KeyMismatchException.ValueKey"/>, holding the mismatched key values that caused the
    /// exception.
    /// </para><para>
    /// <b>KeyMismatchException</b> is thrown by the generic dictionary classes in the 
    /// <b>Tektosyne.Collections</b> namespace when the user attempts to associate an <see
    /// cref="IKeyedValue{TKey}"/> instance with a dictionary <see cref="KeyValuePair{TKey,
    /// TValue}.Key"/> that differs from the object’s own <see cref="IKeyedValue{T}.Key"/>.
    /// </para></remarks>

    [Serializable]
    public class KeyMismatchException: InvalidOperationException {
        #region KeyMismatchException()

        /// <overloads>
        /// Initializes a new instance of the <see cref="KeyMismatchException"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMismatchException"/> class with default
        /// properties.</summary>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="KeyMismatchException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Message"/></term>
        /// <description>A localized message indicating mismatched key values.</description>
        /// </item><item>
        /// <term><see cref="Key"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="ValueKey"/></term>
        /// <description>A null reference.</description>
        /// </item></list></remarks>

        public KeyMismatchException(): this(null, null, null) { }

        #endregion
        #region KeyMismatchException(Object, Object)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMismatchException"/> class with the
        /// mismatched key values that caused the exception.</summary>
        /// <param name="key">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> that was found or stored in a <see
        /// cref="KeyValuePair{TKey, TValue}"/> associated with <paramref name="valueKey"/>.</param>
        /// <param name="valueKey">
        /// The <see cref="IKeyedValue{TKey}.Key"/> that was found or stored in an <see
        /// cref="IKeyedValue{TKey}"/> instance associated with <paramref name="key"/>.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="KeyMismatchException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Message"/></term>
        /// <description>A localized message, followed by the specified <paramref name="key"/> and
        /// <paramref name="valueKey"/> values.</description>
        /// </item><item>
        /// <term><see cref="Key"/></term>
        /// <description>The specified <paramref name="key"/>.</description>
        /// </item><item>
        /// <term><see cref="ValueKey"/></term>
        /// <description>The specified <paramref name="valueKey"/>.</description>
        /// </item></list></remarks>

        public KeyMismatchException(object key, object valueKey): this(key, valueKey, null) { }

        #endregion
        #region KeyMismatchException(Object, Object, String)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMismatchException"/> class with the
        /// mismatched key values that caused the exception, and with the specified error message.
        /// </summary>
        /// <param name="key">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> that was found or stored in a <see
        /// cref="KeyValuePair{TKey, TValue}"/> associated with <paramref name="valueKey"/>.</param>
        /// <param name="valueKey">
        /// The <see cref="IKeyedValue{TKey}.Key"/> that was found or stored in an <see
        /// cref="IKeyedValue{TKey}"/> instance associated with <paramref name="key"/>.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="KeyMismatchException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Message"/></term>
        /// <description>The specified <paramref name="message"/>, followed by the specified
        /// <paramref name="key"/> and <paramref name="valueKey"/>.</description>
        /// </item><item>
        /// <term><see cref="Key"/></term>
        /// <description>The specified <paramref name="key"/>.</description>
        /// </item><item>
        /// <term><see cref="ValueKey"/></term>
        /// <description>The specified <paramref name="valueKey"/>.</description>
        /// </item></list><para>
        /// If the specified <paramref name="message"/> is a null reference or an empty string, the
        /// <b>Message</b> property will contain a localized message indicating mismatched key
        /// values.</para></remarks>

        public KeyMismatchException(object key, object valueKey, string message):
            base(StringUtility.Validate(message, Strings.DictionaryKeyMismatch)) {

            _key = key;
            _valueKey = valueKey;
        }

        #endregion
        #region KeyMismatchException(String)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMismatchException"/> class with the
        /// specified error message.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="KeyMismatchException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="Message"/></term>
        /// <description>The specified <paramref name="message"/>.</description>
        /// </item><item>
        /// <term><see cref="Key"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="ValueKey"/></term>
        /// <description>A null reference.</description>
        /// </item></list></remarks>

        public KeyMismatchException(string message): base(message) { }

        #endregion
        #region KeyMismatchException(String, Exception)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMismatchException"/> class with the
        /// specified error message and with the previous exception that is the cause of this <see
        /// cref="KeyMismatchException"/>.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current <see
        /// cref="KeyMismatchException"/>.</param>
        /// <remarks><para>
        /// The following table shows the initial property values for the new instance of <see
        /// cref="KeyMismatchException"/>:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="Exception.InnerException"/></term>
        /// <description>The specified <paramref name="innerException"/>.</description>
        /// </item><item>
        /// <term><see cref="Message"/></term>
        /// <description>The specified <paramref name="message"/>.</description>
        /// </item><item>
        /// <term><see cref="Key"/></term>
        /// <description>A null reference.</description>
        /// </item><item>
        /// <term><see cref="ValueKey"/></term>
        /// <description>A null reference.</description>
        /// </item></list></remarks>

        public KeyMismatchException(string message, Exception innerException):
            base(message, innerException) { }

        #endregion
        #region KeyMismatchException(SerializationInfo, StreamingContext)

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMismatchException"/> class with
        /// serialized data.</summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object providing serialized object data for the <see
        /// cref="KeyMismatchException"/>.</param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> object containing contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is a null reference.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="InvalidOperationException(SerializationInfo,
        /// StreamingContext)"/> for details.
        /// </para><para>
        /// The values of the <see cref="Key"/> and <see cref="ValueKey"/> properties are
        /// deserialized from two additional fields, named "Key" and "ValueKey".</para></remarks>

        protected KeyMismatchException(SerializationInfo info, StreamingContext context):
            base(info, context) {

            _key = info.GetValue("Key", typeof(object));
            _valueKey = info.GetValue("ValueKey", typeof(object));
        }

        #endregion
        #region Private Fields

        // property backers
        private readonly object _key, _valueKey;

        #endregion
        #region Key

        /// <summary>
        /// Gets the dictionary key that caused the exception.</summary>
        /// <value>
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> of a <see cref="KeyValuePair{TKey,
        /// TValue}"/> that caused the <see cref="KeyMismatchException"/>.</value>

        public object Key {
            [DebuggerStepThrough]
            get { return _key; }
        }

        #endregion
        #region Message

        /// <summary>
        /// Gets the error message, followed by the mismatched key values if available.</summary>
        /// <value>
        /// The error message passed to the constructor, followed by the values of the <see
        /// cref="Key"/> and <see cref="ValueKey"/> properties if they are not null references.
        /// </value>
        /// <remarks>
        /// The error message should be localized.</remarks>

        public override string Message {
            get {
                if (_key == null && _valueKey == null)
                    return base.Message;

                StringBuilder sb = new StringBuilder();
                sb.Append(base.Message);

                if (_key != null) {
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat(Strings.DictionaryKey, _key);
                }

                if (_valueKey != null) {
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat(Strings.IKeyedValueKey, _valueKey);
                }

                return sb.ToString();
            }
        }

        #endregion
        #region ValueKey

        /// <summary>
        /// Gets the <see cref="IKeyedValue{TKey}"/> key that caused the exception.</summary>
        /// <value>
        /// The <see cref="IKeyedValue{TKey}.Key"/> of an <see cref="IKeyedValue{TKey}"/> instance
        /// that caused the <see cref="KeyMismatchException"/>.</value>

        public object ValueKey {
            [DebuggerStepThrough]
            get { return _valueKey; }
        }

        #endregion
        #region ISerializable Members

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> object with the data needed to serialize the
        /// exception.</summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object that receives the serialized object data of
        /// the <see cref="KeyMismatchException"/>.</param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> object containing contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is a null reference.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="Exception.GetObjectData"/> for details.
        /// </para><para>
        /// The values of the <see cref="Key"/> and <see cref="ValueKey"/> properties are serialized
        /// to two additional fields, named "Key" and "ValueKey".</para></remarks>

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {

            base.GetObjectData(info, context);
            info.AddValue("Key", _key);
            info.AddValue("ValueKey", _valueKey);
        }

        #endregion
    }
}
