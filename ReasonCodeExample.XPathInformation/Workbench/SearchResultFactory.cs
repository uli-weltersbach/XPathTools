using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Workbench
{
    internal class SearchResultFactory
    {
        public IEnumerable<SearchResult> Parse(object xpathResult)
        {
            if(xpathResult == null)
            {
                return Enumerable.Empty<SearchResult>();
            }

            if(xpathResult is IEnumerable)
            {
                return ParseEnumerableResult(xpathResult as IEnumerable);
            }

            return new[]
                   {
                       ParseSimpleResult(xpathResult)
                   };
        }

        private IEnumerable<SearchResult> ParseEnumerableResult(IEnumerable xpathResults)
        {
            var searchResults = new List<SearchResult>();
            foreach(var xpathResult in xpathResults)
            {
                var searchResult = TryParseElementSearchResult(xpathResult as XElement) ?? TryParseAttributeSearchResult(xpathResult as XAttribute);
                if(searchResult == null)
                {
                    searchResults.Add(ParseSimpleResult(xpathResult));
                }
                else
                {
                    searchResults.Add(searchResult);
                }
            }
            return searchResults;
        }

        private SearchResult TryParseElementSearchResult(XElement element)
        {
            if(element == null)
            {
                return null;
            }
            var searchResult = new SearchResult {Value = element.ToString(SaveOptions.None)};
            SetLineInfo(element, searchResult);
            return searchResult;
        }

        private void SetLineInfo(IXmlLineInfo lineInfo, SearchResult searchResult)
        {
            if(lineInfo.HasLineInfo())
            {
                searchResult.LineNumber = lineInfo.LineNumber;
                searchResult.LinePosition = lineInfo.LinePosition;
            }
        }

        private SearchResult TryParseAttributeSearchResult(XAttribute attribute)
        {
            if(attribute == null)
            {
                return null;
            }
            var searchResult = new SearchResult {Value = attribute.ToString()};
            SetLineInfo(attribute, searchResult);
            return searchResult;
        }

        private SearchResult ParseSimpleResult(object xpathResult)
        {
            return new SearchResult { Value = xpathResult.ToString() };
        }
    }
}