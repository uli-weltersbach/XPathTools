using NUnit.Framework;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration.Commands
{
    [TestFixture]
    public class CommandTextFormatterTests
    {
        [TestCase("text", 1, "(1 match) text")]
        [TestCase("text", 2, "(2 matches) text")]
        [TestCase("abcdefghij", 2, "(2 matches) abcdefghij")]
        [TestCase("abcdefghijk", 2, "(2 matches) ...bcdefghijk")]
        [TestCase("abcdefghijklmnopqrstuvwxyz", 2, "(2 matches) ...qrstuvwxyz")]
        public void EllipsisIsPrependedCorrectly(string commandText, int elementCount, string expectedCommandText)
        {
            // Arrange
            CommandTextFormatter.MaxLength = 10;

            // Act
            var output = CommandTextFormatter.Format(commandText, elementCount);

            // Assert
            Assert.That(output, Is.EqualTo(expectedCommandText));
        }
    }
}