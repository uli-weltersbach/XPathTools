using System.Xml.Linq;
using NUnit.Framework;
using System.Xml;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.Tests.Formatters
{
    [TestFixture]
    public class AbsoluteXPathFormatterTests
    {
        private readonly IXPathFormatter _formatter = new AbsoluteXPathFormatter();

        [Test]
        public void ElementIndexIsIncludedWhenElementHasNoSiblingsWithSameName()
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
            Assert.That(xpath, Is.EqualTo("/parent[1]/childY[1]"));
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
            Assert.That(xpath, Is.EqualTo("/parent[1]/child[2]"));
        }

        [Test]
        public void ElementIndicesAreAddedForEntirePath()
        {
            // Arrange
            XElement parent = new XElement("parent");

            XElement firstChild = new XElement("child");
            parent.Add(firstChild);

            XElement secondChild = new XElement("child");
            parent.Add(secondChild);

            XElement secondChildFirstGrandChild = new XElement("grandChild");
            secondChild.Add(secondChildFirstGrandChild);
            
            XElement secondChildSecondGrandChild = new XElement("grandChild");
            secondChild.Add(secondChildSecondGrandChild);

            XElement thirdChild = new XElement("child");
            parent.Add(thirdChild);

            // Act
            string xpath = _formatter.Format(secondChildFirstGrandChild);

            // Assert
            Assert.That(xpath, Is.EqualTo("/parent[1]/child[2]/grandChild[1]"));
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
            Assert.That(xpath, Is.EqualTo("/ns:parent[1]/child[2]"));
        }
    }
}