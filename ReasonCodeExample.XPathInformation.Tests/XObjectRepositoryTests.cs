using System.Xml.Linq;
using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests
{
    [TestFixture]
    public class XObjectRepositoryTests
    {
        [Test]
        public void XPathIsStored()
        {
            // Arrange
            XElement expectedValue = new XElement("value");
            XObjectRepository repository = new XObjectRepository();
            XObjectRepository otherRepository = new XObjectRepository();

            // Act
            repository.Put(expectedValue);

            // Assert
            Assert.That(otherRepository.Get(), Is.EqualTo(expectedValue));
        }
    }
}