using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration.Commands
{
    [TestFixture]
    public class CommandFactoryTests
    {
        [Test]
        public void CanCreateInstance()
        {
            CommandFactory commandFactory = new CommandFactory();
        }

        [Test]
        public void ImplementsVisualStudioPackageInterface()
        {
            // Assert
            Assert.That(new CommandFactory(), Is.InstanceOf<IVsPackage>());
        }

        [Test]
        public void CanSetSite()
        {
            // Arrange
            IVsPackage commandFactory = new CommandFactory();
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Act
            int actualResult = commandFactory.SetSite(serviceProvider);

            // Assert
            Assert.That(actualResult, Is.EqualTo(VSConstants.S_OK), "SetSite(serviceProvider) did not return S_OK");
        }

        [Test]
        public void CanResetSite()
        {
            // Arrange
            IVsPackage commandFactory = new CommandFactory();

            // Act
            int actualResult = commandFactory.SetSite(null);

            // Assert
            Assert.That(actualResult, Is.EqualTo(VSConstants.S_OK), "SetSite(null) did not return S_OK");
        }

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
    }
}