using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;
using NSubstitute;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Tests.Writers
{
    [TestFixture]
    public class XPathWriterTests
    {
        [TestCase("<a><b><c d=\"value\" /></b></a>", "/a/b/c[@d='value']/@d")]
        [TestCase("<a><b x=\"1\"><c d=\"value\" /></b></a>", "/a/b[@x='1']/c[@d='value']/@d")]
        [TestCase("<a><b x=\"1\" y=\"1\"><c d=\"value\" /></b></a>", "/a/b[@x='1' and @y='1']/c[@d='value']/@d")]
        [TestCase("<a w=\"1\"><b x=\"1\" y=\"1\"><c d=\"value\" /></b></a>", "/a[@w='1']/b[@x='1' and @y='1']/c[@d='value']/@d")]
        [TestCase("<ns:a xmlns:ns=\"-\" w=\"1\"><ns:b x=\"1\" y=\"1\"><c d=\"value\" /></ns:b></ns:a>", "/ns:a[@w='1']/ns:b[@x='1' and @y='1']/c[@d='value']/@d")]
        [TestCase("<ns:a xmlns:ns=\"-\" ns:w=\"1\" />", "/ns:a[@ns:w='1']")]
        [TestCase("<ns:a xmlns:ns=\"-\" ns:w=\"1\" />", "/ns:a[@ns:w='1']/@ns:w")]
        [TestCase("<a xmlns=\"no-prefix\" w=\"1\" />", "/*[local-name()='a' and namespace-uri()='no-prefix' and @w='1']")]
        [TestCase("<a xmlns=\"no-prefix\" w=\"1\" />", "/*[local-name()='a' and namespace-uri()='no-prefix' and @w='1']/@w")]
        public void WriteOutputIsValid(string xml, string expectedXPath)
        {
            // Arrange
            var includeAllAttributesExceptNamespaceDeclarations = Substitute.For<INodeFilter>();
            includeAllAttributesExceptNamespaceDeclarations.IsIncluded(Arg.Any<XObject>()).Returns(info => !info.Arg<XAttribute>().IsNamespaceDeclaration);
            var filters = new[] { includeAllAttributesExceptNamespaceDeclarations };
            var testNode = GetTestNode(xml, expectedXPath);

            // Act
            var actualXPath = new XPathWriter(filters).Write(testNode);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }

        private XObject GetTestNode(string xml, string xpath)
        {
            var document = XDocument.Parse(xml);
            var enumerator = ((IEnumerable)document.Root.XPathEvaluate(xpath, new SimpleXmlNamespaceResolver(document))).GetEnumerator();
            enumerator.MoveNext();
            var testNode = enumerator.Current as XObject;
            return testNode;
        }
    }
}