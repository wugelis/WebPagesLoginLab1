namespace EasyArchitect.PageModel.AuthExtensions
{
    /// <summary>
    /// NewCookie 的實際內容（Indexer 物件）
    /// </summary>
    public class Content
    {
        private Dictionary<string, object> _cookieValue = new Dictionary<string, object>();
        /// <summary>
        /// NewCookie 的 Indexer
        /// </summary>
        /// <param name="key">對應在 Cookie 內容 (Values) 的索引 key 值</param>
        /// <returns>回傳當前的 Cookie 索引內容值 (object)</returns>
        public string this[string key]
        {
            get
            {
                object result;
                if (_cookieValue.TryGetValue(key, out result))
                {
                    return result?.ToString();
                }
                return "";
            }
            set { _cookieValue[key] = value; }
        }
        /// <summary>
        /// 在 NewCookie 加入新的 Values 值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public void Add(string key, object obj)
        {
            _cookieValue.Add(key, obj);
        }
        /// <summary>
        /// 取得 Doctionary 內所有的 Keys
        /// </summary>
        public IEnumerable<string> Keys
        {
            get
            {
                return _cookieValue.Keys.AsEnumerable();
            }
        }
        /// <summary>
        /// 取得 Dictionary 內的個數
        /// </summary>
        public int Count
        {
            get
            {
                return this._cookieValue.Count;
            }
        }
    }
}