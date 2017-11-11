using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Tektosyne.Xml {

    /// <summary>
    /// Provides auxiliary methods concerning XML serialization for
    /// <b>System.Runtime.Serialization</b> and <b>System.Xml</b>.</summary>

    public static class XmlSerialization {
        #region AttributeAsEnum<T>

        /// <summary>
        /// Converts the specified <see cref="XAttribute"/> of the specified <see cref="XElement"/>
        /// into an enumeration value of the specified type.</summary>
        /// <typeparam name="T">
        /// The <see cref="Enum"/> that defines the valid values for <paramref name="attribute"/>.
        /// Any underlying type is acceptable, and the <see cref="Enum"/> may have the <see
        /// cref="FlagsAttribute"/>.</typeparam>
        /// <param name="element">
        /// The <see cref="XElement"/> containing the <see cref="XAttribute"/> to convert.</param>
        /// <param name="attribute">
        /// The <see cref="XName.LocalName"/> of the <see cref="XAttribute"/> to convert.</param>
        /// <param name="value">
        /// Returns the <typeparamref name="T"/> value read from <paramref name="attribute"/>, if
        /// found; otherwise, this parameter remains unchanged.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="attribute"/> does not contain one or more of the named constants defined
        /// for <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element"/> is a null reference.</exception>
        /// <exception cref="ArgumentNullOrEmptyException">
        /// <paramref name="attribute"/> is a null reference or an empty string.</exception>
        /// <remarks><para>
        /// The contents of the specified <paramref name="attribute"/> are converted into a 
        /// <typeparamref name="T"/> value using the <see cref="Enum.Parse"/> method of the <see
        /// cref="Enum"/> class. This operation is case-sensitive.
        /// </para><para>
        /// If <typeparamref name="T"/> carries the <see cref="FlagsAttribute"/>, the specified
        /// <paramref name="attribute"/> may contain multiple <typeparamref name="T"/> values, and
        /// <b>AttributeAsEnum</b> will return a bitwise combination of these values. The named
        /// constants in <paramref name="attribute"/> must be separated either by a comma followed
        /// by whitespace (the default string representation of a bit field <see cref="Enum"/>), or
        /// just by whitespace (the delimiter used by the <c>xsd:list</c> type of XML Schema).
        /// </para></remarks>

        public static void AttributeAsEnum<T>(XElement element, string attribute, ref T value) {

            if (element == null)
                ThrowHelper.ThrowArgumentNullException("element");
            if (String.IsNullOrEmpty(attribute))
                ThrowHelper.ThrowArgumentNullOrEmptyException("attribute");

            XAttribute xa = element.Attribute(attribute);
            if (xa != null) {
                // normalize flag value separators
                string text = Regex.Replace(xa.Value, @"([^,\s])(\s+\b)", @"$1,$2");
                value = (T) Enum.Parse(typeof(T), text);
            }
        }

        #endregion
        #region Deserialize<T>

        /// <summary>
        /// Deserializes an object from the specified XML document.</summary>
        /// <typeparam name="T">
        /// The type of the object to deserialize.</typeparam>
        /// <param name="document">
        /// The complete XML document from which to deserialize the object.</param>
        /// <returns>
        /// The <typeparamref name="T"/> object resulting from the deserialization of <paramref
        /// name="document"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// The top-level object of <paramref name="document"/> is not of type <typeparamref
        /// name="T"/>.</exception>
        /// <remarks>
        /// <b>Deserialize</b> uses a <see cref="NetDataContractSerializer"/> using <see
        /// cref="FormatterAssemblyStyle.Simple"/> assembly mode, and an <see cref="XmlReader"/>
        /// using the settings returned by <see cref="XmlUtility.CreateReaderSettings"/>.</remarks>

        public static T Deserialize<T>(string document) {
            var settings = XmlUtility.CreateReaderSettings();

            using (var textReader = new StringReader(document))
            using (var reader = XmlReader.Create(textReader, settings)) {
                var serializer = new NetDataContractSerializer();
                serializer.AssemblyFormat = FormatterAssemblyStyle.Simple;
                return (T) serializer.ReadObject(reader);
            }
        }

        #endregion
        #region Serialize<T>

        /// <summary>
        /// Serializes the specified object to a complete XML document.</summary>
        /// <typeparam name="T">
        /// The type of the object to serialize.</typeparam>
        /// <param name="obj">
        /// The object to serialize.</param>
        /// <returns>
        /// A <see cref="String"/> containing the complete XML document resulting from the 
        /// serialization of <paramref name="obj"/>.</returns>
        /// <exception cref="SerializationException">
        /// <paramref name="obj"/> could not be serialized.</exception>
        /// <remarks>
        /// <b>Serialize</b> uses a <see cref="NetDataContractSerializer"/> and an <see
        /// cref="XmlWriter"/> using the settings returned by <see
        /// cref="XmlUtility.CreateWriterSettings"/>.</remarks>

        public static string Serialize<T>(T obj) {

            var builder = new StringBuilder();
            var settings = XmlUtility.CreateWriterSettings();

            using (var writer = XmlWriter.Create(builder, settings)) {
                var serializer = new NetDataContractSerializer();
                serializer.WriteObject(writer, obj);
            }

            return builder.ToString();
        }

        #endregion
    }
}
