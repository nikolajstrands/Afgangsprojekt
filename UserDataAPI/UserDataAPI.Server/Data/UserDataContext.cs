using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UserDataAPI.Models;
using System;

namespace UserDataAPI.Data
{
    // Konfiguration af Entity Framework
    public class UserAlbumContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("USER_DATA_CONNECTION_STRING"));
    }
}