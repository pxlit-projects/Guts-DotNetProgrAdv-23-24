using Bank.Domain;
using Bank.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Tests
{
    public class DatabaseTests
    {
        private const string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BankTestDB;Integrated Security=True";
        private static bool _databaseInitialized;

        protected Random RandomGenerator;


        public DatabaseTests()
        {
            RandomGenerator = new Random();
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateDbContext())
                {
                    context.Database.EnsureCreated();
                }
                _databaseInitialized = true;
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (_databaseInitialized)
            {
                using (var context = CreateDbContext())
                {
                    context.Database.EnsureDeleted();
                }
                _databaseInitialized = false;
            }
        }

        internal BankContext CreateDbContext()
        => new BankContext(
            new DbContextOptionsBuilder<BankContext>()
                .UseSqlServer(ConnectionString)
                .Options);


        internal City CreateExistingCity(BankContext context)
        {
            var existingCity = new City { Name = Guid.NewGuid().ToString() };
            context.Add(existingCity);
            context.SaveChanges();
            return existingCity;
        }

        internal Customer CreateExistingCustomer(BankContext context)
        {
            City existingCity = CreateExistingCity(context);
            Customer existingCustomer = new CustomerBuilder().WithZipCode(existingCity.ZipCode).Build();
            context.Set<Customer>().Add(existingCustomer);
            context.SaveChanges();
            return existingCustomer;
        }
    }
}
