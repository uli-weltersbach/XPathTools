using System;
using System.Xml;
using Microsoft.VisualStudio.Text.Editor;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    internal class CaretPositionLineInfo : IXmlLineInfo
    {
        public CaretPositionLineInfo(CaretPositionChangedEventArgs e)
            : this(e.TextView, e.NewPosition.BufferPosition.Position)
        {
        }

        public CaretPositionLineInfo(ITextView textView, int caretPosition)
        {
            if (textView == null)
                throw new ArgumentNullException("textView");
            LineNumber = GetLineNumber(textView, caretPosition);
            LinePosition = GetLinePosition(textView, caretPosition);
        }

        public int LineNumber
        {
            get;
            private set;
        }

        public int LinePosition
        {
            get;
            private set;
        }

        public bool HasLineInfo()
        {
            return true;
        }

        private int GetLineNumber(ITextView textView, int caretPosition)
        {
            return textView.TextSnapshot.GetLineNumberFromPosition(caretPosition) + Constants.TextEditorLineNumberOffset;
        }

        private int GetLinePosition(ITextView textView, int caretPosition)
        {
            int lineStart = textView.TextSnapshot.GetLineFromPosition(caretPosition).Start;
            var caretPositionInLine = caretPosition - lineStart;
            return caretPositionInLine + Constants.XmlLineInfoLinePositionOffset;
        }
    }
}