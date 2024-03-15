namespace WebPagesLoginLab1.Security
{
    public class Content
    {
        private Dictionary<string, object> _cookieValue = new Dictionary<string, object>();
        public Dictionary<string, object> CookieValue
        {
            get
            {
                return _cookieValue;
            }
        }

        public object this[string key]
        {
            get
            {
                return _cookieValue[key];
            }
            set { _cookieValue[key] = value; }
        }

        public void Add(string key, object obj)
        {
            _cookieValue.Add(key, obj);
        }

        public int Count
        {
            get
            {
                return _cookieValue.Count;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return _cookieValue.Keys.AsEnumerable();

            }
        }
    }
}
