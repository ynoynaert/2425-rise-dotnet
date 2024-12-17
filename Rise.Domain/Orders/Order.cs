using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rise.Domain.Quotes;

namespace Rise.Domain.Orders;

public class Order : Entity
{
    private string orderNumber = default!;
    private Quote quote = default!;
    private DateTime date = default;
    private bool isCancelled = false;

    public string OrderNumber
    {
        get => orderNumber;
        set => orderNumber = Guard.Against.NullOrWhiteSpace(value);
    }

    public Quote Quote
    {
        get => quote;
        set => quote = Guard.Against.Null(value);
    }
    public DateTime Date
    {
        get => date;
        set => date = Guard.Against.Default(value);
    }

    public bool IsCancelled
    {
        get => isCancelled;
        set => isCancelled = value;
    }

}
