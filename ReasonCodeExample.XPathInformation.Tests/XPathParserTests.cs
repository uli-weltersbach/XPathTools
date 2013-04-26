using System;
using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests
{
    public class XPathParserTests
    {
        [TestCase("<a />", 1, 1, "/a")]
        [TestCase("<node />", 1, 1, "/node")]
        [TestCase("<ns:node xmlns:ns=\"ns-namespace\"/>", 1, 1, "/ns:node")]
        [TestCase("<parent><child /></parent>", 1, 9, "/parent/child")]
        [TestCase("<parent><child /></parent>", 1, 12, "/parent/child")]
        [TestCase("<parent><child /></parent>", 1, 16, "/parent/child")]
        [TestCase("<parent><child /></parent>", 1, 1, "/parent")]
        [TestCase("<parent><child /><child /><child></child></parent>", 1, 9, "/parent/child")]
        [TestCase("<parent><sibling></sibling><node /></parent>", 1, 28, "/parent/node")]
        [TestCase("<parent><child><grandChild><grandGrandChild /></grandChild><node /></child></parent>", 1, 60, "/parent/child/node")]
        [TestCase("<node><node><node><node /></node><node /></node></node>", 1, 13, "/node/node/node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><o:node /></grandChild></child></parent>", 1, 66, "/parent/child/grandChild/o:node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><o:node><node /></o:node></grandChild></child></parent>", 1, 74, "/parent/child/grandChild/o:node/node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><node><node xmlns:x=\"x-namespace\"><x:node /></node></node></grandChild></child></parent>", 1, 100, "/parent/child/grandChild/node/node/x:node")]
        public void SimpleNestedNodes(string xml, int lineNumber, int linePosition, string expectedXPath)
        {
            // Arrange
            XPathParser parser = new XPathParser();

            // Act
            string actualXPath = parser.GetPath(xml, lineNumber, linePosition);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }

        [Test]
        public void Document()
        {
            // Arrange
            XPathParser parser = new XPathParser();
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
                         @" <GitSccOptions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" + Environment.NewLine +
                         @" <GitBashPath>C:\Program Files (x86)\Git\bin\sh.exe</GitBashPath>" + Environment.NewLine +
                         "  <GitExtensionPath />" + Environment.NewLine +
                         "  <node />  </GitSccOptions>";
            string expectedXPath = "/GitSccOptions/node";

            // Act
            string actualXPath = parser.GetPath(xml, 5, 7);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }

        [Test]
        public void DocumentWithComments()
        {
            // Arrange
            XPathParser parser = new XPathParser();
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
                         @"<!-- <node> -->" + Environment.NewLine +
                         @"<GitSccOptions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" + Environment.NewLine +
                         @"<GitBashPath>C:\Program Files (x86)\Git\bin\sh.exe</GitBashPath>" + Environment.NewLine +
                         @"<!-- <node>" + Environment.NewLine +
                         "<GitExtensionPath />  -->" + Environment.NewLine +
                         "<node /></GitSccOptions>";
            string expectedXPath = "/GitSccOptions/node";

            // Act
            string actualXPath = parser.GetPath(xml, 7, 1);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }
    }
}