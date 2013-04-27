using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;

namespace ReasonCodeExample.XPathInformation
{
    internal class XPathStatusbarInformation
    {
        private const int TextEditorLineNumberOffset = 1;
        private const int XmlLineInfoLinePositionOffset = 1;
        private readonly ResultCachingXmlParser _parser = new ResultCachingXmlParser();
        private readonly XmlNodeRepository _repository = new XmlNodeRepository();
        private readonly XPathFormatter _formatter = new XPathFormatter();
        private readonly IVsStatusbar _statusbar;

        public XPathStatusbarInformation(ITextView view)
        {
            view.Caret.PositionChanged += UpdateXPath;
            _statusbar = (IVsStatusbar)ServiceProvider.GlobalProvider.GetService(typeof(IVsStatusbar));
        }

        private void UpdateXPath(object sender, CaretPositionChangedEventArgs e)
        {
            try
            {
                string xpath = GetXPath(e);
                _statusbar.SetText(xpath);
            }
            catch (Exception ex)
            {
                _statusbar.SetText(ex.Message);
            }
        }

        private string GetXPath(CaretPositionChangedEventArgs e)
        {
            XElement rootElement = _parser.Parse(e.TextView.TextSnapshot.GetText());
            int lineNumber = GetLineNumber(e);
            int linePosition = GetLinePosition(e, lineNumber);
            XElement selectedElement = _repository.GetElement(rootElement, lineNumber + TextEditorLineNumberOffset, linePosition + XmlLineInfoLinePositionOffset);
            XAttribute selectedAttribute = _repository.GetAttribute(selectedElement, linePosition + XmlLineInfoLinePositionOffset);
            return _formatter.Format(selectedElement) + _formatter.Format(selectedAttribute);
        }

        private int GetLineNumber(CaretPositionChangedEventArgs e)
        {
            return e.TextView.TextSnapshot.GetLineNumberFromPosition(e.NewPosition.BufferPosition);
        }

        private int GetLinePosition(CaretPositionChangedEventArgs e, int lineNumber)
        {
            int lineStart = e.TextView.TextSnapshot.GetLineFromLineNumber(lineNumber).Start.Position;
            int caretPositionInDocument = e.NewPosition.BufferPosition.Position;
            int caretPositionInLine = caretPositionInDocument - lineStart;
            return caretPositionInLine;
        }
    }
}