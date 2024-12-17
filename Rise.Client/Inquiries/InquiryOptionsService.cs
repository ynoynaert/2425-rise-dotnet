using System.Net.Http.Json;
using Rise.Shared.Inquiries;

namespace Rise.Client.Inquiries
{
    public class InquiryOptionsService(HttpClient httpClient) : IInquiryOptionService
    {
        public async Task<InquiryOptionDto.Index> CreateInquiryOptionAsync(InquiryOptionDto.Create inquiryDto)
        {
            var response = await httpClient.PostAsJsonAsync("InquiryOption", inquiryDto);
            var inquiryOption = await response.Content.ReadFromJsonAsync<InquiryOptionDto.Index>();
            return inquiryOption!;
        }

        public async Task DeleteInquiryOptionAsync(int id)
        {
            await httpClient.DeleteAsync($"InquiryOption/{id}");
        }

        public async Task<InquiryOptionDto.Index> GetInquiryOptionAsync(int id)
        {
            var response = await httpClient.GetAsync($"InquiryOption/{id}");
            var inquiryOption = await response.Content.ReadFromJsonAsync<InquiryOptionDto.Index>();
            return inquiryOption!;
        }

        public Task<IEnumerable<InquiryOptionDto.Index>> GetInquiryOptionsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InquiryOptionDto.Index>> GetInquiryOptionsByInquiryIdAsync(int id)
        {
            var response = await httpClient.GetFromJsonAsync<IEnumerable<InquiryOptionDto.Index>>($"InquiryOption/ByInquiry/{id}");
            return response!;
        }
    }
}
