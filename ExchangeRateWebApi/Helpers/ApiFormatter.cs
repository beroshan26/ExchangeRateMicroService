using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateWebApi.Helpers
{
    public class ApiFormatter : IApiFormatter
    {
        private string _decimalFormat;
      
        public ApiFormatter(IConfiguration configuration)
        {
            _decimalFormat = configuration["AppSettings:DecimalFormat"];
        }
        public double FormattedDoubleValue(double valueToFormat)
        {
            return double.Parse(valueToFormat.ToString(_decimalFormat));
        }

        public string FormatCacheKey(string baseCcy, string targetCcy)
        {
            return $"{baseCcy.ToUpper()}##{targetCcy.ToUpper()}##{DateTime.Now.Date}";
        }
    }
}
