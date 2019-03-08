using System.Xml.Linq;
using NUnit.Framework;
using ReasonCodeExample.XPathTools.Configuration;
using ReasonCodeExample.XPathTools.Writers;

namespace ReasonCodeExample.XPathTools.Tests.Writers
{
    [TestFixture]
    public class AttributeFilterTests
    {
        [TestCase("a", null, "e", null, true)]
        [TestCase("a", null, "e", "X", false)]
        [TestCase("a", "X", "e", null, false)]
        [TestCase("a", "X", "e", "X", false)]
        [TestCase("a", "a-xmlns", "e", "e-xmlns", true)]
        [TestCase("a", "a-xmlns", "e", null, true)]
        [TestCase("a", null, "e", "e-xmlns", true)]
        public void NameAndNamespaceFiltering(string attributeName, string attributeNamespace, string elementName, string elementNamespace, bool expectedResult)
        {
            // Arrange
            var attribute = new XAttribute(XName.Get("a", "a-xmlns"), string.Empty);
            var element = new XElement(XName.Get("e", "e-xmlns"));
            element.Add(attribute);
            var settings = new[]
                           {
                               new XPathSetting
                               {
                                   AttributeName = attributeName,
                                   AttributeNamespace = attributeNamespace,
                                   ElementName = elementName,
                                   ElementNamespace = elementNamespace
                               }
                           };
            var filter = new AttributeFilter(settings);

            // Act
            var isIncluded = filter.IsIncluded(attribute);

            // Assert
            Assert.That(isIncluded, Is.EqualTo(expectedResult));
        }
    }
}