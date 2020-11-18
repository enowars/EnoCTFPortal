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
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authentication.OAuth;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
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
            services.AddAuthentication(configureOptions =>
            {
                configureOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(configureOptions =>
               {
                   configureOptions.IncludeErrorDetails = true;
               })
               .AddOAuth("ctftime.org", configureOptions =>
               {
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
               });
            services.AddControllers();
            services.AddDbContextPool<LandingPageDatabaseContext>(options => options.UseSqlite(LandingPageDatabaseContext.CONNECTIONSTRING));
            services.AddScoped<ILandingPageDatabase, LandingPageDatabase>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EnoLandingPage", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILandingPageDatabase db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EnoLandingPageBackend v1"));
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            db.Migrate();
        }
    }
}
