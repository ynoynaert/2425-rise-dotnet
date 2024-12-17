using Rise.Shared.Customers;
using Rise.Shared.Machineries;

namespace Rise.Shared.Quotes;

public class QuoteDto
{
    public class Index
    {
        public required string QuoteNumber { get; set; }
        public int Id { get; set; }
        public required bool IsApproved { get; set; }
        public int NewQuoteId { get; set; }
        public required CustomerDto.Index Customer { get; set; }
        public required DateTime Date { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TotalWithoutVat { get; set; }
        public decimal TotalWithVat { get; set; }
        public required MachineryDto.Index Machinery { get; set; }
        public List<QuoteOptionDto.OptionInfo> QuoteOptions { get; set; } = new();
        public List<MainOptionDto> MainOptions { get; set; } = new();
        public required string SalespersonId { get; set; }
        public string? TopText { get; set; }
        public string? BottomText { get; set; }
    }

    public class Create
    {
        public string? QuoteNumber { get; set; }
        public bool IsApproved { get; set; }
        public int NewQuoteId { get; set; }
        public required int CustomerId { get; set; }
        public required DateTime Date { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TotalWithoutVat { get; set; }
        public decimal TotalWithVat { get; set; }
        public required int MachineryId { get; set; }
        public required string SalespersonId { get; set; }
        public List<MainOptionDto> MainOptions { get; set; } = new();
        public string? TopText { get; set; }
        public string? BottomText { get; set; }

    }

    public class Update
    {
        public required string quoteNumber { get; set; }
        public string? TopText { get; set; }
        public string? BottomText { get; set; }
    }

    public class ExcelRow
    {
        public string? Position { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsCategory { get; set; }
    }

    public class MainOptionDto
    {
        public string? Category { get; set; }
        public List<string>? Options { get; set; }
    }

    public class ExcelModel
    {
        public int Id { get; set; }
        public string? QuoteNumber { get; set; }
        public string? CurrentDate { get; set; }
        public string? MachineName { get; set; }
        public int MachineId { get; set; }
        public decimal? BasePrice { get; set; }
        public decimal? TotalWithoutVat { get; set; } 
        public decimal? TotalWithVat { get; set; }
        public List<QuoteOptionDto.OptionInfo> QuoteOptions { get; set; } = new();
        public List<MainOptionDto> MainOptions { get; set; } = new();
        public string? TopText { get; set; }
        public string? BottomText { get; set; }
        public CustomerDto.Detail Customer { get; set; } = new CustomerDto.Detail

        {
            Id = 0,
            Name = string.Empty,
            Street = string.Empty,
            StreetNumber = string.Empty,
            City = string.Empty,
            PostalCode = string.Empty,
            Country = string.Empty
        };
        public string? SalespersonId { get; set; }
        public bool IsApproved { get; set; }
        public int NewQuoteId { get; set; }

        public List<TradedMachineryDto.Index> TradedMachineries { get; set; } = new();
	}

    public class Android
    {
        public required string QuoteNumber { get; set; }
        public int Id { get; set; }
        public required bool IsApproved { get; set; }
        public int NewQuoteId { get; set; }
        public required CustomerDto.Detail Customer { get; set; }
        public required DateTime Date { get; set; }
        public decimal TotalWithoutVat { get; set; }
        public decimal TotalWithVat { get; set; }
        public required MachineryDto.Index Machinery { get; set; }
        public List<QuoteOptionDto.OptionInfo> QuoteOptions { get; set; } = new();
        public List<MainOptionDto> MainOptions { get; set; } = new();
        public string? TopText { get; set; }
        public string? BottomText { get; set; }
        public List<TradedMachineryDto.Index> TradedMachineries { get; set; } = new();
        public required string? SalespersonName { get; set; }
    }
}


