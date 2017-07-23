namespace ReasonCodeExample.XPathTools.Writers
{
    internal interface ICommandTextFormatter
    {
        string Format(string xpath, int? elementCount);
    }
}