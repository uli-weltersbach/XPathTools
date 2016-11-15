using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ReasonCodeExample.XPathInformation.Properties;

[assembly: AssemblyTitle("ReasonCodeExample.XPathInformation")]
[assembly: AssemblyCompany("Reason→Code→Example (http://reasoncodeexample.com)")]
[assembly: AssemblyProduct("ReasonCodeExample.XPathInformation")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("4.0.0.*")]
[assembly: InternalsVisibleTo(InternalsVisibleTo.ReasonCodeExampleXPathInformationTests)]
[assembly: InternalsVisibleTo(InternalsVisibleTo.DynamicProxyGenAssembly2)]
[assembly: InternalsVisibleTo(InternalsVisibleTo.CastleCore)]

namespace ReasonCodeExample.XPathInformation.Properties
{
    internal static class InternalsVisibleTo
    {
        public const string ReasonCodeExampleXPathInformationTests = "ReasonCodeExample.XPathInformation.Tests";
        public const string CastleCore = "Castle.Core";
        public const string DynamicProxyGenAssembly2 = "DynamicProxyGenAssembly2";
    }
}