using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACoreX.AssemblyLoader;
using ACoreX.Authentication.Core;
using ACoreX.Authentication.JWT;
using ACoreX.Injector.Abstractions;
using ACoreX.Injector.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Smart.View.Run
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
            //services.AddControllers();
            string libPath = "";

            libPath = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent.FullName, "lib");
            IContainerBuilder builder = services.AddBuilder(new NetCoreContainerBuilder(services));
            services
           .AddAuthenticationInstance<JWTAuthService>()
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
