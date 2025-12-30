using Microsoft.EntityFrameworkCore;
using ImageApp.Application.Interfaces.Repositories;
using ImageApp.Domain.Entities;

namespace ImageApp.Infrastructure.Persistence.Repositories
{
    public class UserRepository(AppDbContext db) : IUserRepository
    {
        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            db.Users.SingleOrDefaultAsync(x => x.Id == id, ct);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
            await db.Users.SingleOrDefaultAsync(x => x.Email == email, ct);

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
            await db.Users.AnyAsync(x => x.Email == email, ct);

        public async Task AddAsync(User user, CancellationToken ct = default) =>
            await db.Users.AddAsync(user, ct);

        public async Task SaveChangesAsync(CancellationToken ct = default) =>
            await db.SaveChangesAsync(ct);

        public async Task<IReadOnlyList<User>> SearchAsync(string query, int take, CancellationToken ct)
        {
            query = (query ?? "").Trim();
            if (query.Length == 0) return Array.Empty<User>();

            return await db.Users.AsNoTracking()
                .Where(u => u.Email.Contains(query) || u.DisplayName.Contains(query))
                .OrderBy(u => u.Email)
                .Take(Math.Clamp(take, 1, 50))
                .ToListAsync(ct);
        }
    }
}