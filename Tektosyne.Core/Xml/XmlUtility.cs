using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

namespace Tektosyne.Xml {

    /// <summary>
    /// Provides auxiliary methods for <b>System.Xml</b>.</summary>

    public static class XmlUtility {
        #region CreateReaderSettings()

        /// <overloads>
        /// Creates standard <see cref="XmlReaderSettings"/>.</overloads>
        /// <summary>
        /// Creates standard <see cref="XmlReaderSettings"/> without a schema.</summary>
        /// <returns>
        /// A new <see cref="XmlReaderSettings"/> object with standard options.</returns>
        /// <remarks><para>
        /// <b>CreateReaderSettings</b> simplifies the process of creating <see
        /// cref="XmlReaderSettings"/> with commonly used options. The following properties are
        /// explicitly set on the new instance, with the others remaining at their default values:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="XmlReaderSettings.IgnoreComments"/></term>
        /// <description><c>true</c></description>
        /// </item><item>
        /// <term><see cref="XmlReaderSettings.IgnoreProcessingInstructions"/></term>
        /// <description><c>true</c></description>
        /// </item><item>
        /// <term><see cref="XmlReaderSettings.IgnoreWhitespace"/></term>
        /// <description><c>true</c></description>
        /// </item></list></remarks>

        public static XmlReaderSettings CreateReaderSettings() {
            XmlReaderSettings settings = new XmlReaderSettings();

            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;

            return settings;
        }

        #endregion
        #region CreateReaderSettings(String)

        /// <summary>
        /// Creates standard <see cref="XmlReaderSettings"/> with the specified schema.</summary>
        /// <param name="schemaUri">
        /// The URI that specifies the schema to load.</param>
        /// <returns>
        /// A new <see cref="XmlReaderSettings"/> object with standard options that validates 
        /// against the specified <paramref name="schemaUri"/>.</returns>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="schemaUri"/> is a null reference or an empty string.</exception>
        /// <exception cref="XmlSchemaException">
        /// <paramref name="schemaUri"/> does not specify a valid XML schema.</exception>
        /// <remarks><para>
        /// <b>CreateReaderSettings</b> simplifies the process of creating <see
        /// cref="XmlReaderSettings"/> with commonly used options that validates against a schema.
        /// The following properties are explicitly set on the new instance, with the others
        /// remaining at their default values:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="XmlReaderSettings.IgnoreComments"/></term>
        /// <description><c>true</c></description>
        /// </item><item>
        /// <term><see cref="XmlReaderSettings.IgnoreProcessingInstructions"/></term>
        /// <description><c>true</c></description>
        /// </item><item>
        /// <term><see cref="XmlReaderSettings.IgnoreWhitespace"/></term>
        /// <description><c>true</c></description>
        /// </item><item>
        /// <term><see cref="XmlReaderSettings.Schemas"/></term>
        /// <description>Contains one element with the specified <paramref name="schemaUri"/> and
        /// the default target namespace defined in the schema.</description>
        /// </item><item>
        /// <term><see cref="XmlReaderSettings.ValidationType"/></term>
        /// <description><see cref="ValidationType.Schema"/></description>
        /// </item></list></remarks>

        public static XmlReaderSettings CreateReaderSettings(string schemaUri) {
            if (String.IsNullOrEmpty(schemaUri))
                ThrowHelper.ThrowArgumentNullOrEmptyException("schemaUri");

            XmlReaderSettings settings = CreateReaderSettings();

            settings.Schemas.Add(null, schemaUri);
            settings.ValidationType = ValidationType.Schema;

            return settings;
        }

        #endregion
        #region CreateWriterSettings

        /// <summary>
        /// Creates standard <see cref="XmlWriterSettings"/>.</summary>
        /// <returns>
        /// A new <see cref="XmlWriterSettings"/> object with standard options.</returns>
        /// <remarks><para>
        /// <b>CreateWriterSettings</b> simplifies the process of creating <see
        /// cref="XmlWriterSettings"/> with commonly used options. The following properties are
        /// explicitly set on the new instance, with the others remaining at their default values:
        /// </para><list type="table"><listheader>
        /// <term>Property</term><description>Value</description>
        /// </listheader><item>
        /// <term><see cref="XmlWriterSettings.Encoding"/></term>
        /// <description><see cref="Encoding.UTF8"/></description>
        /// </item><item>
        /// <term><see cref="XmlWriterSettings.Indent"/></term>
        /// <description><c>true</c></description>
        /// </item><item>
        /// <term><see cref="XmlWriterSettings.IndentChars"/></term>
        /// <description>Two space characters (Unicode character 32, SPACE).</description>
        /// </item></list></remarks>

        public static XmlWriterSettings CreateWriterSettings() {
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.IndentChars = "  ";

            return settings;
        }

        #endregion
        #region GetXmlMessage

        /// <summary>
        /// Extracts the message of the specified <see cref="Exception"/>, prepending the error
        /// location if it is an <see cref="XmlException"/> or <see cref="XmlSchemaException"/>.
        /// </summary>
        /// <param name="exception">
        /// The <see cref="Exception"/> object whose error message to extract.</param>
        /// <returns>
        /// A <see cref="String"/> containing the error location (if available) and <see
        /// cref="Exception.Message"/> of the specified <paramref name="exception"/>, or an empty
        /// string if <paramref name="exception"/> is a null reference.</returns>
        /// <remarks><para>
        /// <b>GetXmlMessage</b> provides a convenient way to extract the additional error
        /// information provided by <see cref="XmlException"/> and <see cref="XmlSchemaException"/>,
        /// namely the error location returned by the <see cref="XmlException.LineNumber"/> and <see
        /// cref="XmlException.LinePosition"/> properties.
        /// </para><para>
        /// If the specified <paramref name="exception"/> is of either type and specifies a
        /// <b>LineNumber</b> greater than zero, <b>GetXmlMessage</b> prepends a localized error
        /// location statement to the <see cref="Exception.Message"/> specified by <paramref
        /// name="exception"/>. Otherwise, only the <b>Message</b> is returned.</para></remarks>

        public static string GetXmlMessage(Exception exception) {
            if (exception == null) return "";

            // XML exceptions provide error location
            int lineNumber = -1, linePosition = -1;
            string intro = "";

            XmlException xe = exception as XmlException;
            if (xe != null) {
                intro = Strings.XmlError;
                lineNumber = xe.LineNumber;
                linePosition = xe.LinePosition;
            } else {
                XmlSchemaException xse = exception as XmlSchemaException;
                if (xse != null) {
                    intro = Strings.XmlSchemaError;
                    lineNumber = xse.LineNumber;
                    linePosition = xse.LinePosition;
                }
            }

            // create location statement
            if (lineNumber > 0) {

                // specify error type (XML or XML Schema)
                StringBuilder sb = new StringBuilder(intro);

                // append error line if available
                sb.AppendFormat(Strings.FormatAtLine, lineNumber);

                // append error column if available
                if (linePosition > 0)
                    sb.AppendFormat(Strings.FormatAtColumn, linePosition);

                // conclude error type line
                sb.Append('.');

                // append message if available
                if (!String.IsNullOrEmpty(exception.Message)) {
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                    sb.Append(exception.Message);
                }

                // return message with location
                return sb.ToString();
            }

            // just return message
            return exception.Message;
        }

        #endregion
        #region MoveToEndElement

        /// <summary>
        /// Moves the specified <see cref="XmlReader"/> to the next element end tag.</summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> to move.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is a null reference.</exception>
        /// <remarks><para>
        /// <b>MoveToEndElement</b> returns immediately if the <see cref="XmlReader.ReadState"/> of
        /// the specified <paramref name="reader"/> is not <b>ReadState.Interactive</b>.
        /// </para><para>
        /// Otherwise, <b>MoveToEndElement</b> calls <see cref="XmlReader.MoveToContent"/> to skip
        /// over any white space, comments, and processing instructions, and then repositions
        /// <paramref name="reader"/> as follows, depending on content of the current node:
        /// </para><list type="table"><listheader>
        /// <term>Current Node</term><description>New Position</description>
        /// </listheader><item>
        /// <term>Empty element tag (<see cref="XmlReader.IsEmptyElement"/> is <c>true</c>)</term>
        /// <description>Unchanged</description>
        /// </item><item>
        /// <term>End tag (<b>XmlNodeType.EndElement</b>)</term>
        /// <description>Unchanged</description>
        /// </item><item>
        /// <term>Start tag (<b>XmlNodeType.Element</b>)</term>
        /// <description>Next element end tag of the same depth (effectively skips over any child
        /// nodes of the current node)</description>
        /// </item><item>
        /// <term>Other</term><description>Next element end tag of any depth</description>
        /// </item></list></remarks>
        /// <example><para>
        /// Note that standard methods such as <see cref="XmlReader.Skip"/> or <see
        /// cref="XmlReader.ReadOuterXml"/> do not position the <see cref="XmlReader"/> on the next
        /// end tag, but rather on the node <em>following</em> the next end tag, making them
        /// difficult to use in common loops such as this:
        /// </para><code>
        /// while (reader.Read() &amp;&amp; reader.IsStartElement()) {
        /// 
        ///     if (reader.Name == "DontLikeThisNode") {
        ///         // ERROR: loop will miss next start tag!
        ///         reader.Skip();
        ///     }
        ///     else if (reader.Name == "DontLikeThatEither") {
        ///         // OK: loop will hit next start tag
        ///         MoveToEndElement(reader);
        ///     }
        /// }</code></example>

        public static void MoveToEndElement(this XmlReader reader) {
            if (reader == null)
                ThrowHelper.ThrowArgumentNullException("reader");

            // don’t try to read from an inactive reader
            if (reader.ReadState != ReadState.Interactive)
                return;

            // move to next content node if not on one
            reader.MoveToContent();

            // do nothing if now on empty element or end tag
            if (reader.IsEmptyElement || reader.NodeType == XmlNodeType.EndElement)
                return;

            // store current depth if on start tag
            if (reader.NodeType == XmlNodeType.Element) {
                int depth = reader.Depth;

                // read up to end tag of same depth
                while (reader.Read())
                    if (reader.NodeType == XmlNodeType.EndElement)
                        if (reader.Depth == depth) break;

                return;
            }

            // unknown element, just read up to next end tag
            while (reader.Read())
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;
        }

        #endregion
        #region MoveToStartElement

        /// <summary>
        /// Moves the specified <see cref="XmlReader"/> to the next content node which must be an
        /// element start tag of the specified name.</summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> to move to the element start tag with the specified
        /// <paramref name="name"/>.</param>
        /// <param name="name">
        /// The <see cref="XmlReader.Name"/> of the expected start tag (<b>XmlNodeType.Element</b>).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="name"/> is a null reference or an empty string.</exception>
        /// <exception cref="XmlException">
        /// The next content node was not a start element tag (<b>XmlNodeType.Element</b>), or its
        /// name was not <paramref name="name"/>.</exception>
        /// <remarks>
        /// <b>MoveToStartElement</b> calls <see cref="XmlReader.IsStartElement"/> with the
        /// specified <paramref name="name"/> to test the node type and name of the next content
        /// node returned by the specified <paramref name="reader"/>. If the test fails,
        /// <b>MoveToStartElement</b> throws an <see cref="XmlException"/> indicating the expected
        /// name; otherwise, it returns silently.</remarks>

        public static void MoveToStartElement(this XmlReader reader, string name) {

            if (reader == null)
                ThrowHelper.ThrowArgumentNullException("reader");
            if (String.IsNullOrEmpty(name))
                ThrowHelper.ThrowArgumentNullOrEmptyException("name");

            // check next content, return on success
            if (reader.IsStartElement(name)) return;

            // throw exception on failure
            ThrowHelper.ThrowXmlExceptionWithFormat(Strings.XmlContentNotElementName, name);
        }

        #endregion
        #region ReadAttributeAsBoolean

        /// <summary>
        /// Reads the specified attribute from the specified <see cref="XmlReader"/> and converts it
        /// into a <see cref="Boolean"/> value.</summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.</param>
        /// <param name="attribute">
        /// The <see cref="XmlReader.Name"/> of the XML attribute to read.</param>
        /// <param name="value">
        /// Returns the <see cref="Boolean"/> value read from <paramref name="attribute"/>, if
        /// found; otherwise, this parameter remains unchanged.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="attribute"/> is a null reference or an empty string.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="attribute"/> does not represent a <see cref="Boolean"/> value.
        /// </exception>
        /// <remarks>
        /// <b>ReadAttributeAsBoolean</b> does not move the specified <paramref name="reader"/>.
        /// </remarks>

        public static void ReadAttributeAsBoolean(this XmlReader reader,
            string attribute, ref bool value) {

            if (reader == null)
                ThrowHelper.ThrowArgumentNullException("reader");
            if (String.IsNullOrEmpty(attribute))
                ThrowHelper.ThrowArgumentNullOrEmptyException("attribute");

            string text = reader[attribute];
            if (text != null) value = XmlConvert.ToBoolean(text);
        }

        #endregion
        #region ReadAttributeAsByte

        /// <summary>
        /// Reads the specified attribute from the specified <see cref="XmlReader"/> and converts it
        /// into a <see cref="Byte"/> value.</summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.</param>
        /// <param name="attribute">
        /// The <see cref="XmlReader.Name"/> of the XML attribute to read.</param>
        /// <param name="value">
        /// Returns the <see cref="Byte"/> value read from <paramref name="attribute"/>, if found;
        /// otherwise, this parameter remains unchanged.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="attribute"/> is a null reference or an empty string.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="attribute"/> is not in the correct format.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="attribute"/> represents a number less than <see cref="Byte.MinValue"/>
        /// or <see cref="Byte.MaxValue"/>.</exception>
        /// <remarks>
        /// <b>ReadAttributeAsByte</b> does not move the specified <paramref name="reader"/>.
        /// </remarks>

        public static void ReadAttributeAsByte(this XmlReader reader,
            string attribute, ref byte value) {

            if (reader == null)
                ThrowHelper.ThrowArgumentNullException("reader");
            if (String.IsNullOrEmpty(attribute))
                ThrowHelper.ThrowArgumentNullOrEmptyException("attribute");

            string text = reader[attribute];
            if (text != null) value = XmlConvert.ToByte(text);
        }

        #endregion
        #region ReadAttributeAsDouble

        /// <summary>
        /// Reads the specified attribute from the specified <see cref="XmlReader"/> and converts it
        /// into a <see cref="Double"/> value.</summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.</param>
        /// <param name="attribute">
        /// The <see cref="XmlReader.Name"/> of the XML attribute to read.</param>
        /// <param name="value">
        /// Returns the <see cref="Double"/> value read from <paramref name="attribute"/>, if found;
        /// otherwise, this parameter remains unchanged.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="attribute"/> is a null reference or an empty string.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="attribute"/> is not in the correct format.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="attribute"/> represents a number less than <see cref="Double.MinValue"/>
        /// or <see cref="Double.MaxValue"/>.</exception>
        /// <remarks>
        /// <b>ReadAttributeAsDouble</b> does not move the specified <paramref name="reader"/>.
        /// </remarks>

        public static void ReadAttributeAsDouble(this XmlReader reader,
            string attribute, ref double value) {

            if (reader == null)
                ThrowHelper.ThrowArgumentNullException("reader");
            if (String.IsNullOrEmpty(attribute))
                ThrowHelper.ThrowArgumentNullOrEmptyException("attribute");

            string text = reader[attribute];
            if (text != null) value = XmlConvert.ToDouble(text);
        }

        #endregion
        #region ReadAttributeAsEnum<T>

        /// <summary>
        /// Reads the specified attribute from the specified <see cref="XmlReader"/> and converts it
        /// into an enumeration value of the specified type.</summary>
        /// <typeparam name="T">
        /// The <see cref="Enum"/> that defines the valid values for <paramref name="attribute"/>.
        /// Any underlying type is acceptable, and the <see cref="Enum"/> may have the <see
        /// cref="FlagsAttribute"/>.</typeparam>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.</param>
        /// <param name="attribute">
        /// The <see cref="XmlReader.Name"/> of the XML attribute to read.</param>
        /// <param name="value">
        /// Returns the <typeparamref name="T"/> value read from <paramref name="attribute"/>, if
        /// found; otherwise, this parameter remains unchanged.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="attribute"/> does not contain one or more of the named constants defined
        /// for <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="attribute"/> is a null reference or an empty string.</exception>
        /// <remarks><para>
        /// <b>ReadAttributeAsEnum</b> does not move the specified <paramref name="reader"/>.
        /// </para><para>
        /// The contents of the specified <paramref name="attribute"/> are converted into a 
        /// <typeparamref name="T"/> value using the <see cref="Enum.Parse"/> method of the <see
        /// cref="Enum"/> class. The operation is case-sensitive.
        /// </para><para>
        /// If <typeparamref name="T"/> carries the <see cref="FlagsAttribute"/>, the specified
        /// <paramref name="attribute"/> may contain multiple <typeparamref name="T"/> values, and
        /// <b>ReadAttributeAsEnum</b> will return a bitwise combination of these values. The named
        /// constants in <paramref name="attribute"/> must be separated either by a comma followed
        /// by whitespace (the default string representation of a bit field <see cref="Enum"/>), or
        /// just by whitespace (the delimiter used by the <c>xsd:list</c> type of XML Schema).
        /// </para></remarks>

        public static void ReadAttributeAsEnum<T>(this XmlReader reader,
            string attribute, ref T value) {

            if (reader == null)
                ThrowHelper.ThrowArgumentNullException("reader");
            if (String.IsNullOrEmpty(attribute))
                ThrowHelper.ThrowArgumentNullOrEmptyException("attribute");

            string text = reader[attribute];
            if (text != null) {
                // normalize flag value separators
                text = Regex.Replace(text, @"([^,\s])(\s+\b)", @"$1,$2");
                value = (T) Enum.Parse(typeof(T), text);
            }
        }

        #endregion
        #region ReadAttributeAsInt16

        /// <summary>
        /// Reads the specified attribute from the specified <see cref="XmlReader"/> and converts it
        /// into an <see cref="Int16"/> value.</summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.</param>
        /// <param name="attribute">
        /// The <see cref="XmlReader.Name"/> of the XML attribute to read.</param>
        /// <param name="value">
        /// Returns the <see cref="Int16"/> value read from <paramref name="attribute"/>, if found;
        /// otherwise, this parameter remains unchanged.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="attribute"/> is a null reference or an empty string.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="attribute"/> is not in the correct format.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="attribute"/> represents a number less than <see cref="Int16.MinValue"/>
        /// or <see cref="Int16.MaxValue"/>.</exception>
        /// <remarks>
        /// <b>ReadAttributeAsInt16</b> does not move the specified <paramref name="reader"/>.
        /// </remarks>

        public static void ReadAttributeAsInt16(this XmlReader reader,
            string attribute, ref short value) {

            if (reader == null)
                ThrowHelper.ThrowArgumentNullException("reader");
            if (String.IsNullOrEmpty(attribute))
                ThrowHelper.ThrowArgumentNullOrEmptyException("attribute");

            string text = reader[attribute];
            if (text != null) value = XmlConvert.ToInt16(text);
        }

        #endregion
        #region ReadAttributeAsInt32

        /// <summary>
        /// Reads the specified attribute from the specified <see cref="XmlReader"/> and converts it
        /// into an <see cref="Int32"/> value.</summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.</param>
        /// <param name="attribute">
        /// The <see cref="XmlReader.Name"/> of the XML attribute to read.</param>
        /// <param name="value">
        /// Returns the <see cref="Int32"/> value read from <paramref name="attribute"/>, if found;
        /// otherwise, this parameter remains unchanged.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="attribute"/> is a null reference or an empty string.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="attribute"/> is not in the correct format.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="attribute"/> represents a number less than <see cref="Int32.MinValue"/>
        /// or <see cref="Int32.MaxValue"/>.</exception>
        /// <remarks>
        /// <b>ReadAttributeAsInt32</b> does not move the specified <paramref name="reader"/>.
        /// </remarks>

        public static void ReadAttributeAsInt32(this XmlReader reader,
            string attribute, ref int value) {

            if (reader == null)
                ThrowHelper.ThrowArgumentNullException("reader");
            if (String.IsNullOrEmpty(attribute))
                ThrowHelper.ThrowArgumentNullOrEmptyException("attribute");

            string text = reader[attribute];
            if (text != null) value = XmlConvert.ToInt32(text);
        }

        #endregion
        #region ReadAttributeAsInt64

        /// <summary>
        /// Reads the specified attribute from the specified <see cref="XmlReader"/> and converts it
        /// into an <see cref="Int64"/> value.</summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.</param>
        /// <param name="attribute">
        /// The <see cref="XmlReader.Name"/> of the XML attribute to read.</param>
        /// <param name="value">
        /// Returns the <see cref="Int64"/> value read from <paramref name="attribute"/>, if found;
        /// otherwise, this parameter remains unchanged.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="attribute"/> is a null reference or an empty string.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="attribute"/> is not in the correct format.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="attribute"/> represents a number less than <see cref="Int64.MinValue"/>
        /// or <see cref="Int64.MaxValue"/>.</exception>
        /// <remarks>
        /// <b>ReadAttributeAsInt64</b> does not move the specified <paramref name="reader"/>.
        /// </remarks>

        public static void ReadAttributeAsInt64(this XmlReader reader,
            string attribute, ref long value) {

            if (reader == null)
                ThrowHelper.ThrowArgumentNullException("reader");
            if (String.IsNullOrEmpty(attribute))
                ThrowHelper.ThrowArgumentNullOrEmptyException("attribute");

            string text = reader[attribute];
            if (text != null) value = XmlConvert.ToInt64(text);
        }

        #endregion
        #region ReadAttributeAsSingle

        /// <summary>
        /// Reads the specified attribute from the specified <see cref="XmlReader"/> and converts it
        /// into a <see cref="Single"/> value.</summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.</param>
        /// <param name="attribute">
        /// The <see cref="XmlReader.Name"/> of the XML attribute to read.</param>
        /// <param name="value">
        /// Returns the <see cref="Single"/> value read from <paramref name="attribute"/>, if found;
        /// otherwise, this parameter remains unchanged.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="attribute"/> is a null reference or an empty string.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="attribute"/> is not in the correct format.</exception>
        /// <exception cref="OverflowException">
        /// <paramref name="attribute"/> represents a number less than <see cref="Single.MinValue"/>
        /// or <see cref="Single.MaxValue"/>.</exception>
        /// <remarks>
        /// <b>ReadAttributeAsSingle</b> does not move the specified <paramref name="reader"/>.
        /// </remarks>

        public static void ReadAttributeAsSingle(this XmlReader reader,
            string attribute, ref float value) {

            if (reader == null)
                ThrowHelper.ThrowArgumentNullException("reader");
            if (String.IsNullOrEmpty(attribute))
                ThrowHelper.ThrowArgumentNullOrEmptyException("attribute");

            string text = reader[attribute];
            if (text != null) value = XmlConvert.ToSingle(text);
        }

        #endregion
        #region ReadAttributeAsString

        /// <summary>
        /// Reads the specified attribute from the specified <see cref="XmlReader"/>.</summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.</param>
        /// <param name="attribute">
        /// The <see cref="XmlReader.Name"/> of the XML attribute to read.</param>
        /// <param name="value">
        /// Returns the <see cref="String"/> value read from <paramref name="attribute"/>, if found;
        /// otherwise, this parameter remains unchanged.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="attribute"/> is a null reference or an empty string.</exception>
        /// <remarks><para>
        /// <b>ReadAttributeAsString</b> does not move the specified <paramref name="reader"/>.
        /// </para><para>
        /// If the specified <paramref name="attribute"/> is found, its <see cref="String"/> value
        /// is interned before assignment to <paramref name="value"/>.</para></remarks>

        public static void ReadAttributeAsString(this XmlReader reader,
            string attribute, ref string value) {

            if (reader == null)
                ThrowHelper.ThrowArgumentNullException("reader");
            if (String.IsNullOrEmpty(attribute))
                ThrowHelper.ThrowArgumentNullOrEmptyException("attribute");

            string text = reader[attribute];
            if (text != null) value = String.Intern(text);
        }

        #endregion
        #region ReadTextElement

        /// <summary>
        /// Reads a text-only element and stops at the end element tag.</summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/> from which to read.</param>
        /// <returns>
        /// The text contained in the element that was read from the specified <paramref
        /// name="reader"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is a null reference.</exception>
        /// <exception cref="XmlException">
        /// <paramref name="reader"/> is not positioned on or before a start element tag
        /// (<b>XmlNodeType.Element</b>) that contains only simple text and/or CDATA blocks
        /// (<b>XmlNodeType.Text</b> and <b>XmlNodeType.CDATA</b>).</exception>
        /// <remarks><para>
        /// <b>ReadTextElement</b> calls <see cref="XmlReader.IsStartElement"/> to position the
        /// specified <paramref name="reader"/> on the next content node. If that node is an empty
        /// element, <b>ReadTextElement</b> returns an empty string without further moving the
        /// <paramref name="reader"/>.
        /// </para><para>
        /// Otherwise, <b>ReadTextElement</b> concatenates any adjacent simple text and CDATA blocks
        /// (<b>XmlNodeType.Text</b> and <b>XmlNodeType.CDATA</b>) until the matching end element
        /// tag is found (<b>XmlNodeType.EndElement</b>), and then returns the concatenated text.
        /// </para><para>
        /// <b>ReadTextElement</b> differs from the standard library method <see
        /// cref="XmlReader.ReadElementString"/> in the position of <paramref name="reader"/> when
        /// the method returns. <b>ReadElementString</b> moves its <paramref name="reader"/> to the
        /// node <em>following</em> the end tag, making it difficult to use in loops of the form
        /// <c>while (reader.Read())</c>, as described in <see cref="MoveToEndElement"/>.
        /// </para></remarks>

        public static string ReadTextElement(this XmlReader reader) {
            if (reader == null)
                ThrowHelper.ThrowArgumentNullException("reader");

            // check for start element tag
            if (!reader.IsStartElement())
                ThrowHelper.ThrowXmlException(Strings.XmlContentNotElement);

            // nothing to read if element is empty
            if (reader.IsEmptyElement) return "";

            string text = "";
            while (true) {
                // move to content node or end element tag
                reader.Read();
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // check for simple text or CDATA block
                if (reader.NodeType != XmlNodeType.Text &&
                    reader.NodeType != XmlNodeType.CDATA)
                    ThrowHelper.ThrowXmlException(Strings.XmlContentNotText);

                text += reader.Value;
            }

            return text;
        }

        #endregion
    }
}
