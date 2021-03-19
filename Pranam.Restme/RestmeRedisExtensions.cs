using System;
using System.Threading.Tasks;

namespace Pranam
{
    public static class RestmeRedisExtensions
    {
        public static async Task<T> RedisGetAsync<T>(this Rest restme, string redisKey)
        {
            MustBeRedisMode(restme);
            string stringValue = await restme.redisDatabase.StringGetAsync(redisKey);

            restme.LogDebug($" RestmeRedis - GET:  {redisKey}\n RESULT: \n {stringValue}");
            if (!stringValue.IsNotNullOrEmpty()) return default(T);
            if (typeof(T).IsPrimitiveType())
            {
                return (T) Convert.ChangeType(stringValue, typeof(T));
            }

            return stringValue.JsonDeserialize<T>(restme.Configuration.UseRestConvertForCollectionSerialization);
        }

        public static async Task<T> RedisPostAsync<T>(this Rest restme, string redisKey, object dataObject)
        {
            MustBeRedisMode(restme);
            var objectInString =
                dataObject.JsonSerialize(restme.Configuration.UseRestConvertForCollectionSerialization);
            restme.LogDebug($"Redis convert - object to string:  {objectInString}");
            if (await restme.redisDatabase.StringSetAsync(redisKey, objectInString))
                return (T) dataObject;
            return default(T);
        }

        public static async Task<T> RedisDeleteAsync<T>(this Rest restme, string redisKey)
        {
            MustBeRedisMode(restme);
            var result = await restme.redisDatabase.KeyDeleteAsync(redisKey);
            if (typeof(T) == typeof(bool))
                return (T) Convert.ChangeType(result, typeof(T));
            return default(T);
        }

        #region Private Methods

        private static void MustBeRedisMode(Rest restme)
        {
            if (restme?.CurrentMode != RestMode.RedisCacheClient)
                throw new InvalidOperationException(
                    $"current request is not valid operation, you are under RestMode: {restme.CurrentMode.ToString()}");
        }

        #endregion
    }
}