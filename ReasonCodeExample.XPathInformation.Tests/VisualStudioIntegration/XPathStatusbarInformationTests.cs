using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using NSubstitute;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    /// <summary>
    /// Integration tests.
    /// </summary>
    [TestFixture]
    public class XPathStatusbarInformationTests
    {
        private readonly string _testXml = @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
                                           @"<!-- <node> -->" + Environment.NewLine +
                                           @"<GitSccOptions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" + Environment.NewLine +
                                           @"<GitBashPath>C:\Program Files (x86)\Git\bin\sh.exe</GitBashPath>" + Environment.NewLine +
                                           @"<!-- <node>" + Environment.NewLine +
                                           "<GitExtensionPath />  -->" + Environment.NewLine +
                                           "<node /></GitSccOptions>";

        [TestCase(0, 1, "")]
        [TestCase(1, 1, "")]
        [TestCase(2, 0, "")]
        [TestCase(2, 1, "/GitSccOptions")]
        [TestCase(2, 16, "/GitSccOptions[@xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance']")]
        [TestCase(3, 0, "")]
        [TestCase(3, 1, "/GitSccOptions/GitBashPath")]
        [TestCase(4, 1, "")]
        [TestCase(5, 1, "")]
        [TestCase(6, 0, "")]
        [TestCase(6, 1, "/GitSccOptions/node")]
        public void StatusbarTextIsUpdated(int lineNumber, int linePosition, string expectedStatusbarText)
        {
            // Arrange
            CaretPositionChangedEventArgs args = CreateEventArgs(linePosition, lineNumber);
            ITextCaret caret = Substitute.For<ITextCaret>();
            ITextView textView = Substitute.For<ITextView>();
            textView.Caret.Returns(caret);
            IVsStatusbar statusbar = Substitute.For<IVsStatusbar>();
            new XPathStatusbarInformation(textView, statusbar);

            // Act
            caret.PositionChanged += Raise.EventWith(args);

            // Assert
            statusbar.Received().SetText(Arg.Is(expectedStatusbarText));
        }

        private CaretPositionChangedEventArgs CreateEventArgs(int linePosition, int lineNumber)
        {
            ITextSnapshotLine line = Substitute.For<ITextSnapshotLine>();

            ITextSnapshot textSnapshot = Substitute.For<ITextSnapshot>();
            textSnapshot.GetText().Returns(_testXml);
            textSnapshot.Length.Returns(_testXml.Length);
            textSnapshot.GetLineNumberFromPosition(Arg.Is(linePosition)).Returns(lineNumber);
            textSnapshot.GetLineFromPosition(Arg.Is(linePosition)).Returns(line);

            SnapshotPoint lineStartSnapshotPoint = new SnapshotPoint(textSnapshot, 0);
            line.Start.Returns(lineStartSnapshotPoint);

            SnapshotPoint snapshotPoint = new SnapshotPoint(textSnapshot, linePosition);
            VirtualSnapshotPoint virtualSnapshotPoint = new VirtualSnapshotPoint(snapshotPoint);
            CaretPosition caretPosition = new CaretPosition(virtualSnapshotPoint, Substitute.For<IMappingPoint>(), PositionAffinity.Successor);

            ITextCaret caret = Substitute.For<ITextCaret>();
            caret.Position.Returns(caretPosition);

            ITextView textView = Substitute.For<ITextView>();
            textView.TextSnapshot.Returns(textSnapshot);
            textView.Caret.Returns(caret);
            
            return new CaretPositionChangedEventArgs(textView, new CaretPosition(), caretPosition);
        }
    }
}