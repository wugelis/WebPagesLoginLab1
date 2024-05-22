using System.Text.Json;

namespace EasyArchitect.PageModel.AuthExtensions
{
    /// <summary>
    /// 新客製型別：代表 Cookie 內容
    /// </summary>
    public class NewCookie
    {
        public NewCookie() { }
        public NewCookie(string cookieId)
        {
            _cookieId = cookieId;
        }

        private string _cookieId = string.Empty;
        private Content _content = new Content();

        public Content Values
        {
            get
            {
                return _content;
            }
        }

        public string CookieID
        {
            get
            {
                return _cookieId;
            }
            private set { _cookieId = value; }
        }

        public void Add(string key, object content)
        {
            _content.Add(key, content);
        }
        /// <summary>
        /// 透過 Cookie 字串產生 NewCookie 執行個體
        /// </summary>
        /// <param name="cookieValue">Cookie 字串內容</param>
        /// <param name="cookieId">需傳入原本的 cookieId</param>
        /// <returns></returns>
        public static NewCookie GetNewCookieByString(string? cookieValue, string cookieId)
        {
            List<CookieItem> result = JsonSerializer.Deserialize<List<CookieItem>>(cookieValue);

            NewCookie cookieSource = new NewCookie(cookieId);
            foreach (var item in result)
            {
                cookieSource.Add(item.ItemKey, item.ItemValue);
            }

            return cookieSource;
        }
        /// <summary>
        /// 透過 NewCookie 物件取得原本的 Cookie 序列化字串
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string GetJsonByNewCookie(NewCookie cookie)
        {
            List<CookieItem> items = new List<CookieItem>();
            foreach (var key in cookie.Keys)
            {
                items.Add(new CookieItem()
                {
                    ItemKey = key,
                    ItemValue = cookie.Values[key]?.ToString()
                });
            }
            var jsonString01 = JsonSerializer.Serialize(items);
            return jsonString01;
        }
        /// <summary>
        /// 取得 Doctionary 內所有的 Keys
        /// </summary>
        public IEnumerable<string> Keys
        {
            get
            {
                return _content.Keys;
            }
        }
        /// <summary>
        /// 取得 Dictionary 內的個數
        /// </summary>
        public int Count
        {
            get
            {
                return _content.Count;
            }
        }
    }
}