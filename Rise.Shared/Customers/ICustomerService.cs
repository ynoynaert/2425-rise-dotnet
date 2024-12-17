using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Customers;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto.Detail>> GetCustomersAsync();
    Task<CustomerDto.Detail> GetCustomerAsync(int id);
    Task<CustomerDto.Detail> GetCustomerByCustomerNameAsync(string name);
    Task<CustomerDto.Detail> CreateCustomerAsync(CustomerDto.Create customerDto);
    Task<CustomerDto.Detail> UpdateCustomerAsync(int id, CustomerDto.Detail customerDto);
    Task DeleteCustomerAsync(int id);
}
