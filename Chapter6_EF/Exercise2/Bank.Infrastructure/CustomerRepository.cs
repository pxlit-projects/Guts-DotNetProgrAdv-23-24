using System;
using System.Collections.Generic;
using System.Linq;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure
{
    internal class CustomerRepository : ICustomerRepository
    {
        private readonly BankContext _context;

        public CustomerRepository(BankContext context)
        {
            _context = context;
        }

        public IReadOnlyList<Customer> GetAllWithAccounts()
        {
            return _context.Customers.Include(c => c.Accounts).ToList();
        }

        public void Add(Customer newCustomer)
        {
            if (newCustomer.Id > 0)
            {
                throw new ArgumentException("Only new customers are allowed. This customer has an id", nameof(newCustomer.Id));
            }
            _context.Customers.Add(newCustomer);
            _context.SaveChanges();
        }
    }
}