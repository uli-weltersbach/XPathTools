using System.Collections.Generic;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration.Configuration
{
    [TestFixture]
    public class CommaSeparatedValueConverterTests
    {
        [Test]
        public void CanConvertFromCommaSeparatedString()
        {
            // Arrange
            CommaSeparatedValueConverter converter = new CommaSeparatedValueConverter();
            string value = "1,2,3,4,5,,6,";
            IEnumerable<string> expectedConvertedValue = new List<string>
            {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6"
            };

            // Act
            IEnumerable<string> convertedValue = converter.ConvertFrom(value) as IEnumerable<string>;

            // Assert
            Assert.That(convertedValue, Is.EqualTo(expectedConvertedValue));
        }

        [Test]
        public void CanConvertToCommaSeparatedString()
        {
            // Arrange
            CommaSeparatedValueConverter converter = new CommaSeparatedValueConverter();
            IEnumerable<string> value = new List<string>
            {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6"
            };
            string expectedConvertedValue = "1, 2, 3, 4, 5, 6";

            // Act
            string convertedValue = converter.ConvertTo(value, typeof(string)) as string;

            // Assert
            Assert.That(convertedValue, Is.EqualTo(expectedConvertedValue));
        }
    }
}