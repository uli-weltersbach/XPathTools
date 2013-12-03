using System.Xml.Linq;
using NUnit.Framework;
using System.Xml;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.Tests.Formatters
{
    [TestFixture]
    public class PathFormatterTests
    {
        private readonly IPathFormatter _formatter = new PathFormatter();

        [Test]
        public void AttributeFormat()
        {
            // Arrange
            XAttribute attribute = new XAttribute("attribute", "value");

            // Act
            string xpath = _formatter.Format(attribute);

            // Assert
            Assert.That(xpath, Is.EqualTo("[@attribute='value']"));
        }

        [Test]
        [ExpectedException(typeof(XmlException))]
        public void MissingAttributeParent()
        {
            // Arrange
            XNamespace @namespace = "test namespace";
            XAttribute attribute = new XAttribute(@namespace + "attribute", "value");

            // Act
            _formatter.Format(attribute);
        }

        [Test]
        public void AttributeNamespaceFormat()
        {
            // Arrange
            XNamespace @namespace = "test namespace";
            XElement element = new XElement(@namespace + "parent");
            XAttribute namespaceAttribute = new XAttribute(XNamespace.Xmlns + "ns", @namespace);
            element.Add(namespaceAttribute);
            XAttribute attribute = new XAttribute(@namespace + "attribute", "value");
            element.Add(attribute);

            // Act
            string xpath = _formatter.Format(attribute);

            // Assert
            Assert.That(xpath, Is.EqualTo("[@ns:attribute='value']"));
        }

        [Test]
        public void ElementFormat()
        {
            // Arrange
            XElement parent = new XElement("parent");
            XElement child = new XElement("child");
            parent.Add(child);

            // Act
            string xpath = _formatter.Format(child);

            // Assert
            Assert.That(xpath, Is.EqualTo("/parent/child"));
        }

        [Test]
        public void ElementNamespaceFormat()
        {
            // Arrange
            XNamespace @namespace = "test namespace";
            XElement parent = new XElement(@namespace + "parent");
            parent.Add(new XAttribute(XNamespace.Xmlns + "ns", @namespace));
            XElement child = new XElement("child");
            parent.Add(child);

            // Act
            string xpath = _formatter.Format(child);

            // Assert
            Assert.That(xpath, Is.EqualTo("/ns:parent/child"));
        }
    }
}