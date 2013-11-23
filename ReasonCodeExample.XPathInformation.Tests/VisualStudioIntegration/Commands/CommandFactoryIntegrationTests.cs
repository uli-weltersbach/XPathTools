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
            Guid packageGuid = new Guid(CommandFactory.PackageID);
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

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CanRetrieveCommands()
        {
            // Arrange
            DTE dte = VsIdeTestHostContext.Dte;

            // Act
            EnvDTE.Commands commands = dte.Commands;

            // Assert
            Assert.IsNotNull(commands, "VsIdeTestHostContext.Dte.Commands is null");
        }

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CanExecutecopyXPathCommand()
        {
            ExecuteCopyXPathCommand();
        }

        private void ExecuteCopyXPathCommand()
        {
            object customIn = null;
            object customOut = null;
            string menuGroupID = new Guid(CommandFactory.CommandsID).ToString("B").ToUpper();
            DTE dte = VsIdeTestHostContext.Dte;
            dte.Commands.Raise(menuGroupID, CommandFactory.CopyXPathCommandID, ref customIn, ref customOut);
        }

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CopyXPathCommandCopiesXPathToClipboard()
        {
            Action test = () =>
                {
                    // Arrange
                    Clipboard.Clear();
                    string expectedText = Guid.NewGuid().ToString();
                    new XPathRepository().Put(expectedText);

                    // Act
                    ExecuteCopyXPathCommand();
                    string actualText = Clipboard.GetText();

                    // Assert
                    Assert.AreEqual(expectedText, actualText);
                };
            UIThreadInvoker.Invoke(test);
        }
    }
}