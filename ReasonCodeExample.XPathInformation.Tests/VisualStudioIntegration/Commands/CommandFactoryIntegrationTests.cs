using System;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration.Commands
{
    /// <summary>
    /// These tests have to be run using the "standard" Microsoft test runner.
    /// </summary>
    [TestClass]
    public class CommandFactoryIntegrationTests
    {
        private const string HostType = "VS IDE";

        private delegate void LoadPackageDelegate();

        [TestMethod]
        [HostType(HostType)]
        public void CanCreateServiceProvider()
        {
            // Act
            IServiceProvider serviceProvider = VsIdeTestHostContext.ServiceProvider;

            // Assert
            Assert.IsNotNull(serviceProvider, "VsIdeTestHostContext.ServiceProvider is null");
        }

        [TestMethod]
        [HostType(HostType)]
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
        [HostType(HostType)]
        public void CanLoadPackage()
        {
            UIThreadInvoker.Invoke((LoadPackageDelegate)LoadPackage);
        }

        private void LoadPackage()
        {
            // Arrange
            IServiceProvider serviceProvider = VsIdeTestHostContext.ServiceProvider;
            IVsShell vistualStudioShell = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
            Guid packageGuid = new Guid(CommandFactory.PackageID);
            IVsPackage package;

            // Act
            int actualLoadPackageResult = vistualStudioShell.LoadPackage(ref packageGuid, out package);

            // Assert
            Assert.AreEqual(VSConstants.S_OK, actualLoadPackageResult, "Load package result wasn't S_OK");
            Assert.IsNotNull(package, "Package failed to load");
        }
    }
}