using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathTools.Writers
{
    internal class AttributeFilter : IAttributeFilter
    {
        public AttributeFilter(IEnumerable<XPathSetting> settings)
        {
            if(settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            Settings = settings;
        }

        protected IEnumerable<XPathSetting> Settings
        {
            get;
            private set;
        }

        public virtual bool IsIncluded(XAttribute attribute)
        {
            if (attribute?.Parent == null)
            {
                return false;
            }
            return Settings.Any(setting => IsMatch(setting, attribute) && IsMatch(setting, attribute.Parent));
        }

        private bool IsMatch(XPathSetting setting, XAttribute attribute)
        {
            if(string.IsNullOrEmpty(setting.AttributeName) && string.IsNullOrEmpty(setting.AttributeNamespace))
            {
                return true;
            }
            if(string.IsNullOrEmpty(setting.AttributeName))
            {
                return attribute.Name.NamespaceName == setting.AttributeNamespace;
            }
            if(string.IsNullOrEmpty(setting.AttributeNamespace))
            {
                return attribute.Name.LocalName == setting.AttributeName;
            }
            return attribute.Name == XName.Get(setting.AttributeName, setting.AttributeNamespace);
        }

        private bool IsMatch(XPathSetting setting, XElement element)
        {
            if(string.IsNullOrEmpty(setting.ElementName) && string.IsNullOrEmpty(setting.ElementNamespace))
            {
                return true;
            }
            if(string.IsNullOrEmpty(setting.ElementName))
            {
                return element.Name.NamespaceName == setting.ElementNamespace;
            }
            if(string.IsNullOrEmpty(setting.ElementNamespace))
            {
                return element.Name.LocalName == setting.ElementName;
            }
            return element.Name == XName.Get(setting.ElementName, setting.ElementNamespace);
        }
    }
}