using System;

namespace ExchangeRateWebApi.Model
{
    public class ExchangeRateResponse
    {
        public ExchangeRateResponse(string baseCurrency, string targetCurrency, double exchgRate, DateTime timeStamp)
        {
            BaseCurrency = baseCurrency;
            TargetCurrency = targetCurrency;
            CcyExchangeRate = exchgRate;
            TimeStamp = timeStamp;
        }
      
        public string BaseCurrency { get; }
               
        public string TargetCurrency { get; }
        
        public double CcyExchangeRate { get; }

        public DateTime TimeStamp { get; set; }
    }
}
