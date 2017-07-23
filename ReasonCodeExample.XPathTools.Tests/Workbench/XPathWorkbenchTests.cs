using System.Xml.Linq;
using NUnit.Framework;
using ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Tests.Workbench
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
            _instance.ClickContextMenuEntry(PackageResources.ShowXPathWorkbenchCommandText);
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
            _instance.ClickContextMenuEntry(PackageResources.ShowXPathWorkbenchCommandText);
            var xpathWorkbench = new XPathWorkbenchAutomationModel(_instance.MainWindow);
            xpathWorkbench.Search("§ invalid XPath §");

            // Assert
            Assert.That(xpathWorkbench.SearchResultText, Does.Contain(PackageResources.XPathEvaluationErrorText));
        }

        [Test]
        public void WorkbenchShowsSearchResultCount()
        {
            // Arrange
            var xml = new XElement("xml");
            _instance.OpenXmlFile(xml.ToString(SaveOptions.DisableFormatting), 0);
            _instance.ClickContextMenuEntry(PackageResources.ShowXPathWorkbenchCommandText);
            var xpathWorkbench = new XPathWorkbenchAutomationModel(_instance.MainWindow);
            var expectedResultText = string.Format(PackageResources.SingleResultText, 1);

            // Act
            xpathWorkbench.Search("/xml");

            // Assert
            Assert.That(xpathWorkbench.SearchResultText, Does.Contain(expectedResultText));
        }
    }
}