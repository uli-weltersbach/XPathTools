using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.Tests.Formatters
{
    [TestFixture]
    public class DistinctXPathFormatterTests
    {
        private readonly IXPathFormatter _formatter = new DistinctXPathFormatter();

        [TestCase("<a><b /><b /><b /><b><c id='value' /><c id='other-value' /></b></a>", 5, "/a/b/c[@id='value']")]
        [TestCase("<a><b /><b /><b /><b><c id='same' /><c id='same' /></b></a>", 5, "")]
        [TestCase("<a><b /><b /><b /><b><c id='same' name='unique'/><c id='same' /></b></a>", 5, "/a/b/c[@name='unique']")]
        [TestCase("<a><b /><b /><b /><b><c id='same' type='unique'/><c id='same' /></b></a>", 5, "/a/b/c[@type='unique']")]
        [TestCase("<a><b id='same'><c id='same' /></b><b id='same'><c id='same' /></b><b id='unique'><c id='same' /></b></a>", 6, "/a/b[@id='unique']/c")]
        public void PreferredAttributeIsUsedInDistinctPath(string xml, int testElementIndex, string expectedXPath)
        {
            // Arrange
            XElement element = XElement.Parse(xml);
            XObject testElement = element.DescendantNodesAndSelf().ElementAt(testElementIndex);

            // Act
            string actualXPath = _formatter.Format(testElement);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }

        [Test]
        public void ElementNamespaceFormat()
        {
            // Arrange
            // Arrange
            XDocument document = XDocument.Parse(@"<configuration>
                                                        <runtime>
                                                            <assemblyBinding id='1' xmlns='urn:schemas-microsoft-com:asm.v1'>
                                                               <child />
                                                            </assemblyBinding>
                                                            <assemblyBinding id='2' xmlns='urn:schemas-microsoft-com:asm.v1'>
                                                                <child />
                                                            </assemblyBinding>
                                                        </runtime>
                                                    </configuration>");

            // Act
            string xpath = _formatter.Format(document.Descendants().ElementAt(2));

            // Assert
            Assert.That(xpath, Is.EqualTo("/configuration/runtime/*[local-name()='assemblyBinding' and namespace-uri()='urn:schemas-microsoft-com:asm.v1'][@id='1']"));
        }

        [TestCase("/configuration/runtime/*[local-name()='assemblyBinding' and namespace-uri()='urn:schemas-microsoft-com:asm.v1'][@id='1']", 1)]
        public void XPathContainingNamespaceIsValid(string xpath, int expectedElementCount)
        {
            // Arrange
            XDocument document = XDocument.Parse(@"<configuration>
                                                        <runtime>
                                                            <assemblyBinding id='1' xmlns='urn:schemas-microsoft-com:asm.v1'>
                                                               <child />
                                                            </assemblyBinding>
                                                            <assemblyBinding id='2' xmlns='urn:schemas-microsoft-com:asm.v1'>
                                                                <child />
                                                            </assemblyBinding>
                                                        </runtime>
                                                    </configuration>");
            XmlReader reader = document.CreateReader();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(reader.NameTable);

            // Act
            int elementCount = document.XPathSelectElements(xpath, namespaceManager).Count();

            // Assert
            Assert.That(elementCount, Is.EqualTo(expectedElementCount));
        }
    }
}