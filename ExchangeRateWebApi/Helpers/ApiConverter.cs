using ExchangeRateWebApi.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateWebApi.Helpers
{
    public class ApiConverter : IApiConverter
    {
        IApiFormatter _formatter;
        public ApiConverter(IApiFormatter formatter)
        {
            _formatter = formatter;
        }

        public string ConvertExchnageRateResponseToString(ExchangeRateResponse exchResp)
        {
            return JsonConvert.SerializeObject(exchResp);
        }

        public ExchangeRateResponse ConvertStringToExchnageRateResponse(string exchRatestring)
        {
            ExchangeRateResponse responseResult = null;
            if (string.IsNullOrEmpty(exchRatestring))
                return responseResult;

            var jobject = JObject.Parse(exchRatestring);
            var baseCcy = jobject.Value<string>("BaseCurrency");
            var targetCcy = jobject.Value<string>("TargetCurrency");
            var ccyRate = jobject.Value<double>("CcyExchangeRate");
            ccyRate = _formatter.FormattedDoubleValue(ccyRate);
            var timeStamp = DateTime.Parse(jobject.Value<string>("TimeStamp"), CultureInfo.InvariantCulture); ;
            responseResult = new ExchangeRateResponse(baseCcy, targetCcy, ccyRate, timeStamp);

            return responseResult;
        }
    }
}
