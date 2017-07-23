using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using NSubstitute;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    public class XPathInformationPackageTests
    {
        [TestCase(XPathFormat.Generic, "<a><b><c/></b></a>", "/a/b/c")]
        [TestCase(XPathFormat.Absolute, "<a><b><c/></b></a>", "/a[1]/b[1]/c[1]")]
        public void StatusbarXPathFormatChangesWhenConfigurationIsChanged(XPathFormat xpathFormat, string xml, string expectedXPath)
        {
            // Arrange
            var serviceContainer = new ServiceContainer();

            var package = new XPathInformationPackage(serviceContainer);

            var configuration = Substitute.For<IConfiguration>();
            configuration.StatusbarXPathFormat.ReturnsForAnyArgs(XPathFormat.Simplified);
            configuration.AlwaysDisplayedAttributes.ReturnsForAnyArgs(new List<XPathSetting>());
            configuration.PreferredAttributeCandidates.ReturnsForAnyArgs(new List<XPathSetting>());

            var menuCommandService = Substitute.For<IMenuCommandService>();
            var statusbar = Substitute.For<IVsStatusbar>();
            package.Initialize(configuration, menuCommandService, statusbar);

            var statusbarAdapter = serviceContainer.Get<StatusbarAdapter>();

            // Act
            configuration.StatusbarXPathFormat.ReturnsForAnyArgs(xpathFormat);
            statusbarAdapter.SetText(null, null);

            // Assert
            statusbar.Received().SetText(Arg.Is(expectedXPath));
        }
    }
}