namespace EnoLandingPageBackend
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text.Json;
    using System.Threading.Tasks;
    using EnoLandingPageBackend.Database;
    using EnoLandingPageBackend.Hetzner;
    using EnoLandingPageCore;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authentication.OAuth;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<LandingPageSettings>(this.Configuration.GetSection("EnoLandingPage"));
            var enoLandingPageSettings = this.Configuration
                .GetSection("EnoLandingPage")
                .Get<LandingPageSettings>();
            services.AddSingleton(enoLandingPageSettings);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
            services.AddAuthentication(configureOptions =>
            {
                configureOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.IsEssential = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                })
                .AddOAuth("ctftime.org", configureOptions =>
                {
                    configureOptions.Scope.Add("team:read");
                    configureOptions.ClaimActions.MapJsonSubKey(LandingPageClaimTypes.CtftimeId, "team", "id");
                    configureOptions.ClaimActions.MapJsonSubKey(ClaimTypes.Name, "team", "name");
                    configureOptions.ClaimActions.MapJsonKey(LandingPageClaimTypes.CtftimeId, "id");
                    configureOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "uid");
                    configureOptions.ClientId = enoLandingPageSettings.OAuthClientId;
                    configureOptions.ClientSecret = enoLandingPageSettings.OAuthClientSecret;
                    configureOptions.CallbackPath = "/authorized";
                    configureOptions.AuthorizationEndpoint = enoLandingPageSettings.OAuthAuthorizationEndpoint;
                    configureOptions.TokenEndpoint = enoLandingPageSettings.OAuthTokenEndpoint;
                    configureOptions.UserInformationEndpoint = enoLandingPageSettings.OAuthUserInformationEndpoint;
                    configureOptions.Scope.Add(enoLandingPageSettings.OAuthScope);
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
                        },
                    };
                });
            services.AddAuthorization();
            services.AddControllers();
            services.AddDbContextPool<LandingPageDatabaseContext>(options => options.UseSqlite(LandingPageDatabaseContext.CONNECTIONSTRING));
            services.AddScoped<LandingPageDatabase>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EnoLandingPage", Version = "v1" });
            });
            services.AddSingleton<IHostedService, HetznerCloudApi>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, LandingPageDatabase db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EnoLandingPageBackend v1"));
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            db.Migrate();
            var rewrite = new RewriteOptions()
                .AddRewrite("^$", "/index.html", true)
                .AddRewrite(@"^[\w\/]*$", "/index.html", true);
            app.UseRewriter(rewrite);
            app.UseStaticFiles(new StaticFileOptions()
            {
                ServeUnknownFileTypes = true,
            });
        }
    }
}
