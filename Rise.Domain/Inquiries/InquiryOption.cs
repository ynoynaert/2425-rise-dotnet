using Rise.Domain.Machineries;

namespace Rise.Domain.Inquiries;

public class InquiryOption : Entity
{
    private Inquiry inquiry = default!;
    private MachineryOption machineryOption = default!;

    public Inquiry Inquiry
    {
        get => inquiry;
        set => inquiry = Guard.Against.Null(value);
    }

    public MachineryOption MachineryOption
    {
        get => machineryOption;
        set => machineryOption = Guard.Against.Null(value);
    }
}
