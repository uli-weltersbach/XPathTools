using System.Xml.Linq;
using NSubstitute;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Tests.Writers
{
    [TestFixture]
    public class SimplifiedXPathWriterTests
    {
        [TestCase("<a><b><c d=\"value\" /></b></a>", "/a/b/c[@d='value']/@d", "/a/b/c[@d='value']/@d")]
        [TestCase("<a><b x=\"1\"><c d=\"value\" /></b></a>", "/a/b[@x='1']/c[@d='value']/@d", "/a/b[@x='1']/c[@d='value']/@d")]
        [TestCase("<a><b x=\"1\" y=\"1\"><c d=\"value\" /></b></a>", "/a/b[@x='1'][@y='1']/c[@d='value']/@d", "/a/b[@x='1'][@y='1']/c[@d='value']/@d")]
        [TestCase("<a w=\"1\"><b x=\"1\" y=\"1\"><c d=\"value\" /></b></a>", "/a[@w='1']/b[@x='1'][@y='1']/c[@d='value']/@d", "/a[@w='1']/b[@x='1'][@y='1']/c[@d='value']/@d")]
        [TestCase("<ns:a xmlns:ns=\"-\" w=\"1\"><ns:b x=\"1\" y=\"1\"><c d=\"value\" /></ns:b></ns:a>", "/ns:a[@w='1']/ns:b[@x='1'][@y='1']/c[@d='value']/@d", "/a[@w='1']/b[@x='1'][@y='1']/c[@d='value']/@d")]
        [TestCase("<ns:a xmlns:ns=\"-\" ns:w=\"1\" />", "/ns:a[@ns:w='1']", "/a[@w='1']")]
        [TestCase("<ns:a xmlns:ns=\"-\" ns:w=\"1\" />", "/ns:a[@ns:w='1']/@ns:w", "/a[@w='1']/@w")]
        [TestCase("<a xmlns=\"no-prefix\" w=\"1\" />", "/*[local-name()='a'][namespace-uri()='no-prefix'][@w='1']", "/a[@w='1']")]
        [TestCase("<a xmlns=\"no-prefix\" w=\"1\" />", "/*[local-name()='a'][namespace-uri()='no-prefix'][@w='1']/@w", "/a[@w='1']/@w")]
        public void WriteOutputIsValid(string xml, string validXPath, string simplifiedXPath)
        {
            // Arrange
            var includeAllAttributesExceptNamespaceDeclarations = Substitute.For<IAttributeFilter>();
            includeAllAttributesExceptNamespaceDeclarations.IsIncluded(Arg.Any<XAttribute>()).Returns(info => !info.Arg<XAttribute>().IsNamespaceDeclaration);
            var filters = new[] {includeAllAttributesExceptNamespaceDeclarations};
            var testNode = xml.SelectSingleNode(validXPath);

            // Act
            var actualXPath = new SimplifiedXPathWriter(filters).Write(testNode);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(simplifiedXPath));
        }

        [Test]
        public void WriterIsReusable()
        {
            // Arrange
            var writer = new SimplifiedXPathWriter();
            var element = new XElement("a");
            writer.Write(element);

            // Act
            var result = writer.Write(element);

            // Assert
            Assert.That(result, Is.EqualTo("/a"));
        }
    }
}