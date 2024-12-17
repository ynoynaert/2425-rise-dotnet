using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Customers;
using Serilog;

namespace Rise.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator, Verkoper")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    private readonly ICustomerService customerService = customerService;

    [HttpGet]
    public async Task<IEnumerable<CustomerDto.Detail>> Get()
    {
        var customers = await customerService.GetCustomersAsync();
        Log.Information("Customers retrieved");
        return customers;
    }

    [HttpGet("{id}")]
    public async Task<CustomerDto.Detail> Get(int id)
    {
        var customer = await customerService.GetCustomerAsync(id);
        Log.Information("Customer retrieved by id");
        return customer;
    }

    [HttpGet("name/{name}")]
    public async Task<CustomerDto.Detail> Get(string name)
    {
        var customer = await customerService.GetCustomerByCustomerNameAsync(name);
        Log.Information("Customer retrieved by name");
        return customer;
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto.Detail>> Post(CustomerDto.Create customer)
    {
        var newCustomer = await customerService.CreateCustomerAsync(customer);
        Log.Information("Customer created");
        return CreatedAtAction(nameof(Get), new { id = newCustomer.Id }, newCustomer);
    }

    [HttpPut]
    public async Task<ActionResult<CustomerDto.Detail>> Put(CustomerDto.Detail customer)
    {
        var updatedCustomer = await customerService.UpdateCustomerAsync(customer.Id, customer);
        Log.Information("Customer updated");
        return updatedCustomer;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await customerService.DeleteCustomerAsync(id);
        Log.Information("Customer deleted");
        return NoContent();
    }
}
