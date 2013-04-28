using System;
using System.Xml.Linq;
using NUnit.Framework;

namespace ReasonCodeExample.XPathInformation.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public void Document()
        {
            // Arrange
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
                         @" <GitSccOptions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" + Environment.NewLine +
                         @" <GitBashPath>C:\Program Files (x86)\Git\bin\sh.exe</GitBashPath>" + Environment.NewLine +
                         "  <GitExtensionPath />" + Environment.NewLine +
                         "  <node />  </GitSccOptions>";

            // Act
            string actualXPath = Parse(xml, 5, 8);

            // Assert
            Assert.That(actualXPath, Is.EqualTo("/GitSccOptions/node"));
        }

        [Test]
        public void DocumentWithComments()
        {
            // Arrange
            string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>" + Environment.NewLine +
                         @"<!-- <node> -->" + Environment.NewLine +
                         @"<GitSccOptions xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">" + Environment.NewLine +
                         @"<GitBashPath>C:\Program Files (x86)\Git\bin\sh.exe</GitBashPath>" + Environment.NewLine +
                         @"<!-- <node>" + Environment.NewLine +
                         "<GitExtensionPath />  -->" + Environment.NewLine +
                         "<node /></GitSccOptions>";

            // Act
            string actualXPath = Parse(xml, 7, 2);

            // Assert
            Assert.That(actualXPath, Is.EqualTo("/GitSccOptions/node"));
        }

        private string Parse(string xml, int lineNumber, int linePosition)
        {
            XElement rootElement = XElement.Parse(xml, LoadOptions.SetLineInfo);
            XmlNodeRepository repository = new XmlNodeRepository();
            XElement selectedElement = repository.GetElement(rootElement, lineNumber, linePosition);
            XAttribute selectedAttribute = repository.GetAttribute(selectedElement, linePosition);
            XPathFormatter formatter = new XPathFormatter();
            return formatter.Format(selectedElement) + formatter.Format(selectedAttribute);
        }
    }
}