using Lottery.Domain;
using Lottery.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Lottery.Tests
{
    internal abstract class DatabaseTests
    {
        private string _migrationError = string.Empty;

        [OneTimeSetUp]
        public void CreateDatabase()
        {
            using var context = CreateDbContext(false);

            context.Database.Migrate();

            //Check if migration succeeded
            try
            {
                context.Find<LotteryGame>(1);
                context.Find<Draw>(1);
            }
            catch (Exception e)
            {
                var messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("The migration (creation) of the database is not configured properly.");
                messageBuilder.AppendLine();
                messageBuilder.AppendLine(e.Message);
                _migrationError = messageBuilder.ToString();
            }
        }

        [OneTimeTearDown]
        public void DropDatabase()
        {
            using var context = CreateDbContext(false);
            context.Database.EnsureDeleted();
        }

        protected LotteryContext CreateDbContext(bool assertMigration = true)
        {
            if (assertMigration)
            {
                AssertMigratedSuccessfully();
            }

            // use the following connection string to connect to a real database on localDb
            string testDatabaseConnectionString =
                @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LotteryTestDB;Integrated Security=True";

            var options = new DbContextOptionsBuilder<LotteryContext>()
                .UseSqlServer(testDatabaseConnectionString)
                .Options;

            return new LotteryContext(options);
        }

        private void AssertMigratedSuccessfully()
        {
            if (!string.IsNullOrEmpty(_migrationError))
            {
                Assert.Fail(_migrationError);
            }
        }
    }
}