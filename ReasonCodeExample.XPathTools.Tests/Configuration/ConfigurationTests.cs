﻿using System.Windows.Forms;
using NUnit.Framework;
using ReasonCodeExample.XPathTools.Tests.Statusbar;
using ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Tests.Configuration
{
    [TestFixture]
    [Category(TestCategory.Integration)]
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
        
        [TestCase(XPathFormat.Generic, "<a><b id='hello'><c/></b><b id='world'><c/></b></a>", 41, "/a/b/c")]
        [TestCase(XPathFormat.Absolute, "<a><b id='hello'><c/></b><b id='world'><c/></b></a>", 41, "/a[1]/b[2]/c[1]")]
        [TestCase(XPathFormat.Distinct, "<a><b id='hello'><c/></b><b id='world'><c/></b></a>", 41, "/a/b[@id='world']/c")]
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
            var statusbar = new StatusbarAutomationModel(_visualStudio.MainWindow);
            Assert.That(statusbar.GetText(), Is.EqualTo(expectedXPath));
        }
    }
}
