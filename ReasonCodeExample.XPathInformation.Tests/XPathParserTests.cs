using System;
using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests
{
    public class XPathParserTests
    {
        [TestCase("<node />", "/node")]
        [TestCase("<ns:node />", "/ns:node")]
        [TestCase("<parent><child />", "/parent/child")]
        [TestCase("<parent><child /><child />", "/parent/child")]
        [TestCase("<parent><child /><child /><child ", "/parent/child")]
        [TestCase("<parent><sibling></sibling><node />", "/parent/node")]
        [TestCase("<parent><child><grandChild><grandGrandChild /></grandChild><node />", "/parent/child/node")]
        [TestCase("<node><node><node><node /></node><node />", "/node/node/node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><o:node />", "/parent/child/grandChild/o:node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><o:node><node />", "/parent/child/grandChild/o:node/node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><node><node xmlns:x=\"x-namespace\"><x:node />", "/parent/child/grandChild/node/node/x:node")]
        public void SimpleNestedNodes(string xml, string expectedXPath)
        {
            // Arrange
            XPathParser parser = new XPathParser();

            // Act
            string actualXPath = parser.Parse(xml);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }

        [Test]
        public void PartialDocument()
        {
            // Arrange
            XPathParser parser = new XPathParser();
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
                         @"<GitSccOptions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" + Environment.NewLine +
                         @"<GitBashPath>C:\Program Files (x86)\Git\bin\sh.exe</GitBashPath>" + Environment.NewLine +
                         "<GitExtensionPath />" + Environment.NewLine +
                         "<node";
            string expectedXPath = "/GitSccOptions/node";

            // Act
            string actualXPath = parser.Parse(xml);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }
    }
}