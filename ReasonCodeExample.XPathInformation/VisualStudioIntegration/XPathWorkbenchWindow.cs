using System.Runtime.InteropServices;
using System.Windows.Controls;
using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    [Guid(Symbols.ToolWindowIDs.XPathWorkbench)]
    internal sealed class XPathWorkbenchWindow : ToolWindowPane
    {
        public XPathWorkbenchWindow()
            : base(null)
        {
            Caption = "XPath Workbench";
            Content = new TextBox {Text = "Hello World!"};
        }
    }
}