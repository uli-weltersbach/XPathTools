using System.Collections.Generic;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using NSubstitute;
using NUnit.Framework;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration
{
    public class XPathToolsPackageTests
    {
        [TestCase(XPathFormat.Generic, "<a><b id='hello'><c/></b></a>", 19, "/a/b[@id='hello']/c")]
        [TestCase(XPathFormat.Absolute, "<a id='world'><b><c/></b></a>", 19, "/a[1][@id='world']/b[1]/c[1]")]
        public void StatusbarXPathFormatChangesWhenConfigurationIsChanged(XPathFormat xpathFormat, string xml, int xmlElementIndex, string expectedXPath)
        {
            // Arrange
            var serviceContainer = new ServiceContainer();

            var package = new XPathToolsPackage(serviceContainer);

            // Set up the configuration to a default XPathFormat
            var configuration = Substitute.For<IConfiguration>();
            configuration.StatusbarXPathFormat.ReturnsForAnyArgs(XPathFormat.Simplified);
            // Define an XML attribute which should always be shown in the statusbar XPath
            configuration.AlwaysDisplayedAttributes.ReturnsForAnyArgs(new List<XPathSetting>{new XPathSetting{AttributeName = "id"}});
            configuration.PreferredAttributeCandidates.ReturnsForAnyArgs(new List<XPathSetting>());

            // Initialize all services, incl. the statusbar adapter
            var menuCommandService = Substitute.For<IMenuCommandService>();
            var statusbar = Substitute.For<IVsStatusbar>();
            package.Initialize(configuration, menuCommandService, statusbar);

            // Set up the XML repo initialized by the package
            var xmlRepository = serviceContainer.Get<XmlRepository>();
            xmlRepository.LoadXml(xml, null);
            var selectedElement = xmlRepository.GetElement(xmlRepository.GetRootElement(), 1, xmlElementIndex);
            xmlRepository.SetSelected(selectedElement);
            
            var statusbarAdapter = serviceContainer.Get<StatusbarAdapter>();

            // Act
            configuration.StatusbarXPathFormat.ReturnsForAnyArgs(xpathFormat);
            statusbarAdapter.SetText(null, null);

            // Assert
            statusbar.Received().SetText(Arg.Is(expectedXPath));
        }
    }
}