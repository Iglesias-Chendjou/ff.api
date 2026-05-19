using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Memberships;
using FoodFirst.Service.Interfaces;
using FoodFirst.Tools.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Service.Implementations;

public class MembershipService(AppDbContext db) : IMembershipService
{
    public async Task<MembershipDto?> GetMineAsync(Guid userId, CancellationToken ct = default)
    {
        var m = await db.Memberships.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);
        return m is null ? null : Map(m);
    }

    public async Task<MembershipDto> SubscribeAsync(Guid userId, CancellationToken ct = default)
    {
        var existing = await db.Memberships.FirstOrDefaultAsync(m => m.UserId == userId, ct);
        if (existing is not null)
        {
            if (existing.Status == MembershipStatus.Active)
                throw new InvalidOperationException("Membership already active.");

            // Réactivation d'un abonnement annulé/suspendu
            existing.Status = MembershipStatus.Active;
            existing.StartDate = DateTime.UtcNow;
            existing.NextBillingDate = DateTime.UtcNow.AddMonths(1);
            existing.CancelledAt = null;
            // TODO Stripe : reprendre l'abonnement existant (ou en recréer un nouveau)
            await db.SaveChangesAsync(ct);
            return Map(existing);
        }

        // TODO Stripe :
        //   1. Créer ou récupérer un Customer Stripe pour ce user
        //   2. Créer une Subscription Stripe sur le Price "FoodFirstPlus"
        //   3. Renvoyer le clientSecret pour confirmation côté front
        //   4. Stocker StripeSubscriptionId / StripeCustomerId
        // Pour l'instant, on crée directement l'abonnement actif côté plateforme
        // (cohérent avec SubscriptionService existant qui ne câble pas Stripe non plus).
        var membership = new Membership
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = MembershipStatus.Active,
            MonthlyPrice = BusinessRules.FoodFirstPlusMonthlyPrice,
            StartDate = DateTime.UtcNow,
            NextBillingDate = DateTime.UtcNow.AddMonths(1)
        };
        db.Memberships.Add(membership);
        await db.SaveChangesAsync(ct);
        return Map(membership);
    }

    public async Task CancelAsync(Guid userId, CancellationToken ct = default)
    {
        var m = await db.Memberships.FirstOrDefaultAsync(x => x.UserId == userId, ct)
            ?? throw new KeyNotFoundException("Membership not found.");
        if (m.Status == MembershipStatus.Cancelled) return;

        // TODO Stripe : annuler la subscription via StripeSubscriptionId
        m.Status = MembershipStatus.Cancelled;
        m.CancelledAt = DateTime.UtcNow;
        m.NextBillingDate = null;
        await db.SaveChangesAsync(ct);
    }

    private static MembershipDto Map(Membership m) =>
        new(m.Id, m.Status, m.MonthlyPrice, m.StartDate, m.NextBillingDate, m.CancelledAt);
}
