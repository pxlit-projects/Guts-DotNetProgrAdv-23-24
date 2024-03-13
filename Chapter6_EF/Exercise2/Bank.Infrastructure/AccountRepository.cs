using System;
using System.Linq;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure
{
    internal class AccountRepository : IAccountRepository
    {
        private readonly BankContext _context;

        public AccountRepository(BankContext context)
        {
            _context = context;
        }

        public Account GetByAccountNumber(string accountNumber)
        {
            return _context.Accounts.FirstOrDefault(account => account.AccountNumber == accountNumber);
        }

        public void Add(Account newAccount)
        {
            if (GetByAccountNumber(newAccount.AccountNumber) != null)
            {
                throw new ArgumentException($"An account with the number {newAccount.AccountNumber} already exists.",
                    nameof(newAccount));
            }
            var entry = _context.Entry(newAccount);
            entry.State = EntityState.Added;

            _context.SaveChanges();
        }

        public void CommitChanges()
        {
            _context.SaveChanges();
        }
    }
}
