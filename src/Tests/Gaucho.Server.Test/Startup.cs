using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Gaucho.Server.Test
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
            services.AddControllers();

            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            GlobalConfiguration.Configuration.UseProcessingServer(p =>
            {
                var reader = new WickedFlame.Yaml.YamlReader();
                var config = reader.Read<PipelineConfiguration>("APILogMessage.yml");
                p.BuildPipeline(config);

                config = reader.Read<PipelineConfiguration>("RecurringJob.yml");
                p.BuildPipeline(config);
            });

            LoggerConfiguration.AddLogWriter(new ConsoleLogWriter());

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders()
                    .AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Initialize Gaucho Server");

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                // notify long-running tasks of pending doom  
            });
        }
    }
}