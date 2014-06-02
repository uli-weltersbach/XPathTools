﻿using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(SolutionExists)]
    [ProvideMenuResource(MenuResourceID, 1)]
    [Guid(Symbols.PackageID)]
    [ProvideOptionPage(typeof(XPathInformationConfiguration), "XPath Information", "General", 0, 0, true)]
    internal class XPathInformationPackage : Package
    {
        private const string SolutionExists = "{f1536ef8-92ec-443c-9ed7-fdadf150da82}";
        private const string MenuResourceID = "CommandFactory.ctmenu";
        private readonly XmlNodeRepository _repository;

        public XPathInformationPackage()
            : this(new XmlNodeRepository())
        {
        }

        public XPathInformationPackage(XmlNodeRepository repository)
        {
            _repository = repository;
        }

        protected override void Initialize()
        {
            try
            {
                base.Initialize();
                var menuCommandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
                XPathInformationConfiguration.Current = (XPathInformationConfiguration)GetDialogPage(typeof(XPathInformationConfiguration));
                Initialize(menuCommandService);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error in " + GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Initialize(IMenuCommandService commandService)
        {
            if(commandService == null)
            {
                throw new ArgumentNullException("commandService");
            }
            if(XPathInformationConfiguration.Current == null)
            {
                throw new NullReferenceException("XPathInformationConfiguration.Current is null.");
            }

            var alwaysDisplayedAttributes = XPathInformationConfiguration.Current.AlwaysDisplayedAttributes;
            var attributeFilter = new AttributeFilter(alwaysDisplayedAttributes);

            var copyGenericXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyGenericXPath, _repository, new XPathWriter(new[] {attributeFilter}));
            commandService.AddCommand(copyGenericXPathCommand);

            var copyAbsoluteXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyAbsoluteXPath, _repository, new AbsoluteXPathWriter(new[] {attributeFilter}));
            commandService.AddCommand(copyAbsoluteXPathCommand);

            var preferredAttributeCandidates = XPathInformationConfiguration.Current.PreferredAttributeCandidates;
            var distinctAttributeFilter = new DistinctAttributeFilter(preferredAttributeCandidates.Union(alwaysDisplayedAttributes));
            var copyDistinctXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyDistinctXPath, _repository, new XPathWriter(new[] {distinctAttributeFilter}));
            commandService.AddCommand(copyDistinctXPathCommand);

            var copyXmlStructureCommand = new CopyXmlStructureCommand(Symbols.CommandIDs.CopyXmlStructure, _repository);
            commandService.AddCommand(copyXmlStructureCommand);
        }
    }
}