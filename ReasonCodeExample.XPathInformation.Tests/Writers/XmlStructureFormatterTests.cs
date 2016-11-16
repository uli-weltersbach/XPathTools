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
            var document = XDocument.Parse(xml);
            var element = document.Root.DescendantsAndSelf().ElementAt(testElementIndex);

            // Act
            var actualXml = new XmlStructureWriter().Write(element);
            var nonFormattedXml = XElement.Parse(actualXml).ToString(SaveOptions.DisableFormatting);

            // Assert
            Assert.That(nonFormattedXml, Is.EqualTo(expectedXml));
        }

        [Test]
        public void FormatDoesntModifyOriginalElement()
        {
            // Arrange
            var b2 = new XElement("b2");
            var original = new XElement("a", new XElement("b1"), b2);

            // Act
            var output = new XmlStructureWriter().Write(b2);

            // Assert
            Assert.That(original.ToString(), Does.Contain("b1"));
        }
    }
}