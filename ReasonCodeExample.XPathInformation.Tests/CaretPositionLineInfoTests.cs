using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using NSubstitute;
using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests
{
    [TestFixture]
    public class CaretPositionLineInfoTests
    {
        [TestCase(0)]
        [TestCase(10)]
        [TestCase(100)]
        public void LineNumberIsAdjusted(int lineNumber)
        {
            // Arrange
            const int expectedAdjustment = 1;
            ITextSnapshot textSnapshot = Substitute.For<ITextSnapshot>();
            textSnapshot.GetLineNumberFromPosition(Arg.Any<int>()).Returns(lineNumber);

            ITextView textView = Substitute.For<ITextView>();
            textView.TextSnapshot.Returns(textSnapshot);

            // Act
            CaretPositionLineInfo caretPositionLineInfo = new CaretPositionLineInfo(textView, 0);

            // Assert
            Assert.That(caretPositionLineInfo.LineNumber, Is.EqualTo(lineNumber + expectedAdjustment));
        }

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(100)]
        public void LinePositionIsAdjusted(int caretPosition)
        {
            // Arrange
            const int expectedAdjustment = 1;
            ITextSnapshot textSnapshot = Substitute.For<ITextSnapshot>();
            textSnapshot.GetLineNumberFromPosition(Arg.Is(caretPosition)).Returns(0);
            textSnapshot.Length.Returns(caretPosition);

            ITextView textView = Substitute.For<ITextView>();
            textView.TextSnapshot.Returns(textSnapshot);

            // Act
            CaretPositionLineInfo caretPositionLineInfo = new CaretPositionLineInfo(textView, caretPosition);

            // Assert
            Assert.That(caretPositionLineInfo.LinePosition, Is.EqualTo(caretPosition + expectedAdjustment));
        }
    }
}