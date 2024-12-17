using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Inquiries;

public interface IInquiryService
{
    Task<IEnumerable<InquiryDto.Index>> GetInquiriesAsync();
    Task<InquiryDto.Index> GetInquiryAsync(int id);
    Task<IEnumerable<InquiryDto.Index>> GetInquiriesForSalespersonAsync(string userId);
    Task<InquiryDto.Index> GetInquiryForSalespersonAsync(int id, string userId);
    Task<InquiryDto.Index> CreateInquiryAsync(InquiryDto.Create inquiryDto);
    Task DeleteInquiryAsync(int id);
}
