using System;
using System.Diagnostics;
using System.Globalization;

namespace Tektosyne.Net {

    /// <summary>
    /// Represents the sender or recipient of a Simple MAPI message.</summary>
    /// <remarks><para>
    /// The <see cref="MapiMail"/> class uses <b>MapiAddress</b> instances to represent the sender
    /// or recipient of a Simple MAPI message. Each <b>MapiAddress</b> instance associates a display
    /// name with an actual e-mail address.
    /// </para><para>
    /// <b>MapiAddress</b> may also describe a file attachment. In that case, the <see
    /// cref="MapiAddress.Name"/> component represents the display name of the file, and the <see
    /// cref="MapiAddress.Address"/> component represents its local file path.</para></remarks>

    [Serializable]
    public struct MapiAddress: IEquatable<MapiAddress> {
        #region MapiAddress(String, String)

        /// <summary>
        /// Initializes a new instance of the <see cref="MapiAddress"/> structure.</summary>
        /// <param name="name">
        /// The display name of the <see cref="MapiAddress"/>.</param>
        /// <param name="address">
        /// The e-mail address or the file path of the <see cref="MapiAddress"/>.</param>

        public MapiAddress(string name, string address) {
            _name = name;
            _address = address;
        }

        #endregion
        #region Private Fields

        // property backers
        private readonly string _name, _address;

        #endregion
        #region Empty

        /// <summary>
        /// An empty read-only <see cref="MapiAddress"/> instance.</summary>
        /// <remarks>
        /// <b>Empty</b> contains a <see cref="MapiAddress"/> instance that was created with the
        /// default constructor.</remarks>

        public static readonly MapiAddress Empty = new MapiAddress();

        #endregion
        #region Name

        /// <summary>
        /// Gets the display name of the <see cref="MapiAddress"/>.</summary>
        /// <value>
        /// The display name of the <see cref="MapiAddress"/>. The default is an empty string.
        /// </value>
        /// <remarks>
        /// <b>Name</b> returns an empty string when set to a null reference.</remarks>

        public string Name {
            [DebuggerStepThrough]
            get { return (_name ?? ""); }
        }

        #endregion
        #region Address

        /// <summary>
        /// Gets the e-mail address or the file path of the <see cref="MapiAddress"/>.</summary>
        /// <value>
        /// The e-mail address or the file path of the <see cref="MapiAddress"/>. The default is an
        /// empty string.</value>
        /// <remarks>
        /// <b>Address</b> returns an empty string when set to a null reference.</remarks>

        public string Address {
            [DebuggerStepThrough]
            get { return (_address ?? ""); }
        }

        #endregion
        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this <see cref="MapiAddress"/> instance.</summary>
        /// <returns>
        /// An <see cref="Int32"/> hash code.</returns>
        /// <remarks>
        /// <b>GetHashCode</b> combines the results of <see cref="String.GetHashCode"/> for the <see
        /// cref="Name"/> or <see cref="Address"/> properties.</remarks>

        public override int GetHashCode() {
            unchecked { return Name.GetHashCode() ^ Address.GetHashCode(); }
        }

        #endregion
        #region ToString

        /// <summary>
        /// Returns a <see cref="String"/> that represents the <see cref="MapiAddress"/>.</summary>
        /// <returns>
        /// A <see cref="String"/> containing the culture-invariant string representations of the
        /// <see cref="Name"/> and <see cref="Address"/> properties.</returns>

        public override string ToString() {
            return String.Format(CultureInfo.InvariantCulture, "{{Name={0}, Address={1}}}",
                StringUtility.Validate(Name), StringUtility.Validate(Address));
        }

        #endregion
        #region operator==

        /// <summary>
        /// Determines whether two <see cref="MapiAddress"/> instances have the same value.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="MapiAddress"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="MapiAddress"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(MapiAddress)"/> method to test the two <see
        /// cref="MapiAddress"/> instances for value equality.</remarks>

        public static bool operator ==(MapiAddress x, MapiAddress y) {
            return x.Equals(y);
        }

        #endregion
        #region operator!=

        /// <summary>
        /// Determines whether two <see cref="MapiAddress"/> instances have different values.
        /// </summary>
        /// <param name="x">
        /// The first <see cref="MapiAddress"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="MapiAddress"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is different from the value of
        /// <paramref name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This operator invokes the <see cref="Equals(MapiAddress)"/> method to test the two <see
        /// cref="MapiAddress"/> instances for value inequality.</remarks>

        public static bool operator !=(MapiAddress x, MapiAddress y) {
            return !x.Equals(y);
        }

        #endregion
        #region IEquatable Members
        #region Equals(Object)

        /// <overloads>
        /// Determines whether two <see cref="MapiAddress"/> instances have the same value.
        /// </overloads>
        /// <summary>
        /// Determines whether this <see cref="MapiAddress"/> instance and a specified object, which
        /// must be a <see cref="MapiAddress"/>, have the same value.</summary>
        /// <param name="obj">
        /// An <see cref="Object"/> to compare to this <see cref="MapiAddress"/> instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is another <see cref="MapiAddress"/> instance and
        /// its value is the same as this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// If the specified <paramref name="obj"/> is another <see cref="MapiAddress"/> instance,
        /// <b>Equals</b> invokes the strongly-typed <see cref="Equals(MapiAddress)"/> overload to
        /// test the two instances for value equality.</remarks>

        public override bool Equals(object obj) {
            if (obj == null || !(obj is MapiAddress))
                return false;

            return Equals((MapiAddress) obj);
        }

        #endregion
        #region Equals(MapiAddress)

        /// <summary>
        /// Determines whether this instance and a specified <see cref="MapiAddress"/> have the same
        /// value.</summary>
        /// <param name="pair">
        /// A <see cref="MapiAddress"/> to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="pair"/> is the same as this instance;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> compares the values of the <see cref="Name"/> and <see cref="Address"/>
        /// properties of the two <see cref="MapiAddress"/> instances to test for value equality.
        /// </remarks>

        public bool Equals(MapiAddress pair) {
            return (Name == pair.Name && Address == pair.Address);
        }

        #endregion
        #region Equals(MapiAddress, MapiAddress)

        /// <summary>
        /// Determines whether two specified <see cref="MapiAddress"/> instances have the same
        /// value.</summary>
        /// <param name="x">
        /// The first <see cref="MapiAddress"/> to compare.</param>
        /// <param name="y">
        /// The second <see cref="MapiAddress"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if the value of <paramref name="x"/> is the same as the value of <paramref
        /// name="y"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>Equals</b> invokes the non-static <see cref="Equals(MapiAddress)"/> overload to test
        /// the two <see cref="MapiAddress"/> instances for value equality.</remarks>

        public static bool Equals(MapiAddress x, MapiAddress y) {
            return x.Equals(y);
        }

        #endregion
        #endregion
    }
}
