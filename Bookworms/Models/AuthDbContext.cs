﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bookworms.Models
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration _configuration;
        //public AuthDbContext(DbContextOptions<AuthDbContext> options):base(options){ }
        public AuthDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DbSet<AuditLog> AuditLogs { get; set; } 

        public DbSet<PasswordHistory> PasswordHistories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("AuthConnectionString"); optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
