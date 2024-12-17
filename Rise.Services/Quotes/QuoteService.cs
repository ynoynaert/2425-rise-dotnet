using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Rise.Domain.Exceptions;
using Rise.Domain.Quotes;
using Rise.Persistence;
using Rise.Shared.Customers;
using Rise.Shared.Documents;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;
using Rise.Shared.Quotes;
using Rise.Shared.Translations;
using Rise.Shared.Users;
using Serilog;

namespace Rise.Services.Quotes;
public class QuoteService : IQuoteService
{
    private readonly ApplicationDbContext dbContext;
    private readonly ITranslationService TranslationService;
    private readonly ICustomerService CustomerService;
    private readonly IMachineryService MachineryService;
    private readonly IUserService userService;
    private readonly IDocumentService documentService;

    public QuoteService(ApplicationDbContext dbContext, ITranslationService translationService, ICustomerService customerService, IMachineryService machineryService, IUserService userService, IDocumentService documentService)
    {
        this.dbContext = dbContext;
        this.TranslationService = translationService;
        CustomerService = customerService;
        MachineryService = machineryService;
        this.userService = userService;
        this.documentService = documentService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<IEnumerable<QuoteDto.Index>> GetQuotesAsync(QuoteQueryObject queryObject)
    {
        IQueryable<QuoteDto.Index> query = dbContext.Quotes
            .Where(x => !x.IsDeleted)
            .Include(x => x.Customer)
            .Select(x => new QuoteDto.Index
            {
                Id = x.Id,
                IsApproved = x.IsApproved,
                NewQuoteId = x.NewQuoteId,
                Customer = new CustomerDto.Index
                {
                    Id = x.Customer.Id,
                    Name = x.Customer.Name,
                },
                Date = x.Date,
                QuoteNumber = x.QuoteNumber,
                BasePrice = x.BasePrice,
                TotalWithoutVat = x.TotalWithoutVat,
                TotalWithVat = x.TotalWithVat,
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
                SalespersonId = x.SalespersonId,
            });

        if (!string.IsNullOrEmpty(queryObject.Search))
        {
            query = query.Where(x => x.QuoteNumber.Contains(queryObject.Search) ||
                                     x.Customer.Name.Contains(queryObject.Search) ||
                                     x.Machinery.Name.Contains(queryObject.Search));
        }

        if (queryObject.Before.HasValue)
        {
            query = query.Where(x => x.Date < queryObject.Before.Value);
        }

        if (queryObject.After.HasValue)
        {
            query = query.Where(x => x.Date > queryObject.After.Value);
        }

        if (!string.IsNullOrEmpty(queryObject.Status))
        {
            if (queryObject.Status == "Goedgekeurd")
            {
                query = query.Where(q => q.IsApproved);
            }
            else if (queryObject.Status == "Verouderd")
            {
                query = query.Where(q => !q.IsApproved && q.NewQuoteId != 0);
            }
            else if (queryObject.Status == "In afwachting")
            {
                query = query.Where(q => !q.IsApproved && q.NewQuoteId == 0);
            }
        }
        // Sorting

        if (!string.IsNullOrEmpty(queryObject.SortBy))
        {
            query = queryObject.SortBy.ToLower() switch
            {
                "quotenumber" => queryObject.IsDescending ? query.OrderByDescending(x => x.QuoteNumber) : query.OrderBy(x => x.QuoteNumber),
                "date" => queryObject.IsDescending ? query.OrderByDescending(x => x.Date) : query.OrderBy(x => x.Date),
                "customername" => queryObject.IsDescending ? query.OrderByDescending(x => x.Customer.Name) : query.OrderBy(x => x.Customer.Name),
                "machineryname" => queryObject.IsDescending ? query.OrderByDescending(x => x.Machinery.Name) : query.OrderBy(x => x.Machinery.Name),
                _ => query
            };
        }


        var skip = (queryObject.PageNumber - 1) * queryObject.PageSize;
        query = query.Skip(skip).Take(queryObject.PageSize);

        var quotes = await query.ToListAsync();

        queryObject.HasNext = await dbContext.Quotes
            .Where(x => !x.IsDeleted)
            .Skip(skip + queryObject.PageSize)
            .AnyAsync();

        Log.Information("Quotes retrieved");
        return quotes;
    }

    public async Task<QuoteDto.Android> GetQuoteForSalespersonAsyncAndroid(string quoteNumber, string userId)
    {
        var salesperson = await userService.GetSalesPersonAsync(userId);
        var quote = await dbContext.Quotes
                    .Where(x => x.QuoteNumber == quoteNumber && !x.IsDeleted)
                    .Where(x => x.SalespersonId == userId)
                    .Include(x => x.Customer)
                    .Select(x => new 
                    {
                        Id = x.Id,
                        IsApproved = x.IsApproved,
                        NewQuoteId = x.NewQuoteId,
                        Customer = new CustomerDto.Detail
                        {
                            Id = x.Customer.Id,
                            Name = x.Customer.Name,
                            Street = x.Customer.Street,
                            StreetNumber = x.Customer.StreetNumber,
                            City = x.Customer.City,
                            PostalCode = x.Customer.PostalCode,
                            Country = x.Customer.Country
                        },
                        Date = x.Date,
                        QuoteNumber = x.QuoteNumber,
                        TotalWithoutVat = x.TotalWithoutVat,
                        TotalWithVat = x.TotalWithVat,
                        TopText = x.TopText,
                        BottomText = x.BottomText,
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
                        QuoteOptions = x.QuoteOptions.Select(qo => new QuoteOptionDto.OptionInfo
                        {
                            Id = qo.Id,
                            MachineryOption = new MachineryOptionDto.XtremeDetail
                            {
                                Id = qo.MachineryOption.Id,
                                Option = new OptionDto.Detail
                                {
                                    Id = qo.MachineryOption.Option.Id,
                                    Name = qo.MachineryOption.Option.Name,
                                    Code = qo.MachineryOption.Option.Code,
                                    Category = new CategoryDto.Index
                                    {
                                        Id = qo.MachineryOption.Option.Category.Id,
                                        Name = qo.MachineryOption.Option.Category.Name,
                                        Code = qo.MachineryOption.Option.Category.Code
                                    }
                                },
                                Price = qo.MachineryOption.Price
                            }
                        }).ToList(),
                        MainOptions = x.MainOptions,
                        TradedMachineries = x.TradedMachineries.Select(tm => new TradedMachineryDto.Index
                        {
                            Id = tm.Id,
                            Year = tm.Year,
                            Description = tm.Description,
                            EstimatedValue = tm.EstimatedValue,
                            Images = tm.Images.Select(i => new ImageDto.Index
                            {
                                Id = i.Id,
                                Url = i.Url
                            }).ToList(),
                            MachineryType = new MachineryTypeDto.Index
                            {
                                Id = tm.Type.Id,
                                Name = tm.Type.Name
                            },
                            Name = tm.Name,
                            SerialNumber = tm.SerialNumber,
                        }).ToList()
                    }).SingleOrDefaultAsync(x => x.QuoteNumber == quoteNumber) ?? throw new EntityNotFoundException("Offerte", quoteNumber);

        var parsedMainOptions = ParseMainOptions(quote.MainOptions);
        Log.Information("Quote retrieved by quote number");

        return new QuoteDto.Android
        {
            Id = quote.Id,
            QuoteNumber = quote.QuoteNumber,
            TotalWithoutVat = quote.TotalWithoutVat,
            TotalWithVat = quote.TotalWithVat,
            Customer = quote.Customer,
            QuoteOptions = quote.QuoteOptions,
            MainOptions = parsedMainOptions,
            IsApproved = quote.IsApproved,
            NewQuoteId = quote.NewQuoteId,
            Machinery = quote.Machinery,
            Date = quote.Date,
            TopText = quote.TopText,
            BottomText = quote.BottomText,
            TradedMachineries = quote.TradedMachineries,
            SalespersonName = salesperson.Name
        };
    }

    public async Task<QuoteDto.Android> GetQuoteAsyncAndroid(string quoteNumber)
    {
        var quote = await dbContext.Quotes
            .Where(x => !x.IsDeleted)
            .Include(x => x.Customer)
            .Include(x => x.TradedMachineries)
            .Select(x => new
            {
                Id = x.Id,
                IsApproved = x.IsApproved,
                NewQuoteId = x.NewQuoteId,
                Customer = new CustomerDto.Detail
                {
                    Id = x.Customer.Id,
                    Name = x.Customer.Name,
                    Street = x.Customer.Street,
                    StreetNumber = x.Customer.StreetNumber,
                    City = x.Customer.City,
                    PostalCode = x.Customer.PostalCode,
                    Country = x.Customer.Country
                },
                Date = x.Date,
                QuoteNumber = x.QuoteNumber,
                mainOptions = x.MainOptions,
                TotalWithoutVat = x.TotalWithoutVat,
                TotalWithVat = x.TotalWithVat,
                TopText = x.TopText,
                BottomText = x.BottomText,
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
                QuoteOptions = x.QuoteOptions.Select(qo => new QuoteOptionDto.OptionInfo
                {
                    Id = qo.Id,
                    MachineryOption = new MachineryOptionDto.XtremeDetail
                    {
                        Id = qo.MachineryOption.Id,
                        Option = new OptionDto.Detail
                        {
                            Id = qo.MachineryOption.Option.Id,
                            Name = qo.MachineryOption.Option.Name,
                            Code = qo.MachineryOption.Option.Code,
                            Category = new CategoryDto.Index
                            {
                                Id = qo.MachineryOption.Option.Category.Id,
                                Name = qo.MachineryOption.Option.Category.Name,
                                Code = qo.MachineryOption.Option.Category.Code
                            }
                        },
                        Price = qo.MachineryOption.Price
                    }
                }).ToList(),
                TradedMachineries = x.TradedMachineries.Select(tm => new TradedMachineryDto.Index
                {
                    Id = tm.Id,
                    Year = tm.Year,
                    Description = tm.Description,
                    EstimatedValue = tm.EstimatedValue,
                    Images = tm.Images.Select(i => new ImageDto.Index
                    {
                        Id = i.Id,
                        Url = i.Url
                    }).ToList(),
                    MachineryType = new MachineryTypeDto.Index
                    {
                        Id = tm.Type.Id,
                        Name = tm.Type.Name
                    },
                    Name = tm.Name,
                    SerialNumber = tm.SerialNumber,
                }).ToList(),
                SalesPersonId = x.SalespersonId
            }).SingleOrDefaultAsync(x => x.QuoteNumber == quoteNumber) ?? throw new EntityNotFoundException("Offerte", quoteNumber);

        var salesperson = await userService.GetSalesPersonAsync(quote.SalesPersonId);
        Log.Information("Quote retrieved by quote number");

        return new QuoteDto.Android
         {
             QuoteNumber = quoteNumber,
             Id = quote!.Id,
             IsApproved = quote.IsApproved,
             NewQuoteId = quote.NewQuoteId,
             Customer = quote.Customer,
             Date = quote.Date,
             TotalWithoutVat = quote.TotalWithoutVat,
             TotalWithVat = quote.TotalWithVat,
             Machinery = quote.Machinery,
             QuoteOptions = quote.QuoteOptions,
             MainOptions = ParseMainOptions(quote.mainOptions),
             TopText = quote.TopText,
             BottomText = quote.BottomText,
             TradedMachineries = quote.TradedMachineries,
             SalespersonName = salesperson.Name
        };

    }

    public async Task<QuoteDto.ExcelModel> GetQuoteAsync(string quoteNumber)
    {
        var quote = await dbContext.Quotes
            .Where(q => q.QuoteNumber == quoteNumber && !q.IsDeleted)
            .Include(q => q.Customer)
            .Include(q => q.Machinery)
            .Include(q => q.QuoteOptions)
                .ThenInclude(qo => qo.MachineryOption)
            .Select(q => new
            {
                Id = q.Id,
                QuoteNumber = q.QuoteNumber,
                CurrentDate = q.Date.ToString("dd/MM/yyyy"),
                TotalWithoutVat = q.TotalWithoutVat,
                TotalWithVat = q.TotalWithVat,
                Baseprice = q.BasePrice,
                TopText = q.TopText,
                BottomText = q.BottomText,
                Customer = new CustomerDto.Detail
                {
                    Id = q.Customer.Id,
                    Name = q.Customer.Name,
                    Street = q.Customer.Street,
                    StreetNumber = q.Customer.StreetNumber,
                    City = q.Customer.City,
                    PostalCode = q.Customer.PostalCode,
                    Country = q.Customer.Country
                },
                MachineId = q.Machinery.Id,
                MachineName = q.Machinery.Name,
                QuoteOptions = q.QuoteOptions.Where(qo => !qo.IsDeleted).Select(qo => new QuoteOptionDto.OptionInfo
                {
                    Id = qo.Id,
                    MachineryOption = new MachineryOptionDto.XtremeDetail
                    {
                        Id = qo.MachineryOption.Id,
                        Option = new OptionDto.Detail
                        {
                            Id = qo.MachineryOption.Option.Id,
                            Name = qo.MachineryOption.Option.Name,
                            Code = qo.MachineryOption.Option.Code,
                            Category = new CategoryDto.Index
                            {
                                Id = qo.MachineryOption.Option.Category.Id,
                                Name = qo.MachineryOption.Option.Category.Name,
                                Code = qo.MachineryOption.Option.Category.Code
                            }
                        },

                        Price = qo.MachineryOption.Price
                    }
                }).ToList(),
                MainOptions = q.MainOptions,

                SalespersonId = q.SalespersonId,
                IsApproved = q.IsApproved,
                NewQuoteId = q.NewQuoteId,
                TradedMachineries = q.TradedMachineries.Select(tm => new TradedMachineryDto.Index
                {
                    Id = tm.Id,
                    Year = tm.Year,
                    Description = tm.Description,
                    EstimatedValue = tm.EstimatedValue,
                    Images = tm.Images.Select(i => new ImageDto.Index
                    {
                        Id = i.Id,
                        Url = i.Url
                    }).ToList(),
                    MachineryType = new MachineryTypeDto.Index
                    {
                        Id = tm.Type.Id,
                        Name = tm.Type.Name
                    },
                    Name = tm.Name,
                    SerialNumber = tm.SerialNumber,
                }).ToList()
            })
            .SingleOrDefaultAsync();

        if (quote is null)
        {
            Log.Warning("Quote not found by quote number");
            throw new EntityNotFoundException("Offerte", quoteNumber);
        }

        var parsedMainOptions = ParseMainOptions(quote.MainOptions);
        Log.Information("Quote retrieved by quote number");

        return new QuoteDto.ExcelModel
        {
            Id = quote.Id,
            QuoteNumber = quote.QuoteNumber,
            CurrentDate = quote.CurrentDate,
            BasePrice = quote.Baseprice,
            TotalWithoutVat = quote.TotalWithoutVat,
            TotalWithVat = quote.TotalWithVat,
            TopText = quote.TopText,
            BottomText = quote.BottomText,
            Customer = quote.Customer,
            MachineId = quote.MachineId,
            MachineName = quote.MachineName,
            QuoteOptions = quote.QuoteOptions,
            MainOptions = parsedMainOptions,
            IsApproved = quote.IsApproved,
            NewQuoteId = quote.NewQuoteId,
            SalespersonId = quote.SalespersonId,
            TradedMachineries = quote.TradedMachineries
        };
    }

    public async Task<QuoteDto.Index> CreateQuoteAsync(QuoteDto.Create quoteDto)
    {
        var quoteExists = await dbContext.Quotes.AnyAsync(x => x.QuoteNumber == quoteDto.QuoteNumber);

        if (quoteExists)
        {
            Log.Warning("Quote already exists");
            throw new EntityAlreadyExistsException("Offerte", "offertenummer", quoteDto.QuoteNumber!);
        }

        var customer = await dbContext.Customers.SingleOrDefaultAsync(x => x.Id == quoteDto.CustomerId)
                       ?? throw new EntityNotFoundException("Klant", quoteDto.CustomerId);

        var machinery = await dbContext.Machineries.Include(x => x.Type).SingleOrDefaultAsync(x => x.Id == quoteDto.MachineryId)
                        ?? throw new EntityNotFoundException("Machine", quoteDto.MachineryId);

        string mainOptionsString = ConvertMainOptionsToString(quoteDto.MainOptions);

        var quote = new Quote
        {
            IsApproved = quoteDto.IsApproved,
            NewQuoteId = 0,
            Customer = customer,
            Date = quoteDto.Date,
            QuoteNumber = quoteDto.QuoteNumber!,
            TopText = quoteDto.TopText,
            BottomText = quoteDto.BottomText,
            BasePrice = quoteDto.BasePrice,
            TotalWithoutVat = quoteDto.TotalWithoutVat,
            TotalWithVat = quoteDto.TotalWithVat,
            Machinery = machinery,
            SalespersonId = quoteDto.SalespersonId,
            MainOptions = mainOptionsString
        };

        dbContext.Quotes.Add(quote);
        await dbContext.SaveChangesAsync();
        Log.Information("Quote created");

        return new QuoteDto.Index
        {
            Id = quote.Id,
            IsApproved = quote.IsApproved,
            NewQuoteId = quote.NewQuoteId,
            Customer = new CustomerDto.Index
            {
                Id = quote.Customer.Id,
                Name = quote.Customer.Name,
            },
            Date = quote.Date,
            QuoteNumber = quote.QuoteNumber,
            TopText = quote.TopText,
            BottomText = quote.BottomText,
            BasePrice = quote.BasePrice,
            TotalWithoutVat = quote.TotalWithoutVat,
            TotalWithVat = quote.TotalWithVat,
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
            SalespersonId = quote.SalespersonId,
        };
    }

    public async Task UpdateNewQuoteIdAsync(string quoteNumber, string newQuoteNumber)
    {
        var quote = await dbContext.Quotes
            .Include(x => x.Customer)
            .SingleOrDefaultAsync(x => x.QuoteNumber == quoteNumber)
            ?? throw new EntityNotFoundException("Offerte", quoteNumber);

        var newQuote = await dbContext.Quotes.SingleOrDefaultAsync(x => x.QuoteNumber == newQuoteNumber)
                       ?? throw new EntityNotFoundException("Offerte", newQuoteNumber);

        quote.NewQuoteId = newQuote.Id;
        Log.Information("New quote id updated");

        await dbContext.SaveChangesAsync();

    }

    public async Task<QuoteDto.ExcelModel> ImportFromExcelAsync(string fileBase64, string userId)
    {
        var rows = new List<QuoteDto.ExcelRow>();
        var mainOptionsByCategory = new Dictionary<string, List<string>>();

        var fileBytes = Convert.FromBase64String(fileBase64);
        using var memoryStream = new MemoryStream(fileBytes);
        using var workbook = new XLWorkbook(memoryStream);
        var worksheet = workbook.Worksheet(1);

        string quoteName = worksheet.Cell("A1").GetValue<string>();
        var currentDateName = DateTime.Now.ToString("yyyyMMdd");
        var quoteNumber = $"{currentDateName}{quoteName}-1";

        string currentDateRaw = worksheet.Cell("B1").GetValue<string>();
        string currentDate = DateTime.TryParse(currentDateRaw, out var parsedDate)
            ? parsedDate.ToString("dd/MM/yyyy")
            : currentDateRaw;
        string machineName = worksheet.Cell("B3").GetValue<string>();

        string customerInfo = worksheet.Cell("C1").GetValue<string>();
        string[] customerParts = customerInfo.Split(',');
        string customerName = customerParts.Length > 0 ? customerParts[0].Trim() : string.Empty;
        string street = customerParts.Length > 1 ? customerParts[1].Trim() : string.Empty;
        string streetNumber = street.Split(' ').LastOrDefault() ?? string.Empty;
        street = street.Replace(streetNumber, "").Trim();
        string city = customerParts.Length > 2 ? customerParts[2].Trim() : string.Empty;
        string postalCode = city.Split(' ').FirstOrDefault() ?? string.Empty;
        city = city.Replace(postalCode, "").Trim();
        string country = customerParts.Length > 3 ? customerParts[3].Trim() : string.Empty;

        string translatedStreet = await TranslationService.TranslateText(street);
        string translatedCity = await TranslationService.TranslateText(city);
        string translatedCountry = await TranslationService.TranslateText(country);

        CustomerDto.Detail customer;
        try
        {
            Log.Information("Customer retrieved by name");
            customer = await CustomerService.GetCustomerByCustomerNameAsync(customerName);
        }
        catch (EntityNotFoundException)
        {
            Log.Error("Customer not found by name");
            var customerDto = new CustomerDto.Create
            {
                Name = customerName,
                Street = translatedStreet,
                StreetNumber = streetNumber,
                City = translatedCity,
                PostalCode = postalCode,
                Country = translatedCountry
            };
            customer = await CustomerService.CreateCustomerAsync(customerDto);
        }

        decimal totalWithoutVAT = 0;
        decimal totalWithVAT = 0;
        string currentCategory = string.Empty;

        for (int row = 4; row <= worksheet.LastRowUsed()?.RowNumber(); row++)
        {
            var position = worksheet.Cell(row, 1).GetValue<string>();
            var description = worksheet.Cell(row, 2).GetValue<string>();

            if (string.IsNullOrWhiteSpace(position) && string.IsNullOrWhiteSpace(description))
            {
                Log.Information("Position and description are empty");
                break;
            }

            var quantityCell = worksheet.Cell(row, 3).GetValue<string>();
            var priceCell = worksheet.Cell(row, 4).GetValue<string>();

            bool isCategory = !string.IsNullOrWhiteSpace(description) &&
                              (string.IsNullOrWhiteSpace(quantityCell) || !int.TryParse(quantityCell, out _)) &&
                              string.IsNullOrWhiteSpace(priceCell);

            bool isTotalPrice = !string.IsNullOrWhiteSpace(description) &&
                                string.IsNullOrWhiteSpace(quantityCell) &&
                                !string.IsNullOrWhiteSpace(priceCell) &&
                                string.IsNullOrWhiteSpace(position);

            if (isCategory)
            {
                Log.Information("Category found");
                var translatedCategory = await TranslationService.TranslateText(description);
                currentCategory = translatedCategory;

                if (!mainOptionsByCategory.ContainsKey(currentCategory))
                {
                    Log.Information("Category added to dictionary");
                    mainOptionsByCategory[currentCategory] = new List<string>();
                }
            }
            else if (isTotalPrice)
            {
                Log.Information("Total price found");
                if (decimal.TryParse(priceCell, out decimal parsedPrice))
                {
                    Log.Information("Price parsed");
                    if (totalWithoutVAT == 0)
                    {
                        totalWithoutVAT = parsedPrice;
                    }
                    else
                    {
                        totalWithVAT = parsedPrice;
                        break;
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(description))
            {
                Log.Information("Description translated");
                var translatedDescription = await TranslationService.TranslateText(description);
                mainOptionsByCategory[currentCategory].Add(translatedDescription);
            }
        }

        var mainOptions = mainOptionsByCategory.Select(kvp => new QuoteDto.MainOptionDto
        {
            Category = kvp.Key,
            Options = kvp.Value
        }).ToList();

        var machinery = await MachineryService.GetMachineryByMachineryNameAsync(machineName);

        var excelModel = new QuoteDto.ExcelModel
        {
            QuoteNumber = quoteNumber,
            CurrentDate = currentDate,
            MachineName = machineName,
            MachineId = machinery.Id,
            BasePrice = totalWithoutVAT,
            TotalWithoutVat = totalWithoutVAT,
            TotalWithVat = totalWithVAT,
            Customer = new CustomerDto.Detail
            {
                Id = customer.Id,
                Name = customer.Name,
                Street = customer.Street,
                StreetNumber = customer.StreetNumber,
                City = customer.City,
                PostalCode = customer.PostalCode,
                Country = customer.Country
            },
            IsApproved = false,

        };

        if (!dbContext.Quotes.Any(x => x.QuoteNumber == quoteNumber))
        {
            Log.Information("Quote does not exist");
            var newQuote = new QuoteDto.Create
            {
                QuoteNumber = quoteNumber,
                IsApproved = false,
                CustomerId = customer.Id,
                NewQuoteId = 0,
                Date = DateTime.Now,
                BasePrice = totalWithoutVAT,
                TotalWithoutVat = totalWithoutVAT,
                TotalWithVat = totalWithVAT,
                MachineryId = machinery.Id,
                SalespersonId = userId,
                MainOptions = mainOptions
            };
            var createdQuote = await CreateQuoteAsync(newQuote);
        }
        else
        {
            Log.Information("Quote already exists");
            var existingQuote = await dbContext.Quotes.SingleOrDefaultAsync(x => x.QuoteNumber == quoteNumber);
            if (existingQuote is not null && existingQuote.TotalWithVat != totalWithVAT)
            {
                Log.Information("Quote total price updated");
                existingQuote.BasePrice = totalWithoutVAT;
                existingQuote.TotalWithoutVat = totalWithoutVAT;
                existingQuote.TotalWithVat = totalWithVAT;
                await dbContext.SaveChangesAsync();
            }
        }

        Log.Information("Quote imported from excel");
        return excelModel;
    }

    public async Task<IEnumerable<QuoteDto.Index>> GetQuotesForSalespersonAsync(string userId, QuoteQueryObject queryObject)
    {
        IQueryable<QuoteDto.Index> query = dbContext.Quotes
            .Where(x => !x.IsDeleted)
            .Where(x => x.SalespersonId == userId)
            .Include(x => x.Customer)
            .Select(x => new QuoteDto.Index
            {
                Id = x.Id,
                IsApproved = x.IsApproved,
                NewQuoteId = x.NewQuoteId,
                Customer = new CustomerDto.Index
                {
                    Id = x.Customer.Id,
                    Name = x.Customer.Name,

                },
                Date = x.Date,
                QuoteNumber = x.QuoteNumber,
                BasePrice = x.BasePrice,
                TotalWithoutVat = x.TotalWithoutVat,
                TotalWithVat = x.TotalWithVat,
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
                SalespersonId = x.SalespersonId,
            });

        if (!string.IsNullOrEmpty(queryObject.Search))
        {
            query = query.Where(x => x.QuoteNumber.Contains(queryObject.Search) ||
                                     x.Customer.Name.Contains(queryObject.Search) ||
                                     x.Machinery.Name.Contains(queryObject.Search));
        }

        if (queryObject.Before.HasValue)
        {
            query = query.Where(x => x.Date < queryObject.Before.Value);
        }

        if (queryObject.After.HasValue)
        {
            query = query.Where(x => x.Date > queryObject.After.Value);
        }

        if (!string.IsNullOrEmpty(queryObject.Status))
        {
            if (queryObject.Status == "Goedgekeurd")
            {
                query = query.Where(q => q.IsApproved);
            }
            else if (queryObject.Status == "Verouderd")
            {
                query = query.Where(q => !q.IsApproved && q.NewQuoteId != 0);
            }
            else if (queryObject.Status == "In afwachting")
            {
                query = query.Where(q => !q.IsApproved && q.NewQuoteId == 0);
            }
        }

        // Sorting
        if (!string.IsNullOrEmpty(queryObject.SortBy))
        {
            query = queryObject.SortBy.ToLower() switch
            {
                "quotenumber" => queryObject.IsDescending ? query.OrderByDescending(x => x.QuoteNumber) : query.OrderBy(x => x.QuoteNumber),
                "date" => queryObject.IsDescending ? query.OrderByDescending(x => x.Date) : query.OrderBy(x => x.Date),
                "customername" => queryObject.IsDescending ? query.OrderByDescending(x => x.Customer.Name) : query.OrderBy(x => x.Customer.Name),
                "machineryname" => queryObject.IsDescending ? query.OrderByDescending(x => x.Machinery.Name) : query.OrderBy(x => x.Machinery.Name),
                _ => query
            };
        }


        // pagination
        var skip = (queryObject.PageNumber - 1) * queryObject.PageSize;
        query = query.Skip(skip).Take(queryObject.PageSize);

        var quotes = await query.ToListAsync();

        queryObject.HasNext = await dbContext.Quotes
            .Where(x => !x.IsDeleted)
            .Skip(skip + queryObject.PageSize)
            .AnyAsync();

        Log.Information("Quotes retrieved for salesperson");
        return quotes;
    }

    public async Task<QuoteDto.ExcelModel> GetQuoteForSalespersonAsync(string quoteNumber, string userId)
    {
        var quote = await dbContext.Quotes
            .Where(q => q.QuoteNumber == quoteNumber && !q.IsDeleted)
            .Where(q => q.SalespersonId == userId)
            .Include(q => q.Customer)
            .Include(q => q.Machinery)
            .Include(q => q.QuoteOptions)
                .ThenInclude(qo => qo.MachineryOption)
            .Select(q => new
            {
                Id = q.Id,
                QuoteNumber = q.QuoteNumber,
                CurrentDate = q.Date.ToString("dd/MM/yyyy"),
                Baseprice = q.BasePrice,
                TotalWithoutVat = q.TotalWithoutVat,
                TotalWithVat = q.TotalWithVat,
                TopText = q.TopText,
                BottomText = q.BottomText,
                Customer = new CustomerDto.Detail
                {
                    Id = q.Customer.Id,
                    Name = q.Customer.Name,
                    Street = q.Customer.Street,
                    StreetNumber = q.Customer.StreetNumber,
                    City = q.Customer.City,
                    PostalCode = q.Customer.PostalCode,
                    Country = q.Customer.Country
                },
                MachineId = q.Machinery.Id,
                MachineName = q.Machinery.Name,
                QuoteOptions = q.QuoteOptions.Where(qo => !qo.IsDeleted).Select(qo => new QuoteOptionDto.OptionInfo
                {
                    Id = qo.Id,
                    MachineryOption = new MachineryOptionDto.XtremeDetail
                    {
                        Id = qo.MachineryOption.Id,
                        Option = new OptionDto.Detail
                        {
                            Id = qo.MachineryOption.Option.Id,
                            Name = qo.MachineryOption.Option.Name,
                            Code = qo.MachineryOption.Option.Code,
                            Category = new CategoryDto.Index
                            {
                                Id = qo.MachineryOption.Option.Category.Id,
                                Name = qo.MachineryOption.Option.Category.Name,
                                Code = qo.MachineryOption.Option.Category.Code
                            }
                        },
                        Price = qo.MachineryOption.Price
                    }
                }).ToList(),
                SalespersonId = q.SalespersonId,
                IsApproved = q.IsApproved,
                NewQuoteId = q.NewQuoteId,
                MainOptions = q.MainOptions,
                TradedMachineries = q.TradedMachineries.Select(tm => new TradedMachineryDto.Index
                {
                    Id = tm.Id,
                    Year = tm.Year,
                    Description = tm.Description,
                    EstimatedValue = tm.EstimatedValue,
                    Images = tm.Images.Select(i => new ImageDto.Index
                    {
                        Id = i.Id,
                        Url = i.Url
                    }).ToList(),
                    MachineryType = new MachineryTypeDto.Index
                    {
                        Id = tm.Type.Id,
                        Name = tm.Type.Name
                    },
                    Name = tm.Name,
                    SerialNumber = tm.SerialNumber,
                }).ToList()
            })
            .SingleOrDefaultAsync();

        if (quote == null)
        {
            Log.Warning("Quote not found by quote number");
            throw new EntityNotFoundException("Offerte", quoteNumber);
        }

        var parsedMainOptions = ParseMainOptions(quote.MainOptions);
        Log.Information("Quote retrieved by quote number");

        return new QuoteDto.ExcelModel
        {
            Id = quote.Id,
            QuoteNumber = quote.QuoteNumber,
            CurrentDate = quote.CurrentDate,
            BasePrice = quote.Baseprice,
            TotalWithoutVat = quote.TotalWithoutVat,
            TotalWithVat = quote.TotalWithVat,
            TopText = quote.TopText,
            BottomText = quote.BottomText,
            Customer = quote.Customer,
            MachineId = quote.MachineId,
            MachineName = quote.MachineName,
            QuoteOptions = quote.QuoteOptions,
            MainOptions = parsedMainOptions,
            IsApproved = quote.IsApproved,
            SalespersonId = quote.SalespersonId,
            NewQuoteId = quote.NewQuoteId,
            TradedMachineries= quote.TradedMachineries
        };
    }

    public Task<int> GetTotalQuotesAsync()
    {
        Log.Information("Total quotes retrieved");
        return dbContext.Quotes.CountAsync();
    }

    public Task<int> GetTotalQuotesForSalespersonAsync(string userId)
    {
        Log.Information("Total quotes retrieved for salesperson");
        return dbContext.Quotes.Where(x => x.SalespersonId == userId).CountAsync();
    }

    private List<QuoteDto.MainOptionDto> ParseMainOptions(string mainOptions)
    {
        if (string.IsNullOrEmpty(mainOptions))
        {
            Log.Warning("Main options are empty");
            return new List<QuoteDto.MainOptionDto>();
        }

        var parsed = mainOptions
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(optionGroup =>
            {
                var parts = optionGroup.Split(':', StringSplitOptions.RemoveEmptyEntries);

                return new QuoteDto.MainOptionDto
                {
                    Category = parts[0],
                    Options = parts.Skip(1).ToList()
                };
            })
            .ToList();

        Log.Information("Main options parsed");

        return parsed;
    }

    public async Task<bool> ApproveQuote(string quoteNumber)
    {
        var quoteold = dbContext.Quotes.SingleOrDefault(x => x.QuoteNumber == quoteNumber)
            ?? throw new EntityNotFoundException("Offerte", quoteNumber);

        if (quoteold.IsApproved)
        {
            Log.Warning("Quote already approved");
            throw new Exception("Offerte is al goedgekeurd");
        }
        quoteold.IsApproved = true;

        await dbContext.SaveChangesAsync();
        Log.Information("Quote approved");
        return true;
    }

    private string ConvertMainOptionsToString(List<QuoteDto.MainOptionDto> mainOptions)
    {
        if (mainOptions == null || mainOptions.Count == 0)
        {
            Log.Warning("Main options are empty");
            return string.Empty;
        }

        Log.Information("Main options converted to string");
        return string.Join(";", mainOptions.Select(option =>
            $"{option.Category}:{string.Join(":", option.Options!)}"));
    }

    public async Task<byte[]> GeneratePdf(string quoteNumber)
    {
        try
        {
            Log.Information("Generating PDF");
            var quote = await GetQuoteAsync(quoteNumber);

            if (quote == null)
            {
                Log.Warning("Quote not found by quote number");
                throw new EntityNotFoundException("Offerte", quoteNumber);
            }

            var documentData = new DocumentDto.Index
            {
                QuoteOrOrderNumber = quote.QuoteNumber,
                CurrentDate = quote.CurrentDate,
                Customer = quote.Customer,
                SalespersonId = quote.SalespersonId,
                TopText = quote.TopText,
                BottomText = quote.BottomText,
                MachineName = quote.MachineName,
                TotalWithoutVat = quote.TotalWithoutVat,
                TotalWithVat = quote.TotalWithVat,
                MainOptions = quote.MainOptions,
                QuoteOptions = quote.QuoteOptions,
                TradedMachineries = quote.TradedMachineries
            };
            Log.Information("PDF document data created");
            return documentService.CreatePdfDocument(documentData, false).GeneratePdf();
        }
        catch (Exception ex)
        {
			Log.Warning("An exceoption occured while generating a pdf", ex.Message);
			throw new Exception("An error occurred while generating the PDF.");
        }
    }

    public async Task<QuoteDto.Index> UpdateQuoteAsync(string quoteNumber, QuoteDto.Update quoteDto)
    {
        var quote = await dbContext.Quotes
            .Include(q => q.Customer)
            .Include(q => q.Machinery)
            .Include(q => q.Machinery.Type)
            .SingleOrDefaultAsync(q => q.QuoteNumber == quoteNumber && !q.IsDeleted)
            ?? throw new EntityNotFoundException("Offerte", quoteNumber);

        quote.TopText = quoteDto.TopText;
        quote.BottomText = quoteDto.BottomText;

        await dbContext.SaveChangesAsync();
        Log.Information("Quote updated");

        return new QuoteDto.Index
        {
            Id = quote.Id,
            IsApproved = quote.IsApproved,
            NewQuoteId = quote.NewQuoteId,
            Customer = new CustomerDto.Index
            {
                Id = quote.Customer.Id,
                Name = quote.Customer.Name,
            },
            Date = quote.Date,
            QuoteNumber = quote.QuoteNumber,
            BasePrice = quote.BasePrice,
            TotalWithoutVat = quote.TotalWithoutVat,
            TotalWithVat = quote.TotalWithVat,
            TopText = quote.TopText,
            BottomText = quote.BottomText,
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
            SalespersonId = quote.SalespersonId,
        };
    }
}
