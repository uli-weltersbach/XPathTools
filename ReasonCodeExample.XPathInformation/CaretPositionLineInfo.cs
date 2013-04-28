using System;
using System.Xml;
using Microsoft.VisualStudio.Text.Editor;

namespace ReasonCodeExample.XPathInformation
{
    internal class CaretPositionLineInfo : IXmlLineInfo
    {
        /// <summary>
        /// Visual Studio text lines are 0-based while 
        /// <c>IXmlLineInfo</c> uses 1-based.
        /// </summary>
        private const int TextEditorLineNumberOffset = 1;

        /// <summary>
        /// The start of an <c>IXmlLineInfo</c> is the first letter in 
        /// the element name (e.g. "f" in &lt;fitting&gt;).
        /// </summary>
        private const int XmlLineInfoLinePositionOffset = 1;

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
            return textView.TextSnapshot.GetLineNumberFromPosition(caretPosition) + TextEditorLineNumberOffset;
        }

        private int GetLinePosition(ITextView textView, int caretPosition)
        {
            int lineStart = textView.TextSnapshot.GetLineFromPosition(caretPosition).Start;
            int caretPositionInLine = caretPosition - lineStart;
            return caretPositionInLine + XmlLineInfoLinePositionOffset;
        }
    }
}