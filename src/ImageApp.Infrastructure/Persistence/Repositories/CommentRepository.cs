using Microsoft.EntityFrameworkCore;
using ImageApp.Application.Interfaces.Repositories;
using ImageApp.Domain.Entities;

namespace ImageApp.Infrastructure.Persistence.Repositories;

public class CommentRepository(AppDbContext db) : ICommentRepository
{
    public async Task AddAsync(Comment comment, CancellationToken ct = default)
        => await db.Comments.AddAsync(comment, ct);

    public async Task<IReadOnlyList<Comment>> ListByPhotoIdAsync(Guid photoId, CancellationToken ct = default)
        => await db.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.PhotoId == photoId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}