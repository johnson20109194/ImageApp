using ImageApp.Application.DTOs.Ratings;
using ImageApp.Application.Interfaces;
using ImageApp.Application.Interfaces.Repositories;
using ImageApp.Application.Interfaces.Services;

namespace ImageApp.Application.Services;

public class RatingService(
    IRatingRepository ratings,
    IPhotoRepository photos,
    ICurrentUser currentUser,
    IPhotoService photoService)
    : IRatingService
{
    public async Task RateAsync(Guid photoId, CreateRatingRequest request, CancellationToken ct = default)
    {
        if (!currentUser.IsAuthenticated)
            throw new InvalidOperationException("Not authenticated.");

        var score = request.Score;
        if (score is < 1 or > 5)
            throw new InvalidOperationException("Score must be between 1 and 5.");

        if (!await photos.ExistsAsync(photoId, ct))
            throw new InvalidOperationException("Photo not found.");

        // Upsert the user's rating
        await ratings.UpsertAsync(photoId, currentUser.UserId, score, ct);
        await ratings.SaveChangesAsync(ct);

        // Recompute aggregates and store denormalized values on Photo
        var (avg, count) = await ratings.GetAggregateAsync(photoId, ct);
        await photos.UpdateRatingAggregateAsync(photoId, avg, count, ct);
        await photos.SaveChangesAsync(ct);

        await photoService.UpdateCacheCacheAsync(photoId, ct);
    }

    public async Task<RatingSummaryResponse> GetSummaryAsync(Guid photoId, CancellationToken ct = default)
    {
        if (!await photos.ExistsAsync(photoId, ct))
            throw new InvalidOperationException("Photo not found.");

        var (avg, count) = await ratings.GetAggregateAsync(photoId, ct);

        int? userScore = null;
        if (currentUser.IsAuthenticated && currentUser.UserId != Guid.Empty)
            userScore = await ratings.GetUserScoreAsync(photoId, currentUser.UserId, ct);

        return new RatingSummaryResponse(photoId, avg, count, userScore);
    }
}
