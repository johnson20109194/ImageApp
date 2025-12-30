namespace ImageApp.Application.DTOs.Ratings;

public class RatingSummaryResponse(Guid photoId, double averageRating, int totalRatings, int? currentUserScore)
{
    public Guid PhotoId { get; } = photoId;
    public double AverageRating { get; } = averageRating;
    public int TotalRatings { get; } = totalRatings;
    public int? CurrentUserScore { get; } = currentUserScore;
}