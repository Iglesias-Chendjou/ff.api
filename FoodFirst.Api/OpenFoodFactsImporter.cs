using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Products;
using FoodFirst.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoodFirst.Api;

/// <summary>
/// Imports products from OpenFoodFacts for Delhaize and Colruyt,
/// maps them to ProductTemplates and creates StoreInventory entries.
/// </summary>
public static class OpenFoodFactsImporter
{
    private static readonly string[] Brands = ["Delhaize", "Colruyt", "Boni Selection", "Everyday"];

    private static readonly Dictionary<string, string[]> CategoryKeywords = new()
    {
        ["Fruits & Légumes"] = ["fruit", "légume", "vegetable", "salade", "tomate", "pomme", "banane", "carotte", "oignon"],
        ["Boulangerie"] = ["pain", "bread", "biscuit", "gâteau", "cake", "croissant", "boulangerie"],
        ["Produits laitiers"] = ["lait", "milk", "yaourt", "yogurt", "fromage", "cheese", "beurre", "butter", "crème"],
        ["Viandes & Poissons"] = ["viande", "meat", "poulet", "chicken", "porc", "bœuf", "beef", "poisson", "fish", "saumon"],
        ["Épicerie"] = ["pâtes", "pasta", "riz", "rice", "sauce", "huile", "oil", "conserve", "céréale", "chocolat", "confiture"],
    };

    public static async Task ImportAsync(AppDbContext db, IOpenFoodFactsClient offClient, ILogger logger)
    {
        var categories = await db.ProductCategories.ToListAsync();
        if (!categories.Any()) return;

        var stores = await db.Stores.ToListAsync();
        if (!stores.Any()) return;

        var existingBarcodes = await db.ProductTemplates
            .Where(p => p.Barcode != null)
            .Select(p => p.Barcode!)
            .ToHashSetAsync();

        int totalImported = 0;

        foreach (var brand in Brands)
        {
            logger.LogInformation("Importing products from OpenFoodFacts for brand: {Brand}", brand);

            for (int page = 1; page <= 3; page++) // max 3 pages per brand = ~150 products
            {
                try
                {
                    var result = await offClient.SearchAsync(brand, null, page, 50);
                    if (result.Products.Count == 0) break;

                    foreach (var product in result.Products)
                    {
                        if (string.IsNullOrWhiteSpace(product.Name)) continue;
                        if (string.IsNullOrWhiteSpace(product.Barcode)) continue;
                        if (existingBarcodes.Contains(product.Barcode)) continue;

                        var category = MapCategory(product, categories);
                        var price = GeneratePrice(product);

                        var template = new ProductTemplate
                        {
                            Id = Guid.NewGuid(),
                            CategoryId = category.Id,
                            Name = CleanName(product.Name),
                            Description = $"{product.Brand ?? brand} — {product.Quantity ?? ""}".Trim(' ', '—', ' '),
                            ImageUrl = product.ImageUrl,
                            Barcode = product.Barcode,
                            Brand = product.Brand ?? brand,
                            NutritionGrade = product.NutritionGrade,
                            Unit = ParseUnit(product.Quantity),
                            PriceLowRange = price * 0.80m,
                            PriceMidRange = price,
                            PriceHighRange = price * 1.25m,
                            DiscountPercent = 50,
                            IsActive = true
                        };

                        db.ProductTemplates.Add(template);
                        existingBarcodes.Add(product.Barcode);

                        // Create inventory in each store
                        foreach (var store in stores)
                        {
                            db.StoreInventories.Add(new StoreInventory
                            {
                                Id = Guid.NewGuid(),
                                StoreId = store.Id,
                                ProductTemplateId = template.Id,
                                SelectedRange = PriceRange.Mid,
                                Quantity = Random.Shared.Next(5, 30),
                                AvailableQuantity = Random.Shared.Next(3, 25),
                                ExpirationDate = DateTime.UtcNow.AddDays(Random.Shared.Next(1, 7)),
                                CheckedAt = DateTime.UtcNow,
                                CheckedByUserId = (await db.Users.FirstAsync(u => u.Role == UserRole.Admin)).Id,
                                IsPublished = true
                            });
                        }

                        totalImported++;
                    }

                    await db.SaveChangesAsync();
                    logger.LogInformation("Brand {Brand} page {Page}: imported {Count} products", brand, page, result.Products.Count);

                    // Rate limit: wait 1s between pages to respect OFF API
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to import page {Page} for brand {Brand}", page, brand);
                    break;
                }
            }
        }

        logger.LogInformation("OpenFoodFacts import complete: {Total} products imported", totalImported);
    }

    private static ProductCategory MapCategory(OpenFoodFactsProductDto product, List<ProductCategory> categories)
    {
        var allText = string.Join(" ", product.Categories).ToLowerInvariant()
                      + " " + (product.Name?.ToLowerInvariant() ?? "");

        foreach (var (catName, keywords) in CategoryKeywords)
        {
            if (keywords.Any(k => allText.Contains(k)))
            {
                var match = categories.FirstOrDefault(c => c.Name == catName);
                if (match != null) return match;
            }
        }

        // Default to Épicerie
        return categories.FirstOrDefault(c => c.Name == "Épicerie") ?? categories.First();
    }

    private static decimal GeneratePrice(OpenFoodFactsProductDto product)
    {
        // Generate a realistic price based on product type
        var name = (product.Name ?? "").ToLowerInvariant();
        if (name.Contains("viande") || name.Contains("meat") || name.Contains("poisson") || name.Contains("fish"))
            return Math.Round(5m + Random.Shared.Next(0, 800) / 100m, 2);
        if (name.Contains("fromage") || name.Contains("cheese"))
            return Math.Round(3m + Random.Shared.Next(0, 500) / 100m, 2);
        if (name.Contains("bio") || name.Contains("organic"))
            return Math.Round(3m + Random.Shared.Next(0, 400) / 100m, 2);
        return Math.Round(1.5m + Random.Shared.Next(0, 500) / 100m, 2);
    }

    private static string ParseUnit(string? quantity)
    {
        if (string.IsNullOrWhiteSpace(quantity)) return "pièce";
        var q = quantity.ToLowerInvariant();
        if (q.Contains("kg")) return "kg";
        if (q.Contains("ml") || q.Contains("cl") || q.Contains("l")) return "litre";
        if (q.Contains("g")) return "g";
        return "pièce";
    }

    private static string CleanName(string name)
    {
        // Truncate to 200 chars and clean up
        if (name.Length > 195) name = name[..195] + "...";
        return name.Trim();
    }
}
