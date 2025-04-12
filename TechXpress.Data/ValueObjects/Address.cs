public class Address
{
    public int Id { get; set; }
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string ZipCode { get; private set; }

    private Address()
    {
        Street = string.Empty;
        City = string.Empty;
        State = string.Empty;
        Country = string.Empty;
        ZipCode = string.Empty;
    }

    public Address(string street, string city, string state, string country, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Street Cannot be Empty");
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City Cannot be Empty");
        if (string.IsNullOrWhiteSpace(state)) throw new ArgumentException("State Cannot be Empty");
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country Cannot be Empty");
        if (string.IsNullOrWhiteSpace(zipCode)) throw new ArgumentException("Zip Code Cannot be Empty");

        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }
}
