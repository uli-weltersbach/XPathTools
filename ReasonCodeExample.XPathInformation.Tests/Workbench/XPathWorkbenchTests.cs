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

        //[Test]
        //public void SearchIsRunInBackgroundThread()
        //{
        //    // Arrange

        //    // Act

        //    // Assert
        //    Assert.That(, Is.EqualTo());
        //}

        [Test]
        public void WorkbenchIsActivatedViaContextMenu()
        {
            // Arrange
            _instance.OpenXmlFile("<xml />", 2);

            // Act
            _instance.ClickContextMenuEntry("Open XPath workbench");
            var xpathWorkbench = new XPathWorkbenchAutomationModel(_instance.MainWindow);

            // Assert
            Assert.That(xpathWorkbench.IsVisible, Is.True);
        }

        //[Test]
        //public void WorkbenchRunsQueryEvenThoughNoNodeIsSelected()
        //{
        //    // Arrange

        //    // Act

        //    // Assert
        //    Assert.That(, Is.EqualTo());
        //}

        [Test]
        public void WorkbenchShowsSearchResultCount()
        {
            // Arrange
            var xml = new XElement("xml", new XElement("child"), new XElement("child"), new XElement("child"), new XElement("child"));
            _instance.OpenXmlFile(xml.ToString(), 2);

            // Act
            _instance.ClickContextMenuEntry("Open XPath workbench");
            var xpathWorkbench = new XPathWorkbenchAutomationModel(_instance.MainWindow);
            xpathWorkbench.Run("//child");

            // Assert
            Assert.That(xpathWorkbench.SearchResultCount, Is.EqualTo("4 results."));
        }
    }
}