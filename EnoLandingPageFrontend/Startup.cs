using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EnoLandingPageFrontend.Data;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using EnoLandingPageCore;

namespace EnoLandingPageFrontend
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
            services.Configure<EnoLandingPageSettings>(Configuration.GetSection("EnoLandingPage"));
            var challengePadSettings = Configuration
                .GetSection("ChallengePad")
                .Get<EnoLandingPageSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddOAuth("ctftime.org", configureOptions =>
                {
                    /*
                    configureOptions.ClientId = "1043";
                    configureOptions.ClientSecret = "8fa93a6a0571e05522eda6982bab9557300ba8bb59a931b274dcf2381728592c";
                    configureOptions.AuthorizationEndpoint = "https://oauth.ctftime.org/authorize";
                    configureOptions.TokenEndpoint = "https://oauth.ctftime.org/token";
                    configureOptions.UserInformationEndpoint = "https://oauth.ctftime.org/user";
                    configureOptions.CallbackPath = "/oauth2callback";
                    configureOptions.Scope.Add("team:read");
                    configureOptions.ClaimActions.MapJsonSubKey(ClaimTypes.NameIdentifier, "team", "id");
                    configureOptions.ClaimActions.MapJsonSubKey(ClaimTypes.Name, "team", "name");
                    configureOptions.Events = new OAuthEvents()
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var info = await response.Content.ReadAsStringAsync();
                            var user = JsonDocument.Parse(info);

                            context.RunClaimActions(user.RootElement);
                        },
                    };
                    */

                    configureOptions.ClientId = "e5e8ec6c3e0b228bc361ab8b48a1a7d7451679873c77109ae95ed921c85418164ed2d155310be18a";
                    configureOptions.ClientSecret = "243414e5696d1304d9963c2361a05a8097b9329792e4582d04beb067857644624e6255e349b459ffa2a01bb74112a365cd87527e6d332c";
                    configureOptions.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "uid");
                    configureOptions.CallbackPath = "/authorized";
                    configureOptions.AuthorizationEndpoint = "https://portal.enoflag.de/oauth/authorize";
                    configureOptions.TokenEndpoint = "https://portal.enoflag.de/oauth/token";
                    configureOptions.UserInformationEndpoint = "https://portal.enoflag.de/api/info";
                    configureOptions.Scope.Add("info");
                    configureOptions.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var info = await response.Content.ReadAsStringAsync();
                            var user = JsonDocument.Parse(info);

                            context.RunClaimActions(user.RootElement);
                        }
                    };
                });

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<EnoLandingPageDataService>();
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

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
