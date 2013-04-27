using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace ReasonCodeExample.XPathInformation
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [ContentType(XmlContentTypeName)]
    internal class XPathStatusbarInformationFactory : IWpfTextViewCreationListener
    {
        private const string XmlContentTypeName = "XML";

        public void TextViewCreated(IWpfTextView textView)
        {
            new XPathStatusbarInformation(textView);
        }
    }
}
