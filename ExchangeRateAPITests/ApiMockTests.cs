using Moq;
using ExchangeRateWebApi.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExchangeRateWebApi.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using ExchangeRateWebApi.Model;
using ExchangeRateWebApi.Helpers;
using ExchangeRateWebApi.Cache;
using ExchangeRateWebApi.Validators;

namespace ExchangeRateAPITests
{
    [TestClass]
    public class ApiMockTests
    {
        [TestMethod]
        public void FixerApiMockTest()
        {
            var apimock = new Mock<IFixerApi>();           
            var configMock = new Mock<IConfiguration>();

            FixerIOController ctrl = new FixerIOController(configMock.Object, apimock.Object);
            var result = ctrl.Get("EUR", "USD");
            Assert.AreEqual(result.GetType(), typeof(JsonResult));
        }
    }
}
