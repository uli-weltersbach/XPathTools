namespace ReasonCodeExample.XPathInformation
{
    internal class XPathRepository
    {
        private static readonly object Lock = new object();
        private static string _xpath;

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