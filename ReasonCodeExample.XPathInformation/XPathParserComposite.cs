using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.Text.Editor;

namespace ReasonCodeExample.XPathInformation
{
    internal class XPathParserComposite
    {
        private readonly XPathFormatter _formatter = new XPathFormatter();
        private readonly ResultCachingXmlParser _parser = new ResultCachingXmlParser();
        private readonly XmlNodeRepository _repository = new XmlNodeRepository();

        public virtual string GetXPath(ITextView textView)
        {
            XElement rootElement = _parser.Parse(textView.TextSnapshot.GetText());
            IXmlLineInfo caretPosition = new CaretPositionLineInfo(textView, textView.Caret.Position.BufferPosition);
            XElement selectedElement = _repository.GetElement(rootElement, caretPosition.LineNumber, caretPosition.LinePosition);
            XAttribute selectedAttribute = _repository.GetAttribute(selectedElement, caretPosition.LinePosition);
            return _formatter.Format(selectedElement) + _formatter.Format(selectedAttribute);
        }
    }
}