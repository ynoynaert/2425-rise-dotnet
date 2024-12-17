using System;
using Blazorise;
using Microsoft.AspNetCore.Components.Forms;

namespace Rise.Client.Files;

public class AzureBlobStorageService : IStorageService
{
	private readonly HttpClient httpClient;
	public static long MaxFileSize => 1024 * 1024 * 10; // 10MB
	public AzureBlobStorageService(HttpClient httpClient)
	{
		this.httpClient = httpClient;
	}
	public async Task UploadImageAsync(string sas, IFileEntry file)
	{
		var	content = new StreamContent(file!.OpenReadStream(MaxFileSize)!); 
		content.Headers.Add("x-ms-blob-type", "BlockBlob");
		content.Headers.Add("Content-Type", file.Type);
		var response = await httpClient.PutAsync(sas, content);
		response.EnsureSuccessStatusCode();
	}
}
