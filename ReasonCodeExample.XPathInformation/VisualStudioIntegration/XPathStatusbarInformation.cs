using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    internal class XPathStatusbarInformation
    {
        private readonly IXPathFormatter _formatter = new GenericXPathFormatter();
        private readonly XmlNodeRepository _nodeRepository = new XmlNodeRepository();
        private readonly XObjectRepository _objectRepository = new XObjectRepository();
        private readonly IVsStatusbar _statusbar;

        public XPathStatusbarInformation(ITextView textView)
            : this(textView, (IVsStatusbar) ServiceProvider.GlobalProvider.GetService(typeof (IVsStatusbar)))
        {
        }

        public XPathStatusbarInformation(ITextView textView, IVsStatusbar statusbar)
        {
            if (textView == null)
                throw new ArgumentNullException("textView");
            if (statusbar == null)
                throw new ArgumentNullException("statusbar");
            textView.Caret.PositionChanged += UpdateXPath;
            _statusbar = statusbar;
        }

        private void UpdateXPath(object sender, CaretPositionChangedEventArgs e)
        {
            try
            {
                StoreCurrentNode(e.TextView);
                string xpath = _formatter.Format(_objectRepository.Get());
                _statusbar.SetText(xpath);
            }
            catch (Exception ex)
            {
                _statusbar.SetText(ex.Message);
            }
        }

        private void StoreCurrentNode(ITextView textView)
        {
            IXmlLineInfo caretPosition = new CaretPositionLineInfo(textView, textView.Caret.Position.BufferPosition);
            StoreCurrentNode(textView.TextSnapshot.GetText(), caretPosition.LineNumber, caretPosition.LinePosition);
        }

        private void StoreCurrentNode(string xml, int lineNumber, int linePosition)
        {
            XElement rootElement = _nodeRepository.GetRootElement(xml);
            XElement selectedElement = _nodeRepository.GetElement(rootElement, lineNumber, linePosition);
            XAttribute selectedAttribute = _nodeRepository.GetAttribute(selectedElement, lineNumber, linePosition);
            _objectRepository.Put(selectedAttribute as XObject ?? selectedElement);
        }
    }
}