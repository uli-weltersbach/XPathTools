using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.Tests.Formatters
{
    [TestFixture]
    public class DistinctPathFormatterTests
    {
        private readonly IPathFormatter _formatter = new DistinctPathFormatter();

        [TestCase("<a><b /><b /><b /><b><c id='value' /><c id='other-value' /></b></a>", 5, "/a/b/c[@id='value']")]
        [TestCase("<a><b /><b /><b /><b><c id='same' /><c id='same' /></b></a>", 5, "")]
        [TestCase("<a><b /><b /><b /><b><c id='same' name='unique'/><c id='same' /></b></a>", 5, "/a/b/c[@name='unique']")]
        [TestCase("<a><b /><b /><b /><b><c id='same' type='unique'/><c id='same' /></b></a>", 5, "/a/b/c[@type='unique']")]
        [TestCase("<a><b id='same'><c id='same' /></b><b id='same'><c id='same' /></b><b id='unique'><c id='same' /></b></a>", 6, "/a/b[@id='unique']/c")]
        public void IdAttributeIsUsedInDistinctPath(string xml, int testElementIndex, string expectedXPath)
        {
            // Arrange
            XElement element = XElement.Parse(xml);
            XObject testElement = element.DescendantNodesAndSelf().ElementAt(testElementIndex);

            // Act
            string actualXPath = _formatter.Format(testElement);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }
    }
}