namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class TrimCommandTextFormatter : ICommandTextFormatter
    {
        private const int DefaultMaxLength = 80;
        private readonly int _maxLength;

        public TrimCommandTextFormatter()
            : this(DefaultMaxLength)
        {
        }

        public TrimCommandTextFormatter(int maxLength)
        {
            _maxLength = maxLength;
        }

        public virtual string Format(string xpath, int? elementCount)
        {
            return elementCount.HasValue ? Trim(xpath) : string.Empty;
        }

        private string Trim(string xpath)
        {
            var excessChars = xpath.Length - _maxLength;
            return excessChars > 0 ? "..." + xpath.Substring(excessChars, _maxLength) : xpath;
        }
    }
}