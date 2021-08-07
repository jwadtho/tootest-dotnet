using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using NLog;

namespace tootest_dotnet
{
    public class Startup
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = $"Server=rm-gs5sie41gt309b8jnro.mysql.singapore.rds.aliyuncs.com;Port=3306;Database=ch_product;User Id=test_read;Password=g!Kd6U94GnkuMA%q";

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "tootest_dotnet", Version = "v1" });
            });
            services.AddDbContext<Context>(opt => opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            services.AddTransient<IDbConnection>(s => new MySqlConnection(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "tootest_dotnet v1"));
            }


            logger.Info($"Hi Too!...{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
            //Console.WriteLine($"Hi Too!...{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
            app.UsePathBase(new PathString("/tootest"));
            app.UseRouting();

            app.UseAuthorization(); 

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
