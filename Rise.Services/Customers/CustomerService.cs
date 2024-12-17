using Microsoft.EntityFrameworkCore;
using Rise.Domain.Customers;
using Rise.Domain.Exceptions;
using Rise.Persistence;
using Rise.Shared.Customers;
using Rise.Shared.Translations;
using Serilog;

namespace Rise.Services.Customers;
public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext dbContext;
    private readonly ITranslationService TranslationService;

    public CustomerService(ApplicationDbContext dbContext, ITranslationService translationService)
    {
        this.dbContext = dbContext;
        this.TranslationService = translationService;
    }

    public async Task<IEnumerable<CustomerDto.Detail>> GetCustomersAsync()
    {
        IQueryable<CustomerDto.Detail> query = dbContext.Customers
            .Where(x => !x.IsDeleted)
            .Select(x => new CustomerDto.Detail
            {
                Id = x.Id,
                Name = x.Name,
                Street = x.Street,
                StreetNumber = x.StreetNumber,
                City = x.City,
                PostalCode = x.PostalCode,
                Country = x.Country,
            });

        var customers = await query.ToListAsync();
        Log.Information("Customers retrieved");
        return customers;
    }

    public async Task<CustomerDto.Detail> GetCustomerAsync(int id)
    {
        var customer = await dbContext.Customers
            .Where(x => !x.IsDeleted)
            .Select(x => new CustomerDto.Detail
            {
                Id = x.Id,
                Name = x.Name,
                Street = x.Street,
                StreetNumber = x.StreetNumber,
                City = x.City,
                PostalCode = x.PostalCode,
                Country = x.Country,
            }).SingleOrDefaultAsync(x => x.Id == id);

        Log.Information("Customer retrieved by id");
        return customer ?? throw new EntityNotFoundException("Klant", id);
    }

    public async Task<CustomerDto.Detail> GetCustomerByCustomerNameAsync(string name)
    {
        var customer = await dbContext.Customers
            .Where(x => !x.IsDeleted)
            .Select(x => new CustomerDto.Detail
            {
                Id = x.Id,
                Name = x.Name,
                Street = x.Street,
                StreetNumber = x.StreetNumber,
                City = x.City,
                PostalCode = x.PostalCode,
                Country = x.Country,
            }).SingleOrDefaultAsync(x => x.Name == name);
        
        Log.Information("Customer retrieved by name");
        return customer ?? throw new EntityNotFoundException("Klant", name);
    }

    public async Task<CustomerDto.Detail> CreateCustomerAsync(CustomerDto.Create customerDto)
    {
        var customer = new Customer
        {
            Name = customerDto.Name,
            Street = customerDto.Street,
            StreetNumber = customerDto.StreetNumber,
            City = customerDto.City,
            PostalCode = customerDto.PostalCode,
            Country = customerDto.Country,
        };

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync();
        Log.Information("Customer created");

        return new CustomerDto.Detail
        {
            Id = customer.Id,
            Name = customer.Name,
            Street = customer.Street,
            StreetNumber = customer.StreetNumber,
            City = customer.City,
            PostalCode = customer.PostalCode,
            Country = customer.Country,
        };
    }

    public async Task<CustomerDto.Detail> UpdateCustomerAsync(int id, CustomerDto.Detail customerDto)
    {
        var customer = await dbContext.Customers.SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Klant", id);

        customer.Name = customerDto.Name;
        customer.Street = customerDto.Street;
        customer.StreetNumber = customerDto.StreetNumber;
        customer.City = customerDto.City;
        customer.PostalCode = customerDto.PostalCode;
        customer.Country = customerDto.Country;

        await dbContext.SaveChangesAsync();
        Log.Information("Customer updated");

        return new CustomerDto.Detail
        {
            Id = id,
            Name = customer.Name,
            Street = customer.Street,
            StreetNumber = customer.StreetNumber,
            City = customer.City,
            PostalCode = customer.PostalCode,
            Country = customer.Country,
        };
    }

    public async Task DeleteCustomerAsync(int id)
    {
        var customer = await dbContext.Customers.SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("Klant", id);

        dbContext.Customers.Remove(customer);
        await dbContext.SaveChangesAsync();
        Log.Information("Customer deleted");
    }
}
