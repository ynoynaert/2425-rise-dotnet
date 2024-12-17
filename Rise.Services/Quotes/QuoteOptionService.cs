using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Quotes;
using Rise.Domain.Quotes;
using Rise.Domain.Exceptions;
using Rise.Shared.Customers;
using Rise.Shared.Machineries;
using Ardalis.GuardClauses;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Rise.Shared.Helpers;
using Serilog;

namespace Rise.Services.Quotes
{
    public class QuoteOptionService : IQuoteOptionService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IQuoteService quoteService;

        public QuoteOptionService(ApplicationDbContext dbContext, IQuoteService quoteService)
        {
            this.dbContext = dbContext;
            this.quoteService = quoteService;
        }

        public async Task<QuoteOptionDto.Index> CreateQuoteOptionAsync(QuoteOptionDto.Create quoteDto)
        {
            var quote = await dbContext.Quotes
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.Customer)
                    .Include(x => x.Machinery)
                        .ThenInclude(m => m.Type)
                    .SingleAsync(x => x.Id == quoteDto.QuoteId);

            var machineryOption = await dbContext.MachineryOptions
                .Where(x => !x.IsDeleted)
                .Include(x => x.Machinery)
                    .ThenInclude(m => m.Type)
                .Include(x => x.Option)
                .SingleAsync(x => x.Id == quoteDto.MachineryOptionId);

            var existingQuoteOption = await dbContext.QuoteOptions
                .Where(qo => qo.Quote.Id == quoteDto.QuoteId && qo.MachineryOption.Id == quoteDto.MachineryOptionId)
                .SingleOrDefaultAsync();

            if (existingQuoteOption is not null)
            {
                Log.Information("QuoteOption already exists and is active.");
                if (existingQuoteOption.IsDeleted)
                {
                    Log.Information("QuoteOption is deleted, reactivating it.");
                    existingQuoteOption.IsDeleted = false;

                    quote.TotalWithoutVat += machineryOption.Price;
                    quote.TotalWithVat += machineryOption.Price * 1.21M;

                    await dbContext.SaveChangesAsync();
                    Log.Information("QuoteOption reactivated.");

                    return new QuoteOptionDto.Index
                    {
                        Id = existingQuoteOption.Id,
                        Quote = new QuoteDto.Index
                        {
                            Id = quote.Id,
                            QuoteNumber = quote.QuoteNumber,
                            Customer = new CustomerDto.Index
                            {
                                Id = quote.Customer.Id,
                                Name = quote.Customer.Name,
                            },
                            Date = quote.Date,
                            Machinery = new MachineryDto.Index
                            {
                                Id = quote.Machinery.Id,
                                Name = quote.Machinery.Name,
                                SerialNumber = quote.Machinery.SerialNumber,
                                Type = new MachineryTypeDto.Index
                                {
                                    Id = quote.Machinery.Type.Id,
                                    Name = quote.Machinery.Type.Name
                                },
                                Description = quote.Machinery.Description
                            },
                            IsApproved = quote.IsApproved,
                            SalespersonId = quote.SalespersonId,
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
                    Log.Warning("QuoteOption already exists and is active.");
                    throw new InvalidOperationException("The QuoteOption already exists and is active.");
                }
            }

            var newQuoteOption = new QuoteOption
            {
                Quote = quote,
                MachineryOption = machineryOption
            };

            quote.TotalWithoutVat += machineryOption.Price;
            quote.TotalWithVat += machineryOption.Price * 1.21M;

            dbContext.QuoteOptions.Add(newQuoteOption);
            await dbContext.SaveChangesAsync();
            Log.Information("QuoteOption created.");

            return new QuoteOptionDto.Index
            {
                Id = newQuoteOption.Id,
                Quote = new QuoteDto.Index
                {
                    Id = quote.Id,
                    QuoteNumber = quote.QuoteNumber,
                    Customer = new CustomerDto.Index
                    {
                        Id = quote.Customer.Id,
                        Name = quote.Customer.Name,

                    },
                    Date = quote.Date,
                    Machinery = new MachineryDto.Index
                    {
                        Id = quote.Machinery.Id,
                        Name = quote.Machinery.Name,
                        SerialNumber = quote.Machinery.SerialNumber,
                        Type = new MachineryTypeDto.Index
                        {
                            Id = quote.Machinery.Type.Id,
                            Name = quote.Machinery.Type.Name
                        },
                        Description = quote.Machinery.Description
                    },
                    IsApproved = quote.IsApproved,
                    SalespersonId = quote.SalespersonId,
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

        public async Task DeleteQuoteOptionAsync(int id)
        {
            try
            {
                Log.Information("Deleting QuoteOption with id {id}", id);
                var quoteOption = await dbContext.QuoteOptions
                    .Include(qo => qo.Quote)
                    .Include(qo => qo.MachineryOption)
                    .Where(x => !x.IsDeleted)
                    .SingleOrDefaultAsync(x => x.Id == id);

                if (quoteOption is null)
                {
                    Log.Information("QuoteOption with id {id} not found", id);
                    throw new EntityNotFoundException("Offerteoptie", id);
                }

                var quote = quoteOption.Quote;

                decimal optionPrice = quoteOption.MachineryOption.Price;
                quote.TotalWithoutVat -= optionPrice;
                quote.TotalWithVat -= optionPrice * 1.21M;

                dbContext.QuoteOptions.Remove(quoteOption);
                Log.Information("QuoteOption deleted.");

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Warning($"Error in DeleteQuoteOptionAsync: {ex.Message}");
                throw;
            }
        }


        public async Task<QuoteOptionDto.Index> GetQuoteOptionAsync(int id)
        {
            var quoteOption = await dbContext.QuoteOptions
                .Where(x => !x.IsDeleted && x.Id == id)
                .Include(x => x.Quote)
                    .ThenInclude(q => q.Customer) 
                .Include(x => x.MachineryOption)
                    .ThenInclude(mo => mo.Machinery)
                        .ThenInclude(m => m.Type) 
                .Include(x => x.MachineryOption)
                    .ThenInclude(mo => mo.Option)
                .Select(x => new QuoteOptionDto.Index
                {
                    Id = x.Id,
                    Quote = new QuoteDto.Index
                    {
                        Id = x.Quote.Id,
                        QuoteNumber = x.Quote.QuoteNumber,
                        Customer = new CustomerDto.Index
                        {
                            Id = x.Quote.Customer.Id,
                            Name = x.Quote.Customer.Name,

                        },
                        Date = x.Quote.Date,
                        Machinery = new MachineryDto.Index
                        {
                            Id = x.Quote.Machinery.Id,
                            Name = x.Quote.Machinery.Name,
                            SerialNumber = x.Quote.Machinery.SerialNumber,
                            Type = new MachineryTypeDto.Index
                            {
                                Id = x.Quote.Machinery.Type.Id,
                                Name = x.Quote.Machinery.Type.Name
                            },
                            Description = x.Quote.Machinery.Description
                        },
                        IsApproved = x.Quote.IsApproved,
                        SalespersonId = x.Quote.SalespersonId,
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
                    },

                    }).SingleOrDefaultAsync();

            if (quoteOption is null)
            {
                Log.Warning("QuoteOption not found by id");
                throw new EntityNotFoundException("Offerteoptie", id);
            }

            Log.Information("QuoteOption retrieved by id");
            return quoteOption;
        }

        public async Task<IEnumerable<QuoteOptionDto.Index>> GetQuoteOptionsByQuoteNumberAsync(string quoteNumber)
        {
            IQueryable<QuoteOptionDto.Index> query = dbContext.QuoteOptions
                .Where(x => !x.IsDeleted && x.Quote.QuoteNumber == quoteNumber)
                .Select(x => new QuoteOptionDto.Index
                {
                    Id = x.Id,
                    Quote = new QuoteDto.Index
                    {
                        Id = x.Quote.Id,
                        QuoteNumber = x.Quote.QuoteNumber,
                        Customer = new CustomerDto.Index
                        {
                            Id = x.Quote.Customer.Id,
                            Name = x.Quote.Customer.Name,
                        },
                        Date = x.Quote.Date,
                        Machinery = new MachineryDto.Index
                        {
                            Id = x.Quote.Machinery.Id,
                            Name = x.Quote.Machinery.Name,
                            SerialNumber = x.Quote.Machinery.SerialNumber,
                            Type = new MachineryTypeDto.Index
                            {
                                Id = x.Quote.Machinery.Type.Id,
                                Name = x.Quote.Machinery.Type.Name
                            },
                            Description = x.Quote.Machinery.Description
                        },
                        IsApproved = x.Quote.IsApproved,
                        SalespersonId = x.Quote.SalespersonId,
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
                });

            Log.Information("QuoteOptions retrieved by QuoteNumber");
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<QuoteOptionDto.Index>> GetQuoteOptionsAsync()
        {
            IQueryable<QuoteOptionDto.Index> query = dbContext.QuoteOptions
                .Where(x => !x.IsDeleted)
                .Select(x => new QuoteOptionDto.Index
                {
                    Id = x.Id,
                    Quote = new QuoteDto.Index
                    {
                        Id = x.Quote.Id,
                        QuoteNumber = x.Quote.QuoteNumber,
                        Customer = new CustomerDto.Index
                        {
                            Id = x.Quote.Customer.Id,
                            Name = x.Quote.Customer.Name,

                        },
                        Date = x.Quote.Date,
                        Machinery = new MachineryDto.Index
                        {
                            Id = x.Quote.Machinery.Id,
                            Name = x.Quote.Machinery.Name,
                            SerialNumber = x.Quote.Machinery.SerialNumber,
                            Type = new MachineryTypeDto.Index
                            {
                                Id = x.Quote.Machinery.Type.Id,
                                Name = x.Quote.Machinery.Type.Name
                            },
                            Description = x.Quote.Machinery.Description
                        },
                        IsApproved = x.Quote.IsApproved,
                        SalespersonId = x.Quote.SalespersonId,
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
                });

            Log.Information("QuoteOptions retrieved");
            return await query.ToListAsync();
        }
    }
}
