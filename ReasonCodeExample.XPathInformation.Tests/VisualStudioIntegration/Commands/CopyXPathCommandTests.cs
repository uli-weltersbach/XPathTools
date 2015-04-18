using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration.Commands
{
    [TestFixture]
    [Category("Integration")]
    public class CopyXPathCommandTests
    {
        private readonly VisualStudioExperimentalInstance _instance = new VisualStudioExperimentalInstance();

        [TestFixtureSetUp]
        public void StartVisualStudio()
        {
            _instance.ReStart();
        }

        [TestFixtureTearDown]
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
            return _instance.GetContextMenuCommands("Copy XPath", new Regex(@"\(\d+ match"));
        }
    }
}