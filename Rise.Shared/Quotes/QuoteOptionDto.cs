using Rise.Shared.Machineries;

namespace Rise.Shared.Quotes;

public class QuoteOptionDto
{
    public class Index
    {
        public int Id { get; set; }
        public required QuoteDto.Index Quote { get; set; }
        public required MachineryOptionDto.Detail MachineryOption { get; set; }
    }

    public class Create
    {
        public int QuoteId { get; set; }
        public int MachineryOptionId { get; set; }
    }

    public class OptionInfo
    {
        public int Id { get; set; }
        public required MachineryOptionDto.XtremeDetail MachineryOption { get; set; }
    }

}
