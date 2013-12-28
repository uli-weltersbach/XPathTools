using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration.Commands
{
    [TestFixture]
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

        [Test]
        [STAThread]
        public void XPathIsCopiedToClipboard()
        {
            // Arrange
            string xml = "<xml />";
            _instance.OpenXmlFile(xml, 2);
            string menuText = "Copy XPath";
            string commandText = "Copy generic XPath";

            // Act
            _instance.ExecuteContextMenuCommand(menuText, commandText);

            // Assert
            Assert.That(Clipboard.GetText(), Is.EqualTo("/xml"));
        }
    }
}
