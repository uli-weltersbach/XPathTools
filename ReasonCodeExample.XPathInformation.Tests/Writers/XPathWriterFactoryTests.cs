using System;
using NSubstitute;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Tests.Writers
{
    public class XPathWriterFactoryTests
    {
        [TestCase("Generic", typeof(XPathWriter))]
        [TestCase("Absolute", typeof(AbsoluteXPathWriter))]
        [TestCase("Distinct", typeof(XPathWriter))]
        [TestCase("Simplified", typeof(SimplifiedXPathWriter))]
        public void CanCreateXPathWriterFromFriendlyName(string xpathWriterFriendlyName, Type expectedXPathWriterType)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var factory = new XPathWriterFactory(configuration);

            // Act
            var writer = factory.CreateFromFriendlyName(xpathWriterFriendlyName);

            // Assert
            Assert.That(writer, Is.Not.Null.And.TypeOf(expectedXPathWriterType));
        }
    }
}