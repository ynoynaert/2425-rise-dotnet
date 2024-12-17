namespace Rise.Shared.Customers;

public class CustomerDto
{
    public class Index
    {
        public required int Id { get; set; }
        public required string Name { get; set; }

    }

    public class Create
    {
        public required string Name { get; set; }
        public required string Street { get; set; }
        public required string StreetNumber { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
    }

    public class Detail
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Street { get; set; }
        public required string StreetNumber { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
    }
}
