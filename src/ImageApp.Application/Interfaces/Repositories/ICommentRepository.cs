using ImageApp.Domain.Entities;

namespace ImageApp.Application.Interfaces.Repositories;

public interface ICommentRepository
{
    Task AddAsync(Comment comment, CancellationToken ct = default);
    Task<IReadOnlyList<Comment>> ListByPhotoIdAsync(Guid photoId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}