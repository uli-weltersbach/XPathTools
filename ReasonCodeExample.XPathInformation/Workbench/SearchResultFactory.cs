using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Workbench
{
    internal class SearchResultFactory
    {
        public IEnumerable<ElementSearchResult> Parse(object rawSearchResults)
        {
            var searchResults = new List<ElementSearchResult>();
            if(rawSearchResults == null)
            {
                return searchResults;
            }
            if(rawSearchResults is IEnumerable)
            {
                foreach(var element in (IEnumerable)rawSearchResults)
                {
                    var searchResult = TryParseElementSearchResult(element as XElement);
                    if(searchResult != null)
                    {
                        searchResults.Add(searchResult);
                    }
                }
            }
            return searchResults;
        }

        private static ElementSearchResult TryParseElementSearchResult(XElement element)
        {
            if(element == null)
            {
                return null;
            }
            var lineInfo = element as IXmlLineInfo;
            var searchResult = new ElementSearchResult {XPath = new AbsoluteXPathWriter().Write(element)};
            if(lineInfo.HasLineInfo())
            {
                searchResult.LineNumber = lineInfo.LineNumber;
                searchResult.LinePosition = lineInfo.LinePosition;
            }
            return searchResult;
        }
    }
}