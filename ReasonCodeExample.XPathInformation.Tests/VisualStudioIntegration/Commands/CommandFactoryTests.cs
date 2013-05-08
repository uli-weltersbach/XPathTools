using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration.Commands
{
    [TestFixture]
    public class CommandFactoryTests
    {
        private const int S_OK = 0;

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
            Assert.That(S_OK, Is.EqualTo(actualResult), "SetSite(serviceProvider) did not return S_OK");
        }

        [Test]
        public void CanResetSite()
        {
            // Arrange
            IVsPackage commandFactory = new CommandFactory();
            
            // Act
            int actualResult = commandFactory.SetSite(null);

            // Assert
            Assert.That(S_OK, Is.EqualTo(actualResult), "SetSite(null) did not return S_OK");
        }
    }
}