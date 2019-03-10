using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using EnvDTE;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Workbench
{
    [Guid(Symbols.ToolWindowIDs.XPathWorkbench)]
    internal sealed class XPathWorkbenchWindow : ToolWindowPane
    {
        public XPathWorkbenchWindow()
            : base(null)
        {
            Caption = PackageResources.XPathWorkbenchWindowTitle;
            var workbench = new XPathWorkbench(Registry.Current.Get<XmlRepository>(), Registry.Current.Get<SearchResultFactory>());
            workbench.SearchResultSelected += GoToSearchResult;
            Content = workbench;
        }

        public override bool SearchEnabled => true;

        private void GoToSearchResult(object sender, SearchResult searchResult)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if(!searchResult.LineNumber.HasValue)
                {
                    return;
                }

                if(!searchResult.LinePosition.HasValue)
                {
                    return;
                }

                var document = ActivateSourceDocument(searchResult.SourceDocument) ?? ActivateSourceFile(searchResult.SourceFile);
                if(document == null)
                {
                    var statusbar = (IVsStatusbar)GetService(typeof(IVsStatusbar));
                    statusbar?.SetText(PackageResources.XPathWorkbenchNavigationSourceActivationFailureText);
                    return;
                }

                var textSelection = (TextSelection)document.Selection;
                var lineNumber = searchResult.LineNumber.Value;
                var selectionStart = searchResult.LinePosition.Value;
                textSelection?.MoveTo(lineNumber, selectionStart, false);
                if(searchResult.SelectionLength.HasValue)
                {
                    var selectionEnd = selectionStart + searchResult.SelectionLength.Value;
                    textSelection?.MoveToLineAndOffset(lineNumber, selectionEnd, true);
                }
            }
            catch(Exception)
            {
                MessageBox.Show(PackageResources.XPathWorkbenchNavigationErrorText, PackageResources.XPathWorkbenchNavigationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private Document ActivateSourceDocument(Document sourceDocument)
        {
            if(sourceDocument == null)
            {
                return null;
            }

            try
            {
                sourceDocument.Activate();
                return sourceDocument;
            }
            catch
            {
                // The document window may have been closed after the search result was created.
                return null;
            }
        }

        private Document ActivateSourceFile(FileInfo sourceFile)
        {
            if(sourceFile == null)
            {
                return null;
            }

            if(!sourceFile.Exists)
            {
                // The file may have been moved, renamed or deleted after the search result was created.
                return null;
            }

            var dte = (DTE)GetService(typeof(DTE));

            if(dte.ItemOperations.IsFileOpen(sourceFile.FullName))
            {
                foreach(Document document in dte.Documents)
                {
                    if(string.Equals(document.FullName, sourceFile.FullName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        document.Activate();
                        return document;
                    }
                }
            }

            var window = dte.ItemOperations.OpenFile(sourceFile.FullName);
            return window.Document;
        }

        public override IVsSearchTask CreateSearch(uint dwCookie, IVsSearchQuery pSearchQuery, IVsSearchCallback pSearchCallback)
        {
            if(pSearchQuery == null || pSearchCallback == null)
            {
                return null;
            }

            return new XPathSearchTask(dwCookie, pSearchQuery, pSearchCallback, this);
        }

        public override void ClearSearch()
        {
            var control = (XPathWorkbench)Content;
            control.SearchResults.Clear();
            control.SearchResultText.Text = null;
        }

        public override void ProvideSearchSettings(IVsUIDataSource searchSettings)
        {
            Utilities.SetValue(searchSettings, SearchSettingsDataSource.SearchStartTypeProperty.Name, (uint)VSSEARCHSTARTTYPE.SST_ONDEMAND);

            Utilities.SetValue(searchSettings, SearchSettingsDataSource.RestartSearchIfUnchangedProperty.Name, true);

            Utilities.SetValue(searchSettings, SearchSettingsDataSource.SearchWatermarkProperty.Name, PackageResources.XPathWorkbenchSearchWatermark);
        }
    }
}
