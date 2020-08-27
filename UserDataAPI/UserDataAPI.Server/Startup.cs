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
using UserDataAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace UserDataAPI.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                // Hent autorisationsservers adresse fra environemt
                options.Authority = Environment.GetEnvironmentVariable("VM_AUTH_SERVER");
                options.RequireHttpsMetadata = false;

                options.Audience = "UserDataApi";
            });

            services.AddAuthorization(options =>
            {
                // Politik for læse- og skrive-scope
                options.AddPolicy("ReadAndWriteAccess", policy => 
                                policy.RequireClaim("scope", "UserDataApi"));
                                          
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

            services.AddScoped<IUserDataRepo, UserDataRepo>();

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
