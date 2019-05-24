using ExchangeRateWebApi.API;
using ExchangeRateWebApi.Cache;
using ExchangeRateWebApi.Controllers;
using ExchangeRateWebApi.Helpers;
using ExchangeRateWebApi.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPITests
{
    [TestClass]
    public class FixerIOControllerTests
    {
        IConfiguration _configuration;
        IFixerApi _fixerApi;
        ICache _cache;
        IApiConverter _converter;
        IApiFormatter _formatter;
        ICurrencyValidator _currencyValidator;
        FixerIOController _fixerController;

        [TestInitialize]
        public void TestSetup()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            _formatter = new ApiFormatter(_configuration);
            _converter = new ApiConverter(_formatter);
            _cache = new RedisCache(_configuration, _formatter, _converter);
            _currencyValidator = new CurrencyValidator(_configuration);
            _fixerApi = new FixerApi(_configuration, _cache, _formatter, _converter, _currencyValidator);
            _fixerController = new FixerIOController(_configuration, _fixerApi);
        }

        [TestMethod]
        public void Get_WhenCalled_ReturnsOkResult()
        {
            var result = _fixerController.Get();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Get_WhenCalled_WithNullDI_ReturnsErrorResult()
        {
            _fixerController = new FixerIOController(null, null);

            var result = _fixerController.Get();            
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Get_WhenCalled_WithConfigParamNull_ReturnsErrorResult()
        {
            _fixerController = new FixerIOController(null, _fixerApi);

            var result = _fixerController.Get();
        }


        [TestMethod]        
        public void Get_WhenCalled_WithFixerApiParamNull_ReturnsOkResult()
        {
            _fixerController = new FixerIOController(_configuration, null);

            var result = _fixerController.Get();

            Assert.IsNotNull(result);
        }

        [TestMethod]        
        public void FixerApiControllerGetTestWithMocks()
        {
            var result = _fixerController.Get(null, null);

            Assert.IsNotNull(result);
        }
    }
}
