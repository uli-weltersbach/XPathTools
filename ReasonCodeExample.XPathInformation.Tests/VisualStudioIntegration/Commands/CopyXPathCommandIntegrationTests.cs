using System;
using System.Windows;
using System.Xml.Linq;
using EnvDTE;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration.Commands
{
    [TestClass]
    public class CopyXPathCommandIntegrationTests
    {
        private const string VisualStudioHostType = "VS IDE";

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
        public void CanExecuteCopyXPathCommand()
        {
            ExecuteCopyXPathCommand();
        }

        private void ExecuteCopyXPathCommand()
        {
            object customIn = null;
            object customOut = null;
            string menuGroupID = new Guid(Symbols.PackageID).ToString("B");
            DTE dte = VsIdeTestHostContext.Dte;
            dte.Commands.Raise(menuGroupID, Symbols.CommandIDs.CopyGenericXPath, ref customIn, ref customOut);
        }

        [TestMethod]
        [HostType(VisualStudioHostType)]
        public void CopyXPathCommandCopiesXPathToClipboard()
        {
            Action test = () =>
                {
                    // Arrange
                    Clipboard.Clear();
                    XElement expectedElement = new XElement("element" + DateTime.UtcNow.Ticks);
                    new XPathRepository().Put(expectedElement);

                    // Act
                    ExecuteCopyXPathCommand();
                    string actualText = Clipboard.GetText();

                    // Assert
                    Assert.AreEqual(expectedElement.Name, actualText);
                };
            UIThreadInvoker.Invoke(test);
        }
    }
}