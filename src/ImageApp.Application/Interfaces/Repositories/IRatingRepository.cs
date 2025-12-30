namespace ImageApp.Application.Interfaces.Repositories;

public interface IRatingRepository
{
    /// <summary>
    /// Upserts the user's score for a photo (insert if new, otherwise update).
    /// </summary>
    Task UpsertAsync(Guid photoId, Guid userId, int score, CancellationToken ct = default);

    /// <summary>
    /// Returns (average, count) for all ratings of the photo.
    /// </summary>
    Task<(double average, int count)> GetAggregateAsync(Guid photoId, CancellationToken ct = default);

    /// <summary>
    /// Returns the current user's score if exists.
    /// </summary>
    Task<int?> GetUserScoreAsync(Guid photoId, Guid userId, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}