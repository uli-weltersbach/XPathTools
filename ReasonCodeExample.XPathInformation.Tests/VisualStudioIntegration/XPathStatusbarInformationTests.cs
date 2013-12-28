using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    [TestFixture]
    public class XPathStatusbarInformationTests
    {
        private readonly VisualStudioExperimentalInstance _instance = new VisualStudioExperimentalInstance();

        [TestFixtureSetUp]
        public void StartVisualStudio()
        {
            _instance.ReStart();
            _instance.WaitUntillStarted();
        }

        [TestFixtureTearDown]
        public void StopVisualStudio()
        {
            _instance.Stop();
        }

        [Test]
        public void StatusbarShowsXPath()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(_instance.MainWindow, Is.Not.Null);
        }
    }
}