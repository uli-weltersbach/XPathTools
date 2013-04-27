using System;
using System.Xml.Linq;
using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests
{
    [TestFixture]
    public class XPathInformationTests
    {
        [TestCase("<a />", 1, 1, "")]
        [TestCase("<a />", 1, 2, "/a")]
        [TestCase("<node />", 1, 2, "/node")]
        [TestCase("<ns:node xmlns:ns=\"ns-namespace\"/>", 1, 2, "/ns:node")]
        [TestCase("<parent><child /></parent>", 1, 10, "/parent/child")]
        [TestCase("<parent><child /></parent>", 1, 13, "/parent/child")]
        [TestCase("<parent><child /></parent>", 1, 17, "/parent/child")]
        [TestCase("<parent><child /></parent>", 1, 2, "/parent")]
        [TestCase("<parent><child /><child /><child></child></parent>", 1, 10, "/parent/child")]
        [TestCase("<parent><sibling></sibling><node /></parent>", 1, 29, "/parent/node")]
        [TestCase("<parent><child><grandChild><grandGrandChild /></grandChild><node /></child></parent>", 1, 61, "/parent/child/node")]
        [TestCase("<node><node><node><node /></node><node /></node></node>", 1, 14, "/node/node/node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><o:node /></grandChild></child></parent>", 1, 67, "/parent/child/grandChild/o:node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><o:node><node /></o:node></grandChild></child></parent>", 1, 75, "/parent/child/grandChild/o:node/node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><node><node xmlns:x=\"x-namespace\"><x:node /></node></node></grandChild></child></parent>", 1, 101, "/parent/child/grandChild/node/node/x:node")]
        public void SimpleNestedNodes(string xml, int lineNumber, int linePosition, string expectedXPath)
        {
            // Arrange

            // Act
            string actualXPath = Parse(xml, lineNumber, linePosition);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }

        [Test]
        public void Document()
        {
            // Arrange
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
                         @" <GitSccOptions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" + Environment.NewLine +
                         @" <GitBashPath>C:\Program Files (x86)\Git\bin\sh.exe</GitBashPath>" + Environment.NewLine +
                         "  <GitExtensionPath />" + Environment.NewLine +
                         "  <node />  </GitSccOptions>";
            string expectedXPath = "/GitSccOptions/node";

            // Act
            string actualXPath = Parse(xml, 5, 8);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
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
            string expectedXPath = "/GitSccOptions/node";

            // Act
            string actualXPath = Parse(xml, 7, 2);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }

        private string Parse(string xml, int lineNumber, int linePosition)
        {
            XElement rootElement = XElement.Parse(xml, LoadOptions.SetLineInfo);
            XElement selectedElement = new XmlNodeRepository().Get(rootElement, lineNumber, linePosition);
            return new XPathFormatter().Format(selectedElement);
        }
    }
}