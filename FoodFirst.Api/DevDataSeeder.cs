using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;
using FoodFirst.Tools.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Api;

public static class DevDataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.Users.AnyAsync(u => u.Role == UserRole.Admin))
        {
            db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@foodfirst.be",
                PasswordHash = PasswordHasher.Hash("Admin1234!"),
                FirstName = "Admin",
                LastName = "FoodFirst",
                Phone = "+32400000000",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        if (!await db.Zones.AnyAsync())
        {
            db.Zones.AddRange(
                new Zone { Id = Guid.NewGuid(), Name = "Bruxelles Centre", Communes = ["Bruxelles", "Ixelles", "Saint-Gilles"], MaxDeliveryMinutes = 45, IsActive = true },
                new Zone { Id = Guid.NewGuid(), Name = "Bruxelles Nord", Communes = ["Schaerbeek", "Evere", "Jette"], MaxDeliveryMinutes = 45, IsActive = true },
                new Zone { Id = Guid.NewGuid(), Name = "Bruxelles Sud", Communes = ["Uccle", "Forest", "Anderlecht"], MaxDeliveryMinutes = 45, IsActive = true });
        }

        if (!await db.ProductCategories.AnyAsync())
        {
            db.ProductCategories.AddRange(
                new ProductCategory { Id = Guid.NewGuid(), Name = "Fruits & Légumes", Icon = "leaf", SortOrder = 1, IsActive = true },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Boulangerie", Icon = "bread", SortOrder = 2, IsActive = true },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Produits laitiers", Icon = "milk", SortOrder = 3, IsActive = true },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Viandes & Poissons", Icon = "meat", SortOrder = 4, IsActive = true },
                new ProductCategory { Id = Guid.NewGuid(), Name = "Épicerie", Icon = "basket", SortOrder = 5, IsActive = true });
        }

        if (!await db.SurpriseBoxPlans.AnyAsync())
        {
            db.SurpriseBoxPlans.AddRange(
                new SurpriseBoxPlan { Id = Guid.NewGuid(), Name = "Découverte", Description = "1 colis surprise par mois", MonthlyPrice = 30m, DeliveriesPerMonth = 1, EstimatedBoxValue = 60m, IsActive = true },
                new SurpriseBoxPlan { Id = Guid.NewGuid(), Name = "Classique", Description = "3 colis surprise par mois", MonthlyPrice = 50m, DeliveriesPerMonth = 3, EstimatedBoxValue = 120m, IsActive = true },
                new SurpriseBoxPlan { Id = Guid.NewGuid(), Name = "Premium", Description = "5 colis surprise par mois", MonthlyPrice = 80m, DeliveriesPerMonth = 5, EstimatedBoxValue = 200m, IsActive = true });
        }

        await db.SaveChangesAsync();

        await SeedHappyPathAsync(db);
    }

    private static async Task SeedHappyPathAsync(AppDbContext db)
    {
        var centreZone = await db.Zones.FirstOrDefaultAsync(z => z.Name == "Bruxelles Centre");
        if (centreZone is null) return;

        var client = await db.Users.FirstOrDefaultAsync(u => u.Email == "client@foodfirst.be");
        if (client is null)
        {
            client = new User
            {
                Id = Guid.NewGuid(),
                Email = "client@foodfirst.be",
                PasswordHash = PasswordHasher.Hash("Client1234!"),
                FirstName = "Claire",
                LastName = "Client",
                Phone = "+32470111111",
                Role = UserRole.Client,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(client);
            await db.SaveChangesAsync();
        }

        if (!await db.Addresses.AnyAsync(a => a.UserId == client.Id))
        {
            db.Addresses.Add(new Address
            {
                Id = Guid.NewGuid(),
                UserId = client.Id,
                Street = "Rue de Flandre",
                Number = "5",
                PostalCode = "1000",
                City = "Bruxelles",
                Commune = "Bruxelles",
                Latitude = 50.8510m,
                Longitude = 4.3487m,
                IsDefault = true,
                Label = "Home"
            });
            await db.SaveChangesAsync();
        }

        var driverUser = await db.Users.FirstOrDefaultAsync(u => u.Email == "driver@foodfirst.be");
        if (driverUser is null)
        {
            driverUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "driver@foodfirst.be",
                PasswordHash = PasswordHasher.Hash("Driver1234!"),
                FirstName = "Dimitri",
                LastName = "Driver",
                Phone = "+32470222222",
                Role = UserRole.Delivery,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Users.Add(driverUser);
            await db.SaveChangesAsync();
        }

        if (!await db.DeliveryPersons.AnyAsync(dp => dp.UserId == driverUser.Id))
        {
            db.DeliveryPersons.Add(new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                UserId = driverUser.Id,
                ZoneId = centreZone.Id,
                VehicleType = "Bike",
                IsAvailable = true,
                IsVerified = true,
                AverageRating = 4.80m,
                TotalDeliveries = 0,
                BankAccountIBAN = "BE68539007547034"
            });
            await db.SaveChangesAsync();
        }

        var store = await db.Stores.FirstOrDefaultAsync(s => s.Name == "Delhaize Bruxelles Centre");
        if (store is null)
        {
            store = new Store
            {
                Id = Guid.NewGuid(),
                Name = "Delhaize Bruxelles Centre",
                Brand = "Delhaize",
                Address = "Boulevard Anspach 57",
                PostalCode = "1000",
                City = "Bruxelles",
                Commune = "Bruxelles",
                Latitude = 50.8486m,
                Longitude = 4.3519m,
                Phone = "+3225551234",
                ContactEmail = "bxl-centre@delhaize.be",
                ZoneId = centreZone.Id,
                IsActive = true,
                ContractStartDate = DateTime.UtcNow.AddMonths(-3)
            };
            db.Stores.Add(store);
            await db.SaveChangesAsync();
        }

        if (!await db.ProductTemplates.AnyAsync())
        {
            var boulangerie = await db.ProductCategories.FirstAsync(c => c.Name == "Boulangerie");
            var laitiers = await db.ProductCategories.FirstAsync(c => c.Name == "Produits laitiers");
            var fruitsLegumes = await db.ProductCategories.FirstAsync(c => c.Name == "Fruits & Légumes");

            db.ProductTemplates.AddRange(
                new ProductTemplate
                {
                    Id = Guid.NewGuid(),
                    CategoryId = boulangerie.Id,
                    Name = "Pain de campagne",
                    Description = "Pain artisanal, cuit le jour même",
                    Unit = "pièce",
                    PriceLowRange = 2.50m,
                    PriceMidRange = 3.20m,
                    PriceHighRange = 4.00m,
                    DiscountPercent = 50,
                    IsActive = true
                },
                new ProductTemplate
                {
                    Id = Guid.NewGuid(),
                    CategoryId = laitiers.Id,
                    Name = "Yaourt nature bio",
                    Description = "Yaourt fermier bio 500g",
                    Unit = "pot",
                    PriceLowRange = 1.80m,
                    PriceMidRange = 2.50m,
                    PriceHighRange = 3.20m,
                    DiscountPercent = 50,
                    IsActive = true
                },
                new ProductTemplate
                {
                    Id = Guid.NewGuid(),
                    CategoryId = fruitsLegumes.Id,
                    Name = "Pommes Jonagold (1kg)",
                    Description = "Pommes belges calibre moyen",
                    Unit = "kg",
                    PriceLowRange = 1.50m,
                    PriceMidRange = 2.20m,
                    PriceHighRange = 2.90m,
                    DiscountPercent = 50,
                    IsActive = true
                });
            await db.SaveChangesAsync();
        }

        if (!await db.StoreInventories.AnyAsync(si => si.StoreId == store.Id))
        {
            var templates = await db.ProductTemplates.Take(3).ToListAsync();
            foreach (var t in templates)
            {
                db.StoreInventories.Add(new StoreInventory
                {
                    Id = Guid.NewGuid(),
                    StoreId = store.Id,
                    ProductTemplateId = t.Id,
                    SelectedRange = PriceRange.Mid,
                    Quantity = 20,
                    AvailableQuantity = 20,
                    ExpirationDate = DateTime.UtcNow.AddDays(2),
                    CheckedAt = DateTime.UtcNow,
                    CheckedByUserId = driverUser.Id,
                    IsPublished = true
                });
            }
            await db.SaveChangesAsync();
        }
    }
}
