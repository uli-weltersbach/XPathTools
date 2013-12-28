using System.Windows.Automation;
using System.Xml.Linq;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    [TestFixture]
    public class XPathStatusbarInformationTests
    {
        private readonly VisualStudioExperimentalInstance _instance = new VisualStudioExperimentalInstance();

        [TestFixtureSetUp]
        public void StartVisualStudio()
        {
            _instance.ReStart();
        }

        [TestFixtureTearDown]
        public void StopVisualStudio()
        {
            _instance.Stop();
        }

        [Test]
        public void StatusbarShowsXPath()
        {
            // Arrange
            string xml = "<e1><e2><e3 /></e2></e1>";
            int caretPosition = 11;
            string expectedXPath = "/e1/e2/e3";
            _instance.OpenXmlFile(xml, caretPosition);

            // Act
            AutomationElement statusbar = _instance.MainWindow.FindDescendant(expectedXPath);

            // Assert
            Assert.That(statusbar, Is.Not.Null);
        }
    }
}