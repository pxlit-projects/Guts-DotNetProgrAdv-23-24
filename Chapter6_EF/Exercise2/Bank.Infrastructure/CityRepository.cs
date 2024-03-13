using System.Collections.Generic;
using System.Linq;
using Bank.AppLogic.Contracts.DataAccess;
using Bank.Domain;

namespace Bank.Infrastructure
{
    internal class CityRepository : ICityRepository
    {
        private readonly BankContext _context;

        public CityRepository(BankContext context)
        {
            _context = context;
        }

        public IReadOnlyList<City> GetAllOrderedByZipCode()
        {
            return _context.Cities.OrderBy(c => c.ZipCode).ToList();
        }
    }
}