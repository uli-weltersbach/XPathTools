namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal struct Symbols
    {
        public const string PackageID = "253aa665-a779-4716-9ded-5b0c2cb66710";
        public const string ContextMenuID = "5d1f73e6-77cd-4a05-8f7b-2c4557503939";

        public struct CommandIDs
        {
            public const int CopySimplifiedXPath = 0x2022;
            public const int CopyGenericXPath = 0x1022;
            public const int CopyAbsoluteXPath = 0x1023;
            public const int CopyDistinctXPath = 0x1024;
            public const int CopyXmlStructure = 0x1025;
            public const int ShowXPathWorkbench = 0x1026;
        }

        public struct MenuIDs
        {
            public const int MenuGroup = 0x1019;
            public const int SubMenu = 0x1020;
            public const int SubMenuGroup = 0x1021;
        }

        public struct ToolWindowIDs
        {
            public const string XPathWorkbench = "5A897CD2-96EF-41CF-B52E-8DDCBC6FD8CA";
        }
    }
}