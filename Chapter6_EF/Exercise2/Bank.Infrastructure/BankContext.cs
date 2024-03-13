using System.Collections.Generic;
using Bank.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure
{
    internal class BankContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<City> Cities { get; set; }

        public BankContext() { }

        public BankContext(DbContextOptions<BankContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) //only configure the connection if the parameter-less constructor was used (no options where provided).
            {
                string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BankDB;Integrated Security=True";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(a => a.AccountNumber);
            modelBuilder.Entity<City>().HasKey(c => c.ZipCode);

            modelBuilder.Entity<Customer>().Property(c => c.Name).IsRequired();
            modelBuilder.Entity<Customer>().Property(c => c.FirstName).IsRequired();

            modelBuilder.Entity<Customer>().HasOne(c => c.City).WithMany().HasForeignKey(c => c.ZipCode);

            //Seed data
            var cities = new List<City>
            {
                new City{Name = "Antwerpen", ZipCode = 2000},
                new City{Name = "Brugge", ZipCode = 8000},
                new City{Name = "Gent", ZipCode = 9000},
                new City{Name = "Hasselt", ZipCode = 3500},
                new City{Name = "Leuven", ZipCode = 3000},
            };
            modelBuilder.Entity<City>().HasData(cities);

            base.OnModelCreating(modelBuilder);
        }

        public void CreateOrUpdateDatabase()
        {
            this.Database.Migrate();
        }
    }
}
