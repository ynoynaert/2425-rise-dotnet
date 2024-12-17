using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Rise.Persistence;
using Rise.Services.Inquiries;
using Rise.Shared.Inquiries;

namespace Rise.Services.Tests.Inquiries;

public class InquiryServiceTest : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Seeder _seeder;
    private readonly InquiryService _inquiryService;

    public InquiryServiceTest()
    {
        _context = TestApplicationDbContextFactory.CreateDbContext();
        _context.Database.Migrate();

        _seeder = new Seeder(_context);
        _seeder.Seed();

        _inquiryService = new InquiryService(_context);
    }

    [Fact]
    public async Task GetInquiriesAsync_ShouldReturnInquiries()
    {
        var items = _context.Inquiries.ToList();

        // Act
        var inquiries = await _inquiryService.GetInquiriesAsync();

        // Assert
        Assert.NotNull(inquiries);
        Assert.Equal(items.Count(), inquiries.Count());
    }

    [Fact]
    public async Task GetInquiryAsync_ShouldReturnInquiry()
    {
        // Arrange
        var inquiryId = 1;

        // Act
        var inquiry = await _inquiryService.GetInquiryAsync(inquiryId);

        // Assert
        Assert.NotNull(inquiry);
        Assert.Equal(inquiryId, inquiry.Id);
    }

    [Fact]
    public async Task GetInquiriesForSalespersonAsync_ShouldReturnInquiries()
    {
        // Arrange
        var userId = "auth0|673b73fa1e5482be2e942b7f";
        var items = _context.Inquiries.Where(x => x.SalespersonId == userId).ToList();

        // Act
        var inquiries = await _inquiryService.GetInquiriesForSalespersonAsync(userId);

        // Assert
        Assert.NotNull(inquiries);
        Assert.Equal(items.Count(), inquiries.Count());
    }

    [Fact]
    public async Task CreateInquiryAsync_ShouldCreateInquiry()
    {
        // Arrange
        var inquiryDto = new InquiryDto.Create
        {
            ClientName = "Customer 4",
            MachineryId = 4,
            SalespersonId = "1"
        };

        // Act
        var inquiry = await _inquiryService.CreateInquiryAsync(inquiryDto);

        // Assert
        Assert.NotNull(inquiry);
        Assert.Equal(inquiryDto.ClientName, inquiry.CustomerName);
        Assert.Equal(inquiryDto.MachineryId, inquiry.Machinery.Id);
        Assert.Equal(inquiryDto.SalespersonId, inquiry.SalespersonId);
    }

    [Fact]
    public async Task DeleteInquiryAsync_ShouldDeleteInquiry()
    {
        // Arrange
        var inquiryId = 1;

        // Act
        await _inquiryService.DeleteInquiryAsync(inquiryId);

        // Assert
        var inquiry = await _context.Inquiries.FindAsync(inquiryId);
        Assert.Null(inquiry);
    }

    [Fact]
    public async Task DeleteInquiryAsync_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var inquiryId = 100;

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _inquiryService.DeleteInquiryAsync(inquiryId));
    }

    [Fact]
    public async Task GetInquiriesAsync_ShouldReturnInquiriesWithMachinery()
    {
        var items = _context.Inquiries.Include(x => x.Machinery).ToList();

        // Act
        var inquiries = await _inquiryService.GetInquiriesAsync();

        // Assert
        Assert.NotNull(inquiries);
        Assert.Equal(items.Count(), inquiries.Count());
        Assert.NotNull(inquiries.First().Machinery);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }
}
