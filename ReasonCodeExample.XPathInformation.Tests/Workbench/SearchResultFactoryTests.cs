using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Workbench;

namespace ReasonCodeExample.XPathInformation.Tests.Workbench
{
    [TestFixture]
    public class SearchResultFactoryTests
    {
        private const string Xml = @"<xml>
    <child name='first'>Hello World!</child>
<child name='second'>Guten Morgen Welt!</child>
<child />
</xml>";

        [TestCase(".", 1)]
        [TestCase("/child[@name='first']", 2)]
        [TestCase("/child[@name='second']", 3)]
        public void SetsLineNumber(string xpath, int lineNumber)
        {
            // Arrange
            var factory = new SearchResultFactory();
            var evaluationResult = XElement.Parse(Xml, LoadOptions.SetLineInfo).XPathEvaluate(xpath);

            // Act
            var result = factory.Parse(evaluationResult).Single();

            // Assert
            Assert.That(result.LineNumber, Is.EqualTo(lineNumber));
        }

        [TestCase(".", 2)]
        [TestCase("/child[@name='first']", 6)]
        [TestCase("/child[@name='second']", 2)]
        public void SetsLinePosition(string xpath, int linePosition)
        {
            // Arrange
            var factory = new SearchResultFactory();
            var evaluationResult = XElement.Parse(Xml, LoadOptions.SetLineInfo).XPathEvaluate(xpath);

            // Act
            var result = factory.Parse(evaluationResult).Single();

            // Assert
            Assert.That(result.LinePosition, Is.EqualTo(linePosition));
        }

        [Test]
        public void HandlesNullGracefully()
        {
            // Arrange
            var factory = new SearchResultFactory();

            // Act
            var results = factory.Parse(null);

            // Assert
            Assert.That(results, Is.Not.Null.And.Empty);
        }

        [Test]
        public void ParsesXPathWhichEvaluatesToAttribute()
        {
            // Arrange
            var factory = new SearchResultFactory();
            var evaluationResult = XElement.Parse(Xml).XPathEvaluate("/child/@name");
            var expectedAttributeValues = new[]
                                          {
                                              "name=\"first\"",
                                              "name=\"second\""
                                          };
            // Act
            var results = factory.Parse(evaluationResult).Select(s => s.Value).ToArray();

            // Assert
            Assert.That(results, Is.EquivalentTo(expectedAttributeValues));
        }

        [Test]
        public void ParsesXPathWhichEvaluatesToBoolean()
        {
            // Arrange
            var factory = new SearchResultFactory();
            var evaluationResult = XElement.Parse(Xml).XPathEvaluate("count(/child/@name) > 0");

            // Act
            var result = factory.Parse(evaluationResult).Single();

            // Assert
            Assert.That(result.Value, Is.EqualTo(bool.TrueString));
        }

        [Test]
        public void ParsesXPathWhichEvaluatesToElements()
        {
            // Arrange
            var factory = new SearchResultFactory();
            var evaluationResult = XElement.Parse(Xml).XPathEvaluate("/child");

            // Act
            var results = factory.Parse(evaluationResult);

            // Assert
            Assert.That(results, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void ParsesXPathWhichEvaluatesToNumber()
        {
            // Arrange
            var factory = new SearchResultFactory();
            var evaluationResult = XElement.Parse(Xml).XPathEvaluate("count(/child/@name)");

            // Act
            var result = factory.Parse(evaluationResult).Single();

            // Assert
            Assert.That(result.Value, Is.EqualTo("2"));
        }

        [Test]
        public void ParsesXPathWhichEvaluatesToString()
        {
            // Arrange
            var factory = new SearchResultFactory();
            var evaluationResult = XElement.Parse(Xml).XPathEvaluate("/child/text()");
            var expectedTextValues = new[]
                                     {
                                         "Hello World!",
                                         "Guten Morgen Welt!"
                                     };

            // Act
            var results = factory.Parse(evaluationResult).Select(s => s.Value).ToArray();

            // Assert
            Assert.That(results, Is.EqualTo(expectedTextValues));
        }
    }
}