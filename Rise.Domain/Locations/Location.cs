using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Domain.Locations;

public class Location : Entity
{
    private string name = default!;
    private string street = default!;
    private string streetNumber = default!;
    private string city = default!;
    private string postalCode = default!;
    private string country = default!;
    private string image = default!;
    private string phoneNumber = default!;
    private string vatNumber = default!;
    private string code = default!;

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

    public string Image
    {
        get => image;
        set => image = Guard.Against.NullOrWhiteSpace(value);
    }

    public string PhoneNumber
    {
        get => phoneNumber;
        set => phoneNumber = Guard.Against.NullOrWhiteSpace(value);
    }

    public string VatNumber
    {
        get => vatNumber;
        set => vatNumber = Guard.Against.NullOrWhiteSpace(value);
    }

    public string Code
    {
        get => code;
        set => code = Guard.Against.NullOrWhiteSpace(value);
    }
}
