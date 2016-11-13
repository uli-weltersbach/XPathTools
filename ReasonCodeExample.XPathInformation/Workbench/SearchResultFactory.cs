using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Workbench
{
    internal class SearchResultFactory
    {
        public IEnumerable<SearchResult> Parse(object rawSearchResults)
        {
            var searchResults = new List<SearchResult>();
            if(rawSearchResults == null)
            {
                return searchResults;
            }
            if(rawSearchResults is IEnumerable)
            {
                foreach(var element in (IEnumerable)rawSearchResults)
                {
                    var searchResult = TryParseElementSearchResult(element as XElement) ?? TryParseAttributeSearchResult(element as XAttribute);
                    if(searchResult != null)
                    {
                        searchResults.Add(searchResult);
                    }
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
            var searchResult = new SearchResult {Xml = element.ToString(SaveOptions.None)};
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
            var searchResult = new SearchResult {Xml = attribute.ToString()};
            SetLineInfo(attribute, searchResult);
            return searchResult;
        }
    }
}