using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Models;
using MongoDB.Driver;

namespace IdentityServer.Store
{
    public class IdentityStore: DbContext
    {
        private readonly string _connectionString;
        private readonly string _databaseNamespace;

        public IdentityStore(string connectionString, string databaseNamespace)
        {
            _connectionString = connectionString;
            _databaseNamespace = databaseNamespace;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var mongoClient = new MongoClient(_connectionString);
            optionsBuilder.UseMongoDB(mongoClient, _databaseNamespace);
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<User> Users { get; init; }
        public DbSet<Role> Roles { get; init; }
        public DbSet<IdentityUserClaim<string>> IdentityUserClaims { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUserClaim<string>>()
                        .ToTable(nameof(IdentityUserClaims).Pluralize())
                        .HasKey(d => d.Id);
            modelBuilder.Entity<User>()
                        .ToTable(typeof(User).Name.Pluralize())
                        .HasKey(d => d.Id);
            modelBuilder.Entity<Role>()
                        .ToTable(typeof(Role).Name.Pluralize())
                        .HasKey(d => d.Id);
        }
    }
}

