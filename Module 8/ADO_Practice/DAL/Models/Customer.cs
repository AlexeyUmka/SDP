using DAL.Attributes;

namespace DAL.Models
{
    public class Customer
    {
        [Identifier]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }
}