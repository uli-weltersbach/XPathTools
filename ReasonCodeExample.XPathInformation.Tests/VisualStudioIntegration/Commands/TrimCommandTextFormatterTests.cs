using NUnit.Framework;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration.Commands
{
    [TestFixture]
    public class TrimCommandTextFormatterTests
    {
        [TestCase("text", 1, "text")]
        [TestCase("text", 2, "text")]
        [TestCase("abcdefghij", 2, "abcdefghij")]
        [TestCase("abcdefghijk", 2, "...bcdefghijk")]
        [TestCase("abcdefghijklmnopqrstuvwxyz", 2, "...qrstuvwxyz")]
        public void EllipsisIsPrependedCorrectly(string commandText, int elementCount, string expectedCommandText)
        {
            // Arrange
            var textFormatter = new TrimCommandTextFormatter(10);

            // Act
            var output = textFormatter.Format(commandText, elementCount);

            // Assert
            Assert.That(output, Is.EqualTo(expectedCommandText));
        }
    }
}