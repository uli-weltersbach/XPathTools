using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(SolutionExists)]
    [ProvideMenuResource(MenuResourceID, 1)]
    [Guid(Symbols.PackageID)]
    internal class CommandFactory : Package
    {
        private const string SolutionExists = "{f1536ef8-92ec-443c-9ed7-fdadf150da82}";
        private const string MenuResourceID = "CommandFactory.ctmenu";
        private readonly XPathRepository _repository;

        public CommandFactory()
            : this(new XPathRepository())
        {
        }

        public CommandFactory(XPathRepository repository)
        {
            _repository = repository;
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

            CopyXPathCommand copyXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyGenericXPath, new GenericXPathFormatter(), _repository);
            copyXPathCommand.Register(commandService);

            CopyXPathCommand copyAbsolutePathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyAbsoluteXPath, new AbsoluteXPathFormatter(), _repository);
            copyAbsolutePathCommand.Register(commandService);

            CopyXPathCommand copyDistinctPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyDistinctXPath, new DistinctXPathFormatter(), _repository);
            copyDistinctPathCommand.Register(commandService);
        }
    }
}