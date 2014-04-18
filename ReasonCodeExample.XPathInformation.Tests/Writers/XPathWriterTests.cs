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
        [TestCase("<ns:a xmlns:ns=\"-\" ns:w=\"1\"></ns:a>", "/ns:a[@ns:w='1']")]
        [TestCase("<a xmlns=\"no-prefix\" w=\"1\"></a>", "/*[local-name()='a' and namespace-uri()='no-prefix' and @w='1']")]
        public void WriteOutputIsValid(string xml, string expectedXPath)
        {
            // Arrange
            INodeFilter includeAll = Substitute.For<INodeFilter>();
            includeAll.IsIncluded(Arg.Any<XObject>()).Returns(info => !info.Arg<XAttribute>().IsNamespaceDeclaration);
            XDocument document = XDocument.Parse(xml);
            IEnumerator enumerator = ((IEnumerable)document.Root.XPathEvaluate(expectedXPath, new SimpleXmlNamespaceResolver(document))).GetEnumerator();
            enumerator.MoveNext();
            XObject testNode = enumerator.Current as XObject;

            // Act
            string actualXPath = new XPathWriter(new[] { includeAll }).Write(testNode);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }
    }
}