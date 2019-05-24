using ExchangeRateWebApi.Cache;
using ExchangeRateWebApi.Helpers;
using ExchangeRateWebApi.Model;
using ExchangeRateWebApi.Validators;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeRateWebApi.API
{
    public class FixerApi : IFixerApi
    {
        #region Fields
        private string FixerUri;
        private string FixerApiKey;        
        private string AllowedCurrencies;
        private string AllowedApiBaseCcy;
        private ICache _cache;        
        IApiConverter _converter;
        IApiFormatter _formatter;
        ICurrencyValidator _currencyValidator;
        #endregion

        #region Public methods

        public FixerApi(IConfiguration configuration, ICache cache, IApiFormatter formatter, IApiConverter converter, ICurrencyValidator currencyValidator)
        {
            Initialize(configuration);
            _cache = cache;
            _converter = converter;
            _formatter = formatter;
            _currencyValidator = currencyValidator;
        }

        public void Initialize(IConfiguration configuration)
        {
            FixerUri = configuration["AppSettings:FixerUri"];
            FixerApiKey = configuration["AppSettings:FixerApiKey"];
            
            AllowedCurrencies = configuration["AppSettings:AllowedCurrencies"];
            AllowedApiBaseCcy = configuration["AppSettings:AllowedApiBaseCcy"];           
        }
       
        public ExchangeRateResponse GetExchangeRate(string baseCurrency, string targetCurrency)
        {
            ValidateInputs(baseCurrency, targetCurrency);

            ExchangeRateResponse response = null;

            if (baseCurrency.Equals(targetCurrency))
            {
                response = new ExchangeRateResponse(baseCurrency, targetCurrency, 1, DateTime.Now);
                return response;
            }

            //check if cache has value, if yes return it
            var stringResponse = _cache.GetExchangeRateFromCache(baseCurrency, targetCurrency);
            response = _converter.ConvertStringToExchnageRateResponse(stringResponse);

            if (response == null)
            {
                //if cache does not have, then we need to calculate from other ccy using cache data
                //assuming that cache will have been initialized with other ccy data
                CalculateRateFromCache(baseCurrency, targetCurrency);

                if (_cache.GetExchangeRateFromCache(baseCurrency, targetCurrency) == null)
                {
                    //if still no result then query fixerio and add data to cache, this may not be the case most of the time
                    var fixerResponse = GetRatesFromFixerIo(baseCurrency, targetCurrency);
                    if (fixerResponse != null)
                    {
                        ProcessFixerResponse(fixerResponse.Content.ReadAsStringAsync().Result);
                        if (_cache.GetExchangeRateFromCache(baseCurrency, targetCurrency) == null)
                        {
                            CalculateRateFromCache(baseCurrency, targetCurrency);
                        }
                    }
                    else
                    {
                        throw new Exception("Fixer IO error occured");
                    }
                }

                response = _converter.ConvertStringToExchnageRateResponse(_cache.GetExchangeRateFromCache(baseCurrency, targetCurrency));
            }
            
            return response;
        }

        private void ValidateInputs(string baseCurrency, string targetCurrency)
        {
            var errorMessage = _currencyValidator.ValidateCurrncies(baseCurrency, targetCurrency);

            if(!string.IsNullOrEmpty(errorMessage))
            {
                throw new Exception(errorMessage);
            }
        }

        private void CalculateRateFromCache(string baseCurrency, string targetCurrency)
        {
            var baseApiKeyCcy = _formatter.FormatCacheKey(AllowedApiBaseCcy, baseCurrency);
            var baseApiCcyCacheItem = _cache.GetExchangeRateFromCacheByKey(baseApiKeyCcy);

            var targetApiKeyCcy = _formatter.FormatCacheKey(AllowedApiBaseCcy, targetCurrency);
            var targetApiCcyCacheItem = _cache.GetExchangeRateFromCacheByKey(targetApiKeyCcy);

            if (baseApiCcyCacheItem != null && targetApiCcyCacheItem != null)
            {
                var baseApiKeyExchRate = _converter.ConvertStringToExchnageRateResponse(baseApiCcyCacheItem);
                var targetApiKeyExchRate = _converter.ConvertStringToExchnageRateResponse(targetApiCcyCacheItem);
                var response = new ExchangeRateResponse(baseCurrency, targetCurrency,
                    _formatter.FormattedDoubleValue(targetApiKeyExchRate.CcyExchangeRate / baseApiKeyExchRate.CcyExchangeRate), baseApiKeyExchRate.TimeStamp);

                _cache.AddToCache(response);
            }            
        }

        public async Task<ExchangeRateResponse> GetExchangeRateAsync(string baseCurrency, string targetCurrency)
        {
            return await Task.Run(() => GetExchangeRate(baseCurrency, targetCurrency));
        }

        #endregion

        #region Private methods
        private HttpResponseMessage GetRatesFromFixerIo(string baseCurrency, string targetCurrency)
        {
            var url = GetFixerUrl();

            HttpResponseMessage response = null;

            using (var client = new HttpClient())
            {
                response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
            }

            return response;
            
        }

        private void ProcessFixerResponse(string data)
        {
            var root = JObject.Parse(data);
            var rates = root.Value<JObject>("rates");

            foreach (var item in AllowedCurrencies.Split(','))
            {
                var ccy = item.Trim();
                var exchangeRate = rates.Value<double>(ccy);
                var timeStamp = root.Value<long>("timestamp");
                var timespan = new TimeSpan(timeStamp);
                var exchDateTime = DateTime.ParseExact(root.Value<string>("date"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                var dateTimeWithTimeStamp = new DateTime(exchDateTime.Year, exchDateTime.Month, exchDateTime.Day, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
                var exchRateResponse = new ExchangeRateResponse(AllowedApiBaseCcy, ccy, exchangeRate, dateTimeWithTimeStamp);
                _cache.AddToCache(exchRateResponse);
                exchRateResponse = new ExchangeRateResponse(ccy, AllowedApiBaseCcy, _formatter.FormattedDoubleValue(1 / exchangeRate), dateTimeWithTimeStamp);
                _cache.AddToCache(exchRateResponse);
            }
        }
        
        private string GetFixerUrl()
        {
            //sample url-http://data.fixer.io/api/latest?access_key=API_KEY//&base=USD&symbols=GBP
            return $"{FixerUri}{"latest"}?access_key={FixerApiKey}&base={AllowedApiBaseCcy}&symbols={AllowedCurrencies}";
        }
               
        #endregion
    }
}

