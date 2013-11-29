using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.Tests.Formatters
{
    [TestFixture]
    public class DistinctPathFormatterTests
    {
        private readonly IPathFormatter _formatter = new DistinctPathFormatter();

        [TestCase("<a><b /><b /><b /><b><c id='value' /><c id='other-value' /></b></a>", 5, "/a/b/c[@id='value']")]
        [TestCase("<a><b /><b /><b /><b><c id='same' /><c id='same' /></b></a>", 5, "")]
        [TestCase("<a><b /><b /><b /><b><c id='same' name='unique'/><c id='same' /></b></a>", 5, "/a/b/c[@name='unique']")]
        [TestCase("<a><b /><b /><b /><b><c id='same' type='unique'/><c id='same' /></b></a>", 5, "/a/b/c[@type='unique']")]
        [TestCase("<a><b id='same'><c id='same' /></b><b id='same'><c id='same' /></b><b id='unique'><c id='same' /></b></a>", 6, "/a/b[@id='unique']/c")]
        public void PreferredAttributeIsUsedInDistinctPath(string xml, int testElementIndex, string expectedXPath)
        {
            // Arrange
            XElement element = XElement.Parse(xml);
            XObject testElement = element.DescendantNodesAndSelf().ElementAt(testElementIndex);

            // Act
            string actualXPath = _formatter.Format(testElement);

            // Assert
            Assert.That(actualXPath, Is.EqualTo(expectedXPath));
        }

        [Test]
        public void AttributeIsNotUsedInDistinctPath()
        {
            // Arrange
            XElement element = XElement.Parse(@"<configuration>
	                                                <runtime>
                                                        <assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"">
                                                            <dependentAssembly>
                                                            <assemblyIdentity name=""System.Web.Extensions"" publicKeyToken=""31bf3856ad364e35""/>
                                                            <bindingRedirect oldVersion=""1.0.0.0-1.1.0.0"" newVersion=""3.5.0.0""/>
                                                            </dependentAssembly>
                                                            <dependentAssembly>
                                                            <assemblyIdentity name=""System.Web.Extensions.Design"" publicKeyToken=""31bf3856ad364e35""/>
                                                            <bindingRedirect oldVersion=""1.0.0.0-1.1.0.0"" newVersion=""3.5.0.0""/>
                                                            </dependentAssembly>
                                                            <dependentAssembly>
                                                            <assemblyIdentity name=""Lucene.Net"" publicKeyToken=""85089178b9ac3181""/>
                                                            <bindingRedirect oldVersion=""0.0.0.0-2.9.4.0"" newVersion=""2.9.4.1""/>
                                                            </dependentAssembly>
                                                        </assemblyBinding>
                                                    </runtime>
                                                </configuration>");
            XObject selectedAttribute = element.Element("runtime").Elements().ElementAt(0).FirstAttribute;

            // Act
            string actualXPath = _formatter.Format(selectedAttribute);

            // Assert
            Assert.That(actualXPath, Is.Not.Null.And.Empty);
        }
    }
}