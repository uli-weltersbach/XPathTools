using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration
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
            string s = (string) value;
            if (string.IsNullOrWhiteSpace(s))
                return null;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            object deserializedValue = serializer.Deserialize(new StringReader(s));
            return deserializedValue;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringBuilder content = new StringBuilder();
            using (StringWriter writer = new StringWriter(content))
            {
                serializer.Serialize(writer, value);
                string serialziedValue = writer.ToString();
                return serialziedValue;
            }
        }
    }
}