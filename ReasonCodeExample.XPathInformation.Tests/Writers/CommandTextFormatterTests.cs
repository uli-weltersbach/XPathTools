using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Tests.Writers
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
            var textFormatter = new CommandTextFormatter(10);

            // Act
            var output = textFormatter.Format(commandText, elementCount);

            // Assert
            Assert.That(output, Is.EqualTo(expectedCommandText));
        }
    }
}