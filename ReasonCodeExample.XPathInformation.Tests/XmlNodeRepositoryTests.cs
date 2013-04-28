using System.Xml.Linq;
using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests
{
    [TestFixture]
    public class XmlNodeRepositoryTests
    {
        [TestCase("<a />", 1, "")]
        [TestCase("<a />", 2, "a")]
        [TestCase("<node />", 2, "node")]
        [TestCase("<ns:node xmlns:ns=\"ns-namespace\"/>", 2, "{ns-namespace}node")]
        [TestCase("<parent><child /></parent>", 2, "parent")]
        [TestCase("<parent><child /></parent>", 10, "child")]
        [TestCase("<parent><child /></parent>", 17, "child")]
        [TestCase("<parent><child1 /><child2 /><child3></child3></parent>", 10, "child1")]
        [TestCase("<parent><sibling></sibling><node /></parent>", 29, "node")]
        [TestCase("<parent><child><grandChild><grandGrandChild /></grandChild><node /></child></parent>", 61, "node")]
        [TestCase("<node1><node2><node3><node4 /></node3><node5 /></node2></node1>", 16, "node3")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><o:node /></grandChild></child></parent>", 67, "{o-namespace}node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><o:node><node /></o:node></grandChild></child></parent>", 75, "{default}node")]
        [TestCase("<parent xmlns=\"default\"><child><grandChild xmlns:o=\"o-namespace\"><node><node xmlns:x=\"x-namespace\"><x:node /></node></node></grandChild></child></parent>", 101, "{x-namespace}node")]
        public void ElementIsFound(string xml, int linePosition, string expectedElementName)
        {
            // Arrange
            XmlNodeRepository repository = new XmlNodeRepository();
            XElement rootElement = XElement.Parse(xml, LoadOptions.SetLineInfo);

            // Act
            XElement element = repository.GetElement(rootElement, 1, linePosition);
            string elementName = element == null ? string.Empty : element.Name.ToString();

            // Assert
            Assert.That(elementName, Is.EqualTo(expectedElementName));
        }

        [TestCase("<node name=\"value\" otherName=\"value\"/>", 7, "name")]
        [TestCase("<node name=\"value\" otherName=\"value\"/>", 18, "name")]
        [TestCase("<node name=\"value\" otherName=\"value\"/>", 20, "otherName")]
        [TestCase("<ns:node xmlns:ns=\"ns-namespace\" ns:attribute=\"value\"/>", 10, "{http://www.w3.org/2000/xmlns/}ns")]
        [TestCase("<ns:node xmlns:ns=\"ns-namespace\" ns:attribute=\"value\"/>", 34, "{ns-namespace}attribute")]
        public void AttributeIsFound(string xml, int linePosition, string expectedAttributeName)
        {
            // Arrange
            XmlNodeRepository repository = new XmlNodeRepository();
            XElement rootElement = XElement.Parse(xml, LoadOptions.SetLineInfo);

            // Act
            XAttribute attribute = repository.GetAttribute(rootElement, linePosition);
            string attributeName = attribute == null ? string.Empty : attribute.Name.ToString();

            // Assert
            Assert.That(attributeName, Is.EqualTo(expectedAttributeName));
        }
    }
}