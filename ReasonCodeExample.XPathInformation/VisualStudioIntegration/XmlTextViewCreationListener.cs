using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Ninject;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [ContentType(XmlContentTypeName)]
    internal class XmlTextViewCreationListener : IWpfTextViewCreationListener
    {
        private const string XmlContentTypeName = "XML";
        private readonly XmlRepository _repository;
        private readonly XPathStatusbarInformation _statusbar;

        public XmlTextViewCreationListener()
            : this(Registry.Current.Get<XmlRepository>(), Registry.Current.Get<XPathStatusbarInformation>())
        {
        }

        public XmlTextViewCreationListener(XmlRepository repository, XPathStatusbarInformation statusbar)
        {
            _repository = repository;
            _statusbar = statusbar;
        }

        public void TextViewCreated(IWpfTextView textView)
        {
            if(textView == null)
            {
                return;
            }
            if(textView.Caret == null)
            {
                return;
            }
            textView.Caret.PositionChanged += StoreCurrentNode;
            textView.Caret.PositionChanged += _statusbar.UpdateXPath;
        }

        private void StoreCurrentNode(object sender, CaretPositionChangedEventArgs e)
        {
            if(e == null)
            {
                return;
            }
            if(e.TextView == null)
            {
                return;
            }
            var textView = e.TextView;
            var caretPosition = new CaretPositionLineInfo(textView, textView.Caret.Position.BufferPosition);
            StoreCurrentNode(textView.TextSnapshot.GetText(), caretPosition.LineNumber, caretPosition.LinePosition);
        }

        private void StoreCurrentNode(string xml, int lineNumber, int linePosition)
        {
            var rootElement = _repository.GetRootElement(xml);
            var selectedElement = _repository.GetElement(rootElement, lineNumber, linePosition);
            var selectedAttribute = _repository.GetAttribute(selectedElement, lineNumber, linePosition);
            _repository.Put(selectedAttribute as XObject ?? selectedElement);
        }
    }
}