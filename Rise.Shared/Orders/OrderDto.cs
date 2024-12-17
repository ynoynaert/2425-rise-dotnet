using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rise.Shared.Quotes;

namespace Rise.Shared.Orders;

public class OrderDto
{
    public class Index
    {
        public required int Id { get; set; }
        public required string OrderNumber { get; set; } = default!;
        public required QuoteDto.Index Quote { get; set; } = default!;
        public required DateTime Date { get; set; } = default;
        public required bool IsCancelled { get; set; } = default;
    }

    public class Create
    {
        public required string OrderNumber { get; set; } = default!;
        public required int QuoteId { get; set; } = default!;
        public required DateTime Date { get; set; } = default;
    }

    public class Update
    {
        public required string OrderNumber { get; set; } = default!;
        public required QuoteDto.ExcelModel Quote { get; set; } = default!;
        public required DateTime Date { get; set; } = default;
        public required bool IsCancelled { get; set; } = default;
    }

    public class Detail
    {
        public required int Id { get; set; }
        public required string OrderNumber { get; set; } = default!;
        public required QuoteDto.ExcelModel Quote { get; set; } = default!;
        public required DateTime Date { get; set; } = default;
        public required bool IsCancelled { get; set; } = default;
    }

    public class Android
    {
        public required int Id { get; set; }
        public required string OrderNumber { get; set; } = default!;
        public required QuoteDto.Android Quote { get; set; } = default!;
        public required DateTime Date { get; set; } = default;
        public required bool IsCancelled { get; set; } = default;
    }
}
