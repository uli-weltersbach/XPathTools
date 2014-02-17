using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration
{
    internal class CommaSeparatedValueConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType.IsAssignableFrom(typeof(IEnumerable<string>));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType.IsAssignableFrom(typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = value as string;
            if (string.IsNullOrWhiteSpace(s))
                return Enumerable.Empty<string>();
            return s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(element => element.Trim()).Where(element => !string.IsNullOrWhiteSpace(element)).ToList();
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            IEnumerable<string> values = value as IEnumerable<string>;
            if (values == null)
                return string.Empty;
            return values.Aggregate((combined, current) => combined + ", " + current);
        }
    }
}