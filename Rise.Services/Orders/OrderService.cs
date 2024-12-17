using DocumentFormat.OpenXml.Office2010.Word;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Rise.Domain.Exceptions;
using Rise.Domain.Orders;
using Rise.Persistence;
using Rise.Services.Documents;
using Rise.Shared.Customers;
using Rise.Shared.Documents;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;
using Rise.Shared.Orders;
using Rise.Shared.Quotes;
using Rise.Shared.Users;
using Serilog;


namespace Rise.Services.Orders;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IQuoteService quoteService;
    private readonly IUserService userService;
    private readonly IDocumentService documentService;
    private readonly ILogger<OrderService> logger;

    public OrderService(ApplicationDbContext dbContext, IQuoteService quoteService, IUserService userService, IDocumentService documentService, ILogger<OrderService> logger)
    {
        this.dbContext = dbContext;
        this.quoteService = quoteService;
        this.userService = userService;
        this.documentService = documentService;
        this.logger = logger;
    }

    public async Task<OrderDto.Index> CreateOrderAsync(OrderDto.Create orderDto)
    {

        var existingOrder = await dbContext.Orders
           .SingleOrDefaultAsync(x => x.OrderNumber  == orderDto.OrderNumber);

        if (existingOrder is not null)
        {
            Log.Warning("Order already exists");
            throw new EntityAlreadyExistsException("Bestelling", "Bestellingsnummer", orderDto.OrderNumber);
        }

        var quote = await dbContext.Quotes
            .Include(q => q.Customer)
            .Include(q => q.Machinery)
                .ThenInclude(m => m.Type)
            .SingleOrDefaultAsync(x => x.Id == orderDto.QuoteId)
            ?? throw new EntityNotFoundException("Offerte", orderDto.QuoteId);

        var order = new Order
        {
            OrderNumber = orderDto.OrderNumber,
            Quote = quote,
            Date = orderDto.Date,
            IsCancelled = false

        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();
        Log.Information("Order created");

        return new OrderDto.Index
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Quote = new QuoteDto.Index
            {
                Id = order.Quote.Id,
                QuoteNumber = order.Quote.QuoteNumber,
                Customer = new CustomerDto.Index
                {
                    Id = order.Quote.Customer.Id,
                    Name = order.Quote.Customer.Name,

                },
                IsApproved = order.Quote.IsApproved,
                Date = order.Quote.Date,
                Machinery = new MachineryDto.Index
                {
                    Id = order.Quote.Machinery.Id,
                    Name = order.Quote.Machinery.Name,
                    Description = order.Quote.Machinery.Description,
                    SerialNumber = order.Quote.Machinery.SerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = order.Quote.Machinery.Type.Id,
                        Name = order.Quote.Machinery.Type.Name
                    },
                },
                TotalWithoutVat = order.Quote.TotalWithoutVat,
                TotalWithVat = order.Quote.TotalWithVat,
                SalespersonId = order.Quote.SalespersonId,
            },
            Date = order.Date,
            IsCancelled = order.IsCancelled

        };
    }

    public async Task<IEnumerable<OrderDto.Index>> GetOrdersForSalespersonAsync(string userId, OrderQueryObject query)
    {
        IQueryable<OrderDto.Index> orders = dbContext.Orders
            .Where(x => x.Quote.SalespersonId == userId)
            .Select(x => new OrderDto.Index
            {
                Id = x.Id,
                OrderNumber = x.OrderNumber,
                Date = x.Date,
                Quote = new QuoteDto.Index
                {
                    Id = x.Quote.Id,
                    QuoteNumber = x.Quote.QuoteNumber,
                    Customer = new CustomerDto.Index
                    {
                        Id = x.Quote.Customer.Id,
                        Name = x.Quote.Customer.Name,
                    },
                    IsApproved = x.Quote.IsApproved,
                    Date = x.Quote.Date,
                    Machinery = new MachineryDto.Index
                    {
                        Id = x.Quote.Machinery.Id,
                        Name = x.Quote.Machinery.Name,
                        Description = x.Quote.Machinery.Description,
                        SerialNumber = x.Quote.Machinery.SerialNumber,
                        Type = new MachineryTypeDto.Index
                        {
                            Id = x.Quote.Machinery.Type.Id,
                            Name = x.Quote.Machinery.Type.Name
                        },
                    },
                    TotalWithoutVat = x.Quote.TotalWithoutVat,
                    TotalWithVat = x.Quote.TotalWithVat,
                    SalespersonId = x.Quote.SalespersonId,
                },
                IsCancelled = x.IsCancelled
            });

        // Filteren
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            orders = orders.Where(o => o.OrderNumber.Contains(query.Search) ||
                                       o.Quote.QuoteNumber.Contains(query.Search) ||
                                       o.Quote.Customer.Name.Contains(query.Search) ||
                                       o.Quote.Machinery.Name.Contains(query.Search)); ;
        }

        if (query.Before.HasValue)
        {
            orders = orders.Where(o => o.Date <= query.Before.Value);
        }

        if (query.After.HasValue)
        {
            orders = orders.Where(o => o.Date >= query.After.Value);
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            if (query.Status == "Actief")
            {
                orders = orders.Where(o => !o.IsCancelled);
            }
            else if (query.Status == "Geannuleerd")
            {
                orders = orders.Where(o => o.IsCancelled);
            }
        }

        // Sorteren
        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            orders = query.SortBy switch
            {
                "OrderNumber" => query.IsDescending ? orders.OrderByDescending(o => o.OrderNumber) : orders.OrderBy(o => o.OrderNumber),
                "Date" => query.IsDescending ? orders.OrderByDescending(o => o.Date) : orders.OrderBy(o => o.Date),
                "CustomerName" => query.IsDescending ? orders.OrderByDescending(o => o.Quote.Customer.Name) : orders.OrderBy(o => o.Quote.Customer.Name),
                "MachineryName" => query.IsDescending ? orders.OrderByDescending(o => o.Quote.Machinery.Name) : orders.OrderBy(o => o.Quote.Machinery.Name),
                _ => orders
            };
        }

        // Pagination
        int skip = (query.PageNumber - 1) * query.PageSize;
        orders = orders.Skip(skip).Take(query.PageSize);

        query.HasNext = await orders.CountAsync() > query.PageSize;
        Log.Information("Orders retrieved for salesperson");

        return await orders.ToListAsync();
    }

    public async Task<OrderDto.Detail> GetOrderAsync(string orderNumber)
    {
        var orderData = await dbContext.Orders
            .Where(x => x.OrderNumber == orderNumber)
            .Select(x => new
            {
                x.Id,
                x.OrderNumber,
                x.Date,
                QuoteNumber = x.Quote.QuoteNumber,
                x.IsCancelled
            }).SingleOrDefaultAsync();

        if (orderData is null)
        {
            Log.Warning("Order not found");
            throw new EntityNotFoundException("Bestelling", orderNumber);
        }

        var quote = await quoteService.GetQuoteAsync(orderData.QuoteNumber);
        Log.Information("Order retrieved");

        return new OrderDto.Detail
        {
            Id = orderData.Id,
            OrderNumber = orderData.OrderNumber,
            Date = orderData.Date,
            Quote = quote,
            IsCancelled = orderData.IsCancelled
        };
    }

    public async Task<OrderDto.Detail> GetOrderForSalespersonAsync(string userId, string orderNumber)
    {
        var orderData = await dbContext.Orders
            .Where(x => x.Quote.SalespersonId == userId)
            .Where(x => x.OrderNumber == orderNumber)
            .Select(x => new
            {
                x.Id,
                x.OrderNumber,
                x.Date,
                QuoteNumber = x.Quote.QuoteNumber,
                x.IsCancelled
            }).SingleOrDefaultAsync();

        if (orderData is null)
        {
            Log.Warning("Order not found");
            throw new EntityNotFoundException("Bestelling", orderNumber);
        }
         
        var quote = await quoteService.GetQuoteAsync(orderData.QuoteNumber);
        Log.Information("Order retrieved for salesperson");

        return new OrderDto.Detail
        {
            Id = orderData.Id,
            OrderNumber = orderData.OrderNumber,
            Date = orderData.Date,
            Quote = quote,
            IsCancelled = orderData.IsCancelled
        };
    }


    public async Task<IEnumerable<OrderDto.Index>> GetOrdersAsync(OrderQueryObject query)
    {
        IQueryable<OrderDto.Index> orders = dbContext.Orders
           .Select(x => new OrderDto.Index
           {
               Id = x.Id,
               OrderNumber = x.OrderNumber,
               Date = x.Date,
               Quote = new QuoteDto.Index
               {
                   Id = x.Quote.Id,
                   QuoteNumber = x.Quote.QuoteNumber,
                   Customer = new CustomerDto.Index
                   {
                       Id = x.Quote.Customer.Id,
                       Name = x.Quote.Customer.Name,
                   },
                   IsApproved = x.Quote.IsApproved,
                   Date = x.Quote.Date,
                   Machinery = new MachineryDto.Index
                   {
                       Id = x.Quote.Machinery.Id,
                       Name = x.Quote.Machinery.Name,
                       Description = x.Quote.Machinery.Description,
                       SerialNumber = x.Quote.Machinery.SerialNumber,
                       Type = new MachineryTypeDto.Index
                       {
                           Id = x.Quote.Machinery.Type.Id,
                           Name = x.Quote.Machinery.Type.Name
                       },
                   },
                   TotalWithoutVat = x.Quote.TotalWithoutVat,
                   TotalWithVat = x.Quote.TotalWithVat,
                   SalespersonId = x.Quote.SalespersonId,
               },
               IsCancelled = x.IsCancelled

           });

        // Filteren
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            orders = orders.Where(o => o.OrderNumber.Contains(query.Search) ||
                                       o.Quote.QuoteNumber.Contains(query.Search) ||
                                       o.Quote.Customer.Name.Contains(query.Search) ||
                                       o.Quote.Machinery.Name.Contains(query.Search));
        }

        if (query.Before.HasValue)
        {
            orders = orders.Where(o => o.Date <= query.Before.Value);
        }

        if (query.After.HasValue)
        {
            orders = orders.Where(o => o.Date >= query.After.Value);
        }

		if (!string.IsNullOrEmpty(query.Status))
		{
			if (query.Status == "Actief")
			{
				orders = orders.Where(o => !o.IsCancelled);
			}
			else if (query.Status == "Geannuleerd")
			{
				orders = orders.Where(o => o.IsCancelled);
			}
		}

		// Sorteren
		if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            orders = query.SortBy switch
            {
                "OrderNumber" => query.IsDescending ? orders.OrderByDescending(o => o.OrderNumber) : orders.OrderBy(o => o.OrderNumber),
                "Date" => query.IsDescending ? orders.OrderByDescending(o => o.Date) : orders.OrderBy(o => o.Date),
                "CustomerName" => query.IsDescending ? orders.OrderByDescending(o => o.Quote.Customer.Name) : orders.OrderBy(o => o.Quote.Customer.Name),
                "MachineryName" => query.IsDescending ? orders.OrderByDescending(o => o.Quote.Machinery.Name) : orders.OrderBy(o => o.Quote.Machinery.Name),
                _ => orders
            };
        }

        // Pagination
        int skip = (query.PageNumber - 1) * query.PageSize;
        orders = orders.Skip(skip).Take(query.PageSize);

        query.HasNext = await orders.CountAsync() > query.PageSize;
        Log.Information("Orders retrieved");

        return await orders.ToListAsync();
    }

    public Task<bool> CheckIfQuoteHasOrder(string orderNumber)
    {
        var order = dbContext.Orders.SingleOrDefault(x => x.OrderNumber == orderNumber);
        if (order is not null)
        {
            Log.Information("Order already exists");
            return Task.FromResult(true);
        }
        Log.Information("Order does not exist");
        return Task.FromResult(false);
    }



    public async Task<OrderDto.Detail> UpdateOrderAsync(string orderNumber, OrderDto.Update orderDto)
    {
        var order = await dbContext.Orders
            .Include(x => x.Quote)
            .SingleOrDefaultAsync(x => x.OrderNumber == orderNumber)
            ?? throw new EntityNotFoundException("Bestelling", orderNumber);

        var existingOrder = await dbContext.Orders
            .Where(x => x.Id != order.Id)
            .SingleOrDefaultAsync(x => x.OrderNumber == orderDto.OrderNumber );

        if (existingOrder is not null)
        {
            Log.Warning("Order already exists");
            throw new EntityAlreadyExistsException("Bestelling", "Bestellingsnummer", order.OrderNumber);
        }

        var quote = dbContext.Quotes
            .Include(x => x.Customer)
            .Include(x => x.Machinery)
            .Include(x => x.QuoteOptions)
                .ThenInclude(x => x.MachineryOption)
                .ThenInclude(x => x.Option)
                .ThenInclude(x => x.Category)
            .SingleOrDefault(x => x.Id == orderDto.Quote.Id) ?? throw new EntityNotFoundException("Offerte", orderDto.Quote.Id);

        order.OrderNumber = orderDto.OrderNumber;
        order.Quote = quote;
        order.Date = orderDto.Date;
        order.IsCancelled = orderDto.IsCancelled;

        dbContext.Orders.Update(order);
        await dbContext.SaveChangesAsync();
        Log.Information("Order updated");

        return new OrderDto.Detail
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Date = order.Date,
            IsCancelled = order.IsCancelled,
            Quote = new QuoteDto.ExcelModel
            {
                Id = order.Quote.Id,
                QuoteNumber = order.Quote.QuoteNumber,
                CurrentDate = order.Quote.Date.ToString("dd/MM/yyyy"),
                TotalWithoutVat = order.Quote.TotalWithoutVat,
                TotalWithVat = order.Quote.TotalWithVat,
                Customer = new CustomerDto.Detail
               {
                   Id = order.Quote.Customer.Id,
                   Name = order.Quote.Customer.Name,
                   Street = order.Quote.Customer.Street,
                   StreetNumber = order.Quote.Customer.StreetNumber,
                   City = order.Quote.Customer.City,
                   PostalCode = order.Quote.Customer.PostalCode,
                   Country = order.Quote.Customer.Country
               },
                MachineId = order.Quote.Machinery.Id,
                MachineName = order.Quote.Machinery.Name,
                QuoteOptions = order.Quote.QuoteOptions.Where(qo => !qo.IsDeleted).Select(qo => new QuoteOptionDto.OptionInfo
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
                            },
                        },
                        Price = qo.MachineryOption.Price
                    }
                }).ToList(),
                SalespersonId = order.Quote.SalespersonId,
                NewQuoteId = order.Quote.NewQuoteId
            }

        };


    }

    public async Task<OrderDto.Android> GetOrderForSalespersonAsyncAndroid(string userId, string orderNumber)
    {
        var order = await dbContext.Orders
           .Where(x => x.Quote.SalespersonId == userId)
           .Where(x => x.OrderNumber == orderNumber)
           .Select(x => new
           {
               Id = x.Id,
               OrderNumber = x.OrderNumber,
               Date = x.Date,
               Quote = new
               {
                   Id = x.Quote.Id,
                   QuoteNumber = x.Quote.QuoteNumber,
                   Date = x.Quote.Date,
                   TotalWithoutVat = x.Quote.TotalWithoutVat,
                   TotalWithVat = x.Quote.TotalWithVat,
                   TopText = x.Quote.TopText,
                   BottomText = x.Quote.BottomText,
                   Customer = new CustomerDto.Detail
                   {
                       Id = x.Quote.Customer.Id,
                       Name = x.Quote.Customer.Name,
                       Street = x.Quote.Customer.Street,
                       StreetNumber = x.Quote.Customer.StreetNumber,
                       City = x.Quote.Customer.City,
                       PostalCode = x.Quote.Customer.PostalCode,
                       Country = x.Quote.Customer.Country
                   },
                   Machinery = new MachineryDto.Index
                   {
                       Id = x.Quote.Machinery.Id,
                       Name = x.Quote.Machinery.Name,
                       Description = x.Quote.Machinery.Description,
                       SerialNumber = x.Quote.Machinery.SerialNumber,
                       Type = new MachineryTypeDto.Index
                       {
                           Id = x.Quote.Machinery.Type.Id,
                           Name = x.Quote.Machinery.Type.Name
                       },
                   },
                   IsApproved = x.Quote.IsApproved,
                   MainOptions = x.Quote.MainOptions,
                   QuoteOptions = x.Quote.QuoteOptions.Where(qo => !qo.IsDeleted).Select(qo => new QuoteOptionDto.OptionInfo
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
                               },
                           },
                           Price = qo.MachineryOption.Price
                       }
                   }).ToList(),
                   NewQuoteId = x.Quote.NewQuoteId,
                   TradedMachineries = x.Quote.TradedMachineries.Select(tm => new TradedMachineryDto.Index
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
               },
               IsCancelled = x.IsCancelled
           }).SingleOrDefaultAsync() ?? throw new EntityNotFoundException("Bestelling", orderNumber); ;
        
        var parsedMainOptions = ParseMainOptions(order.Quote.MainOptions);

        var user = await userService.GetSalesPersonAsync(userId);

        return new OrderDto.Android
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Date = order.Date,
            Quote = new QuoteDto.Android
            {
                Id = order.Quote.Id,
                QuoteNumber = order.Quote.QuoteNumber,
                Date = order.Quote.Date,
                TotalWithoutVat = order.Quote.TotalWithoutVat,
                TotalWithVat = order.Quote.TotalWithVat,
                TopText = order.Quote.TopText,
                BottomText = order.Quote.BottomText,
                Customer = new CustomerDto.Detail
                {
                    Id = order.Quote.Customer.Id,
                    Name = order.Quote.Customer.Name,
                    Street = order.Quote.Customer.Street,
                    StreetNumber = order.Quote.Customer.StreetNumber,
                    City = order.Quote.Customer.City,
                    PostalCode = order.Quote.Customer.PostalCode,
                    Country = order.Quote.Customer.Country
                },
                Machinery = new MachineryDto.Index
                {
                    Id = order.Quote.Machinery.Id,
                    Name = order.Quote.Machinery.Name,
                    Description = order.Quote.Machinery.Description,
                    SerialNumber = order.Quote.Machinery.SerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = order.Quote.Machinery.Type.Id,
                        Name = order.Quote.Machinery.Type.Name
                    },
                },
                IsApproved = order.Quote.IsApproved,
                MainOptions = parsedMainOptions,
                QuoteOptions = order.Quote.QuoteOptions.Select(qo => new QuoteOptionDto.OptionInfo
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
                            },
                        },
                        Price = qo.MachineryOption.Price
                    }
                }).ToList(),
                NewQuoteId = order.Quote.NewQuoteId,
                TradedMachineries = order.Quote.TradedMachineries,
                SalespersonName = user.Name
            },
            IsCancelled = order.IsCancelled
        };
    }

    public async Task<OrderDto.Android> GetOrderAsyncAndroid(string orderNumber)
    {
        var order = await dbContext.Orders
           .Where(x => x.OrderNumber == orderNumber)
           .Select(x => new 
           {
               Id = x.Id,
               OrderNumber = x.OrderNumber,
               Date = x.Date,
               Quote = new 
               {
                   Id = x.Quote.Id,
                   QuoteNumber = x.Quote.QuoteNumber,
                   Date = x.Quote.Date,
                   TotalWithoutVat = x.Quote.TotalWithoutVat,
                   TotalWithVat = x.Quote.TotalWithVat,
                   TopText = x.Quote.TopText,
                   BottomText = x.Quote.BottomText,
                   Customer = new CustomerDto.Detail
                   {
                       Id = x.Quote.Customer.Id,
                       Name = x.Quote.Customer.Name,
                       Street = x.Quote.Customer.Street,
                       StreetNumber = x.Quote.Customer.StreetNumber,
                       City = x.Quote.Customer.City,
                       PostalCode = x.Quote.Customer.PostalCode,
                       Country = x.Quote.Customer.Country
                   },
                   Machinery = new MachineryDto.Index
                   {
                       Id = x.Quote.Machinery.Id,
                       Name = x.Quote.Machinery.Name,
                       Description = x.Quote.Machinery.Description,
                       SerialNumber = x.Quote.Machinery.SerialNumber,
                       Type = new MachineryTypeDto.Index
                       {
                           Id = x.Quote.Machinery.Type.Id,
                           Name = x.Quote.Machinery.Type.Name
                       },
                   },
                   IsApproved = x.Quote.IsApproved,
                   MainOptions = x.Quote.MainOptions,
                   QuoteOptions = x.Quote.QuoteOptions.Where(qo => !qo.IsDeleted).Select(qo => new QuoteOptionDto.OptionInfo
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
                               },
                           },
                           Price = qo.MachineryOption.Price
                       }
                   }).ToList(),
                   NewQuoteId = x.Quote.NewQuoteId,
                   TradedMachineries = x.Quote.TradedMachineries.Select(tm => new TradedMachineryDto.Index
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
                   SalespersonId = x.Quote.SalespersonId
               },
               IsCancelled = x.IsCancelled
           }).SingleOrDefaultAsync() ?? throw new EntityNotFoundException("Bestelling", orderNumber);

        var parsedMainOptions = ParseMainOptions(order.Quote.MainOptions);

        var user = await userService.GetSalesPersonAsync(order.Quote.SalespersonId);

        return new OrderDto.Android
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Date = order.Date,
            Quote = new QuoteDto.Android
            {
                Id = order.Quote.Id,
                QuoteNumber = order.Quote.QuoteNumber,
                Date = order.Quote.Date,
                TotalWithoutVat = order.Quote.TotalWithoutVat,
                TotalWithVat = order.Quote.TotalWithVat,
                TopText = order.Quote.TopText,
                BottomText = order.Quote.BottomText,
                Customer = new CustomerDto.Detail
                {
                    Id = order.Quote.Customer.Id,
                    Name = order.Quote.Customer.Name,
                    Street = order.Quote.Customer.Street,
                    StreetNumber = order.Quote.Customer.StreetNumber,
                    City = order.Quote.Customer.City,
                    PostalCode = order.Quote.Customer.PostalCode,
                    Country = order.Quote.Customer.Country
                },
                Machinery = new MachineryDto.Index
                {
                    Id = order.Quote.Machinery.Id,
                    Name = order.Quote.Machinery.Name,
                    Description = order.Quote.Machinery.Description,
                    SerialNumber = order.Quote.Machinery.SerialNumber,
                    Type = new MachineryTypeDto.Index
                    {
                        Id = order.Quote.Machinery.Type.Id,
                        Name = order.Quote.Machinery.Type.Name
                    },
                },
                IsApproved = order.Quote.IsApproved,
                MainOptions = parsedMainOptions,
                QuoteOptions = order.Quote.QuoteOptions.Select(qo => new QuoteOptionDto.OptionInfo
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
                            },
                        },
                        Price = qo.MachineryOption.Price
                    }
                }).ToList(),
                NewQuoteId = order.Quote.NewQuoteId,
                TradedMachineries = order.Quote.TradedMachineries,
                SalespersonName = user.Name
            },
            IsCancelled = order.IsCancelled
        };
    }

    public Task<int> GetTotalOrdersForSalesperson(string userId)
    {
        return dbContext.Orders.Where(x=>x.Quote.SalespersonId == userId).CountAsync();
    }

    public Task<int> GetTotalOrders()
    {
        return dbContext.Orders.CountAsync();
    }

    private List<QuoteDto.MainOptionDto> ParseMainOptions(string mainOptions)
    {
        if (string.IsNullOrEmpty(mainOptions))
        {
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

        return parsed;
    }

    public async Task<byte[]> GeneratePdf(string orderNumber)
    {
        try
        {
            var order = await GetOrderAsync(orderNumber);

            if (order == null)
            {
                throw new EntityNotFoundException("Order", orderNumber);
            }

            var quote = order.Quote;

            if (quote == null)
            {
                Log.Warning("The quote for this order is null");
				throw new EntityNotFoundException("Offerte", $"bij bestelling {orderNumber}");
            }

            var documentData = new DocumentDto.Index
            {
                QuoteOrOrderNumber = orderNumber,
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

            return documentService.CreatePdfDocument(documentData, true).GeneratePdf();
        }
        catch (Exception ex)
        {
            Log.Warning("An exceoption occured while generating a pdf", ex.Message);
            throw new Exception("An error occurred while generating the PDF.");
        }
    }
}
