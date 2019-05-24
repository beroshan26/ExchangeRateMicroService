using ExchangeRateWebApi.API;
using ExchangeRateWebApi.Cache;
using ExchangeRateWebApi.Controllers;
using ExchangeRateWebApi.Helpers;
using ExchangeRateWebApi.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPITests
{
    [TestClass]
    public class FixerApiTests
    {
        IConfiguration _configuration;
        IFixerApi _fixerApi;
        ICache _cache;
        IApiConverter _converter;
        IApiFormatter _formatter;
        ICurrencyValidator _currencyValidator;

        [TestInitialize]
        public void TestSetup()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            _formatter = new ApiFormatter(_configuration);
            _converter = new ApiConverter(_formatter);
            _cache = new RedisCache(_configuration, _formatter, _converter);
            _currencyValidator = new CurrencyValidator(_configuration);
            _fixerApi = new FixerApi(_configuration, _cache, _formatter, _converter, _currencyValidator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NonAllowed_BaseCurrency_Throws_Exception()
        {
            _fixerApi.GetExchangeRate("ABC", "USD");
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void NonAllowed_BaseCurrency_Throws_Exception_Async()
        {
            var task = Task.Run(() => _fixerApi.GetExchangeRateAsync("ABC", "USD"));
            var result = task.Result;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NonAllowed_TargetCurrency_Throws_Exception()
        {
            _fixerApi.GetExchangeRate("USD", "ABC");
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void NonAllowed_TargetCurrency_Throws_Exception_Async()
        {
            var task = Task.Run(() => _fixerApi.GetExchangeRateAsync("ABC", "USD"));
            var result = task.Result;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Null_Currencies_Throws_Exception()
        {
            _fixerApi.GetExchangeRate(null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void Null_Currencies_Throws_Exception_Async()
        {
            var task = Task.Run(() => _fixerApi.GetExchangeRateAsync(null, null));
            var result = task.Result;
        }

        [TestMethod]
        public void Base_Currency_Not_EUR_ReturnsRate()
        {
            var rate = _fixerApi.GetExchangeRate("USD", "EUR");
            Assert.AreNotEqual(rate, null);
        }

        [TestMethod]
        public void Same_Base_And_Target_Returns_1()
        {
            Assert.AreEqual(_fixerApi.GetExchangeRate("EUR", "EUR").CcyExchangeRate, 1);
        }

        [TestMethod]
        public void Allowed_Currency_Returns_Rates()
        {
            var rate = _fixerApi.GetExchangeRate("EUR", "USD");

            Assert.AreNotEqual(rate, null);
        }

        [TestMethod]
        public void Allowed_CCY_Combinations_Returns_Rate()
        {
            var allowedCurrencies = _configuration["AppSettings:AllowedCurrencies"].Split(',');
            foreach (var item1 in allowedCurrencies)
            {
                var ccy1 = item1.Trim();
                foreach (var item2 in allowedCurrencies)
                {
                    var ccy2 = item2.Trim();
                    var rate = _fixerApi.GetExchangeRate(ccy1, ccy2);
                    Assert.AreNotEqual(rate, null);
                }
            }
        }

        [TestMethod]
        public void Allowed_CCY_Combinations_Returns_Rate_Asynchronous()
        {
            var allowedCurrencies = _configuration["AppSettings:AllowedCurrencies"].Split(',').ToList();
            foreach (var item1 in allowedCurrencies)
            {
                var ccy1 = item1.Trim();
                allowedCurrencies.ForEach(async a =>
                {
                    var ccy2 = a.Trim();
                    var rate = await _fixerApi.GetExchangeRateAsync(ccy1, ccy2);
                    Assert.AreNotEqual(rate, null);
                });
            }
        }

    }
}
