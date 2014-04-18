using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;
using NSubstitute;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Tests.Writers
{
    [TestFixture]
    public class AbsoluteXPathWriterTests
    {
        [TestCase("<a><b><c d=\"value\" /></b></a>", "/a[1]/b[1]/c[1][@d='value']/@d")]
        [TestCase("<a><b x=\"1\"><c /><c d=\"value\" /></b></a>", "/a[1]/b[1][@x='1']/c[2][@d='value']/@d")]
        [TestCase("<a><b x=\"1\" y=\"1\"><c /><c d=\"value\" /></b></a>", "/a[1]/b[1][@x='1'][@y='1']/c[2][@d='value']/@d")]
        [TestCase("<ns:a xmlns:ns=\"-\" w=\"1\"><ns:b x=\"1\" y=\"1\"><c d=\"value\" /><c d=\"hello\" /><c d=\"world\" /><c ns:d=\"!\" /><c /></ns:b></ns:a>", "/ns:a[1][@w='1']/ns:b[1][@x='1'][@y='1']/c[4][@ns:d='!']/@ns:d")]
        [TestCase("<ns:a xmlns:ns=\"-\" ns:w=\"1\" />", "/ns:a[1][@ns:w='1']")]
        [TestCase("<ns:a xmlns:ns=\"-\" ns:w=\"1\" />", "/ns:a[1][@ns:w='1']/@ns:w")]
        [TestCase("<r><a xmlns=\"no-prefix\" w=\"1\" /><a xmlns=\"no-prefix\" w=\"1\" /><a xmlns=\"no-prefix\" w=\"1\" /></r>", "/r[1]/*[local-name()='a'][namespace-uri()='no-prefix'][2][@w='1']")]
        [TestCase("<r><a xmlns=\"no-prefix\" w=\"1\" /><a xmlns=\"no-prefix\" w=\"1\" /><a xmlns=\"no-prefix\" w=\"1\" /></r>", "/r[1]/*[local-name()='a'][namespace-uri()='no-prefix'][2][@w='1']/@w")]
        public void WriteOutputIsValid(string xml, string expectedXPath)
        {
            // Arrange
            var includeAllAttributesExceptNamespaceDeclarations = Substitute.For<INodeFilter>();
            includeAllAttributesExceptNamespaceDeclarations.IsIncluded(Arg.Any<XObject>()).Returns(info => !info.Arg<XAttribute>().IsNamespaceDeclaration);
            var filters = new[] { includeAllAttributesExceptNamespaceDeclarations };
            var testNode = GetTestNode(xml, expectedXPath);

            // Act
            var actualXPath = new AbsoluteXPathWriter(filters).Write(testNode);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }

        private XObject GetTestNode(string xml, string xpath)
        {
            var document = XDocument.Parse(xml);
            var enumerator = ((IEnumerable)document.Root.XPathEvaluate(xpath, new SimpleXmlNamespaceResolver(document))).GetEnumerator();
            enumerator.MoveNext();
            return (XObject)enumerator.Current;
        }
    }
}