// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace AuthServer
{
    public static class Config
    {
        // Her defineres OIDC-scopes
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        // Her defineres API-ressourcer
        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[] 
            {
                new ApiResource
                {
                    Name= "StreamingServer",

                    Scopes =
                    {
                        new Scope()
                        {
                            Name = "StreamingServer:Read",
                        },
                        new Scope
                        {
                            Name = "StreamingServer:Write",
                        }
                    }
                },
                new ApiResource
                {
                    Name= "UserDataApi",

                    Scopes =
                    {
                        new Scope()
                        {
                            Name = "UserDataApi",
                        }
                    }
                },
                new ApiResource
                {
                    Name= "CatalogApi",

                    Scopes =
                    {
                        new Scope()
                        {
                            Name = "CatalogApi:Write",
                        }
                    }
                },               
            };
        
        public static IEnumerable<Client> Clients =>
            new Client[] 
            {
                // Konsolbaseret ingest-applikation, der ikke har brugerinteraktion
                new Client 
                {
                    ClientId = "IngestApp",

                    // Ingen interaktiv bruger, brug client-id/secret til autentificering
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // Seret til autentificering
                    ClientSecrets = 
                    {
                        new Secret("secret".Sha256())
                    },

                    // Scopes som applikationen har adgang til
                    AllowedScopes = 
                    {
                        "CatalogApi:Write",
                        "StreamingServer:Write"
                    }
                },
                // React web-klient
                new Client
                {
                    ClientId = "SpaClient",

                    // Skal bruge Authorization Grant med PKCE-udvidelse
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    // Simpelt login
                    RequireConsent = false,

                    // Tillad CORS fra SpaClient
                    RedirectUris =              { "http://localhost:3000/authentication/callback" },
                    PostLogoutRedirectUris =    { "http://localhost:3000/" },
                    AllowedCorsOrigins =        { "http://localhost:3000" },

                    // De scopes som der er adgang til
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "StreamingServer:Read",
                        "UserDataApi"
                    }
                }
             };    
    }
}
