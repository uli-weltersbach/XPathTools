using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using EnvDTE;

namespace ReasonCodeExample.XPathTools.Workbench
{
    internal class SearchResultFactory
    {
        private readonly ActiveDocument _activeDocument;

        public SearchResultFactory(ActiveDocument activeDocument)
        {
            _activeDocument = activeDocument;
        }

        public IList<SearchResult> Parse(object xpathResult)
        {
            if(xpathResult == null)
            {
                return Enumerable.Empty<SearchResult>().ToList();
            }

            var source = _activeDocument.Current;

            if(xpathResult is IEnumerable)
            {
                return ParseEnumerableResult(xpathResult as IEnumerable, source).ToList();
            }

            return new[]
                   {
                       ParseSimpleResult(xpathResult, source)
                   };
        }

        private IEnumerable<SearchResult> ParseEnumerableResult(IEnumerable xpathResults, Document source)
        {
            var searchResults = new List<SearchResult>();
            foreach(var xpathResult in xpathResults)
            {
                var searchResult = TryParseElementSearchResult(xpathResult as XElement, source) ?? TryParseAttributeSearchResult(xpathResult as XAttribute, source);
                if(searchResult == null)
                {
                    searchResults.Add(ParseSimpleResult(xpathResult, source));
                }
                else
                {
                    searchResults.Add(searchResult);
                }
            }

            return searchResults;
        }

        private SearchResult TryParseElementSearchResult(XElement element, Document source)
        {
            if(element == null)
            {
                return null;
            }

            var searchResult = new SearchResult
                               {
                                   Value = element.ToString(SaveOptions.DisableFormatting),
                                   SelectionLength = element.Name.LocalName.Length,
                                   Source = source
                               };
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

        private SearchResult TryParseAttributeSearchResult(XAttribute attribute, Document source)
        {
            if(attribute == null)
            {
                return null;
            }

            var searchResult = new SearchResult
                               {
                                   Value = attribute.ToString(),
                                   SelectionLength = attribute.Name.LocalName.Length,
                                   Source = source
                               };
            SetLineInfo(attribute, searchResult);
            return searchResult;
        }

        private SearchResult ParseSimpleResult(object xpathResult, Document source)
        {
            return new SearchResult
                   {
                       Value = xpathResult.ToString(),
                       Source = source
                   };
        }
    }
}
