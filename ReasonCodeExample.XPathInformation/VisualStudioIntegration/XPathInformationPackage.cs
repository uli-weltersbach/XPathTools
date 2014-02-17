using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathInformation.Formatters;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(SolutionExists)]
    [ProvideMenuResource(MenuResourceID, 1)]
    [Guid(Symbols.PackageID)]
    [ProvideOptionPageAttribute(typeof(ConfigurationPage), "XPath Information", "Configuration", 1000, 1001, true)]
    [ProvideProfileAttribute(typeof(ConfigurationPage), "XPath Information", "Configuration", 1002, 1003, true)]
    internal class XPathInformationPackage : Package
    {
        private const string SolutionExists = "{f1536ef8-92ec-443c-9ed7-fdadf150da82}";
        private const string MenuResourceID = "CommandFactory.ctmenu";
        private readonly XObjectRepository _repository;

        public static ConfigurationPage Configuration
        {
            get;
            private set;
        }

        public XPathInformationPackage()
            : this(new XObjectRepository())
        {
        }

        public XPathInformationPackage(XObjectRepository repository)
        {
            _repository = repository;
            //Configuration = (ConfigurationPage)GetDialogPage(typeof(ConfigurationPage));
        }

        protected override void Initialize()
        {
            base.Initialize();
            Initialize((IMenuCommandService)GetService(typeof(IMenuCommandService)));
        }

        public void Initialize(IMenuCommandService commandService)
        {
            if (commandService == null)
                throw new ArgumentNullException("commandService");
            CopyCommand copyGenericXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyGenericXPath, _repository, new GenericXPathFormatter());
            commandService.AddCommand(copyGenericXPathCommand);
            CopyCommand copyAbsoluteXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyAbsoluteXPath, _repository, new AbsoluteXPathFormatter());
            commandService.AddCommand(copyAbsoluteXPathCommand);
            CopyCommand copyDistinctXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyDistinctXPath, _repository, new DistinctXPathFormatter());
            commandService.AddCommand(copyDistinctXPathCommand);
            CopyCommand copyXmlStructureCommand = new CopyXmlStructureCommand(Symbols.CommandIDs.CopyXmlStructure, _repository);
            commandService.AddCommand(copyXmlStructureCommand);
        }
    }
}