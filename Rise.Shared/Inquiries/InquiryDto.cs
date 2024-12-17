using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rise.Shared.Machineries;

namespace Rise.Shared.Inquiries;

public static class InquiryDto
{
    public class Index
    {
        public int Id { get; set; }
        public required string CustomerName { get; set; }
        public required MachineryDto.Index Machinery { get; set; }
        public List<InquiryOptionDto.OptionInfo> InquiryOptions { get; set; } = new();
        public required string SalespersonId { get; set; }
        public required DateTime CreatedAt { get; set; }
    }

    public class Create
    {
        public required string ClientName { get; set; }
        public required int MachineryId { get; set; }
        public required string SalespersonId { get; set; }
        public List<MachineryOptionDto.Index> MachineryOptions { get; set; } = [];
    }
}
