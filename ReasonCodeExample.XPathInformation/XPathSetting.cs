using System.ComponentModel;
using System.Text;

namespace ReasonCodeExample.XPathInformation
{
    internal class XPathSetting
    {
        [DisplayName("Element name")]
        public string ElementName
        {
            get;
            set;
        }

        [DisplayName("Element XML namespace")]
        public string ElementNamespace
        {
            get;
            set;
        }

        [DisplayName("Attribute name")]
        public string AttributeName
        {
            get;
            set;
        }

        [DisplayName("Attribute XML namespace")]
        public string AttributeNamespace
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            if (!string.IsNullOrEmpty(ElementNamespace))
                text.AppendFormat("{{{0}}}", ElementNamespace);
            if (!string.IsNullOrEmpty(ElementName))
                text.Append(ElementName);
            if (!string.IsNullOrEmpty(AttributeNamespace) || !string.IsNullOrEmpty(AttributeName))
                text.Append("/@");
            if (!string.IsNullOrEmpty(AttributeNamespace))
                text.AppendFormat("{{{0}}}", AttributeNamespace);
            if (!string.IsNullOrEmpty(AttributeName))
                text.Append(AttributeName);
            return text.ToString();
        }
    }
}