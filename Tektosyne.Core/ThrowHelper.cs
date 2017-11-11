using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using Tektosyne.Collections;

namespace Tektosyne {

    /// <summary>
    /// Provides helper methods for throwing exceptions.</summary>
    /// <remarks><para>
    /// <b>ThrowHelper</b> provides several methods that merely throw the <see cref="Exception"/>
    /// indicated by the method name, with the specified arguments. The purpose of these methods is
    /// to help the JIT compiler generate more efficient machine code.
    /// </para><para>
    /// Every <c>throw</c> statement translates into a surprisingly large amount of machine code –
    /// around 30-60 extra bytes compared to an equivalent <b>ThrowHelper</b> call, depending on the
    /// target platform and optimization settings. Composing error messages with <see
    /// cref="String.Format"/> calls further increases the overhead.
    /// </para><para>
    /// This has two detrimental effects on methods that contain <c>throw</c> statements. First,
    /// they are much more likely to exceed the maximum code size for inlining. Second, they carry a
    /// significant amount of "dead" code that is never run during normal execution but may still
    /// consume system resources.
    /// </para><para>
    /// Using <b>ThrowHelper</b> calls instead of explicit <c>throw</c> statements will translate
    /// your methods into smaller machine code. This should improve overall throughput and may even
    /// render some methods eligible for inlining. Most <b>ThrowHelper</b> methods are marked with
    /// the <see cref="MethodImplOptions.NoInlining"/> option to help guide the JIT compiler.
    /// </para></remarks>

    public static class ThrowHelper {
        #region Assert(Boolean)

        /// <overloads>
        /// Throws an <see cref="AssertionException"/> if the specified condition is <c>false</c>.
        /// </overloads>
        /// <summary>
        /// Throws an <see cref="AssertionException"/> if the specified condition is <c>false</c>.
        /// </summary>
        /// <param name="condition">
        /// The condition to test.</param>
        /// <exception cref="AssertionException">
        /// <paramref name="condition"/> is <c>false</c>.</exception>
        /// <remarks><para>
        /// <b>Assert</b> provides an alternative to the eponymous <see
        /// cref="System.Diagnostics.Debug"/> and <see cref="System.Diagnostics.Trace"/> methods
        /// that does not depend on the DEBUG and TRACE symbols, respectively, and that always
        /// responds to <paramref name="condition"/> failure by throwing an exception.
        /// </para><para>
        /// Unlike other <see cref="ThrowHelper"/> methods, both <b>Assert</b> overloads may be
        /// inlined by the JIT compiler.</para></remarks>

        public static void Assert(bool condition) {
            if (!condition) ThrowAssertionException();
        }

        #endregion
        #region Assert(Boolean, String)

        /// <summary>
        /// Throws an <see cref="AssertionException"/> with the specified error message if the
        /// specified condition is <c>false</c>.</summary>
        /// <param name="condition">
        /// The condition to test.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <exception cref="AssertionException">
        /// <paramref name="condition"/> is <c>false</c>.</exception>
        /// <remarks>
        /// Please refer to <see cref="Assert(Boolean)"/> for details.</remarks>

        public static void Assert(bool condition, string message) {
            if (!condition) ThrowAssertionException(message);
        }

        #endregion
        #region ThrowArgumentException

        /// <summary>
        /// Throws an <see cref="ArgumentException"/>.</summary>
        /// <param name="paramName">
        /// The name of the parameter that caused the exception.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <remarks>
        /// <b>ThrowArgumentException</b> reverses the order of <paramref name="paramName"/> and
        /// <paramref name="message"/> compared to the <see cref="ArgumentException"/> constructor,
        /// for conformance with other <see cref="ThrowHelper"/> methods.</remarks>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentException(string paramName, string message) {
            throw new ArgumentException(message, paramName);
        }

        #endregion
        #region ThrowArgumentExceptionWithFormat(..., Object)

        /// <overloads>
        /// Throws an <see cref="ArgumentException"/> with a formatted error message.</overloads>
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> with a formatted error message and a single
        /// argument.</summary>
        /// <param name="paramName">
        /// The name of the parameter that caused the exception.</param>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="argument">
        /// The argument for the <paramref name="format"/> string.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentExceptionWithFormat(
            string paramName, string format, object argument) {

            string message = String.Format(CultureInfo.CurrentCulture, format, argument);
            throw new ArgumentException(message, paramName);
        }

        #endregion
        #region ThrowArgumentExceptionWithFormat(..., Object, Object)

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> with a formatted error message and two
        /// arguments.</summary>
        /// <param name="paramName">
        /// The name of the parameter that caused the exception.</param>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="arg0">
        /// The first argument for the <paramref name="format"/> string.</param>
        /// <param name="arg1">
        /// The second argument for the <paramref name="format"/> string.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentExceptionWithFormat(
            string paramName, string format, object arg0, object arg1) {

            string message = String.Format(CultureInfo.CurrentCulture, format, arg0, arg1);
            throw new ArgumentException(message, paramName);
        }

        #endregion
        #region ThrowArgumentNullException(String)

        /// <overloads>
        /// Throws an <see cref="ArgumentNullException"/>.</overloads>
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> with the specified parameter name.
        /// </summary>
        /// <param name="paramName">
        /// The name of the parameter that caused the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentNullException(string paramName) {
            throw new ArgumentNullException(paramName);
        }

        #endregion
        #region ThrowArgumentNullException(String, String)

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> with the specified parameter name and
        /// error message.</summary>
        /// <param name="paramName">
        /// The name of the parameter that caused the exception.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentNullException(string paramName, string message) {
            throw new ArgumentNullException(paramName, message);
        }

        #endregion
        #region ThrowArgumentNullOrEmptyException

        /// <summary>
        /// Throws an <see cref="ArgumentNullOrEmptyException"/>.</summary>
        /// <param name="paramName">
        /// The name of the parameter that caused the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentNullOrEmptyException(string paramName) {
            throw new ArgumentNullOrEmptyException(paramName);
        }

        #endregion
        #region ThrowArgumentOutOfRangeException

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/>.</summary>
        /// <param name="paramName">
        /// The name of the parameter that caused the exception.</param>
        /// <param name="actualValue">
        /// The actual value of the parameter that caused the exception.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentOutOfRangeException(
            string paramName, object actualValue, string message) {

            throw new ArgumentOutOfRangeException(paramName, actualValue, message);
        }

        #endregion
        #region ThrowArgumentOutOfRangeExceptionWithFormat(..., Object)

        /// <overloads>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> with a formatted error message.
        /// </overloads>
        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> with a formatted error message and a
        /// single argument.</summary>
        /// <param name="paramName">
        /// The name of the parameter that caused the exception.</param>
        /// <param name="actualValue">
        /// The actual value of the parameter that caused the exception.</param>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="argument">
        /// The argument for the <paramref name="format"/> string.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentOutOfRangeExceptionWithFormat(
            string paramName, object actualValue, string format, object argument) {

            string message = String.Format(CultureInfo.CurrentCulture, format, argument);
            throw new ArgumentOutOfRangeException(paramName, actualValue, message);
        }

        #endregion
        #region ThrowArgumentOutOfRangeExceptionWithFormat(..., Object, Object)

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> with a formatted error message and
        /// two arguments.</summary>
        /// <param name="paramName">
        /// The name of the parameter that caused the exception.</param>
        /// <param name="actualValue">
        /// The actual value of the parameter that caused the exception.</param>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="arg0">
        /// The first argument for the <paramref name="format"/> string.</param>
        /// <param name="arg1">
        /// The second argument for the <paramref name="format"/> string.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentOutOfRangeExceptionWithFormat(
            string paramName, object actualValue, string format, object arg0, object arg1) {

            string message = String.Format(CultureInfo.CurrentCulture, format, arg0, arg1);
            throw new ArgumentOutOfRangeException(paramName, actualValue, message);
        }

        #endregion
        #region ThrowAssertionException()

        /// <overloads>
        /// Throws an <see cref="AssertionException"/>.</overloads>
        /// <summary>
        /// Throws an <see cref="AssertionException"/>.</summary>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowAssertionException() {
            throw new AssertionException();
        }

        #endregion
        #region ThrowAssertionException(String)

        /// <summary>
        /// Throws an <see cref="AssertionException"/> with the specified error message.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowAssertionException(string message) {
            throw new AssertionException(message);
        }

        #endregion
        #region ThrowAssertionException(String, Exception)

        /// <summary>
        /// Throws an <see cref="AssertionException"/> with the specified error message and previous
        /// exception.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowAssertionException(string message, Exception innerException) {
            throw new AssertionException(message, innerException);
        }

        #endregion
        #region ThrowDetailException(String)

        /// <overloads>
        /// Throws a <see cref="DetailException"/>.</overloads>
        /// <summary>
        /// Throws a <see cref="DetailException"/> with the specified error message.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowDetailException(string message) {
            throw new DetailException(message);
        }

        #endregion
        #region ThrowDetailException(String, Exception)

        /// <summary>
        /// Throws a <see cref="DetailException"/> with the specified error message and previous
        /// exception.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowDetailException(string message, Exception innerException) {
            throw new DetailException(message, innerException);
        }

        #endregion
        #region ThrowDetailException(String, String)

        /// <summary>
        /// Throws a <see cref="DetailException"/> with the specified error message and technical
        /// details.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="details">
        /// A message that provides technical details about the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowDetailException(string message, string details) {
            throw new DetailException(message, details);
        }

        #endregion
        #region ThrowDetailException(String, String, Exception)

        /// <summary>
        /// Throws a <see cref="DetailException"/> with the specified error message, technical
        /// details, and previous exception.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <param name="details">
        /// A message that provides technical details about the exception.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowDetailException(
            string message, string details, Exception innerException) {

            throw new DetailException(message, details, innerException);
        }

        #endregion
        #region ThrowDetailExceptionWithFormat(String, Object)

        /// <overloads>
        /// Throws a <see cref="DetailException"/> with a formatted error message.</overloads>
        /// <summary>
        /// Throws a <see cref="DetailException"/> with a formatted error message.</summary>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="argument">
        /// The argument for the <paramref name="format"/> string.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowDetailExceptionWithFormat(string format, object argument) {

            string message = String.Format(CultureInfo.CurrentCulture, format, argument);
            throw new DetailException(message);
        }

        #endregion
        #region ThrowDetailExceptionWithFormat(String, Object, Exception)

        /// <summary>
        /// Throws a <see cref="DetailException"/> with a formatted error message and the specified
        /// previous exception.</summary>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="argument">
        /// The argument for the <paramref name="format"/> string.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowDetailExceptionWithFormat(
            string format, object argument, Exception innerException) {

            string message = String.Format(CultureInfo.CurrentCulture, format, argument);
            throw new DetailException(message, innerException);
        }

        #endregion
        #region ThrowDetailExceptionWithFormat(String, Object, String)

        /// <summary>
        /// Throws a <see cref="DetailException"/> with a formatted error message and the specified
        /// technical details.</summary>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="argument">
        /// The argument for the <paramref name="format"/> string.</param>
        /// <param name="details">
        /// A message that provides technical details about the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowDetailExceptionWithFormat(
            string format, object argument, string details) {

            string message = String.Format(CultureInfo.CurrentCulture, format, argument);
            throw new DetailException(message, details);
        }

        #endregion
        #region ThrowDetailExceptionWithFormat(String, Object, String, Exception)

        /// <summary>
        /// Throws a <see cref="DetailException"/> with a formatted error message and the specified
        /// technical details and previous exception.</summary>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="argument">
        /// The argument for the <paramref name="format"/> string.</param>
        /// <param name="details">
        /// A message that provides technical details about the exception.</param>
        /// <param name="innerException">
        /// The previous <see cref="Exception"/> that is the cause of the current exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowDetailExceptionWithFormat(
            string format, object argument, string details, Exception innerException) {

            string message = String.Format(CultureInfo.CurrentCulture, format, argument);
            throw new DetailException(message, details, innerException);
        }

        #endregion
        #region ThrowFileNotFoundException

        /// <summary>
        /// Throws a <see cref="FileNotFoundException"/>.</summary>
        /// <param name="fileName">
        /// The name of the file that caused the exception.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>
        /// <remarks>
        /// <b>ThrowFileNotFoundException</b> reverses the order of <paramref name="fileName"/> and
        /// <paramref name="message"/> compared to the <see cref="FileNotFoundException"/>
        /// constructor, for conformance with other <see cref="ThrowHelper"/> methods.</remarks>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowFileNotFoundException(string fileName, string message) {
            throw new FileNotFoundException(message, fileName);
        }

        #endregion
        #region ThrowInvalidEnumArgumentException

        /// <summary>
        /// Throws an <see cref="InvalidEnumArgumentException"/>.</summary>
        /// <param name="paramName">
        /// The name of the parameter that caused the exception.</param>
        /// <param name="actualValue">
        /// The actual value of the parameter that caused the exception.</param>
        /// <param name="enumClass">
        /// The <see cref="Enum"/> type of the parameter that caused the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowInvalidEnumArgumentException(
            string paramName, int actualValue, Type enumClass) {

            throw new InvalidEnumArgumentException(paramName, actualValue, enumClass);
        }

        #endregion
        #region ThrowInvalidOperationException

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/>.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowInvalidOperationException(string message) {
            throw new InvalidOperationException(message);
        }

        #endregion
        #region ThrowInvalidOperationExceptionWithFormat

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> with a formatted error message.
        /// </summary>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="argument">
        /// The argument for the <paramref name="format"/> string.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowInvalidOperationExceptionWithFormat(
            string format, object argument) {

            string message = String.Format(CultureInfo.CurrentCulture, format, argument);
            throw new InvalidOperationException(message);
        }

        #endregion
        #region ThrowKeyMismatchException

        /// <summary>
        /// Throws a <see cref="KeyMismatchException"/>.</summary>
        /// <param name="key">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> that was found or stored in a <see
        /// cref="KeyValuePair{TKey, TValue}"/> associated with <paramref name="valueKey"/>.</param>
        /// <param name="valueKey">
        /// The <see cref="IKeyedValue{TKey}.Key"/> that was found or stored in an <see
        /// cref="IKeyedValue{TKey}"/> instance associated with <paramref name="key"/>.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowKeyMismatchException(object key, object valueKey) {
            throw new KeyMismatchException(key, valueKey);
        }

        #endregion
        #region ThrowKeyNotFoundException

        /// <summary>
        /// Throws a <see cref="KeyNotFoundException"/>.</summary>
        /// <param name="key">
        /// The key that was not found.</param>
        /// <remarks>
        /// <b>ThrowKeyNotFoundException</b> creates a localized error message that contains a
        /// string representation of the specified <paramref name="key"/>.</remarks>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowKeyNotFoundException(object key) {

            string message = String.Format(CultureInfo.CurrentCulture, Strings.KeyNotFound, key);
            throw new KeyNotFoundException(message);
        }

        #endregion
        #region ThrowNotImplementedException

        /// <summary>
        /// Throws a <see cref="NotImplementedException"/>.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowNotImplementedException(string message) {
            throw new NotImplementedException(message);
        }

        #endregion
        #region ThrowNotSupportedException

        /// <summary>
        /// Throws a <see cref="NotSupportedException"/>.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowNotSupportedException(string message) {
            throw new NotSupportedException(message);
        }

        #endregion
        #region ThrowObjectDisposedException

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/>.</summary>
        /// <param name="objectName">
        /// The name of the disposed object. This argument may be a null reference.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowObjectDisposedException(string objectName) {
            throw new ObjectDisposedException(objectName);
        }

        #endregion
        #region ThrowPropertyValueException(String, String)

        /// <overloads>
        /// Throws a <see cref="PropertyValueException"/>.</overloads>
        /// <summary>
        /// Throws a <see cref="PropertyValueException"/> with the specified property name and error
        /// message.</summary>
        /// <param name="propertyName">
        /// The name of the property that caused the exception.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowPropertyValueException(string propertyName, string message) {
            throw new PropertyValueException(propertyName, message);
        }

        #endregion
        #region ThrowPropertyValueException(String, Object, String)

        /// <summary>
        /// Throws a <see cref="PropertyValueException"/> with the specified property name and value
        /// and error message.</summary>
        /// <param name="propertyName">
        /// The name of the property that caused the exception.</param>
        /// <param name="actualValue">
        /// The actual value of the property that caused the exception.</param>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowPropertyValueException(
            string propertyName, object actualValue, string message) {

            throw new PropertyValueException(propertyName, actualValue, message);
        }

        #endregion
        #region ThrowPropertyValueExceptionWithFormat

        /// <summary>
        /// Throws a <see cref="PropertyValueException"/> with the specified property name and value
        /// and formatted error message.</summary>
        /// <param name="propertyName">
        /// The name of the property that caused the exception.</param>
        /// <param name="actualValue">
        /// The actual value of the property that caused the exception.</param>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="argument">
        /// The argument for the <paramref name="format"/> string.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowPropertyValueExceptionWithFormat(
            string propertyName, object actualValue, string format, object argument) {

            string message = String.Format(CultureInfo.CurrentCulture, format, argument);
            throw new PropertyValueException(propertyName, actualValue, message);
        }

        #endregion
        #region ThrowTypeLoadException

        /// <summary>
        /// Throws a <see cref="TypeLoadException"/>.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowTypeLoadException(string message) {
            throw new TypeLoadException(message);
        }

        #endregion
        #region ThrowTypeLoadExceptionWithFormat

        /// <summary>
        /// Throws a <see cref="TypeLoadException"/> with a formatted error message.</summary>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="argument">
        /// The argument for the <paramref name="format"/> string.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowTypeLoadExceptionWithFormat(string format, object argument) {

            string message = String.Format(CultureInfo.CurrentCulture, format, argument);
            throw new TypeLoadException(message);
        }

        #endregion
        #region ThrowXmlException

        /// <summary>
        /// Throws an <see cref="XmlException"/>.</summary>
        /// <param name="message">
        /// The error message that specifies the reason for the exception.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowXmlException(string message) {
            throw new XmlException(message);
        }

        #endregion
        #region ThrowXmlExceptionWithFormat(..., Object)

        /// <overloads>
        /// Throws an <see cref="XmlException"/> with a formatted error message.</overloads>
        /// <summary>
        /// Throws an <see cref="XmlException"/> with a formatted error message and a single
        /// argument.</summary>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="argument">
        /// The argument for the <paramref name="format"/> string.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowXmlExceptionWithFormat(string format, object argument) {

            string message = String.Format(CultureInfo.CurrentCulture, format, argument);
            throw new XmlException(message);
        }

        #endregion
        #region ThrowXmlExceptionWithFormat(..., Object, Object)

        /// <summary>
        /// Throws an <see cref="XmlException"/> with a formatted error message and two arguments.
        /// </summary>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="arg0">
        /// The first argument for the <paramref name="format"/> string.</param>
        /// <param name="arg1">
        /// The second argument for the <paramref name="format"/> string.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowXmlExceptionWithFormat(string format, object arg0, object arg1) {

            string message = String.Format(CultureInfo.CurrentCulture, format, arg0, arg1);
            throw new XmlException(message);
        }

        #endregion
        #region ThrowXmlExceptionWithFormat(..., Object, Object, Object)

        /// <summary>
        /// Throws an <see cref="XmlException"/> with a formatted error message and three arguments.
        /// </summary>
        /// <param name="format">
        /// A composite <see cref="String.Format"/> string for the error message that specifies the
        /// reason for the exception.</param>
        /// <param name="arg0">
        /// The first argument for the <paramref name="format"/> string.</param>
        /// <param name="arg1">
        /// The second argument for the <paramref name="format"/> string.</param>
        /// <param name="arg2">
        /// The second argument for the <paramref name="format"/> string.</param>

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowXmlExceptionWithFormat(
            string format, object arg0, object arg1, object arg2) {

            string message = String.Format(CultureInfo.CurrentCulture, format, arg0, arg1, arg2);
            throw new XmlException(message);
        }

        #endregion
    }
}
