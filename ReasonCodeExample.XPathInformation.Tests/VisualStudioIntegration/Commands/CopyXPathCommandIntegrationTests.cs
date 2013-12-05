using System.Diagnostics;
using System.IO;
using System.Text;
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
            string tempFile = Path.GetTempFileName();
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(tempFile)))
            {
                writer.WriteLine("<?xml version='1.0' encoding='UTF-8' standalone='yes' ?><a><b /></a>");
            }
            try
            {
                object customIn = null;
                object customOut = null;
                string menuGroupID = new Guid(Symbols.PackageID).ToString("B");
                DTE dte = VsIdeTestHostContext.Dte;
                dte.Application.Documents.Open(tempFile, ReadOnly: true);
                dte.Commands.Raise(menuGroupID, Symbols.CommandIDs.CopyGenericXPath, ref customIn, ref customOut);
            }
            finally
            {
                File.Delete(tempFile);
            }
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