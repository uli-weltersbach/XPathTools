using System;
using Microsoft.VisualStudio.Text.Editor;
using System.Xml;

namespace ReasonCodeExample.XPathInformation
{
    internal class CaretPosition : IXmlLineInfo
    {
        private const int TextEditorLineNumberOffset = 1;
        private const int XmlLineInfoLinePositionOffset = 1;

        public CaretPosition(CaretPositionChangedEventArgs eventArgs)
        {
            if (eventArgs == null)
                throw new ArgumentNullException("eventArgs");
            LineNumber = GetLineNumber(eventArgs);
            LinePosition = GetLinePosition(eventArgs);
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

        private int GetLineNumber(CaretPositionChangedEventArgs e)
        {
            return e.TextView.TextSnapshot.GetLineNumberFromPosition(e.NewPosition.BufferPosition) + TextEditorLineNumberOffset;
        }

        private int GetLinePosition(CaretPositionChangedEventArgs e)
        {
            int lineNumber = e.TextView.TextSnapshot.GetLineNumberFromPosition(e.NewPosition.BufferPosition);
            int lineStart = e.TextView.TextSnapshot.GetLineFromLineNumber(lineNumber).Start.Position;
            int caretPositionInDocument = e.NewPosition.BufferPosition.Position;
            int caretPositionInLine = caretPositionInDocument - lineStart;
            return caretPositionInLine + XmlLineInfoLinePositionOffset;
        }

        public bool HasLineInfo()
        {
            return true;
        }
    }
}