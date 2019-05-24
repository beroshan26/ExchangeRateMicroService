using ExchangeRateWebApi.API;
using ExchangeRateWebApi.Cache;
using ExchangeRateWebApi.Helpers;
using ExchangeRateWebApi.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRateWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped<IFixerApi, FixerApi>();
            services.AddScoped<ICache, RedisCache>();
            services.AddScoped<IApiConverter, ApiConverter>();
            services.AddScoped<IApiFormatter, ApiFormatter>();
            services.AddScoped<ICurrencyValidator, CurrencyValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ICache _cache, IFixerApi _fixerApi)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }            
            app.UseHttpsRedirection();
            app.UseMvc();
            _cache.ClearCache();
            _fixerApi.GetExchangeRate(Configuration["AppSettings:AllowedApiBaseCcy"], Configuration["AppSettings:AllowedCurrencies"]);
        }
    }
}
