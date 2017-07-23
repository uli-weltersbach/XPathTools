using System;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class XPathWriterFactory
    {
        private readonly IConfiguration _configuration;

        public XPathWriterFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IWriter CreateForXPathFormat(XPathFormat format)
        {
            return CreateForCommandId((int)format);
        }

        public IWriter CreateForCommandId(int xpathWriterCommandId)
        {
            switch(xpathWriterCommandId)
            {
                case Symbols.CommandIDs.CopyGenericXPath:
                    return new XPathWriter(new[] {new AttributeFilter(_configuration.AlwaysDisplayedAttributes)});

                case Symbols.CommandIDs.CopyAbsoluteXPath:
                    return new AbsoluteXPathWriter(new[] {new AttributeFilter(_configuration.AlwaysDisplayedAttributes)});

                case Symbols.CommandIDs.CopyDistinctXPath:
                    return new XPathWriter(new[] {new AttributeFilter(_configuration.AlwaysDisplayedAttributes), new DistinctAttributeFilter(_configuration.PreferredAttributeCandidates)});

                case Symbols.CommandIDs.CopySimplifiedXPath:
                    return new SimplifiedXPathWriter(new[] {new AttributeFilter(_configuration.AlwaysDisplayedAttributes)});

                default:
                    throw new ArgumentException($"Unsupported XPath writer ID '{xpathWriterCommandId}'.");
            }
        }
    }
}