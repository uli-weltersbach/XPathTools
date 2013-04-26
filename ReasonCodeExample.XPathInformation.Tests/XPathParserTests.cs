using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests
{
    public class XPathParserTests
    {
        [TestCase("<node />", "/node")]
        [TestCase("<ns:node />", "/ns:node")]
        [TestCase("<parent><child />", "/parent/child")]
        [TestCase("<parent><sibling></sibling><node />", "/parent/node")]
        [TestCase("<parent><child><grandChild><grandGrandChild /></grandChild><node />", "/parent/child/node")]
        [TestCase("<node><node><node><node /></node><node />", "/node/node/node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"override\"><node />", "/parent/child/o:grandChild/o:node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"override\"><node><node />", "/parent/child/o:grandChild/o:node/o:node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"override\"><node><node xmlns:x=\"nested\"><node />", "/parent/child/o:grandChild/o:node/x:node/x:node")]
        public void SingleNode(string xml, string expectedXPath)
        {
            // Arrange
            XPathParser parser = new XPathParser();

            // Act
            string actualXPath = parser.Parse(xml);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }
    }
}