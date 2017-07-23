using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ReasonCodeExample.XPathTools.VisualStudioIntegration
{
    public class SerializableConverter<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var s = (string) value;
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }

            var serializer = new XmlSerializer(typeof(T));
            var deserializedValue = serializer.Deserialize(new StringReader(s));
            return deserializedValue;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var serializer = new XmlSerializer(typeof(T));
            var content = new StringBuilder();
            using (var writer = new StringWriter(content))
            {
                serializer.Serialize(writer, value);
                var serialziedValue = writer.ToString();
                return serialziedValue;
            }
        }
    }
}