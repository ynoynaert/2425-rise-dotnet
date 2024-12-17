using Rise.Domain.Customers;
using Rise.Domain.Machineries;

namespace Rise.Domain.Quotes;

public class Quote : Entity
{
    private string quoteNumber = default!;
    private bool isApproved = false;
    private int newQuoteId = default;
    private DateTime date = default;
    private Customer customer = default!;
    private decimal basePrice = default;
    private decimal totalWithoutVat = default;
    private decimal totalWithVat = default;
    private Machinery machinery = default!;
    private readonly List<QuoteOption> quoteOptions = [];
    private string salespersonId = default!;
    private string mainOptions = default!;
    private readonly List<TradedMachinery> tradedMachineries = [];

    public string QuoteNumber
    {
        get => quoteNumber;
        set => quoteNumber = Guard.Against.NullOrWhiteSpace(value);
    }

    public bool IsApproved
    {
        get => isApproved;
        set => isApproved = value;
    }

    public int NewQuoteId
    {
        get => newQuoteId;
        set => newQuoteId = Guard.Against.Negative(value);
    }

    public DateTime Date
    {
        get => date;
        set => date = Guard.Against.Default(value);
    }

    public Customer Customer
    {
        get => customer;
        set => customer = Guard.Against.Null(value);

    }
    public decimal BasePrice
    {
        get => basePrice;
        set => basePrice = Guard.Against.NegativeOrZero(value);
    }
    public decimal TotalWithoutVat
    {
        get => totalWithoutVat;
        set => totalWithoutVat = Guard.Against.NegativeOrZero(value);
    }

    public decimal TotalWithVat
    {
        get => totalWithVat;
        set => totalWithVat = Guard.Against.NegativeOrZero(value);
    }

    public Machinery Machinery
    {
        get => machinery;
        set => machinery = Guard.Against.Null(value);
    }

    public IReadOnlyList<QuoteOption> QuoteOptions => quoteOptions.AsReadOnly();

    public string SalespersonId
    {
        get => salespersonId;
        set => salespersonId = Guard.Against.NullOrWhiteSpace(value);
    }

    public string MainOptions
    {
        get => mainOptions;
        set => mainOptions = Guard.Against.NullOrWhiteSpace(value);
    }

    public string? TopText { get; set; }
    public string? BottomText { get; set; }

    public IReadOnlyList<TradedMachinery> TradedMachineries => tradedMachineries.AsReadOnly();

    public void AddTradedMachinery(TradedMachinery tradedMachinery)
    {
        if (tradedMachineries.Any(m => m.Id == tradedMachinery.Id))
        {
            return;
        }

        tradedMachineries.Add(tradedMachinery);
    }


}
