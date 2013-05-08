namespace ReasonCodeExample.XPathInformation
{
    internal class XPathRepository
    {
        private static string _xpath;
        private static readonly object Lock = new object();

        public virtual void Put(string xpath)
        {
            lock (Lock)
            {
                _xpath = xpath;
            }
        }

        public virtual string Get()
        {
            lock (Lock)
            {
                return _xpath;
            }
        }
    }
}