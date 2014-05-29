using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Tests.Writers
{
    [TestFixture]
    public class XmlStructureFormatterTests
    {
        [TestCase("<configuration><runtime><child /><assemblyBinding xmlns='urn:schemas-microsoft-com:asm.v1'><child /><child /></assemblyBinding><test xmlns='urn:schemas-microsoft-com:asm.v1'><child /><child /></test><child /></runtime><child /></configuration>", 2, "<configuration><runtime><child /></runtime></configuration>")]
        [TestCase("<configuration xmlns:ns1='urn:schemas-microsoft-com:asm.v1'><runtime><ns1:assemblyBinding /></runtime><child /></configuration>", 2, "<configuration xmlns:ns1=\"urn:schemas-microsoft-com:asm.v1\"><runtime><ns1:assemblyBinding /></runtime></configuration>")]
        [TestCase("<a><!--Comment--><b><!--Comment--><c /></b></a>", 2, "<a><b><c /></b></a>")]
        public void XmlStructureIsCreatedCorrectly(string xml, int testElementIndex, string expectedXml)
        {
            // Arrange
            XDocument document = XDocument.Parse(xml);
            XElement element = document.Root.DescendantsAndSelf().ElementAt(testElementIndex);

            // Act
            string actualXml = new XmlStructureWriter().Write(element).ToString(SaveOptions.DisableFormatting);

            // Assert
            Assert.That(actualXml, Is.EqualTo(expectedXml));
        }

        [Test]
        public void FormatReturnsClonedElement()
        {
            // Arrange
            XElement original = new XElement("original");

            // Act
            XElement clone = new XmlStructureWriter().Write(original);

            // Assert
            Assert.That(ReferenceEquals(original, clone), Is.False);
        }
    }
}