using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Rise.Domain.Machineries;
using Rise.Persistence;
using Rise.Services.Machineries;
using Rise.Shared.Machineries;

namespace Rise.Services.Tests.Machineries;

public class MachineryTypeServiceTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Seeder _seeder;
    private readonly MachineryTypeService _machineryTypeService;

    public MachineryTypeServiceTest()
    {
        _context = TestApplicationDbContextFactory.CreateDbContext();

        _context.Database.Migrate();

        _seeder = new Seeder(_context);
        _seeder.Seed();
        _machineryTypeService = new MachineryTypeService(_context);
    }

    [Fact]
    public async Task GetMachineryTypesAsync_ShouldReturnAllMachineryTypes()
    {
        // Arrange
        var expected = await _context.MachineryTypes
            .Where(x => !x.IsDeleted)
            .Select(x => new MachineryTypeDto.Index
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

        // Act

        var actual = await _machineryTypeService.GetMachineryTypesAsync();

        // Assert
        Assert.True(actual.Zip(expected, (a, b) => a.Id == b.Id && a.Name == b.Name).All(x => x));
    }

    [Fact]
    public async Task GetMachineryTypeAsync_ShouldReturnMachineryType()
    {
        // Arrange
        var expected = await _context.MachineryTypes
            .Where(x => !x.IsDeleted)
            .Select(x => new MachineryTypeDto.Index
            {
                Id = x.Id,
                Name = x.Name
            })
            .SingleAsync(x => x.Id == 1);

        // Act
        var actual = await _machineryTypeService.GetMachineryTypeAsync(1);

        // Assert
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Name, actual.Name);
    }

    [Fact]
    public async Task GetMachineryTypeAsync_ShouldThrowException_MachineryTypeNotFound()
    {
        // Arrange
        var id = (await _context.MachineryTypes.MaxAsync(x => (int?)x.Id) ?? 0) + 1;

        // Act
        var exception = await Record.ExceptionAsync(() => _machineryTypeService.GetMachineryTypeAsync(id));

        // Assert
        Assert.IsType<EntityNotFoundException>(exception);
    }

    [Fact]
    public async Task CreateMachineryTypeAsync_ShouldCreateMachineryType()
    {
        // Arrange
        var machineryTypeDto = new MachineryTypeDto.Create
        {
            Name = "Test"
        };

        // Act
        var actual = await _machineryTypeService.CreateMachineryTypeAsync(machineryTypeDto);

        // Assert
        Assert.Equal(machineryTypeDto.Name, actual.Name);
    }

    [Fact]
    public async Task UpdateMachineryTypeAsync_ShouldUpdateMachineryType()
    {
        // Arrange
        var machineryTypeDto = new MachineryTypeDto.Update
        {
            Name = "Test"
        };

        // Act
        var actual = await _machineryTypeService.UpdateMachineryTypeAsync(1, machineryTypeDto);

        // Assert
        Assert.Equal(machineryTypeDto.Name, actual.Name);
    }

    [Fact]
    public async Task UpdateMachineryTypeAsync_ShouldThrowException_MachineryTypeNotFound()
    {
        // Arrange
        var nonExistentMachineryTypeId = 0;
        var machineryTypeDto = new MachineryTypeDto.Update
        {
            Name = "Test"
        };

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            async () => await _machineryTypeService.UpdateMachineryTypeAsync(nonExistentMachineryTypeId, machineryTypeDto)
        );
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
