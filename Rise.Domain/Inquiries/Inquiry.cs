using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rise.Domain.Machineries;

namespace Rise.Domain.Inquiries;

public class Inquiry : Entity
{
    private string customerName = default!;
    private Machinery machinery = default!;
    private readonly List<InquiryOption> inquiryOptions = [];
    private string salespersonId = default!;

    public string CustomerName
    {
        get => customerName;
        set => customerName = Guard.Against.NullOrWhiteSpace(value);
    }

    public Machinery Machinery
    {
        get => machinery;
        set => machinery = Guard.Against.Null(value);
    }

    public IReadOnlyList<InquiryOption> InquiryOptions => inquiryOptions.AsReadOnly();

    public void AddInquiryOption(InquiryOption inquiryOption)
    {
        inquiryOptions.Add(inquiryOption);
    }

    public void RemoveInquiryOption(InquiryOption inquiryOption)
    {
        inquiryOptions.Remove(inquiryOption);
    }

    public string SalespersonId
    {
        get => salespersonId;
        set => salespersonId = Guard.Against.NullOrWhiteSpace(value);
    }
}
