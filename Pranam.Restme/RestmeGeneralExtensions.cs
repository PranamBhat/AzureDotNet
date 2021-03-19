using System;
using System.Collections.Generic;

namespace Pranam
{
    public static class RestmeGeneralExtensions
    {
        /// <summary>
        /// Use base uri to identify the use of current Restme client
        /// </summary>
        /// <param name="restme"></param>
        public static void PrepareRestMode(this Rest restme)
        {
            restme.CurrentMode = restme.Configuration.OperationMode;

            if (restme.ConnectionString.IsNotNullOrEmpty())
            {
                var connectionString = restme.ConnectionString.ToLower();
                if ((connectionString.Contains("defaultendpointsprotocol") &&
                     connectionString.Contains("accountname") &&
                     connectionString.Contains("accountkey")) ||
                    (connectionString.Contains("usedevelopmentstorage") &&
                     connectionString.Contains("true")
                    ))
                {
                    restme.CurrentMode = RestMode.AzureStorageClient;
                }
                else if (restme.ConnectionString.ToLower().Contains("redis.cache.windows.net") ||
                         restme.ConnectionString.ToLower().Contains(":6379") ||
                         restme.ConnectionString.ToLower().Contains(":6380"))
                {
                    restme.CurrentMode = RestMode.RedisCacheClient;
                }
                else if (connectionString.IsS3Provider())
                {
                    restme.CurrentMode = RestMode.S3Client;
                }
            }
        }
    }
}