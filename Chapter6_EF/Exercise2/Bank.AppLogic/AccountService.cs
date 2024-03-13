using System;
using System.Linq;
using Bank.AppLogic.Contracts;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.AppLogic
{
    internal class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Result AddNewAccountForCustomer(Customer customer, string accountNumber, AccountType type)
        {
            if (customer.Accounts.Any(account => account.AccountType == type))
            {
                return Result.Fail($"Customer already has an account of type {type}");
            }

            if (_accountRepository.GetByAccountNumber(accountNumber) != null)
            {
                return Result.Fail($"An account with number {accountNumber} already exists");
            }

            var newAccount = Account.CreateNewForCustomer(customer.Id, accountNumber, type);
            _accountRepository.Add(newAccount);
            return Result.Success();
        }

        public Result TransferMoney(string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            var fromAccount = _accountRepository.GetByAccountNumber(fromAccountNumber);
            var toAccount = _accountRepository.GetByAccountNumber(toAccountNumber);

            if (fromAccount == null || toAccount == null)
            {
                throw new InvalidOperationException("One of the account numbers in the transaction is invalid.");
            }

            if (fromAccount.AccountType == AccountType.YouthAccount)
            {
                if (fromAccount.Balance < amount)
                {
                    return Result.Fail("Insufficient funds");
                }
            }

            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            _accountRepository.CommitChanges();

            return Result.Success();
        }
    }
}