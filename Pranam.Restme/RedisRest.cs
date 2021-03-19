using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;

namespace Pranam
{
    public partial class Rest
    {
        internal ConnectionMultiplexer redisConnection;
        internal IDatabase redisDatabase;

        private void PrepareRedisRestme()
        {
            try
            {
                redisConnection = new Lazy<ConnectionMultiplexer>(() =>
                {
                    ConnectionMultiplexer result = null;
                    var redisConfig =
                        ConfigurationOptions.Parse(this.ConnectionString);
                    try
                    {
                        result = ConnectionMultiplexer.Connect(redisConfig);
                    }
                    catch (Exception ex)
                    {
                        LogError(ex.Message, ex);
                        var endPoints = redisConfig.EndPoints;
                        foreach (DnsEndPoint endpoint in endPoints)
                        {
                            try
                            {
                                var port = endpoint.Port;
                                if (!IsIpAddress(endpoint.Host))
                                {
                                    IPHostEntry ip = Dns.GetHostEntryAsync(endpoint.Host).WaitAndGetResult(Configuration.DefaultTimeout);
                                    redisConfig.EndPoints.Remove(endpoint);
                                    redisConfig.EndPoints.Add(ip.AddressList.First(), port);
                                }

                                result = ConnectionMultiplexer.Connect(redisConfig);
                            }
                            catch (Exception innerEx)
                            {
                                LogError(innerEx.Message, innerEx);
                                continue;
                            }
                            if (result != null)
                                break;
                        }
                    }
                    return result;
                }).Value;
                redisDatabase = redisConnection?.GetDatabase();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);
                throw new PranamDbException("failed to initialize Redis connection:\n" + ex.Message, ex);
            }
            if (redisConnection?.IsConnected == true)
                Initialized = true;
        }

        bool IsIpAddress(string host)
        {
            string ipPattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
            return Regex.IsMatch(host, ipPattern);
        }

        private Task AttemptDisposeRedis()
        {
            return Task.Run(() =>
            {
                if (redisConnection != null)
                {
                    try
                    {
                        redisConnection.Dispose();
                    }
                    catch
                    {
                    }
                }
            });
        }
    }
}