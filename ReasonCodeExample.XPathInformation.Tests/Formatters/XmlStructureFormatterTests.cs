using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.Tests.Formatters
{
    [TestFixture]
    public class XmlStructureFormatterTests
    {
        [Test]
        public void FormatReturnsClonedElement()
        {
            // Arrange
            XElement original = new XElement("original");

            // Act
            XElement clone = new XmlStructureFormatter().Format(original);

            // Assert
            Assert.That(ReferenceEquals(original, clone), Is.False);
        }

        [TestCase("<configuration><runtime><child /><assemblyBinding xmlns='urn:schemas-microsoft-com:asm.v1'><child /><child /></assemblyBinding><test xmlns='urn:schemas-microsoft-com:asm.v1'><child /><child /></test><child /></runtime><child /></configuration>", 2, "<configuration><runtime><child /></runtime></configuration>")]
        [TestCase("<configuration xmlns:ns1='urn:schemas-microsoft-com:asm.v1'><runtime><ns1:assemblyBinding /></runtime><child /></configuration>", 2, "<configuration xmlns:ns1=\"urn:schemas-microsoft-com:asm.v1\"><runtime><ns1:assemblyBinding /></runtime></configuration>")]
        public void XmlStructureIsCreatedCorrectly(string xml, int testElementIndex, string expectedXml)
        {
            // Arrange
            XDocument document = XDocument.Parse(xml);
            XElement element = document.Root.DescendantsAndSelf().ElementAt(testElementIndex);

            // Act
            string actualXml = new XmlStructureFormatter().Format(element).ToString(SaveOptions.DisableFormatting);

            // Assert
            Assert.That(actualXml, Is.EqualTo(expectedXml));
        }
    }
}
