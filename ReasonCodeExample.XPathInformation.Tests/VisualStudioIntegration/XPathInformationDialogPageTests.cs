using System;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    public class XPathInformationDialogPageTests
    {
        [TestCase("Generic", typeof(XPathWriter))]
        [TestCase("Absolute", typeof(AbsoluteXPathWriter))]
        [TestCase("Distinct", typeof(XPathWriter))]
        [TestCase("Simplified", typeof(SimplifiedXPathWriter))]
        public void CanDeserializeStatusbarXPathWriterFromSetting(XPathFormat xpathFormat, Type expectedType)
        {
            // Arrange
            var configuration = new XPathInformationDialogPage();
            configuration.StatusbarXPathFormatSetting = xpathFormat;

            // Act
            IWriter writer = null; //configuration.StatusbarXPathWriter;

            // Assert
            Assert.That(writer, Is.Not.Null.And.TypeOf(expectedType));
        }
    }
}