using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyArchitect.Infrastructure.Cache
{
    /// <summary>
    /// 資料快取提供者的介面定義
    /// </summary>
    public interface IRedisCacheProvider
    {
        /// <summary>
        /// 建立或置換快取資料值
        /// </summary>
        /// <param name="key">鍵值，如果指定的鍵值不存在則新增一筆快取項目。</param>
        /// <param name="data">資料值</param>
        /// <remarks>資料值必須為可序列化的型別。</remarks>
        void Put(string key, object data);
        /// <summary>
        /// 建立或置換快取資料值
        /// </summary>
        /// <param name="key">鍵值，如果指定的鍵值不存在則新增一筆快取項目。</param>
        /// <param name="data">資料值</param>
        /// <param name="liveTime">存活時間</param>
        /// <remarks>資料值必須為可序列化的型別。</remarks>
        void Put(string key, object data, TimeSpan liveTime);
        /// <summary>
        /// 取得快取資料值
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns>資料值</returns>
        T Get<T>(string key) where T : class;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key);
        /// <summary>
        /// 移除指定的一個快取資料
        /// </summary>
        /// <param name="key">鍵值，如果鍵值不存在將被忽略。</param>
        void Remove(string key);
        /// <summary>
        /// 移除所有符合鍵值過濾條件的快取資料
        /// </summary>
        /// <param name="filter">實作<see cref="ICQKeyFilter"/>介面的過濾條件型別執行個體</param>
        void Remove(ICQKeyFilter filter);
        /// <summary>
        /// 取得所有符合鍵值過濾條件的快取項目資訊。
        /// </summary>
        /// <param name="filter">實作<see cref="ICQKeyFilter"/>介面的過濾條件型別執行個體</param>
        /// <param name="calculateSize">是否要計算快取資料的大小。計算大小可能會增加運算負擔。</param>
        //Dictionary<string, string> GetInfos(ICQKeyFilter filter, bool calculateSize = false);
        /// <summary>
        /// 取得所有符合鍵值過濾條件的鍵值
        /// </summary>
        /// <param name="filter">實作<see cref="ICQKeyFilter"/>介面的過濾條件型別執行個體</param>
        /// <returns>鍵值陣列，如果沒有任何符合的鍵值，則回傳0長度陣列或<see langword="null"/>，是實作此介面的提供者而定。</returns>
        //string[] GetKeys(ICQKeyFilter filter);
        //	/// <summary>
        //	/// 取得所有符合鍵值過濾條件的快取項目個數。
        //	/// </summary>
        //	/// <param name="filter">實作<see cref="ICQKeyFilter"/>介面的過濾條件型別執行個體</param>
        //	/// <returns>項目個數。</returns>
        //long GetCount(ICQKeyFilter filter);

    }
}
