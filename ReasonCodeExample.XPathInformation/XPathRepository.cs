namespace ReasonCodeExample.XPathInformation
{
    public class XPathRepository
    {
        private static string _xpath;
        private static readonly object Lock = new object();

        public void Put(string xpath)
        {
            lock (Lock)
            {
                _xpath = xpath;
            }
        }

        public string Get()
        {
            lock (Lock)
            {
                return _xpath;
            }
        }
    }
}