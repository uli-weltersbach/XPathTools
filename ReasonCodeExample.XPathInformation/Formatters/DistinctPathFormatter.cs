using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ReasonCodeExample.XPathInformation.Formatters
{
    internal class DistinctPathFormatter : PathFormatter
    {
        private readonly IList<string> _preferredAttributeCandidates = new[] { "id", "name", "type" };

        protected override string GetElementXPath(XElement element)
        {
            ICollection<IEnumerable<string>> variations = new List<IEnumerable<string>>();
            foreach (XElement ancestorOrSelf in element.AncestorsAndSelf())
            {
                variations.Add(GetPathVariations(ancestorOrSelf));
            }
            IEnumerable<string>[] cartesian = GetCartesianProduct(variations.Reverse()).ToArray();
            string[] paths = cartesian.Select(variation => variation.Aggregate(string.Empty, (s, path) => s + "/" + path)).ToArray();
            XDocument document = new XDocument(element.AncestorsAndSelf().Last());
            return paths.FirstOrDefault(path => document.XPathSelectElements(path).Count() == 1) ?? string.Empty;
        }

        private IEnumerable<XElement> GetSiblingsWithSameName(XElement element)
        {
            IEnumerable<XElement> before = element.ElementsBeforeSelf(element.Name);
            IEnumerable<XElement> after = element.ElementsAfterSelf(element.Name);
            List<XElement> combined = new List<XElement>(before);
            combined.AddRange(after);
            return combined;
        }

        private IEnumerable<string> GetPathVariations(XElement element)
        {
            List<string> variations = new List<string>();
            variations.Add(GetElementName(element));
            IEnumerable<XAttribute> attributeCandidates = GetAttributeCandidates(element);
            IEnumerable<XElement> siblings = GetSiblingsWithSameName(element);
            variations.AddRange(GetPathVariations(element, attributeCandidates, siblings));
            return variations;
        }

        private IEnumerable<XAttribute> GetAttributeCandidates(XElement element)
        {
            return element.Attributes().Where(attribute => _preferredAttributeCandidates.Contains(attribute.Name.LocalName, StringComparer.InvariantCultureIgnoreCase)).ToArray();
        }

        private IEnumerable<string> GetPathVariations(XElement element, IEnumerable<XAttribute> attributeCandidates, IEnumerable<XElement> siblings)
        {
            foreach (XAttribute attributeCandidate in attributeCandidates)
            {
                foreach (XElement sibling in siblings)
                {
                    if (HasDifferentAttributeValue(attributeCandidate, sibling))
                        return new[] { GetElementName(element) + Format(attributeCandidate) };
                }
            }
            return attributeCandidates.Select(attributeCandidate => GetElementName(element) + Format(attributeCandidate)).ToArray();
        }

        private bool HasDifferentAttributeValue(XAttribute attributeCandidate, XElement otherElement)
        {
            if (otherElement.Attribute(attributeCandidate.Name) == null)
                return true;
            return attributeCandidate.Value != otherElement.Attribute(attributeCandidate.Name).Value;
        }

        private IEnumerable<IEnumerable<T>> GetCartesianProduct<T>(IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyCartesianProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(emptyCartesianProduct, (accumulator, sequence) =>
                                                              from accumulatorSequence in accumulator
                                                              from item in sequence
                                                              select accumulatorSequence.Concat(new[] { item }));
        }

        public override string Format(XAttribute attribute)
        {
            if (attribute == null)
                return string.Empty;

            if (string.IsNullOrEmpty(attribute.Name.NamespaceName))
                return string.Format("[@{0}='{1}']", attribute.Name.LocalName, attribute.Value);

            if (attribute.Parent == null)
                throw new XmlException(string.Format("Unable to determine namespace prefix for attribute \"{0}\". Parent is null.", attribute.Name));

            string namespacePrefix = attribute.Parent.GetPrefixOfNamespace(attribute.Name.Namespace);
            if (string.IsNullOrEmpty(namespacePrefix))
                return string.Format("[@{0}='{1}']", attribute.Name.LocalName, attribute.Value);

            return string.Format("[@{0}:{1}='{2}']", namespacePrefix, attribute.Name.LocalName, attribute.Value);
        }
    }
}