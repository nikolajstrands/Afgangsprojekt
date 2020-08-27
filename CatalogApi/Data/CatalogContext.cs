using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CatalogApi.Models;

namespace CatalogApi.Data
{
    // Her konfigureres Entity Framework
    public class CatalogContext : DbContext
    {
        public DbSet<Album> Albums { get; set; }
        public DbSet<Track> Tracks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
           => optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("CATALOG_CONNECTION_STRING"));

    }
}