using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [Test]
        public void StatusbarShowsXPath()
        {
            // Arrange


            // Act


            // Assert
            Assert.That(_instance.MainWindow, Is.Not.Null);
        }

        [TestFixtureTearDown]
        public void StopVisualStudio()
        {
            _instance.Stop();
        }
    }
}
