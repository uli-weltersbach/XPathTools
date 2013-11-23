using System.Xml.Linq;
using NUnit.Framework;
using System.Xml;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.Tests.Formatters
{
    [TestFixture]
    public class AsbolutePathFormatterTests
    {
        private readonly PathFormatter _formatter = new AbsolutePathFormatter();

        [Test]
        public void ElementIndexIsExcludedWhenElementHasNoSiblingsWithSameName()
        {
            // Arrange
            XElement parent = new XElement("parent");

            XElement firstChild = new XElement("childX");
            parent.Add(firstChild);

            XElement secondChild = new XElement("childY");
            parent.Add(secondChild);

            XElement thirdChild = new XElement("childZ");
            parent.Add(thirdChild);

            // Act
            string xpath = _formatter.Format(secondChild);

            // Assert
            Assert.That(xpath, Is.EqualTo("/parent/childY"));
        }

        [Test]
        public void ElementIndexIsOneBased()
        {
            // Arrange
            XElement parent = new XElement("parent");

            XElement firstChild = new XElement("child");
            parent.Add(firstChild);

            XElement secondChild = new XElement("child");
            parent.Add(secondChild);
            
            XElement thirdChild = new XElement("child");
            parent.Add(thirdChild);
            
            // Act
            string xpath = _formatter.Format(secondChild);

            // Assert
            Assert.That(xpath, Is.EqualTo("/parent/child[2]"));
        }

        [Test]
        public void ElementNamespaceFormat()
        {
            // Arrange
            XNamespace @namespace = "test namespace";
            XElement parent = new XElement(@namespace + "parent");
            parent.Add(new XAttribute(XNamespace.Xmlns + "ns", @namespace));

            XElement firstChild = new XElement("child");
            parent.Add(firstChild);

            XElement secondChild = new XElement("child");
            parent.Add(secondChild);

            XElement thirdChild = new XElement("child");
            parent.Add(thirdChild);

            // Act
            string xpath = _formatter.Format(secondChild);

            // Assert
            Assert.That(xpath, Is.EqualTo("/ns:parent/child[2]"));
        }
    }
}