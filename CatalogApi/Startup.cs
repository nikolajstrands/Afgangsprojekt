using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CatalogApi.Data;

namespace CatalogApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Kaldes ved runtime. Bruges til at tilføje services.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Bruger JWT's som Bearer Tokens
            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                // Hent autorisationsservers adresse fra environemt
                options.Authority =  Environment.GetEnvironmentVariable("VM_AUTH_SERVER");
                options.RequireHttpsMetadata = false;

                // Tokens skal være udstedt til dette API
                options.Audience = "CatalogApi";
            });

            // Tilføj adgangspolitikker
            services.AddAuthorization(options =>
            {
                // Politik for skrive-scope
                options.AddPolicy("WriteAccess", policy => 
                                policy.RequireClaim("scope", "CatalogApi:Write"));
                                          
            });

            services.AddCors(options => 
            {
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddScoped<ICatalogRepo, CatalogRepo>();
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseRouting();

            app.UseCors("default");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
