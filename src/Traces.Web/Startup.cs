using System;
using Blazored.Toast;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Traces.Common;
using Traces.Core.ClientFactories;
using Traces.Core.Repositories;
using Traces.Core.Services;
using Traces.Data;
using Traces.Web.AutoRefresh;
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

            services.AddScoped<IApaleoClientFactory, ApaleoClientFactory>();
            services.AddHttpClient<IApaleoClientFactory, ApaleoClientFactory>(client =>
                    client.BaseAddress = new Uri(Configuration["apaleo:ServiceUri"]))
                .AddTransientHttpErrorPolicy(p => p.RetryAsync(3));

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "apaleo";
                })
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.Cookie.Name = "apaleoautorefresh";
                })
                .AddAutomaticTokenRefresh()
                .AddOpenIdConnect("apaleo", options =>
                {
                    options.Authority = "https://identity.apaleo.com/";

                    options.ClientSecret = Configuration["apaleo:ClientSecret"];
                    options.ClientId = Configuration["apaleo:ClientId"];
                    options.CallbackPath = "/signin-apaleo";

                    options.ResponseType = "code id_token";

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("offline_access");
                    foreach (var scope1 in Configuration["apaleo:Scope"].Split(','))
                    {
                        options.Scope.Add(scope1);
                    }

                    options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");

                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role,
                    };
                });

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