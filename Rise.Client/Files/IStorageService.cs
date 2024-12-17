
using Blazorise;

namespace Rise.Client.Files;

public interface IStorageService
{
    Task UploadImageAsync(string sas, IFileEntry file);
}
