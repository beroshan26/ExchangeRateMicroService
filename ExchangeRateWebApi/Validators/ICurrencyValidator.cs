using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateWebApi.Validators
{
    public interface ICurrencyValidator
    {
        string ValidateCurrncies(string baseCurrency, string targetCurrency);
    }
}
