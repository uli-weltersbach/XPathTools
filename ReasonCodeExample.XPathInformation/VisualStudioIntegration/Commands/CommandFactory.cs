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
            IMenuCommandService service = (IMenuCommandService)GetService(typeof(IMenuCommandService));

            CopyPathCommand copyPathCommand = new CopyPathCommand(Symbols.CommandIDs.CopyPath, new PathFormatter(), _repository);
            copyPathCommand.Register(service);

            CopyPathCommand copyAbsolutePathCommand = new CopyPathCommand(Symbols.CommandIDs.CopyAbsolutePath, new AbsolutePathFormatter(), _repository);
            copyAbsolutePathCommand.Register(service);

            CopyPathCommand copyDistinctPathCommand = new CopyPathCommand(Symbols.CommandIDs.CopyDistinctPath, new DistinctPathFormatter(), _repository);
            copyDistinctPathCommand.Register(service);
        }
    }
}