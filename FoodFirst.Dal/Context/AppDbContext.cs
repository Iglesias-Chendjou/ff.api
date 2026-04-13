using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FoodFirst.Dal.Entities;

namespace FoodFirst.Dal.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductTemplate> ProductTemplates => Set<ProductTemplate>();
    public DbSet<StoreInventory> StoreInventories => Set<StoreInventory>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<DeliveryPerson> DeliveryPersons => Set<DeliveryPerson>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SurpriseBoxPlan> SurpriseBoxPlans => Set<SurpriseBoxPlan>();
    public DbSet<SurpriseBoxSubscription> SurpriseBoxSubscriptions => Set<SurpriseBoxSubscription>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<BulkPurchaseRequest> BulkPurchaseRequests => Set<BulkPurchaseRequest>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<TraceabilityLog> TraceabilityLogs => Set<TraceabilityLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ──── User ────
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.PasswordHash).HasMaxLength(512);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role).HasConversion<string>().HasMaxLength(20);
        });

        // ──── Address ────
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Street).HasMaxLength(256);
            entity.Property(e => e.Number).HasMaxLength(10);
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Commune).HasMaxLength(100);
            entity.Property(e => e.Label).HasMaxLength(50);
            entity.Property(e => e.Latitude).HasPrecision(9, 6);
            entity.Property(e => e.Longitude).HasPrecision(9, 6);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── Store ────
        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(256);
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Commune).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ContactEmail).HasMaxLength(256);
            entity.Property(e => e.TabletDeviceId).HasMaxLength(100);
            entity.Property(e => e.Latitude).HasPrecision(9, 6);
            entity.Property(e => e.Longitude).HasPrecision(9, 6);

            entity.HasOne(e => e.Zone)
                .WithMany(z => z.Stores)
                .HasForeignKey(e => e.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── Zone ────
        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);

            // Store Communes as comma-separated string
            entity.Property(e => e.Communes)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries),
                    new ValueComparer<string[]>(
                        (a, b) => (a ?? Array.Empty<string>()).SequenceEqual(b ?? Array.Empty<string>()),
                        v => v.Aggregate(0, (acc, s) => HashCode.Combine(acc, s.GetHashCode())),
                        v => v.ToArray()))
                .HasMaxLength(2000);
        });

        // ──── ProductCategory ────
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Icon).HasMaxLength(100);
        });

        // ──── ProductTemplate ────
        modelBuilder.Entity<ProductTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Unit).HasMaxLength(30);
            entity.Property(e => e.PriceLowRange).HasPrecision(10, 2);
            entity.Property(e => e.PriceMidRange).HasPrecision(10, 2);
            entity.Property(e => e.PriceHighRange).HasPrecision(10, 2);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.ProductTemplates)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── StoreInventory ────
        modelBuilder.Entity<StoreInventory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SelectedRange).HasConversion<string>().HasMaxLength(10);

            entity.HasOne(e => e.Store)
                .WithMany(s => s.Inventories)
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ProductTemplate)
                .WithMany(p => p.StoreInventories)
                .HasForeignKey(e => e.ProductTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── Order ────
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.SubTotal).HasPrecision(10, 2);
            entity.Property(e => e.DeliveryFee).HasPrecision(10, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
            entity.Property(e => e.TVA).HasPrecision(10, 2);
            entity.Property(e => e.StripePaymentIntentId).HasMaxLength(256);
            entity.Property(e => e.StripeChargeId).HasMaxLength(256);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Client)
                .WithMany(u => u.Orders)
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeliveryAddress)
                .WithMany()
                .HasForeignKey(e => e.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Zone)
                .WithMany()
                .HasForeignKey(e => e.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── OrderItem ────
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.PriceRange).HasConversion<string>().HasMaxLength(10);
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);
            entity.Property(e => e.LineTotal).HasPrecision(10, 2);

            entity.HasOne(e => e.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.StoreInventory)
                .WithMany()
                .HasForeignKey(e => e.StoreInventoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Store)
                .WithMany()
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── Payment ────
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StripePaymentIntentId).HasMaxLength(256);
            entity.Property(e => e.StripeChargeId).HasMaxLength(256);
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Currency).HasMaxLength(5);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.RefundAmount).HasPrecision(10, 2);

            entity.HasOne(e => e.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(e => e.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── Delivery ────
        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.CurrentLatitude).HasPrecision(9, 6);
            entity.Property(e => e.CurrentLongitude).HasPrecision(9, 6);
            entity.Property(e => e.ProofPhotoUrl).HasMaxLength(500);
            entity.Property(e => e.ClientSignature).HasMaxLength(500);
            entity.Property(e => e.ClientComment).HasMaxLength(500);

            entity.HasOne(e => e.Order)
                .WithOne(o => o.Delivery)
                .HasForeignKey<Delivery>(e => e.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeliveryPerson)
                .WithMany(dp => dp.Deliveries)
                .HasForeignKey(e => e.DeliveryPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Zone)
                .WithMany(z => z.Deliveries)
                .HasForeignKey(e => e.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── DeliveryPerson ────
        modelBuilder.Entity<DeliveryPerson>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VehicleType).HasMaxLength(50);
            entity.Property(e => e.CurrentLatitude).HasPrecision(9, 6);
            entity.Property(e => e.CurrentLongitude).HasPrecision(9, 6);
            entity.Property(e => e.AverageRating).HasPrecision(3, 2);
            entity.Property(e => e.BankAccountIBAN).HasMaxLength(34);

            entity.HasOne(e => e.User)
                .WithOne(u => u.DeliveryPerson)
                .HasForeignKey<DeliveryPerson>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Zone)
                .WithMany(z => z.DeliveryPersons)
                .HasForeignKey(e => e.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── Subscription ────
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlanType).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.StripeSubscriptionId).HasMaxLength(256);
            entity.Property(e => e.StripeCustomerId).HasMaxLength(256);
            entity.Property(e => e.MonthlyPrice).HasPrecision(10, 2);
            entity.Property(e => e.PreferredCategories).HasMaxLength(1000);

            entity.HasOne(e => e.Client)
                .WithOne(u => u.Subscription)
                .HasForeignKey<Subscription>(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeliveryAddress)
                .WithMany()
                .HasForeignKey(e => e.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── SurpriseBoxPlan ────
        modelBuilder.Entity<SurpriseBoxPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.MonthlyPrice).HasPrecision(10, 2);
            entity.Property(e => e.EstimatedBoxValue).HasPrecision(10, 2);
        });

        // ──── SurpriseBoxSubscription ────
        modelBuilder.Entity<SurpriseBoxSubscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.StripeSubscriptionId).HasMaxLength(256);
            entity.Property(e => e.StripeCustomerId).HasMaxLength(256);

            entity.HasOne(e => e.Client)
                .WithOne(u => u.SurpriseBoxSubscription)
                .HasForeignKey<SurpriseBoxSubscription>(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Plan)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(e => e.SurpriseBoxPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeliveryAddress)
                .WithMany()
                .HasForeignKey(e => e.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── Supplier ────
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VATNumber).IsUnique();
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.ContactName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.SupplierType).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.VATNumber).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(256);
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
        });

        // ──── BulkPurchaseRequest ────
        modelBuilder.Entity<BulkPurchaseRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductDescription).HasMaxLength(500);
            entity.Property(e => e.Unit).HasMaxLength(30);
            entity.Property(e => e.ProposedPricePerUnit).HasPrecision(10, 2);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.AdminNotes).HasMaxLength(1000);

            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.BulkPurchaseRequests)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── Notification ────
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Body).HasMaxLength(1000);
            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.Data).HasMaxLength(4000);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ──── TraceabilityLog ────
        modelBuilder.Entity<TraceabilityLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.BatchNumber).HasMaxLength(100);
            entity.Property(e => e.CollectedTemperature).HasPrecision(5, 2);
            entity.Property(e => e.DeliveredTemperature).HasPrecision(5, 2);

            entity.HasOne(e => e.OrderItem)
                .WithMany()
                .HasForeignKey(e => e.OrderItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Store)
                .WithMany()
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
