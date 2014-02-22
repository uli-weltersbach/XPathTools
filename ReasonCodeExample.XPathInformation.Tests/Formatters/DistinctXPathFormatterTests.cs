using System;
using System.Diagnostics;
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
        [TestCase("<a:element xmlns:a=\"1\"><b:element xmlns:b=\"2\"/></a:element>", 1, "/a:element/b:element")]
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

        [TestCase("<a:element xmlns:a=\"1\" xmlns:b=\"2\"><b:element id=\"1\"/><b:element id=\"2\"/><b:element id=\"3\"/></a:element>", "id", 3, "/a:element/b:element[@id='3']")]
        public void PreferredAttributeIsNotDuplicatedInDistinctPath(string xml, string testAttributeName, int testElementIndex, string expectedXPath)
        {
            // Arrange
            XElement element = XElement.Parse(xml);
            XElement testElement = (XElement) element.DescendantNodesAndSelf().ElementAt(testElementIndex);
            XAttribute testAttribute = testElement.Attribute(testAttributeName);

            // Act
            string actualXPath = _formatter.Format(testAttribute as XObject);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
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

        private XElement CreateShallowDocument(int elementCount)
        {
            XElement root = new XElement("root");
            for (int i = 0; i < elementCount; i++)
            {
                root.Add(new XElement("child"));
            }
            XDocument document = new XDocument();
            document.Add(root);
            return root.Document.Root.Elements().ElementAt(elementCount/2);
        }

        private XElement CreateDeepDocument(int elementCount)
        {
            XElement root = new XElement("root");
            XElement current = root;
            for (int i = 0; i < elementCount; i++)
            {
                XElement child = new XElement("child");
                current.Add(child);
                current = child;
            }
            XDocument document = new XDocument();
            document.Add(root);
            return root.Document.Root.DescendantsAndSelf().ElementAt(elementCount/2);
        }

        [Test]
        [ExpectedException(typeof (XPathException), ExpectedMessage = "The xpath query is too complex.")]
        public void DeepDocumentCausesXPathComplexityIssue()
        {
            // Arrange
            XElement element = CreateDeepDocument(2050);

            // Act
            string xpath = _formatter.Format(element);
        }

        [Test]
        public void DeepDocumentDoesNotCausePerformanceIssues([Values(10, 100, 1000, 2000)] int elementCount)
        {
            // Arrange
            XElement element = CreateDeepDocument(elementCount);
            TimeSpan maxAcceptableComputationTime = TimeSpan.FromSeconds(1);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            // Act
            string xpath = _formatter.Format(element);
            timer.Stop();

            // Assert
            Assert.That(timer.Elapsed, Is.LessThanOrEqualTo(maxAcceptableComputationTime));
        }

        [Test]
        public void ElementNamespaceFormatIsCorrect()
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

            // Act
            string xpath = _formatter.Format(document.Descendants().ElementAt(2));

            // Assert
            Assert.That(xpath, Is.EqualTo("/configuration/runtime/*[local-name()='assemblyBinding' and namespace-uri()='urn:schemas-microsoft-com:asm.v1'][@id='1']"));
        }

        [Test]
        public void ShallowDocumentDoesNotCausePerformanceIssues([Values(10, 100, 1000, 10000, 100000)] int elementCount)
        {
            // Arrange
            XElement element = CreateShallowDocument(elementCount);
            TimeSpan maxAcceptableComputationTime = TimeSpan.FromSeconds(1);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            // Act
            string xpath = _formatter.Format(element);
            timer.Stop();

            // Assert
            Assert.That(timer.Elapsed, Is.LessThanOrEqualTo(maxAcceptableComputationTime));
        }
    }
}