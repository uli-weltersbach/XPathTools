using System.Xml.Linq;
using NSubstitute;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Formatters;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration;

namespace ReasonCodeExample.XPathInformation.Tests.Formatters
{
    [TestFixture]
    public class XmlAttributeFilterTests
    {
        [Test]
        public void AttributesThatAreNotIncludedAreRemoved()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            configuration.IncludedAttributes.Returns(new[] { "included" });
            var filter = new XmlAttributeFilter(configuration);
            var document = new XDocument(new XElement("element",
                new XAttribute("included", "should not be removed"),
                new XAttribute("excluded", "should be removed")));

            // Act
            filter.ApplyTo(document);
            string filteredXml = document.ToString(SaveOptions.DisableFormatting);

            // Assert
            Assert.That(filteredXml, Is.EqualTo("<element included=\"should not be removed\" />"));
        }

        [Test]
        public void AttributesThatAreExcludedAreRemoved()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            configuration.ExcludedAttributes.Returns(new[] { "excluded" });
            var filter = new XmlAttributeFilter(configuration);
            var document = new XDocument(new XElement("element",
                new XAttribute("included", "should not be removed"),
                new XAttribute("excluded", "should be removed")));

            // Act
            filter.ApplyTo(document);
            string filteredXml = document.ToString(SaveOptions.DisableFormatting);

            // Assert
            Assert.That(filteredXml, Is.EqualTo("<element included=\"should not be removed\" />"));
        }
    }
}