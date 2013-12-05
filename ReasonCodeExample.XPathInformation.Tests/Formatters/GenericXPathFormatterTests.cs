using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using System.Xml;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.Tests.Formatters
{
    [TestFixture]
    public class GenericXPathFormatterTests
    {
        private readonly IXPathFormatter _formatter = new GenericXPathFormatter();

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
        public void AttributeNamespacePrefixFormat()
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
            XElement child = new XElement("child");
            parent.Add(child);

            // Act
            string xpath = _formatter.Format(child);

            // Assert
            Assert.That(xpath, Is.EqualTo("/*[local-name()='parent' and namespace-uri()='test namespace']/child"));
        }

        [Test]
        public void ElementNamespacePrefixFormat()
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

        [TestCase("descendant-or-self::*[namespace-uri()='urn:schemas-microsoft-com:asm.v1']", 4)]
        [TestCase("/runtime/assemblyBinding[namespace-uri()='urn:schemas-microsoft-com:asm.v1']", 0)]
        [TestCase("/configuration/runtime/*[namespace-uri()='urn:schemas-microsoft-com:asm.v1']", 2)]
        [TestCase("/configuration/runtime/*[local-name()='assemblyBinding' and namespace-uri()='urn:schemas-microsoft-com:asm.v1']", 1)]
        public void XPathContainingNamespaceIsValid(string xpath, int expectedElementCount)
        {
            // Arrange
            XDocument document = XDocument.Parse(@"<configuration>
                                                        <runtime>
                                                            <assemblyBinding xmlns='urn:schemas-microsoft-com:asm.v1'>
                                                               <child />
                                                            </assemblyBinding>
                                                            <test xmlns='urn:schemas-microsoft-com:asm.v1'>
                                                                <child />
                                                            </test>
                                                        </runtime>
                                                    </configuration>");
            XmlReader reader = document.CreateReader();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(reader.NameTable);

            // Act
            int elementCount = document.XPathSelectElements(xpath, namespaceManager).Count();

            // Assert
            Assert.That(elementCount, Is.EqualTo(expectedElementCount));
        }
    }
}