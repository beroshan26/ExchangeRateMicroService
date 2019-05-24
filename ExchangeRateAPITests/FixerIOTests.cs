using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;

namespace ExchangeRateAPITests
{
    [TestClass]
    public class FixerIOTests
    {
        IConfiguration _configuration;

        [TestInitialize]
        public void TestSetup()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        [TestMethod]
        public void FixerIo_Returns_OK_When_URI_Is_correct()
        {
            var url = GetFixerUrl();

            HttpResponseMessage response = null;

            using (var client = new HttpClient())
            {
                response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
            }

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public void FixerIo_Returns_Error_When_URI_Is_Incorrect()
        {
            var url = GetFakeFixerUrl();

            HttpResponseMessage response = null;

            using (var client = new HttpClient())
            {
                response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
            }

            Assert.IsTrue(response.Content.ReadAsStringAsync().Result.Contains("invalid_access_key"));

        }

        public string GetFixerUrl()
        {
            //sample url-http://data.fixer.io/api/latest?access_key=API_KEY//&base=USD&symbols=GBP
            return $"{_configuration["AppSettings:FixerUri"]}{"latest"}?access_key={_configuration["AppSettings:FixerApiKey"]}&base=" +
                $"{_configuration["AppSettings:AllowedApiBaseCcy"]}&symbols={_configuration["AppSettings:AllowedCurrencies"]}";
        }

        public string GetFakeFixerUrl()
        {
            //sample url-http://data.fixer.io/api/latest?access_key=API_KEY//&base=USD&symbols=GBP
            return $"{_configuration["AppSettings:FixerUri"]}{"latest"}?access_key=invalidaccesskey&base=" +
                $"{_configuration["AppSettings:AllowedApiBaseCcy"]}&symbols={_configuration["AppSettings:AllowedCurrencies"]}";
        }
    }
}
