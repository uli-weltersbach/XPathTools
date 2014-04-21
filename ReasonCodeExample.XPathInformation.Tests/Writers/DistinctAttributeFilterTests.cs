using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Tests.Writers
{
    [TestFixture]
    public class DistinctAttributeFilterTests
    {
        [TestCase("<a><b /><b /><b /><b><c id='value' /><c id='other-value' /></b></a>", "/a/b/c[@id='value']")]
        [TestCase("<a><b /><b /><b /><b><c id='same' /><c id='same' /></b></a>", "")]
        [TestCase("<a><b /><b /><b /><b><c id='same' name='unique'/><c id='same' /></b></a>", "/a/b/c[@name='unique']")]
        [TestCase("<a><b /><b /><b /><b><c id='same' type='unique'/><c id='same' /></b></a>", "/a/b/c[@type='unique']")]
        [TestCase("<a><b id='same'><c id='same' /></b><b id='same'><c id='same' /></b><b id='unique'><c id='same' /></b></a>", "/a/b[@id='unique']/c")]
        [TestCase("<a:element xmlns:a=\"1\"><b:element xmlns:b=\"2\"/></a:element>", "/a:element/b:element")]
        public void PreferredAttributeIsUsedInDistinctPath(string xml, string expectedXPath)
        {
            // Arrange
            var element = string.IsNullOrEmpty(expectedXPath) ? null : xml.SelectSingleNode(expectedXPath);
            var settings = new[]
                           {
                               new XPathSetting {AttributeName = "id"},
                               new XPathSetting {AttributeName = "name"},
                               new XPathSetting {AttributeName = "type"}
                           };
            var filter = new DistinctAttributeFilter(settings);
            var writer = new XPathWriter(new[] {filter});

            // Act
            var actualXPath = writer.Write(element);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }
    }
}