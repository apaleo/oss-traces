using System;
using System.Net.Http;
using Blazored.Toast;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;
using Traces.Common;
using Traces.Common.Utils;
using Traces.Core.ClientFactories;
using Traces.Core.Repositories;
using Traces.Core.Services;
using Traces.Data;
using Traces.Web.AutoRefresh;
using Traces.Web.Middlewares;
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
            services.AddRazorPages(options => options.Conventions.AuthorizeFolder("/"));

            services.AddServerSideBlazor();

            services.AddBlazorise(options => { options.ChangeTextOnKeyPress = false; })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            services.AddHttpContextAccessor();

            services.Configure<IntegrationConfig>(Configuration.GetSection("apaleo:IntegrationConfig"));
            services.Configure<ServicesUriConfig>(Configuration.GetSection("apaleo:ServicesUri"));
            services.Configure<ApaleoConfig>(Configuration.GetSection("apaleo"));

            // Here we have a retry policy only for read-only requests such as GET or HEAD
            // In addition there is a waiting time for the circuit breaker to avoid too many requests per second to the apaleo api
            services.AddHttpClient<IApaleoClientsFactory, ApaleoClientsFactory>()
                .AddPolicyHandler(request =>
                    IsReadOnlyRequest(request)
                        ? HttpPolicyExtensions.HandleTransientHttpError()
                            .WaitAndRetryAsync(3, t => TimeSpan.FromSeconds(Math.Pow(2, t)))
                        : (IAsyncPolicy<HttpResponseMessage>)Policy.NoOpAsync<HttpResponseMessage>())
                .AddTransientHttpErrorPolicy(policy => policy.AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5,
                    samplingDuration: TimeSpan.FromSeconds(10),
                    minimumThroughput: 8,
                    durationOfBreak: TimeSpan.FromSeconds(30)));

            services.AddResponseCompression(options => { options.EnableForHttps = true; });
            services.AddHttpsRedirection(options => { options.HttpsPort = 443; });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardedHeaders = ForwardedHeaders.All;
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "apaleo";
                })
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.Cookie.Name = "apaleo_auth";
                    options.Cookie.SameSite = SameSiteMode.None;
                })
                .AddAutomaticTokenRefresh()
                .AddOpenIdConnect("apaleo", options =>
                {
                    options.Authority = "https://identity.apaleo.com/";

                    options.ClientSecret = Configuration["apaleo:ClientSecret"];
                    options.ClientId = Configuration["apaleo:ClientId"];
                    options.CallbackPath = "/signin-apaleo";

                    options.ResponseType = "code";

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
                    HerokuUtils.ConvertConnectionStringIfSet(Configuration["DATABASE_URL"]) ?? Configuration["ConnectionStrings:DefaultDatabase"],
                    npgSqlOptions => npgSqlOptions.UseNodaTime()));

            services.AddBlazoredToast();

            services.AddScoped<IRequestContext, RequestContext>();
            services.AddScoped<ITraceRepository, TraceRepository>();
            services.AddScoped<ITraceService, TraceService>();
            services.AddScoped<ITraceModifierService, TraceModifierService>();
            services.AddScoped<ITracesCollectorService, TracesCollectorService>();
            services.AddScoped<IApaleoOneService, ApaleoOneService>();
            services.AddScoped<IApaleoSetupService, ApaleoSetupService>();
            services.AddScoped<IApaleoRolesCollectorService, ApaleoRolesCollectorService>();
            services.AddScoped<IndexViewModel>();
            services.AddScoped<TracesAccountViewModel>();
            services.AddScoped<TracesReservationViewModel>();
            services.AddScoped<TracesPropertyViewModel>();
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
                app.UseForwardedHeaders();
            }

            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.ApplicationServices
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

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

        private static bool IsReadOnlyRequest(HttpRequestMessage request) =>
            request.Method == HttpMethod.Get || request.Method == HttpMethod.Head ||
            request.Method == HttpMethod.Options;
    }
}
