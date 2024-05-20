
using Compat.Runtime.Serialization;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;

namespace EasyArchitect.Infrastructure.Cache
{
    /// <summary>
    /// 實作 (多個查詢方法成員的鍵值過濾型別的介面定義，用來進行鍵值比對)
    /// </summary>
    public class RedisCacheProvider : IRedisCacheProvider
    {
        private ConnectionMultiplexer _redis;
        private IDatabase _db;
        private int _port;
        private int _portSlave;
        private EndPoint[] _currentEndPoint;

        //private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        //{
        //    TypeNameHandling = TypeNameHandling.All
        //};
        public static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            //PropertyNamingPolicy = null
        };

        private string _master1 = string.Empty;
        private string _slave1 = string.Empty;
        /// <summary>
        /// 建立執行個體
        /// 必須設定好 Redis Server for master and slave.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public RedisCacheProvider()
        {
            //_master1 = "10.1.11.87";
            _master1 = "127.0.0.1";

            //_slave1 = "10.1.11.87";
            _slave1 = "127.0.0.1";

            _port = 6379;

            //_portSlave = 6380;

            //_redis = RedisState.CreateRedisConnection(_master1, _slave1, 6379);
            //_redis = RedisState.CreateRedisConnection(_master1, string.Empty, 6379);
            _redis = RedisState.CreateRedisConnection(_master1, _slave1, _port, _portSlave);

            _currentEndPoint = Current.Multiplexer.GetEndPoints();
        }

        private IDatabase Current
        {
            get
            {
                if (_db == null)
                {
                    _db = _redis.GetDatabase();
                }
                return _db;
            }
        }
        /// <summary>
        /// 建立或置換快取資料值
        /// </summary>
        /// <param name="key">鍵值，如果指定的鍵值不存在則新增一筆快取項目。</param>
        /// <param name="data">資料值</param>
        /// <remarks>資料值必須為可序列化的型別。</remarks>
        public void Put(string key, object data)
        {
            /*
            if( (data.GetType() == typeof(System.String))
                || data.GetType() == typeof(System.Int16)
                || data.GetType() == typeof(System.Int32)
                || data.GetType() == typeof(System.Int64)
                || data.GetType() == typeof(System.Decimal)
                || data.GetType() == typeof(System.Boolean)
                )
            {
                string input = data.ToString();
                Current.StringSet(key, input);
            }
            else
            {
                Current.StringSet(key, JsonConvert.SerializeObject(data));
            }
            */
            string result = JsonSerializer.Serialize(data, _jsonSerializerOptions);
            //string result = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
            Current.StringSet(key, result);
        }
        /// <summary>
        /// 建立或置換快取資料值
        /// </summary>
        /// <param name="key">鍵值，如果指定的鍵值不存在則新增一筆快取項目。</param>
        /// <param name="data">資料值</param>
        /// <param name="liveTime">存活時間</param>
        /// <remarks>資料值必須為可序列化的型別。</remarks>
        public void Put(string key, object data, TimeSpan liveTime)
        {
            string result = JsonSerializer.Serialize(data, _jsonSerializerOptions);
            //string result = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
            
            Current.StringSet(key, result, liveTime);
        }
        /// <summary>
        /// 取得快取資料值
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns>資料值，<see langword="null"/> 如果取值失敗。</returns>
        public object Get(string key)
        {
            //		string s = Current.StringGet(key);
            //		s.Dump();
            //		return s;

            object result = null;
            try
            {
                RedisValue cacheResult = Current.StringGet(key);
                result = JsonSerializer.Serialize(cacheResult, _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Fail to Get(\"{0}\")! Treat as a null result.\nError Detail:{1}\n", key, ex.ToString()));
            }

            return result;

        }
        //
        public T? Get<T>(string key)
            where T : class
        {
            try
            {
                var jsonString = Current.StringGet(key);
                if (!jsonString.IsNull)
                {
                    return JsonSerializer.Deserialize<T>(jsonString!, _jsonSerializerOptions);
                    //return JsonConvert.DeserializeObject<T>(jsonString);
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 移除指定的一個快取資料
        /// </summary>
        /// <param name="key">鍵值，如果鍵值不存在將被忽略。</param>
        public void Remove(string key)
        {
            try
            {
                _db.KeyDelete(key);
            }
            catch (Exception ex)
            {
                // Write Log (未實作)
                //string.Format("在刪除 Redis Cache 內容時，發生錯誤！. Error Info:{0}", ex.Message).Dump();
            }
        }

        public void RemoveKeys(string key)
        {
            try
            {
                foreach (var kv in _redis.GetServer(GetCurrentMasterServer()).Keys(0, key))
                {
                    //var result = _db.StringGet(kv);
                    //result.Dump();
                    _db.KeyDelete(kv);
                }
                //_db.KeyDelete(string.Format("{0}*", key));
            }
            catch (Exception e)
            {
                // Write Log (未實作)
                //string.Format("在刪除 Redis Cache 內容時，發生錯誤！. Error Info:{0}", e.Message).Dump();
            }
        }
        /// <summary>
        /// 移除所有符合鍵值過濾條件的快取資料
        /// </summary>
        /// <param name="filter">實作<see cref="ICQKeyFilter"/>介面的過濾條件型別執行個體</param>
        public void Remove(ICQKeyFilter filter)
        {
            string exactKey = filter.ExactKey();
            if (string.IsNullOrEmpty(exactKey))
            {
                foreach (var kv in _redis.GetServer(GetCurrentMasterServer()).Keys())
                {
                    if (filter.Contains(kv.ToString()))
                    {
                        Remove(kv.ToString());
                    }
                }
            }
            else
            {
                Remove(exactKey);
            }
        }

        private long getObjectSize(object o)
        {
            long size = 0;
            using (Stream s = new MemoryStream())
            {
                NetDataContractSerializer ser = new NetDataContractSerializer();
                ser.Serialize(s, o);
                size = s.Length;
            }
            return size;
        }
        public long GetObjectLength(object o)
        {
            return this.getObjectSize(o);
        }
        ///// <summary>
        ///// 取得所有符合鍵值過濾條件的快取項目資訊。
        ///// </summary>
        ///// <param name="filter">實作<see cref="ICQKeyFilter"/>介面的過濾條件型別執行個體</param>
        ///// <param name="calculateSize">是否要計算快取資料的大小。計算大小可能會增加運算負擔。</param>
        //public Dictionary<string, string> GetInfos(ICQKeyFilter filter, bool calculateSize = false)
        //{
        //    Dictionary<string, string> d = new Dictionary<string, string>();
        //    string exactKey = filter.ExactKey();
        //    const string UNKNOWN_SIZE = "(N/A)";
        //    if (string.IsNullOrEmpty(exactKey))
        //    {
        //        foreach (var kv in _redis.GetServer(GetCurrentMasterServer()).Keys())
        //        {
        //            if (d.Count < filter.GetMaxCount() && filter.Contains(kv.ToString()))
        //            {
        //                string strSize = string.Empty;
        //                if (calculateSize)
        //                {
        //                    try
        //                    {
        //                        strSize = string.Format("({0} bytes)", getObjectSize(kv));
        //                    }
        //                    catch
        //                    {
        //                        strSize = UNKNOWN_SIZE;
        //                    }
        //                }
        //                d.Add(kv.ToString(), kv.GetType().FullName + strSize);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        object o = Get(exactKey);
        //        if (o != null)
        //        {
        //            string strSize = string.Empty;
        //            if (calculateSize)
        //            {
        //                try
        //                {
        //                    strSize = string.Format("({0} bytes)", getObjectSize(o));
        //                }
        //                catch
        //                {
        //                    strSize = UNKNOWN_SIZE;
        //                }
        //            }
        //            d.Add(exactKey, o.GetType().FullName + strSize);
        //        }
        //    }
        //    return d;
        //}
        ///// <summary>
        ///// 取得所有符合鍵值過濾條件的鍵值
        ///// </summary>
        ///// <param name="filter">實作<see cref="ICQKeyFilter"/>介面的過濾條件型別執行個體</param>
        ///// <returns>鍵值陣列，如果沒有任何符合的鍵值，則回傳0長度陣列或<see langword="null"/>，是實作此介面的提供者而定。</returns>
        //public string[] GetKeys(ICQKeyFilter filter)
        //{
        //    List<string> d = new List<string>();
        //    string exactKey = filter.ExactKey();
        //    if (string.IsNullOrEmpty(exactKey))
        //    {
        //        foreach (var kv in _redis.GetServer(GetCurrentMasterServer()).Keys())
        //        {
        //            if (d.Count < filter.GetMaxCount() && filter.Contains(kv.ToString()))
        //            {
        //                d.Add(kv);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (Get(exactKey) != null) d.Add(exactKey);
        //    }
        //    return d.ToArray();
        //}
        ///// <summary>
        ///// 取得所有符合鍵值過濾條件的快取項目個數。
        ///// </summary>
        ///// <param name="filter">實作<see cref="ICQKeyFilter"/>介面的過濾條件型別執行個體</param>
        ///// <returns>項目個數。</returns>
        //public long GetCount(ICQKeyFilter filter)
        //{
        //    long count = 0;
        //    string exactKey = filter.ExactKey();
        //    if (string.IsNullOrEmpty(exactKey))
        //    {
        //        foreach (var kv in _redis.GetServer(GetCurrentMasterServer()).Keys())
        //        {
        //            if (filter.Contains(kv.ToString()))
        //            {
        //                count++;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (Get(exactKey) != null) count++;
        //    }
        //    return count;
        //}

        public void GetRedisState()
        {
            IPEndPoint master = GetCurrentMasterServer();
        }

        public IPEndPoint GetCurrentMasterServer()
        {
            EndPoint[] currentEndPoint = Current.Multiplexer.GetEndPoints();
            //currentEndPoint.Dump();
            foreach (IPEndPoint endPoint in currentEndPoint)
            {
                IServer server = _redis.GetServer(endPoint);

                if (server.IsConnected)
                    return endPoint;
            }

            return null;
        }
        public ConnectionMultiplexer GetRedisMultipexer { get { return _redis; } }
        public EndPoint[] GetAllRedisEndpoint()
        {
            return Current.Multiplexer.GetEndPoints();
        }
        //取得 Redis Server 的設定資訊
        public ConfigurationOptions GetConfiguration(bool ssl, string clientName)
        {
            string config = string.Format("{0}:{2},{1}:{3},password=gelis123,serviceName=master", _master1, _slave1, _port, _portSlave);
            var configuration = ConfigurationOptions.Parse(config);
            configuration.Ssl = ssl;
            //configuration.ClientName = clientName;
            //configuration.AbortOnConnectFail = false;
            //configuration.Password = "gelis123"; 
            return configuration;
        }

        public IServer GetRedisServer()
        {
            EndPoint[] currentEndPoint = Current.Multiplexer.GetEndPoints();
            IServer server = null;

            foreach (IPEndPoint endPoint in currentEndPoint)
            {
                server = _redis.GetServer(endPoint);
                if (server.IsConnected)
                    return server;
            }
            return server;
        }
    }
}
