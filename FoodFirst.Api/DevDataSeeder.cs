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

        var nordZone = await db.Zones.FirstOrDefaultAsync(z => z.Name == "Bruxelles Nord");
        var sudZone = await db.Zones.FirstOrDefaultAsync(z => z.Name == "Bruxelles Sud");

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

        if (!await db.Stores.AnyAsync(s => s.Name == "Colruyt Schaerbeek") && nordZone is not null)
        {
            db.Stores.Add(new Store
            {
                Id = Guid.NewGuid(),
                Name = "Colruyt Schaerbeek",
                Brand = "Colruyt",
                Address = "Chaussée de Haecht 200",
                PostalCode = "1030",
                City = "Bruxelles",
                Commune = "Schaerbeek",
                Latitude = 50.8625m,
                Longitude = 4.3750m,
                Phone = "+3225559876",
                ContactEmail = "schaerbeek@colruyt.be",
                ZoneId = nordZone.Id,
                IsActive = true,
                ContractStartDate = DateTime.UtcNow.AddMonths(-2)
            });
            await db.SaveChangesAsync();
        }

        if (!await db.Stores.AnyAsync(s => s.Name == "Carrefour Forest") && sudZone is not null)
        {
            db.Stores.Add(new Store
            {
                Id = Guid.NewGuid(),
                Name = "Carrefour Forest",
                Brand = "Carrefour",
                Address = "Avenue du Globe 25",
                PostalCode = "1190",
                City = "Bruxelles",
                Commune = "Forest",
                Latitude = 50.8120m,
                Longitude = 4.3200m,
                Phone = "+3225554321",
                ContactEmail = "forest@carrefour.be",
                ZoneId = sudZone.Id,
                IsActive = true,
                ContractStartDate = DateTime.UtcNow.AddMonths(-1)
            });
            await db.SaveChangesAsync();
        }

        if (!await db.ProductTemplates.AnyAsync())
        {
            var boulangerie = await db.ProductCategories.FirstAsync(c => c.Name == "Boulangerie");
            var laitiers = await db.ProductCategories.FirstAsync(c => c.Name == "Produits laitiers");
            var fruitsLegumes = await db.ProductCategories.FirstAsync(c => c.Name == "Fruits & Légumes");
            var viandes = await db.ProductCategories.FirstAsync(c => c.Name == "Viandes & Poissons");
            var epicerie = await db.ProductCategories.FirstAsync(c => c.Name == "Épicerie");

            db.ProductTemplates.AddRange(
                // Fruits & Légumes
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = fruitsLegumes.Id, Name = "Barquette de fraises", Description = "Fraises de saison, 500g", Unit = "barquette", PriceLowRange = 3.00m, PriceMidRange = 4.50m, PriceHighRange = 5.50m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = fruitsLegumes.Id, Name = "Bananes bio", Description = "Bananes biologiques, 1kg", Unit = "kg", PriceLowRange = 1.99m, PriceMidRange = 2.99m, PriceHighRange = 3.50m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = fruitsLegumes.Id, Name = "Carottes lavées", Description = "Carottes belges lavées, 500g", Unit = "sachet", PriceLowRange = 1.20m, PriceMidRange = 1.89m, PriceHighRange = 2.50m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = fruitsLegumes.Id, Name = "Brocoli frais", Description = "Brocoli frais, 400g", Unit = "pièce", PriceLowRange = 1.80m, PriceMidRange = 2.49m, PriceHighRange = 3.20m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = fruitsLegumes.Id, Name = "Pommes Jonagold", Description = "Pommes belges calibre moyen, 1kg", Unit = "kg", PriceLowRange = 1.50m, PriceMidRange = 2.20m, PriceHighRange = 2.90m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = fruitsLegumes.Id, Name = "Salade César", Description = "Salade César prête à manger, 350g", Unit = "barquette", PriceLowRange = 3.50m, PriceMidRange = 4.99m, PriceHighRange = 6.00m, DiscountPercent = 50, IsActive = true },
                // Boulangerie
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = boulangerie.Id, Name = "Pain de campagne", Description = "Pain artisanal, cuit le jour même, 800g", Unit = "pièce", PriceLowRange = 2.50m, PriceMidRange = 3.80m, PriceHighRange = 4.50m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = boulangerie.Id, Name = "Croissants au beurre x4", Description = "4 croissants pur beurre", Unit = "lot", PriceLowRange = 2.50m, PriceMidRange = 3.60m, PriceHighRange = 4.80m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = boulangerie.Id, Name = "Pâtes complètes 500g", Description = "Pâtes complètes, 500g", Unit = "paquet", PriceLowRange = 1.50m, PriceMidRange = 2.19m, PriceHighRange = 2.90m, DiscountPercent = 50, IsActive = true },
                // Produits laitiers
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = laitiers.Id, Name = "Fromage Brie Président", Description = "Brie crémeux, 300g", Unit = "pièce", PriceLowRange = 3.80m, PriceMidRange = 5.49m, PriceHighRange = 6.50m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = laitiers.Id, Name = "Yaourt grec nature x6", Description = "6 yaourts grecs nature, 6x150g", Unit = "pack", PriceLowRange = 2.80m, PriceMidRange = 4.20m, PriceHighRange = 5.00m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = laitiers.Id, Name = "Yaourt nature bio", Description = "Yaourt fermier bio, 500g", Unit = "pot", PriceLowRange = 1.80m, PriceMidRange = 2.50m, PriceHighRange = 3.20m, DiscountPercent = 50, IsActive = true },
                // Viandes & Poissons
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = viandes.Id, Name = "Filet de poulet fermier", Description = "Filet de poulet fermier, 500g", Unit = "barquette", PriceLowRange = 5.50m, PriceMidRange = 7.99m, PriceHighRange = 9.50m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = viandes.Id, Name = "Aile de dinde", Description = "Ailes de dinde fraîches, 500g", Unit = "barquette", PriceLowRange = 3.99m, PriceMidRange = 5.99m, PriceHighRange = 7.00m, DiscountPercent = 50, IsActive = true },
                // Épicerie
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = epicerie.Id, Name = "Soupe de légumes maison", Description = "Soupe de légumes fait maison, 1L", Unit = "bouteille", PriceLowRange = 2.50m, PriceMidRange = 3.99m, PriceHighRange = 5.00m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = epicerie.Id, Name = "Jus d'orange pressé", Description = "Jus d'orange frais pressé, 1L", Unit = "bouteille", PriceLowRange = 2.20m, PriceMidRange = 3.49m, PriceHighRange = 4.50m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = epicerie.Id, Name = "Limonade artisanale bio", Description = "Limonade bio artisanale, 330ml", Unit = "bouteille", PriceLowRange = 1.80m, PriceMidRange = 2.50m, PriceHighRange = 3.20m, DiscountPercent = 50, IsActive = true },
                new ProductTemplate { Id = Guid.NewGuid(), CategoryId = epicerie.Id, Name = "Huile d'olive extra vierge", Description = "Huile d'olive extra vierge, 750ml", Unit = "bouteille", PriceLowRange = 6.00m, PriceMidRange = 8.99m, PriceHighRange = 11.00m, DiscountPercent = 50, IsActive = true }
            );
            await db.SaveChangesAsync();
        }

        // Seed inventory: all products in all stores
        var allStores = await db.Stores.Where(s => s.IsActive).ToListAsync();
        var allTemplates = await db.ProductTemplates.Where(t => t.IsActive).ToListAsync();
        var rng = new Random(42); // deterministic seed

        foreach (var s in allStores)
        {
            if (await db.StoreInventories.AnyAsync(si => si.StoreId == s.Id)) continue;

            // Each store gets a random subset of products (at least 8)
            var storeProducts = allTemplates.OrderBy(_ => rng.Next()).Take(Math.Max(8, allTemplates.Count * 2 / 3)).ToList();
            foreach (var t in storeProducts)
            {
                var qty = rng.Next(5, 30);
                db.StoreInventories.Add(new StoreInventory
                {
                    Id = Guid.NewGuid(),
                    StoreId = s.Id,
                    ProductTemplateId = t.Id,
                    SelectedRange = (PriceRange)rng.Next(0, 3),
                    Quantity = qty,
                    AvailableQuantity = qty,
                    ExpirationDate = DateTime.UtcNow.AddDays(rng.Next(7, 30)),
                    CheckedAt = DateTime.UtcNow,
                    CheckedByUserId = driverUser.Id,
                    IsPublished = true
                });
            }
        }
        await db.SaveChangesAsync();
    }
}
