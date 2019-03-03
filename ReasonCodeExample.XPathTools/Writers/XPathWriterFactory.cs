using System;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Writers
{
    internal class XPathWriterFactory
    {
        private readonly Func<IConfiguration> _configurationProvider;

        public XPathWriterFactory(Func<IConfiguration> configurationProvider)
        {
            _configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
        }

        public IWriter CreateForXPathFormat(XPathFormat format)
        {
            return CreateForCommandId((int)format);
        }

        public IWriter CreateForCommandId(int xpathWriterCommandId)
        {
            var configuration = _configurationProvider();
            switch(xpathWriterCommandId)
            {
                case Symbols.CommandIDs.CopyGenericXPath:
                    return new XPathWriter(new[] {new AttributeFilter(configuration.AlwaysDisplayedAttributes)});

                case Symbols.CommandIDs.CopyAbsoluteXPath:
                    return new AbsoluteXPathWriter(new[] {new AttributeFilter(configuration.AlwaysDisplayedAttributes)});

                case Symbols.CommandIDs.CopyDistinctXPath:
                    return new XPathWriter(new[] {new AttributeFilter(configuration.AlwaysDisplayedAttributes), new DistinctAttributeFilter(configuration.PreferredAttributeCandidates)});

                case Symbols.CommandIDs.CopySimplifiedXPath:
                    return new SimplifiedXPathWriter(new[] {new AttributeFilter(configuration.AlwaysDisplayedAttributes)});

                default:
                    throw new ArgumentException($"Unsupported XPath writer ID '{xpathWriterCommandId}'.");
            }
        }
    }
}
