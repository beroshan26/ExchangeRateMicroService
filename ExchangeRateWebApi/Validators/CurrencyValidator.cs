using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateWebApi.Validators
{
    public class CurrencyValidator : ICurrencyValidator
    {
        private string AllowedCurrencies;
        private string AllowedApiBaseCcy;

        public CurrencyValidator(IConfiguration configuration)
        {
            AllowedCurrencies = configuration["AppSettings:AllowedCurrencies"];
            AllowedApiBaseCcy = configuration["AppSettings:AllowedApiBaseCcy"];
        }
        public string ValidateCurrncies(string baseCurrency, string targetCurrency)
        {
            if (baseCurrency == null || targetCurrency == null)
                return ("Base or Target Currency Cannot be null");

            baseCurrency = baseCurrency.ToUpper();
            targetCurrency = targetCurrency.ToUpper();

            if (!AllowedCurrencies.Contains(baseCurrency) || !AllowedCurrencies.Contains(targetCurrency))
                return "Invalid currency found in the request. Allowed Currency Symbols are " + AllowedCurrencies;

            return null;
        }
    }
}
