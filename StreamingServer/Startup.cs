using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace StreamingServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

             services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Environment.GetEnvironmentVariable("VM_AUTH_SERVER");
                    options.RequireHttpsMetadata = false;

                    options.Audience = "StreamingServer";
                });

            services.AddAuthorization(options =>
            {
                // Politik for skrive-scope
                options.AddPolicy("WriteAccess", policy => 
                                policy.RequireClaim("scope", "StreamingServer:Write"));
                
                // Politik for læse-scope
                options.AddPolicy("ReadAccess", policy => 
                                policy.RequireClaim("scope", "StreamingServer:Read"));
                                          
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
