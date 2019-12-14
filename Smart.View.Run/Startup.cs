using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ACoreX.AssemblyLoader;

using ACoreX.Injector.Core;
using ACoreX.Injector.Abstractions;
using ACoreX.Injector.NetCore;
namespace Smart.View.Run
{
    public class Startup
    {

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string libPath = Configuration["Moduels:Path"];
            IContainerBuilder builder = services.AddBuilder(new NetCoreContainerBuilder(services));
            builder.AddSingleton<ACoreX.Configurations.Abstractions.IConfiguration, ACoreX.Configurations.NetCore.NetCoreConfiguration>();
            services
           //.AddAuthenticationInstance<JWTAuthService>()
           .AddCors(options =>
           {
               options.AddPolicy(MyAllowSpecificOrigins,
               builder =>
               {
                   builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
               });
           })
           .AddMvc()
           .AddNewtonsoftJson()
           .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
           .LoadModules(builder, libPath)
           .AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(MyAllowSpecificOrigins);
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
