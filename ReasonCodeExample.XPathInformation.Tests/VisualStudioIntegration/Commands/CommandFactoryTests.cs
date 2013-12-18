using Microsoft.VisualStudio.Shell.Interop;
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
    }
}