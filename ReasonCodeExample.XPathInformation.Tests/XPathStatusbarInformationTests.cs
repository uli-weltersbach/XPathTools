using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using NSubstitute;
using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests
{
    [TestFixture]
    public class XPathStatusbarInformationTests
    {
        [Test]
        public void Document()
        {
            // Arrange
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
                         @" <GitSccOptions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" + Environment.NewLine +
                         @" <GitBashPath>C:\Program Files (x86)\Git\bin\sh.exe</GitBashPath>" + Environment.NewLine +
                         "  <GitExtensionPath />" + Environment.NewLine +
                         "  <node />  </GitSccOptions>";
            int lineNumber = 5;
            var args = CreateEventArgs(xml, 0, lineNumber);
            ITextView textView = Substitute.For<ITextView>();
            IVsStatusbar statusbar = Substitute.For<IVsStatusbar>();
            new XPathStatusbarInformation(textView, statusbar);

            // Act
            textView.Caret.PositionChanged += Raise.EventWith(args);

            // Assert
            statusbar.Received().SetText(Arg.Is("/GitSccOptions/node"));
        }

        private CaretPositionChangedEventArgs CreateEventArgs(string xml, int caretPosition, int lineNumber)
        {
            ITextSnapshotLine line = Substitute.For<ITextSnapshotLine>();

            ITextSnapshot textSnapshot = Substitute.For<ITextSnapshot>();
            textSnapshot.GetText().Returns(xml);
            textSnapshot.GetLineNumberFromPosition(Arg.Is(caretPosition)).Returns(lineNumber);
            textSnapshot.GetLineFromPosition(Arg.Is(caretPosition)).Returns(line);
            textSnapshot.Length.Returns(xml.Length);

            int lineStart = GetLineStart(xml, lineNumber);
            line.Start.Returns(new SnapshotPoint(textSnapshot, lineStart));

            ITextView textView = Substitute.For<ITextView>();
            textView.TextSnapshot.Returns(textSnapshot);

            CaretPosition oldPosition = new CaretPosition();
            VirtualSnapshotPoint position = new VirtualSnapshotPoint(textSnapshot, caretPosition);
            CaretPosition newPosition = new CaretPosition(position, Substitute.For<IMappingPoint>(), PositionAffinity.Successor);
            return new CaretPositionChangedEventArgs(textView, oldPosition, newPosition);
        }

        private int GetLineStart(string s, int lineNumber)
        {
            StringReader reader = new StringReader(s);
            int lineStart = 0;
            for (int i = 0; i < lineNumber; i++)
            {
                string line = reader.ReadLine();
                lineStart += line.Length;
            }
            return lineStart;
        }

        [Test]
        public void DocumentWithComments()
        {
            // Arrange
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
                         @"<!-- <node> -->" + Environment.NewLine +
                         @"<GitSccOptions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" + Environment.NewLine +
                         @"<GitBashPath>C:\Program Files (x86)\Git\bin\sh.exe</GitBashPath>" + Environment.NewLine +
                         @"<!-- <node>" + Environment.NewLine +
                         "<GitExtensionPath />  -->" + Environment.NewLine +
                         "<node /></GitSccOptions>";

            // Act
            string actualXPath = Parse(xml, 7, 2);

            // Assert
            Assert.That(actualXPath, Is.EqualTo("/GitSccOptions/node"));
        }

        private string Parse(string xml, int lineNumber, int linePosition)
        {
            XElement rootElement = XElement.Parse(xml, LoadOptions.SetLineInfo);
            XmlNodeRepository repository = new XmlNodeRepository();
            XElement selectedElement = repository.GetElement(rootElement, lineNumber, linePosition);
            XAttribute selectedAttribute = repository.GetAttribute(selectedElement, linePosition);
            XPathFormatter formatter = new XPathFormatter();
            return formatter.Format(selectedElement) + formatter.Format(selectedAttribute);
        }
    }
}