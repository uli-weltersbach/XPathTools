using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ReasonCodeExample.XPathTools.Properties;

[assembly: AssemblyTitle("ReasonCodeExample.XPathTools")]
[assembly: AssemblyCompany("Reason→Code→Example (http://reasoncodeexample.com)")]
[assembly: AssemblyProduct("ReasonCodeExample.XPathTools")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("6.0.0.*")]
[assembly: InternalsVisibleTo(InternalsVisibleTo.ReasonCodeExampleXPathToolsTests)]
[assembly: InternalsVisibleTo(InternalsVisibleTo.DynamicProxyGenAssembly2)]
[assembly: InternalsVisibleTo(InternalsVisibleTo.CastleCore)]

namespace ReasonCodeExample.XPathTools.Properties
{
    internal static class InternalsVisibleTo
    {
        public const string ReasonCodeExampleXPathToolsTests = "ReasonCodeExample.XPathTools.Tests";
        public const string CastleCore = "Castle.Core";
        public const string DynamicProxyGenAssembly2 = "DynamicProxyGenAssembly2";
    }
}
