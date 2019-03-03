using System;
using NSubstitute;
using NUnit.Framework;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;
using ReasonCodeExample.XPathTools.Writers;

namespace ReasonCodeExample.XPathTools.Tests.Writers
{
    public class XPathWriterFactoryTests
    {
        [TestCase(XPathFormat.Generic, typeof(XPathWriter))]
        [TestCase(XPathFormat.Absolute, typeof(AbsoluteXPathWriter))]
        [TestCase(XPathFormat.Distinct, typeof(XPathWriter))]
        [TestCase(XPathFormat.Simplified, typeof(SimplifiedXPathWriter))]
        public void CanCreateXPathWriter(XPathFormat format, Type expectedXPathWriterType)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var factory = new XPathWriterFactory(() => configuration);

            // Act
            var writer = factory.CreateForXPathFormat(format);

            // Assert
            Assert.That(writer, Is.Not.Null.And.TypeOf(expectedXPathWriterType));
        }
    }
}
