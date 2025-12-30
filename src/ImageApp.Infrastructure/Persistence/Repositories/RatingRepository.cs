using Microsoft.EntityFrameworkCore;
using ImageApp.Application.Interfaces.Repositories;
using ImageApp.Domain.Entities;

namespace ImageApp.Infrastructure.Persistence.Repositories;

public class RatingRepository(AppDbContext db) : IRatingRepository
{
    public async Task UpsertAsync(Guid photoId, Guid userId, int score, CancellationToken ct = default)
    {
        // With unique index on (PhotoId, UserId), this is safe and efficient.
        var existing = await db.Ratings
            .SingleOrDefaultAsync(r => r.PhotoId == photoId && r.UserId == userId, ct);

        if (existing is null)
        {
            await db.Ratings.AddAsync(new Rating
            {
                Id = Guid.NewGuid(),
                PhotoId = photoId,
                UserId = userId,
                Score = score,
                CreatedAt = DateTime.UtcNow
            }, ct);
        }
        else
        {
            existing.Score = score;
            // keep CreatedAt as original "first rated" time, or update a separate UpdatedAt if you add it
        }
    }

    public async Task<(double average, int count)> GetAggregateAsync(Guid photoId, CancellationToken ct = default)
    {
        var query = db.Ratings.AsNoTracking().Where(r => r.PhotoId == photoId);

        var count = await query.CountAsync(ct);
        if (count == 0) return (0, 0);

        var avg = await query.AverageAsync(r => (double)r.Score, ct);
        return (avg, count);
    }

    public async Task<int?> GetUserScoreAsync(Guid photoId, Guid userId, CancellationToken ct = default)
    {
        return await db.Ratings.AsNoTracking()
            .Where(r => r.PhotoId == photoId && r.UserId == userId)
            .Select(r => (int?)r.Score)
            .SingleOrDefaultAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
