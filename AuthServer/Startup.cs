// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;

namespace AuthServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureNonBreakingSameSiteCookies();

            // MVC-grænseflade
            services.AddControllersWithViews();

            // Certifikatets kodeord hentes fra environment
            string certPassword = System.Environment.GetEnvironmentVariable("CERT_PW");

            // Indlæs et RSA-certifikat til digital signering
            var rsaCertificate = new X509Certificate2("certificate.pfx", certPassword);

            var builder = services.AddIdentityServer()
                // Tilføj data om id'er, api'er og klienter fra Config-klasse
                .AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryClients(Config.Clients)
                // Tilføj testbrugere
                .AddTestUsers(TestUsers.Users)
                // Tilføj certifikat
                .AddSigningCredential(rsaCertificate);


            // builder.Services.ConfigureExternalCookie(options => {
            // options.Cookie.IsEssential = true;
            //     options.Cookie.SameSite = (SameSiteMode)(-1); //SameSiteMode.Unspecified in .NET Core 3.1
            // });

            // builder.Services.ConfigureApplicationCookie(options => {
            // options.Cookie.IsEssential = true;
            //     options.Cookie.SameSite = (SameSiteMode)(-1); //SameSiteMode.Unspecified in .NET Core 3.1
            // });

        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCookiePolicy();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
