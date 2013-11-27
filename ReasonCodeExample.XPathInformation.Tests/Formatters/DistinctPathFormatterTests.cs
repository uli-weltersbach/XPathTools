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

        [TestCase("<a><b /><b /><b /><b><c id=\"1\" /><c id=\"2\" /></b></a>", 5, "/a/b/c[@id='1']")]
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