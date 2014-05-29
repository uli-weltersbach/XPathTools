namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal static class CommandTextFormatter
    {
        static CommandTextFormatter()
        {
            MaxLength = 80;
        }

        public static int MaxLength
        {
            get;
            set;
        }

        public static string Format(string xpath, int elementCount)
        {
            var excessChars = xpath.Length - MaxLength;
            var commandText = excessChars > 0 ? "..." + xpath.Substring(excessChars, MaxLength) : xpath;
            var matchText = elementCount == 1 ? "match" : "matches";
            return string.Format("({0} {1}) {2}", elementCount, matchText, commandText);
        }
    }
}