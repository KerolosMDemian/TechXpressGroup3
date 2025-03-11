
using System.Security.Principal;

namespace TechXpress.Data.ValueObjects
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get;}
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string ZipCode { get; }
        private Address()
        {
            Street = string.Empty;
            City = string.Empty;
            State = string.Empty;
            Country = string.Empty;
            ZipCode = string.Empty;
        }
        public Address(string street , string city , string state , string country , string zipCode )
        {
            if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Street Cannot be Empty");
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City Cannot be Empty");
            if (string.IsNullOrWhiteSpace(state)) throw new ArgumentException("State Cannot be Empty");
            if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country Cannot be Empty");
            if (string.IsNullOrWhiteSpace(zipCode)) throw new ArgumentException("Zip Code Cannot be Empty");
            Street =street;
            City=city;
            State=state;
            Country=country;
            ZipCode =zipCode;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Address other) return false;
            return Street == other.Street && 
                City == other.City &&
                State == other.State &&
                Country == other.Country && 
                ZipCode == other.ZipCode;
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(Street, City, State, Country,ZipCode);
        }

    }
}
