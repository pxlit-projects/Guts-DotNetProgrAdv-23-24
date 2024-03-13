using System.ComponentModel.DataAnnotations;

namespace Bank.Domain
{
    public class City
    {
        public int ZipCode { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{ZipCode} - {Name}";
        }
    }
}