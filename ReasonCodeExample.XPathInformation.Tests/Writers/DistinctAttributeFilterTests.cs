using NUnit.Framework;
using ReasonCodeExample.XPathTools.Writers;

namespace ReasonCodeExample.XPathTools.Tests.Writers
{
    [TestFixture]
    public class DistinctAttributeFilterTests
    {
        [TestCase("<a><b /><b /><b /><b><c id='value' /><c id='other-value' /></b></a>", "/a/b/c[@id='value']")]
        [TestCase("<a><b /><b /><b /><b><c id='same' /><c id='same' /></b></a>", "")]
        [TestCase("<a><b /><b /><b /><b><c id='same' name='unique'/><c id='same' /></b></a>", "/a/b/c[@name='unique']")]
        [TestCase("<a><b /><b /><b /><b><c id='same' type='unique'/><c id='same' /></b></a>", "/a/b/c[@type='unique']")]
        [TestCase("<a><b id='same'><c id='same' /></b><b id='same'><c id='same' /></b><b id='unique'><c id='same' /></b></a>", "/a/b[@id='unique']/c")]
        [TestCase("<a><b id='same'><c id='same' /></b><b id='same'><c id='same' /></b><b id='unique'><c /><c /><c id='X' /></b></a>", "/a/b[@id='unique']/c[@id='X']")]
        [TestCase("<a:element xmlns:a=\"1\"><b:element xmlns:b=\"2\"/></a:element>", "/a:element/b:element")]
        [TestCase("<a><b id='1' name='1' type='1'/><b id='1' name='2' type='2'/></a>", "/a/b[@name='2']")]
        public void PreferredAttributeIsUsedInDistinctPath(string xml, string expectedXPath)
        {
            // Arrange
            var element = string.IsNullOrEmpty(expectedXPath) ? null : xml.SelectSingleNode(expectedXPath);
            var writer = CreateXPathWriter();

            // Act
            var actualXPath = writer.Write(element);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }

        private XPathWriter CreateXPathWriter()
        {
            var settings = new[]
                           {
                               new XPathSetting {AttributeName = "id"},
                               new XPathSetting {AttributeName = "name"},
                               new XPathSetting {AttributeName = "type"}
                           };
            var filter = new DistinctAttributeFilter(settings);
            return new XPathWriter(new[] { filter });
        }

        [TestCase("<a:element xmlns:a=\"1\" xmlns:b=\"2\"><b:element id=\"1\"/><b:element id=\"2\"/><b:element id=\"3\"/></a:element>", "/a:element/b:element[@id='3']")]
        public void PreferredAttributeIsNotDuplicatedInDistinctPath(string xml, string expectedXPath)
        {
            // Arrange
            var element = string.IsNullOrEmpty(expectedXPath) ? null : xml.SelectSingleNode(expectedXPath);
            var writer = CreateXPathWriter();

            // Act
            var actualXPath = writer.Write(element);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }

        [Test]
        public void ElementNamespaceFormatIsCorrect()
        {
            // Arrange
            var expectedXPath = "/configuration/runtime/*[local-name()='assemblyBinding'][namespace-uri()='urn:schemas-microsoft-com:asm.v1'][@id='1']";
            var xml = @"<configuration>
                            <runtime>
                                <assemblyBinding id='1' xmlns='urn:schemas-microsoft-com:asm.v1'>
                                    <child />
                                </assemblyBinding>
                                <assemblyBinding id='2' xmlns='urn:schemas-microsoft-com:asm.v1'>
                                    <child />
                                </assemblyBinding>
                            </runtime>
                        </configuration>";
            var element = xml.SelectSingleNode(expectedXPath);
            var writer = CreateXPathWriter();

            // Act
            var actualXPath = writer.Write(element);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }

        [TestCase("/runtime/*[local-name()='assemblyBinding'][namespace-uri()='urn:schemas-microsoft-com:asm.v1']/*[local-name()='dependentAssembly'][namespace-uri()='urn:schemas-microsoft-com:asm.v1']/*[local-name()='assemblyIdentity'][namespace-uri()='urn:schemas-microsoft-com:asm.v1'][@name='Lucene.Net']")]
        [TestCase("/runtime/*[local-name()='assemblyBinding'][namespace-uri()='urn:schemas-microsoft-com:asm.v1']/*[local-name()='dependentAssembly'][namespace-uri()='urn:schemas-microsoft-com:asm.v1']/*[local-name()='assemblyIdentity'][namespace-uri()='urn:schemas-microsoft-com:asm.v1'][@name='Newtonsoft.Json']")]
        [TestCase("/runtime/*[local-name()='assemblyBinding'][namespace-uri()='urn:schemas-microsoft-com:asm.v1']/*[local-name()='dependentAssembly'][namespace-uri()='urn:schemas-microsoft-com:asm.v1']/*[local-name()='assemblyIdentity'][namespace-uri()='urn:schemas-microsoft-com:asm.v1'][@name='System.Web.Mvc']")]
        [TestCase("/runtime/*[local-name()='assemblyBinding'][namespace-uri()='urn:schemas-microsoft-com:asm.v1']/*[local-name()='dependentAssembly'][namespace-uri()='urn:schemas-microsoft-com:asm.v1']/*[local-name()='assemblyIdentity'][namespace-uri()='urn:schemas-microsoft-com:asm.v1'][@name='System.Web.WebPages.Razor']")]
        public void PrefixlessNamespaceFormatIsCorrect(string expectedXPath)
        {
            // Arrange
            var xml = @"<runtime>
                          <assemblyBinding xmlns='urn:schemas-microsoft-com:asm.v1'>
                            <dependentAssembly>
                              <assemblyIdentity name='Lucene.Net' publicKeyToken='85089178b9ac3181' />
                              <bindingRedirect oldVersion='0.0.0.0-2.9.4.0' newVersion='3.0.3.0' />
                            </dependentAssembly>
                            <dependentAssembly>
                              <assemblyIdentity name='Newtonsoft.Json' publicKeyToken='30ad4fe6b2a6aeed' />
                              <bindingRedirect oldVersion='0.0.0.0-3.5.0.0' newVersion='4.5.0.0' />
                            </dependentAssembly>
                            <dependentAssembly>
                              <assemblyIdentity name='System.Web.Mvc' publicKeyToken='31bf3856ad364e35' xmlns='urn:schemas-microsoft-com:asm.v1' />
                              <bindingRedirect oldVersion='1.0.0.0-5.0.0.0' newVersion='5.1.0.0' xmlns='urn:schemas-microsoft-com:asm.v1' />
                            </dependentAssembly>
                            <dependentAssembly>
                              <assemblyIdentity name='System.Web.WebPages.Razor' publicKeyToken='31bf3856ad364e35' xmlns='urn:schemas-microsoft-com:asm.v1' />
                              <bindingRedirect oldVersion='1.0.0.0-3.0.0.0' newVersion='3.0.0.0' xmlns='urn:schemas-microsoft-com:asm.v1' />
                            </dependentAssembly>
                          </assemblyBinding>
                        </runtime>";
            var element = xml.SelectSingleNode(expectedXPath);
            var writer = CreateXPathWriter();

            // Act
            var actualXPath = writer.Write(element);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }
    }
}