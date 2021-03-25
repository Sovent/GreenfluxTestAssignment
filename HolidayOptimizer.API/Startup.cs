using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nager.Date.ApiGateway;

namespace HolidayOptimizer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var supportedCountries = Configuration.GetSection("SupportedCountries").Get<string[]>();
            services.AddSingleton<IHolidayStatsCalculator>(provider =>
                new HolidayStatsCalculator(
                    provider.GetRequiredService<INagerDateClient>(),
                    supportedCountries));
            
            RegisterNagerDateClient(services);
            
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HolidayOptimizer V1");
            });
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
        
        private void RegisterNagerDateClient(IServiceCollection services)
        {
            const string nagerDateHttpClientName = "nagerDateHttpClient";
            services.AddHttpClient(nagerDateHttpClientName);
            var nagerDateClientConfig = Configuration.GetSection("NagerDateClient").Get<NagerDateClientConfig>();
            services.AddSingleton(provider =>
            {
                var nagerDateClientFactory = new NagerDateClientFactory();
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nagerDateHttpClientName);
                return nagerDateClientFactory.Create(httpClient, nagerDateClientConfig);
            });
        }
    }
}