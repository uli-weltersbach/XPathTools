using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration;
using Assert = NUnit.Framework.Assert;

namespace ReasonCodeExample.XPathInformation.Tests.Writers
{
    [TestFixture]
    [Category("Integration")]
    public class CopyXPathCommandTests
    {
        private readonly VisualStudioExperimentalInstance _instance = new VisualStudioExperimentalInstance();

        [OneTimeSetUp]
        public void StartVisualStudio()
        {
            _instance.ReStart(VisualStudioVersion.VS2015);
        }

        [OneTimeTearDown]
        public void StopVisualStudio()
        {
            _instance.Stop();
        }

        [TestCase("<xml />", 2, "/xml")]
        [STAThread]
        public void XPathCommandsAreAvailable(string xml, int caretPosition, string expectedXPath)
        {
            // Arrange
            _instance.OpenXmlFile(xml, caretPosition);

            // Act
            var matches = GetAvailableCopyXPathCommands();

            // Assert
            Assert.That(matches.Count, Is.EqualTo(3));
        }

        [TestCase("<xml />", 2, "/xml")]
        [STAThread]
        public void GenericXPathIsCopiedToClipboard(string xml, int caretPosition, string expectedXPath)
        {
            // Arrange
            _instance.OpenXmlFile(xml, caretPosition);
            var matches = GetAvailableCopyXPathCommands();
            var copyGenericXPathCommand = matches.First();

            // Act
            copyGenericXPathCommand.LeftClick();

            // Assert
            Assert.That(Clipboard.GetText(), Is.EqualTo(expectedXPath));
        }

        private IList<AutomationElement> GetAvailableCopyXPathCommands()
        {
            return _instance.GetContextMenuSubMenuCommands("Copy XPath", new Regex(@"\(\d+ match"));
        }
    }
}