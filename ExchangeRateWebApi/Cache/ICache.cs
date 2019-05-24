using ExchangeRateWebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateWebApi.Cache
{
    public interface ICache
    {
        string GetExchangeRateFromCache(string baseCurrency, string targetCurrency);
        string GetExchangeRateFromCacheByKey(string cacheKey);
        void AddToCache(ExchangeRateResponse exchangeRateResponse);

        void ClearCache();
    }
}
