using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyArchitect.Infrastructure.Cache
{
    /// <summary>
    /// 多個查詢方法成員的鍵值過濾型別的介面定義，用來進行鍵值比對
    /// </summary>
    public interface ICQKeyFilter
    {
        /// <summary>
        /// 檢查鍵值是否符合過濾條件。
        /// </summary>
        /// <param name="key">要檢查的鍵值</param>
        /// <returns>若 <see langword="true" /> 則符合過濾條件。</returns>
        bool Contains(string key);
        /// <summary>
        /// 取得限制筆數
        /// </summary>
        /// <returns>限制筆數</returns>
        long GetMaxCount();
        /// <summary>
        /// 完整比對的鍵值(此方法如果傳回字串則表示使用者輸入的查詢條件未含有過濾用的字符)
        /// </summary>
        /// <returns><see langword="null"/>表示有過濾條件，沒有完整比對的鍵值。</returns>
        string ExactKey();
        /// <summary>
        /// 是否所有鍵值比對都會符合過濾條件
        /// </summary>
        /// <returns></returns>
        bool IsAlwaysMatch();
    }
}
