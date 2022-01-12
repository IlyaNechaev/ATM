using ATMApplication.Models;
using ATMApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Data
{
    public class ATMDbContext : DbContext
    {
        ISecurityService SecurityService { get; init; }
        string ConnectionString { get; init; }

        public DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }

        public ATMDbContext(DbContextOptions<ATMDbContext> builder,
                               [FromServices] ISecurityService securityService,
                               [FromServices] IConfiguration configuration)
        {
            SecurityService = securityService;

            ConnectionString = configuration.GetConnectionString("DefaultConnection");

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
