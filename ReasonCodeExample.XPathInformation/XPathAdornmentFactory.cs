using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace ReasonCodeExample.XPathInformation
{
    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(XmlContentTypeName)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal class XPathAdornmentFactory : IWpfTextViewCreationListener
    {
        private const string AdornmentLayerName = "ReasonCodeExample.XPathInformation";
        private const string XmlContentTypeName = "XML";

        /// <summary>
        /// Defines the adornment layer for the scarlet adornment. This layer is ordered 
        /// after the selection layer in the Z-order
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name(AdornmentLayerName)]
        [Order(After = PredefinedAdornmentLayers.Caret)]
        public AdornmentLayerDefinition EditorAdornmentLayer
        {
            get;
            set;
        }

        public void TextViewCreated(IWpfTextView textView)
        {
            new XPathAdornment(AdornmentLayerName, textView);
        }
    }
}
