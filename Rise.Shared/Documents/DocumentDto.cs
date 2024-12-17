using Rise.Shared.Customers;
using Rise.Shared.Quotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rise.Shared.Quotes.QuoteDto;

namespace Rise.Shared.Documents;

public class DocumentDto
{
    public class Index
    {
        public string? QuoteOrOrderNumber { get; set; }
        public string? CurrentDate { get; set; }
        public CustomerDto.Detail? Customer { get; set; }
        public string? SalespersonId { get; set; }
        public string? TopText { get; set; }
        public string? BottomText { get; set; }
        public string? MachineName { get; set; }
        public decimal? TotalWithoutVat { get; set; }
        public decimal? TotalWithVat { get; set; }
        public List<MainOptionDto>? MainOptions { get; set; }
        public List<QuoteOptionDto.OptionInfo>? QuoteOptions { get; set; }
        public List<TradedMachineryDto.Index>? TradedMachineries { get; set; }
    }
}
