using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Inquiries;

public interface IInquiryOptionService
{
    Task<IEnumerable<InquiryOptionDto.Index>> GetInquiryOptionsAsync();
    Task<InquiryOptionDto.Index> GetInquiryOptionAsync(int id);
    Task<InquiryOptionDto.Index> CreateInquiryOptionAsync(InquiryOptionDto.Create inquiryDto);
    Task<IEnumerable<InquiryOptionDto.Index>> GetInquiryOptionsByInquiryIdAsync(int id);
    Task DeleteInquiryOptionAsync(int id);
}
