using EnvDTE;

namespace ReasonCodeExample.XPathTools.Workbench
{
    public class SearchResult
    {
        public string Value
        {
            get;
            set;
        }

        public int? LineNumber
        {
            get;
            set;
        }

        public int? LinePosition
        {
            get;
            set;
        }
        public int? SelectionLength
        {
            get;
            set;
        }

        public Document Source
        {
            get;
            set;
        }
    }
}
