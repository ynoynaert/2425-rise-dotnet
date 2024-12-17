using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Rise.Domain.Inquiries;
using Rise.Domain.Quotes;
using Rise.Persistence;
using Rise.Shared.Inquiries;
using Rise.Shared.Machineries;
using Rise.Shared.Quotes;

namespace Rise.Services.Inquiries;

public class InquiryService : IInquiryService
{
    private readonly ApplicationDbContext dbContext;

    public InquiryService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<InquiryDto.Index> CreateInquiryAsync(InquiryDto.Create inquiryDto)
    {
        var machinery = await dbContext.Machineries.Include(x => x.Type).SingleOrDefaultAsync(x => x.Id == inquiryDto.MachineryId)
                        ?? throw new EntityNotFoundException("Machine", inquiryDto.MachineryId);

        var inquiry = new Inquiry
        {
            CustomerName = inquiryDto.ClientName,
            Machinery = machinery,
            SalespersonId = inquiryDto.SalespersonId,
        };

        foreach (var optionDto in inquiryDto.MachineryOptions)
        {
            var machineryOption = await dbContext.MachineryOptions
                .Include(x => x.Option)
                .ThenInclude(x => x.Category)
                .SingleOrDefaultAsync(mo => mo.Id == optionDto.Id)
                ?? throw new EntityNotFoundException("Machineoptie", optionDto.Id);

            var inquiryOption = new InquiryOption
            {
                Inquiry = inquiry,
                MachineryOption = machineryOption
            };

            inquiry.AddInquiryOption(inquiryOption);
        }

        dbContext.Inquiries.Add(inquiry);
        await dbContext.SaveChangesAsync();

        return new InquiryDto.Index
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
            CreatedAt = inquiry.CreatedAt,
            InquiryOptions = inquiry.InquiryOptions.Select(x =>
            {
                return new InquiryOptionDto.OptionInfo
                {
                    Id = x.Id,
                    MachineryOption = new MachineryOptionDto.XtremeDetail
                    {
                        Id = x.MachineryOption.Id,
                        Option = new OptionDto.Detail
                        {
                            Id = x.MachineryOption.Option.Id,
                            Name = x.MachineryOption.Option.Name,
                            Code = x.MachineryOption.Option.Code,
                            Category = new CategoryDto.Index
                            {
                                Id = x.MachineryOption.Option.Category.Id,
                                Name = x.MachineryOption.Option.Category.Name,
                                Code = x.MachineryOption.Option.Category.Code
                            }
                        },
                        Price = x.MachineryOption.Price
                    }
                };
            }).ToList()
        };


    }

    public Task DeleteInquiryAsync(int id)
    {
        var inquiry = dbContext.Inquiries.Find(id) ?? throw new EntityNotFoundException("Offertevoorstel", id);

        dbContext.Inquiries.Remove(inquiry);
        return dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<InquiryDto.Index>> GetInquiriesAsync()
    {
        IQueryable<InquiryDto.Index> query = dbContext.Inquiries
            .Where(x => !x.IsDeleted)
            .Select(x => new InquiryDto.Index
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                Machinery = new MachineryDto.Index
                {
                    Id = x.Machinery.Id,
                    Name = x.Machinery.Name,
                    SerialNumber = x.Machinery.SerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = x.Machinery.Type.Id,
                        Name = x.Machinery.Type.Name
                    },
                    Description = x.Machinery.Description
                },
                InquiryOptions = x.InquiryOptions.Select(y => new InquiryOptionDto.OptionInfo
                {
                    Id = y.Id,
                    MachineryOption = new MachineryOptionDto.XtremeDetail
                    {
                        Id = y.MachineryOption.Id,
                        Option = new OptionDto.Detail
                        {
                            Id = y.MachineryOption.Option.Id,
                            Name = y.MachineryOption.Option.Name,
                            Code = y.MachineryOption.Option.Code,
                            Category = new CategoryDto.Index
                            {
                                Id = y.MachineryOption.Option.Category.Id,
                                Name = y.MachineryOption.Option.Category.Name,
                                Code = y.MachineryOption.Option.Category.Code
                            }
                        },
                        Price = y.MachineryOption.Price
                    }
                }).ToList(),
                SalespersonId = x.SalespersonId,
                CreatedAt = x.CreatedAt,
            });

        var inquiries = await query.ToListAsync();
        return inquiries;
    }

    public async Task<InquiryDto.Index> GetInquiryAsync(int id)
    {
        var query = dbContext.Inquiries
            .Where(x => !x.IsDeleted && x.Id == id)
            .Select(x => new InquiryDto.Index
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                Machinery = new MachineryDto.Index
                {
                    Id = x.Machinery.Id,
                    Name = x.Machinery.Name,
                    SerialNumber = x.Machinery.SerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = x.Machinery.Type.Id,
                        Name = x.Machinery.Type.Name
                    },
                    Description = x.Machinery.Description
                },
                InquiryOptions = x.InquiryOptions.Select(y => new InquiryOptionDto.OptionInfo
                {
                    Id = y.Id,
                    MachineryOption = new MachineryOptionDto.XtremeDetail
                    {
                        Id = y.MachineryOption.Id,
                        Option = new OptionDto.Detail
                        {
                            Id = y.MachineryOption.Option.Id,
                            Name = y.MachineryOption.Option.Name,
                            Code = y.MachineryOption.Option.Code,
                            Category = new CategoryDto.Index
                            {
                                Id = y.MachineryOption.Option.Category.Id,
                                Name = y.MachineryOption.Option.Category.Name,
                                Code = y.MachineryOption.Option.Category.Code
                            }
                        },
                        Price = y.MachineryOption.Price
                    }
                }).ToList(),
                SalespersonId = x.SalespersonId,
                CreatedAt = x.CreatedAt,
            });

        var inquiry = await query.SingleOrDefaultAsync()
            ?? throw new EntityNotFoundException("Offertevoorstel", id);

        return inquiry;
    }



    public async Task<IEnumerable<InquiryDto.Index>> GetInquiriesForSalespersonAsync(string userId)
    {
        IQueryable<InquiryDto.Index> query = dbContext.Inquiries
            .Where(x => !x.IsDeleted)
            .Where(x => x.SalespersonId == userId)
            .Select(x => new InquiryDto.Index
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                Machinery = new MachineryDto.Index
                {
                    Id = x.Machinery.Id,
                    Name = x.Machinery.Name,
                    SerialNumber = x.Machinery.SerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = x.Machinery.Type.Id,
                        Name = x.Machinery.Type.Name
                    },
                    Description = x.Machinery.Description
                },
                InquiryOptions = x.InquiryOptions.Select(y => new InquiryOptionDto.OptionInfo
                {
                    Id = y.Id,
                    MachineryOption = new MachineryOptionDto.XtremeDetail
                    {
                        Id = y.MachineryOption.Id,
                        Option = new OptionDto.Detail
                        {
                            Id = y.MachineryOption.Option.Id,
                            Name = y.MachineryOption.Option.Name,
                            Code = y.MachineryOption.Option.Code,
                            Category = new CategoryDto.Index
                            {
                                Id = y.MachineryOption.Option.Category.Id,
                                Name = y.MachineryOption.Option.Category.Name,
                                Code = y.MachineryOption.Option.Category.Code
                            }
                        },
                        Price = y.MachineryOption.Price
                    }
                }).ToList(),
                SalespersonId = x.SalespersonId,
                CreatedAt = x.CreatedAt,
            });

        var inquiries = await query.ToListAsync();
        return inquiries;
    }

    public async Task<InquiryDto.Index> GetInquiryForSalespersonAsync(int id, string userId)
    {
        var query = dbContext.Inquiries
        .Where(x => !x.IsDeleted && x.Id == id && x.SalespersonId == userId)
        .Select(x => new InquiryDto.Index
        {
            Id = x.Id,
            CustomerName = x.CustomerName,
            Machinery = new MachineryDto.Index
            {
                Id = x.Machinery.Id,
                Name = x.Machinery.Name,
                SerialNumber = x.Machinery.SerialNumber,
                Type = new MachineryTypeDto.Index
                {
                    Id = x.Machinery.Type.Id,
                    Name = x.Machinery.Type.Name
                },
                Description = x.Machinery.Description
            },
            InquiryOptions = x.InquiryOptions.Select(y => new InquiryOptionDto.OptionInfo
            {
                Id = y.Id,
                MachineryOption = new MachineryOptionDto.XtremeDetail
                {
                    Id = y.MachineryOption.Id,
                    Option = new OptionDto.Detail
                    {
                        Id = y.MachineryOption.Option.Id,
                        Name = y.MachineryOption.Option.Name,
                        Code = y.MachineryOption.Option.Code,
                        Category = new CategoryDto.Index
                        {
                            Id = y.MachineryOption.Option.Category.Id,
                            Name = y.MachineryOption.Option.Category.Name,
                            Code = y.MachineryOption.Option.Category.Code
                        }
                    },
                    Price = y.MachineryOption.Price
                }
            }).ToList(),
            SalespersonId = x.SalespersonId,
            CreatedAt = x.CreatedAt
        });

        var inquiry = await query.SingleOrDefaultAsync()
            ?? throw new EntityNotFoundException("Offertevoorstel", id);

        return inquiry;
    }
}
