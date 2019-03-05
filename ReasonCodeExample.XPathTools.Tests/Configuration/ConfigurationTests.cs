using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Xml.Linq;
using Microsoft.VisualStudio.Shell.Interop;
using NSubstitute;
using NUnit.Framework;
using ReasonCodeExample.XPathTools.Configuration;
using ReasonCodeExample.XPathTools.Statusbar;
using ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration;
using ReasonCodeExample.XPathTools.Tests.Workbench;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Tests.Configuration
{
    [TestFixture]
    [Category("Integration")]
    public class ConfigurationTests
    {
        private readonly VisualStudioExperimentalInstance _instance = new VisualStudioExperimentalInstance();

        [OneTimeSetUp]
        public void StartVisualStudio()
        {
            _instance.ReStart();
        }

        [OneTimeTearDown]
        public void StopVisualStudio()
        {
            _instance.Stop();
        }
        
        [TestCase(XPathFormat.Generic, "<a><b id='hello'><c/></b></a>", 19, "/a/b[@id='hello']/c")]
        [TestCase(XPathFormat.Absolute, "<a id='world'><b><c/></b></a>", 19, "/a[1][@id='world']/b[1]/c[1]")]
        public void StatusbarXPathFormatChangesWhenConfigurationIsChanged(XPathFormat xpathFormat, string xml, int xmlElementIndex, string expectedXPath)
        {
            // Arrange
            _instance.OpenXmlFile(xml, xmlElementIndex);
            var configuration = new XPathToolsDialogPageAutomationModel(_instance.MainWindow);
            
            // Act
            configuration.SetStatusbarXPathFormat(xpathFormat);

            // Assert
            throw new NotImplementedException();
            // CHECK THAT STATUSBAR TEXT MATCHES "expectedXPath"
        }
    }
}
