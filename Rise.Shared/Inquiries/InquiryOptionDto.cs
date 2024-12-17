using Rise.Shared.Machineries;

namespace Rise.Shared.Inquiries;

public static class InquiryOptionDto
{
    public class  Index
    {
        public int Id { get; set; }
        public required InquiryDto.Index Inquiry { get; set; }
        public required MachineryOptionDto.Detail MachineryOption { get; set; }
    }

    public class Create
    {
        public int InquiryId { get; set; }
        public int MachineryOptionId { get; set; }
    }

    public class OptionInfo
    {
        public int Id { get; set; }
        public required MachineryOptionDto.XtremeDetail MachineryOption { get; set; }
    }
}
