using System.IO;
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

        public Document SourceDocument
        {
            get;
            set;
        }

        public FileInfo SourceFile
        {
            get;
            set;
        }
    }
}
