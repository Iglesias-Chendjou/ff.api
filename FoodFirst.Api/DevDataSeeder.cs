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
    }
}
