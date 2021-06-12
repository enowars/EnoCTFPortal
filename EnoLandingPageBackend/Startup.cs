namespace EnoLandingPageBackend
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
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
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;

    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.Configuration = configuration;
            this.Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<LandingPageSettings>(this.Configuration.GetSection("EnoLandingPage"));
            var enoLandingPageSettings = this.Configuration
                .GetSection("EnoLandingPage")
                .Get<LandingPageSettings>();
            Validator.ValidateObject(enoLandingPageSettings, new ValidationContext(enoLandingPageSettings));
            services.AddSingleton(enoLandingPageSettings);

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[]
                    {
                        "application/octet-stream",
                    });
            });

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
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    if (!this.Environment.IsDevelopment())
                    {
                        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    }
                })
                .AddOAuth("ctftime.org", configureOptions =>
                {
                    configureOptions.Scope.Add("team:read");
                    configureOptions.ClaimActions.MapJsonSubKey(LandingPageClaimTypes.CtftimeId, "team", "id");
                    configureOptions.ClaimActions.MapJsonSubKey(ClaimTypes.Name, "team", "name");
                    configureOptions.ClientId = enoLandingPageSettings.OAuthClientId;
                    configureOptions.ClientSecret = enoLandingPageSettings.OAuthClientSecret;
                    configureOptions.CallbackPath = "/authorized";
                    configureOptions.AuthorizationEndpoint = "https://oauth.ctftime.org/authorize";
                    configureOptions.TokenEndpoint = "https://oauth.ctftime.org/token";
                    configureOptions.UserInformationEndpoint = "https://oauth.ctftime.org/user";
                    configureOptions.Scope.Add("team:read");
                    configureOptions.Events = new OAuthEvents
                    {
                        OnTicketReceived = async context =>
                        {
                            // This voodoo is necessary, because chrome/firefox do not allow strict cookies to be set during a redirect.
                            // Instead of a redirect we return a 200 with a http-equiv="refresh", which is totally ok because... well...
                            context.HandleResponse();
                            context.Response.ContentType = "text/html";
                            await context.HttpContext.SignInAsync(context.Principal!);
                            await context.Response.WriteAsync($"<html><head><meta http-equiv=\"refresh\" content=\"0; URL={context.ReturnUri}\"/></head><body><p>Moved to <a href=\"{context.ReturnUri}\" >{context.ReturnUri}</a>.</p></body></html>");
                            await context.Response.CompleteAsync();
                        },
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
            // Register Swagger services 
            // TODO: Add examples
            //services.AddSwaggerExamplesFromAssembyOf<ExampleClasses>()
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EnoLandingPage", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.EnableAnnotations();
                // TODO Maybe add Filters
                //c.DocumentFilter<DefaultFilter>();
                //c.DescribeAllEnumsAsStrings();
            });
            services.AddSingleton<HetznerCloudApi>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, LandingPageDatabase db)
        {
            app.UseForwardedHeaders();
            app.UseResponseCompression();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.DisplayOperationId();
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EnoLandingPageBackend v1");
                });
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

            app.UseStaticFiles();
            //app.UseSpaStaticFiles();


            var rewrite = new RewriteOptions()
                .AddRewrite("^$", "/index.html", true)
                .AddRewrite(@"^[\w\/]*$", "/index.html", true);
            app.UseRewriter(rewrite);
            app.UseStaticFiles(new StaticFileOptions()
            {
                ServeUnknownFileTypes = true,
                HttpsCompression = HttpsCompressionMode.Compress,
                DefaultContentType = "application/octet-stream",
            });
        }
    }
}
