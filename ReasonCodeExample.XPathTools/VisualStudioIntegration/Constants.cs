namespace ReasonCodeExample.XPathTools.VisualStudioIntegration
{
    internal static class Constants
    {
        /// <summary>
        /// Visual Studio text lines are 0-based while 
        /// <c>IXmlLineInfo</c> uses 1-based.
        /// </summary>
        public const int TextEditorLineNumberOffset = 1;

        /// <summary>
        /// The start of an <c>IXmlLineInfo</c> is the first letter in 
        /// the element name (e.g. "f" in &lt;fitting&gt;).
        /// </summary>
        public const int XmlLineInfoLinePositionOffset = 1;

        public const string XmlContentTypeName = "XML";
    }
}