using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration;

namespace ReasonCodeExample.XPathInformation.Formatters
{
    internal class XmlAttributeFilter
    {
        private readonly IConfiguration _configuration;

        public XmlAttributeFilter(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            _configuration = configuration;
        }

        public void ApplyTo(XObject xobject)
        {
            if (xobject == null)
                return;
            if (xobject.Document == null)
                return;
            ApplyTo(xobject.Document);
        }

        private void ApplyTo(XDocument document)
        {
            List<XAttribute> attributes = document.Descendants().SelectMany(element => element.Attributes()).ToList();
            if (_configuration.ExcludedAttributes.Any())
                RemoveExcludedAttributes(attributes);
            if (_configuration.IncludedAttributes.Any())
                RemoveNonIncludedAttributes(attributes);
        }

        private void RemoveExcludedAttributes(IEnumerable<XAttribute> attributes)
        {
            foreach (XAttribute attribute in attributes)
            {
                if (_configuration.ExcludedAttributes.Contains(attribute.Name.LocalName, StringComparer.InvariantCultureIgnoreCase))
                    attribute.Remove();
            }
        }

        private void RemoveNonIncludedAttributes(IEnumerable<XAttribute> attributes)
        {
            foreach (XAttribute attribute in attributes)
            {
                if (!_configuration.IncludedAttributes.Contains(attribute.Name.LocalName, StringComparer.InvariantCultureIgnoreCase))
                    attribute.Remove();
            }
        }
    }
}