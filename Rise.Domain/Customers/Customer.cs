namespace Rise.Domain.Customers
{
    public class Customer : Entity
    {
        private string name = default!;
        private string street = default!;
        private string streetNumber = default!;
        private string city = default!;
        private string postalCode = default!;
        private string country = default!;

        public string Name
        {
            get => name;
            set => name = Guard.Against.NullOrWhiteSpace(value);
        }

        public string Street
        {
            get => street;
            set => street = Guard.Against.NullOrWhiteSpace(value);
        }

        public string StreetNumber
        {
            get => streetNumber;
            set => streetNumber = Guard.Against.NullOrWhiteSpace(value);
        }

        public string City
        {
            get => city;
            set => city = Guard.Against.NullOrWhiteSpace(value);
        }

        public string PostalCode
        {
            get => postalCode;
            set => postalCode = Guard.Against.NullOrWhiteSpace(value);
        }

        public string Country
        {
            get => country;
            set => country = Guard.Against.NullOrWhiteSpace(value);
        }
    }
}
