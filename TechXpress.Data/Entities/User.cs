
using Microsoft.AspNetCore.Identity;
using TechXpress.Data.ValueObjects;

namespace TechXpress.Data.Entities
{
    public class User : IdentityUser
    {
        public string Name { get;  set; } = string.Empty;
        public Address Address { get; set; }
        public List<Order> Orders { get; private set; } = new List<Order>();
        public User()
        {
            Address = null!;
        }
        public User(string name, string email)
        {
            Name = name;
            Email = new EmailUser(email).ToString();
            UserName = email;
            Address = new Address("Street", "City", "State", "Country", "Zip Code");
        }
        public void UpdateAddress(Address newaddress) 
        { 
            
            Address = newaddress ?? throw new ArgumentException("Address Cannot Be Empty");
        
        
        }
    }
}
