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

        public IWriter CreateFromFriendlyName(string xpathWriterFriendlyName)
        {
            switch(xpathWriterFriendlyName)
            {
                case "Generic":
                    return CreateFromCommandId(Symbols.CommandIDs.CopyGenericXPath);

                case "Absolute":
                    return CreateFromCommandId(Symbols.CommandIDs.CopyAbsoluteXPath);

                case "Distinct":
                    return CreateFromCommandId(Symbols.CommandIDs.CopyDistinctXPath);

                case "Simplified":
                    return CreateFromCommandId(Symbols.CommandIDs.CopySimplifiedXPath);

                default:
                    throw new ArgumentException($"Unsupported XPath writer friendly name '{xpathWriterFriendlyName}'.");
            }
        }

        public IWriter CreateFromCommandId(int xpathWriterCommandId)
        {
            switch(xpathWriterCommandId)
            {
                case Symbols.CommandIDs.CopyGenericXPath:
                    return new XPathWriter(new[] {new AttributeFilter(_configuration.AlwaysDisplayedAttributes)});

                case Symbols.CommandIDs.CopyAbsoluteXPath:
                    return new AbsoluteXPathWriter(new[] {new AttributeFilter(_configuration.AlwaysDisplayedAttributes)});

                case Symbols.CommandIDs.CopyDistinctXPath:
                    return new XPathWriter(new[] { new AttributeFilter(_configuration.AlwaysDisplayedAttributes), new DistinctAttributeFilter(_configuration.PreferredAttributeCandidates) });

                case Symbols.CommandIDs.CopySimplifiedXPath:
                    return new SimplifiedXPathWriter(new[] {new AttributeFilter(_configuration.AlwaysDisplayedAttributes)});

                default:
                    throw new ArgumentException($"Unsupported XPath writer ID '{xpathWriterCommandId}'.");
            }
        }
    }
}