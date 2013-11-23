using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.Text.Editor;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation
{
    internal class XPathParserComposite
    {
        private readonly PathFormatter _formatter = new PathFormatter();
        private readonly ResultCachingXmlParser _parser = new ResultCachingXmlParser();
        private readonly XmlNodeRepository _repository = new XmlNodeRepository();

        public virtual string GetXPath(ITextView textView)
        {
            IXmlLineInfo caretPosition = new CaretPositionLineInfo(textView, textView.Caret.Position.BufferPosition);
            return GetXPath(textView.TextSnapshot.GetText(), caretPosition.LineNumber, caretPosition.LinePosition);
        }

        public virtual string GetXPath(string xml, int lineNumber, int linePosition)
        {
            XElement rootElement = _parser.Parse(xml);
            XElement selectedElement = _repository.GetElement(rootElement, lineNumber, linePosition);
            XAttribute selectedAttribute = _repository.GetAttribute(selectedElement, lineNumber, linePosition);
            return _formatter.Format(selectedElement) + _formatter.Format(selectedAttribute);
        }
    }
}