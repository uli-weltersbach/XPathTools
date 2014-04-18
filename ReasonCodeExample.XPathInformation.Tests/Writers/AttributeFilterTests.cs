using System.Xml.Linq;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Tests.Writers
{
    [TestFixture]
    public class AttributeFilterTests
    {
        [TestCase("id", "")]
        [TestCase("name", "")]
        [TestCase("type", "")]
        [TestCase("id", "a")]
        [TestCase("name", "b")]
        [TestCase("type", "c")]
        public void AttributeNameFiltering(string attributeName, string attributeNamespace)
        {
            // Arrange
            XAttribute attribute = new XAttribute(XName.Get(attributeName, attributeNamespace), string.Empty);
            var settings = new[] { new XPathSetting { AttributeName = attributeName, AttributeNamespace = attributeNamespace} };
            var filter = new AttributeFilter(settings);

            // Act
            var isIncluded = filter.IsIncluded(attribute);

            // Assert
            Assert.That(isIncluded, Is.True);
        }
    }
}