using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Formatters
{
    internal class DistinctPathFormatter : PathFormatter
    {
        private readonly IList<string> _preferredAttributeCandidates = new[] {"id", "name", "type"};

        protected override string GetElementXPath(XElement element)
        {
            IEnumerable<XElement> siblings = GetSiblingsWithSameName(element);
            if (!siblings.Any())
                return base.GetElementXPath(element);

            ICollection<IEnumerable<string>> variations = new List<IEnumerable<string>>();
            foreach (XElement ancestorOrSelf in element.AncestorsAndSelf())
            {
                variations.Add(GetPathVariations(ancestorOrSelf));
            }
            IEnumerable<string>[] cartesian = GetCartesianProduct(variations.Reverse()).ToArray();
            string[] paths = cartesian.Select(variation => variation.Aggregate(string.Empty, (s, path) => s + "/" + path)).ToArray();
            return paths.FirstOrDefault();
        }

        private IEnumerable<string> GetPathVariations(XElement element)
        {
            IEnumerable<XElement> siblings = GetSiblingsWithSameName(element);
            IEnumerable<XAttribute> attributeCandidates = GetAttributeCandidates(element);
            if (siblings.Any() && attributeCandidates.Any())
                return GetPathVariations(element, attributeCandidates, siblings);
            return new[] {GetElementName(element)};
        }

        private IEnumerable<string> GetPathVariations(XElement element, IEnumerable<XAttribute> attributeCandidates, IEnumerable<XElement> siblings)
        {
            foreach (XAttribute attributeCandidate in attributeCandidates)
            {
                foreach (XElement sibling in siblings)
                {
                    if (HasSameAttributeValue(element, sibling, attributeCandidate))
                        continue;
                    return new[] {GetElementName(element) + Format(attributeCandidate)};
                }
            }
            return attributeCandidates.Select(attributeCandidate => GetElementName(element) + Format(attributeCandidate)).ToArray();
        }

        private IEnumerable<IEnumerable<T>> GetCartesianProduct<T>(IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyCartesianProduct = new[] {Enumerable.Empty<T>()};
            return sequences.Aggregate(emptyCartesianProduct, (accumulator, sequence) =>
                                                              from accumulatorSequence in accumulator
                                                              from item in sequence
                                                              select accumulatorSequence.Concat(new[] {item}));
        }

        private bool HasSameAttributeValue(XElement element, XElement otherElement, XAttribute attribute)
        {
            if (element.Attribute(attribute.Name) == null)
                return false;
            if (otherElement.Attribute(attribute.Name) == null)
                return false;
            return element.Attribute(attribute.Name).Value == otherElement.Attribute(attribute.Name).Value;
        }

        private IEnumerable<XAttribute> GetAttributeCandidates(XElement element)
        {
            return element.Attributes().Where(attribute => _preferredAttributeCandidates.Contains(attribute.Name.LocalName, StringComparer.InvariantCultureIgnoreCase)).ToArray();
        }

        private IEnumerable<XElement> GetSiblingsWithSameName(XElement element)
        {
            IEnumerable<XElement> before = element.ElementsBeforeSelf(element.Name);
            IEnumerable<XElement> after = element.ElementsAfterSelf(element.Name);
            List<XElement> combined = new List<XElement>(before);
            combined.AddRange(after);
            return combined;
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