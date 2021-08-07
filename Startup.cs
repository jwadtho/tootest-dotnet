using System;

using System.Data;

using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using NLog;
using OpenTracing;
using OpenTracing.Contrib.NetCore.Configuration;
using OpenTracing.Util;

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

            ConfigureJaeger(services);
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

        private void ConfigureJaeger(IServiceCollection services)
        {
            // Registers OpenTracing.Contrib.NetCore to instrument the application. 
            services.AddOpenTracing();
            // Filters requests that are collected by HttpClient to obtain requests for using Jaeger to report data. 
            var httpOption = new HttpHandlerDiagnosticOptions();
            httpOption.IgnorePatterns.Add(req => req.RequestUri.AbsolutePath.Contains("/api/traces"));
            services.AddSingleton(Options.Create(httpOption));

            // Adds the Jaeger Tracer.
            services.AddSingleton<ITracer>(serviceProvider =>
            {
                //string serviceName = serviceProvider.GetRequiredService<IWebHostEnvironment>().ApplicationName;
                string serviceName = "tootest_dotnet";
                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                Configuration.SenderConfiguration senderConfiguration = new Configuration.SenderConfiguration(loggerFactory)
                 //(访问https://tracing-analysis.console.aliyun.com 获取jaeger endpoint)
                 .WithEndpoint("http://tracing-analysis-dc-sh-internal.aliyuncs.com/adapt_ai31mu20eu@a75cdfaffe00107_ai31mu20eu@53df7ad2afe8301/api/traces");

                // This will log to a default localhost installation of Jaeger.
                var tracer = new Tracer.Builder(serviceName)
                    .WithSampler(new ConstSampler(true))
                    .WithReporter(new RemoteReporter.Builder().WithSender(senderConfiguration.GetSender()).Build())
                    .Build();

                // Allows code that can't use DI to also access the tracer.
                GlobalTracer.Register(tracer);

                return tracer;
            });
        }
    }
}
