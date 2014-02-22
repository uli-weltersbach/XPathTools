using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration
{
    internal class XPathInformationConfiguration : DialogPage, IConfiguration
    {
        public static IConfiguration Current
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

        [Category("Attributes")]
        [DisplayName("Included attributes")]
        [Description("Attributes to include in XPaths, separated by commas.")]
        [TypeConverter(typeof(CommaSeparatedValueConverter))]
        public IList<string> IncludedAttributes
        {
            get;
            set;
        }

        [Category("Attributes")]
        [DisplayName("Excluded attributes")]
        [Description("Attributes to exclude from XPaths, separated by commas.")]
        [TypeConverter(typeof(CommaSeparatedValueConverter))]
        public IList<string> ExcludedAttributes
        {
            get;
            set;
        }

        [Category("Distinct XPath")]
        [DisplayName("Preferred attribute candidate names, separated by commas.")]
        [Description("Comma separated list of local attribute names to be used when attempting to determine a distinct XPath to an element.")]
        [DefaultValue("id, name, type")]
        [TypeConverter(typeof(CommaSeparatedValueConverter))]
        public IList<string> PreferredAttributeCandidateNames
        {
            get;
            set;
        }
    }
}