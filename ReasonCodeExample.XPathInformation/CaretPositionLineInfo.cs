using System;
using Microsoft.VisualStudio.Text.Editor;
using System.Xml;
using Microsoft.VisualStudio.Text;

namespace ReasonCodeExample.XPathInformation
{
    internal class CaretPositionLineInfo : IXmlLineInfo
    {
        /// <summary>
        /// Visual Studio text lines are 0-based while 
        /// <c>XElement</c> uses 1-based.
        /// </summary>
        private const int TextEditorLineNumberOffset = 1;

        /// <summary>
        /// The start of an <c>XElement</c> is the first letter in 
        /// the element name (e.g. "f" in &lt;fitting&gt;).
        /// </summary>
        private const int XmlLineInfoLinePositionOffset = 1;

        public CaretPositionLineInfo(CaretPositionChangedEventArgs e)
            : this(e.TextView, e.NewPosition.BufferPosition)
        {
        }

        public CaretPositionLineInfo(ITextView textView, SnapshotPoint caretPosition)
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

        private int GetLineNumber(ITextView textView, SnapshotPoint caretPosition)
        {
            return textView.TextSnapshot.GetLineNumberFromPosition(caretPosition) + TextEditorLineNumberOffset;
        }

        private int GetLinePosition(ITextView textView, SnapshotPoint caretPosition)
        {
            int lineNumber = textView.TextSnapshot.GetLineNumberFromPosition(caretPosition);
            int lineStart = textView.TextSnapshot.GetLineFromLineNumber(lineNumber).Start.Position;
            int caretPositionInDocument = caretPosition.Position;
            int caretPositionInLine = caretPositionInDocument - lineStart;
            return caretPositionInLine + XmlLineInfoLinePositionOffset;
        }

        public bool HasLineInfo()
        {
            return true;
        }
    }
}