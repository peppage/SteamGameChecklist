using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpaCliMiddleware;
using SteamGameChecklist.Web.Services;

namespace SteamGameChecklist.Web
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

            services.AddControllersWithViews();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });

            services.AddSingleton(Configuration);
            services.AddSingleton<IGetSteamGamesService, GetSteamGamesService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            var steamService = app.ApplicationServices.GetService<IGetSteamGamesService>();
            steamService.LoadGames();

            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
#if DEBUG
                if (env.IsDevelopment())
                {
                    endpoints.MapToSpaCliProxy(
                        "{*path}",
                        new SpaOptions { SourcePath = "ClientApp" },
                        npmScript: env.IsDevelopment() ? "autobuild" : "",
                        port: 35729,
                        regex: "LiveReload enabled",
                        forceKill: true,
                        useProxy: false
                    );
                }
#endif
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
