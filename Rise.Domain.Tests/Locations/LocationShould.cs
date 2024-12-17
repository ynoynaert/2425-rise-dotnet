using Rise.Domain.Locations;
using Shouldly;

namespace Rise.Domain.Tests.Locations;

public class LocationShould
{
    [Fact]
    public void BeCreated()
    {
        Location location = new Location
        {
            Name = "Test Location",
            Street = "Test Street",
            StreetNumber = "1",
            City = "Test City",
            PostalCode = "1234",
            Country = "Test Country",
            Image = "image",
            PhoneNumber = "123456789",
            VatNumber = "123456789",
            Code = "LOC-1234"
        };

        location.Name.ShouldBe("Test Location");
        location.Street.ShouldBe("Test Street");
        location.StreetNumber.ShouldBe("1");
        location.City.ShouldBe("Test City");
        location.PostalCode.ShouldBe("1234");
        location.Country.ShouldBe("Test Country");
        location.Image.ShouldBe("image");
        location.PhoneNumber.ShouldBe("123456789");
        location.VatNumber.ShouldBe("123456789");
        location.Code.ShouldBe("LOC-1234");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidName(string? name)
    {
        Action act = () =>
        {
            Location location = new Location
            {
                Name = name!,
                Street = "Test Street",
                StreetNumber = "1",
                City = "Test City",
                PostalCode = "1234",
                Country = "Test Country",
                Image = "image",
                PhoneNumber = "123456789",
                VatNumber = "123456789",
                Code = "LOC-1234"
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidStreet(string? street)
    {
        Action act = () =>
        {
            Location location = new Location
            {
                Name = "Test Location",
                Street = street!,
                StreetNumber = "1",
                City = "Test City",
                PostalCode = "1234",
                Country = "Test Country",
                Image = "image",
                PhoneNumber = "123456789",
                VatNumber = "123456789",
                Code = "LOC-1234"
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidStreetNumber(string? streetNumber)
    {
        Action act = () =>
        {
            Location location = new Location
            {
                Name = "Test Location",
                Street = "Test Street",
                StreetNumber = streetNumber!,
                City = "Test City",
                PostalCode = "1234",
                Country = "Test Country",
                Image = "image",
                PhoneNumber = "123456789",
                VatNumber = "123456789",
                Code = "LOC-1234"
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidCity(string? city)
    {
        Action act = () =>
        {
            Location location = new Location
            {
                Name = "Test Location",
                Street = "Test Street",
                StreetNumber = "1",
                City = city!,
                PostalCode = "1234",
                Country = "Test Country",
                Image = "image",
                PhoneNumber = "123456789",
                VatNumber = "123456789",
                Code = "LOC-1234"
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidPostalCode(string? postalCode)
    {
        Action act = () =>
        {
            Location location = new Location
            {
                Name = "Test Location",
                Street = "Test Street",
                StreetNumber = "1",
                City = "Test City",
                PostalCode = postalCode!,
                Country = "Test Country",
                Image = "image",
                PhoneNumber = "123456789",
                VatNumber = "123456789",
                Code = "LOC-1234"
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidCountry(string? country)
    {
        Action act = () =>
        {
            Location location = new Location
            {
                Name = "Test Location",
                Street = "Test Street",
                StreetNumber = "1",
                City = "Test City",
                PostalCode = "1234",
                Country = country!,
                Image = "image",
                PhoneNumber = "123456789",
                VatNumber = "123456789",
                Code = "LOC-1234"
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidImage(string? image)
    {
        Action act = () =>
        {
            Location location = new Location
            {
                Name = "Test Location",
                Street = "Test Street",
                StreetNumber = "1",
                City = "Test City",
                PostalCode = "1234",
                Country = "Test Country",
                Image = image!,
                PhoneNumber = "123456789",
                VatNumber = "123456789",
                Code = "LOC-1234"
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidPhoneNumber(string? phoneNumber)
    {
        Action act = () =>
        {
            Location location = new Location
            {
                Name = "Test Location",
                Street = "Test Street",
                StreetNumber = "1",
                City = "Test City",
                PostalCode = "1234",
                Country = "Test Country",
                Image = "image",
                PhoneNumber = phoneNumber!,
                VatNumber = "123456789",
                Code = "LOC-1234"
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidVatNumber(string? vatNumber)
    {
        Action act = () =>
        {
            Location location = new Location
            {
                Name = "Test Location",
                Street = "Test Street",
                StreetNumber = "1",
                City = "Test City",
                PostalCode = "1234",
                Country = "Test Country",
                Image = "image",
                PhoneNumber = "123456789",
                VatNumber = vatNumber!,
                Code = "LOC-1234"
            };
        };

        act.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NotBeCreatedWithAnInvalidCode(string? code)
    {
        Action act = () =>
        {
            Location location = new Location
            {
                Name = "Test Location",
                Street = "Test Street",
                StreetNumber = "1",
                City = "Test City",
                PostalCode = "1234",
                Country = "Test Country",
                Image = "image",
                PhoneNumber = "123456789",
                VatNumber = "123456789",
                Code = code!
            };
        };

        act.ShouldThrow<ArgumentException>();
    }
}
