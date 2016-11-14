using System.Xml.Linq;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration;

namespace ReasonCodeExample.XPathInformation.Tests.Workbench
{
    [TestFixture]
    [Category("Integration")]
    public class XPathWorkbenchTests
    {
        private readonly VisualStudioExperimentalInstance _instance = new VisualStudioExperimentalInstance();

        [OneTimeSetUp]
        public void StartVisualStudio()
        {
            _instance.ReStart(VisualStudioVersion.VS2015);
        }

        [OneTimeTearDown]
        public void StopVisualStudio()
        {
            _instance.Stop();
        }

        [Test]
        public void WorkbenchIsActivatedViaContextMenu()
        {
            // Arrange
            var xml = new XElement("xml");
            _instance.OpenXmlFile(xml.ToString(), 0);

            // Act
            _instance.ClickContextMenuEntry("Open XPath workbench");
            var xpathWorkbench = new XPathWorkbenchAutomationModel(_instance.MainWindow);

            // Assert
            Assert.That(xpathWorkbench.IsVisible, Is.True);
        }

        [Test]
        public void WorkbenchRunsQueryEvenThoughNoNodeIsSelected()
        {
            // Arrange
            var xml = new XElement("xml");
            _instance.OpenXmlFile(xml.ToString(), 0);

            // Act
            _instance.ClickContextMenuEntry("Open XPath workbench");
            var xpathWorkbench = new XPathWorkbenchAutomationModel(_instance.MainWindow);
            xpathWorkbench.Run("§ invalid XPath §");

            // Assert
            Assert.That(xpathWorkbench.SearchResultText, Does.Contain("Error evaluating XPath."));
        }

        [Test]
        public void WorkbenchShowsSearchResultCount()
        {
            // Arrange
            var xml = new XElement("xml");
            _instance.OpenXmlFile(xml.ToString(SaveOptions.DisableFormatting), 0);

            // Act
            _instance.ClickContextMenuEntry("Open XPath workbench");
            var xpathWorkbench = new XPathWorkbenchAutomationModel(_instance.MainWindow);
            xpathWorkbench.Run("/xml");

            // Assert
            Assert.That(xpathWorkbench.SearchResultText, Does.Contain("1 result."));
        }
    }
}