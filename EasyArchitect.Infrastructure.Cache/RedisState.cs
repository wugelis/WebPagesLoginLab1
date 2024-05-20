using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyArchitect.Infrastructure.Cache
{
    /// <summary>
    /// Redis Cache 連線物件管理
    /// </summary>
    public class RedisState
    {
        /// <summary>
        /// 建立 Redis 連線物件 (password 未來須來自 config)
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static ConnectionMultiplexer CreateRedisConnection(string host, string slave, int port, int? portSlave)
        {
            if (string.IsNullOrEmpty(slave))
            {
                return ConnectionMultiplexer.Connect(string.Format("{0}:{2},password=Aa123456,serviceName=master,allowAdmin=true", host, slave, port));
            }
            else
            {
                if (portSlave.HasValue)
                {
                    //return ConnectionMultiplexer.Connect(string.Format("{0}:{2},{1}:{3},password=Aa123456,serviceName=master,allowAdmin=true", host, slave, port, portSlave));
                    // 暫時 mark 掉，因為指定 serviceName 就必須設定 Setinel
                    return ConnectionMultiplexer.Connect($"{host}:{port},password=Aa123456,allowAdmin=true");
                }
                else
                {
                    return ConnectionMultiplexer.Connect(string.Format("{0}:{2},{1}:{2},password=Aa123456,serviceName=master,allowAdmin=true", host, slave, port));
                }
            }
        }
    }
}
