using System.Windows.Automation;
using System.Windows.Forms;
using NUnit.Framework;
using ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Tests.Configuration
{
    [TestFixture]
    [Category("Integration")]
    public class ConfigurationTests
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
        
        [TestCase(XPathFormat.Generic, "<a><b id='hello'><c/></b></a>", 19, "/a/b/c")]
        [TestCase(XPathFormat.Absolute, "<a id='world'><b><c/></b></a>", 19, "/a[1]/b[1]/c[1]")]
        [TestCase(XPathFormat.Distinct, "<a><b id='hello'><c/></b></a>", 19, "/a/b[@id='hello']/c")]
        public void StatusbarXPathFormatChangesWhenConfigurationIsChanged(XPathFormat xpathFormat, string xml, int xmlElementIndex, string expectedXPath)
        {
            // Arrange
            var configuration = new XPathToolsDialogPageAutomationModel(_visualStudio);
            configuration.SetStatusbarXPathFormat(XPathFormat.Simplified);
            _visualStudio.OpenXmlFile(xml, xmlElementIndex);

            // Act
            configuration.SetStatusbarXPathFormat(xpathFormat);
            SendKeys.SendWait("{LEFT}"); // Move the caret to trigger a statusbar update

            // Assert
            var statusbar = _visualStudio.MainWindow.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "StatusBar", PropertyConditionFlags.IgnoreCase));
            var text = statusbar.FindDescendantByText(expectedXPath);
            Assert.That(text, Is.Not.Null);
        }
    }
}
