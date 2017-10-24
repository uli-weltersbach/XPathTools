using System.Xml.Linq;
using NUnit.Framework;
using ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;
using ReasonCodeExample.XPathTools.Workbench;
using System.Threading;

namespace ReasonCodeExample.XPathTools.Tests.Workbench
{
    [TestFixture]
    [Category("Integration")]
    public class XPathWorkbenchTests
    {
        private readonly VisualStudioExperimentalInstance _instance = new VisualStudioExperimentalInstance();

        [OneTimeSetUp]
        public void StartVisualStudio()
        {
            //_instance.ReStart(VisualStudioVersion.VS2015);
        }

        [OneTimeTearDown]
        public void StopVisualStudio()
        {
            //_instance.Stop();
        }

        [Test]
        public void WorkbenchIsActivatedViaContextMenu()
        {
            // Arrange
            var xml = new XElement("xml");
            _instance.OpenXmlFile(xml.ToString(), 0);

            // Act
            _instance.ClickContextMenuEntry(PackageResources.ShowXPathWorkbenchCommandText);
            var xpathWorkbench = new XPathWorkbenchAutomationModel(_instance.MainWindow);

            // Assert
            Assert.That(xpathWorkbench.IsVisible, Is.True);
        }

        [Test]
        public void WorkbenchRunsQueryEvenThoughNoNodeIsSelected()
        {
            // Arrange
            var xml = new XElement("xml");
            _instance.OpenXmlFile(xml.ToString(), 0);

            // Act
            _instance.ClickContextMenuEntry(PackageResources.ShowXPathWorkbenchCommandText);
            var xpathWorkbench = new XPathWorkbenchAutomationModel(_instance.MainWindow);
            xpathWorkbench.Search("§ invalid XPath §");

            // Assert
            Assert.That(xpathWorkbench.SearchResultText, Does.Contain(PackageResources.XPathEvaluationErrorText));
        }

        [Test]
        public void WorkbenchShowsSearchResultCount()
        {
            // Arrange
            var xml = new XElement("xml");
            _instance.OpenXmlFile(xml.ToString(SaveOptions.DisableFormatting), 0);
            _instance.ClickContextMenuEntry(PackageResources.ShowXPathWorkbenchCommandText);
            var xpathWorkbench = new XPathWorkbenchAutomationModel(_instance.MainWindow);
            var expectedResultText = string.Format(PackageResources.SingleResultText, 1);

            // Act
            xpathWorkbench.Search("/xml");

            // Assert
            Assert.That(xpathWorkbench.SearchResultText, Does.Contain(expectedResultText));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void WorkbenchHandlesXmlNamespaces()
        {
            // Arrange
            var xml = @"<configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                            <runtime>
                                <assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"" xmlns:urn=""urn:schemas-microsoft-com:asm.v1"">
                                    <dependentAssembly xdt:Transform=""Replace"" xdt:Locator=""XPath(/configuration/runtime//*[local-name()='assemblyIdentity' and @name='Newtonsoft.Json']/..)"">
                                        <assemblyIdentity name=""Newtonsoft.Json"" publicKeyToken=""30ad4fe6b2a6aeed"" />
                                        <bindingRedirect oldVersion=""0.0.0.0-6.0.0.0"" newVersion=""6.0.0.0"" />
                                    </dependentAssembly>
                                </assemblyBinding>
                            </runtime>
                        </configuration>";
            var repository = new XmlRepository();
            repository.LoadXml(xml, null);
            var workbench = new XPathWorkbench(repository, new SearchResultFactory());

            // Act
            var results = workbench.Search("/configuration/runtime/urn:assemblyBinding/urn:dependentAssembly/urn:assemblyIdentity[@name='Newtonsoft.Json']");

            // Assert
            Assert.That(results.Count, Is.EqualTo(1));
        }
    }
}