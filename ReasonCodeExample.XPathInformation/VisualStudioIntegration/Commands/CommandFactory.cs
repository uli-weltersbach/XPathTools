using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource("CommandFactory.ctmenu", 1)]
    [Guid(Symbols.PackageID)]
    internal class CommandFactory : Package
    {
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
            IMenuCommandService service = (IMenuCommandService) GetService(typeof (IMenuCommandService));

            CopyCommand copyPathCommand = new CopyCommand(Symbols.CommandIDs.CopyPath, new PathFormatter(), _repository);
            copyPathCommand.Register(service);

            CopyCommand copyAbsolutePathCommand = new CopyCommand(Symbols.CommandIDs.CopyAbsolutePath, new AbsolutePathFormatter(), _repository);
            copyAbsolutePathCommand.Register(service);
        }
    }
}