namespace CatenaccioStore.Core.Entities.Orders
{
    public class Address
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public Address()
        {
        }
        public Address(string firstName, string lastName, string city, string street, string zipCode)
        {
            FirstName = firstName;
            LastName = lastName;
            City = city;
            Street = street;
            ZipCode = zipCode;
        }
    }
}
