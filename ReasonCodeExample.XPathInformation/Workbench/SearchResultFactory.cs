using System.Collections.Generic;
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
            if(rawSearchResults is IEnumerable<XElement>)
            {
                var elements = (IEnumerable<XElement>)rawSearchResults;
                foreach(var element in elements)
                {
                    var searchResult = new ElementSearchResult {XPath = new AbsoluteXPathWriter().Write(element)};
                    searchResults.Add(searchResult);
                }
            }
            return searchResults;
        }
    }
}