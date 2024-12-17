using Auth0.ManagementApi;
using Microsoft.EntityFrameworkCore;
using Moq;
using Rise.Domain.Exceptions;
using Rise.Persistence;
using Rise.Services.Locations;

namespace Rise.Services.Tests.Locations;

public class LocationServiceTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Seeder _seeder;
    private readonly LocationService _locationService;
    private readonly Mock<IManagementApiClient> _mockManagementApiClient;

    public LocationServiceTest()
    {
        _mockManagementApiClient = new Mock<IManagementApiClient>();
        _context = TestApplicationDbContextFactory.CreateDbContext();
        _context.Database.Migrate();
        _seeder = new Seeder(_context);
        _seeder.Seed();
        _locationService = new LocationService(_context, _mockManagementApiClient.Object);
    }

    [Fact]
    public async Task GetLocationsAsync_ShouldReturnAllLocations()
    {
        // Arrange
        var expectedCount = _context.Location.Count(x => !x.IsDeleted);
        // Act
        var locations = await _locationService.GetLocationsAsync();
        // Assert
        Assert.Equal(expectedCount, locations.Count());
    }

    [Fact]
    public async Task GetLocationAsync_ShouldReturnLocation()
    {
        // Arrange
        var loc = await _context.Location.Where(x => !x.IsDeleted).FirstOrDefaultAsync();
        // Act
        var location = await _locationService.GetLocationAsync(loc!.Id);
        // Assert
        Assert.NotNull(location);
        Assert.Equal(loc.Id, location.Id);
    }

    [Fact]
    public async Task GetLocationAsync_ShouldThrowEntityNotFoundException_WhenLocationDoesNotExist()
    {
        // Arrange
        var nonExistentId = 9999;

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _locationService.GetLocationAsync(nonExistentId));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
