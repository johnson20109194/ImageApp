using ImageApp.Domain.Entities;

namespace ImageApp.Application.Interfaces.Repositories;

public interface IPhotoRepository
{
    Task<Photo?> GetAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Photo photo, CancellationToken ct = default);

    Task<(List<Photo>, int total)> SearchAsync(string? search, string? location, int page, int pageSize);

    Task<Photo> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid photoId, CancellationToken ct = default);
    Task UpdateRatingAggregateAsync(Guid photoId, double average, int total, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);


}