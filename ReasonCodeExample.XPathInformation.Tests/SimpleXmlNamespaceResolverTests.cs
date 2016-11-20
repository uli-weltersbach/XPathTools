using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests
{
    [TestFixture]
    public class SimpleXmlNamespaceResolverTests
    {
        [TestCase("<a:element xmlns:a=\"1\"/>", new[] {"1"})]
        [TestCase("<a:element xmlns:a=\"1\"><b:element xmlns:b=\"2\"/></a:element>", new[] {"1", "2"})]
        public void NamespaceNamesAreParsedCorrectly(string xml, string[] expectedNamespaces)
        {
            // Arrange
            var document = XDocument.Parse(xml);

            // Act
            var manager = new SimpleXmlNamespaceResolver(document);
            var namespaces = manager.GetNamespacesInScope(XmlNamespaceScope.All).Keys;

            // Assert
            Assert.That(namespaces, Is.EquivalentTo(expectedNamespaces));
        }

        [TestCase("<a:element xmlns:a=\"1\"/>", new[] {"a"})]
        [TestCase("<a:element xmlns:a=\"1\"><b:element xmlns:b=\"2\"/></a:element>", new[] {"a", "b"})]
        [TestCase("<a xmlns:xlink=\"http://www.w3.org/1999/xlink\" />", new [] {"xlink"})]
        public void NamespacePrefixesAreParsedCorrectly(string xml, string[] expectedNamespaces)
        {
            // Arrange
            var document = XDocument.Parse(xml);

            // Act
            var manager = new SimpleXmlNamespaceResolver(document);
            var namespaces = manager.GetNamespacesInScope(XmlNamespaceScope.All).Values;

            // Assert
            Assert.That(namespaces, Is.EquivalentTo(expectedNamespaces));
        }
    }
}