using System.Xml.Linq;
using NUnit.Framework;
using System.Xml;

namespace ReasonCodeExample.XPathInformation.Tests
{
    [TestFixture]
    public class XPathFormatterTests
    {
        [Test]
        public void AttributeFormat()
        {
            // Arrange
            XPathFormatter formatter = new XPathFormatter();
            XAttribute attribute = new XAttribute("attribute", "value");

            // Act
            string xpath = formatter.Format(attribute);

            // Assert
            Assert.That(xpath, Is.EqualTo("[@attribute]"));
        }

        [Test]
        [ExpectedException(typeof(XmlException))]
        public void MissingAttributeParent()
        {
            // Arrange
            XPathFormatter formatter = new XPathFormatter();
            XNamespace @namespace = "test namespace";
            XAttribute attribute = new XAttribute(@namespace + "attribute", "value");

            // Act
            formatter.Format(attribute);
        }

        [Test]
        public void AttributeNamespaceFormat()
        {
            // Arrange
            XPathFormatter formatter = new XPathFormatter();
            XNamespace @namespace = "test namespace";
            XElement element = new XElement(@namespace + "parent");
            XAttribute namespaceAttribute = new XAttribute(XNamespace.Xmlns + "ns", @namespace);
            element.Add(namespaceAttribute);
            XAttribute attribute = new XAttribute(@namespace + "attribute", "value");
            element.Add(attribute);

            // Act
            string xpath = formatter.Format(attribute);

            // Assert
            Assert.That(xpath, Is.EqualTo("[@ns:attribute]"));
        }

        [Test]
        public void ElementFormat()
        {
            // Arrange
            XPathFormatter formatter = new XPathFormatter();
            XElement parent = new XElement("parent");
            XElement child = new XElement("child");
            parent.Add(child);

            // Act
            string xpath = formatter.Format(child);

            // Assert
            Assert.That(xpath, Is.EqualTo("/parent/child"));
        }

        [Test]
        public void ElementNamespaceFormat()
        {
            // Arrange
            XPathFormatter formatter = new XPathFormatter();
            XNamespace @namespace = "test namespace";
            XElement parent = new XElement(@namespace + "parent");
            parent.Add(new XAttribute(XNamespace.Xmlns + "ns", @namespace));
            XElement child = new XElement("child");
            parent.Add(child);

            // Act
            string xpath = formatter.Format(child);

            // Assert
            Assert.That(xpath, Is.EqualTo("/ns:parent/child"));
        }
    }
}