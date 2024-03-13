using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Bank.Domain
{
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string Address { get; set; }

        public City City { get; set; }
        public int ZipCode { get; set; }

        public ICollection<Account> Accounts { get; set; }

        public Customer()
        {
            Accounts = new List<Account>();
        }

        public Result Validate(IReadOnlyList<City> validCities)
        {
            if (string.IsNullOrEmpty(Name)) return Result.Fail("(Last)name is required.");
            if (string.IsNullOrEmpty(FirstName)) return Result.Fail("First name is required.");
            if (string.IsNullOrEmpty(Address)) return Result.Fail("Address is required.");
            if (validCities.All(city => city.ZipCode != ZipCode))
            {
                return Result.Fail("The zipcode is invalid.");
            }
            return Result.Success();
        }
    }
}