using Microsoft.EntityFrameworkCore;
using ImageApp.Application.Interfaces.Repositories;
using ImageApp.Domain.Entities;

namespace ImageApp.Infrastructure.Persistence.Repositories;

public class PhotoRepository(AppDbContext db) : IPhotoRepository
{
    public async Task<Photo?> GetAsync(Guid id, CancellationToken ct = default) =>
        await db.Photos.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(Photo photo, CancellationToken ct = default) =>
        await db.Photos.AddAsync(photo, ct);

    public async Task<(List<Photo>, int total)> SearchAsync(string? search, string? location, int page, int pageSize)
    {
        var q = db.Photos.AsNoTracking().Where(p => p.IsPublic);

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(p => p.Title.Contains(search) || (p.Caption ?? "").Contains(search));

        if (!string.IsNullOrWhiteSpace(location))
            q = q.Where(p => p.Location == location);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Photo> GetByIdAsync(Guid id)
    {
        var p = await db.Photos.AsNoTracking()
            // .Include(p => p.Comments)
            //.Include(p => p.Ratings)
            .SingleAsync(x => x.Id == id && x.IsPublic);
        return p;
    }

    public Task<bool> ExistsAsync(Guid photoId, CancellationToken ct = default)
        => db.Photos.AnyAsync(p => p.Id == photoId && p.IsPublic, ct);

    public async Task UpdateRatingAggregateAsync(Guid photoId, double average, int total, CancellationToken ct = default)
    {
        var photo = await db.Photos.SingleOrDefaultAsync(p => p.Id == photoId, ct)
                    ?? throw new InvalidOperationException("Photo not found.");

        photo.AverageRating = average;
        photo.TotalRatings = total;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}