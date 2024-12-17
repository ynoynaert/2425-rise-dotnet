using Blazorise;
using Rise.Client.Files;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Client.Machineries;
public class FakeStorageService : IStorageService
{
    private List<ImageDto.Index> _allImages;
    public FakeStorageService()
    {
        _allImages = Enumerable.Range(1, 11)
            .Select(i => new ImageDto.Index
            {
                Id = i,
                Url = $"https://via.placeholder.com/150"
            }).ToList();
    }

    public Task UploadImageAsync(string sas, IFileEntry file)
    {
        return Task.CompletedTask;
    }
}
