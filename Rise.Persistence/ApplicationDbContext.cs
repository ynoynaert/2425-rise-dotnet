using Microsoft.EntityFrameworkCore;
using Rise.Domain.Machineries;
using Rise.Domain.Translations;
using Rise.Domain.Quotes;
using Rise.Domain.Customers;
using Rise.Domain.Orders;
using Rise.Domain.Inquiries;
using Rise.Domain.Locations;

namespace Rise.Persistence;

/// <inheritdoc />
public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Option> Options => base.Set<Option>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<MachineryType> MachineryTypes => Set<MachineryType>();
    public DbSet<Machinery> Machineries => Set<Machinery>();
    public DbSet<Translation> Translations => base.Set<Translation>();
    public DbSet<MachineryOption> MachineryOptions => base.Set<MachineryOption>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<QuoteOption> QuoteOptions => Set<QuoteOption>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Image> Images => base.Set<Image>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Inquiry> Inquiries => Set<Inquiry>();
    public DbSet<InquiryOption> InquiryOptions => Set<InquiryOption>();
    public DbSet<Location> Location => base.Set<Location>();
    public DbSet<TradedMachinery> TradedMachineries => base.Set<TradedMachinery>();
    public DbSet<TradedMachineryImage> TradedMachineryImages => base.Set<TradedMachineryImage>();


    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // All columns in the database have a maxlength of 4000.
        // in NVARACHAR 4000 is the maximum length that can be indexed by a database.
        // Some columns need more length, but these can be set on the configuration level for that Entity in particular.
        configurationBuilder.Properties<string>().HaveMaxLength(4_000);
        // All decimals columns should have 2 digits after the comma
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Applying all types of IEntityTypeConfiguration in the Persistence project.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

}

