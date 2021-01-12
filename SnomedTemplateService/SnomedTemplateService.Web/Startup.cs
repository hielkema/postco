using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SnomedTemplateService.Core.Interfaces;
using SnomedTemplateService.Data;
using SnomedTemplateService.Parser;

namespace SnomedTemplateService.Web
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
            services.AddCors(
                options => options.AddPolicy(name: Policies.CorsAllowAnyOrigin,
                    builder =>
                    {
                        builder.WithOrigins("*");
                    })
                );
            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.Formatting = Formatting.Indented);
            services.AddMemoryCache();
            services.AddScoped<ITemplateRepository, XmlFileTemplateRepository>();
            services.AddScoped<IEtlParseService, AntlrEtlParseService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime applicationLifeTime,
            ILogger<Startup> logger,
            ITemplateRepository templateRepository)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (templateRepository.FoundErrorsInTemplates)
            {
                Environment.ExitCode = 65;
                applicationLifeTime.StopApplication();
            } 
        }
    }
}
