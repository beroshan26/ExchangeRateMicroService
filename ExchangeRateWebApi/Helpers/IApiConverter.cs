using ExchangeRateWebApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateWebApi.Helpers
{
    public interface IApiConverter
    {
        string ConvertExchnageRateResponseToString(ExchangeRateResponse exchResp);

        ExchangeRateResponse ConvertStringToExchnageRateResponse(string exchRatestring);
    }
}
