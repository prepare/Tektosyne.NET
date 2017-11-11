using System;
using System.Reflection;

namespace Tektosyne {

    /// <summary>
    /// Provides extension methods for <b>System.Reflection.Assembly</b>.</summary>
    /// <remarks>
    /// <b>AssemblyExtensions</b> allows convenient retrieval of informational attributes and public
    /// key tokens from a specified <see cref="Assembly"/>.</remarks>

    public static class AssemblyExtensions {
        #region Company

        /// <summary>
        /// Gets the company name for the specified <see cref="Assembly"/>.</summary>
        /// <param name="assembly">
        /// The <see cref="Assembly"/> to examine.</param>
        /// <returns>
        /// The content of the <see cref="AssemblyCompanyAttribute"/> attribute for the specified
        /// <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is a null reference.</exception>
        /// <remarks>
        /// <b>Company</b> returns an empty string if the specified <paramref name="assembly"/> does
        /// not define an <see cref="AssemblyCompanyAttribute"/>.</remarks>

        public static string Company(this Assembly assembly) {
            if (assembly == null)
                ThrowHelper.ThrowArgumentNullException("assembly");

            Attribute attribute = Attribute.GetCustomAttribute(
                assembly, typeof(AssemblyCompanyAttribute));

            return (attribute == null ? "" :
                ((AssemblyCompanyAttribute) attribute).Company);
        }

        #endregion
        #region Copyright

        /// <summary>
        /// Gets the copyright notice for the specified <see cref="Assembly"/>.</summary>
        /// <param name="assembly">
        /// The <see cref="Assembly"/> to examine.</param>
        /// <returns>
        /// The content of the <see cref="AssemblyCopyrightAttribute"/> attribute for the specified
        /// <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is a null reference.</exception>
        /// <remarks>
        /// <b>Copyright</b> returns an empty string if the specified <paramref name="assembly"/>
        /// does not define an <see cref="AssemblyCopyrightAttribute"/>.</remarks>

        public static string Copyright(this Assembly assembly) {
            if (assembly == null)
                ThrowHelper.ThrowArgumentNullException("assembly");

            Attribute attribute = Attribute.GetCustomAttribute(
                assembly, typeof(AssemblyCopyrightAttribute));

            return (attribute == null ? "" :
                ((AssemblyCopyrightAttribute) attribute).Copyright);
        }

        #endregion
        #region Description

        /// <summary>
        /// Gets description information for the specified <see cref="Assembly"/>.</summary>
        /// <param name="assembly">
        /// The <see cref="Assembly"/> to examine.</param>
        /// <returns>
        /// The content of the <see cref="AssemblyDescriptionAttribute"/> attribute for the
        /// specified <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is a null reference.</exception>
        /// <remarks>
        /// <b>Description</b> returns an empty string if the specified <paramref name="assembly"/>
        /// does not define an <see cref="AssemblyDescriptionAttribute"/>.</remarks>

        public static string Description(this Assembly assembly) {
            if (assembly == null)
                ThrowHelper.ThrowArgumentNullException("assembly");

            Attribute attribute = Attribute.GetCustomAttribute(
                assembly, typeof(AssemblyDescriptionAttribute));

            return (attribute == null ? "" :
                ((AssemblyDescriptionAttribute) attribute).Description);
        }

        #endregion
        #region FileVersion

        /// <summary>
        /// Gets the file version number for the specified <see cref="Assembly"/>.</summary>
        /// <param name="assembly">
        /// The <see cref="Assembly"/> to examine.</param>
        /// <returns>
        /// The content of the <see cref="AssemblyFileVersionAttribute"/> attribute for the
        /// specified <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is a null reference.</exception>
        /// <remarks>
        /// <b>FileVersion</b> returns the value of <see cref="Version"/> if the specified <paramref
        /// name="assembly"/> does not define an <see cref="AssemblyFileVersionAttribute"/>.
        /// </remarks>

        public static string FileVersion(this Assembly assembly) {
            if (assembly == null)
                ThrowHelper.ThrowArgumentNullException("assembly");

            Attribute attribute = Attribute.GetCustomAttribute(
                assembly, typeof(AssemblyFileVersionAttribute));

            return (attribute == null ?
                assembly.GetName().Version.ToString() :
                ((AssemblyFileVersionAttribute) attribute).Version);
        }

        #endregion
        #region InformationalVersion

        /// <summary>
        /// Gets version information for the specified <see cref="Assembly"/>.</summary>
        /// <param name="assembly">
        /// The <see cref="Assembly"/> to examine.</param>
        /// <returns>
        /// The content of the <see cref="AssemblyInformationalVersionAttribute"/> attribute for the
        /// specified <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is a null reference.</exception>
        /// <remarks>
        /// <b>InformationalVersion</b> returns the value of <see cref="Version"/> if the specified
        /// <paramref name="assembly"/> does not define an <see
        /// cref="AssemblyInformationalVersionAttribute"/>.</remarks>

        public static string InformationalVersion(this Assembly assembly) {
            if (assembly == null)
                ThrowHelper.ThrowArgumentNullException("assembly");

            Attribute attribute = Attribute.GetCustomAttribute(
                assembly, typeof(AssemblyInformationalVersionAttribute));

            return (attribute == null ?
                assembly.GetName().Version.ToString() :
                ((AssemblyInformationalVersionAttribute) attribute).InformationalVersion);
        }

        #endregion
        #region Product

        /// <summary>
        /// Gets the product name for the specified <see cref="Assembly"/>.</summary>
        /// <param name="assembly">
        /// The <see cref="Assembly"/> to examine.</param>
        /// <returns>
        /// The content of the <see cref="AssemblyProductAttribute"/> attribute for the specified
        /// <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is a null reference.</exception>
        /// <remarks>
        /// <b>Product</b> returns an empty string if the specified <paramref name="assembly"/> does
        /// not define an <see cref="AssemblyProductAttribute"/>.</remarks>

        public static string Product(this Assembly assembly) {
            if (assembly == null)
                ThrowHelper.ThrowArgumentNullException("assembly");

            Attribute attribute = Attribute.GetCustomAttribute(
                assembly, typeof(AssemblyProductAttribute));

            return (attribute == null ? "" :
                ((AssemblyProductAttribute) attribute).Product);
        }

        #endregion
        #region PublicKeyToken

        /// <summary>
        /// Gets the public key token for the specified <see cref="Assembly"/>.</summary>
        /// <param name="assembly">
        /// The <see cref="Assembly"/> to examine.</param>
        /// <returns>
        /// A string of hexadecimal digits representing the public key token of the <see
        /// cref="AssemblyName"/> for the specified <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is a null reference.</exception>
        /// <remarks>
        /// <b>PublicKeyToken</b> returns a null reference if the specified <paramref
        /// name="assembly"/> is not signed with a strong name.</remarks>

        public static string PublicKeyToken(this Assembly assembly) {
            if (assembly == null)
                ThrowHelper.ThrowArgumentNullException("assembly");

            byte[] token = assembly.GetName().GetPublicKeyToken();
            return (token == null ? null : BitConverter.ToString(token));
        }

        #endregion
        #region Title

        /// <summary>
        /// Gets title information for the specified <see cref="Assembly"/>.</summary>
        /// <param name="assembly">
        /// The <see cref="Assembly"/> to examine.</param>
        /// <returns>
        /// The content of the <see cref="AssemblyTitleAttribute"/> attribute for the specified
        /// <paramref name="assembly"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is a null reference.</exception>
        /// <remarks>
        /// <b>Title</b> returns an empty string if the specified <paramref name="assembly"/> does
        /// not define an <see cref="AssemblyTitleAttribute"/>.</remarks>

        public static string Title(this Assembly assembly) {
            if (assembly == null)
                ThrowHelper.ThrowArgumentNullException("assembly");

            Attribute attribute = Attribute.GetCustomAttribute(
                assembly, typeof(AssemblyTitleAttribute));

            return (attribute == null ? "" :
                ((AssemblyTitleAttribute) attribute).Title);
        }

        #endregion
        #region Version

        /// <summary>
        /// Gets the version number for the specified <see cref="Assembly"/>.</summary>
        /// <param name="assembly">
        /// The <see cref="Assembly"/> to examine.</param>
        /// <returns>
        /// The <see cref="AssemblyName.Version"/> of the <see cref="AssemblyName"/> for the
        /// specified <paramref name="assembly"/>, formatted with all four components.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is a null reference.</exception>
        /// <remarks>
        /// <b>Version</b> returns the literal string “0.0.0.0” if the specified <paramref
        /// name="assembly"/> does not define an <see cref="AssemblyVersionAttribute"/> or otherwise
        /// specify a non-default <see cref="AssemblyName.Version"/>.</remarks>

        public static string Version(this Assembly assembly) {
            if (assembly == null)
                ThrowHelper.ThrowArgumentNullException("assembly");

            /*
             * AssemblyVersionAttribute can be used to specify AssemblyName.Version but is
             * never actually stored as a custom attribute, only in AssemblyName.Version.
             */
            return assembly.GetName().Version.ToString();
        }

        #endregion
    }
}
