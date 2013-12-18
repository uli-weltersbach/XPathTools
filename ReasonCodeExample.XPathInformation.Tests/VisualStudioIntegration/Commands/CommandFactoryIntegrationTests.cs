using System.Text;
using System.Windows.Automation;
using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration.Commands
{
    [TestFixture]
    public class CommandFactoryIntegrationTests
    {
        [Test]
        public void CanStartExperimentalInstance()
        {
            // Arrange
            VisualStudioExperimentalInstance instance = null;
            try
            {
                // Act
                instance = new VisualStudioExperimentalInstance();

                // Assert
                Assert.That(instance.DevelopmentEnvironment, Is.Not.Null);
            }
            finally
            {
                if (instance != null)
                    instance.Dispose();
            }
        }

        [Test]
        public void CanAccessExperimentalInstanceMainWindow()
        {
            // Arrange
            VisualStudioExperimentalInstance instance = null;
            try
            {
                // Act
                instance = new VisualStudioExperimentalInstance();

                // Assert
                Assert.That(instance.MainWindow, Is.Not.Null);
            }
            finally
            {
                if (instance != null)
                    instance.Dispose();
            }
        }

        [Test]
        public void ExtensionIsLoaded()
        {
            // Arrange
            VisualStudioExperimentalInstance instance = null;
            try
            {
                // Act
                instance = new VisualStudioExperimentalInstance();
                var menu = instance.MainWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Tools", PropertyConditionFlags.IgnoreCase));

                // Assert
                Assert.That(menu, Is.Not.Null);
            }
            finally
            {
                if (instance != null)
                    instance.Dispose();
            }
        }
    }
}