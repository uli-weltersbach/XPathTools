using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    internal class XPathStatusbarInformation
    {
        private readonly XmlRepository _repository = new XmlRepository();
        private readonly IVsStatusbar _statusbar;
        private readonly XPathWriter _writer;

        public XPathStatusbarInformation(ITextView textView)
            : this(textView, (IVsStatusbar)ServiceProvider.GlobalProvider.GetService(typeof(IVsStatusbar)), XPathInformationConfiguration.Current)
        {
        }

        public XPathStatusbarInformation(ITextView textView, IVsStatusbar statusbar, IConfiguration configuration)
        {
            if(textView == null)
            {
                throw new ArgumentNullException("textView");
            }
            if(statusbar == null)
            {
                throw new ArgumentNullException("statusbar");
            }
            if(configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            textView.Caret.PositionChanged += UpdateXPath;
            _statusbar = statusbar;
            _writer = new XPathWriter(new[] {new AttributeFilter(configuration.AlwaysDisplayedAttributes)});
        }

        private void UpdateXPath(object sender, CaretPositionChangedEventArgs e)
        {
            try
            {
                StoreCurrentNode(e.TextView);
                var xpath = _writer.Write(_repository.Get());
                _statusbar.SetText(xpath);
            }
            catch(Exception ex)
            {
                _statusbar.SetText(ex.Message);
            }
        }

        private void StoreCurrentNode(ITextView textView)
        {
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