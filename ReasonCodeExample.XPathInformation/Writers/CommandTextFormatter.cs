namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class CommandTextFormatter : TrimCommandTextFormatter
    {
        public CommandTextFormatter()
        {
        }

        public CommandTextFormatter(int maxLength)
            : base(maxLength)
        {
        }

        public override string Format(string xpath, int? elementCount)
        {
            var trimmedXPath = base.Format(xpath, elementCount);
            if (elementCount.HasValue)
            {
                var matchText = elementCount == 1 ? "match" : "matches";
                return $"({elementCount} {matchText}) {trimmedXPath}";
            }
            return trimmedXPath;
        }
    }
}