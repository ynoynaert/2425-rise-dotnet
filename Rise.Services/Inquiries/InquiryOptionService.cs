using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Rise.Domain.Inquiries;
using Rise.Persistence;
using Rise.Shared.Inquiries;
using Rise.Shared.Machineries;

namespace Rise.Services.Inquiries;

public class InquiryOptionService : IInquiryOptionService
{
        private readonly ApplicationDbContext dbContext;

    public InquiryOptionService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<InquiryOptionDto.Index>> GetInquiryOptionsAsync()
    {
        var inquiryOptions = await dbContext.InquiryOptions
            .Where(x => !x.IsDeleted)
            .Select(x => new InquiryOptionDto.Index
            {
                Id = x.Id,
                Inquiry = new InquiryDto.Index
                {
                    Id = x.Inquiry.Id,
                    CustomerName = x.Inquiry.CustomerName,
                    Machinery = new MachineryDto.Index
                    {
                        Id = x.Inquiry.Machinery.Id,
                        Name = x.Inquiry.Machinery.Name,
                        SerialNumber = x.Inquiry.Machinery.SerialNumber,
                        Type = new MachineryTypeDto.Index
                        {
                            Id = x.Inquiry.Machinery.Type.Id,
                            Name = x.Inquiry.Machinery.Type.Name
                        },
                        Description = x.Inquiry.Machinery.Description
                    },
                    SalespersonId = x.Inquiry.SalespersonId,
                    CreatedAt = x.Inquiry.CreatedAt
                },
                MachineryOption = new MachineryOptionDto.Detail
                {
                    Id = x.MachineryOption.Id,
                    Machinery = new MachineryDto.Index
                    {
                        Id = x.MachineryOption.Machinery.Id,
                        Name = x.MachineryOption.Machinery.Name,
                        SerialNumber = x.MachineryOption.Machinery.SerialNumber,
                        Type = new MachineryTypeDto.Index
                        {
                            Id = x.MachineryOption.Machinery.Type.Id,
                            Name = x.MachineryOption.Machinery.Type.Name
                        },
                        Description = x.MachineryOption.Machinery.Description
                    },
                    Option = new OptionDto.Index
                    {
                        Id = x.MachineryOption.Option.Id,
                        Name = x.MachineryOption.Option.Name,
                        Code = x.MachineryOption.Option.Code
                    },
                    Price = x.MachineryOption.Price
                }
            }).ToListAsync();

        return inquiryOptions;
    }

    public async Task<InquiryOptionDto.Index> GetInquiryOptionAsync(int id)
    {
        var quoteOption = await dbContext.InquiryOptions
        .Where(x => !x.IsDeleted && x.Id == id)
        .Include(x => x.Inquiry)
        .Include(x => x.MachineryOption)
        .ThenInclude(x => x.Option)
        .Select(x => new InquiryOptionDto.Index
        {
            Id = x.Id,
            Inquiry = new InquiryDto.Index
            {
                Id = x.Inquiry.Id,
                CustomerName = x.Inquiry.CustomerName,
                Machinery = new MachineryDto.Index
                {
                    Id = x.Inquiry.Machinery.Id,
                    Name = x.Inquiry.Machinery.Name,
                    SerialNumber = x.Inquiry.Machinery.SerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = x.Inquiry.Machinery.Type.Id,
                        Name = x.Inquiry.Machinery.Type.Name
                    },
                    Description = x.Inquiry.Machinery.Description
                },
                SalespersonId = x.Inquiry.SalespersonId,
                CreatedAt = x.Inquiry.CreatedAt
            },
            MachineryOption = new MachineryOptionDto.Detail
            {
                Id = x.MachineryOption.Id,
                Machinery = new MachineryDto.Index
                {
                    Id = x.MachineryOption.Machinery.Id,
                    Name = x.MachineryOption.Machinery.Name,
                    SerialNumber = x.MachineryOption.Machinery.SerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = x.MachineryOption.Machinery.Type.Id,
                        Name = x.MachineryOption.Machinery.Type.Name
                    },
                    Description = x.MachineryOption.Machinery.Description
                },
                Option = new OptionDto.Index
                {
                    Id = x.MachineryOption.Option.Id,
                    Name = x.MachineryOption.Option.Name,
                    Code = x.MachineryOption.Option.Code
                },
                Price = x.MachineryOption.Price
            }
        }).SingleOrDefaultAsync();

        if(quoteOption is null)
        {
            throw new EntityNotFoundException("Offertevoorsteloptie", id);
        }

        return quoteOption;
    }

    public async Task<InquiryOptionDto.Index> CreateInquiryOptionAsync(InquiryOptionDto.Create inquiryDto)
    {
        var inquiry = await dbContext.Inquiries
            .Include(i => i.Machinery)
            .ThenInclude(m => m.Type)
            .SingleOrDefaultAsync(x => x.Id == inquiryDto.InquiryId)
            ?? throw new EntityNotFoundException("offertevoorstel", inquiryDto.InquiryId);

        var machineryOption = await dbContext.MachineryOptions
            .Include(mo => mo.Machinery)
            .ThenInclude(m => m.Type)
            .Include(mo => mo.Option)
            .SingleOrDefaultAsync(x => x.Id == inquiryDto.MachineryOptionId)
            ?? throw new EntityNotFoundException("Machineoptie", inquiryDto.MachineryOptionId);

        var existingOption = await dbContext.InquiryOptions
            .SingleOrDefaultAsync(x => x.Inquiry.Id == inquiryDto.InquiryId && x.MachineryOption.Id == inquiryDto.MachineryOptionId);

        if (existingOption is not null)
        {
            if (existingOption.IsDeleted)
            {
                existingOption.IsDeleted = false;
                await dbContext.SaveChangesAsync();

                return new InquiryOptionDto.Index
                {
                    Id = existingOption.Id,
                    Inquiry = new InquiryDto.Index
                    {
                        Id = inquiry.Id,
                        CustomerName = inquiry.CustomerName,
                        Machinery = new MachineryDto.Index
                        {
                            Id = inquiry.Machinery.Id,
                            Name = inquiry.Machinery.Name,
                            SerialNumber = inquiry.Machinery.SerialNumber,
                            Type = new MachineryTypeDto.Index
                            {
                                Id = inquiry.Machinery.Type.Id,
                                Name = inquiry.Machinery.Type.Name
                            },
                            Description = inquiry.Machinery.Description
                        },
                        SalespersonId = inquiry.SalespersonId,
                        CreatedAt = inquiry.CreatedAt
                    },
                    MachineryOption = new MachineryOptionDto.Detail
                    {
                        Id = machineryOption.Id,
                        Machinery = new MachineryDto.Index
                        {
                            Id = machineryOption.Machinery.Id,
                            Name = machineryOption.Machinery.Name,
                            SerialNumber = machineryOption.Machinery.SerialNumber,
                            Type = new MachineryTypeDto.Index
                            {
                                Id = machineryOption.Machinery.Type.Id,
                                Name = machineryOption.Machinery.Type.Name
                            },
                            Description = machineryOption.Machinery.Description
                        },
                        Option = new OptionDto.Index
                        {
                            Id = machineryOption.Option.Id,
                            Name = machineryOption.Option.Name,
                            Code = machineryOption.Option.Code
                        },
                        Price = machineryOption.Price
                    }
                };
            }
            else
            {
                throw new InvalidOperationException("Optie bestaat al");
            }
        }

        var newInquiryOption = new InquiryOption
        {
            Inquiry = inquiry,
            MachineryOption = machineryOption
        };

        dbContext.InquiryOptions.Add(newInquiryOption);
        await dbContext.SaveChangesAsync();

        return new InquiryOptionDto.Index
        {
            Id = newInquiryOption.Id,
            Inquiry = new InquiryDto.Index
            {
                Id = inquiry.Id,
                CustomerName = inquiry.CustomerName,
                Machinery = new MachineryDto.Index
                {
                    Id = inquiry.Machinery.Id,
                    Name = inquiry.Machinery.Name,
                    SerialNumber = inquiry.Machinery.SerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = inquiry.Machinery.Type.Id,
                        Name = inquiry.Machinery.Type.Name
                    },
                    Description = inquiry.Machinery.Description
                },
                SalespersonId = inquiry.SalespersonId,
                CreatedAt = inquiry.CreatedAt
            },
            MachineryOption = new MachineryOptionDto.Detail
            {
                Id = machineryOption.Id,
                Machinery = new MachineryDto.Index
                {
                    Id = machineryOption.Machinery.Id,
                    Name = machineryOption.Machinery.Name,
                    SerialNumber = machineryOption.Machinery.SerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = machineryOption.Machinery.Type.Id,
                        Name = machineryOption.Machinery.Type.Name
                    },
                    Description = machineryOption.Machinery.Description
                },
                Option = new OptionDto.Index
                {
                    Id = machineryOption.Option.Id,
                    Name = machineryOption.Option.Name,
                    Code = machineryOption.Option.Code
                },
                Price = machineryOption.Price
            }
        };
    }

    public async Task<IEnumerable<InquiryOptionDto.Index>> GetInquiryOptionsByInquiryIdAsync(int id)
    {
        var inquiryOptions = await dbContext.InquiryOptions
            .Where(x => !x.IsDeleted && x.Inquiry.Id == id)
            .Select(x => new InquiryOptionDto.Index
            {
                Id = x.Id,
                Inquiry = new InquiryDto.Index
                {
                    Id = x.Inquiry.Id,
                    CustomerName = x.Inquiry.CustomerName,
                    Machinery = new MachineryDto.Index
                    {
                        Id = x.Inquiry.Machinery.Id,
                        Name = x.Inquiry.Machinery.Name,
                        SerialNumber = x.Inquiry.Machinery.SerialNumber,
                        Type = new MachineryTypeDto.Index
                        {
                            Id = x.Inquiry.Machinery.Type.Id,
                            Name = x.Inquiry.Machinery.Type.Name
                        },
                        Description = x.Inquiry.Machinery.Description
                    },
                    SalespersonId = x.Inquiry.SalespersonId,
                    CreatedAt = x.Inquiry.CreatedAt
                },
                MachineryOption = new MachineryOptionDto.Detail
                {
                    Id = x.MachineryOption.Id,
                    Machinery = new MachineryDto.Index
                    {
                        Id = x.MachineryOption.Machinery.Id,
                        Name = x.MachineryOption.Machinery.Name,
                        SerialNumber = x.MachineryOption.Machinery.SerialNumber,
                        Type = new MachineryTypeDto.Index
                        {
                            Id = x.MachineryOption.Machinery.Type.Id,
                            Name = x.MachineryOption.Machinery.Type.Name
                        },
                        Description = x.MachineryOption.Machinery.Description
                    },
                    Option = new OptionDto.Index
                    {
                        Id = x.MachineryOption.Option.Id,
                        Name = x.MachineryOption.Option.Name,
                        Code = x.MachineryOption.Option.Code
                    },
                    Price = x.MachineryOption.Price
                }
            }).ToListAsync();

            return inquiryOptions;
    }

    public async Task DeleteInquiryOptionAsync(int id)
    {
        try
        {
            var inquiryOption = await dbContext.InquiryOptions
            .SingleOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException("offertevoorsteloptie", id);

            inquiryOption.IsDeleted = true;
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Er is iets fout gegaan bij het verwijderen van de optie", ex);
        }
    }
}
