using ExchangeRateWebApi.Helpers;
using ExchangeRateWebApi.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceStack.Redis;
using System;
using System.Globalization;
using System.Net.Http;

namespace ExchangeRateWebApi.Cache
{
    public class RedisCache : ICache
    {
        IConfiguration _configuration;
        private string redisHost;
        IApiConverter _converter;
        IApiFormatter _formatter;

        public RedisCache(IConfiguration configuration, IApiFormatter formatter, IApiConverter converter)
        {
            _configuration = configuration;
            redisHost = _configuration["AppSettings:RedisHostName"];
            _converter = converter;
            _formatter = formatter;
        }
        public string GetExchangeRateFromCache(string baseCurrency, string targetCurrency)
        {
            var key = _formatter.FormatCacheKey(baseCurrency, targetCurrency);
            string response = null;
            using (RedisClient redisClient = new RedisClient(redisHost))
            {
                var cacheItem = redisClient.Get<string>(key);
                if (cacheItem != null)
                {
                    response = cacheItem;
                }
            }
            return response;
        }
        
        public void AddToCache(ExchangeRateResponse exchangeRateResponse)
        {
            var key = _formatter.FormatCacheKey(exchangeRateResponse.BaseCurrency, exchangeRateResponse.TargetCurrency);
            var itemToCache = _converter.ConvertExchnageRateResponseToString(exchangeRateResponse);
            using (RedisClient redisClient = new RedisClient(redisHost))
            {
                SetCache(key, itemToCache, redisClient);
            }
        }

        private void SetCache(string key, string itemToCache, RedisClient redisClient)
        {
            if (redisClient.Get<string>(key) != null)
            {
                redisClient.Remove(key);
            }
            if (redisClient.Get<string>(key) == null)
            {
                redisClient.Set(key, itemToCache, TimeSpan.FromDays(1));
            }
        }

        public void ClearCache()
        {
            using (RedisClient redisClient = new RedisClient(redisHost))
            {
                redisClient.GetAllKeys().ForEach(cache => redisClient.Remove(cache));
            }
        }

        public string GetExchangeRateFromCacheByKey(string cacheKey)
        {
            string cacheItem;
            using (RedisClient redisClient = new RedisClient(redisHost))
            {
                cacheItem = redisClient.Get<string>(cacheKey);
            }
            return cacheItem;
        }
    }
}
