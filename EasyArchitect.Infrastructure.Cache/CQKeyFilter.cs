using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyArchitect.Infrastructure.Cache
{
    /// <summary>
    /// 使用於 <see cref="ICQCacheProvider"/> 多個查詢方法成員的鍵值過濾型別，此型別實作使用 '%' 字元代表可替換的任何字串，進行鍵值比對。
    /// </summary>
    /// <remarks>
    /// <para>識別字符'%'僅能用在字串的最前端或最尾端，其他中間位置皆視為一般字符當成過濾條件字串</para>
    /// <para>"%ABC" 表示查詢結尾為 "ABC" 的鍵值</para>
    /// <para>"ABC%" 表示查詢開頭圍 "ABC" 的鍵值</para>
    /// <para>"%ABC%" 表示查詢含有 "ABC" 的鍵值</para>
    /// <para>空字串<see cref="String.Empty"/> 或 <see langword="null"/> 表示不過濾鍵值(亦即所有鍵值皆符合查詢條件)。</para>
    /// </remarks>
    public class CQKeyFilter : ICQKeyFilter
    {
        const char CS_ANYWORD_TOKEN = '%';
        /// <summary>
        /// 筆數限制預設值
        /// </summary>
        public const long MAX_COUNT = 10000;
        bool isStartsWith, isEndsWith, isContains, hasFilter;
        string filter = null;
        long maxCount = MAX_COUNT;
        /// <summary>
        /// 初始化新執行個體。 
        /// </summary>
        /// <param name="strFilter">過濾樣板字串，預設為不過濾。</param>
        /// <param name="maxCount">限制筆數，預設為<see cref="CQPercentKeyFilter.MAX_COUNT"/></param>
        /// <remarks>
        /// <para>"%ABC" 表示查詢結尾為 "ABC" 的鍵值</para>
        /// <para>"ABC%" 表示查詢開頭圍 "ABC" 的鍵值</para>
        /// <para>"%ABC%" 表示查詢含有 "ABC" 的鍵值</para>
        /// <para>空字串<see cref="String.Empty"/> 或 <see langword="null"/> 表示不過濾鍵值(亦即查詢所有鍵值)。</para>
        /// </remarks>
        public CQKeyFilter(string strFilter = null, long maxCount = MAX_COUNT)
        {
            this.maxCount = maxCount;
            isStartsWith = isEndsWith = isContains = hasFilter = false;
            filter = strFilter;
            if (!string.IsNullOrEmpty(filter))
            {
                if (filter.Length > 1 && filter[filter.Length - 1] == CS_ANYWORD_TOKEN)
                {
                    isStartsWith = true;
                    filter = filter.Substring(0, filter.Length - 1);
                }
                if (filter.Length > 1 && filter[0] == CS_ANYWORD_TOKEN)
                {
                    isEndsWith = true;
                    filter = filter.Substring(1, filter.Length - 1);
                }
                if (isStartsWith && isEndsWith)
                {
                    isContains = true;
                }
                hasFilter = !string.IsNullOrEmpty(filter);
            }
        }
        /// <summary>
        /// 檢查鍵值是否符合過濾條件。
        /// </summary>
        /// <param name="key">要檢查的鍵值</param>
        /// <returns>若 <see langword="true" /> 則符合過濾條件。</returns>
        public bool Contains(string key)
        {
            if (hasFilter)
            {
                if (isContains)
                {
                    if (key.Contains(filter)) return true;
                }
                else if (isStartsWith)
                {
                    if (key.StartsWith(filter)) return true;
                }
                else if (isEndsWith)
                {
                    if (key.EndsWith(filter)) return true;
                }
                else
                {
                    if (key == filter) return true;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 取得限制筆數
        /// </summary>
        /// <returns>限制筆數</returns>
        public long GetMaxCount()
        {
            return maxCount;
        }
        /// <summary>
        /// 完整比對的鍵值(此方法如果傳回字串則表示使用者輸入的查詢條件未含有過濾用的識別字符)
        /// </summary>
        /// <returns><see langword="null"/>表示有過濾條件，沒有完整比對的鍵值。</returns>
        public string ExactKey()
        {
            if (hasFilter && !(isStartsWith || isEndsWith || isContains))
            {
                return filter;
            }
            return null;
        }
        /// <summary>
        /// 是否所有鍵值比對都會符合過濾條件
        /// </summary>
        /// <returns></returns>
        public bool IsAlwaysMatch()
        {
            return !hasFilter;
        }
    }

}
