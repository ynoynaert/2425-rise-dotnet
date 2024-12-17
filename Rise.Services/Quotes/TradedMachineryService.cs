using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Rise.Domain.Quotes;
using Rise.Persistence;
using Rise.Services.Files;
using Rise.Shared.Quotes;
using Serilog;

namespace Rise.Services.Quotes;

public class TradedMachineryService(IStorageService storageService, ApplicationDbContext dbContext) : ITradedMachineryService
{
    public async Task<TradedMachineryResult.Create> CreateTradedMachineryAsync(TradedMachineryDto.Create tradedMachineryDto)
    {
        if (tradedMachineryDto is null)
        {
            Log.Warning("Traded machinery can't be created because the input is null");
            throw new EntityNotFoundException("Ingeruilde machine", "");
        }

        var existingQuote = await dbContext.Quotes.Include(x=>x.TradedMachineries).SingleOrDefaultAsync(x => x.QuoteNumber == tradedMachineryDto.QuoteNumber)
            ?? throw new EntityNotFoundException("Offerte", tradedMachineryDto.QuoteNumber!);

        if (!existingQuote.IsApproved)
        {
            Log.Warning("Traded machinery can't be created because the quote is not approved");
            throw new InvalidOperationException("De offerte moet goedgekeurd zijn voordat je een machine kunt inruilen.");
        }

        var bestelling = await dbContext.Orders.SingleOrDefaultAsync(x => x.Quote.Id == existingQuote.Id);
        if (bestelling is not null)
        {
            Log.Warning("Traded machinery can't be created because there is already an order for this quote");
            throw new InvalidOperationException("Er is al een betselling voor deze offerte, het is niet meer mogelijk om een machine in te ruilen.");
        }

        var existingMachineryType = await dbContext.MachineryTypes.SingleOrDefaultAsync(x => x.Id == tradedMachineryDto.TypeId)
            ?? throw new EntityNotFoundException("Machinetype", tradedMachineryDto.TypeId);

        var existingPrice = existingQuote.TradedMachineries == null ? existingQuote.TotalWithVat : existingQuote.TotalWithVat - existingQuote.TradedMachineries!.Sum(x => x.EstimatedValue);
        var tradedMachineryprice = existingQuote.TradedMachineries == null ? 0 : existingQuote.TradedMachineries!.Sum(x => x.EstimatedValue);
        if (existingPrice < tradedMachineryDto.EstimatedValue)
        {
            Log.Warning("Traded machinery can't be created because the value of the traded machinery is too high");
            throw new InvalidOperationException($"De waarde van de ingeruile machine is te groot! Er kan nog maar een waarde van {existingPrice} ingeruild worden.");
        }

        var tradedMachinery = new TradedMachinery
        {
            Name = tradedMachineryDto.Name!,
            Type = existingMachineryType,
            SerialNumber = tradedMachineryDto.SerialNumber!,
            Description = tradedMachineryDto.Description!,
            EstimatedValue = tradedMachineryDto.EstimatedValue,
            Year = tradedMachineryDto.Year,
            Quote = existingQuote
        };

        Log.Information("Traded machinery created");
        dbContext.TradedMachineries.Add(tradedMachinery);
        List<Uri> uris = [];
        foreach (var contentType in tradedMachineryDto.ImageContentType)
        {
            Domain.Files.Image image = new Domain.Files.Image(storageService.BasePath, contentType);
            Uri uploadSas = storageService.GenerateImageUploadSas(image);
            uris.Add(uploadSas);
            Domain.Quotes.TradedMachineryImage tradedMachineryImage = new() { TradedMachinery = tradedMachinery, Url = image.FileUri.ToString() };
            dbContext.TradedMachineryImages.Add(tradedMachineryImage);
        }

        Log.Information("Traded machinery images created");
        await dbContext.SaveChangesAsync();

        Log.Information("Traded machinery images saved");
        return new TradedMachineryResult.Create()
        {
            Id = tradedMachinery.Id,
            UploadUris = uris.Select(x => x.ToString()).ToList()
        };
    }
}
