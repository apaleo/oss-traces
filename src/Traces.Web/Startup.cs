using System;
using Blazored.Toast;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Traces.Common;
using Traces.Core.Repositories;
using Traces.Core.Services;
using Traces.Data;
using Traces.Web.Helpers;
using Traces.Web.Services;
using Traces.Web.ViewModels;

namespace Traces.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages()
                .AddMvcOptions(options => options.Filters.Add(typeof(ContextFilter)));

            services.AddServerSideBlazor();

            services.AddDbContext<TracesDbContext>(
                options => options.UseNpgsql(
                    Configuration["ConnectionStrings:DefaultDatabase"],
                    npgSqlOptions => npgSqlOptions.UseNodaTime()));

            services.AddBlazoredToast();

            services.AddScoped<IRequestContext, RequestContext>();
            services.AddScoped<ITraceRepository, TraceRepository>();
            services.AddScoped<ITraceService, TraceService>();
            services.AddScoped<ITraceModifierService, TraceModifierService>();
            services.AddScoped<ITracesCollectorService, TracesCollectorService>();
            services.AddScoped<TracesViewModel>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                ExecuteDataMigrationsAndEnsureSeedData(serviceScope.ServiceProvider);
            }
        }

        private static void ExecuteDataMigrationsAndEnsureSeedData(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<TracesDbContext>();
            context.Database.Migrate();
        }
    }
}