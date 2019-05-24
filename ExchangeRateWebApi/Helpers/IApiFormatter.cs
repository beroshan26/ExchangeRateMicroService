using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateWebApi.Helpers
{
    public interface IApiFormatter
    {
        double FormattedDoubleValue(double valueToFormat);

        string FormatCacheKey(string baseCcy, string targetCcy);
    }
}
