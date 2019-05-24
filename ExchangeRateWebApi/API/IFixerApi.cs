using System.Threading.Tasks;
using ExchangeRateWebApi.Model;

namespace ExchangeRateWebApi.API
{
    public interface IFixerApi
    {
        ExchangeRateResponse GetExchangeRate(string baseCurrency, string targetCurrency);
        Task<ExchangeRateResponse> GetExchangeRateAsync(string baseCurrency, string targetCurrency);
    }
}