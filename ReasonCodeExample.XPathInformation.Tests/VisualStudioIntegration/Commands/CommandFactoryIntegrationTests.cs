using System.Diagnostics;
using System.Xml.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration.Commands
{
    /// <summary>
    /// These tests have to be run using the Microsoft test runner.
    /// </summary>
    [TestClass]
    public class CommandFactoryIntegrationTests
    {
        private const string VisualStudioHostType = "VS IDE";

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CanCreateServiceProvider()
        {
            // Act
            IServiceProvider serviceProvider = VsIdeTestHostContext.ServiceProvider;

            // Assert
            Assert.IsNotNull(serviceProvider, "VsIdeTestHostContext.ServiceProvider is null");
        }

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CanCreateVisualStudioShell()
        {
            // Arrange
            IServiceProvider serviceProvider = VsIdeTestHostContext.ServiceProvider;

            // Act
            IVsShell vistualStudioShell = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;

            // Assert
            Assert.IsNotNull(vistualStudioShell, "vistualStudioShell is null");
        }

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CanLoadPackage()
        {
            // Arrange
            IServiceProvider serviceProvider = VsIdeTestHostContext.ServiceProvider;
            IVsShell visualStudioShell = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
            Guid packageGuid = new Guid(Symbols.PackageID);
            IVsPackage package;

            // Act
            int actualLoadPackageResult = visualStudioShell.LoadPackage(ref packageGuid, out package);

            // Assert
            Assert.AreEqual(VSConstants.S_OK, actualLoadPackageResult, "Load package result wasn't S_OK");
            Assert.IsNotNull(package, "Package failed to load");
        }

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CanCreateDTE()
        {
            // Act
            DTE dte = VsIdeTestHostContext.Dte;

            // Assert
            Assert.IsNotNull(dte, "VsIdeTestHostContext.Dte is null");
        }
    }
}