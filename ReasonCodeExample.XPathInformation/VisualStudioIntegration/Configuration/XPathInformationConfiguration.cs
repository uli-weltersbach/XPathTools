using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using Microsoft.VisualStudio.Shell;
using Wexman.Design;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration
{
    internal class XPathInformationConfiguration : DialogPage, IConfiguration
    {
        public XPathInformationConfiguration()
        {
            InnerIncludedAttributes = new Collection<string>();
            InnerExcludedAttributes = new Collection<string>();
            PreferredAttributeCandidateNames = new Collection<string>();
            AlwaysIncludedAttributes = new Dictionary<string, string>();
            SetPropertyDefaultValues();
        }

        [Browsable(false)]
        public static IConfiguration Current
        {
            get;
            set;
        }

        [TypeConverter(typeof (CollectionConverter))]
        [DisplayName("Included attributes")]
        [Description("")]
        public Collection<string> InnerIncludedAttributes
        {
            get;
            set;
        }

        [TypeConverter(typeof (CollectionConverter))]
        [DisplayName("Excluded attributes")]
        [Description("")]
        public Collection<string> InnerExcludedAttributes
        {
            get;
            set;
        }

        [Editor(typeof (GenericDictionaryEditor<string, string>), typeof (UITypeEditor))]
        [GenericDictionaryEditor(Title = "Always visible attributes", KeyDisplayName = "Element name", ValueDisplayName = "Attribute name")]
        [Description("Use this option to specify attributes which should always be included in the XPath of certain elements.")]
        public Dictionary<string, string> AlwaysIncludedAttributes
        {
            get;
            set;
        }

        [Category("Attributes")]
        [DisplayName("Show attribute XPath")]
        [Description("Determines whether to show the XPath to the attribute itself or to the element containing it when an attribute is at the caret position. E.g. \"/a/b/c/@id\" vs. \"/a/b/c[@id]\".")]
        public bool ShowAttributeXPath
        {
            get;
            set;
        }

        [Browsable(false)]
        public IList<string> IncludedAttributes
        {
            get { return InnerIncludedAttributes; }
        }

        [Browsable(false)]
        public IList<string> ExcludedAttributes
        {
            get { return InnerExcludedAttributes; }
        }

        [Browsable(false)]
        public IList<string> PreferredAttributeCandidateNames
        {
            get;
            set;
        }

        private void SetPropertyDefaultValues()
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                DefaultValueAttribute defaultValue = property.GetCustomAttribute<DefaultValueAttribute>();
                if (defaultValue != null)
                    property.SetValue(this, defaultValue.Value);
            }
        }
    }
}