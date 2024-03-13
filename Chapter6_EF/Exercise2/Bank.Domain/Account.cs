using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Bank.Domain
{
    public class Account : INotifyPropertyChanged
    {
        [MaxLength(100)]
        public string AccountNumber { get; set; }

        private decimal _balance;
        public decimal Balance
        {
            get => _balance;
            set
            {
                _balance = value;
                OnPropertyChanged();
            }
        }

        public AccountType AccountType { get; set; }

        public Customer Customer { get; set; }
        public int CustomerId { get; set; }

        private Account() //Needed for EF
        {
        }

        public static Account CreateNewForCustomer(int customerId, string accountNumber, AccountType type)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                throw new ArgumentException($"Invalid accountNumber: {accountNumber}");
            }

            if (customerId <= 0)
            {
                throw new ArgumentException($"Invalid customer id: {customerId}");
            }

            var account = new Account
            {
                Balance = 100,
                AccountNumber = accountNumber,
                AccountType = type,
                CustomerId = customerId
            };
            return account;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
