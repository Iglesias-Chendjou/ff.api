using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Repository.Implementations;

public class UserRepository(AppDbContext db) : Repository<User>(db), IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        Set.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        Set.AnyAsync(u => u.Email == email, ct);
}
