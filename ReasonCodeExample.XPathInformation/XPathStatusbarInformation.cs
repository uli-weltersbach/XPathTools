using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;

namespace ReasonCodeExample.XPathInformation
{
    internal class XPathStatusbarInformation
    {
        private const int LineNumberOffset = 1;
        private const int LinePositionOffset = 1;
        private readonly IVsStatusbar _statusbar;

        public XPathStatusbarInformation(IWpfTextView view)
        {
            view.Caret.PositionChanged += UpdateXPath;
            _statusbar = (IVsStatusbar)ServiceProvider.GlobalProvider.GetService(typeof(IVsStatusbar));
        }

        private void UpdateXPath(object sender, CaretPositionChangedEventArgs e)
        {
            string xpath = TryGetXPath(e);
            UpdateStatusbarText(xpath);
        }

        private string TryGetXPath(CaretPositionChangedEventArgs e)
        {
            string xml = e.TextView.TextSnapshot.GetText();
            if (string.IsNullOrEmpty(xml))
                return string.Empty;

            int lineNumber = GetLineNumber(e);
            int caretPosition = GetCaretPosition(e, lineNumber);
            try
            {
                XElement rootElement = XElement.Parse(xml, LoadOptions.SetLineInfo);
                XElement selectedElement = new XmlNodeRepository().Get(rootElement, lineNumber + LineNumberOffset, caretPosition + LinePositionOffset);
                return new XPathFormatter().Format(selectedElement);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private int GetLineNumber(CaretPositionChangedEventArgs e)
        {
            return e.TextView.TextSnapshot.GetLineNumberFromPosition(e.NewPosition.BufferPosition);
        }

        private int GetCaretPosition(CaretPositionChangedEventArgs e, int lineNumber)
        {
            int lineStart = e.TextView.TextSnapshot.GetLineFromLineNumber(lineNumber).Start.Position;
            int caretPositionInDocument = e.NewPosition.BufferPosition.Position;
            int caretPositionInLine = caretPositionInDocument - lineStart;
            return caretPositionInLine;
        }

        private void UpdateStatusbarText(string xpath)
        {
            _statusbar.SetText(xpath);
        }
    }
}