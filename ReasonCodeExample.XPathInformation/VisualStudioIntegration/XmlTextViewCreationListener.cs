using System;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Ninject;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [ContentType(Constants.XmlContentTypeName)]
    internal class XmlTextViewCreationListener : IWpfTextViewCreationListener
    {
        private readonly XmlRepository _repository;
        private readonly StatusbarAdapter _statusbar;

        public XmlTextViewCreationListener()
            : this(Registry.Current.Get<XmlRepository>(), Registry.Current.Get<StatusbarAdapter>())
        {
        }

        public XmlTextViewCreationListener(XmlRepository repository, StatusbarAdapter statusbar)
        {
            _repository = repository;
            _statusbar = statusbar;
        }

        public void TextViewCreated(IWpfTextView textView)
        {
            if(textView?.Caret == null)
            {
                return;
            }
            _repository.LoadXml(textView.TextSnapshot.GetText());
            textView.Closed += ResetXml;
            textView.Caret.PositionChanged += StoreCurrentNode;
            textView.Caret.PositionChanged += _statusbar.SetText;
        }

        private void ResetXml(object sender, EventArgs e)
        {
            _repository.LoadXml(null);
        }

        private void StoreCurrentNode(object sender, CaretPositionChangedEventArgs e)
        {
            if(e?.TextView == null)
            {
                return;
            }
            var textView = e.TextView;
            var caretPosition = new CaretPositionLineInfo(textView, textView.Caret.Position.BufferPosition);
            StoreCurrentNode(textView.TextSnapshot.GetText(), caretPosition.LineNumber, caretPosition.LinePosition);
        }

        private void StoreCurrentNode(string xml, int lineNumber, int linePosition)
        {
            try
            {
                _repository.LoadXml(xml);
                var rootElement = _repository.GetRootElement();
                var selectedElement = _repository.GetElement(rootElement, lineNumber, linePosition);
                var selectedAttribute = _repository.GetAttribute(selectedElement, lineNumber, linePosition);
                _repository.SetSelected((XObject)selectedAttribute ?? selectedElement);
            }
            catch(Exception ex)
            {
                _statusbar.SetText(ex.Message);
            }
        }
    }
}