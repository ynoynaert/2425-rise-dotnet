using Rise.Shared.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Locations;

public class LocationDto
{
    public class Index
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Street { get; set; }
        public required string StreetNumber { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
        public required string Image { get; set; }
        public required string PhoneNumber { get; set; }
        public required string VatNumber { get; set; }
        public required string Code { get; set; }
    }

    public class Detail
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Street { get; set; }
        public required string StreetNumber { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
        public required string Image { get; set; }
        public required string PhoneNumber { get; set; }
        public required string VatNumber { get; set; }
        public required string Code { get; set; }
        public IEnumerable<UserDto.Index>? SalesPeople { get; set; }
    }
}
