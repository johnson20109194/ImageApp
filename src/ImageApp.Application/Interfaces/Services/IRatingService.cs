using ImageApp.Application.DTOs.Ratings;

namespace ImageApp.Application.Interfaces.Services;

public interface IRatingService
{
    Task RateAsync(Guid photoId, CreateRatingRequest request, CancellationToken ct = default);
    Task<RatingSummaryResponse> GetSummaryAsync(Guid photoId, CancellationToken ct = default);
}