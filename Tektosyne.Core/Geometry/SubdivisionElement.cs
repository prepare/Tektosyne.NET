using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Tektosyne.Geometry {

    /// <summary>
    /// Represents one element of a planar <see cref="Subdivision"/>.</summary>
    /// <remarks>
    /// <b>SubdivisionElement</b> is an immutable structure that contains either an <see
    /// cref="Subdivision.Edges"/>, <see cref="Subdivision.Faces"/>, or <see
    /// cref="Subdivision.Vertices"/> element of a planar <see cref="Subdivision"/>. A <see
    /// cref="SubdivisionElementType"/> value indicates which element type is present.</remarks>

    [Serializable]
    public struct SubdivisionElement: IEquatable<SubdivisionElement>, ISerializable {
        #region SubdivisionElement(PointD)

        /// <overloads>
        /// Initializes a new instance of the <see cref="SubdivisionElement"/> structure.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="SubdivisionElement"/> structure with the
        /// specified <see cref="Subdivision"/> vertex.</summary>
        /// <param name="vertex">
        /// The <see cref="Subdivision"/> vertex stored in the <see cref="SubdivisionElement"/>.
        /// </param>
        /// <remarks>
        /// <see cref="ElementType"/> is set to <see cref="SubdivisionElementType.Vertex"/>.
        /// </remarks>

        public SubdivisionElement(PointD vertex) {

            ElementType = SubdivisionElementType.Vertex;
            _value = null;
            _vertex = vertex;
        }

        #endregion
        #region SubdivisionElement(SubdivisionEdge)

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdivisionElement"/> structure with the
        /// specified <see cref="SubdivisionEdge"/>.</summary>
        /// <param name="edge">
        /// The <see cref="SubdivisionEdge"/> stored in the <see cref="SubdivisionElement"/>.
        /// </param>
        /// <remarks>
        /// <see cref="ElementType"/> is set to <see cref="SubdivisionElementType.Edge"/>.</remarks>

        public SubdivisionElement(SubdivisionEdge edge) {

            ElementType = SubdivisionElementType.Edge;
            _value = edge;
            _vertex = PointD.Empty;
        }

        #endregion
        #region SubdivisionElement(SubdivisionFace)

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdivisionElement"/> structure with the
        /// specified <see cref="SubdivisionFace"/>.</summary>
        /// <param name="face">
        /// The <see cref="SubdivisionFace"/> stored in the <see cref="SubdivisionElement"/>.
        /// </param>
        /// <remarks>
        /// <see cref="ElementType"/> is set to <see cref="SubdivisionElementType.Face"/>.</remarks>

        public SubdivisionElement(SubdivisionFace face) {

            ElementType = SubdivisionElementType.Face;
            _value = face;
            _vertex = PointD.Empty;
        }

        #endregion
        #region SubdivisionElement(SerializationInfo, StreamingContext)

        /// <summary>
        /// Initializes a new instance of the <see cref="SubdivisionElement"/> structure with
        /// serialized data.</summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object providing serialized object data for the <see
        /// cref="SubdivisionElement"/>.</param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> object containing contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is a null reference.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="ISerializable"/> for details.
        /// </para><para>
        /// The values of the <see cref="ElementType"/> property and either the <see cref="Edge"/>,
        /// <see cref="Face"/>, or <see cref="Vertex"/> property are deserialized from two fields,
        /// named "Type" and "Value".</para></remarks>

        private SubdivisionElement(SerializationInfo info, StreamingContext context) {

            ElementType = (SubdivisionElementType)
                info.GetValue("Type", typeof(SubdivisionElementType));

            _value = null;
            _vertex = PointD.Empty;

            switch (ElementType) {
                case SubdivisionElementType.Edge:
                    _value = info.GetValue("Value", typeof(SubdivisionEdge));
                    break;

                case SubdivisionElementType.Face:
                    _value = info.GetValue("Value", typeof(SubdivisionFace));
                    break;

                case SubdivisionElementType.Vertex:
                    _vertex = (PointD) info.GetValue("Value", typeof(PointD));
                    break;
            }
        }

        #endregion
        #region Private Fields

        // property backers
        private readonly object _value;
        private readonly PointD _vertex;

        #endregion
        #region NullFace

        /// <summary>
        /// A <see cref="SubdivisionElement"/> whose <see cref="ElementType"/> equals <see
        /// cref="SubdivisionElementType.Face"/> and whose <see cref="Face"/> is a null reference.
        /// </summary>
        /// <remarks>
        /// By convention, <b>NullFace</b> represents the unbounded face of a planar <see
        /// cref="Subdivision"/>. Use <see cref="IsUnboundedFace"/> to test for this condition, as
        /// well as for the actual unbounded <see cref="SubdivisionFace"/>.</remarks>

        public static readonly SubdivisionElement NullFace
            = new SubdivisionElement((SubdivisionFace) null);

        #endregion
        #region ElementType

        /// <summary>
        /// A <see cref="SubdivisionElementType"/> value indicating the type of the <see
        /// cref="SubdivisionElement"/>.</summary>

        public readonly SubdivisionElementType ElementType;

        #endregion
        #region Edge

        /// <summary>
        /// Gets the <see cref="SubdivisionEdge"/> stored in the <see cref="SubdivisionElement"/>.
        /// </summary>
        /// <value>
        /// The <see cref="SubdivisionEdge"/> stored in the <see cref="SubdivisionElement"/>.
        /// </value>
        /// <exception cref="PropertyValueException">
        /// <see cref="ElementType"/> does not equal <see cref="SubdivisionElementType.Edge"/>.
        /// </exception>

        public SubdivisionEdge Edge {
            get {
                if (ElementType != SubdivisionElementType.Edge)
                    ThrowHelper.ThrowPropertyValueException(
                        "ElementType", ElementType, Strings.PropertyInvalidValue);

                return (SubdivisionEdge) _value;
            }
        }

        #endregion
        #region Face

        /// <summary>
        /// Gets the <see cref="SubdivisionFace"/> stored in the <see cref="SubdivisionElement"/>.
        /// </summary>
        /// <value>
        /// The <see cref="SubdivisionFace"/> stored in the <see cref="SubdivisionElement"/>.
        /// </value>
        /// <exception cref="PropertyValueException">
        /// <see cref="ElementType"/> does not equal <see cref="SubdivisionElementType.Face"/>.
        /// </exception>

        public SubdivisionFace Face {
            get {
                if (ElementType != SubdivisionElementType.Face)
                    ThrowHelper.ThrowPropertyValueException(
                        "ElementType", ElementType, Strings.PropertyInvalidValue);

                return (SubdivisionFace) _value;
            }
        }

        #endregion
        #region Vertex

        /// <summary>
        /// Gets the <see cref="Subdivision"/> vertex stored in the <see
        /// cref="SubdivisionElement"/>.</summary>
        /// <value>
        /// The <see cref="Subdivision"/> vertex stored in the <see cref="SubdivisionElement"/>.
        /// </value>
        /// <exception cref="PropertyValueException">
        /// <see cref="ElementType"/> does not equal <see cref="SubdivisionElementType.Vertex"/>.
        /// </exception>

        public PointD Vertex {
            get {
                if (ElementType != SubdivisionElementType.Vertex)
                    ThrowHelper.ThrowPropertyValueException(
                        "ElementType", ElementType, Strings.PropertyInvalidValue);

                return _vertex;
            }
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="SubdivisionElement"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> returns the result of <see cref="Object.GetHashCode"/> for either the
        /// <see cref="Edge"/>, <see cref="Face"/>, or <see cref="Vertex"/> property, depending on
        /// the <see cref="ElementType"/>.</remarks>

        public override int GetHashCode() {

            if (ElementType == SubdivisionElementType.Vertex)
                return _vertex.GetHashCode();

            if (_value != null)
                return _value.GetHashCode();

            return (int) ElementType;
        }

        #endregion
        #region IsUnboundedFace

        /// <summary>
        /// Gets a value indicating whether the <see cref="SubdivisionElement"/> represents an
        /// unbounded <see cref="SubdivisionFace"/>.</summary>
        /// <value>
        /// <c>true</c> if <see cref="ElementType"/> equals <see
        /// cref="SubdivisionElementType.Face"/>, and <see cref="Face"/> is either a null reference
        /// or has a <see cref="SubdivisionFace.Key"/> of zero; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// By convention, <see cref="NullFace"/> represents the unbounded face of a planar <see
        /// cref="Subdivision"/>. The actual unbounded <see cref="SubdivisionFace"/> always has a
        /// <see cref="SubdivisionFace.Key"/> of zero. <b>IsUnboundedFace</b> tests for both
        /// possibilities.</remarks>

        public bool IsUnboundedFace {
            get {
                return (ElementType == SubdivisionElementType.Face &&
                    (_value == null || ((SubdivisionFace) _value)._key == 0));
            }
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="SubdivisionElement"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> containing the value of the <see cref="ElementType"/> property, 
        /// followed by the value of either the <see cref="Edge"/>, <see cref="Face"/>, or <see
        /// cref="Vertex"/> property.</returns>

        public override string ToString() {

            string value = (ElementType == SubdivisionElementType.Vertex ?
                _vertex.ToString() : StringUtility.Validate(_value));

            return String.Format(CultureInfo.InvariantCulture,
                "{{Type={0}, Value={1}}}", ElementType, value);
        }

        #endregion
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="SubdivisionElement"/> instances have the same value.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="SubdivisionElement"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="SubdivisionElement"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(SubdivisionElement)"/> method to test the
        /// two <see cref="SubdivisionElement"/> instances for value equality.</remarks>

        public static bool operator ==(SubdivisionElement a, SubdivisionElement b) {
            return a.Equals(b);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="SubdivisionElement"/> instances have different values.
        /// </summary>
        /// <param name="a">
        /// The first <see cref="SubdivisionElement"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="SubdivisionElement"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is different from the value of
        /// <paramref name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(SubdivisionElement)"/> method to test the
        /// two <see cref="SubdivisionElement"/> instances for value inequality.</remarks>

        public static bool operator !=(SubdivisionElement a, SubdivisionElement b) {
            return !a.Equals(b);
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="SubdivisionElement"/> instances have the same value.
        /// </overloads>
        /// <summary>
        /// Determines whether this <see cref="SubdivisionElement"/> instance and a specified
        /// object, which must be a <see cref="SubdivisionElement"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="SubdivisionElement"/> instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="SubdivisionElement"/>
        /// instance and its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="SubdivisionElement"/>
        /// instance, <b>Equals</b> invokes the strongly-typed <see
        /// cref="Equals(SubdivisionElement)"/> overload to test the two instances for value
        /// equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is SubdivisionElement))
                return false;

            return Equals((SubdivisionElement) obj);
        }

        #endregion
        #region Equals(SubdivisionElement)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="SubdivisionElement"/> have
        /// the same value.</summary>
        /// <param name="element">
        /// A <see cref="SubdivisionElement"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="element"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="PropertyValueException">
        /// <see cref="ElementType"/> is not a valid <see cref="SubdivisionElementType"/> value.
        /// </exception>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="ElementType"/> property and either
        /// the <see cref="Edge"/>, <see cref="Face"/>, or <see cref="Vertex"/> property of the two
        /// <see cref="SubdivisionElement"/> instances to test for value equality.</remarks>

        public bool Equals(SubdivisionElement element) {
            if (ElementType != element.ElementType)
                return false;

            switch (ElementType) {
                case SubdivisionElementType.Edge:
                case SubdivisionElementType.Face:
                    return Object.ReferenceEquals(_value, element._value);

                case SubdivisionElementType.Vertex:
                    return _vertex.Equals(element._vertex);

                default:
                    ThrowHelper.ThrowPropertyValueException(
                        "ElementType", ElementType, Strings.PropertyInvalidValue);
                    return false;
            }
        }

        #endregion
        #region Equals(SubdivisionElement, SubdivisionElement)

        /// <summary>
        /// Determines whether two specified <see cref="SubdivisionElement"/> instances have the
        /// same value.</summary>
        /// <param name="a">
        /// The first <see cref="SubdivisionElement"/> to compare.</param>
        /// <param name="b">
        /// The second <see cref="SubdivisionElement"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref
        /// name="b"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(SubdivisionElement)"/> overload
        /// to test the two <see cref="SubdivisionElement"/> instances for value equality.</remarks>

        public static bool Equals(SubdivisionElement a, SubdivisionElement b) {
            return a.Equals(b);
        }

        #endregion
        #endregion
        #region ISerializable Members

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> object with the data needed to serialize the
        /// <see cref="SubdivisionElement"/>.</summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> object that receives the serialized object data of
        /// the <see cref="SubdivisionElement"/>.</param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> object containing contextual information about the
        /// source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="info"/> is a null reference.</exception>
        /// <remarks><para>
        /// Please refer to <see cref="ISerializable.GetObjectData"/> for details.
        /// </para><para>
        /// The values of the <see cref="ElementType"/> property and either the <see cref="Edge"/>,
        /// <see cref="Face"/>, or <see cref="Vertex"/> property are serialized to two fields, named
        /// "Type" and "Value".</para></remarks>

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {

            info.AddValue("Type", ElementType);
            info.AddValue("Value",
                (ElementType == SubdivisionElementType.Vertex ? _vertex : _value));
        }

        #endregion
    }
}
