using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using MakeMagic.MakeMagicApi;
using MakeMagic.Persistence;
using MakeMagic.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MakeMagic
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Como o app e o banco são inicializados pelo docker-compose ao mesmo tempo, precisamos de um tempo antes que o banco se torne responsivo.
        /// Esperamos até dez segundos para que isso aconteça.
        /// </summary>
        /// <param name="attempts"></param>
        private void AwaitAndInitDb(int attempts = 10)
        {
            try
            {
                DatabaseBootstraper.Init(new MySqlConnection(Configuration.GetConnectionString("Default")));
            }
            catch
            {
                if (attempts < 1)
                    throw;
                Thread.Sleep(1000);
                AwaitAndInitDb(attempts - 1);
            }
            
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddScoped<CharacterEditor>();
            services.AddScoped<CharactersRepository>(sp =>
            {
                var connectionString = Configuration.GetConnectionString("Default");
                var connection = new MySqlConnection(connectionString);                
                connection.Open();
                connection.ChangeDatabase("MakeMagic");
                return new CharactersRepository(connection);
            });
            services.AddScoped<MakeMagicApiClient>(sp =>
            {
                var baseUrlAsString = Configuration.GetSection("MakeMagicApiConfig").GetSection("BaseUrl").Value;
                var apiKey = Configuration.GetSection("MakeMagicApiConfig").GetSection("ApiKey").Value;
                var baseUrl = new Uri(baseUrlAsString);
                return new MakeMagicApiClient(HttpClientProviderSingleton.GetHttpClientForBaseUri(baseUrl), apiKey);
            });

            AwaitAndInitDb();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
