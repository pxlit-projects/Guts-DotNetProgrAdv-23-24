using System;
using System.Linq;
using Bank.Domain;
using Bank.Infrastructure;
using Guts.Client.Core;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Bank.Tests
{
    [ExerciseTestFixture("progAdvNet", "H06", "Exercise02", @"Bank.Infrastructure\AccountRepository.cs")]
    internal class AccountRepositoryTests : DatabaseTests
    {
        [MonitoredTest]
        public void GetByAccountNumber_AccountExists_ShouldReturnMatchingAccount()
        {
            string accountNumber;
            using (var context = CreateDbContext())
            {
                var existingCustomer = CreateExistingCustomer(context);
                Account existingAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id)
                    .WithBalance(RandomGenerator.Next(500, 1001)).Build();
                context.Set<Account>().Add(existingAccount);
                context.SaveChanges();
                accountNumber = existingAccount.AccountNumber;
            }

            using (var context = CreateDbContext())
            {
                var repo = new AccountRepository(context);

                //Act
                Account retrievedAccount = repo.GetByAccountNumber(accountNumber);

                Assert.That(retrievedAccount, Is.Not.Null);
                Assert.That(retrievedAccount.AccountNumber, Is.EqualTo(accountNumber));
            }
        }

        [MonitoredTest]
        public void GetByAccountNumber_AccountDoesNotExist_ShouldReturnNull()
        {
            using (var context = CreateDbContext())
            {
                var existingCustomer = CreateExistingCustomer(context);
                Account existingAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id)
                    .WithBalance(RandomGenerator.Next(500, 1001)).Build();
                context.Set<Account>().Add(existingAccount);
                context.SaveChanges();
            }

            string invalidAccountNumber = Guid.NewGuid().ToString();

            using (var context = CreateDbContext())
            {
                var repo = new AccountRepository(context);

                //Act
                Account retrievedAccount = repo.GetByAccountNumber(invalidAccountNumber);

                Assert.That(retrievedAccount, Is.Null);
            }
        }

        [MonitoredTest]
        public void Add_ShouldAddANewAccountToTheDatabase()
        {
            //Arrange
            Customer existingCustomer;
            using (var context = CreateDbContext())
            {
                existingCustomer = CreateExistingCustomer(context);
            }

            var newAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id).Build();

            using (var context = CreateDbContext())
            {
                var repo = new AccountRepository(context);

                //Act
                repo.Add(newAccount);

                //Assert
                var addedAccount = context.Set<Account>().FirstOrDefault(a => a.CustomerId == existingCustomer.Id);
                Assert.That(addedAccount, Is.Not.Null,
                    "The account is not added correctly to the database.");
                Assert.That(addedAccount.AccountNumber, Is.EqualTo(newAccount.AccountNumber),
                    "The 'AccountNumber' is not saved correctly.");
                Assert.That(addedAccount.AccountType, Is.EqualTo(newAccount.AccountType),
                    "The 'AccountType' is not saved correctly.");
                Assert.That(addedAccount.Balance, Is.EqualTo(newAccount.Balance),
                    "The 'Balance' is not saved correctly.");
            }
        }

        [MonitoredTest]
        public void Add_ShouldNotSaveCustomerRelationInADisconnectedScenario()
        {
            //Arrange
            Customer existingCustomer;
            using (var context = CreateDbContext())
            {
                existingCustomer = CreateExistingCustomer(context);
            }

            var newAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id).Build();
            existingCustomer.TrySetAccounts(null);
            newAccount.TrySetCustomer(existingCustomer);

            using (var context = CreateDbContext())
            {
                var repo = new AccountRepository(context);

                //Act
                try
                {
                    repo.Add(newAccount);
                }
                catch (DbUpdateException updateException)
                {
                    if (updateException.InnerException != null && updateException.InnerException.Message.ToLower()
                            .Contains("unique constraint"))
                    {
                        Assert.Fail(
                            "If the 'Customer' navigation property is set to an untracked instance of customer, " +
                            "the application tries to add the customer to the database. " +
                            "Make sure relations are not saved.");
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    Assert.Fail("Something went wrong when adding the Account.");
                }

            }
        }

        [MonitoredTest]
        public void Add_ShouldThrowArgumentExceptionWhenTheAccountAlreadyExists()
        {
            //Arrange
            Account existingAccount;
            using (var context = CreateDbContext())
            {
                var existingCustomer = CreateExistingCustomer(context);
                existingAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id).Build();
                context.Set<Account>().Add(existingAccount);
                context.SaveChanges();
            }

            using (var context = CreateDbContext())
            {
                var repo = new AccountRepository(context);

                //Act + Assert
                Assert.That(() => repo.Add(existingAccount), Throws.ArgumentException);
            }
        }

        [MonitoredTest]
        public void CommitChanges_ShouldSaveChangesOnTrackedEntities()
        {
            string accountNumber;
            using (var context = CreateDbContext())
            {
                var existingCustomer = CreateExistingCustomer(context);
                Account existingAccount = new AccountBuilder().WithCustomerId(existingCustomer.Id)
                    .WithBalance(RandomGenerator.Next(500, 1001)).Build();
                context.Set<Account>().Add(existingAccount);
                context.SaveChanges();
                accountNumber = existingAccount.AccountNumber;
            }

            decimal newBalance;
            using (var context = CreateDbContext())
            {
                var repo = new AccountRepository(context);

                Account account = context.Set<Account>().Find(accountNumber);
                Assert.That(account, Is.Not.Null, "Cannot retrieve saved account from database.");

                newBalance = account.Balance - RandomGenerator.Next(10, 101);
                account.Balance = newBalance;

                //Act
                repo.CommitChanges();
            }

            using (var context = CreateDbContext())
            {
                var updatedAccount = context.Set<Account>().Find(accountNumber);
                Assert.That(updatedAccount, Is.Not.Null, "Cannot retrieve saved account from database.");

                //Assert
                Assert.That(updatedAccount.Balance, Is.EqualTo(newBalance), "Balance of the 'account' is not updated correctly.");
            }
        }
    }
}
