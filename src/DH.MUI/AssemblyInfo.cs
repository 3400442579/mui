using System.Reflection;
using System.Windows;
using System.Windows.Markup;

[assembly: AssemblyDescription("MUI UI for WPF .NET Core 3.1")]


[assembly: XmlnsDefinition("http://dhmui.com/MUI", "DH.MUI.Presentation")]
[assembly: XmlnsDefinition("http://dhmui.com/MUI", "DH.MUI.Controls")]
[assembly: XmlnsDefinition("http://dhmui.com/MUI", "DH.MUI.Converters")]
[assembly: XmlnsDefinition("http://dhmui.com/MUI", "DH.MUI.Win32")]
[assembly: XmlnsDefinition("http://dhmui.com/MUI", "DH.MUI")]
[assembly: XmlnsPrefix("http://dhmui.com/MUI", "mui")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly
)]
