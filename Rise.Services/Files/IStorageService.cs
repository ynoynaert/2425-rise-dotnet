using Rise.Domain.Files;

namespace Rise.Services.Files;

public interface IStorageService
{
    Uri BasePath { get; }
    Uri GenerateImageUploadSas(Image image);
}
