using ExchangeRateWebApi.API;
using ExchangeRateWebApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace ExchangeRateWebApi.Controllers
{
    [Route("api/[controller]")]
    public class FixerIOController : Controller
    {
        #region Fields
        IConfiguration _configuration;
        IFixerApi _fixerApi;

        #endregion

        #region Constructor
        public FixerIOController(IConfiguration configuration, IFixerApi fixerApi)
        {
            _configuration = configuration;
            _fixerApi = fixerApi;
        }

        #endregion      

        #region Get methods
        // GET api/fixerio/
        [HttpGet]
        public string Get()
        {
            return "Exchange Rates API. To get the rates please use the url format as " + _configuration["AppSettings:ApiRequestFormat"];
        }

        // GET api/fixerio/rates/
        [HttpGet("{rates}")]
        public JsonResult Get(string baseCcy, string targetCcy)
        {
            try
            {
                var response = _fixerApi.GetExchangeRateAsync(baseCcy, targetCcy);
                return new JsonResult(response.Result as ExchangeRateResponse);
            }
            catch (Exception ex)
            {
                //Log the exception accordingly
                return new JsonResult("Exception thrown when requesting for rates. Error details -> " + ex.Message);
            }

        }

        #endregion
    }
}
