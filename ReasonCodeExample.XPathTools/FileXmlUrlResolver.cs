using System;
using System.Threading.Tasks;
using System.Xml;

namespace ReasonCodeExample.XPathTools
{
    /// <summary>
    /// A <c>XmlUrlResolver</c> which supports *only* file lookup.
    /// This avoids timeouts due to slow or blocked HTTP requests, 
    /// while still solving all currently supported use cases - 
    /// presumably because XML validation isn't currently on the list of use cases.
    /// </summary>
    /// <remarks>
    /// See the following resources for further discussion:
    /// https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlurlresolver?view=netframework-4.7.2
    /// https://stackoverflow.com/questions/2558021/an-error-has-occurred-opening-extern-dtd-w3-org-xhtml1-transitional-dtd-503
    /// https://msdn.microsoft.com/en-us/library/aa302284.aspx
    /// </remarks>
    internal class FileXmlUrlResolver : XmlUrlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri == null)
            {
                throw new ArgumentNullException(nameof(absoluteUri));
            }
            if (absoluteUri.Scheme.StartsWith("file", StringComparison.InvariantCultureIgnoreCase))
            {
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }
            throw new NotSupportedException($"URI scheme \"{absoluteUri.Scheme}\" isn't supported by {nameof(FileXmlUrlResolver)}.");
        }

        public override Task<object> GetEntityAsync(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            return new Task<object>(() => GetEntity(absoluteUri, role, ofObjectToReturn));
        }
    }
}
