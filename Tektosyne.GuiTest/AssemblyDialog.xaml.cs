using System;
using System.Reflection;
using System.Windows;

namespace Tektosyne.GuiTest {

    /// <summary>
    /// Provides a <see cref="Window"/> that shows <see cref="Assembly"/> information extracted by
    /// the <see cref="AssemblyExtensions"/> class.</summary>

    public partial class AssemblyDialog: Window {

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyDialog"/> class.</summary>

        public AssemblyDialog() {
            InitializeComponent();
            Assembly assembly = Assembly.GetExecutingAssembly();

            ProductText.Content = assembly.Product();
            TitleText.Content = assembly.Title();
            CompanyText.Content = assembly.Company();
            CopyrightText.Content = assembly.Copyright();
            DescriptionText.Content = assembly.Description();

            VersionText.Content = assembly.Version();
            FileVersionText.Content = assembly.FileVersion();
            InfoVersionText.Content = assembly.InformationalVersion();

            string token = assembly.PublicKeyToken();
            if (!String.IsNullOrEmpty(token)) KeyTokenText.Content = token;
        }
    }
}
