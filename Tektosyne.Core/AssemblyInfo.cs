using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Tektosyne Library Core Components")]
[assembly: AssemblyDescription("Contains platform-independent components for the Tektosyne library.")]
[assembly: AssemblyVersion("5.6.6.0")]
[assembly: AssemblyFileVersion("5.6.6.0")]

[assembly: AssemblyProduct("Tektosyne")]
[assembly: AssemblyInformationalVersion("5.6.6")]
[assembly: AssemblyCompany("Kynosarges")]
[assembly: AssemblyCopyright("Copyright \u00A9 2002-2015 Christoph Nahr")]

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

// embedded resources are based on English culture
[assembly: NeutralResourcesLanguageAttribute("en-US")]

#if SIGNED
[assembly: InternalsVisibleTo("Tektosyne.UnitTest, PublicKey=0024000004800000940000000602000000240000525341310004000001000100a575fb1653022afb50543ba7db159a9c97f8d3e0d85cca4ae55c3ec38d78e30ca334a7eca4b6c158a6d934e94ba2bce9452f0854c9ff45cffa46327bc9333457da70cff894f8e13dca9f4ac46bf342eb8ffba78326fe0b854815c6a4ea850293961ffd702b9e7ad284ed5ca161db072c9f1686b5d0413f10e868fe41215c4fdd")]
#else
[assembly: InternalsVisibleTo("Tektosyne.UnitTest")]
#endif
