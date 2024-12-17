using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rise.Shared.Quotes;
using Rise.Shared.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rise.Shared.Users;
using Serilog;
using Rise.Domain.Exceptions;

namespace Rise.Services.Documents;

public class DocumentService : IDocumentService
{
    private readonly IUserService userService;

    public DocumentService(IUserService userService)
    {
        this.userService = userService;
    }

    #pragma warning disable CS0618
	public IDocument CreatePdfDocument(DocumentDto.Index model, bool isOrder)
    {
        var location = userService.GetSalesPersonAsync(model.SalespersonId!).Result.Location ?? throw new EntityNotFoundException("Locatie", $"bij gebruiker {model.SalespersonId}");
        var logodata = GetImageFromUrl("https://risea02.blob.core.windows.net/machinery/logo-with-text.png");

        Log.Information("Creating PDF document for {QuoteOrOrderNumber}", model.QuoteOrOrderNumber);
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeColumn().Stack(stack =>
                            {
                                stack.Item().Text("Dozer").Bold().FontSize(20);
                                stack.Item().Text($"{location.Street} {location.StreetNumber}, {location.City}, {location.PostalCode}");
                                stack.Item().Text($"{location.Country}");
                                stack.Item().Text($"Tel: {location.PhoneNumber} | BTW: {location.VatNumber}");
                            });

                            row.RelativeColumn().AlignRight().Container().Element(container =>
                            {
                                container.Width(100).Height(100).Image(logodata).FitArea();
                            });
                        });

                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeColumn().Stack(stack =>
                            {
                                if (isOrder)
                                {
                                    stack.Item().Text($"Bestelling: {model.QuoteOrOrderNumber}").Bold().FontSize(16);
                                }
                                else
                                {
                                    stack.Item().Text($"Offerte: {model.QuoteOrOrderNumber}").Bold().FontSize(16);
                                }
                            });

                            row.RelativeColumn().AlignRight().Stack(stack =>
                            {
                                stack.Item().Text(model.CurrentDate).AlignRight();
                            });
                        });

                        column.Item().PaddingTop(5).LineHorizontal(1);
                    });

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Item().PaddingBottom(20).Stack(stack =>
                    {
                        stack.Item().PaddingBottom(10).Text(model.Customer!.Name).Bold().FontSize(14);
                        stack.Item().Text($"{model.Customer.Street} {model.Customer.StreetNumber}");
                        stack.Item().Text($"{model.Customer.City}, {model.Customer.PostalCode}");
                        stack.Item().Text($"{model.Customer.Country}");
                    });

                    column.Item().PaddingBottom(10).Text(model.TopText).FontSize(10);
                    column.Item().PaddingBottom(10).Text(model.MachineName).Bold().FontSize(14);

                    column.Item().PaddingBottom(10).Text("Fabrieksopties").Bold().FontSize(12);

                    column.Item().Stack(stack =>

                    {

                        foreach (var mainOption in model.MainOptions!)

                        {

                            stack.Item().Text(mainOption.Category).Bold().FontSize(10);

                            foreach (var option in mainOption.Options!)

                            {

                                stack.Item().Text(option).FontSize(10);

                            }

                        }

                    });



                    column.Item().PaddingBottom(10).PaddingTop(10).Text("Dozer Opties").Bold().FontSize(12);

                    if (model.QuoteOptions is null || model.QuoteOptions.Count == 0)

                    {

                        column.Item().Text("Geen extra opties.");

                    }

                    else

                    {

                        column.Item().Element(CreateDozerOptionsTable(model));

                    }




                    //tradedmachinery


                    column.Item().PaddingBottom(10).PaddingTop(10).Text("Ingeruilde Machines").Bold().FontSize(12);

                    if (model.TradedMachineries == null || !model.TradedMachineries.Any())
                    {
                        column.Item().Text("Geen ingeruilde machines.");
                    }
                    else
                    {
                        column.Item().Element(CreateTradedMachineriesTable(model.TradedMachineries));
                    }

                    column.Item().PaddingVertical(10);

                    column.Item().PaddingBottom(10).AlignRight().Stack(stack =>
                    {
                        stack.Item().Text($"Totaal (excl. BTW): {model.TotalWithoutVat:C}").Bold();
                        stack.Item().Text($"Totaal (incl. BTW): {model.TotalWithVat:C}").Bold();
                        stack.Item().Text($"Waarde ingeruilde machines: -{string.Format("{0:N2}", model.TradedMachineries == null ? 0 : model.TradedMachineries!.Sum(x => x.EstimatedValue)):C} €").Bold();
                        stack.Item().Text($"Totaal incl. BTW na inruilen: {string.Format("{0:N2}", model.TradedMachineries == null ? model.TotalWithVat : model.TotalWithVat - model.TradedMachineries!.Sum(x => x.EstimatedValue)):C} €").Bold();
                    });

                    column.Item().PaddingBottom(10).Text(model.BottomText).FontSize(10);
                });


                // Footer

                page.Footer()

                    .AlignCenter()

                    .Text(text =>

                    {

                        text.Span("Pagina ");

                        text.CurrentPageNumber();

                        text.Span(" van ");

                        text.TotalPages();

                    });

            });

        });

    }
    #pragma warning restore CS0618
	private Action<QuestPDF.Infrastructure.IContainer> CreateDozerOptionsTable(DocumentDto.Index model)
    {
        Log.Information("Creating Dozer Options Table");
        return container =>
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(4);
                });

                table.Header(header =>
                {
                    header.Cell().BorderBottom(1.5f).Padding(5).Text("Code").Bold();
                    header.Cell().BorderBottom(1.5f).Padding(5).Text("Optie").Bold();
                    header.Cell().BorderBottom(1.5f).Padding(5).Text("Prijs").Bold();
                });

                foreach (var quoteOption in model.QuoteOptions!)
                {
                    table.Cell().Element(CellStyle).Text(quoteOption.MachineryOption.Option.Code);
                    table.Cell().Element(CellStyle).Text(quoteOption.MachineryOption.Option.Name);
                    table.Cell().Element(CellStyle).Text($"{quoteOption.MachineryOption.Price:C}");
                }

            });
        };
    }

    private Action<QuestPDF.Infrastructure.IContainer> CreateTradedMachineriesTable(IReadOnlyList<TradedMachineryDto.Index> tradedMachineries)
    {
        Log.Information("Creating Traded Machineries Table");
        return container =>
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().BorderBottom(1.5f).Padding(5).Text("Naam").Bold();
                    header.Cell().BorderBottom(1.5f).Padding(5).Text("Serienummer").Bold();
                    header.Cell().BorderBottom(1.5f).Padding(5).Text("Geschatte Waarde").Bold();
                });

                foreach (var machinery in tradedMachineries)
                {
                    table.Cell().Element(CellStyle).Text(machinery.Name);
                    table.Cell().Element(CellStyle).Text(machinery.SerialNumber);
                    table.Cell().Element(CellStyle).Text($"{machinery.EstimatedValue:C}");
                }
            });
        };
    }

    private QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
    {
        Log.Information("Creating Cell Style");
        return container
            .BorderBottom(0.5f)
            .Padding(5);
    }
    private byte[] GetImageFromUrl(string url)
    {
        using var client = new HttpClient();
        var result = client.GetAsync(url).Result;
        if (result.IsSuccessStatusCode)
        {
            Log.Information("Image fetched from URL");
            return result.Content.ReadAsByteArrayAsync().Result;
        }
        Log.Warning("Unable to fetch image from URL");
        throw new Exception("Unable to fetch image from URL.");
    }
}
