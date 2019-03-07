using NUnit.Framework;
using ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Tests.Statusbar
{
    [TestFixture]
    [Category("Integration")]
    public class StatusbarAdapterTests
    {
        private readonly VisualStudioExperimentalInstance _visualStudio = new VisualStudioExperimentalInstance();

        [OneTimeSetUp]
        public void StartVisualStudio()
        {
            _visualStudio.ReStart();
        }

        [OneTimeTearDown]
        public void StopVisualStudio()
        {
            _visualStudio.Stop();
        }

        [Test]
        public void StatusbarShowsXPath()
        {
            // Arrange
            var xml = "<e1><e2><e3 /></e2></e1>";
            var caretPosition = 11;
            var expectedXPath = "/e1/e2/e3";
            _visualStudio.OpenXmlFile(xml, caretPosition);

            // Act
            var statusbar = new StatusbarAutomationModel(_visualStudio.MainWindow);

            // Assert
            Assert.That(statusbar.GetText(), Is.EqualTo(expectedXPath));
        }
    }
}
