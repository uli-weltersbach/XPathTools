using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using NUnit.Framework;
using ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Tests.Writers
{
    [TestFixture]
    [Category("Integration")]
    [Apartment(ApartmentState.STA)]
    public class CopyXPathCommandTests
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

        [Test]
        public void XPathCommandsAreAvailable()
        {
            // Arrange
            var xml = "<xml />";
            var caretPosition = 2;
            _instance.OpenXmlFile(xml, caretPosition);

            // Act
            var matches = GetAvailableCopyXPathCommands();

            // Assert
            Assert.That(matches.Count, Is.EqualTo(3));
        }

        [Test]
        public void GenericXPathIsCopiedToClipboard()
        {
            // Arrange
            var xml = "<xml />";
            var caretPosition = 2;
            var expectedXPath = "/xml";
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
