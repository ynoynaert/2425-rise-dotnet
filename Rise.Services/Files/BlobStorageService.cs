using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Rise.Domain.Files;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Services.Files;

public class BlobStorageService(IConfiguration configuration) : IStorageService
{
    private readonly string connectionString = configuration.GetConnectionString("Storage")
			?? throw new ArgumentNullException(nameof(configuration), "Storage connection string is missing.");
    public Uri BasePath => new Uri("https://risea02.blob.core.windows.net/machinery");

	public Uri GenerateImageUploadSas(Image image)
    {
        string containerName = "machinery";
        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(image.Filename);
        var blobSasBuilder = new BlobSasBuilder
        {
            ExpiresOn = DateTime.UtcNow.AddMinutes(5),
            BlobContainerName = containerName,
            BlobName = image.Filename,
        };
        blobSasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);
        var sas = blobClient.GenerateSasUri(blobSasBuilder);
        Log.Information("SAS generated");
        return sas;
    }
}
